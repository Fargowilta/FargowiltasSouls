using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class RainEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Rain Enchantment");
            Tooltip.SetDefault(
@"Grants immunity to Wet
Spawns a miniature storm to follow you around
Shooting it will make it grow
At maximum size, attacks will turn into lightning bolts
'Come again some other day'");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "雨云魔石");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, 
            // @"使你免疫潮湿减益
            // 召唤一个微型风暴跟着你
            // 向其射击会使其变大
            // 尺寸达到最大时攻击会转化为闪电
            // '改日再来'");
        }

        protected override Color nameColor => new Color(255, 236, 0);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.LightPurple;
            Item.value = 150000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoSoulsPlayer>().RainEffect(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ItemID.RainHat)
            .AddIngredient(ItemID.RainCoat)
            .AddIngredient(ItemID.UmbrellaHat)
            //inner tube
            .AddIngredient(ItemID.Umbrella)
            //tragic umbrella
            .AddIngredient(ItemID.NimbusRod)
            .AddIngredient(ItemID.WaterGun)
            //.AddIngredient(ItemID.RainbowBrick, 50);
            //volt bunny pet

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
}
