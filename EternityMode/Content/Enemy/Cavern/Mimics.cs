using System.IO;
using Terraria.ModLoader.IO;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.Projectiles.Masomode;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using System;

namespace FargowiltasSouls.EternityMode.Content.Enemy.Cavern
{
    public class Mimics : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.Mimic,
            NPCID.PresentMimic,
            NPCID.IceMimic,
            NPCID.BigMimicCorruption,
            NPCID.BigMimicCrimson,
            NPCID.BigMimicHallow,
            NPCID.BigMimicJungle
        );

        public int InvulFrameTimer;
        public int AttackTimer = 0;
        public int Attack = 0;
        public int FlightCD = 0;
        public bool Flying = false;

        public Vector2 LockVector = Vector2.Zero;

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            binaryWriter.Write7BitEncodedInt(AttackTimer);
            binaryWriter.Write7BitEncodedInt(Attack);
            binaryWriter.Write7BitEncodedInt(FlightCD);
            binaryWriter.Write(LockVector.X);
            binaryWriter.Write(LockVector.Y);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            AttackTimer = binaryReader.Read7BitEncodedInt();
            Attack = binaryReader.Read7BitEncodedInt();
            FlightCD = binaryReader.Read7BitEncodedInt();
            LockVector.X = binaryReader.ReadSingle();
            LockVector.Y = binaryReader.ReadSingle();
        }

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);
            if (!Main.hardMode)
                npc.damage = (int)Math.Round(npc.damage * 0.5);
        }

        public override bool SafePreAI(NPC npc)
        {
            Player player = Main.player[npc.target];
            bool returnbool = base.SafePreAI(npc); //used so entire PreAI always runs
            const int AttackCD = 180; //time between attacks where mimic does the vanilla ai

            if (npc.type == NPCID.Mimic || npc.type == NPCID.PresentMimic || npc.type == NPCID.IceMimic) //delete ice mimic and give it its own attacks later
            {
                //Main.NewText(npc.ai[0].ToString() + " | " + npc.ai[1].ToString() + " | " + npc.ai[2].ToString() + " | " +  npc.ai[3].ToString());
                //Main.NewText(npc.localAI[0].ToString() + " | " + npc.localAI[1].ToString() + " | " + npc.localAI[2].ToString() + " | " + npc.localAI[3].ToString());
                void TeleportPunchAttack()
                {
                    int Timer = AttackTimer - AttackCD;
                    if (Timer == 1)
                    {
                        LockVector = player.Center + (player.velocity * 3);
                        npc.velocity.X = 0;
                        SoundEngine.PlaySound(SoundID.Item6, npc.Center);
                    }
                    if (Timer > 1 && Timer < 60)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            Dust.NewDust(LockVector - new Vector2(npc.width / 2, npc.height / 2), npc.width, npc.height, DustID.MagicMirror);
                        }
                    }
                    if (Timer == 60)
                    {
                        npc.position = LockVector - new Vector2(npc.width / 2, npc.height / 2);
                        npc.netUpdate = true;
                        NetSync(npc);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<MimicTitanGlove>(), 0, 1f, Main.myPlayer, ai1: npc.whoAmI);
                        }
                    }
                    if (Timer == 120)
                    {
                        npc.velocity = npc.DirectionTo(player.Center) * 20;
                        SoundEngine.PlaySound(SoundID.DD2_SonicBoomBladeSlash, npc.Center);
                        npc.noGravity = true;
                        //npc.noTileCollide = true;
                    }
                    npc.velocity *= 0.99f;
                    if (Timer == 140)
                    {
                        npc.noGravity = false;
                        //npc.noTileCollide = false;
                    }
                    
                    
                    if (Timer >= 160)
                    {
                        AttackTimer = 0;
                    }
                }
                void DaggerAttack()
                {
                    int Timer = AttackTimer - AttackCD;
                    npc.velocity.X = 0;
                    if (Timer % 20 == 0 && Timer <= 20 && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        float DaggerSpeed = 4;
                        float rot = MathHelper.ToRadians(Main.rand.NextFloat(-6, 6));
                        Vector2 DaggerPosition = npc.Center + new Vector2(Main.rand.Next(-10, 10), Main.rand.Next(-10, 10));
                        Vector2 DaggerVelocity = (Vector2.Normalize(player.Center - DaggerPosition) * DaggerSpeed).RotatedBy(rot);
                        Projectile.NewProjectile(npc.GetSource_FromThis(), DaggerPosition, DaggerVelocity, ModContent.ProjectileType<MimicDagger>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 1f, Main.myPlayer);
                    }
                    if (Timer >= 100)
                    {
                        AttackTimer = 0;
                    }
                }
                void StarveilAttack()
                {
                    int Timer = AttackTimer - AttackCD;
                    const int StarAmount = 3;
                    npc.velocity.X = 0;

                    if (Timer == 5 && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = 0; i < StarAmount; i++)
                        {
                            float StarSpeed = 12;
                            float randrot = MathHelper.ToRadians(1);
                            Vector2 StarPosition = npc.Center + new Vector2(-300 + Main.rand.Next(800), -1000);
                            Vector2 StarVelocity = (Vector2.Normalize(player.Center - StarPosition) * StarSpeed).RotatedBy(Main.rand.NextFloat(-randrot, randrot));
                            int p = Projectile.NewProjectile(npc.GetSource_FromThis(), StarPosition, StarVelocity, ModContent.ProjectileType<MimicHallowStar>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 1f, Main.myPlayer);
                            Main.projectile[p].friendly = false;
                            Main.projectile[p].hostile = true;
                            Main.projectile[p].tileCollide = false;
                        }
                    }

                    if (Timer >= 60)
                    {
                        AttackTimer = 0;
                    }
                }
                void Flight()
                {

                    //if (Math.Abs(npc.velocity.ToRotation() - npc.DirectionTo(player.Center).ToRotation()) > Math.PI) //if velociting in the wrong direction, change direction toward player
                        //npc.velocity = npc.DirectionTo(player.Center);

                    //npc.velocity += npc.DirectionTo(player.Center) * 0.5f;
                    FlyToward(player.Center);
                    npc.noTileCollide = true;
                    npc.noGravity = true;
                    if (npc.localAI[3] % 10 == 0)
                    {
                        SoundEngine.PlaySound(SoundID.Run, npc.Center);
                    }
                    Dust.NewDust(npc.Center + new Vector2(0, npc.height / 2), 0, 0, DustID.Cloud, Scale: 1.5f);
                    npc.localAI[3]++;
                    if (npc.Distance(player.Center) < 200 && Collision.CanHitLine(npc.position, npc.width, npc.height, player.position, player.width, player.height))
                    {
                        FlightCD = 60 * 15;
                        Flying = false;
                        npc.noGravity = false;
                        npc.noTileCollide = false;
                        npc.localAI[3] = 0;
                        npc.velocity = Vector2.Zero;
                    }
                }
                void FlyToward(Vector2 v)
                {
                    float inertia = 5f;
                    float deadzone = 25f;
                    Vector2 vectorToIdlePosition = v - npc.Center;
                    float num = vectorToIdlePosition.Length();
                    if (num > deadzone)
                    {
                        vectorToIdlePosition.Normalize();
                        vectorToIdlePosition *= 6;
                        npc.velocity = (npc.velocity * (inertia - 1f) + vectorToIdlePosition) / inertia;
                    }
                    else if (npc.velocity == Vector2.Zero)
                    {
                        npc.velocity.X = -0.15f;
                        npc.velocity.Y = -0.05f;
                    }
                }
                if (npc.life < npc.lifeMax && npc.ai[0] == 1 && player.active && !player.dead) //if mimic awake and target active
                {
                    if (AttackTimer < AttackCD) //only do flight when not attacking
                    {
                        if (FlightCD == 0 && (npc.Distance(player.Center) > 1000 || (npc.Distance(player.Center) > 200 && !Collision.CanHitLine(npc.position, npc.width, npc.height, player.position, player.width, player.height)))) //if far or collision, fly toward target
                        {
                            Flying = true;
                        }
                        if (Flying)
                        {
                            Flight();
                            if (AttackTimer < AttackCD - 5)
                            {
                                AttackTimer++; //progress attack timer up to attack, even while flying, so you dont cheese by just letting him charge forever
                            }
                            return false; //no attacking or normal ai while flying
                        }
                    }
                    if (FlightCD > 0)
                    {
                        FlightCD--;
                    }
                    if (AttackTimer == AttackCD - 5) //get random a bit before, for net sync
                    {
                        Attack = Main.rand.Next(3);
                        //Main.NewText(Attack.ToString());
                        npc.netUpdate = true;
                        NetSync(npc);
                    }
                    if (AttackTimer >= AttackCD)
                    {
                        switch (Attack)
                        {
                            case 0:
                                {
                                    TeleportPunchAttack();
                                    break;
                                }
                            case 1:
                                {
                                    DaggerAttack();
                                    break;
                                }
                            case 2:
                                {
                                    StarveilAttack();
                                    break;
                                }
                            default:
                                {
                                    break;
                                }
                        }
                        returnbool = false; //don't run AI when attacking
                    }

                    AttackTimer++;
                }
            }

            return returnbool;
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (npc.type == NPCID.Mimic || npc.type == NPCID.PresentMimic)
            {
                npc.dontTakeDamage = false;
                if (npc.justHit && Main.hardMode)
                    InvulFrameTimer = 20;
                if (InvulFrameTimer > 0)
                {
                    InvulFrameTimer--;
                    npc.dontTakeDamage = true;
                }
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(ModContent.BuffType<Midas>(), 600);
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.GoldenCrate));
        }

        public override void OnKill(NPC npc)
        {
            base.OnKill(npc);

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int max = 5;
                for (int i = 0; i < max; i++)
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.position.X + Main.rand.Next(npc.width), npc.position.Y + Main.rand.Next(npc.height),
                        Main.rand.Next(-30, 31) * .1f, Main.rand.Next(-40, -15) * .1f, ModContent.ProjectileType<FakeHeart>(), 20, 0f, Main.myPlayer);
            }
        }
    }
}
