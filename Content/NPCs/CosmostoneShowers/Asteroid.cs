using System.Collections.Generic;

namespace Cascade.Content.NPCs.CosmostoneShowers
{
    public class Asteroid : ModNPC, ILocalizedModType
    {
        private PrimitiveTrail TrailDrawer = null;

        public ref float RotationSpeedSpawnFactor => ref NPC.Cascade().ExtraAI[0];

        public new string LocalizationCategory => "NPCs.CosmostoneShowers";

        public override string Texture => "Cascade/Content/Projectiles/Ambient/Comet";

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 3;
            NPCID.Sets.TrailCacheLength[Type] = 12;
            NPCID.Sets.TrailingMode[Type] = 1;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            NPCID.Sets.CannotDropSouls[Type] = true;
            NPCID.Sets.CountsAsCritter[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 36;
            NPC.height = 36;
            NPC.damage = 0;
            NPC.defense = 40;
            NPC.lifeMax = 150;
            NPC.aiStyle = -1;
            NPC.dontCountMe = true;
            NPC.lavaImmune = true;
            NPC.noTileCollide = false;
            NPC.noGravity = true;
            NPC.dontTakeDamageFromHostiles = true;
            NPC.chaseable = false;
            NPC.knockBackResist = 0.5f;
            NPC.Opacity = 0f;

            NPC.HitSound = SoundID.Tink;
            NPC.DeathSound = SoundID.Item70;
        }

        public override void OnSpawn(IEntitySource source)
        {
            // Initialize a bunch of fields.
            RotationSpeedSpawnFactor = Main.rand.NextFloat(-2160f, 2160f);
            NPC.rotation = Main.rand.NextFloat(TwoPi);
            NPC.scale = Main.rand.NextFloat(1f, 2f);
            NPC.spriteDirection = Main.rand.NextBool().ToDirectionInt();
            NPC.frame.Y = Main.rand.Next(0, 3) * 38;
            NPC.netUpdate = true;
        }

        public override void AI()
        {
            NPC.TargetClosest();
            // Fade in.
            NPC.Opacity = Clamp(NPC.Opacity + 0.02f, 0f, 1f);

            // Idly rotate.
            NPC.rotation += Pi / RotationSpeedSpawnFactor;
            NPC.rotation += NPC.velocity.X * 0.03f;

            // If a comet falls within a certain distance of Terraria's mesosphere, it
            // begins to be pulled in by the planet's gravity.

            // In simple terms, the Y-velocity is increased once the comet is pushed low enough.
            if (NPC.Bottom.Y >= Main.maxTilesY + 135f)
            {
                // Increase damage as Y-velocity begins to increase.
                NPC.damage = 150 * (int)Utils.GetLerpValue(0f, 1f, NPC.velocity.Y / 12f, true);
                NPC.velocity.Y = Clamp(NPC.velocity.Y + 0.03f, 0f, 18f);

                // Die upon tile collision, explode into Cometstone.
                if (Collision.SolidCollision(NPC.Center, NPC.width, NPC.height))
                {
                    NPC.life = 0;
                    NPC.checkDead();
                    // Calling all the dust stuff here as well cause HitEffect is dumb.
                    for (int i = 0; i < 45; i++)
                    {
                        Vector2 speed = Utils.RandomVector2(Main.rand, -1f, 1f) * NPC.velocity.Y;
                        Dust d = Dust.NewDustPerfect(NPC.Bottom, DustID.BlueTorch, speed * 0.85f);
                        d.noGravity = true;
                        d.scale = Main.rand.NextFloat(1f, 2f);
                    }

                    for (int i = 0; i < 12; i++)
                    {
                        Vector2 velocity = Vector2.UnitX.RotatedByRandom(TwoPi) * Main.rand.NextFloat(0.6f, 0.85f) * NPC.velocity.Y;
                        Color initialColor = Color.Lerp(Color.DarkGray, Color.Cyan, Main.rand.NextFloat());
                        Color fadeColor = Color.SaddleBrown;
                        float scale = Main.rand.NextFloat(0.85f, 1.75f) * NPC.scale;
                        float opacity = Main.rand.NextFloat(180f, 240f);
                        MediumMistParticle deathSmoke = new MediumMistParticle(NPC.Bottom, velocity, initialColor, fadeColor, scale, opacity, 0.03f);
                        Utilities.SpawnParticleBetter(deathSmoke);
                    }
                    NPC.netUpdate = true;
                    return;
                }
            }
            else
            {
                // Kill the comet's damage if it is simply floating around.
                NPC.damage = 0;

                // Always decrease velocity so that they don't drift off into No Man's Land.
                NPC.velocity *= 0.99f;
            }

            NPC.ShowNameOnHover = false;

            // Collision detection.
            List<NPC> activeAsteroids = new List<NPC>();
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && npc.type == Type && npc.whoAmI != NPC.whoAmI)
                    activeAsteroids.Add(npc);
            }

            // Bounce off of other nearby asteroids.
            foreach (NPC asteroid in activeAsteroids)
            {
                if (NPC.Hitbox.Intersects(asteroid.Hitbox))
                {
                    NPC.velocity = -NPC.DirectionTo(asteroid.Center) * (1f + NPC.velocity.Length() + asteroid.scale) * 0.45f;
                    asteroid.velocity = -asteroid.DirectionTo(NPC.Center) * (1f + NPC.velocity.Length() + NPC.scale) * 0.45f;
                }
            }


            // Resize the hitbox based on scale.
            int oldWidth = NPC.width;
            int idealWidth = (int)(NPC.scale * 36f);
            int idealHeight = (int)(NPC.scale * 36f);
            if (idealWidth != oldWidth)
            {
                NPC.position.X += NPC.width / 2;
                NPC.position.Y += NPC.height / 2;
                NPC.width = idealWidth;
                NPC.height = idealHeight;
                NPC.position.X -= NPC.width / 2;
                NPC.position.Y -= NPC.height / 2;
            }

            Lighting.AddLight(NPC.Center, Color.SkyBlue.ToVector3());
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 15; i++)
                {
                    Vector2 speed = Utils.RandomVector2(Main.rand, -1f, 1f);
                    Dust d = Dust.NewDustPerfect(NPC.Center, DustID.BlueFairy, speed * 5f * hit.HitDirection);
                    d.noGravity = true;
                    d.scale = Main.rand.NextFloat(1f, 2f);

                    Dust d2 = Dust.NewDustPerfect(NPC.Center, DustID.TintableDust, speed * 5f * hit.HitDirection);
                    d2.color = Color.Lerp(Color.SlateGray, Color.DarkGray, Main.rand.NextFloat());
                    d2.scale = Main.rand.NextFloat(1f, 2f);
                }

                for (int i = 0; i < 12; i++)
                {
                    Vector2 velocity = Vector2.UnitX.RotatedByRandom(TwoPi) * Main.rand.NextFloat(3f, 7f) * hit.HitDirection;
                    Color initialColor = Color.Lerp(Color.DarkGray, Color.Cyan, Main.rand.NextFloat());
                    Color fadeColor = Color.SaddleBrown;
                    float scale = Main.rand.NextFloat(0.85f, 1.75f) * NPC.scale;
                    float opacity = Main.rand.NextFloat(180f, 240f);
                    MediumMistParticle deathSmoke = new MediumMistParticle(NPC.Center, velocity, initialColor, fadeColor, scale, opacity, Main.rand.NextFloat(0.1f, 0.4f));
                    Utilities.SpawnParticleBetter(deathSmoke);
                }
            }
            else
            {
                for (int i = 0; i < 7; i++)
                {
                    Vector2 speed = Utils.RandomVector2(Main.rand, -1f, 1f);
                    Dust d = Dust.NewDustPerfect(NPC.Center, DustID.BlueTorch, speed * 5f * hit.HitDirection);
                    d.noGravity = true;
                    d.scale = Main.rand.NextFloat(1f, 2f);


                    Dust d2 = Dust.NewDustPerfect(NPC.Center, DustID.TintableDust, speed * 5f * hit.HitDirection);
                    d2.color = Color.Lerp(Color.SlateGray, Color.DarkGray, Main.rand.NextFloat());
                    d2.scale = Main.rand.NextFloat(1f, 2f);
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {

        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            DrawTrail();
            DrawAsteroid();
            return false;
        }

        public void DrawAsteroid()
        {
            Texture2D texture = TextureAssets.Npc[Type].Value;
            Vector2 drawPosition = NPC.Center - Main.screenPosition;
            Vector2 origin = NPC.frame.Size() / 2f;

            // Backglow effects.
            Main.spriteBatch.SetBlendState(BlendState.Additive);
            for (int i = 0; i < 4; i++)
            {
                float spinAngle = Main.GlobalTimeWrappedHourly * 0.35f;
                Vector2 backglowDrawPosition = drawPosition + Vector2.UnitY.RotatedBy(spinAngle + TwoPi * i / 4) * 5f;
                Main.EntitySpriteDraw(texture, backglowDrawPosition, NPC.frame, NPC.GetAlpha(Color.Cyan), NPC.rotation, origin, NPC.scale, SpriteEffects.None);
            }
            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);

            Main.EntitySpriteDraw(texture, drawPosition, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, origin, NPC.scale, SpriteEffects.None);
        }

        public float SetTrailWidth(float completionRatio)
        {
            return 20f * Utils.GetLerpValue(0.75f, 0f, completionRatio, true) * NPC.scale * NPC.Opacity;
        }

        public Color SetTrailColor(float completionRatio)
        {
            return Color.Lerp(Color.SkyBlue, Color.DeepSkyBlue, completionRatio) * NPC.Opacity;
        }

        public void DrawTrail()
        {
            TrailDrawer ??= new PrimitiveTrail(SetTrailWidth, SetTrailColor, null, GameShaders.Misc["CalamityMod:ArtemisLaser"]);

            Main.spriteBatch.EnterShaderRegion();
            GameShaders.Misc["CalamityMod:ArtemisLaser"].UseImage1("Images/Extra_189");
            GameShaders.Misc["CalamityMod:ArtemisLaser"].UseImage2("Images/Misc/Perlin");
            TrailDrawer.Draw(NPC.oldPos.ToArray(), NPC.Size * 0.5f - Main.screenPosition, 85);
            Main.spriteBatch.ExitShaderRegion();
        }
    }
}
