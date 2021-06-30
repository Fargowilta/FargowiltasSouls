using System;
using System.Collections.Generic;
using System.Linq;
using Fargowiltas.Projectiles;
using FargowiltasSouls.Buffs.Boss;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.Buffs.Souls;
using FargowiltasSouls.NPCs;
using FargowiltasSouls.NPCs.Champions;
using FargowiltasSouls.Projectiles.BossWeapons;
using FargowiltasSouls.Projectiles.Masomode;
using FargowiltasSouls.Projectiles.Minions;
using FargowiltasSouls.Projectiles.Souls;
using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles
{
    public class FargoGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity
        {
            get
            {
                return true;
            }
        }

        private bool townNPCProj = false;
        private int counter;
        private int rainbowCounter = 0;
        public bool Rainbow = false;
        public int GrazeCD;

        //enchants
        public bool CanSplit = true;
        private int numSplits = 1;
        private bool stormBoosted = false;
        private int stormTimer;
        private int preStormDamage;
        public bool TungstenProjectile = false;
        private bool tikiMinion = false;
        private int tikiTimer = 300;
        public int shroomiteMushroomCD = 0;
        private int spookyCD = 0;
        public bool FrostFreeze = false;
        public bool SuperBee = false;
        public bool ChilledProj = false;
        public int ChilledTimer;
        public bool SilverMinion;

        public Func<Projectile, bool> GrazeCheck = projectile =>
            projectile.Distance(Main.LocalPlayer.Center) < Math.Min(projectile.width, projectile.height) / 2 + Player.defaultHeight + 100
            && Collision.CanHit(projectile.Center, 0, 0, Main.LocalPlayer.Center, 0, 0)
            && (projectile.modProjectile == null ? true : projectile.modProjectile.CanDamage() && projectile.modProjectile.CanHitPlayer(Main.LocalPlayer));

        private bool firstTick = true;
        private bool squeakyToy = false;
        public const int TimeFreezeMoveDuration = 10;
        public int TimeFrozen = 0;
        public bool TimeFreezeImmune;
        public bool TimeFreezeCheck;
        public bool HasKillCooldown;
        public bool ImmuneToMutantBomb;
        public bool ImmuneToGuttedHeart;

        public bool masobool;

        public int ModProjID;

        public bool canHurt;

        public override void SetDefaults(Projectile projectile)
        {
            canHurt = true;

            switch (projectile.type)
            {
                case ProjectileID.StardustGuardian:
                case ProjectileID.StardustGuardianExplosion:
                    TimeFreezeImmune = true;
                    break;

                case ProjectileID.Sharknado:
                case ProjectileID.Cthulunado:
                case ProjectileID.PhantasmalDeathray:
                case ProjectileID.SaucerDeathray:
                case ProjectileID.SandnadoHostile:
                case ProjectileID.SandnadoHostileMark:
                    ImmuneToGuttedHeart = true;
                    break;

                case ProjectileID.BabySlime:
                    if (FargoSoulsWorld.MasochistMode)
                    {
                        projectile.minionSlots = 0.5f;
                        projectile.usesLocalNPCImmunity = true;
                        projectile.localNPCHitCooldown = 20;
                    }
                    break;

                case ProjectileID.Flamelash:
                case ProjectileID.MagicMissile:
                case ProjectileID.RainbowRodBullet:
                    if (FargoSoulsWorld.MasochistMode)
                        projectile.timeLeft = 300;
                    break;

                case ProjectileID.SpiritHeal:
                    if (FargoSoulsWorld.MasochistMode)
                        projectile.timeLeft = 240 * 4; //account for extraupdates
                    break;

                case ProjectileID.DD2BetsyFlameBreath:
                    if (FargoSoulsWorld.MasochistMode)
                    {
                        projectile.tileCollide = false;
                        projectile.penetrate = -1;
                        ImmuneToGuttedHeart = true;
                    }
                    break;

                case ProjectileID.ChlorophyteBullet:
                    if (FargoSoulsWorld.MasochistMode)
                    {
                        projectile.extraUpdates = 0;
                        projectile.timeLeft = 120;
                    }
                    break;

                case ProjectileID.CrystalBullet:
                case ProjectileID.HolyArrow:
                case ProjectileID.HallowStar:
                    if (FargoSoulsWorld.MasochistMode)
                        HasKillCooldown = true;
                    break;

                case ProjectileID.StardustCellMinionShot:
                case ProjectileID.MiniSharkron:
                    ProjectileID.Sets.MinionShot[projectile.type] = true; //can hurt maso ml
                    break;

                case ProjectileID.SaucerLaser:
                    if (FargoSoulsWorld.MasochistMode)
                        projectile.tileCollide = false;
                    break;

                case ProjectileID.CultistBossFireBall:
                    if (FargoSoulsWorld.MasochistMode && EModeGlobalNPC.BossIsAlive(ref EModeGlobalNPC.cultBoss, NPCID.CultistBoss)
                        && Main.npc[EModeGlobalNPC.cultBoss].life < Main.npc[EModeGlobalNPC.cultBoss].lifeMax / 2)
                    {
                        projectile.timeLeft = 1;
                        canHurt = false;
                    }
                    break;

                case ProjectileID.CultistBossFireBallClone:
                    if (FargoSoulsWorld.MasochistMode && EModeGlobalNPC.BossIsAlive(ref EModeGlobalNPC.cultBoss, NPCID.CultistBoss))
                    {
                        projectile.timeLeft = 1;
                        canHurt = false;
                    }
                    break;

                case ProjectileID.CursedFlameHostile:
                    /*if (FargoSoulsGlobalNPC.BossIsAlive(ref FargoSoulsGlobalNPC.wallBoss, NPCID.WallofFlesh))
                    {
                        projectile.tileCollide = false;
                        projectile.timeLeft = 120;
                        projectile.extraUpdates = 1;
                    }*/
                    break;

                case ProjectileID.SharknadoBolt:
                    if (FargoSoulsWorld.MasochistMode && EModeGlobalNPC.BossIsAlive(ref EModeGlobalNPC.fishBossEX, NPCID.DukeFishron))
                        projectile.extraUpdates++;
                    break;

                case ProjectileID.FlamesTrap:
                    if (FargoSoulsWorld.MasochistMode && NPC.golemBoss != -1
                        && Main.npc[NPC.golemBoss].active && Main.npc[NPC.golemBoss].type == NPCID.Golem)
                    {
                        projectile.tileCollide = false;
                    }
                    break;

                case ProjectileID.UnholyTridentHostile:
                    if (FargoSoulsWorld.MasochistMode)
                        projectile.extraUpdates++;
                    break;

                case ProjectileID.BulletSnowman:
                    if (FargoSoulsWorld.MasochistMode)
                    {
                        projectile.tileCollide = false;
                        projectile.timeLeft = 600;
                    }
                    break;

                case ProjectileID.CannonballHostile:
                    if (FargoSoulsWorld.MasochistMode)
                        projectile.scale = 2f;
                    break;

                case ProjectileID.EyeLaser:
                case ProjectileID.EyeFire:
                    if (FargoSoulsWorld.MasochistMode)
                        projectile.tileCollide = false;
                    break;

                case ProjectileID.SpiderEgg:
                case ProjectileID.BabySpider:
                case ProjectileID.FrostBlastFriendly:
                case ProjectileID.RainbowCrystalExplosion:
                case ProjectileID.MoonlordTurretLaser:
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
                    projectile.minion = true;
                    break;

                default:
                    break;
            }

            Fargowiltas.ModProjDict.TryGetValue(projectile.type, out ModProjID);
        }

        public static int[] noSplit = {
            ProjectileID.CrystalShard,
            ProjectileID.SandnadoFriendly,
            ProjectileID.LastPrism,
            ProjectileID.LastPrismLaser,
            ProjectileID.FlowerPetal,
            ProjectileID.BabySpider,
            ProjectileID.CrystalLeafShot,
            ProjectileID.Phantasm,
            ProjectileID.VortexBeater,
            ProjectileID.ChargedBlasterCannon,
            ProjectileID.MedusaHead,
            ProjectileID.WireKite,
            ProjectileID.DD2PhoenixBow,
            ProjectileID.LaserMachinegun,
            ProjectileID.Flairon
        };

        public override bool PreAI(Projectile projectile)
        {
            bool retVal = true;
            Player player = Main.player[Main.myPlayer];
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();
            counter++;

            if (spookyCD > 0)
            {
                spookyCD--;
            }

            if (projectile.owner == Main.myPlayer)
            {
                if (firstTick)
                {
                    townNPCProj = projectile.friendly && !projectile.hostile
                        && !projectile.melee && !projectile.ranged && !projectile.magic && !projectile.minion && !projectile.thrown
                        && !projectile.sentry && !ProjectileID.Sets.MinionShot[projectile.type] && !ProjectileID.Sets.SentryShot[projectile.type];
                    /*for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];

                        if (npc.active && npc.townNPC && projectile.Hitbox.Intersects(npc.Hitbox))
                        {
                            townNPCProj = true;
                        }
                    }*/
                    
                    if (modPlayer.SilverEnchant && IsMinionDamage(projectile) && player.GetToggleValue("SilverSpeed"))
                    {
                        SilverMinion = true;
                        projectile.extraUpdates++;
                    }

                    if (modPlayer.TungstenEnchant && player.GetToggleValue("TungstenProj") && (modPlayer.TungstenCD == 0 || projectile.aiStyle == 19 || projectile.type == ProjectileID.MonkStaffT2) && projectile.aiStyle != 99 && !townNPCProj && projectile.damage != 0 && !projectile.trap && !projectile.minion && projectile.type != ProjectileID.Arkhalis && projectile.type != ModContent.ProjectileType<BlenderOrbital>() && projectile.friendly)
                    {
                        projectile.position = projectile.Center;
                        projectile.scale *= 2f;
                        projectile.width *= 2;
                        projectile.height *= 2;
                        projectile.Center = projectile.position;
                        TungstenProjectile = true;
                        modPlayer.TungstenCD = 30;

                        if (modPlayer.Eternity)
                        {
                            modPlayer.TungstenCD = 0;
                        }
                        else if (modPlayer.TerraForce || modPlayer.WizardEnchant)
                        {
                            modPlayer.TungstenCD /= 2;
                        }
                    }

                    if (modPlayer.TikiEnchant)
                    {
                        if (modPlayer.TikiMinion && IsMinionDamage(projectile))
                        {
                            tikiMinion = true;

                            if (projectile.type != ModContent.ProjectileType<EaterBody>() && projectile.type != ProjectileID.StardustDragon2 && projectile.type != ProjectileID.StardustDragon3)
                            {
                                tikiMinion = true;
                                tikiTimer = 300;

                                if (modPlayer.SpiritForce || modPlayer.WizardEnchant)
                                {
                                    tikiTimer = 480;
                                }
                            }
                        } 
                    }

                    if (modPlayer.StardustEnchant && projectile.type == ProjectileID.StardustGuardianExplosion)
                    {
                        projectile.damage *= 5;
                    }

                    if (!townNPCProj && modPlayer.AdamantiteEnchant && modPlayer.AdamantiteCD == 0 && CanSplit && projectile.friendly && !projectile.hostile && projectile.damage > 0 && !projectile.minion && projectile.aiStyle != 19 && projectile.aiStyle != 99
                        && player.GetToggleValue("Adamantite") && Array.IndexOf(noSplit, projectile.type) <= -1
                        && !(projectile.type == ProjectileID.DD2BetsyArrow && projectile.ai[1] == -1))
                    {
                        modPlayer.AdamantiteCD = 60;

                        if (modPlayer.Eternity)
                        {
                            modPlayer.AdamantiteCD = 0;
                        }
                        else if (modPlayer.TerrariaSoul)
                        {
                            modPlayer.AdamantiteCD = 30;
                        }
                        else if (modPlayer.EarthForce || modPlayer.WizardEnchant)
                        {
                            modPlayer.AdamantiteCD = 45;
                        }

                        float damageRatio = 0.5f;

                        if (projectile.penetrate > 1)
                        {
                            damageRatio = 1;
                        }

                        SplitProj(projectile, 3, MathHelper.Pi / 16, damageRatio);
                    }

                    if (projectile.bobber && CanSplit)
                    {
                        /*if (modPlayer.FishSoul1)
                        {
                            SplitProj(projectile, 5);
                        }*/
                        if (player.whoAmI == Main.myPlayer && modPlayer.FishSoul2)
                        {
                            SplitProj(projectile, 11, MathHelper.Pi / 3, 1);
                        }
                    }

                    /*if (modPlayer.BeeEnchant && (projectile.type == ProjectileID.GiantBee || projectile.type == ProjectileID.Bee || projectile.type == ProjectileID.Wasp))
                    {
                        projectile.usesLocalNPCImmunity = true;
                        projectile.localNPCHitCooldown = 5;
                        projectile.penetrate *= 2;
                        projectile.timeLeft *= 2;
                        projectile.scale *= 2.5f;
                        //projectile.damage = (int)(projectile.damage * 1.5);
                        SuperBee = true;
                    }*/
                }

                if (TungstenProjectile && (!modPlayer.TungstenEnchant || !player.GetToggleValue("TungstenProj")))
                {
                    projectile.position = projectile.Center;
                    projectile.scale /= 2f;
                    projectile.width /= 2;
                    projectile.height /= 2;
                    projectile.Center = projectile.position;
                    TungstenProjectile = false;
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
                            if (player.HeldItem.type == mod.ItemType("Blender"))
                            {
                                projectile.localAI[0]++;
                                if(projectile.localAI[0] > 60)
                                {
                                    projectile.Kill();
                                    Main.PlaySound(SoundID.NPCKilled, (int)projectile.Center.X, (int)projectile.Center.Y, 11, 0.5f);
                                    int proj2 = mod.ProjectileType("BlenderProj3");
                                    Projectile.NewProjectile(new Vector2(projectile.Center.X, projectile.Center.Y), projectile.DirectionFrom(player.Center) * 8, proj2, projectile.damage, projectile.knockBack, projectile.owner);
                                }
                            }
                        }
                        break;
                }
                
                if (tikiMinion)
                {
                    projectile.alpha = 120;

                    //dust
                    if (Main.rand.Next(4) < 2)
                    {
                        int dust = Dust.NewDust(new Vector2(projectile.position.X - 2f, projectile.position.Y - 2f), projectile.width + 4, projectile.height + 4, 44, projectile.velocity.X * 0.4f, projectile.velocity.Y * 0.4f, 100, Color.LimeGreen, .8f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= 1.8f;
                        Dust expr_1CCF_cp_0 = Main.dust[dust];
                        expr_1CCF_cp_0.velocity.Y = expr_1CCF_cp_0.velocity.Y - 0.5f;
                        if (Main.rand.Next(4) == 0)
                        {
                            Main.dust[dust].noGravity = false;
                            Main.dust[dust].scale *= 0.5f;
                        }
                    }

                    tikiTimer--;

                    if (tikiTimer <= 0)
                    {
                        for (int num468 = 0; num468 < 20; num468++)
                        {
                            int num469 = Dust.NewDust(new Vector2(projectile.Center.X, projectile.Center.Y), projectile.width, projectile.height, 44, -projectile.velocity.X * 0.2f,
                                -projectile.velocity.Y * 0.2f, 100, Color.LimeGreen, 1f);
                            Main.dust[num469].noGravity = true;
                            Main.dust[num469].velocity *= 2f;
                            num469 = Dust.NewDust(new Vector2(projectile.Center.X, projectile.Center.Y), projectile.width, projectile.height, 44, -projectile.velocity.X * 0.2f,
                                -projectile.velocity.Y * 0.2f, 100, Color.LimeGreen, .5f);
                            Main.dust[num469].velocity *= 2f;
                        }

                        //stardust dragon fix
                        if (projectile.type == ProjectileID.StardustDragon2)
                        {
                            int tailIndex = -1;
                            for (int i = 0; i < Main.maxProjectiles; i++)
                            {
                                Projectile p = Main.projectile[i];

                                if (p.active && p.type == ProjectileID.StardustDragon4)
                                {
                                    tailIndex = i;
                                    break;
                                }
                            }

                            Projectile prev = Main.projectile[tailIndex];
                            List<int> list = new List<int>();
                            list.Add(prev.whoAmI);

                            while (prev.type != ProjectileID.StardustDragon1)
                            {
                                list.Add((int)prev.ai[0]);
                                prev = Main.projectile[(int)prev.ai[0]];
                            }

                            int listIndex = list.IndexOf(projectile.whoAmI);
                            Main.projectile[list[listIndex - 2]].ai[0] = list[listIndex + 1];
                        }

                        projectile.Kill();
                    }
                }

                if (SuperBee && (modPlayer.LifeForce || modPlayer.WizardEnchant))
                {
                    projectile.position += projectile.velocity;
                }

                //prob change in 1.4
                if (modPlayer.StardustEnchant && projectile.type == ProjectileID.StardustGuardian)
                {
                    projectile.localAI[0] = 0f;
                }

                //hook ai
                if (modPlayer.MahoganyEnchant && player.GetToggleValue("Mahogany", false) && projectile.aiStyle == 7 && (modPlayer.WoodForce || modPlayer.WizardEnchant))
                {
                    projectile.extraUpdates = 1;
                }

                if (projectile.friendly && !projectile.hostile)
                {
                    if (modPlayer.ForbiddenEnchant && projectile.damage > 0 && projectile.type != ModContent.ProjectileType<ForbiddenTornado>() && !stormBoosted)
                    {
                        Projectile nearestProj = null;

                        List<Projectile> projs = Main.projectile.Where(x => x.type == ModContent.ProjectileType<ForbiddenTornado>() && x.active).ToList();

                        foreach (Projectile p in projs)
                        {
                            Vector2 stormDistance = p.Center - projectile.Center;

                            if (Math.Abs(stormDistance.X) < p.width / 2 && Math.Abs(stormDistance.Y) < p.height / 2)
                            {
                                nearestProj = p;
                                break;
                            }
                        }

                        if (nearestProj != null)
                        {
                            float multiplier = 1.3f;

                            if (modPlayer.SpiritForce || modPlayer.WizardEnchant)
                            {
                                multiplier = 1.6f;
                            }

                            preStormDamage = projectile.damage;

                            projectile.damage = (int)(projectile.damage * multiplier);

                            stormBoosted = true;
                            stormTimer = 240;
                        }
                    }

                    if (stormTimer > 0)
                    {
                        stormTimer--;

                        if (stormTimer == 0)
                        {
                            projectile.damage = preStormDamage;
                            stormBoosted = false;
                        }
                    }

                    if (modPlayer.Jammed && projectile.ranged && projectile.type != ProjectileID.ConfettiGun)
                    {
                        Projectile.NewProjectile(projectile.Center, projectile.velocity, ProjectileID.ConfettiGun, 0, 0f);
                        projectile.active = false;
                    }

                    if (modPlayer.Atrophied && projectile.thrown)
                    {
                        projectile.damage = 0;
                        projectile.position = new Vector2(Main.maxTilesX);
                        projectile.Kill();
                    }

                    if (modPlayer.ShroomEnchant && player.GetToggleValue("ShroomiteShroom") && projectile.damage > 0 && !townNPCProj && projectile.velocity.Length() > 1 && projectile.minionSlots == 0 && projectile.type != ModContent.ProjectileType<ShroomiteShroom>() && player.ownedProjectileCounts[ModContent.ProjectileType<ShroomiteShroom>()] < 50)
                    {
                        if (shroomiteMushroomCD >= 15)
                        {
                            shroomiteMushroomCD = 0;

                            if (player.stealth == 0 || modPlayer.NatureForce || modPlayer.WizardEnchant)
                            {
                                shroomiteMushroomCD = 10;
                            }

                            Projectile.NewProjectile(projectile.position.X + (float)(projectile.width / 2), projectile.position.Y + (float)(projectile.height / 2), projectile.velocity.X, projectile.velocity.Y, ModContent.ProjectileType<ShroomiteShroom>(), projectile.damage / 2, 0f, projectile.owner, 0f, 0f);
                        }
                        shroomiteMushroomCD++;
                    }

                    if (modPlayer.SpookyEnchant && player.GetToggleValue("Spooky")
                        && projectile.minion && projectile.minionSlots > 0 && spookyCD == 0)
                    {
                        float minDistance = 500f;
                        int npcIndex = -1;
                        for (int i = 0; i < 200; i++)
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

                            if (Collision.CanHit(projectile.position, projectile.width, projectile.height, target.position, target.width, target.height))
                            {
                                Vector2 velocity = Vector2.Normalize(target.Center - projectile.Center) * 20;

                                int p = Projectile.NewProjectile(projectile.Center, velocity, ModContent.ProjectileType<SpookyScythe>(), projectile.damage, 2, projectile.owner);

                                Main.PlaySound(SoundID.Item, (int)projectile.position.X, (int)projectile.position.Y, 62, 0.5f);

                                spookyCD = 30 + Main.rand.Next(player.maxMinions * 5);

                                if (modPlayer.ShadowForce || modPlayer.WizardEnchant)
                                {
                                    spookyCD -= 10;
                                }
                            }
                        }

                    }
                }
            }

            if (ChilledTimer > 0)
            {
                ChilledTimer--;

                if (ChilledTimer == 0)
                {
                    ChilledProj = false;
                }
            }

            //if (modPlayer.SnowEnchant && player.GetToggleValue("Snow") && projectile.hostile && !ChilledProj)
            //{
            //    ChilledProj = true;
            //    projectile.timeLeft *= 2;
            //    projectile.netUpdate = true;
            //}
            
            if (TimeFrozen > 0 && !firstTick && !TimeFreezeImmune)
            {
                if (counter % projectile.MaxUpdates == 0) //only decrement once per tick
                    TimeFrozen--;
                if (counter > TimeFreezeMoveDuration * projectile.MaxUpdates)
                {
                    projectile.position = projectile.oldPosition;
                    if (projectile.frameCounter > 0)
                        projectile.frameCounter--;
                    projectile.timeLeft++;
                    retVal = false;
                }
            }

            //masomode unicorn meme and pearlwood meme
            if (Rainbow)
            {
                Player p = Main.player[projectile.owner];

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
                if (FargoSoulsWorld.MasochistMode && projectile.type == ProjectileID.Cthulunado && projectile.ai[1] == 24)
                {
                    TimeFrozen = 20;
                    canHurt = false;
                    retVal = false;
                }

                firstTick = false;
            }

            return retVal;
        }

        public override bool PreDraw(Projectile projectile, SpriteBatch spriteBatch, Color lightColor)
        {
            switch(projectile.type)
            {
                case ProjectileID.RedCounterweight:
                case ProjectileID.BlackCounterweight:
                case ProjectileID.BlueCounterweight:
                case ProjectileID.GreenCounterweight:
                case ProjectileID.PurpleCounterweight:
                case ProjectileID.YellowCounterweight:
                    {
                        Player player = Main.player[projectile.owner];
                        if(player.HeldItem.type == mod.ItemType("Blender"))
                        {
                            Texture2D texture2D13 = mod.GetTexture("Projectiles/PlanteraTentacle");
                            Rectangle rectangle = new Rectangle(0, 0, texture2D13.Width, texture2D13.Height);
                            Vector2 origin2 = rectangle.Size() / 2f;

                            SpriteEffects spriteEffects = projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

                            Vector2 toPlayer = projectile.Center - player.Center;
                            float drawRotation = toPlayer.ToRotation() + MathHelper.Pi;
                            if (projectile.spriteDirection < 0)
                                drawRotation += (float)Math.PI;
                            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), 
                                drawRotation, origin2, projectile.scale * 0.8f, spriteEffects, 0f);
                            return false;
                        }
                    }
                    break;
                default:
                    break;
            }
            return base.PreDraw(projectile, spriteBatch, lightColor);
        }

        public static void SplitProj(Projectile projectile, int number, float maxSpread, float damageRatio)
        {
            if (projectile.type == ModContent.ProjectileType<SpawnProj>())
            {
                return;
            }

            //if its odd, we just keep the original 
            if (number % 2 != 0)
            {
                number--;
            }

            Projectile split;

            double spread = maxSpread / number;

            for (int i = 0; i < number / 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    int factor = (j == 0) ? 1 : -1;
                    split = NewProjectileDirectSafe(projectile.Center, projectile.velocity.RotatedBy(factor * spread * (i + 1)), projectile.type, (int)(projectile.damage * damageRatio), projectile.knockBack, projectile.owner, projectile.ai[0], projectile.ai[1]);

                    if (split != null)
                    {
                        split.friendly = projectile.friendly;
                        split.hostile = projectile.hostile;
                        split.timeLeft = projectile.timeLeft;
                        split.GetGlobalProjectile<FargoGlobalProjectile>().numSplits = projectile.GetGlobalProjectile<FargoGlobalProjectile>().numSplits;
                        //split.GetGlobalProjectile<FargoGlobalProjectile>().firstTick = false;
                        split.GetGlobalProjectile<FargoGlobalProjectile>().CanSplit = false;
                        split.GetGlobalProjectile<FargoGlobalProjectile>().TungstenProjectile = projectile.GetGlobalProjectile<FargoGlobalProjectile>().TungstenProjectile;
                    }
                }
            }
        }

        private void KillPet(Projectile projectile, Player player, int buff, bool toggle, bool minion = false)
        {
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();

            if (player.FindBuffIndex(buff) == -1)
            {
                if (player.dead || !modPlayer.VoidSoul || (!modPlayer.StardustEnchant && minion) || !SoulConfig.Instance.GetValue(toggle) || (!modPlayer.PetsActive && !minion))
                    projectile.Kill();
            }
        }

        public override void AI(Projectile projectile)
        {
            Player player = Main.player[projectile.owner];
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();

            switch (projectile.type)
            {
                #region pets

                case ProjectileID.BabyHornet:
                    KillPet(projectile, player, BuffID.BabyHornet, player.GetToggleValue("PetHornet"));
                    break;

                case ProjectileID.Sapling:
                    KillPet(projectile, player, BuffID.PetSapling, player.GetToggleValue("PetSeed"));
                    break;

                case ProjectileID.BabyFaceMonster:
                    KillPet(projectile, player, BuffID.BabyFaceMonster, player.GetToggleValue("PetFaceMonster"));
                    break;

                case ProjectileID.CrimsonHeart:
                    KillPet(projectile, player, BuffID.CrimsonHeart, player.GetToggleValue("PetHeart"));
                    break;

                case ProjectileID.MagicLantern:
                    KillPet(projectile, player, BuffID.MagicLantern, player.GetToggleValue("PetLantern"));
                    break;

                case ProjectileID.MiniMinotaur:
                    KillPet(projectile, player, BuffID.MiniMinotaur, player.GetToggleValue("PetMinitaur"));
                    break;

                case ProjectileID.BlackCat:
                    KillPet(projectile, player, BuffID.BlackCat, player.GetToggleValue("PetBlackCat"));
                    break;

                case ProjectileID.Wisp:
                    KillPet(projectile, player, BuffID.Wisp, player.GetToggleValue("PetWisp"));
                    break;

                case ProjectileID.CursedSapling:
                    KillPet(projectile, player, BuffID.CursedSapling, player.GetToggleValue("PetCursedSapling"));
                    break;

                case ProjectileID.EyeSpring:
                    KillPet(projectile, player, BuffID.EyeballSpring, player.GetToggleValue("PetEyeSpring"));
                    break;

                case ProjectileID.Turtle:
                    KillPet(projectile, player, BuffID.PetTurtle, player.GetToggleValue("PetTurtle"));
                    break;

                case ProjectileID.PetLizard:
                    KillPet(projectile, player, BuffID.PetLizard, player.GetToggleValue("PetLizard"));
                    break;

                case ProjectileID.Truffle:
                    KillPet(projectile, player, BuffID.BabyTruffle, player.GetToggleValue("PetShroom"));
                    break;

                case ProjectileID.Spider:
                    KillPet(projectile, player, BuffID.PetSpider, player.GetToggleValue("PetSpider"));
                    break;

                case ProjectileID.Squashling:
                    KillPet(projectile, player, BuffID.Squashling, player.GetToggleValue("PetSquash"));
                    break;

                case ProjectileID.BlueFairy:
                    KillPet(projectile, player, BuffID.FairyBlue, player.GetToggleValue("PetNavi"));
                    break;

                case ProjectileID.StardustGuardian:
                    KillPet(projectile, player, BuffID.StardustGuardianMinion, player.GetToggleValue("Stardust"), true);
                    if (modPlayer.FreezeTime && modPlayer.freezeLength > 60) //throw knives in stopped time
                    {
                        if (projectile.owner == Main.myPlayer && counter % 20 == 0)
                        {
                            int target = -1;

                            NPC minionAttackTargetNpc = projectile.OwnerMinionAttackTargetNPC;
                            if (minionAttackTargetNpc != null && minionAttackTargetNpc.CanBeChasedBy())
                            {
                                target = minionAttackTargetNpc.whoAmI;
                            }
                            else
                            {
                                const float homingMaximumRangeInPixels = 1000;
                                for (int i = 0; i < Main.maxNPCs; i++)
                                {
                                    NPC n = Main.npc[i];
                                    if (n.CanBeChasedBy(projectile))
                                    {
                                        float distance = projectile.Distance(n.Center);
                                        if (distance <= homingMaximumRangeInPixels &&
                                            (target == -1 || //there is no selected target
                                            projectile.Distance(Main.npc[target].Center) > distance)) //or we are closer to this target than the already selected target
                                        {
                                            target = i;
                                        }
                                    }
                                }
                            }

                            if (target != -1)
                            {
                                const int totalUpdates = 2 + 1;
                                const int travelTime = TimeFreezeMoveDuration * totalUpdates;

                                Vector2 spawnPos = projectile.Center + 16f * projectile.DirectionTo(Main.npc[target].Center);

                                //adjust speed so it always lands just short of touching the enemy
                                Vector2 vel = Main.npc[target].Center - spawnPos;
                                float length = (vel.Length() - 0.6f * Math.Max(Main.npc[target].width, Main.npc[target].height)) / travelTime;
                                if (length < 0.1f)
                                    length = 0.1f;
                                
                                float offset = 1f - (modPlayer.freezeLength - 60f) / 540f; //change how far they stop as time decreases
                                if (offset < 0.1f)
                                    offset = 0.1f;
                                if (offset > 1f)
                                    offset = 1f;
                                length *= offset;

                                const int max = 3;
                                int damage = 100; //at time of writing, raw hellzone does 190 damage, 7.5 times per second, 1425 dps
                                if (modPlayer.CosmoForce)
                                    damage = 150;
                                if (modPlayer.TerrariaSoul)
                                    damage = 300;
                                damage = (int)(damage * player.minionDamage);
                                float rotation = MathHelper.ToRadians(60) * Main.rand.NextFloat(0.2f, 1f);
                                float rotationOffset = MathHelper.ToRadians(5) * Main.rand.NextFloat(-1f, 1f);
                                for (int i = -max; i <= max; i++)
                                {
                                    Projectile.NewProjectile(spawnPos, length * Vector2.Normalize(vel).RotatedBy(rotation / max * i + rotationOffset),
                                        ModContent.ProjectileType<StardustKnife>(), damage, 4f, Main.myPlayer);
                                }
                            }
                        }
                    }
                    break;

                case ProjectileID.TikiSpirit:
                    KillPet(projectile, player, BuffID.TikiSpirit, player.GetToggleValue("PetTiki"));
                    break;

                case ProjectileID.Penguin:
                    KillPet(projectile, player, BuffID.BabyPenguin, player.GetToggleValue("PetPenguin"));
                    break;

                case ProjectileID.BabySnowman:
                    KillPet(projectile, player, BuffID.BabySnowman, player.GetToggleValue("PetSnowman"));
                    break;

                case ProjectileID.BabyGrinch:
                    KillPet(projectile, player, BuffID.BabyGrinch, player.GetToggleValue("PetGrinch"));
                    break;

                case ProjectileID.DD2PetGato:
                    KillPet(projectile, player, BuffID.PetDD2Gato, player.GetToggleValue("PetGato"));
                    break;

                case ProjectileID.Parrot:
                    KillPet(projectile, player, BuffID.PetParrot, player.GetToggleValue("PetParrot"));
                    break;

                case ProjectileID.Puppy:
                    KillPet(projectile, player, BuffID.Puppy, player.GetToggleValue("PetPup"));
                    break;

                case ProjectileID.CompanionCube:
                    KillPet(projectile, player, BuffID.CompanionCube, player.GetToggleValue("PetCompanionCube"));
                    break;

                case ProjectileID.DD2PetDragon:
                    KillPet(projectile, player, BuffID.PetDD2Dragon, player.GetToggleValue("PetDragon"));
                    break;

                case ProjectileID.BabySkeletronHead:
                    KillPet(projectile, player, BuffID.BabySkeletronHead, player.GetToggleValue("PetDG"));
                    break;

                case ProjectileID.BabyDino:
                    KillPet(projectile, player, BuffID.BabyDinosaur, player.GetToggleValue("PetDino"));
                    break;

                case ProjectileID.BabyEater:
                    KillPet(projectile, player, BuffID.BabyEater, player.GetToggleValue("PetEater"));
                    break;

                case ProjectileID.ShadowOrb:
                    KillPet(projectile, player, BuffID.ShadowOrb, player.GetToggleValue("PetOrb"));
                    break;

                case ProjectileID.SuspiciousTentacle:
                    KillPet(projectile, player, BuffID.SuspiciousTentacle, player.GetToggleValue("PetSuspEye"));
                    break;

                case ProjectileID.DD2PetGhost:
                    KillPet(projectile, player, BuffID.PetDD2Ghost, player.GetToggleValue("PetFlicker"));
                    break;

                case ProjectileID.ZephyrFish:
                    KillPet(projectile, player, BuffID.ZephyrFish, player.GetToggleValue("PetZephyr"));
                    break;

                #endregion

                case ProjectileID.BabySlime:
                    if (FargoSoulsWorld.MasochistMode)
                    {
                        if (!masobool)
                        {
                            masobool = true;
                            if (CanSplit)
                            {
                                if (projectile.owner == Main.myPlayer)
                                    SplitProj(projectile, 2, MathHelper.PiOver2, 1f);
                                CanSplit = false;
                            }
                        }
                    }
                    break;

                case ProjectileID.VampireHeal:
                    if (FargoSoulsWorld.MasochistMode)
                    {
                        if (!masobool)
                        {
                            masobool = true;
                            //each lifesteal hits timer again when above 33% life (total, halved lifesteal rate)
                            if (Main.player[projectile.owner].statLife > Main.player[projectile.owner].statLifeMax2 / 3)
                                Main.player[projectile.owner].lifeSteal -= projectile.ai[1];
                            //each lifesteal hits timer again when above 33% life (stacks with above, total 1/3rd lifesteal rate)
                            if (Main.player[projectile.owner].statLife > Main.player[projectile.owner].statLifeMax2 * 2 / 3)
                                Main.player[projectile.owner].lifeSteal -= projectile.ai[1];
                        }
                    }
                    break;

                case ProjectileID.DD2SquireSonicBoom:
                    projectile.position += projectile.velocity / 2f;
                    break;

                case ProjectileID.TowerDamageBolt:
                    if (FargoSoulsWorld.MasochistMode)
                    {
                        if (!masobool)
                        {
                            masobool = true;

                            int ai0 = (int)projectile.ai[0];
                            if (ai0 > -1 && ai0 < Main.maxNPCs && Main.npc[ai0].active)
                            {
                                //not kill, because kill decrements shield
                                if (projectile.Distance(Main.npc[ai0].Center) > 4000)
                                    projectile.active = false;
                                int p = Player.FindClosest(projectile.Center, 0, 0);
                                if (p != -1 && !(Main.player[p].active && Main.npc[ai0].Distance(Main.player[p].Center) < 4000))
                                    projectile.active = false;
                            }
                        }
                    }
                    break;
                    
                case ProjectileID.SpiritHeal:
                    if (FargoSoulsWorld.MasochistMode)
                        projectile.position -= projectile.velocity / 4;
                    break;

                case ProjectileID.Sharknado: //ai0 15 ai1 15
                case ProjectileID.Cthulunado: //ai0 15 ai1 24
                    if (FargoSoulsWorld.MasochistMode)
                    {
                        canHurt = true;
                    }
                    break;

                case ProjectileID.WireKite:
                    if (FargoSoulsWorld.MasochistMode)
                    {
                        if (Main.player[projectile.owner].GetModPlayer<FargoPlayer>().LihzahrdCurse
                            && Framing.GetTileSafely(projectile.Center).wall == WallID.LihzahrdBrickUnsafe)
                        {
                            projectile.Kill();
                        }
                    }
                    break;

                case ProjectileID.Fireball:
                    if (FargoSoulsWorld.MasochistMode)
                    {
                        if (NPC.golemBoss > -1 && NPC.golemBoss < Main.maxNPCs && Main.npc[NPC.golemBoss].active && Main.npc[NPC.golemBoss].type == NPCID.Golem
                            && !Main.npc[NPC.golemBoss].dontTakeDamage)
                        {
                            projectile.timeLeft = 0;
                        }
                    }
                    break;

                case ProjectileID.GeyserTrap:
                    if (FargoSoulsWorld.MasochistMode)
                    {
                        if (NPC.golemBoss > -1 && NPC.golemBoss < Main.maxNPCs && Main.npc[NPC.golemBoss].active && Main.npc[NPC.golemBoss].type == NPCID.Golem)
                        {
                            if (counter > 45)
                                projectile.Kill();
                        }
                    }
                    break;

                case ProjectileID.NebulaSphere:
                    if (FargoSoulsWorld.MasochistMode)
                    {
                        if (EModeGlobalNPC.BossIsAlive(ref EModeGlobalNPC.cultBoss, NPCID.CultistBoss) && counter % 60 < 30)
                        {
                            projectile.position += projectile.velocity;
                        }
                    }
                    break;

                case ProjectileID.PhantasmalBolt:
                    if (FargoSoulsWorld.MasochistMode)
                    {
                        if (EModeGlobalNPC.BossIsAlive(ref EModeGlobalNPC.moonBoss, NPCID.MoonLordCore) && !FargoSoulsWorld.SwarmActive)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                for (int i = -2; i <= 2; i++)
                                {
                                    Projectile.NewProjectile(projectile.Center,
                                        1.5f * Vector2.Normalize(projectile.velocity).RotatedBy(Math.PI / 2 / 2 * i),
                                        ModContent.ProjectileType<PhantasmalBolt2>(), projectile.damage, 0f, Main.myPlayer);
                                }
                                projectile.Kill();
                            }
                        }
                    }
                    break;

                case ProjectileID.DeathLaser:
                    if (FargoSoulsWorld.MasochistMode)
                    {
                        if (EModeGlobalNPC.BossIsAlive(ref EModeGlobalNPC.retiBoss, NPCID.Retinazer) && !FargoSoulsWorld.SwarmActive)
                        {
                            if (!masobool)
                            {
                                masobool = true;
                                projectile.velocity.Normalize();
                            }
                            
                            if (projectile.timeLeft % (projectile.extraUpdates + 1) == 0) //only run once per tick
                            {
                                if (++projectile.localAI[1] < 90)
                                {
                                    projectile.velocity *= 1.06f;
                                }
                            }
                        }
                    }
                    break;

                case ProjectileID.EyeBeam:
                    if (FargoSoulsWorld.MasochistMode)
                    {
                        if (EModeGlobalNPC.BossIsAlive(ref NPC.golemBoss, NPCID.Golem) && !FargoSoulsWorld.SwarmActive)
                        {
                            if (!masobool)
                            {
                                masobool = true;
                                projectile.velocity.Normalize();
                            }

                            if (projectile.timeLeft % (projectile.extraUpdates + 1) == 0) //only run once per tick
                            {
                                if (++projectile.localAI[1] < 90)
                                {
                                    projectile.velocity *= 1.04f;
                                }
                            }
                        }
                    }
                    break;

                case ProjectileID.JavelinHostile:
                case ProjectileID.FlamingWood:
                    if (FargoSoulsWorld.MasochistMode)
                        projectile.position += projectile.velocity * .5f;
                    break;

                case ProjectileID.VortexAcid:
                    if (FargoSoulsWorld.MasochistMode)
                        projectile.position += projectile.velocity * .25f;
                    break;

                case ProjectileID.CultistRitual:
                    if (FargoSoulsWorld.MasochistMode && !FargoSoulsWorld.SwarmActive)
                    {
                        if (projectile.ai[1] > -1 && projectile.ai[1] < Main.maxNPCs 
                            && Main.npc[(int)projectile.ai[1]].ai[3] == -1f && Main.npc[(int)projectile.ai[1]].ai[0] == 5)
                        {
                            projectile.Center = Main.player[Main.npc[(int)projectile.ai[1]].target].Center;
                        }

                        if (!masobool) //MP sync data to server
                        {
                            masobool = true;

                            if (projectile.ai[1] > -1 && projectile.ai[1] < Main.maxNPCs)
                            {
                                NPC cultist = Main.npc[(int)projectile.ai[1]];
                                if (Main.netMode == NetmodeID.MultiplayerClient)
                                {
                                    EModeGlobalNPC fargoCultist = cultist.GetGlobalNPC<EModeGlobalNPC>();

                                    var netMessage = mod.GetPacket();
                                    netMessage.Write((byte)10);
                                    netMessage.Write((byte)projectile.ai[1]);
                                    netMessage.Write(fargoCultist.Counter[0]);
                                    netMessage.Write(fargoCultist.Counter[1]);
                                    netMessage.Write(fargoCultist.Counter[2]);
                                    netMessage.Write(cultist.localAI[3]);
                                    netMessage.Send();

                                    fargoCultist.Counter[0] = 0; //clear client side data now
                                    fargoCultist.Counter[1] = 0;
                                    fargoCultist.Counter[2] = 0;
                                    cultist.localAI[3] = 0f;
                                }
                                else //refresh ritual
                                {
                                    for (int i = 0; i < Main.maxProjectiles; i++)
                                    {
                                        if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<CultistRitual>())
                                        {
                                            Main.projectile[i].Kill();
                                            break;
                                        }
                                    }
                                    Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<CultistRitual>(), cultist.damage / 4, 0f, Main.myPlayer, 0f, cultist.whoAmI);
                                }
                            }

                            for (int i = 0; i < Main.maxProjectiles; i++) //purge spectre mask bolts and homing nebula spheres
                            {
                                if (Main.projectile[i].active && (Main.projectile[i].type == ProjectileID.SpectreWrath || Main.projectile[i].type == ProjectileID.NebulaSphere))
                                    Main.projectile[i].Kill();
                            }
                        }

                        if (Fargowiltas.Instance.MasomodeEXLoaded && projectile.ai[0] > 120f && projectile.ai[0] < 299f)
                            projectile.ai[0] = 299f;

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

                                NPC cultist = Main.npc[cult];
                                EModeGlobalNPC fargoCultist = cultist.GetGlobalNPC<EModeGlobalNPC>();
                                int[] weight = new int[4];
                                weight[0] = fargoCultist.Counter[0];
                                weight[1] = fargoCultist.Counter[1];
                                weight[2] = fargoCultist.Counter[2];
                                weight[3] = (int)cultist.localAI[3];
                                fargoCultist.Counter[0] = 0;
                                fargoCultist.Counter[1] = 0;
                                fargoCultist.Counter[2] = 0;
                                cultist.localAI[3] = 0f;
                                int max = 0;
                                for (int i = 1; i < 4; i++)
                                    if (weight[max] < weight[i])
                                        max = i;
                                if (weight[max] > 0)
                                    ai0 = max;

                                if ((cultist.life < cultist.lifeMax / 2 || Fargowiltas.Instance.MasomodeEXLoaded) && Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(projectile.Center, Vector2.UnitY * -10f, ModContent.ProjectileType<CelestialPillar>(),
                                        75, 0f, Main.myPlayer, ai0);
                            }
                        }
                    }
                    break;

                case ProjectileID.MoonLeech:
                    if (FargoSoulsWorld.MasochistMode && projectile.ai[0] > 0f && !FargoSoulsWorld.SwarmActive)
                    {
                        Vector2 distance = Main.player[(int)projectile.ai[1]].Center - projectile.Center - projectile.velocity;
                        if (distance != Vector2.Zero)
                            projectile.position += Vector2.Normalize(distance) * Math.Min(16f, distance.Length());
                    }
                    break;

                case ProjectileID.DesertDjinnCurse:
                    if (EModeGlobalNPC.BossIsAlive(ref EModeGlobalNPC.championBoss, ModContent.NPCType<NPCs.Champions.SpiritChampion>())
                        && projectile.damage > 0)
                    {
                        projectile.damage = Main.npc[EModeGlobalNPC.championBoss].damage / 4;
                    }
                    break;

                case ProjectileID.SandnadoHostile:
                    if (EModeGlobalNPC.BossIsAlive(ref EModeGlobalNPC.deviBoss, ModContent.NPCType<NPCs.DeviBoss.DeviBoss>())
                        && projectile.Distance(Main.npc[EModeGlobalNPC.deviBoss].Center) < 2000f)
                    {
                        projectile.damage = Main.npc[EModeGlobalNPC.deviBoss].damage / 4;
                        if (Main.npc[EModeGlobalNPC.deviBoss].ai[0] != 5 && projectile.timeLeft > 90)
                            projectile.timeLeft = 90;
                    }
                    else if (EModeGlobalNPC.BossIsAlive(ref EModeGlobalNPC.championBoss, ModContent.NPCType<NPCs.Champions.SpiritChampion>()))
                    {
                        projectile.damage = Main.npc[EModeGlobalNPC.championBoss].damage / 4;
                    }
                    else if (FargoSoulsWorld.MasochistMode && projectile.timeLeft == 1199 && NPC.CountNPCS(NPCID.SandShark) < 10 && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int n = NPC.NewNPC((int)projectile.Center.X, (int)projectile.Center.Y, NPCID.SandShark);
                        if (n < 200)
                        {
                            Main.npc[n].velocity.X = Main.rand.NextFloat(-10, 10);
                            Main.npc[n].velocity.Y = Main.rand.NextFloat(-20, -10);
                            Main.npc[n].netUpdate = true;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                        }
                    }
                    break;

                case ProjectileID.GoldenShowerHostile:
                    /*if (FargoSoulsWorld.MasochistMode && Main.netMode != NetmodeID.MultiplayerClient && Main.rand.Next(6) == 0
                        && !(projectile.position.Y / 16 > Main.maxTilesY - 200 && FargoSoulsGlobalNPC.BossIsAlive(ref FargoSoulsGlobalNPC.wallBoss, NPCID.WallofFlesh)))
                    {
                        int p = Projectile.NewProjectile(projectile.Center, projectile.velocity, ProjectileID.CrimsonSpray, 0, 0f, Main.myPlayer, 8f);
                        if (p != 1000)
                            Main.projectile[p].timeLeft = 6;
                    }*/
                    break;

                case ProjectileID.RuneBlast:
                    if (FargoSoulsWorld.MasochistMode && projectile.ai[0] == 1f)
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
    
                case ProjectileID.PhantasmalEye:
                    if (FargoSoulsWorld.MasochistMode)
                    {
                        if (EModeGlobalNPC.BossIsAlive(ref EModeGlobalNPC.moonBoss, NPCID.MoonLordCore) && !FargoSoulsWorld.SwarmActive)
                        {
                            if (projectile.ai[0] == 2 && counter > 60) //diving down and homing
                            {
                                projectile.velocity.Y = 9;
                            }
                            else
                            {
                                projectile.position.Y -= projectile.velocity.Y / 4;
                            }

                            if (projectile.velocity.X > 1)
                                projectile.velocity.X = 1;
                            else if (projectile.velocity.X < -1)
                                projectile.velocity.X = -1;
                        }
                    }
                    break;

                case ProjectileID.PhantasmalSphere:
                    if (FargoSoulsWorld.MasochistMode && !FargoSoulsWorld.SwarmActive)
                    {
                        if (!masobool)
                        {
                            masobool = true;
                            int ai1 = (int)projectile.ai[1];
                            if (ai1 > -1 && ai1 < Main.maxNPCs && Main.npc[ai1].active && Main.npc[ai1].type == NPCID.MoonLordHand)
                            {
                                projectile.localAI[0] = 1;
                            }
                        }

                        canHurt = projectile.alpha == 0;

                        if (projectile.ai[0] == -1 && projectile.localAI[0] > 0) //sent to fly, flagged as from hand
                        {
                            if (++projectile.localAI[1] < 150)
                                projectile.velocity *= 1.018f;

                            if (projectile.localAI[0] == 1 && projectile.velocity.Length() > 11) //only do this once
                            {
                                projectile.localAI[0] = 2;
                                projectile.velocity.Normalize();

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(projectile.Center, projectile.velocity, ModContent.ProjectileType<PhantasmalSphereDeathray>(),
                                        0, 0f, Main.myPlayer, 0f, projectile.whoAmI);
                                }
                            }
                        }
                    }
                    break;

                case ProjectileID.BombSkeletronPrime: //needs to be set every tick
                    if (FargoSoulsWorld.MasochistMode && !FargoSoulsWorld.SwarmActive)
                        projectile.damage = 40;
                    break;

                case ProjectileID.DD2BetsyFireball:
                    if (FargoSoulsWorld.MasochistMode && !FargoSoulsWorld.SwarmActive)
                    {
                        if (!masobool)
                        {
                            masobool = true;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                bool phase2 = EModeGlobalNPC.BossIsAlive(ref EModeGlobalNPC.betsyBoss, NPCID.DD2Betsy)
                                    && Main.npc[EModeGlobalNPC.betsyBoss].GetGlobalNPC<EModeGlobalNPC>().masoBool[3];
                                int max = phase2 ? 2 : 1;
                                for (int i = 0; i < max; i++)
                                {
                                    Vector2 speed = Main.rand.NextFloat(8, 12) * -Vector2.UnitY.RotatedByRandom(Math.PI / 2);
                                    float ai1 = phase2 ? 60 + Main.rand.Next(60) : 90 + Main.rand.Next(30);
                                    Projectile.NewProjectile(projectile.Center, speed, ModContent.ProjectileType<BetsyPhoenix>(),
                                        projectile.damage, 0f, Main.myPlayer, Player.FindClosest(projectile.Center, 0, 0), ai1);
                                }
                            }
                        }
                    }
                    break;

                case ProjectileID.DD2BetsyFlameBreath:
                    if (FargoSoulsWorld.MasochistMode && !FargoSoulsWorld.SwarmActive)
                    {
                        bool phase2 = EModeGlobalNPC.BossIsAlive(ref EModeGlobalNPC.betsyBoss, NPCID.DD2Betsy)
                                    && Main.npc[EModeGlobalNPC.betsyBoss].GetGlobalNPC<EModeGlobalNPC>().masoBool[3];

                        if (counter > (phase2 ? 2 : 4))
                        {
                            counter = 0;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Main.PlaySound(SoundID.Item34, projectile.Center);
                                Vector2 projVel = projectile.velocity.RotatedBy((Main.rand.NextDouble() - 0.5) * Math.PI / 10);
                                projVel.Normalize();
                                projVel *= Main.rand.NextFloat(8f, 12f);
                                int type = ProjectileID.CultistBossFireBall;
                                if (!phase2 || Main.rand.Next(2) == 0)
                                {
                                    type = ModContent.ProjectileType<Champions.WillFireball>();
                                    projVel *= 2f;
                                    if (phase2)
                                        projVel *= 1.5f;
                                }
                                Projectile.NewProjectile(projectile.Center, projVel, type, projectile.damage, 0f, Main.myPlayer);
                            }
                        }
                    }
                    break;

                default:
                        break;
            }

            if (stormBoosted)
            {
                int dustId = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.GoldFlame, projectile.velocity.X, projectile.velocity.Y, 100, default(Color), 1.5f);
                Main.dust[dustId].noGravity = true;
            }

            if (ChilledProj)
            {
                int dustId = Dust.NewDust(projectile.position, projectile.width, projectile.height, 76, projectile.velocity.X, projectile.velocity.Y, 100, default(Color), 1f);
                Main.dust[dustId].noGravity = true;

                projectile.position -= projectile.velocity * 0.5f;
            }

            if (SilverMinion && projectile.owner == Main.myPlayer)
            {
                /*if (counter == 60) //i hate netcode, proj array desyncs between clients
                {
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        var netMessage = mod.GetPacket();
                        netMessage.Write((byte)19);
                        netMessage.Write(projectile.whoAmI);
                        netMessage.Write(projectile.type);
                        netMessage.Write(projectile.extraUpdates);
                        netMessage.Send();
                    }
                }*/

                if (!(modPlayer.SilverEnchant && player.GetToggleValue("SilverSpeed")))
                    projectile.Kill();
            }

            if (projectile.bobber && modPlayer.FishSoul1)
            {
                //ai0 = in water, localai1 = counter up to catching an item
                if (projectile.wet && projectile.ai[0] == 0 && projectile.localAI[1] < 655 && Main.player[projectile.owner].FishingLevel() != -1) //fishron check
                    projectile.localAI[1] = 655; //quick catch. not 660 and up, may break things
            }
        }

        public static void PrintAI(Projectile projectile)
        {
            Main.NewText(projectile.ai[0].ToString() + " " + projectile.ai[1].ToString() + " " + projectile.localAI[0].ToString() + " " + projectile.localAI[1].ToString());
        }

        public override void PostAI(Projectile projectile)
        {
            if (!TimeFreezeCheck)
            {
                TimeFreezeCheck = true;
                if (projectile.whoAmI == Main.player[projectile.owner].heldProj)
                    TimeFreezeImmune = true;
            }

            if (projectile.hostile && projectile.damage > 0 && canHurt && Main.LocalPlayer.active && !Main.LocalPlayer.dead) //graze
            {
                FargoPlayer fargoPlayer = Main.LocalPlayer.GetModPlayer<FargoPlayer>();
                if (fargoPlayer.Graze && --GrazeCD < 0 && !Main.LocalPlayer.immune && Main.LocalPlayer.hurtCooldowns[0] <= 0 && Main.LocalPlayer.hurtCooldowns[1] <= 0)
                {
                    if (CanHitPlayer(projectile, Main.LocalPlayer) && GrazeCheck(projectile))
                    {
                        double grazeCap = 0.25;
                        if (fargoPlayer.MutantEye)
                            grazeCap += 0.25;

                        double grazeGain = 0.0125;
                        if (fargoPlayer.CyclonicFin)
                            grazeGain *= 2;

                        GrazeCD = 30 * projectile.MaxUpdates;
                        fargoPlayer.GrazeBonus += grazeGain;
                        if (fargoPlayer.GrazeBonus > grazeCap)
                            fargoPlayer.GrazeBonus = grazeCap;
                        fargoPlayer.GrazeCounter = -1; //reset counter whenever successful graze

                        if (!Main.dedServ)
                        {
                            Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Graze").WithVolume(0.5f), Main.LocalPlayer.Center);
                        }

                        Vector2 baseVel = Vector2.UnitX.RotatedByRandom(2 * Math.PI);
                        const int max = 36; //make some indicator dusts
                        for (int i = 0; i < max; i++)
                        {
                            Vector2 vector6 = baseVel * 4f;
                            vector6 = vector6.RotatedBy((i - (max / 2 - 1)) * 6.28318548f / max) + Main.LocalPlayer.Center;
                            Vector2 vector7 = vector6 - Main.LocalPlayer.Center;
                            //changes color when bonus is maxed
                            int d = Dust.NewDust(vector6 + vector7, 0, 0, fargoPlayer.GrazeBonus >= grazeCap ? 86 : 228, 0f, 0f, 0, default(Color));
                            Main.dust[d].scale = fargoPlayer.GrazeBonus >= grazeCap ? 2f : 0.75f;
                            Main.dust[d].noGravity = true;
                            Main.dust[d].velocity = vector7;
                        }
                    }
                    else
                    {
                        GrazeCD = 6; //don't check per tick ech
                    }
                }
            }
        }

        public override bool TileCollideStyle(Projectile projectile, ref int width, ref int height, ref bool fallThrough)
        {
            if (projectile.type == ProjectileID.SmokeBomb)
            {
                fallThrough = false;
            }

            if (TungstenProjectile)
            {
                width /= 2;
                height /= 2;
            }

            return base.TileCollideStyle(projectile, ref width, ref height, ref fallThrough);
        }

        public override bool CanHitPlayer(Projectile projectile, Player target)
        {
            if (!canHurt)
                return false;
            if (TimeFrozen > 0 && counter > TimeFreezeMoveDuration * projectile.MaxUpdates)
                return false;
            return true;
        }

        public override bool? CanHitNPC(Projectile projectile, NPC target)
        {
            if (!canHurt)
                return false;
            if (TimeFrozen > 0 && counter > TimeFreezeMoveDuration * projectile.MaxUpdates)
                return false;
            return null;
        }

        public override void ModifyHitNPC(Projectile projectile, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (FargoSoulsWorld.MasochistMode)
            {
                if (projectile.arrow) //change archery and quiver to additive damage
                {
                    if (Main.player[projectile.owner].archery)
                    {
                        damage = (int)(damage / 1.2);
                        damage = (int)((double)damage * (1.0 + 0.2 / Main.player[projectile.owner].rangedDamage));
                    }

                    if (Main.player[projectile.owner].magicQuiver)
                    {
                        damage = (int)(damage / 1.1);
                        damage = (int)((double)damage * (1.0 + 0.1 / Main.player[projectile.owner].rangedDamage));
                    }
                }
            }

            if (projectile.type >= ProjectileID.StardustDragon1 && projectile.type <= ProjectileID.StardustDragon4
                && Main.player[projectile.owner].GetModPlayer<FargoPlayer>().TikiMinion
                && Main.player[projectile.owner].ownedProjectileCounts[ProjectileID.StardustDragon2] > Main.player[projectile.owner].GetModPlayer<FargoPlayer>().actualMinions)
            {
                int newDamage = (int)(projectile.damage * (1.69 + 0.46 * Main.player[projectile.owner].GetModPlayer<FargoPlayer>().actualMinions));
                if (damage > newDamage)
                    damage = newDamage;
            }
            
            if (SilverMinion)
            {
                if (projectile.maxPenetrate == 1 || projectile.usesLocalNPCImmunity || projectile.type == ProjectileID.StardustCellMinionShot)
                    damage /= 2;
                //else damage = (int)(damage * 3.0 / 4.0);
            }
        }

        public override void OnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            if (FargoSoulsWorld.MasochistMode)
            {
                switch (projectile.type)
                {
                    case ProjectileID.VenomSpider:
                    case ProjectileID.JumperSpider:
                    case ProjectileID.DangerousSpider:
                        target.immune[projectile.owner] = 5;
                        break;

                    case ProjectileID.MiniRetinaLaser:
                    case ProjectileID.Retanimini:
                    case ProjectileID.Spazmamini:
                        target.immune[projectile.owner] = 5;
                        break;

                    case ProjectileID.DeadlySphere:
                        target.immune[projectile.owner] = 8;
                        break;

                    default:
                        break;
                }
            }

            if (FrostFreeze)
            {
                FargoSoulsGlobalNPC globalNPC = target.GetGlobalNPC<FargoSoulsGlobalNPC>();

                globalNPC.frostCount++;

                if (globalNPC.frostCD == 0)
                {
                    globalNPC.frostCD = 30;
                }

                //1st icicle
                if (!target.HasBuff(ModContent.BuffType<TimeFrozen>()))
                {
                    if (target.realLife != -1)
                    {
                        NPC head = Main.npc[target.realLife];
                        head.AddBuff(ModContent.BuffType<TimeFrozen>(), 60);
                        head.AddBuff(BuffID.Chilled, 300);

                        NPC next = Main.npc[(int)head.ai[0]];
                        int bodyType = next.type;

                        while (next.active && next.type == bodyType)
                        {
                            next.AddBuff(ModContent.BuffType<TimeFrozen>(), 60);
                            next.AddBuff(BuffID.Chilled, 300);
                            next = Main.npc[(int)next.ai[0]];
                        }

                        //one more for the tail
                        next.AddBuff(ModContent.BuffType<TimeFrozen>(), 60);
                        next.AddBuff(BuffID.Chilled, 300);
                    }
                    else
                    {
                        target.AddBuff(ModContent.BuffType<TimeFrozen>(), 30);
                        target.AddBuff(BuffID.Chilled, 300);
                    }
                }
                else
                {
                    //full 10 icicles means 1.5 extra seconds of freeze pog
                    if (target.realLife != -1)
                    {
                        NPC head = Main.npc[target.realLife];
                        head.AddBuff(ModContent.BuffType<TimeFrozen>(), 10);

                        NPC next = Main.npc[(int)head.ai[0]];
                        int bodyType = next.type;

                        while (next.active && next.type == bodyType)
                        {
                            next.AddBuff(ModContent.BuffType<TimeFrozen>(), 10);
                            next = Main.npc[(int)next.ai[0]];
                        }

                        //one more for the tail
                        next.AddBuff(ModContent.BuffType<TimeFrozen>(), 10);
                    }
                    else
                    {
                        target.AddBuff(ModContent.BuffType<TimeFrozen>(), 10);
                    }
                }
            }
        }

        public override bool OnTileCollide(Projectile projectile, Vector2 oldVelocity)
        {
            Player player = Main.player[Main.myPlayer];
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();
            
            if (FargoSoulsWorld.MasochistMode)
            {
                switch (projectile.type)
                {
                    case ProjectileID.SnowBallHostile:
                        projectile.active = false;
                        break;
                    case ProjectileID.SandBallFalling:
                        //antlion sand
                        if (projectile.ai[0] == 2f)
                        {
                            int num129 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 32, 0f, projectile.velocity.Y / 2f, 0, default(Color), 1f);
                            Dust expr_59B0_cp_0 = Main.dust[num129];
                            expr_59B0_cp_0.velocity.X = expr_59B0_cp_0.velocity.X * 0.4f;
                            projectile.active = false;
                            
                        }
                        break;
                }
                


            }

            return true;
        }

        public override void ModifyHitPlayer(Projectile projectile, Player target, ref int damage, ref bool crit)
        {
            if(squeakyToy)
            {
                damage = 1;
                target.GetModPlayer<FargoPlayer>().Squeak(target.Center);
            }

            if (FargoSoulsWorld.MasochistMode)
            {
                switch (projectile.type)
                {
                    case ProjectileID.VortexLightning:
                        if (NPC.downedGolemBoss)
                            damage *= 2;
                        break;

                    default:
                        break;
                }
            }
        }

        public override void OnHitPlayer(Projectile projectile, Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.MasochistMode)
            {
                switch(projectile.type)
                {
                    case ProjectileID.JavelinHostile:
                        target.AddBuff(ModContent.BuffType<Defenseless>(), 600);
                        if (!target.HasBuff(ModContent.BuffType<Stunned>()))
                            target.AddBuff(ModContent.BuffType<Stunned>(), 60);
                        break;

                    case ProjectileID.DemonSickle:
                        target.AddBuff(ModContent.BuffType<Shadowflame>(), 300);
                        break;

                    case ProjectileID.HarpyFeather:
                        target.AddBuff(ModContent.BuffType<ClippedWings>(), 300);
                        break;
                        
                    case ProjectileID.SandBallFalling:
                        if (!target.HasBuff(ModContent.BuffType<Stunned>()) && projectile.velocity.X != 0) //so only antlion sand and not falling sand 
                            target.AddBuff(ModContent.BuffType<Stunned>(), 90);
                        break;

                    case ProjectileID.Stinger:
                        target.AddBuff(ModContent.BuffType<Swarming>(), 300);
                        break;

                    case ProjectileID.Skull:
                        if (Main.rand.Next(2) == 0)
                            target.AddBuff(BuffID.Cursed, 60);
                        if (EModeGlobalNPC.BossIsAlive(ref EModeGlobalNPC.guardBoss, NPCID.DungeonGuardian))
                        {
                            target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 600);
                            /*target.AddBuff(ModContent.BuffType<GodEater>(), 420);
                            target.AddBuff(ModContent.BuffType<FlamesoftheUniverse>(), 420);
                            target.immune = false;
                            target.immuneTime = 0;*/
                        }
                        break;

                    case ProjectileID.EyeLaser:
                    case ProjectileID.GoldenShowerHostile:
                    case ProjectileID.CursedFlameHostile:
                        if (EModeGlobalNPC.BossIsAlive(ref EModeGlobalNPC.wallBoss, NPCID.WallofFlesh) && target.ZoneUnderworldHeight)
                            target.AddBuff(BuffID.OnFire, 300);
                        break;

                    case ProjectileID.DeathSickle:
                        target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 600);
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

                    case ProjectileID.CultistBossLightningOrb:
                    case ProjectileID.CultistBossLightningOrbArc:
                        target.AddBuff(BuffID.Electrified, 300);
                        break;

                    case ProjectileID.CultistBossIceMist:
                        if (!target.HasBuff(BuffID.Frozen))
                            target.AddBuff(BuffID.Frozen, 60);
                        target.AddBuff(ModContent.BuffType<Hypothermia>(), 600);
                        break;

                    case ProjectileID.CultistBossFireBall:
                        target.AddBuff(BuffID.OnFire, 300);
                        target.AddBuff(BuffID.Burning, 120);
                        if (EModeGlobalNPC.BossIsAlive(ref EModeGlobalNPC.betsyBoss, NPCID.DD2Betsy))
                        {
                            //target.AddBuff(BuffID.OnFire, 600);
                            //target.AddBuff(BuffID.Ichor, 600);
                            target.AddBuff(BuffID.WitheredArmor, Main.rand.Next(60, 300));
                            target.AddBuff(BuffID.WitheredWeapon, Main.rand.Next(60, 300));
                            target.AddBuff(BuffID.Burning, 300);
                        }
                        break;

                    case ProjectileID.CultistBossFireBallClone:
                        target.AddBuff(ModContent.BuffType<Shadowflame>(), 600);
                        break;

                    case ProjectileID.PaladinsHammerHostile:
                        if (!target.HasBuff(ModContent.BuffType<Stunned>()))
                            target.AddBuff(ModContent.BuffType<Stunned>(), 60);
                        break;

                    case ProjectileID.RuneBlast:

                        //target.AddBuff(ModContent.BuffType<Hexed>(), 240);
                        if (!EModeGlobalNPC.BossIsAlive(ref EModeGlobalNPC.deviBoss, mod.NPCType("DeviBoss")))
                        {
                            target.AddBuff(ModContent.BuffType<FlamesoftheUniverse>(), 60);
                            target.AddBuff(BuffID.Suffocation, 240);
                        }  
                        
                        break;

                    case ProjectileID.ThornBall:
                    case ProjectileID.PoisonSeedPlantera:
                    case ProjectileID.SeedPlantera:
                        target.AddBuff(BuffID.Poisoned, 300);
                        target.AddBuff(ModContent.BuffType<Infested>(), 180);
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
                        //target.AddBuff(ModContent.BuffType<Hexed>(), 240);
                        target.AddBuff(ModContent.BuffType<ReverseManaFlow>(), 600);
                        break;

                    case ProjectileID.InfernoHostileBlast:
                    case ProjectileID.InfernoHostileBolt:
                        /*if (!EModeGlobalNPC.BossIsAlive(ref EModeGlobalNPC.deviBoss, mod.NPCType("DeviBoss")))
                        {
                            if (Main.rand.Next(5) == 0)
                                target.AddBuff(ModContent.BuffType<Fused>(), 1800);
                        }*/
                        target.AddBuff(ModContent.BuffType<Jammed>(), 600);
                        break;

                    case ProjectileID.ShadowBeamHostile:
                        /*if (!EModeGlobalNPC.BossIsAlive(ref EModeGlobalNPC.deviBoss, mod.NPCType("DeviBoss")))
                        {
                            target.AddBuff(ModContent.BuffType<Rotting>(), 1800);
                            target.AddBuff(ModContent.BuffType<Shadowflame>(), 300);
                        }*/
                        target.AddBuff(ModContent.BuffType<Atrophied>(), 600);
                        break;

                    case ProjectileID.PhantasmalDeathray:
                        target.AddBuff(ModContent.BuffType<CurseoftheMoon>(), 360);
                        break;

                    case ProjectileID.PhantasmalBolt:
                    case ProjectileID.PhantasmalEye:
                    case ProjectileID.PhantasmalSphere:
                        target.AddBuff(ModContent.BuffType<CurseoftheMoon>(), 360);
                        if (EModeGlobalNPC.BossIsAlive(ref EModeGlobalNPC.mutantBoss, ModContent.NPCType<NPCs.MutantBoss.MutantBoss>()))
                        {
                            /*target.GetModPlayer<FargoPlayer>().MaxLifeReduction += 100;
                            target.AddBuff(ModContent.BuffType<OceanicMaul>(), 5400);*/
                            target.AddBuff(ModContent.BuffType<MutantFang>(), 180);
                        }
                        break;

                    case ProjectileID.RocketSkeleton:
                        target.AddBuff(BuffID.Dazed, 120);
                        target.AddBuff(ModContent.BuffType<Defenseless>(), 300);
                        break;

                    case ProjectileID.FlamesTrap:
                    case ProjectileID.GeyserTrap:
                    case ProjectileID.Fireball:
                    case ProjectileID.EyeBeam:
                        target.AddBuff(BuffID.OnFire, 300);
                        if (NPC.golemBoss != -1 && Main.npc[NPC.golemBoss].active && Main.npc[NPC.golemBoss].type == NPCID.Golem)
                        {
                            target.AddBuff(BuffID.BrokenArmor, 600);
                            target.AddBuff(ModContent.BuffType<Defenseless>(), 600);
                            target.AddBuff(BuffID.WitheredArmor, 600);
                            if (Main.tile[(int)Main.npc[NPC.golemBoss].Center.X / 16, (int)Main.npc[NPC.golemBoss].Center.Y / 16] == null || //outside temple
                                Main.tile[(int)Main.npc[NPC.golemBoss].Center.X / 16, (int)Main.npc[NPC.golemBoss].Center.Y / 16].wall != WallID.LihzahrdBrickUnsafe)
                            {
                                target.AddBuff(BuffID.Burning, 120);
                            }
                        }

                        if (EModeGlobalNPC.BossIsAlive(ref EModeGlobalNPC.championBoss, ModContent.NPCType<EarthChampion>()))
                        {
                            target.AddBuff(BuffID.Burning, 300);
                        }

                        if (EModeGlobalNPC.BossIsAlive(ref EModeGlobalNPC.championBoss, ModContent.NPCType<TerraChampion>()))
                        {
                            target.AddBuff(BuffID.OnFire, 600);
                            target.AddBuff(ModContent.BuffType<LivingWasteland>(), 600);
                        }
                        break;

                    case ProjectileID.SpikyBallTrap:
                        /*if (NPC.golemBoss != -1 && Main.npc[NPC.golemBoss].active && Main.npc[NPC.golemBoss].type == NPCID.Golem)
                        {
                            target.AddBuff(BuffID.BrokenArmor, 600);
                            target.AddBuff(ModContent.BuffType<Defenseless>(), 600);
                            target.AddBuff(BuffID.WitheredArmor, 600);
                        }*/
                        break;

                    case ProjectileID.DD2BetsyFireball:
                    case ProjectileID.DD2BetsyFlameBreath:
                        //target.AddBuff(BuffID.OnFire, 600);
                        //target.AddBuff(BuffID.Ichor, 600);
                        target.AddBuff(BuffID.WitheredArmor, Main.rand.Next(60, 300));
                        target.AddBuff(BuffID.WitheredWeapon, Main.rand.Next(60, 300));
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
                        target.AddBuff(ModContent.BuffType<OceanicMaul>(), 1800);
                        target.GetModPlayer<FargoPlayer>().MaxLifeReduction += EModeGlobalNPC.BossIsAlive(ref EModeGlobalNPC.fishBossEX, NPCID.DukeFishron) ? 100 : 25;
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
                        if (!target.HasBuff(BuffID.Frozen) && Main.rand.Next(2) == 0)
                            target.AddBuff(BuffID.Frozen, 60);
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
                        if (EModeGlobalNPC.BossIsAlive(ref EModeGlobalNPC.retiBoss, NPCID.Retinazer))
                            target.AddBuff(BuffID.Ichor, 600);
                        if (EModeGlobalNPC.BossIsAlive(ref EModeGlobalNPC.destroyBoss, NPCID.TheDestroyer))
                            target.AddBuff(BuffID.Electrified, 60);
                        break;

                    case ProjectileID.BulletDeadeye:
                    case ProjectileID.CannonballHostile:
                        target.AddBuff(ModContent.BuffType<Defenseless>(), 600);
                        target.AddBuff(ModContent.BuffType<Midas>(), 900);
                        break;

                    case ProjectileID.AncientDoomProjectile:
                        target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 300);
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
        }

        public override bool PreKill(Projectile projectile, int timeLeft)
        {
            if (FargoSoulsWorld.MasochistMode && projectile.owner == Main.myPlayer && HasKillCooldown)
            {
                if (Main.player[projectile.owner].GetModPlayer<FargoPlayer>().MasomodeCrystalTimer <= 60)
                {
                    Main.player[projectile.owner].GetModPlayer<FargoPlayer>().MasomodeCrystalTimer += 15;
                    return true;
                }
                else
                {
                    /*if (projectile.type == ProjectileID.CrystalBullet)
                    {
                        Main.PlaySound(0, (int)projectile.position.X, (int)projectile.position.Y, 1, 1f, 0.0f);
                        for (int index1 = 0; index1 < 5; ++index1) //vanilla dusts
                        {
                            int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 68, 0.0f, 0.0f, 0, new Color(), 1f);
                            Main.dust[index2].noGravity = true;
                            Dust dust1 = Main.dust[index2];
                            dust1.velocity = dust1.velocity * 1.5f;
                            Dust dust2 = Main.dust[index2];
                            dust2.scale = dust2.scale * 0.9f;
                        }
                    }
                    else if (projectile.type == ProjectileID.HolyArrow || projectile.type == ProjectileID.HallowStar)
                    {
                        Main.PlaySound(SoundID.Item10, projectile.position);
                        for (int index = 0; index < 10; ++index)
                            Dust.NewDust(projectile.position, projectile.width, projectile.height, 58, projectile.velocity.X * 0.1f, projectile.velocity.Y * 0.1f, 150, new Color(), 1.2f);
                        for (int index = 0; index < 3; ++index)
                            Gore.NewGore(projectile.position, new Vector2(projectile.velocity.X * 0.05f, projectile.velocity.Y * 0.05f), Main.rand.Next(16, 18), 1f);
                        if (projectile.type == 12 && projectile.damage < 500)
                        {
                            for (int index = 0; index < 10; ++index)
                                Dust.NewDust(projectile.position, projectile.width, projectile.height, 57, projectile.velocity.X * 0.1f, projectile.velocity.Y * 0.1f, 150, new Color(), 1.2f);
                            for (int index = 0; index < 3; ++index)
                                Gore.NewGore(projectile.position, new Vector2(projectile.velocity.X * 0.05f, projectile.velocity.Y * 0.05f), Main.rand.Next(16, 18), 1f);
                        }
                    }*/
                    return false;
                }
            }


            


            return true;
        }

        public override void Kill(Projectile projectile, int timeLeft)
        {
            Player player = Main.player[projectile.owner];
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();
            
            if (!townNPCProj && CanSplit && projectile.friendly && projectile.damage > 0 && !projectile.minion && projectile.aiStyle != 19)
            {
                if (modPlayer.CobaltEnchant)
                {
                    if (player.GetToggleValue("Cobalt") && player.whoAmI == Main.myPlayer && modPlayer.CobaltCD == 0 && Main.rand.Next(4) == 0)
                    {
                        Main.PlaySound(SoundID.Item, (int)player.position.X, (int)player.position.Y, 27);

                        int damage = (int)(25 * player.rangedDamage);

                        if (modPlayer.TerrariaSoul)
                        {
                            damage *= 5;
                            modPlayer.CobaltCD = 10;
                        }
                        else if (modPlayer.EarthForce || modPlayer.WizardEnchant)
                        {
                            damage *= 2;
                            modPlayer.CobaltCD = 20;
                        }
                        else
                        {
                            modPlayer.CobaltCD = 30;
                        }

                        for (int i = 0; i < 5; i++)
                        {
                            float velX = -projectile.velocity.X * Main.rand.Next(40, 70) * 0.01f + Main.rand.Next(-20, 21) * 0.4f;
                            float velY = -projectile.velocity.Y * Main.rand.Next(40, 70) * 0.01f + Main.rand.Next(-20, 21) * 0.4f;
                            int p = Projectile.NewProjectile(projectile.position.X + velX, projectile.position.Y + velY, velX, velY, ProjectileID.CrystalShard, damage, 0f, projectile.owner);
                            if (p != Main.maxProjectiles)
                                Main.projectile[p].GetGlobalProjectile<FargoGlobalProjectile>().CanSplit = false;
                        }
                    }
                }
                else if (modPlayer.AncientCobaltEnchant && !modPlayer.CobaltEnchant && player.GetToggleValue("AncientCobalt") && player.whoAmI == Main.myPlayer && modPlayer.CobaltCD == 0 && Main.rand.Next(5) == 0)
                {
                    Projectile[] projs = XWay(3, projectile.Center, ProjectileID.HornetStinger, 5f, projectile.damage / 2, 0);

                    for (int i = 0; i < projs.Length; i++)
                    {
                        projs[i].penetrate = 3;
                        projs[i].timeLeft /= 2;
                    }

                    modPlayer.CobaltCD = 60;
                }
            }
        }

        public override void UseGrapple(Player player, ref int type)
        {
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();

            if (modPlayer.JungleEnchant)
            {
                modPlayer.CanJungleJump = true;
            }
        }

        public override void GrapplePullSpeed(Projectile projectile, Player player, ref float speed)
        {
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();

            if (modPlayer.MahoganyEnchant && player.GetToggleValue("Mahogany", false))
            {
                float multiplier = 1.5f;

                if (modPlayer.WoodForce || modPlayer.WizardEnchant)
                {
                    multiplier = 2.5f;
                }

                speed *= multiplier;
            }
        }

        public override void GrappleRetreatSpeed(Projectile projectile, Player player, ref float speed)
        {
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();

            if (modPlayer.MahoganyEnchant && player.GetToggleValue("Mahogany", false))
            {
                float multiplier = 1.5f;

                if (modPlayer.WoodForce || modPlayer.WizardEnchant)
                {
                    multiplier = 2.5f;
                }

                speed *= multiplier;
            }
        }

        public static Projectile[] XWay(int num, Vector2 pos, int type, float speed, int damage, float knockback)
        {
            Projectile[] projs = new Projectile[num];
            double spread = 2 * Math.PI / num;
            for (int i = 0; i < num; i++)
                projs[i] = NewProjectileDirectSafe(pos, new Vector2(speed, speed).RotatedBy(spread * i), type, damage, knockback, Main.myPlayer);
            return projs;
        }

        public static int CountProj(int type)
        {
            int count = 0;

            for (int i = 0; i < 1000; i++)
            {
                if (Main.projectile[i].type == type)
                {
                    count++;
                }
            }

            return count;
        }

        public static Projectile NewProjectileDirectSafe(Vector2 pos, Vector2 vel, int type, int damage, float knockback, int owner = 255, float ai0 = 0f, float ai1 = 0f)
        {
            int p = Projectile.NewProjectile(pos, vel, type, damage, knockback, owner, ai0, ai1);
            return (p < 1000) ? Main.projectile[p] : null;
        }

        public static int GetByUUIDReal(int player, int projectileIdentity, params int[] projectileType)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].identity == projectileIdentity && Main.projectile[i].owner == player
                    && (projectileType.Length == 0 || projectileType.Contains(Main.projectile[i].type)))
                { return i;
                }
            }
            return -1;
        }

        public static bool IsMinionDamage(Projectile projectile)
        {
            if (projectile.melee || projectile.ranged || projectile.magic)
                return false;
            return projectile.minion || projectile.sentry || projectile.minionSlots > 0 || ProjectileID.Sets.MinionShot[projectile.type] || ProjectileID.Sets.SentryShot[projectile.type];
        }
    }
}
