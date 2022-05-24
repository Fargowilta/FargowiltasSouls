using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class ShadowEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Shadow Enchantment");
            Tooltip.SetDefault(
@"Two Shadow Orbs will orbit around you
Attacking a Shadow Orb will cause it to release a burst of homing shadow energy
'You feel your body slip into the deepest of shadows'");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "暗影魔石");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, 
            // @"两颗暗影珠围绕着你旋转
            // 攻击暗影珠会使其释放追踪暗影能量
            // '你感觉你的身体堕入到了黑暗的深渊之中'");
        }

        protected override Color nameColor => new Color(66, 53, 111);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Orange;
            Item.value = 50000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoSoulsPlayer>().ShadowEffect(hideVisual);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.ShadowHelmet)
            .AddIngredient(ItemID.ShadowScalemail)
            .AddIngredient(ItemID.ShadowGreaves)
            .AddIngredient(ItemID.Musket)
            .AddIngredient(ItemID.WarAxeoftheNight)
            .AddIngredient(ItemID.ShadowOrb)

            //ball o hurt
            //demon bow
            //.AddIngredient(ItemID.PurpleClubberfish);
            //fisher of souls
            //.AddIngredient(ItemID.EatersBone);


            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}
