using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class GaiaGreaves : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gaia Greaves");
            Tooltip.SetDefault(@"10% increased damage
5% increased critical strike chance
10% increased movement speed");
            DisplayName.AddTranslation(GameCulture.Chinese, "盖亚护胫");
            Tooltip.AddTranslation(GameCulture.Chinese, "增加10%伤害\n增加5%暴击几率\n增加10%移动速度");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.rare = ItemRarityID.Yellow;
            item.value = Item.sellPrice(0, 5);
            item.defense = 15;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetModPlayer<FargoPlayer>().AllDamageUp(0.1f);
            player.GetModPlayer<FargoPlayer>().AllCritUp(5);
            player.moveSpeed += 0.1f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BeetleHusk, 3);
            recipe.AddIngredient(ItemID.ShroomiteBar, 6);
            recipe.AddIngredient(ItemID.SpectreBar, 6);
            recipe.AddIngredient(ItemID.SpookyWood, 100);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
