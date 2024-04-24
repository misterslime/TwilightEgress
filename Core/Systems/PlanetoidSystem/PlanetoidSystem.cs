using Cascade.Core.Players;

namespace Cascade.Core.Systems.PlanetoidSystem
{
    public class PlanetoidSystem : ModSystem
    {
        public static Dictionary<Type, Planetoid> planetoidsByType = new();
        public PlanetoidSystem() { }

        public override void Load()
        {
            On_Player.DryCollision += UpdateVelocityNearPlanetoidEntities;
            base.Load();
        }

        public override void Unload()
        {
            On_Player.DryCollision -= UpdateVelocityNearPlanetoidEntities;
            base.Unload();
        }

        private void UpdateVelocityNearPlanetoidEntities(On_Player.orig_DryCollision orig, Player self, bool fallThrough, bool ignorePlats)
        {
            // Setting the player's velocity to 0 here is what allows the player to jump out of planetoids, as well as giving them their running visuals.
            OrbitalGravityPlayer player = self.GetModPlayer<OrbitalGravityPlayer>();
            if (player.Planetoid is not null && player.Planetoid.NPC.active)
                self.velocity.Y = 0f;

            orig.Invoke(self, fallThrough, ignorePlats);
        }
    }

    public class PlanetoidPlayer : ModPlayer
    {
        public Planetoid planetoid = null;
        public float angle = 0;

        public PlanetoidPlayer() { }
    }
}