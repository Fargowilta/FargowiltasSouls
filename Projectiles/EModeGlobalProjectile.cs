using FargowiltasSouls.Buffs.Boss;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode;
using FargowiltasSouls.EternityMode.Content.Boss.HM;
using FargowiltasSouls.EternityMode.Content.Boss.PHM;
using FargowiltasSouls.NPCs;
using FargowiltasSouls.NPCs.Champions;
using FargowiltasSouls.Projectiles.Champions;
using FargowiltasSouls.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles
{
    public class EModeGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public bool HasKillCooldown;
        public bool EModeCanHurt = true;
        public int NerfDamageBasedOnProjTypeCount;
        public bool altBehaviour;

        private int counter;
        private bool preAICheckDone;
        private bool firstTickAICheckDone;

        public static Dictionary<int, bool> IgnoreMinionNerf = new Dictionary<int, bool>();

        public List<int> ReworkedSpears = new List<int>
            {
                ProjectileID.Spear,
                ProjectileID.AdamantiteGlaive,
                ProjectileID.CobaltNaginata,
                ProjectileID.MythrilHalberd,
                ProjectileID.OrichalcumHalberd,
                ProjectileID.PalladiumPike,
                ProjectileID.TitaniumTrident,
                ProjectileID.Trident,
                ProjectileID.ObsidianSwordfish,
                ProjectileID.Swordfish,
                ProjectileID.ChlorophytePartisan
            };
            

        public override void Unload()
        {
            base.Unload();

            IgnoreMinionNerf.Clear();
        }

        public override void SetStaticDefaults()
        {
            IgnoreMinionNerf[ProjectileID.StardustDragon1] = true;
            IgnoreMinionNerf[ProjectileID.StardustDragon2] = true;
            IgnoreMinionNerf[ProjectileID.StardustDragon3] = true;
            IgnoreMinionNerf[ProjectileID.StardustDragon4] = true;
            IgnoreMinionNerf[ProjectileID.StormTigerGem] = true;
            IgnoreMinionNerf[ProjectileID.StormTigerTier1] = true;
            IgnoreMinionNerf[ProjectileID.StormTigerTier2] = true;
            IgnoreMinionNerf[ProjectileID.StormTigerTier3] = true;

            A_SourceNPCGlobalProjectile.SourceNPCSync[ProjectileID.SharpTears] = true;
            A_SourceNPCGlobalProjectile.SourceNPCSync[ProjectileID.JestersArrow] = true;
            A_SourceNPCGlobalProjectile.SourceNPCSync[ProjectileID.MeteorShot] = true;
            A_SourceNPCGlobalProjectile.SourceNPCSync[ProjectileID.ShadowFlame] = true;
            A_SourceNPCGlobalProjectile.SourceNPCSync[ProjectileID.MoonlordBullet] = true;
            A_SourceNPCGlobalProjectile.SourceNPCSync[ProjectileID.WaterBolt] = true;
            A_SourceNPCGlobalProjectile.SourceNPCSync[ProjectileID.WaterStream] = true;
            A_SourceNPCGlobalProjectile.SourceNPCSync[ProjectileID.DeathSickle] = true;
            A_SourceNPCGlobalProjectile.SourceNPCSync[ProjectileID.IceSickle] = true;
            A_SourceNPCGlobalProjectile.SourceNPCSync[ProjectileID.SwordBeam] = true;
            A_SourceNPCGlobalProjectile.SourceNPCSync[ProjectileID.CultistBossFireBall] = true;
            A_SourceNPCGlobalProjectile.SourceNPCSync[ProjectileID.CultistBossFireBallClone] = true;
            A_SourceNPCGlobalProjectile.SourceNPCSync[ProjectileID.SharknadoBolt] = true;
            A_SourceNPCGlobalProjectile.SourceNPCSync[ProjectileID.BloodShot] = true;
            A_SourceNPCGlobalProjectile.SourceNPCSync[ProjectileID.HallowBossRainbowStreak] = true;
            A_SourceNPCGlobalProjectile.SourceNPCSync[ProjectileID.HallowBossLastingRainbow] = true;
            A_SourceNPCGlobalProjectile.SourceNPCSync[ProjectileID.FairyQueenLance] = true;
            A_SourceNPCGlobalProjectile.SourceNPCSync[ProjectileID.BulletDeadeye] = true;
            A_SourceNPCGlobalProjectile.SourceNPCSync[ProjectileID.JestersArrow] = true;
            A_SourceNPCGlobalProjectile.SourceNPCSync[ProjectileID.MeteorShot] = true;
            A_SourceNPCGlobalProjectile.SourceNPCSync[ProjectileID.MoonlordBullet] = true;
            A_SourceNPCGlobalProjectile.SourceNPCSync[ProjectileID.FlamesTrap] = true;
            A_SourceNPCGlobalProjectile.SourceNPCSync[ProjectileID.FlamethrowerTrap] = true;
            A_SourceNPCGlobalProjectile.SourceNPCSync[ProjectileID.GeyserTrap] = true;
            A_SourceNPCGlobalProjectile.SourceNPCSync[ProjectileID.Fireball] = true;
            A_SourceNPCGlobalProjectile.SourceNPCSync[ProjectileID.EyeBeam] = true;
            A_SourceNPCGlobalProjectile.SourceNPCSync[ProjectileID.PhantasmalBolt] = true;
            A_SourceNPCGlobalProjectile.SourceNPCSync[ProjectileID.PhantasmalEye] = true;
            A_SourceNPCGlobalProjectile.SourceNPCSync[ProjectileID.PhantasmalSphere] = true;
            A_SourceNPCGlobalProjectile.SourceNPCSync[ProjectileID.ShadowBeamHostile] = true;
            A_SourceNPCGlobalProjectile.SourceNPCSync[ProjectileID.InfernoHostileBlast] = true;
            A_SourceNPCGlobalProjectile.SourceNPCSync[ProjectileID.InfernoHostileBolt] = true;
            A_SourceNPCGlobalProjectile.SourceNPCSync[ProjectileID.RuneBlast] = true;
            A_SourceNPCGlobalProjectile.SourceNPCSync[ProjectileID.EyeLaser] = true;
            A_SourceNPCGlobalProjectile.SourceNPCSync[ProjectileID.GoldenShowerHostile] = true;
            A_SourceNPCGlobalProjectile.SourceNPCSync[ProjectileID.CursedFlameHostile] = true;
            A_SourceNPCGlobalProjectile.SourceNPCSync[ProjectileID.Skull] = true;
            A_SourceNPCGlobalProjectile.SourceNPCSync[ProjectileID.DD2ExplosiveTrapT3Explosion] = true;
            A_SourceNPCGlobalProjectile.SourceNPCSync[ProjectileID.QueenSlimeGelAttack] = true;
            A_SourceNPCGlobalProjectile.SourceNPCSync[ProjectileID.BombSkeletronPrime] = true;
            A_SourceNPCGlobalProjectile.SourceNPCSync[ProjectileID.SandnadoHostile] = true;
            A_SourceNPCGlobalProjectile.SourceNPCSync[ProjectileID.NebulaSphere] = true;

        }

        public override void SetDefaults(Projectile projectile)
        {
            if (!FargoSoulsWorld.EternityMode)
                return;

            switch (projectile.type)
            {
                case ProjectileID.FinalFractal: //zenith
                    if (!FargoSoulsWorld.downedMutant)
                    {
                        projectile.usesLocalNPCImmunity = false;
                        projectile.localNPCHitCooldown = 0;

                        projectile.usesIDStaticNPCImmunity = true;
                        if (FargoSoulsWorld.downedAbom)
                            projectile.idStaticNPCHitCooldown = 3;
                        else
                            projectile.idStaticNPCHitCooldown = 5;

                        projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().noInteractionWithNPCImmunityFrames = true;
                    }
                    break;

                case ProjectileID.Sharknado:
                case ProjectileID.Cthulunado:
                    if (!FargoSoulsWorld.MasochistModeReal)
                    {
                        EModeCanHurt = false;
                        projectile.hide = true;
                    }
                    break;

                case ProjectileID.DD2BetsyFlameBreath:
                    projectile.tileCollide = false;
                    projectile.penetrate = -1;
                    break;

                case ProjectileID.ChlorophyteBullet:
                    projectile.extraUpdates = 1;
                    projectile.timeLeft = 150;
                    break;

                case ProjectileID.CrystalBullet:
                case ProjectileID.HolyArrow:
                case ProjectileID.HallowStar:
                    //HasKillCooldown = true;
                    break;

                case ProjectileID.SaucerLaser:
                    projectile.tileCollide = false;
                    break;

                case ProjectileID.AncientDoomProjectile:
                    projectile.scale *= 1.5f;
                    break;

                case ProjectileID.UnholyTridentHostile:
                    projectile.extraUpdates++;
                    break;

                case ProjectileID.BulletSnowman:
                    projectile.tileCollide = false;
                    projectile.timeLeft = 600;
                    break;

                case ProjectileID.CannonballHostile:
                    projectile.scale = 2f;
                    break;

                case ProjectileID.EyeLaser:
                case ProjectileID.EyeFire:
                    projectile.tileCollide = false;
                    break;

                case ProjectileID.QueenSlimeMinionBlueSpike:
                    projectile.scale *= 1.5f;
                    projectile.timeLeft = 180;
                    projectile.tileCollide = false;
                    break;

                case ProjectileID.BloodShot:
                    projectile.tileCollide = false;
                    break;

                case ProjectileID.DeerclopsRangedProjectile:
                    projectile.extraUpdates = 1;
                    break;

                case ProjectileID.FairyQueenLance: //these are here due to mp sync concerns and edge case on spawn
                case ProjectileID.HallowBossLastingRainbow:
                case ProjectileID.HallowBossRainbowStreak:
                case ProjectileID.PhantasmalSphere:
                    EModeCanHurt = false;
                    break;
                default:
                    break;
            }
        }

        private bool NonSwarmFight(Projectile projectile, params int[] types)
        {
            if (FargoSoulsWorld.SwarmActive)
                return false;

            NPC npc = projectile.GetSourceNPC();
            return projectile.GetSourceNPC() is NPC && types.Contains(npc.type);
        }

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (!FargoSoulsWorld.EternityMode)
                return;

            Projectile sourceProj = null;
            if (source is EntitySource_Parent parent && parent.Entity is Projectile)
                sourceProj = parent.Entity as Projectile;

            if (FargoSoulsUtil.IsSummonDamage(projectile, true, false))
            {
                if (projectile.minion && !(IgnoreMinionNerf.TryGetValue(projectile.type, out bool ignoreNerf1) && ignoreNerf1))
                    NerfDamageBasedOnProjTypeCount = projectile.type;
                else if (sourceProj is Projectile && !(IgnoreMinionNerf.TryGetValue(sourceProj.type, out bool ignoreNerf2) && ignoreNerf2))
                    NerfDamageBasedOnProjTypeCount = sourceProj.GetGlobalProjectile<EModeGlobalProjectile>().NerfDamageBasedOnProjTypeCount;
            }

            switch (projectile.type)
            {
                case ProjectileID.ZapinatorLaser:
                    projectile.originalDamage = projectile.damage;
                    break;

                case ProjectileID.FallingStar:
                    if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.mutantBoss, ModContent.NPCType<NPCs.MutantBoss.MutantBoss>()))
                        projectile.active = false;
                    break;

                case ProjectileID.VampireHeal:
                    //each lifesteal hits timer again when above 33% life (total, halved lifesteal rate)
                    //if (Main.player[projectile.owner].statLife > Main.player[projectile.owner].statLifeMax2 / 3)
                    //    Main.player[projectile.owner].lifeSteal -= projectile.ai[1];

                    //each lifesteal hits timer again when above 33% life (stacks with above, total 1/3rd lifesteal rate)
                    //if (Main.player[projectile.owner].statLife > Main.player[projectile.owner].statLifeMax2 * 2 / 3)
                    //    Main.player[projectile.owner].lifeSteal -= projectile.ai[1];
                    break;

                case ProjectileID.Cthulunado:
                    if (NonSwarmFight(projectile, NPCID.DukeFishron) && FargoSoulsWorld.MasochistModeReal)
                    {
                        if (projectile.ai[1] == 25 || (sourceProj is Projectile && sourceProj.GetGlobalProjectile<EModeGlobalProjectile>().altBehaviour))
                            altBehaviour = true;
                    }
                    break;

                case ProjectileID.DeerclopsIceSpike: //note to future self: these are all mp compatible apparently?
                    if (FargoSoulsWorld.SwarmActive)
                        break;

                    if (FargoSoulsWorld.MasochistModeReal)
                        projectile.ai[0] -= 20;

                    if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.deerBoss, NPCID.Deerclops))
                    {
                        if (Main.npc[EModeGlobalNPC.deerBoss].ai[0] == 4) //double walls
                        {
                            projectile.ai[0] -= 30;
                            if (Main.npc[EModeGlobalNPC.deerBoss].GetGlobalNPC<Deerclops>().EnteredPhase2)
                                projectile.ai[0] -= 30;
                            if (Main.npc[EModeGlobalNPC.deerBoss].GetGlobalNPC<Deerclops>().EnteredPhase3)
                                projectile.ai[0] -= 120;
                        }
                    }

                    if (projectile.GetSourceNPC() is NPC && projectile.GetSourceNPC().type == NPCID.Deerclops && sourceProj is not Projectile)
                    {
                        //is a final spike of the attack
                        if ((projectile.GetSourceNPC().ai[0] == 1 && projectile.GetSourceNPC().ai[1] == 52) || (projectile.GetSourceNPC().ai[0] == 4 && projectile.GetSourceNPC().ai[1] == 70 && !projectile.GetSourceNPC().GetGlobalNPC<Deerclops>().DoLaserAttack))
                        {
                            bool isSingleWaveAttack = projectile.GetSourceNPC().ai[0] == 1;

                            bool shouldSplit = true;
                            if (isSingleWaveAttack) //because deerclops spawns like 4 of them stacked on each other?
                            {
                                for (int i = 0; i < Main.maxProjectiles; i++)
                                {
                                    if (Main.projectile[i].active && Main.projectile[i].type == projectile.type
                                        && Main.projectile[i].scale == projectile.scale
                                        && Math.Sign(Main.projectile[i].velocity.X) == Math.Sign(projectile.velocity.X))
                                    {
                                        if (i != projectile.whoAmI)
                                            shouldSplit = false;
                                        break;
                                    }
                                }
                            }

                            if (shouldSplit)
                            {
                                //projectile.ai[0] -= 60;
                                //projectile.netUpdate = true;

                                float ai1 = 1.3f;
                                if (projectile.GetSourceNPC().GetGlobalNPC<Deerclops>().EnteredPhase2)
                                    ai1 = 1.35f; //triggers recursive ai
                                                 //if (projectile.GetSourceNPC().GetGlobalNPC<Deerclops>().EnteredPhase3 || FargoSoulsWorld.MasochistModeReal)
                                                 //    ai1 = 1.4f;
                                Vector2 spawnPos = projectile.Center + 200 * Vector2.Normalize(projectile.velocity);

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(projectile.GetSource_FromThis(), spawnPos, projectile.velocity, projectile.type, projectile.damage, projectile.knockBack, projectile.owner, 0f, ai1);

                                    if (isSingleWaveAttack)
                                    {
                                        Projectile.NewProjectile(projectile.GetSource_FromThis(), spawnPos, Vector2.UnitX * Math.Sign(projectile.velocity.X) * projectile.velocity.Length(), projectile.type, projectile.damage, projectile.knockBack, projectile.owner, 0f, ai1);
                                        Projectile.NewProjectile(projectile.GetSource_FromThis(), spawnPos, new Vector2(projectile.velocity.X, -projectile.velocity.Y), projectile.type, projectile.damage, projectile.knockBack, projectile.owner, 0f, ai1);
                                    }
                                    else
                                    {
                                        Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, new Vector2(-projectile.velocity.X, projectile.velocity.Y), projectile.type, projectile.damage, projectile.knockBack, projectile.owner, 0f, ai1);
                                        if (projectile.Center.Y < projectile.GetSourceNPC().Center.Y)
                                        {
                                            Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, -projectile.velocity, projectile.type, projectile.damage, projectile.knockBack, projectile.owner, 0f, ai1);
                                            Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, new Vector2(projectile.velocity.X, -projectile.velocity.Y), projectile.type, projectile.damage, projectile.knockBack, projectile.owner, 0f, ai1);
                                        }
                                        else
                                        {
                                            Projectile.NewProjectile(projectile.GetSource_FromThis(), spawnPos, new Vector2(-projectile.velocity.X, projectile.velocity.Y), projectile.type, projectile.damage, projectile.knockBack, projectile.owner, 0f, ai1);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;

                default:
                    break;
            }
        }

        //separate from OnSpawn for multiplayer sync
        public void OnFirstTick(Projectile projectile)
        {
            if (A_SourceNPCGlobalProjectile.NeedsSync(A_SourceNPCGlobalProjectile.SourceNPCSync, projectile.type))
            {
                NPC sourceNPC = projectile.GetSourceNPC();

                switch (projectile.type)
                {
                    case ProjectileID.SharpTears:
                    case ProjectileID.JestersArrow:
                    case ProjectileID.MeteorShot:
                    case ProjectileID.ShadowFlame:
                    case ProjectileID.MoonlordBullet:
                    case ProjectileID.WaterBolt:
                    case ProjectileID.WaterStream:
                    case ProjectileID.DeathSickle:
                    case ProjectileID.IceSickle:
                    case ProjectileID.SwordBeam:
                        if (sourceNPC is NPC && !sourceNPC.friendly && !sourceNPC.townNPC)
                        {
                            projectile.friendly = false;
                            projectile.hostile = true;
                            projectile.DamageType = DamageClass.Default;
                        }
                        break;

                    case ProjectileID.CultistBossFireBall: //disable proj
                        if (NonSwarmFight(projectile, NPCID.CultistBoss) && sourceNPC.GetGlobalNPC<LunaticCultist>().EnteredPhase2)
                        {
                            projectile.timeLeft = 0;
                            EModeCanHurt = false;
                        }
                        break;

                    case ProjectileID.CultistBossFireBallClone: //disable proj
                        if (NonSwarmFight(projectile, NPCID.CultistBoss))
                        {
                            projectile.timeLeft = 0;
                            EModeCanHurt = false;
                        }
                        break;

                    case ProjectileID.PhantasmalBolt:
                        if (NonSwarmFight(projectile, NPCID.MoonLordHand, NPCID.MoonLordHead, NPCID.MoonLordFreeEye))
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                for (int i = -2; i <= 2; i++)
                                {
                                    Projectile.NewProjectile(Terraria.Entity.InheritSource(projectile), projectile.Center,
                                        1.5f * Vector2.Normalize(projectile.velocity).RotatedBy(Math.PI / 2 / 2 * i),
                                        ModContent.ProjectileType<PhantasmalBolt2>(), projectile.damage, 0f, Main.myPlayer);
                                }
                            }
                            projectile.Kill();
                        }
                        break;

                    case ProjectileID.SharknadoBolt:
                        if (sourceNPC is NPC && sourceNPC.type == NPCID.DukeFishron && sourceNPC.GetGlobalNPC<DukeFishron>().IsEX)
                            projectile.extraUpdates++;
                        break;

                    case ProjectileID.HallowBossRainbowStreak:
                        if (NonSwarmFight(projectile, NPCID.HallowBoss))
                        {
                            if (FargoSoulsWorld.MasochistModeReal && sourceNPC.ai[0] != 8 && sourceNPC.ai[0] != 9)
                                EModeCanHurt = true;

                            if (sourceNPC.ai[0] == 12)
                                projectile.velocity *= 0.7f;
                        }
                        break;

                    case ProjectileID.BloodShot:
                        if (sourceNPC is NPC && sourceNPC.type == NPCID.BloodSquid)
                            projectile.damage /= 2;
                        break;

                    case ProjectileID.HallowBossLastingRainbow:
                        if (NonSwarmFight(projectile, NPCID.HallowBoss))
                        {
                            projectile.timeLeft += 60;
                            projectile.localAI[1] = projectile.velocity.ToRotation();

                            if (sourceNPC.ai[0] == 7 && sourceNPC.ai[1] >= 255 && sourceNPC.GetGlobalNPC<EmpressofLight>().DoParallelSwordWalls)
                            {
                                altBehaviour = true;
                            }
                            else if (sourceNPC.GetGlobalNPC<EmpressofLight>().AttackTimer == 1)
                            {
                                projectile.localAI[0] = 1f;
                            }
                        }
                        break;

                    case ProjectileID.FairyQueenLance:
                        if (NonSwarmFight(projectile, NPCID.HallowBoss) && sourceNPC.ai[0] == 7)
                        {
                            if (sourceNPC.ai[1] < 255) //vanilla attack has random variation, purely visual
                            {
                                Vector2 appearVel = Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2();
                                appearVel *= 2f;
                                projectile.position -= appearVel * 60f;
                                projectile.velocity = appearVel;
                            }
                            else if (sourceNPC.GetGlobalNPC<EmpressofLight>().DoParallelSwordWalls)
                            {
                                altBehaviour = true;
                            }
                        }
                        break;

                    default:
                        break;
                }
            }
        }

        public override bool? CanDamage(Projectile projectile)
        {
            if (!FargoSoulsWorld.EternityMode)
                return base.CanDamage(projectile);

            if (!EModeCanHurt)
                return false;

            return base.CanDamage(projectile);
        }
        public int SwingDirection = 1;
        public override bool PreAI(Projectile projectile)
        {
            if (!FargoSoulsWorld.EternityMode)
            {
                preAICheckDone = true;
                return base.PreAI(projectile);
            }

            if (!preAICheckDone)
            {
                preAICheckDone = true;

                OnFirstTick(projectile);
            }

            counter++;

            //delay the very bottom piece of sharknados spawning in, also delays spawning sharkrons
            if (counter < 30 && projectile.ai[0] == 15 && !FargoSoulsWorld.MasochistModeReal
                && (projectile.type == ProjectileID.Sharknado || projectile.type == ProjectileID.Cthulunado)
                && projectile.ai[1] == (projectile.type == ProjectileID.Sharknado ? 15 : 24))
            {
                projectile.timeLeft++;
                return false;
            }
            switch (projectile.type)
            {
                case var _ when ReworkedSpears.Contains(projectile.type):
                    #region Special Spear Swing Style
                    Texture2D tex = (Texture2D)TextureAssets.Projectile[projectile.type];
                    float HoldoutRangeMax = (float)tex.Size().Length() * projectile.scale; //since sprite is diagonal
                    float HoldoutRangeMin = (float)projectile.Size.Length(); //(float)-tex.Size().Length() / 4 * projectile.scale; 
                    Player player = Main.player[projectile.owner];




                    int duration = (int)(player.itemAnimationMax / 1.5f);
                    int WaitTime = player.itemAnimationMax / 5;
                    player.heldProj = projectile.whoAmI;
                    projectile.spriteDirection = player.direction;
                    if (projectile.ai[1] == 0)
                        SwingDirection = Main.rand.NextBool(2) ? 1 : -1;
                    float Swing = 13; //higher value = less swing
                    projectile.usesLocalNPCImmunity = true;
                    projectile.localNPCHitCooldown = player.itemAnimationMax; //only hit once per swing
                                                                              //projectile.ai[1] is time from spawn
                                                                              //projectile.ai[0] is extension, between 0 and 1
                    if (projectile.timeLeft > player.itemAnimationMax)
                    {
                        projectile.timeLeft = player.itemAnimationMax;
                    }
                    if (projectile.ai[1] <= duration / 2)
                    {
                        projectile.ai[0] = projectile.ai[1] / (duration / 2);
                        projectile.velocity = projectile.velocity.RotatedBy(SwingDirection * projectile.spriteDirection * -Math.PI / (Swing * player.itemAnimationMax));
                    }
                    else if (projectile.ai[1] <= duration / 2 + WaitTime)
                    {
                        projectile.ai[0] = 1;
                        projectile.velocity = projectile.velocity.RotatedBy(SwingDirection * projectile.spriteDirection * (1.5 * duration / WaitTime) * Math.PI / (Swing * player.itemAnimationMax)); //i know how wacky this looks
                    }
                    else
                    {
                        //projectile.friendly = false; //no hit on backswing
                        projectile.ai[0] = (duration + WaitTime - projectile.ai[1]) / (duration / 2);
                        projectile.velocity = projectile.velocity.RotatedBy(SwingDirection * projectile.spriteDirection * -Math.PI / (Swing * player.itemAnimationMax));
                    }
                    //if (projectile.ai[1] == duration / 2)
                        //SoundEngine.PlaySound(SoundID.Item1, player.Center);

                    projectile.ai[1]++;
                    projectile.velocity = Vector2.Normalize(projectile.velocity); //store direction
                    projectile.Center = player.MountedCenter + Vector2.SmoothStep(projectile.velocity * HoldoutRangeMin, projectile.velocity * HoldoutRangeMax, projectile.ai[0]);

                    projectile.rotation = projectile.velocity.ToRotation();
                    if (projectile.spriteDirection == -1)
                    {
                        projectile.rotation += MathHelper.ToRadians(45f);
                    }
                    else
                    {
                        projectile.rotation += MathHelper.ToRadians(135f);
                    }
                    #endregion
                    return false;
                default:
                    break;
                    
            }

            return base.PreAI(projectile);
        }

        public override void AI(Projectile projectile)
        {
            if (!FargoSoulsWorld.EternityMode)
                return;

            NPC sourceNPC = projectile.GetSourceNPC();

            switch (projectile.type)
            {
                case ProjectileID.ZapinatorLaser:
                    if (projectile.damage > projectile.originalDamage)
                        projectile.damage = projectile.originalDamage;
                    break;

                case ProjectileID.InsanityShadowHostile:
                    if (FargoSoulsWorld.SwarmActive)
                        break;

                    if (Main.player[projectile.owner].ownedProjectileCounts[projectile.type] >= 4)
                    {
                        projectile.extraUpdates = 1;
                        projectile.position += projectile.velocity * 0.5f;
                        EModeCanHurt = true;
                        counter = -600;
                    }
                    else if (!FargoSoulsWorld.MasochistModeReal)
                    {
                        EModeCanHurt = false;
                        projectile.position -= projectile.velocity;
                        projectile.ai[0]--;
                        projectile.alpha = 255;

                        if (counter > 30 && Main.player[projectile.owner].ownedProjectileCounts[projectile.type] <= 1)
                            projectile.timeLeft = 0;
                    }
                    break;

                case ProjectileID.DeerclopsIceSpike:
                    if (!FargoSoulsWorld.SwarmActive && counter == 2f && projectile.hostile && projectile.ai[1] > 1.3f) //only larger spikes
                    {
                        float ai1 = 1.3f;
                        if (projectile.ai[1] > 1.35f)
                            ai1 = 1.35f;

                        for (int i = -1; i <= 1; i++) //recursive fractal spread
                        {
                            Vector2 baseVel = Vector2.Lerp(projectile.velocity, Vector2.UnitX * projectile.velocity.Length() * Math.Sign(projectile.velocity.X), 0.75f);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(Terraria.Entity.InheritSource(projectile), projectile.Center + 200f * Vector2.Normalize(projectile.velocity), baseVel.RotatedBy(MathHelper.ToRadians(30) * i), projectile.type, projectile.damage, projectile.knockBack, projectile.owner, 0f, ai1);
                            }
                        }
                    }
                    break;

                case ProjectileID.BloodShot:
                case ProjectileID.BloodNautilusTears:
                case ProjectileID.BloodNautilusShot:
                    if (!Collision.SolidTiles(projectile.Center, 0, 0))
                    {
                        Lighting.AddLight(projectile.Center, TorchID.Crimson);

                        if (counter > 180)
                            projectile.tileCollide = true;
                    }
                    break;

                case ProjectileID.HallowBossLastingRainbow:
                    if (!NonSwarmFight(projectile, NPCID.HallowBoss))
                    {
                        EModeCanHurt = true;
                        altBehaviour = false;
                        break;
                    }

                    if (!FargoSoulsWorld.SwarmActive)
                    {
                        if (Math.Abs(MathHelper.WrapAngle(projectile.velocity.ToRotation() - projectile.localAI[1])) > MathHelper.Pi * 0.9f)
                            EModeCanHurt = true;

                        projectile.extraUpdates = EModeCanHurt ? 1 : 3;

                        if (projectile.localAI[0] == 1f)
                            projectile.velocity = projectile.velocity.RotatedBy(-projectile.ai[0] * 2f);
                    }

                    if (altBehaviour)
                    {
                        if (EModeCanHurt)
                            projectile.velocity = projectile.velocity.RotatedBy(-projectile.ai[0] * 0.5f);
                    }
                    break;

                case ProjectileID.HallowBossRainbowStreak:
                    if (!EModeCanHurt)
                        EModeCanHurt = FargoSoulsWorld.SwarmActive || projectile.timeLeft < 100;
                    break;

                case ProjectileID.FairyQueenLance:
                    EModeCanHurt = projectile.localAI[0] > 60;
                    if (altBehaviour)
                    {
                        const float slowdown = 0.33f;

                        if (!EModeCanHurt)
                        {
                            counter = 0;
                            projectile.timeLeft++;
                            projectile.localAI[0] -= slowdown;
                        }

                        projectile.position -= projectile.velocity * slowdown * Utils.Clamp((float)Math.Sqrt(1f - counter / 60f), 0f, 1f);
                    }
                    else if (NonSwarmFight(projectile, NPCID.HallowBoss) && sourceNPC.ai[0] == 6 && sourceNPC.ai[1] > 60)
                    {
                        projectile.position += sourceNPC.position - sourceNPC.oldPosition;
                    }
                    break;

                case ProjectileID.FairyQueenSunDance:
                    if (!FargoSoulsWorld.SwarmActive)
                    {
                        NPC npc = FargoSoulsUtil.NPCExists(projectile.ai[1], NPCID.HallowBoss);
                        if (npc != null)
                        {
                            if (npc.ai[0] == 8 || npc.ai[0] == 9) //doing dash
                            {
                                projectile.rotation = projectile.ai[0]; //negate rotation

                                if (counter < 60) //force proj into active state faster
                                    counter += 9;
                                if (projectile.localAI[0] < 60)
                                    projectile.localAI[0] += 9;
                            }

                            if (npc.ai[0] == 1 || npc.ai[0] == 10) //while empress is moving back over player or p2 transition
                            {
                                EModeCanHurt = false;
                                counter = 0;
                                projectile.timeLeft = 0;
                            }

                            if (npc.ai[0] == 6 && npc.GetGlobalNPC<EmpressofLight>().AttackCounter % 2 == 0)
                            {
                                projectile.scale *= Utils.Clamp(npc.ai[1] / 80f, 0f, 2.5f);
                            }
                            else if (counter >= 60 && projectile.scale > 0.5f && counter % 10 == 0)
                            {
                                float offset = MathHelper.ToRadians(90) * MathHelper.Lerp(0f, 1f, (counter % 50f) / 50f);
                                for (int i = -1; i <= 1; i += 2)
                                {
                                    if (Math.Abs(offset) < 0.001f && i < 0)
                                        continue;

                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        float spawnOffset = 800;
                                        Projectile.NewProjectile(Terraria.Entity.InheritSource(projectile), projectile.Center + projectile.rotation.ToRotationVector2() * spawnOffset, Vector2.Zero, ProjectileID.FairyQueenLance, projectile.damage, projectile.knockBack, projectile.owner, projectile.rotation + offset * i, projectile.ai[0]);
                                    }
                                }
                            }
                        }
                    }
                    break;

                case ProjectileID.QueenBeeStinger:
                    projectile.velocity.Y -= 0.1f; //negate gravity
                    break;

                case ProjectileID.BeeHive:
                    if (projectile.timeLeft > 30 && (projectile.velocity.X != 0 || projectile.velocity.Y == 0))
                        projectile.timeLeft = 30;
                    break;

                case ProjectileID.TowerDamageBolt:
                    if (!firstTickAICheckDone)
                    {
                        NPC npc = FargoSoulsUtil.NPCExists(projectile.ai[0], NPCID.LunarTowerNebula, NPCID.LunarTowerSolar, NPCID.LunarTowerStardust, NPCID.LunarTowerVortex);
                        if (npc != null)
                        {
                            //not kill, because kill decrements shield
                            if (projectile.Distance(npc.Center) > 4000)
                                projectile.active = false;
                            int p = Player.FindClosest(projectile.Center, 0, 0);
                            if (p != -1 && !(Main.player[p].active && npc.Distance(Main.player[p].Center) < 4000))
                                projectile.active = false;
                        }
                    }
                    break;

                case ProjectileID.Sharknado: //this only runs after changes in preAI() finish blocking it
                case ProjectileID.Cthulunado:
                    EModeCanHurt = true;
                    projectile.hide = false;
                    if (!FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.fishBoss, NPCID.DukeFishron))
                        projectile.timeLeft = Math.Min(120, projectile.timeLeft);
                    break;

                case ProjectileID.WireKite:
                    if (projectile.owner == Main.myPlayer && Main.player[projectile.owner].GetModPlayer<FargoSoulsPlayer>().LihzahrdCurse)
                    {
                        projectile.Kill();
                    }
                    break;

                case ProjectileID.Fireball:
                    {
                        NPC golem = FargoSoulsUtil.NPCExists(NPC.golemBoss, NPCID.Golem);
                        if (golem != null && !golem.dontTakeDamage)
                            projectile.timeLeft = 0;
                    }
                    break;

                case ProjectileID.GeyserTrap:
                    if (!FargoSoulsWorld.MasochistModeReal && sourceNPC is NPC && sourceNPC.type == NPCID.Golem && counter > 45)
                        projectile.Kill();
                    break;

                case ProjectileID.CultistBossFireBall:
                    if (!FargoSoulsWorld.MasochistModeReal && NonSwarmFight(projectile, NPCID.CultistBoss))
                        projectile.position -= projectile.velocity * Math.Max(0, 1f - counter / 45f / projectile.MaxUpdates); //accel startup
                    break;

                case ProjectileID.NebulaSphere:
                    if (sourceNPC is NPC && sourceNPC.type == NPCID.CultistBoss)
                    {
                        int p = Player.FindClosest(projectile.Center, 0, 0);
                        if (p != -1 && projectile.Distance(Main.player[p].Center) > 240)
                            projectile.position += projectile.velocity;
                    }
                    break;

                case ProjectileID.EyeBeam:
                    if (!FargoSoulsWorld.MasochistModeReal && NonSwarmFight(projectile, NPCID.GolemHead, NPCID.GolemHeadFree))
                    {
                        if (!firstTickAICheckDone)
                        {
                            projectile.velocity = projectile.velocity.SafeNormalize(Vector2.UnitY);
                            projectile.timeLeft = 180 * projectile.MaxUpdates;
                        }

                        if (projectile.timeLeft % projectile.MaxUpdates == 0) //only run once per tick
                        {
                            if (++projectile.localAI[1] < 90)
                                projectile.velocity *= 1.04f;
                        }
                    }
                    break;

                case ProjectileID.JavelinHostile:
                case ProjectileID.FlamingWood:
                    projectile.position += projectile.velocity * .5f;
                    break;

                case ProjectileID.VortexAcid:
                    projectile.position += projectile.velocity * .25f;
                    break;

                case ProjectileID.NebulaEye:
                    {
                        NPC npc = FargoSoulsUtil.NPCExists(projectile.ai[1], NPCID.NebulaBrain);
                        if (npc != null)
                        {
                            if (npc.ai[0] < 45 && projectile.ai[0] >= 180 - 5)
                                projectile.ai[0] -= 180; //prevent firing shortly after teleport
                        }
                    }
                    break;

                case ProjectileID.CultistRitual:
                    if (!FargoSoulsWorld.SwarmActive)
                    {
                        NPC npc = FargoSoulsUtil.NPCExists(projectile.ai[1], NPCID.CultistBoss);
                        if (npc != null && npc.ai[3] == -1f && npc.ai[0] == 5)
                        {
                            projectile.Center = Main.player[npc.target].Center;
                        }

                        if (!firstTickAICheckDone) //MP sync data to server
                        {
                            if (npc != null)
                            {
                                if (Main.netMode == NetmodeID.MultiplayerClient)
                                {
                                    LunaticCultist cultistData = npc.GetGlobalNPC<LunaticCultist>();

                                    var netMessage = Mod.GetPacket(); //sync damage contribution (which is client side) to server
                                    netMessage.Write((byte)FargowiltasSouls.PacketID.SyncCultistDamageCounterToServer);
                                    netMessage.Write((byte)npc.whoAmI);
                                    netMessage.Write(cultistData.MeleeDamageCounter);
                                    netMessage.Write(cultistData.RangedDamageCounter);
                                    netMessage.Write(cultistData.MagicDamageCounter);
                                    netMessage.Write(cultistData.MinionDamageCounter);
                                    netMessage.Send();
                                }
                                else //refresh ritual
                                {
                                    for (int i = 0; i < Main.maxProjectiles; i++)
                                    {
                                        if (Main.projectile[i].active && Main.projectile[i].ai[1] == npc.whoAmI && Main.projectile[i].type == ModContent.ProjectileType<CultistRitual>())
                                        {
                                            Main.projectile[i].Kill();
                                            break;
                                        }
                                    }

                                    Projectile.NewProjectile(npc.GetSource_FromThis(), projectile.Center, Vector2.Zero, ModContent.ProjectileType<CultistRitual>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer, 0f, npc.whoAmI);
                                    const int max = 16;
                                    const float appearRadius = 1600f - 100f;
                                    for (int i = 0; i < max; i++)
                                    {
                                        float rotation = MathHelper.TwoPi / max * i;
                                        for (int j = -1; j <= 1; j += 2)
                                        {
                                            Vector2 spawnoffset = new Vector2(appearRadius, 0).RotatedBy(rotation);
                                            Projectile.NewProjectile(npc.GetSource_FromThis(), projectile.Center + spawnoffset, j * Vector2.UnitY.RotatedBy(rotation), ModContent.ProjectileType<GlowLine>(), 0, 0f, Main.myPlayer, 18, npc.whoAmI);
                                        }
                                    }
                                }
                            }

                            for (int i = 0; i < Main.maxProjectiles; i++) //purge spectre mask bolts and homing nebula spheres
                            {
                                if (Main.projectile[i].active && (Main.projectile[i].type == ProjectileID.SpectreWrath || Main.projectile[i].type == ProjectileID.NebulaSphere))
                                    Main.projectile[i].Kill();
                            }
                        }

                        //if (Fargowiltas.Instance.MasomodeEXLoaded && projectile.ai[0] > 120f && projectile.ai[0] < 299f) projectile.ai[0] = 299f;

                        bool dunk = false;

                        if (projectile.ai[1] == -1)
                        {
                            if (counter == 5)
                                dunk = true;
                        }
                        else
                        {
                            counter = 0;
                            if (projectile.ai[0] == 299f)
                                dunk = true;
                        }

                        if (dunk) //pillar dunk
                        {
                            int cult = -1;
                            for (int i = 0; i < Main.maxNPCs; i++)
                            {
                                if (Main.npc[i].active && Main.npc[i].type == NPCID.CultistBoss && Main.npc[i].ai[2] == projectile.whoAmI)
                                {
                                    cult = i;
                                    break;
                                }
                            }

                            if (cult != -1)
                            {
                                float ai0 = Main.rand.Next(4);

                                LunaticCultist cultistData = Main.npc[cult].GetGlobalNPC<LunaticCultist>();
                                int[] weight = new int[4];
                                weight[0] = cultistData.MagicDamageCounter;
                                weight[1] = cultistData.MeleeDamageCounter;
                                weight[2] = cultistData.RangedDamageCounter;
                                weight[3] = cultistData.MinionDamageCounter;

                                cultistData.MeleeDamageCounter = 0;
                                cultistData.RangedDamageCounter = 0;
                                cultistData.MagicDamageCounter = 0;
                                cultistData.MinionDamageCounter = 0;

                                Main.npc[cult].netUpdate = true;

                                int max = 0;
                                for (int i = 1; i < 4; i++)
                                    if (weight[max] < weight[i])
                                        max = i;
                                if (weight[max] > 0)
                                    ai0 = max;

                                if ((cultistData.EnteredPhase2 /*|| Fargowiltas.Instance.MasomodeEXLoaded*/ || FargoSoulsWorld.MasochistModeReal) && Main.netMode != NetmodeID.MultiplayerClient && !Main.projectile.Any(p => p.active && p.hostile && p.type == ModContent.ProjectileType<CelestialPillar>()))
                                {
                                    Projectile.NewProjectile(Main.npc[cult].GetSource_FromThis(), projectile.Center, Vector2.UnitY * -10f, ModContent.ProjectileType<CelestialPillar>(),
                                        Math.Max(75, FargoSoulsUtil.ScaledProjectileDamage(Main.npc[cult].damage, 4)), 0f, Main.myPlayer, ai0);
                                }
                            }
                        }
                    }
                    break;

                case ProjectileID.MoonLeech:
                    if (projectile.ai[0] > 0f && !FargoSoulsWorld.SwarmActive)
                    {
                        Vector2 distance = Main.player[(int)projectile.ai[1]].Center - projectile.Center - projectile.velocity;
                        if (distance != Vector2.Zero)
                            projectile.position += Vector2.Normalize(distance) * Math.Min(16f, distance.Length());
                    }
                    break;

                case ProjectileID.SandnadoHostile:
                    if (Main.hardMode && projectile.timeLeft == 1199 && NPC.CountNPCS(NPCID.SandShark) < 10 && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (!(sourceNPC is NPC && (sourceNPC.type == ModContent.NPCType<NPCs.DeviBoss.DeviBoss>() || sourceNPC.type == ModContent.NPCType<SpiritChampion>())))
                        {
                            FargoSoulsUtil.NewNPCEasy(Terraria.Entity.InheritSource(projectile), projectile.Center, NPCID.SandShark,
                                velocity: new Vector2(Main.rand.NextFloat(-10, 10), Main.rand.NextFloat(-20, -10)));
                        }
                    }

                    if (sourceNPC is NPC && sourceNPC.type == ModContent.NPCType<NPCs.DeviBoss.DeviBoss>() && sourceNPC.ai[0] != 5)
                        projectile.ai[0] += 2; //despawn faster
                    break;

                case ProjectileID.PhantasmalEye:
                    if (NonSwarmFight(projectile, NPCID.MoonLordHand, NPCID.MoonLordHead, NPCID.MoonLordFreeEye))
                    {
                        if (projectile.ai[0] == 2 && counter > 60) //diving down and homing
                            projectile.velocity.Y = 9;
                        else
                            projectile.position.Y -= projectile.velocity.Y / 4;

                        float cap = FargoSoulsWorld.MasochistModeReal ? 2 : 1;

                        if (projectile.velocity.X > cap)
                            projectile.velocity.X = cap;
                        else if (projectile.velocity.X < -cap)
                            projectile.velocity.X = -cap;
                    }
                    break;

                case ProjectileID.PhantasmalSphere:
                    if (!FargoSoulsWorld.SwarmActive)
                    {
                        EModeCanHurt = projectile.alpha == 0;

                        //when from hand, nerf with telegraph and accel startup
                        if (sourceNPC is NPC && sourceNPC.type == NPCID.MoonLordHand)
                        {
                            if (projectile.ai[0] == -1) //sent to fly
                            {
                                if (++projectile.localAI[1] < 150)
                                    projectile.velocity *= 1.018f;

                                if (projectile.localAI[0] == 0 && projectile.velocity.Length() > 11) //only do this once
                                {
                                    projectile.localAI[0] = 1;
                                    projectile.velocity.Normalize();

                                    if (Main.netMode != NetmodeID.MultiplayerClient && !FargoSoulsWorld.MasochistModeReal)
                                    {
                                        Projectile.NewProjectile(Terraria.Entity.InheritSource(projectile), projectile.Center, projectile.velocity, ModContent.ProjectileType<PhantasmalSphereDeathray>(),
                                            0, 0f, Main.myPlayer, 0f, projectile.identity);
                                    }

                                    projectile.netUpdate = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        EModeCanHurt = true;
                    }
                    break;

                case ProjectileID.PhantasmalDeathray:
                    if (FargoSoulsWorld.MasochistModeReal)
                    {
                        projectile.velocity = projectile.velocity.RotatedBy(projectile.ai[0] * 0.5f);
                        projectile.rotation = projectile.velocity.ToRotation() - MathHelper.PiOver2;

                        projectile.scale *= sourceNPC is NPC && sourceNPC.type == NPCID.MoonLordHead
                            ? Main.rand.NextFloat(6f, 9f)
                            : Main.rand.NextFloat(4f, 6f);

                        if (!Main.dedServ && Main.LocalPlayer.active)
                            Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().Screenshake = 2;
                    }
                    break;

                case ProjectileID.BombSkeletronPrime: //needs to be set every tick
                    if (sourceNPC is NPC && sourceNPC.type == NPCID.UndeadMiner)
                        projectile.damage = sourceNPC.damage / 2;
                    if (!FargoSoulsWorld.SwarmActive)
                        projectile.damage = 40;
                    break;

                case ProjectileID.DD2BetsyFireball: //when spawned, also spawn a phoenix
                    if (!firstTickAICheckDone && NonSwarmFight(projectile, NPCID.DD2Betsy))
                    {
                        bool phase2 = sourceNPC.GetGlobalNPC<Betsy>().InPhase2;
                        int max = phase2 ? 2 : 1;
                        for (int i = 0; i < max; i++)
                        {
                            Vector2 speed = Main.rand.NextFloat(8, 12) * -Vector2.UnitY.RotatedByRandom(Math.PI / 2);
                            float ai1 = phase2 ? 60 + Main.rand.Next(60) : 90 + Main.rand.Next(30);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(Terraria.Entity.InheritSource(projectile), projectile.Center, speed, ModContent.ProjectileType<BetsyPhoenix>(),
                                    projectile.damage, 0f, Main.myPlayer, Player.FindClosest(projectile.Center, 0, 0), ai1);
                            }
                        }
                    }
                    break;

                case ProjectileID.DD2BetsyFlameBreath:
                    if (NonSwarmFight(projectile, NPCID.DD2Betsy))
                    {
                        bool phase2 = sourceNPC.GetGlobalNPC<Betsy>().InPhase2;

                        //add chain blasts in maso p2
                        if (phase2 && !firstTickAICheckDone && FargoSoulsWorld.MasochistModeReal && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(
                                Terraria.Entity.InheritSource(projectile),
                                projectile.Center + 100f * Vector2.Normalize(sourceNPC.velocity),
                                Vector2.Zero,
                                ModContent.ProjectileType<EarthChainBlast>(),
                                projectile.damage,
                                0f,
                                Main.myPlayer,
                                sourceNPC.velocity.ToRotation(),
                                7);
                        }

                        //add fireballs
                        if (counter > (phase2 ? 2 : 4))
                        {
                            counter = 0;

                            SoundEngine.PlaySound(SoundID.Item34, projectile.Center);

                            Vector2 projVel = projectile.velocity.RotatedBy((Main.rand.NextDouble() - 0.5) * Math.PI / 10);
                            projVel.Normalize();
                            projVel *= Main.rand.NextFloat(8f, 12f);

                            int type = ProjectileID.CultistBossFireBall;
                            if (!phase2 || Main.rand.NextBool())
                            {
                                type = ModContent.ProjectileType<WillFireball>();
                                projVel *= 2f;
                                if (phase2)
                                    projVel *= 1.5f;
                            }

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(Terraria.Entity.InheritSource(projectile), projectile.Center, projVel, type, projectile.damage, 0f, Main.myPlayer);
                        }
                    }
                    break;

                case ProjectileID.QueenSlimeGelAttack:
                    if (FargoSoulsWorld.SwarmActive)
                        break;

                    if (!FargoSoulsWorld.MasochistModeReal)
                    {
                        float ratio = Math.Max(0, 1f - counter / 60f / projectile.MaxUpdates);
                        projectile.position -= projectile.velocity * ratio; //accel startup
                        projectile.velocity.Y -= 0.15f * ratio; //compensate the gravity
                    }

                    if (!firstTickAICheckDone)
                    {
                        //if (projectile.velocity.Y > 0)
                        //    projectile.velocity.Y *= -.5f; //shoot up instead

                        //p1 always shoots up
                        if (sourceNPC is NPC && sourceNPC.type == NPCID.QueenSlimeBoss && sourceNPC.life > sourceNPC.lifeMax / 2)
                            projectile.velocity.Y -= 6f;
                    }

                    //when begins falling, spray out
                    if (projectile.velocity.Y > 0 && projectile.localAI[0] == 0)
                    {
                        projectile.localAI[0] = 1;

                        for (int j = -1; j <= 1; j += 2)
                        {
                            if (Math.Sign(projectile.velocity.X) == -j) //very specific phrasing so 0 horiz sprays both ways
                                continue;

                            Vector2 baseVel = Vector2.UnitX.RotatedBy(MathHelper.ToRadians(10 * j));
                            const int max = 12;
                            for (int i = 0; i < max; i++)
                            {
                                Vector2 vel = Main.rand.NextFloat(6f, 18f) * j * baseVel.RotatedBy(MathHelper.PiOver4 / max * (i + 0.5f) * -j);
                                vel *= FargoSoulsWorld.MasochistModeReal ? 2f : 1.5f;
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(Terraria.Entity.InheritSource(projectile), projectile.Center, vel, ProjectileID.QueenSlimeMinionBlueSpike, projectile.damage, 0f, Main.myPlayer);
                            }
                        }
                    }
                    break;

                case ProjectileID.QueenSlimeMinionPinkBall:
                    if (!FargoSoulsWorld.MasochistModeReal && !FargoSoulsWorld.SwarmActive)
                    {
                        float ratio = Math.Max(0, 1f - counter / 60f / projectile.MaxUpdates);
                        projectile.position -= projectile.velocity * ratio; //accel startup
                        projectile.velocity.Y -= 0.15f * ratio; //compensate the gravity
                    }
                    break;

                default:
                    break;
            }

            firstTickAICheckDone = true;
        }

        public override void ModifyHitNPC(Projectile projectile, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (!FargoSoulsWorld.EternityMode)
                return;

            //if (projectile.arrow) //change archery and quiver to additive damage
            //{
            //    if (Main.player[projectile.owner].archery)
            //    {
            //        damage = (int)(damage / 1.2);
            //        damage = (int)((double)damage * (1.0 + 0.2 / Main.player[projectile.owner].GetDamage(DamageClass.Ranged)));
            //    }

            //    if (Main.player[projectile.owner].magicQuiver)
            //    {
            //        damage = (int)(damage / 1.1);
            //        damage = (int)((double)damage * (1.0 + 0.1 / Main.player[projectile.owner].GetDamage(DamageClass.Ranged)));
            //    }
            //}

            /*if (NerfDamageBasedOnProjTypeCount != 0 && Main.player[projectile.owner].ownedProjectileCounts[NerfDamageBasedOnProjTypeCount] > 0)
            {
                int projTypeToCheck = NerfDamageBasedOnProjTypeCount;

                //note: projs needed to reach max nerf is the sum of these values
                const int allowedBeforeNerfBegins = 3;
                const int maxRampup = 9;

                float modifier = Utils.Clamp((float)(Main.player[projectile.owner].ownedProjectileCounts[projTypeToCheck] - allowedBeforeNerfBegins) / maxRampup, 0f, 1f);

                const double maxNerfStrength = 1.0 / 3.0;
                damage = (int)(damage * (1.0 - modifier * maxNerfStrength));
            }*/

            //if (projectile.type == ProjectileID.ChlorophyteBullet)
            //{
            //    damage = (int)(damage * 0.75);
            //}
        }

        public override bool OnTileCollide(Projectile projectile, Vector2 oldVelocity)
        {
            if (!FargoSoulsWorld.EternityMode)
                return base.OnTileCollide(projectile, oldVelocity);

            switch (projectile.type)
            {
                case ProjectileID.SnowBallHostile:
                    projectile.active = false; //no block
                    break;

                case ProjectileID.SandBallFalling:
                    if (projectile.ai[0] == 2f) //antlion sand
                    {
                        int num129 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 32, 0f, projectile.velocity.Y / 2f, 0, default(Color), 1f);
                        Dust expr_59B0_cp_0 = Main.dust[num129];
                        expr_59B0_cp_0.velocity.X = expr_59B0_cp_0.velocity.X * 0.4f;
                        projectile.active = false;

                    }
                    break;

                case ProjectileID.QueenSlimeMinionPinkBall:
                case ProjectileID.QueenSlimeGelAttack:
                    if (FargoSoulsWorld.MasochistModeReal)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            for (int i = 0; i < 8; i++)
                                Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, Vector2.UnitY.RotatedBy(2 * Math.PI / 8 * i) * 4f, ProjectileID.HallowSpray, 0, 0f, Main.myPlayer, 8f);
                        }
                    }
                    else
                    {
                        //if (projectile.localAI[1] == 1)
                        projectile.timeLeft = 0;
                        //projectile.localAI[1] = 1;
                    }
                    break;

                case ProjectileID.ThornBall:
                    projectile.timeLeft = 0;

                    NPC plantera = FargoSoulsUtil.NPCExists(NPC.plantBoss, NPCID.Plantera);
                    if (plantera != null && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 vel = 200f / 25f * projectile.DirectionTo(plantera.Center);
                        Projectile.NewProjectile(plantera.GetSource_FromThis(), projectile.Center - projectile.oldVelocity, vel, ModContent.ProjectileType<DicerPlantera>(), projectile.damage, projectile.knockBack, projectile.owner, 0, 0);
                    }
                    break;

                default:
                    break;
            }

            return base.OnTileCollide(projectile, oldVelocity);
        }

        public override void ModifyHitPlayer(Projectile projectile, Player target, ref int damage, ref bool crit)
        {
            if (!FargoSoulsWorld.EternityMode)
                return;

            if (NPC.downedGolemBoss && projectile.type == ProjectileID.VortexLightning)
                damage *= 2;
        }

        public override void OnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            base.OnHitNPC(projectile, target, damage, knockback, crit);

            if (!FargoSoulsWorld.EternityMode)
                return;
            Player player = Main.player[projectile.owner];
            switch (projectile.type)
            {
                case ProjectileID.PalladiumPike:
                    if (target.type != NPCID.TargetDummy && !target.friendly) //may add more checks here idk
                        player.AddBuff(BuffID.RapidHealing, 60*5);
                    break;
                case ProjectileID.CobaltNaginata:
                    Projectile p = FargoSoulsUtil.NewProjectileDirectSafe(player.GetSource_OnHit(target), target.Center, Vector2.Zero, ModContent.ProjectileType<CobaltExplosion>(), (int)(damage / 2), 0f, Main.myPlayer);
                    if (p != null)
                        p.GetGlobalProjectile<FargoSoulsGlobalProjectile>().CanSplit = false;
                    break;
                default:
                    break;
            }
        }

        public override void OnHitPlayer(Projectile projectile, Player target, int damage, bool crit)
        {
            if (!FargoSoulsWorld.EternityMode)
                return;

            NPC sourceNPC = projectile.GetSourceNPC();

            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.moonBoss, NPCID.MoonLordCore)
                && sourceNPC is NPC
                && sourceNPC.TryGetGlobalNPC(out MoonLordBodyPart _))
            {
                target.AddBuff(ModContent.BuffType<CurseoftheMoon>(), 180);
            }

            //if (sourceNPC is NPC && sourceNPC.ModNPC is NPCs.MutantBoss.MutantBoss)
            //    target.AddBuff(ModContent.BuffType<MutantFang>(), 180);

            switch (projectile.type)
            {
                case ProjectileID.DD2ExplosiveTrapT3Explosion:
                    if (sourceNPC is NPC && sourceNPC.type == ModContent.NPCType<TimberChampion>())
                        target.AddBuff(ModContent.BuffType<Defenseless>(), 300);
                    break;

                case ProjectileID.InsanityShadowHostile:
                case ProjectileID.DeerclopsIceSpike:
                case ProjectileID.DeerclopsRangedProjectile:
                    target.AddBuff(BuffID.Frostburn, 90);
                    target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 900);
                    target.AddBuff(ModContent.BuffType<Hypothermia>(), 1200);
                    break;

                case ProjectileID.BloodShot:
                case ProjectileID.BloodNautilusTears:
                case ProjectileID.BloodNautilusShot:
                case ProjectileID.SharpTears:
                    target.AddBuff(ModContent.BuffType<Anticoagulation>(), 600);
                    break;

                case ProjectileID.FairyQueenLance:
                    if (FargoSoulsWorld.EternityMode && sourceNPC is NPC && sourceNPC.type == ModContent.NPCType<NPCs.MutantBoss.MutantBoss>())
                    {
                        target.GetModPlayer<FargoSoulsPlayer>().MaxLifeReduction += 100;
                        target.AddBuff(ModContent.BuffType<OceanicMaul>(), 5400);
                        target.AddBuff(ModContent.BuffType<MutantFang>(), 180);
                    }
                    goto case ProjectileID.FairyQueenSunDance;

                case ProjectileID.FairyQueenHymn:
                case ProjectileID.FairyQueenSunDance:
                case ProjectileID.HallowBossRainbowStreak:
                case ProjectileID.HallowBossLastingRainbow:
                case ProjectileID.HallowBossSplitShotCore:
                    target.AddBuff(ModContent.BuffType<Purified>(), 300);
                    target.AddBuff(ModContent.BuffType<Smite>(), 1800);
                    break;

                case ProjectileID.RollingCactus:
                case ProjectileID.RollingCactusSpike:
                    target.AddBuff(BuffID.Poisoned, 120);
                    break;

                case ProjectileID.TorchGod:
                    target.AddBuff(BuffID.OnFire, 60);
                    break;

                case ProjectileID.Boulder:
                    target.AddBuff(ModContent.BuffType<Defenseless>(), 600);
                    break;

                case ProjectileID.PoisonDartTrap:
                case ProjectileID.SpearTrap:
                case ProjectileID.SpikyBallTrap:
                    target.AddBuff(ModContent.BuffType<IvyVenom>(), 360);
                    break;

                case ProjectileID.JavelinHostile:
                    target.AddBuff(ModContent.BuffType<Defenseless>(), 600);
                    target.GetModPlayer<FargoSoulsPlayer>().AddBuffNoStack(ModContent.BuffType<Stunned>(), 60);
                    break;

                case ProjectileID.DemonSickle:
                    target.AddBuff(ModContent.BuffType<Shadowflame>(), 300);
                    break;

                case ProjectileID.HarpyFeather:
                    target.AddBuff(ModContent.BuffType<ClippedWings>(), 300);
                    break;

                case ProjectileID.SandBallFalling:
                    if (projectile.velocity.X != 0) //so only antlion sand and not falling sand 
                    {
                        target.GetModPlayer<FargoSoulsPlayer>().AddBuffNoStack(ModContent.BuffType<Stunned>(), 120);
                    }
                    break;

                case ProjectileID.Stinger:
                case ProjectileID.QueenBeeStinger:
                    target.AddBuff(ModContent.BuffType<Swarming>(), 300);
                    break;

                case ProjectileID.Skull:
                    target.GetModPlayer<FargoSoulsPlayer>().AddBuffNoStack(BuffID.Cursed, 30);
                    if (sourceNPC is NPC && sourceNPC.type == NPCID.DungeonGuardian)
                        target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 600);
                    break;

                case ProjectileID.EyeLaser:
                case ProjectileID.GoldenShowerHostile:
                case ProjectileID.CursedFlameHostile:
                    if (sourceNPC is NPC && (sourceNPC.type == NPCID.WallofFlesh || sourceNPC.type == NPCID.WallofFleshEye))
                        target.AddBuff(BuffID.OnFire, 300);
                    break;

                case ProjectileID.DrManFlyFlask:
                    switch (Main.rand.Next(7))
                    {
                        case 0:
                            target.AddBuff(BuffID.Venom, 300);
                            break;
                        case 1:
                            target.AddBuff(BuffID.Confused, 300);
                            break;
                        case 2:
                            target.AddBuff(BuffID.CursedInferno, 300);
                            break;
                        case 3:
                            target.AddBuff(BuffID.OgreSpit, 300);
                            break;
                        case 4:
                            target.AddBuff(ModContent.BuffType<LivingWasteland>(), 600);
                            break;
                        case 5:
                            target.AddBuff(ModContent.BuffType<Defenseless>(), 600);
                            break;
                        case 6:
                            target.AddBuff(ModContent.BuffType<Purified>(), 600);
                            break;

                        default:
                            break;
                    }
                    target.AddBuff(BuffID.Stinky, 1200);
                    break;

                case ProjectileID.SpikedSlimeSpike:
                    target.AddBuff(BuffID.Slimed, 120);
                    break;

                case ProjectileID.QueenSlimeGelAttack:
                case ProjectileID.QueenSlimeMinionBlueSpike:
                case ProjectileID.QueenSlimeMinionPinkBall:
                case ProjectileID.QueenSlimeSmash:
                    target.AddBuff(BuffID.Slimed, 180);
                    break;

                case ProjectileID.CultistBossLightningOrb:
                case ProjectileID.CultistBossLightningOrbArc:
                    target.AddBuff(BuffID.Electrified, 300);
                    break;

                case ProjectileID.CultistBossIceMist:
                    target.GetModPlayer<FargoSoulsPlayer>().AddBuffNoStack(BuffID.Frozen, 45);
                    target.AddBuff(ModContent.BuffType<Hypothermia>(), 1200);
                    break;

                case ProjectileID.CultistBossFireBall:
                    target.AddBuff(BuffID.OnFire, 300);
                    target.AddBuff(BuffID.Burning, 120);

                    if (sourceNPC is NPC && sourceNPC.type == NPCID.DD2Betsy)
                    {
                        //target.AddBuff(BuffID.OnFire, 600);
                        //target.AddBuff(BuffID.Ichor, 600);
                        target.AddBuff(BuffID.WitheredArmor, 300);
                        target.AddBuff(BuffID.WitheredWeapon, 300);
                        target.AddBuff(BuffID.Burning, 300);
                    }
                    break;

                case ProjectileID.CultistBossFireBallClone:
                    target.AddBuff(ModContent.BuffType<Shadowflame>(), 600);
                    break;

                case ProjectileID.PaladinsHammerHostile:
                    target.GetModPlayer<FargoSoulsPlayer>().AddBuffNoStack(ModContent.BuffType<Stunned>(), 60);
                    break;

                case ProjectileID.RuneBlast:
                    target.AddBuff(ModContent.BuffType<Hexed>(), 240);

                    if (sourceNPC is NPC && sourceNPC.type == NPCID.RuneWizard)
                    {
                        target.AddBuff(ModContent.BuffType<FlamesoftheUniverse>(), 60);
                        target.AddBuff(BuffID.Suffocation, 240);
                    }
                    break;

                case ProjectileID.PoisonSeedPlantera:
                    target.AddBuff(BuffID.Poisoned, 300);
                    goto case ProjectileID.SeedPlantera;
                case ProjectileID.SeedPlantera:
                case ProjectileID.ThornBall:
                    target.AddBuff(ModContent.BuffType<IvyVenom>(), 240);
                    break;

                case ProjectileID.DesertDjinnCurse:
                    if (target.ZoneCorrupt)
                        target.AddBuff(BuffID.CursedInferno, 240);
                    else if (target.ZoneCrimson)
                        target.AddBuff(BuffID.Ichor, 240);
                    break;

                case ProjectileID.BrainScramblerBolt:
                    target.AddBuff(ModContent.BuffType<Flipped>(), 60);
                    target.AddBuff(ModContent.BuffType<Unstable>(), 60);
                    break;

                case ProjectileID.MartianTurretBolt:
                case ProjectileID.GigaZapperSpear:
                    target.AddBuff(ModContent.BuffType<LightningRod>(), 300);
                    break;

                case ProjectileID.RayGunnerLaser:
                    target.AddBuff(BuffID.VortexDebuff, 180);
                    break;

                case ProjectileID.SaucerMissile:
                    target.AddBuff(ModContent.BuffType<ClippedWings>(), 300);
                    target.AddBuff(ModContent.BuffType<Crippled>(), 300);
                    break;

                case ProjectileID.SaucerLaser:
                    target.AddBuff(BuffID.Electrified, 300);
                    break;

                case ProjectileID.UFOLaser:
                case ProjectileID.SaucerDeathray:
                    target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 360);
                    break;

                case ProjectileID.FlamingWood:
                case ProjectileID.GreekFire1:
                case ProjectileID.GreekFire2:
                case ProjectileID.GreekFire3:
                    target.AddBuff(ModContent.BuffType<Shadowflame>(), 180);
                    break;

                case ProjectileID.VortexAcid:
                case ProjectileID.VortexLaser:
                    target.AddBuff(ModContent.BuffType<LightningRod>(), 600);
                    target.AddBuff(ModContent.BuffType<ClippedWings>(), 300);
                    break;

                case ProjectileID.VortexLightning:
                    target.AddBuff(BuffID.Electrified, 300);
                    break;

                case ProjectileID.LostSoulHostile:
                    target.AddBuff(ModContent.BuffType<Hexed>(), 240);
                    break;

                case ProjectileID.InfernoHostileBlast:
                case ProjectileID.InfernoHostileBolt:
                    if (!(sourceNPC is NPC && sourceNPC.type == ModContent.NPCType<NPCs.DeviBoss.DeviBoss>()))
                    {
                        if (Main.rand.NextBool(5))
                            target.AddBuff(ModContent.BuffType<Fused>(), 1800);
                    }
                    break;

                case ProjectileID.ShadowBeamHostile:
                    if (!(sourceNPC is NPC && sourceNPC.type == ModContent.NPCType<NPCs.DeviBoss.DeviBoss>()))
                    {
                        target.AddBuff(ModContent.BuffType<Rotting>(), 1800);
                        target.AddBuff(ModContent.BuffType<Shadowflame>(), 300);
                    }
                    break;

                case ProjectileID.PhantasmalDeathray:
                    target.AddBuff(ModContent.BuffType<CurseoftheMoon>(), 360);
                    break;

                case ProjectileID.PhantasmalBolt:
                case ProjectileID.PhantasmalEye:
                case ProjectileID.PhantasmalSphere:
                    target.AddBuff(ModContent.BuffType<CurseoftheMoon>(), 360);
                    if (FargoSoulsWorld.EternityMode && sourceNPC is NPC && sourceNPC.type == ModContent.NPCType<NPCs.MutantBoss.MutantBoss>())
                    {
                        target.GetModPlayer<FargoSoulsPlayer>().MaxLifeReduction += 100;
                        target.AddBuff(ModContent.BuffType<OceanicMaul>(), 5400);
                        target.AddBuff(ModContent.BuffType<MutantFang>(), 180);
                    }
                    break;

                case ProjectileID.RocketSkeleton:
                    target.AddBuff(BuffID.Dazed, 120);
                    target.AddBuff(ModContent.BuffType<Defenseless>(), 300);
                    break;

                case ProjectileID.FlamesTrap:
                case ProjectileID.FlamethrowerTrap:
                case ProjectileID.GeyserTrap:
                case ProjectileID.Fireball:
                case ProjectileID.EyeBeam:
                    target.AddBuff(BuffID.OnFire, 300);

                    if (sourceNPC is NPC)
                    {
                        if (sourceNPC.type == NPCID.Golem)
                        {
                            target.AddBuff(BuffID.BrokenArmor, 600);
                            target.AddBuff(ModContent.BuffType<Defenseless>(), 600);
                            target.AddBuff(BuffID.WitheredArmor, 600);

                            if (Framing.GetTileSafely(sourceNPC.Center).WallType != WallID.LihzahrdBrickUnsafe)
                                target.AddBuff(BuffID.Burning, 120);
                        }

                        if (sourceNPC.type == ModContent.NPCType<EarthChampion>())
                            target.AddBuff(BuffID.Burning, 300);

                        if (sourceNPC.type == ModContent.NPCType<TerraChampion>())
                        {
                            target.AddBuff(BuffID.OnFire, 600);
                            target.AddBuff(ModContent.BuffType<LivingWasteland>(), 600);
                        }
                    }
                    break;

                case ProjectileID.DD2BetsyFireball:
                case ProjectileID.DD2BetsyFlameBreath:
                    //target.AddBuff(BuffID.OnFire, 600);
                    //target.AddBuff(BuffID.Ichor, 600);
                    target.AddBuff(BuffID.WitheredArmor, 300);
                    target.AddBuff(BuffID.WitheredWeapon, 300);
                    target.AddBuff(BuffID.Burning, 300);
                    break;

                case ProjectileID.DD2DrakinShot:
                    target.AddBuff(ModContent.BuffType<Shadowflame>(), 600);
                    break;

                case ProjectileID.NebulaSphere:
                case ProjectileID.NebulaLaser:
                case ProjectileID.NebulaBolt:
                    target.AddBuff(ModContent.BuffType<Berserked>(), 300);
                    target.AddBuff(ModContent.BuffType<Lethargic>(), 300);
                    break;

                case ProjectileID.StardustJellyfishSmall:
                case ProjectileID.StardustSoldierLaser:
                case ProjectileID.Twinkle:
                    target.AddBuff(BuffID.Obstructed, 20);
                    target.AddBuff(BuffID.Blackout, 300);
                    break;

                case ProjectileID.Sharknado:
                case ProjectileID.Cthulunado:
                    target.AddBuff(ModContent.BuffType<Defenseless>(), 600);
                    target.AddBuff(ModContent.BuffType<OceanicMaul>(), 20 * 60);
                    target.GetModPlayer<FargoSoulsPlayer>().MaxLifeReduction += FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.fishBossEX, NPCID.DukeFishron) ? 100 : 25;
                    break;

                case ProjectileID.FlamingScythe:
                    target.AddBuff(BuffID.OnFire, 900);
                    target.AddBuff(ModContent.BuffType<LivingWasteland>(), 900);
                    break;

                case ProjectileID.FrostWave:
                case ProjectileID.FrostShard:
                    target.AddBuff(ModContent.BuffType<Hypothermia>(), 600);
                    break;

                case ProjectileID.SnowBallHostile:
                    target.GetModPlayer<FargoSoulsPlayer>().AddBuffNoStack(BuffID.Frozen, 45);
                    break;

                case ProjectileID.BulletSnowman:
                    target.AddBuff(ModContent.BuffType<Hypothermia>(), 600);
                    break;

                case ProjectileID.UnholyTridentHostile:
                    target.AddBuff(BuffID.Darkness, 300);
                    target.AddBuff(BuffID.Blackout, 300);
                    target.AddBuff(ModContent.BuffType<Shadowflame>(), 600);
                    //target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 180);
                    break;

                case ProjectileID.BombSkeletronPrime:
                    target.AddBuff(ModContent.BuffType<Defenseless>(), 600);
                    break;

                case ProjectileID.DeathLaser:
                    if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.retiBoss, NPCID.Retinazer))
                        target.AddBuff(BuffID.Ichor, 600);
                    if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.destroyBoss, NPCID.TheDestroyer))
                        target.AddBuff(BuffID.Electrified, 60);
                    break;

                case ProjectileID.MoonlordBullet:
                    if (sourceNPC is NPC && sourceNPC.type == NPCID.VortexRifleman)
                    {
                        target.AddBuff(ModContent.BuffType<LightningRod>(), 300);
                        target.AddBuff(ModContent.BuffType<ClippedWings>(), 120);
                    }
                    break;

                case ProjectileID.IceSickle:
                    target.AddBuff(BuffID.Frostburn, 240);
                    target.AddBuff(BuffID.Chilled, 120);
                    break;

                case ProjectileID.WaterBolt:
                case ProjectileID.WaterStream:
                    target.AddBuff(BuffID.Wet, 600);
                    break;

                case ProjectileID.DeathSickle:
                    target.AddBuff(ModContent.BuffType<LivingWasteland>(), 600);
                    target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 300);
                    break;

                case ProjectileID.MeteorShot:
                    if (sourceNPC is NPC && sourceNPC.type == NPCID.TacticalSkeleton)
                    {
                        target.AddBuff(BuffID.OnFire, 360);
                        target.AddBuff(BuffID.Burning, 180);
                    }
                    goto case ProjectileID.BulletDeadeye;
                case ProjectileID.JestersArrow:
                    if (sourceNPC is NPC && sourceNPC.type == NPCID.BigMimicHallow)
                        target.AddBuff(ModContent.BuffType<Smite>(), 600);
                    goto case ProjectileID.BulletDeadeye;
                case ProjectileID.BulletDeadeye:
                    if (sourceNPC is NPC && (sourceNPC.type == NPCID.PirateShipCannon || sourceNPC.type == NPCID.PirateDeadeye || sourceNPC.type == NPCID.PirateCrossbower))
                        target.AddBuff(ModContent.BuffType<Midas>(), 600);
                    break;

                case ProjectileID.CannonballHostile:
                    target.AddBuff(ModContent.BuffType<Defenseless>(), 600);
                    target.AddBuff(ModContent.BuffType<Midas>(), 900);
                    break;

                case ProjectileID.AncientDoomProjectile:
                    target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 300);
                    target.AddBuff(ModContent.BuffType<Shadowflame>(), 300);
                    break;

                case ProjectileID.ShadowFlame:
                    target.AddBuff(ModContent.BuffType<Shadowflame>(), 300);
                    break;

                case ProjectileID.SandnadoHostile:
                    if (!target.HasBuff(BuffID.Dazed))
                        target.AddBuff(BuffID.Dazed, 120);
                    break;

                case ProjectileID.DD2OgreSmash:
                    target.AddBuff(BuffID.BrokenArmor, 300);
                    break;

                case ProjectileID.DD2OgreStomp:
                    target.AddBuff(BuffID.Dazed, 120);
                    break;

                case ProjectileID.DD2DarkMageBolt:
                    target.AddBuff(ModContent.BuffType<Hexed>(), 240);
                    break;

                case ProjectileID.IceSpike:
                    //target.AddBuff(BuffID.Slimed, 120);
                    target.AddBuff(ModContent.BuffType<Hypothermia>(), 300);
                    break;

                case ProjectileID.JungleSpike:
                    //target.AddBuff(BuffID.Slimed, 120);
                    target.AddBuff(ModContent.BuffType<Infested>(), 300);
                    break;

                default:
                    break;
            }
        }

        public override bool PreKill(Projectile projectile, int timeLeft)
        {
            if (!FargoSoulsWorld.EternityMode)
                return base.PreKill(projectile, timeLeft);

            if (projectile.owner == Main.myPlayer && HasKillCooldown)
            {
                if (Main.player[projectile.owner].GetModPlayer<EModePlayer>().MasomodeCrystalTimer > 60)
                    return false;

                Main.player[projectile.owner].GetModPlayer<EModePlayer>().MasomodeCrystalTimer += 12;
                return true;
            }

            return base.PreKill(projectile, timeLeft);
        }

        public override void Kill(Projectile projectile, int timeLeft)
        {
            if (!FargoSoulsWorld.EternityMode)
                return;

            switch (projectile.type)
            {
                case ProjectileID.BloodNautilusTears:
                case ProjectileID.BloodNautilusShot:
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(Terraria.Entity.InheritSource(projectile), projectile.Center, Vector2.Zero, ModContent.ProjectileType<BloodFountain>(), projectile.damage, 0f, Main.myPlayer, Main.rand.Next(16, 48));
                    break;

                default:
                    break;
            }
        }

        public override Color? GetAlpha(Projectile projectile, Color lightColor)
        {
            if (!FargoSoulsWorld.EternityMode)
                return base.GetAlpha(projectile, lightColor);

            if ((projectile.type == ProjectileID.PoisonSeedPlantera || projectile.type == ProjectileID.SeedPlantera)
                && counter % 8 < 4)
            {
                return new Color(255, 255, 255, 0) * projectile.Opacity;
            }

            return base.GetAlpha(projectile, lightColor);
        }
    }
}
