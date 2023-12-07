namespace Cascade.Content.Buffs.Debuffs
{
    public class BellbirdCry : ModBuff, ILocalizedModType
    {
        public new string LocalizationCategory => "Buffs.Debuffs";

        public override string Texture => "Terraria/Images/Buff_160";

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player Player, ref int buffIndex) => Player.Cascade_Buffs().BellbirdStun = true;

        public override void Update(NPC npc, ref int buffIndex) => npc.Cascade_Buffs().BellbirdStun = true;
    }
}
