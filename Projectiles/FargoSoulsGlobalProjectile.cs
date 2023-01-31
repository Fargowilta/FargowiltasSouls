using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.Buffs.Souls;
using FargowiltasSouls.Items.Accessories.Enchantments;
using FargowiltasSouls.Items.Accessories.Forces;
using FargowiltasSouls.Items.Accessories.Masomode;
using FargowiltasSouls.Items.Accessories.Souls;
using FargowiltasSouls.Items.Weapons.SwarmDrops;
using FargowiltasSouls.NPCs;
using FargowiltasSouls.Projectiles.BossWeapons;
using FargowiltasSouls.Projectiles.Minions;
using FargowiltasSouls.Projectiles.Souls;
using FargowiltasSouls.Toggler;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles
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
        // private int numSplits = 1;
        public int stormTimer;
        public float TungstenScale = 1;
        public int AdamModifier;
        public bool tikiMinion;
        public int tikiTimer;
        public int shroomiteMushroomCD;
        private int spookyCD;
        public bool FrostFreeze;
        //        public bool SuperBee;
        public bool ChilledProj;
        public int ChilledTimer;
        public int NinjaSpeedup;

        public int HuntressProj = -1; // -1 = non weapon proj, doesnt matter if it hits
        //1 = marked as weapon proj
        //2 = has successfully hit an enemy

        public Func<Projectile, bool> GrazeCheck = projectile =>
            projectile.Distance(Main.LocalPlayer.Center) < Math.Min(projectile.width, projectile.height) / 2 + Player.defaultHeight + Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().GrazeRadius
            && (projectile.ModProjectile == null ? true : projectile.ModProjectile.CanDamage() != false)
            && Collision.CanHit(projectile.Center, 0, 0, Main.LocalPlayer.Center, 0, 0);

        private bool firstTick = true;
        private bool squeakyToy = false;
        public const int TimeFreezeMoveDuration = 10;
        public int TimeFrozen = 0;
        public bool TimeFreezeImmune;
        public int DeletionImmuneRank;

        public bool canHurt = true;

        public bool noInteractionWithNPCImmunityFrames;
        private int tempIframe;

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

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            //not doing this causes player array index error during worldgen in some cases maybe??
            if (projectile.owner < 0 || projectile.owner >= Main.maxPlayers)
                return;

            Player player = Main.player[projectile.owner];
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            if (projectile.friendly && FargoSoulsUtil.IsSummonDamage(projectile, true, false))
            {
                //projs shot by tiki-buffed minions will also inherit the tiki buff
                if (source is EntitySource_Parent parent && parent.Entity is Projectile sourceProj 
                    && FargoSoulsUtil.IsSummonDamage(sourceProj, true, false)
                    && sourceProj.GetGlobalProjectile<FargoSoulsGlobalProjectile>().tikiMinion)
                {
                    tikiMinion = true;
                    tikiTimer = sourceProj.GetGlobalProjectile<FargoSoulsGlobalProjectile>().tikiTimer;
                }
            }

            switch (projectile.type)
            {
                case ProjectileID.SpiritHeal:
                    if (modPlayer.SpectreEnchantActive && !modPlayer.TerrariaSoul)
                    {
                        projectile.extraUpdates = 1;
                        projectile.timeLeft = 180 * projectile.MaxUpdates;
                    }
                    break;

                case ProjectileID.DD2ExplosiveTrapT3Explosion:
                    {
                        if (projectile.damage > 0 && source is EntitySource_Parent parent && parent.Entity is NPC npc && npc.active
                            && (npc.type == ModContent.NPCType<NPCs.Challengers.TrojanSquirrel>() || npc.type == ModContent.NPCType<NPCs.Champions.TimberChampion>()))
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
                            && npc.type == ModContent.NPCType<NPCs.Champions.ShadowChampion>())
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

                            projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().CanSplit = false;
                            projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().noInteractionWithNPCImmunityFrames = true;

                            if (ModLoader.TryGetMod("Fargowiltas", out Mod fargo))
                                fargo.Call("LowRenderProj", projectile);
                        }
                    }
                    break;

                case ProjectileID.DesertDjinnCurse:
                    {
                        if (projectile.damage > 0 && source is EntitySource_Parent parent && parent.Entity is NPC npc && npc.active && npc.type == ModContent.NPCType<NPCs.Champions.ShadowChampion>())
                            projectile.damage = FargoSoulsUtil.ScaledProjectileDamage(npc.damage);
                    }
                    break;

                case ProjectileID.SandnadoHostile:
                    {
                        if (projectile.damage > 0 && source is EntitySource_Parent parent && parent.Entity is NPC npc && npc.active)
                        {
                            if (npc.type == ModContent.NPCType<NPCs.DeviBoss.DeviBoss>())
                            {
                                projectile.damage = FargoSoulsUtil.ScaledProjectileDamage(npc.damage);
                                if (npc.ai[0] == 5)
                                    projectile.timeLeft = Math.Min(projectile.timeLeft, 360 + 90 - (int)npc.ai[1]);
                                else
                                    projectile.timeLeft = 90;
                            }
                            else if (npc.type == ModContent.NPCType<NPCs.Champions.ShadowChampion>())
                            {
                                projectile.damage = FargoSoulsUtil.ScaledProjectileDamage(npc.damage);
                            }
                        }
                    }
                    break;

                default:
                    break;
            }

            if (modPlayer.TungstenEnchantItem != null && player.GetToggleValue("TungstenProj"))
            {
                TungstenEnchant.TungstenIncreaseProjSize(projectile, modPlayer, source);
            }

            if (modPlayer.HuntressEnchantActive && player.GetToggleValue("Huntress")
                && FargoSoulsUtil.IsProjSourceItemUseReal(projectile, source)
                && projectile.damage > 0 && projectile.friendly && !projectile.hostile && !projectile.trap
                && projectile.DamageType != DamageClass.Default
                && !ProjectileID.Sets.CultistIsResistantTo[projectile.type]
                && !FargoSoulsUtil.IsSummonDamage(projectile, true, false))
            {
                HuntressProj = 1;
            }

            if (modPlayer.AdamantiteEnchantItem != null && player.GetToggleValue("Adamantite")
                && FargoSoulsUtil.OnSpawnEnchCanAffectProjectile(projectile, false)
                && CanSplit && Array.IndexOf(noSplit, projectile.type) <= -1)
            {
                if (projectile.owner == Main.myPlayer
                    && (FargoSoulsUtil.IsProjSourceItemUseReal(projectile, source)
                    || (source is EntitySource_Parent parent && parent.Entity is Projectile sourceProj && (sourceProj.minion || sourceProj.sentry || (ProjectileID.Sets.IsAWhip[sourceProj.type] && !ProjectileID.Sets.IsAWhip[projectile.type])))))
                {
                    AdamantiteEnchant.AdamantiteSplit(projectile, modPlayer);
                }

                AdamModifier = modPlayer.EarthForce ? 3 : 2;

                projectile.ArmorPenetration += projectile.damage / 2;
            }

            if (projectile.bobber && CanSplit && source is EntitySource_ItemUse)
            {
                if (player.whoAmI == Main.myPlayer && modPlayer.FishSoul2)
                    SplitProj(projectile, 11, MathHelper.Pi / 3, 1);
            }
        }

        public static int[] noSplit => new int[] {
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
        };

        public override bool PreAI(Projectile projectile)
        {
            bool retVal = true;
            Player player = Main.player[projectile.owner];
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            counter++;

            if (spookyCD > 0)
            {
                spookyCD--;
            }

            if (projectile.owner == Main.myPlayer)
            {
                //reset tungsten size
                if (TungstenScale != 1 && (modPlayer.TungstenEnchantItem == null || !player.GetToggleValue("TungstenProj")))
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
                if (modPlayer.MahoganyEnchantItem != null && player.GetToggleValue("Mahogany", false) && projectile.aiStyle == 7)
                {
                    RichMahoganyEnchant.MahoganyHookAI(projectile, modPlayer);
                }

                if (!projectile.hostile && !projectile.trap && !projectile.npcProj)
                {
                    if (modPlayer.Jammed && projectile.CountsAsClass(DamageClass.Ranged) && projectile.type != ProjectileID.ConfettiGun)
                    {
                        Projectile.NewProjectile(Terraria.Entity.InheritSource(projectile), projectile.Center, projectile.velocity, ProjectileID.ConfettiGun, 0, 0f, projectile.owner);
                        projectile.active = false;
                    }

                    if (modPlayer.Atrophied && projectile.CountsAsClass(DamageClass.Throwing))
                    {
                        projectile.damage = 0;
                        projectile.Kill();
                    }

                    if (modPlayer.ShroomEnchantActive && player.GetToggleValue("ShroomiteShroom") && projectile.damage > 0 /*&& !townNPCProj*/ && projectile.velocity.Length() > 1 && projectile.minionSlots == 0 && projectile.type != ModContent.ProjectileType<ShroomiteShroom>() && player.ownedProjectileCounts[ModContent.ProjectileType<ShroomiteShroom>()] < 50)
                    {
                        if (shroomiteMushroomCD >= 15)
                        {
                            shroomiteMushroomCD = 0;

                            if (player.stealth == 0 || modPlayer.NatureForce || modPlayer.WizardEnchantActive)
                            {
                                shroomiteMushroomCD = 10;
                            }

                            Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, projectile.velocity, ModContent.ProjectileType<ShroomiteShroom>(), projectile.damage / 2, projectile.knockBack / 2, projectile.owner);
                        }
                        shroomiteMushroomCD++;
                    }

                    if (modPlayer.SpookyEnchantActive && player.GetToggleValue("Spooky")
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

                                Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, velocity, ModContent.ProjectileType<SpookyScythe>(), projectile.damage, 2, projectile.owner);

                                SoundEngine.PlaySound(SoundID.Item62 with { Volume = 0.5f }, projectile.Center);

                                spookyCD = 30 + Main.rand.Next(player.maxMinions * 5);

                                if (modPlayer.ShadowForce || modPlayer.WizardEnchantActive)
                                {
                                    spookyCD -= 10;
                                }
                            }
                        }

                    }
                }

                if (modPlayer.Asocial && FargoSoulsUtil.IsSummonDamage(projectile, true, false))
                {
                    projectile.Kill();
                    retVal = false;
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

            // if (modPlayer.SnowEnchantActive && player.GetToggleValue("Snow") && projectile.hostile && !ChilledProj)
            // {
            //     ChilledProj = true;
            //     projectile.timeLeft *= 2;
            //     projectile.netUpdate = true;
            // }

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

                int deathTimer = 15;

                if (projectile.hostile)
                    deathTimer = 60;

                if (counter >= deathTimer)
                    projectile.Kill();
            }

            if (firstTick)
            {
                if (modPlayer.NinjaEnchantItem != null && FargoSoulsUtil.OnSpawnEnchCanAffectProjectile(projectile, true) && projectile.type != ProjectileID.WireKite)
                {
                    NinjaEnchant.NinjaSpeedSetup(modPlayer, projectile, this);
                }

                if (projectile.type == ProjectileID.ShadowBeamHostile)
                {
                    if (projectile.GetSourceNPC() is NPC sourceNPC && sourceNPC.type == ModContent.NPCType<NPCs.DeviBoss.DeviBoss>())
                    {
                        projectile.timeLeft = FargoSoulsWorld.MasochistModeReal ? 1200 : 420;
                    }
                }

                if (projectile.type == ProjectileID.DD2ExplosiveTrapT3Explosion && projectile.hostile)
                {
                    if (projectile.GetSourceNPC() is NPC sourceNPC && (sourceNPC.type == ModContent.NPCType<NPCs.Challengers.TrojanSquirrel>() || sourceNPC.type == ModContent.NPCType<NPCs.Champions.TimberChampion>()))
                    {
                        projectile.position = projectile.Bottom;
                        projectile.height = 16 * 6;
                        projectile.Bottom = projectile.position;
                    }
                }

                firstTick = false;
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
                            Texture2D texture2D13 = FargowiltasSouls.Instance.Assets.Request<Texture2D>("Projectiles/PlanteraTentacle", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                            Rectangle rectangle = new Rectangle(0, 0, texture2D13.Width, texture2D13.Height);
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
                default:
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

            List<Projectile> projList = new List<Projectile>();
            Projectile split;
            double spread = maxSpread / number;

            for (int i = 0; i < number / 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    int factor = (j == 0) ? 1 : -1;
                    split = FargoSoulsUtil.NewProjectileDirectSafe(projectile.GetSource_FromThis(), projectile.Center, projectile.velocity.RotatedBy(factor * spread * (i + 1)), projectile.type, (int)(projectile.damage * damageRatio), projectile.knockBack, projectile.owner, projectile.ai[0], projectile.ai[1]);
                    if (split != null)
                    {
                        split.localAI[0] = projectile.localAI[0];
                        split.localAI[1] = projectile.localAI[1];

                        split.friendly = projectile.friendly;
                        split.hostile = projectile.hostile;
                        split.timeLeft = projectile.timeLeft;
                        split.DamageType = projectile.DamageType;

                        //split.GetGlobalProjectile<FargoSoulsGlobalProjectile>().numSplits = projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().numSplits;
                        if (!allowMoreSplit)
                            split.GetGlobalProjectile<FargoSoulsGlobalProjectile>().CanSplit = false;
                        split.GetGlobalProjectile<FargoSoulsGlobalProjectile>().TungstenScale = projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().TungstenScale;

                        projList.Add(split);
                    }
                }
            }

            return projList;
        }

        private void KillPet(Projectile projectile, Player player, int buff, bool toggle, bool minion = false)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            if (player.FindBuffIndex(buff) == -1)
            {
                if (player.dead || !toggle || (minion ? !modPlayer.StardustEnchantActive : !modPlayer.VoidSoul) || (!modPlayer.PetsActive && !minion))
                {
                    projectile.Kill();
                }
            }
        }

        public override void AI(Projectile projectile)
        {
            Player player = Main.player[projectile.owner];
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            switch (projectile.type)
            {
                #region pets

                case ProjectileID.StardustGuardian:
                    KillPet(projectile, player, BuffID.StardustGuardianMinion, player.GetToggleValue("Stardust"), true);
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

                    //            Vector2 spawnPos = projectile.Center + 16f * projectile.DirectionTo(Main.npc[target].Center);

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

                int dustId = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.GoldFlame, projectile.velocity.X, projectile.velocity.Y, 100, default(Color), 1.5f);
                Main.dust[dustId].noGravity = true;
            }

            if (ChilledProj)
            {
                int dustId = Dust.NewDust(projectile.position, projectile.width, projectile.height, 76, projectile.velocity.X, projectile.velocity.Y, 100, default(Color), 1f);
                Main.dust[dustId].noGravity = true;

                projectile.position -= projectile.velocity * 0.5f;
            }

            if (NinjaSpeedup > 0)
            {
                projectile.extraUpdates = Math.Max(projectile.extraUpdates, NinjaSpeedup);

                if (projectile.owner == Main.myPlayer && !(modPlayer.NinjaEnchantItem != null && player.GetToggleValue("NinjaSpeed")))
                    projectile.Kill();
            }

            if (projectile.bobber && modPlayer.FishSoul1)
            {
                //ai0 = in water, localai1 = counter up to catching an item
                if (projectile.wet && projectile.ai[0] == 0 && projectile.ai[1] == 0 && projectile.localAI[1] < 655)
                    projectile.localAI[1] = 655; //quick catch. not 660 and up, may break things
            }

            if (ProjectileID.Sets.IsAWhip[projectile.type] && projectile.owner == Main.myPlayer
                && Main.player[projectile.owner].GetModPlayer<FargoSoulsPlayer>().TikiEnchantActive)
            {
                foreach (Projectile p in Main.projectile.Where(p => p.active && !p.hostile && p.owner == Main.myPlayer
                    && FargoSoulsUtil.IsSummonDamage(p, true, false)
                    && !ProjectileID.Sets.IsAWhip[p.type]
                    && projectile.Colliding(projectile.Hitbox, p.Hitbox)))
                {
                    p.GetGlobalProjectile<FargoSoulsGlobalProjectile>().tikiMinion = true;
                    p.GetGlobalProjectile<FargoSoulsGlobalProjectile>().tikiTimer = 60 * p.MaxUpdates;
                }
            }

            if (tikiMinion)
            {
                if (projectile.type != ProjectileID.UFOLaser) //avoid movement glitches
                {
                    //move faster
                    projectile.position.X += projectile.velocity.X;
                    if (!projectile.tileCollide || projectile.velocity.Y < 0 || projectile.shouldFallThrough)
                        projectile.position.Y += projectile.velocity.Y;
                }

                if (tikiTimer > 0)
                    tikiTimer--;
                else
                    tikiMinion = false;

                //dust
                if (Main.rand.NextBool(2))
                {
                    int dust = Dust.NewDust(new Vector2(projectile.position.X - 2f, projectile.position.Y - 2f), projectile.width + 4, projectile.height + 4, 44, projectile.velocity.X * 0.4f, projectile.velocity.Y * 0.4f, 100, Color.LimeGreen, .8f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Dust expr_1CCF_cp_0 = Main.dust[dust];
                    expr_1CCF_cp_0.velocity.Y = expr_1CCF_cp_0.velocity.Y - 0.5f;
                    if (Main.rand.NextBool(4))
                    {
                        Main.dust[dust].noGravity = false;
                        Main.dust[dust].scale *= 0.5f;
                    }
                }
            }
        }

        public override void PostAI(Projectile projectile)
        {
            Player player = Main.player[projectile.owner];
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            if (projectile.whoAmI == player.heldProj
                || projectile.aiStyle == ProjAIStyleID.HeldProjectile
                || projectile.type == ProjectileID.LastPrismLaser)
            {
                DeletionImmuneRank = 2;
				TimeFreezeImmune = true;

                projectile.CritChance = player.GetWeaponCrit(player.HeldItem);

                if (player.HeldItem.damage > 0 && player.HeldItem.pick == 0)
                {
                    modPlayer.WeaponUseTimer = Math.Max(modPlayer.WeaponUseTimer, 2);

                    modPlayer.TryAdditionalAttacks(projectile.damage, projectile.DamageType);

                    //because the bow refuses to acknowledge changes in attack speed after initial spawning
                    if (projectile.type == ProjectileID.DD2PhoenixBow && modPlayer.MythrilEnchantItem != null && modPlayer.MythrilTimer > -60 && counter > 60)
                        projectile.Kill();
                }
				
				//bandaid for how capping proj array lets phantasm spawn and fire arrows every tick
				//reusedelay scales down to 0 after first shot
				if (projectile.type == ProjectileID.Phantasm)
				{
					player.reuseDelay = Math.Max(0, 20 - counter);
				}
            }

            //graze
            if (projectile.hostile && projectile.damage > 0 && --GrazeCD < 0)
            {
                GrazeCD = 6; //don't check per tick ech

                if (Main.LocalPlayer.active && !Main.LocalPlayer.dead)
                {
                    FargoSoulsPlayer fargoPlayer = Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>();
                    if (fargoPlayer.Graze && !Main.LocalPlayer.immune && Main.LocalPlayer.hurtCooldowns[0] <= 0 && Main.LocalPlayer.hurtCooldowns[1] <= 0)
                    {
                        if (ProjectileLoader.CanDamage(projectile) != false && ProjectileLoader.CanHitPlayer(projectile, Main.LocalPlayer) && GrazeCheck(projectile))
                        {
                            GrazeCD = 30 * projectile.MaxUpdates;

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

        public override bool CanHitPlayer(Projectile projectile, Player target)
        {
            return true;
        }

        public override void ModifyHitNPC(Projectile projectile, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            Player player = Main.player[projectile.owner];
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            if (AdamModifier != 0)
            {
                damage /= AdamModifier;
            }

            if (stormTimer > 0)
                damage = (int)(damage * (Main.player[projectile.owner].GetModPlayer<FargoSoulsPlayer>().SpiritForce ? 1.6 : 1.3));

            if (noInteractionWithNPCImmunityFrames)
                tempIframe = target.immune[projectile.owner];

            if (NinjaSpeedup > 0)
                damage /= 2;

            if (projectile.type == ProjectileID.SharpTears && !projectile.usesLocalNPCImmunity && projectile.usesIDStaticNPCImmunity && projectile.idStaticNPCHitCooldown == 60 && noInteractionWithNPCImmunityFrames)
            {
                crit = true;
            }

            if (tikiMinion)
            {
                crit = true;
            }
        }

        public override void OnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            if (noInteractionWithNPCImmunityFrames)
                target.immune[projectile.owner] = tempIframe;

            if (projectile.type == ProjectileID.SharpTears && !projectile.usesLocalNPCImmunity && projectile.usesIDStaticNPCImmunity && projectile.idStaticNPCHitCooldown == 60 && noInteractionWithNPCImmunityFrames)
            {
                target.AddBuff(ModContent.BuffType<Anticoagulation>(), 360);

                if (FargoSoulsUtil.NPCExists(target.realLife) != null)
                {
                    foreach (NPC n in Main.npc.Where(n => n.active && (n.realLife == target.realLife || n.whoAmI == target.realLife) && n.whoAmI != target.whoAmI))
                    {
                        Projectile.perIDStaticNPCImmunity[projectile.type][n.whoAmI] = Main.GameUpdateCount + (uint)projectile.idStaticNPCHitCooldown;
                    }
                }
            }

            if (FrostFreeze)
            {
                target.AddBuff(BuffID.Frostburn2, 360);

                FargoSoulsGlobalNPC globalNPC = target.GetGlobalNPC<FargoSoulsGlobalNPC>();

                int debuff = ModContent.BuffType<Frozen>();
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

			if (NinjaSpeedup > 0)
				ReduceIFrames(projectile, target, 2);
			
			if (AdamModifier != 0)
				ReduceIFrames(projectile, target, AdamModifier);

            if (projectile.type == ProjectileID.IceBlock && Main.player[projectile.owner].GetModPlayer<FargoSoulsPlayer>().FrigidGemstoneItem != null)
            {
				target.AddBuff(BuffID.Frostburn, 360);
            }
        }
		
		void ReduceIFrames(Projectile projectile, NPC target, int iframeModifier)
		{
			if (projectile.maxPenetrate != 1 && !projectile.usesLocalNPCImmunity)
            {
                if (projectile.usesIDStaticNPCImmunity)
                {
                    if (projectile.idStaticNPCHitCooldown > 1)
                        Projectile.perIDStaticNPCImmunity[projectile.type][target.whoAmI] = Main.GameUpdateCount + (uint)(projectile.idStaticNPCHitCooldown / iframeModifier);
                }
                else if (!noInteractionWithNPCImmunityFrames && target.immune[projectile.owner] > 1)
                {
                    target.immune[projectile.owner] /= iframeModifier;
                }
            }
		}

        public override void ModifyHitPlayer(Projectile projectile, Player target, ref int damage, ref bool crit)
        {
            NPC sourceNPC = projectile.GetSourceNPC();
            if (sourceNPC is NPC && sourceNPC.GetGlobalNPC<FargoSoulsGlobalNPC>().BloodDrinker)
            {
                damage = (int)Math.Round(damage * 1.3);
            }

            if (squeakyToy)
            {
                damage = 1;
                target.GetModPlayer<FargoSoulsPlayer>().Squeak(target.Center);
            }
        }

        public override void Kill(Projectile projectile, int timeLeft)
        {
            Player player = Main.player[projectile.owner];
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            if (HuntressProj == 1) //dying without hitting anything
            {
                modPlayer.HuntressStage = 0;
                //Main.NewText("MISS");
                //sound effect
            }
        }

        //        public override void UseGrapple(Player player, ref int type)
        //        {
        //            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

        //            if (modPlayer.JungleEnchant)
        //            {
        //                modPlayer.CanJungleJump = true;
        //            }
        //        }

        public override void GrapplePullSpeed(Projectile projectile, Player player, ref float speed)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            if (modPlayer.MahoganyEnchantItem != null && player.GetToggleValue("Mahogany", false))
            {
                float multiplier = 1.5f;

                if (modPlayer.WoodForce || modPlayer.WizardEnchantActive)
                {
                    multiplier = 2.5f;
                }

                speed *= multiplier;
            }
        }

        public override void GrappleRetreatSpeed(Projectile projectile, Player player, ref float speed)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            if (modPlayer.MahoganyEnchantItem != null && player.GetToggleValue("Mahogany", false))
            {
                float multiplier = 3f;
                speed *= multiplier;
            }
        }

        public override void PostDraw(Projectile projectile, Color lightColor)
        {
            if (projectile.type == ProjectileID.RuneBlast)
            {
                Texture2D texture2D13 = FargowiltasSouls.Instance.Assets.Request<Texture2D>("Projectiles/RuneBlast", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                int num156 = texture2D13.Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
                int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
                Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
                Vector2 origin2 = rectangle.Size() / 2f;
                SpriteEffects effects = SpriteEffects.None;
                Main.EntitySpriteDraw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), new Color(255, 255, 255), projectile.rotation, origin2, projectile.scale, effects, 0);
                Main.EntitySpriteDraw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), new Color(255, 255, 255, 0), projectile.rotation, origin2, projectile.scale, effects, 0);
            }
        }
    }
}
