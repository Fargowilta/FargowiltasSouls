using FargowiltasSouls.Buffs;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.Buffs.Souls;
using FargowiltasSouls.Items.Accessories.Enchantments;
using FargowiltasSouls.NPCs;
using FargowiltasSouls.NPCs.EternityMode;
using FargowiltasSouls.Projectiles;
using FargowiltasSouls.Projectiles.BossWeapons;
using FargowiltasSouls.Projectiles.Masomode;
using FargowiltasSouls.Projectiles.Minions;
using FargowiltasSouls.Projectiles.Souls;
using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.Graphics.Capture;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls
{
    public partial class FargoSoulsPlayer
    {
        //        #region enchantments

        public void BeeEffect(bool hideVisual)
        {
            Player.strongBees = true;
            //bees ignore defense
            BeeEnchantActive = true;
        }

        public void BeetleEffect()
        {
            if (!Player.GetToggleValue("Beetle"))
                return;

            //don't let this stack
            if (Player.beetleDefense || Player.beetleOffense)
                return;

            BeetleEnchantActive = true;

            if (BeetleEnchantDefenseTimer > 0) //do defensive beetle things
            {
                Player.beetleDefense = true;
                Player.beetleCounter += 1f;
                int cap = TerrariaSoul ? 3 : 2;
                int time = 180 * 3 / cap;
                if (Player.beetleCounter >= time)
                {
                    if (Player.beetleOrbs > 0 && Player.beetleOrbs < cap)
                    {
                        for (int k = 0; k < Player.MaxBuffs; k++)
                        {
                            if (Player.buffType[k] >= 95 && Player.buffType[k] <= 96)
                            {
                                Player.DelBuff(k);
                            }
                        }
                    }
                    if (Player.beetleOrbs < cap)
                    {
                        Player.AddBuff(BuffID.BeetleEndurance1 + Player.beetleOrbs, 5, false);
                        Player.beetleCounter = 0f;
                    }
                    else
                    {
                        Player.beetleCounter = time;
                    }
                }
            }
            else
            {
                Player.beetleOffense = true;

                Player.beetleCounter -= 3f;
                Player.beetleCounter = Player.beetleCounter - Player.beetleCountdown / 10f;

                Player.beetleCountdown += 1;

                if (Player.beetleCounter < 0)
                    Player.beetleCounter = 0;

                int beetles = 0;
                int num2 = 400;
                int num3 = 1200;
                int num4 = 4600;

                if (Player.beetleCounter > num2 + num3 + num4 + num3)
                    Player.beetleCounter = num2 + num3 + num4 + num3;
                if (TerrariaSoul && Player.beetleCounter > num2 + num3 + num4)
                {
                    Player.AddBuff(BuffID.BeetleMight3, 5, false);
                    beetles = 3;
                }
                else
                {
                    Player.buffImmune[BuffID.BeetleMight3] = true;

                    if (Player.beetleCounter > num2 + num3)
                    {
                        Player.AddBuff(BuffID.BeetleMight2, 5, false);
                        beetles = 2;
                    }
                    else if (Player.beetleCounter > num2)
                    {
                        Player.AddBuff(BuffID.BeetleMight1, 5, false);
                        beetles = 1;
                    }
                }

                if (beetles < Player.beetleOrbs)
                    Player.beetleCountdown = 0;
                else if (beetles > Player.beetleOrbs)
                    Player.beetleCounter = Player.beetleCounter + 200f;

                float damage = beetles * 0.10f;
                Player.GetDamage(DamageClass.Generic) += damage;
                Player.GetDamage(DamageClass.Melee) -= damage; //offset the actual vanilla beetle buff

                if (beetles != Player.beetleOrbs && Player.beetleOrbs > 0)
                {
                    for (int b = 0; b < Player.MaxBuffs; ++b)
                    {
                        if (Player.buffType[b] >= 98 && Player.buffType[b] <= 100 && Player.buffType[b] != 97 + beetles)
                            Player.DelBuff(b);
                    }
                }
            }

            //vanilla code for beetle visuals
            if (!Player.beetleDefense && !Player.beetleOffense)
            {
                Player.beetleCounter = 0f;
            }
            else
            {
                Player.beetleFrameCounter++;
                if (Player.beetleFrameCounter >= 1)
                {
                    Player.beetleFrameCounter = 0;
                    Player.beetleFrame++;
                    if (Player.beetleFrame > 2)
                    {
                        Player.beetleFrame = 0;
                    }
                }
                for (int l = Player.beetleOrbs; l < 3; l++)
                {
                    Player.beetlePos[l].X = 0f;
                    Player.beetlePos[l].Y = 0f;
                }
                for (int m = 0; m < Player.beetleOrbs; m++)
                {
                    Player.beetlePos[m] += Player.beetleVel[m];
                    Vector2[] expr_6EcCp0 = Player.beetleVel;
                    int expr_6EcCp1 = m;
                    expr_6EcCp0[expr_6EcCp1].X = expr_6EcCp0[expr_6EcCp1].X + Main.rand.Next(-100, 101) * 0.005f;
                    Vector2[] expr71ACp0 = Player.beetleVel;
                    int expr71ACp1 = m;
                    expr71ACp0[expr71ACp1].Y = expr71ACp0[expr71ACp1].Y + Main.rand.Next(-100, 101) * 0.005f;
                    float num6 = Player.beetlePos[m].X;
                    float num7 = Player.beetlePos[m].Y;
                    float num8 = (float)Math.Sqrt(num6 * num6 + num7 * num7);
                    if (num8 > 100f)
                    {
                        num8 = 20f / num8;
                        num6 *= -num8;
                        num7 *= -num8;
                        int num9 = 10;
                        Player.beetleVel[m].X = (Player.beetleVel[m].X * (num9 - 1) + num6) / num9;
                        Player.beetleVel[m].Y = (Player.beetleVel[m].Y * (num9 - 1) + num7) / num9;
                    }
                    else if (num8 > 30f)
                    {
                        num8 = 10f / num8;
                        num6 *= -num8;
                        num7 *= -num8;
                        int num10 = 20;
                        Player.beetleVel[m].X = (Player.beetleVel[m].X * (num10 - 1) + num6) / num10;
                        Player.beetleVel[m].Y = (Player.beetleVel[m].Y * (num10 - 1) + num7) / num10;
                    }
                    num6 = Player.beetleVel[m].X;
                    num7 = Player.beetleVel[m].Y;
                    num8 = (float)Math.Sqrt(num6 * num6 + num7 * num7);
                    if (num8 > 2f)
                    {
                        Player.beetleVel[m] *= 0.9f;
                    }
                    Player.beetlePos[m] -= Player.velocity * 0.25f;
                }
            }
        }

        public void BeetleHurt()
        {
            BeetleEnchantDefenseTimer = 600;

            //AFTER THIS DAMAGE, transfer all offense beetles to endurance instead
            //doesnt really work rn, only trasnfers 1 beetle, but tbh its more balanced this way
            if (Player.whoAmI == Main.myPlayer)
            {
                int strongestBuff = -1;
                for (int i = 0; i < Player.MaxBuffs; i++)
                {
                    if (Player.buffTime[i] > 0)
                    {
                        if (Player.buffType[i] == BuffID.BeetleMight1 || Player.buffType[i] == BuffID.BeetleMight2 || Player.buffType[i] == BuffID.BeetleMight3)
                        {
                            if (strongestBuff < Player.buffType[i])
                                strongestBuff = Player.buffType[i];

                            Player.DelBuff(i);
                            i -= 1;
                        }
                    }
                }

                if (strongestBuff != -1)
                {
                    int offset = BuffID.BeetleMight1 - strongestBuff;
                    Player.AddBuff(BuffID.BeetleEndurance1 + offset, 5, false);
                }
            }
        }

        public void ChloroEffect(Item item, bool hideVisual)
        {
            ChloroEnchantActive = true;

            ChloroEnchantItem = item;

            if (Player.whoAmI == Main.myPlayer && Player.GetToggleValue("Chlorophyte") && Player.ownedProjectileCounts[ModContent.ProjectileType<Chlorofuck>()] == 0)
            {
                int dmg = NatureForce ? 150 : 75;
                const int max = 5;
                float rotation = 2f * (float)Math.PI / max;
                for (int i = 0; i < max; i++)
                {
                    Vector2 spawnPos = Player.Center + new Vector2(60, 0f).RotatedBy(rotation * i);
                    FargoSoulsUtil.NewSummonProjectile(Player.GetSource_Misc(""), spawnPos, Vector2.Zero, ModContent.ProjectileType<Chlorofuck>(), dmg, 10f, Player.whoAmI, 0, rotation * i);
                }
            }
        }

        public void DarkArtistEffect(bool hideVisual)
        {
            if (Player.ownedProjectileCounts[ModContent.ProjectileType<FlameburstMinion>()] == 0)
            {
                DarkArtistSpawn = true;
                //DarkSpawnCD = 60;
            }

            ApprenticeEnchantActive = true;
            DarkArtistEnchantActive = true;

            //int maxTowers = 3;

            //if (TerrariaSoul)
            //{
            //    maxTowers = 5;
            //}
            //else if (ShadowForce)
            //{
            //    maxTowers = 4;
            //}

            //spawn tower boi
            if (Player.whoAmI == Main.myPlayer && DarkArtistSpawn && DarkArtistSpawnCD <= 0 && Player.GetToggleValue("DarkArt"))
            //&& Player.ownedProjectileCounts[ModContent.ProjectileType<FlameburstMinion>()] < maxTowers)
            {
                Projectile proj = Projectile.NewProjectileDirect(Player.GetSource_Misc(""), Player.Center, Vector2.Zero, ModContent.ProjectileType<FlameburstMinion>(), 0, 0f, Player.whoAmI);
                proj.netUpdate = true; // TODO make this proj sync meme

                DarkArtistSpawn = false;
                DarkArtistSpawnCD = 60;
            }

            if (DarkArtistSpawnCD > 0)
            {
                DarkArtistSpawnCD--;
            }


        }

        public void ForbiddenEffect()
        {
            if (!Player.GetToggleValue("Forbidden"))
                return;
            ForbiddenEnchantActive = true;

            //Player.setForbidden = true;
            //add cd

            if (DoubleTap)
            {
                CommandForbiddenStorm();

                /*Vector2 mouse = Main.MouseWorld;

                if (Player.ownedProjectileCounts[ModContent.ProjectileType<ForbiddenTornado>()] > 0)
                {
                    for (int i = 0; i < 1000; i++)
                    {
                        Projectile proj = Main.projectile[i];

                        if (proj.type == ModContent.ProjectileType<ForbiddenTornado>())
                        {
                            proj.Kill();
                        }
                    }
                }

                Projectile.NewProjectile(mouse.X, mouse.Y - 10, 0f, 0f, ModContent.ProjectileType<ForbiddenTornado>(), (WoodForce) ? 45 : 15, 0f, Player.whoAmI);*/
            }


            //Player.UpdateForbiddenSetLock();
            Lighting.AddLight(Player.Center, 0.8f, 0.7f, 0.2f);
            //storm boosted
        }

        public void CommandForbiddenStorm()
        {
            List<int> list = new List<int>();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile projectile = Main.projectile[i];
                if (projectile.active && projectile.type == ModContent.ProjectileType<ForbiddenTornado>() && projectile.owner == Player.whoAmI)
                {
                    list.Add(i);
                }
            }

            Vector2 center = Player.Center;
            Vector2 mouse = Main.MouseWorld;

            bool flag3 = false;
            float[] array = new float[10];
            Vector2 v = mouse - center;
            Collision.LaserScan(center, v.SafeNormalize(Vector2.Zero), 60f, v.Length(), array);
            float num = 0f;
            for (int j = 0; j < array.Length; j++)
            {
                if (array[j] > num)
                {
                    num = array[j];
                }
            }
            float[] array2 = array;
            for (int k = 0; k < array2.Length; k++)
            {
                float num2 = array2[k];
                if (Math.Abs(num2 - v.Length()) < 10f)
                {
                    flag3 = true;
                    break;
                }
            }
            if (list.Count <= 1)
            {
                Vector2 vector = center + v.SafeNormalize(Vector2.Zero) * num;
                Vector2 value2 = vector - center;
                if (value2.Length() > 0f)
                {
                    for (float num3 = 0f; num3 < value2.Length(); num3 += 15f)
                    {
                        Vector2 position = center + value2 * (num3 / value2.Length());
                        Dust dust = Main.dust[Dust.NewDust(position, 0, 0, 269, 0f, 0f, 0, default(Color), 1f)];
                        dust.position = position;
                        dust.fadeIn = 0.5f;
                        dust.scale = 0.7f;
                        dust.velocity *= 0.4f;
                        dust.noLight = true;
                    }
                }
                for (float num4 = 0f; num4 < 6.28318548f; num4 += 0.209439516f)
                {
                    Dust dust2 = Main.dust[Dust.NewDust(vector, 0, 0, 269, 0f, 0f, 0, default(Color), 1f)];
                    dust2.position = vector;
                    dust2.fadeIn = 1f;
                    dust2.scale = 0.3f;
                    dust2.noLight = true;
                }
            }

            //Main.NewText(" " + (list.Count <= 1) + " " + flag3 + " " + Player.CheckMana(20, true, false));

            bool flag = (list.Count <= 1);
            flag &= flag3;



            if (flag)
            {
                flag = Player.CheckMana(20, true, false);
                if (flag)
                {
                    Player.manaRegenDelay = (int)Player.maxRegenDelay;
                }
            }
            if (!flag)
            {
                return;
            }
            foreach (int current in list)
            {
                Projectile projectile2 = Main.projectile[current];
                if (projectile2.ai[0] < 780f)
                {
                    projectile2.ai[0] = 780f + projectile2.ai[0] % 60f;
                    projectile2.netUpdate = true;
                }
            }

            int damage = (int)(20f * (1f + Player.GetDamage(DamageClass.Magic).Additive + Player.GetDamage(DamageClass.Summon).Additive - 2f));
            Projectile.NewProjectile(Player.GetSource_Misc(""), mouse, Vector2.Zero, ModContent.ProjectileType<ForbiddenTornado>(), damage, 0f, Main.myPlayer, 0f, 0f);
        }



        public void FrostEffect(bool hideVisual)
        {
            FrostEnchantActive = true;

            if (Player.whoAmI == Main.myPlayer && Player.GetToggleValue("Frost"))
            {
                if (icicleCD <= 0 && IcicleCount < 10 && Player.ownedProjectileCounts[ModContent.ProjectileType<FrostIcicle>()] < 10)
                {
                    IcicleCount++;

                    //kill all current ones
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        Projectile proj = Main.projectile[i];

                        if (proj.active && proj.type == ModContent.ProjectileType<FrostIcicle>() && proj.owner == Player.whoAmI)
                        {
                            proj.active = false;
                            proj.netUpdate = true;
                        }
                    }

                    //respawn in formation
                    for (int i = 0; i < IcicleCount; i++)
                    {
                        float radians = (360f / IcicleCount) * i * (float)(Math.PI / 180);
                        Projectile frost = FargoSoulsUtil.NewProjectileDirectSafe(Player.GetSource_Misc(""), Player.Center, Vector2.Zero, ModContent.ProjectileType<FrostIcicle>(), 0, 0f, Player.whoAmI, 5, radians);
                        frost.netUpdate = true;
                    }

                    float dustScale = 1.5f;

                    if (IcicleCount % 10 == 0)
                    {
                        dustScale = 3f;
                    }

                    //dust
                    for (int j = 0; j < 20; j++)
                    {
                        Vector2 vector6 = Vector2.UnitY * 5f;
                        vector6 = vector6.RotatedBy((j - (20 / 2 - 1)) * 6.28318548f / 20) + Player.Center;
                        Vector2 vector7 = vector6 - Player.Center;
                        int d = Dust.NewDust(vector6 + vector7, 0, 0, 15);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].scale = dustScale;
                        Main.dust[d].velocity = vector7;

                        if (IcicleCount % 10 == 0)
                        {
                            Main.dust[d].velocity *= 2;
                        }
                    }

                    icicleCD = 30;
                }

                if (icicleCD > 0)
                    icicleCD--;

                if (IcicleCount >= 1 && Player.controlUseItem && Player.HeldItem.damage > 0 && Player.HeldItem.createTile == -1 && Player.HeldItem.createWall == -1 && Player.HeldItem.ammo == AmmoID.None && Player.HeldItem.hammer == 0 && Player.HeldItem.pick == 0 && Player.HeldItem.axe == 0)
                {
                    int dmg = NatureForce ? 100 : 50;

                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        Projectile proj = Main.projectile[i];

                        if (proj.active && proj.type == ModContent.ProjectileType<FrostIcicle>() && proj.owner == Player.whoAmI)
                        {
                            Vector2 vel = (Main.MouseWorld - proj.Center).SafeNormalize(-Vector2.UnitY) * 20f;

                            int p = Projectile.NewProjectile(Player.GetSource_Misc(""), proj.Center, vel, ProjectileID.Blizzard, FargoSoulsUtil.HighestDamageTypeScaling(Player, dmg), 1f, Player.whoAmI);
                            if (p != Main.maxProjectiles)
                            {
                                Main.projectile[p].GetGlobalProjectile<FargoSoulsGlobalProjectile>().CanSplit = false;
                                Main.projectile[p].GetGlobalProjectile<FargoSoulsGlobalProjectile>().FrostFreeze = true;
                            }

                            proj.Kill();
                        }
                    }

                    IcicleCount = 0;
                    icicleCD = 120;
                }
            }


        }


        public void GoldEffect(bool hideVisual)
        {
            //gold ring
            Player.goldRing = true;
            //lucky coin
            if (Player.whoAmI == Main.myPlayer && Player.GetToggleValue("Gold"))
                Player.hasLuckyCoin = true;
            //discount card
            Player.discount = true;
            //midas
            GoldEnchantActive = true;

            if (Player.GetToggleValue("GoldToPiggy", false))
            {
                for (int i = 50; i <= 53; i++) //detect coins in coin slots
                {
                    if (!Player.inventory[i].IsAir && Player.inventory[i].IsACoin)
                        GoldEnchMoveCoins = true;
                }
            }
        }

        public void GoldKey()
        {
            if (!Player.HasBuff(ModContent.BuffType<GoldenStasis>()) && !Player.HasBuff(ModContent.BuffType<GoldenStasisCD>()))
            {
                int duration = 300;

                if (WillForce)
                {
                    duration *= 2;
                }

                Player.AddBuff(ModContent.BuffType<GoldenStasis>(), duration);
                Player.AddBuff(ModContent.BuffType<GoldenStasisCD>(), 3600);

                goldHP = Player.statLife;

                if (!Main.dedServ)
                    SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Sounds/Zhonyas"), Player.Center);
            }
            //cancel it early
            else
            {
                Player.ClearBuff(ModContent.BuffType<GoldenStasis>());
            }
        }

        public void GoldUpdate()
        {
            Player.immune = true;
            Player.immuneTime = 90;
            Player.hurtCooldowns[0] = 90;
            Player.hurtCooldowns[1] = 90;
            Player.stealth = 1;

            //immune to DoT
            if (Player.statLife < goldHP)
                Player.statLife = goldHP;

            if (Player.ownedProjectileCounts[ModContent.ProjectileType<GoldShellProj>()] <= 0)
                Projectile.NewProjectile(Player.GetSource_Misc(""), Player.Center.X, Player.Center.Y, 0f, 0f, ModContent.ProjectileType<GoldShellProj>(), 0, 0, Main.myPlayer);
        }

        

        public bool CanJungleJump = false;
        public bool JungleJumping = false;
        private int savedRocketTime;

        public void JungleDash()
        {
            if (Player.mount.Active)
                return;

            if (HasDash)
                return;

            HasDash = true;

            if (dashCD <= 0)
            {
                float dashSpeed = ChloroEnchantActive ? 12f : 9f;

                if (Player.controlRight && Player.releaseRight)
                {
                    if (Player.doubleTapCardinalTimer[2] > 0 && Player.doubleTapCardinalTimer[2] != 15)
                    {
                        dashCD = 60;
                        if (IsDashingTimer < 10)
                            IsDashingTimer = 10;
                        Player.velocity.X = dashSpeed;
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                            NetMessage.SendData(MessageID.PlayerControls, number: Player.whoAmI);
                    }
                }

                if (Player.controlLeft && Player.releaseLeft)
                {
                    if (Player.doubleTapCardinalTimer[3] > 0 && Player.doubleTapCardinalTimer[3] != 15)
                    {
                        dashCD = 60;
                        if (IsDashingTimer < 10)
                            IsDashingTimer = 10;
                        Player.velocity.X = -dashSpeed;
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                            NetMessage.SendData(MessageID.PlayerControls, number: Player.whoAmI);
                    }
                }
            }

            if (IsDashingTimer > 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    int d = Dust.NewDust(Player.position, Player.width, Player.height, DustID.JunglePlants, Scale: 1.5f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 0.2f;
                }
            }
        }

        public void JungleEffect()
        {
            if (Player.whoAmI != Main.myPlayer)
                return;

            if (Player.GetToggleValue("JungleDash", false))
                JungleDash();

            if (Player.grapCount > 0)
            {
                CanJungleJump = true;
                JungleJumping = false;
            }
            else if (Player.controlJump && Player.GetToggleValue("Jungle"))
            {
                if (Player.canJumpAgain_Blizzard || Player.canJumpAgain_Sandstorm || Player.canJumpAgain_Cloud || Player.canJumpAgain_Fart || Player.canJumpAgain_Sail || Player.canJumpAgain_Unicorn)
                {
                }
                else
                {
                    if (Player.jump == 0 && Player.releaseJump && Player.velocity.Y != 0f && !Player.mount.Active && CanJungleJump)
                    {
                        Player.velocity.Y = -Player.jumpSpeed * Player.gravDir;
                        Player.jump = (int)((double)Player.jumpHeight * 3);

                        JungleJumping = true;
                        JungleCD = 0;
                        CanJungleJump = false;

                        if (Main.netMode == NetmodeID.MultiplayerClient)
                            NetMessage.SendData(MessageID.PlayerControls, number: Player.whoAmI);
                    }
                }
            }

            if (JungleJumping && Player.GetToggleValue("Jungle"))
            {
                if (Player.rocketBoots > 0)
                {
                    savedRocketTime = Player.rocketTimeMax;
                    Player.rocketTime = 0;
                }

                Player.runAcceleration *= 3f;
                //Player.maxRunSpeed *= 2f;

                //spwn cloud
                if (JungleCD == 0)
                {
                    int tier = 1;
                    if (ChloroEnchantActive)
                        tier++;
                    if (NatureForce)
                        tier++;

                    JungleCD = 17 - tier * tier;
                    int dmg = 12 * tier * tier;

                    SoundEngine.PlaySound(SoundID.Item62 with { Volume = 0.5f }, Player.Center);

                    foreach (Projectile p in FargoSoulsUtil.XWay(10, Player.GetSource_Misc(""), Player.Bottom, ProjectileID.SporeCloud, 4f, FargoSoulsUtil.HighestDamageTypeScaling(Player, dmg), 0f))
                    {
                        if (p == null)
                            continue;
                        p.usesIDStaticNPCImmunity = true;
                        p.idStaticNPCHitCooldown = 10;
                        p.GetGlobalProjectile<FargoSoulsGlobalProjectile>().noInteractionWithNPCImmunityFrames = true;
                    }
                }

                if (Player.jump == 0 || Player.velocity == Vector2.Zero)
                {
                    JungleJumping = false;
                    Player.rocketTime = savedRocketTime;
                }
            }
            else if (Player.jump <= 0 && Player.velocity.Y == 0f)
            {
                CanJungleJump = true;
            }

            if (JungleCD != 0)
            {
                JungleCD--;
            }
        }

        public void MeteorEffect(Item item)
        {
            MeteorEnchantItem = item;

            if (Player.whoAmI == Main.myPlayer && Player.GetToggleValue("Meteor"))
            {
                int damage = CosmoForce ? 50 : 20;

                if (MeteorShower)
                {
                    if (MeteorTimer % (CosmoForce ? 2 : 4) == 0)
                    {
                        Vector2 pos = new Vector2(Player.Center.X + Main.rand.NextFloat(-1000, 1000), Player.Center.Y - 1000);
                        Vector2 vel = new Vector2(Main.rand.NextFloat(-2, 2), Main.rand.NextFloat(8, 12));
                        
                        //chance to focus on a nearby enemy with slight predictive aim
                        if (Main.rand.NextBool())
                        {
                            List<NPC> targetables = Main.npc.Where(n => n.CanBeChasedBy() && n.Distance(Player.Center) < 900).ToList();
                            if (targetables.Count > 0)
                            {
                                NPC target = targetables[Main.rand.Next(targetables.Count)];
                                pos.X = target.Center.X + Main.rand.NextFloat(-32, 32);

                                //can retarget better at them, but dont aim meteors upwards
                                Vector2 predictive = Main.rand.NextFloat(10f, 30f) * target.velocity;
                                pos.X += predictive.X;
                                Vector2 targetPos = target.Center + predictive;
                                if (pos.Y < targetPos.Y)
                                {
                                    Vector2 accurateVel = vel.Length() * pos.DirectionTo(targetPos);
                                    vel = Vector2.Lerp(vel, accurateVel, Main.rand.NextFloat());
                                }
                            }
                        }

                        Projectile.NewProjectile(Player.GetSource_Accessory(item), pos, vel, Main.rand.Next(424, 427), FargoSoulsUtil.HighestDamageTypeScaling(Player, damage), 0.5f, Player.whoAmI, 0, 0.5f + (float)Main.rand.NextDouble() * 0.3f);
                    }

                    if (--MeteorTimer <= 0)
                    {
                        MeteorShower = false;
                        MeteorCD = CosmoForce ? 240 : 480;
                    }
                }
                else
                {
                    MeteorTimer = 150 + MeteorEnchant.METEOR_ADDED_DURATION / (CosmoForce ? 1 : 2);

                    if (WeaponUseTimer > 0)
                    {
                        if (--MeteorCD <= 0)
                            MeteorShower = true;
                    }
                    else if (MeteorCD < 150) //when not using weapons, gradually increment back up
                    {
                        MeteorCD++;
                    }
                }
            }
        }

        public void NebulaEffect()
        {
            if (!Player.GetToggleValue("Nebula", false))
                return;

            if (Player.setNebula)
                return;

            Player.setNebula = true;
            if (Player.nebulaCD > 0)
                Player.nebulaCD--;

            if (!TerrariaSoul && !CosmoForce) //cap boosters
            {
                void DecrementBuff(int buffType)
                {
                    for (int i = 0; i < Player.buffType.Length; i++)
                    {
                        if (Player.buffType[i] == buffType && Player.buffTime[i] > 3)
                        {
                            Player.buffTime[i] = 3;
                            break;
                        }
                    }
                };

                if (Player.nebulaLevelDamage == 3)
                    DecrementBuff(BuffID.NebulaUpDmg3);
                if (Player.nebulaLevelLife == 3)
                    DecrementBuff(BuffID.NebulaUpLife3);
                if (Player.nebulaLevelMana == 3)
                    DecrementBuff(BuffID.NebulaUpMana3);
            }
        }

        public void NebulaOnHit(NPC target, Projectile projectile, DamageClass damageClass)
        {
            if (damageClass != DamageClass.Magic && Player.nebulaCD <= 0 && Main.rand.NextBool(3))
            {
                Player.nebulaCD = 30;
                int num35 = Utils.SelectRandom(Main.rand, new int[]
                {
                                                            3453,
                                                            3454,
                                                            3455
                });
                int i = Item.NewItem(Player.GetSource_OpenItem(num35), (int)target.position.X, (int)target.position.Y, target.width, target.height, num35, 1, false, 0, false, false);
                Main.item[i].velocity.Y = Main.rand.Next(-20, 1) * 0.2f;
                Main.item[i].velocity.X = Main.rand.Next(10, 31) * 0.2f * (projectile == null ? Player.direction : projectile.direction);
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, i);
                }
            }
        }

        

        public void ShadowEffect(bool hideVisual)
        {
            ShadowEnchantActive = true;
        }

        public void ShadowEffectPostEquips()
        {
            if (Player.whoAmI == Main.myPlayer && Player.GetToggleValue("Shadow"))
            {
                int currentOrbs = Player.ownedProjectileCounts[ModContent.ProjectileType<ShadowEnchantOrb>()];

                int max = 2;

                if (TerrariaSoul)
                {
                    max = 5;
                }
                else if (ShadowForce)
                {
                    max = 4;
                }
                else if (AncientShadowEnchantActive)
                {
                    max = 3;
                }

                //spawn for first time
                if (currentOrbs == 0)
                {
                    float rotation = 2f * (float)Math.PI / max;

                    for (int i = 0; i < max; i++)
                    {
                        Vector2 spawnPos = Player.Center + new Vector2(60, 0f).RotatedBy(rotation * i);
                        int p = Projectile.NewProjectile(Player.GetSource_Misc(""), spawnPos, Vector2.Zero, ModContent.ProjectileType<ShadowEnchantOrb>(), 0, 10f, Player.whoAmI, 0, rotation * i);
                        Main.projectile[p].GetGlobalProjectile<FargoSoulsGlobalProjectile>().CanSplit = false;
                    }
                }
                //equipped somwthing that allows for more or less, respawn
                else if (currentOrbs != max)
                {
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        Projectile proj = Main.projectile[i];

                        if (proj.active && proj.type == ModContent.ProjectileType<ShadowEnchantOrb>() && proj.owner == Player.whoAmI)
                        {
                            proj.Kill();
                        }
                    }

                    float rotation = 2f * (float)Math.PI / max;

                    for (int i = 0; i < max; i++)
                    {
                        Vector2 spawnPos = Player.Center + new Vector2(60, 0f).RotatedBy(rotation * i);
                        int p = Projectile.NewProjectile(Player.GetSource_Misc(""), spawnPos, Vector2.Zero, ModContent.ProjectileType<ShadowEnchantOrb>(), 0, 10f, Player.whoAmI, 0, rotation * i);
                        Main.projectile[p].GetGlobalProjectile<FargoSoulsGlobalProjectile>().CanSplit = false;
                    }
                }
            }
        }

        private void ShinobiDash(int direction)
        {
            dashCD = 90;

            var teleportPos = Player.position;

            const int length = 400; //make sure this is divisible by 16 btw

            if (Player.GetToggleValue("Shinobi")) //go through walls
            {
                teleportPos.X += length * direction;

                while (Collision.SolidCollision(teleportPos, Player.width, Player.height))
                {
                    if (direction == 1)
                    {
                        teleportPos.X++;
                    }
                    else
                    {
                        teleportPos.X--;
                    }
                }
            }
            else
            {
                for (int i = 0; i < length; i += 16)
                {
                    if (DeerclawpsItem != null)
                    {
                        if (Player.whoAmI == Main.myPlayer && Player.GetToggleValue("Deerclawps"))
                        {
                            DeerclawpsAttack(new Vector2(teleportPos.X, Player.Bottom.Y));
                        }
                    }

                    teleportPos.X += 16 * direction;

                    if (Collision.SolidCollision(teleportPos, Player.width, Player.height))
                    {
                        teleportPos.X -= 16 * direction;
                        break;
                    }
                }
            }

            if (teleportPos.X > 50 && teleportPos.X < (double)(Main.maxTilesX * 16 - 50) && teleportPos.Y > 50 && teleportPos.Y < (double)(Main.maxTilesY * 16 - 50))
            {
                FargoSoulsUtil.GrossVanillaDodgeDust(Player);
                Player.Teleport(teleportPos, 1);
                FargoSoulsUtil.GrossVanillaDodgeDust(Player);
                NetMessage.SendData(MessageID.Teleport, -1, -1, null, 0, Player.whoAmI, teleportPos.X, teleportPos.Y, 1);

                Player.velocity.X = 12f * direction;
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    NetMessage.SendData(MessageID.PlayerControls, number: Player.whoAmI);
            }
        }

        private void ShinobiDashChecks()
        {
            if (HasDash)
                return;

            HasDash = true;

            if (Player.GetToggleValue("ShinobiDash") && Player.whoAmI == Main.myPlayer & dashCD <= 0 && !Player.mount.Active)
            {
                if ((Player.controlRight && Player.releaseRight))
                {
                    if (Player.doubleTapCardinalTimer[2] > 0 && Player.doubleTapCardinalTimer[2] != 15)
                    {
                        ShinobiDash(1);
                    }
                }

                if ((Player.controlLeft && Player.releaseLeft))
                {
                    if (Player.doubleTapCardinalTimer[3] > 0 && Player.doubleTapCardinalTimer[3] != 15)
                    {
                        ShinobiDash(-1);
                    }
                }
            }
        }

        public void ShinobiEffect(bool hideVisual)
        {
            //tele through wall until open space on dash into wall
            /*if (Player.GetToggleValue("Shinobi") && Player.whoAmI == Main.myPlayer && Player.dashDelay == -1 && Player.mount.Type == -1 && Player.velocity.X == 0)
            {
                var teleportPos = new Vector2();
                int direction = Player.direction;

                teleportPos.X = Player.position.X + direction;
                teleportPos.Y = Player.position.Y;

                while (Collision.SolidCollision(teleportPos, Player.width, Player.height))
                {
                    if (direction == 1)
                    {
                        teleportPos.X++;
                    }
                    else
                    {
                        teleportPos.X--;
                    }
                }
                if (teleportPos.X > 50 && teleportPos.X < (double)(Main.maxTilesX * 16 - 50) && teleportPos.Y > 50 && teleportPos.Y < (double)(Main.maxTilesY * 16 - 50))
                {
                    Player.Teleport(teleportPos, 1);
                    NetMessage.SendData(MessageID.Teleport, -1, -1, null, 0, Player.whoAmI, teleportPos.X, teleportPos.Y, 1);
                }
            }*/

            ShinobiEnchantActive = true;
            MonkEnchantActive = true;
        }

        public void ShroomiteEffect(bool hideVisual)
        {
            if (!TerrariaSoul && Player.GetToggleValue("Shroomite"))
                Player.shroomiteStealth = true;

            ShroomEnchantActive = true;
        }

        public void ShroomiteMeleeEffect(Item item, Rectangle hitbox)
        {
            if (Player.stealth == 0 && !item.noMelee && (Player.itemAnimation == (int)((double)Player.itemAnimationMax * 0.1) || Player.itemAnimation == (int)((double)Player.itemAnimationMax * 0.3) || Player.itemAnimation == (int)((double)Player.itemAnimationMax * 0.5) || Player.itemAnimation == (int)((double)Player.itemAnimationMax * 0.7) || Player.itemAnimation == (int)((double)Player.itemAnimationMax * 0.9)))
            {
                //hellish code from hammush
                float num340 = 0f;
                float num341 = 0f;
                float num342 = 0f;
                float num343 = 0f;
                if (Player.itemAnimation == (int)((double)Player.itemAnimationMax * 0.9))
                {
                    num340 = -7f;
                }
                if (Player.itemAnimation == (int)((double)Player.itemAnimationMax * 0.7))
                {
                    num340 = -6f;
                    num341 = 2f;
                }
                if (Player.itemAnimation == (int)((double)Player.itemAnimationMax * 0.5))
                {
                    num340 = -4f;
                    num341 = 4f;
                }
                if (Player.itemAnimation == (int)((double)Player.itemAnimationMax * 0.3))
                {
                    num340 = -2f;
                    num341 = 6f;
                }
                if (Player.itemAnimation == (int)((double)Player.itemAnimationMax * 0.1))
                {
                    num341 = 7f;
                }
                if (Player.itemAnimation == (int)((double)Player.itemAnimationMax * 0.7))
                {
                    num343 = 26f;
                }
                if (Player.itemAnimation == (int)((double)Player.itemAnimationMax * 0.3))
                {
                    num343 -= 4f;
                    num342 -= 20f;
                }
                if (Player.itemAnimation == (int)((double)Player.itemAnimationMax * 0.1))
                {
                    num342 += 6f;
                }
                if (Player.direction == -1)
                {
                    if (Player.itemAnimation == (int)((double)Player.itemAnimationMax * 0.9))
                    {
                        num343 -= 8f;
                    }
                    if (Player.itemAnimation == (int)((double)Player.itemAnimationMax * 0.7))
                    {
                        num343 -= 6f;
                    }
                }
                num340 *= 1.5f;
                num341 *= 1.5f;
                num343 *= (float)Player.direction;
                num342 *= Player.gravDir;
                Projectile.NewProjectile(Player.GetSource_ItemUse(item), (float)(hitbox.X + hitbox.Width / 2) + num343, (float)(hitbox.Y + hitbox.Height / 2) + num342, (float)Player.direction * num341, num340 * Player.gravDir, ModContent.ProjectileType<ShroomiteShroom>(), item.damage / 5, 0f, Player.whoAmI, 0f, 0);
            }
        }

        public void SolarEffect()
        {
            if (!Player.GetToggleValue("Solar"))
                return;

            if (!Player.setSolar && !TerrariaSoul) //nerf DR
            {
                Player.endurance -= 0.15f;
            }

            Player.AddBuff(BuffID.SolarShield3, 5, false);

            Player.setSolar = true;
            int solarCD = 240;
            if (++Player.solarCounter >= solarCD)
            {
                if (Player.solarShields > 0 && Player.solarShields < 3)
                {
                    for (int i = 0; i < Player.MaxBuffs; i++)
                    {
                        if (Player.buffType[i] >= BuffID.SolarShield1 && Player.buffType[i] <= BuffID.SolarShield2)
                        {
                            Player.DelBuff(i);
                        }
                    }
                }
                if (Player.solarShields < 3)
                {
                    Player.AddBuff(BuffID.SolarShield1 + Player.solarShields, 5, false);
                    for (int i = 0; i < 16; i++)
                    {
                        Dust dust = Main.dust[Dust.NewDust(Player.position, Player.width, Player.height, 6, 0f, 0f, 100)];
                        dust.noGravity = true;
                        dust.scale = 1.7f;
                        dust.fadeIn = 0.5f;
                        dust.velocity *= 5f;
                    }
                    Player.solarCounter = 0;
                }
                else
                {
                    Player.solarCounter = solarCD;
                }
            }
            for (int i = Player.solarShields; i < 3; i++)
            {
                Player.solarShieldPos[i] = Vector2.Zero;
            }
            for (int i = 0; i < Player.solarShields; i++)
            {
                Player.solarShieldPos[i] += Player.solarShieldVel[i];
                Vector2 value = (Player.miscCounter / 100f * 6.28318548f + i * (6.28318548f / Player.solarShields)).ToRotationVector2() * 6f;
                value.X = Player.direction * 20;
                Player.solarShieldVel[i] = (value - Player.solarShieldPos[i]) * 0.2f;
            }
            if (Player.dashDelay >= 0)
            {
                Player.solarDashing = false;
                Player.solarDashConsumedFlare = false;
            }
            bool flag = Player.solarDashing && Player.dashDelay < 0;
            if (Player.solarShields > 0 || flag)
            {
                Player.dashType = 3;
                HasDash = true; //doesnt check this itself, so overrides most other dashes(?)
            }
        }

        public void SpectreEffect(bool hideVisual)
        {
            SpectreEnchantActive = true;

        }

        public void SpectreHeal(NPC npc, Projectile proj)
        {
            if (npc.canGhostHeal && !Player.moonLeech)
            {
                float num = 0.2f;
                num -= proj.numHits * 0.05f;
                if (num <= 0f)
                {
                    return;
                }
                float num2 = proj.damage * num;
                if ((int)num2 <= 0)
                {
                    return;
                }
                if (Main.player[Main.myPlayer].lifeSteal <= 0f)
                {
                    return;
                }
                Main.player[Main.myPlayer].lifeSteal -= num2 * 5; //original damage

                float num3 = 0f;
                int num4 = proj.owner;
                for (int i = 0; i < 255; i++)
                {
                    if (Main.player[i].active && !Main.player[i].dead && ((!Main.player[proj.owner].hostile && !Main.player[i].hostile) || Main.player[proj.owner].team == Main.player[i].team))
                    {
                        float num5 = Math.Abs(Main.player[i].position.X + (Main.player[i].width / 2) - proj.position.X + (proj.width / 2)) + Math.Abs(Main.player[i].position.Y + (Main.player[i].height / 2) - proj.position.Y + (proj.height / 2));
                        if (num5 < 1200f && (Main.player[i].statLifeMax2 - Main.player[i].statLife) > num3)
                        {
                            num3 = (Main.player[i].statLifeMax2 - Main.player[i].statLife);
                            num4 = i;
                        }
                    }
                }
                Projectile.NewProjectile(proj.GetSource_FromThis(), proj.position.X, proj.position.Y, 0f, 0f, ProjectileID.SpiritHeal, 0, 0f, proj.owner, num4, num2);
            }
        }

        public void SpectreHurt(Projectile proj)
        {
            int num = proj.damage / 2;
            if (proj.damage / 2 <= 1)
            {
                return;
            }
            int num2 = 1000;
            if (Main.player[Main.myPlayer].ghostDmg > (float)num2)
            {
                return;
            }
            Main.player[Main.myPlayer].ghostDmg += (float)num;
            int[] array = new int[200];
            int num3 = 0;
            int num4 = 0;
            for (int i = 0; i < 200; i++)
            {
                if (Main.npc[i].CanBeChasedBy(this, false))
                {
                    float num5 = Math.Abs(Main.npc[i].position.X + (Main.npc[i].width / 2) - proj.position.X + (proj.width / 2)) + Math.Abs(Main.npc[i].position.Y + (Main.npc[i].height / 2) - proj.position.Y + (proj.height / 2));
                    if (num5 < 800f)
                    {
                        if (Collision.CanHit(proj.position, 1, 1, Main.npc[i].position, Main.npc[i].width, Main.npc[i].height) && num5 > 50f)
                        {
                            array[num4] = i;
                            num4++;
                        }
                        else if (num4 == 0)
                        {
                            array[num3] = i;
                            num3++;
                        }
                    }
                }
            }
            if (num3 == 0 && num4 == 0)
            {
                return;
            }
            int num6;
            if (num4 > 0)
            {
                num6 = array[Main.rand.Next(num4)];
            }
            else
            {
                num6 = array[Main.rand.Next(num3)];
            }
            float num7 = 4f;
            float num8 = Main.rand.Next(-100, 101);
            float num9 = Main.rand.Next(-100, 101);
            float num10 = (float)Math.Sqrt((double)(num8 * num8 + num9 * num9));
            num10 = num7 / num10;
            num8 *= num10;
            num9 *= num10;
            Projectile.NewProjectile(proj.GetSource_FromThis(), proj.position.X, proj.position.Y, num8, num9, ProjectileID.SpectreWrath, num, 0f, proj.owner, (float)num6, 0);
        }

        public void SpiderEffect(bool hideVisual)
        {
            //minion crits
            SpiderEnchantActive = true;

            Player.GetCritChance(DamageClass.Summon) += 4;
        }

        public void SpookyEffect(bool hideVisual)
        {
            //scythe doom
            SpookyEnchantActive = true;

        }

        public void StardustEffect(Item item)
        {
            StardustEnchantActive = true;

            if (Player.whoAmI == Main.myPlayer && Player.ownedProjectileCounts[ProjectileID.StardustGuardian] < 1 && Player.GetToggleValue("Stardust"))
            {
                FargoSoulsUtil.NewSummonProjectile(Player.GetSource_Accessory(item), Player.Center, Vector2.Zero, ProjectileID.StardustGuardian, 30, 10f, Main.myPlayer);
            }


            //AddMinion(item, Player.GetToggleValue("Stardust"), ProjectileID.StardustGuardian, 30, 10f);
            //AddPet(Player.GetToggleValue("Stardust"), false, BuffID.StardustGuardianMinion, ProjectileID.StardustGuardian);
            Player.setStardust = true;

            if (FreezeTime && freezeLength > 0)
            {
                Player.buffImmune[ModContent.BuffType<TimeFrozen>()] = true;

                if (Main.netMode != NetmodeID.Server)
                {
                    if (!Filters.Scene["FargowiltasSouls:Invert"].IsActive())
                        Filters.Scene.Activate("FargowiltasSouls:Invert");

                    if (Filters.Scene["FargowiltasSouls:Invert"].IsActive())
                        Filters.Scene["FargowiltasSouls:Invert"].GetShader().UseTargetPosition(Player.Center);
                }

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.active && !npc.HasBuff(ModContent.BuffType<TimeFrozen>()))
                        npc.AddBuff(ModContent.BuffType<TimeFrozen>(), freezeLength);
                }

                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile p = Main.projectile[i];
                    if (p.active && !(p.minion && !ProjectileID.Sets.MinionShot[p.type]) && !p.GetGlobalProjectile<FargoSoulsGlobalProjectile>().TimeFreezeImmune && p.GetGlobalProjectile<FargoSoulsGlobalProjectile>().TimeFrozen == 0)
                    {
                        p.GetGlobalProjectile<FargoSoulsGlobalProjectile>().TimeFrozen = freezeLength;

                        /*if (p.owner == Player.whoAmI && p.friendly && !p.hostile)
                        {
                            //p.maxPenetrate = 1;
                            if (!p.usesLocalNPCImmunity && !p.usesIDStaticNPCImmunity)
                            {
                                p.usesLocalNPCImmunity = true;
                                p.localNPCHitCooldown = 1;
                            }
                        }*/
                    }
                }

                freezeLength--;

                FargowiltasSouls.Instance.ManageMusicTimestop(freezeLength < 5);

                if (freezeLength == 90)
                {
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Sounds/ZaWarudoResume"), Player.Center);
                }

                if (freezeLength <= 0)
                {
                    FreezeTime = false;
                    freezeLength = TIMESTOP_DURATION;

                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (npc.active && !npc.dontTakeDamage && npc.life == 1 && npc.lifeMax > 1)
                            npc.StrikeNPC(9999, 0f, 0);
                    }
                }
            }
        }

        public void TikiEffect(bool hideVisual)
        {
            Player.whipRangeMultiplier += 0.2f;

            if (Player.GetToggleValue("Tiki"))
                TikiEnchantActive = true;
        }

        private int getNumSentries()
        {
            int count = 0;

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];

                if (p.active && p.owner == Player.whoAmI && p.sentry)
                {
                    count++;
                }
            }

            return count;
        }




        public void TurtleEffect(bool hideVisual)
        {
            Player.turtleThorns = true;
            Player.thorns = 1f;

            TurtleEnchantActive = true;


            if (Player.GetToggleValue("Turtle") && !Player.HasBuff(ModContent.BuffType<BrokenShell>())
                && IsStandingStill && !Player.controlUseItem && Player.whoAmI == Main.myPlayer && !noDodge)
            {
                TurtleCounter++;

                if (TurtleCounter > 20)
                {
                    Player.AddBuff(ModContent.BuffType<ShellHide>(), 2);
                }
            }
            else
            {
                TurtleCounter = 0;
            }

            if (TurtleShellHP < 20 && !Player.HasBuff(ModContent.BuffType<BrokenShell>()) && !ShellHide && (LifeForce))
            {
                turtleRecoverCD--;
                if (turtleRecoverCD <= 0)
                {
                    turtleRecoverCD = 240;

                    TurtleShellHP++;
                }
            }

            //Main.NewText($"shell HP: {TurtleShellHP}, counter: {TurtleCounter}, recovery: {turtleRecoverCD}");
        }

        public void ValhallaEffect(bool hideVisual)
        {
            if (!Player.GetToggleValue("SquirePanic"))
                Player.buffImmune[BuffID.BallistaPanic] = true;

            SquireEnchantActive = true;
            ValhallaEnchantActive = true;
        }

        public void VortexEffect(bool hideVisual)
        {
            //portal spawn
            VortexEnchantActive = true;
            //stealth memes
            if (Player.whoAmI == Main.myPlayer && DoubleTap)
            {
                VortexStealth = !VortexStealth;

                if (!Player.GetToggleValue("VortexS"))
                    VortexStealth = false;

                if (Main.netMode == NetmodeID.MultiplayerClient)
                    NetMessage.SendData(MessageID.SyncPlayer, number: Player.whoAmI);

                if (VortexStealth && Player.GetToggleValue("VortexV") && !Player.HasBuff(ModContent.BuffType<VortexCD>()))
                {
                    int p = Projectile.NewProjectile(Player.GetSource_Misc(""), Player.Center.X, Player.Center.Y, 0f, 0f, ModContent.ProjectileType<Projectiles.Souls.Void>(), FargoSoulsUtil.HighestDamageTypeScaling(Player, 60), 5f, Player.whoAmI);
                    Main.projectile[p].GetGlobalProjectile<FargoSoulsGlobalProjectile>().CanSplit = false;
                    Main.projectile[p].netUpdate = true;

                    Player.AddBuff(ModContent.BuffType<VortexCD>(), 3600);
                }
            }

            if (Player.mount.Active)
                VortexStealth = false;

            if (VortexStealth)
            {
                Player.moveSpeed *= 0.3f;
                Player.aggro -= 1200;
                Player.setVortex = true;
                Player.stealth = 0f;
            }
        }







        private int apprenticePrevItem = -1;
        private bool apprenticeSwitchReady = false;
        private bool apprenticeBonusDamage = false;

        public void ApprenticeEffect()
        {
            //shadow shoot meme
            if (Player.GetToggleValue("Apprentice") && Player.controlUseItem)
            {
                Item heldItem = Player.HeldItem;

                //must hold for so long then switch bonus is active
                if (heldItem.type == apprenticePrevItem)
                {
                    ApprenticeCD++;

                    if (ApprenticeCD > 120)
                    {
                        apprenticeSwitchReady = true;

                        //dust
                        int dustId = Dust.NewDust(new Vector2(Player.position.X, Player.position.Y + 2f), Player.width, Player.height + 5, DustID.FlameBurst, 0, 0, 100, Color.Black, 2f);
                        Main.dust[dustId].noGravity = true;
                    }
                }
                else if (apprenticeSwitchReady && heldItem.type != apprenticePrevItem)
                {
                    apprenticeBonusDamage = true;
                }

                apprenticePrevItem = heldItem.type;

                //if (apprenticeCD == 0 && heldItem.damage > 0 && Player.controlUseItem && Player.itemAnimation != 0 && prevPosition != null && heldItem.type != ItemID.ExplosiveBunny && heldItem.type != ItemID.Cannonball
                //&& heldItem.createTile == -1 && heldItem.createWall == -1 && heldItem.ammo == AmmoID.None)
                //{
                //    if (prevPosition != null)
                //    {
                //        Vector2 vel = (Main.MouseWorld - prevPosition).SafeNormalize(-Vector2.UnitY) * 15;

                //        Projectile.NewProjectile(prevPosition, vel, ProjectileID.DD2FlameBurstTowerT3Shot, HighestDamageTypeScaling(heldItem.damage / 2), 1, Player.whoAmI);

                //        for (int i = 0; i < 5; i++)
                //        {
                //            int dustId = Dust.NewDust(new Vector2(prevPosition.X, prevPosition.Y + 2f), Player.width, Player.height + 5, DustID.Shadowflame, 0, 0, 100, Color.Black, 2f);
                //            Main.dust[dustId].noGravity = true;
                //        }
                //    }

                //    prevPosition = Player.position;
                //    apprenticeCD = 20;
                //}

                //if (apprenticeCD > 0)
                //{
                //    apprenticeCD--;
                //}
            }
        }




        public void MonkEffect()
        {
            MonkEnchantActive = true;

            if (Player.GetToggleValue("Monk") && !Player.HasBuff(ModContent.BuffType<MonkBuff>()))
            {
                monkTimer++;

                if (monkTimer >= 120)
                {
                    Player.AddBuff(ModContent.BuffType<MonkBuff>(), 2);
                    monkTimer = 0;

                    //dust
                    double spread = 2 * Math.PI / 36;
                    for (int i = 0; i < 36; i++)
                    {
                        Vector2 velocity = new Vector2(2, 2).RotatedBy(spread * i);

                        int index2 = Dust.NewDust(Player.Center, 0, 0, DustID.GoldCoin, velocity.X, velocity.Y, 100);
                        Main.dust[index2].noGravity = true;
                        Main.dust[index2].noLight = true;
                    }
                }
            }
        }

        public void SnowEffect(bool hideVisual)
        {
            SnowEnchantActive = true;

            if (ChillSnowstorm)
            {
                SnowVisual = true;

                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile proj = Main.projectile[i];

                    if (proj.active && proj.hostile && proj.damage > 0 && FargoSoulsUtil.CanDeleteProjectile(proj) && !proj.GetGlobalProjectile<FargoSoulsGlobalProjectile>().TimeFreezeImmune)
                    {
                        FargoSoulsGlobalProjectile globalProj = proj.GetGlobalProjectile<FargoSoulsGlobalProjectile>();
                        globalProj.ChilledProj = true;
                        globalProj.ChilledTimer = 6;
                        proj.netUpdate = true;
                    }
                }

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];

                    if (npc.active && !npc.friendly && npc.damage > 0 && !npc.dontTakeDamage && !npc.buffImmune[ModContent.BuffType<TimeFrozen>()])
                    {
                        npc.GetGlobalNPC<FargoSoulsGlobalNPC>().SnowChilled = true;
                        npc.GetGlobalNPC<FargoSoulsGlobalNPC>().SnowChilledTimer = 6;
                        npc.netUpdate = true;
                    }
                }

                if (--chillLength <= 0)
                    ChillSnowstorm = false;

                const int warning = 180;
                if (chillLength <= warning && chillLength % 60 == 0)
                {
                    float rampup = MathHelper.Lerp(1.5f, 0.5f, (float)chillLength / warning);

                    SoundEngine.PlaySound(SoundID.Item27 with { Volume = rampup }, Player.Center);

                    for (int i = 0; i < 20; i++)
                    {
                        int d = Dust.NewDust(Player.position, Player.width, Player.height, DustID.GemSapphire, 0, 0, 0, default, 2f * rampup);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].velocity *= 6f * rampup;
                    }
                }
            }
        }

        public void AncientShadowEffect()
        {
            //darkness
            AncientShadowEnchantActive = true;
        }

        //        #endregion

        //        #region souls
        public void ColossusSoul(Item item, int maxHP, float damageResist, int lifeRegen, bool hideVisual)
        {
            Player.statLifeMax2 += maxHP;
            Player.endurance += damageResist;
            Player.lifeRegen += lifeRegen;

            //hand warmer, pocket mirror, ankh shield
            Player.buffImmune[BuffID.Chilled] = true;
            Player.buffImmune[BuffID.Frozen] = true;
            Player.buffImmune[BuffID.Stoned] = true;
            Player.buffImmune[BuffID.Weak] = true;
            Player.buffImmune[BuffID.BrokenArmor] = true;
            Player.buffImmune[BuffID.Bleeding] = true;
            Player.buffImmune[BuffID.Poisoned] = true;
            Player.buffImmune[BuffID.Slow] = true;
            Player.buffImmune[BuffID.Confused] = true;
            Player.buffImmune[BuffID.Silenced] = true;
            Player.buffImmune[BuffID.Cursed] = true;
            Player.buffImmune[BuffID.Darkness] = true;
            Player.noKnockback = true;
            Player.fireWalk = true;
            Player.noFallDmg = true;
            //brain of confusion
            if (Player.GetToggleValue("DefenseBrain"))
            {
                Player.brainOfConfusionItem = item;
            }
            //charm of myths
            Player.pStone = true;
            //bee cloak, star veil
            if (Player.GetToggleValue("DefenseStar"))
            {
                Player.starCloakItem = item;
            }
            if (Player.GetToggleValue("DefenseBee"))
            {
                Player.honeyCombItem = item;
            }
            Player.longInvince = true;
            //shiny stone
            Player.shinyStone = true;
            //flesh knuckles
            if (Player.GetToggleValue("DefenseFleshKnuckle", false))
            {
                Player.aggro += 400;
            }

            //frozen turtle shell
            if (Player.GetToggleValue("DefenseFrozen", false))
            {
                if (Player.statLife <= Player.statLifeMax2 * 0.5)
                    Player.AddBuff(BuffID.IceBarrier, 5, true);
            }

            //paladins shield
            if (Player.GetToggleValue("DefensePaladin", false) && Player.statLife > Player.statLifeMax2 * .25)
            {
                Player.hasPaladinShield = true;
                for (int k = 0; k < Main.maxPlayers; k++)
                {
                    Player target = Main.player[k];

                    if (target.active && Player != target && Vector2.Distance(target.Center, Player.Center) < 400) target.AddBuff(BuffID.PaladinsShield, 30);
                }
            }
        }

        private bool extraCarpetDuration = true;

        public void SupersonicSoul(Item item, bool hideVisual)
        {
            Player.hasMagiluminescence = true;

            //amph boot
            Player.autoJump = true;
            Player.frogLegJumpBoost = true;

            if (Player.GetToggleValue("Supersonic") && !Player.GetModPlayer<FargoSoulsPlayer>().noSupersonic && !FargoSoulsUtil.AnyBossAlive())
            {
                // 5 is the default value, I removed the config for it because the new toggler doesn't have sliders
                Player.runAcceleration += 5f * .1f;
                Player.maxRunSpeed += 5f * 2;
                //frog legs
                Player.autoJump = true;
                Player.jumpSpeedBoost += 2.4f;
                Player.maxFallSpeed += 5f;
                Player.jumpBoost = true;
            }
            else
            {
                //calculated to match flight mastery soul, 6.75 same as frostspark
                Player.accRunSpeed = Player.GetToggleValue("RunSpeed", false) ? 15.6f : 6.75f;
            }

            if (Player.GetToggleValue("Momentum", false))
                NoMomentum = true;

            Player.moveSpeed += 0.5f;

            if (Player.GetToggleValue("SupersonicRocketBoots", false))
            {
                Player.rocketBoots = Player.vanityRocketBoots = ArmorIDs.RocketBoots.TerrasparkBoots;
                Player.rocketTimeMax = 10;
            }

            Player.iceSkate = true;

            //lava waders
            Player.waterWalk = true;
            Player.fireWalk = true;
            Player.lavaImmune = true;
            Player.noFallDmg = true;

            //bundle
            if (Player.GetToggleValue("SupersonicJumps") && Player.wingTime == 0)
            {
                Player.hasJumpOption_Cloud = true;
                Player.hasJumpOption_Sandstorm = true;
                Player.hasJumpOption_Blizzard = true;
                Player.hasJumpOption_Fart = true;
            }

            //magic carpet
            if (Player.whoAmI == Main.myPlayer && Player.GetToggleValue("SupersonicCarpet"))
            {
                Player.carpet = true;
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    NetMessage.SendData(MessageID.SyncPlayer, number: Player.whoAmI);

                if (Player.canCarpet)
                {
                    extraCarpetDuration = true;
                }
                else if (extraCarpetDuration)
                {
                    extraCarpetDuration = false;
                    Player.carpetTime = 1000;
                }
            }

            //EoC Shield
            if (Player.GetToggleValue("CthulhuShield"))
                Player.dashType = 2;

            //ninja gear
            if (Player.GetToggleValue("SupersonicTabi", false))
                Player.dashType = 1;
            if (Player.GetToggleValue("BlackBelt"))
                Player.blackBelt = true;
            if (Player.GetToggleValue("SupersonicClimbing"))
                Player.spikedBoots = 2;

            //sweetheart necklace
            if (Player.GetToggleValue("DefenseBee"))
            {
                Player.honeyCombItem = item;
            }
            if (Player.GetToggleValue("DefensePanic"))
            {
                Player.panic = true;
            }

            if (!Player.flowerBoots && Player.GetToggleValue("MasoAeolusFlower"))
            {
                Player.flowerBoots = true;
                if (Player.whoAmI == Main.myPlayer)
                    Player.DoBootsEffect(new Utils.TileActionAttempt(Player.DoBootsEffect_PlaceFlowersOnTile));
            }
        }

        public void FlightMasterySoul()
        {
            Player.wingTimeMax = 999999;
            Player.wingTime = Player.wingTimeMax;
            Player.ignoreWater = true;

            if (Player.GetToggleValue("FlightMasteryInsignia", false)) //soaring insig
                Player.empressBrooch = true;

            if (Player.GetToggleValue("FlightMasteryGravity", false))
                Player.gravity = Player.defaultGravity * 1.5f;

            //hover
            if (Player.controlDown && Player.controlJump && !Player.mount.Active)
            {
                Player.position.Y -= Player.velocity.Y;
                if (Player.velocity.Y > 0.1f)
                    Player.velocity.Y = 0.1f;
                else if (Player.velocity.Y < -0.1f)
                    Player.velocity.Y = -0.1f;
            }

            //grav
            if (Player.GetToggleValue("MasoGrav"))
                Player.gravControl = true;
        }

        public void TrawlerSoul(Item item, bool hideVisual)
        {
            //instacatch
            FishSoul1 = true;
            //extra lures
            if (Player.GetToggleValue("Trawler"))
            {
                FishSoul2 = true;
            }

            //tackle bag
            Player.fishingSkill += 60;
            Player.sonarPotion = true;
            Player.cratePotion = true;
            Player.accFishingLine = true;
            Player.accTackleBox = true;
            Player.accFishFinder = true;
            Player.accLavaFishing = true;

            //volatile gel
            if (Player.GetToggleValue("TrawlerGel"))
                VolatileGelatin(Player, item);

            //spore sac
            if (Player.whoAmI == Main.myPlayer && Player.GetToggleValue("TrawlerSpore"))
            {
                Player.sporeSac = true;
                Player.SporeSac(item);
            }

            //arctic diving gear
            Player.arcticDivingGear = true;
            Player.accFlipper = true;
            Player.accDivingHelm = true;
            Player.iceSkate = true;
            if (Player.wet)
            {
                Lighting.AddLight((int)Player.Center.X / 16, (int)Player.Center.Y / 16, 0.2f, 0.8f, 0.9f);
            }

            //sharkron balloon
            if (Player.GetToggleValue("TrawlerJump") && Player.wingTime == 0)
                Player.hasJumpOption_Sail = true;

            Player.jumpBoost = true;
            Player.noFallDmg = true;
        }

        public void VolatileGelatin(Player player, Item sourceItem)
        {
            if (Main.myPlayer != player.whoAmI)
            {
                return;
            }
            player.volatileGelatinCounter++;
            if (player.volatileGelatinCounter > 50)
            {
                player.volatileGelatinCounter = 0;
                int damage = 65;
                float knockBack = 7f;
                float num = 640f;
                NPC npc = null;
                for (int i = 0; i < 200; i++)
                {
                    NPC npc2 = Main.npc[i];
                    if (npc2 != null && npc2.active && npc2.CanBeChasedBy(player, false) && Collision.CanHit(player, npc2))
                    {
                        float num2 = Vector2.Distance(npc2.Center, player.Center);
                        if (num2 < num)
                        {
                            num = num2;
                            npc = npc2;
                        }
                    }
                }
                if (npc != null)
                {
                    Vector2 vector = npc.Center - player.Center;
                    vector = vector.SafeNormalize(Vector2.Zero) * 6f;
                    vector.Y -= 2f;
                    Projectile.NewProjectile(player.GetSource_Accessory(sourceItem), player.Center.X, player.Center.Y, vector.X, vector.Y, 937, damage, knockBack, player.whoAmI, 0f, 0f);
                }
            }
        }

        public void WorldShaperSoul(bool hideVisual)
        {
            //mining speed, spelunker, dangersense, light, hunter, pet
            MinerEnchant.MinerEffect(Player, .66f);
            //placing speed up
            Player.tileSpeed += 0.5f;
            Player.wallSpeed += 0.5f;
            //toolbox
            if (Player.whoAmI == Main.myPlayer)
            {
                Player.tileRangeX += 10;
                Player.tileRangeY += 10;
            }
            //gizmo pack
            Player.autoPaint = true;
            //presserator
            Player.autoActuator = true;
            //royal gel
            Player.npcTypeNoAggro[1] = true;
            Player.npcTypeNoAggro[16] = true;
            Player.npcTypeNoAggro[59] = true;
            Player.npcTypeNoAggro[71] = true;
            Player.npcTypeNoAggro[81] = true;
            Player.npcTypeNoAggro[138] = true;
            Player.npcTypeNoAggro[121] = true;
            Player.npcTypeNoAggro[122] = true;
            Player.npcTypeNoAggro[141] = true;
            Player.npcTypeNoAggro[147] = true;
            Player.npcTypeNoAggro[183] = true;
            Player.npcTypeNoAggro[184] = true;
            Player.npcTypeNoAggro[204] = true;
            Player.npcTypeNoAggro[225] = true;
            Player.npcTypeNoAggro[244] = true;
            Player.npcTypeNoAggro[302] = true;
            Player.npcTypeNoAggro[333] = true;
            Player.npcTypeNoAggro[335] = true;
            Player.npcTypeNoAggro[334] = true;
            Player.npcTypeNoAggro[336] = true;
            Player.npcTypeNoAggro[537] = true;

            if (Player.whoAmI == Main.myPlayer && Player.GetToggleValue("Builder"))
            {
                BuilderMode = true;
                //if (Main.netMode == NetmodeID.MultiplayerClient) NetMessage.SendData(MessageID.SyncPlayer, number: Player.whoAmI);

                for (int i = 0; i < TileLoader.TileCount; i++)
                {
                    Player.adjTile[i] = true;
                }

                //placing speed up
                Player.tileSpeed += 0.5f;
                Player.wallSpeed += 0.5f;

                //toolbox
                if (Player.HeldItem.createWall == 0) //tiles
                {
                    Player.tileRangeX += 60;
                    Player.tileRangeY += 60;
                }
                else //walls
                {
                    Player.tileRangeX += 20;
                    Player.tileRangeY += 20;
                }
            }

            //cell phone
            Player.accWatch = 3;
            Player.accDepthMeter = 1;
            Player.accCompass = 1;
            Player.accFishFinder = true;
            Player.accDreamCatcher = true;
            Player.accOreFinder = true;
            Player.accStopwatch = true;
            Player.accCritterGuide = true;
            Player.accJarOfSouls = true;
            Player.accThirdEye = true;
            Player.accCalendar = true;
            Player.accWeatherRadio = true;
        }


        //        #endregion

        #region maso acc

        private readonly int[] ElectricAttacks = new int[]
        {
            ProjectileID.DeathLaser,
            ProjectileID.EyeLaser,
            ProjectileID.PinkLaser,
            ProjectileID.EyeBeam,
            ProjectileID.MartianTurretBolt,
            ProjectileID.BrainScramblerBolt,
            ProjectileID.GigaZapperSpear,
            ProjectileID.RayGunnerLaser,
            ProjectileID.SaucerLaser,
            ProjectileID.NebulaLaser,
            ProjectileID.VortexVortexLightning,
            ProjectileID.DD2LightningBugZap
        };

        public void GroundStickCheck(Projectile proj, ref int damage)
        {
            if (!Player.GetToggleValue("MasoLightning"))
                return;

            bool electricAttack = false;

            if (proj.ModProjectile == null)
            {
                if (proj.aiStyle == ProjAIStyleID.MartianDeathRay
                    || proj.aiStyle == ProjAIStyleID.ThickLaser
                    || proj.aiStyle == ProjAIStyleID.LightningOrb
                    || ElectricAttacks.Contains(proj.type))
                {
                    electricAttack = true;
                }
            }
            else if (proj.ModProjectile is Projectiles.Deathrays.BaseDeathray)
            {
                electricAttack = true;
            }
            else
            {
                string name = proj.ModProjectile.Name.ToLower();
                if (name.Contains("lightning") || name.Contains("electr") || name.Contains("thunder") || name.Contains("laser"))
                    electricAttack = true;
            }

            if (electricAttack && Player.whoAmI == Main.myPlayer && !Player.HasBuff(ModContent.BuffType<Supercharged>()))
            {
                damage /= 2;

                Player.AddBuff(ModContent.BuffType<Supercharged>(), 60 * 30);

                foreach (Projectile p in Main.projectile.Where(p => p.active && p.minion && p.owner == Player.whoAmI
                    && (p.type == ModContent.ProjectileType<Probe1>() || p.type == ModContent.ProjectileType<Probe2>())))
                {
                    p.ai[1] = 180;
                    p.netUpdate = true;
                }

                SoundEngine.PlaySound(SoundID.NPCDeath6, Player.Center);
                SoundEngine.PlaySound(SoundID.Item92, Player.Center);
                SoundEngine.PlaySound(SoundID.Item14, Player.Center);

                for (int i = 0; i < 20; i++)
                {
                    int dust = Dust.NewDust(Player.position, Player.width, Player.height, 229, 0f, 0f, 100, default, 3f);
                    Main.dust[dust].velocity *= 1.4f;
                }

                for (int i = 0; i < 10; i++)
                {
                    int dust = Dust.NewDust(Player.position, Player.width, Player.height, 6, 0f, 0f, 100, default, 3.5f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 7f;
                    dust = Dust.NewDust(Player.position, Player.width, Player.height, 6, 0f, 0f, 100, default, 1.5f);
                    Main.dust[dust].velocity *= 3f;
                }

                for (int index1 = 0; index1 < 30; ++index1)
                {
                    int index2 = Dust.NewDust(Player.position, Player.width, Player.height, 229, 0f, 0f, 100, new Color(), 2f);
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].velocity *= 21f;
                    Main.dust[index2].noLight = true;
                    int index3 = Dust.NewDust(Player.position, Player.width, Player.height, 229, 0f, 0f, 100, new Color(), 1f);
                    Main.dust[index3].velocity *= 12f;
                    Main.dust[index3].noGravity = true;
                    Main.dust[index3].noLight = true;
                }

                for (int i = 0; i < 20; i++)
                {
                    int d = Dust.NewDust(Player.position, Player.width, Player.height, 229, 0f, 0f, 100, default, Main.rand.NextFloat(2f, 5f));
                    if (Main.rand.NextBool(3))
                        Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= Main.rand.NextFloat(12f, 18f);
                    Main.dust[d].position = Player.Center;
                }
            }
        }

        public void DeerclawpsAttack(Vector2 pos)
        {
            if (Player.whoAmI == Main.myPlayer && Player.GetToggleValue("Deerclawps"))
            {
                Vector2 vel = 16f * -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(30));

                int dam = 32;
                int type = ProjectileID.DeerclopsIceSpike;
                float ai0 = -15f;
                float ai1 = Main.rand.NextFloat(0.5f, 1f);
                if (LumpOfFlesh)
                {
                    dam = 48;
                    type = ProjectileID.SharpTears;
                    ai0 *= 2f;
                    ai1 += 0.5f;
                }
                dam = (int)(dam * Player.ActualClassDamage(DamageClass.Melee));

                if (Player.velocity.Y == 0)
                    Projectile.NewProjectile(Player.GetSource_Accessory(DeerclawpsItem), pos, vel, type, dam, 4f, Main.myPlayer, ai0, ai1);
                else
                    Projectile.NewProjectile(Player.GetSource_Accessory(DeerclawpsItem), pos, vel * (Main.rand.NextBool() ? 1 : -1), type, dam, 4f, Main.myPlayer, ai0, ai1 / 2);
            }
        }

        public void GuttedHeartEffect()
        {
            if (Player.whoAmI != Main.myPlayer)
                return;

            GuttedHeartCD--;

            if (Player.velocity == Vector2.Zero && Player.itemAnimation == 0)
                GuttedHeartCD--;

            if (GuttedHeartCD <= 0)
            {
                GuttedHeartCD = 900;
                if (Player.GetToggleValue("MasoBrain"))
                {
                    int count = 0;
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<CreeperGutted>() && Main.npc[i].ai[0] == Player.whoAmI)
                            count++;
                    }
                    if (count < 5)
                    {
                        int multiplier = 1;
                        if (PureHeart)
                            multiplier = 2;
                        if (MasochistSoul)
                            multiplier = 5;
                        if (Main.netMode == NetmodeID.SinglePlayer)
                        {
                            int n = NPC.NewNPC(NPC.GetBossSpawnSource(Player.whoAmI), (int)Player.Center.X, (int)Player.Center.Y, ModContent.NPCType<CreeperGutted>(), 0, Player.whoAmI, 0f, multiplier);
                            if (n != Main.maxNPCs)
                                Main.npc[n].velocity = Vector2.UnitX.RotatedByRandom(2 * Math.PI) * 8;
                        }
                        else if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            var netMessage = Mod.GetPacket();
                            netMessage.Write((byte)FargowiltasSouls.PacketID.RequestGuttedCreeper);
                            netMessage.Write((byte)Player.whoAmI);
                            netMessage.Write((byte)multiplier);
                            netMessage.Send();
                        }
                    }
                    else
                    {
                        int lowestHealth = -1;
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<CreeperGutted>() && Main.npc[i].ai[0] == Player.whoAmI)
                            {
                                if (lowestHealth < 0)
                                    lowestHealth = i;
                                else if (Main.npc[i].life < Main.npc[lowestHealth].life)
                                    lowestHealth = i;
                            }
                        }
                        if (Main.npc[lowestHealth].life < Main.npc[lowestHealth].lifeMax)
                        {
                            if (Main.netMode == NetmodeID.SinglePlayer)
                            {
                                int damage = Main.npc[lowestHealth].lifeMax - Main.npc[lowestHealth].life;
                                Main.npc[lowestHealth].life = Main.npc[lowestHealth].lifeMax;
                                CombatText.NewText(Main.npc[lowestHealth].Hitbox, CombatText.HealLife, damage);
                            }
                            else if (Main.netMode == NetmodeID.MultiplayerClient)
                            {
                                var netMessage = Mod.GetPacket();
                                netMessage.Write((byte)FargowiltasSouls.PacketID.RequestCreeperHeal);
                                netMessage.Write((byte)Player.whoAmI);
                                netMessage.Write((byte)lowestHealth);
                                netMessage.Send();
                            }
                        }
                    }
                }
            }
        }

        public void GuttedHeartNurseHeal()
        {
            if (Player.whoAmI == Main.myPlayer && Player.GetToggleValue("MasoBrain"))
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];

                    if (npc.type == ModContent.NPCType<CreeperGutted>() && npc.ai[0] == Player.whoAmI)
                    {
                        int heal = npc.lifeMax - npc.life;

                        if (Main.netMode == NetmodeID.SinglePlayer)
                        {
                            if (heal > 0)
                            {
                                npc.HealEffect(heal);
                                npc.life = npc.lifeMax;
                            }
                        }
                        else if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            var netMessage = Mod.GetPacket();
                            netMessage.Write((byte)FargowiltasSouls.PacketID.RequestCreeperHeal);
                            netMessage.Write((byte)Player.whoAmI);
                            netMessage.Write((byte)i);
                            netMessage.Send();
                        }
                    }
                }
            }
        }

        public void WretchedPouchEffect()
        {
            if (!Player.GetToggleValue("MasoPouch"))
                return;

            if (!(Player.controlUseItem || Player.controlUseTile || WeaponUseTimer > 0))
                return;

            if (Player.HeldItem.IsAir || Player.HeldItem.damage <= 0 || Player.HeldItem.pick > 0 || Player.HeldItem.axe > 0 || Player.HeldItem.hammer > 0)
                return;

            Player.AddBuff(ModContent.BuffType<WretchedHex>(), 2);

            int d = Dust.NewDust(Player.position, Player.width, Player.height, DustID.Shadowflame, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 0, new Color(), 3f);
            Main.dust[d].noGravity = true;
            Main.dust[d].velocity *= 5f;

            Player.GetDamage(DamageClass.Generic) += 1.20f;
            Player.endurance -= 0.20f;

            Player.velocity.X *= 0.85f;
            Player.velocity.Y *= 0.85f;

            if (--WretchedPouchCD <= 0)
            {
                WretchedPouchCD = 25;

                if (Player.whoAmI == Main.myPlayer)
                {
                    Vector2 vel = Main.rand.NextVector2Unit();
                    
                    NPC target = Main.npc.FirstOrDefault(n => n.active && n.Distance(Player.Center) < 360 && n.CanBeChasedBy() && Collision.CanHit(Player.position, Player.width, Player.height, n.position, n.width, n.height));
                    if (target != null)
                        vel = Player.DirectionTo(target.Center);

                    vel *= 8f;

                    SoundEngine.PlaySound(SoundID.Item103, Player.Center);

                    int dam = 40;
                    if (MasochistSoul)
                        dam *= 3;
                    dam = (int)(dam * Player.ActualClassDamage(DamageClass.Magic));

                    void ShootTentacle(Vector2 baseVel, float variance, int aiMin, int aiMax)
                    {
                        Vector2 speed = baseVel.RotatedBy(variance * (Main.rand.NextDouble() - 0.5));
                        float ai0 = Main.rand.Next(aiMin, aiMax) * (1f / 1000f);
                        if (Main.rand.NextBool())
                            ai0 *= -1f;
                        float ai1 = Main.rand.Next(aiMin, aiMax) * (1f / 1000f);
                        if (Main.rand.NextBool())
                            ai1 *= -1f;
                        Projectile.NewProjectile(Player.GetSource_Accessory(WretchedPouchItem), Player.Center, speed, ModContent.ProjectileType<ShadowflameTentacle>(), dam, 4f, Player.whoAmI, ai0, ai1);
                    };

                    int max = target == null ? 3 : 6;
                    float rotationOffset = MathHelper.TwoPi / max;
                    if (target != null)
                    {
                        for (int i = 0; i < max / 2; i++) //shoot right at them
                            ShootTentacle(vel, rotationOffset, 60, 90);
                    }
                    for (int i = 0; i < max; i++) //shoot everywhere
                        ShootTentacle(vel.RotatedBy(rotationOffset * i), rotationOffset, 30, 50);
                }
            }
        }

        public void OnLandingEffects()
        {
            //Player.justJumped use this tbh
            if (SlimyShieldFalling) //landing
            {
                if (Player.velocity.Y < 0f)
                    SlimyShieldFalling = false;

                if (Player.velocity.Y == 0f)
                {
                    SlimyShieldFalling = false;
                    if (Player.whoAmI == Main.myPlayer && Player.gravDir > 0)
                    {
                        if (SlimyShieldItem != null && Player.GetToggleValue("MasoSlime"))
                        {
                            SoundEngine.PlaySound(SoundID.Item21, Player.Center);
                            Vector2 mouse = Main.MouseWorld;
                            int damage = 8;
                            if (SupremeDeathbringerFairy)
                                damage = 16;
                            if (MasochistSoul)
                                damage = 80;
                            damage = (int)(damage * Player.ActualClassDamage(DamageClass.Melee));
                            for (int i = 0; i < 3; i++)
                            {
                                Vector2 spawn = new Vector2(mouse.X + Main.rand.Next(-200, 201), mouse.Y - Main.rand.Next(600, 901));
                                if (Collision.CanHitLine(mouse, 0, 0, spawn, 0, 0))
                                {
                                    Vector2 speed = mouse - spawn;
                                    speed.Normalize();
                                    speed *= 10f;
                                    Projectile.NewProjectile(Player.GetSource_Accessory(SlimyShieldItem, "SlimyShield"), spawn, speed, ModContent.ProjectileType<SlimeBall>(), damage, 1f, Main.myPlayer);
                                }
                            }
                        }

                        if (GelicWingsItem != null)
                        {
                            int dam = 60; //deliberately no scaling
                            for (int j = -1; j <= 1; j += 2)
                            {
                                Vector2 baseVel = Vector2.UnitX.RotatedBy(MathHelper.ToRadians(-10 * j));
                                const int max = 8;
                                for (int i = 0; i < max; i++)
                                {
                                    Vector2 vel = Main.rand.NextFloat(5f, 10f) * j * baseVel.RotatedBy(-MathHelper.PiOver4 * 0.8f / max * i * j);
                                    Projectile.NewProjectile(Player.GetSource_Accessory(GelicWingsItem), Player.Bottom - Vector2.UnitY * 8, vel, ModContent.ProjectileType<GelicWingSpike>(), dam, 5f, Main.myPlayer);
                                }
                            }
                        }
                    }
                }
            }
            else if (Player.velocity.Y > 3f)
            {
                SlimyShieldFalling = true;
            }
        }

        public void AgitatingLensEffect()
        {
            if (AgitatingLensCD++ > 15)
            {
                AgitatingLensCD = 0;
                if ((Math.Abs(Player.velocity.X) >= 5 || Math.Abs(Player.velocity.Y) >= 5) && Player.whoAmI == Main.myPlayer && Player.GetToggleValue("MasoEye"))
                {
                    int damage = 18;
                    if (SupremeDeathbringerFairy)
                        damage *= 2;
                    if (MasochistSoul)
                        damage *= 2;
                    damage = (int)(damage * Player.ActualClassDamage(DamageClass.Magic));
                    int proj = Projectile.NewProjectile(Player.GetSource_Accessory(AgitatingLensItem), Player.Center, Player.velocity * 0.1f, ModContent.ProjectileType<BloodScytheFriendly>(), damage, 5f, Player.whoAmI);
                }
            }
        }

        public void DarkenedHeartAttack(Projectile projectile)
        {
            if (DarkenedHeartCD <= 0)
            {
                DarkenedHeartCD = 60;

                if (Player.GetToggleValue("MasoEater") && (projectile == null || projectile.type != ProjectileID.TinyEater))
                {
                    SoundEngine.PlaySound(SoundID.NPCHit1, Player.Center);
                    for (int index1 = 0; index1 < 20; ++index1)
                    {
                        int index2 = Dust.NewDust(Player.position, Player.width, Player.height, 184, 0.0f, 0.0f, 0, new Color(), 1f);
                        Dust dust = Main.dust[index2];
                        dust.scale = dust.scale * 1.1f;
                        Main.dust[index2].noGravity = true;
                    }
                    for (int index1 = 0; index1 < 30; ++index1)
                    {
                        int index2 = Dust.NewDust(Player.position, Player.width, Player.height, 184, 0.0f, 0.0f, 0, new Color(), 1f);
                        Dust dust1 = Main.dust[index2];
                        dust1.velocity = dust1.velocity * 2.5f;
                        Dust dust2 = Main.dust[index2];
                        dust2.scale = dust2.scale * 0.8f;
                        Main.dust[index2].noGravity = true;
                    }
                    int num = 2;
                    if (Main.rand.NextBool(3))
                        ++num;
                    if (Main.rand.NextBool(6))
                        ++num;
                    if (Main.rand.NextBool(9))
                        ++num;
                    int dam = PureHeart ? 30 : 12;
                    if (MasochistSoul)
                        dam *= 2;
                    for (int index = 0; index < num; ++index)
                        Projectile.NewProjectile(Player.GetSource_Accessory(DarkenedHeartItem), Player.Center.X, Player.Center.Y, Main.rand.Next(-35, 36) * 0.02f * 10f,
                            Main.rand.Next(-35, 36) * 0.02f * 10f, ProjectileID.TinyEater, (int)(dam * Player.ActualClassDamage(DamageClass.Melee)), 1.75f, Player.whoAmI);
                }
            }
        }

        public void FrigidGemstoneKey()
        {
            if (FrigidGemstoneCD > 0)
                return;

            if (!Player.CheckMana(6, true))
                return;

            FrigidGemstoneCD = 10;

            SoundEngine.PlaySound(SoundID.Item28, Player.Center);

            Projectile.NewProjectile(Player.GetSource_Accessory(FrigidGemstoneItem),
                Player.Center, 12f * Player.DirectionTo(Main.MouseWorld), ProjectileID.IceBlock,
                (int)(14 * Player.ActualClassDamage(DamageClass.Magic)), 2f,
                Player.whoAmI, Player.tileTargetX, Player.tileTargetY);
        }

        public void CelestialRuneSupportAttack(int damage, DamageClass damageType)
        {
            Vector2 position = Player.Center;
            Vector2 velocity = Vector2.Normalize(Main.MouseWorld - position);

            if (damageType.CountsAsClass(DamageClass.Melee)) //fireball
            {
                SoundEngine.PlaySound(SoundID.Item34, position);
                for (int i = 0; i < 3; i++)
                {
                    Projectile.NewProjectile(Player.GetSource_Accessory(CelestialRuneItem), position, velocity.RotatedByRandom(Math.PI / 6) * Main.rand.NextFloat(6f, 10f),
                        ModContent.ProjectileType<CelestialRuneFireball>(), (int)(50f * Player.ActualClassDamage(DamageClass.Melee)), 9f, Player.whoAmI);
                }
            }
            if (damageType.CountsAsClass(DamageClass.Ranged)) //lightning
            {
                for (int i = -1; i <= 1; i++)
                {
                    float ai1 = Main.rand.Next(100);
                    Vector2 vel = Vector2.Normalize(velocity.RotatedByRandom(Math.PI / 4)).RotatedBy(MathHelper.ToRadians(5) * i) * 7f;
                    Projectile.NewProjectile(Player.GetSource_Accessory(CelestialRuneItem), position, vel, ModContent.ProjectileType<CelestialRuneLightningArc>(),
                        (int)(50f * Player.ActualClassDamage(DamageClass.Ranged)), 1f, Player.whoAmI, velocity.ToRotation(), ai1);
                }
            }
            if (damageType.CountsAsClass(DamageClass.Magic)) //ice mist
            {
                Projectile.NewProjectile(Player.GetSource_Accessory(CelestialRuneItem), position, velocity * 4.25f, ModContent.ProjectileType<CelestialRuneIceMist>(), (int)(50f * Player.ActualClassDamage(DamageClass.Magic)), 4f, Player.whoAmI);
            }
            if (damageType.CountsAsClass(DamageClass.Summon)) //ancient vision
            {
                FargoSoulsUtil.NewSummonProjectile(Player.GetSource_Accessory(CelestialRuneItem), position, velocity * 16f, ModContent.ProjectileType<CelestialRuneAncientVision>(), 50, 3f, Player.whoAmI);
            }
        }

        public void SpecialDashKey()
        {
            if (SpecialDashCD <= 0)
            {
                SpecialDashCD = 180;

                if (Player.whoAmI == Main.myPlayer)
                {
                    Player.RemoveAllGrapplingHooks();

                    /*Player.controlLeft = false;
                    Player.controlRight = false;
                    Player.controlJump = false;
                    Player.controlDown = false;*/
                    Player.controlUseItem = false;
                    Player.controlUseTile = false;
                    Player.controlHook = false;
                    Player.controlMount = false;

                    Player.itemAnimation = 0;
                    Player.itemTime = 0;
                    Player.reuseDelay = 0;

                    if (BetsysHeartItem != null)
                    {
                        Vector2 vel = Player.DirectionTo(Main.MouseWorld) * (MasochistHeart ? 25 : 20);
                        Projectile.NewProjectile(Player.GetSource_Accessory(BetsysHeartItem), Player.Center, vel, ModContent.ProjectileType<Projectiles.BetsyDash>(), (int)(100 * Player.ActualClassDamage(DamageClass.Melee)), 6f, Player.whoAmI);

                        Player.immune = true;
                        Player.immuneTime = Math.Max(Player.immuneTime, 2);
                        Player.hurtCooldowns[0] = Math.Max(Player.hurtCooldowns[0], 2);
                        Player.hurtCooldowns[1] = Math.Max(Player.hurtCooldowns[1], 2);

                        //immune to all debuffs
                        foreach (int debuff in FargowiltasSouls.DebuffIDs)
                        {
                            if (!Player.HasBuff(debuff))
                            {
                                Player.buffImmune[debuff] = true;
                            }
                        }
                    }
                    else if (QueenStingerItem != null)
                    {
                        SpecialDashCD += 60;

                        Vector2 vel = Player.DirectionTo(Main.MouseWorld) * 20;
                        Projectile.NewProjectile(Player.GetSource_Accessory(QueenStingerItem), Player.Center, vel, ModContent.ProjectileType<Projectiles.BeeDash>(), (int)(44 * Player.ActualClassDamage(DamageClass.Melee)), 6f, Player.whoAmI);
                    }

                    Player.AddBuff(ModContent.BuffType<Buffs.BetsyDash>(), 20);
                }
            }
        }

        public void MagicalBulbKey()
        {
            if (Player.HasBuff(ModContent.BuffType<MagicalCleanseCD>()))
                return;

            bool cleansed = false;

            int max = Player.buffType.Length;
            for (int i = 0; i < max; i++)
            {
                int timeLeft = Player.buffTime[i];
                if (timeLeft <= 0)
                    continue;

                int buffType = Player.buffType[i];
                if (buffType <= 0)
                    continue;

                if (timeLeft > 5
                    && Main.debuff[buffType]
                    && !Main.buffNoTimeDisplay[buffType]
                    && !BuffID.Sets.NurseCannotRemoveDebuff[buffType])
                {
                    Player.DelBuff(i);

                    cleansed = true;

                    i--;
                    max--; //just in case, to prevent being stuck here forever
                }
            }

            if (cleansed)
            {
                Player.AddBuff(ModContent.BuffType<MagicalCleanseCD>(), 60 * 120);

                SoundEngine.PlaySound(SoundID.Item4, Player.Center);

                for (int index1 = 0; index1 < 50; ++index1)
                {
                    int index2 = Dust.NewDust(Player.position, Player.width, Player.height, Main.rand.NextBool() ? 107 : 157, 0f, 0f, 0, new Color(), 3f);
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].velocity *= 8f;
                }
            }
        }

        public void BombKey()
        {
            if (MutantEyeItem != null && MutantEyeCD <= 0)
            {
                MutantEyeCD = 3600;

                if (!Main.dedServ && Main.LocalPlayer.active)
                    Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().Screenshake = 30;

                const int invulTime = 90;
                Player.immune = true;
                Player.immuneTime = invulTime;
                Player.hurtCooldowns[0] = invulTime;
                Player.hurtCooldowns[1] = invulTime;

                SoundEngine.PlaySound(SoundID.Item84, Player.Center);

                const int max = 100; //make some indicator dusts
                for (int i = 0; i < max; i++)
                {
                    Vector2 vector6 = Vector2.UnitY * 30f;
                    vector6 = vector6.RotatedBy((i - (max / 2 - 1)) * 6.28318548f / max) + Main.LocalPlayer.Center;
                    Vector2 vector7 = vector6 - Main.LocalPlayer.Center;
                    int d = Dust.NewDust(vector6 + vector7, 0, 0, 229, 0f, 0f, 0, default(Color), 3f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity = vector7;
                }

                for (int i = 0; i < 50; i++) //make some indicator dusts
                {
                    int d = Dust.NewDust(Player.position, Player.width, Player.height, 229, 0f, 0f, 0, default(Color), 2.5f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].noLight = true;
                    Main.dust[d].velocity *= 24f;
                }

                FargoSoulsUtil.ClearHostileProjectiles(1);

                void SpawnSphereRing(int ringMax, float speed, int damage2, float rotationModifier)
                {
                    float rotation = 2f * (float)Math.PI / ringMax;
                    Vector2 vel = Vector2.UnitY * speed;
                    int type = ModContent.ProjectileType<PhantasmalSphereRing>();
                    for (int i = 0; i < ringMax; i++)
                    {
                        vel = vel.RotatedBy(rotation);
                        Projectile.NewProjectile(Player.GetSource_Accessory(MutantEyeItem), Player.Center, vel, type, damage2, 3f, Main.myPlayer, rotationModifier * Player.direction, speed);
                    }
                }

                int damage = (int)(1700 * Player.ActualClassDamage(DamageClass.Magic));
                SpawnSphereRing(24, 12f, damage, -1f);
                SpawnSphereRing(24, 12f, damage, 1f);
            }
            else if (CirnoGraze)
            {
                Projectile cirnoBomb = Main.projectile.FirstOrDefault(p => p.active && p.friendly && p.owner == Player.whoAmI && p.type == ModContent.ProjectileType<CirnoBomb>());
                if (cirnoBomb != null)
                {
                    cirnoBomb.ai[0] = 1;
                    cirnoBomb.netUpdate = true;

                    CirnoGrazeCounter = 0;
                }
            }
        }

        public void DebuffInstallKey()
        {
            if (AgitatingLensItem != null
                && Player.GetToggleValue("MasoEyeInstall", false)
                && Player.controlUp && Player.controlDown)
            {
				if (!Player.HasBuff(ModContent.BuffType<BerserkerInstall>())
					&& !Player.HasBuff(ModContent.BuffType<BerserkerInstallCD>()))
				{
					SoundEngine.PlaySound(SoundID.Item119, Player.Center);

					Player.AddBuff(ModContent.BuffType<BerserkerInstall>(), 7 * 60 + 30); //7.5sec

					for (int i = 0; i < 60; i++)
					{
						int index2 = Dust.NewDust(Player.position, Player.width, Player.height, DustID.RedTorch, 0f, 0f, 0, default, 3f);
						Main.dust[index2].noGravity = true;
						Main.dust[index2].velocity *= 9;
					}
				}
            }
			else if (FusedLens && Player.GetToggleValue("FusedLensInstall", false))
            {
                int buffType = ModContent.BuffType<TwinsInstall>();
                if (Player.HasBuff(buffType))
                {
                    Player.ClearBuff(buffType);
                }
                else
                {
                    SoundEngine.PlaySound(SoundID.Item119, Player.Center);

                    Player.AddBuff(ModContent.BuffType<TwinsInstall>(), 60);

                    int max = 60;
                    for (int i = 0; i < max; i++)
                    {
                        float scale = 3f;
                        int index2 = Dust.NewDust(Player.position, Player.width, Player.height, Main.rand.NextBool() ? 90 : 89, 0f, 0f, 0, new Color(), scale);
                        Main.dust[index2].noGravity = true;
                        Main.dust[index2].velocity *= scale * 3;
                    }
                }
            }
        }

        public void AmmoCycleKey()
        {
            SoundEngine.PlaySound(SoundID.Unlock, Player.Center);

            for (int i = 54; i <= 56; i++)
            {
                int j = i + 1;

                Item temp = Player.inventory[i];
                Player.inventory[i] = Player.inventory[j];
                Player.inventory[j] = temp;
            }
        }

        int lihzahrdFallCD;
        public void LihzahrdTreasureBoxUpdate()
        {
            if (lihzahrdFallCD > 0)
                lihzahrdFallCD--;

            if (Player.gravDir > 0 && Player.GetToggleValue("MasoGolem"))
            {
                if (lihzahrdFallCD <= 0 && !Player.mount.Active && Player.controlDown && Player.releaseDown && !Player.controlJump && Player.doubleTapCardinalTimer[0] > 0 && Player.doubleTapCardinalTimer[0] != 15)
                {
                    if (Player.velocity.Y != 0f)
                    {
                        if (Player.velocity.Y < 15f)
                        {
                            Player.velocity.Y = 15f;
                        }

                        if (GroundPound <= 0)
                        {
                            GroundPound = 1;
                        }
                    }
                }

                if (GroundPound > 0)
                {
                    lihzahrdFallCD = 60;

                    if (Player.velocity.Y == 0f && Player.controlDown)
                    {
                        Vector2 vec = Collision.TileCollision(Player.position, 15f * Vector2.UnitY, Player.width, Player.height, true, true, (int)Player.gravDir);
                        if (vec != Vector2.Zero)
                        {
                            Player.position += vec;
                            Player.velocity.Y = 15f;
                        }
                    }

                    if (Player.velocity.Y < 0f || Player.mount.Active || (Player.controlJump && !Player.controlDown))
                    {
                        GroundPound = 0;
                    }
                    else if (Player.velocity.Y == 0f && Player.oldVelocity.Y == 0f)
                    {
                        int x = (int)(Player.Center.X) / 16;
                        int y = (int)(Player.position.Y + Player.height + 8) / 16;
                        if (/*GroundPound > 15 && */x >= 0 && x < Main.maxTilesX && y >= 0 && y < Main.maxTilesY
                            && Main.tile[x, y] != null && Main.tile[x, y].HasUnactuatedTile && Main.tileSolid[Main.tile[x, y].TileType])
                        {
                            GroundPound = 0;

                            if (Player.GetToggleValue("MasoBoulder"))
                            {
                                if (!Main.dedServ)
                                    Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().Screenshake = 60;

                                if (Player.whoAmI == Main.myPlayer)
                                {
                                    //explosion
                                    Projectile.NewProjectile(Player.GetSource_Accessory(LihzahrdTreasureBoxItem), Player.Center, Vector2.Zero, ModContent.ProjectileType<MoonLordSunBlast>(), 0, 0f, Player.whoAmI);
                                    int p = Projectile.NewProjectile(Player.GetSource_Accessory(LihzahrdTreasureBoxItem), Player.Center, Vector2.Zero, ModContent.ProjectileType<Explosion>(), (int)(200 * Player.ActualClassDamage(DamageClass.Melee)), 9f, Player.whoAmI);
                                    if (p != Main.maxProjectiles)
                                        Main.projectile[p].DamageType = DamageClass.Melee;

                                    //boulders
                                    int dam = 50;
                                    if (MasochistSoul)
                                        dam *= 3;
                                    for (int i = -5; i <= 5; i += 2)
                                    {
                                        Projectile.NewProjectile(Player.GetSource_Accessory(LihzahrdTreasureBoxItem), Player.Center, -10f * Vector2.UnitY.RotatedBy(MathHelper.PiOver2 / 6 * i),
                                            ModContent.ProjectileType<LihzahrdBoulderFriendly>(), (int)(dam * Player.ActualClassDamage(DamageClass.Melee)), 7.5f, Player.whoAmI);
                                    }

                                    //geysers
                                    int baseDamage = (int)(50 * Player.ActualClassDamage(DamageClass.Melee));
                                    if (MasochistSoul)
                                        baseDamage *= 3;
                                    Projectile.NewProjectile(Player.GetSource_Accessory(LihzahrdTreasureBoxItem), Player.Center, Vector2.Zero, ModContent.ProjectileType<ExplosionSmall>(), baseDamage * 2, 12f, Player.whoAmI);
                                    y -= 2;
                                    for (int i = -3; i <= 3; i++)
                                    {
                                        if (i == 0)
                                            continue;
                                        int tilePosX = x + 16 * i;
                                        int tilePosY = y;
                                        if (Main.tile[tilePosX, tilePosY] != null && tilePosX >= 0 && tilePosX < Main.maxTilesX)
                                        {
                                            while (Main.tile[tilePosX, tilePosY] != null && tilePosY >= 0 && tilePosY < Main.maxTilesY
                                                && !(Main.tile[tilePosX, tilePosY].HasUnactuatedTile && Main.tileSolid[Main.tile[tilePosX, tilePosY].TileType]))
                                            {
                                                tilePosY++;
                                            }
                                            Projectile.NewProjectile(Player.GetSource_Accessory(LihzahrdTreasureBoxItem), tilePosX * 16 + 8, tilePosY * 16 + 8, 0f, -8f, ModContent.ProjectileType<GeyserFriendly>(), baseDamage, 8f, Player.whoAmI);
                                        }
                                    }
                                }
                            }
                        }

                    }
                    else
                    {
                        if (Player.controlDown && Player.velocity.Y < 15f)
                        {
                            Player.velocity.Y = 15f;
                        }

                        Player.maxFallSpeed = 15f;
                        GroundPound++;

                        for (int i = 0; i < 5; i++)
                        {
                            int d = Dust.NewDust(Player.position, Player.width, Player.height, DustID.Torch, Scale: 1.5f);
                            Main.dust[d].noGravity = true;
                            Main.dust[d].velocity *= 0.2f;
                        }
                    }
                }
            }
        }

        public void AbomWandUpdate()
        {
            if (AbominableWandRevived) //has been revived already
            {
                if (Player.statLife >= Player.statLifeMax2) //can revive again
                {
                    AbominableWandRevived = false;

                    SoundEngine.PlaySound(SoundID.Item28, Player.Center);

                    FargoSoulsUtil.DustRing(Player.Center, 50, 87, 8f, default, 2f);

                    for (int i = 0; i < 30; i++)
                    {
                        int d = Dust.NewDust(Player.position, Player.width, Player.height, 87, 0f, 0f, 0, default(Color), 2.5f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].velocity *= 8f;
                    }
                }
                else //cannot currently revive
                {
                    Player.AddBuff(ModContent.BuffType<AbomCooldown>(), 2);
                }
            }
        }

        public void DreadParryCounter()
        {
            SoundEngine.PlaySound(SoundID.Item119, Player.Center);

            if (Player.whoAmI == Main.myPlayer)
            {
                int projDamage = FargoSoulsUtil.HighestDamageTypeScaling(Player, 1000);

                const int max = 20;
                for (int i = 0; i < max; i++)
                {
                    void SharpTears(Vector2 pos, Vector2 vel)
                    {
                        int p = Projectile.NewProjectile(Player.GetSource_Accessory(DreadShellItem), pos, vel, ProjectileID.SharpTears, projDamage, 12f, Player.whoAmI, 0f, Main.rand.NextFloat(0.5f, 1f));
                        if (p != Main.maxProjectiles)
                        {
                            Main.projectile[p].DamageType = DamageClass.Default;
                            Main.projectile[p].usesLocalNPCImmunity = false;
                            Main.projectile[p].usesIDStaticNPCImmunity = true;
                            Main.projectile[p].idStaticNPCHitCooldown = 60;
                            Main.projectile[p].GetGlobalProjectile<FargoSoulsGlobalProjectile>().noInteractionWithNPCImmunityFrames = true;
                            Main.projectile[p].GetGlobalProjectile<FargoSoulsGlobalProjectile>().DeletionImmuneRank = 1;
                        }
                    }

                    SharpTears(Player.Center, 16f * Vector2.UnitX.RotatedBy(MathHelper.TwoPi / max * (i + Main.rand.NextFloat(-0.5f, 0.5f))));

                    for (int k = 0; k < 6; k++)
                    {
                        Vector2 spawnPos = Player.Bottom;
                        spawnPos.X += Main.rand.NextFloat(-256, 256);

                        bool foundTile = false;

                        for (int j = 0; j < 40; j++)
                        {
                            Tile tile = Framing.GetTileSafely(spawnPos);
                            if (tile.HasUnactuatedTile && (Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType]))
                            {
                                foundTile = true;
                                break;
                            }
                            spawnPos.Y += 16;
                        }

                        if (!foundTile)
                            spawnPos.Y = Player.Bottom.Y + Main.rand.NextFloat(-64, 64);

                        for (int j = 0; j < 40; j++)
                        {
                            Tile tile = Framing.GetTileSafely(spawnPos);
                            if (tile.HasUnactuatedTile && (Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType]))
                                spawnPos.Y -= 16;
                            else
                                break;
                        }

                        if (!Collision.SolidCollision(spawnPos, 0, 0))
                        {
                            spawnPos.Y += 16;
                            SharpTears(spawnPos, 16f * -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(30)));
                            break;
                        }
                    }
                }
            }
        }

        public void PumpkingsCapeCounter(int damage)
        {
            SoundEngine.PlaySound(SoundID.Item62, Player.Center);

            if (Player.whoAmI == Main.myPlayer)
            {
                int heal = getHealMultiplier(damage);
                Player.statLife += heal;
                if (Player.statLife > Player.statLifeMax2)
                    Player.statLife = Player.statLifeMax2;
                Player.HealEffect(heal);

                int counterDamage = Math.Min(1000, damage);
                for (int i = 0; i < 30; i++)
                {
                    Vector2 vel = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi);
                    vel *= Main.rand.NextFloat(12f, 24f);
                    FargoSoulsUtil.NewSummonProjectile(Player.GetSource_Accessory(PumpkingsCapeItem), Player.Center, vel, ModContent.ProjectileType<SpookyScythe>(), counterDamage, 6f, Player.whoAmI);
                }
            }
        }

        public void DeerSinewEffect()
        {
            if (DeerSinewFreezeCD > 0)
                DeerSinewFreezeCD--;

            if (!Player.GetToggleValue("DeerSinewDash", false) || HasDash || Player.mount.Active || Player.whoAmI != Main.myPlayer)
                return;

            HasDash = true;

            DeerSinewNerf = true;

            if (dashCD <= 0)
            {
                float dashSpeed = 12f;

                if (Player.controlRight && Player.releaseRight)
                {
                    if (Player.doubleTapCardinalTimer[2] > 0 && Player.doubleTapCardinalTimer[2] != 15)
                    {
                        dashCD = 60;
                        if (IsDashingTimer < 15)
                            IsDashingTimer = 15;
                        Player.velocity.X = dashSpeed;
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                            NetMessage.SendData(MessageID.PlayerControls, number: Player.whoAmI);
                    }
                }

                if (Player.controlLeft && Player.releaseLeft)
                {
                    if (Player.doubleTapCardinalTimer[3] > 0 && Player.doubleTapCardinalTimer[3] != 15)
                    {
                        dashCD = 60;
                        if (IsDashingTimer < 15)
                            IsDashingTimer = 15;
                        Player.velocity.X = -dashSpeed;
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                            NetMessage.SendData(MessageID.PlayerControls, number: Player.whoAmI);
                    }
                }
            }

            if (IsDashingTimer > 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    int d = Dust.NewDust(Player.position, Player.width, Player.height, DustID.GemSapphire, Scale: 1.25f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 0.2f;
                }
            }
        }

        public float DeerSinewCritNerf()
        {
            float ratio = Math.Min(Player.velocity.Length() / 16f, 1f);
            return MathHelper.Lerp(1f, 0.75f, ratio);
        }

        #endregion maso acc

        public bool TryParryAttack(int damage)
        {
            if (GuardRaised && shieldTimer > 0 && !Player.immune)
            {
                Player.immune = true;
                int invul = Player.longInvince ? 90 : 60;
                int extrashieldCD = 40;

                if (DreadShellItem != null || PumpkingsCapeItem != null)
                {
                    invul += 60;
                    extrashieldCD = LONG_SHIELD_COOLDOWN;
                }

                if (DreadShellItem != null)
                {
                    DreadParryCounter();
                }

                if (PumpkingsCapeItem != null)
                {
                    PumpkingsCapeCounter(damage);
                }

                if (SilverEnchantItem != null)
                {
                    extrashieldCD = BASE_SHIELD_COOLDOWN;

                    if (TerraForce)
                        Player.AddBuff(BuffID.ParryDamageBuff, 300);

                    Projectile.NewProjectile(Player.GetSource_Misc(""), Player.Center, Vector2.Zero, ModContent.ProjectileType<IronParry>(), 0, 0f, Main.myPlayer);
                }

                Player.immuneTime = invul;
                Player.hurtCooldowns[0] = invul;
                Player.hurtCooldowns[1] = invul;
                ParryDebuffImmuneTime = invul;
                shieldCD = invul + extrashieldCD;

                foreach (int debuff in FargowiltasSouls.DebuffIDs) //immune to all debuffs
                {
                    if (!Player.HasBuff(debuff))
                        Player.buffImmune[debuff] = true;
                }

                return true;
            }

            return false;
        }

        private const int BASE_PARRY_WINDOW = 20;
        private const int BASE_SHIELD_COOLDOWN = 100;
        private const int HARD_PARRY_WINDOW = 10;
        private const int LONG_SHIELD_COOLDOWN = 360;

        void RaisedShieldEffects()
        {
            if (DreadShellItem != null)
            {
                if (!MasochistSoul)
                    DreadShellVulnerabilityTimer = 60;
            }

            if (PumpkingsCapeItem != null) //strong aura effect
            {
                const float distance = 300f;
                for (int i = 0; i < Main.maxNPCs; i++)
                    if (Main.npc[i].active && !Main.npc[i].friendly && Main.npc[i].Distance(Player.Center) < distance)
                        Main.npc[i].AddBuff(ModContent.BuffType<Rotting>(), 600);

                for (int i = 0; i < 20; i++)
                {
                    Vector2 offset = new Vector2();
                    double angle = Main.rand.NextDouble() * 2d * Math.PI;
                    offset.X += (float)(Math.Sin(angle) * distance);
                    offset.Y += (float)(Math.Cos(angle) * distance);
                    Dust dust = Main.dust[Dust.NewDust(Player.Center + offset - new Vector2(4, 4), 0, 0, 119, 0, 0, 100, Color.White, 1f)];
                    dust.velocity = Player.velocity;
                    if (Main.rand.NextBool(3))
                        dust.velocity += Vector2.Normalize(offset) * -5f;
                    dust.noGravity = true;
                }
            }

            if ((DreadShellItem != null || PumpkingsCapeItem != null) && SilverEnchantItem == null)
            {
                Player.velocity.X *= 0.85f;
                if (Player.velocity.Y < 0)
                    Player.velocity.Y *= 0.85f;
            }

            int cooldown = BASE_SHIELD_COOLDOWN;
            if (DreadShellItem != null || PumpkingsCapeItem != null)
                cooldown = LONG_SHIELD_COOLDOWN;
            if (SilverEnchantItem != null)
                cooldown = BASE_SHIELD_COOLDOWN;

            if (shieldCD < cooldown)
                shieldCD = cooldown;
        }

        public void UpdateShield()
        {
            GuardRaised = false;

            //no need when player has brand of inferno
            if ((SilverEnchantItem == null && DreadShellItem == null && PumpkingsCapeItem == null) ||
                (Player.inventory[Player.selectedItem].type == ItemID.DD2SquireDemonSword || Player.inventory[Player.selectedItem].type == ItemID.BouncingShield))
            {
                shieldTimer = 0;
                wasHoldingShield = false;
                return;
            }

            Player.shieldRaised = Player.selectedItem != 58 && Player.controlUseTile && !Player.tileInteractionHappened && Player.releaseUseItem
                && !Player.controlUseItem && !Player.mouseInterface && !CaptureManager.Instance.Active && !Main.HoveringOverAnNPC
                && !Main.SmartInteractShowingGenuine && !Player.mount.Active &&
                Player.itemAnimation == 0 && Player.itemTime == 0 && Player.reuseDelay == 0 && PlayerInput.Triggers.Current.MouseRight;

            if (Player.shieldRaised)
            {
                GuardRaised = true;

                for (int i = 3; i < 8 + Player.extraAccessorySlots; i++)
                {
                    if (Player.shield == -1 && Player.armor[i].shieldSlot != -1)
                        Player.shield = Player.armor[i].shieldSlot;
                }

                if (shieldTimer > 0)
                    shieldTimer--;

                if (!wasHoldingShield)
                {
                    wasHoldingShield = true;

                    if (shieldCD == 0) //if cooldown over, enable parry
                    {
                        if (DreadShellItem != null || PumpkingsCapeItem != null)
                            shieldTimer = HARD_PARRY_WINDOW;
                        if (SilverEnchantItem != null)
                            shieldTimer = BASE_PARRY_WINDOW;
                    }

                    Player.itemAnimation = 0;
                    Player.itemTime = 0;
                    Player.reuseDelay = 0;
                }
                else //doing this so that on the first tick, these things DO NOT run
                {
                    RaisedShieldEffects();
                }

                if (shieldTimer == 1) //parry window over
                {
                    SoundEngine.PlaySound(SoundID.Item27, Player.Center);

                    List<int> dusts = new List<int>();
                    if (DreadShellItem != null)
                        dusts.Add(DustID.LifeDrain);
                    if (PumpkingsCapeItem != null)
                        dusts.Add(87);
                    if (SilverEnchantItem != null)
                        dusts.Add(66);

                    if (dusts.Count > 0)
                    {
                        for (int i = 0; i < 20; i++)
                        {
                            int d = Dust.NewDust(Player.position, Player.width, Player.height, Main.rand.Next(dusts), 0, 0, 0, default, 1.5f);
                            Main.dust[d].noGravity = true;
                            Main.dust[d].velocity *= 3f;
                        }
                    }
                }
            }
            else
            {
                shieldTimer = 0;

                if (wasHoldingShield)
                {
                    wasHoldingShield = false;

                    Player.shield_parry_cooldown = 0; //prevent that annoying tick noise
                    //Player.shieldParryTimeLeft = 0;
                    //ironShieldTimer = 0;
                }

                if (shieldCD == 1) //cooldown over
                {
                    SoundEngine.PlaySound(SoundID.Item28, Player.Center); //make a sound for refresh

                    List<int> dusts = new List<int>();
                    if (DreadShellItem != null)
                        dusts.Add(DustID.LifeDrain);
                    if (PumpkingsCapeItem != null)
                        dusts.Add(87);
                    if (SilverEnchantItem != null)
                        dusts.Add(66);
                    
                    if (dusts.Count > 0)
                    {
                        for (int i = 0; i < 30; i++)
                        {
                            int d = Dust.NewDust(Player.position, Player.width, Player.height, Main.rand.Next(dusts), 0, 0, 0, default, 2.5f);
                            Main.dust[d].noGravity = true;
                            Main.dust[d].velocity *= 6f;
                        }
                    }
                }

                if (shieldCD > 0)
                    shieldCD--;
            }

            //Main.NewText($"{ironShieldCD}, {ironShieldTimer}");
        }
    }
}
