using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cascade.Content.NPCs.CosmostoneShowers.Asteroids
{
    internal class AsteroidUtil
    {
        internal static List<int> ViableCollisionTypes = new List<int>()
        {
            ModContent.NPCType<CosmostoneAsteroidSmall>(),
            ModContent.NPCType<CosmostoneAsteroidMedium>(),
            ModContent.NPCType<CosmostoneAsteroidLarge>(),
            ModContent.NPCType<CometstoneAsteroidSmall>(),
            ModContent.NPCType<CometstoneAsteroidMedium>(),
            ModContent.NPCType<CometstoneAsteroidLarge>(),
        };
    }
}
