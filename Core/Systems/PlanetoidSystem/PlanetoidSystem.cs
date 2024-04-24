using Cascade.Core.Players;

namespace Cascade.Core.Systems.PlanetoidSystem
{
    public class PlanetoidSystem : ModSystem
    {
        public static Dictionary<Type, Planetoid> PlanetoidsByType = new();
        public static Asset<Texture2D>[] PlanetoidTextures = [];
        public static Planetoid[] planetoids;

        public static void AddTexture(Asset<Texture2D> texture)
        {
            Array.Resize(ref PlanetoidTextures, PlanetoidTextures.Length + 1);
            PlanetoidTextures[PlanetoidTextures.Length - 1] = texture;
        }

        public override void Load()
        {
            On_Player.DryCollision += UpdateVelocityNearPlanetoidEntities;
            planetoids = new Planetoid[100];
            base.Load();
        }

        public override void Unload()
        {
            On_Player.DryCollision -= UpdateVelocityNearPlanetoidEntities;
            PlanetoidTextures = null;
            PlanetoidsByType = null;
            planetoids = null;
            base.Unload();
        }

        public override void PreUpdateNPCs()
        {
            base.PreUpdateNPCs();
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
}