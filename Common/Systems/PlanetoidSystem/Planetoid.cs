namespace Cascade.Common.Systems.PlanetoidSystem
{
    public abstract class Planetoid : ModType<Entity>, ILocalizedModType
    {
        public int ID;
        public float rotation;
        public virtual float Gravity { get; set; } = 0.0f;
        public virtual bool doesCollide { get; set; }

        public abstract float maxAttractionRadius { get; }
        public abstract float walkableRadius { get; }

        public virtual string Texture => (GetType().Namespace + "." + Name).Replace('.', '/');

        public virtual string LocalizationCategory => "Planetoids";

        protected sealed override Entity CreateTemplateEntity() => Entity;

        protected sealed override void Register()
        {
            ID = PlanetoidSystem.planetoidsByType.Count;
            PlanetoidSystem.planetoidsByType.Add(GetType(), this);
        }

        public sealed override void SetupContent()
            => CascadeTextureRegistry.Planetoids[ID] = ModContent.Request<Texture2D>(Texture);

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
                var planetoidPlayer = player.GetModPlayer<PlanetoidPlayer>();
            }

            Update();
        }

        public virtual bool Colliding(Player player)
            => player.Hitbox.Intersects(Entity.Hitbox);

    }
}