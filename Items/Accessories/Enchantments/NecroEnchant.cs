using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class NecroEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("Necro Enchantment");
            Tooltip.SetDefault(
@"Slain enemies may drop a pile of bones
Touch a pile of bones to spawn a friendly Dungeon Guardian
Damage scales with the defeated enemy's max HP
Bosses will drop bones every 10% of their HP lost
'Welcome to the bone zone'");
            DisplayName.AddTranslation(GameCulture.Chinese, "死灵魔石");
            Tooltip.AddTranslation(GameCulture.Chinese,
@"击杀敌人时有几率爆出一摞骨头
拾取骨头时有几率生成一个地牢守卫
地牢守卫的伤害取决于被击杀的敌人的最大生命值
Boss每损失10%生命值便会掉落骨头
'欢迎来到骸骨领域'");*/
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(86, 86, 67);
                }
            }
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            ItemID.Sets.ItemNoGravity[item.type] = true;
            item.rare = ItemRarityID.Orange;
            item.value = 50000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoPlayer>().NecroEffect(hideVisual);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.NecroHelmet);
            recipe.AddIngredient(ItemID.NecroBreastplate);
            recipe.AddIngredient(ItemID.NecroGreaves);
            recipe.AddIngredient(ItemID.BoneGlove);
            recipe.AddIngredient(ItemID.BookofSkulls); //spinal tap?
            //quad barrel shotgun
            //maggot
            recipe.AddIngredient(ItemID.TheGuardiansGaze);
            //recipe.AddIngredient(ItemID.BoneKey);

            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
