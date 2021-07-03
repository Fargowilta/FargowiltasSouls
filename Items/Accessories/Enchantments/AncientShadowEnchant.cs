using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class AncientShadowEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ancient Shadow Enchantment");
            Tooltip.SetDefault(
@"Your attacks may inflict Darkness on enemies
Darkened enemies occasionally fire shadowflame tentacles at other enemies
Three Shadow Orbs will orbit around you
'Archaic, yet functional'");
            DisplayName.AddTranslation(GameCulture.Chinese, "远古暗影魔石");
            Tooltip.AddTranslation(GameCulture.Chinese, 
@"攻击有几率造成黑暗减益
身上带有黑暗减益的敌人偶尔会向其他敌人发射暗影焰触手
三颗暗影珠围绕着你旋转
攻击暗影珠会使其释放追踪暗影能量
“十分古老，却非常实用”");
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(94, 85, 220);
                }
            }
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            ItemID.Sets.ItemNoGravity[item.type] = true;
            item.rare = ItemRarityID.Pink;
            item.value = 100000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();
            modPlayer.AncientShadowEffect();
            modPlayer.ShadowEffect(hideVisual);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(ItemID.AncientShadowHelmet);
            recipe.AddIngredient(ItemID.AncientShadowScalemail);
            recipe.AddIngredient(ItemID.AncientShadowGreaves);
            //recipe.AddIngredient(ItemID.AncientNecroHelmet);
            //recipe.AddIngredient(ItemID.AncientGoldHelmet);
            recipe.AddIngredient(null, "ShadowEnchant");
            recipe.AddIngredient(ItemID.ShadowFlameKnife);
            recipe.AddIngredient(ItemID.ShadowFlameHexDoll);
            //dart rifle
            //toxicarp

            recipe.AddTile(TileID.CrystalBall);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
