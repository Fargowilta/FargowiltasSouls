using FargowiltasSouls.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class StyxChestplate : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            // DisplayName.SetDefault("Styx Chestplate");
            /* Tooltip.SetDefault(@"15% increased damage
10% increased critical strike chance
Reduces damage taken by 10%
Increases your life regeneration"); */
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.rare = ItemRarityID.Purple;
            Item.value = Item.sellPrice(0, 25);
            Item.defense = 35;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.15f;
            player.GetCritChance(DamageClass.Generic) += 10;
            player.endurance += 0.1f;
            player.lifeRegen += 4;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.SoulofMight, 15)
            .AddIngredient(ItemID.LunarBar, 5)
            .AddIngredient(ModContent.ItemType<AbomEnergy>(), 10)
            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))

            .Register();
        }
    }
}