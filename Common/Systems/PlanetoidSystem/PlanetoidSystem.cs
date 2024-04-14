namespace Cascade.Common.Systems.PlanetoidSystem {
    public class PlanetoidSystem : ModSystem {

        public Dictionary<Type, Planetoid> planetoidsByType = new();
        public PlanetoidSystem() { }

        public override void Load() {
            On_Player.DryCollision += (orig, self, fallThrough, ignorePlats) => {
                
            };
            base.Load();
        }
    }

    public class PlanetoidPlayer : ModPlayer {

        public Planetoid? planetoid = null;
        public float angle = 0;

        public PlanetoidPlayer() { }
    }
}
