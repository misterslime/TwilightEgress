using CalamityMod.Rarities;
using Cascade.Common.Physics.Gravity;
using Cascade.Common.Physics.VerletIntegration;
using Terraria;

namespace Cascade.Core.Systems
{
    public class PlanetoidSystem : ModSystem
    {
        public static MassiveObject[] planetoids;

        public override void Load()
        {
            On_Main.DrawDust += DrawPlanetoids;
            planetoids = new MassiveObject[200];
        }

        public override void Unload()
        {
            On_Main.DrawDust -= DrawPlanetoids;
            planetoids = null;
        }

        public override void PreUpdatePlayers()
        {
            planetoids.UpdateVerlets(1);
            planetoids.ApplyGravity(1);
        }

        public static void NewPlanetoid(Vector2 position, Vector2 velocity, float radius, float rotationSpeed, float mass)
        {
            for (int i = 0; i < 200; i++)
            {
                if (planetoids[i] is not null && planetoids[i].Active) continue;

                planetoids[i] = new MassiveObject(position, velocity, radius, rotationSpeed, mass);
                planetoids[i].WhoAmI = i;
                break;
            }
        }

        private void DrawPlanetoids(On_Main.orig_DrawDust orig, Main self)
        {
            orig(self);

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D galileoTexture = ModContent.Request<Texture2D>("Cascade/Content/NPCs/CosmostoneShowers/Planetoids/GalileoPlanetoid").Value;

            foreach (MassiveObject? planetoid in planetoids)
            {
                if (planetoid is not null && planetoid.Active)
                    Main.spriteBatch.Draw(galileoTexture, planetoid.Position - Main.screenPosition, galileoTexture.Frame(), Color.White, 0f, galileoTexture.Frame().Size() * 0.5f, planetoid.Radius / 47f, 0, 0f);
            }

            Main.spriteBatch.End();
        }
    }
}
