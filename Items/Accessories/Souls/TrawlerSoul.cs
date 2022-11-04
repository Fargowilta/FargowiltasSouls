using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Souls
{
    public class TrawlerSoul : BaseSoul
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Trawler Soul");

            string tooltip =
@"Increases fishing skill substantially
All fishing rods will have 10 extra lures
You catch fish almost instantly
Permanent Sonar and Crate Buffs
Effects of Angler Tackle Bag, Volatile Gel, and Spore Sac 
Effects of Pink Horseshoe Balloon and Arctic Diving Gear
'The fish catch themselves'";
            Tooltip.SetDefault(tooltip);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.value = 750000;
        }

        protected override Color? nameColor => new Color(0, 238, 125);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            modPlayer.TrawlerSoul(Item, hideVisual);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(null, "AnglerEnchant")
            //inner tube
            .AddIngredient(ItemID.BalloonHorseshoeSharkron)
            .AddIngredient(ItemID.ArcticDivingGear)
            //frog gear
            .AddIngredient(ItemID.VolatileGelatin)
            .AddIngredient(ItemID.SporeSac)

            //engineer rod
            .AddIngredient(ItemID.SittingDucksFishingRod)
            //hotline fishing
            .AddIngredient(ItemID.GoldenFishingRod)
            .AddIngredient(ItemID.GoldenCarp)
            .AddIngredient(ItemID.ReaverShark)
            .AddIngredient(ItemID.Bladetongue)
            .AddIngredient(ItemID.ObsidianSwordfish)
            .AddIngredient(ItemID.FuzzyCarrot)
            .AddIngredient(ItemID.HardySaddle)

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))

            .Register();
        }
    }
}
