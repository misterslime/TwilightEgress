namespace Cascade
{
    public partial class Cascade
    {
        #region NPC Lists
        public static List<NPC> BasePlanetoidInheriters { get; set; }

        public static List<NPC> BaseAsteroidInheriters { get; set; }
        #endregion

        private static void LoadLists()
        {
            BasePlanetoidInheriters = new();
            BaseAsteroidInheriters = new();
        }

        private static void UnloadLists()
        {
            BasePlanetoidInheriters = null;
            BaseAsteroidInheriters = null;
        }
    }
}
