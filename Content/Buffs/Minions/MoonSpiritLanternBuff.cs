using Cascade.Content.DedicatedContent.MPG;

namespace Cascade.Content.Buffs.Minions
{
    public class MoonSpiritLanternBuff : ModBuff
    {
        public override string Texture => "Terraria/Images/Buff";

        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.CascadePlayer_Minions().MoonSpiritLantern = true;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<UnderworldLantern>()] < 0)
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}
