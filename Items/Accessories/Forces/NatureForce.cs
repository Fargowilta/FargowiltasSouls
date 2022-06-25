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
[i:{1}] 所有敌怪在狱火圈内时多受到50%伤害
[i:{2}] 召唤一个微型雨云跟着你的光标
[i:{2}] 拥有浮游圈的效果
[i:{3}] 你的周围会出现冰锥
[i:{4}] 按下“冻结”键后将一切冻结15秒
[i:{5}] 召唤一圈叶状水晶攻击附近的敌人
[i:{6}] 使你获得孢子二段跳能力
[i:{6}] 你能进行短程冲刺
[i:{7}] 站定不动时使你进入隐身状态
[i:{7}] 所有攻击都会留下蘑菇尾迹
“走遍荒野的每一个秘密角落”";
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, string.Format(tooltip_ch, Enchants[0], Enchants[1], Enchants[2], Enchants[3], ModContent.ItemType<SnowEnchant>(), Enchants[4], ModContent.ItemType<JungleEnchant>(), Enchants[5]));
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
