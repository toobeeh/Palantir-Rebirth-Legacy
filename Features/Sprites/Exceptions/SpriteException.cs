using Palantir_Rebirth.Data.SQLite;
using Palantir_Rebirth.Features.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Palantir_Rebirth.Features.Sprites.Exceptions
{
    [Serializable]
    internal abstract class SpriteException : Exception
    {
        public int SpriteId { get; }
        public SpritesEntity? Sprite { get; protected set; }
        public SpriteException(int spriteId, string message) : base(message)
        {
            this.SpriteId = spriteId;
        }
    }
}
