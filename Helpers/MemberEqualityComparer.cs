using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CheckinPPP.Data.Entities;
using CheckinPPP.Models;

namespace CheckinPPP.Helpers
{
    public class MemberEqualityComparer : IEqualityComparer<ApplicationUser>
    {
        public bool Equals([AllowNull] ApplicationUser x, [AllowNull] ApplicationUser y)
        {
            return (x.Name.ToLower() == y.Name.ToLower())
                && (x.Surname.ToLower() == y.Surname.ToLower()
                && (x.Email == y.Email));
        }

        public int GetHashCode([DisallowNull] ApplicationUser obj)
        {
            throw new NotImplementedException();
        }

    }

    public class DistinctEqualityComparer : IEqualityComparer<Member>
    {
        public bool Equals([AllowNull] Member x, [AllowNull] Member y)
        {
            return (x.Id == y.Id);
        }

        public int GetHashCode([DisallowNull] Member obj)
        {
            throw new NotImplementedException();
        }
    }
}
