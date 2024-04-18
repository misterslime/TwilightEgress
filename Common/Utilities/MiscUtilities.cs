namespace Cascade
{
    public static partial class CascadeUtilities
    {
        public static readonly string EmptyPixelPath = "Cascade/Assets/ExtraTextures/EmptyPixel";

        public static readonly string PixelPath = "Luminance/Assets/Pixel";

        /// <summary>
        /// Shorthand check for <c>Main.netMode != NetmodeID.MultiplayerClient</c>. Typically used when spawning entities such as NPCs and Projectiles.
        /// This does not need to be called when using Cascade's entity spawning utilities, such as <see cref="CascadeUtilities.SpawnNPC(NPC, float, float, int, int, float, float, float, float, int, Vector2)"/>
        /// </summary>
        /// <returns></returns>
        public static bool ObligatoryNetmodeCheckForSpawningEntities() => Main.netMode != NetmodeID.MultiplayerClient;
    }
}
