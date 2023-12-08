namespace Cascade.Content.DedicatedContent.Raesh
{
    public class FlytrapMaw : ModProjectile, ILocalizedModType
    {
        private ref float ViableTargetIndex => ref Projectile.ai[0];

        private Player Owner => Main.player[Projectile.owner];

        private List<NPC> NPCsWhoHaveBeenHit { get; set; }

        private PrimitiveDrawer TrailDrawer { get; set; }

        public new string LocalizationCategory => "Projectiles.Magic";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
            ProjectileID.Sets.TrailCacheLength[Type] = 24;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 28;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 30;
            Projectile.Opacity = 0f;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
        }

        public override void OnSpawn(IEntitySource source)
        {
            // Scale up depending on the player's current life.
            NPCsWhoHaveBeenHit = new();
            float scaleFactor = Lerp(1f, 1.75f, Utils.GetLerpValue(Owner.statLifeMax, 100f, Owner.statLife, true));
            Projectile.scale = 1f * scaleFactor;
        }

        public override void AI()
        {
            NPC viableTarget = Main.npc[(int)ViableTargetIndex];
            if (viableTarget == null)
            {
                Projectile.Kill();
                return;
            }

            Projectile.Opacity = Clamp(Projectile.Opacity + 0.05f, 0f, 1f);
            Projectile.rotation = Projectile.velocity.ToRotation() + Pi;
            Projectile.UpdateProjectileAnimationFrames(0, 4, 5);
            Projectile.AdjustProjectileHitboxByScale(28f, 28f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!NPCsWhoHaveBeenHit.Contains(target))
            {
                /* Add the whoAmI indexes of each hit NPC to the list. 
                 * This is how we'll keep track of which NPCs we shouldn't target. */
                NPCsWhoHaveBeenHit.Add(target);
                SoundEngine.PlaySound(CascadeSoundRegistry.FlytrapMawBounce with { MaxInstances = 1 }, Projectile.Center);
            }

            // Find the closest target in range and bounce to them from the last enemy.
            // If there are no targets, carry on as usual.
            NPC viableBounceTarget = Projectile.FindTargetWithinRange(1000f);
            if (viableBounceTarget == null)
                return;

            if (viableBounceTarget.CanBeChasedBy() && !NPCsWhoHaveBeenHit.Contains(viableBounceTarget))
            {
                Projectile.velocity = Projectile.SafeDirectionTo(viableBounceTarget.Center) * 35f;
                ViableTargetIndex = viableBounceTarget.whoAmI;
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath1);

            int dustCount = 15 * (int)Lerp(1f, 2f, Utils.GetLerpValue(Owner.statLifeMax, 100f, Owner.statLife, true));
            float speed = Lerp(5f, 10f, Utils.GetLerpValue(Owner.statLifeMax, 100f, Owner.statLife, true));
            float scale = Main.rand.NextFloat(0.65f, 1.25f) * Projectile.scale;
            Utilities.CreateRandomizedDustExplosion(dustCount, Projectile.Center, DustID.Plantera_Green, speed, dustScale: scale);

            for (int i = 0; i < 2; i++)
            {
                int goreType = 388 + i;
                Vector2 goreSpawnPosition = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height);
                Gore.NewGore(Projectile.GetSource_Death(), goreSpawnPosition, Projectile.velocity * 0.1f, goreType, Projectile.scale);
            }    
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
            {
                Projectile.Kill();
                return true;
            }
            else
            {
                Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
                SoundEngine.PlaySound(CascadeSoundRegistry.FlytrapMawBounce with { MaxInstances = 1 }, Projectile.Center);

                if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
                    Projectile.velocity.X = -oldVelocity.X;

                if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
                    Projectile.velocity.Y = -oldVelocity.Y;
            }

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawPrims();
            Projectile.DrawTextureOnProjectile(Color.White, Projectile.rotation, Projectile.scale, animated: true);
            return false;
        }

        public float SetTrailWidth(float completionRatio) => 12f * Utils.GetLerpValue(1f, 0f, completionRatio, true) * Projectile.scale * Projectile.Opacity;

        public Color SetTrailColor(float completionRatio) => Color.White * Projectile.Opacity;

        public void DrawPrims()
        {
            Asset<Texture2D> trailTexture = ModContent.Request<Texture2D>("Cascade/Content/DedicatedContent/Raesh/FlytrapMaw_Chain");
            TrailDrawer ??= new PrimitiveDrawer(SetTrailWidth, SetTrailColor, true, GameShaders.Misc["CalamityMod:PrimitiveTexture"]);

            Main.spriteBatch.EnterShaderRegion();
            GameShaders.Misc["CalamityMod:PrimitiveTexture"].SetShaderTexture(trailTexture, 1);
            GameShaders.Misc["CalamityMod:PrimitiveTexture"].Shader.Parameters["uPrimitiveSize"].SetValue(600f);
            GameShaders.Misc["CalamityMod:PrimitiveTexture"].Apply();

            TrailDrawer.DrawPrimitives(Projectile.oldPos.ToList(), Projectile.Size * 0.5f - Main.screenPosition, 85);
            Main.spriteBatch.ExitShaderRegion();
        }
    }
}
