using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class ShinobiEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Shinobi Infiltrator Enchantment");

            // Tooltip.SetDefault(tooltip);
        }

        public override Color nameColor => new(147, 91, 24);


        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Yellow;
            Item.value = 250000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            AddEffects(player, Item);
        }
        public static void AddEffects(Player player, Item item)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            player.AddEffect<ShinobiDashEffect>(item);
            player.AddEffect<ShinobiThroughWalls>(item);
            modPlayer.ShinobiEnchantActive = true;
            MonkEnchant.AddEffects(player, item);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.MonkAltHead)
            .AddIngredient(ItemID.MonkAltShirt)
            .AddIngredient(ItemID.MonkAltPants)
            .AddIngredient<MonkEnchant>()
            .AddIngredient(ItemID.ChainGuillotines)
            .AddIngredient(ItemID.PsychoKnife)
            //code 2
            //flower pow
            //stynger

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
    public class ShinobiDashEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<ShadowHeader>();
        public override int ToggleItemType => ModContent.ItemType<ShinobiEnchant>();
    }
    public class ShinobiThroughWalls : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<ShadowHeader>();
        public override int ToggleItemType => ModContent.ItemType<ShinobiEnchant>();
    }
}
