using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class CrystalAssassinEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Crystal Assassin Enchantment");
            Tooltip.SetDefault(@"Allows the ability to dash
Use Ninja hotkey to throw a smoke bomb, use it again to teleport to it and gain the First Strike Buff
Using the Rod of Discord will also grant this buff
When you teleport, you also spawn several homing blades
Effects of Volatile Gel''");

            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "水晶刺客魔石");
            //             string tooltip_ch =
            // @"拥有挥发明胶效果
            // ''";
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, tooltip_ch);
        }

        protected override Color nameColor => new Color(231, 178, 28); //change e

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Pink;
            Item.value = 150000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //player.GetModPlayer<FargoSoulsPlayer>().ForbiddenEffect(); //effect tele on party girl bathwater, when tele slashes through enemies
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.CrystalNinjaHelmet)
                .AddIngredient(ItemID.CrystalNinjaChestplate)
                .AddIngredient(ItemID.CrystalNinjaLeggings)
                .AddIngredient(ModContent.ItemType<NinjaEnchant>())
                .AddIngredient(ItemID.VolatileGelatin)
                .AddIngredient(ItemID.FlyingKnife)

                .AddTile(TileID.CrystalBall)
                .Register();
        }
    }
}
