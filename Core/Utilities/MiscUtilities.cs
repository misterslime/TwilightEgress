namespace Cascade
{
    public static partial class Utilities
    {
        /// <summary>
        /// Shorthand check for <c>Main.netMode != NetmodeID.MultiplayerClient</c>. Typically used when spawning entities such as NPCs and Projectiles.
        /// This does not need to be called when using Cascade's entity spawning utilities, such as <see cref="Utilities.SpawnNPC(NPC, float, float, int, int, float, float, float, float, int, Vector2)"/>
        /// </summary>
        /// <returns></returns>
        public static bool ObligatoryNetmodeCheckForSpawningEntities() => Main.netMode != NetmodeID.MultiplayerClient;
    }
}
