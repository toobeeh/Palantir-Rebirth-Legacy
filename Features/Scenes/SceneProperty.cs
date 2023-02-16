using Palantir_Rebirth.Data.SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Palantir_Rebirth.Features.Scenes
{
    internal class SceneProperty : SceneEntity
    {
        public bool Activated { get; set; }
        public SceneProperty(SceneEntity entity, bool activated)
        {
            Activated = activated;
            URL = entity.URL;
            Artist = entity.Artist;
            ID = entity.ID;
            Name = entity.Name;
            Color= entity.Color;
            GuessedColor = entity.GuessedColor;
            EventID= entity.EventID;
            Exclusive= entity.Exclusive;
        }
    }
}
