using Cascade.Core.Configs;
using Cascade.Core.Graphics.GraphicalObjects.SkyEntities;

namespace Cascade.Core.Graphics.GraphicalObjects.SkyEntitySystem
{
    public abstract class SkyEntity
    {
        /// <summary>
        /// The stored texture of this sky entity, obtained from its <see cref="TexturePath"/>.
        /// This is stored as a property so you won't have to endlessly make <see cref="ModContent.Request{T}(string, AssetRequestMode)"/>
        /// calls when getting a sky entity's texture.
        /// </summary>
        public Texture2D StoredTexture { get; protected set; }

        /// <summary>
        /// The internal ID of this sky entity. Each sky entity of the same type has the same ID, and those that are spawned in-game inherit
        /// said ID. This is used for automatically loading sky entity textures, you won't have to do anything with this.
        /// </summary>
        public int ID { get; private set; }

        /// <summary>
        /// A variable that constantly increments by 1 once a sky entity has spawned. 
        /// </summary>
        public int Time;

        /// <summary>
        /// The total amount of time this sky entity should remain active for.
        /// </summary>
        public int Lifespan;

        /// <summary>
        /// A local variable which can be utilized by animated sky entities. For setting which frame of animation this sky entity is 
        /// currently at.
        /// </summary>
        public int Frame;

        /// <summary>
        /// A local variable which can be utilized by animated sky entities. For counting the amount of frames between when an animation frame
        /// updates.
        /// </summary>
        public int FrameCounter;

        /// <summary>
        /// The current position of the sky entity in the world. Updates with <see cref="Velocity"/>.
        /// </summary>
        public Vector2 Position;

        /// <summary>
        /// The current velocity which this sky entity is moving at.
        /// </summary>
        public Vector2 Velocity;

        /// <summary>
        /// The opacity of the sky entity.
        /// </summary>
        public float Opacity;

        /// <summary>
        /// The scale of the sky entity.
        /// </summary>
        public float Scale;

        /// <summary>
        /// The rotation of the sky entity.
        /// </summary>
        public float Rotation;

        /// <summary>
        /// The depth of the sky entity. Defaults to 1.
        /// </summary>
        public float Depth;

        /// <summary>
        /// Whether or not the sky entity is currently active or not. Shall be set to true on-spawn.
        /// </summary>
        public bool Active;

        /// <summary>
        /// The color of the sky entity. To be used in drawing.
        /// </summary>
        public Color Color;

        private bool hasSpawned;

        /// <summary>
        /// The path where this sky entity should try pulling its texture from. Used when automatically loading sky entity textures.
        /// </summary>
        public abstract string TexturePath { get; } 

        public virtual int MaxFrames => 1;

        /// <summary>
        /// Whether or not this sky entity should spawn regardless of the sky entity limit.
        /// </summary>
        public virtual bool ShouldSpawnRegardless => false;

        /// <summary>
        /// Whether or not this sky entity should immediately be removed upon its <see cref="Time"/> variable reaching its <see cref="Lifespan"/>.
        /// </summary>
        public virtual bool DieWithLifespan => true;

        /// <summary>
        /// The base draw position used in <see cref="GetDrawPositionBasedOnDepth"/>. Defaults to <see cref="Position"/>.
        /// </summary>
        /// <param name="positionOverride">An overrideable Vector2 parameter which can be used to change the base position if required.</param>
        public virtual Vector2 GetBaseDrawPosition(Vector2? positionOverride = null) => positionOverride ?? Position;

        /// <summary>
        /// The blend state the sky entity shall use when being drawn. Defaults to <see cref="BlendState.AlphaBlend"/>.
        /// </summary>
        public virtual BlendState BlendState => BlendState.AlphaBlend;

        /// <summary>
        /// Affects when the sky entity is drawn, whether before vanilla's background fog or after it.
        /// </summary>
        public virtual SkyEntityDrawContext DrawContext => SkyEntityDrawContext.BeforeBackgroundFog;

        /// <summary>
        /// Allows you to run code only once this sky entity spawn.
        /// </summary>
        public virtual void OnSpawn() { }

        /// <summary>
        /// Any code written here will be checked and ran every frame via <see cref="ModSystem.PostUpdateEverything"/>. 
        /// Use this method to update specific parts of your sky entity.
        /// </summary>
        public virtual void Update() { }

        /// <summary>
        /// Removes the sky entity from the database entirely.
        /// </summary>
        public virtual void Kill() => SkyEntityManager.ActiveSkyEntities.Remove(this);

        /// <summary>
        /// How the sky entity shall be drawn. This method is called automatically via a detour of 
        /// <see cref="Terraria.GameContent.Skies.AmbientSky.Draw(SpriteBatch, float, float)"/>.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public virtual void Draw(SpriteBatch spriteBatch) { }

        /// <summary>
        /// Calculates the position at which this sky entity should properly draw in the world based on its depth.
        /// </summary>
        /// <returns>A final position, being the properly calculated draw position subtracted by <see cref="Main.Camera"/>'s UnscaldPosition parameter.</returns>
        public Vector2 GetDrawPositionBasedOnDepth()
        {
            Vector2 drawPositionByDepth = (GetBaseDrawPosition() - Main.Camera.Center) * new Vector2(1f / Depth, 0.9f / Depth) + Main.Camera.Center;
            return drawPositionByDepth - Main.Camera.UnscaledPosition;
        }

        /// <summary>
        /// Spawns the specified <see cref="SkyEntity"/>.
        /// </summary>
        public void Spawn()
        {
            // Do not spawn any sky entities serverside.
            if (Main.netMode == NetmodeID.Server)
                return;
            
            // Determine whether this sky entity can spawn or not.
            bool canSpawn =
                !hasSpawned &&
                !Main.gamePaused &&
                (SkyEntityManager.ActiveSkyEntities?.Count < GraphicalConfig.Instance.SkyEntityLimit || ShouldSpawnRegardless);

            if (canSpawn)
            {
                // Mark this sky entity as active.
                Active = true;

                // Set the internal ID and stroed texture for this sky entity.
                ID = SkyEntityManager.SkyEntityIDs[GetType()];
                StoredTexture = SkyEntityManager.SkyEntityTextures[ID];

                // Add the sky entity to the list of currently active sky entities.
                SkyEntityManager.ActiveSkyEntities.Add(this);

                // Run any code that's meant to run on spawn.
                OnSpawn();

                // Set the position of the sky entity by it's depth.
                // This is what gives it its psuedo-3D movement.
                SetPositionByDepth(Position);

                // Mark the sky entity as spawned.
                hasSpawned = true;
            }
        }

        public void SetPositionByDepth(Vector2 worldPosition)
        {
            Vector2 vector = worldPosition - Main.Camera.Center;
            Vector2 depthPosition = Main.Camera.Center + vector * (Depth / 3f);
            Position = depthPosition;
        }
    }
}
