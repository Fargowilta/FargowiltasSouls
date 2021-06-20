using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Souls
{
    //[AutoloadEquip(EquipType.Back)]
    public class TrawlerSoul : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Trawler Soul");
            
            DisplayName.AddTranslation(GameCulture.Chinese, "垂钓者之魂");
            
            string tooltip =
@"Increases fishing skill substantially
All fishing rods will have 10 extra lures
You catch fish almost instantly
Permanent Sonar and Crate Buffs
Effects of Angler Tackle Bag and Spore Sac 
Effects of Pink Horseshoe Balloon and Arctic Diving Gear,
'The fish catch themselves'";
            Tooltip.SetDefault(tooltip);

            string tooltip_ch =
@"大幅增加渔力
钓竿会额外扔出10根鱼线
你几乎能立刻就钓到鱼
拥有声呐和宝匣效果
拥有渔夫渔具袋和孢子囊的效果
拥有粉马掌气球和北极潜水装备的效果
“愿者上钩”";
            Tooltip.AddTranslation(GameCulture.Chinese, tooltip_ch);

        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.value = 750000;
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
                    tooltipLine.overrideColor = new Color?(new Color(0, 238, 125));
                }
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();
            modPlayer.TrawlerSoul(hideVisual);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "AnglerEnchantment");
            //inner tube
            recipe.AddIngredient(ItemID.BalloonHorseshoeSharkron);
            recipe.AddIngredient(ItemID.ArcticDivingGear);
            //frog gear
            //volatile gel
            recipe.AddIngredient(ItemID.SporeSac);

            //engineer rod
            recipe.AddIngredient(ItemID.SittingDucksFishingRod);
            //hotline fishing
            recipe.AddIngredient(ItemID.GoldenFishingRod);
            recipe.AddIngredient(ItemID.GoldenCarp);
            recipe.AddIngredient(ItemID.ReaverShark);
            recipe.AddIngredient(ItemID.Bladetongue);
            recipe.AddIngredient(ItemID.ObsidianSwordfish);
            recipe.AddIngredient(ItemID.FuzzyCarrot);
            recipe.AddIngredient(ItemID.HardySaddle);
            //recipe.AddIngredient(ItemID.ZephyrFish);

            recipe.AddTile(ModLoader.GetMod("Fargowiltas").TileType("CrucibleCosmosSheet"));

            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
