using Microsoft.CodeAnalysis.CSharp;
using System.Text.Encodings.Web;

namespace Cascade.Core.Graphics.SpecificEffectManagers
{
    public class SpecialScreenEffectSystem : ModSystem
    {
        #region Chromatic Abberation

        public SmartRenderTarget ChromaticAbberationTarget
        {
            get;
            private set;
        }

        private static int ChromaTime;

        private static int ChromaLifespan;

        private static float ChromaStrength;

        private static Vector2 ChromaPosition;

        private static bool ChromaIsActive;

        private static float ChromaLifespanRatio => ChromaTime / (float)ChromaLifespan;

        #endregion

        #region Dark Vignette

        public SmartRenderTarget DarkVignetteTarget
        {
            get;
            private set;
        }

        private static int VignetteTime;

        private static int VignetteLifespan;

        private static float VignettePower;

        private static float VignetteBrightness;

        private static Vector2 VignettePosition;

        private static bool VignetteIsActive;

        private static float VignetteLifespanRatio => VignetteTime / (float)VignetteLifespan;

        private static bool CanDrawVignette => VignettePower > 0 || VignetteBrightness > 0;

        #endregion

        public override void OnModLoad()
        {
            On_FilterManager.EndCapture += DrawScreenEffects;
            Main.QueueMainThreadAction(() =>
            {
                if (Main.netMode == NetmodeID.Server)
                    return;

                DarkVignetteTarget = new(SmartTargetManager.CreateCustomRenderTarget, true);
                ChromaticAbberationTarget = new(SmartTargetManager.CreateCustomRenderTarget, true);
            });
        }

        public override void OnModUnload()
        {
            Main.QueueMainThreadAction(() =>
            {
                DarkVignetteTarget?.Dispose();
                ChromaticAbberationTarget?.Dispose();
            });
        }

        // Reset all variables.
        public override void OnWorldLoad() => ResetAllScreenEffectVariables();

        public override void OnWorldUnload() => ResetAllScreenEffectVariables();

        public override void PostUpdateEverything()
        {
            // Dark vignette.
            if (VignetteIsActive)
            {
                VignetteTime++;
                if (VignetteTime >= VignetteLifespan)
                {
                    VignetteTime = 0;
                    VignetteIsActive = false;
                }
            }
            else
            {
                if (VignetteBrightness > 0)
                {
                    VignettePower = Clamp(VignettePower + 0.03f, 0f, 20f);
                    VignetteBrightness = Clamp(VignetteBrightness - 0.01f, 0f, 50f);
                }
                else
                {
                    VignetteBrightness = 0;
                    VignettePower = 0;
                    VignettePosition = Vector2.Zero;
                }
            }

            // Chromatic abberation.
            if (ChromaIsActive)
            {
                ChromaTime++;
                if (ChromaTime >= ChromaLifespan)
                {
                    ChromaTime = 0;
                    ChromaStrength = 0f;
                    ChromaPosition = Vector2.Zero;
                    ChromaIsActive = false;
                }
            }
        }

        public static void ApplyDarkVignette(Vector2 vignettePosition, float vignettePower, float vignetteBrightness, int vignetteLifespan)
        {
            VignettePosition = vignettePosition;
            VignettePower = vignettePower;
            VignetteBrightness = vignetteBrightness;
            VignetteLifespan = vignetteLifespan;

            VignetteTime = 0;
            VignetteIsActive = true;
        }

        public static void ApplyChromaticAbberation(Vector2 chromaPosition, float chromaStrength, int chromaLifespan)
        {
            ChromaPosition = chromaPosition;
            ChromaStrength = chromaStrength;
            ChromaLifespan = chromaLifespan;

            ChromaTime = 0;
            ChromaIsActive = true;
        }

        private void ResetAllScreenEffectVariables()
        {
            VignettePosition = Vector2.Zero;
            VignettePower = 0f;
            VignetteBrightness = 0f;
            VignetteTime = 0;
            VignetteLifespan = 0;
            VignetteIsActive = false;

            ChromaPosition = Vector2.Zero;
            ChromaTime = 0;
            ChromaStrength = 0f;
            ChromaLifespan = 0;
            ChromaIsActive = false;
        }

        private void DrawScreenEffects(On_FilterManager.orig_EndCapture orig, FilterManager self, RenderTarget2D finalTexture, RenderTarget2D screenTarget1, RenderTarget2D screenTarget2, Color clearColor)
        {
            if (CanDrawVignette)
            {
                // Draw the screen contents to our Render Target.
                DarkVignetteTarget.SwapToTarget();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                Main.spriteBatch.Draw(screenTarget1, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                Main.spriteBatch.End();

                // Swap back to the screen target and then redraw our target with the applied shader.
                screenTarget1.SwapToTarget();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

                var shader = Utilities.TryGetScreenShader("EllipticalVignetteShader");
                shader.TrySetParameterValue("vignettePower", VignettePower);
                shader.TrySetParameterValue("vignetteBrightness", VignetteBrightness);
                shader.Apply();

                // This will extend the effect an extra 500 tiles over the screen.
                // The effect won't fit on the screen properly without this.
                Rectangle screenFit = new(-500, -500, Main.screenWidth + 600, Main.screenHeight + 600);

                Vector2 drawPosition = VignettePosition - Main.screenPosition;
                Vector2 origin = VignettePosition + new Vector2(500f) - Main.screenPosition;
                Main.spriteBatch.Draw(DarkVignetteTarget.RenderTarget, drawPosition, screenFit, Color.White, 0f, origin, 1f, SpriteEffects.None, 0f);
                Main.spriteBatch.End();
            }

            if (ChromaIsActive)
            {
                // Draw the screen contents to our Render Target.
                ChromaticAbberationTarget.SwapToTarget();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                Main.spriteBatch.Draw(screenTarget1, Vector2.Zero, Color.White);
                Main.spriteBatch.End();

                // Swap back to the screen target and then redraw our target with the applied shader.
                screenTarget1.SwapToTarget();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

                var shader = Utilities.TryGetScreenShader("ChromaticAbberationShader");
                shader.TrySetParameterValue("distortionAmount", (1f - ChromaLifespanRatio) * ChromaStrength);
                shader.TrySetParameterValue("impactPosition", ChromaPosition - Main.screenPosition);
                shader.Apply();

                Main.spriteBatch.Draw(ChromaticAbberationTarget.RenderTarget, Vector2.Zero, Color.White);
                Main.spriteBatch.End();
            }

            orig(self, finalTexture, screenTarget1, screenTarget2, clearColor);
        }
    }
}
