using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace Cascade.Core.ILEditing
{
    public static class HookManager
    {
        public static List<IHookEdit> HookEdits { get; private set; } = new();

        public static List<ILHook> ILHooks { get; set; } = new();

        public static void LoadEdits()
        {
            foreach (Type type in Cascade.Instance.Code.GetTypes())
            {
                if (!type.IsAbstract && type.GetInterfaces().Contains(typeof(IHookEdit)))
                {
                    IHookEdit edit = (IHookEdit)FormatterServices.GetUninitializedObject(type);
                    edit.Load();
                    HookEdits.Add(edit);
                }
            }
        }

        public static void UnloadEdits()
        {
            foreach (IHookEdit edit in HookEdits)
                edit.Unload();

            foreach (ILHook hook in ILHooks)
                hook?.Undo();
        }

        public static void ModifyHook(MethodBase method, ILContext.Manipulator hookModification)
        {
            ILHooks ??= new();

            ILHook hook = new(method, hookModification);
            hook.Apply();
            ILHooks.Add(hook);
        }

        public static void FuckYou()
        {
            // why
        }
    }
}
