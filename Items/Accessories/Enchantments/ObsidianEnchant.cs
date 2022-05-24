using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class ObsidianEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Obsidian Enchantment");
            Tooltip.SetDefault(
@"
'The earth calls'");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "黑曜石魔石");
            //@"使你免疫火与岩浆
            //使你可以在岩浆中正常移动和游泳
            //在岩浆中时，你的攻击会引发爆炸
            //'大地的呼唤'"); e
        }

        protected override Color nameColor => new Color(69, 62, 115);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Orange;
            Item.value = 50000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoSoulsPlayer>().ObsidianEffect(); //add effect
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.ObsidianHelm)
            .AddIngredient(ItemID.ObsidianShirt)
            .AddIngredient(ItemID.ObsidianPants)
            .AddIngredient(ItemID.ObsidianRose) //molten skull rose
            //.AddIngredient(ItemID.ObsidianHorseshoe);
            .AddIngredient(ItemID.Cascade)
            .AddIngredient(ItemID.Fireblossom)
            //magma snail
            //obsidifsh
            //mimic pet

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}
