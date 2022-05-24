using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class MonkEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Monk Enchantment");
            Tooltip.SetDefault(
@"Allows the ability to dash
Double tap a direction
You are immune to damage and debuffs for half a second after dashing
Dash cooldown is twice as long as normal dashes
Lightning Aura can now crit and strikes faster
'Return to Monk'");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "武僧魔石");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, 
            // @"使你获得冲刺能力
            // 双击'左'或'右'键进行冲刺
            // 在冲刺后的0.5秒内使你免疫伤害和减益
            // 冲刺冷却是普通冲刺的二倍
            // 闪电光环现在可以暴击且攻击速度更快
            // '返本还僧'");
        }

        protected override Color nameColor => new Color(146, 5, 32);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Yellow;
            Item.value = 150000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoSoulsPlayer>().MonkEffect();
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ItemID.MonkBrows)
            .AddIngredient(ItemID.MonkShirt)
            .AddIngredient(ItemID.MonkPants)
            //.AddIngredient(ItemID.MonkBelt);
            .AddIngredient(ItemID.DD2LightningAuraT2Popper)
            //meatball
            //blue moon
            //valor
            .AddIngredient(ItemID.DaoofPow)
            .AddIngredient(ItemID.MonkStaffT2)

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
}
