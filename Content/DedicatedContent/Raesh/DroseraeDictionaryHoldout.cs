using System.Collections.Generic;
using System.Xml;

namespace Cascade.Content.DedicatedContent.Raesh
{
    public class DroseraeDictionaryHoldout : ModProjectile
    {
        private class BloodRune
        {
            public Vector2 Position;

            public Vector2 Velocity;

            public float Scale;

            public float Opacity;

            public float Rotation;

            public float RotationSpeed;

            public float RotationDirction;

            public Color Color;

            private string[] Textures = new string[]
            {
                "CalamityMod/UI/CalamitasEnchantments/CurseIcon_Aflame",
                "CalamityMod/UI/CalamitasEnchantments/CurseIcon_Bloodthirsty",
                "CalamityMod/UI/CalamitasEnchantments/CurseIcon_Capricorn",
                "CalamityMod/UI/CalamitasEnchantments/CurseIcon_Ephemeral",
                "CalamityMod/UI/CalamitasEnchantments/CurseIcon_Eratic",
                "CalamityMod/UI/CalamitasEnchantments/CurseIcon_Evasive",
                "CalamityMod/UI/CalamitasEnchantments/CurseIcon_Evocative",
                "CalamityMod/UI/CalamitasEnchantments/CurseIcon_Hellbound",
                "CalamityMod/UI/CalamitasEnchantments/CurseIcon_Incisive",
                "CalamityMod/UI/CalamitasEnchantments/CurseIcon_Indigant",
                "CalamityMod/UI/CalamitasEnchantments/CurseIcon_Lecherous",
                "CalamityMod/UI/CalamitasEnchantments/CurseIcon_Oblatory",
                "CalamityMod/UI/CalamitasEnchantments/CurseIcon_Obstinate",
                "CalamityMod/UI/CalamitasEnchantments/CurseIcon_Persecuted",
                "CalamityMod/UI/CalamitasEnchantments/CurseIcon_Polluted",
                "CalamityMod/UI/CalamitasEnchantments/CurseIcon_Relentless",
                "CalamityMod/UI/CalamitasEnchantments/CurseIcon_Resentful",
                "CalamityMod/UI/CalamitasEnchantments/CurseIcon_Splintered",
                "CalamityMod/UI/CalamitasEnchantments/CurseIcon_Tainted",
                "CalamityMod/UI/CalamitasEnchantments/CurseIcon_Traitorous",
                "CalamityMod/UI/CalamitasEnchantments/CurseIcon_Vindictive",
                "CalamityMod/UI/CalamitasEnchantments/CurseIcon_Withered"
            };

            private Texture2D Texture;

            public BloodRune(Vector2 position, Vector2 velocity, Color color, float scale, float rotation, float rotationSpeed)
            {
                Position = position;
                Velocity = velocity;
                Color = color;
                Scale = scale;
                Rotation = rotation;
                RotationSpeed = rotationSpeed;
                RotationDirction = Main.rand.NextBool(-1).ToDirectionInt();
                Opacity = 0f;

                for (int i = 0; i < Textures.Length; i++)
                    Texture = ModContent.Request<Texture2D>(Textures[i]).Value;
            }

            public void Update()
            {
                Opacity = Clamp(Opacity + 0.01f, 0f, 1f);
                Rotation += RotationSpeed * RotationDirction;
            }

            public void Draw()
            {
                Rectangle rec = Texture.Frame();
                Vector2 origin = rec.Size() / 2f;
                Main.spriteBatch.Draw(Texture, Position - Main.screenPosition, rec, Color * Opacity, Rotation, origin, Scale, SpriteEffects.None, 0f);
            }
        }

        private Player Owner => Main.player[Projectile.owner];

        private ref float Timer => ref Projectile.ai[0];

        private const int MaxChargeTime = 60;

        private const int BackglowRotationIndex = 0;

        private const int BackglowOpacityIndex = 1;

        private bool ShouldDespawn => Owner.dead || Owner.CCed || !Owner.active || Owner.HeldItem.type != ModContent.ItemType<DroseraeDictionary>();

        private List<BloodRune> BloodRunes = new List<BloodRune>();

        public override string Texture => "Cascade/Content/DedicatedContent/Raesh/DroseraeDictionary";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            if (ShouldDespawn)
            {
                Projectile.Kill();
                return;
            }

            // Update all Blood Runes.
            foreach (BloodRune rune in BloodRunes)
            {
                rune.Position += rune.Velocity;
                rune.Update();
            }

            UpdatePlayerVariables();
        }

        public override void Kill(int timeLeft)
        {
            BloodRunes.Clear();
        }

        public void UpdatePlayerVariables()
        {
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.ChangeDir(Sign(Projectile.rotation.ToRotationVector2().X));
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - PiOver2);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawBloodRitualCircle();
            DrawBook(lightColor);

            foreach (BloodRune rune in  BloodRunes)
                rune.Draw();

            return false;
        }

        public void DrawBook(Color lightColor)
        {
            ref float backglowRotation = ref Projectile.Cascade().ExtraAI[BackglowRotationIndex];
            ref float backglowOpacity = ref Projectile.Cascade().ExtraAI[BackglowOpacityIndex];

            Texture2D texture = TextureAssets.Projectile[Type].Value;
            SpriteEffects effects = Owner.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float rotation = Projectile.rotation + (Owner.direction < 0 ? Pi : 0f);
            Vector2 drawPosition = Owner.MountedCenter + new Vector2(0f, 0f) + Projectile.rotation.ToRotationVector2() - Main.screenPosition;

            // Draw pulsing backglow effects.
            if (backglowOpacity > 0f)
            {
                for (int i = 0; i < 6; i++)
                {
                    backglowRotation += TwoPi / 300f;
                    float backglowRadius = Lerp(2f, 5f, SineInOutEasing((float)(Main.timeForVisualEffects / 30f), 1));
                    Vector2 backglowDrawPositon = drawPosition + Vector2.UnitY.RotatedBy(backglowRotation + TwoPi * i / 6) * backglowRadius;

                    Main.spriteBatch.SetBlendState(BlendState.Additive);
                    Main.EntitySpriteDraw(texture, backglowDrawPositon, texture.Frame(), Projectile.GetAlpha(Color.Crimson) * backglowOpacity, rotation, texture.Size() / 2f, Projectile.scale, effects, 0);
                    Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
                }
            }

            // Draw the main sprite.
            Main.EntitySpriteDraw(texture, drawPosition, texture.Frame(), Projectile.GetAlpha(lightColor), rotation, texture.Size() / 2f, Projectile.scale, effects, 0);
        }

        public void DrawBloodRitualCircle()
        {
            Texture2D mainCircleOuter = ModContent.Request<Texture2D>("Cascade/Content/DedicatedContent/Jacob/TankGodRitualCircle").Value;
            Texture2D mainCircleInner = ModContent.Request<Texture2D>("Cascade/Content/DedicatedContent/Jacob/TankGodRitualCircleInner").Value;
            
        }
    }
}
