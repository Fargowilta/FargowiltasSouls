using FargowiltasSouls.Items.Accessories.Enchantments;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Forces
{
    public class ShadowForce : BaseForce
    {
        public static int[] Enchants => new int[]
        {
            ModContent.ItemType<AncientShadowEnchant>(),
            ModContent.ItemType<NecroEnchant>(),
            ModContent.ItemType<CrystalAssassinEnchant>(),
            ModContent.ItemType<SpookyEnchant>(),
            ModContent.ItemType<ShinobiEnchant>(),
            ModContent.ItemType<DarkArtistEnchant>()
        };

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Shadow Force");

            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "暗影之力");

            string tooltip =
$"[i:{ModContent.ItemType<ShadowEnchant>()}] Four Shadow Orbs will orbit around you\n" +
$"[i:{ModContent.ItemType<AncientShadowEnchant>()}] Your attacks may inflict Darkness on enemies\n" +
$"[i:{ModContent.ItemType<NecroEnchant>()}] Slain enemies may drop a pile of bones\n" +
$"[i:{ModContent.ItemType<NinjaEnchant>()}] Throw a smoke bomb to teleport to it and gain the First Strike Buff\n" +

$"[i:{ModContent.ItemType<SpookyEnchant>()}] All of your minions gain an extra scythe attack\n" +
$"[i:{ModContent.ItemType<MonkEnchant>()}] Don't attack to gain a single use monk dash\n" +
$"[i:{ModContent.ItemType<ShinobiEnchant>()}] Dash into any walls, to teleport through them to the next opening\n" +
$"[i:{ModContent.ItemType<ApprenticeEnchant>()}] After attacking for 2 seconds you will be enveloped in flames\n" +
$"[i:{ModContent.ItemType<ApprenticeEnchant>()}] Switching weapons will increase the next attack's damage by 150%\n" +
$"[i:{ModContent.ItemType<DarkArtistEnchant>()}] Summons a Flameburst minion that will travel to your mouse after charging\n" +
"'Dark, Darker, Yet Darker'";
            Tooltip.SetDefault(tooltip);

            string tooltip_ch =
@"[i:{0}] 四颗暗影珠绕着你旋转
[i:{1}] 攻击有几率造成黑暗减益
[i:{2}] 击杀敌人时有几率爆出一个骨堆
[i:{3}] 你的召唤物能进行额外的镰刀攻击
[i:{4}] 你可以扔出烟雾弹、传送至烟雾弹的位置获得先发制人增益
[i:{5}] 每隔几秒，你能进行一次武僧冲刺
[i:{6}] 朝墙壁冲刺时会直接穿过去
[i:{7}] 召唤一个爆炸烈焰仆从，在充能完毕后会移动至光标位置
[i:{8}] 持续攻击两秒后你将被火焰包裹
[i:{8}] 切换武器后，下次攻击的伤害增加150%
“Dark, Darker, Yet Darker”";
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, string.Format(tooltip_ch, ModContent.ItemType<ShadowEnchant>(), Enchants[0], Enchants[1], Enchants[2], ModContent.ItemType<NinjaEnchant>(), ModContent.ItemType<MonkEnchant>(), Enchants[3], Enchants[4], ModContent.ItemType<ApprenticeEnchant>()));
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            modPlayer.ShadowForce = true;
            modPlayer.DarkArtistEffect(hideVisual);
            modPlayer.ApprenticeEffect();

            NecroEnchant.NecroEffect(player, this.Item);
            //shadow orbs
            modPlayer.AncientShadowEffect();
            //darkness debuff
            modPlayer.ShadowEffect(hideVisual);
            //tele thru walls
            modPlayer.ShinobiEffect(hideVisual);
            //monk dash mayhem
            modPlayer.MonkEffect();
            //smoke bomb nonsense
            NinjaEnchant.NinjaEffect(player);
            CrystalAssassinEnchant.CrystalAssassinEffect(player, Item);
            //scythe doom
            modPlayer.SpookyEffect(hideVisual);
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
