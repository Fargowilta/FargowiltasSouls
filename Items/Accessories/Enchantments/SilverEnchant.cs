using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using FargowiltasSouls.Toggler;
using FargowiltasSouls.Projectiles.Minions;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class SilverEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Silver Enchantment");
            
            DisplayName.AddTranslation(GameCulture.Chinese, "银魔石");
            
            string tooltip =
@"Summons a sword familiar that scales with minion damage
Drastically increases minion speed
'Have you power enough to wield me?'";
            Tooltip.SetDefault(tooltip);

            string tooltip_ch =
@"召唤一柄剑，剑的伤害接受召唤伤害加成
“你有足够的力量驾驭我吗？”";
            Tooltip.AddTranslation(GameCulture.Chinese, tooltip_ch);

        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(180, 180, 204);
                }
            }
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            ItemID.Sets.ItemNoGravity[item.type] = true;
            item.rare = ItemRarityID.Blue;
            item.value = 30000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();
            modPlayer.SilverEnchant = true;
            modPlayer.AddMinion(player.GetToggleValue("Silver"), ModContent.ProjectileType<SilverSword>(), (int)(20 * player.minionDamage), 0f);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(ItemID.SilverHelmet);
            recipe.AddIngredient(ItemID.SilverChainmail);
            recipe.AddIngredient(ItemID.SilverGreaves);
            recipe.AddIngredient(ItemID.SilverBroadsword);
            //recipe.AddIngredient(ItemID.SilverBow);
            recipe.AddIngredient(ItemID.SapphireStaff);
            recipe.AddIngredient(ItemID.BluePhaseblade);
            //leather whip
            //recipe.AddIngredient(ItemID.TreeNymphButterfly);
            //roasted duck

            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
