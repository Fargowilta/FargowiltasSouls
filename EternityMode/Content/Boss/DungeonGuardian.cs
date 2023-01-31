using System.IO;
using Terraria.ModLoader.IO;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.Items.Accessories.Masomode;
using FargowiltasSouls.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Boss
{
    public class DungeonGuardian : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.DungeonGuardian);

        public int AITimer;
        public int AttackTimer;

        public bool TeleportCheck;

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            binaryWriter.Write7BitEncodedInt(AITimer);
            binaryWriter.Write7BitEncodedInt(AttackTimer);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            AITimer = binaryReader.Read7BitEncodedInt();
            AttackTimer = binaryReader.Read7BitEncodedInt();
        }

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.boss = true;
            npc.lifeMax /= 4;
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            NPCs.EModeGlobalNPC.guardBoss = npc.whoAmI;
            npc.damage = npc.defDamage;
            npc.defense = npc.defDefense;

            while (npc.buffType[0] != 0)
            {
                npc.buffImmune[npc.buffType[0]] = true;
                npc.DelBuff(0);
            }

            /*if (npc.velocity.Length() < 5f) //old spam bones and skulls code
            {
                npc.velocity.Normalize();
                npc.velocity *= 5f;
            }
            if (--Counter < 0)
            {
                Counter = 60;
                if (npc.HasPlayerTarget && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 speed = Main.player[npc.target].Center - npc.Center;
                    speed.X += Main.rand.Next(-20, 21);
                    speed.Y += Main.rand.Next(-20, 21);
                    speed.Normalize();
                    speed *= 3f;
                    speed += npc.velocity * 2f;
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, speed, ProjectileID.Skull, FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0, Main.myPlayer, -1f, 0);
                }
            }
            if (++Counter2 > 6)
            {
                Counter2 = 0;
                Vector2 speed = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                speed.Normalize();
                speed *= 6f;
                speed += npc.velocity * 1.25f;
                speed.Y -= Math.Abs(speed.X) * 0.2f;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, speed, ModContent.ProjectileType<SkeletronBone>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer);
            }*/

            if (Main.LocalPlayer.active && !Main.LocalPlayer.dead && !Main.LocalPlayer.ghost && npc.Hitbox.Intersects(Main.LocalPlayer.Hitbox))
            {
                Main.LocalPlayer.immune = false;
                Main.LocalPlayer.immuneTime = 0;
                Main.LocalPlayer.hurtCooldowns[0] = 0;
                Main.LocalPlayer.hurtCooldowns[1] = 0;
            }

            if (npc.HasValidTarget && npc.ai[1] == 2f) //while actually attacking
            {
                npc.position -= npc.velocity; //offset regular velocity

                float speed = 6f; //base speed
                float compareSpeed = Math.Max(Math.Abs(Main.player[npc.target].velocity.X), Math.Abs(Main.player[npc.target].velocity.Y));
                compareSpeed *= 1.02f; //always outrun slightly (player can move diagonally)
                if (speed < compareSpeed)
                    speed = compareSpeed;

                npc.position += Vector2.Normalize(npc.velocity) * speed;
            }

            if (!TeleportCheck) //teleport closer
            {
                TeleportCheck = true;

                npc.TargetClosest(false);

                if (npc.HasValidTarget && npc.Distance(Main.player[npc.target].Center) > 800 && npc.Distance(Main.player[npc.target].Center) < 3000)
                {
                    for (int i = 0; i < 50; i++)
                    {
                        int d = Dust.NewDust(npc.position, npc.width, npc.height, 112, 0f, 0f, 0, Color.White, 2.5f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].velocity *= 12f;
                    }

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        npc.Center = Main.player[npc.target].Center + 800 * Vector2.UnitX.RotatedByRandom(2 * Math.PI);

                    for (int i = 0; i < 50; i++)
                    {
                        int d = Dust.NewDust(npc.position, npc.width, npc.height, 112, 0f, 0f, 0, Color.White, 2.5f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].velocity *= 12f;
                    }
                }

                npc.netUpdate = true;
                NetSync(npc);
            }

            if (++AITimer < 90)
            {
                if (AITimer == 1 && npc.HasPlayerTarget)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(npc.GetSource_FromThis(), Main.player[npc.target].Center, Vector2.UnitY,
                                ModContent.ProjectileType<GuardianDeathraySmall>(), 0, 0f, Main.myPlayer, npc.target, -1f);
                        Projectile.NewProjectile(npc.GetSource_FromThis(), Main.player[npc.target].Center, -Vector2.UnitY,
                                ModContent.ProjectileType<GuardianDeathraySmall>(), 0, 0f, Main.myPlayer, npc.target, -1f);
                    }

                    npc.netUpdate = true;
                    NetSync(npc);
                }

                if (++AttackTimer > 1) //spray bone rain above player
                {
                    SoundEngine.PlaySound(SoundID.Item1, npc.Center);

                    AttackTimer = 0;

                    Vector2 spawnPos = Main.player[npc.target].Center;
                    spawnPos.X += Main.rand.NextFloat(-100, 100);
                    spawnPos.Y -= Main.rand.NextFloat(700, 800);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(npc.GetSource_FromThis(), spawnPos, Vector2.UnitY * Main.rand.NextFloat(10f, 20f),
                            ModContent.ProjectileType<SkeletronBone>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage, 4f / 20), 0f, Main.myPlayer);
                    }
                }
            }
            else if (AITimer < 220)
            {
                if (AITimer == 91)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, npc.DirectionTo(Main.player[npc.target].Center).RotatedBy(Math.PI / 3 * i),
                                ModContent.ProjectileType<GuardianDeathraySmall>(), 0, 0f, Main.myPlayer, -1f, npc.whoAmI);
                        }
                    }

                    AttackTimer = 30;

                    npc.netUpdate = true;
                    NetSync(npc);
                }

                if (++AttackTimer > 60) //homing skulls
                {
                    AttackTimer = 0;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 speed = Main.player[npc.target].Center - npc.Center;
                        speed.X += Main.rand.Next(-20, 21);
                        speed.Y += Main.rand.Next(-20, 21);
                        speed.Normalize();
                        speed *= 3f;
                        for (int i = 0; i < 6; i++)
                        {
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, speed.RotatedBy(Math.PI / 3 * i),
                                ProjectileID.Skull, FargoSoulsUtil.ScaledProjectileDamage(npc.damage, 4f / 20), 0, Main.myPlayer, -1f, 0);
                        }
                    }
                }
            }
            else if (AITimer < 280)
            {
                //nothing
            }
            else if (AITimer < 410)
            {
                if (AITimer == 281)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(npc.GetSource_FromThis(), Main.player[npc.target].Center, Vector2.UnitX.RotatedBy(Math.PI / 2 * i),
                                ModContent.ProjectileType<GuardianDeathraySmall>(), 0, 0f, Main.myPlayer, npc.target, -1f);
                            Projectile.NewProjectile(npc.GetSource_FromThis(), Main.player[npc.target].Center + 160 * Vector2.UnitY.RotatedBy(Math.PI / 2 * i), Vector2.UnitX.RotatedBy(Math.PI / 2 * i),
                                ModContent.ProjectileType<GuardianDeathraySmall>(), 0, 0f, Main.myPlayer, npc.target, -1f);
                            Projectile.NewProjectile(npc.GetSource_FromThis(), Main.player[npc.target].Center + -160 * Vector2.UnitY.RotatedBy(Math.PI / 2 * i), Vector2.UnitX.RotatedBy(Math.PI / 2 * i),
                                ModContent.ProjectileType<GuardianDeathraySmall>(), 0, 0f, Main.myPlayer, npc.target, -1f);
                        }
                    }

                    AttackTimer = 0;

                    npc.netUpdate = true;
                    NetSync(npc);
                }

                if (++AttackTimer == 30) //wall of babies from all sides
                {
                    //AttackTimer = 0;

                    AITimer += 60; //my brain is so fried and this old timer system is so wack i cant be bothered to recalculate all the offsets for this

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            for (int j = -2; j <= 2; j++)
                            {
                                Vector2 spawnPos = new Vector2(1200, 80 * j);
                                Vector2 vel = -18 * Vector2.UnitX;
                                spawnPos = Main.player[npc.target].Center + spawnPos.RotatedBy(Math.PI / 2 * i);
                                vel = vel.RotatedBy(Math.PI / 2 * i);
                                Projectile.NewProjectile(npc.GetSource_FromThis(), spawnPos, vel, ModContent.ProjectileType<Projectiles.Champions.ShadowGuardian>(),
                                    FargoSoulsUtil.ScaledProjectileDamage(npc.damage, 4f / 20), 0f, Main.myPlayer);
                            }
                        }
                    }
                }
            }
            else if (AITimer < 540)
            {
                if (AITimer == 481)
                {
                    for (int i = 0; i < 16; i++)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(npc.GetSource_FromThis(), Main.player[npc.target].Center, Vector2.UnitX.RotatedBy(Math.PI / 8 * i),
                                ModContent.ProjectileType<GuardianDeathraySmall>(), 0, 0f, Main.myPlayer, npc.target, -1f);
                        }
                    }

                    AttackTimer = 0;

                    npc.netUpdate = true;
                    NetSync(npc);
                }

                if (++AttackTimer == 30) // ring of guardians
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        const int max = 16;
                        Vector2 baseOffset = npc.DirectionTo(Main.player[npc.target].Center);
                        for (int i = 0; i < max; i++)
                        {
                            Projectile.NewProjectile(npc.GetSource_FromThis(), Main.player[npc.target].Center + 1000 * baseOffset.RotatedBy(2 * Math.PI / max * i),
                                -10f * baseOffset.RotatedBy(2 * Math.PI / max * i), ModContent.ProjectileType<Projectiles.DeviBoss.DeviGuardian>(),
                                FargoSoulsUtil.ScaledProjectileDamage(npc.damage, 4f / 20), 0f, Main.myPlayer);
                        }
                    }
                }
            }
            else if (AITimer < 700) //mindless bone spray
            {
                if (AITimer == 541)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.UnitY,
                            ModContent.ProjectileType<GuardianDeathraySmall>(), 0, 0f, Main.myPlayer, -1f, npc.whoAmI);
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center + -200 * Vector2.UnitX, Vector2.UnitY,
                            ModContent.ProjectileType<GuardianDeathraySmall>(), 0, 0f, Main.myPlayer, -1f, npc.whoAmI);
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center + 200 * Vector2.UnitX, Vector2.UnitY,
                            ModContent.ProjectileType<GuardianDeathraySmall>(), 0, 0f, Main.myPlayer, -1f, npc.whoAmI);

                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, -Vector2.UnitY,
                            ModContent.ProjectileType<GuardianDeathraySmall>(), 0, 0f, Main.myPlayer, -1f, npc.whoAmI);
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center + -200 * Vector2.UnitX, -Vector2.UnitY,
                            ModContent.ProjectileType<GuardianDeathraySmall>(), 0, 0f, Main.myPlayer, -1f, npc.whoAmI);
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center + 200 * Vector2.UnitX, -Vector2.UnitY,
                            ModContent.ProjectileType<GuardianDeathraySmall>(), 0, 0f, Main.myPlayer, -1f, npc.whoAmI);
                    }

                    npc.netUpdate = true;
                    NetSync(npc);
                }

                if (++AttackTimer > 2)
                {
                    SoundEngine.PlaySound(SoundID.Item1, npc.Center);

                    AttackTimer = 0;
                    Vector2 speed = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                    speed.Normalize();
                    speed *= 6f;
                    speed += npc.velocity * 1.25f;
                    speed.Y -= Math.Abs(speed.X) * 0.2f;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, speed, ModContent.ProjectileType<SkeletronBone>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage, 4f / 20), 0f, Main.myPlayer);
                }
            }
            else if (AITimer < 820) //fuck everywhere except where you're standing
            {
                if (AITimer == 701)
                {
                    for (int i = 0; i < 4; i++) //from the cardinals
                    {
                        Vector2 spawnPos = Main.player[npc.target].Center + 1000 * Vector2.UnitX.RotatedBy(Math.PI / 2 * i);
                        for (int j = -1; j <= 1; j++) //to both sides
                        {
                            if (j == 0)
                                continue;

                            Vector2 baseVel = Main.player[npc.target].DirectionFrom(spawnPos).RotatedBy(MathHelper.ToRadians(15) * j);
                            for (int k = 0; k < 7; k++) //a fan of skulls
                            {
                                if (k % 2 == 1) //only draw every other ray
                                    continue;

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(npc.GetSource_FromThis(), spawnPos, baseVel.RotatedBy(MathHelper.ToRadians(10) * j * k),
                                        ModContent.ProjectileType<GuardianDeathraySmall>(), 0, 0f, Main.myPlayer, npc.target, -1f);
                                }
                            }
                        }
                    }

                    AttackTimer = 0;

                    npc.netUpdate = true;
                    NetSync(npc);
                }

                if (++AttackTimer == 30)
                {
                    for (int i = 0; i < 4; i++) //from the cardinals
                    {
                        Vector2 spawnPos = Main.player[npc.target].Center + 1000 * Vector2.UnitX.RotatedBy(Math.PI / 2 * i);
                        for (int j = -1; j <= 1; j++) //to both sides
                        {
                            if (j == 0)
                                continue;

                            Vector2 baseVel = 22f * Main.player[npc.target].DirectionFrom(spawnPos).RotatedBy(MathHelper.ToRadians(15) * j);
                            for (int k = 0; k < 7; k++) //a fan of skulls
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(npc.GetSource_FromThis(), spawnPos, baseVel.RotatedBy(MathHelper.ToRadians(10) * j * k),
                                        ModContent.ProjectileType<Projectiles.Champions.ShadowGuardian>(),
                                        FargoSoulsUtil.ScaledProjectileDamage(npc.damage, 4f / 20), 0f, Main.myPlayer);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                AITimer = 0;
                AttackTimer = 0;
                TeleportCheck = false;

                npc.netUpdate = true;
                NetSync(npc);
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(ModContent.BuffType<GodEater>(), 420);
            target.AddBuff(ModContent.BuffType<FlamesoftheUniverse>(), 420);
            target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 420);

            //target.immune = false; //handled by special checks in ai
            //target.immuneTime = 0;
        }

        public override bool StrikeNPC(NPC npc, ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            damage = 1;

            return base.StrikeNPC(npc, ref damage, defense, ref knockback, hitDirection, ref crit);
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.BossBag(ModContent.ItemType<SinisterIcon>()));
        }
    }
}
