using Cascade.Common.Physics.VerletIntegration;

namespace Cascade.Common.Physics.Gravity
{
    public class MassiveObject : VerletObject
    {
        public float Mass;

        public MassiveObject(Vector2 position, Vector2 velocity, float radius, float rotationSpeed, float mass)
        {
            Position = position;
            OldPosition = position - velocity;
            Radius = radius;
            RotationSpeed = rotationSpeed;
            Mass = mass;
        }
    }
}
