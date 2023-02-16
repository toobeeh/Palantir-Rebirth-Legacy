using Palantir_Rebirth.Features.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Palantir_Rebirth.Features.Scenes
{
    internal class SceneManager
    {
        public static readonly int SceneStartPrice = 20000;
        public static readonly int ScenePriceFactor = 2;

        private readonly PalantirMember member;
        public SceneManager(PalantirMember member)
        {
            this.member = member;
        }

        public int GetCurrentScenePrice()
        {
            int price = SceneStartPrice;
            foreach(var scene in member.Scenes)
            {
                price *= ScenePriceFactor;
            }
            return price;
        }

        public int GetSceneInvWorth()
        {
            int worth = 0;
            foreach (var scene in member.Scenes)
            {
                worth += worth == 0 ? SceneStartPrice : worth * ScenePriceFactor;
            }
            return worth;
        }
    }
}
