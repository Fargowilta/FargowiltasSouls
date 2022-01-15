using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class CrystalLeafShot : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_227";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Crystal Leaf");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 900;
            projectile.aiStyle = 43;
            aiType = ProjectileID.CrystalLeafShot;
            projectile.penetrate = -1;
        }

        public override void AI()
        {
            if (!Collision.SolidCollision(projectile.position + projectile.velocity, projectile.width, projectile.height))
                Lighting.AddLight(projectile.Center + projectile.velocity, 0.1f, 0.4f, 0.2f);
            if (projectile.timeLeft < 900 - 120)
                projectile.tileCollide = true;
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            if (target.hurtCooldowns[1] == 0)
            {
                target.AddBuff(BuffID.Poisoned, 300);
                target.AddBuff(mod.BuffType("Infested"), 180);
                target.AddBuff(mod.BuffType("IvyVenom"), 240);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            SpriteEffects spriteEffects = projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Color color26 = lightColor;
            color26 = projectile.GetAlpha(color26);

            float scale = projectile.scale * 1.5f;

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, projectile.rotation, origin2, scale, spriteEffects, 0f);

            color26.A = 150;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, projectile.rotation, origin2, scale, spriteEffects, 0f);
            return false;
        }
    }
}