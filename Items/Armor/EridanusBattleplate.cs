using FargowiltasSouls.Items.Misc;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class EridanusBattleplate : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eridanus Battleplate");
            Tooltip.SetDefault(@"10% increased damage
10% increased critical strike chance
Reduces damage taken by 10%
Grants life regeneration");
            DisplayName.AddTranslation(GameCulture.Chinese, "宇宙英灵板甲");
            Tooltip.AddTranslation(GameCulture.Chinese, "增加10%伤害\n增加10%暴击几率\n减少10%所受伤害\n增加生命恢复速度"); 
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.rare = ItemRarityID.Purple;
            item.value = Item.sellPrice(0, 20);
            item.defense = 30;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetModPlayer<FargoPlayer>().AllDamageUp(0.1f);
            player.GetModPlayer<FargoPlayer>().AllCritUp(10);
            player.endurance += 0.1f;
            player.lifeRegen += 4;
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
