using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class MinerEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Miner Enchantment");
            Tooltip.SetDefault(
@"50% increased mining speed
Shows the location of enemies, traps, and treasures
Effects of Night Owl, Spelunker, Hunter, Shine, and Dangersense Potions
'The planet trembles with each swing of your pick'");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "矿工魔石");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, 
            // @"增加50%挖掘速度
            // 高亮标记敌人、陷阱和宝藏
            // 你会散发光芒
            // '大地随着你的每一次挥镐而颤动'");
        }

        protected override Color nameColor => new Color(95, 117, 151);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Green;
            Item.value = 20000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MinerEffect(player, .5f);
        }

        public static void MinerEffect(Player player, float pickSpeed)
        {
            player.pickSpeed -= pickSpeed;
            player.nightVision = true;

            if (player.GetToggleValue("MiningSpelunk"))
            {
                player.findTreasure = true;
            }

            if (player.GetToggleValue("MiningHunt"))
            {
                player.detectCreature = true;
            }

            if (player.GetToggleValue("MiningDanger"))
            {
                player.dangerSense = true;
            }

            if (player.GetToggleValue("MiningShine"))
            {
                Lighting.AddLight(player.Center, 0.8f, 0.8f, 0);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.UltrabrightHelmet)
                .AddIngredient(ItemID.MiningShirt)
                .AddIngredient(ItemID.MiningPants)
                .AddIngredient(ItemID.AncientChisel)
                .AddIngredient(ItemID.CopperPickaxe)
                .AddIngredient(ItemID.GoldPickaxe)
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}
