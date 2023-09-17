using System.Drawing.Drawing2D;

namespace Cascade.Content.Particles
{
    public class RampartMaterialsPulledIn : Particle
    {
        private enum RampartMaterials
        {
            EssenceOfEleum,
            TurtleShell,
            PhilosophersStone,
            BandOfRegeneration,
            StarCloak,
            CrossNecklace,
            PrismShard,
            AstralOre,
            Stardust,
            FrozenTurtleShell,
            PaladinsShield,
            CharmOfMyths,
            StarVeil,
            SeaPrism,
            AstralBar,
            DarksunFragment,
            EndothermicEnergy,
            NightmareFuel,
            Phantoplasm,
            FrozenShield,
            CosmiliteBar,
            DivineGeode,
            DeificAmulet,
            AscendantSpiritEssence
        }

        private float Opacity;

        private float PullSpeed;

        private Vector2 PullTowardsPosition;

        private Asset<Texture2D> SpriteTexture;

        private int FrameCounter;

        private int Frame;

        private int MaxFrames;

        public RampartMaterialsPulledIn(Vector2 spawnPosition, Vector2 velocity, Color color, float opacity, float scale, int lifespan)
        {
            Position = spawnPosition;
            Velocity = velocity;
            Color = color;
            Opacity = opacity;
            Scale = scale;
            Lifetime = lifespan;
            Variant = Main.rand.Next(24);
            MaxFrames = 1;
        }

        public override string Texture => Utilities.EmptyPixelPath;

        public override bool SetLifetime => true;

        public override bool UseCustomDraw => true;

        public override void Update()
        {
            switch ((RampartMaterials)Variant)
            {
                case RampartMaterials.EssenceOfEleum:
                    SpriteTexture = ModContent.Request<Texture2D>("CalamityMod/Items/Materials/EssenceofEleum");
                    break;

                case RampartMaterials.TurtleShell:
                    SpriteTexture = TextureAssets.Item[ItemID.TurtleShell];
                    break;

                case RampartMaterials.PhilosophersStone:
                    SpriteTexture = TextureAssets.Item[ItemID.PhilosophersStone];
                    break;

                case RampartMaterials.BandOfRegeneration:
                    SpriteTexture = TextureAssets.Item[ItemID.BandofRegeneration];
                    break;

                case RampartMaterials.StarCloak:
                    SpriteTexture = TextureAssets.Item[ItemID.StarCloak];
                    break;

                case RampartMaterials.CrossNecklace:
                    SpriteTexture = TextureAssets.Item[ItemID.CrossNecklace];
                    break;

                case RampartMaterials.PrismShard:
                    SpriteTexture = ModContent.Request<Texture2D>("CalamityMod/Items/Placeables/PrismShard");
                    break;

                case RampartMaterials.AstralOre:
                    SpriteTexture = ModContent.Request<Texture2D>("CalamityMod/Items/Placeables/Ores/AstralOre");
                    break;

                case RampartMaterials.Stardust:
                    SpriteTexture = ModContent.Request<Texture2D>("CalamityMod/Items/Materials/Stardust");
                    break;

                case RampartMaterials.FrozenTurtleShell:
                    SpriteTexture = TextureAssets.Item[ItemID.FrozenTurtleShell];
                    break;

                case RampartMaterials.PaladinsShield:
                    SpriteTexture = TextureAssets.Item[ItemID.PaladinsShield];
                    break;

                case RampartMaterials.CharmOfMyths:
                    SpriteTexture = TextureAssets.Item[ItemID.CharmofMyths];
                    break;

                case RampartMaterials.StarVeil:
                    SpriteTexture = TextureAssets.Item[ItemID.StarVeil];
                    break;

                case RampartMaterials.SeaPrism:
                    SpriteTexture = ModContent.Request<Texture2D>("CalamityMod/Items/Placeables/SeaPrism");
                    break;

                case RampartMaterials.AstralBar:
                    SpriteTexture = ModContent.Request<Texture2D>("CalamityMod/Items/Placeables/AstralBar_Animated");
                    MaxFrames = 12;
                    break;

                case RampartMaterials.DarksunFragment:
                    SpriteTexture = ModContent.Request<Texture2D>("CalamityMod/Items/Materials/DarksunFragment");
                    MaxFrames = 8;
                    break;

                case RampartMaterials.EndothermicEnergy:
                    SpriteTexture = ModContent.Request<Texture2D>("CalamityMod/Items/Materials/EndothermicEnergy");
                    MaxFrames = 6;
                    break;

                case RampartMaterials.NightmareFuel:
                    SpriteTexture = ModContent.Request<Texture2D>("CalamityMod/Items/Materials/NightmareFuel");
                    MaxFrames = 6;
                    break;

                case RampartMaterials.Phantoplasm:
                    SpriteTexture = ModContent.Request<Texture2D>("CalamityMod/Items/Materials/Polterplasm");
                    MaxFrames = 5;
                    break;

                case RampartMaterials.FrozenShield:
                    SpriteTexture = TextureAssets.Item[ItemID.FrozenShield];
                    break;

                case RampartMaterials.CosmiliteBar:
                    SpriteTexture = ModContent.Request<Texture2D>("CalamityMod/Items/Materials/CosmiliteBar_Animated");
                    MaxFrames = 10;
                    break;

                case RampartMaterials.DivineGeode:
                    SpriteTexture = ModContent.Request<Texture2D>("CalamityMod/Items/Materials/DivineGeode");
                    break;

                case RampartMaterials.DeificAmulet:
                    SpriteTexture = ModContent.Request<Texture2D>("CalamityMod/Items/Accessories/DeificAmulet");
                    break;

                case RampartMaterials.AscendantSpiritEssence:
                    SpriteTexture = ModContent.Request<Texture2D>("CalamityMod/Items/Materials/AscendantSpiritEssence");
                    MaxFrames = 6;
                    break;
            }

            // Animate the sprites if necessary.
            if (MaxFrames > 1)
            {
                FrameCounter++;
                if (FrameCounter >= 3)
                {
                    FrameCounter = 0;
                    if (++Frame >= MaxFrames)
                    {
                        Frame = 0;
                    }
                }
            }

            Scale -= 0.03f;
            Opacity -= 0.03f;
            Rotation += Velocity.X * 0.03f;
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            Texture2D texture = SpriteTexture.Value;
            Rectangle spriteFrame = texture.Frame(1, MaxFrames, 0, Frame);
            Vector2 origin = spriteFrame.Size() / 2f;
            spriteBatch.Draw(texture, Position, spriteFrame, Color * Opacity, Rotation, origin, Scale, SpriteEffects.None, 0f);
        }
    }
}
