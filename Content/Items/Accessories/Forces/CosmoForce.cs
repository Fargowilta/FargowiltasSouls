using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Materials;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Forces
{
    public class CosmoForce : BaseForce
    {
        public override void SetStaticDefaults()
        {
            Enchants[Type] = new int[]
            {
                ModContent.ItemType<MeteorEnchant>(),
                ModContent.ItemType<WizardEnchant>(),
                ModContent.ItemType<SolarEnchant>(),
                ModContent.ItemType<VortexEnchant>(),
                ModContent.ItemType<NebulaEnchant>(),
                ModContent.ItemType<StardustEnchant>()
            };
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            //meme speed, solar flare,
            SetActive(player);
            modPlayer.WizardEnchantActive = true;
            //meteor shower
            MeteorEnchant.AddEffects(player, Item);
            //solar shields
            player.AddEffect<SolarEffect>(Item);
            player.AddEffect<SolarFlareEffect>(Item);
            //stealth, voids, pet
            VortexEnchant.AddEffects(player, Item);
            //boosters
            player.AddEffect<NebulaEffect>(Item);
            //guardian and time freeze
            player.AddEffect<StardustMinionEffect>(Item);
            player.AddEffect<StardustEffect>(Item);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            foreach (int ench in Enchants[Type])
                recipe.AddIngredient(ench);

            recipe.AddIngredient(ModContent.ItemType<Eridanium>(), 5);

            recipe.AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"));
            recipe.Register();
        }
    }
}
