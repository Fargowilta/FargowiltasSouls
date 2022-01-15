using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class RainbowSlimeSpikeFriendly : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_605";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Rainbow Slime Spike");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 6;
            projectile.aiStyle = -1;
            projectile.penetrate = -1;
            projectile.alpha = 255;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.timeLeft = 300;
        }

        public override void AI()
        {
            projectile.alpha -= 50;
            if (projectile.alpha < 0)
                projectile.alpha = 0;
            if (projectile.alpha == 0 && Main.rand.NextBool(3))
            {
                int d = Dust.NewDust(projectile.position - projectile.velocity * 3f, projectile.width, projectile.height, 4, 0f, 0f, 50,
                    new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, 150), 1.2f);
                Main.dust[d].velocity *= 0.3f;
                Main.dust[d].velocity += projectile.velocity * 0.3f;
                Main.dust[d].noGravity = true;
            }
            if (projectile.localAI[0] == 0f)
            {
                projectile.localAI[0] = 1f;
                Main.PlaySound(SoundID.Item17, projectile.position);
            }
            projectile.velocity.Y += 0.15f;

            projectile.rotation = projectile.velocity.ToRotation() + (float)Math.PI / 2f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(mod.BuffType("FlamesoftheUniverse"), 180);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, 200);
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