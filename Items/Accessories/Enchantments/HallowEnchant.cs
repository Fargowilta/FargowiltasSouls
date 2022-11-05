using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class HallowEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Hallowed Enchantment");

            Tooltip.SetDefault(
@"Become immune after striking an enemy
'Hit me with your best shot'");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "神圣魔石");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, 
            // @"使你获得一面可以反弹弹幕的盾牌
            // 召唤一柄附魔剑，附魔剑的伤害取决于你的召唤伤害
            // '愿人都尊你的剑与盾为圣'");
        }

        protected override Color nameColor => new Color(150, 133, 100);
        public override string wizardEffect => Language.GetTextValue("Mods.FargowiltasSouls.WizardEffect.Hallow");

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.LightPurple;
            Item.value = 180000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            HallowEffect(player);
        }

        public static void HallowEffect(Player player)
        {
            if (player.GetToggleValue("HallowDodge"))
                player.onHitDodge = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("FargowiltasSouls:AnyHallowHead")
                .AddIngredient(ItemID.HallowedPlateMail)
                .AddIngredient(ItemID.HallowedGreaves)
                .AddIngredient(ItemID.HallowJoustingLance)
                .AddIngredient(ItemID.RainbowRod)
                .AddIngredient(ItemID.MajesticHorseSaddle)

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
}
