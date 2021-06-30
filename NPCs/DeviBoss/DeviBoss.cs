using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Projectiles;
using FargowiltasSouls.Projectiles.Deathrays;
using FargowiltasSouls.Projectiles.DeviBoss;
using FargowiltasSouls.Projectiles.Masomode;
using FargowiltasSouls.Items.Summons;

namespace FargowiltasSouls.NPCs.DeviBoss
{
    [AutoloadBossHead]
    public class DeviBoss : ModNPC
    {
        public bool playerInvulTriggered;
        private bool droppedSummon = false;

        public int[] attackQueue = new int[4];
        public int lastStrongAttack;

        public int ringProj, spriteProj;

        private bool ContentModLoaded => Fargowiltas.Instance.CalamityLoaded || Fargowiltas.Instance.ThoriumLoaded
            || Fargowiltas.Instance.SoALoaded || Fargowiltas.Instance.MasomodeEXLoaded;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Deviantt");
            Main.npcFrameCount[npc.type] = 4;
        }

        /*public override bool Autoload(ref string name)
        {
            return false;
        }*/

        public override void SetDefaults()
        {
            npc.width = 120;
            npc.height = 120;
            npc.damage = 64;
            npc.defense = 10;
            npc.lifeMax = 5000;
            npc.HitSound = SoundID.NPCHit9;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.npcSlots = 50f;
            npc.knockBackResist = 0f;
            npc.boss = true;
            npc.lavaImmune = true;
            npc.aiStyle = -1;
            npc.netAlways = true;
            npc.buffImmune[BuffID.Chilled] = true;
            npc.buffImmune[BuffID.OnFire] = true;
            npc.buffImmune[BuffID.Suffocation] = true;
            npc.buffImmune[mod.BuffType("Lethargic")] = true;
            npc.buffImmune[mod.BuffType("ClippedWings")] = true;
            //npc.buffImmune[mod.BuffType("MutantNibble")] = true;
            //npc.buffImmune[mod.BuffType("OceanicMaul")] = true;
            npc.timeLeft = NPC.activeTime * 30;
            npc.GetGlobalNPC<FargoSoulsGlobalNPC>().SpecialEnchantImmune = true;
            Mod musicMod = ModLoader.GetMod("FargowiltasMusic");
            music = musicMod != null ? ModLoader.GetMod("FargowiltasMusic").GetSoundSlot(SoundType.Music, "Sounds/Music/LexusCyanixs") : MusicID.Boss1;
            musicPriority = (MusicPriority)10;

            npc.value = Item.buyPrice(0, 5);

            if (ContentModLoaded)
                npc.lifeMax = (int)(npc.lifeMax * 1.5);

            bossBag = ModContent.ItemType<Items.Misc.DeviBag>();
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.damage = (int)(npc.damage * 0.5f);
            npc.lifeMax = (int)(npc.lifeMax /** 0.5f*/ * bossLifeScale);
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return npc.Distance(target.Center) < target.height / 2 + 20;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(npc.localAI[0]);
            writer.Write(npc.localAI[1]);
            writer.Write(npc.localAI[2]);
            writer.Write(npc.localAI[3]);
            writer.Write(attackQueue[0]);
            writer.Write(attackQueue[1]);
            writer.Write(attackQueue[2]);
            writer.Write(attackQueue[3]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc.localAI[0] = reader.ReadSingle();
            npc.localAI[1] = reader.ReadSingle();
            npc.localAI[2] = reader.ReadSingle();
            npc.localAI[3] = reader.ReadSingle();
            attackQueue[0] = reader.ReadInt32();
            attackQueue[1] = reader.ReadInt32();
            attackQueue[2] = reader.ReadInt32();
            attackQueue[3] = reader.ReadInt32();
        }

        private bool ProjectileExists(int id, int type)
        {
            return id > -1 && id < Main.maxProjectiles && Main.projectile[id].active && Main.projectile[id].type == type;
        }

        public override void AI()
        {
            EModeGlobalNPC.deviBoss = npc.whoAmI;

            if (npc.localAI[3] == 0)
            {
                npc.TargetClosest();
                if (npc.timeLeft < 30)
                    npc.timeLeft = 30;

                if (npc.Distance(Main.player[npc.target].Center) < 2000)
                {
                    npc.localAI[3] = 1;
                    Main.PlaySound(SoundID.Roar, (int)npc.Center.X, (int)npc.Center.Y, 0);

                    do
                    {
                        RefreshAttackQueue();
                    } while (attackQueue[0] == 3 || attackQueue[0] == 5 || attackQueue[0] == 9 || attackQueue[0] == 10);
                    //don't start with wyvern, mage spam, frostballs, baby guardian
                }
            }
            /*else if (npc.localAI[3] == 1)
            {
                Aura(2000f, mod.BuffType("GodEater"), true, 86);
            }*/
            /*else if (Main.player[Main.myPlayer].active && npc.Distance(Main.player[Main.myPlayer].Center) < 3000f)
            {
                if (FargoSoulsWorld.MasochistMode)
                    Main.player[Main.myPlayer].AddBuff(mod.BuffType("DeviPresence"), 2);
            }*/

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!ProjectileExists(ringProj, ModContent.ProjectileType<DeviRitual2>()))
                    ringProj = Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<DeviRitual2>(), 0, 0f, Main.myPlayer, 0f, npc.whoAmI);

                if (!ProjectileExists(spriteProj, ModContent.ProjectileType<Projectiles.DeviBoss.DeviBoss>()))
                {
                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        int number = 0;
                        for (int index = 999; index >= 0; --index)
                        {
                            if (!Main.projectile[index].active)
                            {
                                number = index;
                                break;
                            }
                        }
                        if (number >= 0)
                        {
                            if (Main.netMode == NetmodeID.SinglePlayer)
                            {
                                Projectile projectile = Main.projectile[number];
                                projectile.SetDefaults(ModContent.ProjectileType<Projectiles.DeviBoss.DeviBoss>());
                                projectile.Center = npc.Center;
                                projectile.owner = Main.myPlayer;
                                projectile.velocity.X = 0;
                                projectile.velocity.Y = 0;
                                projectile.damage = 0;
                                projectile.knockBack = 0f;
                                projectile.identity = number;
                                projectile.gfxOffY = 0f;
                                projectile.stepSpeed = 1f;
                                projectile.ai[1] = npc.whoAmI;

                                spriteProj = number;
                            }
                        }
                    }
                    else //server
                    {
                        spriteProj = Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.DeviBoss.DeviBoss>(), 0, 0f, Main.myPlayer, 0, npc.whoAmI);
                    }
                }
            }

            int projectileDamage = npc.damage / (npc.localAI[3] > 1 ? 4 : 5);

            Player player = Main.player[npc.target];
            npc.direction = npc.spriteDirection = npc.Center.X < player.Center.X ? 1 : -1;
            Vector2 targetPos;

            void StrongAttackTeleport()
            {
                const float range = 450f;
                if (npc.Distance(player.Center) < range)
                    return;

                TeleportDust();
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (player.velocity == Vector2.Zero)
                        npc.Center = player.Center + range * Vector2.UnitX.RotatedByRandom(2 * Math.PI);
                    else
                        npc.Center = player.Center + range * Vector2.Normalize(player.velocity);
                    npc.velocity /= 2f;
                    npc.netUpdate = true;
                }
                TeleportDust();
                Main.PlaySound(SoundID.Item84, npc.Center);
            };

            switch ((int)npc.ai[0])
            {
                case -2: //ACTUALLY dead
                    if (!AliveCheck(player))
                        break;
                    npc.velocity *= 0.9f;
                    npc.dontTakeDamage = true;
                    for (int i = 0; i < 5; i++)
                    {
                        int d = Dust.NewDust(npc.position, npc.width, npc.height, 86, 0f, 0f, 0, default(Color), 2.5f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].velocity *= 12f;
                    }
                    if (++npc.ai[1] > 180)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            if (!NPC.AnyNPCs(ModLoader.GetMod("Fargowiltas").NPCType("Deviantt")))
                            {
                                int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModLoader.GetMod("Fargowiltas").NPCType("Deviantt"));
                                if (n != 200 && Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                            }
                        }
                        npc.life = 0;
                        npc.dontTakeDamage = false;
                        npc.checkDead();
                    }
                    break;

                case -1: //phase 2 transition
                    npc.velocity *= 0.9f;
                    npc.dontTakeDamage = true;
                    if (npc.buffType[0] != 0)
                        npc.DelBuff(0);
                    if (++npc.ai[1] > 60)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            int d = Dust.NewDust(npc.position, npc.width, npc.height, 86, 0f, 0f, 0, default(Color), 1.5f);
                            Main.dust[d].noGravity = true;
                            Main.dust[d].velocity *= 4f;
                        }
                        npc.localAI[3] = 2; //npc marks p2
                        if (FargoSoulsWorld.MasochistMode)
                        {
                            int heal = (int)(npc.lifeMax / 90 * Main.rand.NextFloat(1f, 1.5f));
                            npc.life += heal;
                            if (npc.life > npc.lifeMax)
                                npc.life = npc.lifeMax;
                            CombatText.NewText(npc.Hitbox, CombatText.HealLife, heal);
                        }
                        if (npc.ai[1] > 150)
                        {
                            RefreshAttackQueue();
                            attackQueue[3] = 15; //always do sparkling love
                            npc.localAI[2] = npc.localAI[3] > 1 ? 1 : 0;
                            GetNextAttack();
                        }
                    }
                    else if (npc.ai[1] == 60)
                    {
                        /*for (int i = 0; i < Main.maxProjectiles; i++)
                            if (Main.projectile[i].active && Main.projectile[i].friendly && !Main.projectile[i].minion && Main.projectile[i].damage > 0)
                                Main.projectile[i].Kill();
                        for (int i = 0; i < Main.maxProjectiles; i++)
                            if (Main.projectile[i].active && Main.projectile[i].friendly && !Main.projectile[i].minion && Main.projectile[i].damage > 0)
                                Main.projectile[i].Kill();
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<AbomRitual>(), npc.damage / 2, 0f, Main.myPlayer, 0f, npc.whoAmI);
                        }*/
                        Main.PlaySound(SoundID.Roar, (int)npc.Center.X, (int)npc.Center.Y, 0);
                    }
                    break;

                case 0: //track player, decide which attacks to use
                    if (!AliveCheck(player) || Phase2Check())
                        break;

                    npc.dontTakeDamage = false;

                    targetPos = player.Center;
                    targetPos.X += 500 * (npc.Center.X < targetPos.X ? -1 : 1);
                    if (npc.Distance(targetPos) > 50)
                        Movement(targetPos, npc.localAI[3] > 0 ? 0.15f : 2f, npc.localAI[3] > 0 ? 12f : 1200f);

                    if (npc.localAI[3] > 0) //in range, fight has begun, choose attacks
                    {
                        npc.netUpdate = true;
                        GetNextAttack();
                    }
                    break;

                case 1: //teleport marx hammers
                    if (!AliveCheck(player) || Phase2Check())
                        break;

                    if (npc.localAI[1] == 0) //pick random number of teleports to do
                    {
                        npc.localAI[1] = npc.localAI[3] > 1 ? Main.rand.Next(3, 10) : Main.rand.Next(3, 6);
                        npc.netUpdate = true;
                    }

                    npc.velocity = Vector2.Zero;
                    if (++npc.ai[1] > (npc.localAI[3] > 1 ? 10 : 20) && npc.ai[2] < npc.localAI[1])
                    {
                        npc.localAI[1] = 0;
                        npc.ai[1] = 0;
                        npc.ai[2]++;

                        TeleportDust();
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            bool wasOnLeft = npc.Center.X < player.Center.X;
                            npc.Center = player.Center + 200 * Vector2.UnitX.RotatedBy(Main.rand.NextFloat(0, 2 * (float)Math.PI));
                            if (wasOnLeft ? npc.Center.X < player.Center.X : npc.Center.X > player.Center.X)
                            {
                                float x = player.Center.X - npc.Center.X;
                                npc.position.X += x * 2;
                            }
                            npc.netUpdate = true;
                        }
                        TeleportDust();
                        Main.PlaySound(SoundID.Item84, npc.Center);
                    }

                    if (npc.ai[1] == 60) //finished all the prior teleports, now attack
                    {
                        npc.netUpdate = true;

                        for (int i = 0; i < 36; i++) //dust ring
                        {
                            Vector2 vector6 = Vector2.UnitY * 9f;
                            vector6 = vector6.RotatedBy((i - (36 / 2 - 1)) * 6.28318548f / 36) + npc.Center;
                            Vector2 vector7 = vector6 - npc.Center;
                            int d = Dust.NewDust(vector6 + vector7, 0, 0, 246, 0f, 0f, 0, default(Color), 3f);
                            Main.dust[d].noLight = true;
                            Main.dust[d].noGravity = true;
                            Main.dust[d].velocity = vector7;
                        }

                        Main.PlaySound(SoundID.Item92, npc.Center);

                        if (Main.netMode != NetmodeID.MultiplayerClient) //hammers
                        {
                            const float retiRad = 150;
                            const float spazRad = 100;
                            const int retiTime = 45;
                            const int spazTime = 45;

                            float retiSpeed = 2 * (float)Math.PI * retiRad / retiTime;
                            float spazSpeed = 2 * (float)Math.PI * spazRad / spazTime;
                            float retiAcc = retiSpeed * retiSpeed / retiRad * npc.direction;
                            float spazAcc = spazSpeed * spazSpeed / spazRad * -npc.direction;

                            for (int i = 0; i < 4; i++)
                            {
                                Projectile.NewProjectile(npc.Center, Vector2.UnitX.RotatedBy(Math.PI / 2 * i) * retiSpeed, ModContent.ProjectileType<DeviHammer>(), projectileDamage, 0f, Main.myPlayer, retiAcc, retiTime);
                                Projectile.NewProjectile(npc.Center, Vector2.UnitX.RotatedBy(Math.PI / 2 * i + Math.PI / 4) * spazSpeed, ModContent.ProjectileType<DeviHammer>(), projectileDamage, 0f, Main.myPlayer, spazAcc, spazTime);
                            }
                        }
                    }
                    else if (npc.ai[1] > 90)
                    {
                        npc.netUpdate = true;
                        if (npc.localAI[3] > 1 && ++npc.localAI[0] < 3)
                        {
                            npc.ai[2] = 0; //reset tp counter and attack again
                            npc.localAI[1] = 0;
                        }
                        else
                        {
                            GetNextAttack();
                        }
                    }
                    break;

                case 2: //heart barrages
                    if (!AliveCheck(player) || Phase2Check())
                        break;

                    targetPos = player.Center;
                    targetPos.X += 400 * (npc.Center.X < targetPos.X ? -1 : 1);
                    if (npc.Distance(targetPos) > 50)
                        Movement(targetPos, 0.2f);

                    if (--npc.ai[1] < 0)
                    {
                        npc.netUpdate = true;
                        npc.ai[1] = 75;
                        if (++npc.ai[2] > 3)
                        {
                            GetNextAttack();
                        }
                        else
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int damage = (int)(npc.damage / 3.2); //comes out to 20 raw, fake hearts ignore the usual multipliers

                                Vector2 spawnVel = npc.DirectionFrom(Main.player[npc.target].Center) * 10f;
                                for (int i = -3; i < 3; i++)
                                {
                                    Projectile.NewProjectile(npc.Center, spawnVel.RotatedBy(Math.PI / 7 * i),
                                        ModContent.ProjectileType<FakeHeart2>(), damage, 0f, Main.myPlayer, 20, 30);
                                }
                                if (npc.localAI[3] > 1)
                                {
                                    for (int i = -5; i < 5; i++)
                                    {
                                        Projectile.NewProjectile(npc.Center, 1.5f * spawnVel.RotatedBy(Math.PI / 10 * i),
                                            ModContent.ProjectileType<FakeHeart2>(), damage, 0f, Main.myPlayer, 20, 40 + 5 * Math.Abs(i));
                                    }
                                }
                            }
                        }
                        break;
                    }
                    break;

                case 3: //slow while shooting wyvern orb spirals
                    if (!AliveCheck(player) || Phase2Check())
                        break;
                    
                    targetPos = player.Center + player.DirectionTo(npc.Center) * 375;
                    if (npc.Distance(targetPos) > 50)
                        Movement(targetPos, 0.15f);

                    if (--npc.ai[1] < 0)
                    {
                        npc.netUpdate = true;
                        npc.ai[1] = 150;

                        if (++npc.ai[2] > 3)
                        {
                            GetNextAttack();
                        }
                        else
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int max = npc.localAI[3] > 1 ? 8 : 12;
                                Vector2 vel = Vector2.Normalize(npc.velocity);
                                for (int i = 0; i < max; i++)
                                {
                                    Projectile.NewProjectile(npc.Center, vel.RotatedBy(2 * Math.PI / max * i), ModContent.ProjectileType<DeviLightBall>(), projectileDamage, 0f, Main.myPlayer, 0f, .008f * npc.direction);
                                    if (npc.localAI[3] > 1)
                                        Projectile.NewProjectile(npc.Center, vel.RotatedBy(2 * Math.PI / max * i), ModContent.ProjectileType<DeviLightBall>(), projectileDamage, 0f, Main.myPlayer, 0f, .008f * -npc.direction);
                                }
                            }
                        }
                    }
                    break;

                case 4: //mimics
                    if (!AliveCheck(player) || Phase2Check())
                        break;

                    targetPos = player.Center;
                    targetPos.X += 300 * (npc.Center.X < targetPos.X ? -1 : 1);
                    targetPos.Y -= 300;
                    if (npc.Distance(targetPos) > 50)
                        Movement(targetPos, 0.15f);

                    if (++npc.ai[1] < 120)
                    {
                        if (++npc.ai[2] > 20)
                        {
                            npc.ai[2] = 0;

                            Main.PlaySound(SoundID.Item84, npc.Center);

                            int delay = npc.localAI[3] > 1 ? 45 : 60;
                            Vector2 target = player.Center;
                            target.Y -= 400;
                            Vector2 speed = (target - npc.Center) / delay;

                            for (int i = 0; i < 20; i++) //dust spray
                                Dust.NewDust(npc.Center, 0, 0, Main.rand.Next(2) == 0 ? DustID.GoldFlame : DustID.SilverCoin, speed.X, speed.Y, 0, default(Color), 2f);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int type = ModContent.ProjectileType<DeviMimic>();
                                float ai0 = player.position.Y - 16;

                                if (npc.localAI[3] > 1)
                                {
                                    type = ModContent.ProjectileType<DeviBigMimic>();
                                    ai0 = player.whoAmI;
                                }

                                Projectile.NewProjectile(npc.Center, speed, type, projectileDamage, 0f, Main.myPlayer, ai0, delay);
                            }
                        }
                    }
                    else if (npc.ai[1] == 180) //big wave of mimics, aimed ahead of you
                    {
                        Main.PlaySound(SoundID.Item84, npc.Center);

                        int modifier = 150;
                        if (player.velocity.X != 0)
                            modifier *= Math.Sign(player.velocity.X);
                        else
                            modifier *= Math.Sign(player.Center.X - npc.Center.X);

                        Vector2 target = player.Center;
                        target.Y -= 400;

                        for (int j = 0; j < 7; j++)
                        {
                            int delay = npc.localAI[3] > 1 ? 45 : 60;
                            Vector2 speed = (target - npc.Center) / delay;

                            for (int i = 0; i < 20; i++) //dust spray
                                Dust.NewDust(npc.Center, 0, 0, Main.rand.Next(2) == 0 ? DustID.GoldFlame : DustID.SilverCoin, speed.X, speed.Y, 0, default(Color), 2f);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int type = ModContent.ProjectileType<DeviMimic>();
                                float ai0 = player.position.Y - 16;

                                if (npc.localAI[3] > 1)
                                {
                                    type = ModContent.ProjectileType<DeviBigMimic>();
                                    ai0 = player.whoAmI;
                                }

                                Projectile.NewProjectile(npc.Center, speed, type, projectileDamage, 0f, Main.myPlayer, ai0, delay);
                            }

                            target.X += modifier;
                        }
                    }
                    else if (npc.ai[1] > 240)
                    {
                        GetNextAttack();
                    }
                    break;

                case 5: //frostballs and nados
                    if (!AliveCheck(player) || Phase2Check())
                        break;

                    targetPos = player.Center + player.DirectionTo(npc.Center) * 350;
                    if (npc.Distance(targetPos) > 50)
                        Movement(targetPos, 0.2f);

                    if (++npc.ai[1] > 360)
                    {
                        GetNextAttack();

                        for (int i = 0; i < Main.maxProjectiles; i++)
                        {
                            if (Main.projectile[i].active && Main.projectile[i].hostile && Main.projectile[i].type == ModContent.ProjectileType<FrostfireballHostile>()
                                && Main.projectile[i].timeLeft > 120)
                            {
                                Main.projectile[i].timeLeft = 120;
                            }
                        }
                    }
                    if (++npc.ai[2] > (npc.localAI[3] > 1 ? 10 : 20))
                    {
                        npc.netUpdate = true;
                        npc.ai[2] = 0;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(npc.Center, new Vector2(4f, 0f).RotatedBy(Main.rand.NextDouble() * Math.PI * 2),
                                ModContent.ProjectileType<FrostfireballHostile>(), projectileDamage, 0f, Main.myPlayer, npc.target, 15f);
                        }
                    }
                    if (npc.localAI[3] > 1 && --npc.ai[3] < 0) //spawn sandnado
                    {
                        npc.netUpdate = true;
                        npc.ai[3] = 110;

                        Vector2 target = player.Center;
                        target.X += player.velocity.X * 90;
                        target.Y -= 150;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
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
                    break;

                case 6: //rune wizard
                    if (!AliveCheck(player) || Phase2Check())
                        break;

                    if (npc.Distance(Main.LocalPlayer.Center) < 3000f) //auras
                    {
                        if (npc.Distance(Main.LocalPlayer.Center) > 450f)
                        {
                            Main.LocalPlayer.AddBuff(mod.BuffType("Hexed"), 2);
                            Main.LocalPlayer.AddBuff(mod.BuffType("Crippled"), 2);
                            Main.LocalPlayer.AddBuff(BuffID.Dazed, 2);
                            Main.LocalPlayer.AddBuff(BuffID.OgreSpit, 2);
                        }
                        for (int i = 0; i < 20; i++)
                        {
                            Vector2 offset = new Vector2();
                            double angle = Main.rand.NextDouble() * 2d * Math.PI;
                            offset.X += (float)(Math.Sin(angle) * 450);
                            offset.Y += (float)(Math.Cos(angle) * 450);
                            Dust dust = Main.dust[Dust.NewDust(
                                npc.Center + offset - new Vector2(4, 4), 0, 0,
                                74, 0, 0, 100, default(Color), 1f
                                )];
                            dust.velocity = npc.velocity;
                            if (Main.rand.Next(3) == 0)
                                dust.velocity += Vector2.Normalize(offset) * 5f;
                            dust.noGravity = true;
                            dust.color = Color.GreenYellow;
                        }
                        
                        if (npc.Distance(Main.LocalPlayer.Center) < 150)
                        {
                            Main.LocalPlayer.AddBuff(mod.BuffType("Hexed"), 2);
                            Main.LocalPlayer.AddBuff(mod.BuffType("Crippled"), 2);
                            Main.LocalPlayer.AddBuff(BuffID.Dazed, 2);
                            Main.LocalPlayer.AddBuff(BuffID.OgreSpit, 2);
                        }
                        for (int i = 0; i < 10; i++)
                        {
                            Vector2 offset = new Vector2();
                            double angle = Main.rand.NextDouble() * 2d * Math.PI;
                            offset.X += (float)(Math.Sin(angle) * 150);
                            offset.Y += (float)(Math.Cos(angle) * 150);
                            Dust dust = Main.dust[Dust.NewDust(
                                npc.Center + offset - new Vector2(4, 4), 0, 0,
                                73, 0, 0, 100, default(Color), 1f
                                )];
                            dust.velocity = npc.velocity;
                            if (Main.rand.Next(3) == 0)
                                dust.velocity -= Vector2.Normalize(offset) * 5f;
                            dust.noGravity = true;
                        }
                    }

                    npc.velocity = Vector2.Zero;
                    if (++npc.ai[1] == 1)
                    {
                        TeleportDust();
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            bool wasOnLeft = npc.Center.X < player.Center.X;
                            npc.Center = player.Center + 300 * Vector2.UnitX.RotatedBy(Main.rand.NextFloat(0, 2 * (float)Math.PI));
                            if (wasOnLeft ? npc.Center.X < player.Center.X : npc.Center.X > player.Center.X)
                            {
                                float x = player.Center.X - npc.Center.X;
                                npc.position.X += x * 2;
                            }
                            npc.netUpdate = true;
                        }
                        TeleportDust();
                        Main.PlaySound(SoundID.Item84, npc.Center);
                    }
                    else if (npc.ai[1] == 50)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            for (int i = -1; i <= 1; i++) //rune blast spread
                                Projectile.NewProjectile(npc.Center,
                                    12f * npc.DirectionTo(player.Center).RotatedBy(MathHelper.ToRadians(5) * i),
                                    ProjectileID.RuneBlast, projectileDamage, 0f, Main.myPlayer);

                            if (npc.localAI[3] > 1) //rune blast ring
                            {
                                Vector2 vel = npc.DirectionFrom(Main.player[npc.target].Center) * 8;
                                for (int i = 0; i < 5; i++)
                                {
                                    int p = Projectile.NewProjectile(npc.Center, vel.RotatedBy(2 * Math.PI / 5 * i),
                                        ProjectileID.RuneBlast, projectileDamage, 0f, Main.myPlayer, 1);
                                    if (p != 1000)
                                        Main.projectile[p].timeLeft = 300;
                                }
                            }
                        }
                    }
                    else if (npc.ai[1] > 100)
                    {
                        if (++npc.ai[2] > 3)
                        {
                            GetNextAttack();

                            for (int i = 0; i < Main.maxProjectiles; i++)
                            {
                                if (Main.projectile[i].active && Main.projectile[i].hostile && Main.projectile[i].type == ProjectileID.RuneBlast
                                    && Main.projectile[i].timeLeft > 60)
                                {
                                    Main.projectile[i].timeLeft = 60;
                                }
                            }
                        }
                        else
                        {
                            npc.netUpdate = true;
                            npc.ai[1] = 0;
                        }
                    }
                    break;

                case 7: //moth dust charges
                    if (!AliveCheck(player) || Phase2Check())
                        break;

                    if (npc.localAI[0] == 0) //teleport behind you
                    {
                        npc.localAI[0] = 1;
                        npc.ai[1] = -45;

                        TeleportDust();
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            bool wasOnLeft = npc.Center.X < player.Center.X;
                            npc.Center = player.Center;
                            npc.position.X += wasOnLeft ? 400 : -400;
                            npc.netUpdate = true;
                        }
                        TeleportDust();

                        Main.PlaySound(SoundID.Item84, npc.Center);
                        Main.PlaySound(SoundID.Roar, (int)npc.Center.X, (int)npc.Center.Y, 0);
                    }

                    if (++npc.ai[3] > 2)
                    {
                        npc.ai[3] = 0;

                        if (Main.netMode != NetmodeID.MultiplayerClient) //make moth dust trail
                            Projectile.NewProjectile(npc.Center, Main.rand.NextVector2Unit() * 2f, ModContent.ProjectileType<MothDust>(), projectileDamage, 0f, Main.myPlayer);
                    }

                    npc.velocity *= 0.9f;
                    if (++npc.ai[1] > (npc.localAI[3] > 1 ? 30 : 45))
                    {
                        npc.netUpdate = true;
                        if (++npc.ai[2] > 5)
                        {
                            GetNextAttack();
                        }
                        else
                        {
                            npc.ai[0]++;
                            npc.ai[1] = 0;
                            npc.velocity = npc.DirectionTo(player.Center + player.velocity) * 20f;
                        }
                    }
                    break;

                case 8: //while dashing
                    if (Phase2Check())
                        break;

                    npc.direction = npc.spriteDirection = Math.Sign(npc.velocity.X);

                    if (++npc.ai[3] > 2)
                    {
                        npc.ai[3] = 0;

                        if (Main.netMode != NetmodeID.MultiplayerClient) //make moth dust trail
                            Projectile.NewProjectile(npc.Center, Main.rand.NextVector2Unit() * 2f, ModContent.ProjectileType<MothDust>(), projectileDamage, 0f, Main.myPlayer);
                    }

                    if (++npc.ai[1] > 30)
                    {
                        npc.netUpdate = true;
                        npc.ai[0]--;
                        npc.ai[1] = 0;
                    }
                    break;

                case 9: //mage skeleton attacks
                    if (!AliveCheck(player) || Phase2Check())
                        break;

                    npc.velocity = npc.DirectionTo(player.Center);

                    if (++npc.ai[1] == 1)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, npc.whoAmI, -1);
                        Main.PlaySound(SoundID.Roar, (int)npc.Center.X, (int)npc.Center.Y, 0);
                    }
                    else if (npc.ai[1] < 120) //spam shadowbeams after delay
                    {
                        if (++npc.ai[2] > 90)
                        {
                            if (npc.ai[3] == 0) //store rotation briefly before shooting
                            {
                                npc.localAI[0] = npc.DirectionTo(player.Center).ToRotation();
                            }

                            if (++npc.ai[3] > (npc.localAI[3] > 1 ? 5 : 8))
                            {
                                npc.ai[3] = 0;

                                if (Main.netMode != NetmodeID.MultiplayerClient) //shoot a shadowbeam
                                {
                                    int p = Projectile.NewProjectile(npc.Center, 6f * Vector2.UnitX.RotatedBy(npc.localAI[0]), ProjectileID.ShadowBeamHostile, projectileDamage, 0f, Main.myPlayer);
                                    if (p != Main.maxProjectiles)
                                        Main.projectile[p].timeLeft = 300;
                                }
                            }
                        }
                    }
                    else if (npc.ai[1] < 240)
                    {
                        npc.ai[3] = 0;
                        npc.localAI[0] = 0;

                        if (++npc.ai[2] > (npc.localAI[3] > 1 ? 20 : 40)) //shoot diabolist bolts
                        {
                            npc.ai[2] = 0;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                float speed = npc.localAI[3] > 1 ? 16 : 8;
                                Vector2 blastPos = npc.Center + Main.rand.NextFloat(1, 2) * npc.Distance(player.Center) * npc.DirectionTo(player.Center);
                                int p = Projectile.NewProjectile(npc.Center, speed * npc.DirectionTo(player.Center), ProjectileID.InfernoHostileBolt, projectileDamage, 0f, Main.myPlayer, blastPos.X, blastPos.Y);
                                if (p != Main.maxProjectiles)
                                    Main.projectile[p].timeLeft = 300;
                            }
                        }
                    }
                    else
                    {
                        npc.velocity /= 2;

                        if (npc.ai[1] == 300) //spray ragged caster bolts
                        {
                            Main.PlaySound(SoundID.ForceRoar, (int)npc.Center.X, (int)npc.Center.Y, -1, 1f, 0f); //eoc roar

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int max = npc.localAI[3] > 1 ? 30 : 20;
                                for (int i = 0; i < max; i++)
                                {
                                    int p = Projectile.NewProjectile(npc.Center, Main.rand.NextFloat(2f, 6f) * Vector2.UnitX.RotatedBy(Main.rand.NextFloat((float)Math.PI * 2)), ModContent.ProjectileType<DeviLostSoul>(), projectileDamage, 0f, Main.myPlayer);
                                    if (p != Main.maxProjectiles)
                                        Main.projectile[p].timeLeft = 300;
                                }
                            }
                        }

                        if (npc.ai[1] > 360)
                        {
                            GetNextAttack();
                        }
                    }
                    break;

                case 10: //baby guardians
                    if (!AliveCheck(player) || Phase2Check())
                        break;

                    targetPos = player.Center;
                    targetPos.Y -= 400;
                    if (npc.Distance(targetPos) > 50)
                        Movement(targetPos, 0.3f);

                    if (npc.ai[1] == 1) //tp above player
                    {
                        TeleportDust();
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            npc.Center = player.Center;
                            npc.position.X += 500 * (Main.rand.Next(2) == 0 ? -1 : 1);
                            npc.position.Y -= Main.rand.NextFloat(300, 500);
                            npc.netUpdate = true;
                        }
                        TeleportDust();
                        Main.PlaySound(SoundID.Item84, npc.Center);
                    }

                    if (++npc.ai[1] < 180)
                    {
                        //warning dust
                        for (int i = 0; i < 3; i++)
                        {
                            int d = Dust.NewDust(npc.Center, 0, 0, DustID.Fire, 0f, 0f, 0, default(Color), 3f);
                            Main.dust[d].noGravity = true;
                            Main.dust[d].noLight = true;
                            Main.dust[d].velocity *= 12f;
                        }
                    }
                    else if (npc.ai[1] == 180)
                    {
                        npc.netUpdate = true;

                        Main.PlaySound(SoundID.Roar, (int)npc.Center.X, (int)npc.Center.Y, 0);

                        if (Main.netMode != NetmodeID.MultiplayerClient) //shoot guardians
                        {
                            for (int i = -1; i <= 1; i++) //left and right sides
                            {
                                if (i == 0)
                                    continue;

                                for (int j = -1; j <= 1; j++)
                                {
                                    Vector2 spawnPos = player.Center;
                                    spawnPos.X += 1200 * i;
                                    spawnPos.Y += 50 * j;
                                    Vector2 vel = 14 * Vector2.UnitX * -i;
                                    Projectile.NewProjectile(spawnPos, vel, ModContent.ProjectileType<DeviGuardian>(), npc.damage / 3, 0f, Main.myPlayer);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (++npc.ai[2] > 3)
                        {
                            npc.ai[2] = 0;
                            Main.PlaySound(SoundID.Item21, npc.Center);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Vector2 spawnPos = player.Center;
                                spawnPos.X += Main.rand.Next(-200, 201);
                                spawnPos.Y += 700;
                                Vector2 vel = Main.rand.NextFloat(12, 16f) * Vector2.Normalize(player.Center - spawnPos);
                                Projectile.NewProjectile(spawnPos, vel, ModContent.ProjectileType<DeviGuardian>(), npc.damage / 3, 0f, Main.myPlayer);
                            }
                        }

                        if (npc.ai[1] > 360)
                        {
                            GetNextAttack();
                        }

                        if (npc.localAI[3] > 1 && npc.ai[1] == 270) //surprise!
                        {
                            Main.PlaySound(SoundID.ForceRoar, (int)npc.Center.X, (int)npc.Center.Y, -1, 1f, 0f); //eoc roar

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                for (int i = -1; i <= 1; i++) //left and right sides
                                {
                                    if (i == 0)
                                        continue;

                                    for (int j = -1; j <= 1; j++)
                                    {
                                        Vector2 spawnPos = player.Center;
                                        spawnPos.X += 1200 * i;
                                        spawnPos.Y += 60 * j;
                                        Vector2 vel = 14 * Vector2.UnitX * -i;
                                        Projectile.NewProjectile(spawnPos, vel, ModContent.ProjectileType<DeviGuardian>(), npc.damage / 3, 0f, Main.myPlayer);
                                    }
                                }
                            }
                        }
                    }
                    break;

                case 11: //noah/irisu geyser rain
                    if (!AliveCheck(player) || Phase2Check())
                        break;

                    if (npc.localAI[0] == 0 && npc.localAI[1] == 0)
                    {
                        StrongAttackTeleport();

                        if (Main.netMode != NetmodeID.MultiplayerClient) //spawn ritual for strong attacks
                            Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<DeviRitual>(), npc.damage / 4, 0f, Main.myPlayer, 0f, npc.whoAmI);

                        npc.localAI[0] = npc.Center.X;
                        npc.localAI[1] = npc.Center.Y;
                        npc.netUpdate = true;
                    }

                    //Main.NewText(npc.localAI[0].ToString() + ", " + npc.localAI[1].ToString());

                    targetPos = player.Center;
                    targetPos.Y -= 350;
                    if (npc.Distance(targetPos) > 50)
                        Movement(targetPos, 0.15f);

                    if (++npc.ai[1] < 120)
                    {
                        if (++npc.ai[2] > 2)
                        {
                            npc.ai[2] = 0;
                            Main.PlaySound(SoundID.Item44, npc.Center);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(npc.Center, -24 * Vector2.UnitY.RotatedBy((Main.rand.NextDouble() - 0.5) * Math.PI / 2),
                                    ModContent.ProjectileType<DeviVisualHeart>(), 0, 0f, Main.myPlayer);
                            }
                        }
                    }
                    else if (npc.ai[1] < 420)
                    {
                        if (--npc.ai[3] < 0)
                        {
                            npc.netUpdate = true;
                            npc.ai[3] = 85;

                            npc.ai[2] = npc.ai[2] == 1 ? -1 : 1;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Vector2 speed = 24 * Vector2.UnitY.RotatedBy(MathHelper.ToRadians(10) * npc.ai[2]);
                                int type = npc.localAI[3] > 1 ? ModContent.ProjectileType<DeviRainHeart2>() : ModContent.ProjectileType<DeviRainHeart>();
                                int damage = npc.localAI[3] > 1 ? npc.damage / 3 : npc.damage / 4;
                                int range = npc.localAI[3] > 1 ? 8 : 12;
                                float spacing = 1200f / range;
                                float offset = Main.rand.NextFloat(-spacing, spacing);
                                for (int i = -range; i <= range; i++)
                                {
                                    Vector2 spawnPos = new Vector2(npc.localAI[0], npc.localAI[1]);
                                    spawnPos.X += spacing * i + offset;
                                    spawnPos.Y -= 1200;
                                    Projectile.NewProjectile(spawnPos, speed, type, damage, 0f, Main.myPlayer, 0f, npc.whoAmI);
                                }
                            }
                        }
                    }
                    else if (npc.ai[1] > 510)
                    {
                        GetNextAttack();
                    }
                    break;

                case 12: //lilith cross ray hearts
                    if (!AliveCheck(player) || Phase2Check())
                        break;

                    targetPos = player.Center + player.DirectionTo(npc.Center) * 400;
                    if (npc.Distance(targetPos) > 50)
                        Movement(targetPos, 0.3f);

                    if (npc.localAI[0] == 0)
                    {
                        StrongAttackTeleport();
                        
                        if (Main.netMode != NetmodeID.MultiplayerClient) //spawn ritual for strong attacks
                            Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<DeviRitual>(), npc.damage / 4, 0f, Main.myPlayer, 0f, npc.whoAmI);

                        npc.localAI[0] = 1;
                        npc.netUpdate = true;
                    }

                    if (++npc.ai[2] > (npc.localAI[3] > 1 ? 75 : 100))
                    {
                        if (++npc.ai[3] > 5)
                        {
                            npc.ai[3] = 0;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Vector2 target = player.Center - npc.Center;
                                target.X += Main.rand.Next(-75, 76);
                                target.Y += Main.rand.Next(-75, 76);

                                Vector2 speed = 2 * target / 90;
                                float acceleration = -speed.Length() / 90;

                                int damage = npc.localAI[3] > 1 ? npc.damage / 3 : npc.damage / 4;

                                Projectile.NewProjectile(npc.Center, speed, ModContent.ProjectileType<DeviEnergyHeart>(),
                                    damage, 0f, Main.myPlayer, 0f, acceleration);
                            }
                        }

                        if (npc.ai[2] > 130)
                        {
                            npc.netUpdate = true;
                            npc.ai[2] = 0;
                        }
                    }

                    if (++npc.ai[1] > (npc.localAI[3] > 1 ? 450 : 480))
                    {
                        GetNextAttack();
                    }
                    break;

                case 13: //that one boss that was a bunch of gems burst rain but with butterflies
                    if (!AliveCheck(player) || Phase2Check())
                        break;

                    npc.velocity = Vector2.Zero;

                    if (npc.ai[2] == 0)
                    {
                        StrongAttackTeleport();

                        npc.ai[2] = 1;
                        Main.PlaySound(SoundID.Roar, (int)npc.Center.X, (int)npc.Center.Y, 0);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            float offset = Main.rand.NextFloat(600);
                            int damage = npc.localAI[3] > 1 ? npc.damage / 3 : npc.damage / 4;
                            for (int i = 0; i < 8; i++) //make butterflies
                            {
                                Vector2 speed = new Vector2(Main.rand.NextFloat(40f), Main.rand.NextFloat(-20f, 20f));
                                Projectile.NewProjectile(npc.Center, speed, ModContent.ProjectileType<DeviButterfly>(),
                                   damage, 0f, Main.myPlayer, npc.whoAmI, 300 / 4 * i + offset);
                            }
                        }
                        
                        if (Main.netMode != NetmodeID.MultiplayerClient) //spawn ritual for strong attacks
                            Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<DeviRitual>(), npc.damage / 4, 0f, Main.myPlayer, 0f, npc.whoAmI);
                    }

                    if (++npc.ai[1] > 480)
                    {
                        GetNextAttack();
                    }
                    break;

                case 14: //medusa ray
                    if ((npc.ai[1] < 420 && !AliveCheck(player)) || Phase2Check())
                        break;

                    if (npc.localAI[0] == 0)
                    {
                        StrongAttackTeleport();

                        npc.localAI[0] = 1;
                        npc.velocity = Vector2.Zero;

                        if (Main.netMode != NetmodeID.MultiplayerClient) //spawn ritual for strong attacks
                            Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<DeviRitual>(), npc.damage / 4, 0f, Main.myPlayer, 0f, npc.whoAmI);
                    }

                    if (npc.ai[3] < 4 && npc.Distance(Main.LocalPlayer.Center) < 3000 && Collision.CanHitLine(npc.Center, 0, 0, Main.LocalPlayer.Center, 0, 0)
                        && Math.Sign(Main.LocalPlayer.direction) == Math.Sign(npc.Center.X - Main.LocalPlayer.Center.X))
                    {
                        Vector2 target = Main.LocalPlayer.Center - Vector2.UnitY * 12;
                        Vector2 source = npc.Center - Vector2.UnitY * 6;
                        Vector2 distance = target - source;

                        int length = (int)distance.Length() / 10;
                        Vector2 offset = Vector2.Normalize(distance) * 10f;
                        for (int i = 0; i <= length; i++) //dust indicator
                        {
                            int d = Dust.NewDust(source + offset * i, 0, 0, DustID.GoldFlame, 0f, 0f, 0, new Color());
                            Main.dust[d].noLight = true;
                            Main.dust[d].noGravity = true;
                            Main.dust[d].scale = 1f;
                        }
                    }

                    if (++npc.ai[2] > 60)
                    {
                        npc.ai[2] = 0;
                        //only make rings in p2 and before firing ray
                        if (npc.localAI[3] > 1 && npc.ai[3] < 7 && !Main.player[npc.target].stoned)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                const int max = 12;
                                int damage = npc.localAI[3] > 1 ? npc.damage / 3 : npc.damage / 4;
                                for (int i = 0; i < max; i++)
                                {
                                    Projectile.NewProjectile(npc.Center, 6f * npc.DirectionTo(player.Center).RotatedBy(2 * Math.PI / max * i),
                                        ModContent.ProjectileType<DeviHeart>(), damage, 0f, Main.myPlayer);
                                }
                            }
                        }

                        if (++npc.ai[3] < 4) //medusa warning
                        {
                            npc.netUpdate = true;
                            Main.PlaySound(SoundID.ForceRoar, (int)npc.Center.X, (int)npc.Center.Y, -1, 1f, 0f); //eoc roar

                            for (int i = 0; i < 120; i++) //warning dust ring
                            {
                                Vector2 vector6 = Vector2.UnitY * 20f;
                                vector6 = vector6.RotatedBy((i - (120 / 2 - 1)) * 6.28318548f / 120) + npc.Center;
                                Vector2 vector7 = vector6 - npc.Center;
                                int d = Dust.NewDust(vector6 + vector7, 0, 0, DustID.GoldFlame, 0f, 0f, 0, default(Color), 2f);
                                Main.dust[d].noGravity = true;
                                Main.dust[d].velocity = vector7;
                            }

                            if (npc.ai[3] == 1 && Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(npc.Center, new Vector2(0, -1), ModContent.ProjectileType<DeviMedusa>(), 0, 0, Main.myPlayer);
                        }
                        else if (npc.ai[3] == 4) //petrify
                        {
                            Main.PlaySound(SoundID.NPCKilled, npc.Center, 17);

                            if (npc.Distance(Main.LocalPlayer.Center) < 3000 && Collision.CanHitLine(npc.Center, 0, 0, Main.LocalPlayer.Center, 0, 0)
                                && Math.Sign(Main.LocalPlayer.direction) == Math.Sign(npc.Center.X - Main.LocalPlayer.Center.X))
                            {
                                for (int i = 0; i < 40; i++) //petrify dust
                                {
                                    int d = Dust.NewDust(Main.LocalPlayer.Center, 0, 0, DustID.Stone, 0f, 0f, 0, default(Color), 2f);
                                    Main.dust[d].velocity *= 3f;
                                }

                                Main.LocalPlayer.AddBuff(BuffID.Stoned, 300);
                                Main.LocalPlayer.AddBuff(BuffID.Featherfall, 300);

                                Projectile.NewProjectile(Main.LocalPlayer.Center, new Vector2(0, -1), ModContent.ProjectileType<DeviMedusa>(), 0, 0, Main.myPlayer);
                            }
                        }
                        else if (npc.ai[3] < 7) //ray warning
                        {
                            npc.netUpdate = true;

                            for (int i = 0; i < 160; i++) //warning dust ring
                            {
                                Vector2 vector6 = Vector2.UnitY * 40f;
                                vector6 = vector6.RotatedBy((i - (160 / 2 - 1)) * 6.28318548f / 160) + npc.Center;
                                Vector2 vector7 = vector6 - npc.Center;
                                int d = Dust.NewDust(vector6 + vector7, 0, 0, 86, 0f, 0f, 0, default(Color), 2.5f);
                                Main.dust[d].noGravity = true;
                                Main.dust[d].velocity = vector7;
                            }
                            
                            npc.localAI[1] = npc.DirectionTo(player.Center).ToRotation(); //store for aiming ray

                            if (npc.ai[3] == 6 && Main.netMode != NetmodeID.MultiplayerClient) //final warning
                            {
                                Projectile.NewProjectile(npc.Center, Vector2.UnitX.RotatedBy(npc.localAI[1]), ModContent.ProjectileType<DeviDeathraySmall>(),
                                    0, 0f, Main.myPlayer, 0f, npc.whoAmI);
                            }
                        }
                        else if (npc.ai[3] == 7) //fire deathray
                        {
                            Main.PlaySound(SoundID.Roar, (int)npc.Center.X, (int)npc.Center.Y, 0);

                            npc.velocity = -3f * Vector2.UnitX.RotatedBy(npc.localAI[1]);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(npc.Center, Vector2.UnitX.RotatedBy(npc.localAI[1]), ModContent.ProjectileType<DeviBigDeathray>(),
                                    npc.damage / 4, 0f, Main.myPlayer, 0f, npc.whoAmI);
                            }
                        }
                    }

                    if (npc.ai[3] >= 7) //firing laser dust
                    {
                        /*for (int i = 0; i < 5; i++)
                        {
                            int d = Dust.NewDust(npc.position, npc.width, npc.height, 86, -npc.velocity.X, -npc.velocity.Y, 0, default(Color), 2.5f);
                            Main.dust[d].noGravity = true;
                            Main.dust[d].velocity *= 12f;
                        }*/
                    }
                    else //charge up dust
                    {
                        float num1 = 0.99f;
                        if (npc.ai[3] >= 1f)
                            num1 = 0.79f;
                        if (npc.ai[3] >= 2f)
                            num1 = 0.58f;
                        if (npc.ai[3] >= 3f)
                            num1 = 0.43f;
                        if (npc.ai[3] >= 4f)
                            num1 = 0.33f;
                        for (int i = 0; i < 9; ++i)
                        {
                            if (Main.rand.NextFloat() >= num1)
                            {
                                float f = Main.rand.NextFloat() * 6.283185f;
                                float num2 = Main.rand.NextFloat();
                                Dust dust = Dust.NewDustPerfect(npc.Center + f.ToRotationVector2() * (110 + 600 * num2), 86, (f - 3.141593f).ToRotationVector2() * (14 + 8 * num2), 0, default, 1f);
                                dust.scale = 0.9f;
                                dust.fadeIn = 1.15f + num2 * 0.3f;
                                //dust.color = new Color(1f, 1f, 1f, num1) * (1f - num1);
                                dust.noGravity = true;
                                //dust.noLight = true;
                            }
                        }
                    }

                    if (npc.localAI[1] != 0)
                        npc.direction = npc.spriteDirection = Math.Sign(npc.localAI[1].ToRotationVector2().X);

                    if (++npc.ai[1] > 600)//(npc.localAI[3] > 1 ? 540 : 600))
                    {
                        GetNextAttack();
                    }
                    break;

                case 15: //sparkling love
                    if (npc.localAI[0] == 0)
                    {
                        TeleportDust();
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            bool wasOnLeft = npc.Center.X < player.Center.X;
                            npc.Center = player.Center;
                            npc.position.X += wasOnLeft ? -300 : 300;
                            npc.position.Y -= 100;
                            npc.netUpdate = true;
                        }
                        TeleportDust();
                        Main.PlaySound(SoundID.Item84, npc.Center);

                        npc.localAI[0] = 1;

                        //StrongAttackTeleport();
                        if (Main.netMode != NetmodeID.MultiplayerClient) //spawn ritual for strong attacks
                        {
                            Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<DeviRitual>(), npc.damage / 4, 0f, Main.myPlayer, 0f, npc.whoAmI);
                        }
                    }
                    
                    if (++npc.ai[1] < 150)
                    {
                        npc.velocity = Vector2.Zero;

                        if (npc.ai[2] == 0) //spawn weapon, teleport
                        {
                            double angle = npc.position.X < player.position.X ? -Math.PI / 4 : Math.PI / 4;
                            npc.ai[2] = (float)angle * -4f / 30;

                            //spawn axe
                            const int loveOffset = 300;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(npc.Center + -Vector2.UnitY.RotatedBy(angle) * loveOffset, Vector2.Zero, ModContent.ProjectileType<DeviSparklingLove>(), npc.damage / 2, 0f, Main.myPlayer, npc.whoAmI, loveOffset);
                            }

                            //spawn hitboxes
                            const int spacing = 80;
                            Vector2 offset = -Vector2.UnitY.RotatedBy(angle) * spacing;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                for (int i = 0; i < 7; i++)
                                    Projectile.NewProjectile(npc.Center + offset * i, Vector2.Zero, ModContent.ProjectileType<DeviAxe>(), npc.damage / 2, 0f, Main.myPlayer, npc.whoAmI, spacing * i);
                            }
                        }

                        npc.direction = npc.spriteDirection = Math.Sign(npc.ai[2]);
                    }
                    else if (npc.ai[1] == 150) //start swinging
                    {
                        targetPos = player.Center;
                        targetPos.X -= 265 * Math.Sign(npc.ai[2]);
                        //targetPos.Y -= 200;
                        npc.velocity = (targetPos - npc.Center) / 30;
                        npc.netUpdate = true;

                        npc.direction = npc.spriteDirection = Math.Sign(npc.ai[2]);

                        if (Math.Sign(targetPos.X - npc.Center.X) != Math.Sign(npc.ai[2]))
                            npc.velocity.X *= 0.5f; //worse movement if you're behind her
                    }
                    else if (npc.ai[1] < 180)
                    {
                        npc.ai[3] += npc.ai[2];
                        npc.direction = npc.spriteDirection = Math.Sign(npc.ai[2]);
                    }
                    else
                    {
                        targetPos = player.Center + player.DirectionTo(npc.Center) * 400;
                        if (npc.Distance(targetPos) > 50)
                            Movement(targetPos, 0.2f);

                        if (npc.ai[1] > 300)
                        {
                            GetNextAttack();
                        }
                    }
                    break;

                case 16: //pause between attacks
                    if (!AliveCheck(player) || Phase2Check())
                        break;

                    npc.dontTakeDamage = false;

                    targetPos = player.Center + player.DirectionTo(npc.Center) * 200;
                    Movement(targetPos, 0.1f);
                    if (npc.Distance(player.Center) < 100)
                        Movement(targetPos, 0.5f);

                    if (++npc.ai[1] > (npc.localAI[3] > 1 ? 60 : 90))
                    {
                        npc.netUpdate = true;
                        npc.ai[0] = attackQueue[(int)npc.localAI[2]];
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;
                        npc.localAI[0] = 0;
                        npc.localAI[1] = 0;

                        int threshold = attackQueue.Length; //only do super attacks in maso
                        if (!FargoSoulsWorld.MasochistMode)
                            threshold -= 1;
                        if (++npc.localAI[2] >= threshold)
                        {
                            npc.localAI[2] = npc.localAI[3] > 1 ? 1 : 0;
                            RefreshAttackQueue();
                        }
                    }
                    break;

                default:
                    Main.NewText("UH OH, STINKY");
                    npc.netUpdate = true;
                    npc.ai[0] = 0;
                    goto case 0;
            }

            if (player.immune || player.hurtCooldowns[0] != 0 || player.hurtCooldowns[1] != 0)
                playerInvulTriggered = true;

            //drop summon
            if (!FargoSoulsWorld.downedDevi && Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget && !droppedSummon)
            {
                Item.NewItem(player.Hitbox, ModContent.ItemType<DevisCurse>());
                droppedSummon = true;
            }
        }

        private void GetNextAttack()
        {
            npc.TargetClosest();
            npc.netUpdate = true;
            npc.ai[0] = 16;// attackQueue[(int)npc.localAI[2]];
            npc.ai[1] = 0;
            npc.ai[2] = 0;
            npc.ai[3] = 0;
            npc.localAI[0] = 0;
            npc.localAI[1] = 0;
        }

        private void RefreshAttackQueue()
        {
            npc.netUpdate = true;

            int[] newQueue = new int[4];
            for (int i = 0; i < 3; i++)
            {
                newQueue[i] = Main.rand.Next(1, 11);

                bool repeat = false;
                if (newQueue[i] == 8) //npc is the middle of an attack pattern, dont pick it
                    repeat = true;
                for (int j = 0; j < 3; j++) //cant pick attack that's queued in the previous set
                    if (newQueue[i] == attackQueue[j])
                        repeat = true;
                for (int j = i; j >= 0; j--) //can't pick attack that's already queued in npc set
                    if (i != j && newQueue[i] == newQueue[j])
                        repeat = true;

                if (repeat) //retry npc one if needed
                    i--;
            }

            do
            {
                newQueue[3] = Main.rand.Next(11, 16);
            }
            while (newQueue[3] == attackQueue[3] || newQueue[3] == lastStrongAttack || (newQueue[3] == 15 && npc.localAI[3] <= 1));
            //don't do sparkling love in p1

            lastStrongAttack = attackQueue[3]; //a strong attack can't be used again for the next 2 checks
            attackQueue = newQueue;

            /*Main.NewText("queue: "
                + attackQueue[0].ToString() + " "
                + attackQueue[1].ToString() + " "
                + attackQueue[2].ToString() + " "
                + attackQueue[3].ToString());*/
        }

        private void Aura(float distance, int buff, bool reverse = false, int dustid = DustID.GoldFlame, bool checkDuration = false)
        {
            //works because buffs are client side anyway :ech:
            Player p = Main.player[Main.myPlayer];
            float range = npc.Distance(p.Center);
            if (reverse ? range > distance && range < 5000f : range < distance)
                p.AddBuff(buff, checkDuration && Main.expertMode && Main.expertDebuffTime > 1 ? 1 : 2);

            for (int i = 0; i < 30; i++)
            {
                Vector2 offset = new Vector2();
                double angle = Main.rand.NextDouble() * 2d * Math.PI;
                offset.X += (float)(Math.Sin(angle) * distance);
                offset.Y += (float)(Math.Cos(angle) * distance);
                Dust dust = Main.dust[Dust.NewDust(
                    npc.Center + offset - new Vector2(4, 4), 0, 0,
                    dustid, 0, 0, 100, Color.White, 1.5f)];
                dust.velocity = npc.velocity;
                if (Main.rand.Next(3) == 0)
                    dust.velocity += Vector2.Normalize(offset) * (reverse ? 5f : -5f);
                dust.noGravity = true;
            }
        }

        private bool AliveCheck(Player player)
        {
            if ((!player.active || player.dead || Vector2.Distance(npc.Center, player.Center) > 5000f) && npc.localAI[3] > 0)
            {
                npc.TargetClosest();
                player = Main.player[npc.target];
                if (!player.active || player.dead || Vector2.Distance(npc.Center, player.Center) > 5000f)
                {
                    if (npc.timeLeft > 30)
                        npc.timeLeft = 30;
                    npc.velocity.Y -= 1f;
                    if (npc.timeLeft == 1)
                    {
                        if (npc.position.Y < 0)
                            npc.position.Y = 0;
                        if (Main.netMode != NetmodeID.MultiplayerClient && !NPC.AnyNPCs(ModLoader.GetMod("Fargowiltas").NPCType("Deviantt")))
                        {
                            for (int i = 0; i < 1000; i++)
                                if (Main.projectile[i].active && Main.projectile[i].hostile)
                                    Main.projectile[i].Kill();
                            for (int i = 0; i < 1000; i++)
                                if (Main.projectile[i].active && Main.projectile[i].hostile)
                                    Main.projectile[i].Kill();
                            int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModLoader.GetMod("Fargowiltas").NPCType("Deviantt"));
                            if (n != 200 && Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                        }
                    }
                    return false;
                }
            }
            if (npc.timeLeft < 600)
                npc.timeLeft = 600;
            return true;
        }

        private bool Phase2Check()
        {
            if (npc.localAI[3] > 1)
                return false;

            if (npc.life < npc.lifeMax * 0.5 && Main.expertMode)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.ai[0] = -1;
                    npc.ai[1] = 0;
                    npc.ai[2] = 0;
                    npc.ai[3] = 0;
                    npc.localAI[0] = 0;
                    npc.localAI[1] = 0;
                    npc.netUpdate = true;
                    for (int i = 0; i < 1000; i++)
                        if (Main.projectile[i].active && Main.projectile[i].hostile)
                            Main.projectile[i].Kill();
                    for (int i = 0; i < 1000; i++)
                        if (Main.projectile[i].active && Main.projectile[i].hostile)
                            Main.projectile[i].Kill();
                }
                return true;
            }
            return false;
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

        private void TeleportDust()
        {
            for (int index1 = 0; index1 < 25; ++index1)
            {
                int index2 = Dust.NewDust(npc.position, npc.width, npc.height, 272, 0f, 0f, 100, new Color(), 2f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity *= 7f;
                Main.dust[index2].noLight = true;
                int index3 = Dust.NewDust(npc.position, npc.width, npc.height, 272, 0f, 0f, 100, new Color(), 1f);
                Main.dust[index3].velocity *= 4f;
                Main.dust[index3].noGravity = true;
                Main.dust[index3].noLight = true;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(mod.BuffType("Lovestruck"), 240);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int i = 0; i < 3; i++)
            {
                int d = Dust.NewDust(npc.position, npc.width, npc.height, 86, 0f, 0f, 0, default(Color), 1f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 3f;
            }
        }

        public override void ModifyHitByItem(Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            if (item.melee && !ContentModLoaded)
                damage = (int)(damage * 1.25);
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if ((projectile.melee || projectile.minion) && !ContentModLoaded)
                damage = (int)(damage * 1.25);
        }

        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if (Main.LocalPlayer.loveStruck)
            {
                npc.life += (int)damage;
                if (npc.life > npc.lifeMax)
                    npc.life = npc.lifeMax;
                CombatText.NewText(npc.Hitbox, CombatText.HealLife, (int)damage);
                npc.netUpdate = true;

                damage = 0;
                return false;
            }
            return true;
        }

        public override bool CheckDead()
        {
            if (npc.ai[0] == -2 && npc.ai[1] >= 180)
                return true;

            npc.life = 1;
            npc.active = true;
            if (npc.localAI[3] < 2)
            {
                npc.localAI[3] = 2;
                /*if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<AbomRitual>(), npc.damage / 2, 0f, Main.myPlayer, 0f, npc.whoAmI);
                }*/
            }
            if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[0] > -2)
            {
                npc.ai[0] = -2;
                npc.ai[1] = 0;
                npc.ai[2] = 0;
                npc.ai[3] = 0;
                npc.localAI[0] = 0;
                npc.localAI[1] = 0;
                npc.dontTakeDamage = true;
                npc.netUpdate = true;
                for (int i = 0; i < 1000; i++)
                    if (Main.projectile[i].active && Main.projectile[i].damage > 0 && Main.projectile[i].hostile)
                        Main.projectile[i].Kill();
                for (int i = 0; i < 1000; i++)
                    if (Main.projectile[i].active && Main.projectile[i].damage > 0 && Main.projectile[i].hostile)
                        Main.projectile[i].Kill();
            }
            return false;
        }

        public override void NPCLoot()
        {
            if (!playerInvulTriggered && FargoSoulsWorld.MasochistMode)
            {
                Item.NewItem(npc.Hitbox, mod.ItemType("BrokenBlade"));
                Item.NewItem(npc.Hitbox, mod.ItemType("ChibiHat"));
            }

            FargoSoulsWorld.downedDevi = true;
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData); //sync world
            
            if (FargoSoulsWorld.MasochistMode)
            {
                npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("SparklingAdoration"));
            }

            npc.DropItemInstanced(npc.position, npc.Size, ModContent.ItemType<Items.Misc.DeviBag>());

            if (Main.rand.Next(10) == 0)
                Item.NewItem(npc.Hitbox, mod.ItemType("DeviTrophy"));
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.HealingPotion;
        }

        public override void FindFrame(int frameHeight)
        {
            if (++npc.frameCounter > 6)
            {
                npc.frameCounter = 0;
                npc.frame.Y += frameHeight;
                if (npc.frame.Y >= 4 * frameHeight)
                    npc.frame.Y = 0;
            }
        }

        public override void BossHeadSpriteEffects(ref SpriteEffects spriteEffects)
        {
            //spriteEffects = npc.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.npcTexture[npc.type];
            Rectangle rectangle = npc.frame;
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = npc.GetAlpha(color26);

            SpriteEffects effects = npc.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.spriteBatch.Draw(texture2D13, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), npc.GetAlpha(lightColor), npc.rotation, origin2, npc.scale, effects, 0f);
            return false;
        }
    }
}