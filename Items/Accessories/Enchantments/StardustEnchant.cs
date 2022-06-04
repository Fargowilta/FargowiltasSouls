using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class StardustEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Stardust Enchantment");
            Tooltip.SetDefault(
@"A stardust guardian will protect you from nearby enemies
Press the Freeze Key to freeze time
While time is frozen, your minions will continue to attack
There is a 60 second cooldown for this effect
'The power of the Stand is yours'");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "星尘魔石");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, 
            // @"双击'下'键将你的守卫引至光标位置
            // 按下'冻结'键后会冻结5秒时间
            // 你的召唤物不受时间冻结影响且星尘守卫在时间冻结期间获得全新的攻击
            // 此效果有60秒冷却时间
            // '替身之力归你所有'");
        }

        protected override Color nameColor => new Color(0, 174, 238);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Red;
            Item.value = 400000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoSoulsPlayer>().StardustEffect(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.StardustHelmet)
            .AddIngredient(ItemID.StardustBreastplate)
            .AddIngredient(ItemID.StardustLeggings)
            //stardust wings
            //.AddIngredient(ItemID.StardustPickaxe);
            .AddIngredient(ItemID.StardustCellStaff) //estee pet
            .AddIngredient(ItemID.StardustDragonStaff)
            .AddIngredient(ItemID.RainbowCrystalStaff)
            //MoonlordTurretStaff


            .AddTile(TileID.LunarCraftingStation)
            .Register();
        }
    }
}
