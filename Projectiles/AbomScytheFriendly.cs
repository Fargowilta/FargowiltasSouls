using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles
{
    public class AbomScytheFriendly : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Abominable Scythe");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 600;
            projectile.alpha = 100;
            projectile.penetrate = -1;

            projectile.GetGlobalProjectile<FargoGlobalProjectile>().CanSplit = false;
        }

        public override void AI()
        {
            if (projectile.localAI[1] == 0f)
            {
                projectile.localAI[1] = 1f;
                switch ((int)projectile.ai[1])
                {
                    case 1: projectile.melee = true; break;
                    case 2: projectile.ranged = true; break;
                    case 3: projectile.magic = true; break;
                    case 4: projectile.minion = true; break;
                    case 5: projectile.thrown = true; break;
                    case 6: projectile.ranged = true; projectile.timeLeft -= 420; break;
                    default: break;
                }
                projectile.ai[1] = projectile.velocity.Length();
                projectile.netUpdate = true;
            }
            projectile.velocity = projectile.velocity.RotatedBy(projectile.ai[1] / (2 * Math.PI * projectile.ai[0] * ++projectile.localAI[0]));
            
            projectile.rotation += 1f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 1;
            //target.AddBuff(mod.BuffType("OceanicMaul"), 900);
            //target.AddBuff(mod.BuffType("CurseoftheMoon"), 900);
        }

        public override void Kill(int timeLeft)
        {
            int num1 = 36;
            for (int index1 = 0; index1 < num1; ++index1)
            {
                Vector2 vector2_1 = (Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f).RotatedBy((double)(index1 - (num1 / 2 - 1)) * 6.28318548202515 / (double)num1, new Vector2()) + projectile.Center;
                Vector2 vector2_2 = vector2_1 - projectile.Center;
                int index2 = Dust.NewDust(vector2_1 + vector2_2, 0, 0, DustID.Shadowflame, vector2_2.X * 2f, vector2_2.Y * 2f, 100, new Color(), 1.4f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].noLight = true;
                Main.dust[index2].velocity = vector2_2;
            }
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

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i += 2)
            {
                Color color27 = color26;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
    }
}