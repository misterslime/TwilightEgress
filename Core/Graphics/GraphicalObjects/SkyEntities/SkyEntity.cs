namespace Cascade.Core.Graphics.GraphicalObjects.SkyEntitySystem
{
    /// <summary>
    /// A class used to create the various other entities that can randonly be seen in the sky.
    /// <see cref="Draw"/> must be called within the <see cref="CustomSky"/> an entity is spawned in.
    /// </summary>
    public abstract class SkyEntity
    {
        public int Time;

        public int Lifespan;

        public int Frame;

        public int FrameCounter;

        public Vector2 Position;

        public Vector2 Velocity;

        public float Opacity;

        public float Scale;

        public float Rotation;

        public float Depth;

        public Color Color;

        public virtual int MaxFrames => 1;

        public virtual bool ShouldBypassLimit => false;

        public virtual bool DieWithLifespan => true;

        public virtual void Update() { }

        public virtual void Draw(SpriteBatch spriteBatch, float intensity) { }

        public virtual void Kill()
        {
            SkyEntityHandler.RemoveSkyEntity(this);
        }
    }
}
