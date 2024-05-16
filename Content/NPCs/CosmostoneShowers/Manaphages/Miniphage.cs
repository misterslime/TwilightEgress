using Cascade.Core.BaseEntities.ModNPCs;

namespace Cascade.Content.NPCs.CosmostoneShowers.Manaphages
{
    public class Miniphage : BasePhage
    {
        public override void SetPhageDefaults()
        {
            NPC.width = 22;
            NPC.height = 26;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.lifeMax = 20;
            NPC.value = 0f;
        }

        #region Drawing and Animation
        public void UpdateAnimationFrames(ManaphageAnimation manaphageAnimation, float frameSpeed, int? specificYFrame = null)
        {
            int frameX = manaphageAnimation switch
            {
                ManaphageAnimation.Inject => 1,
                ManaphageAnimation.Suck => 2,
                _ => 0
            };

            FrameX = frameX;
            FrameY = specificYFrame ?? (int)Math.Floor(Timer / frameSpeed) % Main.npcFrameCount[Type];
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            DrawManaTank();
            DrawMainSprite(drawColor);
            return false;
        }

        public void DrawMainSprite(Color drawColor)
        {
            ref float spriteStretchX = ref NPC.Cascade().ExtraAI[SpriteStretchXIndex];
            ref float spriteStretchY = ref NPC.Cascade().ExtraAI[SpriteStretchYIndex];

            Texture2D texture = TextureAssets.Npc[Type].Value;
            Vector2 drawPosition = NPC.Center - Main.screenPosition;
            Vector2 stretchFactor = new(spriteStretchX, spriteStretchY);

            Rectangle rectangle = texture.Frame(3, Main.npcFrameCount[Type], FrameX, FrameY);
            Main.EntitySpriteDraw(texture, drawPosition, rectangle, NPC.GetAlpha(drawColor), NPC.rotation, rectangle.Size() / 2f, NPC.scale * stretchFactor, 0);
        }

        public void DrawManaTank()
        {
            ref float spriteStretchX = ref NPC.Cascade().ExtraAI[SpriteStretchXIndex];
            ref float spriteStretchY = ref NPC.Cascade().ExtraAI[SpriteStretchYIndex];
            ref float manaTankShaderTime = ref NPC.Cascade().ExtraAI[ManaTankShaderTimeIndex];

            Texture2D miniphageTank = ModContent.Request<Texture2D>("Cascade/Content/NPCs/CosmostoneShowers/Manaphages/Miniphage_Tank").Value;
            Texture2D miniphageTankMask = ModContent.Request<Texture2D>("Cascade/Content/NPCs/CosmostoneShowers/Manaphages/Miniphage_Tank_Mask").Value;

            Vector2 stretchFactor = new(spriteStretchX, spriteStretchY);
            Vector2 origin = miniphageTank.Size() / 2f;
            Vector2 drawPosition = NPC.Center - Main.screenPosition - Vector2.UnitY.RotatedBy(NPC.rotation) * 21f * spriteStretchY;

            float manaCapacityInterpolant = Utils.GetLerpValue(1f, 0f, CurrentManaCapacity / MaximumManaCapacity, true);

            Main.spriteBatch.PrepareForShaders();
            ShaderManager.TryGetShader("Cascade.ManaphageTankShader", out ManagedShader manaTankShader);
            manaTankShader.TrySetParameter("time", Main.GlobalTimeWrappedHourly * manaTankShaderTime);
            manaTankShader.TrySetParameter("manaCapacity", manaCapacityInterpolant);
            manaTankShader.TrySetParameter("pixelationFactor", 0.075f);
            manaTankShader.SetTexture(CascadeTextureRegistry.BlueCosmicGalaxy, 1, SamplerState.AnisotropicWrap);
            manaTankShader.SetTexture(CascadeTextureRegistry.SmudgyNoise, 2, SamplerState.AnisotropicWrap);
            manaTankShader.Apply();

            // Draw the tank mask with the shader applied to it.
            Main.EntitySpriteDraw(miniphageTankMask, drawPosition, null, Color.Black, NPC.rotation, origin, NPC.scale * 0.98f, 0);
            Main.spriteBatch.ExitShaderRegion();

            // Draw the tank itself.
            Main.EntitySpriteDraw(miniphageTank, drawPosition, null, Color.White * NPC.Opacity, NPC.rotation, origin, NPC.scale, 0);
        }
        #endregion
    }
}
