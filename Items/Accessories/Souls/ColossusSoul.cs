using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Souls
{
    //[AutoloadEquip(EquipType.Shield)]
    public class ColossusSoul : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("Colossus Soul");
            
            DisplayName.AddTranslation(GameCulture.Chinese, "巨像之魂");
            
            string tooltip =
@"Increases HP by 100
15% damage reduction
Increases life regeneration by 5
Grants immunity to knockback, several debuffs, and fall damage
Enemies are more likely to target you
Effects of Brain of Confusion, Star Veil, and Bee Cloak
Effects of Shiny Stone, Paladin's Shield, and Frozen Turtle Shell
'Nothing can stop you'";
            Tooltip.SetDefault(tooltip);

            string tooltip_ch =
@"增加100点最大生命值
增加15%伤害减免
增加5点生命恢复速度
使你免疫击退、一些减益和摔落伤害
增加敌人以你为目标的几率
拥有混乱之脑、星星面纱和蜜蜂斗篷效果
拥有闪亮石、圣骑士护盾和冰冻海龟壳效果
'你无人可挡'";
            Tooltip.AddTranslation(GameCulture.Chinese, tooltip_ch);*/

        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.defense = 10;
            item.value = 1000000;
            item.rare = ItemRarityID.Purple;
            item.shieldSlot = 4;
            ItemID.Sets.ItemNoGravity[item.type] = true;
        }
        public override Color? GetAlpha(Color lightColor) => Color.White;
        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color?(new Color(252, 59, 0));
                }
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();
            //any new effects, brain of confusion
            modPlayer.ColossusSoul(100, 0.15f, 5, hideVisual);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(ItemID.HandWarmer);
            recipe.AddIngredient(ItemID.ObsidianHorseshoe);
            recipe.AddIngredient(ItemID.WormScarf);
            recipe.AddIngredient(ItemID.BrainOfConfusion);
            recipe.AddIngredient(ItemID.PocketMirror);
            recipe.AddIngredient(ItemID.CharmofMyths);
            recipe.AddIngredient(ItemID.BeeCloak);
            recipe.AddIngredient(ItemID.StarVeil);
            recipe.AddIngredient(ItemID.FleshKnuckles); //hero shield
            recipe.AddIngredient(ItemID.ShinyStone);

            recipe.AddIngredient(ItemID.FrozenTurtleShell); //frozen shield
            recipe.AddIngredient(ItemID.PaladinsShield); // ^
            recipe.AddIngredient(ItemID.AnkhShield);

            recipe.AddTile(ModLoader.GetMod("Fargowiltas").TileType("CrucibleCosmosSheet"));

            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
