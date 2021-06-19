using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Forces
{
    public class LifeForce : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Force of Life");

            string tooltip =
@"You will grow pumpkins while walking on the ground
100% of contact damage is reflected
Enemies may explode into needles on death
Any friendly bee or wasp will become a Mega Bee
30% chance for minions and sentries to crit
When standing still and not attacking, you will enter your shell
Beetles protect you from damage
Increases flight time by 50%
'Rare is a living thing that dare disobey your will'";
            string tooltip_ch =
@"你在地上行走时会种下南瓜
反弹100%接触伤害
敌人死亡时有几率爆裂出针刺
使友方蜜蜂或黄蜂转化为超级蜜蜂
仆从和哨兵可以造成暴击，且有30%基础暴击率
站定不动时且不攻击时你会缩进壳里
甲虫会保护你，减免下次受到的伤害
延长50%飞行时间
“罕有活物敢违背你的意愿”";

            Tooltip.SetDefault(tooltip);
            DisplayName.AddTranslation(GameCulture.Chinese, "生命之力");
            Tooltip.AddTranslation(GameCulture.Chinese, tooltip_ch);
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            ItemID.Sets.ItemNoGravity[item.type] = true;
            item.rare = ItemRarityID.Purple;
            item.value = 600000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();
            //tide hunter, yew wood, iridescent effects
            modPlayer.LifeForce = true;
            //bees ignore defense, super bees, pet
            modPlayer.BeeEffect(hideVisual);
            //minion crits and pet
            modPlayer.SpiderEffect(hideVisual);
            //defense beetle bois
            modPlayer.BeetleEffect();
            if (!modPlayer.TerrariaSoul)
                modPlayer.wingTimeModifier += .5f;
            //flame trail, pie heal, pet
            modPlayer.PumpkinEffect(hideVisual);
            //shell hide, pets
            modPlayer.TurtleEffect(hideVisual);
            //needle spray
            modPlayer.CactusEffect();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(null, "PumpkinEnchant");
            recipe.AddIngredient(null, "BeeEnchant");
            recipe.AddIngredient(null, "SpiderEnchant");
            recipe.AddIngredient(null, "TurtleEnchant");
            recipe.AddIngredient(null, "BeetleEnchant");

            recipe.AddTile(ModLoader.GetMod("Fargowiltas").TileType("CrucibleCosmosSheet"));

            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
