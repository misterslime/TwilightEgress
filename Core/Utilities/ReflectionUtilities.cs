using Terraria.ModLoader.Core;

namespace Cascade
{
    public static partial class Utilities
    {
        public static IEnumerable<Type> GetAllSubclassesOfType(Mod modToSearch, Type baseType)
        {
            Type[] loadableTypes = AssemblyManager.GetLoadableTypes(modToSearch.Code);
            foreach (Type type in loadableTypes)
            {
                if (!type.IsAbstract && type.IsSubclassOf(baseType) && type != baseType)
                    yield return type;
            }
        }
    }
}
