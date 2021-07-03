using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Misc
{
    public class RabiesVaccine : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rabies Vaccine");
            Tooltip.SetDefault(@"Permanently grants immunity to Feral Bite");
            DisplayName.AddTranslation(GameCulture.Chinese, "狂犬病疫苗");
            Tooltip.AddTranslation(GameCulture.Chinese, @"永久免疫野性咬噬");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = ItemRarityID.Orange;
            item.maxStack = 1;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.useAnimation = 17;
            item.useTime = 17;
            item.consumable = true;
            item.UseSound = SoundID.Item3;
        }

        public override bool CanUseItem(Player player)
        {
            return !player.GetModPlayer<FargoPlayer>().RabiesVaccine;
        }

        public override bool UseItem(Player player)
        {
            if (player.itemAnimation > 0 && player.itemTime == 0)
            {
                player.GetModPlayer<FargoPlayer>().RabiesVaccine = true;
            }

            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(ModContent.ItemType<RabiesShot>(), 30);

            recipe.AddTile(TileID.AlchemyTable);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
