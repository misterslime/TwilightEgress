using Cascade.Core.Graphics.SpecificEffectManagers;

namespace Cascade.Core.Players.BuffHandlers
{
    public class DebuffHandler : ModPlayer
    {
        public bool CerebralMindtrick { get; set; }

        public bool CurseOfNecromancy { get; set; }

        public bool BellbirdStun { get; set; }

        public int CurseOfNecromancyMinionSlotStack { get; set; }

        private int BellbirdStunTime;

        private const int BellbirdStunMaxTime = 240;

        private float BellbirdStunTimeRatio => BellbirdStunTime / (float)BellbirdStunMaxTime;

        public override void ResetEffects()
        {
            CerebralMindtrick = false;
            CurseOfNecromancy = false;
            BellbirdStun = false;
        }

        public override void UpdateDead()
        {
            CerebralMindtrick = false;
            CurseOfNecromancy = false;
            BellbirdStun = false;

            CurseOfNecromancyMinionSlotStack = 0;
            BellbirdStunTime = 0;
        }

        public override void PostUpdateBuffs()
        {
            if (CerebralMindtrick)
            {

            }

            // Everytime the right click ability is used with the debuff applied, the 
            // effects of the curse stack.
            if (CurseOfNecromancy)
            {
                Player.maxMinions -= CurseOfNecromancyMinionSlotStack;

                // Some light dust visuals.
                Vector2 dustPosition = Player.Center + Main.rand.NextVector2Circular(Player.width, Player.height);
                Vector2 dustVelocity = Vector2.UnitY * Main.rand.NextFloat(-8f, -2f);
                Utilities.CreateDustLoop(2, dustPosition, dustVelocity, DustID.PurpleTorch);
            }

            if (BellbirdStun)
            {
                // Messed with a few player stats to mimic a more "realstic" disorentation effect.
                float statFuckeryInterpolant = Lerp(1f, 0.08f, BellbirdStunTimeRatio);

                // Reduce the player's acceleration and run speed.
                Player.runAcceleration = 0.08f * statFuckeryInterpolant;
                Player.moveSpeed = 1f * statFuckeryInterpolant;

                // Increase their fall speed from the mass disorentation.
                float fallSpeedInterpolant = Lerp(1f, 2.5f, BellbirdStunTimeRatio);
                Player.maxFallSpeed = 10f * fallSpeedInterpolant;

                // Visual effects.
                if (BellbirdStunTime <= BellbirdStunMaxTime)
                {
                    float abberationInterpolant = Lerp(0f, 35f, BellbirdStunTimeRatio);
                    SpecialScreenEffectSystem.ApplyChromaticAbberation(Main.LocalPlayer.Center, abberationInterpolant, 240);

                    float vignettePowerInterpolant = Lerp(20f, 2f, SineInOutEasing(BellbirdStunTimeRatio, 0));
                    float vignetteBrightnessInterpolant = Lerp(0f, 3f, SineInOutEasing(BellbirdStunTimeRatio, 0));
                    SpecialScreenEffectSystem.ApplyDarkVignette(Main.LocalPlayer.Center, vignettePowerInterpolant, vignetteBrightnessInterpolant, 180);
                }

                BellbirdStunTime++;
            }
        }

        public override void PostUpdate()
        {
            // Reset the stack count to zero.
            if (!CurseOfNecromancy)
                CurseOfNecromancyMinionSlotStack = 0;

            if (!BellbirdStun)
                BellbirdStunTime = 0;
        }
    }
}
