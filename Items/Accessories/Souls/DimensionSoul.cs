using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Souls
{
    [AutoloadEquip(EquipType.Wings)]
    public class DimensionSoul : FlightMasteryWings
    {
        protected override bool HasSupersonicSpeed => true;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Soul of Dimensions");

            string tooltip =
@"Increases HP by 300
20% damage reduction
Increases life regeneration by 8
Grants immunity to knockback and several debuffs
Enemies are more likely to target you
Allows Supersonic running and infinite flight
Increases fishing skill substantially, All fishing rods will have 10 extra lures
Increased block and wall placement speed by 50%
Near infinite block placement and mining reach, Mining speed tripled
Shine, Spelunker, Hunter, and Dangersense effects
Auto paint and actuator effect
Grants the ability to enable Builder Mode
Effects of the Brain of Confusion, Star Veil, Sweetheart Necklace, Bee Cloak, Spore Sac, and Shiny Stone
Effects of Paladin's Shield, Frozen Turtle Shell, Arctic Diving Gear, Frog Legs, and Flying Carpet
Effects of Lava Waders, Angler Tackle Bag, Paint Sprayer, Presserator, Cell Phone, and Gravity Globe
Effects of Shield of Cthulhu and Master Ninja Gear
'The dimensions of Terraria at your fingertips'";
            Tooltip.SetDefault(tooltip);


            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 18));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
        }
        public override int NumFrames => 18;

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.accessory = true;
            Item.defense = 15;
            Item.value = 5000000;
            Item.rare = -12;
            Item.expert = true;

            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item6;
            Item.useTime = Item.useAnimation = 90;
        }

        public override bool? UseItem(Player player) => true;

        public override void UseItemFrame(Player player)
        {
            if (player.itemTime == player.itemTimeMax / 2)
            {
                player.Spawn(PlayerSpawnContext.RecallFromItem);

                for (int d = 0; d < 70; d++)
                    Dust.NewDust(player.position, player.width, player.height, 15, 0f, 0f, 150, default(Color), 1.5f);
            }
        }

        public override void UpdateInventory(Player player)
        {
            //cell phone
            player.accWatch = 3;
            player.accDepthMeter = 1;
            player.accCompass = 1;
            player.accFishFinder = true;
            player.accDreamCatcher = true;
            player.accOreFinder = true;
            player.accStopwatch = true;
            player.accCritterGuide = true;
            player.accJarOfSouls = true;
            player.accThirdEye = true;
            player.accCalendar = true;
            player.accWeatherRadio = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            modPlayer.ColossusSoul(Item, 300, 0.2f, 8, hideVisual);
            modPlayer.SupersonicSoul(Item, hideVisual);
            modPlayer.FlightMasterySoul();
            modPlayer.TrawlerSoul(Item, hideVisual);
            modPlayer.WorldShaperSoul(hideVisual);
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(null, "ColossusSoul")
            .AddIngredient(null, "SupersonicSoul")
            .AddIngredient(null, "FlightMasterySoul")
            .AddIngredient(null, "TrawlerSoul")
            .AddIngredient(null, "WorldShaperSoul")
            .AddIngredient(null, "AbomEnergy", 10)

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))

            .Register();
        }
    }
}
