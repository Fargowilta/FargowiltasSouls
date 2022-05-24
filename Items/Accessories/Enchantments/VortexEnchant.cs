using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class VortexEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Vortex Enchantment");
            Tooltip.SetDefault(
@"Double tap down to toggle stealth, reducing chance for enemies to target you but slowing movement
When entering stealth, spawn a vortex that draws in enemies and projectiles
While in stealth, your own projectiles will not be sucked in
'Tear into reality'");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "星旋魔石");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, 
            // @"双击'下'键切换至隐形模式，减少敌人以你为目标的几率，但大幅降低移动速度
            // 进入隐形状态时生成一个会吸引敌人和弹幕的旋涡
            // 处于隐形状态时你的弹幕不会被旋涡吸引
            // '撕裂现实'");
        }

        protected override Color nameColor => new Color(0, 242, 170);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Red;
            Item.value = 400000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoSoulsPlayer>().VortexEffect(hideVisual);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.VortexHelmet)
            .AddIngredient(ItemID.VortexBreastplate)
            .AddIngredient(ItemID.VortexLeggings)
            //vortex wings
            .AddIngredient(ItemID.VortexBeater)
            .AddIngredient(ItemID.Phantasm)
            //chain gun
            //electrosphere launcher
            .AddIngredient(ItemID.SDMG)

            .AddTile(TileID.LunarCraftingStation)
            .Register();
        }
    }
}
