using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Essences
{
    public class OccultistsEssence : BaseEssence
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Occultist's Essence");
            /* Tooltip.SetDefault(
@"18% increased summon damage
Increases your max number of minions by 1
Increases your max number of sentries by 1
'This is only the beginning..'"); */

            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "术士精华");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese,
            // @"增加18%召唤伤害
            //             +1最大召唤栏
            //             +1最大哨兵栏
            //             '这只是个开始...'");
        }

        public override Color nameColor => new(0, 255, 255);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Summon) += 0.18f;
            player.maxMinions += 1;
            player.maxTurrets += 1;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BabyBirdStaff)
                .AddIngredient(ItemID.SlimeStaff)
                .AddIngredient(ItemID.VampireFrogStaff)
                .AddIngredient(ItemID.HoundiusShootius)
                .AddIngredient(ItemID.HornetStaff)
                .AddIngredient(ItemID.ThornWhip) //snapthorn 
                .AddIngredient(ItemID.ImpStaff)
                .AddIngredient(ItemID.SummonerEmblem)
                .AddIngredient(ItemID.HallowedBar, 5)

                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}
