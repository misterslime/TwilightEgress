using Cascade.Content.Skies.SkyEntities;
using Terraria.Graphics;

namespace Cascade.Content.Skies
{
    public class CosmostoneShowersSceneEffect : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

        public override float GetWeight(Player player) => 0.8f;

        public override bool IsSceneEffectActive(Player player) => player.ZoneCosmostoneShowers();

        public override void SpecialVisuals(Player player, bool isActive) => player.ManageSpecialBiomeVisuals("Cascade:CosmostoneShowers", isActive);
    }

    public class CosmostoneShowersSky : CustomSky
    {
        private bool isActive;

        private float FadeOpacity;

        private List<ShiningStar> Stars;

        private List<TravellingAsteroid> TravellingAsteroids;

        private int StarSpawnChance
        {
            get
            {
                if (!isActive)
                    return int.MaxValue;
                return 6;
            }
        }

        private int AsteroidSpawnChance
        {
            get
            {
                if (!isActive)
                    return int.MaxValue;
                return 45;
            }
        }

        private const int MaxConstellations = 6;

        private Color GetGlowStarColor()
        {
            // Blues, reds and purples.
            Color firstColor = Utils.SelectRandom(Main.rand, Color.SkyBlue, Color.AliceBlue, Color.DeepSkyBlue, Color.Purple, Color.MediumPurple, Color.BlueViolet, Color.Violet, Color.PaleVioletRed, Color.MediumVioletRed);
            // Yellows, oranges and whites.
            Color secondColor = Utils.SelectRandom(Main.rand, Color.Yellow, Color.Goldenrod, Color.LightGoldenrodYellow, Color.LightYellow, Color.Orange, Color.OrangeRed, Color.White, Color.FloralWhite, Color.NavajoWhite);
            return Color.Lerp(firstColor, secondColor, Main.rand.NextFloat(0.1f, 1f));
        }

        public override void OnLoad()
        {
            Stars = new();
            TravellingAsteroids = new();
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
                FadeOpacity += 0.01f;
            else if (!isActive && FadeOpacity > 0f)
                FadeOpacity -= 0.01f;

            SpawnSkyEntities();

            Stars.RemoveAll(s => !s.Active || s.Time >= s.Lifespan);
            TravellingAsteroids.RemoveAll(a => !a.Active || a.Time >= a.Lifespan);
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (maxDepth >= float.MaxValue && minDepth < float.MaxValue)
            {
                Texture2D skyTexture = ModContent.Request<Texture2D>("Cascade/Content/Skies/CosmostoneShowersSky").Value;
                spriteBatch.Draw(skyTexture, new Rectangle(0, Math.Max(0, (int)((Main.worldSurface * 16f - (Main.screenPosition.Y + 2400f * 400f)) * 0.1f)), Main.screenWidth, Main.screenHeight - 100), Color.White * FadeOpacity * 0.6f);
            }
        }

        public void SpawnSkyEntities()
        {
            if (!isActive)
                return;

            int totalStars = 300;
            int totalAsteroids = 100;
            int totalStarLayers = 7;
            int totalAsteroidsLayers = 3;

            VirtualCamera virtualCamera = new(Main.LocalPlayer);

            // Shining Stars.
            if (Stars.Count < totalStars)
            {
                for (int i = 0; i < totalStarLayers; i++)
                {
                    float x = Main.maxTilesX * Main.rand.NextFloat(-2f, 2f); 
                    float y = (float)(Main.worldSurface * 16f) * Main.rand.NextFloat(-0.01f, -0.3f);
                    Vector2 position = virtualCamera.Player.Center + new Vector2(x, y);

                    float maxScale = Main.rand.NextFloat(1f, 3f);
                    int lifespan = Main.rand.Next(120, 240);

                    // Spawn the stars.
                    ShiningStar shiningStar = new(position, GetGlowStarColor(), maxScale, i + 1.5f, new Vector2(1f, 1.5f), lifespan);
                    if (Main.rand.NextBool(StarSpawnChance))
                        shiningStar.Spawn();
                    Stars.Add(shiningStar);
                }
            }

            // Horizontally-travelling Asteroids.
            if (TravellingAsteroids.Count < totalAsteroids)
            {
                for (int i = 0; i < totalAsteroidsLayers; i++)
                {
                    float x = -virtualCamera.Size.X - 1280f;
                    float y = (float)(Main.worldSurface * 16f) * Main.rand.NextFloat(-0.2f, 0.3f);
                    Vector2 position = virtualCamera.Center + new Vector2(x, y);

                    float speed = Main.rand.NextFloat(3f, 15f);
                    Vector2 velocity = Vector2.UnitX * speed;

                    float maxScale = Main.rand.NextFloat(0.5f, 3f);
                    float depth = i + 3f;
                    int lifespan = Main.rand.Next(1200, 1800);

                    // Spawn the stars.
                    TravellingAsteroid asteroid = new(position, velocity, maxScale, depth, speed * Main.rand.NextFloat(0.01f, 0.02f), lifespan);
                    if (Main.rand.NextBool(AsteroidSpawnChance))
                        asteroid.Spawn();
                    TravellingAsteroids.Add(asteroid);
                }
            }

            Utilities.CreateDustLoop(2, Utilities.ScreenCenter, Vector2.Zero, DustID.Dirt);
        }
    }
}
