using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Items.Accessories.Enchantments;

namespace FargowiltasSouls.NPCs.Champions
{
    [AutoloadBossHead]
    public class NatureChampion : ModNPC
    {
        /* order of heads:
         * 0 = crimson
         * 1 = molten
         * 2 = rain
         * 3 = frost
         * 4 = chloro
         * 5 = shroomite
         */
        public int[] heads = { -1, -1, -1, -1, -1, -1 };
        public int lastSet = 0;
        public static readonly KeyValuePair<int, int>[] configurations = {
            new KeyValuePair<int, int>(0, 1),
            new KeyValuePair<int, int>(1, 3),
            new KeyValuePair<int, int>(3, 5),
            new KeyValuePair<int, int>(3, 4),
            new KeyValuePair<int, int>(2, 4),
            new KeyValuePair<int, int>(0, 5),
            new KeyValuePair<int, int>(1, 2),
            new KeyValuePair<int, int>(2, 5),
            new KeyValuePair<int, int>(0, 4)
        };

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Champion of Nature");
            DisplayName.AddTranslation(GameCulture.Chinese, "自然英灵");
            Main.npcFrameCount[npc.type] = 14;
        }

        public override void SetDefaults()
        {
            npc.width = 180;
            npc.height = 120;
            npc.damage = 110;
            npc.defense = 100;
            npc.lifeMax = 600000;
            npc.HitSound = SoundID.NPCHit6;
            npc.DeathSound = SoundID.NPCDeath1;
            //npc.noGravity = true;
            //npc.noTileCollide = true;
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
            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            for (int i = 0; i < heads.Length; i++)
                writer.Write(heads[i]);
            writer.Write(lastSet);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            for (int i = 0; i < heads.Length; i++)
                heads[i] = reader.ReadInt32();
            lastSet = reader.ReadInt32();
        }

        public override void AI()
        {
            if (npc.localAI[3] == 0) //spawn friends
            {
                npc.TargetClosest(false);
                npc.localAI[3] = 1;
                /*if (npc.Distance(Main.player[npc.target].Center) < 1500)
                {
                    npc.localAI[3] = 1;
                }
                else
                {
                    Movement(Main.player[npc.target].Center, 0.8f, 32f);
                    return;
                }*/

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<NatureChampionHead>(), npc.whoAmI, 0f, npc.whoAmI, 0f, -3f, npc.target);
                    if (n != Main.maxNPCs)
                    {
                        heads[0] = n;
                        Main.npc[n].velocity.X = Main.rand.NextFloat(-24f, 24f);
                        Main.npc[n].velocity.Y = Main.rand.NextFloat(-24f, 24f);
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                    }
                    n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<NatureChampionHead>(), npc.whoAmI, 0f, npc.whoAmI, 0f, -2f, npc.target);
                    if (n != Main.maxNPCs)
                    {
                        heads[1] = n;
                        Main.npc[n].velocity.X = Main.rand.NextFloat(-24f, 24f);
                        Main.npc[n].velocity.Y = Main.rand.NextFloat(-24f, 24f);
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                    }
                    n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<NatureChampionHead>(), npc.whoAmI, 0f, npc.whoAmI, 0f, -1f, npc.target);
                    if (n != Main.maxNPCs)
                    {
                        heads[2] = n;
                        Main.npc[n].velocity.X = Main.rand.NextFloat(-24f, 24f);
                        Main.npc[n].velocity.Y = Main.rand.NextFloat(-24f, 24f);
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                    }
                    n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<NatureChampionHead>(), npc.whoAmI, 0f, npc.whoAmI, 0f, 1f, npc.target);
                    if (n != Main.maxNPCs)
                    {
                        heads[3] = n;
                        Main.npc[n].velocity.X = Main.rand.NextFloat(-24f, 24f);
                        Main.npc[n].velocity.Y = Main.rand.NextFloat(-24f, 24f);
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                    }
                    n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<NatureChampionHead>(), npc.whoAmI, 0f, npc.whoAmI, 0f, 2f, npc.target);
                    if (n != Main.maxNPCs)
                    {
                        heads[4] = n;
                        Main.npc[n].velocity.X = Main.rand.NextFloat(-24f, 24f);
                        Main.npc[n].velocity.Y = Main.rand.NextFloat(-24f, 24f);
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                    }
                    n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<NatureChampionHead>(), npc.whoAmI, 0f, npc.whoAmI, 0f, 3f, npc.target);
                    if (n != Main.maxNPCs)
                    {
                        heads[5] = n;
                        Main.npc[n].velocity.X = Main.rand.NextFloat(-24f, 24f);
                        Main.npc[n].velocity.Y = Main.rand.NextFloat(-24f, 24f);
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                    }

                    for (int i = 0; i < heads.Length; i++) //failsafe, die if couldnt spawn heads
                    {
                        if (heads[i] == -1 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            npc.active = false;
                            return;
                        }
                    }
                }
            }

            EModeGlobalNPC.championBoss = npc.whoAmI;

            Player player = Main.player[npc.target];
            Vector2 targetPos;
            
            if (npc.HasValidTarget && npc.Distance(player.Center) < 3000 && player.Center.Y >= Main.worldSurface * 16 && !player.ZoneUnderworldHeight)
                npc.timeLeft = 600;

            if (player.Center.X < npc.position.X)
                npc.direction = npc.spriteDirection = -1;
            else if (player.Center.X > npc.position.X + npc.width)
                npc.direction = npc.spriteDirection = 1;
            
            switch ((int)npc.ai[0])
            {
                case -1: //mourning wood movement
                    {
                        npc.noTileCollide = true;
                        npc.noGravity = true;

                        if (npc.position.X < player.Center.X && player.Center.X < npc.position.X + npc.width)
                        {
                            npc.velocity.X *= 0.92f;
                            if (Math.Abs(npc.velocity.X) < 0.1f)
                                npc.velocity.X = 0f;
                        }
                        else
                        {
                            float accel = 2f;
                            /*if (Math.Abs(player.Center.X - npc.Center.X) > 1200) //secretly fast run
                            {
                                accel = 24f;
                            }
                            else
                            {
                                if (Math.Abs(npc.velocity.X) > 2)
                                    npc.velocity.X *= 0.97f;
                            }*/
                            if (player.Center.X > npc.Center.X)
                                npc.velocity.X = (npc.velocity.X * 20 + accel) / 21;
                            else
                                npc.velocity.X = (npc.velocity.X * 20 - accel) / 21;
                        }

                        bool onPlatforms = false;
                        for (int i = (int)npc.position.X; i <= npc.position.X + npc.width; i += 16)
                        {
                            if (Framing.GetTileSafely(new Vector2(i, npc.position.Y + npc.height + npc.velocity.Y + 1)).type == TileID.Platforms)
                            {
                                onPlatforms = true;
                                break;
                            }
                        }

                        bool onCollision = Collision.SolidCollision(npc.position, npc.width, npc.height);

                        if (npc.position.X < player.position.X && npc.position.X + npc.width > player.position.X + player.width
                            && npc.position.Y + npc.height < player.position.Y + player.height - 16)
                        {
                            npc.velocity.Y += 0.5f;
                        }
                        else if (onCollision || (onPlatforms && player.position.Y + player.height <= npc.position.Y + npc.height))
                        {
                            if (npc.velocity.Y > 0f)
                                npc.velocity.Y = 0f;

                            if (onCollision)
                            {
                                if (npc.velocity.Y > -0.2f)
                                    npc.velocity.Y -= 0.025f;
                                else
                                    npc.velocity.Y -= 0.2f;

                                if (npc.velocity.Y < -4f)
                                    npc.velocity.Y = -4f;
                            }
                        }
                        else
                        {
                            if (npc.velocity.Y < 0f)
                                npc.velocity.Y = 0f;

                            if (npc.velocity.Y < 0.1f)
                                npc.velocity.Y += 0.025f;
                            else
                                npc.velocity.Y += 0.5f;
                        }

                        if (npc.velocity.Y > 10f)
                            npc.velocity.Y = 10f;
                    }
                    break;

                case 0: //think
                    npc.noTileCollide = false;
                    npc.noGravity = false;

                    if (++npc.ai[1] > 45)
                    {
                        npc.TargetClosest();
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;
                        npc.netUpdate = true;
                    }
                    goto case -1;

                case 1: //stomp
                    {
                        void StompDust()
                        {
                            Main.PlaySound(SoundID.Item, npc.Center, 14);

                            for (int k = -2; k <= 2; k++) //explosions
                            {
                                Vector2 dustPos = npc.Center;
                                int width = npc.width / 5;
                                dustPos.X += width * k + Main.rand.NextFloat(-width, width);
                                dustPos.Y += Main.rand.NextFloat(npc.height / 2);

                                for (int i = 0; i < 30; i++)
                                {
                                    int dust = Dust.NewDust(dustPos, 32, 32, 31, 0f, 0f, 100, default(Color), 3f);
                                    Main.dust[dust].velocity *= 1.4f;
                                }

                                for (int i = 0; i < 20; i++)
                                {
                                    int dust = Dust.NewDust(dustPos, 32, 32, 6, 0f, 0f, 100, default(Color), 3.5f);
                                    Main.dust[dust].noGravity = true;
                                    Main.dust[dust].velocity *= 7f;
                                    dust = Dust.NewDust(dustPos, 32, 32, 6, 0f, 0f, 100, default(Color), 1.5f);
                                    Main.dust[dust].velocity *= 3f;
                                }

                                float scaleFactor9 = 0.5f;
                                for (int j = 0; j < 4; j++)
                                {
                                    int gore = Gore.NewGore(dustPos, default(Vector2), Main.rand.Next(61, 64));
                                    Main.gore[gore].velocity *= scaleFactor9;
                                    Main.gore[gore].velocity.X += 1f;
                                    Main.gore[gore].velocity.Y += 1f;
                                }
                            }
                        }

                        int jumpTime = 60;
                        if (npc.ai[3] == 1)
                            jumpTime = 30;

                        npc.noGravity = true;
                        npc.noTileCollide = true;

                        if (npc.ai[2] == 0) //move over player
                        {
                            StompDust();

                            npc.ai[2] = 1;
                            npc.netUpdate = true;

                            targetPos = player.Center;
                            targetPos.Y -= npc.ai[3] == 1 ? 300 : 600;

                            npc.velocity = (targetPos - npc.Center) / jumpTime;
                        }

                        if (++npc.ai[1] > jumpTime + (npc.ai[3] == 1 ? 1 : 18)) //do the stomp
                        {
                            npc.noGravity = false;
                            npc.noTileCollide = false;

                            if (npc.velocity.Y == 0 || npc.ai[3] == 1) //landed, now stomp
                            {
                                StompDust();

                                npc.TargetClosest();
                                npc.ai[0]++;
                                npc.ai[1] = npc.ai[3] == 1 ? 40 : 0;
                                npc.ai[2] = 0;
                                npc.ai[3] = 0;
                                npc.netUpdate = true;
                            }
                        }
                        else if (npc.ai[1] > jumpTime) //falling
                        {
                            if (npc.velocity.X > 2)
                                npc.velocity.X = 2;
                            if (npc.velocity.X < -2)
                                npc.velocity.X = -2;
                            npc.velocity.Y = 30f;
                        }
                    }
                    break;

                case 2:
                    if (!player.active || player.dead || Vector2.Distance(npc.Center, player.Center) > 3000f
                        || player.Center.Y < Main.worldSurface * 16 || player.ZoneUnderworldHeight) //despawn code
                    {
                        npc.TargetClosest(false);
                        if (npc.timeLeft > 30)
                            npc.timeLeft = 30;

                        npc.noTileCollide = true;
                        npc.noGravity = true;
                        npc.velocity.Y += 1f;

                        break;
                    }
                    goto case 0;

                case 3: //decide an attack
                    if (npc.ai[2] == 0)
                    {
                        void ActivateHead(int targetHead)
                        {
                            Main.npc[targetHead].ai[0] += Main.npc[targetHead].ai[3];
                            Main.npc[targetHead].localAI[0] = 0;
                            Main.npc[targetHead].ai[2] = 0;
                            Main.npc[targetHead].localAI[1] = 0;
                            Main.npc[targetHead].netUpdate = true;

                            Main.PlaySound(SoundID.ForceRoar, Main.npc[targetHead].Center, -1);

                            int dustType;
                            switch ((int)Main.npc[targetHead].ai[3])
                            {
                                case -3: dustType = 183; break;
                                case -2: dustType = 6; break;
                                case -1: dustType = 87; break;
                                case 1: dustType = 111; break;
                                case 2: dustType = 89; break;
                                case 3: dustType = 113; break;
                                default: dustType = 1; break;
                            }

                            const int num226 = 70;
                            for (int num227 = 0; num227 < num226; num227++)
                            {
                                Vector2 vector6 = Vector2.UnitX * 30f;
                                vector6 = vector6.RotatedBy(((num227 - (num226 / 2 - 1)) * 6.28318548f / num226), default(Vector2)) + Main.npc[targetHead].Center;
                                Vector2 vector7 = vector6 - Main.npc[targetHead].Center;
                                int num228 = Dust.NewDust(vector6 + vector7, 0, 0, dustType, 0f, 0f, 0, default(Color), 3f);
                                Main.dust[num228].noGravity = true;
                                Main.dust[num228].velocity = vector7;
                            }
                        };

                        npc.ai[2] = 1;
                        npc.netUpdate = true;
                        
                        int set = Main.rand.Next(configurations.Length);
                        while (heads[configurations[set].Key] == heads[configurations[lastSet].Key] //don't reuse heads you just attacked with
                            || heads[configurations[set].Key] == heads[configurations[lastSet].Value]
                            || heads[configurations[set].Value] == heads[configurations[lastSet].Key]
                            || heads[configurations[set].Value] == heads[configurations[lastSet].Value])
                            set = Main.rand.Next(configurations.Length);
                        lastSet = set;

                        if (Main.expertMode) //activate both in expert
                        {
                            ActivateHead(heads[configurations[set].Key]);
                            ActivateHead(heads[configurations[set].Value]);
                        }
                        else //only activate one in normal
                        {
                            if (Main.rand.Next(2) == 0)
                                ActivateHead(heads[configurations[set].Key]);
                            else
                                ActivateHead(heads[configurations[set].Value]);
                        }
                    }

                    if (++npc.ai[1] > 300) //wait
                    {
                        npc.TargetClosest();
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;
                        npc.netUpdate = true;
                    }
                    goto case -1;

                case 4:
                    goto case 2;

                case 5:
                    goto case 3;

                case 6:
                    goto case 2;

                case 7:
                    goto case 3;

                case 8:
                    goto case 2;

                case 9:
                    goto case 1;

                case 10:
                    goto case 2;

                case 11: //deathrays
                    if (npc.ai[2] == 0 && FargoSoulsWorld.MasochistMode)
                    {
                        npc.ai[2] = 1;

                        Main.PlaySound(SoundID.Roar, npc.Center, 0);

                        for (int i = 0; i < heads.Length; i++) //activate all heads
                        {
                            Main.npc[heads[i]].ai[0] = 4f;
                            Main.npc[heads[i]].localAI[0] = 0;
                            Main.npc[heads[i]].ai[2] = 0;
                            Main.npc[heads[i]].localAI[1] = 0;
                            Main.npc[heads[i]].netUpdate = true;

                            int dustType;
                            switch ((int)Main.npc[heads[i]].ai[3])
                            {
                                case -3: dustType = 183; break;
                                case -2: dustType = 6; break;
                                case -1: dustType = 87; break;
                                case 1: dustType = 111; break;
                                case 2: dustType = 89; break;
                                case 3: dustType = 113; break;
                                default: dustType = 1; break;
                            }

                            const int num226 = 70;
                            for (int num227 = 0; num227 < num226; num227++)
                            {
                                Vector2 vector6 = Vector2.UnitX * 30f;
                                vector6 = vector6.RotatedBy(((num227 - (num226 / 2 - 1)) * 6.28318548f / num226), default(Vector2)) + Main.npc[heads[i]].Center;
                                Vector2 vector7 = vector6 - Main.npc[heads[i]].Center;
                                int num228 = Dust.NewDust(vector6 + vector7, 0, 0, dustType, 0f, 0f, 0, default(Color), 3f);
                                Main.dust[num228].noGravity = true;
                                Main.dust[num228].velocity = vector7;
                            }
                        }
                    }

                    if (++npc.ai[1] > 330 || !FargoSoulsWorld.MasochistMode) //wait
                    {
                        npc.TargetClosest();
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;
                        npc.netUpdate = true;
                    }
                    goto case -1;

                default:
                    npc.ai[0] = 0;
                    goto case 0;
            }

            if (FargoSoulsWorld.MasochistMode)
            {
                if (npc.HasValidTarget && npc.Distance(player.Center) > 1400 && Vector2.Distance(npc.Center, player.Center) < 3000f
                  && player.Center.Y > Main.worldSurface * 16 && !player.ZoneUnderworldHeight && npc.ai[0] > 1 && npc.ai[0] != 9) //enrage
                {
                    npc.ai[0] = 1;
                    npc.ai[1] = 0;
                    npc.ai[2] = 0;
                    npc.ai[3] = 1; //marks enrage jump
                    npc.netUpdate = true;
                }

                Vector2 dustOffset = Vector2.Normalize(player.Center - npc.Center) * 1400;
                for (int i = 0; i < 20; i++) //dust ring for enrage range
                {
                    int d = Dust.NewDust(npc.Center + dustOffset.RotatedByRandom(2 * Math.PI), 0, 0, 59, Scale: 2f);
                    Main.dust[d].velocity = npc.velocity;
                    Main.dust[d].noGravity = true;
                }
            }
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
            if (npc.velocity == Vector2.Zero)
            {
                if (npc.frame.Y < frameHeight * 8)
                    npc.frame.Y = frameHeight * 8;

                if (++npc.frameCounter > 5)
                {
                    npc.frameCounter = 0;
                    npc.frame.Y += frameHeight;
                }

                if (npc.frame.Y >= Main.npcFrameCount[npc.type] * frameHeight)
                    npc.frame.Y = frameHeight * 8;
            }
            else
            {
                if (++npc.frameCounter > 5)
                {
                    npc.frameCounter = 0;
                    npc.frame.Y += frameHeight;
                }

                if (npc.frame.Y >= frameHeight * 7)
                    npc.frame.Y = 0;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.MasochistMode)
            {
                target.AddBuff(BuffID.Frostburn, 300);
                target.AddBuff(BuffID.OnFire, 300);
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                for (int i = 1; i <= 6; i++)
                {
                    Vector2 pos = npc.position + new Vector2(Main.rand.NextFloat(npc.width), Main.rand.NextFloat(npc.height));
                    Gore.NewGore(pos, npc.velocity, mod.GetGoreSlot("Gores/NatureGore" + i.ToString()), npc.scale);
                }
                
                for (int i = 0; i < Main.maxNPCs; i++) //find neck segments, place gores there
                {
                    if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<NatureChampionHead>() && Main.npc[i].ai[1] == npc.whoAmI)
                    {
                        Vector2 connector = Main.npc[i].Center;
                        Vector2 neckOrigin = npc.Center + new Vector2(54 * npc.spriteDirection, -10);
                        float chainsPerUse = 0.05f;
                        bool spawnNeck = false;
                        for (float j = 0; j <= 1; j += chainsPerUse)
                        {
                            if (j == 0)
                                continue;
                            Vector2 distBetween = new Vector2(X(j, neckOrigin.X, (neckOrigin.X + connector.X) / 2, connector.X) -
                            X(j - chainsPerUse, neckOrigin.X, (neckOrigin.X + connector.X) / 2, connector.X),
                            Y(j, neckOrigin.Y, (neckOrigin.Y + 50), connector.Y) -
                            Y(j - chainsPerUse, neckOrigin.Y, (neckOrigin.Y + 50), connector.Y));
                            if (distBetween.Length() > 36 && chainsPerUse > 0.01f)
                            {
                                chainsPerUse -= 0.01f;
                                j -= chainsPerUse;
                                continue;
                            }
                            float projTrueRotation = distBetween.ToRotation() - (float)Math.PI / 2;
                            Vector2 lightPos = new Vector2(X(j, neckOrigin.X, (neckOrigin.X + connector.X) / 2, connector.X), Y(j, neckOrigin.Y, (neckOrigin.Y + 50), connector.Y));

                            spawnNeck = !spawnNeck;
                            if (spawnNeck)
                                Gore.NewGore(lightPos, Main.npc[i].velocity, mod.GetGoreSlot("Gores/NatureGore7"), Main.npc[i].scale);
                        }

                        for (int j = 8; j <= 10; j++) //head gores
                        {
                            Vector2 pos = Main.npc[i].position + new Vector2(Main.rand.NextFloat(Main.npc[i].width), Main.rand.NextFloat(Main.npc[i].height));
                            Gore.NewGore(pos, Main.npc[i].velocity, mod.GetGoreSlot("Gores/NatureGore" + j.ToString()), Main.npc[i].scale);
                        }
                    }
                }
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.SuperHealingPotion;
        }

        public override void NPCLoot()
        {
            FargoSoulsWorld.downedChampions[3] = true;
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData); //sync world

            int[] drops = {
                ModContent.ItemType<CrimsonEnchant>(),
                ModContent.ItemType<MoltenEnchant>(),
                ModContent.ItemType<RainEnchant>(),
                ModContent.ItemType<FrostEnchant>(),
                ModContent.ItemType<ChlorophyteEnchant>(),
                ModContent.ItemType<ShroomiteEnchant>()
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

        public Vector2 position, oldPosition;
        private static float X(float t, float x0, float x1, float x2)
        {
            return (float)(
                x0 * Math.Pow((1 - t), 2) +
                x1 * 2 * t * Math.Pow((1 - t), 1) +
                x2 * Math.Pow(t, 2)
            );
        }
        private static float Y(float t, float y0, float y1, float y2)
        {
            return (float)(
                 y0 * Math.Pow((1 - t), 2) +
                 y1 * 2 * t * Math.Pow((1 - t), 1) +
                 y2 * Math.Pow(t, 2)
             );
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

            float scale = (Main.mouseTextColor / 200f - 0.35f) * 0.4f + 0.8f;
            Main.spriteBatch.Draw(texture2D13, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), npc.GetAlpha(lightColor) * 0.5f, npc.rotation, origin2, npc.scale * scale, effects, 0f);
            Main.spriteBatch.Draw(texture2D13, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), npc.GetAlpha(lightColor), npc.rotation, origin2, npc.scale, effects, 0f);

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<NatureChampionHead>() && Main.npc[i].ai[1] == npc.whoAmI)
                {
                    if (npc.Distance(Main.LocalPlayer.Center) <= 1200)
                    {
                        string neckTex = "NPCs/Champions/NatureChampion_Neck";
                        Texture2D neckTex2D = mod.GetTexture(neckTex);
                        Vector2 connector = Main.npc[i].Center;
                        Vector2 neckOrigin = npc.Center + new Vector2(54 * npc.spriteDirection, -10);
                        float chainsPerUse = 0.05f;
                        for (float j = 0; j <= 1; j += chainsPerUse)
                        {
                            if (j == 0)
                                continue;
                            Vector2 distBetween = new Vector2(X(j, neckOrigin.X, (neckOrigin.X + connector.X) / 2, connector.X) -
                            X(j - chainsPerUse, neckOrigin.X, (neckOrigin.X + connector.X) / 2, connector.X),
                            Y(j, neckOrigin.Y, (neckOrigin.Y + 50), connector.Y) -
                            Y(j - chainsPerUse, neckOrigin.Y, (neckOrigin.Y + 50), connector.Y));
                            if (distBetween.Length() > 36 && chainsPerUse > 0.01f)
                            {
                                chainsPerUse -= 0.01f;
                                j -= chainsPerUse;
                                continue;
                            }
                            float projTrueRotation = distBetween.ToRotation() - (float)Math.PI / 2;
                            Vector2 lightPos = new Vector2(X(j, neckOrigin.X, (neckOrigin.X + connector.X) / 2, connector.X), Y(j, neckOrigin.Y, (neckOrigin.Y + 50), connector.Y));
                            spriteBatch.Draw(neckTex2D, new Vector2(X(j, neckOrigin.X, (neckOrigin.X + connector.X) / 2, connector.X) - Main.screenPosition.X, Y(j, neckOrigin.Y, (neckOrigin.Y + 50), connector.Y) - Main.screenPosition.Y),
                            new Rectangle(0, 0, neckTex2D.Width, neckTex2D.Height), npc.GetAlpha(Lighting.GetColor((int)lightPos.X / 16, (int)lightPos.Y / 16)), projTrueRotation,
                            new Vector2(neckTex2D.Width * 0.5f, neckTex2D.Height * 0.5f), 1f, connector.X < neckOrigin.X ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
                        }
                    }

                    /*Texture2D texture = mod.GetTexture("NPCs/Champions/NatureChampion_Neck");
                    Vector2 position = Main.npc[i].Center;
                    Vector2 mountedCenter = npc.Center + new Vector2(54 * npc.spriteDirection, -10);
                    Rectangle? sourceRectangle = new Rectangle?();
                    Vector2 origin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
                    float num1 = texture.Height;
                    Vector2 vector24 = mountedCenter - position;
                    float rotation = (float)Math.Atan2(vector24.Y, vector24.X) - 1.57f;
                    bool flag = true;
                    if (float.IsNaN(position.X) && float.IsNaN(position.Y))
                        flag = false;
                    if (float.IsNaN(vector24.X) && float.IsNaN(vector24.Y))
                        flag = false;
                    while (flag)
                        if (vector24.Length() < num1 + 1.0)
                        {
                            flag = false;
                        }
                        else
                        {
                            Vector2 vector21 = vector24;
                            vector21.Normalize();
                            position += vector21 * num1;
                            vector24 = mountedCenter - position;
                            Color color2 = Lighting.GetColor((int)position.X / 16, (int)(position.Y / 16.0));
                            color2 = npc.GetAlpha(color2);
                            Main.spriteBatch.Draw(texture, position - Main.screenPosition, sourceRectangle, color2, rotation, origin, 1f, 
                                position.X < mountedCenter.X ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0.0f);
                        }*/

                    DrawHead(spriteBatch, Lighting.GetColor((int)Main.npc[i].Center.X / 16, (int)Main.npc[i].Center.Y / 16), Main.npc[i]);
                }
            }
            return false;
        }

        private void DrawHead(SpriteBatch spriteBatch, Color lightColor, NPC head)
        {
            Texture2D texture2D13 = Main.npcTexture[head.type];
            //int num156 = Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type]; //ypos of lower right corner of sprite to draw
            //int y3 = num156 * npc.frame.Y; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = head.frame;//new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = head.GetAlpha(color26);

            SpriteEffects effects = head.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < NPCID.Sets.TrailCacheLength[head.type]; i++)
            {
                Color color27 = color26 * 0.5f;
                color27 *= (float)(NPCID.Sets.TrailCacheLength[head.type] - i) / NPCID.Sets.TrailCacheLength[head.type];
                Vector2 value4 = head.oldPos[i];
                float num165 = head.rotation; //head.oldRot[i];
                Main.spriteBatch.Draw(texture2D13, value4 + head.Size / 2f - Main.screenPosition + new Vector2(0, head.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, head.scale, effects, 0f);
            }

            int glow = (int)head.ai[3];
            if (glow > 0)
                glow--;
            glow += 3;
            Texture2D texture2D14 = mod.GetTexture("NPCs/Champions/NatureChampionHead_Glow" + glow.ToString());

            Main.spriteBatch.Draw(texture2D13, head.Center - Main.screenPosition + new Vector2(0f, head.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), head.GetAlpha(lightColor), head.rotation, origin2, head.scale, effects, 0f);
            Main.spriteBatch.Draw(texture2D14, head.Center - Main.screenPosition + new Vector2(0f, head.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, head.rotation, origin2, head.scale, effects, 0f);
        }
    }
}
