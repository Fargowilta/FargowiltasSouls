using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class MeteorEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Meteor Enchantment");

            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "陨星魔石");

            string tooltip =
@"A meteor shower initiates every few seconds while attacking
'Drop a draco on 'em'";
            Tooltip.SetDefault(tooltip);

            //             string tooltip_ch =
            // @"攻击时每过几秒便会释放一次流星雨
            // '宇宙之力构建你的毁灭力量'";
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, tooltip_ch);
        }

        protected override Color nameColor => new Color(95, 71, 82);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Pink;
            Item.value = 100000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            modPlayer.MeteorEffect();
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ItemID.MeteorHelmet)
            .AddIngredient(ItemID.MeteorSuit)
            .AddIngredient(ItemID.MeteorLeggings)
            .AddIngredient(ItemID.SpaceGun)
            .AddIngredient(ItemID.SuperStarCannon)
            .AddIngredient(ItemID.MeteorStaff)

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
}
