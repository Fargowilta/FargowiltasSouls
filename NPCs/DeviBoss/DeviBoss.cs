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
        public bool ignoreMoney;

        public int ringProj, spriteProj;

        private bool ContentModLoaded => Fargowiltas.Instance.CalamityLoaded || Fargowiltas.Instance.ThoriumLoaded
            || Fargowiltas.Instance.SoALoaded || Fargowiltas.Instance.MasomodeEXLoaded;

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Deviantt");
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
            if (FargoSoulsWorld.MasochistModeReal)
                npc.lifeMax = (int)(npc.lifeMax * 1.2);
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
            return npc.Distance(FargoSoulsUtil.ClosestPointInHitbox(target, npc.Center)) < Player.defaultHeight;
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
            writer.Write(ignoreMoney);
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
            ignoreMoney = reader.ReadBoolean();
        }

        private bool ProjectileExists(int id, int type)
        {
            return id > -1 && id < Main.maxProjectiles && Main.projectile[id].active && Main.projectile[id].type == type;
        }

        public override void AI()
        {
            EModeGlobalNPC.deviBoss = npc.whoAmI;

            const int platinumToBribe = 10;

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

            void StrongAttackTeleport(Vector2 teleportTarget = default)
            {
                const float range = 450f;
                if (npc.Distance(player.Center) < range)
                    return;

                TeleportDust();
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (teleportTarget != default)
                        npc.Center = teleportTarget;
                    else if (player.velocity == Vector2.Zero)
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
                                if (n != Main.maxNPCs)
                                {
                                    Main.npc[n].homeless = true;
                                    if (Main.netMode == NetmodeID.Server)
                                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                                }
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
                        if (FargoSoulsWorld.EternityMode)
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
                        //npc.localAI[1] = 0;
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

                        if (npc.ai[2] == npc.localAI[1])
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                for (int i = -1; i <= 1; i += 2)
                                {
                                    for (int j = 0; j < 3; j++)
                                    {
                                        float ai1 = MathHelper.ToRadians(90 + 15) - MathHelper.ToRadians(30) * j;
                                        ai1 *= i;
                                        ai1 = ai1 / 60 * 2;
                                        Projectile.NewProjectile(npc.Center, -Vector2.UnitY, ModContent.ProjectileType<DeviHammerHeld>(), npc.damage / 4, 0f, Main.myPlayer, npc.whoAmI, ai1);
                                    }
                                }
                            }
                        }
                    }

                    if (npc.ai[1] == 60) //finished all the prior teleports, now attack
                    {
                        npc.netUpdate = true;
                        
                        FargoSoulsUtil.DustRing(npc.Center, 36, 246, 9f, default, 3f, true);

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
                                Dust.NewDust(npc.Center, 0, 0, Main.rand.NextBool() ? DustID.GoldFlame : DustID.SilverCoin, speed.X, speed.Y, 0, default(Color), 2f);

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
                                Dust.NewDust(npc.Center, 0, 0, Main.rand.NextBool() ? DustID.GoldFlame : DustID.SilverCoin, speed.X, speed.Y, 0, default(Color), 2f);

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
                    
                    EModeGlobalNPC.Aura(npc, 450, true, 74, Color.GreenYellow, mod.BuffType("Hexed"), mod.BuffType("Crippled"), BuffID.Dazed, BuffID.OgreSpit);
                    EModeGlobalNPC.Aura(npc, 150, false, 73, default, mod.BuffType("Hexed"), mod.BuffType("Crippled"), BuffID.Dazed, BuffID.OgreSpit);

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
                                    if (p != Main.maxProjectiles)
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

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(npc.Center, new Vector2(npc.Center.X < player.Center.X ? -1f : 1f, -1f),
                                ModContent.ProjectileType<DeviSparklingLoveSmall>(), npc.damage / 4, 0f, Main.myPlayer, npc.whoAmI, 0.0001f * Math.Sign(player.Center.X - npc.Center.X));
                        }
                    }

                    if (++npc.ai[3] > 2)
                    {
                        npc.ai[3] = 0;

                        if (Main.netMode != NetmodeID.MultiplayerClient) //make moth dust trail
                            Projectile.NewProjectile(npc.Center, Main.rand.NextVector2Unit() * 2f, ModContent.ProjectileType<MothDust>(), projectileDamage, 0f, Main.myPlayer);
                    }

                    npc.velocity *= 0.9f;
                    if (++npc.ai[1] > (npc.localAI[3] > 1 ? 45 : 60))
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
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                float rotation = MathHelper.Pi * 1.5f * (npc.ai[2] % 2 == 0 ? 1 : -1);
                                Projectile.NewProjectile(npc.Center, Vector2.Normalize(npc.velocity).RotatedBy(-rotation / 2),
                                    ModContent.ProjectileType<DeviSparklingLoveSmall>(), npc.damage / 4, 0f, Main.myPlayer, npc.whoAmI, rotation / 60 * 2);
                            }
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

                    npc.velocity = npc.DirectionTo(player.Center) * 2f;

                    if (++npc.ai[1] == 1)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, npc.whoAmI, -1);
                            Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowLine>(), 0, 0f, Main.myPlayer, 10, npc.whoAmI);
                            Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowLine>(), 0, 0f, Main.myPlayer, 10, npc.whoAmI);
                        }
                        Main.PlaySound(SoundID.Roar, (int)npc.Center.X, (int)npc.Center.Y, 0);
                    }
                    else if (npc.ai[1] < 120) //spam shadowbeams after delay
                    {
                        if (npc.ai[3] <= 0) //store rotation briefly before shooting
                            npc.localAI[0] = npc.DirectionTo(player.Center).ToRotation();

                        if (++npc.ai[2] > 90)
                        {
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
                            npc.position.X += 500 * (Main.rand.NextBool() ? -1 : 1);
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
                        StrongAttackTeleport(player.Center + new Vector2(player.Center.X - npc.Center.X, -Math.Abs(player.Center.Y - npc.Center.Y)));

                        if (Main.netMode != NetmodeID.MultiplayerClient) //spawn ritual for strong attacks
                            Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<DeviRitual>(), npc.damage / 4, 0f, Main.myPlayer, 0f, npc.whoAmI);

                        npc.localAI[0] = npc.Center.X;
                        npc.localAI[1] = npc.Center.Y;
                        npc.netUpdate = true;
                    }

                    //Main.NewText(npc.localAI[0].ToString() + ", " + npc.localAI[1].ToString());

                    targetPos = player.Center;
                    if (npc.Center.Y > player.Center.Y)
                        targetPos.X += 300 * (npc.Center.X < targetPos.X ? -1 : 1);
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
                            npc.ai[3] = FargoSoulsWorld.MasochistModeReal ? 70 : 85;

                            npc.ai[2] = npc.ai[2] == 1 ? -1 : 1;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Vector2 speed = 24 * Vector2.UnitY.RotatedBy(MathHelper.ToRadians(10) * npc.ai[2]);
                                int type = npc.localAI[3] > 1 ? ModContent.ProjectileType<DeviRainHeart2>() : ModContent.ProjectileType<DeviRainHeart>();
                                int damage = npc.localAI[3] > 1 ? npc.damage / 3 : npc.damage / 4;
                                int range = npc.localAI[3] > 1 ? 8 : 10;
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
                        if (++npc.ai[3] > (FargoSoulsWorld.MasochistModeReal ? 3 : 5))
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
                                float rotation = FargoSoulsWorld.MasochistModeReal ? MathHelper.ToRadians(Main.rand.NextFloat(-10, 10)) : 0;

                                Projectile.NewProjectile(npc.Center, speed, ModContent.ProjectileType<DeviEnergyHeart>(),
                                    damage, 0f, Main.myPlayer, rotation, acceleration);
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
                            int max = 8;
                            for (int i = 0; i < max; i++) //make butterflies
                            {
                                Vector2 speed = new Vector2(Main.rand.NextFloat(40f), Main.rand.NextFloat(-20f, 20f));
                                Projectile.NewProjectile(npc.Center, speed, ModContent.ProjectileType<DeviButterfly>(),
                                   damage, 0f, Main.myPlayer, npc.whoAmI, 300 / 4 * i + offset);
                            }
                        }
                        
                        if (Main.netMode != NetmodeID.MultiplayerClient) //spawn ritual for strong attacks
                            Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<DeviRitual>(), npc.damage / 4, 0f, Main.myPlayer, 0f, npc.whoAmI);
                    }

                    if (npc.ai[1] % 240 == 90)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int max = npc.localAI[3] > 1 ? 8 : 12;
                            Vector2 vel = Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2();
                            for (int i = 0; i < max; i++)
                            {
                                Projectile.NewProjectile(npc.Center, vel.RotatedBy(2 * Math.PI / max * i), ModContent.ProjectileType<DeviLightBall>(), projectileDamage, 0f, Main.myPlayer, 0f, .008f * npc.direction);
                                if (npc.localAI[3] > 1)
                                    Projectile.NewProjectile(npc.Center, vel.RotatedBy(2 * Math.PI / max * i), ModContent.ProjectileType<DeviLightBall>(), projectileDamage, 0f, Main.myPlayer, 0f, .008f * -npc.direction);
                            }
                        }
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

                    if (++npc.ai[2] > (FargoSoulsWorld.MasochistModeReal ? 30 : 60))
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

                            FargoSoulsUtil.DustRing(npc.Center, 120, 228, 20f, default, 2f);

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

                            FargoSoulsUtil.DustRing(npc.Center, 160, 86, 40f, default, 2.5f);
                            
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

                            const int ring = 160;
                            for (int i = 0; i < ring; ++i)
                            {
                                Vector2 vector2 = (-Vector2.UnitY.RotatedBy(i * 3.14159274101257 * 2 / ring) * new Vector2(8f, 16f)).RotatedBy(npc.velocity.ToRotation());
                                int index2 = Dust.NewDust(npc.Center, 0, 0, 86, 0.0f, 0.0f, 0, new Color(), 1f);
                                Main.dust[index2].scale = 5f;
                                Main.dust[index2].noGravity = true;
                                Main.dust[index2].position = npc.Center;
                                Main.dust[index2].velocity = vector2 * 3f;
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
                        StrongAttackTeleport(player.Center + new Vector2(300 * Math.Sign(npc.Center.X - player.Center.X), -100));

                        npc.localAI[0] = 1;

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
                            const int loveOffset = 90;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(npc.Center + -Vector2.UnitY.RotatedBy(angle) * loveOffset, Vector2.Zero, ModContent.ProjectileType<DeviSparklingLove>(), npc.damage / 2, 0f, Main.myPlayer, npc.whoAmI, loveOffset);
                            }

                            //spawn hitboxes
                            const int spacing = 80;
                            Vector2 offset = -Vector2.UnitY.RotatedBy(angle) * spacing;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                void SpawnAxeHitbox(Vector2 spawnPos)
                                {
                                    Projectile.NewProjectile(spawnPos, Vector2.Zero, ModContent.ProjectileType<DeviAxe>(), npc.damage / 2, 0f, Main.myPlayer, npc.whoAmI, npc.Distance(spawnPos));
                                }

                                for (int i = 0; i < 8; i++)
                                    SpawnAxeHitbox(npc.Center + offset * i);
                                for (int i = 1; i < 3; i++)
                                {
                                    SpawnAxeHitbox(npc.Center + offset * 5 + offset.RotatedBy(-angle * 2) * i);
                                    SpawnAxeHitbox(npc.Center + offset * 6 + offset.RotatedBy(-angle * 2) * i);
                                }
                            }

                            if (FargoSoulsWorld.MasochistModeReal && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                for (int i = 0; i < 4; i++)
                                {
                                    Vector2 target = new Vector2(80f, 80f).RotatedBy(MathHelper.Pi / 2 * i);

                                    Vector2 speed = 2 * target / 90;
                                    float acceleration = -speed.Length() / 90;

                                    int damage = npc.localAI[3] > 1 ? npc.damage / 3 : npc.damage / 4;

                                    Projectile.NewProjectile(npc.Center, speed, ModContent.ProjectileType<DeviEnergyHeart>(),
                                        damage, 0f, Main.myPlayer, 0, acceleration);
                                }
                            }
                        }

                        npc.direction = npc.spriteDirection = Math.Sign(npc.ai[2]);
                    }
                    else if (npc.ai[1] == 150) //start swinging
                    {
                        targetPos = player.Center;
                        targetPos.X -= 360 * Math.Sign(npc.ai[2]);
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
                    {
                        if (!AliveCheck(player) || Phase2Check())
                            break;

                        npc.dontTakeDamage = false;

                        targetPos = player.Center + player.DirectionTo(npc.Center) * 200;
                        Movement(targetPos, 0.1f);
                        if (npc.Distance(player.Center) < 100)
                            Movement(targetPos, 0.5f);

                        int delay = 180;
                        if (FargoSoulsWorld.MasochistModeReal)
                        {
                            delay -= 60;
                            ignoreMoney = true;
                        }
                        if (FargoSoulsWorld.EternityMode)
                            delay -= 60;
                        if (npc.localAI[3] > 1)
                            delay -= 30;
                        if (++npc.ai[1] > delay)
                        {
                            npc.netUpdate = true;
                            npc.ai[0] = 16; //placeholder
                            npc.ai[1] = 0;
                            npc.ai[2] = 0;
                            npc.ai[3] = 0;
                            npc.localAI[0] = 0;
                            npc.localAI[1] = 0;

                            if (!ignoreMoney && npc.extraValue > Item.buyPrice(platinumToBribe))
                            {
                                npc.ai[0] = 17;
                            }
                            else
                            {
                                npc.ai[0] = attackQueue[(int)npc.localAI[2]];

                                int threshold = attackQueue.Length; //only do super attacks in maso
                                if (!FargoSoulsWorld.EternityMode)
                                    threshold -= 1;
                                if (++npc.localAI[2] >= threshold)
                                {
                                    npc.localAI[2] = npc.localAI[3] > 1 ? 1 : 0;
                                    RefreshAttackQueue();
                                }
                            }
                        }
                    }
                    break;

                case 17: //i got money
                    {
                        npc.dontTakeDamage = true;
                        npc.velocity *= 0.95f;
                        if (npc.timeLeft < 600)
                            npc.timeLeft = 600;

                        if (npc.buffType[0] != 0)
                            npc.DelBuff(0);

                        Rectangle displayPoint = new Rectangle(npc.Hitbox.Center.X, npc.Hitbox.Center.Y - npc.height / 4, 2, 2);
                        
                        if (npc.ai[1] == 0)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient) //clear my arena
                            {
                                for (int i = 0; i < Main.maxProjectiles; i++)
                                {
                                    if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<DeviRitual>() && Main.projectile[i].ai[1] == npc.whoAmI)
                                    {
                                        Main.projectile[i].Kill();
                                    }
                                }
                            }
                        }
                        else if (npc.ai[1] == 60)
                        {
                            CombatText.NewText(displayPoint, Color.HotPink, "Wait a second...");
                        }
                        else if (npc.ai[1] == 150)
                        {
                            CombatText.NewText(displayPoint, Color.HotPink, "You're throwing money at me!", true);
                        }
                        else if (npc.ai[1] == 300)
                        {
                            CombatText.NewText(displayPoint, Color.HotPink, "Trying to bribe me is pretty gutsy, you know!", true);
                        }
                        else if (npc.ai[1] == 450)
                        {
                            if (FargoSoulsWorld.downedDevi)
                            {
                                CombatText.NewText(displayPoint, Color.HotPink, "Then again, this is a LOT of money...");
                            }
                            else
                            {
                                CombatText.NewText(displayPoint, Color.HotPink, "Show you're tough enough and I won't mind, but not a second before!", true);
                            }
                        }
                        else if (npc.ai[1] == 600)
                        {
                            if (FargoSoulsWorld.downedDevi)
                            {
                                CombatText.NewText(displayPoint, Color.HotPink, "But my big bro said not to! What to do, what to do...?", true);
                            }
                            else
                            {
                                CombatText.NewText(displayPoint, Color.HotPink, "Here, you can have this back.", true);

                                Main.PlaySound(SoundID.Item28, player.Center);
                                Vector2 spawnPos = npc.Center + Vector2.UnitX * npc.width * 2 * (player.Center.X < npc.Center.X ? -1 : 1);
                                for (int i = 0; i < 30; i++)
                                {
                                    int d = Dust.NewDust(spawnPos, 0, 0, 66, 0, 0, 0, default, 1f);
                                    Main.dust[d].noGravity = true;
                                    Main.dust[d].velocity *= 6f;
                                }
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(spawnPos, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, -1, -21);

                                    Item.NewItem(spawnPos, ItemID.PlatinumCoin, platinumToBribe);
                                }
                            }

                            npc.extraValue -= Item.buyPrice(platinumToBribe);
                        }
                        else if (npc.ai[1] == 900)
                        {
                            if (FargoSoulsWorld.downedDevi)
                            {
                                CombatText.NewText(displayPoint, Color.HotPink, "Aw, what the heck! But this is our little secret, okay?", true);
                            }
                            else
                            {
                                CombatText.NewText(displayPoint, Color.HotPink, "Let's get this show back on the road!", true);
                            }
                        }

                        if (++npc.ai[1] > 1050)
                        {
                            ignoreMoney = true;
                            if (FargoSoulsWorld.downedDevi)
                            {
                                npc.life = 0;
                                npc.checkDead();
                            }
                            else
                            {
                                npc.ai[0] = 16;
                                npc.ai[1] = 0;
                                npc.velocity = 20f * npc.DirectionFrom(player.Center);
                            }
                            npc.netUpdate = true;
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
                if (Main.rand.NextBool(3))
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
                            FargoSoulsUtil.ClearHostileProjectiles(2, npc.whoAmI);
                            int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModLoader.GetMod("Fargowiltas").NPCType("Deviantt"));
                            if (n != Main.maxNPCs)
                            {
                                Main.npc[n].homeless = true;
                                if (Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                            }
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

                    FargoSoulsUtil.ClearHostileProjectiles(2, npc.whoAmI);
                }
                return true;
            }
            return false;
        }

        private void Movement(Vector2 targetPos, float speedModifier, float cap = 12f)
        {
            if (Math.Abs(npc.Center.X - targetPos.X) > 10)
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
            //if (item.melee && !ContentModLoaded) damage = (int)(damage * 1.25);
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            //if ((projectile.melee || projectile.minion) && !ContentModLoaded) damage = (int)(damage * 1.25);
        }

        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if (Main.LocalPlayer.active && Main.LocalPlayer.loveStruck)
            {
                /*npc.life += (int)damage;
                if (npc.life > npc.lifeMax)
                    npc.life = npc.lifeMax;
                CombatText.NewText(npc.Hitbox, CombatText.HealLife, (int)damage);*/
                npc.netUpdate = true;

                Vector2 speed = Main.rand.NextFloat(1, 2) * Vector2.UnitX.RotatedByRandom(Math.PI * 2);
                float ai1 = 30 + Main.rand.Next(30);
                Projectile.NewProjectile(Main.LocalPlayer.Center, speed, ModContent.ProjectileType<HostileHealingHeart>(), (int)damage, 0f, Main.myPlayer, npc.whoAmI, ai1);

                damage = 1;
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
                FargoSoulsUtil.ClearHostileProjectiles(2, npc.whoAmI);
            }
            return false;
        }

        public override void NPCLoot()
        {
            if (!playerInvulTriggered && FargoSoulsWorld.EternityMode)
            {
                Item.NewItem(npc.Hitbox, mod.ItemType("BrokenBlade"));
                Item.NewItem(npc.Hitbox, mod.ItemType("ChibiHat"));
            }

            FargoSoulsWorld.downedDevi = true;
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData); //sync world
            
            if (FargoSoulsWorld.EternityMode)
            {
                npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("SparklingAdoration"));
            }

            if (Main.expertMode)
            {
                npc.DropItemInstanced(npc.position, npc.Size, ModContent.ItemType<Items.Misc.DeviBag>());
            }
            else
            {
                Item.NewItem(npc.Hitbox, mod.ItemType("DeviatingEnergy"), Main.rand.Next(16) + 15);
            }

            if (Main.rand.NextBool(10))
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