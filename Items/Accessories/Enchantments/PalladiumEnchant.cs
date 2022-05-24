using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class PalladiumEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Palladium Enchantment");
            Tooltip.SetDefault(
@"Briefly increases life regeneration after striking an enemy
You spawn an orb of damaging life energy every 80 life regenerated
'You feel your wounds slowly healing' ");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "钯金魔石");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese,
            // @"攻击敌人后暂时增加你的生命恢复速度
            // 你每恢复80点生命值便会生成一个伤害性的生命能量球
            // '你感到你的伤口在慢慢愈合'");
        }

        protected override Color nameColor => new Color(245, 172, 40);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Pink;
            Item.value = 100000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoSoulsPlayer>().PalladiumEffect();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddRecipeGroup("FargowiltasSouls:AnyPallaHead")
            .AddIngredient(ItemID.PalladiumBreastplate)
            .AddIngredient(ItemID.PalladiumLeggings)
            .AddIngredient(ItemID.BatBat)
            .AddIngredient(ItemID.SoulDrain)
            .AddIngredient(ItemID.UndergroundReward)

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
}
