using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cascade.Common.VerletIntegration
{
    public static class VerletSolvers
    {
        public static void UpdateVerlets(this VerletObject[] verletCollection, float deltaTime)
        {
            int substeps = 8;
            float substepDeltaTime = deltaTime / substeps;

            for (int i = 0; i < substeps; i++)
            {
                //verletCollection.ApplyConstraints();
                verletCollection.SolveCollisions();
                verletCollection.UpdatePositions(substepDeltaTime);
            }
        }

        public static void UpdatePositions(this VerletObject[] verletCollection, float deltaTime)
        {
            foreach (VerletObject? verlet in verletCollection)
                verlet?.UpdatePosition(deltaTime);
        }

        public static void ApplyConstraints(this VerletObject[] verletCollection)
        {
            Vector2 position = Main.player[Main.myPlayer].Center;
            float radius = 400f;

            foreach (VerletObject? verlet in verletCollection)
            {
                if (verlet is not null)
                {
                    Vector2 toObject = verlet.Position - position;
                    float distance = toObject.Length();

                    // 50 is the default radius
                    if (distance > radius - verlet.Radius)
                    {
                        Vector2 normal = toObject / distance;
                        verlet.Position =  position + normal * (radius - verlet.Radius);
                    }
                }
            }
        }

        public static void SolveCollisions(this VerletObject[] verletCollection)
        {
            float gravity = Main.player[Main.myPlayer].gravity;

            for (int i = 0; i < verletCollection.Length; i++)
            {
                VerletObject? verlet1 = verletCollection[i];

                if (verlet1 is not null)
                {
                    for (int j = i + 1; j < verletCollection.Length; j++)
                    {
                        VerletObject? verlet2 = verletCollection[j];

                        if (verlet2 is not null)
                        {
                            Vector2 collisionAxis = verlet1.Position - verlet2.Position;
                            float distance = collisionAxis.Length();
                            float minDistance = verlet1.Radius + verlet2.Radius;

                            if (distance < minDistance)
                            {
                                Vector2 normal = collisionAxis / distance;
                                float delta = minDistance - distance;

                                verlet1.Position += delta * normal / 2f;
                                verlet2.Position -= delta * normal / 2f;
                            } else
                            {
                                float mass1 = verlet1.Radius * verlet1.Radius * gravity;
                                //float mass2 = verlet2.Radius * verlet2.Radius * gravity;

                                // F = G * M1 * M2 / d^2
                                // ma = G * M1 * M2 / d^2
                                // a = (G * M1 * M2) / (d^2 * m)

                                float acceleration = 30 * (mass1 * mass1) / (distance * distance * mass1);

                                Vector2 normal = collisionAxis / distance;

                                verlet1.Accelerate(-acceleration * normal);
                                verlet2.Accelerate(acceleration * normal);
                            }
                        }
                    }
                }
            }
        }
    }
}
