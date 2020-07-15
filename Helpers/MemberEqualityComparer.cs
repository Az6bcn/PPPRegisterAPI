using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CheckinPPP.Data.Entities;

namespace CheckinPPP.Helpers
{
    public class MemberEqualityComparer : IEqualityComparer<Member>
    {
        public bool Equals([AllowNull] Member x, [AllowNull] Member y)
        {
            return (x.Name.ToLower() == y.Name.ToLower()) && (x.Surname.ToLower() == y.Surname.ToLower());
        }

        public int GetHashCode([DisallowNull] Member obj)
        {
            throw new NotImplementedException();
        }

    }

    public class NotMemberEqualityComparer : IEqualityComparer<Member>
    {
        public bool Equals([AllowNull] Member x, [AllowNull] Member y)
        {
            return (x.Name.ToLower() != y.Name.ToLower()) && (x.Surname.ToLower() != y.Surname.ToLower());
        }

        public int GetHashCode([DisallowNull] Member obj)
        {
            throw new NotImplementedException();
        }
    }
}
