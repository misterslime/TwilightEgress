using Cascade.Core.Configs;
using Terraria.Graphics.CameraModifiers;

namespace Cascade.Core.Systems.CameraSystem
{
    public class CascadeCameraSystem : ModSystem
    {
        private static Vector2 ShakePosition = Main.LocalPlayer.Center;

        private static int Shake;

        private static int ShakeLifespan;

        private static int ShakeTime;

        private static bool ShakeIsActive;

        private const int MaximumShakePower = 120;

        /// <summary>
        /// Shakes the screen at the specified location.
        /// </summary>
        /// <param name="position">The center position of where the screenshake should originate from.</param>
        /// <param name="shakePower">The power of the screenshake.</param>
        /// <param name="lifespan">How long the shake will last before it begins to rapidly decay.</param>
        public static void Screenshake(int shakePower, int lifespan, Vector2? position = null)
        {
            Shake = shakePower;
            ShakeLifespan = lifespan;
            ShakePosition = position ?? Main.LocalPlayer.Center;
            ShakeIsActive = true;
        }

        public override void PostUpdateEverything()
        {
            // Update nothing if a player has Calamity's screenshake configuration disabled.
            if (!CalamityConfig.Instance.Screenshake)
                return;

            // Clamp the shake power to ensure that thing's don't get too crazy.
            Shake = Utils.Clamp(Shake, Shake, MaximumShakePower);

            if (ShakeIsActive)
            {
                ShakeTime++;
                // Slowly decay over time.
                Shake = (int)Lerp(Shake, 0, ShakeTime / ShakeLifespan);
                if (ShakeTime >= ShakeLifespan)
                {
                    // Reset variables once the shake has fully decayed.
                    ShakeIsActive = false;
                    Shake = 0;
                    ShakeTime = 0;
                }
            }
        }

        public override void ModifyScreenPosition()
        {
            if (ShakeTime > 0)
            {
                //float multiplier = GraphicsConfig.Instance.ScreenshakeMult;
                Main.instance.CameraModifiers.Add(new PunchCameraModifier(ShakePosition, Main.rand.NextFloat(3.14f).ToRotationVector2(), Shake, 15f, ShakeLifespan, 2000, "Cascade Custom Screenshake"));
            }
        }

        public void ResetModifiers()
        {
            Shake = 0;
            ShakeLifespan = 0;
            ShakeTime = 0;
            ShakeIsActive = false;
        }

        public override void OnWorldLoad()
        {
            ResetModifiers();
        }
    }
}
