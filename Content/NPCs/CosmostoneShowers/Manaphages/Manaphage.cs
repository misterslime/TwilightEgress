namespace Cascade.Content.NPCs.CosmostoneShowers.Manaphages
{
    public class Manaphage : ModNPC
    {
        public enum ManaphageBehavior
        {
            Idle,
            Fleeing,
            Latching,
            Attacking,
        }

        public const float MaximumPlayerSearchDistance = 100f;

        public const float MaximumNPCSearchDistance = 1000f;

        public bool ShouldTargetPlayers;

        public bool ShouldTargetNPCs;

        public ref float Timer => ref NPC.ai[0];

        public ref float AIState => ref NPC.ai[1];

        public const int MovementIntervalIndex = 0;

        public const int IdleMovementDirectionIndex = 1;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.UsesNewTargetting[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 38;
            NPC.height = 50;
            NPC.damage = 25;
            NPC.defense = 3;
            NPC.lifeMax = 90;
            NPC.HitSound = SoundID.NPCHit25;
            NPC.DeathSound = SoundID.NPCDeath25;
            NPC.knockBackResist = 0.8f;
            NPC.value = 9f;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(ShouldTargetPlayers);
            writer.Write(ShouldTargetNPCs);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            ShouldTargetPlayers = reader.ReadBoolean();
            ShouldTargetNPCs = reader.ReadBoolean();
        }

        public override void AI()
        {
            bool playerSearchFilter(Player player)
                => ShouldTargetPlayers && player.WithinRange(NPC.Center, MaximumPlayerSearchDistance);

            bool npcSearchFilter(NPC npc)
            {
                if (npc.type == ModContent.NPCType<CosmostoneAsteroid>())
                    return ShouldTargetNPCs && npc.WithinRange(NPC.Center, MaximumNPCSearchDistance);
                return false;
            }

            Utilities.AdvancedNPCTargetSearching(NPC, playerSearchFilter, npcSearchFilter);
            NPCAimedTarget target = NPC.GetTargetData();

            DoBehavior_Idle(target);
            Timer++;
        }

        public void DoBehavior_Idle(NPCAimedTarget target)
        {
            ref float movementInterval = ref NPC.Cascade().ExtraAI[MovementIntervalIndex];
            ref float idleMovementDirection = ref NPC.Cascade().ExtraAI[IdleMovementDirectionIndex];

            if (Timer is 0)
            {
                idleMovementDirection = Main.rand.NextBool().ToDirectionInt();
                movementInterval = (int)(600 * Main.rand.NextFloat(0.6f, 1.4f));
            }

            // Avoid leaving the world and avoid running into tiles.
            Vector2 centerAhead = NPC.Center + NPC.velocity * 30f;
            bool leavingWorldBounds = centerAhead.X >= Main.maxTilesX * 16f - 700f || centerAhead.X < 700f || centerAhead.Y < Main.maxTilesY * 0.3f;
            bool shouldTurnAround = leavingWorldBounds;

            Vector2 velocityAhead = NPC.velocity.SafeNormalize(Vector2.Zero).RotatedBy(Pi);
            if (!Collision.CanHit(NPC.Center, 1, 1, NPC.Center + velocityAhead * 75f, 1, 1))
                shouldTurnAround = true;

            if (shouldTurnAround)
            {
                float distanceToTileCollisonLeft = DistanceToTileCollisionHit(NPC.Center, NPC.velocity.RotatedBy(-Pi)) ?? 1000f;
                float distanceToTileCollisonRight = DistanceToTileCollisionHit(NPC.Center, NPC.velocity.RotatedBy(Pi)) ?? 1000f;
                float turnAroundDirection = distanceToTileCollisonLeft > distanceToTileCollisonRight ? -1f : 1f;

                Vector2 turnAroundVelocity = NPC.velocity.RotatedBy(PiOver2 * turnAroundDirection);
                if (leavingWorldBounds)
                    turnAroundVelocity = centerAhead.Y < Main.maxTilesY * 0.3f ? Vector2.UnitY * 5f : centerAhead.X >= Main.maxTilesX * 16f - 700f ? Vector2.UnitX * -5f : Vector2.UnitX * 5f;

                NPC.velocity.MoveTowards(turnAroundVelocity, 0.15f);
                NPC.velocity = Vector2.Lerp(NPC.velocity, turnAroundVelocity, 0.15f);
            }
            else
            {
                // Carry on with normal idle movement.
                NPC.velocity = NPC.velocity.RotatedBy(Pi * idleMovementDirection * 0.003f);
                if (Timer % movementInterval == 0)
                    NPC.velocity = Vector2.One.RotatedByRandom(TwoPi) * Main.rand.NextFloat(0.4f, 1.3f);
            }

            // If there are targets, however, choose which AIState to move to.
            // If the player gets too close, switch to attacking them.
            if (target.Type == Terraria.Enums.NPCTargetType.Player)
            {

            }

            // If the Manaphage needs to replenish mana, switch to latching onto a nearby Asteroid.
            if (target.Type == Terraria.Enums.NPCTargetType.NPC)
            {

            }

            NPC.rotation = NPC.velocity.ToRotation() + 1.57f;
        }

        public void DoBehavior_Latching(NPCAimedTarget target)
        {
            // If by any chance there are no more available targets nearby, 
            // switch back to idling.
            if (target.Invalid)
            {

            }

            // Find the closest asteroid and turn towards it.
            float distanceBetween = Vector2.Distance(NPC.Center, target.Center);
            bool closestToProjectile = Vector2.Distance(NPC.Center, target.Center) > distanceBetween;
            if (!target.Invalid && closestToProjectile)
            {
                
            }
        }
    }
}
