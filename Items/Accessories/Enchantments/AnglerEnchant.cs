using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class AnglerEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Angler Enchantment");
            Tooltip.SetDefault(
@"Increases fishing power
You catch fish almost instantly
Effects of Lavaproof Tackle Bag
'As long as they aren't all shoes, you can go home happily'");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "渔夫魔石");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chine, 
            // @"增加渔力
            // 你几乎能立刻就钓到鱼
            // 拥有渔夫渔具袋效果
            // '只要不全是鞋子, 你就可以高高兴兴地回家'");
        }

        protected override Color nameColor => new Color(113, 125, 109);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.value = 100000;
            Item.rare = ItemRarityID.Pink;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoSoulsPlayer>().FishSoul1 = true;
            player.fishingSkill += 10;

            //tackle bag
            player.accFishingLine = true;
            player.accTackleBox = true;
            player.accLavaFishing = true;


        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.AnglerHat)
                .AddIngredient(ItemID.AnglerVest)
                .AddIngredient(ItemID.AnglerPants)
                .AddIngredient(ItemID.LavaproofTackleBag)
                .AddIngredient(ItemID.BloodFishingRod)
                .AddIngredient(ItemID.FiberglassFishingPole)
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}
