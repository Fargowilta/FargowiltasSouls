using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Items.Accessories.Enchantments;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.Projectiles;
using FargowiltasSouls.Projectiles.Champions;
using FargowiltasSouls.Items.Misc;

namespace FargowiltasSouls.NPCs.Champions
{
    [AutoloadBossHead]
    public class CosmosChampion : ModNPC
    {
        bool hitChildren;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eridanus, Champion of Cosmos");
            DisplayName.AddTranslation(GameCulture.Chinese, "厄里达诺斯,宇宙英灵");
            Main.npcFrameCount[npc.type] = 9;
            NPCID.Sets.TrailCacheLength[npc.type] = 6;
            NPCID.Sets.TrailingMode[npc.type] = 1;
        }

        public override void SetDefaults()
        {
            npc.width = 80;
            npc.height = 100;
            npc.damage = 150;
            npc.defense = 70;
            npc.lifeMax = 500000;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath7;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.knockBackResist = 0f;
            npc.lavaImmune = true;
            npc.aiStyle = -1;
            npc.value = Item.buyPrice(0, 30);
            npc.boss = true;

            npc.buffImmune[BuffID.Chilled] = true;
            npc.buffImmune[BuffID.OnFire] = true;
            npc.buffImmune[BuffID.Suffocation] = true;
            npc.buffImmune[BuffID.Lovestruck] = true;
            npc.buffImmune[mod.BuffType("Lethargic")] = true;
            npc.buffImmune[mod.BuffType("ClippedWings")] = true;
            npc.buffImmune[mod.BuffType("TimeFrozen")] = true;
            npc.buffImmune[mod.BuffType("LightningRod")] = true;
            npc.GetGlobalNPC<FargoSoulsGlobalNPC>().SpecialEnchantImmune = true;

            Mod musicMod = ModLoader.GetMod("FargowiltasMusic");
            music = musicMod != null ? ModLoader.GetMod("FargowiltasMusic").GetSoundSlot(SoundType.Music, "Sounds/Music/Champions") : MusicID.LunarBoss;
            musicPriority = MusicPriority.BossHigh;

            npc.scale *= 1.5f;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            //npc.damage = (int)(npc.damage * 0.5f);
            npc.lifeMax = (int)(npc.lifeMax * bossLifeScale);
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return true;
        }

        /*public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (npc.ai[0] == 15 && npc.ai[1] > 90 && npc.ai[1] < 210) //intangible during timestop
                return false;
            return null;
        }*/

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(npc.localAI[0]);
            writer.Write(npc.localAI[1]);
            writer.Write(npc.localAI[2]);
            writer.Write(npc.localAI[3]);
            writer.Write(hitChildren);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc.localAI[0] = reader.ReadSingle();
            npc.localAI[1] = reader.ReadSingle();
            npc.localAI[2] = reader.ReadSingle();
            npc.localAI[3] = reader.ReadSingle();
            hitChildren = reader.ReadBoolean();
        }

        public override void AI()
        {
            if (npc.localAI[3] == 0) //just spawned
            {
                if (npc.HasValidTarget && npc.Distance(Main.player[npc.target].Center) < 1500)
                    npc.localAI[3] = 1;
                npc.TargetClosest(false);
            }

            EModeGlobalNPC.championBoss = npc.whoAmI;

            Player player = Main.player[npc.target];
            Vector2 targetPos;

            if ((npc.HasValidTarget && npc.Distance(player.Center) < 2500) || npc.localAI[3] == 0)
                npc.timeLeft = 600;

            npc.direction = npc.spriteDirection = npc.Center.X < player.Center.X ? 1 : -1;

            if (npc.localAI[2] == 0 && npc.ai[0] != -1 && npc.life < npc.lifeMax * (FargoSoulsWorld.MasochistMode ? .8 : .5))
            {
                if (npc.ai[0] == 15 && npc.ai[1] < 210 + 60) //dont phase transition during timestop
                {
                    npc.life = (int)(npc.lifeMax * (FargoSoulsWorld.MasochistMode ? .8 : .5));
                }
                else
                {
                    float buffer = npc.ai[0];
                    npc.ai[0] = -1;
                    npc.ai[1] = 0;
                    npc.ai[2] = 0;
                    npc.ai[3] = buffer;
                    npc.netUpdate = true;

                    if (Main.netMode != NetmodeID.MultiplayerClient && !EModeGlobalNPC.OtherBossAlive(npc.whoAmI)) //clear projs
                    {
                        for (int i = 0; i < Main.maxProjectiles; i++)
                        {
                            if (Main.projectile[i].active && Main.projectile[i].hostile && Main.projectile[i].damage > 0)
                                Main.projectile[i].Kill();
                        }
                        for (int i = 0; i < Main.maxProjectiles; i++)
                        {
                            if (Main.projectile[i].active && Main.projectile[i].hostile && Main.projectile[i].damage > 0)
                                Main.projectile[i].Kill();
                        }
                    }
                }
            }

            if (FargoSoulsWorld.MasochistMode && npc.localAI[2] < 2 && npc.ai[0] != -2 && npc.life < npc.lifeMax * .2)
            {
                if (npc.ai[0] == 15 && npc.ai[1] < 210 + 60) //dont phase transition during timestop
                {
                    npc.life = (int)(npc.lifeMax * .2);
                }
                else
                {
                    npc.ai[0] = -2;
                    npc.ai[1] = 0;
                    npc.ai[2] = 0;
                    npc.ai[3] = 0;
                    npc.localAI[0] = 0;
                    npc.localAI[1] = 0;
                    npc.netUpdate = true;

                    if (Main.netMode != NetmodeID.MultiplayerClient && !EModeGlobalNPC.OtherBossAlive(npc.whoAmI)) //clear projs
                    {
                        for (int i = 0; i < Main.maxProjectiles; i++)
                        {
                            if (Main.projectile[i].active && Main.projectile[i].hostile && Main.projectile[i].damage > 0)
                                Main.projectile[i].Kill();
                        }
                        for (int i = 0; i < Main.maxProjectiles; i++)
                        {
                            if (Main.projectile[i].active && Main.projectile[i].hostile && Main.projectile[i].damage > 0)
                                Main.projectile[i].Kill();
                        }
                    }
                }
            }

            npc.dontTakeDamage = false;

            switch ((int)npc.ai[0])
            {
                case -4: //hit children
                    {
                        npc.timeLeft = 600;

                        int ai2 = (int)npc.ai[2];
                        if (++npc.ai[3] < 360 && ai2 > -1 && ai2 < Main.maxNPCs && Main.npc[ai2].active && Main.npc[ai2].type == ModLoader.GetMod("Fargowiltas").NPCType("Deviantt"))
                        {
                            targetPos = Main.npc[ai2].Center;
                            npc.direction = npc.spriteDirection = npc.Center.X < targetPos.X ? 1 : -1;

                            targetPos.X += npc.width / 4 * (npc.Center.X < targetPos.X ? -1 : 1);
                            if (npc.Distance(targetPos) > npc.width / 4)
                                Movement(targetPos, 1.6f, 64f);

                            if (npc.localAI[1] == 0)
                            {
                                npc.localAI[1] = 1;
                                Main.PlaySound(SoundID.Roar, npc.Center, 0);
                            }

                            if (++npc.localAI[0] <= 5)
                            {
                                npc.rotation = npc.DirectionTo(Main.npc[ai2].Center).ToRotation();
                                if (npc.direction < 0)
                                    npc.rotation += (float)Math.PI;

                                if (npc.localAI[0] == 5)
                                {
                                    npc.netUpdate = true;
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        Vector2 offset = Vector2.UnitX;
                                        if (npc.direction < 0)
                                            offset.X *= -1f;
                                        offset = offset.RotatedBy(npc.DirectionTo(Main.npc[ai2].Center).ToRotation());

                                        int modifier = Math.Sign(npc.Center.Y - Main.npc[ai2].Center.Y);
                                        Projectile.NewProjectile(npc.Center + offset + 3000 * npc.DirectionFrom(Main.npc[ai2].Center) * modifier,
                                            npc.DirectionTo(Main.npc[ai2].Center) * modifier,
                                            ModContent.ProjectileType<CosmosDeathray>(), npc.damage / 4, 0f, Main.myPlayer);
                                    }
                                }
                            }
                            else
                            {
                                if (npc.localAI[0] > 10)
                                {
                                    npc.localAI[0] = 0;
                                    npc.netUpdate = true;
                                }
                            }
                        }
                        else
                        {
                            if (npc.ai[3] >= 360) //if couldn't kill deviantt in 6 seconds, just stop trying
                                hitChildren = true;

                            npc.ai[0] = npc.ai[1];
                            npc.ai[1] = 0;
                            npc.ai[2] = 0;
                            npc.ai[3] = 0;
                            npc.localAI[0] = 0;
                            npc.localAI[1] = 0;
                            npc.netUpdate = true;
                        }
                    }
                    break;

                case -3: //final phase
                    if (!player.active || player.dead || Vector2.Distance(npc.Center, player.Center) > 2500f) //despawn code
                    {
                        npc.TargetClosest(false);
                        if (npc.timeLeft > 30)
                            npc.timeLeft = 30;

                        npc.velocity.Y -= 1f;
                        break;
                    }

                    npc.rotation = 0;
                    npc.velocity *= 0.9f;

                    if (npc.ai[1] == 0)
                    {
                        npc.ai[1] = 1;

                        if (!Main.dedServ && Main.LocalPlayer.active)
                            Main.LocalPlayer.GetModPlayer<FargoPlayer>().Screenshake = 30;

                        Main.PlaySound(SoundID.Roar, npc.Center, 0);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            if (FargoSoulsWorld.MasochistMode)
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<CosmosRitual>(), npc.damage * 2 / 7, 0f, Main.myPlayer, 0f, npc.whoAmI);

                            const int max = 3;
                            float startRotation = npc.DirectionFrom(player.Center).ToRotation(); //ensure never spawn one directly at you
                            for (int i = 0; i < max; i++)
                            {
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<CosmosMoon>(), npc.damage / 3, 0f, Main.myPlayer, MathHelper.TwoPi / max * i + startRotation, npc.whoAmI);
                            }

                            Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, npc.whoAmI, -2);
                        }

                        Vector2 size = new Vector2(500, 500);
                        Vector2 spawnPos = npc.Center;
                        spawnPos.X -= size.X / 2;
                        spawnPos.Y -= size.Y / 2;

                        for (int num615 = 0; num615 < 30; num615++)
                        {
                            int num616 = Dust.NewDust(spawnPos, (int)size.X, (int)size.Y, 31, 0f, 0f, 100, default(Color), 1.5f);
                            Main.dust[num616].velocity *= 1.4f;
                        }

                        for (int num617 = 0; num617 < 50; num617++)
                        {
                            int num618 = Dust.NewDust(spawnPos, (int)size.X, (int)size.Y, DustID.Fire, 0f, 0f, 100, default(Color), 3.5f);
                            Main.dust[num618].noGravity = true;
                            Main.dust[num618].velocity *= 7f;
                            num618 = Dust.NewDust(spawnPos, (int)size.X, (int)size.Y, DustID.Fire, 0f, 0f, 100, default(Color), 1.5f);
                            Main.dust[num618].velocity *= 3f;
                        }

                        for (int num619 = 0; num619 < 2; num619++)
                        {
                            float scaleFactor9 = 0.4f;
                            if (num619 == 1) scaleFactor9 = 0.8f;
                            int num620 = Gore.NewGore(npc.Center, default(Vector2), Main.rand.Next(61, 64));
                            Main.gore[num620].velocity *= scaleFactor9;
                            Gore gore97 = Main.gore[num620];
                            gore97.velocity.X = gore97.velocity.X + 1f;
                            Gore gore98 = Main.gore[num620];
                            gore98.velocity.Y = gore98.velocity.Y + 1f;
                            num620 = Gore.NewGore(npc.Center, default(Vector2), Main.rand.Next(61, 64));
                            Main.gore[num620].velocity *= scaleFactor9;
                            Gore gore99 = Main.gore[num620];
                            gore99.velocity.X = gore99.velocity.X - 1f;
                            Gore gore100 = Main.gore[num620];
                            gore100.velocity.Y = gore100.velocity.Y + 1f;
                            num620 = Gore.NewGore(npc.Center, default(Vector2), Main.rand.Next(61, 64));
                            Main.gore[num620].velocity *= scaleFactor9;
                            Gore gore101 = Main.gore[num620];
                            gore101.velocity.X = gore101.velocity.X + 1f;
                            Gore gore102 = Main.gore[num620];
                            gore102.velocity.Y = gore102.velocity.Y - 1f;
                            num620 = Gore.NewGore(npc.Center, default(Vector2), Main.rand.Next(61, 64));
                            Main.gore[num620].velocity *= scaleFactor9;
                            Gore gore103 = Main.gore[num620];
                            gore103.velocity.X = gore103.velocity.X - 1f;
                            Gore gore104 = Main.gore[num620];
                            gore104.velocity.Y = gore104.velocity.Y - 1f;
                        }


                        for (int k = 0; k < 20; k++) //make visual dust
                        {
                            Vector2 dustPos = spawnPos;
                            dustPos.X += Main.rand.Next((int)size.X);
                            dustPos.Y += Main.rand.Next((int)size.Y);

                            for (int i = 0; i < 20; i++)
                            {
                                int dust = Dust.NewDust(dustPos, 32, 32, 31, 0f, 0f, 100, default(Color), 3f);
                                Main.dust[dust].velocity *= 1.4f;
                            }

                            for (int i = 0; i < 10; i++)
                            {
                                int dust = Dust.NewDust(dustPos, 32, 32, DustID.Fire, 0f, 0f, 100, default(Color), 3.5f);
                                Main.dust[dust].noGravity = true;
                                Main.dust[dust].velocity *= 7f;
                                dust = Dust.NewDust(dustPos, 32, 32, DustID.Fire, 0f, 0f, 100, default(Color), 1.5f);
                                Main.dust[dust].velocity *= 3f;
                            }

                            float scaleFactor9 = 0.5f;
                            for (int j = 0; j < 2; j++)
                            {
                                int gore = Gore.NewGore(dustPos, default(Vector2), Main.rand.Next(61, 64));
                                Main.gore[gore].velocity *= scaleFactor9;
                                Main.gore[gore].velocity.X += 1f;
                                Main.gore[gore].velocity.Y += 1f;
                            }
                        }
                    }

                    if (++npc.ai[2] > 200 || npc.ai[2] == 100)
                    {
                        if (npc.ai[2] > 200)
                            npc.ai[2] = 0;

                        npc.netUpdate = true;

                        Main.PlaySound(SoundID.Item92, npc.Center);

                        if (!Main.dedServ && Main.LocalPlayer.active)
                            Main.LocalPlayer.GetModPlayer<FargoPlayer>().Screenshake = 30;

                        int type; //for dust

                        if (npc.ai[3] == 0) //solar
                        {
                            npc.ai[3]++;
                            type = 127;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                float modifier = (float)npc.life / (npc.lifeMax * 0.2f);
                                if (modifier < 0)
                                    modifier = 0;
                                if (modifier > 1)
                                    modifier = 1;
                                modifier = 11f + 4f * modifier;

                                const int max = 12;
                                for (int i = 0; i < max; i++)
                                {
                                    Projectile.NewProjectile(npc.Center, modifier * npc.DirectionTo(player.Center).RotatedBy(2 * Math.PI / max * i),
                                        ModContent.ProjectileType<CosmosFireball2>(), npc.damage * 2 / 7, 0f, Main.myPlayer, 30, 30 + 60);
                                }
                            }
                        }
                        else if (npc.ai[3] == 1) //vortex
                        {
                            npc.ai[3]++;
                            type = 229;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                if (!Main.dedServ)
                                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Thunder").WithVolume(0.8f).WithPitchVariance(-0.5f), npc.Center);
                                const int max = 16;
                                for (int i = 0; i < max; i++)
                                {
                                    Vector2 dir = npc.DirectionTo(player.Center).RotatedBy(2 * (float)Math.PI / max * i);
                                    float ai1New = (Main.rand.Next(2) == 0) ? 1 : -1; //randomize starting direction
                                    Vector2 vel = Vector2.Normalize(dir) * 6f;
                                    Projectile.NewProjectile(npc.Center, vel * 3f, mod.ProjectileType("CosmosLightning"),
                                        npc.damage * 2 / 7, 0, Main.myPlayer, dir.ToRotation(), ai1New);
                                }
                            }
                        }
                        else if (npc.ai[3] == 2) //nebula
                        {
                            npc.ai[3]++;
                            type = 242;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                /*const int max = 11;
                                for (int i = 0; i < max; i++)
                                {
                                    Projectile.NewProjectile(npc.Center, 3f * npc.DirectionTo(player.Center).RotatedBy(2 * Math.PI / max * (i + 0.5)),
                                        ModContent.ProjectileType<CosmosNebulaBlaze>(), npc.damage * 2 / 7, 0f, Main.myPlayer, 0.007f);
                                }*/

                                for (int j = -1; j <= 1; j++) //to both sides
                                {
                                    if (j == 0)
                                        continue;

                                    const int gap = 20;
                                    const int max = 15;
                                    const int individualOffset = 6;
                                    Vector2 baseVel = npc.DirectionTo(Main.player[npc.target].Center).RotatedBy(MathHelper.ToRadians(gap) * j);
                                    for (int k = 0; k < max; k++) //a fan of blazes
                                    {
                                        Projectile.NewProjectile(npc.Center, 9f * baseVel.RotatedBy(MathHelper.ToRadians(individualOffset) * j * k),
                                            ModContent.ProjectileType<CosmosNebulaBlaze>(), npc.damage * 2 / 7, 0f, Main.myPlayer, 0.009f);
                                    }
                                }
                            }
                        }
                        else //stardust
                        {
                            npc.ai[3] = 0;
                            type = 135;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                const int max = 18;
                                for (int i = 0; i < max; i++)
                                {
                                    Projectile.NewProjectile(npc.Center, Vector2.UnitX.RotatedBy(2 * Math.PI / max * i),
                                        ModContent.ProjectileType<CosmosInvader>(), npc.damage * 2 / 7, 0f, Main.myPlayer, 180, 0.04f);
                                    Projectile.NewProjectile(npc.Center, Vector2.UnitX.RotatedBy(2 * Math.PI / max * (i + 0.5)),
                                        ModContent.ProjectileType<CosmosInvader>(), npc.damage * 2 / 7, 0f, Main.myPlayer, 180, 0.025f);
                                }
                                for(int j = 0; j < 5; j++)
                                {
                                    Vector2 vel = -Vector2.UnitY.RotatedBy(MathHelper.Pi * 0.4f * j);
                                    Projectile.NewProjectile(npc.Center, vel, mod.ProjectileType("CosmosGlowything"), 0, 0f, Main.myPlayer);
                                }
                                Main.PlaySound(SoundID.NPCKilled, (int)npc.position.X, (int)npc.position.Y, 7, 1f, 0.0f);
                            }
                        }

                        /*const int num226 = 150;
                        for (int num227 = 0; num227 < num226; num227++)
                        {
                            Vector2 vector6 = Vector2.UnitX * 50f;
                            vector6 = vector6.RotatedBy(((num227 - (num226 / 2 - 1)) * 6.28318548f / num226)) + npc.Center;
                            Vector2 vector7 = vector6 - npc.Center;
                            int num228 = Dust.NewDust(vector6 + vector7, 0, 0, type, 0f, 0f, 0, default);
                            Main.dust[num228].scale = 3f;
                            Main.dust[num228].noGravity = true;
                            Main.dust[num228].velocity = vector7;
                        }*/

                        /*for (int index = 0; index < 50; ++index) //dust
                        {
                            Dust dust = Main.dust[Dust.NewDust(npc.position, npc.width, npc.height, type, 0.0f, 0.0f, 0, new Color(), 1f)];
                            dust.velocity *= 10f;
                            dust.fadeIn = 1f;
                            dust.scale = 1 + Main.rand.NextFloat() + Main.rand.Next(4) * 0.3f;
                            if (Main.rand.Next(3) != 0)
                            {
                                dust.noGravity = true;
                                dust.velocity *= 3f;
                                dust.scale *= 2f;
                            }
                        }

                        Vector2 size = new Vector2(500, 500);
                        Vector2 spawnPos = npc.Center;
                        spawnPos.X -= size.X / 2;
                        spawnPos.Y -= size.Y / 2;

                        for (int num615 = 0; num615 < 30; num615++)
                        {
                            int num616 = Dust.NewDust(spawnPos, (int)size.X, (int)size.Y, 31, 0f, 0f, 100, default(Color), 1.5f);
                            Main.dust[num616].velocity *= 1.4f;
                        }

                        for (int num617 = 0; num617 < 50; num617++)
                        {
                            int num618 = Dust.NewDust(spawnPos, (int)size.X, (int)size.Y, DustID.Fire, 0f, 0f, 100, default(Color), 3.5f);
                            Main.dust[num618].noGravity = true;
                            Main.dust[num618].velocity *= 7f;
                            num618 = Dust.NewDust(spawnPos, (int)size.X, (int)size.Y, DustID.Fire, 0f, 0f, 100, default(Color), 1.5f);
                            Main.dust[num618].velocity *= 3f;
                        }

                        for (int num619 = 0; num619 < 2; num619++)
                        {
                            float scaleFactor9 = 0.4f;
                            if (num619 == 1) scaleFactor9 = 0.8f;
                            int num620 = Gore.NewGore(npc.Center, default(Vector2), Main.rand.Next(61, 64));
                            Main.gore[num620].velocity *= scaleFactor9;
                            Gore gore97 = Main.gore[num620];
                            gore97.velocity.X = gore97.velocity.X + 1f;
                            Gore gore98 = Main.gore[num620];
                            gore98.velocity.Y = gore98.velocity.Y + 1f;
                            num620 = Gore.NewGore(npc.Center, default(Vector2), Main.rand.Next(61, 64));
                            Main.gore[num620].velocity *= scaleFactor9;
                            Gore gore99 = Main.gore[num620];
                            gore99.velocity.X = gore99.velocity.X - 1f;
                            Gore gore100 = Main.gore[num620];
                            gore100.velocity.Y = gore100.velocity.Y + 1f;
                            num620 = Gore.NewGore(npc.Center, default(Vector2), Main.rand.Next(61, 64));
                            Main.gore[num620].velocity *= scaleFactor9;
                            Gore gore101 = Main.gore[num620];
                            gore101.velocity.X = gore101.velocity.X + 1f;
                            Gore gore102 = Main.gore[num620];
                            gore102.velocity.Y = gore102.velocity.Y - 1f;
                            num620 = Gore.NewGore(npc.Center, default(Vector2), Main.rand.Next(61, 64));
                            Main.gore[num620].velocity *= scaleFactor9;
                            Gore gore103 = Main.gore[num620];
                            gore103.velocity.X = gore103.velocity.X - 1f;
                            Gore gore104 = Main.gore[num620];
                            gore104.velocity.Y = gore104.velocity.Y - 1f;
                        }


                        for (int k = 0; k < 20; k++) //make visual dust
                        {
                            Vector2 dustPos = spawnPos;
                            dustPos.X += Main.rand.Next((int)size.X);
                            dustPos.Y += Main.rand.Next((int)size.Y);

                            for (int i = 0; i < 20; i++)
                            {
                                int dust = Dust.NewDust(dustPos, 32, 32, 31, 0f, 0f, 100, default(Color), 3f);
                                Main.dust[dust].velocity *= 1.4f;
                            }

                            for (int i = 0; i < 10; i++)
                            {
                                int dust = Dust.NewDust(dustPos, 32, 32, DustID.Fire, 0f, 0f, 100, default(Color), 3.5f);
                                Main.dust[dust].noGravity = true;
                                Main.dust[dust].velocity *= 7f;
                                dust = Dust.NewDust(dustPos, 32, 32, DustID.Fire, 0f, 0f, 100, default(Color), 1.5f);
                                Main.dust[dust].velocity *= 3f;
                            }

                            float scaleFactor9 = 0.5f;
                            for (int j = 0; j < 2; j++)
                            {
                                int gore = Gore.NewGore(dustPos, default(Vector2), Main.rand.Next(61, 64));
                                Main.gore[gore].velocity *= scaleFactor9;
                                Main.gore[gore].velocity.X += 1f;
                                Main.gore[gore].velocity.Y += 1f;
                            }
                        }*/
                    }
                    break;

                case -2: //prepare for last phase
                    npc.rotation = 0;
                    npc.dontTakeDamage = true;

                    npc.localAI[2] = 2;

                    targetPos = player.Center;
                    targetPos.X += 600 * (npc.Center.X < targetPos.X ? -1 : 1);
                    Movement(targetPos, 0.8f, 32f);

                    if (--npc.ai[2] < 0)
                    {
                        npc.ai[2] = Main.rand.Next(5);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 spawnPos = npc.position + new Vector2(Main.rand.Next(npc.width), Main.rand.Next(npc.height));
                            int type = ModContent.ProjectileType<Projectiles.BossWeapons.PhantasmalBlast>();
                            Projectile.NewProjectile(spawnPos, Vector2.Zero, type, 0, 0f, Main.myPlayer);
                        }
                    }

                    if (++npc.ai[1] > 150)
                    {
                        /*const int num226 = 80;
                        for (int num227 = 0; num227 < num226; num227++)
                        {
                            Vector2 vector6 = Vector2.UnitX * 40f;
                            vector6 = vector6.RotatedBy(((num227 - (num226 / 2 - 1)) * 6.28318548f / num226), default(Vector2)) + npc.Center;
                            Vector2 vector7 = vector6 - npc.Center;
                            int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 229, 0f, 0f, 0, default(Color), 3f);
                            Main.dust[num228].noGravity = true;
                            Main.dust[num228].velocity = vector7;
                        }*/

                        npc.TargetClosest();
                        npc.ai[0]--;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;
                        npc.netUpdate = true;
                    }
                    break;

                case -1: //phase 2 transition
                    npc.rotation = 0;
                    npc.dontTakeDamage = true;

                    npc.velocity *= 0.9f;

                    if (++npc.ai[1] == 120)
                    {
                        Main.PlaySound(SoundID.Roar, npc.Center, 0);
                        npc.localAI[2] = 1;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, npc.whoAmI, -2);
                    }
                    else if (npc.ai[1] > 180)
                    {
                        npc.TargetClosest();
                        npc.ai[0] = npc.ai[3];
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;
                        npc.netUpdate = true;
                    }
                    break;

                case 0: //float near player, skip next attack and wait if not p2
                    npc.rotation = 0;

                    if ((!player.active || player.dead || Vector2.Distance(npc.Center, player.Center) > 2500f)
                        && npc.localAI[3] != 0) //despawn code
                    {
                        npc.TargetClosest(false);
                        if (npc.timeLeft > 30)
                            npc.timeLeft = 30;

                        npc.velocity.Y -= 1f;
                        break;
                    }
                    
                    targetPos = player.Center + npc.DirectionFrom(player.Center) * 500;
                    if (npc.Distance(targetPos) > 50)
                        Movement(targetPos, 0.8f, 32f);

                    if (++npc.ai[1] > 60)
                    {
                        float oldAi0 = npc.ai[0];

                        npc.TargetClosest();
                        npc.ai[0] += npc.localAI[2] == 0 || !Main.expertMode ? 2 : 1;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;
                        npc.netUpdate = true;

                        if (!hitChildren)
                        {
                            for (int i = 0; i < Main.maxNPCs; i++) //look for deviantt to kill
                            {
                                int type = ModLoader.GetMod("Fargowiltas").NPCType("Deviantt");
                                if (Main.npc[i].active && Main.npc[i].type == type && npc.Distance(Main.npc[i].Center) < 2000 && player.Distance(Main.npc[i].Center) < 2000)
                                {
                                    npc.ai[0] = -4;
                                    npc.ai[1] = oldAi0;
                                    npc.ai[2] = i; //store target npc
                                    break;
                                }
                            }

                        }
                    }
                    break;

                case 1: //deathray punches, p2 only
                    targetPos = player.Center;
                    targetPos.X += 300 * (npc.Center.X < targetPos.X ? -1 : 1);
                    if (npc.Distance(targetPos) > 50)
                        Movement(targetPos, 0.8f, 32f);

                    if (npc.ai[1] == 1)
                        Main.PlaySound(SoundID.Roar, npc.Center, 0);

                    if (++npc.ai[2] <= 6)
                    {
                        npc.rotation = npc.DirectionTo(player.Center).ToRotation();
                        if (npc.direction < 0)
                            npc.rotation += (float)Math.PI;

                        npc.ai[3] = npc.Center.X < player.Center.X ? 1 : -1; //store direction im facing

                        if (npc.ai[2] == 6)
                        {
                            npc.netUpdate = true;
                            if (npc.ai[1] > 50)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Vector2 offset = Vector2.UnitX;
                                    if (npc.direction < 0)
                                        offset.X *= -1f;
                                    offset = offset.RotatedBy(npc.DirectionTo(player.Center).ToRotation());

                                    int modifier = Math.Sign(npc.Center.Y - player.Center.Y);
                                    Projectile.NewProjectile(npc.Center + offset + 3000 * npc.DirectionFrom(player.Center) * modifier, 
                                        npc.DirectionTo(player.Center) * modifier, 
                                        ModContent.ProjectileType<CosmosDeathray>(), npc.damage / 4, 0f, Main.myPlayer);
                                }
                            }
                            else
                            {
                                npc.ai[2] = 0;
                            }
                        }
                    }
                    else
                    {
                        npc.direction = npc.spriteDirection = Math.Sign(npc.ai[3]); //dont turn around if crossed up

                        if (npc.ai[2] > 12)
                        {
                            npc.ai[2] = 0;
                            npc.ai[3] = 0;
                            npc.netUpdate = true;
                        }
                    }

                    if (++npc.ai[1] > 240)
                    {
                        npc.TargetClosest();
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;
                        npc.netUpdate = true;
                    }
                    break;

                case 2: //float near player, proceed to next attack always
                    npc.rotation = 0;

                    if ((!player.active || player.dead || Vector2.Distance(npc.Center, player.Center) > 2500f)
                        && npc.localAI[3] != 0) //despawn code
                    {
                        npc.TargetClosest(false);
                        if (npc.timeLeft > 30)
                            npc.timeLeft = 30;

                        npc.velocity.Y -= 1f;
                        break;
                    }

                    targetPos = player.Center + npc.DirectionFrom(player.Center) * 500;
                    if (npc.Distance(targetPos) > 50)
                        Movement(targetPos, 0.8f, 32f);

                    if (++npc.ai[1] > 60)
                    {
                        float oldAi0 = npc.ai[0];

                        npc.TargetClosest();
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;
                        npc.netUpdate = true;

                        if (!hitChildren)
                        {
                            for (int i = 0; i < Main.maxNPCs; i++) //look for deviantt to kill
                            {
                                int type = ModLoader.GetMod("Fargowiltas").NPCType("Deviantt");
                                if (Main.npc[i].active && Main.npc[i].type == type && npc.Distance(Main.npc[i].Center) < 2000 && player.Distance(Main.npc[i].Center) < 2000)
                                {
                                    npc.ai[0] = -4;
                                    npc.ai[1] = oldAi0;
                                    npc.ai[2] = i; //store target npc
                                    break;
                                }
                            }
                        }
                    }
                    break;

                case 3: //fireball dashes
                    {
                        if (npc.ai[1] == 1)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient) //spawn balls
                            {
                                const int max = 8;
                                const float distance = 120f;
                                float rotation = 2f * (float)Math.PI / max;
                                int damage = npc.damage / 4;
                                for (int i = 0; i < max; i++)
                                {
                                    Vector2 spawnPos = npc.Center + new Vector2(distance, 0f).RotatedBy(rotation * i);
                                    int p = Projectile.NewProjectile(spawnPos, Vector2.Zero, ModContent.ProjectileType<CosmosFireball>(), damage, 0f, Main.myPlayer, npc.whoAmI, rotation * i);
                                    //if (p != Main.maxProjectiles && !(FargoSoulsWorld.MasochistMode && npc.localAI[2] != 0f)) Main.projectile[p].timeLeft = 300;
                                }
                            }
                        }

                        int threshold = 70; //npc.localAI[2] == 0 ? 70 : 50;
                        if (++npc.ai[2] <= threshold)
                        {
                            targetPos = player.Center;
                            targetPos.X += 600 * (npc.Center.X < targetPos.X ? -1 : 1);
                            targetPos.Y += 300 * (npc.Center.Y < targetPos.Y ? -1 : 1);
                            Movement(targetPos, 1.6f, 24f);

                            npc.rotation = npc.DirectionTo(player.Center).ToRotation();
                            if (npc.direction < 0)
                                npc.rotation += (float)Math.PI;

                            if (npc.ai[2] == threshold)
                            {
                                npc.velocity = 42f * npc.DirectionTo(player.Center);
                                npc.netUpdate = true;
                            }
                        }
                        else
                        {
                            npc.direction = npc.spriteDirection = Math.Sign(npc.velocity.X);

                            if (npc.ai[2] > threshold + 30)
                            {
                                npc.ai[2] = 0;
                                npc.netUpdate = true;

                                if (FargoSoulsWorld.MasochistMode && npc.localAI[2] != 0f) //emode p2, do chain blasts
                                {
                                    if (!Main.dedServ && Main.LocalPlayer.active)
                                        Main.LocalPlayer.GetModPlayer<FargoPlayer>().Screenshake = 30;

                                    if (Main.netMode != NetmodeID.MultiplayerClient) //chain explosions
                                    {
                                        Vector2 baseDirection = npc.DirectionTo(player.Center);
                                        const int max = 6; //spread
                                        for (int i = 0; i < max; i++)
                                        {
                                            Vector2 offset = npc.height / 2 * baseDirection.RotatedBy(Math.PI * 2 / max * i);
                                            float ai1 = i <= 1 || i == max - 1 ? 32 : 8;
                                            Projectile.NewProjectile(npc.Center + Main.rand.NextVector2Circular(npc.width / 2, npc.height / 2), Vector2.Zero, ModContent.ProjectileType<Projectiles.Masomode.MoonLordSunBlast>(),
                                                npc.damage / 4, 0f, Main.myPlayer, MathHelper.WrapAngle(offset.ToRotation()), ai1);
                                        }
                                    }
                                }
                            }
                        }

                        if (++npc.ai[1] > 330) //(FargoSoulsWorld.MasochistMode && npc.localAI[2] != 0f ? 360 : 330))
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

                case 4:
                    goto case 0;

                case 5: //meteor punch
                    if (npc.ai[1] == 1)
                        Main.PlaySound(SoundID.Roar, npc.Center, 0);

                    if (++npc.ai[2] <= 75)
                    {
                        targetPos = player.Center;
                        targetPos.X += 350 * (npc.Center.X < targetPos.X ? -1 : 1);
                        targetPos.Y -= 700;
                        Movement(targetPos, 1.6f, 32f);

                        npc.rotation = npc.DirectionTo(player.Center).ToRotation();
                        if (npc.direction < 0)
                            npc.rotation += (float)Math.PI;

                        npc.localAI[0] = npc.Center.X < player.Center.X ? 1 : -1; //store direction im facing

                        if (npc.ai[2] == 75) //falling punch
                        {
                            npc.velocity = 42f * npc.DirectionTo(player.Center);
                            npc.netUpdate = true;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int modifier = Math.Sign(npc.Center.Y - player.Center.Y);
                                Projectile.NewProjectile(npc.Center + 3000 * npc.DirectionFrom(player.Center) * modifier, npc.DirectionTo(player.Center) * modifier,
                                    ModContent.ProjectileType<CosmosDeathray2>(), npc.damage / 4, 0f, Main.myPlayer);

                                const int max = 3;
                                for (int i = -max; i <= max; i++)
                                {
                                    Projectile.NewProjectile(npc.Center, 0.5f * -Vector2.UnitY.RotatedBy(MathHelper.PiOver2 / max * i), 
                                        ModContent.ProjectileType<CosmosBolt>(), npc.damage / 4, 0f, Main.myPlayer);
                                }
                            }

                            npc.ai[3] = Main.rand.Next(4); //random offset each time
                        }
                    }
                    else
                    {
                        npc.direction = npc.spriteDirection = Math.Sign(npc.localAI[0]); //dont turn around if crossed up

                        if (++npc.ai[3] > 4)
                        {
                            npc.ai[3] = 0;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(npc.Center, 0.5f * Vector2.UnitX, ModContent.ProjectileType<CosmosBolt>(), npc.damage / 4, 0f, Main.myPlayer);
                                Projectile.NewProjectile(npc.Center, -0.5f * Vector2.UnitX, ModContent.ProjectileType<CosmosBolt>(), npc.damage / 4, 0f, Main.myPlayer);
                            }
                        }
                    }

                    if (++npc.ai[1] > 240 || (npc.ai[2] > 60 && npc.Center.Y > player.Center.Y + 700))
                    {
                        npc.velocity.Y = 0f;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            const int max = 3;
                            for (int i = -max; i <= max; i++)
                            {
                                Projectile.NewProjectile(npc.Center, 0.5f * Vector2.UnitY.RotatedBy(MathHelper.PiOver2 / max * i),
                                    ModContent.ProjectileType<CosmosBolt>(), npc.damage / 4, 0f, Main.myPlayer);
                            }
                        }

                        npc.TargetClosest();
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;
                        npc.localAI[0] = 0;
                        npc.netUpdate = true;
                    }
                    break;

                case 6:
                    goto case 2;

                case 7: //vortex
                    targetPos = player.Center + npc.DirectionFrom(player.Center) * 500;
                    if (npc.Distance(player.Center) < 200 || npc.Distance(player.Center) > 600)
                    {
                        if (npc.Distance(targetPos) > 50)
                            Movement(targetPos, 0.6f, 32f);
                    }
                    else //skid to a halt a bit
                    {
                        npc.velocity *= 0.97f;
                    }

                    if (npc.ai[1] == 30)
                    {
                        Main.PlaySound(SoundID.ForceRoar, npc.Center, -1);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            float ai1 = FargoSoulsWorld.MasochistMode && npc.localAI[2] != 0 ? -1.2f : (npc.localAI[2] == 0 ? 1f : -1.6f);
                            Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<CosmosVortex>(), npc.damage / 4, 0f, Main.myPlayer, 0f, ai1);
                        }

                        int length = (int)npc.Distance(player.Center);
                        Vector2 offset = npc.DirectionTo(player.Center);
                        for (int i = 0; i < length; i += 10) //dust warning line for sandnado
                        {
                            int d = Dust.NewDust(npc.Center + offset * i, 0, 0, 229, 0f, 0f, 0, new Color());
                            Main.dust[d].noGravity = true;
                            Main.dust[d].scale = 1.5f;
                        }
                    }

                    if (++npc.ai[1] > 450)
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

                case 9: //shen ray and balls torture
                    if (npc.ai[1] == 1)
                        Main.PlaySound(SoundID.Roar, npc.Center, 0);

                    if (++npc.ai[2] <= 200)
                    {
                        targetPos = player.Center;
                        targetPos.X += 600 * (npc.Center.X < targetPos.X ? -1 : 1);
                        npc.position += player.velocity / 3f; //really good tracking movement here
                        Movement(targetPos, 1.2f, 32f);

                        if (++npc.localAI[0] > 60)
                        {
                            npc.localAI[0] = 0;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                const int max = 9;
                                const int travelTime = 25;
                                for (int j = -1; j <= 1; j += 2)
                                {
                                    for (int i = -max; i <= max; i++)
                                    {
                                        Vector2 target = player.Center;
                                        target.X += 180 * i;
                                        target.Y += (Math.Max(Math.Abs(player.velocity.Y) * (travelTime * 2 + 5), 600) + 300 / max * Math.Abs(i)) * j;
                                        //y pos is above and below player, adapt to always outspeed player, with additional V shapes
                                        Vector2 speed = (target - npc.Center) / travelTime;
                                        int individualTiming = Math.Abs(i * 3);
                                        Projectile.NewProjectile(npc.Center, speed / 2, ModContent.ProjectileType<CosmosSphere>(), npc.damage / 4, 0f, Main.myPlayer, travelTime, travelTime * 2 + individualTiming);
                                    }
                                }
                            }
                        }

                        npc.rotation = npc.DirectionTo(player.Center).ToRotation();
                        if (npc.direction < 0)
                            npc.rotation += (float)Math.PI;

                        npc.ai[3] = npc.Center.X < player.Center.X ? 1 : -1; //store direction im facing

                        if (npc.ai[2] == 200) //straight ray punch
                        {
                            npc.velocity = 42f * npc.DirectionTo(player.Center);
                            npc.netUpdate = true;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int modifier = Math.Sign(npc.Center.Y - player.Center.Y);
                                Projectile.NewProjectile(npc.Center + 3000 * npc.DirectionFrom(player.Center) * modifier, npc.DirectionTo(player.Center) * modifier,
                                    ModContent.ProjectileType<CosmosDeathray2>(), npc.damage / 4, 0f, Main.myPlayer);
                            }
                        }
                    }
                    else
                    {
                        npc.direction = npc.spriteDirection = Math.Sign(npc.ai[3]); //dont turn around if crossed up
                    }

                    if (++npc.ai[1] > 400 || (npc.ai[2] > 200 &&
                        (npc.ai[3] > 0 ? npc.Center.X > player.Center.X + 800 : npc.Center.X < player.Center.X - 800)))
                    {
                        npc.velocity.X = 0f;

                        npc.TargetClosest();
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;
                        npc.localAI[0] = 0;
                        npc.netUpdate = true;
                    }
                    break;
                    
                case 10:
                    goto case 2;

                case 11: //reticle and nebula blazes
                    targetPos = player.Center;

                    if (FargoSoulsWorld.MasochistMode && npc.localAI[2] != 0)
                    {
                        int sign = npc.Center.X < targetPos.X ? -1 : 1;
                        Vector2 offset = 600 * sign * Vector2.UnitX;
                        float rotation = MathHelper.ToRadians(80) * npc.ai[1] / 420 * -sign;
                        targetPos += offset.RotatedBy(rotation); //emode p2, slowly rotate to above the player
                        
                        npc.position += player.velocity / 3f; //really good tracking movement here to reduce odds of crossups
                        Movement(targetPos, 0.8f, 24f);
                    }
                    else //just float beside player
                    {
                        targetPos.X += 550 * (npc.Center.X < targetPos.X ? -1 : 1);
                        if (npc.Distance(targetPos) > 50)
                            Movement(targetPos, 0.8f, 24f);
                    }

                    npc.rotation = npc.DirectionTo(player.Center).ToRotation();
                    if (npc.direction < 0)
                        npc.rotation += (float)Math.PI;

                    if (npc.ai[1] == 30 && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<CosmosReticle>(), npc.damage / 4, 0f, Main.myPlayer, npc.whoAmI);
                    }

                    if (npc.ai[1] > 60)
                    {
                        if (++npc.ai[3] == 3)
                        {
                            Main.PlaySound(SoundID.Item20, npc.Center);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Vector2 offset = new Vector2(80, 6);
                                if (player.Center.X < npc.Center.X)
                                    offset.X *= -1f;
                                offset = offset.RotatedBy(npc.rotation);
                                for (int i = 0; i < 2; i++)
                                {
                                    float rotation = MathHelper.ToRadians(npc.localAI[2] == 0 ? 30 : 15) + Main.rand.NextFloat(MathHelper.ToRadians(10));
                                    if (i == 0)
                                        rotation *= -1f;
                                    Vector2 vel = Main.rand.NextFloat(8f, 12f) * npc.DirectionTo(player.Center).RotatedBy(rotation);
                                    Projectile.NewProjectile(npc.Center + offset, vel, ModContent.ProjectileType<CosmosNebulaBlaze>(),
                                        npc.damage / 4, 0f, Main.myPlayer, 0.006f);
                                }
                            }
                        }
                        else if (npc.ai[3] >= 6)
                        {
                            npc.ai[3] = 0;
                        }
                    }

                    if (++npc.ai[1] > 390)
                    {
                        npc.TargetClosest();
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;
                        npc.netUpdate = true;
                    }
                    break;

                case 12:
                    goto case 0;

                case 13: //multi-punch to uppercut
                    if (++npc.ai[1] < 110)
                    {
                        targetPos = player.Center;
                        targetPos.X += 300 * (npc.Center.X < targetPos.X ? -1 : 1);
                        if (npc.Distance(targetPos) > 50)
                            Movement(targetPos, 0.8f, 32f);

                        if (npc.ai[1] == 1)
                            Main.PlaySound(SoundID.Roar, npc.Center, 0);

                        if (++npc.ai[2] <= 6)
                        {
                            npc.rotation = npc.DirectionTo(player.Center).ToRotation();
                            if (npc.direction < 0)
                                npc.rotation += (float)Math.PI;

                            npc.ai[3] = npc.Center.X < player.Center.X ? 1 : -1; //store direction im facing

                            if (npc.ai[2] == 6)
                            {
                                npc.netUpdate = true;
                                if (npc.ai[1] > 50)
                                {
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        Vector2 offset = Vector2.UnitX;
                                        if (npc.direction < 0)
                                            offset.X *= -1f;
                                        offset = offset.RotatedBy(npc.DirectionTo(player.Center).ToRotation());

                                        int modifier = Math.Sign(npc.Center.Y - player.Center.Y);
                                        Projectile.NewProjectile(npc.Center + offset + 3000 * npc.DirectionFrom(player.Center) * modifier, npc.DirectionTo(player.Center) * modifier,
                                            ModContent.ProjectileType<CosmosDeathray>(), npc.damage / 4, 0f, Main.myPlayer);
                                    }
                                }
                                else
                                {
                                    npc.ai[2] = 0;
                                }
                            }
                        }
                        else
                        {
                            npc.direction = npc.spriteDirection = Math.Sign(npc.ai[3]); //dont turn around if crossed up

                            if (npc.ai[2] > 12)
                            {
                                npc.ai[2] = 0;
                                npc.ai[3] = 0;
                                npc.netUpdate = true;
                            }
                        }
                    }
                    else //uppercut time
                    {
                        if (npc.ai[1] <= 110 + 45)
                        {
                            targetPos = player.Center;
                            targetPos.X += 350 * (npc.Center.X < targetPos.X ? -1 : 1);
                            targetPos.Y += 700;
                            npc.position += player.velocity / 3f; //really good tracking movement here
                            Movement(targetPos, 2.4f, 32f);

                            npc.rotation = npc.DirectionTo(player.Center).ToRotation();
                            if (npc.direction < 0)
                                npc.rotation += (float)Math.PI;

                            //npc.ai[3] = npc.Center.X < player.Center.X ? 1 : -1; //store direction im facing

                            if (npc.ai[1] == 110 + 45) //rising punch
                            {
                                const float speed = 42f;
                                npc.velocity = speed * npc.DirectionTo(player.Center);
                                npc.netUpdate = true;

                                npc.ai[3] = Math.Abs(player.Center.Y - npc.Center.Y) / speed; //time to travel to player's Y coord
                                npc.ai[3] *= 2f; //travel twice that to go above
                                
                                npc.localAI[0] = player.Center.X;
                                npc.localAI[1] = player.Center.Y;

                                npc.localAI[0] += npc.Center.X < player.Center.X ? -50 : 50; //horiz offset the centerpoint to one side closer to eri

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    int modifier = Math.Sign(npc.Center.Y - player.Center.Y);
                                    Projectile.NewProjectile(npc.Center + 3000 * npc.DirectionFrom(player.Center) * modifier, npc.DirectionTo(player.Center) * modifier,
                                        ModContent.ProjectileType<CosmosDeathray2>(), npc.damage / 4, 0f, Main.myPlayer);
                                }
                            }
                        }
                        else
                        {
                            npc.direction = npc.spriteDirection = Math.Sign(npc.velocity.X); //dont turn around if crossed up
                            npc.rotation = npc.velocity.ToRotation();
                            if (npc.direction < 0)
                                npc.rotation += (float)Math.PI;

                            if (Math.Abs(npc.Center.Y - npc.localAI[1]) < 300) //make the midpoint better at hitting people
                            {
                                //for finer grain, since eri moves too much in one tick (too much angular rotation change per tick when close)
                                Vector2 midPos = npc.Center - npc.velocity / 2;
                                Vector2 target = new Vector2(npc.localAI[0], npc.localAI[1]);
                                Vector2 vel = Vector2.Normalize(midPos - target);
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    int modifier = Math.Sign(player.Center.X - target.X) == npc.direction ? 1 : -1;
                                    Projectile.NewProjectile(npc.Center, modifier * 0.5f * vel, ModContent.ProjectileType<CosmosBolt>(), npc.damage / 4, 0f, Main.myPlayer);
                                    Projectile.NewProjectile(npc.Center, modifier * 0.5f * npc.DirectionFrom(target), ModContent.ProjectileType<CosmosBolt>(), npc.damage / 4, 0f, Main.myPlayer);
                                }
                            }
                            else if (++npc.ai[2] > 1)
                            {
                                npc.ai[2] = 0;

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Vector2 target = new Vector2(npc.localAI[0], npc.localAI[1]);
                                    int modifier = Math.Sign(player.Center.X - target.X) == npc.direction ? 1 : -1;
                                    Projectile.NewProjectile(npc.Center, 0.5f * npc.DirectionFrom(target), ModContent.ProjectileType<CosmosBolt>(), npc.damage / 4, 0f, Main.myPlayer);
                                }
                            }

                            if (npc.ai[1] > 110 + 45 + npc.ai[3])// || (npc.ai[1] > 110 + 45 && npc.Center.Y < player.Center.Y - 700))
                            {
                                npc.velocity.Y = 0f;

                                npc.TargetClosest();
                                npc.ai[0]++;
                                npc.ai[1] = FargoSoulsWorld.MasochistMode && npc.localAI[2] != 0 ? 0 : -120;
                                npc.ai[2] = 0;
                                npc.ai[3] = 0;
                                npc.localAI[0] = 0;
                                npc.localAI[1] = 0;
                                npc.netUpdate = true;
                            }
                        }
                    }
                    break;

                case 14:
                    goto case 2;

                case 15: //ZA WARUDO
                    targetPos = player.Center + npc.DirectionFrom(player.Center) * 500;
                    if (npc.ai[1] < 10 || npc.Distance(player.Center) < 200 || npc.Distance(player.Center) > 600)
                    {
                        if (npc.Distance(targetPos) > 50)
                            Movement(targetPos, 0.6f, 32f);
                    }
                    else //during the timestop, skid to a halt a bit
                    {
                        npc.velocity *= 0.97f;
                    }

                    if (npc.ai[1] > 10) //for timestop visual
                    {
                        if (Main.netMode != NetmodeID.Server && Filters.Scene["FargowiltasSouls:Invert"].IsActive())
                            Filters.Scene["FargowiltasSouls:Invert"].GetShader().UseTargetPosition(npc.Center);
                    }
                    
                    if (npc.ai[1] < 10)
                    {
                        for (int i = 0; i < 30; i++)
                        {
                            int d = Dust.NewDust(npc.position, npc.width, npc.height, 135, 0f, 0f, 0, default(Color), Main.rand.NextFloat(1f, 4f));
                            Main.dust[d].noGravity = true;
                            Main.dust[d].velocity *= Main.rand.NextFloat(3f, 9f);
                        }
                    }
                    else if (npc.ai[1] == 10)
                    {
                        npc.localAI[0] = Main.rand.NextFloat(2 * (float)Math.PI);

                        if (!Main.dedServ)
                            Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/ZaWarudo").WithVolume(1f).WithPitchVariance(.5f), player.Center);

                        //if (Main.netMode != NetmodeID.MultiplayerClient) Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, npc.whoAmI, -18);

                        /*const int num226 = 80;
                        for (int num227 = 0; num227 < num226; num227++)
                        {
                            Vector2 vector6 = Vector2.UnitX * 20f;
                            vector6 = vector6.RotatedBy(((num227 - (num226 / 2 - 1)) * 6.28318548f / num226), default(Vector2)) + npc.Center;
                            Vector2 vector7 = vector6 - npc.Center;
                            int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 135, 0f, 0f, 0, default(Color), 3f);
                            Main.dust[num228].noGravity = true;
                            Main.dust[num228].velocity = vector7;
                        }*/

                        //cardinal walls
                        /*if (FargoSoulsWorld.MasochistMode && npc.localAI[2] != 0 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            const int iMax = 4;
                            for (int i = 0; i < iMax; i++)
                            {
                                Vector2 offset = 300 * Vector2.UnitX.RotatedBy(2 * Math.PI / iMax * i);
                                const int jMax = 5;
                                for (int j = -jMax; j <= jMax; j++)
                                {
                                    Vector2 spawnPos = player.Center + offset.RotatedBy(MathHelper.ToRadians(15) / jMax * j);
                                    Vector2 vel = 2.5f * player.DirectionFrom(spawnPos);
                                    float ai0 = player.Distance(spawnPos) / 2.5f;
                                    Projectile.NewProjectile(spawnPos, vel, ModContent.ProjectileType<CosmosInvader>(), npc.damage / 4, 0f, Main.myPlayer, ai0);
                                }
                            }
                        }*/
                    }
                    else if (npc.ai[1] < 210)
                    {
                        int duration = 60 + Math.Max(2, 210 - (int)npc.ai[1]);

                        if (Main.LocalPlayer.active && !Main.LocalPlayer.dead)
                        {
                            Main.LocalPlayer.AddBuff(ModContent.BuffType<Buffs.Souls.TimeFrozen>(), duration);
                            //Main.LocalPlayer.AddBuff(BuffID.ChaosState, 300); //no cheesing this attack
                        }

                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            if (Main.npc[i].active)
                                Main.npc[i].AddBuff(ModContent.BuffType<Buffs.Souls.TimeFrozen>(), duration, true);
                        }

                        for (int i = 0; i < Main.maxProjectiles; i++)
                        {
                            if (Main.projectile[i].active && !Main.projectile[i].GetGlobalProjectile<FargoGlobalProjectile>().TimeFreezeImmune)
                                Main.projectile[i].GetGlobalProjectile<FargoGlobalProjectile>().TimeFrozen = duration;
                        }

                        
                        if (npc.ai[1] < 130 && ++npc.ai[2] > 12)
                        {
                            npc.ai[2] = 0;

                            bool altAttack = FargoSoulsWorld.MasochistMode && npc.localAI[2] != 0;

                            int baseDistance = 300; //altAttack ? 500 : 400;
                            float offset = altAttack ? 250f : 150f;
                            float speed = altAttack ? 4f : 2.5f;
                            int damage = npc.damage / 4; //altAttack ? npc.damage * 2 / 7 : npc.damage / 4;

                            if (npc.ai[1] < 130 - 45 || !altAttack)
                            {
                                if (altAttack && npc.ai[3] % 2 == 0) //emode p2, asgore rings
                                {
                                    float radius = baseDistance + npc.ai[3] * offset;
                                    int circumference = (int)(2f * (float)Math.PI * radius);

                                    //always flip it to opposite the previous side
                                    npc.localAI[0] = MathHelper.WrapAngle(npc.localAI[0] + (float)Math.PI + Main.rand.NextFloat((float)Math.PI / 2));
                                    const float safeRange = 60f;

                                    const int arcLength = 120;
                                    for (int i = 0; i < circumference; i += arcLength)
                                    {
                                        float angle = i / radius;
                                        if (angle > 2 * Math.PI - MathHelper.WrapAngle(MathHelper.ToRadians(safeRange)))
                                            continue;

                                        Vector2 spawnPos = player.Center + radius * Vector2.UnitX.RotatedBy(angle + npc.localAI[0]);
                                        Vector2 vel = speed * player.DirectionFrom(spawnPos);
                                        float ai0 = player.Distance(spawnPos) / speed + 30;
                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                            Projectile.NewProjectile(spawnPos, vel, ModContent.ProjectileType<CosmosInvaderTime>(), damage, 0f, Main.myPlayer, ai0, vel.ToRotation());
                                    }
                                }
                                else //scatter
                                {
                                    int max = altAttack ? 12 : 8 + (int)npc.ai[3] * (npc.localAI[2] == 0 ? 2 : 4);
                                    float rotationOffset = Main.rand.NextFloat((float)Math.PI * 2);
                                    for (int i = 0; i < max; i++)
                                    {
                                        float ai0 = baseDistance;
                                        float distance = ai0 + npc.ai[3] * offset;
                                        Vector2 spawnPos = player.Center + distance * Vector2.UnitX.RotatedBy(2 * Math.PI / max * i + rotationOffset);
                                        Vector2 vel = speed * player.DirectionFrom(spawnPos);// distance * player.DirectionFrom(spawnPos) / ai0;
                                        ai0 = distance / speed + 30;
                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                            Projectile.NewProjectile(spawnPos, vel, ModContent.ProjectileType<CosmosInvaderTime>(), damage, 0f, Main.myPlayer, ai0, vel.ToRotation());
                                    }
                                }
                            }

                            npc.ai[3]++;
                        }
                    }
                    
                    if (++npc.ai[1] > 480)
                    {
                        npc.TargetClosest();
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;
                        npc.localAI[0] = 0;
                        npc.netUpdate = true;
                    }
                    break;

                default:
                    npc.ai[0] = 0;
                    goto case 0;
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

        public override void FindFrame(int frameHeight)
        {
            if (++npc.frameCounter > 6)
            {
                npc.frameCounter = 0;
                npc.frame.Y += frameHeight;
            }

            if (npc.frame.Y > frameHeight * 4)
            {
                npc.frame.Y = 0;
            }

            switch ((int)npc.ai[0])
            {
                case -4:
                    npc.frame.Y = frameHeight * 5;
                    if (npc.localAI[0] >= 5)
                        npc.frame.Y = frameHeight * 6;
                    break;

                case -3:
                    if (npc.ai[2] < 30 || (npc.ai[2] > 100 && npc.ai[2] < 130))
                        npc.frame.Y = frameHeight * 8;
                    else if ((npc.ai[2] > 70 && npc.ai[2] < 100) || npc.ai[2] > 170)
                        npc.frame.Y = frameHeight * 7;
                    break;

                case -2:
                    npc.frame.Y = frameHeight * 5;
                    break;

                case -1:
                    if (npc.ai[1] > 120)
                        npc.frame.Y = frameHeight * 8;
                    else if (npc.ai[1] > 100)
                        npc.frame.Y = frameHeight * 7;
                    break;

                case 1:
                    if (npc.ai[2] <= 6)
                        npc.frame.Y = frameHeight * 5;
                    else
                        npc.frame.Y = frameHeight * 6;
                    break;

                case 3:
                    {
                        int threshold = 70; //npc.localAI[2] == 0 ? 70 : 50;
                        if (npc.ai[2] <= threshold)
                            npc.frame.Y = frameHeight * 5;
                        else
                            npc.frame.Y = frameHeight * 6;
                    }
                    break;

                case 5:
                    if (npc.ai[2] <= 75)
                        npc.frame.Y = frameHeight * 5;
                    else
                        npc.frame.Y = frameHeight * 6;
                    break;

                case 7:
                    if (npc.ai[1] < 30)
                        npc.frame.Y = frameHeight * 7;
                    else if (npc.ai[1] < 60)
                        npc.frame.Y = frameHeight * 8;
                    break;

                case 9:
                    if (npc.ai[2] <= 200)
                        npc.frame.Y = frameHeight * 5;
                    else
                        npc.frame.Y = frameHeight * 6;
                    break;

                case 11:
                    if (npc.ai[1] > 60)
                        npc.frame.Y = frameHeight * 6;
                    else
                        npc.frame.Y = frameHeight * 5;
                    break;

                case 13:
                    if (npc.ai[1] < 110)
                    {
                        if (npc.ai[2] <= 6)
                            npc.frame.Y = frameHeight * 5;
                        else
                            npc.frame.Y = frameHeight * 6;
                    }
                    else //uppercut time
                    {
                        if (npc.ai[1] <= 110 + 45)
                            npc.frame.Y = frameHeight * 5;
                        else
                            npc.frame.Y = frameHeight * 6;
                    }
                    break;

                case 15: //ZA WARUDO
                    if (npc.ai[1] < 10)
                        npc.frame.Y = frameHeight * 7;
                    else if (npc.ai[1] < 130)
                        npc.frame.Y = frameHeight * 8;
                    break;

                default:
                    break;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.MasochistMode)
            {
                target.AddBuff(BuffID.OnFire, 300);
                target.AddBuff(BuffID.Electrified, 300);
                target.AddBuff(ModContent.BuffType<Hexed>(), 300);
                target.AddBuff(BuffID.Frostburn, 300);
            }
        }

        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if (FargoSoulsWorld.MasochistMode)
                damage *= 0.9;
            return true;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                for (int i = 1; i <= 7; i++)
                {
                    Vector2 pos = npc.position + new Vector2(Main.rand.NextFloat(npc.width), Main.rand.NextFloat(npc.height));
                    Gore.NewGore(pos, npc.velocity, mod.GetGoreSlot("Gores/CosmosGore" + i.ToString()), npc.scale);
                }
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.SuperHealingPotion;
        }

        public override void NPCLoot()
        {
            FargoSoulsWorld.downedChampions[8] = true;
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData); //sync world

            FargoSoulsGlobalNPC.DropEnches(npc, ModContent.ItemType<Items.Accessories.Forces.CosmoForce>(), true);
            npc.DropItemInstanced(npc.position, npc.Size, ModContent.ItemType<LunarCrystal>(), 5);

            for (int i = 0; i < 10; i++)
            {
                Vector2 spawnPos = npc.position + new Vector2(Main.rand.Next(npc.width), Main.rand.Next(npc.height));
                int type = ModContent.ProjectileType<Projectiles.MutantBoss.MutantBomb>();
                Projectile.NewProjectile(spawnPos, Vector2.Zero, type, 0, 0f, Main.myPlayer);
            }
            Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.Masomode.MoonLordMoonBlast>(), 0, 0f, Main.myPlayer, -Vector2.UnitY.ToRotation(), 32);
        }

       public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);

            Texture2D npcTex = Main.npcTexture[npc.type];

            Texture2D npcGlow = mod.GetTexture("NPCs/Champions/CosmosChampion_Glow");
            Texture2D npcGlow2 = mod.GetTexture("NPCs/Champions/CosmosChampion_Glow2");
            Rectangle rectangle = npc.frame;//new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            SpriteEffects effects = npc.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Color npcColor = npc.GetAlpha(drawColor);

            Color glowColor = new Color(Main.DiscoR / 3 + 150, Main.DiscoG / 3 + 150, Main.DiscoB / 3 + 150);
            /*Color glowColor = new Color(Main.DiscoR / 3, Main.DiscoG / 3, Main.DiscoB / 3);
            switch (npc.ai[0])
            {
                case -3:
                    if (npc.ai[3] == 0) goto case 5;
                    else if (npc.ai[3] == 1) goto case 9;
                    else if (npc.ai[3] == 2) goto case 13;
                    else if (npc.ai[3] == 3) goto case 1;
                    goto default;

                case 0: if (npc.localAI[2] == 0) goto case default; else goto case 1;
                case 1: glowColor.G += 100; glowColor.B += 150; break;

                case 4: if (npc.localAI[2] == 0) goto case default; else goto case 5;
                case 5: glowColor.R += 150; glowColor.G += 50; glowColor.B += 50; break;

                case 8: if (npc.localAI[2] == 0) goto case default; else goto case 9;
                case 9: glowColor.G += 150; glowColor.B += 100; break;

                case 12: if (npc.localAI[2] == 0) goto case default; else goto case 13;
                case 13: glowColor.R += 125; glowColor.B += 125; break;

                default: glowColor.R += 150; glowColor.G += 150; glowColor.B += 150; break;
            }*/

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);

            if (npc.localAI[2] != 0)
            {
                for (int i = 0; i < NPCID.Sets.TrailCacheLength[npc.type]; i++)
                {
                    Vector2 value4 = npc.oldPos[i];
                    float num165 = npc.rotation; //npc.oldRot[i];
                    Main.spriteBatch.Draw(npcGlow, value4 + npc.Size / 2f - Main.screenPosition + new Vector2(0, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), glowColor * 0.5f, num165, origin2, npc.scale, effects, 0f);
                }
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);

            spriteBatch.Draw(npcTex, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), npc.GetAlpha(drawColor), npc.rotation, origin2, npc.scale, effects, 0f);
            spriteBatch.Draw(npcGlow2, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), glowColor, npc.rotation, origin2, npc.scale, effects, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            
            spriteBatch.Draw(npcGlow2, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), glowColor, npc.rotation, origin2, npc.scale, effects, 0f);
            
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            return false;
        }
    }
}
