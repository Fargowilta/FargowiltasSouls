using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class ChlorophyteEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Chlorophyte Enchantment");

            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "叶绿魔石");

            string tooltip =
@"Summons a ring of leaf crystals to shoot at nearby enemies
Grants a double spore jump
While using wings, spores will continuously spawn
Allows the ability to dash slightly
Double tap a direction
'The jungle's essence crystallizes around you'";
            Tooltip.SetDefault(tooltip);

            //             string tooltip_ch =
            // @"召唤一圈叶状水晶射击附近的敌人
            // 使你获得孢子二段跳能力
            // 使用翅膀进行飞行时会在你周围不断生成孢子
            // '丛林的精华凝结在你周围'";
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, tooltip_ch);
        }

        protected override Color nameColor => new Color(36, 137, 0);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Lime;
            Item.value = 150000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            //crystal
            modPlayer.ChloroEffect(Item, hideVisual);
            modPlayer.JungleEnchantActive = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddRecipeGroup("FargowiltasSouls:AnyChloroHead")
            .AddIngredient(ItemID.ChlorophytePlateMail)
            .AddIngredient(ItemID.ChlorophyteGreaves)
            .AddIngredient(null, "JungleEnchant")
            .AddIngredient(ItemID.ChlorophyteWarhammer)
            .AddIngredient(ItemID.ChlorophyteClaymore)
            //grape juice
            //.AddIngredient(ItemID.Seedling);
            //plantero pet

            .AddTile(TileID.CrystalBall)
           .Register();
        }
    }
}
