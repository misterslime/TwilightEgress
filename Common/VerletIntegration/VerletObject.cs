namespace Cascade.Common.VerletIntegration
{
    public class VerletObject
    {
        public float Radius { get; set; }

        public Vector2 Position { get; set; }
        public Vector2 OldPosition { get; set; }
        public Vector2 Acceleration { get; set; }

        public float Rotation { get; set; }
        public float RotationSpeed { get; set; }

        public Vector2 Velocity => Position - OldPosition;

        public VerletObject(Vector2 position, Vector2 velocity, float radius, float rotationSpeed)
        {
            Position = position;
            OldPosition = position - velocity;
            Radius = radius;
            RotationSpeed = rotationSpeed;
        }

        public void UpdatePosition(float deltaTime)
        {
            Vector2 velocity = Position - OldPosition;
            OldPosition = Position;
            Position = Position + velocity + Acceleration * deltaTime * deltaTime;
            Acceleration = Vector2.Zero;
        }

        public void Accelerate(Vector2 acceleration)
        {
            Acceleration += acceleration;
        }
    }
}