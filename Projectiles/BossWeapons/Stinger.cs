using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class Stinger : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_374";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Stinger");
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.HornetStinger);
            aiType = ProjectileID.Stinger;
            projectile.penetrate = 2;
            projectile.minion = false;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.timeLeft = 240;
        }

        public override void AI()
        {
            //projectile.position += projectile.velocity * 0.5f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Venom, 600);
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