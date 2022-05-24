using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class FrostEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Frost Enchantment");

            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "冰霜魔石");

            string tooltip =
@"Icicles will start to appear around you
Attacking will launch them towards the cursor
When they hit an enemy they are frozen solid
Press the Freeze Key to chill everything for 10 seconds
There is a 60 second cooldown for this effect
'Let's coat the world in a deep freeze'";
            Tooltip.SetDefault(tooltip);

            //             string tooltip_ch =
            // @"你的周围会出现冰锥
            // 攻击时会将冰锥发射至光标位置
            // 冰锥击中敌人时会使其短暂冻结并受到25%额外伤害5秒
            // 敌对弹幕飞行速度减半
            // '让我们给这个世界披上一层厚厚的冰衣'";
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, tooltip_ch);
        }

        protected override Color nameColor => new Color(122, 189, 185);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Pink;
            Item.value = 150000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoSoulsPlayer>().FrostEffect(hideVisual);
            player.GetModPlayer<FargoSoulsPlayer>().SnowEffect(hideVisual);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.FrostHelmet)
            .AddIngredient(ItemID.FrostBreastplate)
            .AddIngredient(ItemID.FrostLeggings)
            .AddIngredient(ModContent.ItemType<SnowEnchant>())
            .AddIngredient(ItemID.Frostbrand)
            .AddIngredient(ItemID.IceBow)
            //frost staff
            //coolwhip
            //.AddIngredient(ItemID.BlizzardStaff);
            //.AddIngredient(ItemID.ToySled);
            //.AddIngredient(ItemID.BabyGrinchMischiefWhistle);

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
}
