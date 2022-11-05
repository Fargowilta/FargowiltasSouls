using FargowiltasSouls.Items.Accessories.Enchantments;
using FargowiltasSouls.Toggler;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Forces
{
    public class EarthForce : BaseForce
    {
        public static int[] Enchants => new int[]
        {
            ModContent.ItemType<CobaltEnchant>(),
            ModContent.ItemType<PalladiumEnchant>(),
            ModContent.ItemType<MythrilEnchant>(),
            ModContent.ItemType<OrichalcumEnchant>(),
            ModContent.ItemType<AdamantiteEnchant>(),
            ModContent.ItemType<TitaniumEnchant>()
        };

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Force of Earth");

            Tooltip.SetDefault(
$"[i:{ModContent.ItemType<CobaltEnchant>()}] Grants an explosion jump and you explode on hit\n" +
$"[i:{ModContent.ItemType<MythrilEnchant>()}] Stop attacking to gradually increase attack speed for up to 5 seconds\n" +
$"[i:{ModContent.ItemType<PalladiumEnchant>()}] Grants Rapid Healing after striking an enemy and spawn life orbs based on healing\n" +
$"[i:{ModContent.ItemType<OrichalcumEnchant>()}] Attacks spawn flower petals and damaging debuffs deal 4x damage\n" +
$"[i:{ModContent.ItemType<AdamantiteEnchant>()}] Every weapon shot will split into 3, deal 50% damage and have 50% less iframes\n" +
$"[i:{ModContent.ItemType<TitaniumEnchant>()}] Attacks generate titanium shards, reaching max grants Titanium Shield\n" +
"'Gaia's blessing shines upon you'");

            string tooltip_ch =
@"[i:{0}] 额外获得一次爆炸二段跳，受击时你会剧烈爆炸（译者注：大地之力只有受击爆炸效果，没有爆炸跳跃效果）
[i:{2}] 停止攻击以逐渐增加攻击速度，最多持续5秒
[i:{1}] 暴击敌人会快速治疗，基于回复的血量放出生命能量球
[i:{3}] 攻击生成花瓣，伤害性减益造成4倍伤害
[i:{4}] 你发射的所有弹幕都会分裂成三个，造成50%伤害且伤害频率翻倍，弹幕增加与其一半伤害相等的护甲穿透
[i:{5}] 攻击生成钛金碎片防御屏障，达到最大个数获得钛金护盾
“盖亚的祝福照耀着你”";
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, string.Format(tooltip_ch, Enchants[0], Enchants[1], Enchants[2], Enchants[3], Enchants[4], Enchants[5]));

            string tooltip_pt =
@"[i:{0}] Oferece um pulo explosivo e você explode ao ser atingido
[i:{2}] Temporariamente aumenta a velocidade de uso depois de não atacar por um tempo
[i:{2}] O bônus termina após atacar por 5 segundos e é reconstruído em 5 segundos
[i:{1}] Aumenta bastante a regeneração de vida depois de atacar um inimigo
[i:{1}] Você invoca uma esfera de energia vital que causa danos a cada 80 de vida regenerada
[i:{3}] Pétalas de flores causarão danos adicionais em seu alvo
[i:{3}] Efeitos negativos causarão 5x o dano
[i:{4}] Toda arma disparada se dividirá em 2, causará 50% de dano e terá 50% menos quadros de imunidade
[i:{5}] Atacar gera uma barreira defensiva de fragmentos de titânio
'A bênção de Gaia brilha sobre você'";
            Tooltip.AddTranslation((int)GameCulture.CultureName.Portuguese, string.Format(tooltip_pt, Enchants[0], Enchants[1], Enchants[2], Enchants[3], Enchants[4], Enchants[5]));
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            modPlayer.EarthForce = true;
            //mythril
            MythrilEnchant.MythrilEffect(player, Item);
            //shards
            modPlayer.CobaltEnchantItem = Item;
            //regen on hit, heals
            PalladiumEnchant.PalladiumEffect(player, Item);
            //fireballs and petals
            OrichalcumEnchant.OrichalcumEffect(player, Item);
            AdamantiteEnchant.AdamantiteEffect(player, Item);
            TitaniumEnchant.TitaniumEffect(player, Item);
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
