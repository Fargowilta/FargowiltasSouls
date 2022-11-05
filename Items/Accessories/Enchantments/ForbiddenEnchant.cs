using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class ForbiddenEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Forbidden Enchantment");

            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "禁戒魔石");

            string tooltip =
@"Double tap down to call an ancient storm to the cursor location
Any projectiles shot through your storm gain 30% damage
'Walk like an Egyptian'";
            Tooltip.SetDefault(tooltip);

            //             string tooltip_ch =
            // @"双击'下'键召唤远古风暴至光标位置
            // 穿过远古风暴的弹幕会获得30%额外伤害
            // '走路像个埃及人'";
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, tooltip_ch);
        }

        protected override Color nameColor => new Color(231, 178, 28);
        public override string wizardEffect => Language.GetTextValue("Mods.FargowiltasSouls.WizardEffect.Forbidden");

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Pink;
            Item.value = 150000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoSoulsPlayer>().ForbiddenEffect();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.AncientBattleArmorHat)
            .AddIngredient(ItemID.AncientBattleArmorShirt)
            .AddIngredient(ItemID.AncientBattleArmorPants)
            //sun mask/moon mask
            .AddIngredient(ItemID.DjinnsCurse)
            .AddIngredient(ItemID.SpiritFlame)
            .AddIngredient(ItemID.SkyFracture)
            //sky fracture
            //.AddIngredient(ItemID.RainbowRod);

            //recipe.AddRecipeGroup("FargowiltasSouls:AnyScorpion");
            //fennec fox pet

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
}
