using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Items.Accessories.Enchantments;
using FargowiltasSouls.Projectiles.Masomode;
using FargowiltasSouls.Projectiles.Champions;

namespace FargowiltasSouls.NPCs.Champions
{
    [AutoloadBossHead]
    public class ShadowChampion : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Champion of Shadow");
            DisplayName.AddTranslation(GameCulture.Chinese, "暗影英灵");
            Main.npcFrameCount[npc.type] = 5;
            NPCID.Sets.TrailCacheLength[npc.type] = 6;
            NPCID.Sets.TrailingMode[npc.type] = 1;
        }

        public override void SetDefaults()
        {
            npc.width = 110;
            npc.height = 110;
            npc.damage = 130;
            npc.defense = 60;
            npc.lifeMax = 330000;
            npc.HitSound = SoundID.NPCHit5;
            npc.DeathSound = SoundID.NPCDeath7;
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

            npc.dontTakeDamage = true;
        }

        /*public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return true;
        }*/

        /*public override void SendExtraAI(BinaryWriter writer)
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
        }*/

        public override void AI()
        {
            if (npc.localAI[3] == 0) //spawn friends
            {
                npc.TargetClosest(false);
                Movement(Main.player[npc.target].Center, 0.8f, 32f);
                if (npc.Distance(Main.player[npc.target].Center) < 1500)
                    npc.localAI[3] = 1;
                else
                    return;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    const int max = 8;
                    const float distance = 110f;
                    float rotation = 2f * (float)Math.PI / max;
                    for (int i = 0; i < max; i++)
                    {
                        Vector2 spawnPos = npc.Center + new Vector2(distance, 0f).RotatedBy(rotation * i);
                        int n = NPC.NewNPC((int)spawnPos.X, (int)spawnPos.Y, ModContent.NPCType<ShadowOrb>(), 0, npc.whoAmI, distance, 0, rotation * i);
                        if (Main.netMode == NetmodeID.Server && n < 200)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                    }
                }
            }

            EModeGlobalNPC.championBoss = npc.whoAmI;

            Player player = Main.player[npc.target];
            Vector2 targetPos;

            if (npc.HasValidTarget && npc.Distance(player.Center) < 2500 && !Main.dayTime)
                npc.timeLeft = 600;

            npc.direction = npc.spriteDirection = npc.Center.X < player.Center.X ? 1 : -1;

            if (npc.localAI[3] == 1 && npc.life < npc.lifeMax * (FargoSoulsWorld.MasochistMode ? 0.66 : .5))
            {
                npc.localAI[3] = 2;
                npc.dontTakeDamage = true;
                npc.netUpdate = true;
                float buffer = npc.ai[0];
                npc.ai[0] = -1;
                npc.ai[1] = 0;
                npc.ai[2] = buffer;
                npc.ai[3] = 0;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    const int max = 16;
                    const float distance = 700f;
                    float rotation = 2f * (float)Math.PI / max;
                    for (int i = 0; i < max; i++)
                    {
                        Vector2 spawnPos = npc.Center + new Vector2(distance, 0f).RotatedBy(rotation * i);
                        int n = NPC.NewNPC((int)spawnPos.X, (int)spawnPos.Y, ModContent.NPCType<ShadowOrb>(), 0, npc.whoAmI, distance, 0, rotation * i);
                        if (Main.netMode == NetmodeID.Server && n < 200)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                    }
                }

                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].hostile && Main.projectile[i].type == ModContent.ProjectileType<ShadowClone>())
                        Main.projectile[i].Kill();
                }
            }
            else if (npc.localAI[3] == 2 && npc.life < npc.lifeMax * .33 && FargoSoulsWorld.MasochistMode)
            {
                npc.localAI[3] = 3;
                npc.dontTakeDamage = true;
                npc.netUpdate = true;
                float buffer = npc.ai[0];
                npc.ai[0] = -1;
                npc.ai[1] = 0;
                npc.ai[2] = buffer;
                npc.ai[3] = 0;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    const int max = 24;
                    const float distance = 350f;
                    float rotation = 2f * (float)Math.PI / max;
                    for (int i = 0; i < max; i++)
                    {
                        Vector2 spawnPos = npc.Center + new Vector2(distance, 0f).RotatedBy(rotation * i);
                        int n = NPC.NewNPC((int)spawnPos.X, (int)spawnPos.Y, ModContent.NPCType<ShadowOrb>(), 0, npc.whoAmI, distance, 0, rotation * i);
                        if (Main.netMode == NetmodeID.Server && n < 200)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                    }
                }

                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].hostile && Main.projectile[i].type == ModContent.ProjectileType<ShadowClone>())
                        Main.projectile[i].Kill();
                }
            }

            if (npc.dontTakeDamage && npc.ai[0] != -1)
            {
                bool anyBallInvulnerable = false;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<ShadowOrb>() && Main.npc[i].ai[0] == npc.whoAmI
                        && !Main.npc[i].dontTakeDamage)
                    {
                        anyBallInvulnerable = true;
                        break;
                    }
                }

                if (!anyBallInvulnerable)
                {
                    Main.PlaySound(SoundID.Item92, npc.Center);

                    const int num226 = 80;
                    for (int num227 = 0; num227 < num226; num227++)
                    {
                        Vector2 vector6 = Vector2.UnitX * 40f;
                        vector6 = vector6.RotatedBy(((num227 - (num226 / 2 - 1)) * 6.28318548f / num226), default(Vector2)) + npc.Center;
                        Vector2 vector7 = vector6 - npc.Center;
                        int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 27, 0f, 0f, 0, default(Color), 3f);
                        Main.dust[num228].noGravity = true;
                        Main.dust[num228].velocity = vector7;
                    }

                    npc.dontTakeDamage = false;
                }
            }

            switch ((int)npc.ai[0])
            {
                case -1: //trails for orbs
                    npc.dontTakeDamage = true;
                    npc.velocity *= 0.97f;

                    if (npc.ai[1] == 120)
                    {
                        Main.PlaySound(SoundID.Roar, npc.Center, 0);
                    }

                    if (++npc.ai[3] > 9 && npc.ai[1] > 120)
                    {
                        npc.ai[3] = 0;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            for (int i = 0; i < Main.maxNPCs; i++)
                            {
                                if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<ShadowOrb>() && Main.npc[i].ai[0] == npc.whoAmI)
                                {
                                    Vector2 vel = npc.DirectionTo(Main.npc[i].Center).RotatedBy(Math.PI / 2);
                                    Projectile.NewProjectile(Main.npc[i].Center, vel, ProjectileID.DemonSickle, npc.damage / 3, 0f, Main.myPlayer);
                                }
                            }
                        }
                    }

                    if (++npc.ai[1] > 300)
                    {
                        npc.TargetClosest();
                        npc.ai[0] = npc.ai[2];
                        if (npc.ai[0] % 2 == 1) //always delay before resuming attack
                            npc.ai[0]--;
                        if (npc.ai[0] == 6) //skip shadow dash
                            npc.ai[0] += 2;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;
                        npc.netUpdate = true;
                    }
                    break;

                case 0: //float over player
                    if (!player.active || player.dead || Vector2.Distance(npc.Center, player.Center) > 2500f || Main.dayTime) //despawn code
                    {
                        npc.TargetClosest(false);
                        if (npc.timeLeft > 30)
                            npc.timeLeft = 30;

                        npc.noTileCollide = true;
                        npc.noGravity = true;
                        npc.velocity.Y -= 1f;

                        break;
                    }
                    else
                    {
                        targetPos = player.Center + npc.DirectionFrom(player.Center) * 400f;
                        if (npc.Distance(targetPos) > 50)
                            Movement(targetPos, 0.2f, 24f);
                    }

                    if (++npc.ai[1] > 60)
                    {
                        npc.TargetClosest();
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;
                        npc.netUpdate = true;
                    }
                    break;

                case 1: //dungeon guardians
                    targetPos = player.Center + npc.DirectionFrom(player.Center) * 400f;
                    if (npc.Distance(targetPos) > 50)
                        Movement(targetPos, 0.2f, 24f);

                    //warning dust
                    Main.dust[Dust.NewDust(npc.Center, 0, 0, DustID.Fire, 0f, 0f, 0, default(Color), 2f)].velocity *= 7f;

                    if (npc.ai[1] == 90 && Main.netMode != NetmodeID.MultiplayerClient) //telegraph
                    {
                        for (int i = -1; i <= 1; i++) //on both sides
                        {
                            if (i == 0)
                                continue;

                            //p2 fires from above/below, others fire from sides
                            Vector2 spawnPos = player.Center + i * (npc.localAI[3] == 2 ? Vector2.UnitY * 1000 : Vector2.UnitX * 1000);

                            for (int j = -1; j <= 1; j++) //three angles
                            {
                                Vector2 vel = Vector2.Normalize(player.Center - spawnPos);
                                vel = vel.RotatedBy(MathHelper.ToRadians(25) * j); //offset between three streams
                                Projectile.NewProjectile(spawnPos, vel, ModContent.ProjectileType<GuardianDeathraySmall>(), 0, 0f, Main.myPlayer, npc.target, -1f);
                            }

                            if (npc.localAI[3] == 3) //p3 also spawns one stream from above/below
                            {
                                Vector2 wallSpawn = player.Center + i * Vector2.UnitY * 1000;
                                Projectile.NewProjectile(wallSpawn, Vector2.Normalize(player.Center - wallSpawn),
                                    ModContent.ProjectileType<GuardianDeathraySmall>(), 0, 0f, Main.myPlayer, npc.target, -1f);
                            }
                        }
                    }

                    if (++npc.ai[2] > 5 && npc.ai[1] > 120)
                    {
                        npc.ai[2] = 0;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            for (int i = -1; i <= 1; i++) //on both sides
                            {
                                if (i == 0)
                                    continue;

                                //p2 fires from above/below, others fire from sides
                                Vector2 spawnPos = player.Center + i * (npc.localAI[3] == 2 ? Vector2.UnitY * 1000 : Vector2.UnitX * 1000);

                                for (int j = -1; j <= 1; j++) //three angles
                                {
                                    Vector2 vel = Main.rand.NextFloat(20f, 25f) * Vector2.Normalize(player.Center - spawnPos);
                                    vel = vel.RotatedBy(MathHelper.ToRadians(25) * j); //offset between three streams
                                    vel = vel.RotatedBy(MathHelper.ToRadians(5) * (Main.rand.NextDouble() - 0.5)); //random variation
                                    if (j != 0)
                                        vel *= 1.75f;
                                    int p = Projectile.NewProjectile(spawnPos, vel, ModContent.ProjectileType<ShadowGuardian>(), npc.damage / 3, 0f, Main.myPlayer);
                                    if (p != Main.maxProjectiles)
                                        Main.projectile[p].timeLeft = 240;
                                }

                                if (npc.localAI[3] == 3) //p3 also spawns one stream from above/below
                                {
                                    Vector2 wallSpawn = player.Center + i * Vector2.UnitY * 1000;
                                    int p = Projectile.NewProjectile(wallSpawn, Main.rand.NextFloat(20, 25f) * Vector2.Normalize(player.Center - wallSpawn),
                                        ModContent.ProjectileType<ShadowGuardian>(), npc.damage / 3, 0f, Main.myPlayer);
                                    if (p != Main.maxProjectiles)
                                        Main.projectile[p].timeLeft = 240;
                                }
                            }
                        }
                    }

                    if (++npc.ai[1] == 120)
                    {
                        Main.PlaySound(SoundID.Roar, (int)npc.Center.X, (int)npc.Center.Y, 0);
                    }
                    else if (npc.ai[1] > 300)
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

                case 3: //curving flamebursts
                    targetPos = player.Center + npc.DirectionFrom(player.Center) * 600f;
                    if (npc.Distance(targetPos) > 50)
                        Movement(targetPos, 0.1f, 24f);

                    if (npc.localAI[3] == 2) //faster in p2 only
                        npc.ai[2] += 0.5f;

                    if (++npc.ai[2] > 60)
                    {
                        npc.ai[2] = 0;

                        Main.PlaySound(SoundID.Item, npc.Center, 14);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            if (npc.localAI[3] == 3) //p3, triangle fire
                            {
                                for (int j = 0; j < 3; j++)
                                {
                                    for (int i = 0; i < 20; i++)
                                    {
                                        Vector2 vel = npc.DirectionTo(player.Center).RotatedBy(Math.PI / 6 * (Main.rand.NextDouble() - 0.5) + 2 * Math.PI / 3 * j);
                                        float ai0 = Main.rand.NextFloat(1.04f, 1.06f);
                                        float ai1 = Main.rand.NextFloat(0.05f);
                                        Projectile.NewProjectile(npc.Center, vel, ModContent.ProjectileType<ShadowFlameburst>(), npc.damage / 3, 0f, Main.myPlayer, ai0, ai1);
                                    }
                                }
                            }
                            else if (npc.localAI[3] == 2) //p2, fire them to both sides
                            {
                                for (int j = -1; j <= 1; j++)
                                {
                                    if (j == 0)
                                        continue;

                                    for (int i = 0; i < 25; i++)
                                    {
                                        Vector2 vel = npc.DirectionTo(player.Center).RotatedBy(Math.PI / 6 * (Main.rand.NextDouble() - 0.5) + Math.PI / 2 * j);
                                        float ai0 = Main.rand.NextFloat(1.04f, 1.06f);
                                        float ai1 = Main.rand.NextFloat(0.06f);
                                        Projectile.NewProjectile(npc.Center, vel, ModContent.ProjectileType<ShadowFlameburst>(), npc.damage / 3, 0f, Main.myPlayer, ai0, ai1);
                                    }
                                }
                            }
                            else //p1
                            {
                                for (int i = 0; i < 40; i++)
                                {
                                    Vector2 vel = 3f * npc.DirectionTo(player.Center).RotatedBy(Math.PI / 6 * (Main.rand.NextDouble() - 0.5));
                                    float max = 0.0075f;
                                    float ai0 = Main.rand.NextFloat(1.04f, 1.06f);
                                    float ai1 = Main.rand.NextFloat(-max, max);
                                    Projectile.NewProjectile(npc.Center, vel, ModContent.ProjectileType<ShadowFlameburst>(), npc.damage / 3, 0f, Main.myPlayer, ai0, ai1);
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

                case 4:
                    goto case 0;

                case 5: //flaming scythe shadow orbs
                    targetPos = player.Center + npc.DirectionFrom(player.Center) * 400f;
                    if (npc.Distance(targetPos) > 50)
                        Movement(targetPos, 0.3f, 24f);

                    if (++npc.ai[2] > (npc.localAI[3] > 1 ? 90 : 120) && npc.ai[1] < 330) //fire a little faster depending on phase
                    {
                        npc.ai[2] = 0;

                        Main.PlaySound(SoundID.Item8, npc.Center);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 vel = (player.Center - npc.Center) / 30;
                            if (npc.localAI[3] == 3) //p3 fires them to both sides instead
                            {
                                vel = vel.RotatedBy(Math.PI / 2) * 0.75f;
                                Projectile.NewProjectile(npc.Center, -vel, ModContent.ProjectileType<Projectiles.Champions.ShadowOrb>(), npc.damage / 4, 0f, Main.myPlayer);
                            }
                            Projectile.NewProjectile(npc.Center, vel, ModContent.ProjectileType<Projectiles.Champions.ShadowOrb>(), npc.damage / 4, 0f, Main.myPlayer);
                        }
                    }

                    if (++npc.ai[1] > (npc.localAI[3] == 3 ? 450 : 420))
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

                case 7: //dash for tentacles
                    if (++npc.ai[2] == 1)
                    {
                        Main.PlaySound(SoundID.NPCHit6, npc.Center);
                        npc.velocity = (player.Center - npc.Center) / 30f * (1f + npc.localAI[3] / 3f * 0.75f);
                        npc.netUpdate = true;
                    }
                    else if (npc.ai[2] == 31)
                    {
                        npc.velocity = Vector2.Zero;
                        npc.netUpdate = true;
                    }
                    else if (npc.ai[2] == 38)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 vel = new Vector2(12f, 0f).RotatedByRandom(2 * Math.PI);
                            for (int i = 0; i < 20; i++)
                            {
                                Vector2 speed = vel.RotatedBy(2 * Math.PI / 6 * (i + Main.rand.NextDouble() - 0.5));
                                float ai1 = Main.rand.Next(10, 80) * (1f / 1000f);
                                if (Main.rand.Next(2) == 0)
                                    ai1 *= -1f;
                                float ai0 = Main.rand.Next(10, 80) * (1f / 1000f);
                                if (Main.rand.Next(2) == 0)
                                    ai0 *= -1f;
                                Projectile.NewProjectile(npc.Center, speed, ModContent.ProjectileType<ShadowflameTentacleHostile>(), npc.damage / 4, 0f, Main.myPlayer, ai0, ai1);
                            }
                        }
                    }
                    else if (npc.ai[2] > 60)
                    {
                        npc.ai[2] = 0;
                    }

                    if (++npc.ai[1] > 330)
                    {
                        npc.TargetClosest();
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;
                        npc.netUpdate = true;
                    }
                    break;

                case 8:
                    goto case 0;

                case 9: //shadow clones
                    targetPos = player.Center + npc.DirectionFrom(player.Center) * 400f;
                    if (npc.Distance(targetPos) > 50)
                        Movement(targetPos, 0.3f, 24f);

                    if (npc.ai[2] == 0)
                    {
                        npc.ai[2] = 1;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            for (int i = 0; i < 10; i++)
                            {
                                if (npc.localAI[3] == 1 && i % 2 == 0) //dont do half of them in p1
                                    continue;
                                for (int j = 0; j < (npc.localAI[3] == 3 ? 2 : 1); j++) //do twice as many in p3
                                {
                                    Vector2 spawnPos = player.Center + Main.rand.NextFloat(500, 700) * Vector2.UnitX.RotatedBy(Main.rand.NextDouble() * 2 * Math.PI);
                                    Vector2 vel = npc.velocity.RotatedBy(Main.rand.NextDouble() * Math.PI * 2);
                                    Projectile.NewProjectile(spawnPos, vel, ModContent.ProjectileType<ShadowClone>(),
                                        npc.damage / 4, 0f, Main.myPlayer, npc.target, 60 + 30 * i);
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

                default:
                    npc.ai[0] = 0;
                    goto case 0;
            }

            if (npc.dontTakeDamage)
            {
                for (int i = 0; i < 3; i++)
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, 27, 0f, 0f, 0, default(Color), 2f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 4f;
                }
                for (int i = 0; i < 3; i++)
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, 54, 0f, 0f, 0, default(Color), 5f);
                    Main.dust[d].noGravity = true;
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (++npc.frameCounter > 3)
            {
                npc.frameCounter = 0;
                npc.frame.Y += frameHeight;
                if (npc.frame.Y >= frameHeight * 5)
                    npc.frame.Y = 0;
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

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Darkness, 300);
            if (FargoSoulsWorld.MasochistMode)
            {
                target.AddBuff(mod.BuffType("Shadowflame"), 300);
                target.AddBuff(BuffID.Blackout, 300);
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.SuperHealingPotion;
        }

        public override void NPCLoot()
        {
            FargoSoulsWorld.downedChampions[5] = true;
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData); //sync world

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].hostile && Main.projectile[i].type == ModContent.ProjectileType<ShadowClone>())
                    Main.projectile[i].Kill();
            }

            //Item.NewItem(npc.position, npc.Size, ModContent.ItemType<LifeForce>());
            int[] drops = {
                ModContent.ItemType<AncientShadowEnchant>(),
                ModContent.ItemType<NecroEnchant>(),
                ModContent.ItemType<SpookyEnchant>(),
                ModContent.ItemType<ShinobiEnchant>(),
                ModContent.ItemType<DarkArtistEnchant>(),
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

        public override Color? GetAlpha(Color drawColor)
        {
            if (npc.dontTakeDamage)
                return Color.Black;
            return null;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (npc.dontTakeDamage)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp/*.PointWrap*/, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

                ArmorShaderData shader = GameShaders.Armor.GetShaderFromItemId(ItemID.VoidDye);
                shader.Apply(npc, new Terraria.DataStructures.DrawData?());
            }

            Texture2D texture2D13 = Main.npcTexture[npc.type];
            Texture2D texture2D14 = mod.GetTexture("NPCs/Champions/ShadowChampion_Trail");
            //int num156 = Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type]; //ypos of lower right corner of sprite to draw
            //int y3 = num156 * npc.frame.Y; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = npc.frame;//new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = npc.GetAlpha(color26);

            SpriteEffects effects = npc.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < NPCID.Sets.TrailCacheLength[npc.type]; i++)
            {
                Color color27 = Color.White * 0.25f;
                color27 *= (float)(NPCID.Sets.TrailCacheLength[npc.type] - i) / NPCID.Sets.TrailCacheLength[npc.type];
                Vector2 value4 = npc.oldPos[i];
                float num165 = npc.rotation; //npc.oldRot[i];
                Main.spriteBatch.Draw(texture2D14, value4 + npc.Size / 2f - Main.screenPosition + new Vector2(0, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, npc.scale, effects, 0f);
            }

            Main.spriteBatch.Draw(texture2D13, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), npc.GetAlpha(lightColor), npc.rotation, origin2, npc.scale, effects, 0f);

            if (npc.dontTakeDamage)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            }
            return false;
        }
    }
}
