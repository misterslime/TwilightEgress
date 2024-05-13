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
            On_Main.DrawDust += DrawVerlets;
            planetoids = new MassiveObject[200];
        }

        public override void Unload()
        {
            On_Main.DrawDust -= DrawVerlets;
            planetoids = null;
        }

        public override void PreUpdatePlayers()
        {
            /*bool createVerlet = (int)(Main.GlobalTimeWrappedHourly * 60) % 60 == 0;
            Player player = Main.player[Main.myPlayer];

            //Main.NewText(createVerlet);

            //planetoids = new MassiveObject[200];

            if (createVerlet)
            {
                for (int i = 0; i < 200; i++)
                {
                    if (planetoids[i] is not null) continue;

                    planetoids[i] = new MassiveObject(player.Center - Main.rand.NextVector2Circular(1f, 1f) * 300f, Main.rand.NextVector2Circular(0.2f, 0.2f), 47f, 0f, 100f);
                    break;
                }
            }*/

            //player.velocity += planetoids.GetGravityAtPosition(player.Center, 1f);

            planetoids.UpdateVerlets(1);
            planetoids.PushObjects(1);
        }

        private void DrawVerlets(On_Main.orig_DrawDust orig, Main self)
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
