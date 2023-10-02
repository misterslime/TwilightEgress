using Cascade.Content.Buffs.Minions;

namespace Cascade.Content.DedicatedContent.MPG
{
    public class UnderworldLantern : ModProjectile, ILocalizedModType
    {
        private enum AttackState
        {
            Idling,
            UndeadSpiritTransformation
        }

        private bool ShouldDealContactDamage = false;

        private Player Owner => Main.player[Projectile.owner];

        private ref float Timer => ref Projectile.ai[0];

        private ref float AIState => ref Projectile.ai[1];

        private ref float LocalAIState => ref Projectile.ai[2]; 

        private const int IdleAngleIndex = 0;

        public new string LocalizationCategory => "Projectiles.Summon";

        public override void SetStaticDefaults()
        {
            // Designates the projectile as a pet/minion. Leaving this here as a reminder incase I forget it again.
            Main.projPet[Type] = true;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 13;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults() 
        {
            Projectile.width = 24;
            Projectile.height = 40;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
            Projectile.timeLeft = 9999;
            Projectile.minion = true;
            Projectile.minionSlots = 1;
        }

        public override void SendExtraAI(BinaryWriter writer) => writer.Write(ShouldDealContactDamage);

        public override void ReceiveExtraAI(BinaryReader reader) => ShouldDealContactDamage = reader.ReadBoolean();

        public override bool? CanCutTiles() => false;

        public override bool MinionContactDamage() => ShouldDealContactDamage;

        public override void AI()
        {
            if (!CheckActive(Owner))
                return;

            // Search for any nearby targets.
            Projectile.GetMinionTarget(Owner, 2500f, 300f, out bool foundTarget, out NPC target);
            if (foundTarget)
                Projectile.Cascade().SpecificNPCTypeToCheckOnHit = target.type;

            // AI methods.
            switch ((AttackState)AIState)
            {
                case AttackState.Idling:
                    DoBehavior_Idle(foundTarget);
                    break;

                case AttackState.UndeadSpiritTransformation:
                    DoBehavior_UndeadSpiritTransformation(foundTarget, target);
                    break;
            }

            Timer++;
            Projectile.AdjustProjectileHitboxByScale(24f, 40f);
        }

        public void DoBehavior_Idle(bool foundTarget)
        {
            int timeBeforeSwitchingAI = 60;
            ref float idleAngle = ref Projectile.Cascade().ExtraAI[IdleAngleIndex];

            // Get a list of all active Underworld Lanterns and sort them according to their 
            // minionPos field. Then, get the proper order of each minion and space them
            // out evenly in a elliptical shape above the specified idle position.
            Vector2 idlePosition = Owner.Top;
            List<Projectile> brotherMinions = new List<Projectile>();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile brotherProj = Main.projectile[i];
                if (brotherProj.active && brotherProj.type == Projectile.type && brotherProj.owner == Projectile.owner)
                {
                    brotherMinions.Add(brotherProj);
                }
            }

            brotherMinions.Sort((x, y) => x.minionPos - y.minionPos);
            int minionCount = brotherMinions.Count;
            if (minionCount > 0)
            {
                int order = brotherMinions.IndexOf(Projectile);
                idleAngle = (TwoPi * order) / minionCount;
                idleAngle += TwoPi * Main.GlobalTimeWrappedHourly / 8f;
                idlePosition.X += 140f * Cos(idleAngle);
                idlePosition.Y += -125f - 75f * Sin(idleAngle) + Owner.gfxOffY;
            }

            Projectile.Center = Vector2.Lerp(Projectile.Center, idlePosition, 0.225f);
            Projectile.Opacity = Clamp(Projectile.Opacity + 0.1f, 0f, 1f);
            Projectile.scale = Clamp(Projectile.scale - 0.1f, 1f, 1.75f);

            // Switch to attack mode after some time.
            if (Timer >= timeBeforeSwitchingAI && foundTarget)
                SwitchAIStates(1);
        }

        public void DoBehavior_UndeadSpiritTransformation(bool foundTarget, NPC target)
        {
            int floatTime = 45;
            int chaseTime = 720;
            int returnTime = 60;
            int cooldownTime = 15;
            float maxChaseSpeed = 45f;
            float maxTurnResistance = 40f;

            // Immediately move into the return phase if there is nothing to target.
            if ((!foundTarget || target == null) && LocalAIState != 2f)
            {
                LocalAIState = 2f;
                Timer = 0f;
                Projectile.netUpdate = true;
            }

            // Initially float upwards and fade out.
            if (LocalAIState == 0f)
            {               
                if (Timer <= floatTime)
                {
                    // Float upwards.
                    if (Timer == 1f)
                        Projectile.velocity.Y -= 12f;

                    Projectile.Opacity = Clamp(Projectile.Opacity - 0.05f, 0f, 1f);

                    if (Timer == floatTime)
                    {
                        LocalAIState = 1f;
                        Timer = 0f;
                        Projectile.netUpdate = true;
                    }
                }
            }

            // Emerge from the player's body and dash towards the enemy.
            if (LocalAIState == 1f)
            {
                if (Timer <= chaseTime)
                {
                    if (Timer == 0f)
                        Projectile.Center = Owner.Center + Vector2.UnitX.RotatedByRandom(TwoPi) * 60f;

                    // Fade back in and chase the enemy.
                    Projectile.Opacity = Clamp(Projectile.Opacity + 0.1f, 0f, 1f);
                    Projectile.scale = Clamp(Projectile.scale + 0.1f, 0f, 1.75f);
                    Projectile.SimpleMove(target.Center, maxChaseSpeed, maxTurnResistance);

                    // Adjust the hitbox to accommodate for the new sprite.
                    Projectile.AdjustProjectileHitboxByScale(42f, 50f);

                    // Switch if time is up or the Lantern has hit the target.
                    if (Timer == chaseTime || Projectile.Cascade().HasStruckSpecificNPC)
                    {
                        LocalAIState = 2f;
                        Timer = 0f;
                        Projectile.netUpdate = true;
                    }
                }

                ShouldDealContactDamage = true;
            }

            // Target was hit, the minion lost its target or chase time was up; retreat to the player's center and reset.
            if (LocalAIState == 2f)
            {
                // Ease back to the player and fade out.
                if (Timer <= returnTime)
                {
                    Projectile.Opacity = Lerp(Projectile.Opacity, 0f, Utilities.SineEaseInOut(Timer / returnTime));
                    Projectile.scale = Lerp(Projectile.scale, 1f, Utilities.SineEaseInOut(Timer / returnTime));
                    Projectile.Center = Vector2.Lerp(Projectile.Center, Owner.Center, Utilities.SineEaseInOut(Timer / returnTime));
                }

                // Head back to usual idle AI.
                if (Timer >= returnTime + cooldownTime)
                {
                    SwitchAIStates(0);
                    // Velocity is reset here in order to not screw up the alignment of each lantern for the idle AI code.
                    Projectile.velocity *= 0f;
                }
            }
        }

        public void SwitchAIStates(int aiState)
        {
            AIState = aiState;
            Timer = 0f;
            LocalAIState = 0f;
            ShouldDealContactDamage = false;
            Projectile.netUpdate = true;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                Vector2 velocity = Vector2.UnitX.RotatedByRandom(TwoPi) * Main.rand.NextFloat(2f, 8f);
                Color color = Color.Lerp(Color.Cyan, Color.CornflowerBlue, Main.rand.NextFloat());
                float scale = Main.rand.NextFloat(0.25f, 1.25f);
                HeavySmokeParticle heavySmoke = new(Projectile.Center, velocity, color, Main.rand.Next(75, 140), scale, Main.rand.NextFloat(0.35f, 1f), 0.06f, true, 0);
                GeneralParticleHandler.SpawnParticle(heavySmoke);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D baseTexture = TextureAssets.Projectile[Type].Value;

            int frameHeight = baseTexture.Height / Main.projFrames[Type];
            int frameY = frameHeight * Projectile.frame;
            Rectangle projRec = new Rectangle(0, frameY, baseTexture.Width, frameHeight);

            Main.spriteBatch.SetBlendState(BlendState.Additive);
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
            {
                Vector2 drawPosition = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
                Color trailColor = Color.Cyan * 0.75f * ((float)(Projectile.oldPos.Length - i) / Projectile.oldPos.Length);
                Main.EntitySpriteDraw(baseTexture, drawPosition, projRec, Projectile.GetAlpha(trailColor), Projectile.rotation, projRec.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);

            return false;
        }

        private bool CheckActive(Player owner)
        {
            if (owner.HasBuff(ModContent.BuffType<MoonSpiritLanternBuff>()))
            {
                Projectile.timeLeft = 2;
                return true;
            }
            else
            {
                Projectile.Kill();
                return false;
            }
        }
    }
}
