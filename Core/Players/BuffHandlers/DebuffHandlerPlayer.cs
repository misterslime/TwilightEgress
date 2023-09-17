namespace Cascade.Core.Players.BuffHandlers
{
    public class DebuffHandlerPlayer : ModPlayer
    {
        public bool CerebralMindtrick { get; set; }

        public bool CurseOfNecromancy { get; set; }

        public int CurseOfNecromancyMinionSlotStack { get; set; }

        public override void ResetEffects()
        {
            CerebralMindtrick = false;
            CurseOfNecromancy = false;
        }

        public override void UpdateDead()
        {
            CerebralMindtrick = false;
            CurseOfNecromancy = false;

            CurseOfNecromancyMinionSlotStack = 0;
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
        }

        public override void PostUpdate()
        {
            // Reset the stack count to zero.
            if (!CurseOfNecromancy)
                CurseOfNecromancyMinionSlotStack = 0;
        }
    }
}
