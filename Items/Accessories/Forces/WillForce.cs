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
            ModContent.ItemType<RedRidingEnchant>(),
            ModContent.ItemType<ValhallaKnightEnchant>()
        };

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Force of Will");

            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "意志之力");

            string tooltip =
$"[i:{ModContent.ItemType<GoldEnchant>()}] Press the Gold hotkey to be encased in a Golden Shell\n" +
$"[i:{ModContent.ItemType<GoldEnchant>()}] Automatically sends coins to your piggy bank when picked up\n" +
$"[i:{ModContent.ItemType<PlatinumEnchant>()}] 20% chance for enemies to drop 5x loot\n" +
$"[i:{ModContent.ItemType<GladiatorEnchant>()}] Spears will rain down on struck enemies\n" +
$"[i:{ModContent.ItemType<GladiatorEnchant>()}] Grants knockback immunity when you are facing the attack\n" +
$"[i:{ModContent.ItemType<RedRidingEnchant>()}] Successive attack gain bonus damage \n" +
$"[i:{ModContent.ItemType<ValhallaKnightEnchant>()}] Increases the effectiveness of healing sources by 50%\n" +
"'A mind of unbreakable determination'";
            Tooltip.SetDefault(tooltip);

            string tooltip_ch =
@"[i:{0}] 按下“金身”键会将你包裹在一个黄金壳中
[i:{0}] 捡起钱币时，自动将它们存进猪猪存钱罐中
[i:{1}] 敌人死亡时有20%的几率获得五倍的战利品
[i:{2}] 长矛将倾泄在被攻击的敌人身上
[i:{2}] 当你面向攻击时免疫击退
[i:{3}] 连续攻击命中会获得额外伤害加成
[i:{4}] 回复生命值时，治疗量增加50%
“坚不可摧的决心”";
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, string.Format(tooltip_ch, Enchants[0], Enchants[1], Enchants[2], Enchants[3], Enchants[4]));

            string tooltip_pt =
@"[i:{0}] Aperte a hotkey de Ouro para ser envolto em um Casco Dourado
[i:{0}] Envia moedas ao seu porquinho automaticamente quando coletadas
[i:{1}] 20% de chance de que os inimigos deixem cair 5x os espólios
[i:{2}] Lanças choverão em inimigos atingidos
[i:{2}] Oferece imunidade a recuos quando você está encarando o ataque
[i:{3}] Ataques sucessivos ganham dano bônus
[i:{4}] Aumenta a eficiência de fontes de cura em 50%
'Uma mente de determinação inquebrável'";
            Tooltip.AddTranslation((int)GameCulture.CultureName.Portuguese, string.Format(tooltip_pt, Enchants[0], Enchants[1], Enchants[2], Enchants[3], Enchants[4]));
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            modPlayer.WillForce = true;
            modPlayer.GoldEffect(hideVisual);

            modPlayer.PlatinumEnchantActive = true;

            GladiatorEnchant.GladiatorEffect(player);
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
