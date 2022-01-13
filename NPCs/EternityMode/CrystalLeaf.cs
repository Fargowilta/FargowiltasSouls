using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.EternityMode;
using FargowiltasSouls.EternityMode.Content.Boss.HM;

namespace FargowiltasSouls.NPCs.EternityMode
{
    public class CrystalLeaf : ModNPC
    {
        public override string Texture => "Terraria/Projectile_226";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Crystal Leaf");
            //DisplayName.AddTranslation(GameCulture.Chinese, "叶绿水晶");
            NPCID.Sets.TrailCacheLength[npc.type] = 6;
            NPCID.Sets.TrailingMode[npc.type] = 1;
        }

        public override void SetDefaults()
        {
            npc.width = 28;
            npc.height = 28;
            npc.damage = 60;
            npc.defense = 9999;
            npc.lifeMax = 9999;
            npc.HitSound = SoundID.NPCHit1;
            //npc.DeathSound = SoundID.Grass;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.dontCountMe = true;
            npc.knockBackResist = 0f;
            npc.alpha = 255;
            npc.lavaImmune = true;
            for (int i = 0; i < npc.buffImmune.Length; i++)
                npc.buffImmune[i] = true;
            npc.aiStyle = -1;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = 9999;
            npc.life = 9999;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(npc.localAI[2]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc.localAI[2] = reader.ReadSingle();
        }

        public override void AI()
        {
            if (npc.buffType[0] != 0)
                npc.DelBuff(0);
            
            NPC plantera = FargoSoulsUtil.NPCExists(npc.ai[0], NPCID.Plantera);
            if (plantera == null || FargoSoulsWorld.SwarmActive)
            {
                npc.life = 0;
                npc.HitEffect();
                npc.active = false;
                npc.netUpdate = true;
                return;
            }

            if (npc.localAI[1] == 0)
            {
                npc.localAI[1] = 1;
                for (int index1 = 0; index1 < 30; ++index1)
                {
                    int index2 = Dust.NewDust(npc.position, npc.width, npc.height, Main.rand.NextBool() ? 107 : 157, 0f, 0f, 0, new Color(), 2f);
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].velocity *= 5f;
                }
            }

            npc.target = plantera.target;

            /*if (npc.HasPlayerTarget && Main.player[npc.target].active)
            {
                if (++npc.localAI[2] > 300) //projectile timer
                {
                    npc.localAI[2] = 0;
                    npc.netUpdate = true;
                    if (npc.ai[1] == 130 && plantera.life > plantera.lifeMax / 2)
                    {
                        Main.PlaySound(SoundID.Grass, (int)npc.position.X, (int)npc.position.Y);
                        if (Main.netMode != -1)
                        {
                            for (int i = -2; i <= 2; i++)
                            {
                                Vector2 target = plantera.Center + (Main.player[npc.target].Center - plantera.Center).RotatedBy(MathHelper.ToRadians(80 / 2) * i);
                                Projectile.NewProjectile(npc.Center, 18f * npc.DirectionTo(target), mod.ProjectileType("CrystalLeafShot"), npc.damage / 4, 0f, Main.myPlayer);
                            }
                        }
                        for (int index1 = 0; index1 < 30; ++index1)
                        {
                            int index2 = Dust.NewDust(npc.position, npc.width, npc.height, 157, 0f, 0f, 0, new Color(), 2f);
                            Main.dust[index2].noGravity = true;
                            Main.dust[index2].velocity *= 5f;
                        }
                    }
                }
            }*/

            Lighting.AddLight(npc.Center, 0.1f, 0.4f, 0.2f);
            npc.scale = (Main.mouseTextColor / 200f - 0.35f) * 0.2f + 0.95f;
            npc.life = npc.lifeMax;

            npc.position = plantera.Center + new Vector2(npc.ai[1], 0f).RotatedBy(npc.ai[3]);
            npc.position.X -= npc.width / 2;
            npc.position.Y -= npc.height / 2;

            if (plantera.GetEModeNPCMod<Plantera>().RingTossTimer > 120 && plantera.GetEModeNPCMod<Plantera>().RingTossTimer < 120 + 45 && npc.ai[1] == 130) //pause before shooting
            {
                npc.localAI[3] = 1;
                npc.scale *= 1.5f;
            }
            else
            {
                npc.localAI[3] = 0;

                float modifier = 1f; //npc.localAI[0]++ / 90f;

                if (npc.localAI[0] > 90)
                    npc.localAI[0] = 90;

                float rotation = npc.ai[1] == 130f ? 0.03f : -0.015f;
                npc.ai[3] += rotation * modifier;
                if (npc.ai[3] > (float)Math.PI)
                {
                    npc.ai[3] -= 2f * (float)Math.PI;
                    npc.netUpdate = true;
                }
                npc.rotation = npc.ai[3] + (float)Math.PI / 2f;

                if (npc.ai[1] > 130)
                {
                    /*if (plantera.GetEModeNPCMod<Plantera>().TentacleTimer < 0)
                    {
                        npc.localAI[0] -= 2;
                        if (npc.localAI[0] < 0)
                            npc.localAI[0] = 0;
                    }*/

                    npc.ai[2] += 2 * (float)Math.PI / 360 * modifier;
                    if (npc.ai[2] > (float)Math.PI)
                        npc.ai[2] -= 2 * (float)Math.PI;
                    npc.ai[1] += (float)Math.Sin(npc.ai[2]) * 7 * modifier;
                    npc.scale *= 1.5f;
                }
            }

            npc.alpha -= npc.ai[1] > 130 ? 2 : 3;
            if (npc.alpha < 0)
                npc.alpha = 0;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return npc.alpha == 0;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Poisoned, 300);
            target.AddBuff(mod.BuffType("Infested"), 180);
            target.AddBuff(mod.BuffType("IvyVenom"), 240);
        }

        public override void ModifyHitByItem(Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            damage = 0;
            npc.life++;
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (FargoSoulsUtil.CanDeleteProjectile(projectile))
                projectile.penetrate = 0;
            damage = 0;
            npc.life++;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                for (int index1 = 0; index1 < 30; ++index1)
                {
                    int index2 = Dust.NewDust(npc.position, npc.width, npc.height, Main.rand.NextBool() ? 107 : 157, 0f, 0f, 0, new Color(), 2f);
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].velocity *= 5f;
                }
            }
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool PreNPCLoot()
        {
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
            return new Color(num5, num5, num5, 200) * npc.Opacity;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.npcTexture[npc.type];
            Rectangle rectangle = npc.frame;
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = npc.GetAlpha(color26);

            SpriteEffects effects = npc.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.spriteBatch.Draw(texture2D13, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, npc.rotation, origin2, npc.scale, effects, 0f);

            color26 *= 0.75f;

            if (npc.alpha == 0)
            {
                for (int i = 0; i < NPCID.Sets.TrailCacheLength[npc.type]; i++)
                {
                    Color color27 = color26;
                    color27.A = (byte)(npc.localAI[3] == 0 ? 150 : 0);
                    color27 *= (float)(NPCID.Sets.TrailCacheLength[npc.type] - i) / NPCID.Sets.TrailCacheLength[npc.type];
                    Vector2 value4 = npc.oldPos[i];
                    float num165 = npc.rotation; //npc.oldRot[i];
                    Main.spriteBatch.Draw(texture2D13, value4 + npc.Size / 2f - Main.screenPosition + new Vector2(0, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, npc.scale, effects, 0f);
                }
            }

            Main.spriteBatch.Draw(texture2D13, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, npc.rotation, origin2, npc.scale, effects, 0f);
            return false;
        }
    }
}