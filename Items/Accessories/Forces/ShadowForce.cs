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
            ModContent.ItemType<NinjaEnchant>(),
            ModContent.ItemType<AncientShadowEnchant>(),
            ModContent.ItemType<CrystalAssassinEnchant>(),
            ModContent.ItemType<SpookyEnchant>(),
            ModContent.ItemType<ShinobiEnchant>(),
            ModContent.ItemType<DarkArtistEnchant>(),
            ModContent.ItemType<NecroEnchant>()
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
$"[i:{ModContent.ItemType<NinjaEnchant>()}] Drastically increases projectile and attack speed but reduces damage\n" +
$"[i:{ModContent.ItemType<CrystalAssassinEnchant>()}] Throw a smoke bomb to teleport to it and gain the First Strike Buff\n" +
$"[i:{ModContent.ItemType<CrystalAssassinEnchant>()}] Effects of Volatile Gel\n" +
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
[i:{3}] 攻击有几率造成黑暗减益
[i:{1}] 击杀敌人时有几率爆出一个骨堆，拥有骨头手套的效果
[i:{4}] 你可以扔出烟雾弹、传送至烟雾弹的位置获得先发制人增益，获得水晶刺客冲刺
[i:{5}] 你的召唤物能进行额外的镰刀攻击
[i:{6}] 不攻击可以进行一次武僧冲刺
[i:{7}] 朝墙壁冲刺时会直接穿过去
[i:{8}] 持续攻击两秒后你将被火焰包裹，切换武器后，下次攻击的伤害增加150%
[i:{9}] 召唤一个爆炸烈焰仆从，在充能完毕后会移动至光标位置
“Dark, Darker, Yet Darker”";

            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, string.Format(tooltip_ch, ModContent.ItemType<ShadowEnchant>(), ModContent.ItemType<NecroEnchant>(), Enchants[0], Enchants[1], Enchants[2], Enchants[3], ModContent.ItemType<MonkEnchant>(), Enchants[4], ModContent.ItemType<ApprenticeEnchant>(), Enchants[5]));

            string tooltip_pt =
@"[i:{0}] Quatro Esferas das Sombras orbitarão ao seu redor
[i:{1}] Seus ataques podem infligir Escuridão nos inimigos
[i:{2}] Inimigos derrotados podem deixar cair uma pilha de ossos
[i:{3}] Lance uma bomba de fumaça para teleportar-se a ela e ganhar o efeito Primeiro Ataque
[i:{4}] Efeitos da Gelatina Volátil
[i:{5}] Todos os seus lacaios ganham um ataque de foices adicional
[i:{6}] Não ataque para ganhar uma corrida de monge de uso único
[i:{7}] Faça uma corrida em qualquer parede para teleportar através dela até a próxima abertura
[i:{8}] Depois de atacar por 2 segundos você será envolto em chamas
[i:{8}] Trocar de armas aumentará o dano do próximo ataque em 150%
[i:{9}] Invoca um lacaio de Chamas Explosivas que viajará para o seu mouse depois de carregar
'Escuro, Mais Escuro, Ainda Mais Escuro'";

            Tooltip.AddTranslation((int)GameCulture.CultureName.Portuguese, string.Format(tooltip_pt, ModContent.ItemType<ShadowEnchant>(), Enchants[1], ModContent.ItemType<NecroEnchant>(), Enchants[0], Enchants[2], Enchants[3], ModContent.ItemType<MonkEnchant>(), Enchants[4], ModContent.ItemType<ApprenticeEnchant>(), Enchants[5]));
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            modPlayer.ShadowForce = true;

            modPlayer.NinjaEnchantItem = Item;


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
