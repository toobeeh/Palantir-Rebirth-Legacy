using Palantir_Rebirth.Data.SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Palantir_Rebirth.Features.Sprites
{
    internal class SpriteProperty : SpritesEntity
    {
        public int Slot { get; set; }
        public SpriteProperty(SpritesEntity entity, int slot) {
            Slot = slot;
            URL= entity.URL;
            Cost= entity.Cost;
            Special= entity.Special;
            Rainbow= entity.Rainbow;
            EventDropID= entity.EventDropID;
            Artist= entity.Artist;
            ID= entity.ID;
            Name= entity.Name;
        }
    }
}
