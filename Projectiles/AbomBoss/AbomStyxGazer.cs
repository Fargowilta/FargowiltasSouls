using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.AbomBoss
{
    public class AbomStyxGazer : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Items/Weapons/FinalUpgrades/StyxGazer";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Styx Gazer");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        const int maxTime = 60;

        public override void SetDefaults()
        {
            projectile.width = 60;
            projectile.height = 60;
            projectile.scale = 1f;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.timeLeft = maxTime;
            //projectile.alpha = 250;
            projectile.aiStyle = -1;
            projectile.penetrate = -1;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().DeletionImmuneRank = 2;

            projectile.hide = true;
        }

        public override void AI()
        {
            projectile.hide = false; //to avoid edge case tick 1 wackiness

            //the important part
            NPC npc = FargoSoulsUtil.NPCExists(projectile.ai[0], ModContent.NPCType<NPCs.AbomBoss.AbomBoss>());
            if (npc != null)
            {
                if (npc.ai[0] == 0) projectile.extraUpdates = 1;

                if (projectile.localAI[0] == 0)
                    projectile.localAI[1] = projectile.ai[1] / maxTime; //do this first
                
                projectile.velocity = projectile.velocity.RotatedBy(projectile.ai[1]);
                projectile.ai[1] -= projectile.localAI[1];
                projectile.Center = npc.Center + new Vector2(60, 60).RotatedBy(projectile.velocity.ToRotation() - MathHelper.PiOver4) * projectile.scale;
            }
            else
            {
                projectile.Kill();
                return;
            }

            if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = 1;
                
                /*Vector2 basePos = projectile.Center - projectile.velocity * 141 / 2 * projectile.scale;
                for (int i = 0; i < 40; i++)
                {
                    int d = Dust.NewDust(basePos + projectile.velocity * Main.rand.NextFloat(127) * projectile.scale, 0, 0, 87, Scale: 3f);
                    Main.dust[d].velocity *= 4.5f;
                    Main.dust[d].noGravity = true;
                }*/

                Main.PlaySound(SoundID.Item71, projectile.Center);
            }

            /*if (projectile.timeLeft == maxTime - 20)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int p = Player.FindClosest(projectile.Center, 0, 0);
                    if (p != -1)
                    {
                        Vector2 vel = Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * 30f;
                        int max = 8;
                        for (int i = 0; i < max; i++)
                        {
                            Projectile.NewProjectile(projectile.Center, vel.RotatedBy(MathHelper.TwoPi / max * i), ModContent.ProjectileType<AbomSickle3>(), projectile.damage, projectile.knockBack, projectile.owner, p);
                        }
                    }
                }
            }*/

            projectile.Opacity = (float)Math.Min(1, (2 - projectile.extraUpdates) * Math.Sin(Math.PI * (maxTime - projectile.timeLeft) / maxTime));

            projectile.direction = projectile.spriteDirection = Math.Sign(projectile.ai[1]);
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(projectile.direction < 0 ? 135 : 45);
            //Main.NewText(MathHelper.ToDegrees(projectile.velocity.ToRotation()) + " " + MathHelper.ToDegrees(projectile.ai[1]));
        }

        public override void Kill(int timeLeft)
        {
            /*Vector2 basePos = projectile.Center - projectile.velocity * 141 / 2 * projectile.scale;
            for (int i = 0; i < 40; i++)
            {
                int d = Dust.NewDust(basePos + projectile.velocity * Main.rand.NextFloat(127) * projectile.scale, 0, 0, 87, Scale: 3f);
                Main.dust[d].velocity *= 4.5f;
                Main.dust[d].noGravity = true;
            }*/
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
            {
                //target.AddBuff(mod.BuffType("MutantNibble"), 300);
                target.AddBuff(mod.BuffType("AbomFang"), 300);
                //target.AddBuff(mod.BuffType("Unstable"), 240);
                target.AddBuff(mod.BuffType("Berserked"), 120);
            }
            target.AddBuff(BuffID.Bleeding, 600);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            Color color = lightColor * projectile.Opacity;
            color.A = (byte)(255 * projectile.Opacity);
            return color;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            SpriteEffects effects = projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Color color26 = lightColor;
            color26 = projectile.GetAlpha(color26);

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Color color27 = color26 * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, effects, 0f);
            }

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, projectile.rotation, origin2, projectile.scale, effects, 0f);
            Texture2D texture2D14 = mod.GetTexture("Items/Weapons/FinalUpgrades/StyxGazer_glow");
            Main.spriteBatch.Draw(texture2D14, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White * projectile.Opacity, projectile.rotation, origin2, projectile.scale, effects, 0f);
            return false;
        }
    }
}