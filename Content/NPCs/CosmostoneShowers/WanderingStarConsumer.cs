namespace Cascade.Content.NPCs.CosmostoneShowers
{
    public abstract class WanderingStarConsumer : ModNPC
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            NPCID.Sets.CannotDropSouls[Type] = true;
            NPCID.Sets.CountsAsCritter[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 36;
            NPC.height = 36;
            NPC.damage = 0;
            NPC.defense = 40;
            NPC.lifeMax = 10;
            NPC.aiStyle = -1;
            NPC.dontCountMe = true;
            NPC.lavaImmune = true;
            NPC.noTileCollide = false;
            NPC.noGravity = true;
            NPC.knockBackResist = 0f;
            NPC.Opacity = 0f;
            NPC.dontTakeDamage = true;
            NPC.chaseable = false;
        }

        public override void AI()
        {

        }
    }
}
