using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cascade.Core.Systems.PlanetoidSystem
{
    public class PlanetoidPlayer : ModPlayer
    {
        public Planetoid planetoid = null;
        public float angle = 0;

        public PlanetoidPlayer() { }
    }
}
