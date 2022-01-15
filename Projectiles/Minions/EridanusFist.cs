using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class EridanusFist : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Eridanus Fist");
            Main.projFrames[projectile.type] = 11;
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.alpha = 0;
            projectile.timeLeft = 300;
            projectile.extraUpdates = 1;
            projectile.hide = true;
            projectile.scale = 1.25f;
        }

        public override void AI()
        {
            if (projectile.Distance(Main.player[projectile.owner].Center) > projectile.ai[0])
            {
                projectile.Kill();
                return;
            }

            if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = 1;
                projectile.frame = Main.rand.Next(Main.projFrames[projectile.type]);
                Main.PlaySound(SoundID.Item, projectile.Center, 14);
            }

            projectile.hide = false;
            projectile.direction = projectile.spriteDirection = Math.Sign(projectile.velocity.X);
            projectile.rotation = projectile.velocity.ToRotation();
            if (projectile.spriteDirection < 0)
                projectile.rotation += (float)Math.PI;

            if (++projectile.frameCounter > 2)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= Main.projFrames[projectile.type])
                    projectile.frame = 0;
            }

            int index = Dust.NewDust(projectile.position, projectile.width, projectile.height,
                DustID.Fire, projectile.velocity.X, projectile.velocity.Y, 100, new Color(), 1.2f);
            Main.dust[index].position = (Main.dust[index].position + projectile.Center) / 2f;
            Main.dust[index].noGravity = true;
            Main.dust[index].velocity = Main.dust[index].velocity * 0.3f;
            Main.dust[index].velocity = Main.dust[index].velocity - projectile.velocity * 0.1f;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item, projectile.Center, 14);

            for (int i = 0; i < 5; i++)
            {
                int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 31, 0f, 0f, 100, default(Color), 3f);
                Main.dust[dust].velocity *= 1.4f;
            }

            for (int i = 0; i < 5; i++)
            {
                int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, 0f, 0f, 100, default(Color), 3.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 7f;
                dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, 0f, 0f, 100, default(Color), 1.5f);
                Main.dust[dust].velocity *= 3f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 600);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            SpriteEffects effects = projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, effects, 0f);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * projectile.Opacity;
        }
    }
}