using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode;
using FargowiltasSouls.EternityMode.Content.Boss.HM;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.NPCs.EternityMode
{
    public class CrystalLeaf : ModNPC
    {
        public override string Texture => "Terraria/Images/Projectile_226";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystal Leaf");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "叶绿水晶");
            NPCID.Sets.TrailCacheLength[NPC.type] = 6;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            NPCID.Sets.DebuffImmunitySets.Add(NPC.type, new Terraria.DataStructures.NPCDebuffImmunityData
            {
                ImmuneToAllBuffsThatAreNotWhips = true,
                ImmuneToWhips = true
            });

            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Hide = true
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 28;
            NPC.height = 28;
            NPC.damage = 60;
            NPC.defense = 9999;
            NPC.lifeMax = 9999;
            NPC.HitSound = SoundID.NPCHit1;
            //NPC.DeathSound = SoundID.Grass;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.dontCountMe = true;
            NPC.knockBackResist = 0f;
            NPC.alpha = 255;
            NPC.lavaImmune = true;
            NPC.aiStyle = -1;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = 9999;
            NPC.life = 9999;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[2]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[2] = reader.ReadSingle();
        }

        public override void AI()
        {
            if (NPC.buffType[0] != 0)
                NPC.DelBuff(0);

            NPC plantera = FargoSoulsUtil.NPCExists(NPC.ai[0], NPCID.Plantera);
            if (plantera == null || FargoSoulsWorld.SwarmActive)
            {
                NPC.life = 0;
                NPC.HitEffect();
                NPC.active = false;
                NPC.netUpdate = true;
                return;
            }

            if (NPC.localAI[1] == 0)
            {
                NPC.localAI[1] = 1;
                for (int index1 = 0; index1 < 30; ++index1)
                {
                    int index2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, Main.rand.NextBool() ? 107 : 157, 0f, 0f, 0, new Color(), 2f);
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].velocity *= 5f;
                }
            }

            NPC.target = plantera.target;

            /*if (NPC.HasPlayerTarget && Main.player[NPC.target].active)
            {
                if (++NPC.localAI[2] > 300) //projectile timer
                {
                    NPC.localAI[2] = 0;
                    NPC.netUpdate = true;
                    if (NPC.ai[1] == 130 && plantera.life > plantera.lifeMax / 2)
                    {
                        SoundEngine.PlaySound(SoundID.Grass, NPC.position);
                        if (Main.netMode != -1)
                        {
                            for (int i = -2; i <= 2; i++)
                            {
                                Vector2 target = plantera.Center + (Main.player[NPC.target].Center - plantera.Center).RotatedBy(MathHelper.ToRadians(80 / 2) * i);
                                Projectile.NewProjectile(NPC.Center, 18f * NPC.DirectionTo(target), ModContent.ProjectileType<CrystalLeafShot>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer);
                            }
                        }
                        for (int index1 = 0; index1 < 30; ++index1)
                        {
                            int index2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, 157, 0f, 0f, 0, new Color(), 2f);
                            Main.dust[index2].noGravity = true;
                            Main.dust[index2].velocity *= 5f;
                        }
                    }
                }
            }*/

            Lighting.AddLight(NPC.Center, 0.1f, 0.4f, 0.2f);
            NPC.scale = (Main.mouseTextColor / 200f - 0.35f) * 0.2f + 0.95f;
            NPC.life = NPC.lifeMax;

            NPC.position = plantera.Center + new Vector2(NPC.ai[1], 0f).RotatedBy(NPC.ai[3]);
            NPC.position.X -= NPC.width / 2;
            NPC.position.Y -= NPC.height / 2;

            if (plantera.GetEModeNPCMod<Plantera>().RingTossTimer > 120 && plantera.GetEModeNPCMod<Plantera>().RingTossTimer < 120 + 45 && NPC.ai[1] == 130) //pause before shooting
            {
                NPC.localAI[3] = 1;
                NPC.scale *= 1.5f;
            }
            else
            {
                NPC.localAI[3] = 0;

                float modifier = 1f; //NPC.localAI[0]++ / 90f;

                if (NPC.localAI[0] > 90)
                    NPC.localAI[0] = 90;

                float rotation = NPC.ai[1] == 130f ? 0.03f : -0.015f;
                NPC.ai[3] += rotation * modifier;
                if (NPC.ai[3] > (float)Math.PI)
                {
                    NPC.ai[3] -= 2f * (float)Math.PI;
                    NPC.netUpdate = true;
                }
                NPC.rotation = NPC.ai[3] + (float)Math.PI / 2f;

                if (NPC.ai[1] > 130)
                {
                    /*if (plantera.GetEModeNPCMod<Plantera>().TentacleTimer < 0)
                    {
                        NPC.localAI[0] -= 2;
                        if (NPC.localAI[0] < 0)
                            NPC.localAI[0] = 0;
                    }*/

                    NPC.ai[2] += 2 * (float)Math.PI / 360 * modifier;
                    if (NPC.ai[2] > (float)Math.PI)
                        NPC.ai[2] -= 2 * (float)Math.PI;
                    NPC.ai[1] += (float)Math.Sin(NPC.ai[2]) * 7 * modifier;
                    NPC.scale *= 1.5f;
                }
            }

            NPC.alpha -= NPC.ai[1] > 130 ? 2 : 3;
            if (NPC.alpha < 0)
                NPC.alpha = 0;

            NPC.dontTakeDamage = NPC.alpha > 0;
        }

        public override bool CanHitPlayer(Player target, ref int CooldownSlot)
        {
            return NPC.alpha == 0;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<IvyVenom>(), 240);
        }

        public override void ModifyHitByItem(Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            damage = 0;
            NPC.life++;
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (FargoSoulsUtil.CanDeleteProjectile(projectile))
                projectile.penetrate = 0;
            damage = 0;
            NPC.life++;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int index1 = 0; index1 < 30; ++index1)
                {
                    int index2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, Main.rand.NextBool() ? 107 : 157, 0f, 0f, 0, new Color(), 2f);
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].velocity *= 5f;
                }
            }
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool CheckDead()
        {
            NPC.life = 0;
            NPC.HitEffect();
            NPC.active = false;
            return false;
        }

        public override bool? DrawHealthBar(byte hbPos, ref float scale, ref Vector2 Pos)
        {
            return false;
        }

        public override Color? GetAlpha(Color drawColor)
        {
            float num4 = Main.mouseTextColor / 200f - 0.3f;
            int num5 = (int)(byte.MaxValue * num4) + 50;
            if (num5 > byte.MaxValue)
                num5 = byte.MaxValue;
            return new Color(num5, num5, num5, 200) * NPC.Opacity;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Npc[NPC.type].Value;
            Rectangle rectangle = NPC.frame;
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = drawColor;
            color26 = NPC.GetAlpha(color26);

            SpriteEffects effects = NPC.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture2D13, NPC.Center - Main.screenPosition + new Vector2(0f, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, NPC.rotation, origin2, NPC.scale, effects, 0);

            color26 *= 0.75f;

            if (NPC.alpha == 0)
            {
                for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                {
                    Color color27 = color26;
                    color27.A = (byte)(NPC.localAI[3] == 0 ? 150 : 0);
                    color27 *= (float)(NPCID.Sets.TrailCacheLength[NPC.type] - i) / NPCID.Sets.TrailCacheLength[NPC.type];
                    Vector2 value4 = NPC.oldPos[i];
                    float num165 = NPC.rotation; //NPC.oldRot[i];
                    Main.EntitySpriteDraw(texture2D13, value4 + NPC.Size / 2f - Main.screenPosition + new Vector2(0, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, NPC.scale, effects, 0);
                }
            }

            Main.EntitySpriteDraw(texture2D13, NPC.Center - Main.screenPosition + new Vector2(0f, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, NPC.rotation, origin2, NPC.scale, effects, 0);
            return false;
        }
    }
}