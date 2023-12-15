using Cascade.Core.Players;

namespace Cascade.Core.Systems
{
    public class OrbitalGravitySystem : ModSystem
    {
        public override void OnModLoad() => On_Player.DryCollision += UpdateVelocityNearPlanetoidEntities;
        public override void OnModUnload() => On_Player.DryCollision -= UpdateVelocityNearPlanetoidEntities;

        private void UpdateVelocityNearPlanetoidEntities(On_Player.orig_DryCollision orig, Player self, bool fallThrough, bool ignorePlats)
        {
            // Setting the player's velocity to 0 here is what allows the player to jump out of planetoids, as well as giving them their running visuals.
            OrbitalGravityPlayer player = self.GetModPlayer<OrbitalGravityPlayer>();
            if (player.Planetoid is not null && player.Planetoid.NPC.active)
                self.velocity.Y = 0f;

            orig.Invoke(self, fallThrough, ignorePlats);
        }
    }
}
