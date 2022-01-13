using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.NPCs.AbomBoss
{
    public class AbomSaucer : ModNPC
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Mini Saucer");
            NPCID.Sets.TrailCacheLength[npc.type] = 5;
            NPCID.Sets.TrailingMode[npc.type] = 1;
        }

        public override void SetDefaults()
        {
            npc.width = 25;
            npc.height = 25;
            npc.defense = 90;
            npc.lifeMax = 600;
            npc.scale = 2f;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.knockBackResist = 0f;
            npc.lavaImmune = true;
            npc.aiStyle = -1;
            npc.buffImmune[BuffID.Chilled] = true;
            npc.buffImmune[BuffID.OnFire] = true;
            npc.buffImmune[BuffID.Suffocation] = true;
            npc.buffImmune[mod.BuffType("Lethargic")] = true;
            npc.buffImmune[mod.BuffType("ClippedWings")] = true;
            npc.buffImmune[mod.BuffType("MutantNibble")] = true;
            npc.buffImmune[mod.BuffType("OceanicMaul")] = true;

            npc.dontTakeDamage = true;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.damage = (int)(npc.damage * 0.5f);
            npc.lifeMax = (int)(npc.lifeMax /** 0.5f*/ * bossLifeScale);
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

        public override void AI()
        {
            NPC abom = FargoSoulsUtil.NPCExists(npc.ai[0], ModContent.NPCType<AbomBoss>());
            if (abom == null || abom.dontTakeDamage)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.life = 0;
                    npc.HitEffect();
                    npc.checkDead();
                    npc.active = false;
                }
                return;
            }
            npc.target = abom.target;

            npc.dontTakeDamage = abom.ai[0] == 0 && abom.ai[2] < 3;

            if (++npc.ai[1] > 90) //pause before attacking
            {
                npc.velocity = Vector2.Zero;

                if (npc.ai[3] == 0) //store angle for attack
                {
                    npc.localAI[2] = npc.Distance(Main.player[npc.target].Center);
                    npc.ai[3] = npc.DirectionTo(Main.player[npc.target].Center).ToRotation();

                    if (npc.whoAmI == NPC.FindFirstNPC(npc.type) && Main.netMode != NetmodeID.MultiplayerClient) //reticle telegraph
                    {
                        Projectile.NewProjectile(Main.player[npc.target].Center, Vector2.Zero, mod.ProjectileType("AbomReticle"), 0, 0f, Main.myPlayer);
                    }
                }

                if (npc.ai[1] > 120) //attack and reset
                {
                    Main.PlaySound(SoundID.Item12, npc.Center);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            Vector2 speed = 16f * npc.ai[3].ToRotationVector2().RotatedBy((Main.rand.NextDouble() - 0.5) * 0.785398185253143 / 12.0);
                            speed *= Main.rand.NextFloat(0.9f, 1.1f);
                            int p = Projectile.NewProjectile(npc.Center, speed, mod.ProjectileType("AbomLaser"), abom.damage / 4, 0f, Main.myPlayer);
                            if (p != Main.maxProjectiles)
                                Main.projectile[p].timeLeft = (int)(npc.localAI[2] / speed.Length()) + 1;
                        }
                    }
                    npc.netUpdate = true;
                    npc.ai[1] = 0;
                    npc.ai[3] = 0;
                }
            }
            else
            {
                Vector2 target = Main.player[npc.target].Center; //targeting
                target += Vector2.UnitX.RotatedBy(npc.ai[2]) * (npc.ai[1] < 45 ? 200 : 500);

                Vector2 distance = target - npc.Center;
                distance /= 8f;
                npc.velocity = (npc.velocity * 19f + distance) / 20f;
            }

            npc.ai[2] -= 0.045f; //spin around target
            if (npc.ai[2] < (float)-Math.PI)
                npc.ai[2] += 2 * (float)Math.PI;

            if (npc.localAI[1] == 0) //visuals
                npc.localAI[1] = Main.rand.NextBool() ? 1 : -1;
            npc.rotation = (float)Math.Sin(2 * Math.PI * npc.localAI[0]++ / 90) * (float)Math.PI / 8f * npc.localAI[1];
            if (npc.localAI[0] > 180)
                npc.localAI[0] = 0;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int i = 0; i < 3; i++)
            {
                int d = Dust.NewDust(npc.position, npc.width, npc.height, 87, 0f, 0f, 0, default(Color), 1f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 3f;
            }
            if (npc.life <= 0)
            {
                for (int i = 0; i < 30; i++)
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, 87, 0f, 0f, 0, default(Color), 2.5f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 12f;
                }
            }
        }

        public override Color? GetAlpha(Color drawColor)
        {
            return Color.White;
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.npcTexture[npc.type];
            //int num156 = Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type]; //ypos of lower right corner of sprite to draw
            //int y3 = num156 * npc.frame.Y; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = npc.frame;//new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = npc.GetAlpha(color26);

            SpriteEffects effects = npc.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < NPCID.Sets.TrailCacheLength[npc.type]; i++)
            {
                Color color27 = color26 * 0.5f;
                color27 *= (float)(NPCID.Sets.TrailCacheLength[npc.type] - i) / NPCID.Sets.TrailCacheLength[npc.type];
                Vector2 value4 = npc.oldPos[i];
                float num165 = npc.rotation; //npc.oldRot[i];
                Main.spriteBatch.Draw(texture2D13, value4 + npc.Size / 2f - Main.screenPosition + new Vector2(0, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, npc.scale, effects, 0f);
            }

            Main.spriteBatch.Draw(texture2D13, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), npc.GetAlpha(lightColor), npc.rotation, origin2, npc.scale, effects, 0f);
            return false;
        }
    }
}