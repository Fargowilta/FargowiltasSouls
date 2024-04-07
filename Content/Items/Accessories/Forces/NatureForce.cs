using Fargowiltas.Items.Tiles;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Forces
{
    public class NatureForce : BaseForce
    {
        public override void SetStaticDefaults()
        {
            Enchants[Type] = new int[]
            {
                ModContent.ItemType<CrimsonEnchant>(),
                ModContent.ItemType<MoltenEnchant>(),
                ModContent.ItemType<RainEnchant>(),
                ModContent.ItemType<FrostEnchant>(),
                ModContent.ItemType<ChlorophyteEnchant>(),
                ModContent.ItemType<ShroomiteEnchant>()
            };
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            SetActive(player);

            player.AddEffect<CrimsonEffect>(Item);
            player.AddEffect<MoltenEffect>(Item);
            RainEnchant.AddEffects(player, Item);
            player.AddEffect<FrostEffect>(Item);
            player.AddEffect<SnowEffect>(Item);
            ChlorophyteEnchant.AddEffects(player, Item);
            player.AddEffect<ShroomiteStealthEffect>(Item);
            player.AddEffect<ShroomiteShroomEffect>(Item);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            foreach (int ench in Enchants[Type])
                recipe.AddIngredient(ench);
            recipe.AddTile<CrucibleCosmosSheet>();
            recipe.Register();
        }
    }
}
