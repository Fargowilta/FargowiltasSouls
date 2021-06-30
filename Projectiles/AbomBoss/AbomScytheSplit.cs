using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.AbomBoss
{
    public class AbomScytheSplit : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_274";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Abominationn Scythe");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 600;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            cooldownSlot = 1;

            projectile.scale = 2f;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = 1;
                Main.PlaySound(SoundID.Item71, projectile.Center);
            }

            projectile.rotation += 1f;

            if (--projectile.ai[0] <= 0)
                projectile.Kill();
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 50; i++)
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 70, Scale: 3.5f);
                Main.dust[d].velocity *= 15f;
                Main.dust[d].noGravity = true;
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (projectile.ai[1] == 0)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        Vector2 vel = Vector2.Normalize(projectile.velocity).RotatedBy(Math.PI / 3 * i);
                        Projectile.NewProjectile(projectile.Center, vel, mod.ProjectileType("AbomSickle"), projectile.damage, projectile.knockBack, projectile.owner);
                        Projectile.NewProjectile(projectile.Center, vel, mod.ProjectileType("AbomDeathraySmall"), projectile.damage, 0f, projectile.owner);
                    }
                }
                else
                {
                    int p = Player.FindClosest(projectile.Center, 0, 0);
                    if (p != -1)
                    {
                        Vector2 speed = projectile.DirectionTo(Main.player[p].Center);
                        for (int i = 0; i < 8; i++)
                        {
                            Projectile.NewProjectile(projectile.Center, speed.RotatedBy(Math.PI / 4 * i), mod.ProjectileType("AbomSickle"), projectile.damage, projectile.knockBack, projectile.owner);
                            Projectile.NewProjectile(projectile.Center, speed.RotatedBy(Math.PI / 4 * i), mod.ProjectileType("AbomDeathraySmall"), projectile.damage, 0f, projectile.owner);
                        }
                    }
                }
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.MasochistMode)
            {
                target.AddBuff(mod.BuffType("AbomFang"), 300);
                target.AddBuff(mod.BuffType("Unstable"), 240);
                target.AddBuff(mod.BuffType("Berserked"), 120);
            }
            target.AddBuff(BuffID.Bleeding, 600);
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

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Color color27 = color26;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * projectile.Opacity;
        }
    }
}