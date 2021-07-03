using FargowiltasSouls.Items.Misc;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class EridanusLegwear : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eridanus Legwear");
            Tooltip.SetDefault(@"5% increased damage
5% increased critical strike chance
10% increased movement speed");
            DisplayName.AddTranslation(GameCulture.Chinese, "宇宙英灵腿甲");
            Tooltip.AddTranslation(GameCulture.Chinese, "增加5%伤害\n增加5%暴击几率\n增加10%移动速度");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.rare = ItemRarityID.Purple;
            item.value = Item.sellPrice(0, 14);
            item.defense = 20;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetModPlayer<FargoPlayer>().AllDamageUp(0.05f);
            player.GetModPlayer<FargoPlayer>().AllCritUp(5);
            player.moveSpeed += 0.1f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<LunarCrystal>(), 5);
            recipe.AddTile(ModLoader.GetMod("Fargowiltas").TileType("CrucibleCosmosSheet"));
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
