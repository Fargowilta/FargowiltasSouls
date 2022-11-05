using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class PlatinumEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Platinum Enchantment");
            string tooltip =
@"20% chance for enemies to drop 5x loot
'Its value is immeasurable'";
            Tooltip.SetDefault(tooltip);

            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "铂金魔石");
            //             string tooltip_ch = 
            // @"敌人死亡时掉落的战利品有20%几率翻倍
            // '价值无法估量'";
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, tooltip_ch);
        }

        protected override Color nameColor => new Color(83, 103, 143);
        public override string wizardEffect => Language.GetTextValue("Mods.FargowiltasSouls.WizardEffect.Platinum");

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.LightRed;
            Item.value = 100000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoSoulsPlayer>().PlatinumEnchantActive = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.PlatinumHelmet)
                .AddIngredient(ItemID.PlatinumChainmail)
                .AddIngredient(ItemID.PlatinumGreaves)
                .AddIngredient(ItemID.GardenGnome)
                .AddIngredient(ItemID.GemSquirrelDiamond)
                .AddIngredient(ItemID.LadyBug)

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}
