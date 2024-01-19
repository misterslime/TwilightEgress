namespace Cascade.Content.NPCs.CosmostoneShowers.Manaphages
{
    public class Manaphage : ModNPC
    {
        public enum ManaphageBehavior
        {
            Idle_JellyfishPropulsion,
            Idle_LazeAround,
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

        public ref float LocalAIState => ref NPC.ai[2];

        public const int JellyfishMovementIntervalIndex = 0;

        public const int LazeMovementIntervalIndex = 1;

        public const int IdleMovementDirectionIndex = 2;

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

        public override void OnSpawn(IEntitySource source)
        {
            // Select a random idle movement type on spawn.
            AIState = Utils.SelectRandom(Main.rand, 0f, 1f);
            NPC.netUpdate = true;
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

            switch ((ManaphageBehavior)AIState)
            {
                case ManaphageBehavior.Idle_JellyfishPropulsion:
                    DoBehavior_JellyfishPropulsionIdle(target); 
                    break;

                case ManaphageBehavior.Idle_LazeAround:
                    DoBehavior_LazeAroundIdle(target);
                    break;
            }

            Timer++;
        }

        public void DoBehavior_JellyfishPropulsionIdle(NPCAimedTarget target)
        {
            ref float jellyfishMovementInterval = ref NPC.Cascade().ExtraAI[JellyfishMovementIntervalIndex];
            ref float idleMovementDirection = ref NPC.Cascade().ExtraAI[IdleMovementDirectionIndex];

            int idleSwitchInterval = 1800;

            if (Timer is 0)
            {
                idleMovementDirection = Main.rand.NextBool().ToDirectionInt();
                jellyfishMovementInterval = (int)(150 * Main.rand.NextFloat(0.6f, 1.4f));
            }

            // Move forward every few seconds.
            if (Timer % jellyfishMovementInterval == 0)
            {
                Vector2 velocity = Vector2.One.RotatedByRandom(TwoPi) * Main.rand.NextFloat(3f, 3.2f);

                // Avoid leaving the world and avoid running into tiles.
                Vector2 centerAhead = NPC.Center + NPC.velocity * 40f;
                bool leavingWorldBounds = centerAhead.X >= Main.maxTilesX * 16f - 700f || centerAhead.X < 700f || centerAhead.Y < Main.maxTilesY * 0.3f;
                bool shouldTurnAround = leavingWorldBounds;

                Vector2 velocityAhead = NPC.velocity.SafeNormalize(Vector2.Zero).RotatedBy(Pi);
                if (!Collision.CanHit(NPC.Center, 1, 1, NPC.Center + velocityAhead * 75f, 1, 1))
                    shouldTurnAround = true;

                // Either turn around or continue forward as normal.
                if (shouldTurnAround)
                    NPC.TurnAroundMovement(centerAhead, leavingWorldBounds);
                else
                    NPC.velocity = velocity;

                // Spawn some lil' visual particles everytime it ejects.
                Utilities.CreateRandomizedDustExplosion(15, NPC.Center, DustID.BlueFairy);
                DirectionalPulseRing pulseRing = new(NPC.Center - NPC.SafeDirectionTo(NPC.Center) * 60f, Vector2.Zero, Color.DeepSkyBlue, new Vector2(0.5f, 2f), NPC.velocity.ToRotation(), 0f, 0.3f, 45);
                GeneralParticleHandler.SpawnParticle(pulseRing);
            }

            NPC.velocity *= 0.98f;
            NPC.rotation = NPC.velocity.ToRotation() + 1.57f;

            // Randomly switch to the other idle AI state.
            if (Timer >= idleSwitchInterval && Main.rand.NextBool(2))
            {
                Timer = 0f;
                AIState = 1f;
                NPC.netUpdate = true;
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
        }

        public void DoBehavior_LazeAroundIdle(NPCAimedTarget target)
        {
            ref float lazeMovementInterval = ref NPC.Cascade().ExtraAI[LazeMovementIntervalIndex];
            ref float idleMovementDirection = ref NPC.Cascade().ExtraAI[IdleMovementDirectionIndex];

            int idleSwitchInterval = 1800;

            if (Timer is 0)
            {
                lazeMovementInterval = Main.rand.Next(240, 360);
                idleMovementDirection = Main.rand.NextBool().ToDirectionInt();
            }

            if (Timer % lazeMovementInterval == 0)
            {
                Vector2 velocity = Vector2.One.RotatedByRandom(TwoPi) * Main.rand.NextFloat(0.1f, 0.13f);

                // Avoid leaving the world and avoid running into tiles.
                Vector2 centerAhead = NPC.Center + NPC.velocity * 40f;
                bool leavingWorldBounds = centerAhead.X >= Main.maxTilesX * 16f - 700f || centerAhead.X < 700f || centerAhead.Y < Main.maxTilesY * 0.3f;
                bool shouldTurnAround = leavingWorldBounds;

                Vector2 velocityAhead = NPC.velocity.SafeNormalize(Vector2.Zero).RotatedBy(Pi);
                if (!Collision.CanHit(NPC.Center, 1, 1, NPC.Center + velocityAhead * 75f, 1, 1))
                    shouldTurnAround = true;

                // Either turn around or continue forward as normal.
                if (shouldTurnAround)
                    NPC.TurnAroundMovement(centerAhead, leavingWorldBounds);
                else
                    NPC.velocity = velocity;
            }

            if (NPC.velocity.Length() > 0.13f)
                NPC.velocity *= 0.98f;

            NPC.rotation *= 0.98f;
            // Randomly switch to the other idle AI state.
            if (Timer >= idleSwitchInterval && Main.rand.NextBool(2))
            {
                Timer = 0f;
                AIState = 1f;
                NPC.netUpdate = true;
            }
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
