using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class CelestialFragment : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Celestial Fragment");
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.aiStyle = -1;
            projectile.scale = 1.25f;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 720;
            cooldownSlot = 1;
        }

        public override void AI()
        {
            projectile.velocity *= 0.985f;
            projectile.rotation += projectile.velocity.X / 30f;
            projectile.frame = (int)projectile.ai[0];
            if (Main.rand.NextBool(20))
            {
                int type;
                switch ((int)projectile.ai[0])
                {
                    case 0: type = 242; break; //nebula
                    case 1: type = 127; break; //solar
                    case 2: type = 229; break; //vortex
                    default: type = 135; break; //stardust
                }
                Dust dust = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, type, 0f, 0f, 0, new Color(), 1f)];
                dust.velocity *= 4f;
                dust.fadeIn = 1f;
                dust.scale = 1f + Main.rand.NextFloat() + Main.rand.Next(4) * 0.3f;
                dust.noGravity = true;
            }
        }

        public override void Kill(int timeLeft)
        {
            int type;
            switch ((int)projectile.ai[0])
            {
                case 0: type = 242; break; //nebula
                case 1: type = 127; break; //solar
                case 2: type = 229; break; //vortex
                default: type = 135; break; //stardust
            }
            for (int i = 0; i < 20; i++)
            {
                Dust dust = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, type, 0f, 0f, 0, new Color(), 1f)];
                dust.velocity *= 6f;
                dust.fadeIn = 1f;
                dust.scale = 1f + Main.rand.NextFloat() + Main.rand.Next(4) * 0.3f;
                dust.noGravity = true;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(mod.BuffType("MarkedForDeath"), 300);
            target.AddBuff(mod.BuffType("CurseoftheMoon"), 300);
            switch ((int)projectile.ai[0])
            {
                case 0: target.AddBuff(mod.BuffType("ReverseManaFlow"), 180); break; //nebula
                case 1: target.AddBuff(mod.BuffType("Atrophied"), 180); break; //solar
                case 2: target.AddBuff(mod.BuffType("Jammed"), 180); break; //vortex
                default: target.AddBuff(mod.BuffType("Asocial"), 180); break; //stardust
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
            Color color26 = lightColor;
            color26 = projectile.GetAlpha(color26);
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}