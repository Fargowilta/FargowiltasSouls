using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Accessories.Expert;
using FargowiltasSouls.Content.Items.Armor;
using FargowiltasSouls.Content.Items.Weapons.Challengers;
using FargowiltasSouls.Content.Items.Weapons.SwarmDrops;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace FargowiltasSouls.Core.ModPlayers
{
	public partial class FargoSoulsPlayer
    {
        public override void PreUpdate()
        {
            Toggler.TryLoad();

            if (Player.CCed)
            {
                Player.doubleTapCardinalTimer[2] = 2;
                Player.doubleTapCardinalTimer[3] = 2;
            }

            if (HurtTimer > 0)
                HurtTimer--;

            IsStandingStill = Math.Abs(Player.velocity.X) < 0.05 && Math.Abs(Player.velocity.Y) < 0.05;

            if (!Infested && !FirstInfection)
                FirstInfection = true;


            if (Unstable && Player.whoAmI == Main.myPlayer)
            {
                if (unstableCD == 0)
                {
                    Vector2 pos = Player.position;

                    int x = Main.rand.Next((int)pos.X - 500, (int)pos.X + 500);
                    int y = Main.rand.Next((int)pos.Y - 500, (int)pos.Y + 500);
                    Vector2 teleportPos = new(x, y);

                    while (Collision.SolidCollision(teleportPos, Player.width, Player.height) && teleportPos.X > 50 && teleportPos.X < (double)(Main.maxTilesX * 16 - 50) && teleportPos.Y > 50 && teleportPos.Y < (double)(Main.maxTilesY * 16 - 50))
                    {
                        x = Main.rand.Next((int)pos.X - 500, (int)pos.X + 500);
                        y = Main.rand.Next((int)pos.Y - 500, (int)pos.Y + 500);
                        teleportPos = new(x, y);
                    }

                    Player.Teleport(teleportPos, 1);
                    NetMessage.SendData(MessageID.TeleportEntity, -1, -1, null, 0, Player.whoAmI, teleportPos.X, teleportPos.Y, 1);

                    unstableCD = 60;
                }
                unstableCD--;
            }

            if (OxygenTank)
            {
                RustedOxygenTank.PassiveEffect(Player);
            }


            if (GoldShell)
                GoldUpdate();

            //horizontal dash
            if (MonkDashing > 0)
            {
                MonkDashing--;

                //no loss of height
                //Player.maxFallSpeed = 0f;
                //Player.fallStart = (int)(Player.position.Y / 16f);
                //Player.gravity = 0f;
                //Player.position.Y = Player.oldPosition.Y;

                if (MonkDashing == 0 && Player.mount.Active)
                {
                    if (Player.velocity.Length() > Player.mount._data.dashSpeed)
                    {
                        float difference = Player.velocity.Length() / Player.mount._data.dashSpeed;

                        Player.velocity *= 1 / difference;
                    }
                }
            }
            //vertical dash
            else if (MonkDashing < 0)
            {
                MonkDashing++;

                if (Player.velocity.Y > 0)
                {
                    Player.maxFallSpeed *= 10;
                    Player.gravity = 8;
                    //deactivate hover or those mounts refuse to dash down
                    Player.mount._data.usesHover = false;
                }

                if (MonkDashing == 0 && Player.mount.Active)
                {
                    Player.velocity *= 0.5f;

                    //add hover back
                    Player.mount._data.usesHover = BaseSquireMountData.usesHover;
                }
            }
        }

        public override void PostUpdate()
        {
            if (!FreeEaterSummon && !Main.npc.Any(n => n.active && (n.type == NPCID.EaterofWorldsHead || n.type == NPCID.EaterofWorldsBody || n.type == NPCID.EaterofWorldsTail)))
            {
                FreeEaterSummon = true;
            }

            if (NymphsPerfumeRestoreLife > 0 && --NymphsPerfumeRestoreLife == 0)
            {
                if (Player.statLife < Player.statLifeMax2)
                    Player.statLife = Player.statLifeMax2;
                //doing it like this so it accounts for your lifeMax after respawn
                //regular OnRespawn() doesnt account for lifeforce, and is lowered by dying with oceanic maul
            }

            if (SquireEnchantItem == null && BaseMountType != -1)
            {
                SquireEnchant.ResetMountStats(this);
            }

            ConcentratedRainbowMatterTryAutoHeal();
        }


        public override void PostUpdateBuffs()
        {
            if (Berserked && !Player.CCed)
            {
                Player.controlUseItem = true;
                Player.releaseUseItem = true;
            }

            if (LowGround)
            {
                Player.gravControl = false;
                Player.gravControl2 = false;
            }
        }

        public override void PostUpdateEquips()
        {

            if (MeteorMomentum && !NoMomentum && !Player.mount.Active) //overriden by nomomentum
            {
                Player.runAcceleration *= 1.3f;
                Player.runSlowdown *= 1.3f;

            }
            if (NoMomentum && !Player.mount.Active)
            {
                if (Player.vortexStealthActive && Math.Abs(Player.velocity.X) > 6)
                    Player.vortexStealthActive = false;

                Player.runAcceleration *= 5f;
                Player.runSlowdown *= 5f;

                if (!IsStillHoldingInSameDirectionAsMovement)
                    Player.runSlowdown += 7f;
            }
            if (ApprenticeEnchantItem != null)
            {
                ApprenticeEnchant.ApprenticeSupport(Player);
            }
            if (TribalCharmEquipped)
            {
                Content.Items.Accessories.Masomode.TribalCharm.Effects(this);
            }

            if (PungentEyeball && Player.whoAmI == Main.myPlayer && Player.GetToggleValue("MasoPungentCursor"))
            {
                const float distance = 16 * 5;

                foreach (NPC n in Main.npc.Where(n => n.active && !n.dontTakeDamage && n.lifeMax > 5 && !n.friendly))
                {
                    if (Vector2.Distance(Main.MouseWorld, FargoSoulsUtil.ClosestPointInHitbox(n.Hitbox, Main.MouseWorld)) < distance)
                    {
                        n.AddBuff(ModContent.BuffType<PungentGazeBuff>(), 2, true);
                    }
                }

                for (int i = 0; i < 32; i++)
                {
                    Vector2 spawnPos = Main.MouseWorld + Main.rand.NextVector2CircularEdge(distance, distance);
                    Dust dust = Main.dust[Dust.NewDust(spawnPos, 0, 0, DustID.GemRuby, 0, 0, 100, Color.White)];
                    dust.scale = 0.5f;
                    dust.velocity = Vector2.Zero;
                    if (Main.rand.NextBool(3))
                    {
                        dust.velocity += Vector2.Normalize(Main.MouseWorld - dust.position) * Main.rand.NextFloat(5f);
                        dust.position += dust.velocity * 5f;
                    }
                    dust.noGravity = true;
                }
            }

            if (DarkenedHeartItem != null)
            {
                if (!IsStillHoldingInSameDirectionAsMovement)
                    Player.runSlowdown += 0.2f;
            }

            if (!StardustEnchantActive)
                FreezeTime = false;

            UpdateShield();

            if (ShadowEnchantActive)
                ShadowEffectPostEquips();

            Player.wingTimeMax = (int)(Player.wingTimeMax * WingTimeModifier);

            if (MutantAntibodies && Player.wet)
            {
                Player.wingTime = Player.wingTimeMax;
                Player.AddBuff(ModContent.BuffType<RefreshedBuff>(), 30 * 60);
            }

            if (StyxSet)
            {
                Player.accDreamCatcher = true; //dps counter is on

                //even if you attack weaker enemies or with less dps, you'll eventually get a charge
                if (StyxTimer > 0 && --StyxTimer == 1) //yes, 1, to avoid a possible edge case of frame perfect attacks blocking this
                {
                    int dps = Player.getDPS();
                    if (dps != 0)
                    {
                        int diff = StyxCrown.MINIMUM_DPS - dps;
                        if (diff > 0)
                            StyxMeter += diff / 2; //from testing: compared to 1.4.3, this was giving twice as much meter. thus we're halving it
                    }

                    //if (Player.getDPS() == 0) Main.NewText("bug! styx timer ran with 0 dps, show this to terry");
                }
            }
            else
            {
                StyxMeter = 0;
                StyxTimer = 0;
            }

            if (!GaiaSet)
                GaiaOffense = false;

            if (!EridanusSet)
                EridanusEmpower = false;

            if (QueenStingerItem != null)
            {
                if (Player.honey)
                    Player.GetArmorPenetration(DamageClass.Generic) += 10;
            }

            if (BeetleEnchantDefenseTimer > 0)
                BeetleEnchantDefenseTimer--;

            if (RabiesVaccine)
                Player.buffImmune[BuffID.Rabies] = true;

            if (AbomWandItem != null)
                AbomWandUpdate();

            if (Flipped && !Player.gravControl)
            {
                Player.gravControl = true;
                Player.controlUp = false;
                Player.gravDir = -1f;
                //Player.fallStart = (int)(Player.position.Y / 16f);
                //Player.jump = 0;
            }

            if (DevianttHeartItem != null)
            {
                if (DevianttHeartsCD > 0)
                    DevianttHeartsCD--;
            }

            if ((BetsysHeartItem != null || QueenStingerItem != null) && SpecialDashCD > 0 && --SpecialDashCD == 0)
            {
                SoundEngine.PlaySound(SoundID.Item9, Player.Center);

                for (int i = 0; i < 30; i++)
                {
                    int d = Dust.NewDust(Player.position, Player.width, Player.height, DustID.GemTopaz, 0, 0, 0, default, 2.5f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 4f;
                }
            }

            if (GuttedHeart)
                GuttedHeartEffect();

            if (Player.velocity.Y == 0) //practically, if on the ground or hooked or similar
            {
                CanSummonForbiddenStorm = true;
            }

            if (SlimyShieldItem != null || LihzahrdTreasureBoxItem != null || GelicWingsItem != null)
                OnLandingEffects();

            if (AgitatingLensItem != null)
                AgitatingLensEffect();

            if (WretchedPouchItem != null)
                WretchedPouchEffect();

            if (PalladEnchantItem != null)
                PalladiumEnchant.PalladiumUpdate(this);

            if (noDodge)
            {
                Player.onHitDodge = false;
                Player.shadowDodge = false;
                Player.blackBelt = false;
                Player.brainOfConfusionItem = null;
                GuttedHeart = false;
            }

            #region dashes

            if (Player.dashType != 0)
                HasDash = true;

            if (SolarEnchantActive)
                SolarEffect();

            if (ShinobiEnchantActive)
                ShinobiDashChecks();

            if (JungleEnchantActive)
                JungleEffect();

            if (DeerSinew)
                DeerSinewEffect();

            if (LihzahrdTreasureBoxItem != null)
                LihzahrdTreasureBoxUpdate();

            #endregion dashes

            if (PrecisionSealNoDashNoJump)
            {
                Player.dashType = 0;
                Player.GetJumpState(ExtraJump.CloudInABottle).Disable();
                Player.GetJumpState(ExtraJump.SandstormInABottle).Disable();
                Player.GetJumpState(ExtraJump.BlizzardInABottle).Disable();
                Player.GetJumpState(ExtraJump.FartInAJar).Disable();
                Player.GetJumpState(ExtraJump.TsunamiInABottle).Disable();
                Player.GetJumpState(ExtraJump.UnicornMount).Disable();
                JungleJumping = false;
                CanJungleJump = false;
                dashCD = 2;
                IsDashingTimer = 0;
                HasDash = false;
                Player.dashDelay = 10;

                if (lihzahrdFallCD < 2)
                    lihzahrdFallCD = 2;
            }

            DashManager.ManageDashes(Player);

            if (DeerclawpsItem != null && IsInADashState)
                DeerclawpsAttack(Player.Bottom);

            if (NecromanticBrewItem != null && IsInADashState && Player.GetToggleValue("MasoSkeleSpin"))
            {
                //if (NecromanticBrewRotation == 0)
                //{
                //    NecromanticBrewRotation = 0.001f;
                //    Player.velocity.X *= 1.1f;
                //}

                float dashSpeedBoost = 0.5f * Player.velocity.X;
                Player.position.X += dashSpeedBoost;
                if (Collision.SolidCollision(Player.position, Player.width, Player.height))
                    Player.position.X -= dashSpeedBoost;

                //Collision.StepUp(ref Player.position, ref Player.velocity, Player.width, Player.height, ref Player.stepSpeed, ref Player.gfxOffY, (int)Player.gravDir, Player.controlUp);

                Player.noKnockback = true;
                Player.thorns = 4f;

                NecromanticBrewRotation += 0.6f * Math.Sign(Player.velocity.X == 0 ? Player.direction : Player.velocity.X);
                Player.fullRotation = NecromanticBrewRotation;
                Player.fullRotationOrigin = Player.Center - Player.position;
            }
            else if (NecromanticBrewRotation != 0)
            {
                Player.fullRotation = 0f;
                NecromanticBrewRotation = 0f;
            }
        }
        public override void UpdateBadLifeRegen()
        {
            if (Player.electrified && Player.wet)
                Player.lifeRegen -= 16;

            void DamageOverTime(int badLifeRegen, bool affectLifeRegenCount = false)
            {
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;

                if (affectLifeRegenCount && Player.lifeRegenCount > 0)
                    Player.lifeRegen = 0;

                Player.lifeRegenTime = 0;
                Player.lifeRegen -= badLifeRegen;
            }

            if (NanoInjection)
                DamageOverTime(10);

            if (Shadowflame)
                DamageOverTime(10);

            if (GodEater)
            {
                DamageOverTime(170, true);

                Player.lifeRegenCount -= 70;
            }

            if (MutantNibble)
                DamageOverTime(0, true);

            if (Infested)
                DamageOverTime(InfestedExtraDot());

            if (Rotting)
                DamageOverTime(2);

            if (CurseoftheMoon)
                DamageOverTime(20);

            if (Oiled && Player.lifeRegen < 0)
            {
                Player.lifeRegen *= 2;
            }

            if (MutantPresence)
            {
                if (Player.lifeRegen > 5)
                    Player.lifeRegen = 5;
            }

            if (FlamesoftheUniverse)
                DamageOverTime((30 + 50 + 48 + 30) / 2, true);

            if (Smite)
                DamageOverTime(0, true);

            if (Anticoagulation)
                DamageOverTime(4, true);

            if (Player.onFire && Player.FargoSouls().AshWoodEnchantItem != null)
            {
                Player.lifeRegen += 8;
            }

            if (Player.lifeRegen < 0)
            {
                LeadEffect.ProcessLeadEffectLifeRegen(Player);

                FusedLensCanDebuff = true;
            }

            //placed here so it runs after our modded dots
            if (WorldSavingSystem.EternityMode && !WorldSavingSystem.MasochistModeReal)
            {
                //silently make it much harder to die to DOT debuffs
                if (Player.lifeRegen < 0 && Player.statLife < 10)
                    Player.lifeRegen = 0;
            }
        }

        public override void PostUpdateMiscEffects()
        {
            FargoSoulsPlayer modPlayer = Player.FargoSouls();

            //these are here so that emode minion nerf can properly detect the real set bonuses over in EModePlayer postupdateequips
            if (SquireEnchantItem != null)
                Player.setSquireT2 = true;

            if (ValhallaEnchantItem != null)
                Player.setSquireT3 = true;

            if (ApprenticeEnchantItem != null)
                Player.setApprenticeT2 = true;

            if (DarkArtistEnchantItem != null)
                Player.setApprenticeT3 = true;

            if (MonkEnchantActive)
                Player.setMonkT2 = true;

            if (ShinobiEnchantActive)
                Player.setMonkT3 = true;


            if (Player.channel && WeaponUseTimer < 2)
                WeaponUseTimer = 2;
            if (--WeaponUseTimer < 0)
                WeaponUseTimer = 0;

            if (IsDashingTimer > 0)
            {
                IsDashingTimer--;
                Player.dashDelay = -1;
            }

            if (GoldEnchMoveCoins)
            {
                ChestUI.MoveCoins(Player.inventory, Player.bank.item, ContainerTransferContext.FromUnknown(Player));
                GoldEnchMoveCoins = false;
            }

            if (SpectreCD > 0)
                SpectreCD--;

            if (RustRifleReloading && Player.HeldItem.type == ModContent.ItemType<NavalRustrifle>())
            {
                RustRifleTimer++;
            }

            if (ParryDebuffImmuneTime > 0)
            {
                ParryDebuffImmuneTime--;
                DreadShellVulnerabilityTimer = 0;
            }
            else if (DreadShellVulnerabilityTimer > 0)
            {
                DreadShellVulnerabilityTimer--;

                Player.statDefense -= 30;
                Player.endurance -= 0.3f;
            }

            if (HallowHealTime > 0)
            {
                bool hallowForce = HallowEnchantItem != null ? modPlayer.ForceEffect(HallowEnchantItem.type) : false;
                int healDelay = 60;
                int heal = hallowForce ? 17 : 14;
                if (HallowEnchantItem != null && HallowHealTime % healDelay == 0)
                {
                    Player.Heal(heal);
                }
                HallowHealTime--;
            }
            if (++frameCounter >= 60)
                frameCounter = 0;

            if (HealTimer > 0)
                HealTimer--;

            if (NebulaEnchantActive)
                NebulaEffect();

            if (LowGround)
            {
                Player.waterWalk = false;
                Player.waterWalk2 = false;
            }

            if (dashCD > 0)
                dashCD--;

            if (ReallyAwfulDebuffCooldown > 0)
                ReallyAwfulDebuffCooldown--;

            if (OceanicMaul && FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.fishBossEX, NPCID.DukeFishron))
            {
                Player.statLifeMax2 /= 5;
                if (Player.statLifeMax2 < 100)
                    Player.statLifeMax2 = 100;
            }

            if (StealingCooldown > 0 && !Player.dead)
                StealingCooldown--;

            if (LihzahrdCurse && Framing.GetTileSafely(Player.Center).WallType == WallID.LihzahrdBrickUnsafe)
            {
                Player.dangerSense = false;
                Player.InfoAccMechShowWires = false;
            }

            if (Graze) //decrease graze bonus over time
            {
                if (++DeviGrazeCounter > 60)
                {
                    DeviGrazeCounter = 0;
                    if (DeviGrazeBonus > 0f)
                        DeviGrazeBonus -= 0.01;
                }

                if (CirnoGrazeCounter > 0)
                    CirnoGrazeCounter--;
            }

            if (StabilizedGravity && Player.GetToggleValue("MasoGrav2", false))
                Player.gravity = Math.Max(Player.gravity, Player.defaultGravity);

            if (Atrophied)
            {
                Player.GetDamage(DamageClass.Melee) *= 0.01f;
                Player.GetCritChance(DamageClass.Melee) /= 100;
            }

            if (Slimed)
            {
                //slowed effect
                Player.moveSpeed *= .75f;
                Player.jump /= 2;
            }

            if (GodEater)
            {
                Player.statDefense *= 0;
                Player.endurance = 0;
            }

            if (MutantNibble) //disables lifesteal, mostly
            {
                if (Player.statLife > 0 && StatLifePrevious > 0 && Player.statLife > StatLifePrevious)
                    Player.statLife = StatLifePrevious;
            }

            if (Defenseless)
            {
                Player.statDefense -= 30;
                Player.endurance = 0;
                Player.longInvince = false;
                Player.noKnockback = false;
            }

            if (Purified)
            {
                //tries to remove all buffs/debuffs
                for (int i = Player.MaxBuffs - 1; i >= 0; i--)
                {
                    if (Player.buffType[i] > 0
                        && !Main.debuff[Player.buffType[i]] && !Main.buffNoTimeDisplay[Player.buffType[i]]
                        && !BuffID.Sets.TimeLeftDoesNotDecrease[Player.buffType[i]])
                    {
                        if (!KnownBuffsToPurify.ContainsKey(Player.buffType[i]))
                            KnownBuffsToPurify[Player.buffType[i]] = true;

                        Player.DelBuff(i);
                    }
                }

                foreach (int b in KnownBuffsToPurify.Keys)
                {
                    Player.buffImmune[b] = true;
                }
            }

            if (Asocial)
            {
                KillPets();
                Player.maxMinions = 0;
                Player.maxTurrets = 0;
            }
            else if (WasAsocial) //should only occur when above debuffs end
            {
                Player.hideMisc[0] = HidePetToggle0;
                Player.hideMisc[1] = HidePetToggle1;

                WasAsocial = false;
            }

            if (Rotting)
            {
                Player.moveSpeed *= 0.9f;
                //Player.statLifeMax2 -= Player.statLifeMax / 5;
                Player.statDefense -= 10;
                Player.endurance -= 0.1f;
                AttackSpeed -= 0.1f;
                Player.GetDamage(DamageClass.Generic) -= 0.1f;
            }

            if (Kneecapped)
            {
                Player.accRunSpeed = Player.maxRunSpeed;
            }

            ManageLifeReduction();

            if (Eternity)
                Player.statManaMax2 = 999;
            else if (UniverseSoul)
                Player.statManaMax2 += 300;

            if (AdditionalAttacks && AdditionalAttacksTimer > 0)
                AdditionalAttacksTimer--;

            if (MutantPresence || DevianttPresence)
            {
                Player.statDefense /= 2;
                Player.endurance /= 2;
                Player.shinyStone = false;
            }

            if (RockeaterDistance > EaterLauncher.BaseDistance)
            {
                RockeaterDistance -= (int)((EaterLauncher.IncreasedDistance - EaterLauncher.BaseDistance) / (EaterLauncher.CooldownTime / 3f));
            }
            else
            {
                RockeaterDistance = EaterLauncher.BaseDistance;
            }

            StatLifePrevious = Player.statLife;
        }
    }
}
