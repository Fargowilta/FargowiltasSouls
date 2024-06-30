using FargowiltasSouls.Content.Items.Accessories.Masomode;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Souls
{
    //[AutoloadEquip(EquipType.Shoes)]
    public class SupersonicSoul : BaseSoul
    {

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.value = 750000;
        }
        public static readonly Color ItemColor = new(238, 0, 69);
        protected override Color? nameColor => ItemColor;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            AddEffects(player, Item, hideVisual);
        }
        public static void AddEffects(Player player, Item item, bool hideVisual)
        {
            Player Player = player;
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            player.AddEffect<MasoAeolusFrog>(item);
            player.AddEffect<MasoAeolusFlower>(item);
            player.AddEffect<ZephyrJump>(item);

            modPlayer.SupersonicSoul = true;

            if (Player.AddEffect<SupersonicSpeedEffect>(item) && !modPlayer.noSupersonic && !LumUtils.AnyBosses())
            {
                // 5 is the default value, I removed the config for it because the new toggler doesn't have sliders
                Player.runAcceleration += 5f * .1f;
                Player.maxRunSpeed += 5f * 2;
                //frog legs
                if (player.HasEffect<MasoAeolusFrog>())
                {
                    Player.autoJump = true;
                }
                Player.jumpSpeedBoost += 2.4f;
                Player.maxFallSpeed += 5f;
                Player.jumpBoost = true;
            }
            else
            {
                //calculated to match flight mastery soul, 6.75 same as frostspark
                Player.accRunSpeed = player.AddEffect<RunSpeed>(item) ? 15.6f : 6.75f;
            }

            if (player.AddEffect<NoMomentum>(item))
                modPlayer.NoMomentum = true;

            Player.moveSpeed += 0.5f;

            if (player.AddEffect<SupersonicRocketBoots>(item))
            {
                Player.rocketBoots = Player.vanityRocketBoots = ArmorIDs.RocketBoots.TerrasparkBoots;
                Player.rocketTimeMax = 10;
            }

            Player.iceSkate = true;

            //lava waders
            Player.waterWalk = true;
            Player.fireWalk = true;
            Player.lavaImmune = true;
            Player.noFallDmg = true;

            //bundle
            if (Player.AddEffect<SupersonicJumps>(item) && Player.wingTime == 0)
            {
                Player.GetJumpState(ExtraJump.CloudInABottle).Enable();
                Player.GetJumpState(ExtraJump.SandstormInABottle).Enable();
                Player.GetJumpState(ExtraJump.BlizzardInABottle).Enable();
                Player.GetJumpState(ExtraJump.FartInAJar).Enable();
            }

            //magic carpet
            if (Player.whoAmI == Main.myPlayer && Player.AddEffect<SupersonicCarpet>(item))
            {
                Player.carpet = true;
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    NetMessage.SendData(MessageID.SyncPlayer, number: Player.whoAmI);

                if (Player.canCarpet)
                {
                    modPlayer.extraCarpetDuration = true;
                }
                else if (modPlayer.extraCarpetDuration)
                {
                    modPlayer.extraCarpetDuration = false;
                    Player.carpetTime = 1000;
                }
            }

            //EoC Shield
            if (Player.AddEffect<CthulhuShield>(item))
                Player.dashType = 2;

            //ninja gear
            if (Player.AddEffect<SupersonicTabi>(item))
                Player.dashType = 1;
            if (Player.AddEffect<BlackBelt>(item))
                Player.blackBelt = true;
            if (Player.AddEffect<BlackBelt>(item))
                Player.spikedBoots = 2;

            //sweetheart necklace
            if (Player.HasEffect<DefenseBeeEffect>() || Player.AddEffect<DefenseBeeEffect>(item))
            {
                Player.honeyCombItem = item;
            }
            if (Player.AddEffect<SupersonicPanic>(item))
            {
                Player.panic = true;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ModContent.ItemType<AeolusBoots>()) //add terraspark boots
            .AddIngredient(ItemID.FlyingCarpet)
            .AddIngredient(ItemID.SweetheartNecklace)
            .AddIngredient(ItemID.BalloonHorseshoeHoney)
            .AddIngredient(ItemID.HorseshoeBundle)
            .AddIngredient(ItemID.EoCShield)
            .AddIngredient(ItemID.MasterNinjaGear)

            .AddIngredient(ItemID.MinecartMech)
            .AddIngredient(ItemID.BlessedApple)
            .AddIngredient(ItemID.AncientHorn)
            .AddIngredient(ItemID.ReindeerBells)
            .AddIngredient(ItemID.BrainScrambler)

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))


            .Register();
        }
    }
    // AAAAAAAAAAAAAAAAAAA
    public class RunSpeed : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<SupersonicHeader>();
        
        public override int ToggleItemType => ModContent.ItemType<SupersonicSoul>();
    }
    public class NoMomentum : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<SupersonicHeader>();
        public override int ToggleItemType => ModContent.ItemType<SupersonicSoul>();
        
    }
    public class SupersonicRocketBoots : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<SupersonicHeader>();
        public override int ToggleItemType => ItemID.RocketBoots;
        
    }
    public class SupersonicJumps : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<SupersonicHeader>();
        public override int ToggleItemType => ModContent.ItemType<SupersonicSoul>();
    }
    public class SupersonicCarpet : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<SupersonicHeader>();
        public override int ToggleItemType => ItemID.FlyingCarpet;
        
    }
    public class CthulhuShield : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<SupersonicHeader>();
        public override int ToggleItemType => ItemID.EoCShield;
    }
    public class SupersonicTabi : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<SupersonicHeader>();
        public override int ToggleItemType => ItemID.Tabi;
        
    }
    public class BlackBelt : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<SupersonicHeader>();

        public override int ToggleItemType => ItemID.BlackBelt;
    }
    public class SupersonicClimbing : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<SupersonicHeader>();

        public override int ToggleItemType => ItemID.TigerClimbingGear;
    }
    public class SupersonicPanic : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<SupersonicHeader>();

        public override int ToggleItemType => ItemID.PanicNecklace;
    }
}