using Cascade.Assets.Effects;
using Cascade.Content.Skies.SkyEntities;
using Cascade.Core.Systems.SkyEntitySystem;
using Microsoft.CodeAnalysis.CSharp;
namespace Cascade.Content.Skies
{
    public class CometNightSkyScene : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

        public override float GetWeight(Player player) => 0.8f;

        public override bool IsSceneEffectActive(Player player)
        {
            return player.ZoneCometNight();
        }

        public override void SpecialVisuals(Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals("Cascade:CometNight", isActive);
        }
    }

    public class CometNightSky : CustomSky
    {
        private bool isActive;

        private float FadeOpacity;

        private List<FadingGlowStar> GlowStars;

        private List<FadingGlowBall> GlowBalls;

        private List<StarConstellation> StarConstellations;

        private List<StardustPillar> StardustPillars;

        private int GlowStarSpawnChance
        {
            get
            {
                if (!isActive)
                    return int.MaxValue;
                return 40;
            }
        }

        private int GlowBallSpawnChance
        {
            get
            {
                if (!isActive)
                    return int.MaxValue;
                return 25;
            }
        }

        private int StardustPillarChance
        {
            get
            {
                if (!isActive)
                    return int.MaxValue;
                return 5000;
            }
        }

        private const int MaxStardustPillars = 1;

        private const int MaxConstellations = 6;

        private Color GetGlowStarColor()
        {
            Color firstColor = Utils.SelectRandom(Main.rand, Color.SkyBlue, Color.AliceBlue);
            Color secondColor = Utils.SelectRandom(Main.rand, Color.Cyan, Color.CornflowerBlue);
            return Color.Lerp(firstColor, secondColor, Main.rand.NextFloat(0.1f, 1f));
        }

        public override void OnLoad()
        {
            GlowStars = new List<FadingGlowStar>();
            GlowBalls = new List<FadingGlowBall>();
            StarConstellations = new List<StarConstellation>();
            StardustPillars = new List<StardustPillar>();
        }

        public override float GetCloudAlpha()
        {
            return (1f - FadeOpacity) * 0.3f + 0.7f;
        }

        public override void Activate(Vector2 position, params object[] args)
        {
            isActive = true;
        }

        public override void Deactivate(params object[] args)
        {
            isActive = false;
        }

        public override void Reset()
        {
            isActive = false;
        }

        public override bool IsActive() => isActive || FadeOpacity > 0f;

        public override void Update(GameTime gameTime)
        {
            if (isActive && FadeOpacity < 1f)
            {
                FadeOpacity += 0.01f;
            }
            else if (!isActive && FadeOpacity > 0f)
            {
                FadeOpacity -= 0.01f;
            }

            SpawnSkyEntities();
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (maxDepth >= float.MaxValue && minDepth < float.MaxValue)
            {
                Texture2D skyTexture = ModContent.Request<Texture2D>("Cascade/Content/Skies/CometNightSky").Value;
                spriteBatch.Draw(skyTexture, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight - 100), Color.White with { A = 0 } * FadeOpacity * 0.6f);
            }

            DrawGlowStars(spriteBatch, minDepth, maxDepth);
        }

        public void SpawnSkyEntities()
        {
            if (isActive)
            {
                if (Main.rand.NextBool(GlowStarSpawnChance))
                {
                    Vector2 position = Main.screenPosition + Main.rand.NextVector2Circular(Main.screenWidth * 4f, Main.screenHeight * 4f);
                    float scale = Main.rand.NextFloat(0.3f, 0.8f);
                    float depth = Main.rand.NextFloat() * 8f + 1.5f;
                    int lifespan = Main.rand.Next(180, 200);
                    FadingGlowStar glowStar = new FadingGlowStar(position, GetGlowStarColor(), scale, depth, lifespan);
                    SkyEntityHandler.SpawnSkyEntity(glowStar);
                    GlowStars.Add(glowStar);
                }

                if (Main.rand.NextBool(GlowBallSpawnChance))
                {
                    Vector2 position = Main.screenPosition + Main.rand.NextVector2Circular(Main.screenWidth * 4f, Main.screenHeight * 4f);
                    Vector2 velocity = -Vector2.UnitY * Main.rand.NextFloat(1f, 4f);
                    float scale = Main.rand.NextFloat(0.1f, 0.3f);
                    float depth = Main.rand.NextFloat() * 5f + 1.5f;
                    int lifespan = Main.rand.Next(240, 300);
                    FadingGlowBall glowBall = new FadingGlowBall(position, velocity, GetGlowStarColor(), scale, depth, lifespan);
                    SkyEntityHandler.SpawnSkyEntity(glowBall);
                    GlowBalls.Add(glowBall);
                }

                // Remove inactive entities from the list when its time to.
                GlowStars.RemoveAll(star => star.Time >= star.Lifespan);
                GlowBalls.RemoveAll(ball => ball.Time >= ball.Lifespan);
                StardustPillars.RemoveAll(pillar => pillar.Time >= pillar.Lifespan);
            }
        }

        public void DrawGlowStars(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {       
            foreach (FadingGlowStar star in GlowStars)
            {
                if (star == null) 
                    continue;

                if (maxDepth >= star.Depth && minDepth < star.Depth)
                    star.Draw(spriteBatch, FadeOpacity);
            }
            
            foreach (FadingGlowBall ball in GlowBalls)
            {
                if (ball == null) 
                    continue;

                if (maxDepth >= ball.Depth && minDepth < ball.Depth)
                    ball.Draw(spriteBatch, FadeOpacity);
            }
        }
    }
}
