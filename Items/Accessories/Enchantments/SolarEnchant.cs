using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class SolarEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Solar Enchantment");
            Tooltip.SetDefault(
@"Solar shield allows you to dash through enemies
Solar shield is not depleted on hit, but has reduced damage reduction
Attacks may inflict the Solar Flare debuff
'Too hot to handle'");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "日耀魔石");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, 
            // @"允许你使用日耀护盾进行冲刺
            // 日耀护盾在击中敌人时不会被消耗，但会降低其伤害减免效果
            // 攻击有几率造成耀斑减益
            // '烫手魔石'");
        }

        protected override Color nameColor => new Color(254, 158, 35);
        public override string wizardEffect => Language.GetTextValue("Mods.FargowiltasSouls.WizardEffect.Solar");

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Red;
            Item.value = 400000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            //solar shields and flare debuff
            //flare debuff
            modPlayer.SolarEnchantActive = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.SolarFlareHelmet)
            .AddIngredient(ItemID.SolarFlareBreastplate)
            .AddIngredient(ItemID.SolarFlareLeggings)
            //solar wings
            //.AddIngredient(ItemID.HelFire)
            //golem fist
            //xmas tree sword
            .AddIngredient(ItemID.DayBreak)
            .AddIngredient(ItemID.SolarEruption)
            .AddIngredient(ItemID.StarWrath) //terrarian

            .AddTile(TileID.LunarCraftingStation)
            .Register();

        }
    }
}
