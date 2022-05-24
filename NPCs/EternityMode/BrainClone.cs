using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.NPCs.EternityMode
{
    public class BrainClone : ModNPC
    {
        public override string Texture => "Terraria/Images/NPC_266";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brain of Cthulhu");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "克苏鲁之脑");
            Main.npcFrameCount[NPC.type] = Main.npcFrameCount[NPCID.BrainofCthulhu];
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Hide = true
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 160;
            NPC.height = 110;
            NPC.scale += 0.25f;
            NPC.damage = 30;
            NPC.defense = 14;
            NPC.lifeMax = 1000;
            NPC.HitSound = SoundID.NPCHit9;
            NPC.DeathSound = SoundID.NPCDeath11;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0f;
            NPC.lavaImmune = true;
            NPC.aiStyle = -1;
        }

        public override bool CanHitPlayer(Player target, ref int CooldownSlot)
        {
            return NPC.alpha == 0;
        }

        public override void AI()
        {
            if (EModeGlobalNPC.brainBoss < 0f || EModeGlobalNPC.brainBoss >= Main.maxNPCs)
            {
                NPC.StrikeNPCNoInteraction(9999, 0f, 0);
                NPC.active = false;
                return;
            }
            NPC brain = Main.npc[EModeGlobalNPC.brainBoss];
            if (!brain.active || brain.type != NPCID.BrainofCthulhu)
            {
                NPC.StrikeNPCNoInteraction(9999, 0f, 0);
                NPC.active = false;
                return;
            }

            if (NPC.buffType[0] != 0) //constant debuff cleanse
            {
                NPC.buffImmune[NPC.buffType[0]] = true;
                NPC.DelBuff(0);
            }

            NPC.target = brain.target;
            NPC.damage = brain.damage;
            NPC.defDamage = brain.defDamage;
            NPC.defense = brain.defense;
            NPC.defDefense = brain.defDefense;
            NPC.life = brain.life;
            NPC.lifeMax = brain.lifeMax;
            NPC.knockBackResist = brain.knockBackResist;

            if (NPC.Distance(Main.player[NPC.target].Center) > 250) //immune to knockback unless this close
                NPC.knockBackResist = 0;

            if (NPC.alpha > 0 && (NPC.ai[0] == 2 || NPC.ai[0] == -3) && NPC.HasValidTarget) //stay at a minimum distance
            {
                const float safeRange = 360;
                /*Vector2 stayAwayFromHere = Main.player[NPC.target].Center + Main.player[NPC.target].velocity * 30f;
                if (NPC.Distance(stayAwayFromHere) < safeRange)
                    NPC.Center = stayAwayFromHere + NPC.DirectionFrom(stayAwayFromHere) * safeRange;*/
                Vector2 stayAwayFromHere = Main.player[NPC.target].Center;
                if (NPC.Distance(stayAwayFromHere) < safeRange)
                    NPC.Center = stayAwayFromHere + NPC.DirectionFrom(stayAwayFromHere) * safeRange;
            }

            Vector2 vector2 = NPC.Center;
            float num1 = Main.player[NPC.target].Center.X - vector2.X;
            float num2 = Main.player[NPC.target].Center.Y - vector2.Y;
            float num3 = (NPC.Distance(Main.player[NPC.target].Center) > 500 ? 8f : 4f) / (float)Math.Sqrt(num1 * num1 + num2 * num2);
            float num4 = num1 * num3;
            float num5 = num2 * num3;
            NPC.velocity.X = (NPC.velocity.X * 50 + num4) / 51f;
            NPC.velocity.Y = (NPC.velocity.Y * 50 + num5) / 51f;

            if (FargoSoulsWorld.MasochistModeReal)
            {
                if (NPC.ai[0] == -2)
                {
                    NPC.velocity *= 0.9f;
                    if (Main.netMode != 0)
                        NPC.ai[3] += 15f;
                    else
                        NPC.ai[3] += 25f;
                    if (NPC.ai[3] >= 255)
                    {
                        NPC.ai[3] = 255;
                        NPC.position.X = NPC.ai[1] * 16f - (float)(NPC.width / 2);
                        NPC.position.Y = NPC.ai[2] * 16f - (float)(NPC.height / 2);
                        SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
                        NPC.ai[0] = -3f;
                        NPC.netUpdate = true;
                        NPC.netSpam = 0;
                    }
                    NPC.alpha = (int)NPC.ai[3];
                }
                else if (NPC.ai[0] == -3)
                {
                    if (Main.netMode != 0)
                        NPC.ai[3] -= 15f;
                    else
                        NPC.ai[3] -= 25f;
                    if (NPC.ai[3] <= 0)
                    {
                        NPC.ai[3] = 0.0f;
                        NPC.ai[0] = -1f;
                        NPC.netUpdate = true;
                        NPC.netSpam = 0;
                    }
                    NPC.alpha = (int)NPC.ai[3];
                }
                else
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NPC.localAI[1]++;
                        if (NPC.justHit)
                            NPC.localAI[1] -= Main.rand.Next(5);
                        int num6 = 60 + Main.rand.Next(120);
                        if (Main.netMode != 0)
                            num6 += Main.rand.Next(30, 90);
                        if (NPC.localAI[1] >= num6)
                        {
                            NPC.localAI[1] = 0f;
                            NPC.TargetClosest(true);
                            int num7 = 0;
                            do
                            {
                                ++num7;
                                int num8 = (int)Main.player[NPC.target].Center.X / 16;
                                int num9 = (int)Main.player[NPC.target].Center.Y / 16;
                                int i = Main.rand.Next(2) != 0 ? num8 - Main.rand.Next(7, 13) : num8 + Main.rand.Next(7, 13);
                                int j = Main.rand.Next(2) != 0 ? num9 - Main.rand.Next(7, 13) : num9 + Main.rand.Next(7, 13);
                                if (!WorldGen.SolidTile(i, j))
                                {
                                    NPC.ai[3] = 0.0f;
                                    NPC.ai[0] = -2f;
                                    NPC.ai[1] = (float)i;
                                    NPC.ai[2] = (float)j;
                                    NPC.netUpdate = true;
                                    NPC.netSpam = 0;
                                    break;
                                }
                            }
                            while (num7 <= 100);
                        }
                    }
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.brainBoss, NPCID.BrainofCthulhu))
            {
                NPC.frame.Y = Main.npc[EModeGlobalNPC.brainBoss].frame.Y;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Poisoned, 120);
            target.AddBuff(BuffID.Darkness, 120);
            target.AddBuff(BuffID.Bleeding, 120);
            target.AddBuff(BuffID.Slow, 120);
            target.AddBuff(BuffID.Weak, 120);
            target.AddBuff(BuffID.BrokenArmor, 120);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                //SoundEngine.PlaySound(NPC.DeathSound, NPC.Center);
                for (int i = 0; i < 40; i++)
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 5);
                    Main.dust[d].velocity *= 2.5f;
                    Main.dust[d].scale += 0.5f;
                }
            }
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool CheckDead()
        {
            NPC.GetGlobalNPC<FargoSoulsGlobalNPC>().Needled = false;

            NPC brain = FargoSoulsUtil.NPCExists(EModeGlobalNPC.brainBoss, NPCID.BrainofCthulhu);
            if (brain != null)
            {
                NPC.active = true;
                NPC.life = brain.life;
            }
            return false;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!Terraria.GameContent.TextureAssets.Npc[NPCID.BrainofCthulhu].IsLoaded)
                return false;

            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Npc[NPCID.BrainofCthulhu].Value;
            Rectangle rectangle = NPC.frame;
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = drawColor;
            color26 = NPC.GetAlpha(color26);

            SpriteEffects effects = SpriteEffects.None;

            Main.EntitySpriteDraw(texture2D13, NPC.Center - Main.screenPosition + new Vector2(0f, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, NPC.rotation, origin2, NPC.scale, effects, 0);

            if (NPC.HasPlayerTarget && FargoSoulsWorld.MasochistModeReal)
            {
                Vector2 offset = NPC.Center - Main.player[NPC.target].Center;
                Vector2 spawnPos = Main.player[NPC.target].Center;

                float modifier = 1f - (float)NPC.life / NPC.lifeMax;
                if (modifier >= 0 && modifier <= 1)
                {
                    Main.EntitySpriteDraw(texture2D13, new Vector2(spawnPos.X + offset.X, spawnPos.Y - offset.Y) - Main.screenPosition + new Vector2(0f, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26 * modifier, NPC.rotation, origin2, NPC.scale, effects, 0);
                    Main.EntitySpriteDraw(texture2D13, new Vector2(spawnPos.X - offset.X, spawnPos.Y + offset.Y) - Main.screenPosition + new Vector2(0f, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26 * modifier, NPC.rotation, origin2, NPC.scale, effects, 0);
                    Main.EntitySpriteDraw(texture2D13, new Vector2(spawnPos.X - offset.X, spawnPos.Y - offset.Y) - Main.screenPosition + new Vector2(0f, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26 * modifier, NPC.rotation, origin2, NPC.scale, effects, 0);
                }
            }

            return false;
        }
    }
}