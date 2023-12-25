namespace Cascade.Core.Graphics.GraphicalObjects.SkyEntities
{
    public enum SkyEntityDrawContext
    {
        /// <summary>
        /// Draws the sky entity before any other custom skies are drawn. This can be used to have entities draw behind some
        /// objects in a Custom Sky (as well as other sky entities) to give a more "distant" effect, such as blending with a
        /// custom sky texture depending on the depth of the entity.
        /// </summary>
        BeforeCustomSkies,
        /// <summary>
        /// Draws the sky entity after all custom skies have been drawn. Sky entities that use this setting will be unaffected
        /// by anything drawn within a CustomSky's Draw method. 
        /// </summary>
        AfterCustomSkies
    }
}
