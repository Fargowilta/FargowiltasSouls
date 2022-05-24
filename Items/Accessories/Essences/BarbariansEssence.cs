using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Essences
{
    public class BarbariansEssence : BaseEssence
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Barbarian's Essence");
            Tooltip.SetDefault(
@"18% increased melee damage
10% increased melee speed
5% increased melee crit chance
'This is only the beginning..'");

            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "野蛮人精华");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese,
            // @"增加18%近战伤害
            // 增加10%近战攻速
            // 增加5%近战暴击率
            // '这只是个开始...'");
        }

        protected override Color nameColor => new Color(255, 111, 6);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Melee) += 0.18f;
            player.GetAttackSpeed(DamageClass.Melee) += .1f;
            player.GetCritChance(DamageClass.Melee) += 5;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ZombieArm)
                .AddIngredient(ItemID.ChainKnife)
                .AddIngredient(ItemID.IceBlade)
                .AddIngredient(ItemID.Shroomerang)
                .AddIngredient(ItemID.JungleYoyo)
                .AddIngredient(ItemID.CombatWrench)
                .AddIngredient(ItemID.Flamarang)
                .AddIngredient(ItemID.Terragrim)
                .AddIngredient(ItemID.WarriorEmblem)
                .AddIngredient(ItemID.HallowedBar, 5)

                .AddTile(TileID.TinkerersWorkbench)
                .Register();

        }
    }
}
