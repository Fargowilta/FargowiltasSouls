using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class DarkArtistEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("Dark Artist Enchantment");
            
            DisplayName.AddTranslation(GameCulture.Chinese, "暗黑艺术家魔石");
           
            string tooltip =
@"Summons a Flameburst minion that will travel to your mouse after charging up
It will then act as a sentry
After attacking for 2 seconds you will be enveloped in flames
Switching weapons will increase the next attack's damage by 50% and spawn an inferno
Greatly enhances Flameburst effectiveness
'The shadows hold more than they seem'";

            Tooltip.SetDefault(tooltip);
            string tooltip_ch =
@"召唤一个爆炸烈焰哨兵，在充能完毕后会移动至光标位置
然后将其算作一个哨兵
持续攻击两秒后你将被火焰包裹
切换武器后使下次攻击的伤害增加50%
大幅强化爆炸烈焰哨兵的效果
'阴影蕴含之物远超目之所及'";
            Tooltip.AddTranslation(GameCulture.Chinese, tooltip_ch);*/
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(155, 92, 176);
                }
            }
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            ItemID.Sets.ItemNoGravity[item.type] = true;
            item.rare = ItemRarityID.Yellow;
            item.value = 250000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();
            modPlayer.DarkArtistEffect(hideVisual);
            modPlayer.ApprenticeEffect();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.ApprenticeAltHead);
            recipe.AddIngredient(ItemID.ApprenticeAltShirt);
            recipe.AddIngredient(ItemID.ApprenticeAltPants);
            recipe.AddIngredient(null, "ApprenticeEnchant");
            recipe.AddIngredient(ItemID.DD2FlameburstTowerT3Popper);
            //recipe.AddIngredient(ItemID.ShadowbeamStaff);
            recipe.AddIngredient(ItemID.InfernoFork);
            //Razorpine
            //staff of earth
            //recipe.AddIngredient(ItemID.DD2PetGhost);

            recipe.AddTile(TileID.CrystalBall);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
