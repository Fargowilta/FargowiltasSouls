using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Souls
{
    //[AutoloadEquip(EquipType.HandsOn, EquipType.HandsOff)]
    public class OlympiansSoul : SoulsItem
    {
        public override bool Autoload(ref string name)
        {
            return false;
        }

        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("Olympian's Soul");
            
            DisplayName.AddTranslation(GameCulture.Chinese, "奥林匹斯之魂");
            
            string tooltip =
@"30% increased throwing damage
20% increased throwing speed
15% increased throwing critical chance and velocity
'Strike with deadly precision'";
            Tooltip.SetDefault(tooltip);

            string tooltip_ch =
@"增加30%投掷伤害
增加20%投掷速度
增加15%投掷暴击率和投掷力度
'一击索命，避无可避'";
            Tooltip.AddTranslation(GameCulture.Chinese, tooltip_ch);*/

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
                    tooltipLine.overrideColor = new Color?(new Color(85, 5, 230));
                }
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //throw speed
            player.GetModPlayer<FargoPlayer>().ThrowSoul = true;
            //player.thrownDamage += 0.3f;
            //player.thrownCrit += 15;
            //player.thrownVelocity += 0.15f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(null, "SlingersEssence");

            /*
            else
            {
                recipe.AddIngredient(fargos.ItemType("MagicDaggerThrown"));
                recipe.AddIngredient(fargos.ItemType("BananarangThrown"), 5);
                recipe.AddIngredient(fargos.ItemType("AmarokThrown"));
                recipe.AddIngredient(fargos.ItemType("ShadowFlameKnifeThrown"));
                recipe.AddIngredient(fargos.ItemType("FlyingKnifeThrown"));
                recipe.AddIngredient(fargos.ItemType("LightDiscThrown"), 5);
                recipe.AddIngredient(fargos.ItemType("FlowerPowThrown"));
                recipe.AddIngredient(fargos.ItemType("ToxicFlaskThrown"));
                recipe.AddIngredient(fargos.ItemType("VampireKnivesThrown"));
                recipe.AddIngredient(fargos.ItemType("PaladinsHammerThrown"));
                recipe.AddIngredient(fargos.ItemType("PossessedHatchetThrown"));
                recipe.AddIngredient(fargos.ItemType("TheEyeOfCthulhuThrown"));
                recipe.AddIngredient(fargos.ItemType("TerrarianThrown"));
            }*/

            recipe.AddTile(ModLoader.GetMod("Fargowiltas").TileType("CrucibleCosmosSheet"));

            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
