using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.MutantBoss
{
    public class MutantHeal : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Projectiles/Champions/SpiritSpirit";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Mutant Heal");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.aiStyle = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 1800;
            //projectile.hostile = true;
            projectile.scale = 0.8f;

            projectile.GetGlobalProjectile<FargoGlobalProjectile>().TimeFreezeImmune = true;
        }

        public override bool CanDamage()
        {
            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
            writer.Write(projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
            projectile.localAI[1] = reader.ReadSingle();
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0)
            {
                if (projectile.localAI[1] == 0)
                {
                    projectile.localAI[1] = Main.rand.NextFloat(MathHelper.ToRadians(1f)) * (Main.rand.NextBool() ? 1 : -1);
                    projectile.netUpdate = true;
                }

                projectile.velocity = Vector2.Normalize(projectile.velocity).RotatedBy(projectile.localAI[1]) * (projectile.velocity.Length() - projectile.ai[1]);

                if (projectile.velocity.Length() < 0.01f)
                {
                    projectile.localAI[0] = 1;
                    projectile.netUpdate = true;
                }
            }
            else if (projectile.localAI[0] == 1)
            {
                for (int i = 0; i < 2; i++) //make up for real spectre bolt having 3 extraUpdates
                {
                    projectile.position += projectile.velocity;

                    Vector2 change = Vector2.Normalize(projectile.velocity) * 5f;
                    projectile.velocity = (projectile.velocity * 29f + change).RotatedBy(projectile.localAI[1] * 3) / 30f;
                }

                if (projectile.velocity.Length() > 4.5f)
                {
                    projectile.localAI[0] = 2;
                    projectile.netUpdate = true;

                    projectile.timeLeft = 180 * 2; //compensating for extraUpdates
                }
            }
            else
            {
                projectile.extraUpdates = 1;

                int ai0 = (int)Math.Abs(projectile.ai[0]);
                bool feedPlayer = projectile.ai[0] < 0;
                if (feedPlayer)
                {
                    ai0 -= 1;

                    if (FargoSoulsWorld.MasochistModeReal)
                    {
                        projectile.Kill();
                        return;
                    }
                }

                if (ai0 < 0 || (feedPlayer ? ai0 >= Main.maxPlayers || !Main.player[ai0].active || Main.player[ai0].ghost || Main.player[ai0].dead : ai0 >= Main.maxNPCs || !Main.npc[ai0].active))
                {
                    projectile.Kill();
                    return;
                }

                Vector2 target = feedPlayer ? Main.player[ai0].Center : Main.npc[ai0].Center;

                if (projectile.Distance(target) < 5f)
                {
                    if (feedPlayer) //die and feed player
                    {
                        if (Main.player[ai0].whoAmI == Main.myPlayer)
                        {
                            Main.player[ai0].ClearBuff(mod.BuffType("MutantFang"));
                            Main.player[ai0].statLife += projectile.damage;
                            Main.player[ai0].HealEffect(projectile.damage);
                            if (Main.player[ai0].statLife > Main.player[ai0].statLifeMax2)
                                Main.player[ai0].statLife = Main.player[ai0].statLifeMax2;
                            projectile.Kill();
                        }
                    }
                    else
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Main.npc[ai0].life += projectile.damage;
                            Main.npc[ai0].HealEffect(projectile.damage);
                            if (Main.npc[ai0].life > Main.npc[ai0].lifeMax)
                                Main.npc[ai0].life = Main.npc[ai0].lifeMax;
                            Main.npc[ai0].netUpdate = true;
                            projectile.Kill();
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < 2; i++) //make up for real spectre bolt having 3 extraUpdates
                    {
                        Vector2 change = projectile.DirectionTo(target) * 5f;
                        projectile.velocity = (projectile.velocity * 29f + change) / 30f;
                    }
                }

                for (int i = 0; i < 3; i++) //make up for real spectre bolt having 3 extraUpdates
                {
                    projectile.position += projectile.velocity;
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            /*int ai0 = (int)projectile.ai[0];
            if (ai0 > -1 && ai0 < Main.maxNPCs && Main.npc[ai0].active && Main.npc[ai0].type == ModContent.NPCType<NPCs.MutantBoss.MutantBoss>())
            {
                CombatText.NewText(Main.npc[ai0].Hitbox, CombatText.HealLife, projectile.damage);
            }*/

            for (int i = 0; i < 5; i++)
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 229, 0f, 0f, 0, default(Color), 1.5f);
                Main.dust[d].noGravity = true;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(51, 255, 191, 210) * projectile.Opacity * 0.8f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = projectile.GetAlpha(color26);

            for (float i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i += 0.2f)
            {
                Texture2D glow = texture2D13; //mod.GetTexture("Projectiles/BossWeapons/HentaiSpearSpinGlow");
                Color color27 = color26; //Color.Lerp(new Color(255, 255, 0, 210), Color.Transparent, 0.4f);
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                float scale = projectile.scale;
                scale *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                int max0 = (int)i - 1;//Math.Max((int)i - 1, 0);
                if (max0 < 0)
                    continue;
                Vector2 center = Vector2.Lerp(projectile.oldPos[(int)i], projectile.oldPos[max0], 1 - i % 1);
                float smoothtrail = i % 1 * MathHelper.Pi / 6.85f;

                center += projectile.Size / 2;

                Main.spriteBatch.Draw(
                    glow,
                    center - Main.screenPosition + new Vector2(0, projectile.gfxOffY),
                    null,
                    color27,
                    projectile.rotation,
                    glow.Size() / 2,
                    scale,
                    SpriteEffects.None,
                    0f);
            }
            return false;
        }
    }
}