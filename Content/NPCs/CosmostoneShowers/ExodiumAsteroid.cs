namespace Cascade.Content.NPCs.CosmostoneShowers
{
    public class ExodiumAsteroid : BaseAsteroid, ILocalizedModType
    {
        private List<int> ViableCollisionTypes = new List<int>()
        {
            ModContent.NPCType<CosmostoneAsteroid>(),
            ModContent.NPCType<ExodiumAsteroid>()
        };

        public new string LocalizationCategory => "NPCs.CosmostoneShowers";

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

        public override void SafeOnSpawn(IEntitySource source)
        {
            // Initialize a bunch of fields.
            NPC.rotation = Main.rand.NextFloat(TwoPi);
            NPC.scale = Main.rand.NextFloat(1f, 2f);
            NPC.spriteDirection = Main.rand.NextBool().ToDirectionInt();
            NPC.frame.Y = Main.rand.Next(0, 3) * 38;
            NPC.netUpdate = true;
        }

        public override void OnMeteorCrashKill()
        {
            for (int i = 0; i < 15; i++)
            {
                Vector2 velocity = Vector2.UnitX.RotatedByRandom(TwoPi) * Main.rand.NextFloat(0.6f, 0.85f) * NPC.velocity.Y;
                Color initialColor = Color.Lerp(Color.DarkGray, Color.SlateGray, Main.rand.NextFloat());
                Color fadeColor = Color.SaddleBrown;
                float scale = Main.rand.NextFloat(0.85f, 1.75f) * NPC.scale;
                float opacity = Main.rand.NextFloat(0.6f, 1f);
                MediumMistParticle deathSmoke = new(NPC.Bottom, velocity, initialColor, fadeColor, scale, opacity, Main.rand.Next(180, 240), 0.03f);
                deathSmoke.SpawnCasParticle();
            }
        }

        public override void SafeAI()
        {
            // Collision detection.
            List<NPC> activeAsteroids = Main.npc.Take(Main.maxNPCs).Where((NPC npc) => npc.active && npc.whoAmI != NPC.whoAmI && ViableCollisionTypes.Contains(npc.type)).ToList();
            int count = activeAsteroids.Count;

            if (count > 0)
            {
                // Bounce off of other nearby asteroids.
                foreach (NPC asteroid in activeAsteroids)
                {
                    if (NPC.Hitbox.Intersects(asteroid.Hitbox))
                    {
                        NPC.velocity = -NPC.DirectionTo(asteroid.Center) * (1f + NPC.velocity.Length() + asteroid.scale) * 0.45f;
                        asteroid.velocity = -asteroid.DirectionTo(NPC.Center) * (1f + NPC.velocity.Length() + NPC.scale) * 0.45f;
                    }
                }
            }

            Lighting.AddLight(NPC.Center, Color.DarkSlateBlue.ToVector3());
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 15; i++)
                {
                    Vector2 speed = Utils.RandomVector2(Main.rand, -1f, 1f);
                    Dust d2 = Dust.NewDustPerfect(NPC.Center, DustID.TintableDust, speed * 5f * hit.HitDirection);
                    d2.color = Color.Lerp(Color.SlateGray, Color.DarkGray, Main.rand.NextFloat());
                    d2.scale = Main.rand.NextFloat(1f, 2f);
                }

                for (int i = 0; i < 12; i++)
                {
                    Vector2 velocity = Vector2.UnitX.RotatedByRandom(TwoPi) * Main.rand.NextFloat(3f, 7f) * hit.HitDirection;
                    Color initialColor = Color.Lerp(Color.DarkGray, Color.SlateGray, Main.rand.NextFloat());
                    Color fadeColor = Color.SaddleBrown;
                    float scale = Main.rand.NextFloat(0.85f, 1.75f) * NPC.scale;
                    float opacity = Main.rand.NextFloat(0.6f, 1f);
                    MediumMistParticle deathSmoke = new(NPC.Center, velocity, initialColor, fadeColor, scale, opacity, Main.rand.Next(180, 240), Main.rand.NextFloat(0.1f, 0.4f));
                    deathSmoke.SpawnCasParticle();
                }
            }
            else
            {
                for (int i = 0; i < 7; i++)
                {
                    Vector2 speed = Utils.RandomVector2(Main.rand, -1f, 1f);
                    Dust d2 = Dust.NewDustPerfect(NPC.Center, DustID.TintableDust, speed * 5f * hit.HitDirection);
                    d2.color = Color.Lerp(Color.SlateGray, Color.DarkGray, Main.rand.NextFloat());
                    d2.scale = Main.rand.NextFloat(1f, 2f);
                }
            }
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
            Main.spriteBatch.UseBlendState(BlendState.Additive);
            for (int i = 0; i < 4; i++)
            {
                float spinAngle = Main.GlobalTimeWrappedHourly * 0.35f;
                Vector2 backglowDrawPosition = drawPosition + Vector2.UnitY.RotatedBy(spinAngle + TwoPi * i / 4) * 5f;
                Main.EntitySpriteDraw(texture, backglowDrawPosition, NPC.frame, NPC.GetAlpha(Color.SlateGray), NPC.rotation, origin, NPC.scale, SpriteEffects.None);
            }
            Main.spriteBatch.ResetToDefault();

            Main.EntitySpriteDraw(texture, drawPosition, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, origin, NPC.scale, SpriteEffects.None);
        }

        public float SetTrailWidth(float completionRatio)
        {
            return 20f * Utils.GetLerpValue(0.75f, 0f, completionRatio, true) * NPC.scale * NPC.Opacity;
        }

        public Color SetTrailColor(float completionRatio)
        {
            return Color.Lerp(Color.DarkSlateBlue, Color.White, completionRatio) * NPC.Opacity;
        }

        public void DrawTrail()
        {
            /*TrailDrawer ??= new PrimitiveDrawer(SetTrailWidth, SetTrailColor, true, GameShaders.Misc["CalamityMod:ArtemisLaser"]);

            Main.spriteBatch.EnterShaderRegion();
            GameShaders.Misc["CalamityMod:ArtemisLaser"].UseImage1("Images/Extra_189");
            GameShaders.Misc["CalamityMod:ArtemisLaser"].UseImage2("Images/Misc/Perlin");
            TrailDrawer.DrawPrimitives(NPC.oldPos.ToList(), NPC.Size * 0.5f - Main.screenPosition, 85);
            Main.spriteBatch.ExitShaderRegion();*/

            Vector2 positionToCenterOffset = NPC.Size * 0.5f;
            ManagedShader shader = ShaderManager.GetShader("Luminance.StandardPrimitiveShader");
            PrimitiveSettings trailSettings = new(SetTrailWidth, SetTrailColor, _ => positionToCenterOffset, Shader: shader);
            PrimitiveRenderer.RenderTrail(NPC.oldPos.ToList(), trailSettings, 85);
        }
    }
}
