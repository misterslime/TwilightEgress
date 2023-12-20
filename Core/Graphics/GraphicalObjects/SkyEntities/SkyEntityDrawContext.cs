namespace Cascade.Core.Graphics.GraphicalObjects.SkyEntities
{
    public enum SkyEntityDrawContext
    {
        /// <summary>
        /// Draws the sky entity before vanilla's background fog. This means that depending on the depth, the sky entity's color
        /// will gradually blend with the background to give a more "distant" effct.
        /// </summary>
        BeforeBackgroundFog,
        /// <summary>
        /// Draws the sky entity after vanilla's background fog. This means that the sky entity's color is not affected by how far
        /// back it is in the background and will be drawn using whatever color is set for it in its code.
        /// </summary>
        AfterBackgroundFog
    }
}
