using Terraria.ModLoader.Core;
using System.Runtime.Serialization;

namespace Cascade.Content.Events
{
    public class EventHandlerManager : ModSystem
    {
        public static Dictionary<Type, EventHandler> Events;

        public static bool SpecificEventIsActive<T>() where T : EventHandler => Events[typeof(T)].EventIsActive;

        public override void OnModLoad()
        {
            Events = new();
            foreach (Type type in AssemblyManager.GetLoadableTypes(Cascade.Instance.Code))
            {
                if (type.IsSubclassOf(typeof(EventHandler)) && !type.IsAbstract)
                    Events[type] = (EventHandler)FormatterServices.GetUninitializedObject(type);
            }
        }

        public override void OnModUnload() => Events = null;
    }
}
