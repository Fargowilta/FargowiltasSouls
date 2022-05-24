using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class OrichalcumEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Orichalcum Enchantment");
            Tooltip.SetDefault(
@"Flower petals will cause extra damage to your target and inflict Orichalcum Poison
Damaging debuffs deal 3x damage
'Nature blesses you'");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "山铜魔石");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese,
            // @"花瓣将落到被你攻击的敌人的身上以造成额外伤害和山铜中毒减益
            // 伤害性减益造成的伤害x3
            // '自然祝福着你'");
        }

        protected override Color nameColor => new Color(235, 50, 145);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Pink;
            Item.value = 100000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoSoulsPlayer>().OrichalcumEffect();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddRecipeGroup("FargowiltasSouls:AnyOriHead")
            .AddIngredient(ItemID.OrichalcumBreastplate)
            .AddIngredient(ItemID.OrichalcumLeggings)
            .AddIngredient(ItemID.FlowerofFire)
            .AddIngredient(ItemID.FlowerofFrost)
            .AddIngredient(ItemID.CursedFlames)

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
}
