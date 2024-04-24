using Cascade.Common.Systems.PlanetoidSystem;

namespace Cascade.Content.Planetoids
{
    public class GalileoPlanetoid : Planetoid
    {
        public override float maxAttractionRadius => 150f;

        public override float walkableRadius => 94f;

        public override void Update()
        {
            float totalAttractionRadius = maxAttractionRadius + walkableRadius;
            Vector2 dustPosition = Entity.Center + Main.rand.NextVector2CircularEdge(totalAttractionRadius, totalAttractionRadius);
            if (Main.rand.NextBool(2))
                CascadeUtilities.CreateDustLoop(15, dustPosition, Vector2.UnitX, DustID.Electric);

            rotation += Tau / 600f;
        }
    }
}
