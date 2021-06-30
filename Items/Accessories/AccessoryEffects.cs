using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameInput;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Capture;
using FargowiltasSouls.NPCs;
using FargowiltasSouls.Toggler;
using FargowiltasSouls.Projectiles;
using FargowiltasSouls.Buffs.Souls;
using FargowiltasSouls.Projectiles.Souls;
using FargowiltasSouls.NPCs.MutantBoss;
using FargowiltasSouls.Projectiles.Minions;
using System.Collections.Generic;

namespace FargowiltasSouls
{
    public partial class FargoPlayer
    {
        public void FlowerBoots()
        {
            if (!player.GetToggleValue("SupersonicFlower"))
                return;

            int x = (int)player.Center.X / 16;
            int y = (int)(player.position.Y + player.height - 1f) / 16;

            if (Main.tile[x, y] == null)
            {
                Main.tile[x, y] = new Tile();
            }

            if (!Main.tile[x, y].active() && Main.tile[x, y].liquid == 0 && Main.tile[x, y + 1] != null && WorldGen.SolidTile(x, y + 1))
            {
                Main.tile[x, y].frameY = 0;
                Main.tile[x, y].slope(0);
                Main.tile[x, y].halfBrick(false);

                if (Main.tile[x, y + 1].type == 2)
                {
                    if (Main.rand.Next(2) == 0)
                    {
                        Main.tile[x, y].active(true);
                        Main.tile[x, y].type = 3;
                        Main.tile[x, y].frameX = (short)(18 * Main.rand.Next(6, 11));
                        while (Main.tile[x, y].frameX == 144)
                        {
                            Main.tile[x, y].frameX = (short)(18 * Main.rand.Next(6, 11));
                        }
                    }
                    else
                    {
                        Main.tile[x, y].active(true);
                        Main.tile[x, y].type = 73;
                        Main.tile[x, y].frameX = (short)(18 * Main.rand.Next(6, 21));

                        while (Main.tile[x, y].frameX == 144)
                        {
                            Main.tile[x, y].frameX = (short)(18 * Main.rand.Next(6, 21));
                        }
                    }

                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        NetMessage.SendTileSquare(-1, x, y, 1, TileChangeType.None);
                    }
                }
                else if (Main.tile[x, y + 1].type == 109)
                {
                    if (Main.rand.Next(2) == 0)
                    {
                        Main.tile[x, y].active(true);
                        Main.tile[x, y].type = 110;
                        Main.tile[x, y].frameX = (short)(18 * Main.rand.Next(4, 7));

                        while (Main.tile[x, y].frameX == 90)
                        {
                            Main.tile[x, y].frameX = (short)(18 * Main.rand.Next(4, 7));
                        }
                    }
                    else
                    {
                        Main.tile[x, y].active(true);
                        Main.tile[x, y].type = 113;
                        Main.tile[x, y].frameX = (short)(18 * Main.rand.Next(2, 8));

                        while (Main.tile[x, y].frameX == 90)
                        {
                            Main.tile[x, y].frameX = (short)(18 * Main.rand.Next(2, 8));
                        }
                    }
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        NetMessage.SendTileSquare(-1, x, y, 1, TileChangeType.None);
                    }
                }
                else if (Main.tile[x, y + 1].type == 60)
                {
                    Main.tile[x, y].active(true);
                    Main.tile[x, y].type = 74;
                    Main.tile[x, y].frameX = (short)(18 * Main.rand.Next(9, 17));

                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        NetMessage.SendTileSquare(-1, x, y, 1, TileChangeType.None);
                    }
                }
            }
        }

        #region enchantments

        public void BeeEffect(bool hideVisual)
        {
            player.strongBees = true;
            //bees ignore defense
            BeeEnchant = true;
        }

        public void BeetleEffect()
        {
            if (!player.GetToggleValue("Beetle"))
                return;

            if (player.beetleDefense) //don't let this stack
                return;

            player.beetleDefense = true;
            player.beetleCounter += 1f;
            int num5 = 180;
            int cap = TerrariaSoul ? 3 : 1;
            if (player.beetleCounter >= num5)
            {
                if (player.beetleOrbs > 0 && player.beetleOrbs < cap)
                {
                    for (int k = 0; k < 22; k++)
                    {
                        if (player.buffType[k] >= 95 && player.buffType[k] <= 96)
                        {
                            player.DelBuff(k);
                        }
                    }
                }
                if (player.beetleOrbs < cap)
                {
                    player.AddBuff(95 + player.beetleOrbs, 5, false);
                    player.beetleCounter = 0f;
                }
                else
                {
                    player.beetleCounter = num5;
                }
            }

            if (!player.beetleDefense && !player.beetleOffense)
            {
                player.beetleCounter = 0f;
            }
            else
            {
                player.beetleFrameCounter++;
                if (player.beetleFrameCounter >= 1)
                {
                    player.beetleFrameCounter = 0;
                    player.beetleFrame++;
                    if (player.beetleFrame > 2)
                    {
                        player.beetleFrame = 0;
                    }
                }
                for (int l = player.beetleOrbs; l < 3; l++)
                {
                    player.beetlePos[l].X = 0f;
                    player.beetlePos[l].Y = 0f;
                }
                for (int m = 0; m < player.beetleOrbs; m++)
                {
                    player.beetlePos[m] += player.beetleVel[m];
                    Vector2[] expr_6EcCp0 = player.beetleVel;
                    int expr_6EcCp1 = m;
                    expr_6EcCp0[expr_6EcCp1].X = expr_6EcCp0[expr_6EcCp1].X + Main.rand.Next(-100, 101) * 0.005f;
                    Vector2[] expr71ACp0 = player.beetleVel;
                    int expr71ACp1 = m;
                    expr71ACp0[expr71ACp1].Y = expr71ACp0[expr71ACp1].Y + Main.rand.Next(-100, 101) * 0.005f;
                    float num6 = player.beetlePos[m].X;
                    float num7 = player.beetlePos[m].Y;
                    float num8 = (float)Math.Sqrt(num6 * num6 + num7 * num7);
                    if (num8 > 100f)
                    {
                        num8 = 20f / num8;
                        num6 *= -num8;
                        num7 *= -num8;
                        int num9 = 10;
                        player.beetleVel[m].X = (player.beetleVel[m].X * (num9 - 1) + num6) / num9;
                        player.beetleVel[m].Y = (player.beetleVel[m].Y * (num9 - 1) + num7) / num9;
                    }
                    else if (num8 > 30f)
                    {
                        num8 = 10f / num8;
                        num6 *= -num8;
                        num7 *= -num8;
                        int num10 = 20;
                        player.beetleVel[m].X = (player.beetleVel[m].X * (num10 - 1) + num6) / num10;
                        player.beetleVel[m].Y = (player.beetleVel[m].Y * (num10 - 1) + num7) / num10;
                    }
                    num6 = player.beetleVel[m].X;
                    num7 = player.beetleVel[m].Y;
                    num8 = (float)Math.Sqrt(num6 * num6 + num7 * num7);
                    if (num8 > 2f)
                    {
                        player.beetleVel[m] *= 0.9f;
                    }
                    player.beetlePos[m] -= player.velocity * 0.25f;
                }
            }
        }

        public void CactusEffect()
        {
            if (player.GetToggleValue("Cactus"))
            {
                CactusEnchant = true;
            }
        }

        public void ChloroEffect(bool hideVisual)
        {
            ChloroEnchant = true;

            if (player.whoAmI == Main.myPlayer && player.GetToggleValue("Chlorophyte") && player.ownedProjectileCounts[ModContent.ProjectileType<Chlorofuck>()] == 0)
            {
                int dmg = 100;

                if (NatureForce || WizardEnchant)
                {
                    dmg = 200;
                }

                const int max = 5;
                float rotation = 2f * (float)Math.PI / max;

                for (int i = 0; i < max; i++)
                {
                    Vector2 spawnPos = player.Center + new Vector2(60, 0f).RotatedBy(rotation * i);
                    int p = Projectile.NewProjectile(spawnPos, Vector2.Zero, ModContent.ProjectileType<Chlorofuck>(), dmg, 10f, player.whoAmI, 0, rotation * i);
                    Main.projectile[p].GetGlobalProjectile<FargoGlobalProjectile>().CanSplit = false;
                    Main.projectile[p].netUpdate = true;
                }
            }
        }

        public void CopperEffect(NPC target)
        {
            int dmg = 20;
            int maxTargets = 5;
            int cdLength = 450;

            if (TerraForce)
            {
                dmg = 100;
                maxTargets = 10;
                cdLength = 200;
            }

            float closestDist = 500f;
            NPC closestNPC;

            for (int i = 0; i < maxTargets; i++)
            {
                closestNPC = null;

                for (int j = 0; j < 200; j++)
                {
                    NPC npc = Main.npc[j];
                    if (npc.active && npc != target && npc.Distance(target.Center) < closestDist)
                    {
                        closestNPC = npc;
                        break;
                    }
                }

                if (closestNPC != null)
                {
                    Vector2 ai = closestNPC.Center - target.Center;
                    float ai2 = Main.rand.Next(100);
                    Vector2 velocity = Vector2.Normalize(ai) * 20;

                    Projectile p = FargoGlobalProjectile.NewProjectileDirectSafe(target.Center, velocity, ModContent.ProjectileType<CopperLightning>(), HighestDamageTypeScaling(dmg), 0f, player.whoAmI, ai.ToRotation(), ai2);
                }
                else
                {
                    break;
                }

                target = closestNPC;
            }

            copperCD = cdLength;
        }

        public void CrimsonEffect(bool hideVisual)
        {
            if (CrimsonRegen)
            {
                CrimsonRegenTimer++;

                if (CrimsonRegenTimer > 30)
                {
                    CrimsonRegenTimer = 0;

                    int heal = 5;

                    HealPlayer(heal);

                    CrimsonRegenSoFar += heal;

                    //done regenning
                    if (CrimsonRegenSoFar >= CrimsonTotalToRegen)
                    {
                        CrimsonTotalToRegen = 0;
                        CrimsonRegenSoFar = 0;
                        player.DelBuff(player.FindBuffIndex(ModContent.BuffType<CrimsonRegen>()));
                    }
                }
            }

            //player.crimsonRegen = true;

            CrimsonEnchant = true;
            
        }

        public void DarkArtistEffect(bool hideVisual)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<FlameburstMinion>()] == 0)
            {
                DarkSpawn = true;
                //DarkSpawnCD = 60;
            }

            player.setApprenticeT3 = true;
            DarkEnchant = true;

            //int maxTowers = 3;

            //if (TerrariaSoul)
            //{
            //    maxTowers = 5;
            //}
            //else if (ShadowForce || WizardEnchant)
            //{
            //    maxTowers = 4;
            //}

            //spawn tower boi
            if (player.whoAmI == Main.myPlayer && DarkSpawn && DarkSpawnCD <= 0 && player.GetToggleValue("DarkArt"))
                //&& player.ownedProjectileCounts[ModContent.ProjectileType<FlameburstMinion>()] < maxTowers)
            {
                Projectile proj = Projectile.NewProjectileDirect(player.Center, Vector2.Zero, ModContent.ProjectileType<FlameburstMinion>(), 0, 0f, player.whoAmI);
                proj.netUpdate = true; // TODO make this proj sync meme

                DarkSpawn = false;
                DarkSpawnCD = 60;
            }

            if (DarkSpawnCD > 0)
            {
                DarkSpawnCD--;
            }

            
        }

        public void ForbiddenEffect()
        {
            if (!player.GetToggleValue("Forbidden"))
                return;
            ForbiddenEnchant = true;

            //player.setForbidden = true;
            //add cd

            if ((player.controlDown && player.releaseDown))
            {
                if (player.doubleTapCardinalTimer[0] > 0 && player.doubleTapCardinalTimer[0] != 15)
                {
                    CommandForbiddenStorm();

                    /*Vector2 mouse = Main.MouseWorld;

                    if (player.ownedProjectileCounts[ModContent.ProjectileType<ForbiddenTornado>()] > 0)
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

                    Projectile.NewProjectile(mouse.X, mouse.Y - 10, 0f, 0f, ModContent.ProjectileType<ForbiddenTornado>(), (WoodForce || WizardEnchant) ? 45 : 15, 0f, player.whoAmI);*/
                }
            }


            //player.UpdateForbiddenSetLock();
            Lighting.AddLight(player.Center, 0.8f, 0.7f, 0.2f);
            //storm boosted
        }

        public void CommandForbiddenStorm()
        {
            List<int> list = new List<int>();
            for (int i = 0; i < 1000; i++)
            {
                Projectile projectile = Main.projectile[i];
                if (projectile.active && projectile.type == ModContent.ProjectileType<ForbiddenTornado>() && projectile.owner == player.whoAmI)
                {
                    list.Add(i);
                }
            }

            Vector2 center = player.Center;
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

            //Main.NewText(" " + (list.Count <= 1) + " " + flag3 + " " + player.CheckMana(20, true, false));

            bool flag = (list.Count <= 1);
            flag &= flag3;

            

            if (flag)
            {
                flag = player.CheckMana(20, true, false);
                if (flag)
                {
                    player.manaRegenDelay = (int)player.maxRegenDelay;
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

            int damage = (int)(20f * (1f + player.magicDamage + player.minionDamage - 2f));
            Projectile arg_37A_0 = Main.projectile[Projectile.NewProjectile(mouse, Vector2.Zero, ModContent.ProjectileType<ForbiddenTornado>(), damage, 0f, Main.myPlayer, 0f, 0f)];
        }

        public void FossilEffect(bool hideVisual)
        {
            //bone zone
            FossilEnchant = true;
        }

        public void FrostEffect(bool hideVisual)
        {
            FrostEnchant = true;

            if (player.whoAmI == Main.myPlayer && player.GetToggleValue("Frost"))
            {
                if (icicleCD == 0 && IcicleCount < 10 && player.ownedProjectileCounts[ModContent.ProjectileType<FrostIcicle>()] < 10)
                {
                    IcicleCount++;

                    //kill all current ones
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        Projectile proj = Main.projectile[i];

                        if (proj.active && proj.type == ModContent.ProjectileType<FrostIcicle>() && proj.owner == player.whoAmI)
                        {
                            proj.active = false;
                            proj.netUpdate = true;
                        }
                    }

                    //respawn in formation
                    for (int i = 0; i < IcicleCount; i++)
                    {
                        float radians = (360f / IcicleCount) * i * (float)(Math.PI / 180);
                        Projectile frost = FargoGlobalProjectile.NewProjectileDirectSafe(player.Center, Vector2.Zero, ModContent.ProjectileType<FrostIcicle>(), 0, 0f, player.whoAmI, 5, radians);
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
                        vector6 = vector6.RotatedBy((j - (20 / 2 - 1)) * 6.28318548f / 20) + player.Center;
                        Vector2 vector7 = vector6 - player.Center;
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

                if (icicleCD != 0)
                {
                    icicleCD--;
                }

                if (IcicleCount >= 1 && player.controlUseItem && player.HeldItem.damage > 0 && player.HeldItem.createTile == -1 && player.HeldItem.createWall == -1 && player.HeldItem.ammo == AmmoID.None && player.HeldItem.hammer == 0 && player.HeldItem.pick == 0 && player.HeldItem.axe == 0)
                {
                    int dmg = 75;

                    if (NatureForce || WizardEnchant)
                    {
                        dmg = 150;
                    }

                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        Projectile proj = Main.projectile[i];

                        if (proj.active && proj.type == ModContent.ProjectileType<FrostIcicle>() && proj.owner == player.whoAmI)
                        {
                            Vector2 vel = (Main.MouseWorld - proj.Center).SafeNormalize(-Vector2.UnitY) * 25;

                            int p = Projectile.NewProjectile(proj.Center, vel, ProjectileID.Blizzard, HighestDamageTypeScaling(dmg), 1f, player.whoAmI);
                            proj.Kill();

                            Main.projectile[p].GetGlobalProjectile<FargoGlobalProjectile>().CanSplit = false;
                            Main.projectile[p].GetGlobalProjectile<FargoGlobalProjectile>().FrostFreeze = true;
                        }
                    }

                    IcicleCount = 0;
                    icicleCD = 120;
                }
            }

           
        }

        public void GladiatorEffect(bool hideVisual)
        {
            GladEnchant = true;

            if (gladCount > 0)
            {
                gladCount--;
            }


            
        }

        public void GoldEffect(bool hideVisual)
        {
            //gold ring
            player.goldRing = true;
            //lucky coin
            if (player.whoAmI == Main.myPlayer && player.GetToggleValue("Gold"))
                player.coins = true;
            //discount card
            player.discount = true;
            //midas
            GoldEnchant = true;

            
        }

        public void HallowEffect(bool hideVisual)
        {
            SilverEnchant = true;
            HallowEnchant = true;

            int dmg = 50;

            if (SpiritForce || WizardEnchant)
            {
                dmg = 150;
            }

            AddMinion(player.GetToggleValue("Hallowed"), ModContent.ProjectileType<HallowSword>(), (int)(dmg * player.minionDamage), 0f);

            //reflect proj
            if (player.GetToggleValue("HallowS") && !noDodge && !player.HasBuff(mod.BuffType("HallowCooldown")))
            {
                const int focusRadius = 50;

                //if (Math.Abs(player.velocity.X) < .5f && Math.Abs(player.velocity.Y) < .5f)
                for (int i = 0; i < 20; i++)
                {
                    Vector2 offset = new Vector2();
                    double angle = Main.rand.NextDouble() * 2d * Math.PI;
                    offset.X += (float)(Math.Sin(angle) * focusRadius);
                    offset.Y += (float)(Math.Cos(angle) * focusRadius);
                    Dust dust = Main.dust[Dust.NewDust(
                        player.Center + offset - new Vector2(4, 4), 0, 0,
                        DustID.GoldFlame, 0, 0, 100, Color.White, 0.5f
                        )];
                    dust.velocity = player.velocity;
                    dust.noGravity = true;
                }

                Main.projectile.Where(x => x.active && x.hostile && x.damage > 0).ToList().ForEach(x =>
                {
                    if (Vector2.Distance(x.Center, player.Center) <= focusRadius + Math.Min(x.width, x.height) / 2
                        && !x.GetGlobalProjectile<FargoGlobalProjectile>().ImmuneToGuttedHeart && !x.GetGlobalProjectile<FargoGlobalProjectile>().ImmuneToMutantBomb)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            int dustId = Dust.NewDust(new Vector2(x.position.X, x.position.Y + 2f), x.width, x.height + 5, DustID.GoldFlame, x.velocity.X * 0.2f, x.velocity.Y * 0.2f, 100, default(Color), 3f);
                            Main.dust[dustId].noGravity = true;
                        }

                        // Set ownership
                        x.hostile = false;
                        x.friendly = true;
                        x.owner = player.whoAmI;

                        // Turn around
                        x.velocity *= -1f;

                        // Flip sprite
                        if (x.Center.X > player.Center.X)
                        {
                            x.direction = 1;
                            x.spriteDirection = 1;
                        }
                        else
                        {
                            x.direction = -1;
                            x.spriteDirection = -1;
                        }

                        // Don't know if this will help but here it is
                        x.netUpdate = true;

                        player.AddBuff(mod.BuffType("HallowCooldown"), 600);
                    }
                });
            }

           
        }

        public int ironShieldTimer = 0;
        public int ironShieldCD = 0;
        public bool wasHoldingShield = false;

        public void IronEffect()
        {
            //no need when player has brand of inferno
            if (player.inventory[player.selectedItem].type == ItemID.DD2SquireDemonSword)
            {
                ironShieldTimer = 0;
                wasHoldingShield = false;
                return;
            }

            player.shieldRaised = player.selectedItem != 58 && player.controlUseTile && !player.tileInteractionHappened && player.releaseUseItem 
                && !player.controlUseItem && !player.mouseInterface && !CaptureManager.Instance.Active && !Main.HoveringOverAnNPC 
                && !Main.SmartInteractShowingGenuine && !player.mount.Active &&
                player.itemAnimation == 0 && player.itemTime == 0 && PlayerInput.Triggers.Current.MouseRight;

            /*if (ironShieldTimer > 0)
            {
                //ironShieldTimer++;
                //player.shieldParryTimeLeft = internalTimer;
                if (ironShieldTimer > 30)
                {
                    //player.shieldParryTimeLeft = 0;
                    ironShieldTimer = 0;

                    if (ironShieldCD == 0)
                    {
                        ironShieldCD = 60;
                    }
                }
            }*/

            if (player.shieldRaised)
            {
                IronGuard = true;

                for (int i = 3; i < 8 + player.extraAccessorySlots; i++)
                {
                    if (player.shield == -1 && player.armor[i].shieldSlot != -1)
                        player.shield = player.armor[i].shieldSlot;
                }

                if (ironShieldTimer > 0) //above the line where it's set so its checked in FargoGlobalItem ok
                    ironShieldTimer--;

                if (!wasHoldingShield)
                {
                    wasHoldingShield = true;

                    if (ironShieldCD == 0) //if cooldown over, enable parry
                        ironShieldTimer = 20;

                    player.itemAnimation = 0;
                    player.itemTime = 0;
                    player.reuseDelay = 0;
                }

                if (ironShieldTimer == 1) //parry window over
                {
                    Main.PlaySound(SoundID.Item27, player.Center); //make a sound for refresh
                    for (int i = 0; i < 20; i++)
                    {
                        int d = Dust.NewDust(player.position, player.width, player.height, 1, 0, 0, 0, default, 1.5f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].velocity *= 3f;
                    }
                }

                int cooldown = 40;
                if (ironShieldCD < cooldown)
                    ironShieldCD = cooldown;
            }
            else
            {
                ironShieldTimer = 0;

                if (wasHoldingShield)
                {
                    wasHoldingShield = false;
                    player.shield_parry_cooldown = 0; //prevent that annoying tick noise
                    //player.shieldParryTimeLeft = 0;
                    //ironShieldTimer = 0;
                }

                if (ironShieldCD == 1) //cooldown over
                {
                    Main.PlaySound(SoundID.Item28, player.Center); //make a sound for refresh
                    for (int i = 0; i < 30; i++)
                    {
                        int d = Dust.NewDust(player.position, player.width, player.height, 66, 0, 0, 0, default, 2.5f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].velocity *= 6f;
                    }
                }

                if (ironShieldCD > 0)
                    ironShieldCD--;
            }

            //Main.NewText($"{ironShieldCD}, {ironShieldTimer}");
        }

        public bool CanJungleJump = false;
        private bool jungleJumping = false;
        private int savedRocketTime;

        public void JungleEffect()
        {
            if (player.whoAmI != Main.myPlayer)
                return;

            if (player.controlJump && player.GetToggleValue("Jungle"))
            {
                if (player.jumpAgainBlizzard || player.jumpAgainSandstorm || player.jumpAgainCloud || player.jumpAgainFart ||  player.jumpAgainSail || player.jumpAgainUnicorn)
                {
                }
                else
                {
                    if (player.jump == 0 && player.releaseJump && player.velocity.Y != 0f && !player.mount.Active && CanJungleJump)
                    {
                        player.velocity.Y = -Player.jumpSpeed * player.gravDir;
                        player.jump = (int)((double)Player.jumpHeight * 3);

                        jungleJumping = true;
                        JungleCD = 0;
                        CanJungleJump = false;
                    }
                }
            }

            if (jungleJumping && player.GetToggleValue("Jungle"))
            {
                if (player.rocketBoots > 0)
                {
                    savedRocketTime = player.rocketTimeMax;
                    player.rocketTime = 0;
                }

                player.runAcceleration *= 3f;
                player.maxRunSpeed *= 2f;

                //spwn cloud
                if (JungleCD == 0)
                {
                    int dmg = (NatureForce || WizardEnchant) ? 150 : 30;
                    Main.PlaySound(SoundID.Item, (int)player.position.X, (int)player.position.Y, 62, 0.5f);
                    FargoGlobalProjectile.XWay(10, new Vector2(player.Center.X, player.Center.Y + (player.height / 2)), ProjectileID.SporeCloud, 3f, HighestDamageTypeScaling(dmg), 0f);

                    JungleCD = 8;
                }

                if (player.jump == 0 || player.velocity == Vector2.Zero)
                {
                    jungleJumping = false;
                    player.rocketTime = savedRocketTime;
                }
            }
            else if(player.jump <= 0 && player.velocity.Y == 0f)
            {
                CanJungleJump = true;
            }

            if (JungleCD != 0)
            {
                JungleCD--;
            }
        }

        public void MeteorEffect()
        {
            MeteorEnchant = true;

            if (player.whoAmI == Main.myPlayer && player.GetToggleValue("Meteor"))
            {
                int damage = 50;

                if (meteorShower)
                {
                    if (meteorTimer % 2 == 0)
                    {
                        int p = Projectile.NewProjectile(player.Center.X + Main.rand.Next(-1000, 1000), player.Center.Y - 1000, Main.rand.Next(-2, 2), 0f + Main.rand.Next(8, 12), Main.rand.Next(424, 427), HighestDamageTypeScaling(damage), 0f, player.whoAmI, 0f, 0.5f + (float)Main.rand.NextDouble() * 0.3f);
                        if (p != Main.maxProjectiles)
                        {
                            Main.projectile[p].GetGlobalProjectile<FargoGlobalProjectile>().CanSplit = false;
                            Main.projectile[p].netUpdate = true;
                            if (ModLoader.GetMod("Fargowiltas") != null)
                                ModLoader.GetMod("Fargowiltas").Call("LowRenderProj", Main.projectile[p]);
                        }
                    }

                    meteorTimer--;

                    if (meteorTimer <= 0)
                    {
                        meteorCD = 300;

                        if (CosmoForce || WizardEnchant)
                        {
                            meteorCD = 200;
                        }

                        meteorTimer = 150;
                        meteorShower = false;
                    }
                }
                else
                {
                    if (player.controlUseItem)
                    {
                        meteorCD--;

                        if (meteorCD == 0)
                        {
                            meteorShower = true;
                        }
                    }
                    else
                    {
                        meteorCD = 300;
                    }
                }
            }
        }

        public void MinerEffect(bool hideVisual, float pickSpeed)
        {
            player.pickSpeed -= pickSpeed;

            if (player.GetToggleValue("MiningSpelunk"))
            {
                player.findTreasure = true;
            }

            if (player.GetToggleValue("MiningHunt"))
            {
                player.detectCreature = true;
            }

            if (player.GetToggleValue("MiningDanger"))
            {
                player.dangerSense = true;
            }

            if (player.GetToggleValue("MiningShine"))
            {
                Lighting.AddLight(player.Center, 0.8f, 0.8f, 0f);
            }

            MinerEnchant = true;

            
        }

        public void MoltenEffect()
        {
            MoltenEnchant = true;

            if (player.GetToggleValue("Molten") && player.whoAmI == Main.myPlayer)
            {
                player.inferno = true;
                Lighting.AddLight((int)(player.Center.X / 16f), (int)(player.Center.Y / 16f), 0.65f, 0.4f, 0.1f);
                int buff = BuffID.OnFire;
                float distance = 200f;

                int baseDamage = 30;

                if (NatureForce || WizardEnchant)
                {
                    baseDamage *= 2;
                }
                
                int damage = HighestDamageTypeScaling(baseDamage);

                if (player.whoAmI == Main.myPlayer)
                {
                    for (int i = 0; i < 200; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (npc.active && !npc.friendly && !npc.dontTakeDamage && !(npc.damage == 0 && npc.lifeMax == 5)) //critters
                        {
                            if (Vector2.Distance(player.Center, npc.Center) <= distance)
                            {
                                int dmgRate = 60;

                                if (npc.FindBuffIndex(buff) == -1)
                                {
                                    npc.AddBuff(buff, 120);
                                }

                                if (Vector2.Distance(player.Center, npc.Center) <= 50)
                                {
                                    dmgRate /= 10;
                                }
                                else if (Vector2.Distance(player.Center, npc.Center) <= 100)
                                {
                                    dmgRate /= 5;
                                }
                                else if (Vector2.Distance(player.Center, npc.Center) <= 150)
                                {
                                    dmgRate /= 2;
                                }

                                if (player.infernoCounter % dmgRate == 0)
                                {
                                    player.ApplyDamageToNPC(npc, damage, 0f, 0, false);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void NebulaEffect()
        {
            if (!player.GetToggleValue("Nebula", false)) return;

            Nebula = true;
        }

        public void NecroEffect(bool hideVisual)
        {
            NecroEnchant = true;

            if (NecroCD != 0)
                NecroCD--;

            
        }

        public void NinjaEffect(bool hideVisual)
        {
            if (player.controlUseItem && player.HeldItem.type == ItemID.RodofDiscord)
            {
                player.AddBuff(ModContent.BuffType<FirstStrike>(), 60);
            }

            NinjaEnchant = true;
        }

        public void ObsidianEffect()
        {
            player.buffImmune[BuffID.OnFire] = true;
            player.fireWalk = true;

            player.lavaImmune = true;

            //that new acc effect e

            //in lava effects
            if (player.lavaWet)
            {
                player.gravity = Player.defaultGravity;
                player.ignoreWater = true;
                player.accFlipper = true;

                if (player.GetToggleValue("Obsidian"))
                {
                    player.AddBuff(ModContent.BuffType<ObsidianLavaWetBuff>(), 600);
                }
                
            }

            ObsidianEnchant = (TerraForce || WizardEnchant) || player.lavaWet || LavaWet;
        }

        public void OrichalcumEffect()
        {
            OriEnchant = true;

            if (!player.GetToggleValue("Orichalcum"))
                return;

            player.onHitPetal = true;

            /*int ballAmt = 6;

            if (Eternity)
                ballAmt = 30;

            if (!OriSpawn && player.ownedProjectileCounts[ModContent.ProjectileType<OriFireball>()] < ballAmt)
            {
                if (player.whoAmI == Main.myPlayer)
                {
                    for (int i = 0; i < ballAmt; i++)
                    {
                        float degree = (360 / ballAmt) * i;
                        Projectile fireball = FargoGlobalProjectile.NewProjectileDirectSafe(player.Center, Vector2.Zero, ModContent.ProjectileType<OriFireball>(), HighestDamageTypeScaling(25), 0f, player.whoAmI, 5, degree);
                    }
                }

                OriSpawn = true;
            }*/
        }

        public void PalladiumEffect()
        {
            //no lifesteal needed here for SoE
            if (Eternity) return;

            if (player.GetToggleValue("Palladium"))
            {
                if (EarthForce || TerrariaSoul)
                    player.onHitRegen = true;
                PalladEnchant = true;

                /*if (palladiumCD > 0)
                    palladiumCD--;*/
            }
        }

        public void PumpkinEffect(bool hideVisual)
        {
            PumpkinEnchant = true;

            if (player.GetToggleValue("Pumpkin") && (player.controlLeft || player.controlRight) && !IsStandingStill && player.whoAmI == Main.myPlayer)
            {
                if (pumpkinCD <= 0 && player.ownedProjectileCounts[ModContent.ProjectileType<GrowingPumpkin>()] < 10)
                {
                    int x = (int)player.Center.X / 16;
                    int y = (int)(player.position.Y + player.height - 1f) / 16;

                    if (Main.tile[x, y] == null)
                    {
                        Main.tile[x, y] = new Tile();
                    }

                    if (!Main.tile[x, y].active() && Main.tile[x, y].liquid == 0 && Main.tile[x, y + 1] != null && (WorldGen.SolidTile(x, y + 1) || Main.tile[x, y + 1].type == TileID.Platforms))
                    {
                        Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<GrowingPumpkin>(), 0,  0, player.whoAmI);
                        pumpkinCD = 300;
                    }
                }  
            }

            if (pumpkinCD > 0)
            {
                pumpkinCD--;
            }

            
        }

        public void RainEffect()
        {
            player.buffImmune[BuffID.Wet] = true;
            RainEnchant = true;

            AddMinion(player.GetToggleValue("Rain"), ModContent.ProjectileType<RainCloud>(), 36, 0);
        }

        public void RedRidingEffect(bool hideVisual)
        {
            RedEnchant = true;
            player.setHuntressT3 = true;
            
        }

        public void ShadowEffect(bool hideVisual)
        {
            ShadowEnchant = true;

            if (player.whoAmI == Main.myPlayer && player.GetToggleValue("Shadow"))
            {
                int currentOrbs = player.ownedProjectileCounts[ModContent.ProjectileType<ShadowEnchantOrb>()];

                int max = 2;

                if (TerrariaSoul)
                {
                    max = 5;
                }
                else if (ShadowForce || WizardEnchant)
                {
                    max = 4;
                }
                else if (AncientShadowEnchant)
                {
                    max = 3;
                }

                //spawn for first time
                if (currentOrbs == 0)
                {
                    float rotation = 2f * (float)Math.PI / max;

                    for (int i = 0; i < max; i++)
                    {
                        Vector2 spawnPos = player.Center + new Vector2(60, 0f).RotatedBy(rotation * i);
                        int p = Projectile.NewProjectile(spawnPos, Vector2.Zero, ModContent.ProjectileType<ShadowEnchantOrb>(), 0, 10f, player.whoAmI, 0, rotation * i);
                        Main.projectile[p].GetGlobalProjectile<FargoGlobalProjectile>().CanSplit = false;
                    }
                }
                //equipped somwthing that allows for more or less, respawn
                else if (currentOrbs != max)
                {
                    for (int i = 0; i < 1000; i++)
                    {
                        Projectile proj = Main.projectile[i];

                        if (proj.active && proj.type == ModContent.ProjectileType<ShadowEnchantOrb>() && proj.owner == player.whoAmI)
                        {
                            proj.Kill();
                        }
                    }

                    float rotation = 2f * (float)Math.PI / max;

                    for (int i = 0; i < max; i++)
                    {
                        Vector2 spawnPos = player.Center + new Vector2(60, 0f).RotatedBy(rotation * i);
                        int p = Projectile.NewProjectile(spawnPos, Vector2.Zero, ModContent.ProjectileType<ShadowEnchantOrb>(), 0, 10f, player.whoAmI, 0, rotation * i);
                        Main.projectile[p].GetGlobalProjectile<FargoGlobalProjectile>().CanSplit = false;
                    }
                }
            }




            
        }

        private int shinobiCD = 0;

        public void ShinobiEffect(bool hideVisual)
        {
            player.setMonkT3 = true;

            if (player.GetToggleValue("ShinobiDash") && player.whoAmI == Main.myPlayer & shinobiCD <= 0 && !player.mount.Active)
            {
                if ((player.controlRight && player.releaseRight))
                {
                    if (player.doubleTapCardinalTimer[2] > 0 && player.doubleTapCardinalTimer[2] != 15)
                    {
                        ShinobiDash(1);
                    }
                }

                if ((player.controlLeft && player.releaseLeft))
                {
                    if (player.doubleTapCardinalTimer[3] > 0 && player.doubleTapCardinalTimer[3] != 15)
                    {
                        ShinobiDash(-1);
                    }
                }
            }

            if (shinobiCD > 0)
            {
                shinobiCD--;
            }

            //tele through wall until open space on dash into wall
            /*if (player.GetToggleValue("Shinobi") && player.whoAmI == Main.myPlayer && player.dashDelay == -1 && player.mount.Type == -1 && player.velocity.X == 0)
            {
                var teleportPos = new Vector2();
                int direction = player.direction;

                teleportPos.X = player.position.X + direction;
                teleportPos.Y = player.position.Y;

                while (Collision.SolidCollision(teleportPos, player.width, player.height))
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
                    player.Teleport(teleportPos, 1);
                    NetMessage.SendData(MessageID.Teleport, -1, -1, null, 0, player.whoAmI, teleportPos.X, teleportPos.Y, 1);
                }
            }*/

            ShinobiEnchant = true;
            
        }

        private void ShinobiDash(int direction)
        {
            shinobiCD = 90;

            var teleportPos = player.position;

            const int length = 400; //make sure this is divisible by 16 btw

            if (player.GetToggleValue("Shinobi"))
            {
                teleportPos.X += length * direction;

                while (Collision.SolidCollision(teleportPos, player.width, player.height))
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
                    teleportPos.X += 16 * direction;
                    if (Collision.SolidCollision(teleportPos, player.width, player.height))
                    {
                        teleportPos.X -= 16 * direction;
                        break;
                    }
                }

            }

            

            if (teleportPos.X > 50 && teleportPos.X < (double)(Main.maxTilesX * 16 - 50) && teleportPos.Y > 50 && teleportPos.Y < (double)(Main.maxTilesY * 16 - 50))
            {
                GrossVanillaDodgeDust();
                player.Teleport(teleportPos, 1);
                GrossVanillaDodgeDust();
                NetMessage.SendData(MessageID.Teleport, -1, -1, null, 0, player.whoAmI, teleportPos.X, teleportPos.Y, 1);

                player.velocity.X = 12f * direction;
            }
        }

        private void GrossVanillaDodgeDust()
        {
            for (int index1 = 0; index1 < 50; ++index1)
            {
                int index2 = Dust.NewDust(player.position, player.width, player.height, 31, 0.0f, 0.0f, 100, new Color(), 2f);
                Main.dust[index2].position.X += Main.rand.Next(-20, 21);
                Main.dust[index2].position.Y += Main.rand.Next(-20, 21);
                Dust dust = Main.dust[index2];
                dust.velocity *= 0.4f;
                Main.dust[index2].scale *= 1f + Main.rand.Next(40) * 0.01f;
                if (Main.rand.Next(2) == 0)
                {
                    Main.dust[index2].scale *= 1f + Main.rand.Next(40) * 0.01f;
                    Main.dust[index2].noGravity = true;
                }
            }

            int index3 = Gore.NewGore(new Vector2(player.Center.X - 24, player.Center.Y - 24), new Vector2(), Main.rand.Next(61, 64), 1f);
            Main.gore[index3].scale = 1.5f;
            Main.gore[index3].velocity.X = Main.rand.Next(-50, 51) * 0.01f;
            Main.gore[index3].velocity.Y = Main.rand.Next(-50, 51) * 0.01f;
            Main.gore[index3].velocity *= 0.4f;

            int index4 = Gore.NewGore(new Vector2(player.Center.X - 24, player.Center.Y - 24), new Vector2(), Main.rand.Next(61, 64), 1f);
            Main.gore[index4].scale = 1.5f;
            Main.gore[index4].velocity.X = 1.5f + Main.rand.Next(-50, 51) * 0.01f;
            Main.gore[index4].velocity.Y = 1.5f + Main.rand.Next(-50, 51) * 0.01f;
            Main.gore[index4].velocity *= 0.4f;

            int index5 = Gore.NewGore(new Vector2(player.Center.X - 24, player.Center.Y - 24), new Vector2(), Main.rand.Next(61, 64), 1f);
            Main.gore[index5].scale = 1.5f;
            Main.gore[index5].velocity.X = -1.5f - Main.rand.Next(-50, 51) * 0.01f;
            Main.gore[index5].velocity.Y = 1.5f + Main.rand.Next(-50, 51) * 0.01f;
            Main.gore[index5].velocity *= 0.4f;

            int index6 = Gore.NewGore(new Vector2(player.Center.X - 24, player.Center.Y - 24), new Vector2(), Main.rand.Next(61, 64), 1f);
            Main.gore[index6].scale = 1.5f;
            Main.gore[index6].velocity.X = 1.5f - Main.rand.Next(-50, 51) * 0.01f;
            Main.gore[index6].velocity.Y = -1.5f + Main.rand.Next(-50, 51) * 0.01f;
            Main.gore[index6].velocity *= 0.4f;

            int index7 = Gore.NewGore(new Vector2(player.Center.X - 24, player.Center.Y - 24), new Vector2(), Main.rand.Next(61, 64), 1f);
            Main.gore[index7].scale = 1.5f;
            Main.gore[index7].velocity.X = -1.5f - Main.rand.Next(-50, 51) * 0.01f;
            Main.gore[index7].velocity.Y = -1.5f + Main.rand.Next(-50, 51) * 0.01f;
            Main.gore[index7].velocity *= 0.4f;
        }

        public void ShroomiteEffect(bool hideVisual)
        {
            if (!TerrariaSoul && player.GetToggleValue("Shroomite"))
                player.shroomiteStealth = true;

            ShroomEnchant = true;
            
        }

        public void SolarEffect()
        {
            if (!player.GetToggleValue("Solar"))
                return;

            Solar = true;
        }

        public void SpectreEffect(bool hideVisual)
        {
            SpectreEnchant = true;
            
        }

        public void SpectreHeal(NPC npc, Projectile proj)
        {
            if (npc.canGhostHeal && !player.moonLeech)
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
                Projectile.NewProjectile(proj.position.X, proj.position.Y, 0f, 0f, ProjectileID.SpiritHeal, 0, 0f, proj.owner, num4, num2);
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
            Projectile.NewProjectile(proj.position.X, proj.position.Y, num8, num9, ProjectileID.SpectreWrath, num, 0f, proj.owner, (float)num6, 0f);
        }

        public void SpiderEffect(bool hideVisual)
        {
            //minion crits
            SpiderEnchant = true;

            /*if (!TinEnchant)
            {
                SummonCrit = 20;
            }*/

            
        }

        public void SpookyEffect(bool hideVisual)
        {
            //scythe doom
            SpookyEnchant = true;
            
        }

        public void StardustEffect()
        {
            StardustEnchant = true;
            AddPet(player.GetToggleValue("Stardust"), false, BuffID.StardustGuardianMinion, ProjectileID.StardustGuardian);
            player.setStardust = true;

            if (FreezeTime && freezeLength > 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    if (!Filters.Scene["FargowiltasSouls:Invert"].IsActive())
                        Filters.Scene.Activate("FargowiltasSouls:Invert");

                    if (Filters.Scene["FargowiltasSouls:Invert"].IsActive())
                        Filters.Scene["FargowiltasSouls:Invert"].GetShader().UseTargetPosition(player.Center);
                }

                if (EModeGlobalNPC.BossIsAlive(ref EModeGlobalNPC.mutantBoss, ModContent.NPCType<MutantBoss>()))
                {
                    player.AddBuff(ModContent.BuffType<TimeFrozen>(), freezeLength);

                    if (Main.netMode != NetmodeID.Server && Filters.Scene["FargowiltasSouls:Invert"].IsActive())
                        Filters.Scene["FargowiltasSouls:Invert"].GetShader().UseTargetPosition(Main.npc[EModeGlobalNPC.mutantBoss].Center);
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
                    if (p.active && !(p.minion && !ProjectileID.Sets.MinionShot[p.type]) && !p.GetGlobalProjectile<FargoGlobalProjectile>().TimeFreezeImmune && p.GetGlobalProjectile<FargoGlobalProjectile>().TimeFrozen == 0)
                    {
                        p.GetGlobalProjectile<FargoGlobalProjectile>().TimeFrozen = freezeLength;

                        /*if (p.owner == player.whoAmI && p.friendly && !p.hostile)
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

                if (!Main.dedServ)
                {
                    if (freezeLength > 90)
                    {
                        if (Main.musicVolume > 0.001f)
                        {
                            Fargowiltas.OldVolume = Main.musicVolume;
                            Main.musicVolume = 0.001f;
                        }
                    }
                    else
                    {
                        if (Fargowiltas.OldVolume > Main.musicVolume)
                        {
                            Main.musicVolume = Fargowiltas.OldVolume * Math.Max(0.001f, 1f - freezeLength / 90f);
                            if (freezeLength == 0)
                            {
                                Main.musicVolume = Fargowiltas.OldVolume;
                                Fargowiltas.OldVolume = 0;
                            }
                        }
                    }
                }

                if (freezeLength == 90)
                {
                    if (!Main.dedServ)
                        Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/ZaWarudoResume").WithVolume(1f).WithPitchVariance(.5f), player.Center);
                }

                if (freezeLength <= 0)
                {
                    FreezeTime = false;
                    freezeLength = 540;
                  
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
            TikiEnchant = true;
            
        }

        private int getNumSentries()
        {
            int count = 0;

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];

                if (p.active && p.owner == player.whoAmI && p.sentry)
                {
                    count++;
                }
            }

            return count;
        }

        public void TinEffect()
        {
            if (!player.GetToggleValue("Tin", false)) return;
            
            TinEnchant = true;
        }

        public void TitaniumEffect()
        {
            if (player.GetToggleValue("Titanium"))
            {
                player.onHitDodge = true;
            }
        }

        public void TurtleEffect(bool hideVisual)
        {
            player.turtleThorns = true;
            player.thorns = 1f;

            TurtleEnchant = true;
            

            if (player.GetToggleValue("Turtle") && !player.HasBuff(ModContent.BuffType<BrokenShell>()) && IsStandingStill && !player.controlUseItem && player.whoAmI == Main.myPlayer)
            {
                TurtleCounter++;

                if (TurtleCounter > 20)
                {
                    player.AddBuff(ModContent.BuffType<ShellHide>(), 2);
                }
            }
            else
            {
                TurtleCounter = 0;
            }

            if (TurtleShellHP < 25 && !player.HasBuff(ModContent.BuffType<BrokenShell>()) && !ShellHide && (LifeForce || WizardEnchant))
            {
                turtleRecoverCD--;
                if (turtleRecoverCD == 0)
                {
                    turtleRecoverCD = 240;

                    TurtleShellHP++;
                }
            }
        }

        public void ValhallaEffect(bool hideVisual)
        {
            player.setSquireT2 = true;
            if (!player.GetToggleValue("SquirePanic"))
                player.buffImmune[BuffID.BallistaPanic] = true;
            player.setSquireT3 = true;
            //immune frames
            ValhallaEnchant = true;
            
        }

        public void VortexEffect(bool hideVisual)
        {
            //portal spawn
            VortexEnchant = true;
            //stealth memes
            if (player.whoAmI == Main.myPlayer && player.GetToggleValue("VortexS") && (player.controlDown && player.releaseDown))
            {
                if (player.doubleTapCardinalTimer[0] > 0 && player.doubleTapCardinalTimer[0] != 15)
                {
                    VortexStealth = !VortexStealth;
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        NetMessage.SendData(MessageID.SyncPlayer, number: player.whoAmI);

                    if (player.GetToggleValue("VortexV") && !player.HasBuff(ModContent.BuffType<VortexCD>()) && VortexStealth)
                    {
                        int p = Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, 0f, ModContent.ProjectileType<Projectiles.Void>(),  HighestDamageTypeScaling(60), 5f, player.whoAmI);
                        Main.projectile[p].GetGlobalProjectile<FargoGlobalProjectile>().CanSplit = false;
                        Main.projectile[p].netUpdate = true;

                        player.AddBuff(ModContent.BuffType<VortexCD>(), 3600);
                    }
                }
            }

            if (player.mount.Active)
                VortexStealth = false;

            if (VortexStealth)
            {
                player.moveSpeed *= 0.3f;
                player.aggro -= 1200;
                player.setVortex = true;
                player.stealth = 0f;


            }

            
        }

        public void EbonEffect()
        {
            if (!player.GetToggleValue("Ebon") || player.whoAmI != Main.myPlayer)
                return;

            int dist = 250;

            if (WoodForce || WizardEnchant)
            {
                dist = 350;
            }

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && npc.lifeMax > 5 && npc.Distance(player.Center) < dist)
                {
                    npc.AddBuff(BuffID.ShadowFlame, 15);

                    if (WoodForce || WizardEnchant)
                    {
                        npc.AddBuff(BuffID.CursedInferno, 15);
                    }
                }
                    
            }

            for (int i = 0; i < 20; i++)
            {
                Vector2 offset = new Vector2();
                double angle = Main.rand.NextDouble() * 2d * Math.PI;
                offset.X += (float)(Math.Sin(angle) * dist);
                offset.Y += (float)(Math.Cos(angle) * dist);
                if (!Collision.SolidCollision(player.Center + offset - new Vector2(4, 4), 0, 0))
                {
                    Dust dust = Main.dust[Dust.NewDust(
                      player.Center + offset - new Vector2(4, 4), 0, 0,
                      DustID.Shadowflame, 0, 0, 100, Color.White, 1f
                      )];
                    dust.velocity = player.velocity;
                    if (Main.rand.Next(3) == 0)
                        dust.velocity += Vector2.Normalize(offset) * -5f;
                    dust.noGravity = true;
                }
            }
        }

        public void ShadewoodEffect()
        {
            if (!player.GetToggleValue("Shade") || player.whoAmI != Main.myPlayer)
                return;

            int dist = 200;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && npc.lifeMax > 1 && npc.Distance(player.Center) < dist)
                    npc.AddBuff(ModContent.BuffType<SuperBleed>(), 2);

                npc.netUpdate = true;
            }

            for (int i = 0; i < 20; i++)
            {
                Vector2 offset = new Vector2();
                double angle = Main.rand.NextDouble() * 2d * Math.PI;
                offset.X += (float)(Math.Sin(angle) * dist);
                offset.Y += (float)(Math.Cos(angle) * dist);
                Dust dust = Main.dust[Dust.NewDust(
                    player.Center + offset - new Vector2(4, 4), 0, 0,
                    DustID.Blood, 0, 0, 100, Color.White, 1f
                    )];
                dust.velocity = player.velocity;
                if (Main.rand.Next(3) == 0)
                    dust.velocity += Vector2.Normalize(offset) * -5f;
                dust.noGravity = true;
            }

            if (shadewoodCD > 0)
            {
                shadewoodCD--;
            }
        }

        public void PalmEffect()
        {
            PalmEnchant = true;

            if (player.GetToggleValue("Palm") && (player.controlDown && player.releaseDown) && player.whoAmI == Main.myPlayer)
            {
                if (player.doubleTapCardinalTimer[0] > 0 && player.doubleTapCardinalTimer[0] != 15)
                {
                    Vector2 mouse = Main.MouseWorld;

                    if (player.ownedProjectileCounts[ModContent.ProjectileType<PalmTreeSentry>()] > 0)
                    {
                        for (int i = 0; i < 1000; i++)
                        {
                            Projectile proj = Main.projectile[i];

                            if (proj.type == ModContent.ProjectileType<PalmTreeSentry>() && proj.owner == player.whoAmI)
                            {
                                proj.Kill();
                            }
                        }
                    }

                    Projectile.NewProjectile(mouse.X, mouse.Y - 10, 0f, 0f, ModContent.ProjectileType<PalmTreeSentry>(), (WoodForce || WizardEnchant) ? 45 : 15, 0f, player.whoAmI);
                }
            }
        }

        private int apprenticePrevItem = -1;
        private bool apprenticeSwitchReady = false;
        private bool apprenticeBonusDamage = false;

        public void ApprenticeEffect()
        {
            player.setApprenticeT2 = true;

            //shadow shoot meme
            if (player.GetToggleValue("Apprentice") && player.controlUseItem)
            {
                Item heldItem = player.HeldItem;

                //must hold for so long then switch bonus is active
                if (heldItem.type == apprenticePrevItem)
                {
                    apprenticeCD++;

                    if (apprenticeCD > 120)
                    {
                        apprenticeSwitchReady = true;

                        //dust
                        int dustId = Dust.NewDust(new Vector2(player.position.X, player.position.Y + 2f), player.width, player.height + 5, DustID.FlameBurst, 0, 0, 100, Color.Black, 2f);
                        Main.dust[dustId].noGravity = true;
                    }
                }
                else if (apprenticeSwitchReady && heldItem.type != apprenticePrevItem)
                {
                    apprenticeBonusDamage = true; 
                }

                apprenticePrevItem = heldItem.type;

                //if (apprenticeCD == 0 && heldItem.damage > 0 && player.controlUseItem && player.itemAnimation != 0 && prevPosition != null && heldItem.type != ItemID.ExplosiveBunny && heldItem.type != ItemID.Cannonball
                //&& heldItem.createTile == -1 && heldItem.createWall == -1 && heldItem.ammo == AmmoID.None)
                //{
                //    if (prevPosition != null)
                //    {
                //        Vector2 vel = (Main.MouseWorld - prevPosition).SafeNormalize(-Vector2.UnitY) * 15;

                //        Projectile.NewProjectile(prevPosition, vel, ProjectileID.DD2FlameBurstTowerT3Shot, HighestDamageTypeScaling(heldItem.damage / 2), 1, player.whoAmI);

                //        for (int i = 0; i < 5; i++)
                //        {
                //            int dustId = Dust.NewDust(new Vector2(prevPosition.X, prevPosition.Y + 2f), player.width, player.height + 5, DustID.Shadowflame, 0, 0, 100, Color.Black, 2f);
                //            Main.dust[dustId].noGravity = true;
                //        }
                //    }

                //    prevPosition = player.position;
                //    apprenticeCD = 20;
                //}

                //if (apprenticeCD > 0)
                //{
                //    apprenticeCD--;
                //}
            }
        }

        public void HuntressEffect()
        {
            player.setHuntressT2 = true;

            if (player.GetToggleValue("Huntress") && player.whoAmI == Main.myPlayer)
            {
                huntressCD++;

                Item firstAmmo = PickAmmo();
                int arrowType = firstAmmo.shoot;
                int damage = HighestDamageTypeScaling((int)(firstAmmo.damage * 2.5f));

                if (RedEnchant)
                {
                    damage *= 2;
                }

                //fire arrow at nearby enemy
                if (huntressCD >= 30)
                {
                    Vector2 mouse = Main.MouseWorld;
                    Vector2 pos = new Vector2(mouse.X - player.direction * 100, mouse.Y - 800);
                    Vector2 velocity = Vector2.Normalize(mouse - pos) * 25;

                    int p = Projectile.NewProjectile(pos, velocity, arrowType, damage, 2, player.whoAmI);
                    Main.projectile[p].noDropItem = true;
                    Main.projectile[p].extraUpdates = 2;

                    huntressCD = 0;
                }

                //arrow rain ability
                if (!player.HasBuff(ModContent.BuffType<HuntressCD>()) && (player.controlDown && player.releaseDown))
                {
                    if (player.doubleTapCardinalTimer[0] > 0 && player.doubleTapCardinalTimer[0] != 15)
                    {
                        Vector2 mouse = Main.MouseWorld;

                        int heatray = Projectile.NewProjectile(player.Center, new Vector2(0, -6f), ProjectileID.HeatRay, 0, 0, Main.myPlayer);
                        Main.projectile[heatray].tileCollide = false;
                        //proj spawns arrows all around it until it dies
                        Projectile.NewProjectile(mouse.X, player.Center.Y - 500, 0f, 0f, ModContent.ProjectileType<ArrowRain>(),  HighestDamageTypeScaling(firstAmmo.damage), 0f, player.whoAmI, arrowType, player.direction);

                        player.AddBuff(ModContent.BuffType<HuntressCD>(), RedEnchant ? 600 : 900);
                    }
                }
            }
        }

        public Item PickAmmo()
        {
            Item item = new Item();
            bool flag = false;
            for (int i = 54; i < 58; i++)
            {
                if (player.inventory[i].ammo == AmmoID.Arrow && player.inventory[i].stack > 0)
                {
                    item = player.inventory[i];
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                for (int j = 0; j < 54; j++)
                {
                    if (player.inventory[j].ammo == AmmoID.Arrow && player.inventory[j].stack > 0)
                    {
                        item = player.inventory[j];
                        break;
                    }
                }
            }

            if (item.ammo != AmmoID.Arrow)
            {
                item.SetDefaults(ItemID.WoodenArrow);
            }

            return item;
        }

        public void MonkEffect()
        {
            player.setMonkT2 = true;
            MonkEnchant = true;

            if (player.GetToggleValue("Monk") && !player.HasBuff(ModContent.BuffType<MonkBuff>()))
            {
                monkTimer++;

                if (monkTimer >= 120)
                {
                    player.AddBuff(ModContent.BuffType<MonkBuff>(), 2);
                    monkTimer = 0;

                    //dust
                    double spread = 2 * Math.PI / 36;
                    for (int i = 0; i < 36; i++)
                    {
                        Vector2 velocity = new Vector2(2, 2).RotatedBy(spread * i);

                        int index2 = Dust.NewDust(player.Center, 0, 0, DustID.GoldCoin, velocity.X, velocity.Y, 100);
                        Main.dust[index2].noGravity = true;
                        Main.dust[index2].noLight = true;
                    }
                }
            }
        }

        public void SnowEffect(bool hideVisual)
        {
            SnowEnchant = true;

            if (player.GetToggleValue("Snow"))
            {
                SnowVisual = true;

                if (player.ownedProjectileCounts[ModContent.ProjectileType<Snowstorm>()] < 1)
                {
                    Vector2 mouse = Main.MouseWorld;
                    Projectile.NewProjectile(mouse, Vector2.Zero, ModContent.ProjectileType<Snowstorm>(), 0, 0, player.whoAmI);
                }
                
                //int dist = 200;

                //if (FrostEnchant)
                //{
                //    dist = 300;
                //}

                ////dust
                //for (int i = 0; i < 3; i++)
                //{
                //    Vector2 offset = new Vector2();
                //    double angle = Main.rand.NextDouble() * 2d * Math.PI;
                //    offset.X += (float)(Math.Sin(angle) * Main.rand.Next(dist + 1));
                //    offset.Y += (float)(Math.Cos(angle) * Main.rand.Next(dist + 1));
                //    Dust dust = Main.dust[Dust.NewDust(
                //        player.Center + offset - new Vector2(4, 4), 0, 0,
                //        76, 0, 0, 100, Color.White, .75f)];

                //    dust.noGravity = true;
                //}

                //for (int i = 0; i < 1000; i++)
                //{
                //    Projectile proj = Main.projectile[i];

                //    if (proj.active && proj.hostile && proj.damage > 0 && Vector2.Distance(proj.Center, player.Center) < dist)
                //    {
                //        proj.GetGlobalProjectile<FargoGlobalProjectile>().ChilledProj = true;
                //        proj.GetGlobalProjectile<FargoGlobalProjectile>().ChilledTimer = 30;
                //    }
                //}
            }

            
        }

        public void AncientShadowEffect()
        {
            //darkness
            AncientShadowEnchant = true;

            
        }

        #endregion

        #region souls
        public void ColossusSoul(int maxHP, float damageResist, int lifeRegen, bool hideVisual)
        {
            player.statLifeMax2 += maxHP;
            player.endurance += damageResist;
            player.lifeRegen += lifeRegen;

            //hand warmer, pocket mirror, ankh shield
            player.buffImmune[BuffID.Chilled] = true;
            player.buffImmune[BuffID.Frozen] = true;
            player.buffImmune[BuffID.Stoned] = true;
            player.buffImmune[BuffID.Weak] = true;
            player.buffImmune[BuffID.BrokenArmor] = true;
            player.buffImmune[BuffID.Bleeding] = true;
            player.buffImmune[BuffID.Poisoned] = true;
            player.buffImmune[BuffID.Slow] = true;
            player.buffImmune[BuffID.Confused] = true;
            player.buffImmune[BuffID.Silenced] = true;
            player.buffImmune[BuffID.Cursed] = true;
            player.buffImmune[BuffID.Darkness] = true;
            player.noKnockback = true;
            player.fireWalk = true;
            player.noFallDmg = true;
            //brain of confusion
            player.brainOfConfusion = true;
            //charm of myths
            player.pStone = true;
            //bee cloak, star veil
            if (player.GetToggleValue("DefenseStar"))
            {
                player.starCloak = true;
            }
            if (player.GetToggleValue("DefenseBee"))
            {
                player.bee = true;
            }
            player.longInvince = true;
            //shiny stone
            player.shinyStone = true;
            //flesh knuckles
            if (player.GetToggleValue("DefenseFleshKnuckle", false))
            {
                player.aggro += 400;
            }
            
            //frozen turtle shell
            if (player.statLife <= player.statLifeMax2 * 0.5) player.AddBuff(BuffID.IceBarrier, 5, true);
            //paladins shield
            if (player.statLife > player.statLifeMax2 * .25)
            {
                player.hasPaladinShield = true;
                for (int k = 0; k < 255; k++)
                {
                    Player target = Main.player[k];

                    if (target.active && player != target && Vector2.Distance(target.Center, player.Center) < 400) target.AddBuff(BuffID.PaladinsShield, 30);
                }
            }
        }

        private bool extraCarpetDuration = true;

        public void SupersonicSoul(bool hideVisual)
        {
            if (player.GetToggleValue("Supersonic") && !player.GetModPlayer<FargoPlayer>().noSupersonic && !EModeGlobalNPC.AnyBossAlive())
            {
                // 5 is the default value, I removed the config for it because the new toggler doesn't have sliders
                player.runAcceleration += 5f * .1f;
                player.maxRunSpeed += 5f * 2;
                //frog legs
                player.autoJump = true;
                player.jumpSpeedBoost += 2.4f;
                player.maxFallSpeed += 5f;
                player.jumpBoost = true;
            }
            else
            {
                //6.75 same as frostspark
                player.accRunSpeed = player.GetToggleValue("RunSpeed", false) ? 18.25f : 6.75f;
            }

            if (player.GetToggleValue("Momentum"))
            {
                player.runSlowdown = 2;
            }

            player.moveSpeed += 0.5f;

            if (player.GetToggleValue("SupersonicRocketBoots", false))
            {
                player.rocketBoots = 3;
                player.rocketTimeMax = 10;
            }
            
            player.iceSkate = true;
            //lava waders
            player.waterWalk = true;
            player.fireWalk = true;
            player.lavaImmune = true;
            player.noFallDmg = true;
            //bundle
            if (player.GetToggleValue("SupersonicJumps") && player.wingTime == 0)
            {
                player.doubleJumpCloud = true;
                player.doubleJumpSandstorm = true;
                player.doubleJumpBlizzard = true;
                player.doubleJumpFart = true;
            }
            //magic carpet
            if (player.whoAmI == Main.myPlayer && player.GetToggleValue("SupersonicCarpet"))
            {
                player.carpet = true;
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    NetMessage.SendData(MessageID.SyncPlayer, number: player.whoAmI);

                if (player.canCarpet)
                {
                    extraCarpetDuration = true;
                }
                else if (extraCarpetDuration)
                {
                    extraCarpetDuration = false;
                    player.carpetTime = 1000;
                }
            }
            //EoC Shield
            if (player.GetToggleValue("CthulhuShield"))
            {
                player.dash = 2;
            }
            //ninja gear
            if (player.GetToggleValue("BlackBelt"))
                player.blackBelt = true;
            if (player.GetToggleValue("SupersonicClimbing"))
                player.spikedBoots = 2;
            if (player.GetToggleValue("SupersonicTabi", false))
                player.dash = 1;

            //sweetheart necklace
            if (player.GetToggleValue("DefenseBee"))
            {
                player.bee = true;
            }
            if (player.GetToggleValue("DefensePanic"))
            {
                player.panic = true;
            }

            FlowerBoots();
        }

        public void FlightMasterySoul()
        {
            player.wingTimeMax = 999999;
            player.wingTime = player.wingTimeMax;
            player.ignoreWater = true;

            //hover
            if (player.controlDown && player.controlJump && !player.mount.Active)
            {
                player.position.Y -= player.velocity.Y;
                if (player.velocity.Y > 0.1f)
                    player.velocity.Y = 0.1f;
                else if (player.velocity.Y < -0.1f)
                    player.velocity.Y = -0.1f;
            }

            //grav
            if (player.GetToggleValue("MasoGrav"))
                player.gravControl = true;
        }

        public void TrawlerSoul(bool hideVisual)
        {
            //instacatch
            FishSoul1 = true;
            //extra lures
            if (player.GetToggleValue("Trawler"))
            {
                FishSoul2 = true;
            }

            

            

            //tackle bag
            player.fishingSkill += 60;
            player.sonarPotion = true;
            player.cratePotion = true;
            player.accFishingLine = true;
            player.accTackleBox = true;
            player.accFishFinder = true;

            //spore sac
            if (player.whoAmI == Main.myPlayer && player.GetToggleValue("TrawlerSpore"))
            {
                player.sporeSac = true;
                player.SporeSac();
            }

            //arctic diving gear
            player.arcticDivingGear = true;
            player.accFlipper = true;
            player.accDivingHelm = true;
            player.iceSkate = true;
            if (player.wet)
            {
                Lighting.AddLight((int)player.Center.X / 16, (int)player.Center.Y / 16, 0.2f, 0.8f, 0.9f);
            }

            //sharkron balloon
            if (player.GetToggleValue("TrawlerJump") && player.wingTime == 0)
                player.doubleJumpSail = true;

            player.jumpBoost = true;
            player.noFallDmg = true;
        }

        public void WorldShaperSoul(bool hideVisual)
        {
            //mining speed, spelunker, dangersense, light, hunter, pet
            MinerEffect(hideVisual, .66f);
            //placing speed up
            player.tileSpeed += 0.5f;
            player.wallSpeed += 0.5f;
            //toolbox
            Player.tileRangeX += 10;
            Player.tileRangeY += 10;
            //gizmo pack
            player.autoPaint = true;
            //presserator
            player.autoActuator = true;
            //royal gel
            player.npcTypeNoAggro[1] = true;
            player.npcTypeNoAggro[16] = true;
            player.npcTypeNoAggro[59] = true;
            player.npcTypeNoAggro[71] = true;
            player.npcTypeNoAggro[81] = true;
            player.npcTypeNoAggro[138] = true;
            player.npcTypeNoAggro[121] = true;
            player.npcTypeNoAggro[122] = true;
            player.npcTypeNoAggro[141] = true;
            player.npcTypeNoAggro[147] = true;
            player.npcTypeNoAggro[183] = true;
            player.npcTypeNoAggro[184] = true;
            player.npcTypeNoAggro[204] = true;
            player.npcTypeNoAggro[225] = true;
            player.npcTypeNoAggro[244] = true;
            player.npcTypeNoAggro[302] = true;
            player.npcTypeNoAggro[333] = true;
            player.npcTypeNoAggro[335] = true;
            player.npcTypeNoAggro[334] = true;
            player.npcTypeNoAggro[336] = true;
            player.npcTypeNoAggro[537] = true;

            if (player.whoAmI == Main.myPlayer && player.GetToggleValue("Builder"))
            {
                BuilderMode = true;
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    NetMessage.SendData(MessageID.SyncPlayer, number: player.whoAmI);

                for (int i = 0; i < TileLoader.TileCount; i++)
                {
                    player.adjTile[i] = true;
                }

                //placing speed up
                player.tileSpeed += 0.5f;
                player.wallSpeed += 0.5f;
                //toolbox
                Player.tileRangeX += 50;
                Player.tileRangeY += 50;
            }

            //cell phone
            player.accWatch = 3;
            player.accDepthMeter = 1;
            player.accCompass = 1;
            player.accFishFinder = true;
            player.accDreamCatcher = true;
            player.accOreFinder = true;
            player.accStopwatch = true;
            player.accCritterGuide = true;
            player.accJarOfSouls = true;
            player.accThirdEye = true;
            player.accCalendar = true;
            player.accWeatherRadio = true;
        }


        #endregion

        #region maso acc

        #endregion
    }
}
