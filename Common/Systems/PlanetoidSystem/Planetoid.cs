namespace Cascade.Common.Systems.PlanetoidSystem {
    public abstract class Planetoid : Entity {
        public int ID;
        public virtual float Gravity { get; set; } = 0.0f;
        public virtual bool doesCollide { get; set; }

        public virtual void Update() { }
        public virtual void Collision(Player player) { }

        public void AI() {
            if(doesCollide) {
                for(int i = 0; i < Main.maxPlayers; i++) {
                    Player player = Main.player[i];
                    if(Colliding(player))
                        Collision(player);
                }
            }
            Update();
        }

        public virtual bool Colliding(Player player) 
            => player.Hitbox.Intersects(Hitbox);

    }
}
