using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class SpectreEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Spectre Enchantment");

            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "幽魂魔石");

            string tooltip =
@"Damage has a chance to spawn damaging orbs
If you crit, you might also get a healing orb
'Their lifeforce will be their undoing'";
            Tooltip.SetDefault(tooltip);

            //             string tooltip_ch =
            // @"伤害敌人时有几率生成幽魂珠
            // 攻击造成暴击时有几率生成治疗珠
            // '他们的生命力将毁灭他们自己'";
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, tooltip_ch);

        }

        protected override Color nameColor => new Color(172, 205, 252);
        public override string wizardEffect => Language.GetTextValue("Mods.FargowiltasSouls.WizardEffect.Spectre");

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Yellow;
            Item.value = 250000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoSoulsPlayer>().SpectreEffect(hideVisual);
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddRecipeGroup("FargowiltasSouls:AnySpectreHead")
            .AddIngredient(ItemID.SpectreRobe)
            .AddIngredient(ItemID.SpectrePants)
            //spectre wings
            .AddIngredient(ItemID.UnholyTrident)
            //nettle burst
            //.AddIngredient(ItemID.Keybrand);
            .AddIngredient(ItemID.SpectreStaff)
            .AddIngredient(ItemID.BatScepter)
            //bat scepter
            //.AddIngredient(ItemID.WispinaBottle);

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
}
