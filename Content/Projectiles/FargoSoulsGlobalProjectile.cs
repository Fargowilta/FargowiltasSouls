using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasSouls.Content.Bosses.Champions.Shadow;
using FargowiltasSouls.Content.Bosses.Champions.Timber;
using FargowiltasSouls.Content.Bosses.DeviBoss;
using FargowiltasSouls.Content.Bosses.TrojanSquirrel;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Content.Items.Accessories.Masomode;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls.Content.Items.Armor;
using FargowiltasSouls.Content.Items.Weapons.SwarmDrops;
using FargowiltasSouls.Content.Patreon.DanielTheRobot;
using FargowiltasSouls.Content.PlayerDrawLayers;
using FargowiltasSouls.Content.Projectiles.BossWeapons;
using FargowiltasSouls.Content.Projectiles.Masomode;
using FargowiltasSouls.Content.Projectiles.Minions;
using FargowiltasSouls.Content.Projectiles.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.Systems;
using Luminance.Core.Graphics;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static FargowiltasSouls.Content.Items.Accessories.Forces.TimberForce;

namespace FargowiltasSouls.Content.Projectiles
{
    public class FargoSoulsGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        //        private bool townNPCProj;
        public int counter;
        public bool Rainbow;
        public int GrazeCD;

        //enchants

        /// <summary>
        /// Whether effects like Adamantite Enchantment or generally most SplitProj calls work.
        /// <br/>When trying to disable it, do so in SetDefaults!
        /// <br/>When checking it, bear in mind that OnSpawn comes before a Projectile.NewProjectile() returns! High danger of infinite recursion
        /// </summary>
        public bool CanSplit = true;
        public bool ItemSource = false;
        // private int numSplits = 1;
        public int stormTimer;
        public float TungstenScale = 1;
        public bool TikiTagged;
        private int spookyCD;
        public bool FrostFreeze;
        //        public bool SuperBee;
        public bool ChilledProj;
        public int ChilledTimer;
        public bool canUmbrellaReflect = true;
        public bool Adamantite = false;
        public int HuntressProj = -1; // -1 = non weapon proj, doesnt matter if it hits
                                      //1 = marked as weapon proj
                                      //2 = has successfully hit an enemy

        public Func<Projectile, bool> GrazeCheck = projectile =>
            projectile.Distance(Main.LocalPlayer.Center) < Math.Min(projectile.width, projectile.height) / 2 + Player.defaultHeight + Main.LocalPlayer.FargoSouls().GrazeRadius
            && (projectile.ModProjectile == null || projectile.ModProjectile.CanDamage() != false)
            && Collision.CanHit(projectile.Center, 0, 0, Main.LocalPlayer.Center, 0, 0);

        private bool firstTick = true;
        private readonly bool squeakyToy = false;

        public const int TimeFreezeMoveDuration = 10;
        public int TimeFrozen = 0;
        public bool TimeFreezeImmune;
        public int DeletionImmuneRank;
        public float CirnoBurst;

        public bool IsAHeldProj;

        public bool canHurt = true;

        public bool noInteractionWithNPCImmunityFrames;
        private int tempIframe;

        public int DamageCap;
        public bool EnchantmentProj;
        public float HeldProjMemorizedDamage;
        public float HeldProjMemorizedCrit;
        public bool Reflected;

        public float TagStackMultiplier = 1;

        public static List<int> ShroomiteBlacklist =
        [
            
        ];
        public static List<int> ShroomiteNerfList =
        [
            ModContent.ProjectileType<MechEyeProjectile>(),
            ModContent.ProjectileType<MechFlail>()
        ];

        public override void SetStaticDefaults()
        {
            A_SourceNPCGlobalProjectile.SourceNPCSync[ProjectileID.DD2ExplosiveTrapT3Explosion] = true;
            A_SourceNPCGlobalProjectile.SourceNPCSync[ProjectileID.DesertDjinnCurse] = true;
            A_SourceNPCGlobalProjectile.SourceNPCSync[ProjectileID.SandnadoHostile] = true;

            A_SourceNPCGlobalProjectile.DamagingSync[ProjectileID.DD2ExplosiveTrapT3Explosion] = true;
            A_SourceNPCGlobalProjectile.DamagingSync[ProjectileID.SharpTears] = true;
            A_SourceNPCGlobalProjectile.DamagingSync[ProjectileID.DeerclopsIceSpike] = true;
            A_SourceNPCGlobalProjectile.DamagingSync[ProjectileID.ShadowFlame] = true;
        }

        public override void SetDefaults(Projectile projectile)
        {
            switch (projectile.type)
            {
                case ProjectileID.FinalFractal:
                    DeletionImmuneRank = 2;
                    TimeFreezeImmune = true;
                    break;

                case ProjectileID.StardustGuardian:
                case ProjectileID.StardustGuardianExplosion:
                case ProjectileID.StardustPunch:
                    TimeFreezeImmune = true;
                    break;

                case ProjectileID.Sharknado:
                case ProjectileID.Cthulunado:
                    DeletionImmuneRank = 1;
                    break;

                case ProjectileID.MoonlordTurretLaser:
                    projectile.DamageType = DamageClass.Summon;
                    DeletionImmuneRank = 1;
                    break;

                case ProjectileID.LastPrism:
                case ProjectileID.LastPrismLaser:
                case ProjectileID.ChargedBlasterCannon:
                case ProjectileID.ChargedBlasterLaser:
                    DeletionImmuneRank = 1;
                    TimeFreezeImmune = true;
                    break;

                case ProjectileID.SandnadoFriendly:
                    DeletionImmuneRank = 1;
                    break;

                case ProjectileID.LunarFlare:
                    DeletionImmuneRank = 1;
                    break;

                case ProjectileID.RainbowFront:
                case ProjectileID.RainbowBack:
                    DeletionImmuneRank = 1;
                    break;

                case ProjectileID.PhantasmalDeathray:
                case ProjectileID.DeerclopsIceSpike:
                case ProjectileID.FairyQueenSunDance:
                case ProjectileID.SaucerDeathray:
                case ProjectileID.SandnadoHostile:
                case ProjectileID.SandnadoHostileMark:
                case ProjectileID.StardustSoldierLaser:
                    DeletionImmuneRank = 1;
                    break;

                case ProjectileID.DD2BetsyFlameBreath:
                    DeletionImmuneRank = 1;
                    break;

                case ProjectileID.StardustCellMinionShot:
                case ProjectileID.MiniSharkron:
                case ProjectileID.UFOLaser:
                    ProjectileID.Sets.MinionShot[projectile.type] = true;
                    break;

                case ProjectileID.SpiderEgg:
                case ProjectileID.BabySpider:
                case ProjectileID.FrostBlastFriendly:
                case ProjectileID.RainbowCrystalExplosion:
                case ProjectileID.DD2FlameBurstTowerT1Shot:
                case ProjectileID.DD2FlameBurstTowerT2Shot:
                case ProjectileID.DD2FlameBurstTowerT3Shot:
                case ProjectileID.DD2BallistraProj:
                case ProjectileID.DD2ExplosiveTrapT1Explosion:
                case ProjectileID.DD2ExplosiveTrapT2Explosion:
                case ProjectileID.DD2ExplosiveTrapT3Explosion:
                case ProjectileID.MonkStaffT1Explosion:
                case ProjectileID.DD2LightningAuraT1:
                case ProjectileID.DD2LightningAuraT2:
                case ProjectileID.DD2LightningAuraT3:
                    projectile.DamageType = DamageClass.Summon;
                    break;

                default:
                    break;
            }

            //            Fargowiltas.ModProjDict.TryGetValue(projectile.type, out ModProjID);
        }
        public void ModifyProjectileSize(Projectile projectile, Player player, IEntitySource source)
        {
            float scale = 1f;
            if (player.HasEffect<TungstenEffect>())
                scale += TungstenEffect.TungstenIncreaseProjSize(projectile, player.FargoSouls(), source);

            if (WorldSavingSystem.EternityMode && projectile.type == ProjectileID.PalladiumPike)
                scale += 0.4f;

            if (scale == 1f)
                return;

            projectile.position = projectile.Center;
            projectile.scale *= scale;
            projectile.width = (int)(projectile.width * scale);
            projectile.height = (int)(projectile.height * scale);
            projectile.Center = projectile.position;
            FargoSoulsGlobalProjectile globalProjectile = projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>();
            globalProjectile.TungstenScale = scale;

            if (projectile.aiStyle == ProjAIStyleID.Spear || projectile.aiStyle == ProjAIStyleID.ShortSword)
                projectile.velocity *= scale;
        }
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (WorldGen.generatingWorld)
            {
                return;
            }

            if (source is EntitySource_Parent parentReflected && parentReflected.Entity is Projectile parentReflectedProj && parentReflectedProj.FargoSouls().Reflected)
            {
                projectile.hostile = false;
                projectile.friendly = true;
                Reflected = true;
                projectile.owner = parentReflectedProj.owner;
                projectile.DamageType = parentReflectedProj.DamageType;
                projectile.netUpdate = true;
            }
            //not doing this causes player array index error during worldgen in some cases maybe??
            if (projectile.owner < 0 || projectile.owner >= Main.maxPlayers)
                return;

            Player player = Main.player[projectile.owner];
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            if (projectile.friendly)
            {
                if (FargoSoulsUtil.IsProjSourceItemUseReal(projectile, source))
                    ItemSource = true;
                if (source is EntitySource_Parent parent && parent.Entity is Projectile sourceProj3)
                {
                    //if (sourceProj3.FargoSouls().ItemSource)
                    //    ItemSource = true;
                    //projs shot by tiki-buffed projs will also inherit the tiki buff
                    if (sourceProj3.FargoSouls().TikiTagged)
                    {
                        //TikiTagged = true;
                    }
                }
                if (Main.rand.NextBool(2) && !projectile.hostile && !projectile.trap && !projectile.npcProj && modPlayer.Jammed && projectile.CountsAsClass(DamageClass.Ranged) && projectile.type != ProjectileID.ConfettiGun)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Particle p = new SparkParticle(projectile.Center + projectile.velocity * 4,
                            projectile.velocity.RotatedByRandom(MathHelper.PiOver2 * 0.15f) * Main.rand.NextFloat(0.7f, 1f), Color.OrangeRed, Main.rand.NextFloat(0.75f, 1.25f), 20, true, Color.Red);
                        p.Spawn();
                    }
                    //Projectile.NewProjectile(Entity.InheritSource(projectile), projectile.Center, projectile.velocity, ProjectileID.ConfettiGun, 0, 0f, projectile.owner);
                    projectile.active = false;
                }
            }

            if (player.HasEffect<NebulaEffect>() && projectile.type != ModContent.ProjectileType<NebulaShot>())
            {
                if (modPlayer.NebulaEnchCD <= 0)
                {
                    if (FargoSoulsUtil.OnSpawnEnchCanAffectProjectile(projectile, false)
                        && projectile.whoAmI != player.heldProj
                        && projectile.aiStyle != 190 // fancy sword swings like excalibur
                        && !projectile.minion) 
                    {
                        int damage = Math.Max(1200, projectile.damage);
                        damage = (int)MathHelper.Clamp(damage, 0, 3000);
                        if (modPlayer.ForceEffect<NebulaEnchant>())
                            damage = (int)(damage * 1.66667f);

                        Projectile.NewProjectile(player.GetSource_FromThis(), projectile.Center, projectile.velocity, ModContent.ProjectileType<NebulaShot>(), (int)(damage * player.ActualClassDamage(DamageClass.Magic)), 1f, player.whoAmI, 0);
                        projectile.active = false;
                        modPlayer.NebulaEnchCD = 3 * 60;
                    }
                }
            }

            switch (projectile.type)
            {
                case ProjectileID.SpiritHeal:
                    if (player.HasEffect<SpectreEffect>() && !modPlayer.TerrariaSoul)
                    {
                        projectile.extraUpdates = 1;
                        projectile.timeLeft = 180 * projectile.MaxUpdates;
                    }
                    break;

                case ProjectileID.DD2ExplosiveTrapT3Explosion:
                    {
                        if (projectile.damage > 0 && source is EntitySource_Parent parent && parent.Entity is NPC npc && npc.active
                            && (npc.ModNPC is TrojanSquirrelPart || npc.type == ModContent.NPCType<TimberChampion>()))
                        {
                            projectile.DamageType = DamageClass.Default;
                            projectile.friendly = false;
                            projectile.hostile = true;
                            projectile.alpha = 0;
                            DeletionImmuneRank = 1;
                        }
                    }
                    break;

                case ProjectileID.ShadowFlame:
                    {
                        if (projectile.damage > 0 && source is EntitySource_Parent parent && parent.Entity is NPC npc && npc.active
                            && npc.type == ModContent.NPCType<ShadowChampion>())
                        {
                            projectile.DamageType = DamageClass.Default;
                            projectile.friendly = false;
                            projectile.hostile = true;
                        }
                    }
                    break;

                case ProjectileID.FairyQueenMagicItemShot:
                    {
                        if (source is EntitySource_Misc misc && misc.Context.Equals("Pearlwood"))
                        {
                            projectile.usesLocalNPCImmunity = false;

                            projectile.usesIDStaticNPCImmunity = true;
                            projectile.idStaticNPCHitCooldown = 10;
                            noInteractionWithNPCImmunityFrames = true;
                        }
                    }
                    break;

                case ProjectileID.SharpTears:
                case ProjectileID.DeerclopsIceSpike:
                    {
                        if (source is EntitySource_ItemUse parent1 && (parent1.Item.type == ModContent.ItemType<Deerclawps>() || parent1.Item.type == ModContent.ItemType<LumpOfFlesh>() || parent1.Item.type == ModContent.ItemType<MasochistSoul>()))
                        {
                            projectile.hostile = false;
                            projectile.friendly = true;
                            projectile.DamageType = DamageClass.Melee;
                            projectile.penetrate = -1;

                            projectile.usesLocalNPCImmunity = false;

                            projectile.usesIDStaticNPCImmunity = true;
                            projectile.idStaticNPCHitCooldown = 10;

                            projectile.FargoSouls().CanSplit = false;
                            projectile.FargoSouls().noInteractionWithNPCImmunityFrames = true;

                            FargowiltasSouls.MutantMod.Call("LowRenderProj", projectile);
                        }
                    }
                    break;

                case ProjectileID.DesertDjinnCurse:
                    {
                        if (projectile.damage > 0 && source is EntitySource_Parent parent && parent.Entity is NPC npc && npc.active && npc.type == ModContent.NPCType<ShadowChampion>())
                            projectile.damage = FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage);
                    }
                    break;

                case ProjectileID.SandnadoHostile:
                    {
                        if (projectile.damage > 0 && source is EntitySource_Parent parent && parent.Entity is NPC npc && npc.active)
                        {
                            if (npc.type == ModContent.NPCType<DeviBoss>())
                            {
                                projectile.damage = FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage);
                                if (npc.ai[0] == 5)
                                    projectile.timeLeft = Math.Min(projectile.timeLeft, 360 + 90 - (int)npc.ai[1]);
                                else
                                    projectile.timeLeft = 90;
                            }
                            else if (npc.type == ModContent.NPCType<ShadowChampion>())
                            {
                                projectile.damage = FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage);
                            }
                        }
                    }
                    break;
                case ProjectileID.SeedlerThorn:
                    {
                        if (source is EntitySource_Parent parent && parent.Entity is Projectile parentProj && parentProj.type == ProjectileID.SeedlerNut && parentProj.DamageType == DamageClass.Summon)
                        {
                            projectile.DamageType = DamageClass.Summon;
                            EnchantmentProj = true;
                        }
                            
                    }
                    break;
                default:
                    break;
            }

            ModifyProjectileSize(projectile, player, source);
            /*
            if (player.HasEffect<TungstenEffect>())
                TungstenEffect.TungstenIncreaseProjSize(projectile, modPlayer, source);
            */

            if (player.HasEffect<HuntressEffect>()
                && ItemSource
                && projectile.damage > 0 && projectile.friendly && !projectile.hostile && !projectile.trap
                && projectile.DamageType != DamageClass.Default
                && !ProjectileID.Sets.CultistIsResistantTo[projectile.type]
                && !FargoSoulsUtil.IsSummonDamage(projectile, true, false))
            {
                HuntressProj = 1;
            }

            if (source is EntitySource_Parent parent3 && parent3.Entity is Projectile sourceProj && sourceProj.FargoSouls().DamageCap > 0)
                DamageCap = sourceProj.FargoSouls().DamageCap;

            if (projectile.bobber && CanSplit && source is EntitySource_ItemUse)
            {
                int splitCount = 0;
                if (modPlayer.FishSoul2)
                    splitCount = 11;
                else if (modPlayer.FishSoul1 && modPlayer.ForceEffect<AnglerEnchant>())
                    splitCount = 6;
                if (player.whoAmI == Main.myPlayer && splitCount > 0)
                    SplitProj(projectile, splitCount, MathHelper.Pi / 3, 1);
            }

            // Fix for extended sword hitboxes having a maximum range for some reason
            if (projectile.aiStyle == ProjAIStyleID.NightsEdge)
                projectile.ownerHitCheckDistance *= projectile.scale;
        }

        public static int[] NoSplit => [
            ProjectileID.SandnadoFriendly,
            ProjectileID.LastPrism,
            ProjectileID.LastPrismLaser,
            ProjectileID.BabySpider,
            ProjectileID.Phantasm,
            ProjectileID.VortexBeater,
            ProjectileID.ChargedBlasterCannon,
            ProjectileID.WireKite,
            ProjectileID.DD2PhoenixBow,
            ProjectileID.LaserMachinegun,
            ProjectileID.PiercingStarlight,
            ProjectileID.Celeb2Weapon,
            ProjectileID.Xenopopper
        ];
        public override bool PreAI(Projectile projectile)
        {
            bool retVal = true;
            Player player = Main.player[projectile.owner];
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            counter++;

            if (IsAHeldProj)
            {
                //doing it this janky way so that it takes the relative item stat difference, and applies it to itself
                //this way if proj modifies its own damage, that isn't overridden
                //e.g. whips doing less damage after each hit
                //the actual base damage of the weapon sometimes ends up 1 less than what is shown. idk why. nobody should miss it??
                //player.gettotaldamage makes no sense btw fuck tmod
                long weaponDamage = player.GetWeaponDamage(player.HeldItem);
                int weaponCrit = player.GetWeaponCrit(player.HeldItem);
                if (HeldProjMemorizedDamage == 0)
                    HeldProjMemorizedDamage = weaponDamage;
                if (HeldProjMemorizedCrit == 0)
                    HeldProjMemorizedCrit = weaponCrit;
                double finalDamage = (long)(projectile.damage * weaponDamage / HeldProjMemorizedDamage);
                projectile.damage = (int)Math.Round(finalDamage, 0, MidpointRounding.ToEven);
                projectile.CritChance = (int)Math.Round(projectile.CritChance * weaponCrit / HeldProjMemorizedCrit, 0, MidpointRounding.ToEven);
                HeldProjMemorizedDamage = weaponDamage;
                HeldProjMemorizedCrit = weaponCrit;
            }

            if (spookyCD > 0)
            {
                spookyCD--;
            }
            if (projectile.active && projectile.friendly && !projectile.hostile)
            {
                foreach (int tornadoIndex in modPlayer.ForbiddenTornados)
                {
                    Projectile storm = Main.projectile[tornadoIndex];
                    if (storm.Alive() &&  projectile.owner == storm.owner && projectile.type != storm.type && projectile.Colliding(projectile.Hitbox, storm.Hitbox))
                    {
                        stormTimer = 240;
                    }
                }
                if (projectile.damage > 0 && !FargoSoulsUtil.IsSummonDamage(projectile, false) && projectile.type != ModContent.ProjectileType<ShadowBall>())
                {
                    foreach (int orbIndex in modPlayer.ShadowOrbs)
                    {
                        Projectile orb = Main.projectile[orbIndex];
                        //wait for CD
                        //detect being hit
                        if (orb.Alive() && orb.ai[0] == 0f && projectile.owner == orb.owner && projectile.Colliding(projectile.Hitbox, orb.Hitbox))
                        {
                            int numBalls = 5;
                            int dmg = 25;

                            if (modPlayer.AncientShadowEnchantActive)
                            {
                                numBalls = 7;
                                dmg = 50;
                            }

                            int damage = (int)(dmg * player.ActualClassDamage(DamageClass.Magic));
                            Projectile[] balls = FargoSoulsUtil.XWay(numBalls, orb.GetSource_FromThis(), orb.Center, ModContent.ProjectileType<ShadowBall>(), 6, damage, 0);

                            foreach (Projectile ball in balls)
                            {
                                ball.originalDamage = damage;
                            }


                            if (FargoSoulsUtil.CanDeleteProjectile(projectile))
                                projectile.Kill();

                            orb.ai[0] = 300;
                            orb.netUpdate = true;

                            break;
                        }
                    }
                }
            }

            if (projectile.owner == Main.myPlayer)
            {
                //reset tungsten size
                if (TungstenScale != 1 && !player.HasEffect<TungstenEffect>())
                {
                    projectile.position = projectile.Center;
                    projectile.scale /= TungstenScale;
                    projectile.width = (int)(projectile.width / TungstenScale);
                    projectile.height = (int)(projectile.height / TungstenScale);
                    projectile.Center = projectile.position;

                    TungstenScale = 1;
                }

                switch (projectile.type)
                {
                    case ProjectileID.RedCounterweight:
                    case ProjectileID.BlackCounterweight:
                    case ProjectileID.BlueCounterweight:
                    case ProjectileID.GreenCounterweight:
                    case ProjectileID.PurpleCounterweight:
                    case ProjectileID.YellowCounterweight:
                        {
                            if (projectile.owner == Main.myPlayer && player.HeldItem.type == ModContent.ItemType<Blender>())
                            {
                                if (++projectile.localAI[0] > 60)
                                {
                                    SoundEngine.PlaySound(SoundID.NPCDeath11 with { Volume = 0.5f }, projectile.Center);
                                    int proj2 = ModContent.ProjectileType<BlenderProj3>();
                                    Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, projectile.DirectionFrom(player.Center) * 8, proj2, projectile.damage, projectile.knockBack, projectile.owner);
                                    projectile.Kill();
                                }
                            }
                        }
                        break;
                }

                //hook ai
                if (player.HasEffect<MahoganyEffect>() && projectile.aiStyle == 7)
                {
                    MahoganyEffect.MahoganyHookAI(projectile, modPlayer);
                }

                if (!projectile.hostile && !projectile.trap && !projectile.npcProj)
                {

                    if (player.HasEffect<SpookyEffect>()
                        && projectile.minionSlots > 0 && spookyCD == 0)
                    {
                        float minDistance = 500f;
                        int npcIndex = -1;
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            NPC target = Main.npc[i];

                            if (target.active && Vector2.Distance(projectile.Center, target.Center) < minDistance && Main.npc[i].CanBeChasedBy(projectile, false))
                            {
                                npcIndex = i;
                                minDistance = Vector2.Distance(projectile.Center, target.Center);
                            }
                        }

                        if (npcIndex != -1)
                        {
                            NPC target = Main.npc[npcIndex];

                            if (Collision.CanHit(projectile.Center, 0, 0, target.Center, 0, 0))
                            {
                                Vector2 velocity = Vector2.Normalize(target.Center - projectile.Center) * 20;

                                int cdMultiplier = 2;

                                Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, velocity, ModContent.ProjectileType<SpookyScythe>(), projectile.damage * cdMultiplier, 2, projectile.owner);

                                SoundEngine.PlaySound(SoundID.Item62 with { Volume = 0.5f }, projectile.Center);

                                spookyCD = 30 + Main.rand.Next(player.maxMinions * 5);

                                if (modPlayer.ForceEffect<SpookyEnchant>())
                                {
                                    spookyCD -= 10;
                                }
                                spookyCD *= cdMultiplier;
                            }
                        }

                    }
                }

                
            }

            if (ChilledTimer > 0)
            {
                ChilledTimer--;

                if (retVal && ChilledTimer % 3 == 1)
                {
                    retVal = false;
                    projectile.position = projectile.oldPosition;
                    projectile.timeLeft++;
                }

                if (ChilledTimer <= 0)
                    ChilledProj = false;
            }

            if (TimeFrozen > 0 && !firstTick && !TimeFreezeImmune)
            {
                if (counter % projectile.MaxUpdates == 0) //only decrement once per tick
                    TimeFrozen--;
                if (counter > TimeFreezeMoveDuration * projectile.MaxUpdates)
                {
                    projectile.position = projectile.oldPosition;

                    if (projectile.frameCounter > 0)
                        projectile.frameCounter--;

                    if (retVal)
                    {
                        retVal = false;
                        projectile.timeLeft++;
                    }
                }
            }

            ////masomode unicorn meme and pearlwood meme
            if (Rainbow)
            {
                projectile.tileCollide = false;

                if (counter >= 5)
                    projectile.velocity = Vector2.Zero;

                int deathTimer = 10;

                if (projectile.hostile)
                    deathTimer = 60;

                if (counter >= deathTimer)
                    projectile.Kill();
            }

            if (firstTick)
            {

                if (projectile.type == ProjectileID.ShadowBeamHostile)
                {
                    if (projectile.GetSourceNPC() is NPC sourceNPC && sourceNPC.type == ModContent.NPCType<DeviBoss>())
                    {
                        projectile.timeLeft = WorldSavingSystem.MasochistModeReal ? 1200 : 420;
                    }
                }

                if (projectile.type == ProjectileID.DD2ExplosiveTrapT3Explosion && projectile.hostile)
                {
                    if (projectile.GetSourceNPC() is NPC sourceNPC && (sourceNPC.type == ModContent.NPCType<TrojanSquirrel>() || sourceNPC.type == ModContent.NPCType<TimberChampion>()))
                    {
                        projectile.position = projectile.Bottom;
                        projectile.height = 16 * 6;
                        projectile.Bottom = projectile.position;
                        projectile.hostile = true;
                        projectile.friendly = false;
                        projectile.netUpdate = true;
                    }
                }

                firstTick = false;
            }

            //Tungsten fixes and such

            switch (projectile.type)
            {
                case ProjectileID.MonkStaffT2:
                case ProjectileID.MonkStaffT3_Alt:
                    {

                        Vector2 vector = player.RotatedRelativePoint(player.MountedCenter);
                        projectile.direction = player.direction;
                        player.heldProj = projectile.whoAmI;
                        projectile.Center = vector;
                        if (player.dead)
                        {
                            projectile.Kill();
                            return false;
                        }
                        if (!player.frozen)
                        {
                            if (projectile.type == 699)
                            {
                                projectile.spriteDirection = (projectile.direction = player.direction);
                                Vector2 vector2 = vector;
                                projectile.alpha -= 127;
                                if (projectile.alpha < 0)
                                {
                                    projectile.alpha = 0;
                                }
                                if (projectile.localAI[0] > 0f)
                                {
                                    projectile.localAI[0] -= 1f;
                                }
                                float num = (float)player.itemAnimation / (float)player.itemAnimationMax;
                                float num2 = 1f - num;
                                float num3 = projectile.velocity.ToRotation();
                                float num4 = projectile.velocity.Length() * projectile.scale;
                                float num5 = 22f * projectile.scale;
                                Vector2 spinningpoint = new Vector2(1f, 0f).RotatedBy((float)Math.PI + num2 * ((float)Math.PI * 2f)) * new Vector2(num4, projectile.ai[0] * projectile.scale);
                                projectile.position += spinningpoint.RotatedBy(num3) + new Vector2(num4 + num5, 0f).RotatedBy(num3);
                                Vector2 target = vector2 + spinningpoint.RotatedBy(num3) + new Vector2(num4 + num5 + 40f, 0f).RotatedBy(num3);
                                projectile.rotation = vector2.AngleTo(target) + (float)Math.PI / 4f * (float)player.direction;
                                if (projectile.spriteDirection == -1)
                                {
                                    projectile.rotation += (float)Math.PI;
                                }
                                vector2.SafeDirectionTo(projectile.Center);
                                Vector2 vector3 = vector2.SafeDirectionTo(target);
                                Vector2 vector4 = projectile.velocity.SafeNormalize(Vector2.UnitY);
                                float num6 = 2f;
                                for (int i = 0; (float)i < num6; i++)
                                {
                                    Dust dust = Dust.NewDustDirect(projectile.Center, 14, 14, DustID.GoldFlame, 0f, 0f, 110);
                                    dust.velocity = vector2.SafeDirectionTo(dust.position) * 2f;
                                    dust.position = projectile.Center + vector4.RotatedBy(num2 * ((float)Math.PI * 2f) * 2f + (float)i / num6 * ((float)Math.PI * 2f)) * 10f;
                                    dust.scale = 1f + 0.6f * Main.rand.NextFloat();
                                    dust.velocity += vector4 * 3f;
                                    dust.noGravity = true;
                                }
                                for (int j = 0; j < 1; j++)
                                {
                                    if (Main.rand.NextBool(3))
                                    {
                                        Dust dust2 = Dust.NewDustDirect(projectile.Center, 20, 20, DustID.GoldFlame, 0f, 0f, 110);
                                        dust2.velocity = vector2.SafeDirectionTo(dust2.position) * 2f;
                                        dust2.position = projectile.Center + vector3 * -110f;
                                        dust2.scale = 0.45f + 0.4f * Main.rand.NextFloat();
                                        dust2.fadeIn = 0.7f + 0.4f * Main.rand.NextFloat();
                                        dust2.noGravity = true;
                                        dust2.noLight = true;
                                    }
                                }
                            }
                            else if (projectile.type == 708)
                            {
                                Lighting.AddLight(player.Center, 0.75f, 0.9f, 1.15f);
                                projectile.spriteDirection = (projectile.direction = player.direction);
                                projectile.alpha -= 127;
                                if (projectile.alpha < 0)
                                {
                                    projectile.alpha = 0;
                                }
                                float num7 = (float)player.itemAnimation / (float)player.itemAnimationMax;
                                float num8 = 1f - num7;
                                float num9 = projectile.velocity.ToRotation();
                                float num10 = projectile.velocity.Length() * projectile.scale; //this is literally the only line of code i changed
                                float num11 = 22f * projectile.scale; //this one too
                                Vector2 spinningpoint2 = new Vector2(1f, 0f).RotatedBy((float)Math.PI + num8 * ((float)Math.PI * 2f)) * new Vector2(num10, projectile.ai[0] * projectile.scale);
                                projectile.position += spinningpoint2.RotatedBy(num9) + new Vector2(num10 + num11, 0f).RotatedBy(num9);
                                Vector2 vector5 = vector + spinningpoint2.RotatedBy(num9) + new Vector2(num10 + num11 + 40f, 0f).RotatedBy(num9);
                                projectile.rotation = (vector5 - vector).SafeNormalize(Vector2.UnitX).ToRotation() + (float)Math.PI / 4f * (float)player.direction;
                                if (projectile.spriteDirection == -1)
                                {
                                    projectile.rotation += (float)Math.PI;
                                }
                                (projectile.Center - vector).SafeNormalize(Vector2.Zero);
                                (vector5 - vector).SafeNormalize(Vector2.Zero);
                                Vector2 vector6 = projectile.velocity.SafeNormalize(Vector2.UnitY);
                                if ((player.itemAnimation == 2 || player.itemAnimation == 6 || player.itemAnimation == 10) && projectile.owner == Main.myPlayer)
                                {
                                    Vector2 vector7 = vector6 + Main.rand.NextVector2Square(-0.2f, 0.2f);
                                    vector7 *= 12f;
                                    switch (player.itemAnimation)
                                    {
                                        case 2:
                                            vector7 = vector6.RotatedBy(0.3839724659919739);
                                            break;
                                        case 6:
                                            vector7 = vector6.RotatedBy(-0.3839724659919739);
                                            break;
                                        case 10:
                                            vector7 = vector6.RotatedBy(0.0);
                                            break;
                                    }
                                    vector7 *= 10f + (float)Main.rand.Next(4);
                                    Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, vector7, 709, projectile.damage, 0f, projectile.owner);
                                }
                                for (int k = 0; k < 3; k += 2)
                                {
                                    float num12 = 1f;
                                    float num13 = 1f;
                                    switch (k)
                                    {
                                        case 1:
                                            num13 = -1f;
                                            break;
                                        case 2:
                                            num13 = 1.25f;
                                            num12 = 0.5f;
                                            break;
                                        case 3:
                                            num13 = -1.25f;
                                            num12 = 0.5f;
                                            break;
                                    }
                                    if (!Main.rand.NextBool(6))
                                    {
                                        num13 *= 1.2f;
                                        Dust dust3 = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.Electric, 0f, 0f, 100);
                                        dust3.velocity = vector6 * (4f + 4f * Main.rand.NextFloat()) * num13 * num12;
                                        dust3.noGravity = true;
                                        dust3.noLight = true;
                                        dust3.scale = 0.75f;
                                        dust3.fadeIn = 0.8f;
                                        dust3.customData = this;
                                        if (Main.rand.NextBool(3))
                                        {
                                            dust3.noGravity = false;
                                            dust3.fadeIn = 0f;
                                        }
                                    }
                                }
                            }
                        }
                        if (player.whoAmI == Main.myPlayer && player.itemAnimation <= 2)
                        {
                            projectile.Kill();
                            player.reuseDelay = 2;
                        }
                        return false; //don't run vanilla code
                    }
            }

            return retVal;
        }


        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            switch (projectile.type)
            {
                case ProjectileID.RedCounterweight:
                case ProjectileID.BlackCounterweight:
                case ProjectileID.BlueCounterweight:
                case ProjectileID.GreenCounterweight:
                case ProjectileID.PurpleCounterweight:
                case ProjectileID.YellowCounterweight:
                    {
                        Player player = Main.player[projectile.owner];
                        if (player.HeldItem.type == ModContent.ItemType<Blender>())
                        {
                            Texture2D texture2D13 = ModContent.Request<Texture2D>("FargowiltasSouls/Content/Projectiles/PlanteraTentacle", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                            Rectangle rectangle = new(0, 0, texture2D13.Width, texture2D13.Height);
                            Vector2 origin2 = rectangle.Size() / 2f;

                            SpriteEffects spriteEffects = projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

                            Vector2 toPlayer = projectile.Center - player.Center;
                            float drawRotation = toPlayer.ToRotation() + MathHelper.Pi;
                            if (projectile.spriteDirection < 0)
                                drawRotation += (float)Math.PI;
                            Main.EntitySpriteDraw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor),
                                drawRotation, origin2, projectile.scale * 0.8f, spriteEffects, 0);
                            return false;
                        }
                    }
                    break;
                //Fixes to draw properly with Tungsten Enchantment
                case ProjectileID.Arkhalis:
                case ProjectileID.Terragrim:
                case ProjectileID.FlowerPetal:
                case ProjectileID.SporeCloud:
                case ProjectileID.ChlorophyteOrb:
                case ProjectileID.PaladinsHammerFriendly:
                    {
                        if (projectile.scale != 1)
                        {
                            FargoSoulsUtil.GenericProjectileDraw(projectile, lightColor);
                            return false;
                        }
                    }
                    break;
                    
                case ProjectileID.PiercingStarlight:
                    if (TungstenScale != 1)
                    {
                        float swordScaleModifier = TungstenScale;
                        float slashScaleModifier = TungstenScale * 1.25f;

                        int num = 3;
                        int num2 = 2;
                        Vector2 value = projectile.Center - projectile.rotation.ToRotationVector2() * num2;

                        float num3 = Main.rand.NextFloat();
                        float scale = Utils.GetLerpValue(0f, 0.3f, num3, clamped: true) * Utils.GetLerpValue(1f, 0.5f, num3, clamped: true);
                        Color color = projectile.GetAlpha(Lighting.GetColor(projectile.Center.ToTileCoordinates())) * scale;
                        Texture2D value2 = TextureAssets.Item[4923].Value;
                        Vector2 origin = value2.Size() / 2f;
                        float num4 = Main.rand.NextFloatDirection();
                        float scaleFactor = 8f + MathHelper.Lerp(0f, 20f, num3) + Main.rand.NextFloat() * 6f;
                        scaleFactor *= swordScaleModifier;
                        float num5 = projectile.rotation + num4 * ((float)Math.PI * 2f) * 0.04f;
                        float num6 = num5 + (float)Math.PI / 4f;
                        Vector2 position = value + num5.ToRotationVector2() * scaleFactor + Main.rand.NextVector2Circular(8f, 8f) - Main.screenPosition;
                        SpriteEffects spriteEffects = SpriteEffects.None;
                        if (projectile.rotation < -(float)Math.PI / 2f || projectile.rotation > (float)Math.PI / 2f)
                        {
                            num6 += (float)Math.PI / 2f;
                            spriteEffects |= SpriteEffects.FlipHorizontally;
                        }

                        Main.spriteBatch.Draw(value2, position, null, color, num6, origin, swordScaleModifier, spriteEffects, 0f);


                        for (int j = 0; j < num; j++)
                        {
                            float num7 = Main.rand.NextFloat();
                            float num8 = Utils.GetLerpValue(0f, 0.3f, num7, clamped: true) * Utils.GetLerpValue(1f, 0.5f, num7, clamped: true);
                            float amount = Utils.GetLerpValue(0f, 0.3f, num7, clamped: true) * Utils.GetLerpValue(1f, 0.5f, num7, clamped: true);
                            float scaleFactor2 = MathHelper.Lerp(0.6f, 1f, amount);
                            scaleFactor2 *= slashScaleModifier;
                            Microsoft.Xna.Framework.Color fairyQueenWeaponsColor = projectile.GetFairyQueenWeaponsColor(0.25f, 0f, (Main.rand.NextFloat() * 0.33f + Main.GlobalTimeWrappedHourly) % 1f);
                            Texture2D value3 = TextureAssets.Projectile[projectile.type].Value;
                            Microsoft.Xna.Framework.Color color2 = fairyQueenWeaponsColor;
                            color2 *= num8 * 0.5f;
                            Vector2 origin2 = value3.Size() / 2f;
                            Microsoft.Xna.Framework.Color value4 = Microsoft.Xna.Framework.Color.White * num8;
                            value4.A /= 2;
                            Microsoft.Xna.Framework.Color color3 = value4 * 0.5f;
                            float num9 = 1f;
                            float num10 = Main.rand.NextFloat() * 2f;
                            float num11 = Main.rand.NextFloatDirection();
                            Vector2 vector = new Vector2(2.8f + num10, 1f) * num9 * scaleFactor2;
                            _ = new Vector2(1.5f + num10 * 0.5f, 1f) * num9 * scaleFactor2;
                            int num12 = 50;
                            Vector2 value5 = projectile.rotation.ToRotationVector2() * ((j >= 1) ? 56 : 0);
                            float num13 = 0.03f - (float)j * 0.012f;
                            float scaleFactor3 = 30f + MathHelper.Lerp(0f, num12, num7) + num10 * 16f;
                            scaleFactor3 *= slashScaleModifier;
                            float num14 = projectile.rotation + num11 * ((float)Math.PI * 2f) * num13;
                            float rotation = num14;
                            Vector2 position2 = value + num14.ToRotationVector2() * scaleFactor3 + Main.rand.NextVector2Circular(20f, 20f) + value5 - Main.screenPosition;
                            color2 *= num9;
                            color3 *= num9;
                            SpriteEffects effects = SpriteEffects.None;
                            Main.spriteBatch.Draw(value3, position2, null, color2, rotation, origin2, vector, effects, 0f);
                            Main.spriteBatch.Draw(value3, position2, null, color3, rotation, origin2, vector * 0.6f, effects, 0f);
                        }

                        return false;
                    }
                    break;
                case ProjectileID.ApprenticeStaffT3Shot: // betsy uses this for some reason
                    {
                        NPC sourceNPC = projectile.GetSourceNPC();
                        if (sourceNPC != null && sourceNPC.type == NPCID.DD2Betsy)
                        {
                            Texture2D tex = TextureAssets.Projectile[ProjectileID.DD2BetsyFireball].Value;
                            FargoSoulsUtil.GenericProjectileDraw(projectile, lightColor, tex);
                            return false;
                        }

                    }
                    break;
                default:
                    break;
            }

            switch (projectile.aiStyle)
            {
                case ProjAIStyleID.Flail:
                    if (projectile.ModProjectile == null && projectile.scale != 1)
                    {
                        FargoSoulsUtil.GenericProjectileDraw(projectile, lightColor);
                        return false;
                    }
                    break;
            }
            return base.PreDraw(projectile, ref lightColor);
        }

        public static List<Projectile> SplitProj(Projectile projectile, int number, float maxSpread, float damageRatio, bool allowMoreSplit = false)
        {
            if (ModContent.TryFind("Fargowiltas", "SpawnProj", out ModProjectile spawnProj) && projectile.type == spawnProj.Type)
            {
                return null;
            }

            //if its odd, we just keep the original 
            if (number % 2 != 0)
            {
                number--;
            }

            List<Projectile> projList = [];
            Projectile split;
            double spread = maxSpread / number;

            for (int i = 0; i < number / 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    int factor = j == 0 ? 1 : -1;
                    split = FargoSoulsUtil.NewProjectileDirectSafe(projectile.GetSource_FromThis(), projectile.Center, projectile.velocity.RotatedBy(factor * spread * (i + 1)), projectile.type, (int)(projectile.damage * damageRatio), projectile.knockBack, projectile.owner, projectile.ai[0], projectile.ai[1]);
                    if (split != null)
                    {
                        split.ai[2] = projectile.ai[2];
                        split.localAI[0] = projectile.localAI[0];
                        split.localAI[1] = projectile.localAI[1];
                        split.localAI[2] = projectile.localAI[2];

                        split.friendly = projectile.friendly;
                        split.hostile = projectile.hostile;
                        split.timeLeft = projectile.timeLeft;
                        split.DamageType = projectile.DamageType;

                        //split.FargoSouls().numSplits = projectile.FargoSouls().numSplits;
                        if (!allowMoreSplit)
                            split.FargoSouls().CanSplit = false;
                        split.FargoSouls().TungstenScale = projectile.FargoSouls().TungstenScale;
                        split.FargoSouls().ItemSource = projectile.FargoSouls().ItemSource;

                        projList.Add(split);
                    }
                }
            }

            return projList;
        }

        private static void KillPet(Projectile projectile, Player player, int buff, bool toggle, bool minion = false)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            if (player.FindBuffIndex(buff) == -1)
            {
                if (player.dead || !toggle || (minion ? !player.HasEffect<StardustEffect>() : !modPlayer.VoidSoul) || !modPlayer.PetsActive && !minion)
                {
                    projectile.Kill();
                }
            }
        }

        public override void AI(Projectile projectile)
        {
            Player player = Main.player[projectile.owner];
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            switch (projectile.type)
            {
                #region pets

                case ProjectileID.StardustGuardian:
                    KillPet(projectile, player, BuffID.StardustGuardianMinion, player.HasEffect<StardustMinionEffect>(), true);
                    //if (modPlayer.FreezeTime && modPlayer.freezeLength > 60) //throw knives in stopped time
                    //{
                    //    if (projectile.owner == Main.myPlayer && counter % 20 == 0)
                    //    {
                    //        int target = -1;

                    //        NPC minionAttackTargetNpc = projectile.OwnerMinionAttackTargetNPC;
                    //        if (minionAttackTargetNpc != null && minionAttackTargetNpc.CanBeChasedBy())
                    //        {
                    //            target = minionAttackTargetNpc.whoAmI;
                    //        }
                    //        else
                    //        {
                    //            const float homingMaximumRangeInPixels = 1000;
                    //            for (int i = 0; i < Main.maxNPCs; i++)
                    //            {
                    //                NPC n = Main.npc[i];
                    //                if (n.CanBeChasedBy(projectile))
                    //                {
                    //                    float distance = projectile.Distance(n.Center);
                    //                    if (distance <= homingMaximumRangeInPixels &&
                    //                        (target == -1 || //there is no selected target
                    //                        projectile.Distance(Main.npc[target].Center) > distance)) //or we are closer to this target than the already selected target
                    //                    {
                    //                        target = i;
                    //                    }
                    //                }
                    //            }
                    //        }

                    //        if (target != -1)
                    //        {
                    //            const int totalUpdates = 2 + 1;
                    //            const int travelTime = TimeFreezeMoveDuration * totalUpdates;

                    //            Vector2 spawnPos = projectile.Center + 16f * projectile.SafeDirectionTo(Main.npc[target].Center);

                    //            //adjust speed so it always lands just short of touching the enemy
                    //            Vector2 vel = Main.npc[target].Center - spawnPos;
                    //            float length = (vel.Length() - 0.6f * Math.Max(Main.npc[target].width, Main.npc[target].height)) / travelTime;
                    //            if (length < 0.1f)
                    //                length = 0.1f;

                    //            float offset = 1f - (modPlayer.freezeLength - 60f) / 540f; //change how far they stop as time decreases
                    //            if (offset < 0.1f)
                    //                offset = 0.1f;
                    //            if (offset > 1f)
                    //                offset = 1f;
                    //            length *= offset;

                    //            const int max = 3;
                    //            int damage = 100; //at time of writing, raw hellzone does 190 damage, 7.5 times per second, 1425 dps
                    //            if (modPlayer.CosmoForce)
                    //                damage = 150;
                    //            if (modPlayer.TerrariaSoul)
                    //                damage = 300;
                    //            damage = (int)(damage * player.ActualClassDamage(DamageClass.Summon));
                    //            float rotation = MathHelper.ToRadians(60) * Main.rand.NextFloat(0.2f, 1f);
                    //            float rotationOffset = MathHelper.ToRadians(5) * Main.rand.NextFloat(-1f, 1f);
                    //            for (int i = -max; i <= max; i++)
                    //            {
                    //                Projectile.NewProjectile(projectile.GetSource_FromThis(), spawnPos, length * Vector2.Normalize(vel).RotatedBy(rotation / max * i + rotationOffset),
                    //                    ModContent.ProjectileType<StardustKnife>(), damage, 4f, Main.myPlayer);
                    //            }
                    //        }
                    //    }
                    //}
                    break;

                #endregion

                case ProjectileID.Flamelash:
                case ProjectileID.MagicMissile:
                case ProjectileID.RainbowRodBullet:
                    if (projectile.ai[0] != -1 && projectile.ai[1] != -1 && counter > 900 && Main.player[projectile.owner].ownedProjectileCounts[projectile.type] > 1)
                    {
                        projectile.Kill();
                        Main.player[projectile.owner].ownedProjectileCounts[projectile.type] -= 1;
                    }
                    break;

                case ProjectileID.RuneBlast:
                    if (projectile.ai[0] == 1f)
                    {
                        if (projectile.localAI[0] == 0f)
                        {
                            projectile.localAI[0] = projectile.Center.X;
                            projectile.localAI[1] = projectile.Center.Y;
                        }
                        Vector2 distance = projectile.Center - new Vector2(projectile.localAI[0], projectile.localAI[1]);
                        if (distance != Vector2.Zero && distance.Length() >= 300f)
                        {
                            projectile.velocity = distance.RotatedBy(Math.PI / 2);
                            projectile.velocity.Normalize();
                            projectile.velocity *= 8f;
                        }
                    }
                    break;

                default:
                    break;
            }

            if (stormTimer > 0)
            {
                stormTimer--;

                int dustId = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.GoldFlame, projectile.velocity.X, projectile.velocity.Y, 100, default, 1.5f);
                Main.dust[dustId].noGravity = true;
            }

            if (ChilledProj)
            {
                int dustId = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Snow, projectile.velocity.X, projectile.velocity.Y, 100, default, 1f);
                Main.dust[dustId].noGravity = true;

                projectile.position -= projectile.velocity * 0.5f;
            }

            if (projectile.bobber && modPlayer.FishSoul1)
            {
                //ai0 = in water, localai1 = counter up to catching an item
                if (projectile.wet && projectile.ai[0] == 0 && projectile.ai[1] == 0 && projectile.localAI[1] < 655)
                    projectile.localAI[1] = 655; //quick catch. not 660 and up, may break things
            }

            if (ProjectileID.Sets.IsAWhip[projectile.type] && projectile.owner == Main.myPlayer
                && Main.player[projectile.owner].HasEffect<TikiEffect>())
            {
                foreach (Projectile p in Main.projectile.Where(p => p.active && p.friendly && !p.hostile && p.owner == Main.myPlayer
                    && !ProjectileID.Sets.IsAWhip[p.type]
                    && projectile.Colliding(projectile.Hitbox, p.Hitbox)))
                {
                    if (!p.FargoSouls().TikiTagged)
                    {
                        SoundEngine.PlaySound(SoundID.Item147 with { Pitch = 1, Volume = 0.35f, MaxInstances = 1 }, p.Center);
                        for (int i = 0; i < 8; i++)
                        {
                            int dust = Dust.NewDust(new Vector2(projectile.position.X - 2f, projectile.position.Y - 2f), projectile.width + 4, projectile.height + 4, DustID.JungleSpore, projectile.velocity.X * 0.4f, projectile.velocity.Y * 0.4f, 100, Color.LimeGreen, .8f);
                            Main.dust[dust].noGravity = true;
                            Main.dust[dust].velocity *= 1.8f;
                            Dust expr_1CCF_cp_0 = Main.dust[dust];
                            expr_1CCF_cp_0.velocity.Y -= 0.5f;
                            if (Main.rand.NextBool(4))
                            {
                                Main.dust[dust].noGravity = false;
                                Main.dust[dust].scale *= 0.5f;
                            }
                        }
                    }
                    p.FargoSouls().TikiTagged = true;
                }
            }

            if (TikiTagged)
            {
                //dust
                if (Main.rand.NextBool(10))
                {
                    int dust = Dust.NewDust(new Vector2(projectile.position.X - 2f, projectile.position.Y - 2f), projectile.width + 4, projectile.height + 4, DustID.JungleSpore, projectile.velocity.X * 0.4f, projectile.velocity.Y * 0.4f, 100, Color.LimeGreen, .8f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Dust expr_1CCF_cp_0 = Main.dust[dust];
                    expr_1CCF_cp_0.velocity.Y -= 0.5f;
                    if (Main.rand.NextBool(4))
                    {
                        Main.dust[dust].noGravity = false;
                        Main.dust[dust].scale *= 0.5f;
                    }
                }
            }
        }

        public static int[] FancySwings => [
            ProjectileID.Excalibur,
            ProjectileID.TrueExcalibur,
            ProjectileID.TerraBlade2,
            ProjectileID.TheHorsemansBlade
        ];

        public override void PostAI(Projectile projectile)
        {
            Player player = Main.player[projectile.owner];
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            if (DamageCap > 0 && projectile.damage > DamageCap)
                projectile.damage = DamageCap;

            if (projectile.whoAmI == player.heldProj
                || projectile.aiStyle == ProjAIStyleID.HeldProjectile
                || projectile.type == ProjectileID.LastPrismLaser)
            {
                DeletionImmuneRank = 2;
                TimeFreezeImmune = true;
                IsAHeldProj = true;
                if (TungstenScale == 1)
                    ModifyProjectileSize(projectile, player, null);
                /*
                if (player.HasEffect<TungstenEffect>() && TungstenScale == 1)
                    TungstenEffect.TungstenIncreaseProjSize(projectile, modPlayer, null);
                */

                if (player.HeldItem.IsWeapon())
                {
                    modPlayer.WeaponUseTimer = Math.Max(modPlayer.WeaponUseTimer, 2);

                    modPlayer.TryAdditionalAttacks(projectile.damage, projectile.DamageType);
                    player.AccessoryEffects().TryAdditionalAttacks(projectile.damage, projectile.DamageType);

                    //because the bow refuses to acknowledge changes in attack speed after initial spawning
                    if (projectile.type == ProjectileID.DD2PhoenixBow && player.HasEffect<MythrilEffect>() && modPlayer.Player.FargoSouls().MythrilTimer > -60 && counter > 60)
                        projectile.Kill();
                }

                //bandaid for how capping proj array lets phantasm spawn and fire arrows every tick
                //reusedelay scales down to 0 after first shot
                if (projectile.type == ProjectileID.Phantasm)
                {
                    player.reuseDelay = Math.Max(0, 20 - counter);
                }
            }

            bool canAdaSplit = AdamantiteEffect.CanBeAffected(projectile, player);
            if (player.HasEffect<AdamantiteEffect>() && canAdaSplit && !Adamantite)
                AdamantiteEffect.AdamantiteSplit(projectile, modPlayer, 1 + (int)modPlayer.AdamantiteSpread);

            //graze
            if (projectile.hostile && projectile.damage > 0 && projectile.aiStyle != ProjAIStyleID.FallingTile && --GrazeCD < 0)
            {
                GrazeCD = 6; //don't check per tick ech

                if (Main.LocalPlayer.active && !Main.LocalPlayer.dead)
                {
                    FargoSoulsPlayer fargoPlayer = Main.LocalPlayer.FargoSouls();
                    if (fargoPlayer.Graze && !Main.LocalPlayer.immune && Main.LocalPlayer.hurtCooldowns[0] <= 0 && Main.LocalPlayer.hurtCooldowns[1] <= 0)
                    {
                        if (ProjectileLoader.CanDamage(projectile) != false && ProjectileLoader.CanHitPlayer(projectile, Main.LocalPlayer) && GrazeCheck(projectile))
                        {
                            GrazeCD = 30 * projectile.MaxUpdates;

                            if (fargoPlayer.NekomiSet)
                                NekomiHood.OnGraze(fargoPlayer, projectile.damage * 4);
                            if (fargoPlayer.DeviGraze)
                                SparklingAdoration.OnGraze(fargoPlayer, projectile.damage * 4);
                            if (fargoPlayer.CirnoGraze)
                                IceQueensCrown.OnGraze(fargoPlayer, projectile.damage * 4);
                        }
                    }
                }
            }

            if (HuntressProj == 1 && projectile.Center.Distance(Main.player[projectile.owner].Center) > 1500) //goes off screen without hitting anything
            {
                modPlayer.HuntressStage = 0;
                //Main.NewText("MISS");
                HuntressProj = -1;
                //sound effect
            }

            if (CirnoBurst > 0)
            {
                CirnoBurst -= 1f / projectile.MaxUpdates;
                if (CirnoBurst <= 0 && Main.myPlayer == projectile.owner)
                {
                    Vector2 vel = Main.rand.NextVector2Unit() * Math.Max(projectile.velocity.Length(), 8f);
                    Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, vel, ModContent.ProjectileType<FrostShardFriendly>(), projectile.damage, 2f, projectile.owner);
                }
                projectile.Kill();
            }

            if (projectile.owner == player.whoAmI)
            {
                if (FancySwings.Contains(projectile.type))
                {
                    if (player.FargoSouls().swingDirection != player.direction)
                    {
                        projectile.ai[0] = -player.direction;
                    }

                    float rotation = -90;
                    if (projectile.ai[0] == -1 && player.direction == -1)
                    {
                        rotation = -180;
                    }
                    else if (projectile.ai[0] == -1 && player.direction == 1)
                    {
                        rotation = 0;
                    }
                    projectile.rotation = player.itemRotation + MathHelper.ToRadians(rotation);
                }
            }
        }
        public override bool? Colliding(Projectile projectile, Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projectile.scale > 1)
            {
                if (projectile.type == ProjectileID.MonkStaffT1 || projectile.type == ProjectileID.MonkStaffT3)
                {
                    float f5 = projectile.rotation - (float)Math.PI / 4f * (float)Math.Sign(projectile.velocity.X);
                    float collisionPoint7 = 0f;
                    float num20 = 50f;
                    if (projectile.type == ProjectileID.MonkStaffT1)
                    {
                        num20 = 65f;
                    }
                    if (projectile.type == ProjectileID.MonkStaffT3)
                    {
                        num20 = 110f;
                    }
                    num20 *= projectile.scale;
                    if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center + f5.ToRotationVector2() * (0f - num20), projectile.Center + f5.ToRotationVector2() * num20, 23f * projectile.scale, ref collisionPoint7))
                    {
                        return true;
                    }
                    return false;
                }
            }

            return base.Colliding(projectile, projHitbox, targetHitbox);
        }

        public override bool TileCollideStyle(Projectile projectile, ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            if (projectile.type == ProjectileID.SmokeBomb)
            {
                fallThrough = false;
            }

            if (TungstenScale != 1)
            {
                width = (int)(width / TungstenScale);
                height = (int)(height / TungstenScale);
            }

            return base.TileCollideStyle(projectile, ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        public override bool? CanDamage(Projectile projectile)
        {
            if (!canHurt)
                return false;
            if (TimeFrozen > 0 && counter > TimeFreezeMoveDuration * projectile.MaxUpdates)
                return false;

            return base.CanDamage(projectile);
        }

        private static List<int> MiningExplosives =
        [
            ProjectileID.Bomb,
            ProjectileID.BombFish,
            ProjectileID.BouncyBomb,
            ProjectileID.BouncyDynamite,
            ProjectileID.DirtBomb,
            ProjectileID.DryBomb,
            ProjectileID.Dynamite,
            ProjectileID.HoneyBomb,
            ProjectileID.LavaBomb,
            ProjectileID.ScarabBomb,
            ProjectileID.StickyBomb,
            ProjectileID.DirtStickyBomb,
            ProjectileID.StickyDynamite,
            ProjectileID.WetBomb,
            ProjectileID.Explosives,
            ModContent.Find<ModProjectile>("Fargowiltas", "ShurikenProj").Type
        ];

        public override bool CanHitPlayer(Projectile projectile, Player target)
        {
            if (projectile.friendly && target.FargoSouls().MiningImmunity && MiningExplosives.Contains(projectile.type))
            {
                return false;
            }
            return base.CanHitPlayer(projectile, target);
        }

        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (!projectile.owner.IsWithinBounds(Main.maxPlayers))
                return;
            Player player = Main.player[projectile.owner];
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            if (stormTimer > 0)
                modifiers.FinalDamage *= modPlayer.ForceEffect<ForbiddenEnchant>() ? 1.4f : 1.2f;

            if (TungstenScale != 1 && projectile.type == ProjectileID.PiercingStarlight)
                modifiers.FinalDamage *= 0.75f;

            if (TikiTagged)
            {
                modifiers.FinalDamage *= modPlayer.ForceEffect<TikiEnchant>() ? 1.4f : 1.2f;
                TikiTagged = false;
            }
                

            if (player.HasEffect<NinjaDamageEffect>() && player.ActualClassCrit(projectile.DamageType) > 0 && projectile.CritChance > 0)
            {
                float maxIncrease = modPlayer.ForceEffect<NinjaEnchant>() ? 0.4f : 0.2f;
                float increase = maxIncrease * Math.Clamp((projectile.extraUpdates + 1) * projectile.velocity.Length() / 40f, 0, 1);
                if (Main.rand.NextFloat() < increase)
                    modifiers.SetCrit();

            }

            if (projectile.type == ProjectileID.MythrilHalberd)
            {
                if (Main.player[projectile.owner].Eternity().MythrilHalberdTimer >= 120)
                {
                    modifiers.SourceDamage *= 8 * modPlayer.AttackSpeed;
                }
            }
            /*
            int AccountForDefenseShred(int modifier)
            {
                int defenseIgnored = projectile.ArmorPenetration;
                if (target.ichor)
                    defenseIgnored += 15;
                if (target.betsysCurse)
                    defenseIgnored += 40;

                int actualDefenseIgnored = Math.Min(defenseIgnored, target.defense);
                int effectOnDamage = actualDefenseIgnored / 2;

                return effectOnDamage / modifier;
            }

            if (AdamModifier != 0)
            {
               //modifiers.FinalDamage /= AdamModifier;
                // TODO: maybe use defense here
                //modifiers.FinalDamage.Flat -= AccountForDefenseShred(AdamModifier);
            }
            */

            if (noInteractionWithNPCImmunityFrames)
                tempIframe = target.immune[projectile.owner];

            if (projectile.type == ProjectileID.SharpTears && !projectile.usesLocalNPCImmunity && projectile.usesIDStaticNPCImmunity && projectile.idStaticNPCHitCooldown == 60 && noInteractionWithNPCImmunityFrames)
            {
                modifiers.SetCrit();
            }
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (noInteractionWithNPCImmunityFrames)
                target.immune[projectile.owner] = tempIframe;

            if (Main.player[projectile.owner].HasEffect<NinjaDamageEffect>())
            {
                const float maxKnockbackMult = 2f;
                hit.Knockback *= (maxKnockbackMult * Math.Min((projectile.extraUpdates + 1) * projectile.velocity.Length() / 40, 1f));

            }
            if (projectile.type == ProjectileID.SharpTears && !projectile.usesLocalNPCImmunity && projectile.usesIDStaticNPCImmunity && projectile.idStaticNPCHitCooldown == 60 && noInteractionWithNPCImmunityFrames)
            {
                target.AddBuff(ModContent.BuffType<AnticoagulationBuff>(), 360);

                if (FargoSoulsUtil.NPCExists(target.realLife) != null)
                {
                    foreach (NPC n in Main.npc.Where(n => n.active && (n.realLife == target.realLife || n.whoAmI == target.realLife) && n.whoAmI != target.whoAmI))
                    {
                        Projectile.perIDStaticNPCImmunity[projectile.type][n.whoAmI] = Main.GameUpdateCount + (uint)projectile.idStaticNPCHitCooldown;
                    }
                }
            }

            //if (projectile.type == ProjectileID.SeedlerNut && projectile.owner.IsWithinBounds(Main.maxPlayers) && EnchantmentProj && Main.player[projectile.owner].HasEffect<TimberEffect>())
            //{
            //    ShadewoodEffect.ShadewoodProc(Main.player[projectile.owner], target, projectile);
            //}

            if (FrostFreeze)
            {
                target.AddBuff(BuffID.Frostburn2, 360);

                FargoSoulsGlobalNPC globalNPC = target.FargoSouls();
               
                int debuff = ModContent.BuffType<FrozenBuff>();
                int duration = target.HasBuff(debuff) ? 5 : 15;

                NPC head = FargoSoulsUtil.NPCExists(target.realLife);
                if (head != null)
                {
                    head.AddBuff(debuff, duration);

                    foreach (NPC n in Main.npc.Where(n => n.active && n.realLife == head.whoAmI && n.whoAmI != head.whoAmI))
                        n.AddBuff(debuff, duration);
                }
                else
                {
                    target.AddBuff(debuff, duration);
                }
            }

            Player player = Main.player[projectile.owner];
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            if (projectile.type == ProjectileID.IceBlock && Main.player[projectile.owner].HasEffect<FrigidGemstoneKeyEffect>())
            {
                target.AddBuff(BuffID.Frostburn, 360);
            }

            if (projectile.type == ProjectileID.CrystalLeafShot && player.HasEffect<NatureEffect>() && player.HasEffect<ShroomiteShroomEffect>())
            {
                ShroomiteShroomEffect.SpawnShrooms(player, target, (int)(damageDone * 1f));
            }
        }

        void ReduceIFrames(Projectile projectile, NPC target, int iframeModifier)
        {
            if (projectile.maxPenetrate != 1 && !projectile.usesLocalNPCImmunity && !IsAHeldProj)
            {
                //biased towards rounding down, making it a slight dps increase for compatible weapons
                double RoundReduce(float iframes)
                {
                    double newIframes = Math.Round(iframes / iframeModifier, 0, Main.rand.NextBool(3) ? MidpointRounding.AwayFromZero : MidpointRounding.ToZero);
                    if (newIframes < 1)
                        newIframes = 1;
                    return newIframes;
                }

                if (projectile.usesIDStaticNPCImmunity)
                {
                    if (projectile.idStaticNPCHitCooldown > 1)
                        Projectile.perIDStaticNPCImmunity[projectile.type][target.whoAmI] = Main.GameUpdateCount + (uint)RoundReduce(projectile.idStaticNPCHitCooldown);
                }
                else if (!noInteractionWithNPCImmunityFrames && target.immune[projectile.owner] > 1)
                {
                    target.immune[projectile.owner] = (int)RoundReduce(target.immune[projectile.owner]);
                }
            }
        }

        public override void ModifyHitPlayer(Projectile projectile, Player target, ref Player.HurtModifiers modifiers)
        {
            NPC sourceNPC = projectile.GetSourceNPC();
            if (sourceNPC is not null && sourceNPC.FargoSouls().BloodDrinker)
            {
                modifiers.FinalDamage *= 1.3f;
                // damage = (int)Math.Round(damage * 1.3);
            }

            if (squeakyToy)
            {
                modifiers.SetMaxDamage(1);
                FargoSoulsPlayer.Squeak(target.Center);
            }
        }

        public override void OnKill(Projectile projectile, int timeLeft)
        {
            Player player = Main.player[projectile.owner];
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            if (HuntressProj == 1) //dying without hitting anything
            {
                modPlayer.HuntressStage = 0;
                //Main.NewText("MISS");
                //sound effect
            }
        }

        //        public override void UseGrapple(Player player, ref int type)
        //        {
        //            FargoSoulsPlayer modPlayer = player.FargoSouls();

        //            if (modPlayer.JungleEnchant)
        //            {
        //                modPlayer.CanJungleJump = true;
        //            }
        //        }

        public override void GrapplePullSpeed(Projectile projectile, Player player, ref float speed)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            if (player.HasEffect<MahoganyEffect>())
            {
                float multiplier = 1.5f;

                if (modPlayer.ForceEffect<RichMahoganyEnchant>())
                {
                    multiplier = 2.5f;
                }

                speed *= multiplier;
            }
        }

        public override void GrappleRetreatSpeed(Projectile projectile, Player player, ref float speed)
        {
            if (player.HasEffect<MahoganyEffect>())
            {
                float multiplier = 3f;
                speed *= multiplier;
            }
        }

        public override void PostDraw(Projectile projectile, Color lightColor)
        {
            if (projectile.type == ProjectileID.RuneBlast)
            {
                Texture2D texture2D13 = ModContent.Request<Texture2D>("FargowiltasSouls/Content/Projectiles/RuneBlast", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                int num156 = texture2D13.Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
                int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
                Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
                Vector2 origin2 = rectangle.Size() / 2f;
                SpriteEffects effects = SpriteEffects.None;
                Main.EntitySpriteDraw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), new Color(255, 255, 255), projectile.rotation, origin2, projectile.scale, effects, 0);
                Main.EntitySpriteDraw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), new Color(255, 255, 255, 0), projectile.rotation, origin2, projectile.scale, effects, 0);
            }
        }
    }
}
