namespace Cascade
{
    public static partial class Utilities
    {
        /// <summary>
        /// Simply runs a check to see if the current netMode isn't set to <see cref="NetmodeID.MultiplayerClient"/> to make sure things don't
        /// screw up on Multiplayer. Same ol' particle spawning as usual besides that.
        /// </summary>
        /// <param name="particle"></param>
        public static void SpawnParticleBetter(Particle particle)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
                GeneralParticleHandler.SpawnParticle(particle);
        }

        #region Particle Orchestra Utilities
        // These utilities utilize Vanilla Terraria's ParticleOrchestra system. They cannot be used in conjunction with Calamity's Particle System.

        /// <summary>
        /// Uses a for-loop to spawn a specified amount of particles.
        /// </summary>
        /// <param name="maxParticles">The max amount of particles that should be spawned at a time.</param>
        /// <param name="particlePosition">The position these particles should be spawned at.</param>
        /// <param name="particleVelocity">The velocity of the spawned particles.</param>
        /// <param name="particleType">The specified <see cref="ParticleOrchestraType"/> item that should be spawned.</param>
        /// <param name="clientOnly">Whether or not these particles should only spawn on a client or on the server.</param>
        /// <param name="overrideInvokingPlayerIndex">An override for the player who invoked the particle spawns. Almost always should be <see cref="Main.myPlayer"/>, which is
        /// automatically set when this parameter remains null.</param>
        public static void CreateParticleLoop(int maxParticles, Vector2 particlePosition, Vector2 particleVelocity, ParticleOrchestraType particleType, bool clientOnly = false, int? overrideInvokingPlayerIndex = null)
        {
            for (int i = 0; i < maxParticles; i++)
            {
                ParticleOrchestraSettings particleSettings = new ParticleOrchestraSettings
                {
                    PositionInWorld = particlePosition,
                    MovementVector = particleVelocity
                };
                ParticleOrchestrator.RequestParticleSpawn(clientOnly, particleType, particleSettings, overrideInvokingPlayerIndex);
            }
        }

        /// <summary>
        /// Uses a for-loop to spawn a specified amount of particles.
        /// </summary>
        /// <param name="maxParticles">The max amount of particles that should be spawned at a time.</param>
        /// <param name="settings">The <see cref="ParticleOrchestraSettings"/> that should be used for this particle.
        /// Handles the position and velocity of the particles being spawned.</param>
        /// <param name="particleType">The specified <see cref="ParticleOrchestraType"/> item that should be spawned.</param>
        /// <param name="clientOnly">Whether or not these particles should only spawn on a client or on the server.</param>
        /// <param name="overrideInvokingPlayerIndex">An override for the player who invoked the particle spawns. Almost always should be <see cref="Main.myPlayer"/>, which is
        /// automatically set when this parameter remains null.</param>
        public static void CreateParticleLoop(int maxParticles, ParticleOrchestraSettings settings, ParticleOrchestraType particleType, bool clientOnly = true, int? overrideInvokingPlayerIndex = null)
        {
            for (int i = 0; i < maxParticles; i++)
            {
                CreateParticleLoop(maxParticles, settings.PositionInWorld, settings.MovementVector, particleType, clientOnly, overrideInvokingPlayerIndex);
            }
        }

        /// <summary>
        /// Creates a circle of particles.
        /// </summary>
        /// <param name="maxParticles">The max amount of particles that should be spawned at a time.</param>
        /// <param name="particlePosition">The position these particles should be spawned at.</param>
        /// <param name="particleType">The specified <see cref="ParticleOrchestraType"/> item that should be spawned.</param>
        /// <param name="particlePulseSpeed">The speed at which these particles should pulse out at when spawned.</param>
        /// <param name="clientOnly">Whether or not these particles should only spawn on a client or on the server.</param>
        /// <param name="overrideInvokingPlayerIndex">An override for the player who invoked the particle spawns. Almost always should be <see cref="Main.myPlayer"/>, which is
        /// automatically set when this parameter remains null.</param>
        public static void CreateParticleCircle(int maxParticles, Vector2 particlePosition, ParticleOrchestraType particleType, float particlePulseSpeed = 5f, bool clientOnly = false, int? overrideInvokingPlayerIndex = null)
        {
            for (int i = 0; i < maxParticles; i++)
            {
                Vector2 circleVelocity = Vector2.Normalize(Vector2.UnitY).RotatedBy((i - (maxParticles / 2 - 1) * MathHelper.TwoPi / maxParticles)) + particlePosition;
                Vector2 particleVelocity = Vector2.Normalize(circleVelocity - particlePosition) * particlePulseSpeed;
                ParticleOrchestraSettings particleSettings = new ParticleOrchestraSettings
                {
                    PositionInWorld = particlePosition,
                    MovementVector = particleVelocity
                };
                ParticleOrchestrator.RequestParticleSpawn(clientOnly, particleType, particleSettings, overrideInvokingPlayerIndex);
            }
        }

        /// <summary>
        /// Creates a randomized burst of particles.
        /// </summary>
        /// <param name="maxParticles">The max amount of particles that should be spawned at a time.</param>
        /// <param name="particlePosition">The position these particles should be spawned at.</param>
        /// <param name="particleType">The specified <see cref="ParticleOrchestraType"/> item that should be spawned.</param>
        /// <param name="particlePulseSpeed">The speed at which these particles should pulse out at when spawned.</param>
        /// <param name="circleHalfWidth">The width of the circle.</param>
        /// <param name="circleHalfHeight">The height of the circle.</param>
        /// <param name="clientOnly">Whether or not these particles should only spawn on a client or on the server.</param>
        /// <param name="overrideInvokingPlayerIndex">An override for the player who invoked the particle spawns. Almost always should be <see cref="Main.myPlayer"/>, which is
        /// automatically set when this parameter remains null.</param>
        public static void CreateRandomizedParticleExplosion(int maxParticles, Vector2 particlePosition, ParticleOrchestraType particleType, float particlePulseSpeed = 5f, float circleHalfWidth = 1f, float circleHalfHeight = 1f, bool clientOnly = false, int? overrideInvokingPlayerIndex = null)
        {
            Vector2 particleVelocity = Main.rand.NextVector2Circular(circleHalfWidth, circleHalfHeight) * particlePulseSpeed;
            CreateParticleLoop(maxParticles, particlePosition, particleVelocity, particleType, clientOnly, overrideInvokingPlayerIndex);
        }

        /// <summary>
        /// Spawns particles and pulls them towards a certain center position.
        /// </summary>
        /// <param name="maxParticles">The max amount of particles that should be spawned at a time.</param>
        /// <param name="startingPosition">The position the particles should be spawned at.</param>
        /// <param name="centerToPullTowards">The position the particles should be pulled towards.</param>
        /// <param name="particleType">The specified <see cref="ParticleOrchestraType"/> item that should be spawned.</param>
        /// <param name="particlePullSpeed">The speed at which the particles should be pulled at.</param>
        /// <param name="clientOnly">Whether or not these particles should only spawn on a client or on the server.</param>
        /// <param name="overrideInvokingPlayerIndex">An override for the player who invoked the particle spawns. Almost always should be <see cref="Main.myPlayer"/>, which is
        /// automatically set when this parameter remains null.</param>
        public static void PullParticleInTowardsCenter(int maxParticles, Vector2 startingPosition, Vector2 centerToPullTowards, ParticleOrchestraType particleType, float particlePullSpeed = -8f, bool clientOnly = false, int? overrideInvokingPlayerIndex = null)
        {
            Vector2 particleVelocity = (startingPosition - centerToPullTowards).SafeNormalize(Vector2.UnitY) * particlePullSpeed;
            CreateParticleLoop(maxParticles, startingPosition, particleVelocity, particleType, clientOnly, overrideInvokingPlayerIndex);
        }
        #endregion
    }
}
