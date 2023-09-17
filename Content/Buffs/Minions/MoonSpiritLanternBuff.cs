using Cascade.Content.DedicatedContent.MPG;

namespace Cascade.Content.Buffs.Minions
{
    public class MoonSpiritLanternBuff : ModBuff, ILocalizedModType
    {
        public new string LocalizationCategory => "Buffs";

        public override string Texture => "Terraria/Images/Buff";

        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.CascadePlayer_Minions().MoonSpiritLantern = true;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<UnderworldLantern>()] < 1)
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}
