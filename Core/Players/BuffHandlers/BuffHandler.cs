using Cascade.Content.Cooldowns;

namespace Cascade.Core.Players.BuffHandlers
{
    public class BuffHandler : ModPlayer
    {
        public static List<bool> MechonSlayerBuffs { get; set; }

        private int MechonSlayerArtTime { get; set; }
        private readonly int MechonSlayerArtMaxTime = SecondsToFrames(5);

        public override void Initialize()
        {
            MechonSlayerBuffs = new List<bool>()
            {
                false,      // Armor.
                false,      // Eater.
                false,      // Enchant.
                false,      // Purge
                false       // Speed.
                            // -1 would stand for Mechon Slayer's default state.
            };
        }

        public static void StuffToUnload()
        {
            MechonSlayerBuffs = null;
        }

        public override void UpdateDead() => ResetAllVariables();

        public override void ResetEffects() => ResetAllVariables();

        public override void PostUpdateBuffs()
        {
            MechonSlayerBuffHandler();
        }

        public void MechonSlayerBuffHandler()
        {
            if (MechonSlayerBuffs[0])
            {
                Player.statDefense += 15;
                Player.Calamity().contactDamageReduction += 0.5D;
                Player.Calamity().projectileDamageReduction += 0.5D;

                Vector2 dustPosition = Player.Center + Main.rand.NextVector2Circular(Player.width, Player.height);
                Vector2 dustVelocity = Vector2.UnitY * Main.rand.NextFloat(-8f, -2f);
                Utilities.CreateDustLoop(2, dustPosition, dustVelocity, DustID.OrangeTorch);
            }

            if (MechonSlayerBuffs[1] && !Player.HasCooldown(MechonSlayerEater.ID))
            {
                // Remove all debuffs from the player.
                for (int i = 0; i < Player.MaxBuffs; i++)
                {
                    if (Player.buffType[i] <= 0 || !Main.debuff[Player.buffType[i]])
                        continue;

                    Player.buffTime[i] = 0;
                    Player.buffType[i] = 0;
                }

                // Apply a separate cooldown specifically for this effect.
                Player.AddCooldown(MechonSlayerEater.ID, SecondsToFrames(120));
            }

            if (MechonSlayerBuffs[2])
            {
                Player.GetDamage(Player.HeldItem.DamageType) += 0.10f;
                Player.GetArmorPenetration(Player.HeldItem.DamageType) += 10f;

                Vector2 dustPosition = Player.Center + Main.rand.NextVector2Circular(Player.width, Player.height);
                Vector2 dustVelocity = Vector2.UnitY * Main.rand.NextFloat(-8f, -2f);
                Utilities.CreateDustLoop(2, dustPosition, dustVelocity, DustID.PurpleTorch);
            }

            if (MechonSlayerBuffs[3])
            {
                Player.Calamity().contactDamageReduction += 0.10D;

                Vector2 dustPosition = Player.Center + Main.rand.NextVector2Circular(Player.width, Player.height);
                Vector2 dustVelocity = Vector2.UnitY * Main.rand.NextFloat(-8f, -2f);
                Utilities.CreateDustLoop(2, dustPosition, dustVelocity, DustID.GreenTorch);
            }

            if (MechonSlayerBuffs[4])
            {
                Player.maxRunSpeed *= 1.15f;
                Player.runAcceleration *= 1.05f;

                Vector2 dustPosition = Player.Center + Main.rand.NextVector2Circular(Player.width, Player.height);
                Vector2 dustVelocity = Vector2.UnitY * Main.rand.NextFloat(-8f, -2f);
                Utilities.CreateDustLoop(2, dustPosition, dustVelocity, DustID.IceTorch);
            }

            // Increment this timer in order to be used in ResetAllVariables.
            if (!Player.HasCooldown(MechonSlayerArtSelection.ID))
                MechonSlayerArtTime++;
        }

        public void ApplyMechonSlayerArt(int artID)
        {       
            if (Player.HasCooldown(MechonSlayerArtSelection.ID) || artID <= -1)
                return;

            // Disable all arts before renabling the art at the specified index.
            for (int i = 0; i < MechonSlayerBuffs.Count; i++)
                MechonSlayerBuffs[i] = false;
            MechonSlayerBuffs[artID] = true;

            if (artID != 1)
                Player.AddCooldown(MechonSlayerArtSelection.ID, SecondsToFrames(30));
            MechonSlayerArtTime = 0;
        }

        public void ResetAllVariables()
        {
            // Disable all art effects 5 seconds after the main cooldown has expired.
            if (MechonSlayerArtTime >= MechonSlayerArtMaxTime)
            {
                for (int i = 0; i < MechonSlayerBuffs.Count; i++)
                    MechonSlayerBuffs[i] = false;
            }          
        }
    }
}
