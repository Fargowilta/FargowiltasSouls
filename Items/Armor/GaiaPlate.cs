using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class GaiaPlate : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gaia Plate");
            Tooltip.SetDefault(@"10% increased damage
5% increased critical strike chance
Reduces damage taken by 10%
Increases your life regeneration");
            DisplayName.AddTranslation(GameCulture.Chinese, "盖亚板甲");
            Tooltip.AddTranslation(GameCulture.Chinese, "增加10%伤害\n增加5%暴击几率\n减少10%所受伤害\n增加生命恢复速度");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.rare = ItemRarityID.Yellow;
            item.value = Item.sellPrice(0, 6);
            item.defense = 20;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetModPlayer<FargoPlayer>().AllDamageUp(0.1f);
            player.GetModPlayer<FargoPlayer>().AllCritUp(5);
            player.endurance += 0.1f;
            player.lifeRegen += 2;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BeetleHusk, 6);
            recipe.AddIngredient(ItemID.ShroomiteBar, 9);
            recipe.AddIngredient(ItemID.SpectreBar, 9);
            recipe.AddIngredient(ItemID.SpookyWood, 150);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
