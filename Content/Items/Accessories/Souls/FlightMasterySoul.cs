using FargowiltasSouls.Content.Items.Accessories.Masomode;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Souls
{
    [AutoloadEquip(EquipType.Wings)]
    public class FlightMasterySoul : FlightMasteryWings
    {
        public override bool HasSupersonicSpeed => false;

        public static readonly Color ItemColor = new(56, 134, 255);
        protected override Color? nameColor => ItemColor;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            AddEffects(player, Item);
        }
        public static void AddEffects(Player player, Item item)
        {
            Player Player = player;
            player.FargoSouls().FlightMasterySoul = true;
            Player.wingTimeMax = 999999;
            Player.wingTime = Player.wingTimeMax;
            Player.ignoreWater = true;

            player.AddEffect<FlightMasteryInsignia>(item);
            player.AddEffect<FlightMasteryGravity>(item);
            if (item.ModItem != null && item.ModItem is FlightMasteryWings fmWings && fmWings.HasSupersonicSpeed)
                player.AddEffect<SupersonicSpeedEffect>(item);

            //hover
            if (Player.controlDown && Player.controlJump && !Player.mount.Active)
            {
                Player.position.Y -= Player.velocity.Y;
                if (Player.velocity.Y > 0.1f)
                    Player.velocity.Y = 0.1f;
                else if (Player.velocity.Y < -0.1f)
                    Player.velocity.Y = -0.1f;
            }

            //grav
            player.AddEffect<MasoGravEffect>(item);
        }
        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ItemID.EmpressFlightBooster) //soaring insignia

            .AddIngredient(ItemID.BatWings) //bat wings
            .AddIngredient(ItemID.CreativeWings) //fledgling wings
            .AddIngredient(ItemID.FairyWings)
            .AddIngredient(ItemID.HarpyWings)
            .AddIngredient(ItemID.BoneWings)
            .AddIngredient(ItemID.FrozenWings)
            .AddIngredient(ItemID.FlameWings)
            .AddIngredient(ItemID.TatteredFairyWings)
            .AddIngredient(ItemID.FestiveWings)
            .AddIngredient(ItemID.BetsyWings)
            .AddIngredient(ItemID.FishronWings)
            .AddIngredient(ItemID.RainbowWings) //empress wings
            .AddIngredient(ItemID.LongRainbowTrailWings) //celestial starboard

            .AddIngredient(ItemID.GravityGlobe)

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))


            .Register();
        }
    }
    public class FlightMasteryInsignia : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<FlightMasteryHeader>();
        public override int ToggleItemType => ItemID.EmpressFlightBooster;
        public override bool IgnoresMutantPresence => true;
        public override void PostUpdateEquips(Player player)
        {
            player.empressBrooch = true;
        }
    }
    public class FlightMasteryGravity : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<FlightMasteryHeader>();
        public override int ToggleItemType => ModContent.ItemType<FlightMasterySoul>();
        public override bool IgnoresMutantPresence => true;
        public override void PostUpdateEquips(Player player)
        {
            player.gravity = Player.defaultGravity * 1.5f;
        }
    }
}
