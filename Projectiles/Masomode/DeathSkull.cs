using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class DeathSkull : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Death Skull");
            Main.projFrames[projectile.type] = 8;
        }

        public override void SetDefaults()
        {
            projectile.width = 42;
            projectile.height = 60;
            projectile.tileCollide = false;
            projectile.aiStyle = -1;
        }

        public override void AI()
        {
            projectile.timeLeft = 2;

            Player player = Main.player[projectile.owner];
            if (!player.active || player.dead || player.ghost || !player.GetModPlayer<FargoPlayer>().DeathMarked)
            {
                projectile.Kill();
                return;
            }

            projectile.Center = player.Center - 60f * Vector2.UnitY;

            if (Main.rand.NextBool())
            {
                int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 109, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 0, default(Color), 1.5f);
                Main.dust[dust].velocity.Y--;
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 3f;
                Main.dust[dust].velocity.Y -= 0.5f;
            }

            if (++projectile.frameCounter > 2)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= 8)
                    projectile.frame = 0;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * projectile.Opacity;
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