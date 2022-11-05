using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class NebulaEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Nebula Enchantment");
            Tooltip.SetDefault(
@"Hurting enemies has a chance to spawn buff boosters
Buff booster stacking capped at 2
'The pillars of creation have shined upon you'");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "星云魔石");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, 
            // @"伤害敌人时有几率生成强化增益
            // 强化增益最大堆叠上限为2
            // '创生之柱照耀着你'");
        }

        protected override Color nameColor => new Color(254, 126, 229);
        public override string wizardEffect => Language.GetTextValue("Mods.FargowiltasSouls.WizardEffect.Nebula");

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Red;
            Item.value = 400000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoSoulsPlayer>().NebulaEnchantActive = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.NebulaHelmet)
            .AddIngredient(ItemID.NebulaBreastplate)
            .AddIngredient(ItemID.NebulaLeggings)
            //.AddIngredient(ItemID.WingsNebula);
            .AddIngredient(ItemID.NebulaArcanum)
            .AddIngredient(ItemID.NebulaBlaze)
            //LeafBlower
            //bubble gun
            //chaarged blaster cannon
            .AddIngredient(ItemID.LunarFlareBook)

            .AddTile(TileID.LunarCraftingStation)
            .Register();
        }
    }
}
