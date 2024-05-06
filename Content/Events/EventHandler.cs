using Cascade.Core.Globals.GlobalNPCs;
using Terraria.ModLoader.IO;

namespace Cascade.Content.Events
{
    public abstract class EventHandler : ModSystem
    {
        /// <summary>
        /// Whether the event is active or not.
        /// </summary>
        public bool Active;

        /// <summary>
        /// The condition which determins whether or not the event is active.
        /// </summary>
        public bool EventIsActive
        {
            get
            {
                if (!Active)
                    return false;
                return true;
            }
        }

        /// <summary>
        /// Whether or not the event should remain active after leaving and rejoining the world.
        /// </summary>
        public virtual bool PersistAfterLeavingWorld { get; }

        public sealed override void OnModLoad()
        {
            CascadeGlobalNPC.EditSpawnPoolEvent += EditSpawnPool;
            CascadeGlobalNPC.EditSpawnRateEvent += EditSpawnRate;
            SafeOnModLoad();
        }

        public sealed override void OnModUnload()
        {
            CascadeGlobalNPC.EditSpawnPoolEvent -= EditSpawnPool;
            CascadeGlobalNPC.EditSpawnRateEvent -= EditSpawnRate;
            SafeOnModUnload();
        }

        public override void SaveWorldData(TagCompound tag)
        {
            if (PersistAfterLeavingWorld && Active)
                tag[$"{Name}.Active"] = Active;
            SafeSaveWorldData(tag);
        }

        public override void LoadWorldData(TagCompound tag)
        {
            Active = tag.GetBool($"{Name}.Active");
            SafeLoadWorldData(tag);
        }

        public override void OnWorldLoad() => ResetEventStuff();

        public override void OnWorldUnload() => ResetEventStuff();

        public override void PostUpdateEverything()
        {
            PreUpdateEvent();
            if (EventIsActive && PreUpdateEvent())
                UpdateEvent();
        }

        private void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            if (!PreUpdateEvent() || !EventIsActive)
                return;
            EditEventSpawnPool(pool, spawnInfo);
        }

        private void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            if (!PreUpdateEvent() || !EventIsActive)
                return;
            EditEventSpawnRate(player, ref spawnRate , ref maxSpawns);  
        }

        /// <summary>
        /// Runs in <see cref="ModSystem.OnModLoad"/>. Override this to perform specific loading for your event.
        /// </summary>
        public virtual void SafeOnModLoad() { }

        /// <summary>
        /// Runs in <see cref="ModSystem.OnModUnload"/> Override this to unload things that you loaded in <see cref="SafeOnModLoad"/>.
        /// </summary>
        public virtual void SafeOnModUnload() { }

        /// <summary>
        /// Runs in <see cref="ModSystem.SaveWorldData(TagCompound)"/>. Override this if you need any bits of data saved.
        /// </summary>
        /// <param name="tag"></param>
        public virtual void SafeSaveWorldData(TagCompound tag) { }

        /// <summary>
        /// Runs in <see cref="ModSystem.LoadWorldData(TagCompound)"/> Override this to load whatever you saved in <see cref="SafeSaveWorldData(TagCompound)"/>.
        /// </summary>
        /// <param name="tag"></param>
        public virtual void SafeLoadWorldData(TagCompound tag) { }

        /// <summary>
        /// Runs before your event is active. If this returns false, your event will never be updated.
        /// </summary>
        /// <returns>Whether or not this event updates.</returns>
        public virtual bool PreUpdateEvent() => true;

        /// <summary>
        /// Override to run any code related to your custom event.
        /// </summary>
        public virtual void UpdateEvent() { }

        /// <summary>
        /// Allows you to directly edit the spawn pool for this event.
        /// </summary>
        /// <param name="pool">The current spawn pool for the location. Use this in tandem with <see cref="NPCSpawnInfo"/>.</param>
        /// <param name="spawnInfo">Contains specific information regarding where an NPC spawns and the player it spawns around.</param>
        public virtual void EditEventSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo) { }

        /// <summary>
        /// Allows you to directly edit the spawn rate of NPCs for this event.
        /// </summary>
        /// <param name="spawnRate">The rate at which NPCs spawn around the player.</param>
        /// <param name="maxSpawns">The maximum amount of NPCs that can be active during this time.</param>
        public virtual void EditEventSpawnRate(Player player, ref int spawnRate, ref int maxSpawns) { }

        /// <summary>
        /// Override to place any code related to your custom event that should be reset upon loading and unloading a world. 
        /// </summary>
        public virtual void ResetEventStuff() { }
    }
}
