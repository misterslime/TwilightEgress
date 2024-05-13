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
                if (planetoids[i] is not null) continue;

                planetoids[i] = new MassiveObject(position, velocity, radius, rotationSpeed, mass);
                break;
            }
        }

        private void DrawPlanetoids(On_Main.orig_DrawDust orig, Main self)
        {
            orig(self);

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D galileoTexture = ModContent.Request<Texture2D>("Cascade/Content/NPCs/CosmostoneShowers/Planetoids/GalileoPlanetoid").Value;

            foreach (VerletObject? verlet in planetoids)
            {
                if (verlet is not null)
                    Main.spriteBatch.Draw(galileoTexture, verlet.Position - Main.screenPosition, galileoTexture.Frame(), Color.White, 0f, galileoTexture.Frame().Size() * 0.5f, 1f, 0, 0f);
            }

            Main.spriteBatch.End();
        }
    }
}
