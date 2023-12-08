namespace Cascade.Core.Globals.GlobalNPCs
{
    public partial class CascadeGlobalNPC
    {
        public delegate void EditSpawnPoolDelegate(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo);
        public static event EditSpawnPoolDelegate EditSpawnPoolEvent;

        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            EditSpawnPoolEvent?.Invoke(pool, spawnInfo);
        }


    }
}
