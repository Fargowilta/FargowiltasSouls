using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class SnowEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Snow Enchantment");
            Tooltip.SetDefault(
@"Press the Freeze Key to chill everything for 10 seconds
There is a 60 second cooldown for this effect
'It's Burning Cold Outside'");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "冰雪魔石");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, 
            // @"在光标位置生成暴雪
            // 暴雪中的所有弹幕和NPC的速度减缓50%（NPC包括敌人）
            // '冷的要死'");
        }

        protected override Color nameColor => new Color(37, 195, 242);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Blue;
            Item.value = 50000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoSoulsPlayer>().SnowEffect(hideVisual);
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ItemID.EskimoHood)
            .AddIngredient(ItemID.EskimoCoat)
            .AddIngredient(ItemID.EskimoPants)
            //hand warmer
            //fruitcake chakram
            .AddIngredient(ItemID.IceBoomerang)
            //frost daggerfish
            .AddIngredient(ItemID.FrostMinnow)
            .AddIngredient(ItemID.AtlanticCod)
            //.AddIngredient(ItemID.Fish); 

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}
