using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class CrystalAssassinEnchant : SoulsItem
    {
        public override bool Autoload(ref string name)
        {
            return false;
        }

        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("Crystal Assassin Enchantment");
            
            DisplayName.AddTranslation(GameCulture.Chinese, "水晶刺客魔石");
            
            string tooltip =
@"Effects of Volatile Gel
''";
            Tooltip.SetDefault(tooltip);
            string tooltip_ch =
@"拥有挥发明胶效果
''";
            Tooltip.AddTranslation(GameCulture.Chinese, tooltip_ch);*/
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(231, 178, 28); //change e
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
            item.value = 150000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //player.GetModPlayer<FargoPlayer>().ForbiddenEffect(); //effect tele on party girl bathwater, when tele slashes through enemies
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.AncientBattleArmorHat); //head
            recipe.AddIngredient(ItemID.AncientBattleArmorShirt); //body
            recipe.AddIngredient(ItemID.AncientBattleArmorPants); //legs
            //ninja enchant
            //volatile gel
            //magic dagger
            //flying knife
            //party gitl bathwater
            //hook of dissonance
            //qs mount

            recipe.AddTile(TileID.CrystalBall);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
