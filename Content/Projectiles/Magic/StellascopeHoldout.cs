namespace Cascade.Content.Projectiles.Magic
{
    public class StellascopeHoldout : ModProjectile
    {
        private static Asset<Texture2D> HoldoutTexture;
        private Player Owner => Main.player[Projectile.owner];

        public override void Load() => HoldoutTexture = ModContent.Request<Texture2D>("Cascade/Content/Projectiles/Magic/StellascopeHoldout");
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }
        public override void AI()
        {
            Projectile.velocity = Vector2.Normalize(Main.MouseWorld - Owner.MountedCenter);
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.direction == 1 ? 0f : Pi);
            Projectile.Center = Owner.Center;
            UpdatePlayerVariables();

            if (CalamityUtils.CantUseHoldout(Owner))
                Projectile.Kill();
        }
        private void UpdatePlayerVariables()
        {
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, 0 - Pi / 1.5f * Projectile.direction);
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
            Owner.ChangeDir(Projectile.direction);
            Owner.heldProj = Projectile.whoAmI;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            drawPosition = new Vector2(drawPosition.X + 10 * Projectile.direction, drawPosition.Y - 8);
            Rectangle source = new Rectangle(0, 0, HoldoutTexture.Width(), HoldoutTexture.Height());
            Vector2 rotationOrigin = HoldoutTexture.Size() * 0.5f;
            rotationOrigin.X -= 20 * Projectile.direction;
            SpriteEffects spriteFlip = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(HoldoutTexture.Value, drawPosition, source, lightColor, Projectile.rotation, rotationOrigin, 1f, spriteFlip);
            return false;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => false;
    }
}
