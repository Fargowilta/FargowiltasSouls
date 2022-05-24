using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class GoldEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Gold Enchantment");
            string tooltip =
@"Your attacks inflict Midas
Automatically sends coins to your piggy bank when picked up
Press the Gold hotkey to be encased in a Golden Shell
You will not be able to move or attack, but will be immune to all damage
Press again to exit early
Effects of Greedy Ring
'Gold makes the world go round'";
            Tooltip.SetDefault(tooltip);

            //             attacks spawn coins, collect them to reduce cooldown?

            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "金魔石");
            //             string tooltip_ch =
            // @"攻击会造成迈达斯减益
            // 按下'金身'键后会将你包裹在一个黄金壳中
            // 被包裹时你无法移动或攻击，但你免疫所有伤害
            // 再次按下'金身'键会使你提前离开黄金壳
            // 拥有贪婪戒指效果
            // '黄金使世界运转'";
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, tooltip_ch);

        }

        protected override Color nameColor => new Color(231, 178, 28);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Pink;
            Item.value = 150000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            modPlayer.GoldEffect(hideVisual);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.GoldHelmet)
            .AddIngredient(ItemID.GoldChainmail)
            .AddIngredient(ItemID.GoldGreaves)
            .AddIngredient(ItemID.GoldCrown)
            .AddIngredient(ItemID.GreedyRing)
            //.AddIngredient(ItemID.CoinGun);
            .AddIngredient(ItemID.SquirrelGold)
            //gold goldfish
            //ruby bunny
            //.AddIngredient(ItemID.ParrotCracker);

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
}
