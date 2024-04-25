using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cascade.Core.BaseEntities.ModNPCs;

namespace Cascade.Content.NPCs.CosmostoneShowers.Asteroids
{
    public class CometstoneAsteroidLarge : BaseAsteroid, ILocalizedModType
    {
        public new string LocalizationCategory => "NPCs.CosmostoneShowers";

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 2;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            NPCID.Sets.CannotDropSouls[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 120;
            NPC.height = 120;
            NPC.damage = 0;
            NPC.defense = 20;
            NPC.lifeMax = 500;
            NPC.aiStyle = -1;
            NPC.dontCountMe = true;
            NPC.lavaImmune = true;
            NPC.noTileCollide = false;
            NPC.noGravity = true;
            NPC.dontTakeDamageFromHostiles = true;
            NPC.chaseable = false;
            NPC.knockBackResist = 0.4f;
            NPC.Opacity = 0f;

            NPC.HitSound = SoundID.Tink;
            NPC.DeathSound = SoundID.Item70;
        }

        public override void SafeOnSpawn(IEntitySource source)
        {
            // Slower rotation
            RotationSpeedSpawnFactor = Main.rand.NextFloat(300f, 960f) * Utils.SelectRandom(Main.rand, -1, 1);

            // Initialize a bunch of fields.
            NPC.rotation = Main.rand.NextFloat(TwoPi);
            NPC.scale = Main.rand.NextFloat(1f, 2f);
            NPC.spriteDirection = Main.rand.NextBool().ToDirectionInt();
            NPC.frame.Y = Main.rand.Next(0, 2) * 138;
            NPC.netUpdate = true;
        }

        public override void SafeAI()
        {
            // Collision detection.
            List<NPC> activeAsteroids = Main.npc.Take(Main.maxNPCs).Where((NPC npc) => npc.active && npc.whoAmI != NPC.whoAmI && AsteroidUtil.ViableCollisionTypes.Contains(npc.type)).ToList();
            int count = activeAsteroids.Count;

            if (count > 0)
            {
                // Bounce off of other nearby asteroids.
                foreach (NPC asteroid in activeAsteroids)
                {
                    if (NPC.Hitbox.Intersects(asteroid.Hitbox))
                    {
                        NPC.velocity = -NPC.DirectionTo(asteroid.Center) * (1f + NPC.velocity.Length() + asteroid.scale) * 0.15f;
                        asteroid.velocity = -asteroid.DirectionTo(NPC.Center) * (1f + NPC.velocity.Length() + NPC.scale) * 0.15f;
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Vector2 drawPosition = NPC.Center - Main.screenPosition;
            Vector2 origin = NPC.frame.Size() / 2f;

            Main.EntitySpriteDraw(texture, drawPosition, NPC.frame, drawColor, NPC.rotation, origin, NPC.scale, SpriteEffects.None);
            return false;
        }
    }
}
