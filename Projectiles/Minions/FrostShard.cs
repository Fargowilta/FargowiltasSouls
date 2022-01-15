using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class FrostShard : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_349";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Frost Shard");
            Main.projFrames[projectile.type] = 5;
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.aiStyle = 1;
            aiType = ProjectileID.FrostShard;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.timeLeft = 360;
        }

        public override void AI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter > 1)
            {
                projectile.frameCounter = 0;
                projectile.frame++;
                if (projectile.frame > 5)
                    projectile.frame = 0;
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item27, projectile.position);
            for (int index1 = 0; index1 < 3; ++index1)
            {
                int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 76, 0.0f, 0.0f, 0, new Color(), 1f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].noLight = true;
                Main.dust[index2].scale = 0.7f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 360);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, projectile.alpha);
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