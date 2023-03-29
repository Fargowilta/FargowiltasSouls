using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.ItemDropRules.Conditions;
using FargowiltasSouls.Items.Accessories.Forces;
using FargowiltasSouls.Items.Pets;
using FargowiltasSouls.Items.Placeables.Relics;
using FargowiltasSouls.Projectiles;
using FargowiltasSouls.Projectiles.Champions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.NPCs.Champions
{
    [AutoloadBossHead]
    public class TerraChampion : ModNPC
    {
        private bool spawned;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Champion of Terra");
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "泰拉英灵");

            NPCID.Sets.TrailCacheLength[NPC.type] = 5;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.NoMultiplayerSmoothingByType[NPC.type] = true;

            NPCID.Sets.BossBestiaryPriority.Add(NPC.type);
            NPCID.Sets.DebuffImmunitySets.Add(NPC.type, new Terraria.DataStructures.NPCDebuffImmunityData
            {
                ImmuneToAllBuffsThatAreNotWhips = true
            });

            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                CustomTexturePath = $"FargowiltasSouls/NPCs/Champions/{Name}_Still",
                Scale = 1.25f,
                Position = new Vector2(16 * 10.5f * 1.25f, 0),
                PortraitScale = 1.25f,
                PortraitPositionXOverride = 16 * 8 * 1.25f
            });
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,
                new FlavorTextBestiaryInfoElement($"Mods.FargowiltasSouls.Bestiary.{Name}")
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 80;
            NPC.height = 80;
            NPC.damage = 140;
            NPC.defense = 80;
            NPC.lifeMax = 170000;
            NPC.HitSound = SoundID.NPCHit41;
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

            NPC.behindTiles = true;
            NPC.trapImmune = true;

            NPC.scale *= 1.5f;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            //NPC.damage = (int)(NPC.damage * 0.5f);
            NPC.lifeMax = (int)(NPC.lifeMax * bossLifeScale);
        }

        public override bool CanHitPlayer(Player target, ref int CooldownSlot)
        {
            CooldownSlot = 1;
            return NPC.Distance(FargoSoulsUtil.ClosestPointInHitbox(target, NPC.Center)) < 30 * NPC.scale;
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
            NPC.ai[3] = 0;

            if (!spawned) //just spawned
            {
                spawned = true;
                NPC.TargetClosest(false);

                for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                    NPC.oldPos[i] = NPC.position;

                if (Main.netMode != NetmodeID.MultiplayerClient) //spawn segments
                {
                    int prev = NPC.whoAmI;
                    const int max = 99;
                    for (int i = 0; i < max; i++)
                    {
                        int type = i == max - 1 ? ModContent.NPCType<TerraChampionTail>() : ModContent.NPCType<TerraChampionBody>();
                        int n = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, type, NPC.whoAmI);
                        if (n != Main.maxNPCs)
                        {
                            Main.npc[n].ai[1] = prev;
                            Main.npc[n].ai[3] = NPC.whoAmI;
                            Main.npc[n].realLife = NPC.whoAmI;

                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);

                            prev = n;
                        }
                        else //can't spawn all segments
                        {
                            NPC.active = false;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, NPC.whoAmI);
                            return;
                        }
                    }
                }
            }

            EModeGlobalNPC.championBoss = NPC.whoAmI;

            Player player = Main.player[NPC.target];
            Vector2 targetPos;

            if (NPC.HasValidTarget && player.Center.Y >= Main.worldSurface * 16 && !player.ZoneUnderworldHeight)
                NPC.timeLeft = 600;

            if (FargoSoulsWorld.EternityMode && NPC.ai[1] != -1 && NPC.life < NPC.lifeMax / 10)
            {
                SoundEngine.PlaySound(SoundID.ForceRoarPitched, player.Center);
                NPC.life = NPC.lifeMax / 10;
                NPC.velocity = Vector2.Zero;
                NPC.ai[1] = -1f;
                NPC.localAI[0] = 0;
                NPC.localAI[1] = 0;
                NPC.localAI[2] = 0;
                NPC.localAI[3] = 0;
                NPC.netUpdate = true;
            }

            switch ((int)NPC.ai[1])
            {
                case -1: //flying head alone
                    if (!player.active || player.dead || player.Center.Y < Main.worldSurface * 16 || player.ZoneUnderworldHeight) //despawn code
                    {
                        NPC.TargetClosest(false);
                        if (NPC.timeLeft > 30)
                            NPC.timeLeft = 30;
                        NPC.velocity.Y += 1f;
                        break;
                    }

                    NPC.scale = 3f;
                    targetPos = player.Center;
                    if (NPC.Distance(targetPos) > 50)
                        Movement(targetPos, 0.16f, 32f);

                    NPC.rotation = NPC.DirectionTo(player.Center).ToRotation();

                    if (++NPC.localAI[0] > 50)
                    {
                        NPC.localAI[0] = 0;

                        if (NPC.localAI[1] > 120) //dont shoot while orb is exploding
                        {
                            SoundEngine.PlaySound(SoundID.Item12, NPC.Center);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Vector2 dir = NPC.DirectionTo(player.Center);
                                float ai1New = (Main.rand.NextBool()) ? 1 : -1; //randomize starting direction
                                Vector2 vel = Vector2.Normalize(dir) * 22f;
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel, ModContent.ProjectileType<HostileLightning>(),
                                    FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0, Main.myPlayer, dir.ToRotation(), ai1New);
                            }
                        }
                    }

                    if (--NPC.localAI[1] < 0)
                    {
                        NPC.localAI[1] = 420;

                        if (Main.netMode != NetmodeID.MultiplayerClient) //shoot orb
                        {
                            int p = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<TerraLightningOrb2>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, NPC.whoAmI);
                            Main.projectile[p].localAI[0] += 1f + Main.rand.NextFloatDirection(); //random starting rotation
                            Main.projectile[p].localAI[1] = (Main.rand.NextBool()) ? 1 : -1;
                            Main.projectile[p].netUpdate = true;
                        }
                    }
                    break;

                case 0: //ripped from destroyer
                    {
                        WormMovement(player, 17.22f, 0.122f, 0.188f);

                        if (++NPC.localAI[0] > 420)
                        {
                            NPC.ai[1]++;
                            NPC.localAI[0] = 0;
                        }
                    }

                    NPC.rotation = NPC.velocity.ToRotation();
                    break;

                case 1: //flee and prepare
                    NPC.ai[3] = 2;

                    if (++NPC.localAI[0] < 90)
                    {
                        targetPos = player.Center + NPC.DirectionFrom(player.Center) * 900;
                        Movement(targetPos, 0.4f, 18f);
                        if (NPC.Distance(targetPos) < 100)
                            NPC.localAI[0] = 90 - 1;
                    }
                    else if (NPC.localAI[0] == 90)
                    {
                        foreach (NPC segment in Main.npc.Where(n => n.active && n.realLife == NPC.whoAmI)) //mp sync
                        {
                            segment.netUpdate = true;
                        }

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<GlowRingHollow>(), 0, 0f, Main.myPlayer, 12f, NPC.whoAmI);
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<GlowRingHollow>(), 0, 0f, Main.myPlayer, 12f, NPC.whoAmI);
                        }
                    }
                    else
                    {
                        float rotationDifference = MathHelper.WrapAngle(NPC.velocity.ToRotation() - NPC.DirectionTo(player.Center).ToRotation());
                        bool inFrontOfMe = Math.Abs(rotationDifference) < MathHelper.ToRadians(90 / 2);

                        bool proceed = NPC.localAI[0] > 300 && (NPC.localAI[0] > 360 || inFrontOfMe);

                        if (proceed)
                        {
                            NPC.ai[1]++;
                            NPC.localAI[0] = 0;

                            NPC.velocity /= 4f;
                        }
                        else
                        {
                            NPC.velocity = Vector2.Normalize(NPC.velocity) * Math.Min(48f, NPC.velocity.Length() + 1f);
                            NPC.velocity += NPC.velocity.RotatedBy(MathHelper.PiOver2) * NPC.velocity.Length() / 300;
                        }
                    }

                    NPC.rotation = NPC.velocity.ToRotation();
                    break;

                case 2: //dash
                    {
                        if (NPC.localAI[1] == 0)
                        {
                            SoundEngine.PlaySound(SoundID.Roar, player.Center);
                            NPC.localAI[1] = 1;
                            NPC.velocity = NPC.DirectionTo(player.Center) * 24;
                        }

                        if (++NPC.localAI[2] > 2)
                        {
                            NPC.localAI[2] = 0;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Vector2 vel = NPC.DirectionTo(player.Center) * 12;
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel, ModContent.ProjectileType<TerraFireball>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer);

                                float offset = NPC.velocity.ToRotation() - vel.ToRotation();

                                vel = Vector2.Normalize(NPC.velocity).RotatedBy(offset) * 12;
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel, ModContent.ProjectileType<TerraFireball>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer);
                            }
                        }

                        double angle = NPC.DirectionTo(player.Center).ToRotation() - NPC.velocity.ToRotation();
                        while (angle > Math.PI)
                            angle -= 2.0 * Math.PI;
                        while (angle < -Math.PI)
                            angle += 2.0 * Math.PI;

                        if (++NPC.localAI[0] > 240 || (Math.Abs(angle) > Math.PI / 2 && NPC.Distance(player.Center) > 1200))
                        {
                            NPC.velocity = Vector2.Normalize(NPC.velocity).RotatedBy(Math.PI / 2) * 18f;
                            NPC.ai[1]++;
                            NPC.localAI[0] = 0;
                            NPC.localAI[1] = 0;
                        }

                        NPC.rotation = NPC.velocity.ToRotation();
                    }
                    break;

                case 3:
                    goto case 0;

                case 4: //reposition for sine
                    /*if (NPC.Distance(player.Center) < 1200)
                    {
                        targetPos = player.Center + NPC.DirectionFrom(player.Center) * 1200;
                        Movement(targetPos, 0.6f, 36f);
                    }
                    else //circle at distance to pull segments away
                    {
                        NPC.velocity = NPC.DirectionTo(player.Center).RotatedBy(Math.PI / 2) * 36;
                    }

                    if (++NPC.localAI[0] > 180)
                    {
                        NPC.ai[1]++;
                        NPC.localAI[0] = 0;
                    }

                    NPC.rotation = NPC.velocity.ToRotation();
                    break;*/
                    goto case 1;

                case 5: //sine wave dash
                    {
                        NPC.ai[3] = 1;

                        if (NPC.localAI[0] == 0)
                        {
                            NPC.localAI[1] = NPC.DirectionTo(player.Center).ToRotation();
                            SoundEngine.PlaySound(SoundID.Roar, player.Center);
                        }

                        const int end = 360;

                        /*Vector2 offset;
                        offset.X = 10f * NPC.localAI[0];
                        offset.Y = 600 * (float)Math.Sin(2f * Math.PI / end * 4 * NPC.localAI[0]);

                        NPC.Center = new Vector2(NPC.localAI[2], NPC.localAI[3]) + offset.RotatedBy(NPC.localAI[1]);
                        NPC.velocity = Vector2.Zero;
                        NPC.rotation = (NPC.position - NPC.oldPosition).ToRotation();*/

                        float sinModifier = (float)Math.Sin(2 * (float)Math.PI * (NPC.localAI[0] / end * 3 + 0.25f));
                        NPC.rotation = NPC.localAI[1] + (float)Math.PI / 2 * sinModifier;
                        NPC.velocity = 36f * NPC.rotation.ToRotationVector2();

                        if (Math.Abs(sinModifier) < 0.001f) //account for rounding issues
                        {
                            SoundEngine.PlaySound(SoundID.Item12, NPC.Center);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                for (int j = -5; j <= 5; j++)
                                {
                                    float rotationOffset = (float)Math.PI / 2 + (float)Math.PI / 2 / 5 * j;
                                    rotationOffset *= Math.Sign(-sinModifier);
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center,
                                        6f * Vector2.UnitX.RotatedBy(NPC.localAI[1] + rotationOffset),
                                        ProjectileID.CultistBossFireBall, FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer);
                                }

                                for (int i = -5; i <= 5; i++)
                                {
                                    float rotationOffset = (float)Math.PI / 2 + (float)Math.PI / 2 / 4.5f * i;
                                    rotationOffset *= Math.Sign(-sinModifier);
                                    Vector2 vel2 = Vector2.UnitX.RotatedBy(Math.PI / 4 * (Main.rand.NextDouble() - 0.5)) * 36f;
                                    float ai1New = (Main.rand.NextBool()) ? 1 : -1; //randomize starting direction
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel2.RotatedBy(NPC.localAI[1] + rotationOffset), ModContent.ProjectileType<HostileLightning>(),
                                        FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0, Main.myPlayer, NPC.localAI[1] + rotationOffset, ai1New);
                                }
                            }
                        }

                        if (++NPC.localAI[0] > end * 0.8f)
                        {
                            NPC.ai[1]++;
                            NPC.localAI[0] = 0;
                            NPC.localAI[1] = 0;
                            NPC.localAI[2] = 0;
                            NPC.localAI[3] = 0;
                            NPC.velocity = NPC.DirectionTo(player.Center) * NPC.velocity.Length();
                        }
                    }
                    break;

                case 6:
                    goto case 0;

                case 7:
                    goto case 1;

                case 8: //dash but u-turn
                    if (NPC.localAI[1] == 0)
                    {
                        SoundEngine.PlaySound(SoundID.Roar, player.Center);
                        NPC.localAI[1] = 1;
                        NPC.velocity = NPC.DirectionTo(player.Center) * 36;
                    }

                    if (NPC.localAI[3] == 0)
                    {
                        double angle = NPC.DirectionTo(player.Center).ToRotation() - NPC.velocity.ToRotation();
                        while (angle > Math.PI)
                            angle -= 2.0 * Math.PI;
                        while (angle < -Math.PI)
                            angle += 2.0 * Math.PI;

                        if (Math.Abs(angle) > Math.PI / 2) //passed player, turn around
                        {
                            NPC.localAI[3] = Math.Sign(angle);
                            NPC.velocity = Vector2.Normalize(NPC.velocity) * 24;
                        }
                    }
                    else //turning
                    {
                        NPC.velocity = NPC.velocity.RotatedBy(MathHelper.ToRadians(2.5f) * NPC.localAI[3]);

                        if (++NPC.localAI[2] > 2)
                        {
                            NPC.localAI[2] = 0;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Vector2 vel = 12f * Vector2.Normalize(NPC.velocity).RotatedBy(Math.PI / 2);
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel, ModContent.ProjectileType<TerraFireball>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer);
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, -vel, ModContent.ProjectileType<TerraFireball>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer);
                            }
                        }

                        if (++NPC.localAI[0] > 75)
                        {
                            NPC.ai[1]++;
                            NPC.localAI[0] = 0;
                            NPC.localAI[1] = 0;
                        }
                    }

                    NPC.rotation = NPC.velocity.ToRotation();
                    break;

                case 9:
                    goto case 0;

                case 10:
                    goto case 1;

                case 11: //prepare for coil
                    NPC.ai[3] = 2;
                    targetPos = player.Center + NPC.DirectionFrom(player.Center) * 600;
                    Movement(targetPos, 0.4f, 32f);
                    if (++NPC.localAI[0] > 300 || NPC.Distance(targetPos) < 50f)
                    {
                        NPC.ai[1]++;
                        NPC.localAI[0] = 0;
                        NPC.localAI[1] = NPC.Distance(player.Center);
                        NPC.velocity = 32f * NPC.DirectionTo(player.Center).RotatedBy(Math.PI / 2);
                        SoundEngine.PlaySound(SoundID.Roar, player.Center);
                    }
                    NPC.rotation = NPC.velocity.ToRotation();
                    break;

                case 12: //coiling
                    {
                        NPC.ai[3] = 2;

                        Vector2 acceleration = Vector2.Normalize(NPC.velocity).RotatedBy(-Math.PI / 2) * 32f * 32f / 600f;
                        NPC.velocity = Vector2.Normalize(NPC.velocity) * 32f + acceleration;

                        NPC.rotation = NPC.velocity.ToRotation();

                        Vector2 pivot = NPC.Center;
                        pivot += Vector2.Normalize(NPC.velocity.RotatedBy(-Math.PI / 2)) * 600;

                        Player target = Main.player[NPC.target];
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

                        if (NPC.localAI[0] == 0 && Main.netMode != NetmodeID.MultiplayerClient) //shoot orb
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<TerraLightningOrb2>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, NPC.whoAmI);
                        }

                        if (++NPC.localAI[0] > 420)
                        {
                            NPC.ai[1]++;
                            NPC.localAI[0] = 0;
                            NPC.localAI[1] = 0;
                        }
                    }
                    break;

                case 13: //reset to get rid of troublesome coil
                    goto case 1;

                default:
                    NPC.ai[1] = 0;
                    goto case 0;
            }

            NPC.netUpdate = true;

            Vector2 dustOffset = new Vector2(77, -41) * NPC.scale; //dust from horns
            int dust = Dust.NewDust(NPC.Center + NPC.velocity - dustOffset.RotatedBy(NPC.rotation), 0, 0, DustID.Torch, NPC.velocity.X * .4f, NPC.velocity.Y * 0.4f, 0, default(Color), 2f);
            Main.dust[dust].velocity *= 2;
            if (Main.rand.NextBool())
            {
                Main.dust[dust].scale++;
                Main.dust[dust].noGravity = true;
            }

            dustOffset.Y *= -1f;
            dust = Dust.NewDust(NPC.Center + NPC.velocity - dustOffset.RotatedBy(NPC.rotation), 0, 0, DustID.Torch, NPC.velocity.X * .4f, NPC.velocity.Y * 0.4f, 0, default(Color), 2f);
            Main.dust[dust].velocity *= 2;
            if (Main.rand.NextBool())
            {
                Main.dust[dust].scale++;
                Main.dust[dust].noGravity = true;
            }

            if (NPC.ai[1] != -1 && Collision.SolidCollision(NPC.position, NPC.width, NPC.height) && NPC.soundDelay == 0)
            {
                NPC.soundDelay = (int)(NPC.Distance(player.Center) / 40f);
                if (NPC.soundDelay < 10)
                    NPC.soundDelay = 10;
                if (NPC.soundDelay > 20)
                    NPC.soundDelay = 20;
                SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
            }

            int pastPos = NPCID.Sets.TrailCacheLength[NPC.type] - (int)NPC.ai[3] - 1; //ai3 check is to trace better and coil tightly
            Vector2 myPosAfterVelocity = NPC.position + NPC.velocity;
            if ((myPosAfterVelocity - NPC.oldPos[pastPos - 1]).Length() > 45 * NPC.scale / 1.5f * 1.25f)
            {
                NPC.oldPos[pastPos - 1] = myPosAfterVelocity + Vector2.Normalize(NPC.oldPos[pastPos - 1] - myPosAfterVelocity) * 45 * NPC.scale / 1.5f * 1.25f;
            }
        }

        private void WormMovement(Player player, float maxSpeed, float turnSpeed, float accel)
        {
            if (!player.active || player.dead || player.Center.Y < Main.worldSurface * 16 || player.ZoneUnderworldHeight) //despawn code
            {
                NPC.TargetClosest(false);
                if (NPC.timeLeft > 30)
                    NPC.timeLeft = 30;
                NPC.velocity.Y += 1f;
                NPC.rotation = NPC.velocity.ToRotation();
                return;
            }

            float comparisonSpeed = player.velocity.Length() * 1.5f;
            float rotationDifference = MathHelper.WrapAngle(NPC.velocity.ToRotation() - NPC.DirectionTo(player.Center).ToRotation());
            bool inFrontOfMe = Math.Abs(rotationDifference) < MathHelper.ToRadians(90 / 2);
            if (maxSpeed < comparisonSpeed && inFrontOfMe) //player is moving faster than my top speed
            {
                maxSpeed = comparisonSpeed; //outspeed them
            }

            if (NPC.Distance(player.Center) > 1200f) //better turning when out of range
            {
                turnSpeed *= 2f;
                accel *= 2f;

                if (inFrontOfMe && maxSpeed < 30f) //much higher top speed to return to the fight
                    maxSpeed = 30f;
            }

            if (NPC.velocity.Length() > maxSpeed) //decelerate if over top speed
                NPC.velocity *= 0.975f;

            Vector2 target = player.Center;
            float num17 = target.X;
            float num18 = target.Y;

            float num21 = num17 - NPC.Center.X;
            float num22 = num18 - NPC.Center.Y;
            float num23 = (float)Math.Sqrt((double)num21 * (double)num21 + (double)num22 * (double)num22);

            //ground movement code but it always runs
            float num2 = (float)Math.Sqrt(num21 * num21 + num22 * num22);
            float num3 = Math.Abs(num21);
            float num4 = Math.Abs(num22);
            float num5 = maxSpeed / num2;
            float num6 = num21 * num5;
            float num7 = num22 * num5;
            if ((NPC.velocity.X > 0f && num6 > 0f || NPC.velocity.X < 0f && num6 < 0f) && (NPC.velocity.Y > 0f && num7 > 0f || NPC.velocity.Y < 0f && num7 < 0f))
            {
                if (NPC.velocity.X < num6)
                    NPC.velocity.X += accel;
                else if (NPC.velocity.X > num6)
                    NPC.velocity.X -= accel;
                if (NPC.velocity.Y < num7)
                    NPC.velocity.Y += accel;
                else if (NPC.velocity.Y > num7)
                    NPC.velocity.Y -= accel;
            }
            if (NPC.velocity.X > 0f && num6 > 0f || NPC.velocity.X < 0f && num6 < 0f || NPC.velocity.Y > 0f && num7 > 0f || NPC.velocity.Y < 0f && num7 < 0f)
            {
                if (NPC.velocity.X < num6)
                    NPC.velocity.X += turnSpeed;
                else if (NPC.velocity.X > num6)
                    NPC.velocity.X -= turnSpeed;
                if (NPC.velocity.Y < num7)
                    NPC.velocity.Y += turnSpeed;
                else if (NPC.velocity.Y > num7)
                    NPC.velocity.Y -= turnSpeed;

                if (Math.Abs(num7) < maxSpeed * 0.2f && (NPC.velocity.X > 0f && num6 < 0f || NPC.velocity.X < 0f && num6 > 0f))
                {
                    if (NPC.velocity.Y > 0f)
                        NPC.velocity.Y += turnSpeed * 2f;
                    else
                        NPC.velocity.Y -= turnSpeed * 2f;
                }
                if (Math.Abs(num6) < maxSpeed * 0.2f && (NPC.velocity.Y > 0f && num7 < 0f || NPC.velocity.Y < 0f && num7 > 0f))
                {
                    if (NPC.velocity.X > 0f)
                        NPC.velocity.X += turnSpeed * 2f;
                    else
                        NPC.velocity.X -= turnSpeed * 2f;
                }
            }
            else if (num3 > num4)
            {
                if (NPC.velocity.X < num6)
                    NPC.velocity.X += turnSpeed * 1.1f;
                else if (NPC.velocity.X > num6)
                    NPC.velocity.X -= turnSpeed * 1.1f;

                if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < maxSpeed * 0.5f)
                {
                    if (NPC.velocity.Y > 0f)
                        NPC.velocity.Y += turnSpeed;
                    else
                        NPC.velocity.Y -= turnSpeed;
                }
            }
            else
            {
                if (NPC.velocity.Y < num7)
                    NPC.velocity.Y += turnSpeed * 1.1f;
                else if (NPC.velocity.Y > num7)
                    NPC.velocity.Y -= turnSpeed * 1.1f;

                if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < maxSpeed * 0.5f)
                {
                    if (NPC.velocity.X > 0f)
                        NPC.velocity.X += turnSpeed;
                    else
                        NPC.velocity.X -= turnSpeed;
                }
            }
        }

        private void Movement(Vector2 targetPos, float speedModifier, float cap = 12f, bool fastY = false)
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
                NPC.velocity.Y += fastY ? speedModifier * 2 : speedModifier;
                if (NPC.velocity.Y < 0)
                    NPC.velocity.Y += speedModifier * 2;
            }
            else
            {
                NPC.velocity.Y -= fastY ? speedModifier * 2 : speedModifier;
                if (NPC.velocity.Y > 0)
                    NPC.velocity.Y -= speedModifier * 2;
            }
            if (Math.Abs(NPC.velocity.X) > cap)
                NPC.velocity.X = cap * Math.Sign(NPC.velocity.X);
            if (Math.Abs(NPC.velocity.Y) > cap)
                NPC.velocity.Y = cap * Math.Sign(NPC.velocity.Y);
        }

        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            //if (NPC.ai[3] == 1) damage /= 10;
            if (NPC.life < NPC.lifeMax / 10) damage /= 3;
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
            if (NPC.life <= 0)
            {
                for (int i = 1; i <= 3; i++)
                {
                    Vector2 pos = NPC.position + new Vector2(Main.rand.NextFloat(NPC.width), Main.rand.NextFloat(NPC.height));
                    if (!Main.dedServ)
                        Gore.NewGore(NPC.GetSource_FromThis(), pos, NPC.velocity, ModContent.Find<ModGore>(Mod.Name, $"TerraGore{i}").Type, NPC.scale);
                }
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.SuperHealingPotion;
        }

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref FargoSoulsWorld.downedBoss[(int)FargoSoulsWorld.Downed.TerraChampion], -1);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(new ChampionEnchDropRule(TerraForce.Enchants));
            
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<TerraChampionRelic>()));
            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<MostlyOrdinaryRock>(), 4));
        }

        public override void BossHeadRotation(ref float rotation)
        {
            rotation = NPC.rotation;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Npc[NPC.type].Value;
            Rectangle rectangle = NPC.frame;//new Rectangle(0, y3, texture2D13.Width, turnSpeed6);
            Vector2 origin2 = rectangle.Size() / 2f;
            SpriteEffects effects = NPC.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(texture2D13, NPC.Center - screenPos + new Vector2(0f, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), NPC.GetAlpha(drawColor), NPC.rotation, origin2, NPC.scale, effects, 0);
            Texture2D glowmask = FargowiltasSouls.Instance.Assets.Request<Texture2D>("NPCs/Champions/TerraChampion_Glow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Main.EntitySpriteDraw(glowmask, NPC.Center - screenPos + new Vector2(0f, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, NPC.rotation, origin2, NPC.scale, effects, 0);
            return false;
        }
    }
}
