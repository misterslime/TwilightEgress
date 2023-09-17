using CalamityMod.Projectiles.Magic;
using System.Collections.Generic;

namespace Cascade.Content.NPCs.CosmostoneShowers
{
    public class DwarfJellyfish : ModNPC
    {
        private ref float Timer => ref NPC.ai[0];

        private ref float NaturalDirectionSwitchInterval => ref NPC.ai[1];

        private ref float SpeedMultiplier => ref NPC.ai[2];

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 4;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            NPCID.Sets.CannotDropSouls[Type] = true;
            NPCID.Sets.CountsAsCritter[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 26;
            NPC.height = 26;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.lifeMax = 10;
            NPC.aiStyle = -1;
            NPC.dontCountMe = true;
            NPC.lavaImmune = true;
            NPC.noTileCollide = false;
            NPC.noGravity = true;
            NPC.knockBackResist = 1f;
            NPC.Opacity = 0f;
            NPC.dontTakeDamage = true;
            NPC.chaseable = false;
            NPC.HitSound = SoundID.NPCHit25;
            NPC.DeathSound = SoundID.NPCDeath28;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SpeedMultiplier = Main.rand.NextFloat(1f, 2.25f);
            NaturalDirectionSwitchInterval = Main.rand.NextFloat(600f, 1200f);
            NPC.velocity = Main.rand.NextVector2Circular(Main.rand.NextBool().ToDirectionInt(), Main.rand.NextBool().ToDirectionInt());
            NPC.scale = Main.rand.NextFloat(0.15f, 0.95f);
        }

        public override void AI()
        {
            NPC.TargetClosest();
            // Move in a random direction every few seconds.
            if (Timer % NaturalDirectionSwitchInterval == 0f)
                NPC.velocity = Main.rand.NextVector2Circular(Main.rand.NextBool().ToDirectionInt(), Main.rand.NextBool().ToDirectionInt()) * SpeedMultiplier;

            // Find the nearest tile to the NPC.
            int tileCoordsX = (int)(NPC.Center.X / 16f);
            int tileCoordsY = (int)((NPC.position.Y + NPC.height) / 16f);
            Tile nearestTile = Main.tile[tileCoordsX, tileCoordsY];
            
            // Switch directions when colliding with tiles.
            if (nearestTile.TopSlope)
            {
                NPC.direction = nearestTile.LeftSlope ? -1 : 1;
                NPC.velocity.X = Abs(NPC.velocity.X) * (nearestTile.LeftSlope ? -1f : 1f) * SpeedMultiplier;
            }

            if (NPC.collideX)
            {
                NPC.direction *= -1;
                NPC.velocity.X *= -1f;
            }

            if (NPC.collideY)
            {
                NPC.directionY = NPC.velocity.Y > 0f ? -1 : 1;
                NPC.velocity.Y = Abs(NPC.velocity.Y) * (NPC.velocity.Y > 0f ? -1f : 1f) * SpeedMultiplier;
            }       

            Timer++;
            NPC.rotation = NPC.velocity.ToRotation() + 1.57f;
            NPC.Opacity = Clamp(NPC.Opacity + 0.03f, 0f, 1f);
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.06f + NPC.velocity.Length() / 10f; 
            NPC.frameCounter %= Main.npcFrameCount[Type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D mainTexture = TextureAssets.Npc[Type].Value;
            Texture2D bloomTexture = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle").Value;

            Vector2 mainOrigin = NPC.frame.Size() / 2f;
            Vector2 bloomOrigin = bloomTexture.Size() / 2f;

            spriteBatch.SetBlendState(BlendState.Additive);
            Main.EntitySpriteDraw(bloomTexture, NPC.Center - screenPos, null, NPC.GetAlpha(GetCosmosColors()) * 0.45f, 0f, bloomOrigin, NPC.scale * 1.035f, SpriteEffects.None);
            spriteBatch.SetBlendState(BlendState.AlphaBlend);

            Main.EntitySpriteDraw(mainTexture, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(GetCosmosColors()), NPC.rotation, mainOrigin, NPC.scale, SpriteEffects.None);

            return false;
        }

        private Color GetCosmosColors()
        {
            Color firstColor = Utils.SelectRandom(Main.rand, Color.SkyBlue, Color.AliceBlue);
            Color secondColor = Utils.SelectRandom(Main.rand, Color.Cyan, Color.CornflowerBlue);
            return Color.Lerp(firstColor, secondColor, Main.rand.NextFloat(0.1f, 1f));
        }
    }
}
