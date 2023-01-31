using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode;
using FargowiltasSouls.NPCs;
using FargowiltasSouls.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls
{
    public class EModePlayer : ModPlayer
    {
        public int MasomodeCrystalTimer;
        public int MasomodeFreezeTimer;
        public int MasomodeSpaceBreathTimer;
        public int MasomodeMinionNerfTimer;
        public const int MaxMasomodeMinionNerfTimer = 300;

        public bool ReduceMasomodeMinionNerf;
        public bool HasWhipBuff;
        public int HallowFlipCheckTimer;
        public int TorchGodTimer;

        public int ShorterDebuffsTimer;
        public const int MaxShorterDebuffsTimer = 60;

        public List<int> ReworkedSpears = new List<int>
            {
                ItemID.Spear,
                ItemID.AdamantiteGlaive,
                ItemID.CobaltNaginata,
                ItemID.MythrilHalberd,
                ItemID.OrichalcumHalberd,
                ItemID.PalladiumPike,
                ItemID.TitaniumTrident,
                ItemID.Trident,
                ItemID.ObsidianSwordfish,
                ItemID.Swordfish,
                ItemID.ChlorophytePartisan
            };


        private int WeaponUseTimer => Player.GetModPlayer<FargoSoulsPlayer>().WeaponUseTimer;

        public override void ResetEffects()
        {
            ReduceMasomodeMinionNerf = false;
            HasWhipBuff = false;
        }

        public override void UpdateDead()
        {
            ResetEffects();

            MasomodeMinionNerfTimer = 0;
            ShorterDebuffsTimer = 0;
        }

        public override void OnEnterWorld(Player player)
        {
            foreach (NPC npc in Main.npc.Where(npc => npc.active))
            {
                if (npc.TryGetGlobalNPC(out EModeNPCBehaviour eModeNPC, false))
                    eModeNPC.TryLoadSprites(npc);
            }
        }

        public override void PreUpdate()
        {
            if (!FargoSoulsWorld.EternityMode)
                return;

            FargoSoulsPlayer fargoSoulsPlayer = Player.GetModPlayer<FargoSoulsPlayer>();

            if (Player.active && !Player.dead && !Player.ghost)
            {
                //falling gives you dazed. wings save you
                /*if (Player.velocity.Y == 0f && Player.wingsLogic == 0 && !Player.noFallDmg && !Player.ghost && !Player.dead)
                {
                    int num21 = 25;
                    num21 += Player.extraFall;
                    int num22 = (int)(Player.position.Y / 16f) - Player.fallStart;
                    if (Player.mount.CanFly)
                    {
                        num22 = 0;
                    }
                    if (Player.mount.Cart && Minecart.OnTrack(Player.position, Player.width, Player.height))
                    {
                        num22 = 0;
                    }
                    if (Player.mount.Type == 1)
                    {
                        num22 = 0;
                    }
                    Player.mount.FatigueRecovery();

                    if (((Player.gravDir == 1f && num22 > num21) || (Player.gravDir == -1f && num22 < -num21)))
                    {
                        Player.immune = false;
                        int dmg = (int)(num22 * Player.gravDir - num21) * 10;
                        if (Player.mount.Active)
                            dmg = (int)(dmg * Player.mount.FallDamage);

                        Player.Hurt(PlayerDeathReason.ByOther(0), dmg, 0);
                        Player.AddBuff(BuffID.Dazed, 120);
                    }
                    Player.fallStart = (int)(Player.position.Y / 16f);
                }*/

                if (!NPC.downedBoss3 && Player.ZoneDungeon && !NPC.AnyNPCs(NPCID.DungeonGuardian))
                {
                    NPC.SpawnOnPlayer(Player.whoAmI, NPCID.DungeonGuardian);
                }


                if (Player.ZoneUnderworldHeight)
                {
                    if (!(Player.fireWalk || fargoSoulsPlayer.PureHeart || Player.lavaMax > 0))
                        FargoSoulsUtil.AddDebuffFixedDuration(Player, BuffID.OnFire, 2);
                }

                if (Player.ZoneJungle)
                {
                    if (Player.wet && !Player.lavaWet && !Player.honeyWet && !fargoSoulsPlayer.MutantAntibodies)
                        FargoSoulsUtil.AddDebuffFixedDuration(Player, BuffID.Poisoned, 2);
                }

                if (Player.ZoneSnow)
                {
                    //if (!fargoSoulsPlayer.PureHeart && !Main.dayTime && Framing.GetTileSafely(Player.Center).WallType == WallID.None)
                    //    Player.AddBuff(BuffID.Chilled, Main.expertMode && Main.expertDebuffTime > 1 ? 1 : 2);

                    if (Player.wet && !Player.lavaWet && !Player.honeyWet && !fargoSoulsPlayer.MutantAntibodies
                        && Player.chilled)
                    {
                        //Player.AddBuff(BuffID.Frostburn, Main.expertMode && Main.expertDebuffTime > 1 ? 1 : 2);
                        MasomodeFreezeTimer++;
                        if (MasomodeFreezeTimer >= 600)
                        {
                            FargoSoulsUtil.AddDebuffFixedDuration(Player, BuffID.Frozen, 120);
                            MasomodeFreezeTimer = -300;
                        }
                    }
                    else
                    {
                        MasomodeFreezeTimer = 0;
                    }
                }
                else
                {
                    MasomodeFreezeTimer = 0;
                }

                /*if (Player.wet && !fargoSoulsPlayer.MutantAntibodies)
                {
                    if (Player.ZoneDesert)
                        FargoSoulsUtil.AddDebuffFixedDuration(Player, BuffID.Slow, 2);
                    if (Player.ZoneDungeon)
                        FargoSoulsUtil.AddDebuffFixedDuration(Player, BuffID.Cursed, 2);
                    Tile currentTile = Framing.GetTileSafely(Player.Center);
                    if (currentTile.WallType == WallID.GraniteUnsafe)
                        FargoSoulsUtil.AddDebuffFixedDuration(Player, BuffID.Weak, 2);
                    if (currentTile.WallType == WallID.MarbleUnsafe)
                        FargoSoulsUtil.AddDebuffFixedDuration(Player, BuffID.BrokenArmor, 2);
                }*/

                if (Player.ZoneCorrupt)
                {
                    if (!fargoSoulsPlayer.PureHeart)
                        FargoSoulsUtil.AddDebuffFixedDuration(Player, BuffID.Darkness, 2);
                    if (Player.wet && !Player.lavaWet && !Player.honeyWet && !fargoSoulsPlayer.MutantAntibodies)
                        FargoSoulsUtil.AddDebuffFixedDuration(Player, BuffID.CursedInferno, 2);
                }

                if (Player.ZoneCrimson)
                {
                    if (!fargoSoulsPlayer.PureHeart)
                        FargoSoulsUtil.AddDebuffFixedDuration(Player, BuffID.Bleeding, 2);
                    if (Player.wet && !Player.lavaWet && !Player.honeyWet && !fargoSoulsPlayer.MutantAntibodies)
                        FargoSoulsUtil.AddDebuffFixedDuration(Player, BuffID.Ichor, 2);
                }

                if (Player.ZoneHallow)
                {
                    if (Player.ZoneRockLayerHeight && !fargoSoulsPlayer.PureHeart)
                    {
                        if (++HallowFlipCheckTimer > 6) //reduce computation
                        {
                            HallowFlipCheckTimer = 0;

                            float playerAbove = Player.Center.Y - 16 * 50;
                            float playerBelow = Player.Center.Y + 16 * 50;
                            if (playerAbove / 16 < Main.maxTilesY && playerBelow / 16 < Main.maxTilesY
                                && !Collision.CanHitLine(new Vector2(Player.Left.X, playerAbove), 0, 0, new Vector2(Player.Left.X, playerBelow), 0, 0)
                                && !Collision.CanHitLine(new Vector2(Player.Right.X, playerAbove), 0, 0, new Vector2(Player.Right.X, playerBelow), 0, 0))
                            {
                                if (!Main.wallHouse[Framing.GetTileSafely(Player.Center).WallType]
                                    && !Main.wallHouse[Framing.GetTileSafely(Player.TopLeft).WallType]
                                    && !Main.wallHouse[Framing.GetTileSafely(Player.TopRight).WallType]
                                    && !Main.wallHouse[Framing.GetTileSafely(Player.BottomLeft).WallType]
                                    && !Main.wallHouse[Framing.GetTileSafely(Player.BottomRight).WallType])
                                {
                                    Player.AddBuff(ModContent.BuffType<FlippedHallow>(), 90);
                                }
                            }
                        }
                    }

                    if (Player.wet && !Player.lavaWet && !Player.honeyWet && !fargoSoulsPlayer.MutantAntibodies)
                        Player.AddBuff(ModContent.BuffType<Smite>(), 2);
                }

                if (!fargoSoulsPlayer.PureHeart && Main.raining && (Player.ZoneOverworldHeight || Player.ZoneSkyHeight) 
                    && Player.HeldItem.type != ItemID.Umbrella && Player.HeldItem.type != ItemID.TragicUmbrella
                    && Player.armor[0].type != ItemID.UmbrellaHat && Player.armor[0].type != ItemID.Eyebrella)
                {
                    Tile currentTile = Framing.GetTileSafely(Player.Center);
                    if (currentTile.WallType == WallID.None)
                    {
                        if (Player.ZoneSnow)
                            Player.AddBuff(ModContent.BuffType<Hypothermia>(), 2);
                        else
                            Player.AddBuff(BuffID.Wet, 2);
                        /*if (Main.hardMode)
                        {
                            lightningCounter++;

                            if (lightningCounter >= 600)
                            {
                                //tends to spawn in ceilings if the Player goes indoors/underground
                                Point tileCoordinates = Player.Top.ToTileCoordinates();

                                tileCoordinates.X += Main.rand.Next(-25, 25);
                                tileCoordinates.Y -= 15 + Main.rand.Next(-5, 5);

                                for (int index = 0; index < 10 && !WorldGen.SolidTile(tileCoordinates.X, tileCoordinates.Y) && tileCoordinates.Y > 10; ++index) tileCoordinates.Y -= 1;

                                Projectile.NewProjectile(tileCoordinates.X * 16 + 8, tileCoordinates.Y * 16 + 17, 0f, 0f, ProjectileID.VortexVortexLightning, 0, 2f, Main.myPlayer,
                                    0f, 0);

                                lightningCounter = 0;
                            }
                        }*/
                    }
                }

                if (Player.wet && !Player.lavaWet && !Player.honeyWet && !(Player.accFlipper || Player.gills || fargoSoulsPlayer.MutantAntibodies))
                    Player.AddBuff(ModContent.BuffType<Lethargic>(), 2);

                if (!fargoSoulsPlayer.PureHeart && !Player.buffImmune[BuffID.Suffocation] && Player.ZoneSkyHeight && Player.whoAmI == Main.myPlayer)
                {
                    bool inLiquid = Collision.DrownCollision(Player.position, Player.width, Player.height, Player.gravDir) || (!Player.armor[0].IsAir && (Player.armor[0].type == ItemID.FishBowl || Player.armor[0].type == ItemID.GoldGoldfishBowl));
                    if (!inLiquid)
                    {
                        Player.breath -= 3;
                        if (++MasomodeSpaceBreathTimer > 10)
                        {
                            MasomodeSpaceBreathTimer = 0;
                            Player.breath--;
                        }
                        if (Player.breath == 0)
                            SoundEngine.PlaySound(SoundID.Drown);
                        if (Player.breath <= 0)
                            Player.AddBuff(BuffID.Suffocation, 2);
                    }
                }

                if (!fargoSoulsPlayer.PureHeart && !Player.buffImmune[BuffID.Webbed] && Player.stickyBreak > 0)
                {
                    Vector2 tileCenter = Player.Center;
                    tileCenter.X /= 16;
                    tileCenter.Y /= 16;
                    Tile currentTile = Framing.GetTileSafely((int)tileCenter.X, (int)tileCenter.Y);
                    if (currentTile != null && currentTile.WallType == WallID.SpiderUnsafe)
                    {
                        Player.AddBuff(BuffID.Webbed, 30);
                        Player.AddBuff(BuffID.Slow, 90);
                        Player.stickyBreak = 0;

                        Vector2 vector = Collision.StickyTiles(Player.position, Player.velocity, Player.width, Player.height);
                        if (vector.X != -1 && vector.Y != -1)
                        {
                            int num3 = (int)vector.X;
                            int num4 = (int)vector.Y;
                            WorldGen.KillTile(num3, num4, false, false, false);
                            if (Main.netMode == NetmodeID.MultiplayerClient && !Main.tile[num3, num4].HasTile)
                                NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, num3, num4, 0f, 0, 0, 0);
                        }
                    }
                }

                if (!fargoSoulsPlayer.PureHeart && Main.bloodMoon)
                    Player.AddBuff(BuffID.WaterCandle, 2);

                if (FargoSoulsWorld.MasochistModeReal)
                {
                    Vector2 tileCenter = Player.Center;
                    tileCenter.X /= 16;
                    tileCenter.Y /= 16;
                    Tile currentTile = Framing.GetTileSafely((int)tileCenter.X, (int)tileCenter.Y);
                    if (currentTile != null && currentTile.TileType == TileID.Cactus && currentTile.HasUnactuatedTile)
                    {
                        int damage = 10;
                        if (Player.ZoneCorrupt)
                        {
                            damage *= 2;
                            Player.AddBuff(BuffID.CursedInferno, 150);
                        }
                        if (Player.ZoneCrimson)
                        {
                            damage *= 2;
                            Player.AddBuff(BuffID.Ichor, 150);
                        }
                        if (Player.ZoneHallow)
                        {
                            damage *= 2;
                            Player.AddBuff(BuffID.Confused, 150);
                        }

                        if (Main.hardMode)
                            damage *= 2;

                        if (Player.hurtCooldowns[0] <= 0) //same i-frames as spike tiles
                            Player.Hurt(PlayerDeathReason.ByCustomReason(Player.name + " was pricked by a Cactus."), damage, 0, false, false, false, 0);
                    }
                }
            }
        }

        public override void PostUpdateBuffs()
        {
            if (!FargoSoulsWorld.EternityMode)
                return;

            Player.pickSpeed -= 0.25f;

            Player.tileSpeed += 0.25f;
            Player.wallSpeed += 0.25f;

            Player.moveSpeed += 0.25f;

            Player.statManaMax2 += 100;
            Player.manaRegenDelay = Math.Min(Player.manaRegenDelay, 30);
            Player.manaRegenBonus += 5;

            Player.wellFed = true; //no longer expert half regen unless fed
        }

        public override void PostUpdateEquips()
        {
            if (!FargoSoulsWorld.EternityMode)
                return;

            if (Player.iceBarrier)
                Player.GetDamage(DamageClass.Generic) -= 0.10f;

            if (Player.setSquireT2 || Player.setSquireT3 || Player.setMonkT2 || Player.setMonkT3 || Player.setHuntressT2 || Player.setHuntressT3 || Player.setApprenticeT2 || Player.setApprenticeT3 || Player.setForbidden)
                ReduceMasomodeMinionNerf = true;
        }

        private void HandleTimersAlways()
        {
            if (MasomodeCrystalTimer > 0)
                MasomodeCrystalTimer--;

            //disable minion nerf during ooa
            if (DD2Event.Ongoing && !FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.betsyBoss, NPCID.DD2Betsy))
            {
                int n = NPC.FindFirstNPC(NPCID.DD2EterniaCrystal);
                if (n != -1 && n != Main.maxNPCs && Player.Distance(Main.npc[n].Center) < 3000)
                {
                    MasomodeMinionNerfTimer -= 2;
                    if (MasomodeMinionNerfTimer < 0)
                        MasomodeMinionNerfTimer = 0;
                }
            }

            if (WeaponUseTimer > 0)
                ShorterDebuffsTimer += 1;
            else if (ShorterDebuffsTimer > 0)
                ShorterDebuffsTimer -= 1;

            if (WeaponUseTimer > 0 && Player.HeldItem.DamageType != DamageClass.Summon && Player.HeldItem.DamageType != DamageClass.SummonMeleeSpeed && Player.HeldItem.DamageType != DamageClass.Default)
                MasomodeMinionNerfTimer += 1;
            else if (MasomodeMinionNerfTimer > 0)
                MasomodeMinionNerfTimer -= 1;

            if (MasomodeMinionNerfTimer > MaxMasomodeMinionNerfTimer)
                MasomodeMinionNerfTimer = MaxMasomodeMinionNerfTimer;

            if (ShorterDebuffsTimer > 60)
                ShorterDebuffsTimer = 60;

            //Main.NewText($"{MasomodeWeaponUseTimer} {MasomodeMinionNerfTimer} {ReduceMasomodeMinionNerf}");
        }

        public override void PostUpdateMiscEffects()
        {
            HandleTimersAlways();

            if (!FargoSoulsWorld.EternityMode)
                return;

            //whips no longer benefit from melee speed bonus
            Player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) /= Player.GetAttackSpeed(DamageClass.Melee);

            if (Player.happyFunTorchTime && ++TorchGodTimer > 60)
            {
                TorchGodTimer = 0;

                float ai0 = Main.rand.NextFloat(-2f, 2f);
                float ai1 = Main.rand.NextFloat(-2f, 2f);
                Projectile.NewProjectile(Player.GetSource_Misc("TorchGod"), Main.rand.NextVector2FromRectangle(Player.Hitbox), Vector2.Zero, ModContent.ProjectileType<TorchGodFlame>(), 20, 0f, Main.myPlayer, ai0, ai1);
            }
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (!FargoSoulsWorld.EternityMode)
                return;

            //reduce minion damage in emode if using a weapon, scales as you use weapons
            //if (FargoSoulsUtil.IsSummonDamage(proj, true, false) && MasomodeMinionNerfTimer > 0)
            //{
            //    double modifier = ReduceMasomodeMinionNerf ? 0.5 : 0.75;
            //    modifier *= Math.Min((double)MasomodeMinionNerfTimer / MaxMasomodeMinionNerfTimer, 1.0);

            //    damage = (int)(damage * (1.0 - modifier));
            //}
        }

        private void ShadowDodgeNerf()
        {
            if (Player.shadowDodge) //prehurt hook not called on titanium dodge
                Player.AddBuff(ModContent.BuffType<HolyPrice>(), 600);
        }

        public override void OnHitByNPC(NPC npc, int damage, bool crit)
        {
            if (!FargoSoulsWorld.EternityMode)
                return;

            ShadowDodgeNerf();
        }

        public override void OnHitByProjectile(Projectile proj, int damage, bool crit)
        {
            if (!FargoSoulsWorld.EternityMode)
                return;

            ShadowDodgeNerf();
        }

        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource, ref int cooldownCounter)
        {
            ShorterDebuffsTimer = MaxShorterDebuffsTimer;

            if (!FargoSoulsWorld.EternityMode)
                return base.PreHurt(pvp, quiet, ref damage, ref hitDirection, ref crit, ref customDamage, ref playSound, ref genGore, ref damageSource, ref cooldownCounter);

            //because NO MODIFY/ONHITPLAYER HOOK WORKS
            if (damageSource.SourceProjectileType is int && damageSource.SourceProjectileType == ProjectileID.Explosives)
                Player.GetModPlayer<FargoSoulsPlayer>().AddBuffNoStack(ModContent.BuffType<Stunned>(), 120);

            return base.PreHurt(pvp, quiet, ref damage, ref hitDirection, ref crit, ref customDamage, ref playSound, ref genGore, ref damageSource, ref cooldownCounter);
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if ((Main.snowMoon && NPC.waveNumber < 20) || (Main.pumpkinMoon && NPC.waveNumber < 15))
            {
                if (NPC.waveNumber > 1)
                    NPC.waveNumber--;
                NPC.waveKills /= 4;

                FargoSoulsUtil.PrintLocalization($"Mods.FargowiltasSouls.Message.MoonsDeathPenalty", new Color(175, 75, 255));
            }
        }

        public override void ModifyWeaponDamage(Item item, ref StatModifier damage)
        {
            if (!FargoSoulsWorld.EternityMode)
                return;

            damage *= MasoItemNerfs(item);

            //if (item.DamageType == DamageClass.Ranged) //changes all of these to additive
            //{
            //    //shroomite headpieces
            //    if (item.useAmmo == AmmoID.Arrow || item.useAmmo == AmmoID.Stake)
            //    {
            //        damage /= Player.arrowDamage.Multiplicative;
            //        damage += Player.arrowDamage.Multiplicative - 1f;
            //    }
            //    else if (item.useAmmo == AmmoID.Bullet || item.useAmmo == AmmoID.CandyCorn)
            //    {
            //        damage /= Player.bulletDamage.Multiplicative;
            //        damage += Player.bulletDamage.Multiplicative - 1f;
            //    }
            //    else if (item.useAmmo == AmmoID.Rocket || item.useAmmo == AmmoID.StyngerBolt || item.useAmmo == AmmoID.JackOLantern || item.useAmmo == AmmoID.NailFriendly)
            //    {
            //        damage /= Player.bulletDamage.Multiplicative;
            //        damage += Player.bulletDamage.Multiplicative - 1f;
            //    }
            //}
        }

        float AttackSpeed
        {
            get { return Player.GetModPlayer<FargoSoulsPlayer>().AttackSpeed; }
            set { Player.GetModPlayer<FargoSoulsPlayer>().AttackSpeed = value; }
        }

        private float MasoItemNerfs(Item item)
        {
            switch (item.type)
            {
                //case ItemID.DemonScythe:
                //    if (!NPC.downedBoss2)
                //    {
                //        AttackSpeed *= 0.75f;
                //        return 0.5f;
                //    }
                //    return 2f / 3f;

                //case ItemID.StarCannon:
                //case ItemID.ElectrosphereLauncher:
                //case ItemID.DaedalusStormbow:
                //case ItemID.BeesKnees:
                //case ItemID.LaserMachinegun:
                //    return 2f / 3f;

                //case ItemID.Beenade:
                //case ItemID.BlizzardStaff:
                //    AttackSpeed *= 2f / 3f;
                //    return 2f / 3f;

                //case ItemID.DD2BetsyBow:
                //case ItemID.Uzi:
                //case ItemID.PhoenixBlaster:
                //case ItemID.Handgun:
                //case ItemID.SpikyBall:
                //case ItemID.Xenopopper:
                //case ItemID.PainterPaintballGun:
                //case ItemID.MoltenFury:
                //    return 0.75f;

                //case ItemID.SnowmanCannon:
                //case ItemID.SkyFracture:
                //    return 0.8f;

                //case ItemID.SpaceGun:
                //    return 0.85f;

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
                //case ItemID.NebulaArcanum:
                //case ItemID.Phantasm:
                //case ItemID.Razorpine:
                //case ItemID.StardustDragonStaff:
                //case ItemID.SDMG:
                //case ItemID.LastPrism:
                //    return 0.85f;

                //case ItemID.BeeGun:
                //case ItemID.Grenade:
                //case ItemID.StickyGrenade:
                //case ItemID.BouncyGrenade:
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
                    AttackSpeed *= 0.75f;
                    return 1f;

                case var _ when ReworkedSpears.Contains(item.type):
                    return 2f;
                default:
                    break;
            }

            return 1f;
        }

        public override bool ModifyNurseHeal(NPC nurse, ref int health, ref bool removeDebuffs, ref string chatText)
        {
            if (!FargoSoulsWorld.EternityMode)
                return base.ModifyNurseHeal(nurse, ref health, ref removeDebuffs, ref chatText);

            if (Main.LocalPlayer.HasBuff(ModContent.BuffType<RushJob>()))
            {
                chatText = "I've done all I can in the time I have!";
                return false;
            }

            return base.ModifyNurseHeal(nurse, ref health, ref removeDebuffs, ref chatText);
        }

        public override void PostNurseHeal(NPC nurse, int health, bool removeDebuffs, int price)
        {
            if (!FargoSoulsWorld.EternityMode)
                return;

            if (FargoSoulsUtil.AnyBossAlive())
                Main.LocalPlayer.AddBuff(ModContent.BuffType<RushJob>(), 10);
        }
    }
}
