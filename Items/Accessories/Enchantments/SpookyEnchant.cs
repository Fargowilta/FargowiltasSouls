using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class SpookyEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Spooky Enchantment");
            Tooltip.SetDefault(
@"All of your minions gain an extra scythe attack
'Melting souls since 1902'");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "阴森魔石");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, 
            // @"你的召唤物获得了额外的镰刀攻击
            // '自1902年以来融化的灵魂'");
        }

        protected override Color nameColor => new Color(100, 78, 116);
        public override string wizardEffect => Language.GetTextValue("Mods.FargowiltasSouls.WizardEffect.Spooky");

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Yellow;
            Item.value = 250000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoSoulsPlayer>().SpookyEffect(hideVisual);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.SpookyHelmet)
            .AddIngredient(ItemID.SpookyBreastplate)
            .AddIngredient(ItemID.SpookyLeggings)
            .AddIngredient(ItemID.ButchersChainsaw)
            .AddIngredient(ItemID.DeathSickle)
            .AddIngredient(ItemID.RavenStaff)

            //psycho knife
            //eoc yoyo
            //dark harvest
            //.AddIngredient(ItemID.CursedSapling);
            //.AddIngredient(ItemID.EyeSpring);

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
}
