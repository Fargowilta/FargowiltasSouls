using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Essences
{
    public class ApprenticesEssence : BaseEssence
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Apprentice's Essence");
            Tooltip.SetDefault(
@"18% increased magic damage
5% increased magic crit
Increases your maximum mana by 50
'This is only the beginning..'");

            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "学徒精华");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese,
            // @"增加18%魔法伤害
            // 增加5%魔法暴击率
            // 增加50点最大法力值
            // '这是个开始...'");
        }

        protected override Color nameColor => new Color(255, 83, 255);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Magic) += 0.18f;
            player.GetCritChance(DamageClass.Magic) += 5;
            player.statManaMax2 += 50;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.WandofSparking)
                .AddIngredient(ItemID.ZapinatorGray)
                .AddIngredient(ItemID.Vilethorn)
                .AddIngredient(ItemID.CrimsonRod)
                .AddIngredient(ItemID.WeatherPain)
                .AddIngredient(ItemID.WaterBolt)
                .AddIngredient(ItemID.Flamelash)
                .AddIngredient(ItemID.DemonScythe)
                .AddIngredient(ItemID.SorcererEmblem)
                .AddIngredient(ItemID.HallowedBar, 5)

                .AddTile(TileID.TinkerersWorkbench)
                .Register();

        }
    }
}
