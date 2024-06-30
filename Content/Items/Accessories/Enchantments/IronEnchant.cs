using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class IronEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override Color nameColor => new(152, 142, 131);


        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Blue;
            Item.value = 40000;
        }
        public override void UpdateInventory(Player player) => AddEffects(player, Item);
        public override void UpdateVanity(Player player) => AddEffects(player, Item);
        public override void UpdateAccessory(Player player, bool hideVisual) => AddEffects(player, Item);
        public static void AddEffects(Player player, Item item)
        {
            player.AddEffect<IronEffect>(item);
            player.FargoSouls().IronRecipes = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.IronHelmet)
            .AddIngredient(ItemID.IronChainmail)
            .AddIngredient(ItemID.IronGreaves)
            .AddIngredient(ItemID.IronHammer)
            .AddIngredient(ItemID.IronAnvil)
            .AddIngredient(ItemID.Apricot) //(high in iron pog)

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
    public class IronEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<TerraHeader>();
        public override int ToggleItemType => ModContent.ItemType<IronEnchant>();
        
    }
}
