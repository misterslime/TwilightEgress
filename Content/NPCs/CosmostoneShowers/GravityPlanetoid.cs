using Cascade.Core.Players;

namespace Cascade.Content.NPCs.CosmostoneShowers
{
    // TO-DO:
    // This needs to be expanded into its own Base Class for easy use and replication.
    public class GravityPlanetoid : ModNPC
    {
        public float MaximumAttractionRadius = 200f;

        public float WalkableRadius = 94f;

        public override void SetDefaults()
        {
            NPC.width = 94;
            NPC.height = 94;
            NPC.lifeMax = 2;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.dontTakeDamage = true;
            NPC.noTileCollide = true;
            NPC.lavaImmune = true;
        }

        public override void AI()
        {
            Vector2 dustPosition = NPC.Center + Main.rand.NextVector2CircularEdge(294f, 294f);
            if (Main.rand.NextBool(2))
                Utilities.CreateDustLoop(5, dustPosition, Vector2.UnitX, DustID.Electric);

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (!player.active) 
                    return;

                GravityPlanetoidPlayer planetoidPlayer = player.GetModPlayer<GravityPlanetoidPlayer>();

                float distanceBetweenBodies = Vector2.Distance(player.Center, NPC.Center);
                float totalAttractionRadius = MaximumAttractionRadius + WalkableRadius;

                if (distanceBetweenBodies < totalAttractionRadius && planetoidPlayer.AttractionCooldown <= 0 && planetoidPlayer.Planetoid is null)
                {
                    planetoidPlayer.Planetoid = this;
                    planetoidPlayer.PlayerAngle = (player.Center - NPC.Center).ToRotation();
                }
            }
        }
    }
}
