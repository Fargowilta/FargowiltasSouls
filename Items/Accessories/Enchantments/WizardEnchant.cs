using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class WizardEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Wizard Enchantment");
            Tooltip.SetDefault(
@"Enhances the power of all other Enchantments to their Force effects
'I'm a what?'");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "巫师魔石");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese,
            // @"强化其它魔石，使它们获得在上级合成中才能获得的增强
            // （上级合成指 Forces/力）
            // '我是啥？'");
        }

        protected override Color nameColor => new Color(50, 80, 193);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.LightRed;
            Item.value = 100000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoSoulsPlayer>().WizardEnchantActive = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ItemID.WizardHat)
            //.AddIngredient(ItemID.AmethystRobe);
            //.AddIngredient(ItemID.TopazRobe);

            .AddIngredient(ItemID.SapphireRobe)
            .AddIngredient(ItemID.EmeraldRobe)
            .AddIngredient(ItemID.RubyRobe)
            .AddIngredient(ItemID.DiamondRobe)
            //amber robe
            .AddIngredient(ItemID.RareEnchantment)

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
}
