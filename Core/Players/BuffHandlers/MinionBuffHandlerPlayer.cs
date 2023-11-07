namespace Cascade.Core.Players.BuffHandlers
{
    public class MinionBuffHandlerPlayer : ModPlayer
    {
        public bool MoonSpiritLantern;

        public bool GeminiGenies;

        public bool GeminiGeniesVanity;

        public override void ResetEffects()
        {
            MoonSpiritLantern = false;
            GeminiGenies = false;
            GeminiGeniesVanity = false;
        }

        public override void UpdateDead()
        {
            MoonSpiritLantern = false;
            GeminiGenies = false;
            GeminiGeniesVanity = false;
        }
    }
}
