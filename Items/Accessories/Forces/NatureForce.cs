using FargowiltasSouls.Items.Accessories.Enchantments;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Forces
{
    public class NatureForce : BaseForce
    {
        public static int[] Enchants => new int[]
        {
            ModContent.ItemType<CrimsonEnchant>(),
            ModContent.ItemType<MoltenEnchant>(),
            ModContent.ItemType<RainEnchant>(),
            ModContent.ItemType<FrostEnchant>(),
            ModContent.ItemType<ChlorophyteEnchant>(),
            ModContent.ItemType<ShroomiteEnchant>()
        };

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Force of Nature");

            // DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "自然之力");

            string tooltip =
$"[i:{ModContent.ItemType<CrimsonEnchant>()}] After taking a hit, regen is greatly increased until the hit is healed off\n" +
$"[i:{ModContent.ItemType<MoltenEnchant>()}] Nearby enemies are ignited\n" +
$"[i:{ModContent.ItemType<MoltenEnchant>()}] When you are hurt, you violently explode to damage nearby enemies\n" +
$"[i:{ModContent.ItemType<RainEnchant>()}] Spawns a miniature storm to follow you around\n" +
$"[i:{ModContent.ItemType<FrostEnchant>()}] Icicles will start to appear around you\n" +
$"[i:{ModContent.ItemType<SnowEnchant>()}] Press the Freeze Key to chill everything for 15 seconds\n" +
$"[i:{ModContent.ItemType<ChlorophyteEnchant>()}] Summons a ring of leaf crystals to shoot at nearby enemies\n" +
$"[i:{ModContent.ItemType<JungleEnchant>()}] Grants a double spore jump\n" +
$"[i:{ModContent.ItemType<ShroomiteEnchant>()}] Not moving puts you in stealth\n" +
$"[i:{ModContent.ItemType<ShroomiteEnchant>()}] All attacks gain trails of mushrooms\n" +
"'Tapped into every secret of the wilds'";
            Tooltip.SetDefault(tooltip);

            string tooltip_ch =
@"[i:{0}] 在你受到伤害后大幅增加你的生命恢复速度，直至你恢复的生命量等同于这次受到的伤害量
[i:{1}] 引燃你附近的敌人
[i:{1}] 你受到伤害时会剧烈爆炸并伤害附近的敌人
[i:{2}] 召唤一个微型雨云跟着你
[i:{3}] 你的周围会出现冰锥
[i:{4}] 在你周围生成一个可以将弹幕速度减半的冰雪光环
[i:{5}] 召唤一圈叶状攻击附近的敌人
[i:{6}] 使你获得孢子二段跳能力
[i:{7}] 站定不动时使你进入隐身状态
[i:{7}] 所有攻击都会留下蘑菇尾迹
“走遍荒野的每一个秘密角落”";
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, string.Format(tooltip_ch, Enchants[0], Enchants[1], Enchants[2], Enchants[3], ModContent.ItemType<SnowEnchant>(), Enchants[4], ModContent.ItemType<JungleEnchant>(), Enchants[5]));

            string tooltip_pt =
@"[i:{0}] Após receber um golpe, a regeneração é aumentada consideravelmente até que o golpe seja curado
[i:{1}] Inimigos próximos são incendiados
[i:{1}] Quando você sofre danos, você explode violentamente para causar dano aos inimigos próximos
[i:{2}] Invoca uma tempestade em miniatura que segue você
[i:{3}] Gelos começarão a aparecer ao seu redor
[i:{4}] Aperte a tecla Congelar para esfriar tudo por 15 segundos
[i:{5}] Invoca um anel de cristais de folha para atirar nos inimigos próximos
[i:{6}] Fornece um pulo de esporos duplo
[i:{7}] Ficar parado deixa você em modo sorrateiro
[i:{7}] Todos os ataques ganham trilhas de cogumelos
'Explorando todos os segredos da vida selvagem'";
            Tooltip.AddTranslation((int)GameCulture.CultureName.Portuguese, string.Format(tooltip_pt, Enchants[0], Enchants[1], Enchants[2], Enchants[3], ModContent.ItemType<SnowEnchant>(), Enchants[4], ModContent.ItemType<JungleEnchant>(), Enchants[5]));
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            modPlayer.NatureForce = true;
            //regen
            CrimsonEnchant.CrimsonEffect(player);
            //inferno and explode
            MoltenEnchant.MoltenEffect(player);
            //rain
            RainEnchant.RainEffect(player, Item);
            //icicles
            modPlayer.FrostEffect(hideVisual);
            modPlayer.SnowEffect(hideVisual);
            //crystal
            modPlayer.ChloroEffect(Item, hideVisual);
            //spores
            modPlayer.JungleEnchantActive = true;
            //stealth, shrooms
            modPlayer.ShroomiteEffect(hideVisual);
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
