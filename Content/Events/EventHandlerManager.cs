using Terraria.ModLoader.Core;
using System.Runtime.Serialization;

namespace Cascade.Content.Events
{
    public class EventHandlerManager : ModSystem
    {
        public static Dictionary<Type, EventHandler> Events;

        public override void OnModLoad()
        {
            Events = [];
            foreach (Type type in AssemblyManager.GetLoadableTypes(Cascade.Instance.Code))
            {
                if (type.IsSubclassOf(typeof(EventHandler)) && !type.IsAbstract)
                    Events[type] = Activator.CreateInstance(type) as EventHandler;
            }
        }

        public override void OnModUnload() => Events = null;

        /// <summary>
        /// Checks if the specified event is active or not.
        /// </summary>
        public static bool SpecificEventIsActive<T>() where T : EventHandler => Events[typeof(T)].EventIsActive;

        /// <summary>
        /// Starts the specified event.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void StartEvent<T>() where T : EventHandler
        {
            if (Events == null)
                return;

            Events.TryGetValue(typeof(T), out EventHandler worldEvent);
            worldEvent.Active = true;
        }

        /// <summary>
        /// Starts the specified event.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void StopEvent<T>() where T : EventHandler
        {
            if (Events == null)
                return;

            Events.TryGetValue(typeof(T), out EventHandler worldEvent);
            worldEvent.Active = false;
        }
    }
}
