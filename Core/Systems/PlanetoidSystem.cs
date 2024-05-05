using CalamityMod.Dusts;
using Cascade.Common.VerletIntegration;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;

namespace Cascade.Core.Systems
{
    public class PlanetoidSystem : ModSystem
    {
        public static VerletObject[] verlets;

        public override void Load()
        {
            On_Main.DrawDust += DrawVerlets;
            verlets = new VerletObject[200];
        }

        public override void Unload()
        {
            On_Main.DrawDust += DrawVerlets;
            verlets = null;
        }

        public override void PreUpdatePlayers()
        {
            /*bool createVerlet = (int)(Main.GlobalTimeWrappedHourly * 60) % 60 == 0;
            Player player = Main.player[Main.myPlayer];

            //Main.NewText(createVerlet);

            //verlets = new VerletObject[200];

            if (createVerlet)
            {
                for (int i = 0; i < 200; i++)
                {
                    if (verlets[i] is null)
                    {
                        verlets[i] = new VerletObject(player.Center - Main.rand.NextVector2Circular(1f, 1f) * 300f, Main.rand.NextVector2Circular(1f, 1f), 47f, 0f);
                        break;
                    }
                }
            }

            /*foreach (VerletObject? verlet in verlets)
            {
                if (verlet is not null)
                {
                    Vector2 collisionAxis = player.Center - verlet.Position;
                    float distance = collisionAxis.Length();

                    if (distance >= 150f)
                        continue;

                    float mass1 = verlet.Radius * verlet.Radius * player.gravity;
                    //float mass2 = verlet2.Radius * verlet2.Radius * gravity;

                    // F = G * M1 * M2 / d^2
                    // ma = G * M1 * M2 / d^2
                    // a = (G * M1 * M2) / (d^2 * m)

                    float acceleration = 30 * (mass1 * mass1) / (distance * distance * mass1);

                    Vector2 normal = collisionAxis / distance;

                    player.velocity -= acceleration * normal;
                }
            }*/

            verlets.UpdateVerlets(1);
        }

        private void DrawVerlets(On_Main.orig_DrawDust orig, Main self)
        {
            orig(self);

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D galileoTexture = ModContent.Request<Texture2D>("Cascade/Content/NPCs/CosmostoneShowers/Planetoids/GalileoPlanetoid").Value;

            foreach (VerletObject? verlet in verlets)
            {
                if (verlet is not null)
                {
                    Main.spriteBatch.Draw(galileoTexture, verlet.Position - Main.screenPosition, galileoTexture.Frame(), Color.White, 0f, galileoTexture.Frame().Size() * 0.5f, 1f, 0, 0f);
                }
            }

            Main.spriteBatch.End();
        }
    }
}
