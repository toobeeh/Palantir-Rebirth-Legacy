using Palantir_Rebirth.Data.SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Palantir_Rebirth.Features.User.Exceptions
{

    internal class MemberSpriteException : MemberException
    {
        public static readonly string ALREADY_IN_INV = "Sprite already in inventory";
        public static readonly string CREDIT_TOO_LOW = "Credit is too low";
        public static readonly string PATRON_SPRITE = "Sprite can only be bought by patrons";
        public SpritesEntity Sprite { get; }
        public MemberSpriteException(PalantirMember member, SpritesEntity sprite, string msg) : base(member, msg)
        {
            this.Sprite = sprite;
        }
    }
}
