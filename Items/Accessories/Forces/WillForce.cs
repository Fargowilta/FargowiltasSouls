using FargowiltasSouls.Items.Accessories.Enchantments;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Forces
{
    public class WillForce : BaseForce
    {
        public static int[] Enchants => new int[]
        {
            ModContent.ItemType<GoldEnchant>(),
            ModContent.ItemType<PlatinumEnchant>(),
            ModContent.ItemType<GladiatorEnchant>(),
            ModContent.ItemType<WizardEnchant>(),
            ModContent.ItemType<RedRidingEnchant>(),
            ModContent.ItemType<ValhallaKnightEnchant>()
        };

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Force of Will");

            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "意志之力");

            string tooltip =
$"[i:{ModContent.ItemType<GoldEnchant>()}] Press the Gold hotkey to be encased in a Golden Shell\n" +
$"[i:{ModContent.ItemType<GoldEnchant>()}] Automatically sends coins to your piggy bank when picked up\n" +
$"[i:{ModContent.ItemType<PlatinumEnchant>()}] 20% chance for enemies to drop 5x loot\n" +
$"[i:{ModContent.ItemType<GladiatorEnchant>()}] Spears will rain down on struck enemies\n" +
$"[i:{ModContent.ItemType<RedRidingEnchant>()}] Successive attack gain bonus damage \n" +
$"[i:{ModContent.ItemType<ValhallaKnightEnchant>()}] Increases the effectiveness of healing sources by 50%\n" +
"'A mind of unbreakable determination'";
            Tooltip.SetDefault(tooltip);

            string tooltip_ch =
@"[i:{0}] 按下“金身”键会将你包裹在一个黄金壳中
[i:{0}] 捡起钱币时，自动将它们存进猪猪存钱罐中
[i:{1}] 敌人死亡时有20%的几率获得五倍的战利品
[i:{2}] 长矛将倾泄在被攻击的敌人身上
[i:{3}] 双击'下'键后令箭雨倾斜在光标位置
[i:{4}] 回复生命值时，治疗量增加50%
“坚不可摧的决心”";
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, string.Format(tooltip_ch, Enchants[0], Enchants[1], Enchants[2], Enchants[4], Enchants[5]));
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            modPlayer.WillForce = true;
            modPlayer.GoldEffect(hideVisual);

            modPlayer.PlatinumEnchantActive = true;

            GladiatorEnchant.GladiatorEffect(player);
            modPlayer.WizardEnchantActive = true;
            RedRidingEnchant.RedRidingEffect(player, Item);
            HuntressEnchant.HuntressEffect(player);
            modPlayer.ValhallaEffect(hideVisual);
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
