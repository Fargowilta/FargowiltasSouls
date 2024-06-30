using FargowiltasSouls.Core.ModPlayers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Misc
{
    public abstract class Booster : ModItem
    {
        public const int LunarDuration = 10 * 60;
        public const int TerrariaDuration = 20 * 60;
        public virtual int Frames => 1;
        public override void SetStaticDefaults()
        {
            // Pickup sets that Nebula Boosters are in
            ItemID.Sets.IsAPickup[Type] = true;
            ItemID.Sets.IgnoresEncumberingStone[Type] = true;
            ItemID.Sets.ItemsThatShouldNotBeInInventory[Type] = true;
            ItemID.Sets.ItemNoGravity[Type] = true;
            if (Frames > 1)
            {
                ItemID.Sets.AnimatesAsSoul[Type] = true;
                Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(10, Frames));
            }
        }
        public abstract void PickupEffect(BoosterPlayer boosterPlayer);
        public override bool OnPickup(Player player)
        {
            //SoundEngine.PlaySound(SoundID.Grab, Item.position);
            PickupEffect(player.GetModPlayer<BoosterPlayer>());
            return false;
        }
        public override void GrabRange(Player player, ref int grabRange)
        {
            grabRange += 100;
        }
        public override bool GrabStyle(Player player)
        {
            object[] args = [Item, 12f, 5];
            typeof(Player).GetMethod("PullItem_Pickup", LumUtils.UniversalBindingFlags).Invoke(player, args);
            return true;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
    }
    #region Cosmos
    public class SolarBooster : Booster
    {
        public override void PickupEffect(BoosterPlayer boosterPlayer)
        {
            if (boosterPlayer.SolarTimer <= 0)
                CombatText.NewText(boosterPlayer.Player.Hitbox, Color.Yellow, Language.GetTextValue("Mods.FargowiltasSouls.Items.SolarBooster.Activate", 25), true);
            boosterPlayer.SolarTimer = LunarDuration;
        }
    }
    public class VortexBooster : Booster
    {
        public override void PickupEffect(BoosterPlayer boosterPlayer)
        {
            if (boosterPlayer.VortexTimer <= 0)
                CombatText.NewText(boosterPlayer.Player.Hitbox, Color.LightCyan, Language.GetTextValue("Mods.FargowiltasSouls.Items.VortexBooster.Activate", 35), true);
            boosterPlayer.VortexTimer = LunarDuration;
        }
    }
    public class NebulaBooster : Booster
    {
        public override void PickupEffect(BoosterPlayer boosterPlayer)
        {
            if (boosterPlayer.NebulaTimer <= 0)
                CombatText.NewText(boosterPlayer.Player.Hitbox, Color.Magenta, Language.GetTextValue("Mods.FargowiltasSouls.Items.NebulaBooster.Activate", 5), true);
            boosterPlayer.NebulaTimer = LunarDuration;
        }
    }
    public class StardustBooster : Booster
    {
        public override void PickupEffect(BoosterPlayer boosterPlayer)
        {
            if (boosterPlayer.StardustTimer <= 0)
                CombatText.NewText(boosterPlayer.Player.Hitbox, Color.Cyan, Language.GetTextValue("Mods.FargowiltasSouls.Items.StardustBooster.Activate", 35), true);
            boosterPlayer.StardustTimer = LunarDuration;
        }
    }
    #endregion

    #region Terraria Soul
    public class TimberBooster : Booster // damage
    {
        public override int Frames => 6;
        public override void PickupEffect(BoosterPlayer boosterPlayer)
        {
            if (boosterPlayer.TimberTimer <= 0)
                CombatText.NewText(boosterPlayer.Player.Hitbox, Color.SandyBrown, Language.GetTextValue("Mods.FargowiltasSouls.Items.TimberBooster.Activate", 30), true);
            boosterPlayer.TimberTimer = TerrariaDuration;
        }
    }
    public class TerraBooster : Booster // crit chance
    {
        public override int Frames => 8;
        public override void PickupEffect(BoosterPlayer boosterPlayer)
        {
            if (boosterPlayer.TerraTimer <= 0)
                CombatText.NewText(boosterPlayer.Player.Hitbox, Color.Gray, Language.GetTextValue("Mods.FargowiltasSouls.Items.TerraBooster.Activate", 30), true);
            boosterPlayer.TerraTimer = TerrariaDuration;
        }
    }
    public class EarthBooster : Booster
    {
        public override int Frames => 6;
        public override void PickupEffect(BoosterPlayer boosterPlayer)
        {
            if (boosterPlayer.EarthTimer <= 0)
                CombatText.NewText(boosterPlayer.Player.Hitbox, Color.Cyan, Language.GetTextValue("Mods.FargowiltasSouls.Items.EarthBooster.Activate", 25), true);
            boosterPlayer.EarthTimer = TerrariaDuration;
        }
    }
    public class NatureBooster : Booster
    {
        public override int Frames => 6;
        public override void PickupEffect(BoosterPlayer boosterPlayer)
        {
            if (boosterPlayer.NatureTimer <= 0)
                CombatText.NewText(boosterPlayer.Player.Hitbox, Color.LightGreen, Language.GetTextValue("Mods.FargowiltasSouls.Items.NatureBooster.Activate", 20), true);
            boosterPlayer.NatureTimer = TerrariaDuration;
        }
    }
    public class LifeBooster : Booster
    {
        public override int Frames => 7;
        public override void PickupEffect(BoosterPlayer boosterPlayer)
        {
            if (boosterPlayer.LifeTimer <= 0)
                CombatText.NewText(boosterPlayer.Player.Hitbox, Color.Pink, Language.GetTextValue("Mods.FargowiltasSouls.Items.LifeBooster.Activate", 8), true);
            boosterPlayer.LifeTimer = TerrariaDuration;
        }
    }
    public class DeathBooster : Booster
    {
        public override int Frames => 6;
        public override void PickupEffect(BoosterPlayer boosterPlayer)
        {
            if (boosterPlayer.DeathTimer <= 0)
                CombatText.NewText(boosterPlayer.Player.Hitbox, Color.Purple, Language.GetTextValue("Mods.FargowiltasSouls.Items.DeathBooster.Activate", 50), true);
            boosterPlayer.DeathTimer = TerrariaDuration;
        }
    }
    public class SpiritBooster : Booster
    {
        public override int Frames => 8;
        public override void PickupEffect(BoosterPlayer boosterPlayer)
        {
            if (boosterPlayer.SpiritTimer <= 0)
                CombatText.NewText(boosterPlayer.Player.Hitbox, Color.LightBlue, Language.GetTextValue("Mods.FargowiltasSouls.Items.SpiritBooster.Activate", 30), true);
            boosterPlayer.SpiritTimer = TerrariaDuration;
        }
    }
    public class WillBooster : Booster
    {
        public override int Frames => 7;
        public override void PickupEffect(BoosterPlayer boosterPlayer)
        {
            if (boosterPlayer.WillTimer <= 0)
                CombatText.NewText(boosterPlayer.Player.Hitbox, Color.Gold, Language.GetTextValue("Mods.FargowiltasSouls.Items.WillBooster.Activate", 25), true);
            boosterPlayer.WillTimer = TerrariaDuration;
        }
    }
    public class CosmosBooster : Booster
    {
        public override int Frames => 8;
        public override void PickupEffect(BoosterPlayer boosterPlayer)
        {
            if (boosterPlayer.CosmosTimer <= 0)
                CombatText.NewText(boosterPlayer.Player.Hitbox, Color.Black, Language.GetTextValue("Mods.FargowiltasSouls.Items.CosmosBooster.Activate", 10), true);
            boosterPlayer.CosmosTimer = TerrariaDuration;
        }
    }
    #endregion
}
