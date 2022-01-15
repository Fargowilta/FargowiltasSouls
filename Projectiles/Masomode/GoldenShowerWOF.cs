using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class GoldenShowerWOF : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_288";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Golden Shower");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 15;
            projectile.height = 15;
            projectile.aiStyle = -1;
            projectile.alpha = 255;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 300;
            projectile.hostile = true;
            projectile.extraUpdates = 2;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void AI()
        {
            if (projectile.localAI[1] == 0)
            {
                projectile.localAI[1] = 1;
                Main.PlaySound(SoundID.Item17, projectile.Center);
            }

            /*for (int i = 0; i < 2; i++) //vanilla dusts
            {
                for (int j = 0; j < 2; ++j)
                {
                    int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 170, 0.0f, 0.0f, 100, default, 0.75f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 0.1f;
                    Main.dust[d].velocity += projectile.velocity * 0.5f;
                    Main.dust[d].position -= projectile.velocity / 3 * j;
                }
                if (Main.rand.NextBool(8))
                {
                    int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 170, 0.0f, 0.0f, 100, default, 0.325f);
                    Main.dust[d].velocity *= 0.25f;
                    Main.dust[d].velocity += projectile.velocity * 0.5f;
                }
            }*/
            
            if (--projectile.ai[0] < 0)
                projectile.tileCollide = true;

            projectile.velocity.Y += 0.5f;

            projectile.rotation = projectile.velocity.ToRotation() + (float)Math.PI / 2f;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Ichor, 900);
            target.AddBuff(BuffID.OnFire, 300);
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
                Texture2D glow = mod.GetTexture("Projectiles/BossWeapons/HentaiSpearSpinGlow");
                Color color27 = Color.Lerp(new Color(255, 255, 0, 210), Color.Transparent, 0.4f);
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                float scale = projectile.scale;
                scale *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                int max0 = (int)i - 1;//Math.Max((int)i - 1, 0);
                if (max0 < 0)
                    continue;
                Vector2 center = Vector2.Lerp(projectile.oldPos[(int)i], projectile.oldPos[max0], 1 - i % 1);
                /*bool withinangle = projectile.rotation > -Math.PI / 2 && projectile.rotation < Math.PI / 2;
                if (withinangle && player.direction == 1)
                    smoothtrail *= -1;
                else if (!withinangle && player.direction == -1)
                    smoothtrail *= -1;*/

                center += projectile.Size / 2;

                //Vector2 offset = (projectile.Size / 4).RotatedBy(projectile.oldRot[(int)i] - smoothtrail * (-projectile.direction));
                Main.spriteBatch.Draw(
                    glow,
                    center - Main.screenPosition + new Vector2(0, projectile.gfxOffY),
                    null,
                    color27,
                    projectile.rotation,
                    glow.Size() / 2,
                    scale * 0.15f,
                    SpriteEffects.None,
                    0f);
            }
            return false;
        }
    }
}