using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class BeeEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Bee Enchantment");

            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "蜜蜂魔石");

            string tooltip =
@"Increases the strength of friendly bees
Melee hits and most piercing attacks spawn bees
'According to all known laws of aviation, there is no way a bee should be able to fly'";
            Tooltip.SetDefault(tooltip);

            //             string tooltip_ch =
            // @"增加友好蜜蜂的力量
            // 穿透类弹幕在击中敌人时会生成蜜蜂
            // '根据目前所知的所有航空原理, 蜜蜂应该根本不可能会飞'";
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, tooltip_ch);
        }

        protected override Color nameColor => new Color(254, 246, 37);
        public override string wizardEffect => "";

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Orange;
            Item.value = 50000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoSoulsPlayer>().BeeEffect(hideVisual); //add effect
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ItemID.BeeHeadgear)
            .AddIngredient(ItemID.BeeBreastplate)
            .AddIngredient(ItemID.BeeGreaves)
            .AddIngredient(ItemID.HiveBackpack)
            //stinger necklace
            .AddIngredient(ItemID.BeeGun)
            //.AddIngredient(ItemID.WaspGun);
            //.AddIngredient(ItemID.Beenade, 50);
            //honey bomb
            .AddIngredient(ItemID.Honeyfin)
            //.AddIngredient(ItemID.Nectar);

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}
