using CalamityMod.Items.Accessories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cascade
{
    public partial class Cascade
    {
        public static bool IsBossRush { get => CalamityMod != null ? (bool)CalamityMod.Call("GetDifficultyActive", "bossrush") : false; }

        public static bool CanOverrideMusic(int npcID, bool overrideBossRush = false) => (!IsBossRush || overrideBossRush) && NPC.AnyNPCs(npcID);
    }
}
