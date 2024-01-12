using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    [AutoloadEquip(EquipType.Wings)]
    public class BeeEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = ArmorIDs.Wing.Sets.Stats[ArmorIDs.Wing.CreativeWings];
        }

        protected override Color nameColor => new(254, 246, 37);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Orange;
            Item.value = 50000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = modPlayer.ForceEffect(Item.type) ? ArmorIDs.Wing.Sets.Stats[ArmorIDs.Wing.BeeWings] : ArmorIDs.Wing.Sets.Stats[ArmorIDs.Wing.CreativeWings];
            modPlayer.BeeEffect(hideVisual, Item); //add effect
        }
        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ItemID.BeeHeadgear)
            .AddIngredient(ItemID.BeeBreastplate)
            .AddIngredient(ItemID.BeeGreaves)
            .AddIngredient(ItemID.HiveBackpack)
            //stinger necklace
            .AddIngredient(ItemID.BeeGun)
            //.AddIngredient(ItemID.WaspGun);
            //.AddIngredient(ItemID.Beenade, 50);
            //honey bomb
            .AddIngredient(ItemID.Honeyfin)
            //.AddIngredient(ItemID.Nectar);

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}
