using FargowiltasSouls.Items.Accessories.Enchantments;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Forces
{
    public class LifeForce : BaseForce
    {
        public static int[] Enchants => new int[]
        {
            ModContent.ItemType<PumpkinEnchant>(),
            ModContent.ItemType<BeeEnchant>(),
            ModContent.ItemType<SpiderEnchant>(),
            ModContent.ItemType<TurtleEnchant>(),
            ModContent.ItemType<BeetleEnchant>()
        };

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Force of Life");

            string tooltip =
$"[i:{ModContent.ItemType<PumpkinEnchant>()}] You will grow pumpkins while walking on the ground\n" +
$"[i:{ModContent.ItemType<CactusEnchant>()}] Enemies may explode into needles on death\n" +
$"[i:{ModContent.ItemType<BeeEnchant>()}] Melee hits and most piercing attacks spawn bees\n" +
$"[i:{ModContent.ItemType<SpiderEnchant>()}] 30% chance for minions and sentries to crit for x1.5 damage instead of x2\n" +
$"[i:{ModContent.ItemType<TurtleEnchant>()}] When standing still and not attacking, you will enter your shell\n" +
$"[i:{ModContent.ItemType<BeetleEnchant>()}] Beetles aid both offense and defense\n" +
"'Rare is a living thing that dare disobey your will'";

            Tooltip.SetDefault(tooltip);

            string tooltip_ch =
@"[i:{0}] 你在地上行走时会种下南瓜
[i:{1}] 敌人死亡时有几率爆裂出针刺
[i:{2}] 近战攻击和大多数穿透类弹幕击中敌人时会生成蜜蜂
[i:{3}] 仆从和哨兵可以造成暴击，且有30%基础暴击率，但暴击伤害只有1.5倍而不是2倍
[i:{4}] 站定不动时且不攻击时你会缩进壳里
[i:{5}] 甲虫会提高你的攻击能力和防御能力
“罕有生灵敢违背你的意愿”";
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, string.Format(tooltip_ch, Enchants[0], ModContent.ItemType<CactusEnchant>(), Enchants[1], Enchants[2], Enchants[3], Enchants[4]));
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            modPlayer.LifeForce = true;
            modPlayer.BeeEffect(hideVisual);
            modPlayer.SpiderEffect(hideVisual);
            modPlayer.BeetleEffect();
            PumpkinEnchant.PumpkinEffect(player, Item);
            modPlayer.TurtleEffect(hideVisual);
            CactusEnchant.CactusEffect(player);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            foreach (int ench in Enchants)
                recipe.AddIngredient(ench);
            recipe.AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"));
            recipe.Register();
        }
    }
}
