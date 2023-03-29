using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.ItemDropRules.Conditions;
using FargowiltasSouls.Items.Accessories.Forces;
using FargowiltasSouls.Items.Pets;
using FargowiltasSouls.Projectiles.Champions;
using FargowiltasSouls.Items.Placeables.Relics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.NPCs.Champions
{
    [AutoloadBossHead]
    public class WillChampion : ModNPC
    {
        public bool spawned;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Champion of Will");
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "意志英灵");
            Main.npcFrameCount[NPC.type] = 8;
            NPCID.Sets.TrailCacheLength[NPC.type] = 12;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(NPC.type);
            NPCID.Sets.DebuffImmunitySets.Add(NPC.type, new Terraria.DataStructures.NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[]
                {
                    BuffID.Confused,
                    BuffID.Chilled,
                    BuffID.OnFire,
                    BuffID.Suffocation,
                    ModContent.BuffType<Lethargic>(),
                    ModContent.BuffType<ClippedWings>()
                }
            });

            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Position = new Vector2(16 * 3.5f, 16 * -1.5f),
                PortraitPositionXOverride = 0,
                PortraitPositionYOverride = -12
            });
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Ocean,
                new FlavorTextBestiaryInfoElement($"Mods.FargowiltasSouls.Bestiary.{Name}")
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 150;
            NPC.height = 100;
            NPC.damage = 120;
            NPC.defense = 80;
            NPC.lifeMax = 420000;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0f;
            NPC.lavaImmune = true;
            NPC.aiStyle = -1;
            NPC.value = Item.buyPrice(1);
            NPC.boss = true;

            Music = ModLoader.TryGetMod("FargowiltasMusic", out Mod musicMod)
                ? MusicLoader.GetMusicSlot(musicMod, "Assets/Music/Champions") : MusicID.OtherworldlyBoss1;
            SceneEffectPriority = SceneEffectPriority.BossLow;

            NPC.netAlways = true;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            //NPC.damage = (int)(NPC.damage * 0.5f);
            NPC.lifeMax = (int)(NPC.lifeMax * bossLifeScale);
        }

        public override bool CanHitPlayer(Player target, ref int CooldownSlot)
        {
            CooldownSlot = 1;
            return NPC.Distance(target.Center) < 120;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
        }

        public override void AI()
        {
            if (!spawned)
            {
                NPC.TargetClosest(false);
                Movement(Main.player[NPC.target].Center, 0.8f, 32f);
                if (NPC.Distance(Main.player[NPC.target].Center) < 750f)
                {
                    spawned = true;
                    NPC.ai[2] = 4; //start with a bomb
                    NPC.velocity /= 2;
                    NPC.netUpdate = true;
                }
                else
                {
                    return;
                }
            }

            EModeGlobalNPC.championBoss = NPC.whoAmI;

            if (!NPC.HasValidTarget)
                NPC.TargetClosest(false);

            Player player = Main.player[NPC.target];

            if (NPC.HasValidTarget && NPC.Distance(player.Center) < 2500)
                NPC.timeLeft = 600;

            if (NPC.localAI[2] == 0 && NPC.life < NPC.lifeMax * .66)
            {
                NPC.localAI[2] = 1;
                NPC.ai[0] = -1;
                NPC.ai[1] = 0;
                NPC.ai[2] = 0;
                NPC.localAI[0] = 0;
                NPC.netUpdate = true;
            }
            else if (NPC.localAI[3] == 0 && NPC.life < NPC.lifeMax * .33)
            {
                NPC.localAI[3] = 1;
                NPC.ai[0] = -1;
                NPC.ai[1] = 0;
                NPC.ai[2] = 0;
                NPC.localAI[0] = 0;
                NPC.netUpdate = true;
            }

            NPC.damage = NPC.defDamage;

            switch ((int)NPC.ai[0])
            {
                case -1:
                    {
                        if (!NPC.HasValidTarget)
                            NPC.TargetClosest(false);

                        NPC.damage = 0;
                        NPC.dontTakeDamage = true;
                        NPC.velocity *= 0.9f;

                        if (++NPC.ai[2] >= 60)
                        {
                            NPC.ai[2] = 0;

                            NPC.localAI[0] = NPC.localAI[0] > 0 ? -1 : 1;

                            if (NPC.ai[1] <= 420)
                            {
                                SoundEngine.PlaySound(SoundID.Item92, NPC.Center);

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    int max = NPC.life < NPC.lifeMax / 2 && FargoSoulsWorld.EternityMode ? 10 : 8;
                                    float offset = NPC.localAI[0] > 0 && player.velocity != Vector2.Zero //aim to intercept
                                        ? Main.rand.NextFloat((float)Math.PI * 2) : player.velocity.ToRotation();
                                    for (int i = 0; i < max; i++)
                                    {
                                        float rotation = offset + (float)Math.PI * 2 / max * i;
                                        Projectile.NewProjectile(NPC.GetSource_FromThis(), player.Center + 450 * Vector2.UnitX.RotatedBy(rotation), Vector2.Zero,
                                            ModContent.ProjectileType<WillJavelin3>(), NPC.defDamage / 4, 0f, Main.myPlayer, 0f, rotation + (float)Math.PI);
                                    }
                                }
                            }
                        }

                        if (++NPC.ai[1] == 1)
                        {
                            SoundEngine.PlaySound(SoundID.Item4, NPC.Center);

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
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<WillShell>(), 0, 0f, Main.myPlayer, 0f, NPC.whoAmI);
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, NPC.DirectionTo(player.Center) * 12f, ModContent.ProjectileType<WillBomb>(), NPC.defDamage / 4, 0f, Main.myPlayer, 12f / 40f, NPC.whoAmI);
                            }

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.GlowRing>(), 0, 0f, Main.myPlayer, NPC.whoAmI, -6);
                        }
                        else if (NPC.ai[1] > 480)
                        {
                            NPC.ai[0]++;
                            NPC.ai[1] = 0;
                            NPC.ai[2] = 0;
                            NPC.localAI[0] = 0;
                            NPC.localAI[1] = 0;
                            NPC.netUpdate = true;
                        }

                        /*const int delay = 7;
                        const int gap = 150;

                        int threshold = delay * 2 * 1600 / gap; //rate of spawn * cover length twice * length / gap
                        if (++NPC.ai[2] % delay == 0 && NPC.ai[2] < threshold * 2)
                        {
                            SoundEngine.PlaySound(SoundID.Item92, NPC.Center);

                            Vector2 targetPos = new Vector2(NPC.localAI[0], NPC.localAI[1]);
                            Vector2 speed = new Vector2(Main.rand.NextFloat(-20f, 20f), Main.rand.NextFloat(-20f, 20f));
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(npc.GetSource_FromThis(), NPC.Center, speed, ModContent.ProjectileType<WillJavelin2>(), NPC.defDamage / 4, 0f, Main.myPlayer, targetPos.X, targetPos.Y);
                            }
                            if (NPC.ai[2] < threshold)
                                NPC.localAI[0] += gap;
                            else
                                NPC.localAI[0] -= gap;
                        }

                        if (NPC.ai[2] == threshold)
                        {
                            NPC.localAI[0] += gap / 2; //offset halfway
                        }
                        else if (NPC.ai[2] == threshold * 2 + 30) //final wave
                        {
                            NPC.localAI[0] -= gap / 2; //revert offset

                            for (int i = 0; i < 1600 / gap * 2; i++)
                            {
                                Vector2 targetPos = new Vector2(NPC.localAI[0], NPC.localAI[1]);
                                Vector2 speed = new Vector2(Main.rand.NextFloat(-20f, 20f), Main.rand.NextFloat(-20f, 20f));
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(npc.GetSource_FromThis(), NPC.Center, speed, ModContent.ProjectileType<WillJavelin2>(), NPC.defDamage / 4, 0f, Main.myPlayer, targetPos.X, targetPos.Y);
                                }
                                NPC.localAI[0] += gap;
                            }
                        }

                        if (++NPC.ai[1] == 1)
                        {
                            SoundEngine.PlaySound(SoundID.Item4, NPC.Center);

                            for (int i = 0; i < Main.maxProjectiles; i++) //purge leftover bombs
                            {
                                if (Main.projectile[i].active && Main.projectile[i].hostile && Main.projectile[i].type == ModContent.ProjectileType<WillBomb>())
                                    Main.projectile[i].Kill();
                            }

                            NPC.localAI[0] = NPC.Center.X - 1600;
                            NPC.localAI[1] = NPC.Center.Y - 200;
                            if (NPC.position.Y < 2400)
                                NPC.localAI[1] += 1200;
                            else
                                NPC.localAI[1] -= 1200;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(npc.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<WillShell>(), 0, 0f, Main.myPlayer, 0f, NPC.whoAmI);
                                Projectile.NewProjectile(npc.GetSource_FromThis(), NPC.Center, Vector2.UnitY * -12f, ModContent.ProjectileType<WillBomb>(), NPC.defDamage / 4, 0f, Main.myPlayer, 0f, NPC.whoAmI);
                            }

                            const int num226 = 80;
                            for (int num227 = 0; num227 < num226; num227++)
                            {
                                Vector2 vector6 = Vector2.UnitX * 40f;
                                vector6 = vector6.RotatedBy(((num227 - (num226 / 2 - 1)) * 6.28318548f / num226), default(Vector2)) + NPC.Center;
                                Vector2 vector7 = vector6 - NPC.Center;
                                int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 174, 0f, 0f, 0, default(Color), 3f);
                                Main.dust[num228].noGravity = true;
                                Main.dust[num228].velocity = vector7;
                            }
                        }
                        else if (NPC.ai[1] > threshold * 2 + 270)
                        {
                            NPC.ai[0]++;
                            NPC.ai[1] = 0;
                            NPC.ai[2] = 0;
                            NPC.localAI[0] = 0;
                            NPC.localAI[1] = 0;
                            NPC.netUpdate = true;
                        }*/
                    }
                    break;

                case 0: //float at player
                    NPC.dontTakeDamage = false;

                    if (!player.active || player.dead || Vector2.Distance(NPC.Center, player.Center) > 2500f) //despawn code
                    {
                        if (NPC.timeLeft > 30)
                            NPC.timeLeft = 30;

                        NPC.noTileCollide = true;
                        NPC.noGravity = true;
                        NPC.velocity.Y -= 1f;

                        return;
                    }
                    else
                    {
                        if (++NPC.ai[1] > 45f) //time to progress
                        {
                            NPC.TargetClosest(false);

                            NPC.ai[0]++;
                            NPC.ai[1] = 0;
                            //NPC.ai[2] = 0;
                            //NPC.ai[3] = 0;
                            NPC.netUpdate = true;

                            if (++NPC.ai[2] > 4) //decide next action
                            {
                                NPC.ai[2] = 0;
                                if (++NPC.ai[3] > 3) //count which attack to do next
                                    NPC.ai[3] = 1;
                                NPC.ai[0] += NPC.ai[3];
                            }
                            else //actually just dash
                            {
                                NPC.velocity = NPC.DirectionTo(player.Center) * 33f;

                                SoundEngine.PlaySound(SoundID.NPCHit14, NPC.Center);
                            }
                        }
                        else //regular movement
                        {
                            Vector2 vel = player.Center - NPC.Center;
                            NPC.rotation = vel.ToRotation();

                            const float moveSpeed = 2f;

                            if (vel.X > 0) //im on left side of target
                            {
                                vel.X -= 450;
                                NPC.direction = NPC.spriteDirection = 1;
                            }
                            else //im on right side of target
                            {
                                vel.X += 450;
                                NPC.direction = NPC.spriteDirection = -1;
                            }
                            vel.Y -= 200f;
                            vel.Normalize();
                            vel *= 20f;
                            if (NPC.velocity.X < vel.X)
                            {
                                NPC.velocity.X += moveSpeed;
                                if (NPC.velocity.X < 0 && vel.X > 0)
                                    NPC.velocity.X += moveSpeed;
                            }
                            else if (NPC.velocity.X > vel.X)
                            {
                                NPC.velocity.X -= moveSpeed;
                                if (NPC.velocity.X > 0 && vel.X < 0)
                                    NPC.velocity.X -= moveSpeed;
                            }
                            if (NPC.velocity.Y < vel.Y)
                            {
                                NPC.velocity.Y += moveSpeed;
                                if (NPC.velocity.Y < 0 && vel.Y > 0)
                                    NPC.velocity.Y += moveSpeed;
                            }
                            else if (NPC.velocity.Y > vel.Y)
                            {
                                NPC.velocity.Y -= moveSpeed;
                                if (NPC.velocity.Y > 0 && vel.Y < 0)
                                    NPC.velocity.Y -= moveSpeed;
                            }
                        }
                    }
                    break;

                case 1: //dashing
                    NPC.rotation = NPC.velocity.ToRotation();
                    NPC.direction = NPC.spriteDirection = NPC.velocity.X > 0 ? 1 : -1;

                    int num22 = 7;
                    for (int index1 = 0; index1 < num22; ++index1)
                    {
                        Vector2 vector2_1 = (Vector2.Normalize(NPC.velocity) * new Vector2((NPC.width + 50) / 2f, NPC.height) * 0.75f).RotatedBy((index1 - (num22 / 2 - 1)) * Math.PI / num22, new Vector2()) + NPC.Center;
                        Vector2 vector2_2 = ((float)(Main.rand.NextDouble() * 3.14159274101257) - 1.570796f).ToRotationVector2() * Main.rand.Next(3, 8);
                        Vector2 vector2_3 = vector2_2;
                        int index2 = Dust.NewDust(vector2_1 + vector2_3, 0, 0, 87, vector2_2.X * 2f, vector2_2.Y * 2f, 100, new Color(), 1.4f);
                        Main.dust[index2].noGravity = true;
                        Main.dust[index2].velocity /= 4f;
                        Main.dust[index2].velocity -= NPC.velocity;
                    }

                    if (--NPC.localAI[0] < 0)
                    {
                        NPC.localAI[0] = 2;
                        if (Main.netMode != NetmodeID.MultiplayerClient && NPC.localAI[3] == 1)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, 1.5f * Vector2.Normalize(NPC.velocity).RotatedBy(Math.PI / 2),
                                ModContent.ProjectileType<WillFireball2>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer);
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, 1.5f * Vector2.Normalize(NPC.velocity).RotatedBy(-Math.PI / 2),
                                ModContent.ProjectileType<WillFireball2>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer);
                        }
                    }

                    if (++NPC.ai[1] > 30)
                    {
                        NPC.ai[0]--; //return to previous step
                        NPC.ai[1] = 0;
                        //NPC.ai[2] = 0;
                        //NPC.ai[3] = 0;
                        NPC.localAI[0] = 2;
                        NPC.netUpdate = true;
                    }
                    break;

                case 2: //arena bomb
                    NPC.velocity *= 0.975f;
                    NPC.rotation = NPC.DirectionTo(player.Center).ToRotation();
                    NPC.direction = NPC.spriteDirection = NPC.Center.X < player.Center.X ? 1 : -1;

                    if (++NPC.ai[1] == 30) //spawn bomb
                    {
                        SoundEngine.PlaySound(SoundID.ForceRoarPitched, NPC.Center);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, NPC.DirectionTo(player.Center) * 12f, ModContent.ProjectileType<WillBomb>(), NPC.defDamage / 4, 0f, Main.myPlayer, 12f / 40f, NPC.whoAmI);
                        }
                    }
                    else if (NPC.ai[1] > 120)
                    {
                        NPC.ai[0] = 0;
                        NPC.ai[1] = 0;
                        NPC.netUpdate = true;
                    }
                    break;

                case 3: //spear barrage
                    {
                        Vector2 vel = player.Center - NPC.Center;
                        NPC.rotation = vel.ToRotation();

                        const float moveSpeed = 0.25f;

                        if (vel.X > 0) //im on left side of target
                        {
                            vel.X -= 450;
                            NPC.direction = NPC.spriteDirection = 1;
                        }
                        else //im on right side of target
                        {
                            vel.X += 450;
                            NPC.direction = NPC.spriteDirection = -1;
                        }
                        vel.Y -= 200f;
                        vel.Normalize();
                        vel *= 16f;
                        if (NPC.velocity.X < vel.X)
                        {
                            NPC.velocity.X += moveSpeed;
                            if (NPC.velocity.X < 0 && vel.X > 0)
                                NPC.velocity.X += moveSpeed;
                        }
                        else if (NPC.velocity.X > vel.X)
                        {
                            NPC.velocity.X -= moveSpeed;
                            if (NPC.velocity.X > 0 && vel.X < 0)
                                NPC.velocity.X -= moveSpeed;
                        }
                        if (NPC.velocity.Y < vel.Y)
                        {
                            NPC.velocity.Y += moveSpeed;
                            if (NPC.velocity.Y < 0 && vel.Y > 0)
                                NPC.velocity.Y += moveSpeed;
                        }
                        else if (NPC.velocity.Y > vel.Y)
                        {
                            NPC.velocity.Y -= moveSpeed;
                            if (NPC.velocity.Y > 0 && vel.Y < 0)
                                NPC.velocity.Y -= moveSpeed;
                        }

                        if (--NPC.localAI[0] < 0)
                        {
                            NPC.localAI[0] = NPC.localAI[2] == 1 ? 30 : 40;

                            if (NPC.ai[1] < 110 || NPC.localAI[3] == 1)
                            {
                                SoundEngine.PlaySound(SoundID.ForceRoarPitched, NPC.Center);

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    for (int i = 0; i < 15; i++)
                                    {
                                        const int time = 120;
                                        float speed = Main.rand.NextFloat(240, 720) / time * 2f;
                                        Vector2 velocity = speed * NPC.DirectionFrom(player.Center).RotatedByRandom(MathHelper.PiOver2);
                                        float ai1 = speed / time;
                                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, velocity, ModContent.ProjectileType<WillJavelin>(), NPC.defDamage / 4, 0f, Main.myPlayer, 0f, ai1);
                                    }
                                }
                            }
                        }

                        if (++NPC.ai[1] > 150)
                        {
                            NPC.ai[0] = 0;
                            NPC.ai[1] = 0;
                            NPC.localAI[0] = 0;
                            NPC.netUpdate = true;
                        }
                    }
                    break;

                case 4: //fireballs
                    {
                        Vector2 vel = player.Center - NPC.Center;
                        NPC.rotation = vel.ToRotation();

                        const float moveSpeed = 0.25f;

                        if (vel.X > 0) //im on left side of target
                        {
                            vel.X -= 550;
                            NPC.direction = NPC.spriteDirection = 1;
                        }
                        else //im on right side of target
                        {
                            vel.X += 550;
                            NPC.direction = NPC.spriteDirection = -1;
                        }
                        vel.Y -= 250f;
                        vel.Normalize();
                        vel *= 16f;
                        if (NPC.velocity.X < vel.X)
                        {
                            NPC.velocity.X += moveSpeed;
                            if (NPC.velocity.X < 0 && vel.X > 0)
                                NPC.velocity.X += moveSpeed;
                        }
                        else if (NPC.velocity.X > vel.X)
                        {
                            NPC.velocity.X -= moveSpeed;
                            if (NPC.velocity.X > 0 && vel.X < 0)
                                NPC.velocity.X -= moveSpeed;
                        }
                        if (NPC.velocity.Y < vel.Y)
                        {
                            NPC.velocity.Y += moveSpeed;
                            if (NPC.velocity.Y < 0 && vel.Y > 0)
                                NPC.velocity.Y += moveSpeed;
                        }
                        else if (NPC.velocity.Y > vel.Y)
                        {
                            NPC.velocity.Y -= moveSpeed;
                            if (NPC.velocity.Y > 0 && vel.Y < 0)
                                NPC.velocity.Y -= moveSpeed;
                        }

                        if (++NPC.localAI[0] > 3)
                        {
                            NPC.localAI[0] = 0;
                            if (Main.netMode != NetmodeID.MultiplayerClient && NPC.ai[1] < 90) //shoot fireball
                            {
                                SoundEngine.PlaySound(SoundID.Item34, NPC.Center);
                                Vector2 spawn = new Vector2(40, 50);
                                if (NPC.direction < 0)
                                {
                                    spawn.X *= -1;
                                    spawn = spawn.RotatedBy(Math.PI);
                                }
                                spawn = spawn.RotatedBy(NPC.rotation);
                                spawn += NPC.Center;
                                Vector2 projVel = NPC.DirectionTo(player.Center).RotatedBy((Main.rand.NextDouble() - 0.5) * Math.PI / 10);
                                projVel.Normalize();
                                projVel *= Main.rand.NextFloat(8f, 12f);
                                int type = ProjectileID.CultistBossFireBall;
                                if (Main.rand.NextBool())
                                {
                                    type = ModContent.ProjectileType<WillFireball>();
                                    projVel *= 2.5f;
                                }
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), spawn, projVel, type, NPC.defDamage / 4, 0f, Main.myPlayer);
                            }
                        }

                        if (--NPC.localAI[1] < 0)
                        {
                            NPC.localAI[1] = NPC.localAI[3] == 1 ? 35 : 180;

                            if (NPC.localAI[2] == 1 && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), new Vector2(player.Center.X, Math.Max(600f, player.Center.Y - 2000f)), Vector2.UnitY, ModContent.ProjectileType<WillDeathraySmall>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 4f / 3), 0f, Main.myPlayer, player.Center.X, NPC.whoAmI);
                            }
                        }

                        if (++NPC.ai[1] == 1)
                        {
                            SoundEngine.PlaySound(SoundID.ForceRoarPitched, NPC.Center);
                        }
                        else if (NPC.ai[1] > 120)
                        {
                            NPC.ai[0] = 0;
                            NPC.ai[1] = 0;
                            NPC.localAI[0] = 0;
                            NPC.netUpdate = true;
                        }
                    }
                    break;

                default:
                    NPC.ai[0] = 0;
                    goto case 0;
            }

            if (NPC.spriteDirection < 0 && NPC.ai[0] != -1f)
                NPC.rotation += (float)Math.PI;
        }

        private void Movement(Vector2 targetPos, float speedModifier, float cap = 12f)
        {
            if (NPC.Center.X < targetPos.X)
            {
                NPC.velocity.X += speedModifier;
                if (NPC.velocity.X < 0)
                    NPC.velocity.X += speedModifier * 2;
            }
            else
            {
                NPC.velocity.X -= speedModifier;
                if (NPC.velocity.X > 0)
                    NPC.velocity.X -= speedModifier * 2;
            }
            if (NPC.Center.Y < targetPos.Y)
            {
                NPC.velocity.Y += speedModifier;
                if (NPC.velocity.Y < 0)
                    NPC.velocity.Y += speedModifier * 2;
            }
            else
            {
                NPC.velocity.Y -= speedModifier;
                if (NPC.velocity.Y > 0)
                    NPC.velocity.Y -= speedModifier * 2;
            }
            if (Math.Abs(NPC.velocity.X) > cap)
                NPC.velocity.X = cap * Math.Sign(NPC.velocity.X);
            if (Math.Abs(NPC.velocity.Y) > cap)
                NPC.velocity.Y = cap * Math.Sign(NPC.velocity.Y);
        }

        public override void FindFrame(int frameHeight)
        {
            if (++NPC.frameCounter > 4)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
            }

            if (NPC.ai[0] == 0 || NPC.ai[0] == 2)
            {
                if (NPC.frame.Y >= 6 * frameHeight)
                    NPC.frame.Y = 0;
            }
            else
            {
                NPC.frame.Y = frameHeight * 7;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
            {
                target.AddBuff(ModContent.BuffType<Defenseless>(), 300);
                target.AddBuff(ModContent.BuffType<Midas>(), 300);
            }
            target.AddBuff(BuffID.Bleeding, 300);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int i = 1; i <= 4; i++)
                {
                    Vector2 pos = NPC.position + new Vector2(Main.rand.NextFloat(NPC.width), Main.rand.NextFloat(NPC.height));
                    if (!Main.dedServ)
                        Gore.NewGore(NPC.GetSource_FromThis(), pos, NPC.velocity, ModContent.Find<ModGore>(Mod.Name, $"WillGore{i}").Type, NPC.scale);
                }
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.SuperHealingPotion;
        }

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref FargoSoulsWorld.downedBoss[(int)FargoSoulsWorld.Downed.WillChampion], -1);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(new ChampionEnchDropRule(WillForce.Enchants));
            
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<WillChampionRelic>()));
            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<EnerGear>(), 4));

            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Dyes.WillDye>()));
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Npc[NPC.type].Value;
            Texture2D glowmask = FargowiltasSouls.Instance.Assets.Request<Texture2D>($"NPCs/Champions/{Name}_Glow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Texture2D glowmask2 = FargowiltasSouls.Instance.Assets.Request<Texture2D>($"NPCs/Champions/{Name}_Glow2", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            //int num156 = Terraria.GameContent.TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type]; //ypos of lower right corner of sprite to draw
            //int y3 = num156 * NPC.frame.Y; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = NPC.frame;//new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            SpriteEffects effects = NPC.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            /*for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
            {
                Color color27 = color26 * 0.2f;
                color27 *= (float)(NPCID.Sets.TrailCacheLength[NPC.type] - i) / NPCID.Sets.TrailCacheLength[NPC.type];
                Vector2 value4 = NPC.oldPos[i];
                float num165 = NPC.rotation; //NPC.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, NPC.scale, effects, 0);
            }*/

            Main.EntitySpriteDraw(texture2D13, NPC.Center - screenPos + new Vector2(0f, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), NPC.GetAlpha(drawColor), NPC.rotation, origin2, NPC.scale, effects, 0);
            Main.EntitySpriteDraw(glowmask, NPC.Center - screenPos + new Vector2(0f, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, NPC.rotation, origin2, NPC.scale, effects, 0);

            if (!NPC.IsABestiaryIconDummy)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            }
            ArmorShaderData shader = GameShaders.Armor.GetShaderFromItemId(ItemID.NebulaDye);
            shader.Apply(NPC, new Terraria.DataStructures.DrawData?());

            Color glowColor = Color.White * NPC.Opacity * 0.5f;

            for (float i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i += 0.25f)
            {
                Color color27 = glowColor * 0.25f;
                color27 *= (float)(NPCID.Sets.TrailCacheLength[NPC.type] - i) / NPCID.Sets.TrailCacheLength[NPC.type];
                float scale = NPC.scale;
                //scale *= (float)(NPCID.Sets.TrailCacheLength[NPC.type] - i) / NPCID.Sets.TrailCacheLength[NPC.type];
                int max0 = (int)i - 1;//Math.Max((int)i - 1, 0);
                if (max0 < 0)
                    continue;
                Vector2 value4 = NPC.oldPos[max0];
                float num165 = NPC.rotation; //NPC.oldRot[max0];
                Vector2 center = Vector2.Lerp(NPC.oldPos[(int)i], NPC.oldPos[max0], 1 - i % 1);
                center += NPC.Size / 2;
                Main.EntitySpriteDraw(glowmask2, center - screenPos + new Vector2(0, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, scale, effects, 0);
            }
            Main.EntitySpriteDraw(glowmask2, NPC.Center - screenPos + new Vector2(0f, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), glowColor, NPC.rotation, origin2, NPC.scale, effects, 0);

            if (!NPC.IsABestiaryIconDummy)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            }
            return false;
        }
    }
}
