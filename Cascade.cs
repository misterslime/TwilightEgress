using CalamityMod.Items.Accessories;
using CalamityMod.Items.Placeables.FurnitureAuric;
using Cascade.Core.Players.BuffHandlers;

namespace Cascade
{
    public partial class Cascade : Mod
    {
        internal static Mod CalamityMod;

        internal static Cascade Instance { get; private set; }

        public override void Load()
        {
            Instance = this;
            CalamityMod = null;
            ModLoader.TryGetMod("CalamityMod", out CalamityMod);

            // Cascade-specific loading.
            LoadLists();
        }

        public override void Unload()
        {
            Instance = null;
            CalamityMod = null;
            MusicDisplay = null;
            UnloadLists();
            BuffHandler.StuffToUnload();
        }
    }
}