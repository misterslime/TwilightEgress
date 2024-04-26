using CalamityMod;
using CalamityMod.Particles;
using Cascade.Content.NPCs.CosmostoneShowers.Asteroids;
using Terraria;
using Terraria.ModLoader.IO;

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

        public enum ManaphageAnimation
        {
            Attack,
            Inject,
            Idle,
            LookLeft,
            LookRight,
            Sucking
        }

        public const int JellyfishMovementIntervalIndex = 0;

        public const int LazeMovementIntervalIndex = 1;

        public const int IdleMovementDirectionIndex = 2;

        public const int AdditionalAggroRangeIndex = 3;

        public const int AggroRangeTimerIndex = 4;

        public const int ManaSuckTimerIndex = 5;

        public const int JellyfishMovementAngleIndex = 6;

        public const int SpriteStretchXIndex = 7;

        public const int SpriteStretchYIndex = 8;

        public const int JellyfishPropulsionCountIndex = 9;

        public const int MaxPropulsionsIndex = 10;

        public const float MaximumPlayerSearchDistance = 1200f;

        public const float MaximumNPCSearchDistance = 1200f;

        public const float MaximumManaCapacity = 100f;

        public float CurrentManaCapacity = 0f;

        public bool ShouldTargetPlayers;

        public bool ShouldTargetNPCs;

        public int Animation;

        public NPC AsteroidToSucc;

        public float ManaRatio => CurrentManaCapacity / MaximumManaCapacity;

        public float LifeRatio => NPC.life / (float)NPC.lifeMax;

        public Player NearestPlayer => Main.player[NPC.target];

        public ref float Timer => ref NPC.ai[0];

        public ref float AIState => ref NPC.ai[1];

        public ref float LocalAIState => ref NPC.ai[2];

        public override void SetStaticDefaults()
        {
            NPCID.Sets.UsesNewTargetting[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 44;
            NPC.height = 62;
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
            ref float spriteStretchX = ref NPC.Cascade().ExtraAI[SpriteStretchXIndex];
            ref float spriteStretchY = ref NPC.Cascade().ExtraAI[SpriteStretchYIndex];

            AIState = (float)Utils.SelectRandom(Main.rand, ManaphageBehavior.Idle_JellyfishPropulsion, ManaphageBehavior.Idle_LazeAround);
            CurrentManaCapacity = Main.rand.NextBool(25) ? Main.rand.NextFloat(1f, 10f) : Main.rand.NextFloat(50f, 100f);
            spriteStretchX = 1f;
            spriteStretchY = 1f;
            NPC.netUpdate = true;
        }

        public override ModNPC Clone(NPC newEntity)
        {
            ModNPC clone = base.Clone(newEntity);
            if (clone is Manaphage clonePhage && newEntity.ModNPC is Manaphage manaphage)
                clonePhage.CurrentManaCapacity = manaphage.CurrentManaCapacity;

            return clone;
        }

        public override void SaveData(TagCompound tag)
        {
            tag["CurrentManaCapacity"] = CurrentManaCapacity;
        }

        public override void LoadData(TagCompound tag)
        {
            CurrentManaCapacity = tag.GetFloat("CurrentManaCapacity");
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(ShouldTargetPlayers);
            writer.Write(ShouldTargetNPCs);
            writer.Write(CurrentManaCapacity);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            ShouldTargetPlayers = reader.ReadBoolean();
            ShouldTargetNPCs = reader.ReadBoolean();
            CurrentManaCapacity = reader.ReadSingle();
        }

        public override void AI()
        {
            bool playerSearchFilter(Player player)
                => player.WithinRange(NPC.Center, MaximumPlayerSearchDistance);
            bool npcSearchFilter(NPC npc)
            {
                if (npc.type == ModContent.NPCType<CosmostoneAsteroidSmall>())
                    return ShouldTargetNPCs && npc.WithinRange(NPC.Center, MaximumNPCSearchDistance);
                return false;
            }

            // This utility below needs to be rewritten entirely. Don't bother with it for now.
            // fryzahh.
            //Utilities.AdvancedNPCTargetSearching(NPC, playerSearchFilter, npcSearchFilter);

            NPCAimedTarget target = NPC.GetTargetData();

            // Don't bother targetting any asteroids at 60% and above mana capacity.
            if (ManaRatio > 0.6f)
                ShouldTargetNPCs = false;
            else
                // If the Manaphage is under 50% of it's maximum mana capacity, start detecting asteroids
                // only if the Manaphage isn't currently fleeing from anything.
                ShouldTargetNPCs = AIState != (float)ManaphageBehavior.Fleeing;

            // Testiing purposes till the shader gets done. 
            /*if (Main.gameTimeCache.TotalGameTime.Ticks % 60 == 0)
                CombatText.NewText(new Rectangle((int)NPC.Center.X, (int)NPC.Center.Y, NPC.width, NPC.height), Color.CornflowerBlue, (int)CurrentManaCapacity, true);*/

            switch ((ManaphageBehavior)AIState)
            {
                case ManaphageBehavior.Idle_JellyfishPropulsion:
                    DoBehavior_JellyfishPropulsionIdle(target); 
                    break;

                case ManaphageBehavior.Idle_LazeAround:
                    DoBehavior_LazeAroundIdle(target);
                    break;

                case ManaphageBehavior.Fleeing:
                    DoBehavior_Fleeing(target);
                    break;

                case ManaphageBehavior.Latching:
                    DoBehavior_Latching(target);
                    break;

                case ManaphageBehavior.Attacking:
                    DoBehavior_Attacking(target);
                    break;
            }

            // Clamp the mana capacity.
            CurrentManaCapacity = Clamp(CurrentManaCapacity, 0f, MaximumManaCapacity);

            // Manages the extra additional aggro range and mana suck timers.
            ManageExtraTimers();

            Timer++;
        }

        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone) => OnHitEffects(hit);

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone) => OnHitEffects(hit);

        public void OnHitEffects(NPC.HitInfo hit)
        {
            ref float additionalAggroRange = ref NPC.Cascade().ExtraAI[AdditionalAggroRangeIndex];
            ref float aggroRangeTimer = ref NPC.Cascade().ExtraAI[AggroRangeTimerIndex];
            ref float manaSuckTimer = ref NPC.Cascade().ExtraAI[ManaSuckTimerIndex];

            // Apply an extra 100 pixels of aggro range everytime the Manaphage is attacked.
            // 200 is added instead if the hit was critical.
            additionalAggroRange += hit.Crit ? 100f : 50f;
            if (additionalAggroRange > 500f)
                additionalAggroRange = 500f;

            // Timer resetting.
            aggroRangeTimer = 720f;         
            manaSuckTimer = 720f;
        }

        public void ManageExtraTimers()
        {
            // Manages the extra additional aggro range and mana suck timers.
            ref float additionalAggroRange = ref NPC.Cascade().ExtraAI[AdditionalAggroRangeIndex];
            ref float aggroRangeTimer = ref NPC.Cascade().ExtraAI[AggroRangeTimerIndex];
            ref float manaSuckTimer = ref NPC.Cascade().ExtraAI[ManaSuckTimerIndex];

            // This timer controls how long the additional aggro range is applied for.
            aggroRangeTimer = Clamp(aggroRangeTimer - 1f, 0f, 1200f);
            if (aggroRangeTimer <= 0f)
            {
                aggroRangeTimer = 0f;
                additionalAggroRange = Clamp(additionalAggroRange - 1f, 0f, 500f);
            }

            // This timer controls if a Manaphage should target an asteroid and absorb
            // mana at 50% mana capacity. Manaphages typically start fleeing and looking 
            // Asteroids at aunder 30% mana capacity.
            manaSuckTimer = Clamp(manaSuckTimer - 1f, 0f, 720f);
        }

        public void DoBehavior_JellyfishPropulsionIdle(NPCAimedTarget target)
        {
            ref float jellyfishMovementInterval = ref NPC.Cascade().ExtraAI[JellyfishMovementIntervalIndex];
            ref float jellyfishMovementAngle = ref NPC.Cascade().ExtraAI[JellyfishMovementAngleIndex];
            ref float spriteStretchX = ref NPC.Cascade().ExtraAI[SpriteStretchXIndex];
            ref float spriteStretchY = ref NPC.Cascade().ExtraAI[SpriteStretchYIndex];
            ref float jellyfishPropulsionCount = ref NPC.Cascade().ExtraAI[JellyfishPropulsionCountIndex];
            ref float maxPropulsions = ref NPC.Cascade().ExtraAI[MaxPropulsionsIndex];

            Animation = (int)ManaphageAnimation.Idle;

            if (LocalAIState == 0f)
            {
                jellyfishMovementInterval = 120;
                maxPropulsions = Main.rand.NextFloat(10f, 25f);
                LocalAIState = 1f;
                Timer = 0f;
                NPC.netUpdate = true;
            }

            if (LocalAIState == 1f)
            {
                // Squash the sprite slightly before the propulsion movement to give a
                // more cartoony, jellyfish-like feeling to the movement.
                if (Timer <= jellyfishMovementInterval)
                {
                    float stretchInterpolant = Utils.GetLerpValue(0f, 1f, (float)(Timer / jellyfishMovementInterval), true);
                    spriteStretchX = Lerp(spriteStretchX, 1.25f, CascadeUtilities.SineEaseInOut(stretchInterpolant));
                    spriteStretchY = Lerp(spriteStretchY, 0.75f, CascadeUtilities.SineEaseInOut(stretchInterpolant));
                }

                // Pick a random angle to move towards before propelling forward.
                if (Timer == (jellyfishMovementInterval / 2))
                    jellyfishMovementAngle = Main.rand.NextFloat(TwoPi);

                // Move forward every few seconds.
                if (Timer == jellyfishMovementInterval)
                {
                    //Vector2 velocity = Vector2.One.RotatedBy(jellyfishMovementAngle) * Main.rand.NextFloat(3f, 4f);

                    // BEHAVIOR FOR TURNING AROUND WHEN NEARING TILES OR NEARING THE EDGE OF THE WORLD GOES HERE.
                    // - fryzahh

                    // Spawn some lil' visual particles everytime it ejects.
                    CascadeUtilities.CreateRandomizedDustExplosion(15, NPC.Center, DustID.BlueFairy);
                    PulseRingParticle propulsionRing = new(NPC.Center - NPC.SafeDirectionTo(NPC.Center) * 60f, NPC.SafeDirectionTo(NPC.Center) * -5f, Color.DeepSkyBlue, 0f, 0.3f, new Vector2(0.5f, 2f), NPC.velocity.ToRotation(), 45);
                    propulsionRing.SpawnCasParticle();

                    // Unstretch the sprite.
                    spriteStretchX = 0.8f;
                    spriteStretchY = 1.25f;

                    jellyfishPropulsionCount++;
                }

                if (Timer >= jellyfishMovementInterval + 45)
                    Timer = 0f;

                NPC.velocity *= 0.98f;
                Vector2 futureVelocity = Vector2.One.RotatedBy(jellyfishMovementAngle);
                NPC.rotation = NPC.rotation.AngleLerp(futureVelocity.ToRotation() + 1.57f, 0.1f);

                // Randomly switch to the other idle AI state.
                if (jellyfishPropulsionCount >= maxPropulsions && Main.rand.NextBool(2))
                {
                    Timer = 0f;
                    AIState = (float)ManaphageBehavior.Idle_LazeAround;
                    NPC.netUpdate = true;
                }
            }

            SwitchBehavior_Attacking(target);
            SwitchBehavior_Latching(target);
            SwitchBehavior_Fleeing(target);
        }

        public void DoBehavior_LazeAroundIdle(NPCAimedTarget target)
        {
            ref float lazeMovementInterval = ref NPC.Cascade().ExtraAI[LazeMovementIntervalIndex];
            ref float idleMovementDirection = ref NPC.Cascade().ExtraAI[IdleMovementDirectionIndex];
            ref float spriteStretchX = ref NPC.Cascade().ExtraAI[SpriteStretchXIndex];
            ref float spriteStretchY = ref NPC.Cascade().ExtraAI[SpriteStretchYIndex];

            int idleSwitchInterval = 1800;

            Animation = (int)ManaphageAnimation.Idle;

            if (Timer is 0)
            {
                lazeMovementInterval = Main.rand.Next(240, 360);
                idleMovementDirection = Main.rand.NextBool().ToDirectionInt();
            }

            // BEHAVIOR FOR TURNING AROUND WHEN NEARING TILES OR NEARING THE EDGE OF THE WORLD GOES HERE.
            // - fryzahh
            
            if (Timer % lazeMovementInterval == 0)
            {
                Vector2 velocity = Vector2.One.RotatedByRandom(TwoPi) * Main.rand.NextFloat(0.1f, 0.13f);
                NPC.velocity = velocity;
            }

            if (NPC.velocity.Length() > 0.13f)
                NPC.velocity *= 0.98f;

            NPC.rotation = NPC.rotation.AngleLerp(NPC.rotation + NPC.velocity.X * 0.03f, 0.2f);

            // Randomly switch to the other idle AI state.
            if (Timer >= idleSwitchInterval && Main.rand.NextBool(2))
            {
                Timer = 0f;
                AIState = (float)ManaphageBehavior.Idle_JellyfishPropulsion;
                LocalAIState = 0f;
                NPC.netUpdate = true;
            }

            // Squash and stretch the sprite passively.
            spriteStretchX = Lerp(1f, 1.10f, CascadeUtilities.SineEaseInOut(Timer / 160f));
            spriteStretchY = Lerp(1f, 0.8f, CascadeUtilities.SineEaseInOut(Timer / 120f));

            //SwitchBehavior_Attacking(target);
            SwitchBehavior_Latching(target);
            SwitchBehavior_Fleeing(target);
        }

        public void DoBehavior_Fleeing(NPCAimedTarget target)
        {
            Animation = (int)ManaphageAnimation.Attack;

            // Run away from any nearby players.
            List<Player> nearbyPlayers = Main.player.Take(Main.maxPlayers).Where(player => player.active && player.Distance(NPC.Center) <= 600f).ToList();
            if (nearbyPlayers.Count <= 0 || target.Type != Terraria.Enums.NPCTargetType.Player || target.Invalid)
            {
                AIState = (float)Utils.SelectRandom(Main.rand, ManaphageBehavior.Idle_JellyfishPropulsion, ManaphageBehavior.Idle_LazeAround);
                LocalAIState = 0f;
                Timer = 0f;
                NPC.netUpdate = true;
                return;
            }

            float avoidanceSpeedInterpolant = Utils.GetLerpValue(0f, 1f, NPC.Distance(nearbyPlayers.FirstOrDefault().Center) / 300f, true);
            NPC.velocity += NPC.SafeDirectionTo(nearbyPlayers.FirstOrDefault().Center) * Lerp(-1f, -4f, avoidanceSpeedInterpolant) * 0.05f;
            NPC.rotation = NPC.rotation.AngleLerp(NPC.velocity.ToRotation() + 1.57f, 0.2f);
        }

        public void DoBehavior_Latching(NPCAimedTarget target)
        {
            Animation = (int)ManaphageAnimation.Inject;

            // Reset if the asteroid target variable is null.
            if (AsteroidToSucc is null)
            {
                Timer = 0f;
                AIState = (float)Utils.SelectRandom(Main.rand, ManaphageBehavior.Idle_JellyfishPropulsion, ManaphageBehavior.Idle_LazeAround);
                LocalAIState = 0f;
                NPC.netUpdate = true;
                return;
            }

            if (target.Invalid || ManaRatio >= 1f)
            {
                NPC.velocity = -NPC.SafeDirectionTo(AsteroidToSucc.Center) * 5f;
                AsteroidToSucc = null;
            }

            // Pre-succ.
            if (LocalAIState == 0f)
            {
                NPC.rotation = NPC.rotation.AngleLerp(NPC.AngleTo(AsteroidToSucc.Center) - 1.57f, 0.2f);
                if (Timer >= 45)
                {
                    // Quickly move towards the selected asteroid and stop moving once the two hitboxes
                    // intersect with each other. 
                    NPC.SimpleMove(AsteroidToSucc.Center, 12f, 6f);
                    if (NPC.Hitbox.Intersects(AsteroidToSucc.Hitbox))
                    {
                        LocalAIState = 1f;
                        Timer = 0f;
                        NPC.netUpdate = true;
                    }
                }
            }

            // Post-succ.
            if (LocalAIState == 1f)
            {
                NPC.velocity *= 0.2f;
                // Suck mana out of it.
                if (Timer % 30 == 0)
                {
                    Animation = (int)ManaphageAnimation.Sucking;
                    int damageToAsteroid = Main.rand.Next(10, 15);
                    float manaToSuck = Main.rand.NextFloat(5f, 10f);
                    AsteroidToSucc.SimpleStrikeNPC(damageToAsteroid, 0, noPlayerInteraction: true);
                    CurrentManaCapacity += manaToSuck;
                }
            }

            SwitchBehavior_Fleeing(target);
        }

        public void DoBehavior_Attacking(NPCAimedTarget target)
        {
            Animation = (int)ManaphageAnimation.Attack;

            ref float additionalAggroRange = ref NPC.Cascade().ExtraAI[AdditionalAggroRangeIndex];
            ref float manaSuckTimer = ref NPC.Cascade().ExtraAI[ManaSuckTimerIndex];

            float aggroRange = 550f + additionalAggroRange;
            bool targetOutOfRange = NPC.Distance(target.Center) >= aggroRange;
            if (target.Invalid || target.Type != Terraria.Enums.NPCTargetType.Player || targetOutOfRange)
            {
                AIState = (float)Utils.SelectRandom(Main.rand, ManaphageBehavior.Idle_JellyfishPropulsion, ManaphageBehavior.Idle_LazeAround);
                LocalAIState = 0f;
                Timer = 0f;
                NPC.netUpdate = true;
                return;
            }

            // Reset the mana suck timer to a smaller duration if mana capacity is low. 
            manaSuckTimer = ManaRatio < 0.5f ? 420f : 720f;

            NPC.rotation = NPC.rotation.AngleLerp(NPC.AngleTo(target.Center) - 1.57f, 0.2f);    
            Vector2 areaAroundPlayer = target.Center + NPC.DirectionFrom(target.Center) * 300f;
            // Increase the Manaphage's movement speed depending on how much additional aggro range
            // it has ammased.
            float movementSpeed = Lerp(5f, 10f, Utils.GetLerpValue(0f, 1f, additionalAggroRange / 500f, true));
            NPC.SimpleMove(areaAroundPlayer, movementSpeed, 20f);

            if (Timer % 5 == 0)
            {
                CurrentManaCapacity -= 0.25f;

                Vector2 spawnPosition = NPC.Center + Vector2.UnitY.RotatedBy(NPC.rotation) * 30f;
                Vector2 inkVelocity = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitX) * 14f + NPC.velocity;

                NPC.BetterNewProjectile(spawnPosition, inkVelocity, ModContent.ProjectileType<ManaInk>(), NPC.defDamage.GetPercentageOfInteger(0.45f), 0f);
            }

            SwitchBehavior_Fleeing(target);
        }

        public void SwitchBehavior_Attacking(NPCAimedTarget target)
        {
            ref float additionalAggroRange = ref NPC.Cascade().ExtraAI[AdditionalAggroRangeIndex];

            // If there are targets, however, choose which AIState to move to.
            // If the player gets too close, switch to attacking them.
            if (ManaRatio > 0.3 && target.Type == Terraria.Enums.NPCTargetType.Player)
            {
                float aggroRange = 200f + additionalAggroRange;
                bool playerWithinRange = Vector2.Distance(NPC.Center, target.Center) < aggroRange;
                if (playerWithinRange)
                {
                    CombatText.NewText(new Rectangle((int)NPC.Center.X, (int)NPC.Center.Y, NPC.width, NPC.height), Color.Red, "Attacking the player!", true);
                    AIState = (float)ManaphageBehavior.Attacking;
                    Timer = 0f;
                    LocalAIState = 0f;
                    NPC.netUpdate = true;
                }
            }
        }

        public void SwitchBehavior_Latching(NPCAimedTarget target)
        {
            ref float manaSuckTimer = ref NPC.Cascade().ExtraAI[ManaSuckTimerIndex];

            // If the Manaphage starts becoming low on mana, start looking for nearby Asteroids.
            if (target.Type == Terraria.Enums.NPCTargetType.NPC)
            {
                List<NPC> cosmostoneAsteroids = Main.npc.Take(Main.maxNPCs).Where(npc => npc.active && npc.type == ModContent.NPCType<CosmostoneAsteroidSmall>() && NPC.Distance(npc.Center) <= 300).ToList();
                if (cosmostoneAsteroids.Count <= 0)
                    return;

                // Once the Manaphage reaches a low enough mana capacity, find the nearest asteroid and latch onto it.
                bool canSuckMana = ManaRatio < 0.3f || (ManaRatio < 0.6f && manaSuckTimer <= 0);
                if (canSuckMana)
                {
                    CombatText.NewText(new Rectangle((int)NPC.Center.X, (int)NPC.Center.Y, NPC.width, NPC.height), Color.LightBlue, "Going to succ mana!", true);
                    AsteroidToSucc = cosmostoneAsteroids.FirstOrDefault();
                    AIState = (float)ManaphageBehavior.Latching;
                    Timer = 0f;
                    LocalAIState = 0f;
                    NPC.netUpdate = true;
                }
            }
        }

        public void SwitchBehavior_Fleeing(NPCAimedTarget target)
        {
            float maxDetectionDistance = AIState == (float)ManaphageBehavior.Latching ? 150f : 300f;
            bool canFlee = target.Type == Terraria.Enums.NPCTargetType.Player && NPC.Distance(target.Center) <= maxDetectionDistance && AIState != (float)ManaphageBehavior.Fleeing;

            if ((ManaRatio < 0.3f || LifeRatio < 0.2f) && canFlee)
            {
                CombatText.NewText(new Rectangle((int)NPC.Center.X, (int)NPC.Center.Y, NPC.width, NPC.height), Color.Goldenrod, "Running away!", true);
                AIState = (float)ManaphageBehavior.Fleeing;
                LocalAIState = 0f;
                NPC.netUpdate = true;
            }
        }

        public void CheckForTurnAround(out bool turnAround)
        {
            turnAround = false;

            // Avoid leaving the world and avoid running into tiles.
            Vector2 centerAhead = NPC.Center + NPC.velocity * 40f;
            bool leavingWorldBounds = centerAhead.X >= Main.maxTilesX * 16f - 700f || centerAhead.X < 700f || centerAhead.Y < Main.maxTilesY * 0.3f;
            if (!Collision.CanHit(NPC.Center, 1, 1, centerAhead.SafeNormalize(Vector2.Zero).RotatedBy(NPC.rotation), 1, 1) || leavingWorldBounds)
                turnAround = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            ref float spriteStretchX = ref NPC.Cascade().ExtraAI[SpriteStretchXIndex];
            ref float spriteStretchY = ref NPC.Cascade().ExtraAI[SpriteStretchYIndex];

            Asset<Texture2D> texture = TextureAssets.Npc[Type];
            Vector2 origin = texture.Size() / 2f;
            Vector2 drawPosition = NPC.Center - Main.screenPosition;
            Vector2 stretchFactor = new(spriteStretchX, spriteStretchY);

            Rectangle sourceRectangle = texture.Frame(6, 5, frameX: Animation, frameY: (int)Math.Floor(Timer / 6) % 5);

            Main.EntitySpriteDraw(texture.Value, drawPosition, sourceRectangle, NPC.GetAlpha(drawColor), NPC.rotation, new Vector2(44, 62) / 2f, NPC.scale * stretchFactor, 0);
            return false;
        }
    }
}
