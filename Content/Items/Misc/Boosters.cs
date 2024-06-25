using FargowiltasSouls.Core.ModPlayers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Misc
{
    public abstract class Booster : ModItem
    {
        public const int LunarDuration = 10 * 60;
        public override void SetStaticDefaults()
        {
            // Pickup sets that Nebula Boosters are in
            ItemID.Sets.IsAPickup[Type] = true;
            ItemID.Sets.IgnoresEncumberingStone[Type] = true;
            ItemID.Sets.ItemsThatShouldNotBeInInventory[Type] = true;
            ItemID.Sets.ItemNoGravity[Type] = true;
        }
        public abstract void PickupEffect(BoosterPlayer boosterPlayer);
        public override bool OnPickup(Player player)
        {
            //SoundEngine.PlaySound(SoundID.Grab, Item.position);
            PickupEffect(player.GetModPlayer<BoosterPlayer>());
            return false;
        }
    }
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
}
