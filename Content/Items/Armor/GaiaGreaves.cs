using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class GaiaGreaves : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            // DisplayName.SetDefault("Gaia Greaves");
            /* Tooltip.SetDefault(@"10% increased damage for your current weapon class
5% increased critical strike chance
10% increased movement speed"); */
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(0, 5);
            Item.defense = 15;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.1f;
            player.GetCritChance(DamageClass.Generic) += 5;
            player.moveSpeed += 0.1f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.BeetleHusk, 3)
            .AddIngredient(ItemID.ShroomiteBar, 6)
            .AddIngredient(ItemID.SpectreBar, 6)
            .AddIngredient(ItemID.SpookyWood, 100)
            .AddTile(TileID.LunarCraftingStation)

            .Register();
        }
    }
}
