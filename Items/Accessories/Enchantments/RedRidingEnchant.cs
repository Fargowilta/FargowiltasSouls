using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class RedRidingEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Red Riding Enchantment");
            Tooltip.SetDefault(
@"Arrows will periodically fall towards your cursor
Double tap down to create a rain of arrows that follows the cursor's position for a few seconds
The arrow type is based on the first arrow in your inventory
This has a cooldown of 10 seconds
Greatly enhances Explosive Traps effectiveness
'Big Bad Red Riding Hood'");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "红色游侠魔石");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese,
            // @"箭矢会定期落至你光标周围
            // 双击'下'键后令箭雨倾斜在光标位置
            // 箭矢的种类取决于你背包中第一个箭矢
            // 此效果有10秒冷却时间
            // 大幅强化爆炸陷阱的效果
            // '大坏蛋红色骑术帽！'");
        }

        protected override Color nameColor => new Color(192, 27, 60);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Yellow;
            Item.value = 250000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            modPlayer.RedRidingEffect(hideVisual);
            HuntressEnchant.HuntressEffect(player);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.HuntressAltHead)
            .AddIngredient(ItemID.HuntressAltShirt)
            .AddIngredient(ItemID.HuntressAltPants)
            .AddIngredient(null, "HuntressEnchant")
            //eventide
            .AddIngredient(ItemID.Marrow)
            .AddIngredient(ItemID.DD2BetsyBow)
            //.AddIngredient(ItemID.DogWhistle); //werewolf pet

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
}
