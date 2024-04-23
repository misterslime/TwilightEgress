using Cascade.Core.Graphics.GraphicalObjects.SkyEntities;
using System.Runtime.Serialization;

namespace Cascade.Core.Graphics.GraphicalObjects.SkyEntitySystem
{
    public class SkyEntityManager : ModSystem
    {
        #region Fields
        public static Dictionary<Type, SkyEntity> SkyEntities;

        public static Dictionary<Type, int> SkyEntityIDs;

        public static Dictionary<int, Texture2D> SkyEntityTextures;

        public static List<SkyEntity> ActiveSkyEntities;
        #endregion

        #region Overrides
        #region Loading and Unloading
        public override void OnModLoad()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            SkyEntities = new();
            SkyEntityIDs = new();
            SkyEntityTextures = new();
            ActiveSkyEntities = new();

            // Always ensure that the current ID is set to the highest value in the dictionary.
            int currentID = SkyEntityIDs.Any() ? SkyEntityIDs.Values.Max() : 0;

            IEnumerable<Type> skyEntitySubclasses = Utilities.GetEveryTypeDerivedFrom(typeof(SkyEntity), Cascade.Instance.Code);
            foreach (Type type in skyEntitySubclasses)
            {
                SkyEntity skyEntity = (SkyEntity)FormatterServices.GetUninitializedObject(type);
                SkyEntities[type] = skyEntity;

                // Store an ID for each sky entity. All sky entities of the same type that are spawned will copy this ID.
                SkyEntityIDs[type] = currentID;

                // Texture loading.
                // Automatically load sky entity textures based on their internal ID.
                Texture2D skyEntityTexture = ModContent.Request<Texture2D>(skyEntity.TexturePath, AssetRequestMode.ImmediateLoad).Value;
                SkyEntityTextures[currentID] = skyEntityTexture;

                // Incremenet the ID for each sky enity type.
                currentID++;
            }

            On_SkyManager.DrawDepthRange += DrawSkyEntities_BeforeCustomSkies;
            On_SkyManager.DrawDepthRange += DrawSkyEntities_AfterCustomSkies;
        }

        public override void OnModUnload()
        {
            SkyEntities = null;
            SkyEntityIDs = null;
            SkyEntityTextures = null;
            ActiveSkyEntities = null;

            On_SkyManager.DrawDepthRange -= DrawSkyEntities_BeforeCustomSkies;
            On_SkyManager.DrawDepthRange -= DrawSkyEntities_AfterCustomSkies;
        }

        public override void OnWorldLoad() => ActiveSkyEntities.Clear();

        public override void OnWorldUnload() => ActiveSkyEntities.Clear();

        #endregion

        #region Updating
        public override void PostUpdateEverything()
        {
            foreach (SkyEntity skyEntity in ActiveSkyEntities)
            {
                if (skyEntity == null || !skyEntity.Active)
                    continue;

                skyEntity.Active = true;
                skyEntity.Time++;
                skyEntity.Position += skyEntity.Velocity;
                skyEntity.Update();
            }

            ActiveSkyEntities.RemoveAll(skyEntity => !skyEntity.Active || (skyEntity.Time >= skyEntity.Lifespan && skyEntity.DieWithLifespan));
        }
        #endregion
        #endregion

        #region Static Methods
        public static bool IsSpecificSkyEntityActive<T>() where T : SkyEntity => SkyEntities[typeof(T)].Active;

        public static int CountActiveSkyEntities<T>() where T : SkyEntity
        {
            int count = 0;
            foreach (SkyEntity sky in ActiveSkyEntities)
            {
                if (sky.ID == SkyEntityIDs[typeof(T)])
                    count++;
            }

            return count;
        }

        #endregion

        #region Private Methods
        private void DrawAllSkyEntityInstances(SkyEntity skyEntity, SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (ActiveSkyEntities.Count <= 0)
                return;

            // Prepare for screen culling.
            RasterizerState screenCullState = CascadeUtilities.PrepareScissorRectangleState();

            spriteBatch.Begin(SpriteSortMode.Deferred, skyEntity.BlendState, Main.DefaultSamplerState, DepthStencilState.None, screenCullState, null, Main.GameViewMatrix.TransformationMatrix);

            if (skyEntity.Depth > minDepth && skyEntity.Depth <= maxDepth)
                skyEntity.Draw(spriteBatch);

            spriteBatch.End();
        }

        private void DrawSkyEntities_BeforeCustomSkies(On_SkyManager.orig_DrawDepthRange orig, SkyManager self, SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            spriteBatch.End();
            foreach (SkyEntity skyEntity in ActiveSkyEntities)
            {
                if (skyEntity.DrawContext != SkyEntityDrawContext.BeforeCustomSkies)
                    continue;
                DrawAllSkyEntityInstances(skyEntity, spriteBatch, minDepth, maxDepth);
            }
            spriteBatch.ResetToDefault(false);

            orig.Invoke(self, spriteBatch, minDepth, maxDepth);
        }

        private void DrawSkyEntities_AfterCustomSkies(On_SkyManager.orig_DrawDepthRange orig, SkyManager self, SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            orig.Invoke(self, spriteBatch, minDepth, maxDepth);

            spriteBatch.End();
            foreach (SkyEntity skyEntity in ActiveSkyEntities)
            {
                if (skyEntity.DrawContext != SkyEntityDrawContext.AfterCustomSkies)
                    continue;
                DrawAllSkyEntityInstances(skyEntity, spriteBatch, minDepth, maxDepth);
            }
            spriteBatch.ResetToDefault(false);
        }
        #endregion
    }
}
