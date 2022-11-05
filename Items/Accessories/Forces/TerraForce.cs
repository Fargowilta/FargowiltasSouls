using FargowiltasSouls.Items.Accessories.Enchantments;
using FargowiltasSouls.Toggler;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Forces
{
    [AutoloadEquip(EquipType.Shield)]
    public class TerraForce : BaseForce
    {
        public static int[] Enchants => new int[]
        {
            ModContent.ItemType<CopperEnchant>(),
            ModContent.ItemType<TinEnchant>(),
            ModContent.ItemType<IronEnchant>(),
            ModContent.ItemType<LeadEnchant>(),
            ModContent.ItemType<SilverEnchant>(),
            ModContent.ItemType<TungstenEnchant>(),
            ModContent.ItemType<ObsidianEnchant>()
        };

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Terra Force");

            string tooltip =
$"[i:{ModContent.ItemType<CopperEnchant>()}][i:{ModContent.ItemType<ObsidianEnchant>()}][i:{ModContent.ItemType<LeadEnchant>()}] Attacks spawn lightning, explosions, and inflict Lead Poisoning\n" +
$"[i:{ModContent.ItemType<TinEnchant>()}] Sets Tin crit chance to 10%, increasing up to double your current crit chance or 50%\n" +
$"[i:{ModContent.ItemType<IronEnchant>()}] You attract items from further away, 33% chance to not consume items at an Anvil\n" +
$"[i:{ModContent.ItemType<LeadEnchant>()}] You take 60% less from damage over time\n" +
$"[i:{ModContent.ItemType<SilverEnchant>()}] Right Click to guard with your shield\n" +
$"[i:{ModContent.ItemType<TungstenEnchant>()}] 300% increased sword size and projectile size every quarter second\n" +
$"[i:{ModContent.ItemType<ObsidianEnchant>()}] Grants immunity to fire and lava\n" +
"'The land lends its strength'";
            Tooltip.SetDefault(tooltip);

            string tooltip_ch =
@"[i:{0}][i:{6}][i:{3}] 攻击释放闪电，引发爆炸，造成铅中毒
[i:{1}] 将你的基础暴击率设为10%，暴击率最多提升至你目前暴击率的两倍（至少50%）
[i:{2}] 你能在更远处拾取物品，在砧处合成物品时33%的几率不消耗制作材料
[i:{3}] 受到的持续伤害降低60%
[i:{4}] 右键进行盾牌格挡
[i:{5}] 每过0.25秒剑和弹幕的大小增加300%
[i:{6}] 使你免疫火块与熔岩
“大地赐予它力量”";
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, string.Format(tooltip_ch, Enchants[0], Enchants[1], Enchants[2], Enchants[3], Enchants[4], Enchants[5], Enchants[6]));

            string tooltip_pt =
@"[i:{0}] Ataques têm uma chance de invocar relâmpagos e explosões
[i:{1}] Define sua chance de acerto crítico para 10%
[i:{1}] Cada crítico aumentará em 5% até dobrar sua chance de acerto crítico atual
[i:{2}] Clique com o botão direito para defender-se com seu escudo
[i:{2}] Defenda pouco antes de ser atingido para negar o dano
[i:{2}] Você atrai itens de um alcance maior
[i:{3}] Ataques infligem inimigos com Envenenamento por Chumbo
[i:{4}] 300% de aumento no tamanho das espadas
[i:{4}] Um projétil terá o seu tamanho triplicado a cada quarto de segundo
[i:{5}] Oferece imunidade a fogo e lava
[i:{5}] Seus ataques invocam explosões
[i:{5}] Aumenta o alcance dos chicotes em 50%
'A terra empresta sua força'";
            Tooltip.AddTranslation((int)GameCulture.CultureName.Portuguese, string.Format(tooltip_pt, Enchants[0], Enchants[1], Enchants[2], Enchants[3], Enchants[4], Enchants[5]));
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            modPlayer.TerraForce = true;
            CopperEnchant.CopperEffect(player, Item);
            TinEnchant.TinEffect(player, Item);
            modPlayer.IronEnchantItem = Item;
            LeadEnchant.LeadEffect(player, Item);
            if (player.GetToggleValue("SilverS"))
            {
                //shield
                modPlayer.SilverEnchantItem = Item;
            }
            TungstenEnchant.TungstenEffect(player, Item);
            ObsidianEnchant.ObsidianEffect(player, Item);
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
