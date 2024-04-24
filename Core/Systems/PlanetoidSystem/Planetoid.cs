using Cascade.Core.Players;

namespace Cascade.Core.Systems.PlanetoidSystem
{
    public abstract class Planetoid : ModType<Entity>, ILocalizedModType
    {
        public int ID;
        public float rotation;
        public PlanetoidPlayer ModPlayer = null;
        public virtual float Gravity { get; set; } = 0.0f;
        public virtual bool doesCollide { get; set; }
        public virtual float MaxGravitationalIncrease { get; set; } = 4f;
        public virtual float GravitationalIncrement { get; set; } = 0.2f;
        public virtual float PlanetoidEjectionSpeed { get; set; } = 12f;
        public abstract float maxAttractionRadius { get; }
        public abstract float walkableRadius { get; }

        public virtual string Texture => (GetType().Namespace + "." + Name).Replace('.', '/');

        public virtual string LocalizationCategory => "Planetoids";

        protected sealed override Entity CreateTemplateEntity() => Entity;

        protected sealed override void Register()
        {
            ID = PlanetoidSystem.PlanetoidsByType.Count;
            PlanetoidSystem.PlanetoidsByType.Add(GetType(), this);
        }

        public sealed override void SetupContent() => SetStaticDefaults();

        public override void SetStaticDefaults()
        {
            PlanetoidSystem.AddTexture(ModContent.Request<Texture2D>(Texture));
            base.SetStaticDefaults();
        }

        public virtual void Update() { }
        public virtual void Collision(Player player) { }

        public void AI()
        {
            if (doesCollide)
            {
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    Player player = Main.player[i];
                    if (Colliding(player))
                        Collision(player);
                }
            }

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (!player.active)
                    continue;

                ModPlayer = player.Cascade_Planetoid();

                float totalAttractionRadius = maxAttractionRadius + walkableRadius;
                float distanceBetweenBodies = Vector2.Distance(player.Center, Entity.Center);

                if (distanceBetweenBodies < totalAttractionRadius && ModPlayer.AttractionCooldown <= 0 && ModPlayer.Planetoid is null)
                {
                    // Set the planetoid index and player angle.
                    ModPlayer.Planetoid = this;
                    ModPlayer.PlayerAngle = (player.Center - Entity.Center).ToRotation();
                    //netUpdate here
                }

                // Despawn if the player is too far away.
                if (distanceBetweenBodies >= totalAttractionRadius + 2500f)
                {
                    Entity.active = false;
                    //netUpdate here
                }
            }

            if (ModPlayer.Planetoid is not null && ModPlayer.Planetoid == this)
            {
                // Increment the gravitational variable slowly. This will give the gravity the player experiences a more
                // gradual effect, to give them a more realistic feeling of being pulled into a planet's atmosphere.
                // Full equation is explained in OrbitalGravityPlayer.
                Gravity += GravitationalIncrement;
                Gravity = Clamp(Gravity, 0f, MaxGravitationalIncrease);
            }
            else
            {
                // Reset if this planetoid instance is not being utilized at the moment.
                Gravity = Clamp(Gravity - 1f, 0f, MaxGravitationalIncrease);
            }

            Update();
        }

        public virtual bool Colliding(Player player)
            => player.Hitbox.Intersects(Entity.Hitbox);

        public virtual Vector2? GetWalkablePlanetoidPosition(Player player) => null;
    }
}