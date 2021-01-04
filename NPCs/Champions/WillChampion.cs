using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.Items.Accessories.Enchantments;
using FargowiltasSouls.Projectiles.Champions;
using System.IO;
using Terraria.Graphics.Shaders;

namespace FargowiltasSouls.NPCs.Champions
{
    [AutoloadBossHead]
    public class WillChampion : ModNPC
    {
        public bool spawned;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Champion of Will");
            DisplayName.AddTranslation(GameCulture.Chinese, "意志英灵");
            Main.npcFrameCount[npc.type] = 8;
            NPCID.Sets.TrailCacheLength[npc.type] = 10;
            NPCID.Sets.TrailingMode[npc.type] = 1;
        }

        public override void SetDefaults()
        {
            npc.width = 150;
            npc.height = 100;
            npc.damage = 120;
            npc.defense = 80;
            npc.lifeMax = 420000;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
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

            npc.netAlways = true;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return npc.Distance(target.Center) < 120;
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
            if (!spawned)
            {
                npc.TargetClosest(false);
                Movement(Main.player[npc.target].Center, 0.8f, 32f);
                if (npc.Distance(Main.player[npc.target].Center) < 1500)
                    spawned = true;
                else
                    return;
            }

            EModeGlobalNPC.championBoss = npc.whoAmI;

            if (!npc.HasValidTarget)
                npc.TargetClosest(false);

            Player player = Main.player[npc.target];

            if (npc.HasValidTarget && npc.Distance(player.Center) < 2500)
                npc.timeLeft = 600;

            if (npc.localAI[2] == 0 && npc.life < npc.lifeMax * .66)
            {
                npc.localAI[2] = 1;
                npc.ai[0] = -1;
                npc.ai[1] = 0;
                npc.ai[2] = 0;
                npc.localAI[0] = 0;
                npc.netUpdate = true;
            }
            else if (npc.localAI[3] == 0 && npc.life < npc.lifeMax * .33)
            {
                npc.localAI[3] = 1;
                npc.ai[0] = -1;
                npc.ai[1] = 0;
                npc.ai[2] = 0;
                npc.localAI[0] = 0;
                npc.netUpdate = true;
            }

            npc.damage = npc.defDamage;

            switch ((int)npc.ai[0])
            {
                case -1:
                    {
                        if (!npc.HasValidTarget)
                            npc.TargetClosest(false);

                        npc.damage = 0;
                        npc.dontTakeDamage = true;
                        npc.velocity *= 0.9f;

                        if (++npc.ai[2] > 60)
                        {
                            npc.ai[2] = 20;

                            if (npc.ai[1] < 330)
                                Main.PlaySound(SoundID.Item92, npc.Center);

                            npc.localAI[0] = npc.localAI[0] > 0 ? -1 : 1;
                            
                            if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[1] < 330)
                            {
                                int max = npc.life < npc.lifeMax / 2 && FargoSoulsWorld.MasochistMode ? 8 : 6;
                                float offset = npc.localAI[0] > 0 && player.velocity != Vector2.Zero //aim to intercept
                                    ? Main.rand.NextFloat((float)Math.PI * 2) : player.velocity.ToRotation();
                                for (int i = 0; i < max; i++)
                                {
                                    float rotation = offset + (float)Math.PI * 2 / max * i;
                                    Projectile.NewProjectile(player.Center + 450 * Vector2.UnitX.RotatedBy(rotation), Vector2.Zero,
                                        ModContent.ProjectileType<WillJavelin3>(), npc.defDamage / 4, 0f, Main.myPlayer, 0f, rotation + (float)Math.PI);
                                }
                            }
                        }

                        if (++npc.ai[1] == 1)
                        {
                            Main.PlaySound(SoundID.Item4, npc.Center);

                            for (int i = 0; i < Main.maxProjectiles; i++) //purge leftover bombs and spears
                            {
                                if (Main.projectile[i].active && Main.projectile[i].hostile
                                    && (Main.projectile[i].type == ModContent.ProjectileType<WillBomb>()
                                    || Main.projectile[i].type == ModContent.ProjectileType<WillJavelin>()))
                                {
                                    Main.projectile[i].Kill();
                                }
                            }

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<WillShell>(), 0, 0f, Main.myPlayer, 0f, npc.whoAmI);
                                Projectile.NewProjectile(npc.Center, npc.DirectionTo(player.Center) * 12f, ModContent.ProjectileType<WillBomb>(), npc.defDamage / 4, 0f, Main.myPlayer, 12f / 40f, npc.whoAmI);
                            }

                            const int num226 = 80;
                            for (int num227 = 0; num227 < num226; num227++)
                            {
                                Vector2 vector6 = Vector2.UnitX * 40f;
                                vector6 = vector6.RotatedBy(((num227 - (num226 / 2 - 1)) * 6.28318548f / num226), default(Vector2)) + npc.Center;
                                Vector2 vector7 = vector6 - npc.Center;
                                int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 174, 0f, 0f, 0, default(Color), 3f);
                                Main.dust[num228].noGravity = true;
                                Main.dust[num228].velocity = vector7;
                            }
                        }
                        else if (npc.ai[1] > 380)
                        {
                            npc.ai[0]++;
                            npc.ai[1] = 0;
                            npc.ai[2] = 0;
                            npc.localAI[0] = 0;
                            npc.localAI[1] = 0;
                            npc.netUpdate = true;
                        }

                        /*const int delay = 7;
                        const int gap = 150;

                        int threshold = delay * 2 * 1600 / gap; //rate of spawn * cover length twice * length / gap
                        if (++npc.ai[2] % delay == 0 && npc.ai[2] < threshold * 2)
                        {
                            Main.PlaySound(SoundID.Item92, npc.Center);

                            Vector2 targetPos = new Vector2(npc.localAI[0], npc.localAI[1]);
                            Vector2 speed = new Vector2(Main.rand.NextFloat(-20f, 20f), Main.rand.NextFloat(-20f, 20f));
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(npc.Center, speed, ModContent.ProjectileType<WillJavelin2>(), npc.defDamage / 4, 0f, Main.myPlayer, targetPos.X, targetPos.Y);
                            }
                            if (npc.ai[2] < threshold)
                                npc.localAI[0] += gap;
                            else
                                npc.localAI[0] -= gap;
                        }

                        if (npc.ai[2] == threshold)
                        {
                            npc.localAI[0] += gap / 2; //offset halfway
                        }
                        else if (npc.ai[2] == threshold * 2 + 30) //final wave
                        {
                            npc.localAI[0] -= gap / 2; //revert offset

                            for (int i = 0; i < 1600 / gap * 2; i++)
                            {
                                Vector2 targetPos = new Vector2(npc.localAI[0], npc.localAI[1]);
                                Vector2 speed = new Vector2(Main.rand.NextFloat(-20f, 20f), Main.rand.NextFloat(-20f, 20f));
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(npc.Center, speed, ModContent.ProjectileType<WillJavelin2>(), npc.defDamage / 4, 0f, Main.myPlayer, targetPos.X, targetPos.Y);
                                }
                                npc.localAI[0] += gap;
                            }
                        }

                        if (++npc.ai[1] == 1)
                        {
                            Main.PlaySound(SoundID.Item4, npc.Center);

                            for (int i = 0; i < Main.maxProjectiles; i++) //purge leftover bombs
                            {
                                if (Main.projectile[i].active && Main.projectile[i].hostile && Main.projectile[i].type == ModContent.ProjectileType<WillBomb>())
                                    Main.projectile[i].Kill();
                            }

                            npc.localAI[0] = npc.Center.X - 1600;
                            npc.localAI[1] = npc.Center.Y - 200;
                            if (npc.position.Y < 2400)
                                npc.localAI[1] += 1200;
                            else
                                npc.localAI[1] -= 1200;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<WillShell>(), 0, 0f, Main.myPlayer, 0f, npc.whoAmI);
                                Projectile.NewProjectile(npc.Center, Vector2.UnitY * -12f, ModContent.ProjectileType<WillBomb>(), npc.defDamage / 4, 0f, Main.myPlayer, 0f, npc.whoAmI);
                            }

                            const int num226 = 80;
                            for (int num227 = 0; num227 < num226; num227++)
                            {
                                Vector2 vector6 = Vector2.UnitX * 40f;
                                vector6 = vector6.RotatedBy(((num227 - (num226 / 2 - 1)) * 6.28318548f / num226), default(Vector2)) + npc.Center;
                                Vector2 vector7 = vector6 - npc.Center;
                                int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 174, 0f, 0f, 0, default(Color), 3f);
                                Main.dust[num228].noGravity = true;
                                Main.dust[num228].velocity = vector7;
                            }
                        }
                        else if (npc.ai[1] > threshold * 2 + 270)
                        {
                            npc.ai[0]++;
                            npc.ai[1] = 0;
                            npc.ai[2] = 0;
                            npc.localAI[0] = 0;
                            npc.localAI[1] = 0;
                            npc.netUpdate = true;
                        }*/
                    }
                    break;

                case 0: //float at player
                    npc.dontTakeDamage = false;

                    if (!player.active || player.dead || Vector2.Distance(npc.Center, player.Center) > 2500f) //despawn code
                    {
                        if (npc.timeLeft > 30)
                            npc.timeLeft = 30;

                        npc.noTileCollide = true;
                        npc.noGravity = true;
                        npc.velocity.Y -= 1f;

                        return;
                    }
                    else
                    {
                        if (++npc.ai[1] > 45f) //time to progress
                        {
                            npc.TargetClosest(false);

                            npc.ai[0]++;
                            npc.ai[1] = 0;
                            //npc.ai[2] = 0;
                            //npc.ai[3] = 0;
                            npc.netUpdate = true;

                            if (++npc.ai[2] > 4) //decide next action
                            {
                                npc.ai[2] = 0;
                                if (++npc.ai[3] > 3) //count which attack to do next
                                    npc.ai[3] = 1;
                                npc.ai[0] += npc.ai[3];
                            }
                            else //actually just dash
                            {
                                npc.velocity = npc.DirectionTo(player.Center) * 33f;
                            }
                        }
                        else //regular movement
                        {
                            Vector2 vel = player.Center - npc.Center;
                            npc.rotation = vel.ToRotation();

                            const float moveSpeed = 2f;

                            if (vel.X > 0) //im on left side of target
                            {
                                vel.X -= 450;
                                npc.direction = npc.spriteDirection = 1;
                            }
                            else //im on right side of target
                            {
                                vel.X += 450;
                                npc.direction = npc.spriteDirection = -1;
                            }
                            vel.Y -= 200f;
                            vel.Normalize();
                            vel *= 20f;
                            if (npc.velocity.X < vel.X)
                            {
                                npc.velocity.X += moveSpeed;
                                if (npc.velocity.X < 0 && vel.X > 0)
                                    npc.velocity.X += moveSpeed;
                            }
                            else if (npc.velocity.X > vel.X)
                            {
                                npc.velocity.X -= moveSpeed;
                                if (npc.velocity.X > 0 && vel.X < 0)
                                    npc.velocity.X -= moveSpeed;
                            }
                            if (npc.velocity.Y < vel.Y)
                            {
                                npc.velocity.Y += moveSpeed;
                                if (npc.velocity.Y < 0 && vel.Y > 0)
                                    npc.velocity.Y += moveSpeed;
                            }
                            else if (npc.velocity.Y > vel.Y)
                            {
                                npc.velocity.Y -= moveSpeed;
                                if (npc.velocity.Y > 0 && vel.Y < 0)
                                    npc.velocity.Y -= moveSpeed;
                            }
                        }
                    }
                    break;

                case 1: //dashing
                    npc.rotation = npc.velocity.ToRotation();
                    npc.direction = npc.spriteDirection = npc.velocity.X > 0 ? 1 : -1;

                    int num22 = 7;
                    for (int index1 = 0; index1 < num22; ++index1)
                    {
                        Vector2 vector2_1 = (Vector2.Normalize(npc.velocity) * new Vector2((npc.width + 50) / 2f, npc.height) * 0.75f).RotatedBy((index1 - (num22 / 2 - 1)) * Math.PI / num22, new Vector2()) + npc.Center;
                        Vector2 vector2_2 = ((float)(Main.rand.NextDouble() * 3.14159274101257) - 1.570796f).ToRotationVector2() * Main.rand.Next(3, 8);
                        Vector2 vector2_3 = vector2_2;
                        int index2 = Dust.NewDust(vector2_1 + vector2_3, 0, 0, 87, vector2_2.X * 2f, vector2_2.Y * 2f, 100, new Color(), 1.4f);
                        Main.dust[index2].noGravity = true;
                        Main.dust[index2].velocity /= 4f;
                        Main.dust[index2].velocity -= npc.velocity;
                    }

                    if (--npc.localAI[0] < 0)
                    {
                        npc.localAI[0] = 2;
                        if (Main.netMode != NetmodeID.MultiplayerClient && npc.localAI[3] == 1)
                        {
                            Projectile.NewProjectile(npc.Center, 1.5f * Vector2.Normalize(npc.velocity).RotatedBy(Math.PI / 2), 
                                ModContent.ProjectileType<WillFireball2>(), npc.damage / 4, 0f, Main.myPlayer);
                            Projectile.NewProjectile(npc.Center, 1.5f * Vector2.Normalize(npc.velocity).RotatedBy(-Math.PI / 2),
                                ModContent.ProjectileType<WillFireball2>(), npc.damage / 4, 0f, Main.myPlayer);
                        }
                    }

                    if (++npc.ai[1] > 30)
                    {
                        npc.ai[0]--; //return to previous step
                        npc.ai[1] = 0;
                        //npc.ai[2] = 0;
                        //npc.ai[3] = 0;
                        npc.localAI[0] = 2;
                        npc.netUpdate = true;
                    }
                    break;

                case 2: //arena bomb
                    npc.velocity *= 0.975f;
                    npc.rotation = npc.DirectionTo(player.Center).ToRotation();
                    npc.direction = npc.spriteDirection = npc.Center.X < player.Center.X ? 1 : -1;

                    if (++npc.ai[1] == 30) //spawn bomb
                    {
                        Main.PlaySound(SoundID.ForceRoar, npc.Center, -1);

                        const int num226 = 40;
                        for (int num227 = 0; num227 < num226; num227++)
                        {
                            Vector2 vector6 = Vector2.UnitX * 15f;
                            vector6 = vector6.RotatedBy(((num227 - (num226 / 2 - 1)) * 6.28318548f / num226), default(Vector2)) + npc.Center;
                            Vector2 vector7 = vector6 - npc.Center;
                            int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 174, 0f, 0f, 0, default(Color), 2f);
                            Main.dust[num228].noGravity = true;
                            Main.dust[num228].velocity = vector7;
                        }

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(npc.Center, npc.DirectionTo(player.Center) * 12f, ModContent.ProjectileType<WillBomb>(), npc.defDamage / 4, 0f, Main.myPlayer, 12f / 40f, npc.whoAmI);
                        }
                    }
                    else if (npc.ai[1] > 120)
                    {
                        npc.ai[0] = 0;
                        npc.ai[1] = 0;
                        npc.netUpdate = true;
                    }
                    break;

                case 3: //spear barrage
                    {
                        Vector2 vel = player.Center - npc.Center;
                        npc.rotation = vel.ToRotation();

                        const float moveSpeed = 0.25f;

                        if (vel.X > 0) //im on left side of target
                        {
                            vel.X -= 450;
                            npc.direction = npc.spriteDirection = 1;
                        }
                        else //im on right side of target
                        {
                            vel.X += 450;
                            npc.direction = npc.spriteDirection = -1;
                        }
                        vel.Y -= 200f;
                        vel.Normalize();
                        vel *= 16f;
                        if (npc.velocity.X < vel.X)
                        {
                            npc.velocity.X += moveSpeed;
                            if (npc.velocity.X < 0 && vel.X > 0)
                                npc.velocity.X += moveSpeed;
                        }
                        else if (npc.velocity.X > vel.X)
                        {
                            npc.velocity.X -= moveSpeed;
                            if (npc.velocity.X > 0 && vel.X < 0)
                                npc.velocity.X -= moveSpeed;
                        }
                        if (npc.velocity.Y < vel.Y)
                        {
                            npc.velocity.Y += moveSpeed;
                            if (npc.velocity.Y < 0 && vel.Y > 0)
                                npc.velocity.Y += moveSpeed;
                        }
                        else if (npc.velocity.Y > vel.Y)
                        {
                            npc.velocity.Y -= moveSpeed;
                            if (npc.velocity.Y > 0 && vel.Y < 0)
                                npc.velocity.Y -= moveSpeed;
                        }

                        if (--npc.localAI[0] < 0)
                        {
                            npc.localAI[0] = npc.localAI[2] == 1 ? 30 : 40;

                            if (npc.ai[1] < 110 || npc.localAI[3] == 1)
                            {
                                Main.PlaySound(SoundID.ForceRoar, npc.Center, -1);

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    for (int i = 0; i < 15; i++)
                                    {
                                        float speed = Main.rand.NextFloat(4f, 8f);
                                        Vector2 velocity = speed * Vector2.UnitX.RotatedBy(Main.rand.NextDouble() * -Math.PI);
                                        float ai1 = speed / 120f;
                                        Projectile.NewProjectile(npc.Center, velocity, ModContent.ProjectileType<WillJavelin>(), npc.defDamage / 4, 0f, Main.myPlayer, 0f, ai1);
                                    }
                                }
                            }
                        }

                        if (++npc.ai[1] > 150)
                        {
                            npc.ai[0] = 0;
                            npc.ai[1] = 0;
                            npc.localAI[0] = 0;
                            npc.netUpdate = true;
                        }
                    }
                    break;

                case 4: //fireballs
                    {
                        Vector2 vel = player.Center - npc.Center;
                        npc.rotation = vel.ToRotation();

                        const float moveSpeed = 0.25f;

                        if (vel.X > 0) //im on left side of target
                        {
                            vel.X -= 550;
                            npc.direction = npc.spriteDirection = 1;
                        }
                        else //im on right side of target
                        {
                            vel.X += 550;
                            npc.direction = npc.spriteDirection = -1;
                        }
                        vel.Y -= 250f;
                        vel.Normalize();
                        vel *= 16f;
                        if (npc.velocity.X < vel.X)
                        {
                            npc.velocity.X += moveSpeed;
                            if (npc.velocity.X < 0 && vel.X > 0)
                                npc.velocity.X += moveSpeed;
                        }
                        else if (npc.velocity.X > vel.X)
                        {
                            npc.velocity.X -= moveSpeed;
                            if (npc.velocity.X > 0 && vel.X < 0)
                                npc.velocity.X -= moveSpeed;
                        }
                        if (npc.velocity.Y < vel.Y)
                        {
                            npc.velocity.Y += moveSpeed;
                            if (npc.velocity.Y < 0 && vel.Y > 0)
                                npc.velocity.Y += moveSpeed;
                        }
                        else if (npc.velocity.Y > vel.Y)
                        {
                            npc.velocity.Y -= moveSpeed;
                            if (npc.velocity.Y > 0 && vel.Y < 0)
                                npc.velocity.Y -= moveSpeed;
                        }

                        if (++npc.localAI[0] > 3)
                        {
                            npc.localAI[0] = 0;
                            if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[1] < 90) //shoot fireball
                            {
                                Main.PlaySound(SoundID.Item34, npc.Center);
                                Vector2 spawn = new Vector2(40, 50);
                                if (npc.direction < 0)
                                {
                                    spawn.X *= -1;
                                    spawn = spawn.RotatedBy(Math.PI);
                                }
                                spawn = spawn.RotatedBy(npc.rotation);
                                spawn += npc.Center;
                                Vector2 projVel = npc.DirectionTo(player.Center).RotatedBy((Main.rand.NextDouble() - 0.5) * Math.PI / 10);
                                projVel.Normalize();
                                projVel *= Main.rand.NextFloat(8f, 12f);
                                int type = ProjectileID.CultistBossFireBall;
                                if (Main.rand.Next(2) == 0)
                                {
                                    type = ModContent.ProjectileType<WillFireball>();
                                    projVel *= 2.5f;
                                }
                                Projectile.NewProjectile(spawn, projVel, type, npc.defDamage / 4, 0f, Main.myPlayer);
                            }
                        }

                        if (--npc.localAI[1] < 0)
                        {
                            npc.localAI[1] = npc.localAI[3] == 1 ? 35 : 180;

                            if (npc.localAI[2] == 1 && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(player.Center, Vector2.UnitY, ModContent.ProjectileType<WillDeathraySmall>(), npc.damage / 4, 0f, Main.myPlayer, 0f, npc.whoAmI);
                                Projectile.NewProjectile(player.Center, -Vector2.UnitY, ModContent.ProjectileType<WillDeathraySmall>(), npc.damage / 4, 0f, Main.myPlayer, 0f, npc.whoAmI);
                            }
                        }

                        if (++npc.ai[1] == 1)
                        {
                            Main.PlaySound(SoundID.ForceRoar, npc.Center, -1);
                        }
                        else if (npc.ai[1] > 120)
                        {
                            npc.ai[0] = 0;
                            npc.ai[1] = 0;
                            npc.localAI[0] = 0;
                            npc.netUpdate = true;
                        }
                    }
                    break;

                default:
                    npc.ai[0] = 0;
                    goto case 0;
            }

            if (npc.spriteDirection < 0 && npc.ai[0] != -1f)
                npc.rotation += (float)Math.PI;
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

        public override void FindFrame(int frameHeight)
        {
            if (++npc.frameCounter > 4)
            {
                npc.frameCounter = 0;
                npc.frame.Y += frameHeight;
            }

            if (npc.ai[0] == 0 || npc.ai[0] == 2)
            {
                if (npc.frame.Y >= 6 * frameHeight)
                    npc.frame.Y = 0;
            }
            else
            {
                npc.frame.Y = frameHeight * 7;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.MasochistMode)
            {
                target.AddBuff(ModContent.BuffType<Defenseless>(), 300);
                target.AddBuff(ModContent.BuffType<Midas>(), 300);
            }
            target.AddBuff(BuffID.Bleeding, 300);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                for (int i = 1; i <= 4; i++)
                {
                    Vector2 pos = npc.position + new Vector2(Main.rand.NextFloat(npc.width), Main.rand.NextFloat(npc.height));
                    Gore.NewGore(pos, npc.velocity, mod.GetGoreSlot("Gores/WillGore" + i.ToString()), npc.scale);
                }
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.SuperHealingPotion;
        }

        public override void NPCLoot()
        {
            FargoSoulsWorld.downedChampions[7] = true;
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData); //sync world

            int[] drops = {
                ModContent.ItemType<GoldEnchant>(),
                ModContent.ItemType<PlatinumEnchant>(),
                ModContent.ItemType<GladiatorEnchant>(),
                ModContent.ItemType<RedRidingEnchant>(),
                ModContent.ItemType<ValhallaKnightEnchant>(),
                ModContent.ItemType<WizardEnchant>(),
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
            Texture2D glowmask = ModContent.GetTexture("FargowiltasSouls/NPCs/Champions/WillChampion_Glow");
            Texture2D glowmask2 = ModContent.GetTexture("FargowiltasSouls/NPCs/Champions/WillChampion_Glow2");
            //int num156 = Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type]; //ypos of lower right corner of sprite to draw
            //int y3 = num156 * npc.frame.Y; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = npc.frame;//new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = npc.GetAlpha(color26);

            SpriteEffects effects = npc.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            /*for (int i = 0; i < NPCID.Sets.TrailCacheLength[npc.type]; i++)
            {
                Color color27 = color26 * 0.2f;
                color27 *= (float)(NPCID.Sets.TrailCacheLength[npc.type] - i) / NPCID.Sets.TrailCacheLength[npc.type];
                Vector2 value4 = npc.oldPos[i];
                float num165 = npc.rotation; //npc.oldRot[i];
                Main.spriteBatch.Draw(texture2D13, value4 + npc.Size / 2f - Main.screenPosition + new Vector2(0, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, npc.scale, effects, 0f);
            }*/

            Main.spriteBatch.Draw(texture2D13, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), npc.GetAlpha(lightColor), npc.rotation, origin2, npc.scale, effects, 0f);
            Main.spriteBatch.Draw(glowmask, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, npc.rotation, origin2, npc.scale, effects, 0f);
            
            Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp/*.PointWrap*/, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);
            ArmorShaderData shader = GameShaders.Armor.GetShaderFromItemId(ItemID.NebulaDye);
            shader.Apply(npc, new Terraria.DataStructures.DrawData?());

            for (int i = 0; i < NPCID.Sets.TrailCacheLength[npc.type]; i++)
            {
                Color color27 = Color.White;
                color27 *= (float)(NPCID.Sets.TrailCacheLength[npc.type] - i) / NPCID.Sets.TrailCacheLength[npc.type];
                Vector2 value4 = npc.oldPos[i];
                float num165 = npc.rotation; //npc.oldRot[i];
                Main.spriteBatch.Draw(glowmask2, value4 + npc.Size / 2f - Main.screenPosition + new Vector2(0, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, npc.scale, effects, 0f);
            }
            Main.spriteBatch.Draw(glowmask2, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, npc.rotation, origin2, npc.scale, effects, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            return false;
        }
    }
}
