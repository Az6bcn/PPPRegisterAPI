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
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
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


            // Automatically perform database migration (Azure)
            //services.BuildServiceProvider().GetService<ApplicationDbContext>().Database.Migrate();
            MigrateDatabase(services);

            SeedServiceDatas(services);

            // load email settings so it can be made available via DI
            services.AddOptions<MailGunApiEmailSettings>(); services.Configure<MailGunApiEmailSettings>(Configuration.GetSection("MailGunApiEmailSettings"));

            services.AddTransient<IBookingBusiness, BookingBusiness>();
            services.AddTransient<IBookingQueries, BookingQueries>();
            services.AddTransient<ISendEmails, SendEmails>();

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

            app.UseAuthorization();

            // set default homepage to index.html of the compiled Angular app
            app.UseDefaultFiles();

            //serve static file: to serve built Angular app in wwwwrooot
            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapFallbackToController("index", "Fallback");
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

                    logger.LogInformation("Nothing to Seed");
                }
            }
        }
    }
}
