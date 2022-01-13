using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.Projectiles.Champions;

namespace FargowiltasSouls.NPCs.Champions
{
    [AutoloadBossHead]
    public class TerraChampion : ModNPC
    {
        private bool spawned;

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Champion of Terra");
            //DisplayName.AddTranslation(GameCulture.Chinese, "泰拉英灵");

            NPCID.Sets.TrailCacheLength[npc.type] = 5;
            NPCID.Sets.TrailingMode[npc.type] = 1;
        }

        public override void SetDefaults()
        {
            npc.width = 80;
            npc.height = 80;
            npc.damage = 140;
            npc.defense = 80;
            npc.lifeMax = 170000;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.knockBackResist = 0f;
            npc.lavaImmune = true;
            npc.aiStyle = -1;
            npc.value = Item.buyPrice(0, 15);

            npc.boss = true;

            for (int i = 0; i < npc.buffImmune.Length; i++)
                npc.buffImmune[i] = true;

            Mod musicMod = ModLoader.GetMod("FargowiltasMusic");
            music = musicMod != null ? ModLoader.GetMod("FargowiltasMusic").GetSoundSlot(SoundType.Music, "Sounds/Music/Champions") : MusicID.Boss1;
            musicPriority = MusicPriority.BossHigh;

            npc.behindTiles = true;
            npc.trapImmune = true;

            npc.scale *= 1.5f;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            //npc.damage = (int)(npc.damage * 0.5f);
            npc.lifeMax = (int)(npc.lifeMax * Math.Sqrt(bossLifeScale));
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return npc.Distance(FargoSoulsUtil.ClosestPointInHitbox(target, npc.Center)) < 30 * npc.scale;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(npc.localAI[0]);
            writer.Write(npc.localAI[1]);
            writer.Write(npc.localAI[2]);
            writer.Write(npc.localAI[3]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc.localAI[0] = reader.ReadSingle();
            npc.localAI[1] = reader.ReadSingle();
            npc.localAI[2] = reader.ReadSingle();
            npc.localAI[3] = reader.ReadSingle();
        }

        public override void AI()
        {
            npc.ai[3] = 0;

            if (!spawned) //just spawned
            {
                spawned = true;
                npc.TargetClosest(false);

                for (int i = 0; i < NPCID.Sets.TrailCacheLength[npc.type]; i++)
                    npc.oldPos[i] = npc.position;

                if (Main.netMode != NetmodeID.MultiplayerClient) //spawn segments
                {
                    int prev = npc.whoAmI;
                    const int max = 99;
                    for (int i = 0; i < max; i++)
                    {
                        int type = i == max - 1 ? ModContent.NPCType<TerraChampionTail>() : ModContent.NPCType<TerraChampionBody>();
                        int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, type, npc.whoAmI);
                        if (n != Main.maxNPCs)
                        {
                            Main.npc[n].ai[1] = prev;
                            Main.npc[n].ai[3] = npc.whoAmI;
                            Main.npc[n].realLife = npc.whoAmI;

                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);

                            prev = n;
                        }
                        else //can't spawn all segments
                        {
                            npc.active = false;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                            return;
                        }
                    }
                }
            }

            EModeGlobalNPC.championBoss = npc.whoAmI;

            Player player = Main.player[npc.target];
            Vector2 targetPos;

            if (npc.HasValidTarget && player.Center.Y >= Main.worldSurface * 16 && !player.ZoneUnderworldHeight)
                npc.timeLeft = 600;

            if (FargoSoulsWorld.EternityMode && npc.ai[1] != -1 && npc.life < npc.lifeMax / 10)
            {
                Main.PlaySound(SoundID.ForceRoar, player.Center, -1);
                npc.life = npc.lifeMax / 10;
                npc.velocity = Vector2.Zero;
                npc.ai[1] = -1f;
                npc.localAI[0] = 0;
                npc.localAI[1] = 0;
                npc.localAI[2] = 0;
                npc.localAI[3] = 0;
                npc.netUpdate = true;
            }

            switch ((int)npc.ai[1])
            {
                case -1: //flying head alone
                    if (!player.active || player.dead || player.Center.Y < Main.worldSurface * 16 || player.ZoneUnderworldHeight) //despawn code
                    {
                        npc.TargetClosest(false);
                        if (npc.timeLeft > 30)
                            npc.timeLeft = 30;
                        npc.velocity.Y += 1f;
                        break;
                    }

                    npc.scale = 3f;
                    targetPos = player.Center;
                    if (npc.Distance(targetPos) > 50)
                        Movement(targetPos, 0.16f, 32f);

                    npc.rotation = npc.DirectionTo(player.Center).ToRotation();

                    if (++npc.localAI[0] > 50)
                    {
                        npc.localAI[0] = 0;

                        if (npc.localAI[1] > 120) //dont shoot while orb is exploding
                        {
                            Main.PlaySound(SoundID.Item12, npc.Center);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Vector2 dir = npc.DirectionTo(player.Center);
                                float ai1New = (Main.rand.NextBool()) ? 1 : -1; //randomize starting direction
                                Vector2 vel = Vector2.Normalize(dir) * 22f;
                                Projectile.NewProjectile(npc.Center, vel, mod.ProjectileType("HostileLightning"),
                                    npc.damage / 4, 0, Main.myPlayer, dir.ToRotation(), ai1New);
                            }
                        }
                    }

                    if (--npc.localAI[1] < 0)
                    {
                        npc.localAI[1] = 420;

                        if (Main.netMode != NetmodeID.MultiplayerClient) //shoot orb
                        {
                            int p = Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<TerraLightningOrb2>(), npc.damage / 4, 0f, Main.myPlayer, npc.whoAmI);
                            Main.projectile[p].localAI[0] += 1f + Main.rand.NextFloatDirection(); //random starting rotation
                            Main.projectile[p].localAI[1] = (Main.rand.NextBool()) ? 1 : -1;
                            Main.projectile[p].netUpdate = true;
                        }
                    }
                    break;

                case 0: //ripped from destroyer
                    {
                        WormMovement(player, 17.22f, 0.122f, 0.188f);

                        if (++npc.localAI[0] > 420)
                        {
                            npc.ai[1]++;
                            npc.localAI[0] = 0;
                        }
                    }

                    npc.rotation = npc.velocity.ToRotation();
                    break;

                case 1: //flee and prepare
                    npc.ai[3] = 2;

                    if (++npc.localAI[0] < 90)
                    {
                        targetPos = player.Center + npc.DirectionFrom(player.Center) * 900;
                        Movement(targetPos, 0.4f, 18f);
                        if (npc.Distance(targetPos) < 100)
                            npc.localAI[0] = 90 - 1;
                    }
                    else if (npc.localAI[0] == 90)
                    {
                        foreach (NPC segment in Main.npc.Where(n => n.active && n.realLife == npc.whoAmI)) //mp sync
                        {
                            segment.netUpdate = true;
                        }

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.GlowRingHollow>(), 0, 0f, Main.myPlayer, 12f, npc.whoAmI);
                            Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.GlowRingHollow>(), 0, 0f, Main.myPlayer, 12f, npc.whoAmI);
                        }
                    }
                    else
                    {
                        float rotationDifference = MathHelper.WrapAngle(npc.velocity.ToRotation() - npc.DirectionTo(player.Center).ToRotation());
                        bool inFrontOfMe = Math.Abs(rotationDifference) < MathHelper.ToRadians(90 / 2);

                        bool proceed = npc.localAI[0] > 300 && (npc.localAI[0] > 360 || inFrontOfMe);

                        if (proceed)
                        {
                            npc.ai[1]++;
                            npc.localAI[0] = 0;

                            npc.velocity /= 4f;
                        }
                        else
                        {
                            npc.velocity = Vector2.Normalize(npc.velocity) * Math.Min(48f, npc.velocity.Length() + 1f);
                            npc.velocity += npc.velocity.RotatedBy(MathHelper.PiOver2) * npc.velocity.Length() / 300;
                        }
                    }

                    npc.rotation = npc.velocity.ToRotation();
                    break;

                case 2: //dash
                    {
                        if (npc.localAI[1] == 0)
                        {
                            Main.PlaySound(SoundID.Roar, player.Center, 0);
                            npc.localAI[1] = 1;
                            npc.velocity = npc.DirectionTo(player.Center) * 24;
                        }

                        if (++npc.localAI[2] > 2)
                        {
                            npc.localAI[2] = 0;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Vector2 vel = npc.DirectionTo(player.Center) * 12;
                                Projectile.NewProjectile(npc.Center, vel, ModContent.ProjectileType<TerraFireball>(), npc.damage / 4, 0f, Main.myPlayer);

                                float offset = npc.velocity.ToRotation() - vel.ToRotation();

                                vel = Vector2.Normalize(npc.velocity).RotatedBy(offset) * 12;
                                Projectile.NewProjectile(npc.Center, vel, ModContent.ProjectileType<TerraFireball>(), npc.damage / 4, 0f, Main.myPlayer);
                            }
                        }

                        double angle = npc.DirectionTo(player.Center).ToRotation() - npc.velocity.ToRotation();
                        while (angle > Math.PI)
                            angle -= 2.0 * Math.PI;
                        while (angle < -Math.PI)
                            angle += 2.0 * Math.PI;

                        if (++npc.localAI[0] > 240 || (Math.Abs(angle) > Math.PI / 2 && npc.Distance(player.Center) > 1200))
                        {
                            npc.velocity = Vector2.Normalize(npc.velocity).RotatedBy(Math.PI / 2) * 18f;
                            npc.ai[1]++;
                            npc.localAI[0] = 0;
                            npc.localAI[1] = 0;
                        }

                        npc.rotation = npc.velocity.ToRotation();
                    }
                    break;

                case 3:
                    goto case 0;

                case 4: //reposition for sine
                    /*if (npc.Distance(player.Center) < 1200)
                    {
                        targetPos = player.Center + npc.DirectionFrom(player.Center) * 1200;
                        Movement(targetPos, 0.6f, 36f);
                    }
                    else //circle at distance to pull segments away
                    {
                        npc.velocity = npc.DirectionTo(player.Center).RotatedBy(Math.PI / 2) * 36;
                    }

                    if (++npc.localAI[0] > 180)
                    {
                        npc.ai[1]++;
                        npc.localAI[0] = 0;
                    }

                    npc.rotation = npc.velocity.ToRotation();
                    break;*/
                    goto case 1;

                case 5: //sine wave dash
                    {
                        npc.ai[3] = 1;

                        if (npc.localAI[0] == 0)
                        {
                            npc.localAI[1] = npc.DirectionTo(player.Center).ToRotation();
                            Main.PlaySound(SoundID.Roar, player.Center, 0);
                        }

                        const int end = 360;

                        /*Vector2 offset;
                        offset.X = 10f * npc.localAI[0];
                        offset.Y = 600 * (float)Math.Sin(2f * Math.PI / end * 4 * npc.localAI[0]);

                        npc.Center = new Vector2(npc.localAI[2], npc.localAI[3]) + offset.RotatedBy(npc.localAI[1]);
                        npc.velocity = Vector2.Zero;
                        npc.rotation = (npc.position - npc.oldPosition).ToRotation();*/

                        float sinModifier = (float)Math.Sin(2 * (float)Math.PI * (npc.localAI[0] / end * 3 + 0.25f));
                        npc.rotation = npc.localAI[1] + (float)Math.PI / 2 * sinModifier;
                        npc.velocity = 36f * npc.rotation.ToRotationVector2();

                        if (Math.Abs(sinModifier) < 0.001f) //account for rounding issues
                        {
                            Main.PlaySound(SoundID.Item12, npc.Center);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                for (int j = -5; j <= 5; j++)
                                {
                                    float rotationOffset = (float)Math.PI / 2 + (float)Math.PI / 2 / 5 * j;
                                    rotationOffset *= Math.Sign(-sinModifier);
                                    Projectile.NewProjectile(npc.Center,
                                        6f * Vector2.UnitX.RotatedBy(npc.localAI[1] + rotationOffset),
                                        ProjectileID.CultistBossFireBall, npc.damage / 4, 0f, Main.myPlayer);
                                }

                                for (int i = -5; i <= 5; i++)
                                {
                                    float rotationOffset = (float)Math.PI / 2 + (float)Math.PI / 2 / 4.5f * i;
                                    rotationOffset *= Math.Sign(-sinModifier);
                                    Vector2 vel2 = Vector2.UnitX.RotatedBy(Math.PI / 4 * (Main.rand.NextDouble() - 0.5)) * 36f;
                                    float ai1New = (Main.rand.NextBool()) ? 1 : -1; //randomize starting direction
                                    Projectile.NewProjectile(npc.Center, vel2.RotatedBy(npc.localAI[1] + rotationOffset), mod.ProjectileType("HostileLightning"),
                                        npc.damage / 4, 0, Main.myPlayer, npc.localAI[1] + rotationOffset, ai1New);
                                }
                            }
                        }

                        if (++npc.localAI[0] > end * 0.8f)
                        {
                            npc.ai[1]++;
                            npc.localAI[0] = 0;
                            npc.localAI[1] = 0;
                            npc.localAI[2] = 0;
                            npc.localAI[3] = 0;
                            npc.velocity = npc.DirectionTo(player.Center) * npc.velocity.Length();
                        }
                    }
                    break;

                case 6:
                    goto case 0;

                case 7:
                    goto case 1;

                case 8: //dash but u-turn
                    if (npc.localAI[1] == 0)
                    {
                        Main.PlaySound(SoundID.Roar, player.Center, 0);
                        npc.localAI[1] = 1;
                        npc.velocity = npc.DirectionTo(player.Center) * 36;
                    }

                    if (npc.localAI[3] == 0)
                    {
                        double angle = npc.DirectionTo(player.Center).ToRotation() - npc.velocity.ToRotation();
                        while (angle > Math.PI)
                            angle -= 2.0 * Math.PI;
                        while (angle < -Math.PI)
                            angle += 2.0 * Math.PI;

                        if (Math.Abs(angle) > Math.PI / 2) //passed player, turn around
                        {
                            npc.localAI[3] = Math.Sign(angle);
                            npc.velocity = Vector2.Normalize(npc.velocity) * 24;
                        }
                    }
                    else //turning
                    {
                        npc.velocity = npc.velocity.RotatedBy(MathHelper.ToRadians(2.5f) * npc.localAI[3]);

                        if (++npc.localAI[2] > 2)
                        {
                            npc.localAI[2] = 0;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Vector2 vel = 12f * Vector2.Normalize(npc.velocity).RotatedBy(Math.PI / 2);
                                Projectile.NewProjectile(npc.Center, vel, ModContent.ProjectileType<TerraFireball>(), npc.damage / 4, 0f, Main.myPlayer);
                                Projectile.NewProjectile(npc.Center, -vel, ModContent.ProjectileType<TerraFireball>(), npc.damage / 4, 0f, Main.myPlayer);
                            }
                        }

                        if (++npc.localAI[0] > 75)
                        {
                            npc.ai[1]++;
                            npc.localAI[0] = 0;
                            npc.localAI[1] = 0;
                        }
                    }

                    npc.rotation = npc.velocity.ToRotation();
                    break;

                case 9:
                    goto case 0;

                case 10:
                    goto case 1;

                case 11: //prepare for coil
                    npc.ai[3] = 2;
                    targetPos = player.Center + npc.DirectionFrom(player.Center) * 600;
                    Movement(targetPos, 0.4f, 32f);
                    if (++npc.localAI[0] > 300 || npc.Distance(targetPos) < 50f)
                    {
                        npc.ai[1]++;
                        npc.localAI[0] = 0;
                        npc.localAI[1] = npc.Distance(player.Center);
                        npc.velocity = 32f * npc.DirectionTo(player.Center).RotatedBy(Math.PI / 2);
                        Main.PlaySound(SoundID.Roar, player.Center, 0);
                    }
                    npc.rotation = npc.velocity.ToRotation();
                    break;

                case 12: //coiling
                    {
                        npc.ai[3] = 2;

                        Vector2 acceleration = Vector2.Normalize(npc.velocity).RotatedBy(-Math.PI / 2) * 32f * 32f / 600f;
                        npc.velocity = Vector2.Normalize(npc.velocity) * 32f + acceleration;
                        
                        npc.rotation = npc.velocity.ToRotation();

                        Vector2 pivot = npc.Center;
                        pivot += Vector2.Normalize(npc.velocity.RotatedBy(-Math.PI / 2)) * 600;
                        
                        Player target = Main.player[npc.target];
                        if (target.active && !target.dead) //arena effect
                        {
                            float distance = target.Distance(pivot);
                            if (distance > 600 && distance < 3000)
                            {
                                Vector2 movement = pivot - target.Center;
                                float difference = movement.Length() - 600;
                                movement.Normalize();
                                movement *= difference < 34f ? difference : 34f;
                                target.position += movement;

                                for (int i = 0; i < 20; i++)
                                {
                                    int d = Dust.NewDust(target.position, target.width, target.height, 87, 0f, 0f, 0, default(Color), 2f);
                                    Main.dust[d].noGravity = true;
                                    Main.dust[d].velocity *= 5f;
                                }
                            }
                        }

                        if (npc.localAI[0] == 0 && Main.netMode != NetmodeID.MultiplayerClient) //shoot orb
                        {
                            Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<TerraLightningOrb2>(), npc.damage / 4, 0f, Main.myPlayer, npc.whoAmI);
                        }

                        if (++npc.localAI[0] > 420)
                        {
                            npc.ai[1]++;
                            npc.localAI[0] = 0;
                            npc.localAI[1] = 0;
                        }
                    }
                    break;

                case 13: //reset to get rid of troublesome coil
                    goto case 1;

                default:
                    npc.ai[1] = 0;
                    goto case 0;
            }

            npc.netUpdate = true;

            Vector2 dustOffset = new Vector2(77, -41) * npc.scale; //dust from horns
            int dust = Dust.NewDust(npc.Center + npc.velocity - dustOffset.RotatedBy(npc.rotation), 0, 0, DustID.Fire, npc.velocity.X * .4f, npc.velocity.Y * 0.4f, 0, default(Color), 2f);
            Main.dust[dust].velocity *= 2;
            if (Main.rand.NextBool())
            {
                Main.dust[dust].scale++;
                Main.dust[dust].noGravity = true;
            }

            dustOffset.Y *= -1f;
            dust = Dust.NewDust(npc.Center + npc.velocity - dustOffset.RotatedBy(npc.rotation), 0, 0, DustID.Fire, npc.velocity.X * .4f, npc.velocity.Y * 0.4f, 0, default(Color), 2f);
            Main.dust[dust].velocity *= 2;
            if (Main.rand.NextBool())
            {
                Main.dust[dust].scale++;
                Main.dust[dust].noGravity = true;
            }

            if (npc.ai[1] != -1 && Collision.SolidCollision(npc.position, npc.width, npc.height) && npc.soundDelay == 0)
            {
                npc.soundDelay = (int)(npc.Distance(player.Center) / 40f);
                if (npc.soundDelay < 10)
                    npc.soundDelay = 10;
                if (npc.soundDelay > 20)
                    npc.soundDelay = 20;
                Main.PlaySound(SoundID.Roar, npc.Center, 1);
            }

            int pastPos = NPCID.Sets.TrailCacheLength[npc.type] - (int)npc.ai[3] - 1; //ai3 check is to trace better and coil tightly
            Vector2 myPosAfterVelocity = npc.position + npc.velocity;
            if ((myPosAfterVelocity - npc.oldPos[pastPos - 1]).Length() > 45 * npc.scale / 1.5f * 1.25f)
            {
                npc.oldPos[pastPos - 1] = myPosAfterVelocity + Vector2.Normalize(npc.oldPos[pastPos - 1] - myPosAfterVelocity) * 45 * npc.scale / 1.5f * 1.25f;
            }
        }

        private void WormMovement(Player player, float maxSpeed, float turnSpeed, float accel)
        {
            if (!player.active || player.dead || player.Center.Y < Main.worldSurface * 16 || player.ZoneUnderworldHeight) //despawn code
            {
                npc.TargetClosest(false);
                if (npc.timeLeft > 30)
                    npc.timeLeft = 30;
                npc.velocity.Y += 1f;
                npc.rotation = npc.velocity.ToRotation();
                return;
            }

            float comparisonSpeed = player.velocity.Length() * 1.5f;
            float rotationDifference = MathHelper.WrapAngle(npc.velocity.ToRotation() - npc.DirectionTo(player.Center).ToRotation());
            bool inFrontOfMe = Math.Abs(rotationDifference) < MathHelper.ToRadians(90 / 2);
            if (maxSpeed < comparisonSpeed && inFrontOfMe) //player is moving faster than my top speed
            {
                maxSpeed = comparisonSpeed; //outspeed them
            }

            if (npc.Distance(player.Center) > 1200f) //better turning when out of range
            {
                turnSpeed *= 2f;
                accel *= 2f;

                if (inFrontOfMe && maxSpeed < 30f) //much higher top speed to return to the fight
                    maxSpeed = 30f;
            }

            if (npc.velocity.Length() > maxSpeed) //decelerate if over top speed
                npc.velocity *= 0.975f;

            Vector2 target = player.Center;
            float num17 = target.X;
            float num18 = target.Y;

            float num21 = num17 - npc.Center.X;
            float num22 = num18 - npc.Center.Y;
            float num23 = (float)Math.Sqrt((double)num21 * (double)num21 + (double)num22 * (double)num22);

            //ground movement code but it always runs
            float num2 = (float)Math.Sqrt(num21 * num21 + num22 * num22);
            float num3 = Math.Abs(num21);
            float num4 = Math.Abs(num22);
            float num5 = maxSpeed / num2;
            float num6 = num21 * num5;
            float num7 = num22 * num5;
            if ((npc.velocity.X > 0f && num6 > 0f || npc.velocity.X < 0f && num6 < 0f) && (npc.velocity.Y > 0f && num7 > 0f || npc.velocity.Y < 0f && num7 < 0f))
            {
                if (npc.velocity.X < num6)
                    npc.velocity.X += accel;
                else if (npc.velocity.X > num6)
                    npc.velocity.X -= accel;
                if (npc.velocity.Y < num7)
                    npc.velocity.Y += accel;
                else if (npc.velocity.Y > num7)
                    npc.velocity.Y -= accel;
            }
            if (npc.velocity.X > 0f && num6 > 0f || npc.velocity.X < 0f && num6 < 0f || npc.velocity.Y > 0f && num7 > 0f || npc.velocity.Y < 0f && num7 < 0f)
            {
                if (npc.velocity.X < num6)
                    npc.velocity.X += turnSpeed;
                else if (npc.velocity.X > num6)
                    npc.velocity.X -= turnSpeed;
                if (npc.velocity.Y < num7)
                    npc.velocity.Y += turnSpeed;
                else if (npc.velocity.Y > num7)
                    npc.velocity.Y -= turnSpeed;

                if (Math.Abs(num7) < maxSpeed * 0.2f && (npc.velocity.X > 0f && num6 < 0f || npc.velocity.X < 0f && num6 > 0f))
                {
                    if (npc.velocity.Y > 0f)
                        npc.velocity.Y += turnSpeed * 2f;
                    else
                        npc.velocity.Y -= turnSpeed * 2f;
                }
                if (Math.Abs(num6) < maxSpeed * 0.2f && (npc.velocity.Y > 0f && num7 < 0f || npc.velocity.Y < 0f && num7 > 0f))
                {
                    if (npc.velocity.X > 0f)
                        npc.velocity.X += turnSpeed * 2f;
                    else
                        npc.velocity.X -= turnSpeed * 2f;
                }
            }
            else if (num3 > num4)
            {
                if (npc.velocity.X < num6)
                    npc.velocity.X += turnSpeed * 1.1f;
                else if (npc.velocity.X > num6)
                    npc.velocity.X -= turnSpeed * 1.1f;

                if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < maxSpeed * 0.5f)
                {
                    if (npc.velocity.Y > 0f)
                        npc.velocity.Y += turnSpeed;
                    else
                        npc.velocity.Y -= turnSpeed;
                }
            }
            else
            {
                if (npc.velocity.Y < num7)
                    npc.velocity.Y += turnSpeed * 1.1f;
                else if (npc.velocity.Y > num7)
                    npc.velocity.Y -= turnSpeed * 1.1f;

                if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < maxSpeed * 0.5f)
                {
                    if (npc.velocity.X > 0f)
                        npc.velocity.X += turnSpeed;
                    else
                        npc.velocity.X -= turnSpeed;
                }
            }
        }

        private void Movement(Vector2 targetPos, float speedModifier, float cap = 12f, bool fastY = false)
        {
            if (npc.Center.X < targetPos.X)
            {
                npc.velocity.X += speedModifier;
                if (npc.velocity.X < 0)
                    npc.velocity.X += speedModifier * 2;
            }
            else
            {
                npc.velocity.X -= speedModifier;
                if (npc.velocity.X > 0)
                    npc.velocity.X -= speedModifier * 2;
            }
            if (npc.Center.Y < targetPos.Y)
            {
                npc.velocity.Y += fastY ? speedModifier * 2 : speedModifier;
                if (npc.velocity.Y < 0)
                    npc.velocity.Y += speedModifier * 2;
            }
            else
            {
                npc.velocity.Y -= fastY ? speedModifier * 2 : speedModifier;
                if (npc.velocity.Y > 0)
                    npc.velocity.Y -= speedModifier * 2;
            }
            if (Math.Abs(npc.velocity.X) > cap)
                npc.velocity.X = cap * Math.Sign(npc.velocity.X);
            if (Math.Abs(npc.velocity.Y) > cap)
                npc.velocity.Y = cap * Math.Sign(npc.velocity.Y);
        }

        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            //if (npc.ai[3] == 1) damage /= 10;
            if (npc.life < npc.lifeMax / 10) damage /= 3;
            return true;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 600);
            if (FargoSoulsWorld.EternityMode)
            {
                target.AddBuff(ModContent.BuffType<LivingWasteland>(), 600);
                target.AddBuff(ModContent.BuffType<LightningRod>(), 600);
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                for (int i = 1; i <= 3; i++)
                {
                    Vector2 pos = npc.position + new Vector2(Main.rand.NextFloat(npc.width), Main.rand.NextFloat(npc.height));
                    Gore.NewGore(pos, npc.velocity, mod.GetGoreSlot("Gores/TerraGore" + i.ToString()), npc.scale);
                }
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.SuperHealingPotion;
        }

        public override void NPCLoot()
        {
            FargoSoulsWorld.downedChampions[1] = true;
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData); //sync world

            FargoSoulsGlobalNPC.DropEnches(npc, ModContent.ItemType<Items.Accessories.Forces.TerraForce>());
        }

        public override void BossHeadRotation(ref float rotation)
        {
            rotation = npc.rotation;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.npcTexture[npc.type];
            //int turnSpeed6 = Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type]; //ypos of lower right corner of sprite to draw
            //int y3 = turnSpeed6 * npc.frame.Y; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = npc.frame;//new Rectangle(0, y3, texture2D13.Width, turnSpeed6);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = npc.GetAlpha(color26);

            SpriteEffects effects = npc.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.spriteBatch.Draw(texture2D13, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), npc.GetAlpha(lightColor), npc.rotation, origin2, npc.scale, effects, 0f);
            Texture2D glowmask = ModContent.GetTexture("FargowiltasSouls/NPCs/Champions/TerraChampion_Glow");
            Main.spriteBatch.Draw(glowmask, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, npc.rotation, origin2, npc.scale, effects, 0f);
            return false;
        }
    }
}