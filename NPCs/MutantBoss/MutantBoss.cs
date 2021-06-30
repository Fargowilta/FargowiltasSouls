using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

using FargowiltasSouls.Buffs.Boss;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.Buffs.Souls;
using FargowiltasSouls.Projectiles.MutantBoss;
using FargowiltasSouls.Items.Summons;

namespace FargowiltasSouls.NPCs.MutantBoss
{
    [AutoloadBossHead]
    public class MutantBoss : ModNPC
    {
        public bool playerInvulTriggered;
        public int ritualProj, spriteProj, ringProj;
        private bool droppedSummon = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mutant");
            DisplayName.AddTranslation(GameCulture.Chinese, "突变体");
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.width = 120;//34;
            npc.height = 120;//50;
            npc.damage = 360;
            npc.defense = 360;
            npc.value = Item.buyPrice(2);
            npc.lifeMax = Main.expertMode ? 7000000 : 3500000;
            npc.HitSound = SoundID.NPCHit57;
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
            npc.buffImmune[ModContent.BuffType<Lethargic>()] = true;
            npc.buffImmune[ModContent.BuffType<Sadism>()] = true;
            npc.buffImmune[ModContent.BuffType<GodEater>()] = true;
            npc.buffImmune[ModContent.BuffType<ClippedWings>()] = true;
            npc.buffImmune[ModContent.BuffType<MutantNibble>()] = true;
            npc.buffImmune[ModContent.BuffType<OceanicMaul>()] = true;
            npc.buffImmune[ModContent.BuffType<TimeFrozen>()] = true;
            npc.buffImmune[ModContent.BuffType<LightningRod>()] = true;
            npc.timeLeft = NPC.activeTime * 30;
            if (FargoSoulsWorld.AngryMutant || Fargowiltas.Instance.CalamityLoaded)
            {
                npc.lifeMax = 177000000;
                npc.damage = (int)(npc.damage * 4);
                npc.defense *= 2;
                if (Fargowiltas.Instance.CalamityLoaded)
                {
                    npc.buffImmune[ModLoader.GetMod("CalamityMod").BuffType("ExoFreeze")] = true;
                    npc.buffImmune[ModLoader.GetMod("CalamityMod").BuffType("GlacialState")] = true;
                    npc.buffImmune[ModLoader.GetMod("CalamityMod").BuffType("TemporalSadness")] = true;
                    npc.buffImmune[ModLoader.GetMod("CalamityMod").BuffType("SilvaStun")] = true;
                    npc.buffImmune[ModLoader.GetMod("CalamityMod").BuffType("TimeSlow")] = true;
                    npc.buffImmune[ModLoader.GetMod("CalamityMod").BuffType("PearlAura")] = true;
                }
            }
            npc.GetGlobalNPC<FargoSoulsGlobalNPC>().SpecialEnchantImmune = true;
            if (Fargowiltas.Instance.MasomodeEXLoaded)
            {
                music = Fargowiltas.Instance.MasomodeEXCompatibility.ModInstance.GetSoundSlot(SoundType.Music, "Sounds/Music/rePrologue");
            }
            else
            {
                Mod musicMod = ModLoader.GetMod("FargowiltasMusic");
                if (musicMod == null)
                {
                    music = MusicID.LunarBoss;
                }
                else
                {
                    music = musicMod.GetSoundSlot(SoundType.Music, "Sounds/Music/SteelRed");
                }
            }
            musicPriority = (MusicPriority)12;

            bossBag = ModContent.ItemType<Items.Misc.MutantBag>();
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.damage = (int)(npc.damage * 0.5f);
            npc.lifeMax = (int)(npc.lifeMax * 0.5f * bossLifeScale);
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return npc.Distance(target.Center) < target.height / 2 + 20 && npc.ai[0] > -1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(npc.localAI[0]);
            writer.Write(npc.localAI[1]);
            writer.Write(npc.localAI[2]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc.localAI[0] = reader.ReadSingle();
            npc.localAI[1] = reader.ReadSingle();
            npc.localAI[2] = reader.ReadSingle();
        }

        private bool ProjectileExists(int id, int type)
        {
            return id > -1 && id < Main.maxProjectiles && Main.projectile[id].active && Main.projectile[id].type == type;
        }

        private void KillProjs()
        {
            if (!EModeGlobalNPC.OtherBossAlive(npc.whoAmI))
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                    if (Main.projectile[i].active && Main.projectile[i].damage > 0 && Main.projectile[i].hostile && i != ritualProj)
                        Main.projectile[i].Kill();
                for (int i = 0; i < Main.maxProjectiles; i++)
                    if (Main.projectile[i].active && Main.projectile[i].damage > 0 && Main.projectile[i].hostile && i != ritualProj)
                        Main.projectile[i].Kill();
            }
        }

        public override void AI()
        {
            EModeGlobalNPC.mutantBoss = npc.whoAmI;

            npc.dontTakeDamage = false;
            if (npc.ai[0] < 0)
                npc.dontTakeDamage = true;

            if (npc.localAI[3] == 0)
            {
                npc.TargetClosest();
                if (npc.timeLeft < 30)
                    npc.timeLeft = 30;
                if (npc.Distance(Main.player[npc.target].Center) < 2000)
                {
                    npc.localAI[3] = 1;
                    Main.PlaySound(SoundID.Roar, (int)npc.Center.X, (int)npc.Center.Y, 0);
                    //EdgyBossText("I hope you're ready to embrace suffering.");
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (Fargowiltas.Instance.MasomodeEXLoaded)
                            Projectile.NewProjectile(npc.Center, Vector2.Zero, ModLoader.GetMod("MasomodeEX").ProjectileType("MutantText"), 0, 0f, Main.myPlayer, npc.whoAmI);

                        if (FargoSoulsWorld.downedAbom && (Fargowiltas.Instance.MasomodeEXLoaded || FargoSoulsWorld.AngryMutant))// || Fargowiltas.Instance.CalamityLoaded))
                            Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<BossRush>(), 0, 0f, Main.myPlayer, npc.whoAmI);
                    }
                }
            }
            else if (npc.localAI[3] == 1)
            {
                Aura(2000f, ModContent.BuffType<GodEater>(), true, 86, false, false);
            }
            else if (Main.player[Main.myPlayer].active && npc.Distance(Main.player[Main.myPlayer].Center) < 3000f)
            {
                if (Main.expertMode)
                {
                    Main.player[Main.myPlayer].AddBuff(ModContent.BuffType<MutantPresence>(), 2);
                }
                if (Fargowiltas.Instance.CalamityLoaded)
                {
                    Main.player[Main.myPlayer].buffImmune[ModLoader.GetMod("CalamityMod").BuffType("RageMode")] = true;
                    Main.player[Main.myPlayer].buffImmune[ModLoader.GetMod("CalamityMod").BuffType("AdrenalineMode")] = true;
                }
            }

            if (Main.netMode != NetmodeID.MultiplayerClient) //checks for needed projs
            {
                if ((npc.ai[0] < 0 || npc.ai[0] > 10) && !ProjectileExists(ritualProj, ModContent.ProjectileType<MutantRitual>()))
                    ritualProj = Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<MutantRitual>(), npc.damage / 4, 0f, Main.myPlayer, 0f, npc.whoAmI);

                if (!ProjectileExists(ringProj, ModContent.ProjectileType<MutantRitual5>()))
                    ringProj = Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<MutantRitual5>(), 0, 0f, Main.myPlayer, 0f, npc.whoAmI);

                if (!ProjectileExists(spriteProj, ModContent.ProjectileType<Projectiles.MutantBoss.MutantBoss>()))
                {
                    /*if (Main.netMode == NetmodeID.Server)
                        NetMessage.BroadcastChatMessage(NetworkText.FromLiteral("wheres my sprite"), Color.LimeGreen);
                    else
                        Main.NewText("wheres my sprite");*/
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
                            Projectile projectile = Main.projectile[number];
                            projectile.SetDefaults(ModContent.ProjectileType<Projectiles.MutantBoss.MutantBoss>());
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
                    else //server
                    {
                        spriteProj = Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.MutantBoss.MutantBoss>(), 0, 0f, Main.myPlayer, 0f, npc.whoAmI);
                        /*if (Main.netMode == NetmodeID.Server)
                            NetMessage.BroadcastChatMessage(NetworkText.FromLiteral($"got sprite {spriteProj}"), Color.LimeGreen);
                        else
                            Main.NewText($"got sprite {spriteProj}");*/
                    }
                }
            }

            Player player = Main.player[npc.target];
            npc.direction = npc.spriteDirection = npc.Center.X < player.Center.X ? 1 : -1;
            Vector2 targetPos;
            float speedModifier;
            switch ((int)npc.ai[0])
            {
                case -7: //fade out, drop a mutant
                    npc.velocity = Vector2.Zero;
                    for (int i = 0; i < 5; i++)
                    {
                        int d = Dust.NewDust(npc.position, npc.width, npc.height, 229, 0f, 0f, 0, default(Color), 2.5f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].noLight = true;
                        Main.dust[d].velocity *= 12f;
                    }
                    if (--npc.localAI[0] < 0)
                    {
                        npc.localAI[0] = Main.rand.Next(5);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 spawnPos = npc.Center + Main.rand.NextVector2Circular(240, 240);
                            int type = ModContent.ProjectileType<Projectiles.BossWeapons.PhantasmalBlast>();
                            Projectile.NewProjectile(spawnPos, Vector2.Zero, type, 0, 0f, Main.myPlayer);
                        }
                    }
                    if (++npc.alpha > 255)
                    {
                        npc.alpha = 255;
                        npc.life = 0;
                        npc.dontTakeDamage = false;
                        npc.checkDead();
                        if (Main.netMode != NetmodeID.MultiplayerClient && !NPC.AnyNPCs(ModLoader.GetMod("Fargowiltas").NPCType("Mutant")))
                        {
                            int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModLoader.GetMod("Fargowiltas").NPCType("Mutant"));
                            if (n != Main.maxNPCs && Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                        }
                        //EdgyBossText("Oh, right... my revive...");
                    }
                    break;

                case -6: //actually defeated
                    if (!AliveCheck(player))
                        break;
                    npc.ai[3] -= (float)Math.PI / 6f / 60f;
                    npc.velocity = Vector2.Zero;
                    if (++npc.ai[1] > 120)
                    {
                        npc.netUpdate = true;
                        npc.ai[0]--;
                        npc.ai[1] = 0;
                        npc.ai[3] = (float)-Math.PI / 2;
                        npc.netUpdate = true;
                        if (Main.netMode != NetmodeID.MultiplayerClient) //shoot harmless mega ray
                            Projectile.NewProjectile(npc.Center, Vector2.UnitY * -1, ModContent.ProjectileType<MutantGiantDeathray>(), 0, 0f, Main.myPlayer, 0, npc.whoAmI);
                        //EdgyBossText("I have not a single regret in my existence!");
                    }
                    if (--npc.localAI[0] < 0)
                    {
                        npc.localAI[0] = Main.rand.Next(15);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 spawnPos = npc.position + new Vector2(Main.rand.Next(npc.width), Main.rand.Next(npc.height));
                            int type = ModContent.ProjectileType<Projectiles.BossWeapons.PhantasmalBlast>();
                            Projectile.NewProjectile(spawnPos, Vector2.Zero, type, 0, 0f, Main.myPlayer);
                        }
                    }
                    for (int i = 0; i < 5; i++)
                    {
                        int d = Dust.NewDust(npc.position, npc.width, npc.height, 229, 0f, 0f, 0, default(Color), 1.5f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].noLight = true;
                        Main.dust[d].velocity *= 4f;
                    }
                    break;

                case -5: //FINAL SPARK
                    if (!AliveCheck(player))
                        break;
                    npc.velocity = Vector2.Zero;

                    if (--npc.localAI[0] < 0)
                    {
                        npc.localAI[0] = Main.rand.Next(30);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 spawnPos = npc.position + new Vector2(Main.rand.Next(npc.width), Main.rand.Next(npc.height));
                            int type = ModContent.ProjectileType<Projectiles.BossWeapons.PhantasmalBlast>();
                            Projectile.NewProjectile(spawnPos, Vector2.Zero, type, 0, 0f, Main.myPlayer);
                        }
                    }

                    if (++npc.ai[1] > 120)
                    {
                        npc.ai[1] = 0;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            SpawnSphereRing(10, 6f, npc.damage / 3, 0.5f);
                            SpawnSphereRing(10, 6f, npc.damage / 3, -.5f);
                        }
                    }

                    if (++npc.ai[2] > 1020)
                    {
                        npc.netUpdate = true;
                        npc.ai[0]--;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        for (int i = 0; i < 1000; i++)
                            if (Main.projectile[i].active && Main.projectile[i].hostile && Main.projectile[i].timeLeft > 2)
                                Main.projectile[i].timeLeft = 2;
                    }
                    else if (npc.ai[2] == 420)
                    {
                        npc.ai[3] = npc.DirectionFrom(player.Center).ToRotation() - 0.001f;
                        npc.netUpdate = true;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(npc.Center, Vector2.UnitX.RotatedBy(npc.ai[3]),
                                ModContent.ProjectileType<MutantGiantDeathray2>(), npc.damage / 6, 0f, Main.myPlayer, 0, npc.whoAmI);
                        }
                    }
                    else if (npc.ai[2] < 420) //charging up dust
                    {
                        float num1 = 0.99f;
                        if (npc.ai[2] >= 84f)
                            num1 = 0.79f;
                        if (npc.ai[2] >= 168f)
                            num1 = 0.58f;
                        if (npc.ai[2] >= 252f)
                            num1 = 0.43f;
                        if (npc.ai[2] >= 336f)
                            num1 = 0.33f;
                        for (int i = 0; i < 9; ++i)
                        {
                            if (Main.rand.NextFloat() >= num1)
                            {
                                float f = Main.rand.NextFloat() * 6.283185f;
                                float num2 = Main.rand.NextFloat();
                                Dust dust = Dust.NewDustPerfect(npc.Center + f.ToRotationVector2() * (110 + 600 * num2), 229, (f - 3.141593f).ToRotationVector2() * (14 + 8 * num2), 0, default, 1f);
                                dust.scale = 0.9f;
                                dust.fadeIn = 1.15f + num2 * 0.3f;
                                //dust.color = new Color(1f, 1f, 1f, num1) * (1f - num1);
                                dust.noGravity = true;
                                //dust.noLight = true;
                            }
                        }
                    }

                    for (int i = 0; i < 5; i++)
                    {
                        int d = Dust.NewDust(npc.position, npc.width, npc.height, 229, 0f, 0f, 0, default(Color), 1.5f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].noLight = true;
                        Main.dust[d].velocity *= 4f;
                    }
                    npc.ai[3] -= GetSpinOffset();
                    break;

                case -4: //true boundary
                    if (!AliveCheck(player))
                        break;
                    npc.velocity = Vector2.Zero;
                    if (++npc.ai[1] > 3)
                    {
                        Main.PlaySound(SoundID.Item12, npc.Center);
                        npc.ai[1] = 0;
                        npc.ai[2] += (float)Math.PI / 5 / 480 * npc.ai[3];
                        if (npc.ai[2] > (float)Math.PI)
                            npc.ai[2] -= (float)Math.PI * 2;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            for (int i = 0; i < 8; i++)
                            {
                                Projectile.NewProjectile(npc.Center, new Vector2(6f, 0).RotatedBy(npc.ai[2] + Math.PI / 4 * i),
                                    ModContent.ProjectileType<MutantEye>(), npc.damage / 4, 0f, Main.myPlayer);
                            }
                        }
                    }
                    if (++npc.ai[3] > 480)
                    {
                        npc.TargetClosest();
                        npc.ai[0]--;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;
                        npc.netUpdate = true;
                    }
                    for (int i = 0; i < 5; i++)
                    {
                        int d = Dust.NewDust(npc.position, npc.width, npc.height, 229, 0f, 0f, 0, default(Color), 1.5f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].noLight = true;
                        Main.dust[d].velocity *= 4f;
                    }
                    break;

                case -3: //okuu nonspell
                    if (!AliveCheck(player))
                        break;
                    npc.velocity = Vector2.Zero;
                    if (++npc.ai[1] > 10 && npc.ai[3] > 60 && npc.ai[3] < 300)
                    {
                        npc.ai[1] = 0;
                        float rotation = MathHelper.ToRadians(45) * (npc.ai[3] - 60) / 240 * npc.ai[2];
                        SpawnSphereRing(11, 10f, npc.damage / 4, -0.75f, rotation);
                        SpawnSphereRing(11, 10f, npc.damage / 4, 0.75f, rotation);
                    }
                    if (npc.ai[2] == 0)
                    {
                        npc.ai[2] = Main.rand.Next(2) == 0 ? -1 : 1;
                        npc.ai[3] = Main.rand.NextFloat((float)Math.PI * 2);
                    }
                    if (++npc.ai[3] > 420)
                    {
                        npc.netUpdate = true;
                        npc.ai[0]--;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;
                        npc.TargetClosest();
                    }
                    for (int i = 0; i < 5; i++)
                    {
                        int d = Dust.NewDust(npc.position, npc.width, npc.height, 229, 0f, 0f, 0, default(Color), 1.5f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].noLight = true;
                        Main.dust[d].velocity *= 4f;
                    }
                    break;

                case -2: //final void rays
                    if (!AliveCheck(player))
                        break;
                    npc.velocity = Vector2.Zero;
                    if (--npc.ai[1] < 0)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(npc.Center, new Vector2(2, 0).RotatedBy(npc.ai[2]), ModContent.ProjectileType<MutantMark1>(), npc.damage / 4, 0f, Main.myPlayer);
                        }
                        npc.ai[1] = 1;
                        npc.ai[2] += npc.ai[3];
                        if (npc.localAI[0]++ == 40 || npc.localAI[0] == 80)
                        {
                            npc.netUpdate = true;
                            npc.ai[2] -= npc.ai[3] / 2;
                        }
                        else if (npc.localAI[0] == 120)
                        {
                            npc.netUpdate = true;
                            npc.ai[0]--;
                            npc.ai[1] = 0;
                            npc.ai[2] = 0;
                            npc.ai[3] = 0;
                            npc.localAI[0] = 0;
                        }
                    }
                    for (int i = 0; i < 5; i++)
                    {
                        int d = Dust.NewDust(npc.position, npc.width, npc.height, 229, 0f, 0f, 0, default(Color), 1.5f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].noLight = true;
                        Main.dust[d].velocity *= 4f;
                    }
                    break;

                case -1: //defeated
                    if (FargoSoulsWorld.MasochistMode)
                    {
                        if (!SkyManager.Instance["FargowiltasSouls:MutantBoss"].IsActive())
                            SkyManager.Instance.Activate("FargowiltasSouls:MutantBoss");

                        if (SoulConfig.Instance.MutantMusicIsRePrologue)
                        {
                            Mod musicMod = ModLoader.GetMod("FargowiltasMusic");
                            if (musicMod != null && musicMod.Version >= Version.Parse("1.0.1"))
                                music = ModLoader.GetMod("FargowiltasMusic").GetSoundSlot(SoundType.Music, "Sounds/Music/rePrologue");
                        }
                    }

                    //npc.damage = 0;
                    if (npc.buffType[0] != 0)
                        npc.DelBuff(0);
                    if (++npc.ai[1] > 180)
                    {
                        targetPos = player.Center;
                        targetPos.Y -= 300;
                        Movement(targetPos, 1f);
                        if (npc.Distance(targetPos) < 50 || npc.ai[1] > 360)
                        {
                            npc.netUpdate = true;
                            npc.velocity = Vector2.Zero;
                            npc.localAI[0] = 0;
                            npc.ai[0]--;
                            npc.ai[1] = 0;
                            npc.ai[2] = npc.DirectionFrom(player.Center).ToRotation();
                            npc.ai[3] = (float)Math.PI / 20f;
                            Main.PlaySound(SoundID.Roar, (int)npc.Center.X, (int)npc.Center.Y, 0);
                            if (player.Center.X < npc.Center.X)
                                npc.ai[3] *= -1;
                            //EdgyBossText("But we're not done yet!");
                        }
                    }
                    else
                    {
                        npc.velocity *= 0.9f;
                    }
                    for (int i = 0; i < 5; i++)
                    {
                        int d = Dust.NewDust(npc.position, npc.width, npc.height, 229, 0f, 0f, 0, default(Color), 1.5f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].noLight = true;
                        Main.dust[d].velocity *= 4f;
                    }
                    break;

                case 0: //track player, throw penetrators
                    if (!AliveCheck(player))
                        break;
                    if (Phase2Check())
                        break;
                    npc.localAI[2] = 0;
                    targetPos = player.Center;
                    targetPos.X += 500 * (npc.Center.X < targetPos.X ? -1 : 1);
                    if (npc.Distance(targetPos) > 50)
                    {
                        speedModifier = npc.localAI[3] > 0 ? 0.5f : 2f;
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
                        if (npc.localAI[3] > 0)
                        {
                            if (Math.Abs(npc.velocity.X) > 24)
                                npc.velocity.X = 24 * Math.Sign(npc.velocity.X);
                            if (Math.Abs(npc.velocity.Y) > 24)
                                npc.velocity.Y = 24 * Math.Sign(npc.velocity.Y);
                        }
                    }
                    if (npc.localAI[3] > 0)
                        npc.ai[1]++;
                    if (npc.ai[1] > 120)
                    {
                        npc.netUpdate = true;
                        npc.TargetClosest();
                        npc.ai[1] = 60;
                        if (++npc.ai[2] > 5)
                        {
                            npc.ai[0]++;
                            npc.ai[1] = 0;
                            npc.ai[2] = 0;
                            npc.velocity = npc.DirectionTo(player.Center) * 2f;
                        }
                        else if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(npc.Center, npc.DirectionTo(player.Center) * 25f, ModContent.ProjectileType<MutantSpearThrown>(), npc.damage / 4, 0f, Main.myPlayer, npc.target);
                        }
                    }
                    else if (npc.ai[1] == 61 && npc.ai[2] < 5 && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (FargoSoulsWorld.MasochistMode && FargoSoulsWorld.skipMutantP1 >= 10)
                        {
                            if (FargoSoulsWorld.skipMutantP1 == 10)
                            {
                                string text = "Mutant tires of the charade...";
                                if (Main.netMode == NetmodeID.SinglePlayer)
                                    Main.NewText(text, Color.LimeGreen);
                                else if (Main.netMode == NetmodeID.Server)
                                    NetMessage.BroadcastChatMessage(NetworkText.FromLiteral(text), Color.LimeGreen);
                            }
                            npc.ai[0] = 10;
                            npc.ai[1] = 0;
                            npc.ai[2] = 0;
                            npc.ai[3] = 0;
                            npc.netUpdate = true;
                            break;
                        }
                        Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<MutantSpearAim>(), npc.damage / 4, 0f, Main.myPlayer, npc.whoAmI);
                    }
                    break;

                case 1: //slow drift, shoot phantasmal rings
                    if (Phase2Check())
                        break;
                    if (--npc.ai[1] < 0)
                    {
                        npc.netUpdate = true;
                        npc.ai[1] = 90;
                        if (++npc.ai[2] > 4)
                        {
                            npc.ai[0]++;
                            npc.ai[1] = 0;
                            npc.ai[2] = 0;
                            npc.ai[3] = 0;
                            npc.netUpdate = true;
                            npc.TargetClosest();
                        }
                        else if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            SpawnSphereRing(6, 9f, npc.damage / 5, 1f);
                            SpawnSphereRing(6, 9f, npc.damage / 5, -0.5f);
                        }
                    }
                    break;

                case 2: //fly up to corner above player and then dive
                    if (!AliveCheck(player))
                        break;
                    if (Phase2Check())
                        break;
                    targetPos = player.Center;
                    targetPos.X += 700 * (npc.Center.X < targetPos.X ? -1 : 1);
                    targetPos.Y -= 400;
                    Movement(targetPos, 0.6f);
                    if (npc.Distance(targetPos) < 50 || ++npc.ai[1] > 180) //dive here
                    {
                        npc.velocity.X = 35f * (npc.position.X < player.position.X ? 1 : -1);
                        if (npc.velocity.Y < 0)
                            npc.velocity.Y *= -1;
                        npc.velocity.Y *= 0.3f;
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        npc.netUpdate = true;
                        Main.PlaySound(SoundID.Roar, (int)npc.Center.X, (int)npc.Center.Y, 0);
                    }
                    break;

                case 3: //diving, spawning true eyes
                    npc.velocity *= 0.99f;
                    if (--npc.ai[1] < 0)
                    {
                        npc.ai[1] = 20;
                        if (++npc.ai[2] > 3)
                        {
                            if (npc.ai[0] == 32)
                            {
                                /*float[] options = { 13, 21, 33, 40 };
                                npc.ai[0] = options[Main.rand.Next(options.Length)];*/
                                npc.ai[0]++;
                            }
                            else
                            {
                                npc.ai[0]++;
                            }
                            npc.ai[2] = 0;
                            npc.netUpdate = true;
                            npc.TargetClosest();
                        }
                        else
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int type = ModContent.ProjectileType<MutantTrueEyeL>();
                                if (npc.ai[2] == 2)
                                    type = ModContent.ProjectileType<MutantTrueEyeR>();
                                else if (npc.ai[2] == 3)
                                    type = ModContent.ProjectileType<MutantTrueEyeS>();
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, type, npc.damage / 5, 0f, Main.myPlayer, npc.target);
                            }
                            Main.PlaySound(SoundID.Item92, npc.Center);
                            for (int i = 0; i < 30; i++)
                            {
                                int d = Dust.NewDust(npc.position, npc.width, npc.height, 135, 0f, 0f, 0, default(Color), 3f);
                                Main.dust[d].noGravity = true;
                                Main.dust[d].noLight = true;
                                Main.dust[d].velocity *= 12f;
                            }
                        }
                    }
                    break;

                case 4: //maneuvering under player while spinning penetrator
                    if (Phase2Check())
                        break;
                    if (npc.ai[3] == 0)
                    {
                        if (!AliveCheck(player))
                            break;
                        npc.ai[3] = 1;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<MutantSpearSpin>(), npc.damage / 4, 0f, Main.myPlayer, npc.whoAmI);
                    }
                    targetPos = player.Center;
                    targetPos.Y += 400f;
                    Movement(targetPos, 0.7f, false);
                    if (++npc.ai[1] > 240)
                    {
                        npc.netUpdate = true;
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        npc.ai[3] = 0;
                    }
                    break;

                case 5: //pause and then initiate dash
                    if (Phase2Check())
                        break;
                    npc.velocity *= 0.9f;
                    if (++npc.ai[1] > 10)
                    {
                        npc.netUpdate = true;
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        if (++npc.ai[2] > 5)
                        {
                            npc.ai[0]++; //go to next attack after dashes
                            npc.ai[2] = 0;
                        }
                        else
                        {
                            npc.velocity = npc.DirectionTo(player.Center + player.velocity) * 30f;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<MutantSpearDash>(), npc.damage / 4, 0f, Main.myPlayer, npc.whoAmI);
                        }
                    }
                    break;

                case 6: //while dashing
                    npc.direction = npc.spriteDirection = Math.Sign(npc.velocity.X);
                    if (++npc.ai[1] > 30)
                    {
                        npc.netUpdate = true;
                        npc.ai[0]--;
                        npc.ai[1] = 0;
                    }
                    break;

                case 7: //approach for lasers
                    if (!AliveCheck(player))
                        break;
                    if (Phase2Check())
                        break;
                    targetPos = player.Center + player.DirectionTo(npc.Center) * 250;
                    if (npc.Distance(targetPos) > 50 && ++npc.ai[2] < 180)
                    {
                        Movement(targetPos, 0.5f);
                    }
                    else
                    {
                        npc.netUpdate = true;
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        npc.ai[2] = player.DirectionTo(npc.Center).ToRotation();
                        npc.ai[3] = (float)Math.PI / 10f;
                        if (player.Center.X < npc.Center.X)
                            npc.ai[3] *= -1;
                    }
                    break;

                case 8: //fire lasers in ring
                    if (Phase2Check())
                        break;
                    npc.velocity = Vector2.Zero;
                    if (--npc.ai[1] < 0)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(npc.Center, new Vector2(2, 0).RotatedBy(npc.ai[2]), ModContent.ProjectileType<MutantMark1>(), npc.damage / 4, 0f, Main.myPlayer);
                        npc.ai[1] = 5;
                        npc.ai[2] += npc.ai[3];
                        if (npc.localAI[0]++ == 20)
                        {
                            npc.netUpdate = true;
                            npc.ai[2] -= npc.ai[3] / 2;
                        }
                        else if (npc.localAI[0] == 40)
                        {
                            npc.netUpdate = true;
                            npc.ai[0]++;
                            npc.ai[1] = 0;
                            npc.ai[2] = 0;
                            npc.ai[3] = 0;
                            npc.localAI[0] = 0;
                        }
                    }
                    break;

                case 9: //boundary lite
                    switch ((int)npc.localAI[2])
                    {
                        case 0:
                            if (npc.ai[3] == 0)
                            {
                                if (AliveCheck(player))
                                    npc.ai[3] = 1;
                                else
                                    break;
                            }
                            if (Phase2Check())
                                break;
                            npc.velocity = Vector2.Zero;
                            if (++npc.ai[1] > 2)
                            {
                                Main.PlaySound(SoundID.Item12, npc.Center);
                                npc.ai[1] = 0;
                                npc.ai[2] += (float)Math.PI / 77f;
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    for (int i = 0; i < 4; i++)
                                        Projectile.NewProjectile(npc.Center, new Vector2(6f, 0).RotatedBy(npc.ai[2] + Math.PI / 2 * i), ModContent.ProjectileType<MutantEye>(), npc.damage / 4, 0f, Main.myPlayer);
                            }
                            if (++npc.ai[3] > 241)
                            {
                                npc.TargetClosest();
                                npc.localAI[2]++;
                                npc.ai[1] = 0;
                                npc.ai[2] = 0;
                                npc.ai[3] = 0;
                                npc.netUpdate = true;
                            }
                            break;

                        case 1: //spawn sword
                            if (Main.player[Main.myPlayer].active && npc.Distance(Main.player[Main.myPlayer].Center) < 3000f && Main.expertMode)
                                Main.player[Main.myPlayer].AddBuff(ModContent.BuffType<MutantPresence>(), 2);
                            npc.velocity = Vector2.Zero;
                            if (npc.ai[2] == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                double angle = npc.position.X < player.position.X ? -Math.PI / 4 : Math.PI / 4;
                                npc.ai[2] = (float)angle * -4f / 30;
                                const int spacing = 80;
                                Vector2 offset = Vector2.UnitY.RotatedBy(angle) * -spacing;
                                for (int i = 0; i < 12; i++)
                                    Projectile.NewProjectile(npc.Center + offset * i, Vector2.Zero, ModContent.ProjectileType<MutantSword>(), npc.damage / 3, 0f, Main.myPlayer, npc.whoAmI, spacing * i);
                                Projectile.NewProjectile(npc.Center + offset.RotatedBy(MathHelper.ToRadians(20)) * 7, Vector2.Zero, ModContent.ProjectileType<MutantSword>(), npc.damage / 3, 0f, Main.myPlayer, npc.whoAmI, 60 * 4);
                                Projectile.NewProjectile(npc.Center + offset.RotatedBy(MathHelper.ToRadians(-20)) * 7, Vector2.Zero, ModContent.ProjectileType<MutantSword>(), npc.damage / 3, 0f, Main.myPlayer, npc.whoAmI, 60 * 4);
                                Projectile.NewProjectile(npc.Center + offset.RotatedBy(MathHelper.ToRadians(40)) * 28, Vector2.Zero, ModContent.ProjectileType<MutantSword>(), npc.damage / 3, 0f, Main.myPlayer, npc.whoAmI, 60 * 4);
                                Projectile.NewProjectile(npc.Center + offset.RotatedBy(MathHelper.ToRadians(-40)) * 28, Vector2.Zero, ModContent.ProjectileType<MutantSword>(), npc.damage / 3, 0f, Main.myPlayer, npc.whoAmI, 60 * 4);
                            }
                            if (++npc.ai[1] > 120)
                            {
                                targetPos = player.Center;
                                targetPos.X -= 300 * npc.ai[2];
                                npc.velocity = (targetPos - npc.Center) / 30;

                                npc.localAI[2]++;
                                npc.ai[1] = 0;
                                npc.netUpdate = true;
                            }

                            npc.direction = npc.spriteDirection = Math.Sign(npc.ai[2]);
                            break;

                        case 2: //swinging sword dash
                            if (Main.player[Main.myPlayer].active && npc.Distance(Main.player[Main.myPlayer].Center) < 3000f && Main.expertMode)
                                Main.player[Main.myPlayer].AddBuff(ModContent.BuffType<MutantPresence>(), 2);

                            npc.ai[3] += npc.ai[2];
                            npc.direction = npc.spriteDirection = Math.Sign(npc.ai[2]);

                            if (++npc.ai[1] > 35)
                            {
                                if (!Main.dedServ && Main.LocalPlayer.active)
                                    Main.LocalPlayer.GetModPlayer<FargoPlayer>().Screenshake = 30;

                                npc.ai[0] = 0;
                                npc.ai[1] = 0;
                                npc.ai[2] = 0;
                                npc.ai[3] = 0;
                                npc.localAI[2] = 0;
                                npc.netUpdate = true;
                            }
                            break;

                        default:
                            break;
                    }
                    break;

                case 10: //phase 2 begins
                    npc.velocity *= 0.9f;
                    npc.dontTakeDamage = true;
                    if (npc.buffType[0] != 0)
                        npc.DelBuff(0);
                    if (npc.ai[1] == 0)
                    {
                        if (FargoSoulsWorld.MasochistMode)
                        {
                            Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<MutantRitual2>(), 0, 0f, Main.myPlayer, 0f, npc.whoAmI);
                            Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<MutantRitual3>(), 0, 0f, Main.myPlayer, 0f, npc.whoAmI);
                            Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<MutantRitual4>(), 0, 0f, Main.myPlayer, 0f, npc.whoAmI);
                        }
                    }

                    if (FargoSoulsWorld.MasochistMode)
                    {
                        if (SoulConfig.Instance.MutantMusicIsRePrologue)
                        {
                            Mod musicMod = ModLoader.GetMod("FargowiltasMusic");
                            if (musicMod != null && musicMod.Version >= Version.Parse("1.0.1"))
                                music = ModLoader.GetMod("FargowiltasMusic").GetSoundSlot(SoundType.Music, "Sounds/Music/rePrologue");
                        }
                    }

                    if (++npc.ai[1] > 120)
                    {
                        if (FargoSoulsWorld.MasochistMode)
                        {
                            if (!SkyManager.Instance["FargowiltasSouls:MutantBoss"].IsActive())
                                SkyManager.Instance.Activate("FargowiltasSouls:MutantBoss");
                        }

                        for (int i = 0; i < 5; i++)
                        {
                            int d = Dust.NewDust(npc.position, npc.width, npc.height, 229, 0f, 0f, 0, default(Color), 1.5f);
                            Main.dust[d].noGravity = true;
                            Main.dust[d].noLight = true;
                            Main.dust[d].velocity *= 4f;
                        }
                        npc.localAI[3] = 2;
                        if (FargoSoulsWorld.MasochistMode)
                        {
                            int heal = (int)(npc.lifeMax / 120 * Main.rand.NextFloat(1f, 1.5f));
                            npc.life += heal;
                            if (npc.life > npc.lifeMax)
                                npc.life = npc.lifeMax;
                            CombatText.NewText(npc.Hitbox, CombatText.HealLife, heal);
                        }
                        if (npc.ai[1] > 240)
                        {
                            if (FargoSoulsWorld.MasochistMode)
                            {
                                float[] options = { 11, 16, 18, 24, 26, 31, 35, 37, 39 };
                                npc.ai[0] = options[Main.rand.Next(options.Length)];
                            }
                            else
                            {
                                npc.ai[0]++;
                            }
                            npc.ai[1] = 0;
                            npc.ai[2] = 0;
                            npc.ai[3] = 0;
                            npc.localAI[0] = 0;
                            npc.netUpdate = true;
                        }
                    }
                    else if (npc.ai[1] == 120)
                    {
                        if (FargoSoulsWorld.MasochistMode && FargoSoulsWorld.skipMutantP1 <= 10)
                        {
                            FargoSoulsWorld.skipMutantP1++;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.WorldData);
                        }
                        for (int i = 0; i < Main.maxProjectiles; i++)
                            if (Main.projectile[i].active && Main.projectile[i].friendly && !Main.projectile[i].minion && Main.projectile[i].damage > 0)
                                Main.projectile[i].Kill();
                        for (int i = 0; i < Main.maxProjectiles; i++)
                            if (Main.projectile[i].active && Main.projectile[i].friendly && !Main.projectile[i].minion && Main.projectile[i].damage > 0)
                                Main.projectile[i].Kill();
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            ritualProj = Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<MutantRitual>(), npc.damage / 4, 0f, Main.myPlayer, 0f, npc.whoAmI);
                        }
                        Main.PlaySound(SoundID.Roar, (int)npc.Center.X, (int)npc.Center.Y, 0);
                        for (int i = 0; i < 50; i++)
                        {
                            int d = Dust.NewDust(Main.player[Main.myPlayer].position, Main.player[Main.myPlayer].width, Main.player[Main.myPlayer].height, 229, 0f, 0f, 0, default(Color), 2.5f);
                            Main.dust[d].noGravity = true;
                            Main.dust[d].noLight = true;
                            Main.dust[d].velocity *= 9f;
                        }
                        //if (player.GetModPlayer<FargoPlayer>().TerrariaSoul) EdgyBossText("Hand it over. That thing, your soul toggles.");
                    }
                    break;

                case 11: //approach for laser
                    if (!AliveCheck(player))
                        break;
                    targetPos = player.Center + player.DirectionTo(npc.Center) * 250;
                    if (npc.Distance(targetPos) > 50 && ++npc.ai[2] < 180)
                    {
                        Movement(targetPos, 0.8f);
                    }
                    else
                    {
                        npc.netUpdate = true;
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        npc.ai[2] = player.DirectionTo(npc.Center).ToRotation();
                        npc.ai[3] = (float)Math.PI / 10f;
                        npc.localAI[0] = 0;
                        Main.PlaySound(SoundID.Roar, (int)npc.Center.X, (int)npc.Center.Y, 0);
                        if (player.Center.X < npc.Center.X)
                            npc.ai[3] *= -1;
                    }
                    break;

                case 12: //fire lasers in ring
                    npc.velocity = Vector2.Zero;
                    if (--npc.ai[1] < 0)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(npc.Center, new Vector2(2, 0).RotatedBy(npc.ai[2]), ModContent.ProjectileType<MutantMark1>(), npc.damage / 4, 0f, Main.myPlayer);
                        npc.ai[1] = 3;
                        npc.ai[2] += npc.ai[3];
                        if (npc.localAI[0]++ == 20 || npc.localAI[0] == 40)
                        {
                            npc.netUpdate = true;
                            npc.ai[2] -= npc.ai[3] / 2;
                        }
                        else if (npc.localAI[0] == 60)
                        {
                            npc.TargetClosest();
                            npc.netUpdate = true;
                            /*float[] options = { 13, 16, 18, 20, 21, 24, 25, 26, 33, 39, 40, 41 };
                            npc.ai[0] = options[Main.rand.Next(options.Length)];*/
                            npc.ai[0]++;
                            npc.ai[1] = 0;
                            npc.ai[2] = 0;
                            npc.ai[3] = 0;
                            npc.localAI[0] = 0;
                        }
                    }
                    break;

                case 13: //maneuvering under player while spinning penetrator
                    if (npc.ai[3] == 0)
                    {
                        if (!AliveCheck(player))
                            break;
                        npc.ai[3] = 1;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<MutantSpearSpin>(), npc.damage / 4, 0f, Main.myPlayer, npc.whoAmI);
                    }
                    targetPos = player.Center;
                    targetPos.Y += 400f * Math.Sign(npc.Center.Y - player.Center.Y); //can be above or below
                    Movement(targetPos, 0.7f, false);
                    if (++npc.ai[1] > 180)
                    {
                        npc.netUpdate = true;
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        npc.ai[3] = 0;
                        npc.TargetClosest();
                    }
                    break;

                case 14: //pause and then initiate dash
                    npc.velocity *= 0.9f;

                    if (npc.ai[1] == 20) //telegraph
                    {
                        if (npc.ai[2] < 5 && Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(npc.Center, npc.DirectionTo(player.Center + player.velocity * 30f), ModContent.ProjectileType<MutantDeathrayAim>(), 0, 0f, Main.myPlayer, 30f, npc.whoAmI);
                    }

                    if (npc.ai[1] < 50) //track player up until just before dash
                    {
                        npc.localAI[0] = npc.DirectionTo(player.Center + player.velocity * 30f).ToRotation();
                    }

                    if (++npc.ai[1] > 60)
                    {
                        npc.netUpdate = true;
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        npc.ai[3] = 0;
                        if (++npc.ai[2] > 5)
                        {
                            /*float[] options = { 11, 16, 18, 20, 21, 24, 25, 26, 33, 35, 39, 40, 41 };
                            npc.ai[0] = options[Main.rand.Next(options.Length)]; //go to next attack after dashes*/
                            npc.ai[0]++;
                            npc.ai[2] = 0;
                        }
                        else
                        {
                            npc.velocity = npc.localAI[0].ToRotationVector2() * 45f;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(npc.Center, Vector2.Normalize(npc.velocity), ModContent.ProjectileType<MutantDeathray2>(), npc.damage / 5, 0f, Main.myPlayer);
                                Projectile.NewProjectile(npc.Center, -Vector2.Normalize(npc.velocity), ModContent.ProjectileType<MutantDeathray2>(), npc.damage / 5, 0f, Main.myPlayer);
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<MutantSpearDash>(), npc.damage / 4, 0f, Main.myPlayer, npc.whoAmI, 1f);
                            }
                        }
                        npc.localAI[0] = 0;
                    }
                    break;

                case 15: //while dashing
                    npc.direction = npc.spriteDirection = Math.Sign(npc.velocity.X);
                    if (++npc.ai[1] > 30)
                    {
                        npc.netUpdate = true;
                        npc.ai[0]--;
                        npc.ai[1] = 0;
                    }
                    break;

                case 16: //approach for bullet hell
                    goto case 11;

                case 17: //BOUNDARY OF WAVE AND PARTICLE
                    npc.velocity = Vector2.Zero;
                    if (++npc.ai[1] > 2)
                    {
                        Main.PlaySound(SoundID.Item12, npc.Center);
                        npc.ai[1] = 0;
                        npc.ai[2] += (float)Math.PI / 8 / 480 * npc.ai[3];
                        if (npc.ai[2] > (float)Math.PI)
                            npc.ai[2] -= (float)Math.PI * 2;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int max = FargoSoulsWorld.MasochistMode ? 5 : 4;
                            for (int i = 0; i < max; i++)
                            {
                                Projectile.NewProjectile(npc.Center, new Vector2(6f, 0).RotatedBy(npc.ai[2] + Math.PI * 2 / max * i),
                                      ModContent.ProjectileType<MutantEye>(), npc.damage / 4, 0f, Main.myPlayer);
                            }
                        }
                    }
                    if (++npc.ai[3] > 360)
                    {
                        npc.TargetClosest();
                        /*float[] options = { 11, 18, 20, 25, 40 };
                        npc.ai[0] = options[Main.rand.Next(options.Length)];*/
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;
                        npc.netUpdate = true;
                    }
                    break;

                case 18: //spawn illusions for next attack
                    Main.PlaySound(SoundID.Roar, (int)npc.Center.X, (int)npc.Center.Y, 0);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<MutantIllusion>(), npc.whoAmI, npc.whoAmI, -1, 1, 240);
                        if (n != Main.maxNPCs && Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                        n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<MutantIllusion>(), npc.whoAmI, npc.whoAmI, 1, -1, 120);
                        if (n != Main.maxNPCs && Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                        n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<MutantIllusion>(), npc.whoAmI, npc.whoAmI, 1, 1, 180);
                        if (n != Main.maxNPCs && Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                    }
                    npc.ai[0]++;
                    break;

                case 19: //QUADRUPLE PILLAR ROAD ROLLER
                    if (!AliveCheck(player))
                        break;

                    if (npc.ai[2] == 0 && npc.ai[3] == 0) //target one corner of arena
                    {
                        npc.netUpdate = true;
                        npc.ai[2] = npc.Center.X;
                        npc.ai[3] = npc.Center.Y;
                        for (int i = 0; i < Main.maxProjectiles; i++)
                        {
                            if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<MutantRitual>() && Main.projectile[i].ai[1] == npc.whoAmI)
                            {
                                npc.ai[2] = Main.projectile[i].Center.X;
                                npc.ai[3] = Main.projectile[i].Center.Y;
                                break;
                            }
                        }

                        Vector2 offset = 1000f * Vector2.UnitX.RotatedBy(MathHelper.ToRadians(45));
                        if (Main.rand.NextBool()) //always go to a side player isn't in but pick a way to do it randomly
                        {
                            if (player.Center.X > npc.ai[2])
                                offset.X *= -1;
                            if (Main.rand.NextBool())
                                offset.Y *= -1;
                        }
                        else
                        {
                            if (Main.rand.NextBool())
                                offset.X *= -1;
                            if (player.Center.Y > npc.ai[3])
                                offset.Y *= -1;
                        }

                        npc.localAI[1] = npc.ai[2]; //for illusions
                        npc.localAI[2] = npc.ai[3];

                        npc.ai[2] = offset.Length();
                        npc.ai[3] = offset.ToRotation();
                    }

                    /*if (npc.ai[1] < 360)
                    {
                        targetPos = new Vector2(npc.localAI[1], npc.localAI[2]);

                        Vector2 offset = npc.ai[3].ToRotationVector2();
                        offset *= Math.Min(npc.ai[2], (targetPos - player.Center).Length() + 500); //do this to not go totally offscreen asap.
                        targetPos += offset;
                    }
                    else
                    {
                        targetPos = player.Center + npc.DirectionFrom(player.Center) * 450;
                    }*/

                    targetPos = player.Center;
                    targetPos.X += npc.Center.X < player.Center.X ? -700 : 700;
                    targetPos.Y += npc.ai[1] < 240 ? 400 : 150;

                    if (npc.Distance(targetPos) > 50)
                        Movement(targetPos, 1f);
                    
                    if (++npc.ai[1] > 420)
                    {
                        npc.TargetClosest();
                        /*float[] options = { 13, 20, 21, 24, 25, 35, 39, 40 };
                        npc.ai[0] = options[Main.rand.Next(options.Length)];*/
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;
                        npc.netUpdate = true;
                    }
                    else if (npc.ai[1] == 60)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(npc.Center, Vector2.UnitY * -5, ModContent.ProjectileType<MutantPillar>(), npc.damage / 3, 0, Main.myPlayer, 3, npc.whoAmI);
                    }
                    break;

                case 20: //eoc bullet hell, was blood sickle mines
                    if (!AliveCheck(player))
                        break;

                    if (npc.ai[1] == 0)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(npc.Center, -Vector2.UnitY, ModContent.ProjectileType<MutantEyeOfCthulhu>(), npc.damage / 4, 0f, Main.myPlayer, npc.target);
                    }
                    if (npc.ai[1] < 120) //stop tracking when eoc begins attacking, this locks arena in place
                    {
                        npc.ai[2] = player.Center.X;
                        npc.ai[3] = player.Center.Y;
                    }

                    if (npc.Distance(player.Center) < 250)
                    {
                        Movement(npc.Center + npc.DirectionFrom(player.Center), 0.9f);
                    }
                    else
                    {
                        targetPos = new Vector2(npc.ai[2], npc.ai[3]);
                        targetPos += npc.DirectionFrom(targetPos).RotatedBy(MathHelper.ToRadians(-10)) * 300f;
                        if (npc.Distance(targetPos) > 50)
                            Movement(targetPos, 0.25f);
                    }

                    if (++npc.ai[1] > 450)
                    {
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;
                        npc.TargetClosest();
                        npc.netUpdate = true;
                    }

                    /*if (Math.Abs(targetPos.X - player.Center.X) < 150) //avoid crossing up player
                    {
                        targetPos.X = player.Center.X + 150 * Math.Sign(targetPos.X - player.Center.X);
                        Movement(targetPos, 0.3f);
                    }
                    if (npc.Distance(targetPos) > 50)
                    {
                        Movement(targetPos, 0.5f);
                    }

                    if (--npc.ai[1] < 0)
                    {
                        npc.ai[1] = 60;
                        if (++npc.ai[2] > (FargoSoulsWorld.MasochistMode ? 3 : 1))
                        {
                            //float[] options = { 13, 18, 21, 24, 26, 31, 33, 40 }; npc.ai[0] = options[Main.rand.Next(options.Length)];
                            npc.ai[0]++;
                            npc.ai[2] = 0;
                            npc.TargetClosest();
                        }
                        else
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                for (int i = 0; i < 8; i++)
                                    Projectile.NewProjectile(npc.Center, Vector2.UnitX.RotatedBy(Math.PI / 4 * i) * 10f, ModContent.ProjectileType<MutantScythe1>(), npc.damage / 5, 0f, Main.myPlayer, npc.whoAmI);
                            Main.PlaySound(SoundID.ForceRoar, (int)npc.Center.X, (int)npc.Center.Y, -1, 1f, 0f);
                        }
                        npc.netUpdate = true;
                        break;
                    }*/
                    break;

                case 21: //maneuver above while spinning penetrator
                    if (npc.ai[3] == 0)
                    {
                        if (!AliveCheck(player))
                            break;
                        npc.ai[3] = 1;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<MutantSpearSpin>(), npc.damage / 4, 0f, Main.myPlayer, npc.whoAmI);
                    }
                    targetPos = player.Center;
                    targetPos.Y += 400f * Math.Sign(npc.Center.Y - player.Center.Y); //can be above or below
                    Movement(targetPos, 0.7f, false);
                    if (++npc.ai[1] > 180)
                    {
                        npc.netUpdate = true;
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        npc.ai[3] = 0;
                        npc.TargetClosest();
                    }
                    break;

                case 22: //pause and then initiate dash
                    npc.velocity *= 0.9f;
                    if (++npc.ai[1] > (FargoSoulsWorld.MasochistMode ? 10 : 20))
                    {
                        npc.netUpdate = true;
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        if (++npc.ai[2] > 5)
                        {
                            /*float[] options = { 13, 24, 25, 31, 41 };
                            npc.ai[0] = options[Main.rand.Next(options.Length)]; //go to next attack after dashes*/
                            npc.ai[0]++;
                            npc.ai[2] = 0;
                        }
                        else
                        {
                            npc.velocity = npc.DirectionTo(player.Center) * 45f;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(npc.Center, Vector2.Normalize(npc.velocity), ModContent.ProjectileType<MutantDeathray2>(), npc.damage / 5, 0f, Main.myPlayer);
                                Projectile.NewProjectile(npc.Center, -Vector2.Normalize(npc.velocity), ModContent.ProjectileType<MutantDeathray2>(), npc.damage / 5, 0f, Main.myPlayer);
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<MutantSpearDash>(), npc.damage / 4, 0f, Main.myPlayer, npc.whoAmI);
                            }
                        }
                    }
                    break;

                case 23: //while dashing
                    goto case 15;

                case 24: //destroyers
                    if (!AliveCheck(player))
                        break;
                    if (FargoSoulsWorld.MasochistMode)
                    {
                        targetPos = player.Center + npc.DirectionFrom(player.Center) * 300;
                        if (Math.Abs(targetPos.X - player.Center.X) < 150) //avoid crossing up player
                        {
                            targetPos.X = player.Center.X + 150 * Math.Sign(targetPos.X - player.Center.X);
                            Movement(targetPos, 0.3f);
                        }
                        if (npc.Distance(targetPos) > 50)
                        {
                            Movement(targetPos, 0.9f);
                        }
                    }
                    else
                    {
                        targetPos = player.Center;
                        targetPos.X += 500 * (npc.Center.X < targetPos.X ? -1 : 1);
                        if (npc.Distance(targetPos) > 50)
                        {
                            Movement(targetPos, 0.4f);
                        }
                    }

                    if (++npc.ai[1] > 60)
                    {
                        npc.netUpdate = true;
                        npc.ai[1] = 30;
                        if (++npc.ai[2] > (FargoSoulsWorld.MasochistMode ? 5 : 3))
                        {
                            npc.TargetClosest();
                            /*float[] options = { 13, 20, 21, 25, 29, 31, 35, 40 };
                            npc.ai[0] = options[Main.rand.Next(options.Length)];*/
                            npc.ai[0]++;
                            npc.ai[1] = 0;
                            npc.ai[2] = 0;
                        }
                        else
                        {
                            Main.PlaySound(SoundID.NPCKilled, (int)npc.Center.X, (int)npc.Center.Y, 13);
                            if (Main.netMode != NetmodeID.MultiplayerClient) //spawn worm
                            {
                                Vector2 vel = npc.DirectionFrom(player.Center).RotatedByRandom(MathHelper.ToRadians(120)) * 10f;
                                int current = Projectile.NewProjectile(npc.Center, vel, ModContent.ProjectileType<MutantDestroyerHead>(), npc.damage / 4, 0f, Main.myPlayer, npc.target);
                                for (int i = 0; i < 18; i++)
                                    current = Projectile.NewProjectile(npc.Center, vel, ModContent.ProjectileType<MutantDestroyerBody>(), npc.damage / 4, 0f, Main.myPlayer, Main.projectile[current].identity);
                                int previous = current;
                                current = Projectile.NewProjectile(npc.Center, vel, ModContent.ProjectileType<MutantDestroyerTail>(), npc.damage / 4, 0f, Main.myPlayer, Main.projectile[current].identity);
                                Main.projectile[previous].localAI[1] = Main.projectile[current].identity;
                                Main.projectile[previous].netUpdate = true;
                            }
                        }
                    }
                    break;

                case 25: //improved throw
                    if (!AliveCheck(player))
                        break;
                    targetPos = player.Center;
                    targetPos.X += 500 * (npc.Center.X < targetPos.X ? -1 : 1);
                    if (npc.Distance(targetPos) > 25)
                    {
                        Movement(targetPos, 0.8f);
                    }
                    if (++npc.ai[1] > 60)
                    {
                        npc.netUpdate = true;
                        npc.ai[1] = 0;
                        if (++npc.ai[2] > 5)
                        {
                            /*float[] options = { 11, 13, 20, 21, 24, 26, 31, 39, 40, 41 };
                            npc.ai[0] = options[Main.rand.Next(options.Length)];*/
                            npc.ai[0]++;
                            npc.ai[1] = 0;
                            npc.ai[2] = 0;
                            npc.TargetClosest();
                        }
                        else if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 vel = npc.DirectionTo(player.Center + player.velocity * 30f) * 30f;
                            Projectile.NewProjectile(npc.Center, Vector2.Normalize(vel), ModContent.ProjectileType<MutantDeathray2>(), npc.damage / 5, 0f, Main.myPlayer);
                            Projectile.NewProjectile(npc.Center, -Vector2.Normalize(vel), ModContent.ProjectileType<MutantDeathray2>(), npc.damage / 5, 0f, Main.myPlayer);
                            Projectile.NewProjectile(npc.Center, vel, ModContent.ProjectileType<MutantSpearThrown>(), npc.damage / 4, 0f, Main.myPlayer, npc.target, 1f);
                        }
                    }
                    else if (npc.ai[1] == 1 && npc.ai[2] < 5 && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(npc.Center, npc.DirectionTo(player.Center + player.velocity * 30f), ModContent.ProjectileType<MutantDeathrayAim>(), 0, 0f, Main.myPlayer, 60f, npc.whoAmI);
                        Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<MutantSpearAim>(), npc.damage / 4, 0f, Main.myPlayer, npc.whoAmI, 2);
                    }
                    break;

                case 26: //back away, prepare for ultra laser spam
                    if (!AliveCheck(player))
                        break;
                    targetPos = player.Center;
                    targetPos.X += 600 * (npc.Center.X < targetPos.X ? -1 : 1);
                    if (npc.Distance(targetPos) > 25)
                    {
                        Movement(targetPos, 0.5f);
                    }
                    if (npc.ai[1] == 0)
                    {
                        Main.PlaySound(SoundID.ForceRoar, npc.Center, -1); //eoc roar
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.GlowRing>(), 0, 0f, Main.myPlayer, npc.whoAmI, NPCID.Retinazer);
                    }
                    if (++npc.ai[1] > 120)
                    {
                        npc.netUpdate = true;
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;
                        Main.PlaySound(SoundID.Roar, (int)npc.Center.X, (int)npc.Center.Y, 0);
                        npc.TargetClosest();
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        int d = Dust.NewDust(npc.Center, 0, 0, DustID.Fire, 0f, 0f, 0, default(Color), 3f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].noLight = true;
                        Main.dust[d].velocity *= 12f;
                    }
                    break;

                case 27: //reti fan and prime rain
                    npc.velocity = Vector2.Zero;
                    if (npc.ai[2] == 0)
                    {
                        npc.ai[2] = Main.rand.Next(2) == 0 ? -1 : 1; //randomly aim either up or down
                    }
                    if(npc.ai[3] == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int max = 7;
                        for(int i = 0; i <= max; i++)
                        {
                            Vector2 dir = Vector2.UnitX.RotatedBy(npc.ai[2] * i * MathHelper.Pi / max) * 6; //rotate initial velocity of telegraphs by 180 degrees depending on velocity of lasers
                            Projectile.NewProjectile(npc.Center + dir, Vector2.Zero, mod.ProjectileType("MutantGlowything"), 0, 0f, Main.myPlayer, dir.ToRotation(), npc.whoAmI);
                        }
                    }
                    if (npc.ai[3] > 20 && npc.ai[3] < 240 && ++npc.ai[1] > 10)
                    {
                        npc.ai[1] = 0;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            float rotation = MathHelper.ToRadians(245) / 100f * npc.ai[2];
                            Projectile.NewProjectile(npc.Center, Vector2.UnitX.RotatedBy(MathHelper.ToRadians(8 * npc.ai[2])), 
                                ModContent.ProjectileType<MutantDeathray3>(), npc.damage / 4, 0, Main.myPlayer, rotation, npc.whoAmI);
                            Projectile.NewProjectile(npc.Center, -Vector2.UnitX.RotatedBy(MathHelper.ToRadians(-8 * npc.ai[2])), 
                                ModContent.ProjectileType<MutantDeathray3>(), npc.damage / 4, 0, Main.myPlayer, -rotation, npc.whoAmI);
                        }
                    }
                    if (npc.ai[3] == 90 && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 spawnPos = npc.Center;
                        spawnPos.Y -= npc.ai[2] * 600;
                        for (int i = -3; i <= 3; i++)
                        {
                            Projectile.NewProjectile(spawnPos + Vector2.UnitY * 120 * i, Vector2.Zero,
                                ModContent.ProjectileType<MutantReticle2>(), 0, 0f, Main.myPlayer);
                        }
                    }
                    if (npc.ai[3] == 180)
                    {
                        Main.PlaySound(SoundID.Roar, (int)npc.Center.X, (int)npc.Center.Y, 0);
                    }
                    if (npc.ai[3] > 180 && ++npc.localAI[0] > 2)
                    {
                        npc.localAI[0] = 0;
                        if (npc.localAI[1] > 0)
                            npc.localAI[1] = -1;
                        else
                            npc.localAI[1] = 1;
                        Main.PlaySound(SoundID.Item21, npc.Center);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 spawnPos = npc.Center;
                            spawnPos.Y -= npc.ai[2] * 600;
                            spawnPos.X += Main.rand.NextFloat(-400, 400);
                            spawnPos.Y -= 1000 * npc.localAI[1];
                            Vector2 vel = npc.Center;
                            vel.Y -= npc.ai[2] * 600;
                            vel -= spawnPos;
                            vel.Normalize();
                            vel *= 18;
                            Projectile.NewProjectile(spawnPos, vel, ModContent.ProjectileType<MutantGuardian>(), npc.damage / 3, 0f, Main.myPlayer);
                        }
                    }
                    if (++npc.ai[3] > 300)
                    {
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;
                        npc.localAI[0] = 0;
                        npc.localAI[1] = 0;
                        npc.netUpdate = true;
                        npc.TargetClosest();
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        int d = Dust.NewDust(npc.Center, 0, 0, DustID.Fire, 0f, 0f, 0, default(Color), 3f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].noLight = true;
                        Main.dust[d].velocity *= 12f;
                    }
                    break;

                case 28: //on standby, wait for previous attack to clear
                    if (++npc.ai[3] > 30)
                    {
                        /*float[] options = { 29, 31, 39, 41 };
                        npc.ai[0] = options[Main.rand.Next(options.Length)];*/
                        if (FargoSoulsWorld.MasochistMode)
                        {
                            npc.ai[0]++;
                        }
                        else
                        {
                            npc.ai[0] = 11;
                        }
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;
                        npc.netUpdate = true;
                    }
                    break;

                case 29: //prepare to fishron dive
                    if (!AliveCheck(player))
                        break;
                    npc.ai[0]++;
                    npc.velocity.X = 35f * (npc.position.X < player.position.X ? 1 : -1);
                    //npc.velocity.Y = -10f;
                    break;

                case 30: //spawn fishrons
                    npc.velocity *= 0.98f;
                    if (++npc.ai[1] == 1)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            bool random = Main.rand.Next(2) == 0; //fan above or to sides
                            for (int j = -1; j <= 1; j++) //to both sides of player
                            {
                                if (j == 0)
                                    continue;

                                for (int i = -1; i <= 1; i++) //fan of fishron
                                {
                                    Vector2 offset = random ? Vector2.UnitY.RotatedBy(Math.PI / 3 / 3 * i) * -450f * j : Vector2.UnitX.RotatedBy(Math.PI / 3 / 3 * i) * 475f * j;
                                    Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<MutantFishron>(), npc.damage / 4, 0f, Main.myPlayer, offset.X, offset.Y);
                                }
                            }
                        }
                        for (int i = 0; i < 30; i++)
                        {
                            int d = Dust.NewDust(npc.position, npc.width, npc.height, 135, 0f, 0f, 0, default(Color), 3f);
                            Main.dust[d].noGravity = true;
                            Main.dust[d].noLight = true;
                            Main.dust[d].velocity *= 12f;
                        }
                    }
                    if (++npc.ai[2] > 90)
                    {
                        /*float[] options = { 11, 18, 26, 31, 33 };
                        npc.ai[0] = options[Main.rand.Next(options.Length)];*/
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.netUpdate = true;
                        npc.TargetClosest();
                    }
                    break;

                case 31: //maneuver below for dive
                    if (!AliveCheck(player))
                        break;
                    targetPos = player.Center;
                    targetPos.X += 400 * (npc.Center.X < targetPos.X ? -1 : 1);
                    targetPos.Y += 400;
                    Movement(targetPos, 1.2f);
                    if (++npc.ai[1] > 60) //dive here
                    {
                        npc.velocity.X = 30f * (npc.position.X < player.position.X ? 1 : -1);
                        if (npc.velocity.Y > 0)
                            npc.velocity.Y *= -1;
                        npc.velocity.Y *= 0.3f;
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        npc.netUpdate = true;
                    }
                    break;

                case 32: //spawn eyes
                    goto case 3;

                case 33: //toss nuke, set vel
                    if (!AliveCheck(player))
                        break;
                    targetPos = player.Center;
                    targetPos.X += 400 * (npc.Center.X < targetPos.X ? -1 : 1);
                    targetPos.Y -= 400;
                    Movement(targetPos, 0.6f, false);
                    if (++npc.ai[1] > 180)
                    {
                        Main.PlaySound(SoundID.Roar, (int)npc.Center.X, (int)npc.Center.Y, 0);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            float gravity = 0.2f;
                            const float time = 180f;
                            Vector2 distance = player.Center - npc.Center;
                            distance.X = distance.X / time;
                            distance.Y = distance.Y / time - 0.5f * gravity * time;
                            Projectile.NewProjectile(npc.Center, distance, ModContent.ProjectileType<MutantNuke>(), 0, 0f, Main.myPlayer, gravity);
                            Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<MutantFishronRitual>(), 0, 0f, Main.myPlayer, npc.whoAmI);
                        }
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        if (Math.Sign(player.Center.X - npc.Center.X) == Math.Sign(npc.velocity.X))
                            npc.velocity.X *= -1f;
                        if (npc.velocity.Y < 0)
                            npc.velocity.Y *= -1f;
                        npc.netUpdate = true;
                        npc.TargetClosest();
                    }
                    break;

                case 34: //slow drift, protective aura above self
                    if (!AliveCheck(player))
                        break;
                    npc.velocity.Normalize();
                    npc.velocity *= 3f;
                    if (npc.ai[1] > 180)
                    {
                        if (!Main.dedServ && Main.LocalPlayer.active)
                            Main.LocalPlayer.GetModPlayer<FargoPlayer>().Screenshake = 2;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 safeZone = npc.Center;
                            safeZone.Y -= 100;
                            const float safeRange = 150 + 200;
                            for (int i = 0; i < 3; i++)
                            {
                                Vector2 spawnPos = npc.Center + Main.rand.NextVector2Circular(1200, 1200);
                                if (Vector2.Distance(safeZone, spawnPos) < safeRange)
                                {
                                    Vector2 directionOut = spawnPos - safeZone;
                                    directionOut.Normalize();
                                    spawnPos = safeZone + directionOut * Main.rand.NextFloat(safeRange, 1200);
                                }
                                Projectile.NewProjectile(spawnPos, Vector2.Zero, ModContent.ProjectileType<MutantBomb>(), npc.damage / 4, 0f, Main.myPlayer);
                            }
                        }
                    }
                    if (++npc.ai[1] > 360)
                    {
                        /*float[] options = { 11, 13, 16, 18, 20, 21, 24, 25, 26, 39, 40, 41 };
                        npc.ai[0] = options[Main.rand.Next(options.Length)];*/
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.netUpdate = true;
                        npc.TargetClosest();
                    }
                    for (int i = 0; i < 20; i++)
                    {
                        Vector2 offset = new Vector2();
                        offset.Y -= 100;
                        double angle = Main.rand.NextDouble() * 2d * Math.PI;
                        offset.X += (float)(Math.Sin(angle) * 150);
                        offset.Y += (float)(Math.Cos(angle) * 150);
                        Dust dust = Main.dust[Dust.NewDust(npc.Center + offset - new Vector2(4, 4), 0, 0, 229, 0, 0, 100, Color.White, 1.5f)];
                        dust.velocity = npc.velocity;
                        if (Main.rand.Next(3) == 0)
                            dust.velocity += Vector2.Normalize(offset) * 5f;
                        dust.noGravity = true;
                    }
                    break;

                case 35: //flee to prepare for slime rain
                    if (!AliveCheck(player))
                        break;
                    targetPos = player.Center;
                    targetPos.X += 700 * (npc.Center.X < targetPos.X ? -1 : 1);
                    targetPos.Y += 200;
                    if (npc.Distance(targetPos) > 50)
                        Movement(targetPos, 0.7f);
                    /*if (++npc.ai[1] > 6)
                    {
                        npc.ai[1] = 0;
                        Main.PlaySound(SoundID.Item34, player.Center);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 spawnPos = npc.Center;
                            spawnPos.X += (npc.Center.X < player.Center.X) ? 900 : -900;
                            spawnPos.Y -= 1200;
                            for (int i = 0; i < 15; i++)
                                Projectile.NewProjectile(spawnPos.X + Main.rand.Next(-300, 301), spawnPos.Y + Main.rand.Next(-100, 101), Main.rand.NextFloat(-5f, 5f), Main.rand.NextFloat(30f, 35f), ModContent.ProjectileType<MutantSlimeBall>(), npc.damage / 5, 0f, Main.myPlayer);
                        }
                    }
                    if (npc.ai[3] == 0)
                    {
                        npc.ai[3] = 1;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<MutantSlimeRain>(), npc.damage / 4, 0f, Main.myPlayer, npc.whoAmI);
                    }*/
                    if (++npc.ai[2] > 60)//180)
                    {
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;
                        npc.netUpdate = true;
                        npc.TargetClosest();
                    }
                    break;

                case 36: //slime rain
                    npc.velocity = Vector2.Zero;
                    if (npc.ai[3] == 0)
                    {
                        npc.ai[3] = 1;
                        //Main.NewText(npc.position.Y);
                        Main.PlaySound(SoundID.Roar, (int)npc.Center.X, (int)npc.Center.Y, 0);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<MutantSlimeRain>(), npc.damage / 4, 0f, Main.myPlayer, npc.whoAmI);
                    }
                    if (npc.ai[1] == 0) //telegraphs for where slime will fall
                    {
                        bool first = npc.localAI[0] == 0;
                        npc.localAI[0] = Main.rand.Next(5, 9) * 120;
                        if (first) //always start on the same side as the player
                        {
                            if (player.Center.X < npc.Center.X && npc.localAI[0] > 1200)
                                npc.localAI[0] -= 1200;
                            else if (player.Center.X > npc.Center.X && npc.localAI[0] < 1200)
                                npc.localAI[0] += 1200;
                        }
                        else //after that, always be on opposite side from player
                        {
                            if (player.Center.X < npc.Center.X && npc.localAI[0] < 1200)
                                npc.localAI[0] += 1200;
                            else if (player.Center.X > npc.Center.X && npc.localAI[0] > 1200)
                                npc.localAI[0] -= 1200;
                        }
                        npc.localAI[0] += 60;
                        Vector2 basePos = npc.Center;
                        basePos.X -= 1200;
                        for (int i = -360; i <= 2760; i += 120) //spawn telegraphs
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                if (i + 60 == (int)npc.localAI[0])
                                    continue;
                                Projectile.NewProjectile(basePos.X + i + 60, basePos.Y, 0f, 0f, ModContent.ProjectileType<MutantReticle>(), 0, 0f, Main.myPlayer);
                            }
                        }
                    }
                    if (npc.ai[1] > 100 && npc.ai[1] % 5 == 0) //rain down slime balls
                    {
                        Main.PlaySound(SoundID.Item34, player.Center);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 basePos = npc.Center;
                            basePos.X -= 1200;
                            basePos.Y -= 1300;

                            const float safeRange = 110;
                            for (int i = -360; i <= 2760; i += 75)
                            {
                                float xOffset = i + Main.rand.Next(75);
                                if (Math.Abs(xOffset - npc.localAI[0]) < safeRange) //dont fall over safespot
                                    continue;
                                Vector2 spawnPos = basePos;
                                spawnPos.X += xOffset;
                                Vector2 velocity = Vector2.UnitY * Main.rand.NextFloat(15f, 20f);

                                Projectile.NewProjectile(spawnPos, velocity, ModContent.ProjectileType<MutantSlimeBall>(), npc.damage / 5, 0f, Main.myPlayer);
                            }
                            
                            //spawn right on safespot borders
                            Projectile.NewProjectile(basePos + Vector2.UnitX * (npc.localAI[0] + safeRange),
                                Vector2.UnitY * Main.rand.NextFloat(15f, 20f),
                                ModContent.ProjectileType<MutantSlimeBall>(), npc.damage / 5, 0f, Main.myPlayer);
                            Projectile.NewProjectile(basePos + Vector2.UnitX * (npc.localAI[0] - safeRange),
                                Vector2.UnitY * Main.rand.NextFloat(15f, 20f),
                                ModContent.ProjectileType<MutantSlimeBall>(), npc.damage / 5, 0f, Main.myPlayer);
                        }
                    }
                    if (++npc.ai[1] > 150)
                    {
                        npc.ai[1] = 0;
                    }
                    /*if (--npc.ai[1] < 0)
                    {
                        npc.ai[1] = 100;
                        if (npc.ai[2] < 330 && Main.netMode != NetmodeID.MultiplayerClient) //spawn irisu walls of slime balls
                        {
                            const int xRange = 1400;
                            Vector2 start = npc.Center;
                            start.X -= xRange;
                            start.Y -= 1400 * npc.localAI[0];

                            const int safeRange = 160;
                            int safespot = Main.rand.Next(1000, 1800) - safeRange / 2;

                            int end = 2 * xRange;
                            int type = ModContent.ProjectileType<MutantSlimeBall2>();
                            float speed = 5f * npc.localAI[0];
                            for (int i = 0; i < safespot; i+= 48)
                            {
                                Projectile.NewProjectile(start.X + i + Main.rand.NextFloat(-24, 0), start.Y + Main.rand.NextFloat(-24, 24),
                                        0f, speed, type, npc.damage / 4, 0f, Main.myPlayer, npc.whoAmI, Main.rand.NextFloat(-0.3f, 0.3f));
                                Projectile.NewProjectile(start.X + i + Main.rand.NextFloat(-24, 0), start.Y - 48 + Main.rand.NextFloat(-24, 24),
                                    0f, speed, type, npc.damage / 4, 0f, Main.myPlayer, npc.whoAmI, Main.rand.NextFloat(-0.3f, 0.3f));
                            }
                            Projectile.NewProjectile(start.X + safespot, start.Y - 24,
                                0f, speed, type, npc.damage / 4, 0f, Main.myPlayer, npc.whoAmI, Main.rand.NextFloat(-0.3f, 0.3f));
                            for (int i = safespot + safeRange; i < end; i += 48)
                            {
                                Projectile.NewProjectile(start.X + i + Main.rand.NextFloat(24), start.Y + Main.rand.NextFloat(-24, 24),
                                        0f, speed, type, npc.damage / 4, 0f, Main.myPlayer, npc.whoAmI, Main.rand.NextFloat(-0.3f, 0.3f));
                                Projectile.NewProjectile(start.X + i + Main.rand.NextFloat(24), start.Y - 48 + Main.rand.NextFloat(-24, 24),
                                    0f, speed, type, npc.damage / 4, 0f, Main.myPlayer, npc.whoAmI, Main.rand.NextFloat(-0.3f, 0.3f));
                            }
                            Projectile.NewProjectile(start.X + safespot + safeRange, start.Y - 24,
                                0f, speed, type, npc.damage / 4, 0f, Main.myPlayer, npc.whoAmI, Main.rand.NextFloat(-0.3f, 0.3f));
                        }
                    }*/
                    if (++npc.ai[2] > 450)
                    {
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;
                        npc.localAI[0] = 0;
                        npc.netUpdate = true;
                        npc.TargetClosest();
                    }
                    break;

                case 37: //go above to initiate dash
                    if (!AliveCheck(player))
                        break;
                    targetPos = player.Center;
                    targetPos.X += 400 * (npc.Center.X < targetPos.X ? -1 : 1);
                    targetPos.Y -= 400;
                    Movement(targetPos, 0.9f);
                    if (++npc.ai[1] > 60) //dive here
                    {
                        npc.velocity.X = 35f * (npc.position.X < player.position.X ? 1 : -1);
                        npc.velocity.Y = 10f;
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        npc.netUpdate = true;
                        npc.TargetClosest();
                    }
                    break;

                case 38: //spawn fishrons
                    goto case 30;

                case 39: //approach for okuu nonspell
                    if (!AliveCheck(player))
                        break;
                    targetPos = player.Center + player.DirectionTo(npc.Center) * 250;
                    if (npc.Distance(targetPos) > 50 && ++npc.ai[2] < 180)
                    {
                        Movement(targetPos, 0.8f);
                    }
                    else
                    {
                        npc.netUpdate = true;
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;
                    }
                    break;

                case 40: //SPHERE RING SPAMMMMM
                    npc.velocity = Vector2.Zero;
                    if (++npc.ai[1] > 10 && npc.ai[3] > 60)
                    {
                        npc.ai[1] = 0;
                        float rotation = MathHelper.ToRadians(60) * (npc.ai[3] - 45) / 240 * npc.ai[2];
                        SpawnSphereRing(9, 10f, npc.damage / 4, -1f, rotation);
                        SpawnSphereRing(9, 10f, npc.damage / 4, 1f, rotation);
                    }
                    if (npc.ai[2] == 0)
                    {
                        npc.ai[2] = Main.rand.Next(2) == 0 ? -1 : 1;
                        npc.ai[3] = Main.rand.NextFloat((float)Math.PI * 2);
                        Main.PlaySound(SoundID.Roar, (int)npc.Center.X, (int)npc.Center.Y, 0);
                    }
                    if (++npc.ai[3] > 360)
                    {
                        npc.netUpdate = true;
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;
                        npc.TargetClosest();
                    }
                    for (int i = 0; i < 5; i++)
                    {
                        int d = Dust.NewDust(npc.position, npc.width, npc.height, 229, 0f, 0f, 0, default(Color), 1.5f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].noLight = true;
                        Main.dust[d].velocity *= 4f;
                    }
                    break;

                case 41: //throw penetrator again
                    if (!AliveCheck(player))
                        break;
                    targetPos = player.Center;
                    targetPos.X += 500 * (npc.Center.X < targetPos.X ? -1 : 1);
                    if (npc.Distance(targetPos) > 50)
                        Movement(targetPos, 0.4f);
                    if (++npc.ai[1] > 240)
                    {
                        npc.netUpdate = true;
                        npc.ai[1] = 210;
                        if (++npc.ai[2] > 5)
                        {
                            /*float[] options = { 11, 13, 18, 20, 21, 24, 25, 26, 31, 35, 41 };
                            npc.ai[0] = options[Main.rand.Next(options.Length)];*/
                            npc.ai[0]++;
                            npc.ai[1] = 0;
                            npc.ai[2] = 0;
                            npc.TargetClosest();
                        }
                        else if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 vel = npc.DirectionTo(player.Center) * 30f;
                            Projectile.NewProjectile(npc.Center, Vector2.Normalize(vel), ModContent.ProjectileType<MutantDeathray2>(), npc.damage / 5, 0f, Main.myPlayer);
                            Projectile.NewProjectile(npc.Center, -Vector2.Normalize(vel), ModContent.ProjectileType<MutantDeathray2>(), npc.damage / 5, 0f, Main.myPlayer);
                            Projectile.NewProjectile(npc.Center, vel, ModContent.ProjectileType<MutantSpearThrown>(), npc.damage / 4, 0f, Main.myPlayer, npc.target);
                        }
                    }
                    else if (npc.ai[1] == 211)
                    {
                        if (npc.ai[2] > 0 && npc.ai[2] < 5 && Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<MutantSpearAim>(), npc.damage / 4, 0f, Main.myPlayer, npc.whoAmI, 1);
                    }
                    else if (npc.ai[1] == 180)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<MutantSpearAim>(), npc.damage / 4, 0f, Main.myPlayer, npc.whoAmI);
                    }
                    break;

                case 42: //spawn leaf crystals
                    Main.PlaySound(SoundID.Item84, npc.Center);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int p = Projectile.NewProjectile(npc.Center, Vector2.UnitY * 10f, ModContent.ProjectileType<MutantMark2>(), npc.damage / 4, 0f, Main.myPlayer, 30, 30 + 120);
                        if (p != Main.maxProjectiles)
                        {
                            const int max = 5;
                            const float distance = 125f;
                            float rotation = 2f * (float)Math.PI / max;
                            for (int i = 0; i < max; i++)
                            {
                                float myRot = rotation * i + (float)Math.PI / 2;
                                Vector2 spawnPos = npc.Center + new Vector2(distance, 0f).RotatedBy(myRot);
                                Projectile.NewProjectile(spawnPos, Vector2.Zero, ModContent.ProjectileType<MutantCrystalLeaf>(), npc.damage / 4, 0f, Main.myPlayer, Main.projectile[p].identity, myRot);
                            }
                        }
                        p = Projectile.NewProjectile(npc.Center, Vector2.UnitY * -10f, ModContent.ProjectileType<MutantMark2>(), npc.damage / 4, 0f, Main.myPlayer, 30, 30 + 240);
                        if (p != Main.maxProjectiles)
                        {
                            const int max = 5;
                            const float distance = 125f;
                            float rotation = 2f * (float)Math.PI / max;
                            for (int i = 0; i < max; i++)
                            {
                                float myRot = rotation * i - (float)Math.PI / 2;
                                Vector2 spawnPos = npc.Center + new Vector2(distance, 0f).RotatedBy(rotation * i);
                                Projectile.NewProjectile(spawnPos, Vector2.Zero, ModContent.ProjectileType<MutantCrystalLeaf>(), npc.damage / 4, 0f, Main.myPlayer, Main.projectile[p].identity, myRot);
                            }
                        }

                        for (int i = 0; i < 4; i++)
                        {
                            Projectile.NewProjectile(npc.Center + Vector2.UnitX.RotatedBy(Math.PI / 2 * i) * 525, Vector2.Zero, ModContent.ProjectileType<Projectiles.GlowRingHollow>(), 0, 0f, Main.myPlayer, 1f);
                            Projectile.NewProjectile(npc.Center + Vector2.UnitX.RotatedBy(Math.PI / 2 * i + Math.PI / 4) * 350, Vector2.Zero, ModContent.ProjectileType<Projectiles.GlowRingHollow>(), 0, 0f, Main.myPlayer, 2f);
                        }
                    }
                    npc.ai[0]++;
                    break;

                case 43: //boomerangs
                    npc.velocity = Vector2.Zero;
                    if (++npc.ai[1] > 20)
                    {
                        npc.netUpdate = true;
                        npc.ai[1] = 0;
                        npc.ai[2] = npc.ai[2] > 0 ? -1 : 1;

                        Main.PlaySound(SoundID.Item92, npc.Center);

                        if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[3] < 240)
                        {
                            const float retiRad = 525;
                            const float spazRad = 350;
                            float retiSpeed = 2 * (float)Math.PI * retiRad / 300;
                            float spazSpeed = 2 * (float)Math.PI * spazRad / 180;
                            float retiAcc = retiSpeed * retiSpeed / retiRad * npc.ai[2];
                            float spazAcc = spazSpeed * spazSpeed / spazRad * -npc.ai[2];
                            for (int i = 0; i < 4; i++)
                            {
                                Projectile.NewProjectile(npc.Center, Vector2.UnitX.RotatedBy(Math.PI / 2 * i) * retiSpeed, ModContent.ProjectileType<MutantRetirang>(), npc.damage / 4, 0f, Main.myPlayer, retiAcc, 300);
                                Projectile.NewProjectile(npc.Center, Vector2.UnitX.RotatedBy(Math.PI / 2 * i + Math.PI / 4) * spazSpeed, ModContent.ProjectileType<MutantSpazmarang>(), npc.damage / 4, 0f, Main.myPlayer, spazAcc, 180);
                            }
                        }
                    }
                    if (++npc.ai[3] > 300)
                    {
                        npc.netUpdate = true;
                        /*float[] options = { 11, 13, 18, 20, 21, 24, 26, 29, 33, 35, 39 };
                        npc.ai[0] = options[Main.rand.Next(options.Length)];*/
                        npc.ai[0]++;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;
                        npc.TargetClosest();
                    }
                    break;

                default:
                    npc.ai[0] = 11;
                    //npc.ai[2] = 180; //just start the laser attack without moving
                    goto case 11;
            }

            if (player.immune || player.hurtCooldowns[0] != 0 || player.hurtCooldowns[1] != 0)
                playerInvulTriggered = true;
            
            //drop summon
            if (FargoSoulsWorld.downedAbom && !FargoSoulsWorld.downedMutant && Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget && !droppedSummon)
            {
                Item.NewItem(player.Hitbox, ModContent.ItemType<MutantsCurse>());
                droppedSummon = true;
            }
        }

        private float GetSpinOffset()
        {
            const float PI = (float)Math.PI;
            float newRotation = (Main.player[npc.target].Center - npc.Center).ToRotation();
            float difference = newRotation - npc.ai[3];
            float rotationDirection = 2f * (float)Math.PI * 1f / 6f / 60f;
            while (difference < -PI)
                difference += 2f * PI;
            while (difference > PI)
                difference -= 2f * PI;
            if (difference > 0f)
                rotationDirection *= -1f;
            return rotationDirection;
        }

        private void SpawnSphereRing(int max, float speed, int damage, float rotationModifier, float offset = 0)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) return;
            float rotation = 2f * (float)Math.PI / max;
            int type = ModContent.ProjectileType<MutantSphereRing>();
            for (int i = 0; i < max; i++)
            {
                Vector2 vel = speed * Vector2.UnitY.RotatedBy(rotation * i + offset);
                Projectile.NewProjectile(npc.Center, vel, type, damage, 0f, Main.myPlayer, rotationModifier * npc.spriteDirection, speed);
            }
            Main.PlaySound(SoundID.Item84, npc.Center);
        }

        private void Aura(float distance, int buff, bool reverse = false, int dustid = DustID.GoldFlame, bool checkDuration = false, bool targetEveryone = true)
        {
            //works because buffs are client side anyway :ech:
            Player p = targetEveryone ? Main.player[Main.myPlayer] : Main.player[npc.target];
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
                        if (Main.netMode != NetmodeID.MultiplayerClient && !NPC.AnyNPCs(ModLoader.GetMod("Fargowiltas").NPCType("Mutant")))
                        {
                            KillProjs();
                            int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModLoader.GetMod("Fargowiltas").NPCType("Mutant"));
                            if (n != Main.maxNPCs && Main.netMode == NetmodeID.Server)
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
            if (Main.expertMode && npc.life < npc.lifeMax / 2)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.ai[0] = 10;
                    npc.ai[1] = 0;
                    npc.ai[2] = 0;
                    npc.ai[3] = 0;
                    npc.netUpdate = true;
                    KillProjs();
                    //EdgyBossText("Time to stop playing around.");
                }
                return true;
            }
            return false;
        }

        private void Movement(Vector2 targetPos, float speedModifier, bool fastX = true)
        {
            if (npc.Center.X < targetPos.X)
            {
                npc.velocity.X += speedModifier;
                if (npc.velocity.X < 0)
                    npc.velocity.X += speedModifier * (fastX ? 2 : 1);
            }
            else
            {
                npc.velocity.X -= speedModifier;
                if (npc.velocity.X > 0)
                    npc.velocity.X -= speedModifier * (fastX ? 2 : 1);
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
            if (Math.Abs(npc.velocity.X) > 24)
                npc.velocity.X = 24 * Math.Sign(npc.velocity.X);
            if (Math.Abs(npc.velocity.Y) > 24)
                npc.velocity.Y = 24 * Math.Sign(npc.velocity.Y);
        }

        /*private void EdgyBossText(string text)
        {
            if (Fargowiltas.Instance.CalamityLoaded) //edgy boss text
            {
                if (Main.netMode == NetmodeID.SinglePlayer)
                    Main.NewText(text, Color.LimeGreen);
                else if (Main.netMode == NetmodeID.Server)
                    NetMessage.BroadcastChatMessage(NetworkText.FromLiteral(text), Color.LimeGreen);
            }
        }*/

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.MasochistMode)
            {
                target.GetModPlayer<FargoPlayer>().MaxLifeReduction += 100;
                target.AddBuff(ModContent.BuffType<OceanicMaul>(), 5400);
                target.AddBuff(ModContent.BuffType<MutantFang>(), 180);
            }
            target.AddBuff(ModContent.BuffType<CurseoftheMoon>(), 600);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int i = 0; i < 3; i++)
            {
                int d = Dust.NewDust(npc.position, npc.width, npc.height, 229, 0f, 0f, 0, default(Color), 1f);
                Main.dust[d].noGravity = true;
                Main.dust[d].noLight = true;
                Main.dust[d].velocity *= 3f;
            }
        }

        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if (FargoSoulsWorld.AngryMutant || Fargowiltas.Instance.CalamityLoaded)
                damage *= 0.7f;
            return true;
        }

        public override bool CheckDead()
        {
            if (npc.ai[0] == -7)
                return true;

            npc.life = 1;
            npc.active = true;
            npc.localAI[3] = 2;
            if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[0] > -1)
            {
                npc.ai[0] = FargoSoulsWorld.MasochistMode ? -1 : -6;
                npc.ai[1] = 0;
                npc.ai[2] = 0;
                npc.ai[3] = 0;
                npc.localAI[0] = 0;
                npc.localAI[1] = 0;
                npc.dontTakeDamage = true;
                npc.netUpdate = true;
                KillProjs();
                //EdgyBossText("You're pretty good...");
            }
            return false;
        }

        public override void NPCLoot()
        {
            if (FargoSoulsWorld.MasochistMode)
            {
                if (!playerInvulTriggered)
                {
                    Item.NewItem(npc.Hitbox, mod.ItemType("PhantasmalEnergy"));
                    Item.NewItem(npc.Hitbox, mod.ItemType("SpawnSack"));
                }
                else if (Main.rand.Next(10) == 0)
                {
                    Item.NewItem(npc.Hitbox, mod.ItemType("PhantasmalEnergy"));
                }
            }

            FargoSoulsWorld.downedMutant = true;
            FargoSoulsWorld.skipMutantP1 = 0;
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData); //sync world
            
            if (FargoSoulsWorld.MasochistMode)
            {
                npc.DropItemInstanced(npc.position, npc.Size, ModContent.ItemType<Items.Accessories.Masomode.MutantEye>());
            }

            npc.DropItemInstanced(npc.position, npc.Size, ModContent.ItemType<Items.Misc.MutantBag>());

            if (Main.rand.Next(10) == 0)
                Item.NewItem(npc.Hitbox, ModContent.ItemType<Items.Tiles.MutantTrophy>());
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.SuperHealingPotion;
        }

        public override void FindFrame(int frameHeight)
        {
            if (++npc.frameCounter > 4)
            {
                npc.frameCounter = 0;
                npc.frame.Y += frameHeight;
                if (npc.frame.Y >= Main.npcFrameCount[npc.type] * frameHeight)
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