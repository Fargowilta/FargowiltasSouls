using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class TurtleEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Turtle Enchantment");
            Tooltip.SetDefault(
@"100% of contact damage is reflected
When standing still and not attacking, you will enter your shell
While in your shell, you will gain 90% damage resistance 
Additionally you will destroy incoming projectiles and deal 10x more thorns damage
The shell lasts at least 1 second and up to 20 attacks blocked
Enemies will explode into needles on death if they are struck with your needles
'You suddenly have the urge to hide in a shell'");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "乌龟魔石");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese,
            // @"反弹100%接触伤害
            // 站定不动时且不攻击时你会缩进壳里
            // 当你缩进壳里时增加90%伤害减免
            // 当你缩进壳里时你会摧毁来犯的敌对弹幕且反弹10倍近战伤害
            // 壳可以在消失前手动取消且能抵挡25次攻击
            // 敌人死亡时有几率爆裂出针刺
            // '你突然有一种想躲进壳里的冲动'");
        }

        protected override Color nameColor => new Color(248, 156, 92);
        public override string wizardEffect => Language.GetTextValue("Mods.FargowiltasSouls.WizardEffect.Turtle");

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Yellow;
            Item.value = 250000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            CactusEnchant.CactusEffect(player);
            modPlayer.TurtleEffect(hideVisual);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.TurtleHelmet)
            .AddIngredient(ItemID.TurtleScaleMail)
            .AddIngredient(ItemID.TurtleLeggings)
            .AddIngredient(null, "CactusEnchant")
            .AddIngredient(ItemID.ChlorophytePartisan)
            .AddIngredient(ItemID.Yelets)

            //chloro saber
            //
            //jungle turtle
            //.AddIngredient(ItemID.Seaweed);
            //.AddIngredient(ItemID.LizardEgg);

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
}
