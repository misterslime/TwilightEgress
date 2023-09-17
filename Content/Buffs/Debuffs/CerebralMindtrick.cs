namespace Cascade.Content.Buffs.Debuffs
{
    public class CerebralMindtrick : ModBuff
    {
        public override string Texture => "Terraria/Images/Buff_321";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cerebral Mindtrick");
            // Description.SetDefault("Your head is spinning. You're beginning to see illusions!");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player Player, ref int buffIndex) => Player.Cascade_Debuff().CerebralMindtrick = true;
    }
}
