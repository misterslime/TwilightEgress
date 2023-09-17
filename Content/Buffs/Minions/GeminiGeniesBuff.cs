using Cascade.Content.Projectiles.Summoner.GeminiGenies;

namespace Cascade.Content.Buffs.Minions
{
    public class GeminiGeniesBuff : ModBuff, ILocalizedModType
    {
        public new string LocalizationCategory => "Buffs";

        public override string Texture => "CalamityMod/Buffs/Summon/SandyHealingWaifu";

        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.CascadePlayer_Minions().GeminiGenies = true;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<GeminiGenieSandy>()] < 0)
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}
