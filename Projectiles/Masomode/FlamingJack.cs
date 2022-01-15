using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class FlamingJack : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_321";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Flaming Jack");
            Main.projFrames[projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.aiStyle = -1;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 600;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0f)
            {
                projectile.localAI[0] = 1f;
                Main.PlaySound(SoundID.Item8, projectile.Center);
            }

            if (++projectile.frameCounter > 0)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame > 2)
                    projectile.frame = 0;
            }

            if (--projectile.ai[1] > -60f && projectile.ai[1] < 0f) //homing for 1sec, with delay
            {
                Player player = FargoSoulsUtil.PlayerExists(projectile.ai[0]);
                if (player != null)
                {
                    Vector2 dist = player.Center - projectile.Center;
                    dist.Normalize();
                    dist *= 8f;
                    projectile.velocity.X = (projectile.velocity.X * 40 + dist.X) / 41;
                    projectile.velocity.Y = (projectile.velocity.Y * 40 + dist.Y) / 41;
                }
                else
                {
                    projectile.ai[0] = -1f;
                    projectile.netUpdate = true;
                }
            }

            if (projectile.velocity.X < 0)
            {
                projectile.spriteDirection = -1;
                projectile.rotation = -projectile.velocity.ToRotation();
            }
            else
            {
                projectile.spriteDirection = 1;
                projectile.rotation = projectile.velocity.ToRotation();
            }

            int d = Dust.NewDust(projectile.position + new Vector2(8, 8), projectile.width - 16, projectile.height - 16, 6, 0f, 0f, 0, new Color(), 1f);
            Main.dust[d].velocity *= 0.5f;
            Main.dust[d].velocity += projectile.velocity * 0.5f;
            Main.dust[d].noGravity = true;
            Main.dust[d].noLight = true;
            Main.dust[d].scale = 1.4f;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item10, projectile.position);
            for (int i = 0; i < 20; ++i)
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, -projectile.velocity.X * 0.2f, -projectile.velocity.Y * 0.2f, 100, new Color(), 2f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 2f;
                d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, -projectile.velocity.X * 0.2f, -projectile.velocity.Y * 0.2f, 100, new Color(), 1f);
                Main.dust[d].velocity *= 2f;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 600);
            target.AddBuff(mod.BuffType("LivingWasteland"), 600);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, 0);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}