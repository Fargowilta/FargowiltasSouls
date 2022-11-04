using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class ValhallaKnightEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Valhalla Knight Enchantment");
            Tooltip.SetDefault(
@"Increases the effectiveness of healing sources by 33%
Greatly enhances Ballista effectiveness
'Valhalla calls'");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "英灵殿骑士魔石");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, 
            // @"增加33%受治疗量
            // 大幅强化弩车的效果
            // '瓦尔哈拉的呼唤'");
        }

        protected override Color nameColor => new Color(147, 101, 30);
        public override string wizardEffect => "";

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Yellow;
            Item.value = 250000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoSoulsPlayer>().ValhallaEffect(hideVisual);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.SquireAltHead)
            .AddIngredient(ItemID.SquireAltShirt)
            .AddIngredient(ItemID.SquireAltPants)
            .AddIngredient(ItemID.VikingHelmet)
            .AddIngredient(null, "SquireEnchant")
            //.AddIngredient(ItemID.ShinyStone);
            //starlight
            //shadow lance
            .AddIngredient(ItemID.DD2SquireBetsySword)

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
}
