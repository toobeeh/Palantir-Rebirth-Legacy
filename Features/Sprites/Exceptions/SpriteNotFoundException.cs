using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Palantir_Rebirth.Features.User;
using Palantir_Rebirth.Features.User.Exceptions;

namespace Palantir_Rebirth.Features.Sprites.Exceptions
{
    internal class SpriteNotFoundException : SpriteException
    {
        public SpriteNotFoundException(int spriteId) : base(spriteId)
        {
        }
    }
}
