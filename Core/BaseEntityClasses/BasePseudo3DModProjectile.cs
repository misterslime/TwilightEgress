using System.Collections.Generic;

namespace Cascade.Core.BaseEntityClasses
{
    public abstract class BasePseudo3DModProjectile : ModProjectile
    {
        /// <summary>
        /// Whether or not the projectile should get progressively darker when in the background.
        /// </summary>
        public virtual bool ShouldDarkenInBackground => false;

        public virtual void SafeSetDefaults() { }

        public virtual void SafeSendExtraData(BinaryWriter writer) { }

        public virtual void SafeReceiveExtraData(BinaryReader reader) { }

        public virtual void SafeDrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) { }

        public virtual bool SafePreAI() => true;

        public virtual bool SafePreDraw(Color lightColor) => true;

        public virtual Color? SafeGetAlpha(Color lightColor) => null;

        public float ZPosition
        {
            get; set;
        }

        public float ParallaxAmountX
        {
            get; set;
        }

        public float ParallaxAmountY
        {
            get; set;
        }

        public float AdjustedWidth
        {
            get; set;
        }

        public float AdjustedHeight
        {
            get; set;
        }

        public float zScale;

        public sealed override void SetDefaults()
        {
            SafeSetDefaults();

            // Ensures hitbox scaling works properly.
            AdjustedWidth = Projectile.width;
            AdjustedHeight = Projectile.height;
        }

        // Only deal damage if the projectile is close enough to the player's playing field.
        public sealed override bool? CanDamage() => ZPosition is <= 0.2f or >= -0.2f;

        public sealed override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(ZPosition); 
            writer.Write(ParallaxAmountX); 
            writer.Write(ParallaxAmountY);
            writer.Write(AdjustedWidth);
            writer.Write(AdjustedHeight);

            writer.Write(zScale);

            SafeSendExtraData(writer);
        }

        public sealed override void ReceiveExtraAI(BinaryReader reader)
        {
            ZPosition = reader.ReadSingle();
            ParallaxAmountX = reader.ReadSingle();
            ParallaxAmountY = reader.ReadSingle();
            AdjustedWidth = reader.ReadSingle();
            AdjustedHeight = reader.ReadSingle();

            zScale = reader.ReadSingle();

            SafeReceiveExtraData(reader);
        }

        public sealed override bool PreAI()
        {
            // Adjust the scale of the projectile based on it's z-position.
            Projectile.scale = 1f / (ZPosition + 1f);
            // Hide the projectile behind tiles when in the background.
            Projectile.hide = ZPosition >= 0.2f;

            // Resize the hitbox based on scale.
            int oldWidth = Projectile.width;
            int idealWidth = (int)(Projectile.scale * AdjustedWidth);
            int idealHeight = (int)(Projectile.scale * AdjustedHeight);
            if (idealWidth != oldWidth)
            {
                Projectile.position.X += Projectile.width / 2;
                Projectile.position.Y += Projectile.height / 2;
                Projectile.width = idealWidth;
                Projectile.height = idealHeight;
                Projectile.position.X -= Projectile.width / 2;
                Projectile.position.Y -= Projectile.height / 2;
            }

            return SafePreAI();
        }

        public sealed override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            if (Projectile.hide)
                behindNPCsAndTiles.Add(index);
            else if (ZPosition <= -0.25f)
                overPlayers.Add(index);

            SafeDrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
        }

        public sealed override Color? GetAlpha(Color lightColor)
        {
            if (ShouldDarkenInBackground)
                return Color.Lerp(lightColor, Color.Black, ZPosition / 1f);
            return SafeGetAlpha(lightColor);
        }

        public sealed override bool PreDraw(ref Color lightColor)
        {
            if (SafePreDraw(lightColor))
            {
                Texture2D projectileTexture = TextureAssets.Projectile[Type].Value;
                Rectangle projectileRectangle = projectileTexture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
                Vector2 drawPosition = Projectile.Center - Main.screenPosition + new Vector2(GetParallaxOffsetX(), GetParallaxOffsetY());
                Main.EntitySpriteDraw(projectileTexture, drawPosition, projectileRectangle, Projectile.GetAlpha(lightColor), Projectile.rotation, projectileRectangle.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            }
            else
                SafePreDraw(lightColor);

            return false;
        }

        public float GetParallaxOffsetX() => (Main.screenPosition.X + Main.screenWidth / 2f - Projectile.position.X) * ZPosition * ParallaxAmountX;

        public float GetParallaxOffsetY() => (Main.screenPosition.Y + Main.screenHeight / 2f - Projectile.position.Y) * ZPosition * ParallaxAmountY;

    }
}
