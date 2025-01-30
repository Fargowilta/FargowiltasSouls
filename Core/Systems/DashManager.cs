﻿using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Content.Items.Consumables;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Core.Systems
{
    public class DashManager : ModSystem
    {
        public enum DashType
        {
            None,
            Shadow,
            Monk,
            Jungle,
            Crystal,
            DeerSinew
        }
        public static void AddDashes(Player player)
        {
            if (player.whoAmI != Main.myPlayer)
                return;

            // Furthest down = highest priority
            // Vanilla dashes are processed before this method
            // Other mods depends on their ordering

            
            if (player.HasEffect<JungleDashEffect>())
            {
                JungleDashEffect.AddDash(player);
            }
            if (player.HasEffect<CrystalAssassinDash>())
            {
                CrystalAssassinDash.AddDash(player);
            }
            if (player.HasEffect<MonkDashEffect>())
            {
                MonkDashEffect.AddDash(player);
            }
            /*
            if (player.HasEffect<SolarEffect>())
            {
                SolarEffect.AddDash(player);
            }
            */
            if (player.HasEffect<ShadowForceDashEffect>())
            {
                ShadowForceDashEffect.AddDash(player);
            }
            if (player.HasEffect<DeerSinewEffect>()) // Takes effect last, but doesn't do anything if you have another dash
            {
                DeerSinewEffect.AddDash(player);
            }
        }
        public static void ManageDashes(Player Player)
        {
            if (Player.whoAmI != Main.myPlayer)
                return;
            FargoSoulsPlayer modPlayer = Player.FargoSouls();

            if (modPlayer.FargoDash == DashType.None)
                return;

            Player.dashType = 22;

            if (Player.dashDelay == 0 && !Player.mount.Active)
            {
                HandleDash(out bool dashing, out int dir);
                if (dashing && dir != 0)
                {
                    switch (modPlayer.FargoDash)
                    {
                        case DashType.Shadow:
                            {
                                ShadowForceDashEffect.ShadowDash(Player, dir);
                            }
                            break;
                        case DashType.Monk:
                            {
                                MonkDashEffect.MonkDash(Player, dir);
                            }
                            break;
                        case DashType.Crystal:
                            {
                                CrystalAssassinDash.CrystalDash(Player, dir);
                            }
                            break;
                        case DashType.Jungle:
                            {
                                JungleDashEffect.JungleDash(Player, dir);
                            }
                            break;
                        case DashType.DeerSinew:
                            {
                                modPlayer.DeerSinewDash(dir);
                            }
                            break;
                        default:
                            {
                                Main.NewText("Fargo dash manager: dash not registered");
                            }
                            break;
                    }
                }
            }
            if (Player.dashDelay == -1 && !Player.mount.Active)
            {
                switch (modPlayer.FargoDash)
                {
                    case DashType.Crystal:
                        {
                            CrystalAssassinDash.WhileDashing(Player);
                        }
                        break;
                }
            }
        }

        public static MethodInfo DashHandleMethod { get; set; }
        public override void Load()
        {
            DashHandleMethod = typeof(Player).GetMethod("DoCommonDashHandle", LumUtils.UniversalBindingFlags);
        }
        public static void HandleDash(out bool dashing, out int dir)
        {

            dir = 1;
            dashing = true; //these two are overriden by the actual method anyway

            Player player = Main.LocalPlayer;
            Player.DashStartAction action = null;
            object[] args = [dir, dashing, action];
            DashHandleMethod.Invoke(player, args);
            dir = (int)args[0];
            dashing = (bool)args[1];
        }

    }
}