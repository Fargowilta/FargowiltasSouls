using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Projectiles.Masomode;
using FargowiltasSouls.Content.Projectiles.Minions;
using FargowiltasSouls.Content.Projectiles.Souls;
using FargowiltasSouls.Content.Projectiles;
using Microsoft.Xna.Framework;
using System;
using FargowiltasSouls.Common.Utilities;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using FargowiltasSouls.Content.Items.Armor;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Content.Bosses.DeviBoss;
using FargowiltasSouls.Content.Bosses.AbomBoss;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Content.Buffs;
using Terraria.WorldBuilding;
using Terraria.Audio;
using FargowiltasSouls.Content.Items.Accessories.Masomode;
using FargowiltasSouls.Core.Systems;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Terraria.Localization;

namespace FargowiltasSouls.Core.ModPlayers
{
	public partial class FargoSoulsPlayer
    {
        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (proj.hostile)
                return;

            if (MinionCrits && FargoSoulsUtil.IsSummonDamage(proj))
            {
                if (Main.rand.Next(100) < Player.ActualClassCrit(DamageClass.Summon))
                    modifiers.SetCrit();
            }

            if (SqueakyToy)
            {
                modifiers.FinalDamage.Base = 1;
                Squeak(target.Center);
                return;
            }

            if (Asocial && FargoSoulsUtil.IsSummonDamage(proj, true, false))
            {
                modifiers.Null();
            }

            if (Atrophied && (proj.CountsAsClass(DamageClass.Melee) || proj.CountsAsClass(DamageClass.Throwing)))
            {
                modifiers.Null();
            }
            ModifyHitNPCBoth(target, ref modifiers, proj.DamageType);
        }

        public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers)
        {


            if (SqueakyToy)
            {
                modifiers.SetMaxDamage(1);
                Squeak(target.Center);
                return;
            }

            if (Atrophied)
            {
                modifiers.Null();
            }

            ModifyHitNPCBoth(target, ref modifiers, item.DamageType);
        }

        public void ModifyHitNPCBoth(NPC target, ref NPC.HitModifiers modifiers, DamageClass damageClass)
        {
            
            
            modifiers.ModifyHitInfo += (ref NPC.HitInfo hitInfo) =>
            {
                
                if (hitInfo.Crit)
                {
                    if (Eternity)
                    {
                        hitInfo.Damage *= 5;
                        target.AddBuff(ModContent.BuffType<FlamesoftheUniverseBuff>(), 240);
                    }
                    else if (UniverseSoul)
                    {
                        hitInfo.Damage *= 2;
                        target.AddBuff(ModContent.BuffType<FlamesoftheUniverseBuff>(), 240);
                    }
                    else if (UniverseCore)
                    {
                        float crit = Player.ActualClassCrit(damageClass) / 2;

                        if (Main.rand.NextFloat(100) < crit) //supercrit
                        {
                            hitInfo.Damage *= 2;
                            target.AddBuff(ModContent.BuffType<FlamesoftheUniverseBuff>(), 240);
                            SoundEngine.PlaySound(SoundID.Item147 with { Pitch = 1, Volume = 0.7f }, target.Center);
                        }
                    }
                    if (MinionCrits && damageClass.CountsAsClass(DamageClass.Summon) && !TerrariaSoul)
                        hitInfo.Damage = (int)(hitInfo.Damage * 0.75);
                }

                if (Hexed || (ReverseManaFlow && damageClass == DamageClass.Magic))
                {
                    target.life += hitInfo.Damage;
                    target.HealEffect(hitInfo.Damage);
                    if (target.life > target.lifeMax)
                    {
                        target.life = target.lifeMax;
                    }
                    hitInfo.Null();
                    return;

                }
            };

            if (DeerSinewNerf)
            {
                float ratio = Math.Min(Player.velocity.Length() / 20f, 1f);
                modifiers.FinalDamage *= MathHelper.Lerp(1f, 0.85f, ratio);
            }

            if (CerebralMindbreak)
                modifiers.FinalDamage *= 0.7f;

            if (FirstStrike)
            {
                modifiers.SetCrit();
                modifiers.FinalDamage *= 1.5f;
                Player.ClearBuff(ModContent.BuffType<FirstStrikeBuff>());
                //target.defense -= 5;
                target.AddBuff(BuffID.BrokenArmor, 600);
            }
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)/* tModPorter If you don't need the Projectile, consider using OnHitNPC instead */
        {
            if (target.type == NPCID.TargetDummy || target.friendly)
                return;

            if (proj.minion)// && proj.type != ModContent.ProjectileType<CelestialRuneAncientVision>() && proj.type != ModContent.ProjectileType<SpookyScythe>())
                TryAdditionalAttacks(proj.damage, proj.DamageType);

            OnHitNPCEither(target, hit, proj.DamageType, projectile: proj);
        }

        private void OnHitNPCEither(NPC target, NPC.HitInfo hitInfo, DamageClass damageClass, Projectile projectile = null, Item item = null)
        {

            //doing this so that damage-inheriting effects dont double dip or explode due to taking on crit boost
            int GetBaseDamage()
            {
                // TODO: I guess? test this
                int baseDamage = hitInfo.SourceDamage;
                if (projectile != null)
                    baseDamage = projectile.damage;
                else if (item != null)
                    baseDamage = Player.GetWeaponDamage(item);
                return baseDamage;
            }

            if (StyxSet)
            {
                StyxMeter += hitInfo.Damage;
                if (StyxTimer <= 0 && !target.friendly && target.lifeMax > 5 && target.type != NPCID.TargetDummy)
                    StyxTimer = 60;
            }


            if (Player.HasEffect<TitaniumEffect>() && (projectile == null || projectile.type != ProjectileID.TitaniumStormShard))
            {
                TitaniumEffect.TitaniumShards(this, Player);
            }

            if (DevianttHeartItem != null && DevianttHeartsCD <= 0 && Player.HasEffect<DevianttHearts>()
                && (projectile == null || (projectile.type != ModContent.ProjectileType<FriendRay>() && projectile.type != ModContent.ProjectileType<FriendHeart>())))
            {
                DevianttHeartsCD = AbomWandItem == null ? 600 : 300;

                if (Main.myPlayer == Player.whoAmI)
                {
                    Vector2 offset = 300 * Player.DirectionFrom(Main.MouseWorld);
                    for (int i = -3; i <= 3; i++)
                    {
                        Vector2 spawnPos = Player.Center + offset.RotatedBy(Math.PI / 7 * i);
                        Vector2 speed = Vector2.Normalize(Main.MouseWorld - spawnPos);

                        int baseHeartDamage = AbomWandItem == null ? 17 : 170;
                        //heartDamage = (int)(heartDamage * Player.ActualClassDamage(DamageClass.Summon));

                        float ai1 = (Main.MouseWorld - spawnPos).Length() / 17;

                        if (MutantEyeItem == null)
                            FargoSoulsUtil.NewSummonProjectile(Player.GetSource_Accessory(DevianttHeartItem), spawnPos, 17f * speed, ModContent.ProjectileType<FriendHeart>(), baseHeartDamage, 3f, Player.whoAmI, -1, ai1);
                        else
                            FargoSoulsUtil.NewSummonProjectile(Player.GetSource_Accessory(DevianttHeartItem), spawnPos, speed, ModContent.ProjectileType<FriendRay>(), baseHeartDamage, 3f, Player.whoAmI, (float)Math.PI / 7 * i);

                        FargoSoulsUtil.HeartDust(spawnPos, speed.ToRotation());
                    }
                }
            }

            if (GodEaterImbue)
            {
                /*if (target.FindBuffIndex(ModContent.BuffType<GodEater>()) < 0 && target.aiStyle != 37)
                {
                    if (target.type != ModContent.NPCType<MutantBoss>())
                    {
                        target.DelBuff(4);
                        target.buffImmune[ModContent.BuffType<GodEater>()] = false;
                    }
                }*/
                target.AddBuff(ModContent.BuffType<GodEaterBuff>(), 420);
            }


            //            /*if (PalladEnchant && !TerrariaSoul && palladiumCD == 0 && !target.immortal && !Player.moonLeech)
            //            {
            //                int heal = damage / 10;

            //                if ((EarthForce) && heal > 16)
            //                    heal = 16;
            //                else if (!EarthForce && !WizardEnchant && heal > 8)
            //                    heal = 8;
            //                else if (heal < 1)
            //                    heal = 1;
            //                Player.statLife += heal;
            //                Player.HealEffect(heal);
            //                palladiumCD = 240;
            //            }*/

            if (NymphsPerfume && NymphsPerfumeCD <= 0 && !target.immortal && !Player.moonLeech)
            {
                NymphsPerfumeCD = 600;

                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Item.NewItem(Player.GetSource_OnHit(target), target.Hitbox, ItemID.Heart);
                }
                else if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    var netMessage = Mod.GetPacket();
                    netMessage.Write((byte)FargowiltasSouls.PacketID.RequestPerfumeHeart);
                    netMessage.Write((byte)Player.whoAmI);
                    netMessage.Write((byte)target.whoAmI);
                    netMessage.Send();
                }
            }

            if (MasochistSoul)
            {
                target.AddBuff(ModContent.BuffType<SadismBuff>(), 600);
                //if (target.FindBuffIndex(ModContent.BuffType<Sadism>()) < 0 && target.aiStyle != 37)
                //{
                //    if (target.type != ModContent.NPCType<MutantBoss>())
                //    {
                //        target.DelBuff(4);
                //        target.buffImmune[ModContent.BuffType<Sadism>()] = false;
                //    }
                //    target.AddBuff(ModContent.BuffType<Sadism>(), 600);
                //}
            }

            if (FusedLens)
            {
                if (Player.onFire2 || FusedLensCanDebuff)
                    target.AddBuff(BuffID.CursedInferno, 360);
                if (Player.ichor || FusedLensCanDebuff)
                    target.AddBuff(BuffID.Ichor, 360);
            }

            if (Supercharged)
            {
                target.AddBuff(BuffID.Electrified, 240);
                target.AddBuff(ModContent.BuffType<LightningRodBuff>(), 60);
            }

            if (DarkenedHeartItem != null)
                DarkenedHeartAttack(projectile);
        }

        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.type == NPCID.TargetDummy || target.friendly)
                return;

            OnHitNPCEither(target, hit, item.DamageType, item: item);
        }
        private void ApplyDR(Player player, float dr, ref Player.HurtModifiers modifiers)
        {
            float DRCap = 0.75f;
            player.endurance += dr;
            if (WorldSavingSystem.EternityMode)
            {
                if (Player.endurance > DRCap)
                {
                    player.endurance = DRCap;
                }
            }
        }
        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            float dr = 0;
            dr += NecromanticBrew.NecroBrewDashDR(Player);

            if (npc.FargoSouls().Corrupted || npc.FargoSouls().CorruptedForce)
                dr += 0.2f;

            if (npc.FargoSouls().BloodDrinker)
                dr -= 0.3f;

            if (Player.HasBuff(ModContent.BuffType<ShellHideBuff>()))
                dr -= 1;

            if (Smite)
                dr -= 0.2f;

            if (npc.coldDamage && Hypothermia)
                dr -= 0.2f;

            if (npc.FargoSouls().CurseoftheMoon)
                dr += 0.2f;

            dr += Player.AccessoryEffects().ContactDamageDR(npc, ref modifiers);

            ApplyDR(Player, dr, ref modifiers);
        }

        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            float dr = 0;

            dr += NecromanticBrew.NecroBrewDashDR(Player);

            if (Smite)
                dr -= 0.2f;

            if (proj.coldDamage && Hypothermia)
                dr -= 0.2f;

            dr += Player.AccessoryEffects().ProjectileDamageDR(proj, ref modifiers);

            ApplyDR(Player, dr, ref modifiers);
        }

        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            OnHitByEither(npc, null);
        }

        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            OnHitByEither(null, proj);
        }

        public void OnHitByEither(NPC npc, Projectile proj)
        {
            if (Anticoagulation && Main.myPlayer == Player.whoAmI)
            {
                Entity source = null;
                if (npc != null)
                    source = npc;
                else if (proj != null)
                    source = proj;

                int type = ModContent.ProjectileType<Bloodshed>();
                for (int i = 0; i < 6; i++)
                {
                    if (Main.rand.NextBool(Player.ownedProjectileCounts[type] + 2))
                    {
                        const float speed = 12f;
                        Projectile.NewProjectile(Player.GetSource_OnHurt(source), Player.Center, Main.rand.NextVector2Circular(speed, speed), type, 0, 0f, Main.myPlayer, 0f);
                    }
                }
            }

            if (ModContent.GetInstance<SoulConfig>().BigTossMode)
            {
                AddBuffNoStack(ModContent.BuffType<StunnedBuff>(), 60);

                Vector2 attacker = default;
                if (npc != null)
                    attacker = npc.Center;
                else if (proj != null)
                    attacker = proj.Center;
                if (attacker != default)
                    Player.velocity = Vector2.Normalize(Player.Center - attacker) * 30 * 2;
            }
        }

        public override bool CanBeHitByNPC(NPC npc, ref int CooldownSlot)
        {
            if (BetsyDashing || GoldShell)
                return false;
            return true;
        }

        public override bool CanBeHitByProjectile(Projectile proj)
        {
            if (BetsyDashing || GoldShell)
                return false;
            if (Player.HasEffect<PrecisionSealHurtbox>() && !proj.Colliding(proj.Hitbox, GetPrecisionHurtbox()))
                return false;
            return true;
        }
        public override void ModifyHurt(ref Player.HurtModifiers modifiers)/* tModPorter Override ImmuneTo, FreeDodge or ConsumableDodge instead to prevent taking damage */
        {
            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.deviBoss, ModContent.NPCType<DeviBoss>()) && EModeGlobalNPC.deviBoss.IsWithinBounds(Main.maxNPCs))
                ((DeviBoss)Main.npc[EModeGlobalNPC.deviBoss].ModNPC).playerInvulTriggered = true;

            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.abomBoss, ModContent.NPCType<AbomBoss>()) && EModeGlobalNPC.abomBoss.IsWithinBounds(Main.maxNPCs))
                ((AbomBoss)Main.npc[EModeGlobalNPC.abomBoss].ModNPC).playerInvulTriggered = true;

            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.mutantBoss, ModContent.NPCType<MutantBoss>()) && EModeGlobalNPC.mutantBoss.IsWithinBounds(Main.maxNPCs))
                ((MutantBoss)Main.npc[EModeGlobalNPC.mutantBoss].ModNPC).playerInvulTriggered = true;

            if (DeathMarked)
                modifiers.SourceDamage *= 1.5f;

            if (Player.whoAmI == Main.myPlayer && !noDodge && Player.HasEffect<SqueakEffect>() && Main.rand.NextBool(10))
            {
                Squeak(Player.Center);
                modifiers.SetMaxDamage(1);
            }

            modifiers.ModifyHurtInfo += TryParryAttack;

            if (StyxSet && !BetsyDashing && !GoldShell && Player.ownedProjectileCounts[ModContent.ProjectileType<StyxArmorScythe>()] > 0)
            {
                modifiers.ModifyHurtInfo += (ref Player.HurtInfo hurtInfo) =>
                {
                    if(hurtInfo.Damage <= 1) return;
                    
                    int scythesSacrificed = 0;
                    const int maxSacrifice = 4;
                    const double maxDR = 0.20;
                    int scytheType = ModContent.ProjectileType<StyxArmorScythe>();
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        if (Main.projectile[i].active && Main.projectile[i].type == scytheType && Main.projectile[i].owner == Player.whoAmI)
                        {
                            if (Player.whoAmI == Main.myPlayer)
                                Main.projectile[i].Kill();
                            if (++scythesSacrificed >= maxSacrifice)
                                break;
                        }
                    }

                    // should not go below 1 due to math so no hacking here
                    hurtInfo.Damage = (int)(hurtInfo.Damage * (1.0f - (float)maxDR / maxSacrifice * scythesSacrificed));
                };
            }

            if (DeerSinewNerf && DeerSinewFreezeCD <= 0 && (modifiers.DamageSource.SourceNPCIndex.IsWithinBounds(Main.maxNPCs) || (modifiers.DamageSource.SourceProjectileType.IsWithinBounds(Main.maxProjectiles) && Main.projectile[modifiers.DamageSource.SourceProjectileType].aiStyle != ProjAIStyleID.FallingTile)))
            {
                DeerSinewFreezeCD = 120;
                FargoSoulsUtil.AddDebuffFixedDuration(Player, BuffID.Frozen, 20);
            }
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            WasHurtBySomething = true;

            MahoganyCanUseDR = false;

            if (Player.HasBuff(ModContent.BuffType<TitaniumDRBuff>())
                && !Player.HasBuff(ModContent.BuffType<TitaniumCDBuff>()))
            {
                Player.AddBuff(ModContent.BuffType<TitaniumCDBuff>(), LumUtils.SecondsToFrames(10));
            }

            if (NekomiSet && NekomiHitCD <= 0)
            {
                NekomiHitCD = 90;

                const int heartsLost = 1;
                int meterPerHeart = NekomiHood.MAX_METER / NekomiHood.MAX_HEARTS;
                int meterLost = meterPerHeart * heartsLost;

                int heartsToConsume = NekomiMeter / meterPerHeart;
                if (heartsToConsume > heartsLost)
                    heartsToConsume = heartsLost;
                Player.AddBuff(BuffID.RapidHealing, LumUtils.SecondsToFrames(heartsToConsume) * 5 / heartsLost);

                NekomiMeter -= meterLost;
                if (NekomiMeter < 0)
                    NekomiMeter = 0;
            }

            if (ShellHide)
            {
                TurtleShellHP--;

                //some funny dust
                const int max = 30;
                for (int i = 0; i < max; i++)
                {
                    Vector2 vector6 = Vector2.UnitY * 5f;
                    vector6 = vector6.RotatedBy((i - (max / 2 - 1)) * 6.28318548f / max) + Main.LocalPlayer.Center;
                    Vector2 vector7 = vector6 - Main.LocalPlayer.Center;
                    int d = Dust.NewDust(vector6 + vector7, 0, 0, DustID.GoldFlame, 0f, 0f, 0, default, 2f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity = vector7;
                }
            }

            if (Defenseless)
            {
                SoundEngine.PlaySound(SoundID.Item27, Player.Center);
                for (int i = 0; i < 30; i++)
                {
                    int d = Dust.NewDust(Player.position, Player.width, Player.height, DustID.t_SteampunkMetal, 0, 0, 0, default, 2f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 5f;
                }
            }

            if (Midas && Main.myPlayer == Player.whoAmI)
                Player.DropCoins();

            DeviGrazeBonus = 0;
            DeviGrazeCounter = 0;

            if (Main.myPlayer == Player.whoAmI)
            {
                if (WorldSavingSystem.MasochistModeReal && FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.mutantBoss, ModContent.NPCType<MutantBoss>()) && EModeGlobalNPC.mutantBoss.IsWithinBounds(Main.maxNPCs))
                {
                    if (!Player.HasBuff(ModContent.BuffType<TimeFrozenBuff>()))
                    {
                        The22Incident++;
                        Rectangle rect = new Rectangle((int)Player.Center.X - 111, (int)Player.Center.Y, 222, 222);
                        for (int i = 0; i < The22Incident; i++)
                            CombatText.NewText(rect, Color.DarkOrange, The22Incident, true);
                        if (The22Incident >= 22)
                        {
                            Player.KillMe(Terraria.DataStructures.PlayerDeathReason.ByCustomReason(Language.GetTextValue("Mods.FargowiltasSouls.DeathMessage.TwentyTwo", Player.name)), 22222222, 0);
                            Projectile.NewProjectile(Player.GetSource_Death(), Player.Center, Vector2.Zero, ModContent.ProjectileType<TwentyTwo>(), 0, 0f, Main.myPlayer);
                            Screenshake = 120;
                        }
                    }
                }
                else
                {
                    The22Incident = 0;
                }
            }
        }
    }
}
