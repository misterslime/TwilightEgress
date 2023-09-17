﻿using CalamityMod.Items;
using Cascade.Content.Buffs.Minions;
using Cascade.Content.Projectiles.Summoner.GeminiGenies;

namespace Cascade.Content.Items.Accessories.Summoner
{
    public class GeminiGenieLamp : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";

        public override string Texture => "Terraria/Images/Item_" + ItemID.SpiritFlame;

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 18;
            Item.accessory = true;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.CascadePlayer_Minions().GeminiGenies = true;
            if (player.FindBuffIndex(ModContent.BuffType<GeminiGeniesBuff>()) == -1)
                player.AddBuff(ModContent.BuffType<GeminiGeniesBuff>(), 2);

            int damage = (int)player.GetTotalDamage<SummonDamageClass>().ApplyTo(175);
            if (player.ownedProjectileCounts[ModContent.ProjectileType<GeminiGenieSandy>()] < 1)
            {
                int p = Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<GeminiGenieSandy>(), damage, 4f, player.whoAmI);
                if (Main.projectile.IndexInRange(p))
                    Main.projectile[p].originalDamage = damage;
            }

            if (player.ownedProjectileCounts[ModContent.ProjectileType<GeminiGeniePsychic>()] < 1)
            {
                int p = Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<GeminiGeniePsychic>(), damage, 4f, player.whoAmI);
                if (Main.projectile.IndexInRange(p))
                    Main.projectile[p].originalDamage = damage;
            }
        }

        public override void UpdateVanity(Player player)
        {
            player.CascadePlayer_Minions().GeminiGeniesVanity = true;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<GeminiGenieSandy>()] < 1)
                Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<GeminiGenieSandy>(), 0, 0f, player.whoAmI);
            if (player.ownedProjectileCounts[ModContent.ProjectileType<GeminiGeniePsychic>()] < 1)
                Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<GeminiGeniePsychic>(), 0, 0f, player.whoAmI);
        }
    }
}
