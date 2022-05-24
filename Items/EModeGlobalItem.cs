using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
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

            if (item.type == ItemID.RodofDiscord || item.type == ItemID.WireKite || item.type == ItemID.WireCutter || item.type == ItemID.Wrench || item.type == ItemID.BlueWrench || item.type == ItemID.GreenWrench || item.type == ItemID.MulticolorWrench || item.type == ItemID.YellowWrench || item.type == ItemID.Actuator)
            {
                //either player is affected by lihzahrd curse, or cursor is targeting a place in temple (player standing outside)
                if (player.GetModPlayer<FargoSoulsPlayer>().LihzahrdCurse || (Framing.GetTileSafely(Main.MouseWorld).WallType == WallID.LihzahrdBrickUnsafe && !player.buffImmune[ModContent.BuffType<Buffs.Masomode.LihzahrdCurse>()]))
                    return false;
            }

            if (item.type == ItemID.RodofDiscord && FargoSoulsUtil.AnyBossAlive())
                player.chaosState = true;

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
                    tooltips.Add(new TooltipLine(Mod, "masoNerf", "[c/ff0000:Eternity Mode:] During boss fights, every use takes life"));
                    break;

                //case ItemID.ArcheryPotion:
                //case ItemID.MagicQuiver:
                //case ItemID.ShroomiteHelmet:
                //case ItemID.ShroomiteHeadgear:
                //case ItemID.ShroomiteMask:
                //    tooltips.Add(new TooltipLine(Mod, "masoNerf", "[c/ff0000:Eternity Mode:] Grants additive damage instead of multiplicative"));
                //    break;

                //case ItemID.CrystalBullet:
                //case ItemID.HolyArrow:
                //    tooltips.Add(new TooltipLine(Mod, "masoNerf2", "[c/ff0000:Eternity Mode:] Can only split 4 times per second"));
                //    break;

                case ItemID.ChlorophyteBullet:
                    tooltips.Add(new TooltipLine(Mod, "masoNerf2", "[c/ff0000:Eternity Mode:] Reduced speed and duration"));
                    //tooltips.Add(new TooltipLine(Mod, "masoNerf3", "[c/ff0000:Eternity Mode:] Further reduced damage"));
                    break;

                case ItemID.WaterBolt:
                    if (!NPC.downedBoss3)
                        tooltips.Add(new TooltipLine(Mod, "masoNerf", "[c/ff0000:Eternity Mode:] Cannot be used until Skeletron is defeated"));
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
                    tooltips.Add(new TooltipLine(Mod, "masoNerf", "[c/ff0000:Eternity Mode:] Holy Dodge activation will temporarily reduce your attack speed"));
                    break;

                case ItemID.SpectreHood:
                    tooltips.Add(new TooltipLine(Mod, "masoNerf", "[c/ff0000:Eternity Mode:] Healing orbs move slower and disappear quickly"));
                    break;

                case ItemID.FrozenTurtleShell:
                case ItemID.FrozenShield:
                    tooltips.Add(new TooltipLine(Mod, "masoNerf", "[c/ff0000:Eternity Mode:] Frozen barrier slightly reduces your damage dealt"));
                    break;

                case ItemID.BrainOfConfusion:
                    tooltips.Add(new TooltipLine(Mod, "masoNerf", "[c/ff0000:Eternity Mode:] Lowers your damage output on successful dodge"));
                    break;

                case ItemID.Zenith:
                    if (FargoSoulsWorld.downedMutant)
                    {
                        tooltips.Add(new TooltipLine(Mod, "masoNerf", "[c/ffff00:Eternity Mode:] No nerfs in effect"));
                    }
                    else
                    {
                        string bossesToKill = "";
                        if (!FargoSoulsWorld.downedAbom)
                        {
                            if (!FargoSoulsWorld.downedBoss[(int)FargoSoulsWorld.Downed.CosmosChampion])
                                bossesToKill += " Eridanus,";

                            bossesToKill += " Abominationn,";
                        }
                        bossesToKill += " Mutant";
                        tooltips.Add(new TooltipLine(Mod, "masoNerf", $"[c/ffaa00:Eternity Mode:] Vastly reduced hit rate, improves by defeating:{bossesToKill}"));
                    }
                    break;

                case ItemID.VampireKnives:
                    tooltips.Add(new TooltipLine(Mod, "masoNerf3", "[c/ff0000:Eternity Mode:] Reduced lifesteal rate when above 33% life"));
                    break;

                case ItemID.ZapinatorGray:
                case ItemID.ZapinatorOrange:
                    tooltips.Add(new TooltipLine(Mod, "masoNerf3", "[c/ff0000:Eternity Mode:] Cannot stack damage multipliers"));
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
                //case ItemID.Razorpine:
                //    tooltips.Add(new TooltipLine(Mod, "masoNerf", "[c/ff0000:Eternity Mode:] Reduced damage by 33%"));
                //    tooltips.Add(new TooltipLine(Mod, "masoNerf2", "[c/ff0000:Eternity Mode:] Reduced attack speed by 33%"));
                //    break;

                //case ItemID.Tsunami:
                //case ItemID.Flairon:
                //case ItemID.ChlorophyteShotbow:
                //case ItemID.HellwingBow:
                //case ItemID.DartPistol:
                //case ItemID.DartRifle:
                //case ItemID.Megashark:
                //case ItemID.BatScepter:
                //case ItemID.ChainGun:
                //case ItemID.VortexBeater:
                //case ItemID.RavenStaff:
                //case ItemID.XenoStaff:
                //case ItemID.StardustDragonStaff:
                //case ItemID.Phantasm:
                //case ItemID.NebulaArcanum:
                //case ItemID.SDMG:
                //case ItemID.LastPrism:
                //    tooltips.Add(new TooltipLine(Mod, "masoNerf", "[c/ff0000:Eternity Mode:] Reduced damage by 15%"));
                //    break;

                //case ItemID.BlizzardStaff:
                //    tooltips.Add(new TooltipLine(Mod, "masoNerf", "[c/ff0000:Eternity Mode:] Reduced damage by 33%"));
                //    tooltips.Add(new TooltipLine(Mod, "masoNerf2", "[c/ff0000:Eternity Mode:] Reduced attack speed by 50%"));
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

                case ItemID.DD2BallistraTowerT1Popper:
                case ItemID.DD2BallistraTowerT2Popper:
                case ItemID.DD2BallistraTowerT3Popper:
                case ItemID.DD2ExplosiveTrapT1Popper:
                case ItemID.DD2ExplosiveTrapT2Popper:
                case ItemID.DD2ExplosiveTrapT3Popper:
                case ItemID.DD2FlameburstTowerT1Popper:
                case ItemID.DD2FlameburstTowerT2Popper:
                case ItemID.DD2FlameburstTowerT3Popper:
                case ItemID.DD2LightningAuraT1Popper:
                case ItemID.DD2LightningAuraT2Popper:
                case ItemID.DD2LightningAuraT3Popper:
                    tooltips.Add(new TooltipLine(Mod, "masoNerf", "[c/ff0000:Eternity Mode:] Reduced attack speed by 33%"));
                    break;

                case ItemID.MonkAltHead:
                case ItemID.MonkAltPants:
                case ItemID.MonkAltShirt:
                case ItemID.MonkBrows:
                case ItemID.MonkPants:
                case ItemID.MonkShirt:
                case ItemID.SquireAltHead:
                case ItemID.SquireAltPants:
                case ItemID.SquireAltShirt:
                case ItemID.SquireGreatHelm:
                case ItemID.SquireGreaves:
                case ItemID.SquirePlating:
                case ItemID.HuntressAltHead:
                case ItemID.HuntressAltPants:
                case ItemID.HuntressAltShirt:
                case ItemID.HuntressJerkin:
                case ItemID.HuntressPants:
                case ItemID.HuntressWig:
                case ItemID.ApprenticeAltHead:
                case ItemID.ApprenticeAltPants:
                case ItemID.ApprenticeAltShirt:
                case ItemID.ApprenticeHat:
                case ItemID.ApprenticeRobe:
                case ItemID.ApprenticeTrousers:
                case ItemID.AncientBattleArmorHat:
                case ItemID.AncientBattleArmorPants:
                case ItemID.AncientBattleArmorShirt:
                    tooltips.Add(new TooltipLine(Mod, "masoBuff", "[c/00ff00:Eternity Mode:] Set bonus increases minimum summon damage when you attack using other classes"));
                    break;

                case ItemID.Meowmere:
                    tooltips.Add(new TooltipLine(Mod, "masoBuff", "[c/00ff00:Eternity Mode:] Shoots more projectiles per swing"));
                    break;

                case ItemID.MiningHelmet:
                    tooltips.Add(new TooltipLine(Mod, "masoNerf", "[c/ffff00:Eternity Mode:] Increases Undead Miner spawn rates"));
                    break;

                default:
                    break;
            }

            if (item.DamageType == DamageClass.Summon)
            {
                if (ProjectileID.Sets.IsAWhip[item.shoot])
                {
                    tooltips.Add(new TooltipLine(Mod, "masoWhipNerf", "[c/ff0000:Eternity Mode:] Does not benefit from melee speed bonuses"));
                    tooltips.Add(new TooltipLine(Mod, "masoWhipNerf2", "[c/ff0000:Eternity Mode:] Whip buffs/debuffs can't stack"));
                }
                else
                {
                    tooltips.Add(new TooltipLine(Mod, "masoMinionNerf", "[c/ff0000:Eternity Mode:] Summon damage decreases when you attack using other classes (except in OOA)"));
                }
            }
        }
    }
}