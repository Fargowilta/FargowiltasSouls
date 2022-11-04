using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class ShroomiteEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Shroomite Enchantment");

            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "蘑菇魔石");

            string tooltip =
@"All attacks gain trails of mushrooms
Not moving puts you in stealth
While in stealth, more mushrooms will spawn
'Made with real shrooms!'";
            Tooltip.SetDefault(tooltip);

            //             string tooltip_ch =
            // @"所有攻击都会留下蘑菇尾迹
            // 站定不动时使你进入隐身状态
            // 处于隐身状态时攻击会留下更多蘑菇尾迹
            // '真的是用蘑菇做的！'";
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, tooltip_ch);

        }

        protected override Color nameColor => new Color(0, 140, 244);
        public override string wizardEffect => "";

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Yellow;
            Item.value = 250000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoSoulsPlayer>().ShroomiteEffect(hideVisual);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddRecipeGroup("FargowiltasSouls:AnyShroomHead")
            .AddIngredient(ItemID.ShroomiteBreastplate)
            .AddIngredient(ItemID.ShroomiteLeggings)
            //shroomite digging
            //hammush
            .AddIngredient(ItemID.MushroomSpear)
            .AddIngredient(ItemID.Uzi)
            //venus magnum
            .AddIngredient(ItemID.TacticalShotgun)
            //.AddIngredient(ItemID.StrangeGlowingMushroom);

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
}
