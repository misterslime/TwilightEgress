namespace Cascade.Core.Globals
{
    public partial class CascadeGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public float[] ExtraAI = new float[100];

        internal bool[] IsCustomAISlotBeingUsed = new bool[100];

        internal int TotalCustomAISlotsInUse => IsCustomAISlotBeingUsed.Count(slot => slot);

        public override void SetDefaults(NPC npc)
        {
            for (int i = 0; i < ExtraAI.Length; i++)
                ExtraAI[i] = 0f;
        }

        public override void PostAI(NPC npc)
        {
            for (int i = 0; i < ExtraAI.Length; i++)
            {
                if (ExtraAI[i] != 0f)
                    IsCustomAISlotBeingUsed[i] = true;
            }

            base.PostAI(npc);
        }
    }
}
