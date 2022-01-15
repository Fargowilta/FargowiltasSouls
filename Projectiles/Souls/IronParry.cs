using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Souls
{
    public class IronParry : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Iron Parry");
            Main.projFrames[projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            projectile.width = 78;
            projectile.height = 78;
            projectile.aiStyle = -1;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.scale = 2f;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = 1;
                projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                Main.PlaySound(SoundID.NPCHit4, projectile.Center);

                for (int i = 0; i < 20; i++)
                {
                    int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 87, 0f, 0f, 0, new Color(255, 255, 255, 150), 1.5f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 3f;
                }
            }

            //projectile.Center = Main.player[projectile.owner].Center + new Vector2(projectile.ai[0], projectile.ai[1]);

            if (++projectile.frameCounter >= 3)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= Main.projFrames[projectile.type])
                {
                    projectile.frame--;
                    projectile.Kill();
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 150);
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
    }
}

