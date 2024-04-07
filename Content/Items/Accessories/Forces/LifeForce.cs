using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Core.AccessoryEffectSystem;

namespace FargowiltasSouls.Content.Items.Accessories.Forces
{
    public class LifeForce : BaseForce
    {
        public override void SetStaticDefaults()
        {
            Enchants[Type] = new int[]
            {
                ModContent.ItemType<PumpkinEnchant>(),
                ModContent.ItemType<BeeEnchant>(),
                ModContent.ItemType<SpiderEnchant>(),
                ModContent.ItemType<TurtleEnchant>(),
                ModContent.ItemType<BeetleEnchant>()
            };
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            SetActive(player);
            player.AddEffect<BeeEffect>(Item);
            player.AddEffect<SpiderEffect>(Item);
            BeetleEnchant.AddEffects(player, Item);
            player.AddEffect<PumpkinEffect>(Item);
            TurtleEnchant.AddEffects(player, Item);
            modPlayer.CactusImmune = true;
            player.AddEffect<CactusEffect>(Item);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            foreach (int ench in Enchants[Type])
                recipe.AddIngredient(ench);
            recipe.AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"));
            recipe.Register();
        }
    }
}
