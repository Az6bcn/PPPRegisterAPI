using CheckinPPP.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CheckinPPP.Data;
using Microsoft.Extensions.Logging;
using System.Linq;
using CheckinPPP.Helpers;
using CheckinPPP.Data.Entities;
using CheckinPPP.Business;
using CheckinPPP.Data.Queries;
using CheckinPPP.Models;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Options;
using System;

namespace CheckinPPP
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var key = Encoding.ASCII.GetBytes(Configuration.GetSection("Jwt:Key").Value);


            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 1;

                options.User.RequireUniqueEmail = false;

            });

            var clientUrl = Configuration.GetSection("ClientUrl").Value;

            services.AddCors(options =>
            {
                options.AddPolicy("myPolicy", builder =>
               {
                   builder.WithOrigins(clientUrl);
                   builder.AllowAnyMethod();
                   builder.AllowAnyHeader();
                   builder.AllowCredentials(); // for signalR
               });
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = true,
                        ValidAudience = Configuration.GetSection("Jwt:Audience").Value,
                        ValidateIssuer = true,
                        ValidIssuer = Configuration.GetSection("Jwt:Issuer").Value,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateLifetime = true,
                        RequireExpirationTime = true
                    };
                });

            //services.AddAuthorization();

            // Automatically perform database migration (Azure)
            //services.BuildServiceProvider().GetService<ApplicationDbContext>().Database.Migrate();
            MigrateDatabase(services);

            SeedServiceDatas(services);

            // load email settings so it can be made available via DI
            services.AddOptions<MailGunApiEmailSettings>();
            services.Configure<MailGunApiEmailSettings>(Configuration.GetSection("MailGunApiEmailSettings"));

            services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));
            services.Configure<JwtOptions>(Configuration.GetSection("Jwt"));

            services.AddTransient<IBookingBusiness, BookingBusiness>();
            services.AddTransient<IBookingQueries, BookingQueries>();
            services.AddTransient<ISendEmails, SendEmails>();
            services.AddTransient<IGoogleMailService, GoogleMailService>();
            services.AddTransient<IAccountBusiness, AccountBusiness>();
            services.AddTransient<IJwtFactory, JwtFactory>();

            services.AddSignalR(hubOptions =>
            {
                hubOptions.EnableDetailedErrors = true;
            });

            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    // all dates must be in ISODate format
                    options.SerializerSettings.DateFormatString = "yyyy-MM-ddTHH:mm:ssZ";
                });

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressInferBindingSourcesForParameters = true;
                options.SuppressMapClientErrors = true;
                options.SuppressConsumesConstraintForFormFileParameters = true;
                options.SuppressModelStateInvalidFilter = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("myPolicy");
            app.UseAuthentication();
            app.UseAuthorization();

            // set default homepage to index.html of the compiled Angular app
            app.UseDefaultFiles();

            //serve static file: to serve built Angular app in wwwwrooot
            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapFallbackToController("index", "Fallback");
                endpoints.MapHub<PreciousPeopleHub>("/ppphub");
                endpoints.MapControllers();
            });
        }


        private void MigrateDatabase(IServiceCollection services)
        {
            // build the serviceCollection, returns IServiceProvider, which is used to resolve services
            using (var serviceProvider = services.BuildServiceProvider())
            {
                // create a scope where all my operations will run in.
                using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    // resolve the dependencies I need
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Startup>>();

                    logger.LogInformation("Getting Pending Migrations");
                    var pendingMigrations = context.Database.GetPendingMigrations();

                    if (pendingMigrations.Any())
                    {
                        logger.LogInformation("Ready to apply migration to  Database");
                        context.Database.Migrate();
                        logger.LogInformation("Migrated Database");
                    }
                    logger.LogInformation("No Pending Migrations");
                }
            }
        }

        private void SeedServiceDatas(IServiceCollection services)
        {
            // build the serviceCollection, returns IServiceProvider, which is used to resolve services
            using (var serviceProvider = services.BuildServiceProvider())
            {
                // create a scope where all my operations will run in.
                using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    // resolve the dependencies I need
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Startup>>();

                    if (context.Database.GetService<IRelationalDatabaseCreator>().Exists())
                    {
                        var bookings = context.Set<Booking>()
                            .Select(x => x.Id)
                            .ToList();

                        logger.LogInformation("Prepairing to seed data .....");

                        if (bookings.Count() == 0)
                        {
                            context.AddRange(SeedFirstService.FirstServiceBookingData());
                            context.AddRange(SeedSecondService.SeedSecondServiceBookingData());
                            context.AddRange(SeedWorkersService.SeedWorkersServiceBookingData());

                            context.SaveChanges();
                            logger.LogInformation("Seeded");
                        }

                        // seed special service:
                        var seedSpecialService = Configuration.GetSection("SpecialServices")["SeedSpecialServices"];
                        var shouldSeedSpecialService = Convert.ToBoolean(seedSpecialService);

                        if (shouldSeedSpecialService)
                        {
                            logger.LogInformation("Prepairing to seed special service data .....");
                            var dataToSeed = SeedSpecialService.SeedSingleSpecialService();

                            if (dataToSeed != null)
                            {
                                var alreadyExists = context.Set<Booking>()
                                .Any(x => x.Date.Date == dataToSeed.Date.Date);

                                if (!alreadyExists)
                                {
                                    context.Add(dataToSeed);
                                    context.SaveChanges();
                                    logger.LogInformation("Special service data seeded");
                                }
                            }

                            var datasToSeed = SeedSpecialService.SeedSpecialServices();
                            if (datasToSeed.Any())
                            {
                                var first = datasToSeed.First();

                                var alreadyExists = context.Set<Booking>()
                                .Any(x => x.Date.Date == first.Date.Date);

                                if (!alreadyExists)
                                {
                                    context.AddRange(datasToSeed);
                                    context.SaveChanges();
                                    logger.LogInformation("Special services data seeded");
                                }
                            }
                        }
                    }
                    logger.LogInformation("Nothing to Seed");
                }
            }
        }
    }
}
