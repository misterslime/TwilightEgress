using Cascade.Core.BaseEntities.ModNPCs;
using Cascade.Core.Systems;

namespace Cascade.Content.NPCs.CosmostoneShowers
{
    public class GalileoPlanetoid : BasePlanetoid
    {
        public override float MaximumAttractionRadius => 150f;

        public override float WalkableRadius => 94f;

        public override void SafeSetDefaults()
        {
            NPC.width = 94;
            NPC.height = 94;
            NPC.lifeMax = 2;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.dontTakeDamage = true;
        }

        public override void SafeAI()
        {
            float totalAttractionRadius = MaximumAttractionRadius + WalkableRadius;
            Vector2 dustPosition = NPC.Center + Main.rand.NextVector2CircularEdge(totalAttractionRadius, totalAttractionRadius);
            if (Main.rand.NextBool(2))
                Utilities.CreateDustLoop(15, dustPosition, Vector2.UnitX, DustID.Electric);

            NPC.rotation += Tau / 600f;
            NPC.ShowNameOnHover = false;
        }
    }
}
