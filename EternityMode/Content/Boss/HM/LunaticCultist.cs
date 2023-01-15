using System.IO;
using Terraria.ModLoader.IO;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.ItemDropRules.Conditions;
using FargowiltasSouls.Items.Accessories.Masomode;
using FargowiltasSouls.Items.Consumables;
using FargowiltasSouls.NPCs;
using FargowiltasSouls.Projectiles;
using FargowiltasSouls.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Boss.HM
{
    public class LunaticCultist : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.CultistBoss);

        public float RitualRotation;

        public int MagicDamageCounter;
        public int MeleeDamageCounter;
        public int RangedDamageCounter;
        public int MinionDamageCounter;

        public bool EnteredPhase2;
        public bool DroppedSummon;



        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            binaryWriter.Write(RitualRotation);
            binaryWriter.Write7BitEncodedInt(MeleeDamageCounter);
            binaryWriter.Write7BitEncodedInt(RangedDamageCounter);
            binaryWriter.Write7BitEncodedInt(MagicDamageCounter);
            binaryWriter.Write7BitEncodedInt(MinionDamageCounter);
            bitWriter.WriteBit(EnteredPhase2);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            RitualRotation = binaryReader.ReadSingle();
            MeleeDamageCounter = binaryReader.Read7BitEncodedInt();
            RangedDamageCounter = binaryReader.Read7BitEncodedInt();
            MagicDamageCounter = binaryReader.Read7BitEncodedInt();
            MinionDamageCounter = binaryReader.Read7BitEncodedInt();
            EnteredPhase2 = bitReader.ReadBit();
        }

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.lifeMax *= 2;
        }

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            npc.buffImmune[BuffID.Suffocation] = true;
        }

        public override bool? CanBeHitByProjectile(NPC npc, Projectile projectile)
        {
            if (npc.ai[3] == -1f && FargoSoulsUtil.IsSummonDamage(projectile) && !ProjectileID.Sets.IsAWhip[projectile.type])
                return false;

            return base.CanBeHitByProjectile(npc, projectile);
        }

        public override bool SafePreAI(NPC npc)
        {
            bool result = base.SafePreAI(npc);

            EModeGlobalNPC.cultBoss = npc.whoAmI;

            if (FargoSoulsWorld.SwarmActive)
                return result;

            if (npc.ai[3] == -1f)
            {
                //if (Fargowiltas.Instance.MasomodeEXLoaded && npc.ai[1] >= 120f && npc.ai[1] < 419f) //skip summoning ritual LMAO
                //{
                //    npc.ai[1] = 419f;
                //    npc.netUpdate = true;
                //}

                if (npc.ai[0] == 5)
                {
                    if (npc.ai[1] == 1f)
                    {
                        RitualRotation = Main.rand.Next(360);
                        npc.netUpdate = true;
                        NetSync(npc);
                    }

                    if (npc.ai[1] > 30f && npc.ai[1] < 330f)
                    {
                        RitualRotation += 2f - Math.Min(1f, 2f * npc.life / npc.lifeMax); //always at least 1, begins scaling below 50% life
                        npc.Center = Main.player[npc.target].Center + 180f * Vector2.UnitX.RotatedBy(MathHelper.ToRadians(RitualRotation));
                    }

                    /*if (npc.ai[1] > 275f && npc.ai[1] < 330f) //dust that reveals the real cultist at last second
                    {
                        float modifier = 0;
                        int repeats = (int)npc.ai[1] - 275;
                        for (int i = 0; i < repeats; i++)
                            modifier = MathHelper.Lerp(modifier, 1f, 0.08f);
                        float distance = npc.height * 2 * modifier;
                        float rotation = MathHelper.TwoPi * modifier;
                        for (int i = 0; i < 4; i++)
                        {
                            int d = Dust.NewDust(npc.Center + distance * Vector2.UnitX.RotatedBy(rotation + MathHelper.TwoPi / 4 * i), 0, 0, 88, newColor: Color.White);
                            Main.dust[d].noGravity = true;
                            Main.dust[d].velocity *= npc.ai[1] > 315 ? 18f : 0.5f;
                            Main.dust[d].scale = 0.5f + 2.5f * modifier;
                        }
                    }*/
                }
            }
            else
            {
                //if (npc.ai[3] == 0) npc.damage = 0;

                //about to begin moving for ritual: 0 39 0 12
                //begin transit for ritual: 1 34 0 12
                //pause just before ritual: 0 0 0 13
                //ritual: 5 0 0 -1

                if (!EnteredPhase2 && npc.life < npc.lifeMax / 2) //p2 transition, force a ritual immediately
                {
                    EnteredPhase2 = true;
                    npc.ai[0] = 5;
                    npc.ai[1] = 0;
                    npc.ai[2] = 0;
                    npc.ai[3] = -1;
                    SoundEngine.PlaySound(SoundID.Roar, npc.Center);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        npc.netUpdate = true;
                        NetSync(npc);
                    }
                }

                //necessary because calameme
                int damage = Math.Max(75, FargoSoulsUtil.ScaledProjectileDamage(npc.damage));
                damage /= 4;

                switch ((int)npc.ai[0])
                {
                    case -1:
                        if (npc.ai[1] == 419f) //always start fight with a ritual
                        {
                            npc.ai[0] = 0f;
                            npc.ai[1] = 0f;
                            npc.ai[3] = 11f;
                            npc.netUpdate = true;
                        }
                        break;

                    case 2: //ice mist, frost wave support
                        if (EnteredPhase2)
                        {
                            if (npc.ai[1] < 60 && npc.ai[1] % 4 == 3)
                            {
                                int spacing = 14 - (int)(npc.ai[1] - 3) / 4; //start far and get closer
                                for (int j = -1; j <= 1; j += 2) //from above and below
                                {
                                    for (int i = -1; i <= 1; i += 2) //waves beside you
                                    {
                                        if (i == 0)
                                            continue;
                                        Vector2 spawnPos = Main.player[npc.target].Center;
                                        spawnPos.X += Math.Sign(i) * 150 * 2 + i * 120 * spacing;
                                        spawnPos.Y -= (700 + Math.Abs(i) * 50) * j;
                                        float speed = 8 + spacing * 0.8f;
                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                            Projectile.NewProjectile(npc.GetSource_FromThis(), spawnPos, Vector2.UnitY * speed * j, ProjectileID.FrostWave, damage / 3, 0f, Main.myPlayer);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (npc.ai[1] == (FargoSoulsWorld.MasochistModeReal ? 5f : 60f) && Main.netMode != NetmodeID.MultiplayerClient) //single wave
                            {
                                for (int i = 0; i < Main.maxNPCs; i++)
                                {
                                    if (Main.npc[i].active && Main.npc[i].type == NPCID.CultistBossClone)
                                    {
                                        Vector2 distance = Main.player[npc.target].Center - Main.npc[i].Center;
                                        distance.Normalize();
                                        distance *= Main.rand.NextFloat(8f, 9f);
                                        distance = distance.RotatedByRandom(Math.PI / 24);
                                        Projectile.NewProjectile(npc.GetSource_FromThis(), Main.npc[i].Center, distance,
                                            ProjectileID.FrostWave, damage / 3, 0f, Main.myPlayer);
                                    }
                                }
                            }
                        }
                        break;

                    case 3: //fireballs
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            if (EnteredPhase2) //fireball ring
                            {
                                if (npc.ai[1] == 3f)
                                {
                                    int max = NPC.CountNPCS(NPCID.CultistBossClone) * 2 + 6;

                                    Vector2 baseOffset = npc.DirectionTo(Main.player[npc.target].Center);
                                    const float spawnOffset = 1200f;
                                    const float speed = 7f;
                                    const float ai0 = spawnOffset / speed;
                                    for (int i = 0; i < max; i++)
                                    {
                                        Projectile.NewProjectile(npc.GetSource_FromThis(), Main.player[npc.target].Center + spawnOffset * baseOffset.RotatedBy(2 * Math.PI / max * i),
                                            -speed * baseOffset.RotatedBy(2 * Math.PI / max * i), ModContent.ProjectileType<CultistFireball>(),
                                            damage / 3, 0f, Main.myPlayer, ai0);
                                    }

                                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, npc.whoAmI, npc.type);
                                }
                            }

                            if (npc.ai[1] % 20 == 6) //homing flare support
                            {
                                for (int i = 0; i < Main.maxNPCs; i++)
                                {
                                    if (Main.npc[i].active && Main.npc[i].type == NPCID.CultistBossClone)
                                    {
                                        FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromAI(), Main.npc[i].Center, NPCID.SolarFlare, target: npc.target);
                                    }
                                }
                            }
                        }
                        break;

                    case 4: //lightning
                        if (npc.ai[1] == 19f && npc.HasPlayerTarget && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int cultistCount = 1;
                            for (int i = 0; i < Main.maxNPCs; i++)
                            {
                                if (Main.npc[i].active && Main.npc[i].type == NPCID.CultistBossClone)
                                {
                                    if (EnteredPhase2) //vortex lightning
                                    {
                                        Projectile.NewProjectile(npc.GetSource_FromThis(), Main.npc[i].Center, Main.rand.NextVector2Square(-15, 15), ModContent.ProjectileType<CultistVortex>(),
                                          damage / 15 * 6, 0, Main.myPlayer, 0f, cultistCount);
                                        cultistCount++;
                                    }
                                    else //aimed lightning
                                    {
                                        if (FargoSoulsWorld.MasochistModeReal)
                                        {
                                            Vector2 dir = Main.player[npc.target].Center - Main.npc[i].Center;
                                            float ai1New = Main.rand.Next(100);
                                            Vector2 vel = Vector2.Normalize(dir.RotatedByRandom(Math.PI / 4)) * 24f;
                                            Projectile.NewProjectile(npc.GetSource_FromThis(), Main.npc[i].Center, vel, ModContent.ProjectileType<HostileLightning>(),
                                                damage / 15 * 6, 0, Main.myPlayer, dir.ToRotation(), ai1New);
                                        }
                                        else
                                        {
                                            Vector2 vel = Main.npc[i].DirectionTo(Main.player[npc.target].Center).RotatedByRandom(MathHelper.ToRadians(5));
                                            vel *= Main.rand.NextFloat(4f, 6f);
                                            Projectile.NewProjectile(npc.GetSource_FromThis(), Main.npc[i].Center, vel, ModContent.ProjectileType<LightningVortexHostile>(), damage / 15 * 6, 0, Main.myPlayer);
                                        }
                                    }
                                }
                            }
                        }
                        break;

                    case 7:
                        if (npc.ai[1] == 3f && Main.netMode != NetmodeID.MultiplayerClient) //ancient light, jellyfish support
                        {
                            for (int i = 0; i < Main.maxProjectiles; i++)
                            {
                                if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<CultistRitual>())
                                {
                                    Projectile.NewProjectile(npc.GetSource_FromThis(), new Vector2(Main.projectile[i].Center.X, Main.player[npc.target].Center.Y - 700),
                                        Vector2.Zero, ModContent.ProjectileType<StardustRain>(), damage / 3, 0f, Main.myPlayer);
                                }
                            }
                        }
                        break;

                    case 8:
                        if (npc.ai[1] == 3f) //ancient doom, nebula sphere support
                        {
                            int t = npc.HasPlayerTarget ? npc.target : npc.FindClosestPlayer();
                            if (t != -1 && Main.player[t].active)
                            {
                                for (int i = 0; i < Main.maxNPCs; i++)
                                {
                                    if (Main.npc[i].active && Main.npc[i].type == NPCID.CultistBossClone)
                                        Projectile.NewProjectile(npc.GetSource_FromThis(), Main.npc[i].Center, Vector2.Zero, ProjectileID.NebulaSphere, damage / 15 * 6, 0f, Main.myPlayer);
                                }
                            }
                        }
                        break;

                    default:
                        break;
                }
            }

            npc.defense = npc.defDefense; //prevent vanilla p2 from lowering defense!
            Lighting.AddLight(npc.Center, 1f, 1f, 1f);

            EModeUtils.DropSummon(npc, "CultistSummon", NPC.downedAncientCultist, ref DroppedSummon, NPC.downedGolemBoss);

            return result;
        }

        public override bool CanHitPlayer(NPC npc, Player target, ref int CooldownSlot)
        {
            return false;
        }

        public override void SafeOnHitByItem(NPC npc, Player player, Item item, int damage, float knockback, bool crit)
        {
            base.SafeOnHitByItem(npc, player, item, damage, knockback, crit);

            if (item.CountsAsClass(DamageClass.Melee) || item.CountsAsClass(DamageClass.Throwing))
                MeleeDamageCounter += damage;
            if (item.CountsAsClass(DamageClass.Ranged))
                RangedDamageCounter += damage;
            if (item.CountsAsClass(DamageClass.Magic))
                MagicDamageCounter += damage;
            if (item.CountsAsClass(DamageClass.Summon))
                MinionDamageCounter += damage;
        }

        public override void SafeOnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit)
        {
            base.SafeOnHitByProjectile(npc, projectile, damage, knockback, crit);

            if (projectile.CountsAsClass(DamageClass.Melee) || projectile.CountsAsClass(DamageClass.Throwing))
                MeleeDamageCounter += damage;
            if (projectile.CountsAsClass(DamageClass.Ranged))
                RangedDamageCounter += damage;
            if (projectile.CountsAsClass(DamageClass.Magic))
                MagicDamageCounter += damage;
            if (FargoSoulsUtil.IsSummonDamage(projectile))
                MinionDamageCounter += damage;
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            LeadingConditionRule emodeRule = new LeadingConditionRule(new EModeDropCondition());
            emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<CelestialRune>()));
            emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<MutantsPact>()));
            emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ItemID.DungeonFishingCrateHard, 5));
            npcLoot.Add(emodeRule);
        }

        public override void LoadSprites(NPC npc, bool recolor)
        {
            base.LoadSprites(npc, recolor);

            LoadNPCSprite(recolor, npc.type);
            LoadBossHeadSprite(recolor, 24);
            LoadBossHeadSprite(recolor, 31);
            LoadGoreRange(recolor, 902, 903);
            LoadExtra(recolor, 30);
        }
    }

    public class LunaticCultistClone : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.CultistBossClone);

        public int TotalCultistCount;
        public int MyRitualPosition;

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            binaryWriter.Write7BitEncodedInt(TotalCultistCount);
            binaryWriter.Write7BitEncodedInt(MyRitualPosition);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            TotalCultistCount = binaryReader.Read7BitEncodedInt();
            MyRitualPosition = binaryReader.Read7BitEncodedInt();
        }

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            npc.buffImmune[ModContent.BuffType<ClippedWings>()] = true;
            npc.buffImmune[ModContent.BuffType<Lethargic>()] = true;
            npc.buffImmune[BuffID.Suffocation] = true;
        }

        public override bool? CanBeHitByProjectile(NPC npc, Projectile projectile)
        {
            if (FargoSoulsUtil.IsSummonDamage(projectile) && !ProjectileID.Sets.IsAWhip[projectile.type])
                return false;

            return base.CanBeHitByProjectile(npc, projectile);
        }

        public override bool SafePreAI(NPC npc)
        {
            bool result = base.SafePreAI(npc);

            NPC cultist = FargoSoulsUtil.NPCExists(npc.ai[3], NPCID.CultistBoss);
            if (cultist != null)
            {
                //during ritual
                if (cultist.ai[3] == -1 && cultist.ai[0] == 5) //&& cultist.ai[2] > -1 && cultist.ai[2] < Main.maxProjectiles)
                {
                    if (npc.alpha > 0)
                    {
                        TotalCultistCount = 1; //accounts for cultist boss
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            if (Main.npc[i].active && Main.npc[i].type == npc.type && Main.npc[i].ai[3] == npc.ai[3])
                            {
                                if (i == npc.whoAmI)
                                    MyRitualPosition = TotalCultistCount; //stores which one this is
                                TotalCultistCount++; //stores total number of cultists
                            }
                        }
                    }

                    if (cultist.ai[1] > 30f && cultist.ai[1] < 330f)
                    {
                        Vector2 offset = cultist.Center - Main.player[cultist.target].Center;
                        npc.Center = Main.player[cultist.target].Center + offset.RotatedBy(2 * Math.PI / TotalCultistCount * MyRitualPosition);
                        Lighting.AddLight(npc.Center, 1f, 1f, 1f);
                    }

                    /*int ritual = (int)cultist.ai[2]; //rotate around ritual
                    if (Main.projectile[ritual].active && Main.projectile[ritual].type == ProjectileID.CultistRitual)
                    {
                        Vector2 offset = cultist.Center - Main.projectile[ritual].Center;
                        npc.Center = Main.projectile[ritual].Center + offset.RotatedBy(2 * Math.PI / Counter[0] * Counter[1]);
                    }*/
                }
                else
                {
                    if (cultist.ai[3] == 0) //be visible always
                        npc.alpha = 0;
                }

                Lighting.AddLight(npc.Center, 1f, 1f, 1f);
            }

            return result;
        }

        public override void HitEffect(NPC npc, int hitDirection, double damage)
        {
            base.HitEffect(npc, hitDirection, damage);

            if (!FargoSoulsWorld.SwarmActive)
            {
                NPC cultist = FargoSoulsUtil.NPCExists(npc.ai[3], NPCID.CultistBoss);

                //yes, this spawns two clones without the check
                if (cultist != null && NPC.CountNPCS(npc.type) < (FargoSoulsWorld.MasochistModeReal ? Math.Min(TotalCultistCount + 1, 12) : TotalCultistCount))
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        FargoSoulsUtil.NewNPCEasy(cultist.GetSource_FromAI(), npc.Center, NPCID.CultistBossClone, 0, npc.ai[0], npc.ai[1], npc.ai[2], npc.ai[3], npc.target);
                    }
                }
            }
        }

        public override void LoadSprites(NPC npc, bool recolor)
        {
            base.LoadSprites(npc, recolor);

            LoadNPCSprite(recolor, npc.type);
        }
    }

    public class AncientDoom : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.AncientDoom);

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.lifeMax *= 4;
        }

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            npc.buffImmune[BuffID.Suffocation] = true;
        }

        public override bool CanHitPlayer(NPC npc, Player target, ref int CooldownSlot)
        {
            return base.CanHitPlayer(npc, target, ref CooldownSlot) && npc.localAI[3] > 120;
        }

        public override bool SafePreAI(NPC npc)
        {
            bool result = base.SafePreAI(npc);

            if (npc.localAI[3] == 0f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 pivot = npc.Center + new Vector2(250f, 0f).RotatedByRandom(2 * Math.PI);
                    npc.ai[2] = pivot.X;
                    npc.ai[3] = pivot.Y;
                    npc.netUpdate = true;
                }
            }

            npc.localAI[3]++;

            if (npc.ai[2] > 0f && npc.ai[3] > 0f)
            {
                Vector2 pivot = new Vector2(npc.ai[2], npc.ai[3]);
                npc.velocity = Vector2.Normalize(pivot - npc.Center).RotatedBy(Math.PI / 2) * 6f;
            }

            return result;
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 300);
            target.AddBuff(ModContent.BuffType<Shadowflame>(), 300);
        }
    }

    public class AncientLight : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.AncientLight);

        public int Timer;

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.lavaImmune = true;
            //MoonLordAlive = FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.moonBoss, NPCID.MoonLordCore);

            npc.dontTakeDamage = true;
            npc.immortal = true;
            npc.chaseable = false;
        }

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            npc.buffImmune[BuffID.Suffocation] = true;
            npc.buffImmune[BuffID.OnFire] = true;
        }

        public override bool SafePreAI(NPC npc)
        {
            bool result = base.SafePreAI(npc);

            if (FargoSoulsWorld.SwarmActive)
                return result;

            npc.dontTakeDamage = true;
            npc.immortal = true;
            npc.chaseable = false;

            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.moonBoss, NPCID.MoonLordCore))
            {
                if (npc.HasPlayerTarget)
                {
                    Vector2 speed = Main.player[npc.target].Center - npc.Center;
                    speed.Normalize();
                    speed *= 9f;

                    npc.ai[2] += speed.X / 100f;
                    if (npc.ai[2] > 9f)
                        npc.ai[2] = 9f;
                    if (npc.ai[2] < -9f)
                        npc.ai[2] = -9f;
                    npc.ai[3] += speed.Y / 100f;
                    if (npc.ai[3] > 9f)
                        npc.ai[3] = 9f;
                    if (npc.ai[3] < -9f)
                        npc.ai[3] = -9f;
                }
                else
                {
                    npc.TargetClosest(false);
                }

                Timer++;
                if (Timer > 240)
                {
                    npc.HitEffect(0, 9999);
                    npc.active = false;
                }

                npc.velocity.X = npc.ai[2];
                npc.velocity.Y = npc.ai[3];
            }
            else if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.cultBoss, NPCID.CultistBoss) && !FargoSoulsWorld.MasochistModeReal)
            {
                if (++Timer > 20 && Timer < 40)
                {
                    npc.position -= npc.velocity;
                    return false;
                }

                if (Timer > 180)
                {
                    npc.dontTakeDamage = false;
                    npc.immortal = false;
                }
            }

            return result;
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(ModContent.BuffType<Purified>(), 300);
        }
    }

    public class CultistDragon : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.CultistDragonBody1,
            NPCID.CultistDragonBody2,
            NPCID.CultistDragonBody3,
            NPCID.CultistDragonBody4,
            NPCID.CultistDragonHead,
            NPCID.CultistDragonTail
        );

        public int DamageReductionTimer;

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            npc.buffImmune[BuffID.Suffocation] = true;

            if (npc.type == NPCID.CultistDragonHead)
            {
                if (FargoSoulsWorld.MasochistModeReal && FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.cultBoss, NPCID.CultistBoss))
                    npc.Center = Main.npc[EModeGlobalNPC.cultBoss].Center;

                if (NPC.CountNPCS(NPCID.AncientCultistSquidhead) < 4 && Main.netMode != NetmodeID.MultiplayerClient)
                    FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromAI(), npc.Center, NPCID.AncientCultistSquidhead);
            }
        }

        public override bool SafePreAI(NPC npc)
        {
            bool result = base.SafePreAI(npc);

            DamageReductionTimer++;

            return result;
        }

        public override bool StrikeNPC(NPC npc, ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            damage = damage * Math.Min(1.0, DamageReductionTimer / 300.0);

            return base.StrikeNPC(npc, ref damage, defense, ref knockback, hitDirection, ref crit);
        }

        public override void SafeModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            base.SafeModifyHitByProjectile(npc, projectile, ref damage, ref knockback, ref crit, ref hitDirection);

            if (projectile.maxPenetrate > 1)
                damage /= projectile.maxPenetrate;
            else if (projectile.maxPenetrate < 0)
                damage /= 4;
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(ModContent.BuffType<CurseoftheMoon>(), 360);
            target.AddBuff(ModContent.BuffType<MutantNibble>(), 300);
        }
    }

    public class AncientVision : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.AncientCultistSquidhead);

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            if (!FargoSoulsWorld.MasochistModeReal)
                npc.lifeMax /= 2;
        }

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            npc.buffImmune[BuffID.Suffocation] = true;
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(ModContent.BuffType<CurseoftheMoon>(), 360);
            target.AddBuff(ModContent.BuffType<MutantNibble>(), 300);
        }
    }
}
