using Quartz.Impl.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Palantir_Rebirth.Features.User.Exceptions
{
    [Serializable]
    internal abstract class MemberException : Exception
    {
        public PalantirMember Member { get; }
        public MemberException(PalantirMember member, string msg) : base(msg)
        {
            this.Member = member;
        }
    }
}
