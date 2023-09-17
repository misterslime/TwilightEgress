using rail;

namespace Cascade
{
    public static partial class Utilities
    {
        /// <summary>
        /// A method used for spawning projectiles that also caters for <see cref="Main.netMode"/> and playing sounds.
        /// </summary>
        /// <param name="spawnX">The x spawn position of the projectile.</param>
		/// <param name="spawnY">The y spawn position of the projectile.</param>
		/// <param name="velocityX">The x velocity of the projectile.</param>
		/// <param name="velocityY">The y velocity of the projectile.</param>
		/// <param name="type">The id of the projectile type that is being summoned.</param>
		/// <param name="damage">The damage of the projectile that is being summoned.</param>
		/// <param name="knockback">The knockback of the projectile that is being summoned.</param>
        /// <param name="playSound">Whether or not a sound should be played.</param>
        /// <param name="sound">The ID of the SoundStyle that should be played.</param>
        /// <param name="owner">The owner of the projectile that is being summond. Defaults to Main.myPlayer.</param>
		/// <param name="ai0">An optional <see cref="Projectile.ai"/>[0] fill value. Defaults to 0.</param>
		/// <param name="ai1">An optional <see cref="Projectile.ai"/>[1] fill value. Defaults to 0.</param>
        public static int SpawnProjectile(this Projectile projectile, float spawnX, float spawnY, float velocityX, float velocityY, int type, int damage, float knockback, bool playSound = false, SoundStyle sound = default, int owner = -1, float ai0 = 0, float ai1 = 0, float ai2 = 0)
        {
            int p = 0;
            if (owner == -1)
                owner = Main.myPlayer;

            if (playSound)
                SoundEngine.PlaySound(sound, projectile.Center);

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                p = Projectile.NewProjectile(projectile.GetSource_FromAI(), spawnX, spawnY, velocityX, velocityY, type, damage, knockback, owner, ai0, ai1, ai2);
            }

            return p;
        }

        /// <summary>
        /// A method used for spawning projectiles that also caters for <see cref="Main.netMode"/> and playing sounds.
        /// </summary>
        /// <param name="center">The spawn positon of the projectile.</param>
        /// <param name="velocity">The velocity of the projectile.</param>
        /// <param name="type">The id of the projectile type that is being summoned.</param>
        /// <param name="damage">The damage of the projectile that is being summoned.</param>
        /// <param name="knockback">The knockback of the projectile that is being summoned.</param>
        /// <param name="playSound">Whether or not a sound should be played.</param>
        /// <param name="sound">The ID of the SoundStyle that should be played.</param>
        /// <param name="owner">The owner of the projectile that is being summond. Defaults to Main.myPlayer.</param>
		/// <param name="ai0">An optional <see cref="Projectile.ai"/>[0] fill value. Defaults to 0.</param>
		/// <param name="ai1">An optional <see cref="Projectile.ai"/>[1] fill value. Defaults to 0.</param>
        public static int SpawnProjectile(this Projectile projectile, Vector2 center, Vector2 velocity, int type, int damage, float knockback, bool playSound = false, SoundStyle sound = default, int owner = -1, float ai0 = 0, float ai1 = 0, float ai2 = 0)
        {
            return projectile.SpawnProjectile(center.X, center.Y, velocity.X, velocity.Y, type, damage, knockback, playSound, sound, owner, ai0, ai1, ai2);
        }

        /// <summary>
        /// Moves the projctile towards a Vector2.
        /// </summary>
        /// <param name="projectile">The projectile using this method.</param>
        /// <param name="targetPosition">The position or Vector2 of the position to move to.</param>
        /// <param name="moveSpeed">The maximum movement speed.</param>
        /// <param name="turnResistance">The turn resistance of the movement. This affects how quickly it takes the projectile to adjust direction.</param>
        public static void SimpleMove(this Projectile projectile, Vector2 targetPosition, float moveSpeed, float turnResistance = 10f)
        {
            Vector2 move = targetPosition - projectile.Center;
            float magnitude = (float)Math.Sqrt(move.X * move.X + move.Y * move.Y);
            if (magnitude > moveSpeed)
            {
                move *= moveSpeed / magnitude;
            }
            move = (projectile.velocity * turnResistance + move) / (turnResistance + 1f);
            magnitude = (float)Math.Sqrt(move.X * move.X + move.Y * move.Y);
            if (magnitude > moveSpeed)
            {
                move *= moveSpeed / magnitude;
            }
            projectile.velocity = move;
        }

        /// <summary>
        /// Manually updates <see cref="Projectile.frame"/> and <see cref="Projectile.frameCounter"/> to animate projectile spritesheets.
        /// This method ONLY works with purely vertical spritesheets. If you are creating a projectile that uses a spritesheet with horizontal frames, do not use this method. You will have to manually
        /// write code to animate that.
        /// </summary>
        /// <param name="startFrame">The frame at which the animation starts. The projectile's frame will automatically be set to this if it
        /// is currently not on it.</param>
        /// <param name="endFrame">The frame at which the animation ends.</param>
        /// <param name="frameSpeed">How many frames it takes for <see cref="Projectile.frame"/> to increment.</param>
        public static void UpdateProjectileAnimationFrames(this Projectile projectile, int startFrame, int endFrame, int frameSpeed)
        {
            // Initialize the current frame to ensure it's on the starting frame.
            if (projectile.frameCounter == 0 && projectile.frame != startFrame)
                projectile.frame = startFrame;

            // This esseentially operates the same way as if I were to set frameCounter to 0 after
            // hvaing it reach the max frameSpeed, we just use 0 to initialize the starting frame.
            if (projectile.frameCounter >= frameSpeed + 1)
            {
                projectile.frameCounter = 1;
                if (++projectile.frame >= endFrame)
                {
                    projectile.frame = startFrame;
                }
            }

            projectile.frameCounter++;
        }

        /// <summary>
        /// Adjusts a projectile's hitbox according to the input width, height and their current scale.
        /// </summary>
        /// <param name="initialWidth">The width you'd like the projectile to adjust to.</param>
        /// <param name="initialHeight">The height you'd like the projectile to adjust to.</param>
        public static void AdjustProjectileHitboxByScale(this Projectile projectile, float adjustedWidth, float adjustedHeight)
        {
            int oldWidth = projectile.width;
            int idealWidth = (int)(projectile.scale * adjustedWidth);
            int idealHeight = (int)(projectile.scale * adjustedHeight);
            if (oldWidth != idealWidth)
            {
                projectile.position.X += projectile.width / 2;
                projectile.position.Y += projectile.height / 2;
                projectile.width = idealWidth;
                projectile.height = idealHeight;
                projectile.position.X -= projectile.width / 2;
                projectile.position.Y -= projectile.height / 2;
            }
        }

        public static NPC FindClosestNPCToProjectile(this Projectile projectile, float maxDetectionRadius)
        {
            NPC closestNPC = null;

            float squaredMaxDetectionRadius = maxDetectionRadius * maxDetectionRadius;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC target = Main.npc[i];
                if (target.CanBeChasedBy())
                {
                    float squaredDistanceToTarget = Vector2.DistanceSquared(target.Center, projectile.Center);
                    if (squaredDistanceToTarget < squaredMaxDetectionRadius)
                    {
                        squaredMaxDetectionRadius = squaredDistanceToTarget;
                        closestNPC = target;
                    }
                }
            }

            return closestNPC;
        }

        public static void SearchForViableTargetsForMinion(this Projectile projectile, Player owner, float maxSearchDistance, float maxChaseDistanceThroughWalls, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter) 
        {
            distanceFromTarget = maxSearchDistance;
            targetCenter = projectile.position;
            foundTarget = false;

            // Ensures minions using this are compatible with the item's right click targetting feature.
            if (owner.HasMinionAttackTargetNPC)
            {
                NPC target = Main.npc[owner.MinionAttackTargetNPC];
                float distanceBetweem = Vector2.Distance(target.Center, projectile.Center);

                if (distanceBetweem < maxSearchDistance)
                {
                    distanceFromTarget = distanceBetweem;
                    targetCenter = target.Center;
                    foundTarget = true;
                }
            }

            // Usual targetting code.
            if (!foundTarget)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC target = Main.npc[i];
                    if (target.CanBeChasedBy())
                    {
                        float distanceBetween = Vector2.Distance(target.Center, projectile.Center);
                        bool closestToProjectile = Vector2.Distance(projectile.Center, target.Center) > distanceBetween;
                        bool inRangeOfProjectile = distanceBetween < maxSearchDistance;
                        bool lineOfSight = Collision.CanHitLine(projectile.position, projectile.width, projectile.height, target.position, target.width, target.height);
                        bool closestTargetThroughWalls = distanceBetween < maxChaseDistanceThroughWalls;

                        if (((closestToProjectile && inRangeOfProjectile) || !foundTarget) || (lineOfSight || closestTargetThroughWalls))
                        {
                            distanceFromTarget = distanceBetween;
                            targetCenter = target.Center;
                            foundTarget = true;
                        }
                    }
                }
            }
        }       
    }
}
