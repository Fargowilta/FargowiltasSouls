using FargowiltasSouls.Content.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Systems;
using FargowiltasSouls.Core.Globals;
using Terraria.WorldBuilding;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using System.Collections.Generic;
using FargowiltasSouls.Content.Projectiles.ChallengerItems;
using FargowiltasSouls.Content.Items;
using Terraria.Localization;
using FargowiltasSouls.Content.Projectiles.Souls;
using FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.PumpkinMoon;
using FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.FrostMoon;

namespace FargowiltasSouls.Core.ModPlayers
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

        public int MythrilHalberdTimer;
        public int CobaltHitCounter;

        public int LightningCounter;

        public int CrossNecklaceTimer;
        private int WeaponUseTimer => Player.FargoSouls().WeaponUseTimer;

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

        public override void OnEnterWorld()
        {
            foreach (NPC npc in Main.npc.Where(npc => npc.active))
            {
                foreach (var entityGlobal in npc.EntityGlobals)
                {
                    if (entityGlobal is EModeNPCBehaviour eModeNPC)
                    {
                        eModeNPC.TryLoadSprites(npc);
                    }
                }
            }
        }
        public override void PreUpdateBuffs()
        {
            MurderGreaterDangersense();
        }
        public override void PostUpdate()
        {
            MurderGreaterDangersense();
        }
        private void MurderGreaterDangersense()//KILL alchnpc greater dangersense (when boss alive)
        {
            if (!WorldSavingSystem.EternityMode)
                return;

            if (ModLoader.TryGetMod("AlchemistNPC", out Mod alchNPC) && LumUtils.AnyBosses())
            {
                if (alchNPC.TryFind("GreaterDangersense", out ModBuff greaterDangersense))
                {
                    MurderBuff(greaterDangersense.Type);
                }
            }
            if (ModLoader.TryGetMod("AlchemistNPCLite", out Mod alchNPCLite) && LumUtils.AnyBosses())
            {
                if (alchNPCLite.TryFind("GreaterDangersense", out ModBuff greaterDangersense))
                {
                    MurderBuff(greaterDangersense.Type);
                }
            }
            void MurderBuff(int type)
            {
                if (Player.HasBuff(type))
                {
                    int index = Player.FindBuffIndex(type);
                    Player.DelBuff(index);
                    Player.ClearBuff(type);
                }
            }
        }

        public static List<int> IronTiles = new()
        {
            TileID.Iron,
            TileID.IronBrick,
            TileID.Lead,
            TileID.LeadBrick,
            TileID.MetalBars
        };
        public static List<int> IronWalls = new()
        {
            WallID.IronFence,
            WallID.WroughtIronFence,
            WallID.MetalFence,
        };
        public override void PreUpdate()
        {
            if (!WorldSavingSystem.EternityMode)
                return;

            FargoSoulsPlayer fargoSoulsPlayer = Player.FargoSouls();

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

                if (!NPC.downedBoss3 && Player.ZoneDungeon && !NPC.AnyNPCs(NPCID.DungeonGuardian) && !Main.drunkWorld && !Main.zenithWorld)
                {
                    NPC.SpawnOnPlayer(Player.whoAmI, NPCID.DungeonGuardian);
                }


                if (Player.ZoneUnderworldHeight)
                {
                    bool anyAshwoodEffect = Player.HasEffect<AshWoodEffect>() || Player.HasEffect<ObsidianEffect>();
                    if (anyAshwoodEffect || !(Player.fireWalk || fargoSoulsPlayer.PureHeart || Player.lavaMax > 0))
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
                        Player.AddBuff(ModContent.BuffType<HypothermiaBuff>(), 2);
                        /*
                        MasomodeFreezeTimer++;
                        if (MasomodeFreezeTimer >= 600)
                        {
                            FargoSoulsUtil.AddDebuffFixedDuration(Player, BuffID.Frozen, 120);
                            MasomodeFreezeTimer = -300;
                        }
                        */
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
                                    Player.AddBuff(ModContent.BuffType<FlippedHallowBuff>(), 90);
                                }
                            }
                        }
                    }

                    if (Player.wet && !Player.lavaWet && !Player.honeyWet && !fargoSoulsPlayer.MutantAntibodies)
                        Player.AddBuff(ModContent.BuffType<SmiteBuff>(), 2);
                }

                Vector2 tileCenter = Player.Center;
                tileCenter.X /= 16;
                tileCenter.Y /= 16;
                Tile currentTile = Framing.GetTileSafely((int)tileCenter.X, (int)tileCenter.Y);

                if (!fargoSoulsPlayer.PureHeart && Main.raining && (Player.ZoneOverworldHeight)
                    && Player.HeldItem.type != ItemID.Umbrella && Player.HeldItem.type != ItemID.TragicUmbrella
                    && Player.armor[0].type != ItemID.UmbrellaHat && Player.armor[0].type != ItemID.Eyebrella
                    && !Player.HasEffect<RainUmbrellaEffect>())
                {
                    if (currentTile.WallType == WallID.None)
                    {
                        if (Player.ZoneSnow)
                            Player.AddBuff(ModContent.BuffType<HypothermiaBuff>(), 2);
                        else
                            Player.AddBuff(BuffID.Wet, 2);

                        LightningCounter++;

                        int lighntningMinSeconds = WorldSavingSystem.MasochistModeReal ? 10 : 17;
                        if (LightningCounter >= LumUtils.SecondsToFrames(lighntningMinSeconds))
                        {
                            Point tileCoordinates = Player.Top.ToTileCoordinates();

                            tileCoordinates.X += Main.rand.Next(-25, 25);
                            tileCoordinates.Y -= Main.rand.Next(4, 8);


                            bool foundMetal = false;
                            if (WorldSavingSystem.MasochistModeReal)
                                foundMetal = true;

                            /* TODO: make this work
                            for (int x = -5; x < 5; x++)
                            {
                                for (int y = -5; y < 5; y++)
                                {
                                    int testX = tileCoordinates.X + x;
                                    int testY = tileCoordinates.Y + y;
                                    Tile tile = Main.tile[testX, testY];
                                    if (IronTiles.Contains(tile.TileType) ||IronTiles.Contains(tile.WallType))
                                    {
                                        foundMetal = true;
                                        tileCoordinates.X = testX;
                                        tileCoordinates.Y = testY;
                                        Main.NewText("found metal");
                                        break;
                                    }
                                }
                            }
                            */

                            if (LumUtils.AnyBosses() && !WorldSavingSystem.MasochistModeReal)
                            {
                                LightningCounter = 0;
                            }
                            else if (Main.rand.NextBool(300) || foundMetal)
                            {
                                //tends to spawn in ceilings if the Player goes indoors/underground


                                //for (int index = 0; index < 10 && !WorldGen.SolidTile(tileCoordinates.X, tileCoordinates.Y) && tileCoordinates.Y > 10; ++index) 
                                //    tileCoordinates.Y -= 1;


                                float ai1 = Player.Center.Y;
                                int damage = (Main.hardMode ? 120 : 60) / 4;
                                Projectile.NewProjectile(Player.GetSource_Misc(""), tileCoordinates.X * 16 + 8, (tileCoordinates.Y * 16 + 17) - 900, 0f, 0f, ModContent.ProjectileType<RainLightning>(), damage, 2f, Main.myPlayer,
                                    Vector2.UnitY.ToRotation(), ai1);

                                LightningCounter = 0;
                            }
                        }
                    }
                }

                if (Player.wet && !Player.lavaWet && !Player.honeyWet && !(Player.GetJumpState(ExtraJump.Flipper).Enabled || Player.gills || fargoSoulsPlayer.MutantAntibodies))
                    Player.AddBuff(ModContent.BuffType<LethargicBuff>(), 2);

                if (!fargoSoulsPlayer.PureHeart && !Player.buffImmune[BuffID.Suffocation] && Player.ZoneSkyHeight && Player.whoAmI == Main.myPlayer)
                {
                    bool inLiquid = Collision.DrownCollision(Player.position, Player.width, Player.height, Player.gravDir) || !Player.armor[0].IsAir && (Player.armor[0].type == ItemID.FishBowl || Player.armor[0].type == ItemID.GoldGoldfishBowl);
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

                        if (Player.breath < -10) //don't stack far into negatives
                        {

                            Player.breath = -10;
                        }

                    }
                }

                if (!fargoSoulsPlayer.PureHeart && !Player.buffImmune[BuffID.Webbed] && Player.stickyBreak > 0)
                {

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

                if (currentTile != null && currentTile.TileType == TileID.Cactus && currentTile.HasUnactuatedTile && !fargoSoulsPlayer.CactusImmune)
                {
                    int damage = 10;
                    if (WorldSavingSystem.MasochistModeReal)
                    {
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
                    }

                    if (Main.hardMode)
                        damage *= 2;

                    if (Player.hurtCooldowns[0] <= 0) //same i-frames as spike tiles
                        Player.Hurt(PlayerDeathReason.ByCustomReason(Language.GetTextValue("Mods.FargowiltasSouls.DeathMessage.Cactus", Player.name)), damage, 0, false, false, 0, false);
                }

                if (!fargoSoulsPlayer.PureHeart && Main.bloodMoon)
                    Player.AddBuff(BuffID.WaterCandle, 2);
            }
        }

        public override void PostUpdateBuffs()
        {
            if (!WorldSavingSystem.EternityMode)
                return;

            Player.pickSpeed -= 0.25f;

            Player.tileSpeed += 0.25f;
            Player.wallSpeed += 0.25f;

            Player.moveSpeed += 0.25f;

            Player.statManaMax2 += 50;
            Player.manaRegenDelay = Math.Min(Player.manaRegenDelay, 30);
            Player.manaRegenBonus += 5;

            Player.wellFed = true; //no longer expert half regen unless fed

        }

        public override void PostUpdateEquips()
        {
            if (!WorldSavingSystem.EternityMode)
                return;

            if (Player.longInvince && !Player.immune)
            {
                if (CrossNecklaceTimer < 20)
                {
                    Player.longInvince = false;
                    CrossNecklaceTimer++;
                }
            }
            else
            {
                CrossNecklaceTimer = 0;
            }

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

            if (!WorldSavingSystem.EternityMode)
                return;

            //whips no longer benefit from melee speed bonus
            if (Player.HeldItem.shoot > ProjectileID.None && ProjectileID.Sets.IsAWhip[Player.HeldItem.shoot])
            {
                Player.GetAttackSpeed(DamageClass.Melee) = 1;
            }
            //Player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) /= Player.GetAttackSpeed(DamageClass.Melee);

            if (Player.happyFunTorchTime && ++TorchGodTimer > 60)
            {
                TorchGodTimer = 0;

                float ai0 = Main.rand.NextFloat(-2f, 2f);
                float ai1 = Main.rand.NextFloat(-2f, 2f);
                Projectile.NewProjectile(Player.GetSource_Misc("TorchGod"), Main.rand.NextVector2FromRectangle(Player.Hitbox), Vector2.Zero, ModContent.ProjectileType<TorchGodFlame>(), 20, 0f, Main.myPlayer, ai0, ai1);
            }
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)/* tModPorter If you don't need the Projectile, consider using ModifyHitNPC instead */
        {
            if (!WorldSavingSystem.EternityMode)
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
                Player.AddBuff(ModContent.BuffType<HolyPriceBuff>(), 600);
        }
        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            if (!WorldSavingSystem.EternityMode)
                return;

            ShadowDodgeNerf();

            if (Player.resistCold && npc.coldDamage) //warmth potion nerf
            {
                modifiers.FinalDamage *= 1.15f;
            }
        }
        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            if (!WorldSavingSystem.EternityMode)
                return;

            ShadowDodgeNerf();

            if (Player.resistCold && proj.coldDamage) //warmth potion nerf
            {
                modifiers.SourceDamage *= 1.15f; // warmth potion modifies source damage (pre defense) for some fucking reason
            }
        }
        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            if (!WorldSavingSystem.EternityMode)
                return;

            //ShadowDodgeNerf();
        }

        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            if (!WorldSavingSystem.EternityMode)
                return;
            //ShadowDodgeNerf();
        }

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            ShorterDebuffsTimer = MaxShorterDebuffsTimer;

            if (!WorldSavingSystem.EternityMode)
                base.ModifyHurt(ref modifiers);

            //because NO MODIFY/ONHITPLAYER HOOK WORKS
            if (modifiers.DamageSource.SourceProjectileType == ProjectileID.Explosives)
                Player.FargoSouls().AddBuffNoStack(ModContent.BuffType<StunnedBuff>(), 120);



            base.ModifyHurt(ref modifiers);
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if (WorldSavingSystem.MasochistModeReal)
            {
                foreach (NPC npc in Main.npc.Where(npc => npc.active && (npc.boss || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsTail)))
                {
                    int heal = npc.lifeMax / 10;
                    npc.life += heal;
                    if (npc.life > npc.lifeMax)
                        npc.life = npc.lifeMax;
                    npc.HealEffect(heal);
                    npc.netUpdate = true;
                }
            }

            if (((Main.snowMoon && NPC.waveNumber < FrostMoonBosses.WAVELOCK) || (Main.pumpkinMoon && NPC.waveNumber < PumpkinMoonBosses.WAVELOCK)) && WorldSavingSystem.MasochistModeReal)
            {
                if (NPC.waveNumber > 1)
                    NPC.waveNumber--;
                NPC.waveKills /= 4;

                FargoSoulsUtil.PrintLocalization($"Mods.FargowiltasSouls.Message.MoonsDeathPenalty", new Color(175, 75, 255));
            }

        }

        public override void ModifyWeaponDamage(Item item, ref StatModifier damage)
        {
            if (!WorldSavingSystem.EternityMode)
                return;

            EmodeItemBalance.BalanceWeaponStats(Player, item, ref damage);

            //if (item.DamageType == DamageClass.Ranged) //changes all of these to additive
            //{
            //    //shroomite headpieces
            //    if (item.useAmmo == AmmoID.Arrow || item.useAmmo == AmmoID.Stake)
            //    {
            //        modifiers.FinalDamage Player.arrowDamage.Multiplicative;
            //        damage += Player.arrowDamage.Multiplicative - 1f;
            //    }
            //    else if (item.useAmmo == AmmoID.Bullet || item.useAmmo == AmmoID.CandyCorn)
            //    {
            //        modifiers.FinalDamage /= Player.bulletDamage.Multiplicative;
            //        damage += Player.bulletDamage.Multiplicative - 1f;
            //    }
            //    else if (item.useAmmo == AmmoID.Rocket || item.useAmmo == AmmoID.StyngerBolt || item.useAmmo == AmmoID.JackOLantern || item.useAmmo == AmmoID.NailFriendly)
            //    {
            //        modifiers.FinalDamage /= Player.bulletDamage.Multiplicative;
            //        damage += Player.bulletDamage.Multiplicative - 1f;
            //    }
            //}

        }

        public float AttackSpeed
        {
            get { return Player.FargoSouls().AttackSpeed; }
            set { Player.FargoSouls().AttackSpeed = value; }
        }

        public override bool ModifyNurseHeal(NPC nurse, ref int health, ref bool removeDebuffs, ref string chatText)
        {
            if (!WorldSavingSystem.EternityMode)
                return base.ModifyNurseHeal(nurse, ref health, ref removeDebuffs, ref chatText);

            if (Main.LocalPlayer.HasBuff(ModContent.BuffType<RushJobBuff>()))
            {
                chatText = Language.GetTextValue("Mods.FargowiltasSouls.Buffs.RushJobBuff.NurseChat");
                return false;
            }

            return base.ModifyNurseHeal(nurse, ref health, ref removeDebuffs, ref chatText);
        }

        public override void PostNurseHeal(NPC nurse, int health, bool removeDebuffs, int price)
        {
            if (!WorldSavingSystem.EternityMode)
                return;

            if (LumUtils.AnyBosses())
                Main.LocalPlayer.AddBuff(ModContent.BuffType<RushJobBuff>(), 10);
        }
    }
}
