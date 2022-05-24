﻿using FargowiltasSouls.Projectiles;
using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class AncientCobaltEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Ancient Cobalt Enchantment");
            Tooltip.SetDefault(
@"Grants an explosion jump
'Ancient Kobold'");

            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "远古钴魔石");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese,
            // @"你的弹幕有20%几率爆裂成毒刺
            // 此效果在每秒内只会发生一次
            // '古老的丛林赋予你力量'");
        }

        protected override Color nameColor => new Color(53, 76, 116);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Orange;
            Item.value = 50000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            AncientCobaltEffect(player, Item, 100);
        }

        public static void AncientCobaltEffect(Player player, Item item, int damage)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            if (player.jump <= 0 && player.velocity.Y == 0f)
            {
                modPlayer.CanCobaltJump = true;
                modPlayer.JustCobaltJumped = false;
            }
            else
            {
                modPlayer.CanCobaltJump = false;
            }

            if (player.controlJump && player.releaseJump && player.GetToggleValue("AncientCobalt") && modPlayer.CanCobaltJump && !modPlayer.JustCobaltJumped)
            {
                int projType = ModContent.ProjectileType<CobaltExplosion>();

                if (modPlayer.CobaltEnchantItem != null)
                {
                    projType = ModContent.ProjectileType<Explosion>();
                }

                Projectile.NewProjectile(player.GetSource_Accessory(item), player.Center, Vector2.Zero, projType, damage, 0, player.whoAmI);

                modPlayer.JustCobaltJumped = true;
            }

            if (modPlayer.CanCobaltJump || (modPlayer.JustCobaltJumped && !player.isPerformingJump_Cloud && !player.isPerformingJump_Blizzard && !player.isPerformingJump_Fart && !player.isPerformingJump_Sail && !player.isPerformingJump_Sandstorm && !modPlayer.JungleJumping))
            {
                if (modPlayer.CobaltEnchantItem != null)
                {
                    player.jumpSpeedBoost += 8f;
                }
                else
                {
                    player.jumpSpeedBoost += 5f;
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ItemID.AncientCobaltHelmet)
            .AddIngredient(ItemID.AncientCobaltBreastplate)
            .AddIngredient(ItemID.AncientCobaltLeggings)
            .AddIngredient(ItemID.Bomb, 10)
            .AddIngredient(ItemID.Dynamite, 10)
            .AddIngredient(ItemID.Grenade, 10)

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}
