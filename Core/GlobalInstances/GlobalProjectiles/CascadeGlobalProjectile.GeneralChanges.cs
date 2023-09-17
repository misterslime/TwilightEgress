namespace Cascade.Core.GlobalInstances.GlobalProjectiles
{
    public partial class CascadeGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public float[] ExtraAI = new float[100];

        internal bool[] IsCustomAISlotBeingUsed = new bool[100];

        internal int TotalCustomAISlotsInUse => IsCustomAISlotBeingUsed.Count(slot => slot);

        public override void SetDefaults(Projectile projectile)
        {
            for (int i = 0; i < ExtraAI.Length; i++)
                ExtraAI[i] = 0f;

            base.SetDefaults(projectile);
        }

        public override void PostAI(Projectile projectile)
        {
            for (int i = 0; i < ExtraAI.Length; i++)
            {
                if (ExtraAI[i] != 0f)
                    IsCustomAISlotBeingUsed[i] = true;
            }

            base.PostAI(projectile);
        }
    }
}
