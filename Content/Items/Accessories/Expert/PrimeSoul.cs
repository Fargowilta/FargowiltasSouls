using Fargowiltas.Items.Tiles;
using FargowiltasSouls.Content.Items.Placables;
using FargowiltasSouls.Content.Projectiles.Minions;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Expert
{
    public class PrimeSoul : SoulsItem
    {
        public override bool IsLoadingEnabled(Mod mod) => false;
        public override string Texture => "FargowiltasSouls/Content/Items/Placeholder";
        public override void SetStaticDefaults()
        {

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Expert;
            Item.value = Item.sellPrice(0, 1);

            Item.expert = true;
        }

        void PrimeSoulEffect(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (player.AddEffect<PrimeSoulEffect>(Item))
                modPlayer.PrimeSoulActive = modPlayer.PrimeSoulActiveBuffer = true;
        }

        public override void UpdateInventory(Player player) => PrimeSoulEffect(player);
        public override void UpdateVanity(Player player) => PrimeSoulEffect(player);
        public override void UpdateAccessory(Player player, bool hideVisual) => PrimeSoulEffect(player);

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ModContent.ItemType<BoxofGizmos>())
            .AddIngredient(ModContent.ItemType<RustedOxygenTank>())
            .AddIngredient(ModContent.ItemType<LifeRevitalizer>())
            .AddIngredient(ItemID.SoulofFlight, 3)
            .AddIngredient(ItemID.SoulofLight, 3)
            .AddIngredient(ItemID.SoulofNight, 3)
            .AddIngredient(ItemID.SoulofMight, 3)
            .AddIngredient(ItemID.SoulofFright, 3)
            .AddIngredient(ItemID.SoulofSight, 3)
            //add each other challenger expert drop when they're made

            .AddTile<CrucibleCosmosSheet>()

            .Register();
        }
    }
    public class PrimeSoulEffect : AccessoryEffect
    {
        public override Header ToggleHeader => null;
        public override void PostUpdateEquips(Player player)
        {
            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[ModContent.ProjectileType<MinosPrime>()] < 1)
            {
                FargoSoulsUtil.NewSummonProjectile(GetSource_EffectItem(player), player.Center, Vector2.Zero, ModContent.ProjectileType<MinosPrime>(), 30, 10f, Main.myPlayer);
            }
        }
    }
}