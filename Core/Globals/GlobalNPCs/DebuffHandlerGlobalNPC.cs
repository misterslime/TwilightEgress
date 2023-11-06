namespace Cascade.Core.Globals.GlobalNPCs
{
    public class DebuffHandlerGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool BellbirdStun { get; set; }

        private int BellbirdStunTime;

        private const int BellbirdStunMaxTime = 240;

        private float BellbirdStunTimeRatio => BellbirdStunTime / (float)BellbirdStunMaxTime;

        public override void ResetEffects(NPC npc)
        {
            BellbirdStun = false;
            if (!BellbirdStun)
                BellbirdStunTime = 0;
        }

        public override bool PreAI(NPC npc)
        {
            if (BellbirdStun)
            {
                float statFuckeryInterpolant = Lerp(1f, 0.08f, BellbirdStunTimeRatio);
                // Slow down the affected NPC.
                npc.velocity.X *= 0.8f * statFuckeryInterpolant;          
            }

            return base.PreAI(npc);
        }
    }
}
