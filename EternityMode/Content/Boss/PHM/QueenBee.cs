using Fargowiltas.Items.Summons;
using FargowiltasSouls.EternityMode.Net;
using FargowiltasSouls.EternityMode.Net.Strategies;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.Items.Accessories.Masomode;
using FargowiltasSouls.Items.Tiles;
using FargowiltasSouls.NPCs;
using FargowiltasSouls.NPCs.EternityMode;
using FargowiltasSouls.Projectiles;
using FargowiltasSouls.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.EternityMode.Content.Boss.PHM
{
    public class QueenBee : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.QueenBee);

        public int HiveThrowTimer;
        public int StingerRingTimer;
        public int BeeSwarmTimer = 600;

        public bool SpawnedRoyalSubjectWave1;
        public bool SpawnedRoyalSubjectWave2;
        public bool InPhase2;

        public bool DroppedSummon;

        public override Dictionary<Ref<object>, CompoundStrategy> GetNetInfo() =>
            new Dictionary<Ref<object>, CompoundStrategy> {
                { new Ref<object>(HiveThrowTimer), IntStrategies.CompoundStrategy },
                { new Ref<object>(StingerRingTimer), IntStrategies.CompoundStrategy },
                { new Ref<object>(BeeSwarmTimer), IntStrategies.CompoundStrategy },

                { new Ref<object>(SpawnedRoyalSubjectWave1), BoolStrategies.CompoundStrategy },
                { new Ref<object>(SpawnedRoyalSubjectWave2), BoolStrategies.CompoundStrategy },
                { new Ref<object>(InPhase2), BoolStrategies.CompoundStrategy },
            };

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.buffImmune[BuffID.Poisoned] = true;
        }

        public override bool PreAI(NPC npc)
        {
            base.AI(npc);

            EModeGlobalNPC.beeBoss = npc.whoAmI;

            if (FargoSoulsWorld.SwarmActive)
                return true;

            if (!SpawnedRoyalSubjectWave1 && npc.life < npc.lifeMax / 3 * 2 && npc.HasPlayerTarget)
            {
                SpawnedRoyalSubjectWave1 = true;

                Vector2 vector72 = new Vector2(npc.position.X + npc.width / 2 + Main.rand.Next(20) * npc.direction, npc.position.Y + npc.height * 0.8f);

                int n = NPC.NewNPC((int)vector72.X, (int)vector72.Y, ModContent.NPCType<RoyalSubject>(), 0, 0f, 0f, 0f, 0f, 255);
                if (n != Main.maxNPCs)
                {
                    Main.npc[n].velocity.X = Main.rand.Next(-200, 201) * 0.002f;
                    Main.npc[n].velocity.Y = Main.rand.Next(-200, 201) * 0.002f;
                    Main.npc[n].localAI[0] = 60f;
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                }

                FargoSoulsUtil.PrintText(Language.GetTextValue("Mods.FargowiltasSouls.EternityMode.RoyalSubject"), new Color(175, 75, 255));

                npc.netUpdate = true;
                NetSync(npc);
            }

            if (!SpawnedRoyalSubjectWave2 && npc.life < npc.lifeMax / 3 && npc.HasPlayerTarget)
            {
                SpawnedRoyalSubjectWave2 = true;

                if (FargoSoulsWorld.MasochistModeReal)
                    SpawnedRoyalSubjectWave1 = false; //do this again

                Vector2 vector72 = new Vector2(npc.position.X + npc.width / 2 + Main.rand.Next(20) * npc.direction, npc.position.Y + npc.height * 0.8f);

                int n = NPC.NewNPC((int)vector72.X, (int)vector72.Y, ModContent.NPCType<RoyalSubject>(), 0, 0f, 0f, 0f, 0f, 255);
                if (n != Main.maxNPCs)
                {
                    Main.npc[n].velocity.X = Main.rand.Next(-200, 201) * 0.1f;
                    Main.npc[n].velocity.Y = Main.rand.Next(-200, 201) * 0.1f;
                    Main.npc[n].localAI[0] = 60f;
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                }

                FargoSoulsUtil.PrintText(Language.GetTextValue("Mods.FargowiltasSouls.EternityMode.RoyalSubject"), new Color(175, 75, 255));

                NPC.SpawnOnPlayer(npc.target, ModContent.NPCType<RoyalSubject>()); //so that both dont stack for being spawned from qb

                npc.netUpdate = true;
                NetSync(npc);
            }

            if (!InPhase2 && npc.life < npc.lifeMax / 2) //enable new attack and roar below 50%
            {
                InPhase2 = true;
                Main.PlaySound(SoundID.Roar, npc.Center, 0);

                if (FargoSoulsWorld.MasochistModeReal)
                    SpawnedRoyalSubjectWave1 = false; //do this again

                npc.netUpdate = true;
                NetSync(npc);
            }

            if (NPC.AnyNPCs(ModContent.NPCType<RoyalSubject>()))
            {
                npc.HitSound = SoundID.NPCHit4;
                npc.color = new Color(127, 127, 127);

                int dustId = Dust.NewDust(npc.position, npc.width, npc.height, 1, 0f, 0f, 100, default(Color), 2f);
                Main.dust[dustId].noGravity = true;
                int dustId3 = Dust.NewDust(npc.position, npc.width, npc.height, 1, 0f, 0f, 100, default(Color), 2f);
                Main.dust[dustId3].noGravity = true;

                npc.ai[0] = 3; //always shoot stingers mode
            }
            else
            {
                npc.HitSound = SoundID.NPCHit1;
                npc.color = default;

                if (InPhase2 && HiveThrowTimer % 2 == 0)
                    HiveThrowTimer++; //throw hives faster when no royal subjects alive
            }

            if (FargoSoulsWorld.MasochistModeReal)
                HiveThrowTimer++;

            if (!InPhase2)
            {
                if (npc.ai[0] == 3f || npc.ai[0] == 1f) //only when in stationary modes
                {
                    if (++StingerRingTimer > 90 * 3)
                        StingerRingTimer = 0;

                    if (StingerRingTimer % 90 == 0)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            FargoSoulsUtil.XWay(StingerRingTimer == 90 * 3 ? 16 : 8, npc.Center, ProjectileID.Stinger, 6, 11, 1);
                    }
                }
            }
            else
            {
                if (++HiveThrowTimer > 570 && BeeSwarmTimer <= 600 && (npc.ai[0] == 3f || npc.ai[0] == 1f)) //lobs hives below 50%, not dashing
                {
                    HiveThrowTimer = 0;

                    npc.netUpdate = true;
                    NetSync(npc);

                    const float gravity = 0.25f;
                    float time = 75f;
                    Vector2 distance = Main.player[npc.target].Center - Vector2.UnitY * 16 - npc.Center + Main.player[npc.target].velocity * 30f;
                    distance.X = distance.X / time;
                    distance.Y = distance.Y / time - 0.5f * gravity * time;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(npc.Center, distance, ModContent.ProjectileType<Beehive>(),
                            npc.damage / 4, 0f, Main.myPlayer, time - 5);
                    }
                }

                if (npc.ai[0] == 0 && npc.ai[1] == 1f) //if qb tries to start doing dashes of her own volition
                {
                    npc.ai[0] = 3f;
                    npc.ai[1] = 0f; //don't
                    npc.netUpdate = true;
                }
            }

            //only while stationary mode
            if (npc.ai[0] == 3f || npc.ai[0] == 1f)
            {
                if (InPhase2 && ++BeeSwarmTimer > 600)
                {
                    if (BeeSwarmTimer < 720) //slow down
                    {
                        if (BeeSwarmTimer == 601)
                        {
                            npc.netUpdate = true;
                            NetSync(npc);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, npc.whoAmI, npc.type);

                            if (npc.HasValidTarget)
                                Main.PlaySound(SoundID.ForceRoar, Main.player[npc.target].Center, -1); //eoc roar

                            if (FargoSoulsWorld.MasochistModeReal)
                                BeeSwarmTimer += 30;
                        }

                        if (Collision.CanHitLine(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0))
                        {
                            npc.velocity *= 0.975f;
                        }
                        else if (BeeSwarmTimer > 630)
                        {
                            BeeSwarmTimer--; //stall this section until has line of sight
                            return true;
                        }
                    }
                    else if (BeeSwarmTimer < 840) //spray bees
                    {
                        npc.velocity = Vector2.Zero;

                        if (BeeSwarmTimer % 2 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            const float rotation = 0.025f;
                            for (int i = -1; i <= 1; i += 2)
                            {
                                Projectile.NewProjectile(npc.Center + new Vector2(3 * npc.direction, 15), i * Main.rand.NextFloat(9f, 18f) * Vector2.UnitX.RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-45, 45))),
                                    ModContent.ProjectileType<Bee>(), npc.damage / (FargoSoulsWorld.MasochistModeReal ? 3 : 4), 0f, Main.myPlayer, npc.target, Main.rand.NextBool() ? -rotation : rotation);
                            }
                        }
                    }
                    else if (BeeSwarmTimer > 870) //return to normal AI
                    {
                        BeeSwarmTimer = 0;
                        HiveThrowTimer -= 60;

                        npc.netUpdate = true;
                        NetSync(npc);

                        npc.ai[0] = 0f;
                        npc.ai[1] = 4f; //trigger dashes, but skip the first one
                        npc.ai[2] = -44f;
                        npc.ai[3] = 0f;
                    }

                    if (npc.netUpdate)
                    {
                        npc.netUpdate = false;

                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                    }
                    return false;
                }
            }

            if (npc.ai[0] == 0 && npc.ai[1] == 4 && npc.ai[2] < 0) //when about to do dashes triggered by royal subjects/bee swarm, telegraph and stall
            {
                if (npc.ai[2] == -44) //telegraph
                {
                    Main.PlaySound(SoundID.Item21, npc.Center);

                    for (int i = 0; i < 44; i++)
                    {
                        int d = Dust.NewDust(npc.position, npc.width, npc.height, Main.rand.NextBool() ? 152 : 153, npc.velocity.X * 0.2f, npc.velocity.Y * 0.2f);
                        Main.dust[d].scale = Main.rand.NextFloat(1f, 3f);
                        Main.dust[d].velocity *= Main.rand.NextFloat(4.4f);
                        Main.dust[d].noGravity = Main.rand.NextBool();
                        if (Main.dust[d].noGravity)
                        {
                            Main.dust[d].scale *= 2.2f;
                            Main.dust[d].velocity *= 4.4f;
                        }
                    }

                    if (FargoSoulsWorld.MasochistModeReal)
                        npc.ai[2] = 0;
                }

                npc.velocity *= 0.95f;
                npc.ai[2]++;

                return false;
            }

            EModeUtils.DropSummon(npc, ModContent.ItemType<Abeemination2>(), NPC.downedQueenBee, ref DroppedSummon);

            return true;
        }

        public override void NPCLoot(NPC npc)
        {
            base.NPCLoot(npc);

            npc.DropItemInstanced(npc.position, npc.Size, ItemID.JungleFishingCrate, 5);
            npc.DropItemInstanced(npc.position, npc.Size, ItemID.HerbBag, 5);
            npc.DropItemInstanced(npc.position, npc.Size, ModContent.ItemType<QueenStinger>());

            if ((int)(Main.time / 60 - 30) % 60 == 22) //COOMEDY
                Item.NewItem(npc.Hitbox, ModContent.ItemType<TwentyTwoPainting>());
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(ModContent.BuffType<Infested>(), 300);
            target.AddBuff(ModContent.BuffType<Swarming>(), 600);
        }

        public override bool StrikeNPC(NPC npc, ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if (!FargoSoulsWorld.SwarmActive && NPC.AnyNPCs(ModContent.NPCType<RoyalSubject>()))
                damage /= 2;

            return base.StrikeNPC(npc, ref damage, defense, ref knockback, hitDirection, ref crit);
        }

        public override void LoadSprites(NPC npc, bool recolor)
        {
            base.LoadSprites(npc, recolor);

            LoadNPCSprite(recolor, npc.type);
            LoadBossHeadSprite(recolor, 14);
            LoadGoreRange(recolor, 303, 308);
        }
    }
}
