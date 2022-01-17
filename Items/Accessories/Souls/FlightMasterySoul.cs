using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Souls
{
    [AutoloadEquip(EquipType.Wings)]
    public class FlightMasterySoul : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("Flight Mastery Soul");
            Tooltip.SetDefault(
@"Allows for infinite flight
Hold DOWN and JUMP to hover
Allows the control of gravity
'Ascend'");
            DisplayName.AddTranslation(GameCulture.Chinese, "飞行大师之魂");
            Tooltip.AddTranslation(GameCulture.Chinese, 
@"使你获得无限飞行能力
按住'下'和'跳跃'键悬停
允许你控制重力
'飞升'");*/
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.value = 1000000;
            item.rare = ItemRarityID.Purple;
            ItemID.Sets.ItemNoGravity[item.type] = true;
        }
        public override Color? GetAlpha(Color lightColor) => Color.White;
        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color?(new Color(56, 134, 255));
                }
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();
            modPlayer.FlightMasterySoul();
        }

        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising,
            ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            player.wingsLogic = 22;
            ascentWhenFalling = 0.85f;
            ascentWhenRising = 0.25f;
            maxCanAscendMultiplier = 1f;
            maxAscentMultiplier = 3f;
            constantAscend = 0.135f;
        }

        public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
        {
            speed = 18f;
            acceleration *= 3.5f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            //soaring insignia
            //bat wings
            //fledgling wings
            recipe.AddIngredient(ItemID.FairyWings);
            recipe.AddIngredient(ItemID.HarpyWings);
            recipe.AddIngredient(ItemID.BoneWings);
            recipe.AddIngredient(ItemID.FrozenWings);
            recipe.AddIngredient(ItemID.FlameWings);
            recipe.AddIngredient(ItemID.TatteredFairyWings);
            recipe.AddIngredient(ItemID.FestiveWings);
            recipe.AddIngredient(ItemID.BetsyWings);
            recipe.AddIngredient(ItemID.FishronWings);
            //empress wings
            //celestial starboard

            recipe.AddIngredient(ItemID.GravityGlobe);

            recipe.AddTile(ModLoader.GetMod("Fargowiltas").TileType("CrucibleCosmosSheet"));

            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
