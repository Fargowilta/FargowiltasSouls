using FargowiltasSouls.Projectiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items
{
    public class EModeGlobalItem : GlobalItem
    {
        public override void PickAmmo(Item weapon, Item ammo, Player player, ref int type, ref float speed, ref StatModifier damage, ref float knockback)
        {
            if (!FargoSoulsWorld.EternityMode)
                return;

            //ammo nerf
            //if (ammo.ammo == AmmoID.Arrow || ammo.ammo == AmmoID.Bullet || ammo.ammo == AmmoID.Dart)
            //{
            //    damage -= (int)Math.Round(ammo.damage * player.GetDamage(DamageClass.Ranged).Additive * 0.5, MidpointRounding.AwayFromZero); //always round up
            //}
        }

        public override bool CanUseItem(Item item, Player player)
        {
            if (!FargoSoulsWorld.EternityMode)
                return base.CanUseItem(item, player);

            if (item.damage <= 0 && (item.type == ItemID.RodofDiscord || item.type == ItemID.ActuationRod || item.type == ItemID.WireKite || item.type == ItemID.WireCutter || item.type == ItemID.Wrench || item.type == ItemID.BlueWrench || item.type == ItemID.GreenWrench || item.type == ItemID.MulticolorWrench || item.type == ItemID.YellowWrench || item.type == ItemID.Actuator))
            {
                //either player is affected by lihzahrd curse, or cursor is targeting a place in temple (player standing outside)
                if (player.GetModPlayer<FargoSoulsPlayer>().LihzahrdCurse || (Framing.GetTileSafely(Main.MouseWorld).WallType == WallID.LihzahrdBrickUnsafe && !player.buffImmune[ModContent.BuffType<Buffs.Masomode.LihzahrdCurse>()]))
                    return false;
            }

            if (item.type == ItemID.RodofDiscord && FargoSoulsUtil.AnyBossAlive())
			{
				player.chaosState = true;
			}

            return base.CanUseItem(item, player);
        }

        public override void ModifyShootStats(Item item, Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (!FargoSoulsWorld.EternityMode)
                return;

            if (!NPC.downedBoss3 && item.type == ItemID.WaterBolt)
            {
                type = ProjectileID.WaterGun;
                damage = 0;
            }

            if (!NPC.downedBoss2 && item.type == ItemID.SpaceGun)
            {
                type = ProjectileID.ConfettiGun;
                damage = 0;
            }
        }

        enum EModeChange
        {
            Nerf,
            Buff,
            Neutral
        }
        

        void ItemBalance(List<TooltipLine> tooltips, EModeChange change, string key, int amount = 0)
        {
            string prefix = Language.GetTextValue($"Mods.FargowiltasSouls.EModeBalance.{change}");
            string nerf = Language.GetTextValue($"Mods.FargowiltasSouls.EModeBalance.{key}", amount == 0 ? null : amount);
            tooltips.Add(new TooltipLine(Mod, $"{change}{key}", $"{prefix} {nerf}"));
        }

        void ItemBalance(List<TooltipLine> tooltips, EModeChange change, string key, string extra)
        {
            string prefix = Language.GetTextValue($"Mods.FargowiltasSouls.EModeBalance.{change}");
            string nerf = Language.GetTextValue($"Mods.FargowiltasSouls.EModeBalance.{key}");
            tooltips.Add(new TooltipLine(Mod, $"{change}{key}", $"{prefix} {nerf} {extra}"));
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (!FargoSoulsWorld.EternityMode)
                return;

            //if (item.damage > 0 && (item.ammo == AmmoID.Arrow || item.ammo == AmmoID.Bullet || item.ammo == AmmoID.Dart))
            //{
            //    tooltips.Add(new TooltipLine(Mod, "masoAmmoNerf", "[c/ff0000:Eternity Mode:] Contributes 50% less damage to weapons"));
            //}

            switch (item.type)
            {
                case ItemID.RodofDiscord:
                    ItemBalance(tooltips, EModeChange.Nerf, "RodofDiscord");
                    break;

                //case ItemID.ArcheryPotion:
                //case ItemID.MagicQuiver:
                //case ItemID.ShroomiteHelmet:
                //case ItemID.ShroomiteHeadgear:
                //case ItemID.ShroomiteMask:
                //    tooltips.Add(new TooltipLine(Mod, "masoNerf", "[c/ff0000:Eternity Mode:] Grants additive damage instead of multiplicative"));
                //    break;

                case ItemID.CrystalBullet:
                case ItemID.HolyArrow:
                    //ItemBalance(tooltips, EModeChange.Nerf, "Split");
                    break;

                case ItemID.ChlorophyteBullet:
                    //ItemBalance(tooltips, EModeChange.Nerf, "ChlorophyteBullet");
                    //tooltips.Add(new TooltipLine(Mod, "masoNerf3", "[c/ff0000:Eternity Mode:] Further reduced damage"));
                    break;

                case ItemID.WaterBolt:
                    if (!NPC.downedBoss3)
                        ItemBalance(tooltips, EModeChange.Nerf, "WaterBolt");
                    break;

                case ItemID.HallowedGreaves:
                case ItemID.HallowedHeadgear:
                case ItemID.HallowedHelmet:
                case ItemID.HallowedHood:
                case ItemID.HallowedMask:
                case ItemID.HallowedPlateMail:
                case ItemID.AncientHallowedGreaves:
                case ItemID.AncientHallowedHeadgear:
                case ItemID.AncientHallowedHelmet:
                case ItemID.AncientHallowedHood:
                case ItemID.AncientHallowedMask:
                case ItemID.AncientHallowedPlateMail:
                    ItemBalance(tooltips, EModeChange.Nerf, "HolyDodge");
                    break;

                case ItemID.FrozenTurtleShell:
                case ItemID.FrozenShield:
                    ItemBalance(tooltips, EModeChange.Nerf, "FrozenTurtleShell");
                    break;

                case ItemID.BrainOfConfusion:
                    ItemBalance(tooltips, EModeChange.Nerf, "BrainOfConfusion");
                    break;

                case ItemID.Zenith:
                    if (FargoSoulsWorld.downedMutant)
                    {
                        ItemBalance(tooltips, EModeChange.Neutral, "ZenithNone");
                    }
                    else
                    {
                        string bossesToKill = "";
                        if (!FargoSoulsWorld.downedAbom)
                        {
                            bossesToKill += $" {Language.GetTextValue("Mods.FargowiltasSouls.NPCName.AbomBoss")},";
                        }
                        bossesToKill += $" {Language.GetTextValue("Mods.FargowiltasSouls.NPCName.MutantBoss")}";

                        ItemBalance(tooltips, EModeChange.Nerf, "ZenithHitRate", bossesToKill);
                    }
                    break;

                //case ItemID.VampireKnives:
                //    ItemBalance(tooltips, EModeChange.Nerf, "VampireKnives");
                //    break;

                case ItemID.ZapinatorGray:
                case ItemID.ZapinatorOrange:
                    ItemBalance(tooltips, EModeChange.Nerf, "Zapinator");
                    break;

                //case ItemID.EmpressBlade:
                //    tooltips.Add(new TooltipLine(Mod, "masoNerf", "[c/ff0000:Eternity Mode:] Reduced damage by 15%"));
                //    goto case ItemID.StardustCellStaff;
                //case ItemID.StardustCellStaff:
                //    tooltips.Add(new TooltipLine(Mod, "masoNerf", "[c/ff0000:Eternity Mode:] Damage slightly reduced as more are summoned"));
                //    break;

                //case ItemID.DD2BetsyBow:
                //case ItemID.Uzi:
                //case ItemID.PhoenixBlaster:
                //case ItemID.Handgun:
                //case ItemID.SpikyBall:
                //case ItemID.Xenopopper:
                //case ItemID.PainterPaintballGun:
                //case ItemID.MoltenFury:
                //    tooltips.Add(new TooltipLine(Mod, "masoNerf", "[c/ff0000:Eternity Mode:] Reduced damage by 25%"));
                //    break;

                //case ItemID.SkyFracture:
                //case ItemID.SnowmanCannon:
                //    tooltips.Add(new TooltipLine(Mod, "masoNerf", "[c/ff0000:Eternity Mode:] Reduced damage by 20%"));
                //    break;

                //case ItemID.StarCannon:
                //case ItemID.ElectrosphereLauncher:
                //case ItemID.DaedalusStormbow:
                //case ItemID.BeesKnees:
                //case ItemID.LaserMachinegun:
                //    tooltips.Add(new TooltipLine(Mod, "masoNerf", "[c/ff0000:Eternity Mode:] Reduced damage by 33%"));
                //    break;

                //case ItemID.Beenade:
                //case ItemID.BlizzardStaff:
                //    ItemBalance(tooltips, EModeChange.Nerf, "Damage", 33);
                //    ItemBalance(tooltips, EModeChange.Nerf, "Speed", 33);
                //    break;

                //case ItemID.Tsunami:
                //case ItemID.Flairon:
                //case ItemID.ChlorophyteShotbow:
                //case ItemID.HellwingBow:
                //case ItemID.DartPistol:
                //case ItemID.DartRifle:
                //case ItemID.Megashark:
                //case ItemID.ChainGun:
                //case ItemID.VortexBeater:
                //case ItemID.RavenStaff:
                //case ItemID.XenoStaff:
                //case ItemID.Phantasm:
                //case ItemID.NebulaArcanum:
                //case ItemID.Razorpine:
                //case ItemID.StardustDragonStaff:
                //case ItemID.SDMG:
                //case ItemID.LastPrism:
                //    ItemBalance(tooltips, EModeChange.Nerf, "Damage", 15);
                //    break;

                //case ItemID.DemonScythe:
                //    if (NPC.downedBoss2)
                //    {
                //        tooltips.Add(new TooltipLine(Mod, "masoNerf", "[c/ff0000:Eternity Mode:] Reduced damage by 33%"));
                //    }
                //    else
                //    {
                //        tooltips.Add(new TooltipLine(Mod, "masoNerf", "[c/ff0000:Eternity Mode:] Reduced damage by 50% until an evil boss is defeated"));
                //        tooltips.Add(new TooltipLine(Mod, "masoNerf2", "[c/ff0000:Eternity Mode:] Reduced attack speed by 25% until an evil boss is defeated"));
                //    }
                //    break;

                //case ItemID.SpaceGun:
                //    if (NPC.downedBoss2)
                //    {
                //        tooltips.Add(new TooltipLine(Mod, "masoNerf", "[c/ff0000:Eternity Mode:] Reduced damage by 15%"));
                //    }
                //    else
                //    {
                //        tooltips.Add(new TooltipLine(Mod, "masoNerf", "[c/ff0000:Eternity Mode:] Cannot be used until an evil boss is defeated"));
                //    }
                //    break;

                //case ItemID.BeeGun:
                //case ItemID.Grenade:
                //case ItemID.StickyGrenade:
                //case ItemID.BouncyGrenade:
                //    tooltips.Add(new TooltipLine(Mod, "masoNerf2", "[c/ff0000:Eternity Mode:] Reduced attack speed by 33%"));
                //    break;

                //case ItemID.DD2BallistraTowerT1Popper:
                //case ItemID.DD2BallistraTowerT2Popper:
                //case ItemID.DD2BallistraTowerT3Popper:
                //case ItemID.DD2ExplosiveTrapT1Popper:
                //case ItemID.DD2ExplosiveTrapT2Popper:
                //case ItemID.DD2ExplosiveTrapT3Popper:
                //case ItemID.DD2FlameburstTowerT1Popper:
                //case ItemID.DD2FlameburstTowerT2Popper:
                //case ItemID.DD2FlameburstTowerT3Popper:
                //case ItemID.DD2LightningAuraT1Popper:
                //case ItemID.DD2LightningAuraT2Popper:
                //case ItemID.DD2LightningAuraT3Popper:
                case ItemID.FetidBaghnakhs:
                    ItemBalance(tooltips, EModeChange.Nerf, "Speed", 25);
                    break;

                //case ItemID.MonkAltHead:
                //case ItemID.MonkAltPants:
                //case ItemID.MonkAltShirt:
                //case ItemID.MonkBrows:
                //case ItemID.MonkPants:
                //case ItemID.MonkShirt:
                //case ItemID.SquireAltHead:
                //case ItemID.SquireAltPants:
                //case ItemID.SquireAltShirt:
                //case ItemID.SquireGreatHelm:
                //case ItemID.SquireGreaves:
                //case ItemID.SquirePlating:
                //case ItemID.HuntressAltHead:
                //case ItemID.HuntressAltPants:
                //case ItemID.HuntressAltShirt:
                //case ItemID.HuntressJerkin:
                //case ItemID.HuntressPants:
                //case ItemID.HuntressWig:
                //case ItemID.ApprenticeAltHead:
                //case ItemID.ApprenticeAltPants:
                //case ItemID.ApprenticeAltShirt:
                //case ItemID.ApprenticeHat:
                //case ItemID.ApprenticeRobe:
                //case ItemID.ApprenticeTrousers:
                //case ItemID.AncientBattleArmorHat:
                //case ItemID.AncientBattleArmorPants:
                //case ItemID.AncientBattleArmorShirt:
                //    ItemBalance(tooltips, EModeChange.Buff, "OOASet");
                //    break;

                case ItemID.PumpkinMoonMedallion:
                case ItemID.NaughtyPresent:
                    ItemBalance(tooltips, EModeChange.Nerf, "MoonsWaves");
                    ItemBalance(tooltips, EModeChange.Nerf, "MoonsDrops", item.type == ItemID.PumpkinMoonMedallion ? 12 : 15);
                    break;

                case ItemID.Spear:
                    ItemBalance(tooltips, EModeChange.Buff, "SpearRework");
                    break;
                case ItemID.AdamantiteGlaive:
                    ItemBalance(tooltips, EModeChange.Buff, "SpearRework");
                    break;
                case ItemID.CobaltNaginata:
                    ItemBalance(tooltips, EModeChange.Buff, "SpearRework");
                    ItemBalance(tooltips, EModeChange.Buff, "CobaltNaginataRework");
                    break;
                case ItemID.MythrilHalberd:
                    ItemBalance(tooltips, EModeChange.Buff, "SpearRework");
                    break;
                case ItemID.OrichalcumHalberd:
                    ItemBalance(tooltips, EModeChange.Buff, "SpearRework");
                    break;
                case ItemID.PalladiumPike:
                    ItemBalance(tooltips, EModeChange.Buff, "SpearRework");
                    ItemBalance(tooltips, EModeChange.Buff, "PalladiumPikeRework");
                    break;
                case ItemID.TitaniumTrident:
                    ItemBalance(tooltips, EModeChange.Buff, "SpearRework");
                    break;
                case ItemID.Trident:
                    ItemBalance(tooltips, EModeChange.Buff, "SpearRework");
                    break;
                case ItemID.ObsidianSwordfish:
                    ItemBalance(tooltips, EModeChange.Buff, "SpearRework");
                    break;
                case ItemID.Swordfish:
                    ItemBalance(tooltips, EModeChange.Buff, "SpearRework");
                    break;
                case ItemID.ChlorophytePartisan:
                    ItemBalance(tooltips, EModeChange.Buff, "SpearRework");
                    break;
                default:
                    break;
            }

            if (item.shoot > ProjectileID.None && ProjectileID.Sets.IsAWhip[item.shoot])
            {
                ItemBalance(tooltips, EModeChange.Nerf, "WhipSpeed");
                ItemBalance(tooltips, EModeChange.Nerf, "WhipStack");
            }
            //else if (item.CountsAsClass(DamageClass.Summon))
            //{
            //    if (!(EModeGlobalProjectile.IgnoreMinionNerf.TryGetValue(item.shoot, out bool ignoreNerf) && ignoreNerf))
            //        ItemBalance(tooltips, EModeChange.Nerf, "MinionStack");

            //    ItemBalance(tooltips, EModeChange.Nerf, "SummonMulticlass");
            //}
        }
    }
}