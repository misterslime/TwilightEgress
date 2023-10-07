namespace Cascade.Core.Globals.GlobalNPCs
{
    public class DebuffHandlerGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool BellbirdStun { get; set; }

        public override void ResetEffects(NPC npc)
        {
            BellbirdStun = false;
        }

        public override bool PreAI(NPC npc)
        {
            if (BellbirdStun)
            {
                // Slow down the affected NPC.
                npc.velocity *= 0.4f;
            }

            return base.PreAI(npc);
        }
    }
}
