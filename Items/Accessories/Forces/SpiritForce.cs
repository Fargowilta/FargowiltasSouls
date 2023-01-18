using FargowiltasSouls.Items.Accessories.Enchantments;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Forces
{
    public class SpiritForce : BaseForce
    {
        public static int[] Enchants => new int[]
        {
            ModContent.ItemType<FossilEnchant>(),
            ModContent.ItemType<ForbiddenEnchant>(),
            ModContent.ItemType<HallowEnchant>(),
            ModContent.ItemType<AncientHallowEnchant>(),
            ModContent.ItemType<TikiEnchant>(),
            ModContent.ItemType<SpectreEnchant>()
        };

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Force of Spirit");

            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "心灵之力");

            string tooltip =
$"[i:{ModContent.ItemType<FossilEnchant>()}] If you reach zero HP you will revive with 50 HP and spawn several bones\n" +
$"[i:{ModContent.ItemType<FossilEnchant>()}] Collect the bones once they stop moving to heal for 15 HP each\n" +
$"[i:{ModContent.ItemType<ForbiddenEnchant>()}] Double tap down to call an ancient storm to the cursor location\n" +
$"[i:{ModContent.ItemType<ForbiddenEnchant>()}] Any projectiles shot through your storm gain 60% damage\n" +
$"[i:{ModContent.ItemType<AncientHallowEnchant>()}] You gain a shield that can reflect projectiles\n" +
$"[i:{ModContent.ItemType<AncientHallowEnchant>()}] Summons a Terraprisma familiar\n" +
$"[i:{ModContent.ItemType<AncientHallowEnchant>()}] Drastically increases minion speed\n" +
$"[i:{ModContent.ItemType<HallowEnchant>()}] Become immune after striking an enemy\n" +
$"[i:{ModContent.ItemType<TikiEnchant>()}] Whip your summons to make them work harder\n" +
$"[i:{ModContent.ItemType<SpectreEnchant>()}] Damage has a chance to spawn damaging and healing orbs\n" +
"'Ascend from this mortal realm'";
            Tooltip.SetDefault(tooltip);

            string tooltip_ch =
@"[i:{0}] 受到致死伤害时会以200生命值重生并爆出几根骨头
[i:{0}] 接触停止移动的骨头时会回复20点生命值
[i:{1}] 双击“下”键会在光标位置召唤远古风暴
[i:{1}] 穿过远古风暴的弹幕会额外获得60%伤害
[i:{2}] 使你获得一面可以反弹弹幕的盾牌
[i:{2}] 召唤一柄泰拉棱镜，其伤害接受召唤伤害加成
[i:{2}] 大幅提升召唤物攻击速度，但攻击力会降低，且会增加20点护甲穿透
[i:{3}] 攻击敌人后会使你无敌一小段时间
[i:{4}] 召唤栏位用光后你仍可以召唤临时的哨兵和仆从
[i:{5}] 对敌人造成伤害时有几率生成幽灵珠
“飘飘乎如遗世独立，羽化而登仙”";
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, string.Format(tooltip_ch, Enchants[0], Enchants[1], Enchants[2], Enchants[3], Enchants[4], Enchants[5]));

            string tooltip_pt =
@"[i:{0}] Se você chegar a zero PV, você reviverá com 50 PV e invocará vários ossos
[i:{0}] Colete os ossos quando eles pararem de se mover para ser curado em 15 PV cada
[i:{1}] Toque duas vezes para baixo para chamar uma tempestade antiga até o local do cursor
[i:{1}] Projéteis disparados através de sua tempestade ganham 60% a mais de dano
[i:{2}] Você ganha um escudo que pode refletir projéteis
[i:{2}] Invoca uma Terraprisma familiar
[i:{2}] Aumenta consideravelmente a velocidade dos lacaios
[i:{3}] Fique imune depois de atacar um inimigo
[i:{4}] Você pode continuar invocando lacaios temporários depois de chegar no limite de seus espaços
[i:{5}] Os danos têm uma chance de invocar esferas que causam dano e de cura
'Ascenda deste reino mortal'";
            Tooltip.AddTranslation((int)GameCulture.CultureName.Portuguese, string.Format(tooltip_pt, Enchants[0], Enchants[1], Enchants[2], Enchants[3], Enchants[4], Enchants[5]));
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            //spectre works for all, spirit trapper works for all
            modPlayer.SpiritForce = true;
            FossilEnchant.FossilEffect(player, Item);
            modPlayer.ForbiddenEffect();
            HallowEnchant.HallowEffect(player);
            AncientHallowEnchant.AncientHallowEffect(player, Item);
            modPlayer.TikiEffect(hideVisual);
            modPlayer.SpectreEffect(hideVisual);
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
