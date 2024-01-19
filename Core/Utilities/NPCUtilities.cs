using Terraria.Utilities;

namespace Cascade
{
    public static partial class Utilities
    {
        /// <summary>
        /// A method used for spawning projectiles that also caters for Main.netMode and playing sounds.
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
        public static int SpawnProjectile(this NPC npc, float spawnX, float spawnY, float velocityX, float velocityY, int type, int damage, float knockback, bool playSound, SoundStyle sound, int owner = -1, float ai0 = 0, float ai1 = 0)
        {
            if (owner == -1) { owner = Main.myPlayer; }

            damage = (int)(damage * 0.5);
            if (Main.expertMode) { damage = (int)(damage * 0.5); }
            if (Main.masterMode) { damage = (int)(damage * 0.5); }

            if (playSound)
                SoundEngine.PlaySound(sound, npc.Center);

            int p = 0;
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                p = Projectile.NewProjectile(npc.GetSource_FromAI(), spawnX, spawnY, velocityX, velocityY, type, damage, knockback, owner, ai0, ai1);
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
        public static int SpawnProjectile(this NPC npc, Vector2 center, Vector2 velocity, int type, int damage, float knockback, bool playSound, SoundStyle sound, int owner = -1, float ai0 = 0, float ai1 = 0)
        {
            return npc.SpawnProjectile(center.X, center.Y, velocity.X, velocity.Y, type, damage, knockback, playSound, sound, owner, ai0, ai1);
        }

        /// <summary>
		/// A custom version of <see cref="NPC.NewNPC"/> that handles checking <see cref="Main.netMode"/> and changes the spawn variables to be consistent with <see cref="Projectile.NewProjectile"/>.
		/// </summary>
		/// <param name="spawnX">The X spawn position of the NPC being spawned.</param>
		/// <param name="spawnY">The Y spawn position of the NPC being spawned.</param>
		/// <param name="type">The ID of the NPC that is being spawned.</param>
		/// <param name="Start">Can be used to ensure that the NPC you are spawning is spawned in a slot after an existing NPC. E.g. Ensuring that Boss Minions draw behind the main Boss. Defaults to 0.</param>
		/// <param name="ai0">An optional <see cref="NPC.ai"/>[0] fill value. Defaults to 0.</param>
		/// <param name="ai1">An optional <see cref="NPC.ai"/>[1] fill value. Defaults to 0.</param>
		/// <param name="ai2">An optional <see cref="NPC.ai"/>[2] fill value. Defaults to 0.</param>
		/// <param name="ai3">An optional <see cref="NPC.ai"/>[3] fill value. Defaults to 0.</param>
		/// <param name="target">Can be set to a <see cref="Player.whoAmI"/> to have the NPC being spawned to target a specific player immediately. Defaults to 255.</param>
		/// <param name="velocity">Can be used to give the NPC a new velocity value immediatley upon spawning.</param>
		public static int SpawnNPC(this NPC npc, float spawnX, float spawnY, int type, int Start = 0, float ai0 = 0f, float ai1 = 0f, float ai2 = 0f, float ai3 = 0f, int target = 255, Vector2 velocity = default)
        {
            int n = 0;
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                n = NPC.NewNPC(npc.GetSource_FromAI(), (int)spawnX, (int)spawnY, type, Start, ai0, ai1, ai2, ai3, target);
                if (Main.npc.IndexInRange(n))
                {
                    Main.npc[n].velocity = velocity;
                }
            }
            return n;
        }

        /// <summary>
        /// A custom version of <see cref="NPC.NewNPC"/> that changes the spawn variables to be consistent with <see cref="Projectile.NewProjectile"/>.
        /// Also contains a new velocity paramter that allows you to set NPC velocity right as it spawns.
        /// </summary>
        /// <param name="spawn">The Vector2 position of the NPC being spawned.</param>
        /// <param name="type">The ID of the NPC that is being spawned.</param>
        /// <param name="Start">Can be used to ensure that the NPC you are spawning is spawned in a slot after an existing NPC. E.g. Ensuring that Boss Minions draw behind the main Boss. Defaults to 0.</param>
        /// <param name="ai0">An optional <see cref="NPC.ai"/>[0] fill value. Defaults to 0.</param>
        /// <param name="ai1">An optional <see cref="NPC.ai"/>[1] fill value. Defaults to 0.</param>
        /// <param name="ai2">An optional <see cref="NPC.ai"/>[2] fill value. Defaults to 0.</param>
        /// <param name="ai3">An optional <see cref="NPC.ai"/>[3] fill value. Defaults to 0.</param>
        /// <param name="target">Can be set to a <see cref="Player.whoAmI"/> to have the NPC being spawned to target a specific player immediately. Defaults to 255.</param>
        /// <param name="velocity">Can be used to give the NPC a new velocity value immediatley upon spawning.</param>
        public static int SpawnNPC(this NPC npc, Vector2 spawn, int type, int Start = 0, float ai0 = 0f, float ai1 = 0f, float ai2 = 0f, float ai3 = 0f, int target = 255, Vector2 velocity = default)
        {
            return npc.SpawnNPC(spawn.X, spawn.Y, type, Start, ai0, ai1, ai2, ai3, target, velocity);
        }

        public static void AdjustNPCHitboxToScale(this NPC npc, float originalWidth, float originalHeight)
        {
            int oldWidth = npc.width;
            int idealWidth = (int)(npc.scale * originalWidth);
            int idealHeight = (int)(npc.scale * originalHeight);
            if (idealWidth != oldWidth)
            {
                npc.position.X += npc.width / 2;
                npc.position.Y += npc.height / 2;
                npc.width = idealWidth;
                npc.height = idealHeight;
                npc.position.X -= npc.width / 2;
                npc.position.Y -= npc.height / 2;
            }
        }

        /// <summary>
        /// Allows NPCs to search for nearby <see cref="Player"/> and <see cref="NPC"/> instances as viable targets.
        /// NPCs that use this utility method MUST also havee <see cref="NPCID.Sets.UsesNewTargetting"/> set to true.
        /// </summary>
        /// <param name="searcher"></param>
        /// <param name="targetPlayers"></param>
        /// <param name="playerSearchDistance"></param>
        /// <param name="npcSearchDistance"></param>
        public static void AdvancedNPCTargetSearching(NPC searcher, NPCUtils.SearchFilter<Player> playerSearchFilter, NPCUtils.SearchFilter<NPC> npcSearchFilter)
        {
            NPCUtils.TargetSearchResults targetSearchResults = NPCUtils.SearchForTarget(searcher, NPCUtils.TargetSearchFlag.All, playerSearchFilter, npcSearchFilter);
            if (targetSearchResults.FoundTarget)
            {
                searcher.target = targetSearchResults.NearestTargetIndex;
                searcher.targetRect = targetSearchResults.NearestTargetHitbox;
            }
        }

        public static void TurnAroundMovement(this NPC npc, Vector2 centerAhead, bool leavingWorldBounds)
        {
            float distanceToTileCollisonLeft = DistanceToTileCollisionHit(npc.Center, npc.velocity.RotatedBy(-PiOver2)) ?? 1000f;
            float distanceToTileCollisonRight = DistanceToTileCollisionHit(npc.Center, npc.velocity.RotatedBy(PiOver2)) ?? 1000f;
            float turnAroundDirection = distanceToTileCollisonLeft > distanceToTileCollisonRight ? -1f : 1f;

            Vector2 turnAroundVelocity = npc.velocity.RotatedBy(PiOver2 * turnAroundDirection);
            if (leavingWorldBounds)
                turnAroundVelocity = centerAhead.Y < Main.maxTilesY * 0.3f ? Vector2.UnitY * 5f : centerAhead.X >= Main.maxTilesX * 16f - 700f ? Vector2.UnitX * -5f : Vector2.UnitX * 5f;

            npc.velocity.MoveTowards(turnAroundVelocity, 0.2f);
            npc.velocity = Vector2.Lerp(npc.velocity, turnAroundVelocity, 0.2f);
        }
    }
}
