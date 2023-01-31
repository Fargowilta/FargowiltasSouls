using FargowiltasSouls.Items.Accessories.Enchantments;
using FargowiltasSouls.Items.Materials;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Forces
{
    public class CosmoForce : BaseForce
    {
        public static int[] Enchants => new int[]
        {
            ModContent.ItemType<MeteorEnchant>(),
            ModContent.ItemType<WizardEnchant>(),
            ModContent.ItemType<SolarEnchant>(),
            ModContent.ItemType<VortexEnchant>(),
            ModContent.ItemType<NebulaEnchant>(),
            ModContent.ItemType<StardustEnchant>()
        };

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Force of Cosmos");

            string tooltip =
$"[i:{ModContent.ItemType<MeteorEnchant>()}] A meteor shower initiates every few seconds while attacking\n" +
$"[i:{ModContent.ItemType<SolarEnchant>()}] Solar shield allows you to dash through enemies\n" +
$"[i:{ModContent.ItemType<SolarEnchant>()}] Attacks may inflict the Solar Flare debuff\n" +
$"[i:{ModContent.ItemType<VortexEnchant>()}] Double tap down to toggle stealth and spawn a vortex\n" +
$"[i:{ModContent.ItemType<NebulaEnchant>()}] Hurting enemies has a chance to spawn buff boosters\n" +
$"[i:{ModContent.ItemType<StardustEnchant>()}] A stardust guardian will protect you from nearby enemies\n" +
$"[i:{ModContent.ItemType<StardustEnchant>()}] Press the Freeze Key to freeze time for 9 seconds, 60 second cooldown\n" +
"'Been around since the Big Bang'";
            Tooltip.SetDefault(tooltip);

            string tooltip_ch =
@"[i:{0}] 攻击时每过几秒便会召唤一次流星雨
[i:{1}] 允许你使用日耀护盾进行冲刺
[i:{1}] 攻击有几率造成太阳耀斑减益
[i:{2}] 双击“下”键切换至隐形模式，生成一个旋涡
[i:{3}] 对敌人造成伤害时有几率生成强化焰
[i:{4}] 星尘守卫将保护你不受附近敌人的伤害
[i:{4}] 按下“冻结”键后会冻结时间，持续9秒，有60秒冷却时间
“自宇宙大爆炸以来就一直存在”";
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, string.Format(tooltip_ch, Enchants[0], Enchants[1], Enchants[2], Enchants[3], Enchants[4]));

            string tooltip_pt =
@"[i:{0}] Uma chuva de meteoros é iniciada a cada poucos segundos enquanto você ataca
[i:{1}] O escudo solar permite fazer uma corrida através dos inimigos
[i:{1}] Ataques podem infligir o efeito negativo Chamas do Sol
[i:{2}] Aperte duas vezes para baixo para entrar no modo sorrateiro e invocar um vórtice
[i:{3}] Ferir inimigos tem uma chance de gerar impulsos de reforços
[i:{4}] Um guardião de pó das estrelas protegerá você dos inimigos próximos
[i:{4}] Aperte a tecla Congelar para congelar o tempo por 9 segundos, tempo de recarga de 60 segundos
'Estive por aí desde o Big Bang'";
            Tooltip.AddTranslation((int)GameCulture.CultureName.Portuguese, string.Format(tooltip_pt, Enchants[0], Enchants[1], Enchants[2], Enchants[3], Enchants[4]));
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            //meme speed, solar flare,
            modPlayer.CosmoForce = true;
            modPlayer.WizardEnchantActive = true;
            //meteor shower
            modPlayer.MeteorEffect(Item);
            //solar shields
            modPlayer.SolarEnchantActive = true;
            //stealth, voids, pet
            modPlayer.VortexEffect(hideVisual);
            //boosters and meme speed
            modPlayer.NebulaEnchantActive = true;
            //guardian and time freeze
            modPlayer.StardustEffect(Item);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            foreach (int ench in Enchants)
                recipe.AddIngredient(ench);

            recipe.AddIngredient(ModContent.ItemType<Eridanium>(), 5);

            recipe.AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"));
            recipe.Register();
        }
    }
}
