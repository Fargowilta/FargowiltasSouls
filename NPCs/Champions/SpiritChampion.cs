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
using FargowiltasSouls.Items.Accessories.Enchantments;
using FargowiltasSouls.Projectiles.Champions;

namespace FargowiltasSouls.NPCs.Champions
{
    [AutoloadBossHead]
    public class SpiritChampion : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Champion of Spirit");
            DisplayName.AddTranslation(GameCulture.Chinese, "魂灵英灵");
            Main.npcFrameCount[npc.type] = 2;
            NPCID.Sets.TrailCacheLength[npc.type] = 6;
            NPCID.Sets.TrailingMode[npc.type] = 1;
        }

        public override void SetDefaults()
        {
            npc.width = 150;
            npc.height = 150;
            npc.damage = 125;
            npc.defense = 40;
            npc.lifeMax = 550000;
            npc.HitSound = SoundID.NPCHit54;
            npc.DeathSound = SoundID.NPCDeath52;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.knockBackResist = 0f;
            npc.lavaImmune = true;
            npc.aiStyle = -1;
            npc.value = Item.buyPrice(0, 15);
            npc.boss = true;

            npc.buffImmune[BuffID.Chilled] = true;
            npc.buffImmune[BuffID.OnFire] = true;
            npc.buffImmune[BuffID.Suffocation] = true;
            npc.buffImmune[mod.BuffType("Lethargic")] = true;
            npc.buffImmune[mod.BuffType("ClippedWings")] = true;
            npc.GetGlobalNPC<FargoSoulsGlobalNPC>().SpecialEnchantImmune = true;

            Mod musicMod = ModLoader.GetMod("FargowiltasMusic");
            music = musicMod != null ? ModLoader.GetMod("FargowiltasMusic").GetSoundSlot(SoundType.Music, "Sounds/Music/Champions") : MusicID.Boss1;
            musicPriority = MusicPriority.BossHigh;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return true;
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
            if (npc.localAI[3] == 0) //spawn friends
            {
                npc.TargetClosest(false);
                Movement(Main.player[npc.target].Center, 0.8f, 32f);
                if (npc.Distance(Main.player[npc.target].Center) < 2000)
                    npc.localAI[3] = 1;
                else
                    return;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<SpiritChampionHand>(), npc.whoAmI, 0f, npc.whoAmI, -1f, -1f, npc.target);
                    if (n != Main.maxNPCs)
                    {
                        Main.npc[n].velocity.X = Main.rand.NextFloat(-24f, 24f);
                        Main.npc[n].velocity.Y = Main.rand.NextFloat(-24f, 24f);
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                    }
                    n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<SpiritChampionHand>(), npc.whoAmI, 0f, npc.whoAmI, -1f, 1f, npc.target);
                    if (n != Main.maxNPCs)
                    {
                        Main.npc[n].velocity.X = Main.rand.NextFloat(-24f, 24f);
                        Main.npc[n].velocity.Y = Main.rand.NextFloat(-24f, 24f);
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                    }
                    n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<SpiritChampionHand>(), npc.whoAmI, 0f, npc.whoAmI, 1f, -1f, npc.target);
                    if (n != Main.maxNPCs)
                    {
                        Main.npc[n].velocity.X = Main.rand.NextFloat(-24f, 24f);
                        Main.npc[n].velocity.Y = Main.rand.NextFloat(-24f, 24f);
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                    }
                    n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<SpiritChampionHand>(), npc.whoAmI, 0f, npc.whoAmI, 1f, 1f, npc.target);
                    if (n != Main.maxNPCs)
                    {
                        Main.npc[n].velocity.X = Main.rand.NextFloat(-24f, 24f);
                        Main.npc[n].velocity.Y = Main.rand.NextFloat(-24f, 24f);
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                    }
                }
            }

            EModeGlobalNPC.championBoss = npc.whoAmI;

            Player player = Main.player[npc.target];
            Vector2 targetPos;
            
            if (npc.HasValidTarget && npc.Distance(player.Center) < 2500 && Framing.GetTileSafely(player.Center).wall != WallID.None)
                npc.timeLeft = 600;
            
            switch ((int)npc.ai[0])
            {
                case -4: //final float
                    npc.dontTakeDamage = true;
                    goto case 0;

                case -3: //final you think you're safe
                    if (npc.localAI[2] == 0)
                        npc.localAI[2] = 1;

                    if (!player.active || player.dead || Vector2.Distance(npc.Center, player.Center) > 2500f
                        || Framing.GetTileSafely(player.Center).wall == WallID.None) //despawn code
                    {
                        npc.TargetClosest(false);
                        if (npc.timeLeft > 30)
                            npc.timeLeft = 30;

                        npc.noTileCollide = true;
                        npc.noGravity = true;
                        npc.velocity.Y += 1f;

                        return;
                    }

                    targetPos = new Vector2(npc.localAI[0], npc.localAI[1]);
                    if (npc.Distance(targetPos) > 25)
                        Movement(targetPos, 0.8f, 24f);

                    npc.dontTakeDamage = true;

                    if (npc.ai[1] == 0) //respawn dead hands
                    {
                        bool[] foundHand = new bool[4];

                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<SpiritChampionHand>() && Main.npc[i].ai[1] == npc.whoAmI)
                            {
                                if (!foundHand[0])
                                    foundHand[0] = Main.npc[i].ai[2] == -1f && Main.npc[i].ai[3] == -1f;
                                if (!foundHand[1])
                                    foundHand[1] = Main.npc[i].ai[2] == -1f && Main.npc[i].ai[3] == 1f;
                                if (!foundHand[2])
                                    foundHand[2] = Main.npc[i].ai[2] == 1f && Main.npc[i].ai[3] == -1f;
                                if (!foundHand[3])
                                    foundHand[3] = Main.npc[i].ai[2] == 1f && Main.npc[i].ai[3] == 1f;
                            }
                        }

                        if (Main.netMode != NetmodeID.MultiplayerClient) //if hands somehow disappear
                        {
                            if (!foundHand[0])
                            {
                                int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<SpiritChampionHand>(), npc.whoAmI, 0f, npc.whoAmI, -1f, -1f, npc.target);
                                if (n != Main.maxNPCs)
                                {
                                    Main.npc[n].velocity.X = Main.rand.NextFloat(-24f, 24f);
                                    Main.npc[n].velocity.Y = Main.rand.NextFloat(-24f, 24f);
                                    if (Main.netMode == NetmodeID.Server)
                                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                                }
                            }
                            if (!foundHand[1])
                            {
                                int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<SpiritChampionHand>(), npc.whoAmI, 0f, npc.whoAmI, -1f, 1f, npc.target);
                                if (n != Main.maxNPCs)
                                {
                                    Main.npc[n].velocity.X = Main.rand.NextFloat(-24f, 24f);
                                    Main.npc[n].velocity.Y = Main.rand.NextFloat(-24f, 24f);
                                    if (Main.netMode == NetmodeID.Server)
                                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                                }
                            }
                            if (!foundHand[2])
                            {
                                int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<SpiritChampionHand>(), npc.whoAmI, 0f, npc.whoAmI, 1f, -1f, npc.target);
                                if (n != Main.maxNPCs)
                                {
                                    Main.npc[n].velocity.X = Main.rand.NextFloat(-24f, 24f);
                                    Main.npc[n].velocity.Y = Main.rand.NextFloat(-24f, 24f);
                                    if (Main.netMode == NetmodeID.Server)
                                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                                }
                            }
                            if (!foundHand[3])
                            {
                                int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<SpiritChampionHand>(), npc.whoAmI, 0f, npc.whoAmI, 1f, 1f, npc.target);
                                if (n != Main.maxNPCs)
                                {
                                    Main.npc[n].velocity.X = Main.rand.NextFloat(-24f, 24f);
                                    Main.npc[n].velocity.Y = Main.rand.NextFloat(-24f, 24f);
                                    if (Main.netMode == NetmodeID.Server)
                                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                                }
                            }
                        }
                    }
                    else if (npc.ai[1] == 120)
                    {
                        Main.PlaySound(SoundID.Roar, npc.Center, 0);

                        for (int i = 0; i < Main.maxNPCs; i++) //update ai
                        {
                            if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<SpiritChampionHand>() && Main.npc[i].ai[1] == npc.whoAmI)
                            {
                                Main.npc[i].ai[0] = 1f;
                                Main.npc[i].netUpdate = true;
                            }
                        }

                        if (Main.netMode != NetmodeID.MultiplayerClient) //spawn super hand
                        {

                            int n2 = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<SpiritChampionHand>(), npc.whoAmI, 3f, npc.whoAmI, 1f, 1f, npc.target);
                            if (n2 != Main.maxNPCs)
                            {
                                Main.npc[n2].velocity.X = Main.rand.NextFloat(-24f, 24f);
                                Main.npc[n2].velocity.Y = Main.rand.NextFloat(-24f, 24f);
                                if (Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n2);
                            }
                        }
                    }

                    if (++npc.ai[2] > 85) //bone spray
                    {
                        npc.ai[2] = 0;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Main.PlaySound(SoundID.Item2, npc.Center);

                            for (int i = 0; i < 12; i++)
                            {
                                Projectile.NewProjectile(npc.position.X + Main.rand.Next(npc.width), npc.position.Y + Main.rand.Next(npc.height),
                                    Main.rand.NextFloat(-8f, 8f), Main.rand.NextFloat(-8f, 8f), ModContent.ProjectileType<SpiritCrossBone>(), npc.damage / 4, 0f, Main.myPlayer);
                            }
                        }
                    }

                    if (++npc.ai[3] > 110)
                    {
                        npc.ai[3] = 0;
                        if (Main.netMode != NetmodeID.MultiplayerClient) //sandnado
                        {
                            Vector2 target = player.Center;
                            target.Y -= 100;
                            Projectile.NewProjectile(target, Vector2.Zero, ProjectileID.SandnadoHostileMark, 0, 0f, Main.myPlayer);

                            int length = (int)npc.Distance(target) / 10;
                            Vector2 offset = npc.DirectionTo(target) * 10f;
                            for (int i = 0; i < length; i++) //dust warning line for sandnado
                            {
                                int d = Dust.NewDust(npc.Center + offset * i, 0, 0, 269, 0f, 0f, 0, new Color());
                                Main.dust[d].noLight = true;
                                Main.dust[d].scale = 1.25f;
                            }
                        }
                    }

                    if (++npc.ai[1] > 600)
                    {
                        npc.dontTakeDamage = false;
                        npc.netUpdate = true;
                        npc.ai[0] = 0;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;
                        npc.localAI[3] = 2; //can die now
                    }
                    break;

                case -1:
                    targetPos = new Vector2(npc.localAI[0], npc.localAI[1]);
                    if (npc.Distance(targetPos) > 25)
                        Movement(targetPos, 0.8f, 24f);

                    if (++npc.ai[1] > 360)
                    {
                        npc.TargetClosest();
                        npc.ai[0] = 4;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;
                        npc.netUpdate = true;

                        if (npc.Hitbox.Intersects(player.Hitbox))
                        {
                            player.velocity.X = player.Center.X < npc.Center.X ? -15f : 15f;
                            player.velocity.Y = -10f;
                            Main.PlaySound(SoundID.Roar, npc.Center, 0);
                        }
                    }
                    break;

                case 0: //float to player
                    if (!player.active || player.dead || Vector2.Distance(npc.Center, player.Center) > 2500f
                        || Framing.GetTileSafely(player.Center).wall == WallID.None) //despawn code
                    {
                        npc.TargetClosest(false);
                        if (npc.timeLeft > 30)
                            npc.timeLeft = 30;

                        npc.noTileCollide = true;
                        npc.noGravity = true;
                        npc.velocity.Y += 1f;

                        return;
                    }

                    if (npc.ai[1] == 0)
                    {
                        targetPos = player.Center;
                        npc.velocity = (targetPos - npc.Center) / 75;

                        npc.localAI[0] = targetPos.X;
                        npc.localAI[1] = targetPos.Y;
                    }

                    if (++npc.ai[1] > 75)
                    {
                        npc.TargetClosest();
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;
                        npc.netUpdate = true;
                    }
                    break;

                case 1: //cross bone/sandnado
                    if (npc.localAI[2] == 0)
                        npc.localAI[2] = 1;

                    targetPos = new Vector2(npc.localAI[0], npc.localAI[1]);
                    if (npc.Distance(targetPos) > 25)
                        Movement(targetPos, 0.8f, 24f);

                    if (++npc.ai[2] > 45)
                    {
                        npc.ai[2] = 0;
                        
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            if (npc.ai[1] < 180) //cross bones
                            {
                                Main.PlaySound(SoundID.Item2, npc.Center);

                                for (int i = 0; i < 12; i++)
                                {
                                    Projectile.NewProjectile(npc.position.X + Main.rand.Next(npc.width), npc.position.Y + Main.rand.Next(npc.height),
                                        Main.rand.NextFloat(-8f, 8f), Main.rand.NextFloat(-8f, 8f), ModContent.ProjectileType<SpiritCrossBone>(), npc.damage / 4, 0f, Main.myPlayer);
                                }
                            }
                            else //sandnado
                            {
                                npc.GetGlobalNPC<EModeGlobalNPC>().masoBool[0] = !npc.GetGlobalNPC<EModeGlobalNPC>().masoBool[0];

                                Vector2 target = player.Center;
                                if (npc.GetGlobalNPC<EModeGlobalNPC>().masoBool[0] && npc.life < npc.lifeMax * 0.66)
                                    target += player.velocity * 30f; //alternate between predictive and direct aim
                                target.Y -= 100;
                                Projectile.NewProjectile(target, Vector2.Zero, ProjectileID.SandnadoHostileMark, 0, 0f, Main.myPlayer);

                                int length = (int)npc.Distance(target) / 10;
                                Vector2 offset = npc.DirectionTo(target) * 10f;
                                for (int i = 0; i < length; i++) //dust warning line for sandnado
                                {
                                    int d = Dust.NewDust(npc.Center + offset * i, 0, 0, 269, 0f, 0f, 0, new Color());
                                    Main.dust[d].noLight = true;
                                    Main.dust[d].scale = 1.25f;
                                }
                            }
                        }
                    }
                    
                    if (++npc.ai[1] > 400)
                    {
                        npc.TargetClosest();
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;
                        npc.netUpdate = true;
                    }
                    break;

                case 2:
                    goto case 0;

                case 3: //grab
                    targetPos = new Vector2(npc.localAI[0], npc.localAI[1]);
                    if (npc.Distance(targetPos) > 25)
                        Movement(targetPos, 0.8f, 24f);

                    if (++npc.ai[2] == 30)
                    {
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<SpiritChampionHand>() && Main.npc[i].ai[1] == npc.whoAmI)
                            {
                                Main.npc[i].ai[0] = 1f;
                                Main.npc[i].netUpdate = true;
                            }
                        }
                    }

                    if (npc.life < npc.lifeMax * 0.66)
                    {
                        if (++npc.ai[3] > 55) //homing spectre bolts
                        {
                            npc.ai[3] = 0;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                const int max = 5;
                                for (int i = 0; i < max; i++)
                                {
                                    Vector2 speed = Main.rand.NextFloat(1, 2) * Vector2.UnitX.RotatedByRandom(Math.PI * 2);
                                    float ai1 = 60 + Main.rand.Next(30);
                                    Projectile.NewProjectile(npc.Center, speed, ModContent.ProjectileType<SpiritSpirit>(),
                                        npc.damage / 4, 0f, Main.myPlayer, npc.target, ai1);
                                }
                            }
                        }
                    }

                    if (++npc.ai[1] > 360)
                    {
                        npc.TargetClosest();
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;
                        npc.netUpdate = true;
                    }
                    break;

                case 4:
                    goto case 0;

                case 5: //swords
                    targetPos = new Vector2(npc.localAI[0], npc.localAI[1]);
                    if (npc.Distance(targetPos) > 25)
                        Movement(targetPos, 0.8f, 24f);

                    if (++npc.ai[2] > 80)
                    {
                        npc.ai[2] = 0;

                        Main.PlaySound(SoundID.Item92, npc.Center);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            for (int i = 0; i < 15; i++) //sword burst
                            {
                                float speed = Main.rand.NextFloat(4f, 8f);
                                Vector2 velocity = speed * Vector2.UnitX.RotatedBy(Main.rand.NextDouble() * 2 * Math.PI);
                                float ai1 = speed / Main.rand.NextFloat(60f, 120f);
                                Projectile.NewProjectile(npc.Center, velocity, ModContent.ProjectileType<SpiritSword>(), npc.damage / 4, 0f, Main.myPlayer, 0f, ai1);
                            }

                            if (npc.life < npc.lifeMax * 0.66)
                            {
                                const int max = 12; //hand ring
                                for (int i = 0; i < max; i++)
                                {
                                    Vector2 vel = npc.DirectionTo(player.Center).RotatedBy(Math.PI * 2 / max * i);
                                    float ai0 = 1.045f;
                                    Projectile.NewProjectile(npc.Center, vel, ModContent.ProjectileType<SpiritHand>(), npc.damage / 4, 0f, Main.myPlayer, ai0);
                                }
                            }
                        }
                    }

                    if (++npc.ai[1] > 300)
                    {
                        npc.TargetClosest();
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;
                        npc.netUpdate = true;
                    }
                    break;

                case 6:
                    goto case 0;

                case 7: //skip this number, staying on even number allows hands to remain drawn close
                    npc.ai[0]++;
                    break;

                case 8: //shadow hands, reflect, mummy spirits
                    {
                        targetPos = new Vector2(npc.localAI[0], npc.localAI[1]);
                        if (npc.Distance(targetPos) > 25)
                            Movement(targetPos, 0.8f, 24f);

                        const float distance = 150;

                        for (int i = 0; i < 20; i++)
                        {
                            Vector2 offset = new Vector2();
                            double angle = Main.rand.NextDouble() * 2d * Math.PI;
                            offset.X += (float)(Math.Sin(angle) * distance);
                            offset.Y += (float)(Math.Cos(angle) * distance);
                            Dust dust = Main.dust[Dust.NewDust(
                                npc.Center + offset - new Vector2(4, 4), 0, 0,
                                87, 0, 0, 100, Color.White, 1f
                                )];
                            dust.velocity = npc.velocity;
                            //if (Main.rand.Next(3) == 0) dust.velocity += Vector2.Normalize(offset) * -5f;
                            dust.noGravity = true;
                        }

                        Main.projectile.Where(x => x.active && x.friendly && !x.minion).ToList().ForEach(x => //reflect projectiles
                        {
                            if (Vector2.Distance(x.Center, npc.Center) <= distance)
                            {
                                for (int i = 0; i < 5; i++)
                                {
                                    int dustId = Dust.NewDust(x.position, x.width, x.height, 87,
                                        x.velocity.X * 0.2f, x.velocity.Y * 0.2f, 100, default(Color), 1.5f);
                                    Main.dust[dustId].noGravity = true;
                                }

                            // Set ownership
                            x.hostile = true;
                                x.friendly = false;
                                x.owner = Main.myPlayer;
                            x.damage /= 4;

                            // Turn around
                            x.velocity *= -1f;

                            // Flip sprite
                            if (x.Center.X > npc.Center.X * 0.5f)
                                {
                                    x.direction = 1;
                                    x.spriteDirection = 1;
                                }
                                else
                                {
                                    x.direction = -1;
                                    x.spriteDirection = -1;
                                }

                            //x.netUpdate = true;
                        }
                        });

                        if (npc.ai[1] == 0)
                        {
                            Main.PlaySound(SoundID.Roar, npc.Center, 0);
                        }

                        if (++npc.ai[3] > 10) //spirits
                        {
                            npc.ai[3] = 0;

                            Main.PlaySound(SoundID.Item8, npc.Center);

                            if (Main.netMode != NetmodeID.MultiplayerClient) //vanilla code from desert spirits idfk
                            {
                                Point tileCoordinates1 = npc.Center.ToTileCoordinates();
                                Point tileCoordinates2 = Main.player[npc.target].Center.ToTileCoordinates();
                                Vector2 vector2 = Main.player[npc.target].Center - npc.Center;
                                int num1 = 6;
                                int num2 = 6;
                                int num3 = 0;
                                int num4 = 2;
                                int num5 = 0;
                                bool flag1 = false;
                                if (vector2.Length() > 2000)
                                    flag1 = true;
                                while (!flag1 && num5 < 50)
                                {
                                    ++num5;
                                    int index1 = Main.rand.Next(tileCoordinates2.X - num1, tileCoordinates2.X + num1 + 1);
                                    int index2 = Main.rand.Next(tileCoordinates2.Y - num1, tileCoordinates2.Y + num1 + 1);
                                    if ((index2 < tileCoordinates2.Y - num3 || index2 > tileCoordinates2.Y + num3 || (index1 < tileCoordinates2.X - num3 || index1 > tileCoordinates2.X + num3)) && (index2 < tileCoordinates1.Y - num2 || index2 > tileCoordinates1.Y + num2 || (index1 < tileCoordinates1.X - num2 || index1 > tileCoordinates1.X + num2)) && !Main.tile[index1, index2].nactive())
                                    {
                                        bool flag2 = true;
                                        if (flag2 && Main.tile[index1, index2].lava())
                                            flag2 = false;
                                        if (flag2 && Collision.SolidTiles(index1 - num4, index1 + num4, index2 - num4, index2 + num4))
                                            flag2 = false;
                                        if (flag2)
                                        {
                                            Projectile.NewProjectile(index1 * 16 + 8, index2 * 16 + 8, 0, 0f,
                                                ProjectileID.DesertDjinnCurse, 0, 1f, Main.myPlayer, npc.target, 0f);
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (++npc.ai[2] > 70) //hands
                        {
                            npc.ai[2] = 0;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                for (int i = 0; i < 4; i++)
                                {
                                    Vector2 vel = npc.DirectionTo(player.Center).RotatedBy(Math.PI / 6 * (Main.rand.NextDouble() - 0.5));
                                    float ai0 = Main.rand.NextFloat(1.04f, 1.06f);
                                    float ai1 = Main.rand.NextFloat(0.025f);
                                    Projectile.NewProjectile(npc.Center, vel, ModContent.ProjectileType<SpiritHand>(), npc.damage / 4, 0f, Main.myPlayer, ai0, ai1);
                                }

                                Main.PlaySound(SoundID.Item2, npc.Center);

                                if (npc.life < npc.lifeMax * 0.66) //do vertical only crossbones in p2
                                {
                                    for (int i = 0; i < 6; i++)
                                    {
                                        Projectile.NewProjectile(npc.position.X + Main.rand.Next(npc.width), npc.position.Y + Main.rand.Next(npc.height),
                                            Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-8f, 0f), ModContent.ProjectileType<SpiritCrossBone>(), npc.damage / 4, 0f, Main.myPlayer);
                                    }
                                }
                            }
                        }

                        if (++npc.ai[1] > 360)
                        {
                            npc.TargetClosest();
                            npc.ai[0]++;
                            npc.ai[1] = 0;
                            npc.ai[2] = 0;
                            npc.ai[3] = 0;
                            npc.netUpdate = true;
                        }
                    }
                    break;

                case 9: //skip this number, get back to usual behaviour
                    npc.ai[0]++;
                    break;

                /*case 10:
                    goto case 0;*/

                default:
                    npc.ai[0] = 0;
                    goto case 0;
            }

            if (npc.localAI[2] != 0 && FargoSoulsWorld.MasochistMode) //aura
            {
                const float auraDistance = 1200;
                float range = npc.Distance(player.Center);
                if (range > auraDistance && range < 3000)
                {
                    if (++npc.localAI[2] > 60)
                    {
                        npc.localAI[2] = 1;
                        npc.netUpdate = true;
                        
                        Main.PlaySound(SoundID.Roar, npc.Center, 0);

                        if (Main.netMode != NetmodeID.MultiplayerClient) //spawn super hand
                        {

                            int n2 = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<SpiritChampionHand>(), npc.whoAmI, 4f, npc.whoAmI, 1f, 1f, npc.target);
                            if (n2 != Main.maxNPCs)
                            {
                                Main.npc[n2].velocity.X = Main.rand.NextFloat(-24f, 24f);
                                Main.npc[n2].velocity.Y = Main.rand.NextFloat(-24f, 24f);
                                if (Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n2);
                            }
                        }
                    }
                }
                
                for (int i = 0; i < 20; i++) //dust
                {
                    int d = Dust.NewDust(npc.Center + auraDistance * Vector2.UnitX.RotatedBy(Math.PI * 2 * Main.rand.NextDouble()), 0, 0, 87);
                    Main.dust[d].velocity = npc.velocity;
                    Main.dust[d].noGravity = true;
                    Main.dust[d].scale++;
                }
            }
        }

        public override bool CheckDead()
        {
            if (npc.localAI[3] != 2f && FargoSoulsWorld.MasochistMode)
            {
                npc.active = true;
                npc.life = 1;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.TargetClosest(false);
                    npc.ai[0] = -4f;
                    npc.ai[1] = 0;
                    npc.ai[2] = 0;
                    npc.ai[3] = 0;
                    npc.dontTakeDamage = true;
                    npc.netUpdate = true;
                }

                return false;
            }
            return true;
        }

        private void Movement(Vector2 targetPos, float speedModifier, float cap = 12f)
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
                npc.velocity.Y += speedModifier;
                if (npc.velocity.Y < 0)
                    npc.velocity.Y += speedModifier * 2;
            }
            else
            {
                npc.velocity.Y -= speedModifier;
                if (npc.velocity.Y > 0)
                    npc.velocity.Y -= speedModifier * 2;
            }
            if (Math.Abs(npc.velocity.X) > cap)
                npc.velocity.X = cap * Math.Sign(npc.velocity.X);
            if (Math.Abs(npc.velocity.Y) > cap)
                npc.velocity.Y = cap * Math.Sign(npc.velocity.Y);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.MasochistMode)
            {
                target.AddBuff(ModContent.BuffType<Infested>(), 360);
                target.AddBuff(ModContent.BuffType<ClippedWings>(), 180);
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.SuperHealingPotion;
        }

        public override void FindFrame(int frameHeight)
        {
            switch ((int)npc.ai[0])
            {
                case -4: //eyes closed
                case 0:
                case 2:
                case 4:
                case 6:
                case 8:
                    npc.frame.Y = frameHeight;
                    break;

                default: //eyes open
                    npc.frame.Y = 0;
                    break;
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0 && npc.localAI[3] == 2)
            {
                for (int i = 1; i <= 5; i++)
                {
                    Vector2 pos = npc.position + new Vector2(Main.rand.NextFloat(npc.width), Main.rand.NextFloat(npc.height));
                    Gore.NewGore(pos, npc.velocity, mod.GetGoreSlot("Gores/SpiritGore" + i.ToString()), npc.scale);
                }
            }
        }

        public override void NPCLoot()
        {
            FargoSoulsWorld.downedChampions[6] = true;
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData); //sync world

            //Item.NewItem(npc.position, npc.Size, ModContent.ItemType<SpiritForce>());
            int[] drops = {
                ModContent.ItemType<FossilEnchant>(),
                ModContent.ItemType<ForbiddenEnchant>(),
                ModContent.ItemType<HallowEnchant>(),
                ModContent.ItemType<TikiEnchant>(),
                ModContent.ItemType<SpectreEnchant>(),
            };
            int lastDrop = -1; //don't drop same ench twice
            for (int i = 0; i < 2; i++)
            {
                int thisDrop = Main.rand.Next(drops.Length);

                if (lastDrop == thisDrop) //try again
                {
                    if (++thisDrop >= drops.Length) //drop first ench in line if looped past array
                        thisDrop = 0;
                }

                lastDrop = thisDrop;
                Item.NewItem(npc.position, npc.Size, drops[thisDrop]);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.npcTexture[npc.type];
            //int num156 = Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type]; //ypos of lower right corner of sprite to draw
            //int y3 = num156 * npc.frame.Y; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = npc.frame;//new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = npc.GetAlpha(color26);

            SpriteEffects effects = npc.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < NPCID.Sets.TrailCacheLength[npc.type]; i++)
            {
                Color color27 = color26 * 0.5f;
                color27 *= (float)(NPCID.Sets.TrailCacheLength[npc.type] - i) / NPCID.Sets.TrailCacheLength[npc.type];
                Vector2 value4 = npc.oldPos[i];
                float num165 = npc.rotation; //npc.oldRot[i];
                Main.spriteBatch.Draw(texture2D13, value4 + npc.Size / 2f - Main.screenPosition + new Vector2(0, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, npc.scale, effects, 0f);
            }

            Main.spriteBatch.Draw(texture2D13, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), npc.GetAlpha(lightColor), npc.rotation, origin2, npc.scale, effects, 0f);
            return false;
        }
    }
}
