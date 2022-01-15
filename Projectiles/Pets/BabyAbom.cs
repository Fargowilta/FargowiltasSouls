using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Pets
{
    public class BabyAbom : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Baby Abom");
            Main.projFrames[projectile.type] = 8;
            Main.projPet[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 50;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.aiStyle = 26;
            aiType = ProjectileID.BabyHornet;
            projectile.netImportant = true;
            projectile.friendly = true;
        }

        public override bool PreAI()
        {
            Main.player[projectile.owner].hornet = false;
            return true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();
            if (player.dead)
            {
                modPlayer.BabyAbom = false;
            }
            if (modPlayer.BabyAbom)
            {
                projectile.timeLeft = 2;
            }

            if (projectile.Distance(player.Center) > 3000)
                projectile.Center = player.Center;

            for (int i = 0; i < 2; i++)
            {
                int index = Dust.NewDust(projectile.position, (int)(projectile.width * projectile.scale), (int)(projectile.height * projectile.scale),
                  DustID.Shadowflame, projectile.velocity.X, projectile.velocity.Y, 100, new Color(), 1.5f);
                Vector2 focus = projectile.position;
                if (projectile.direction >= 0)
                    focus.X += projectile.width;
                focus.Y += projectile.height / 2;
                Main.dust[index].position = (Main.dust[index].position + focus) / 2f;
                Main.dust[index].noGravity = true;
                Main.dust[index].velocity = Main.dust[index].velocity * 0.3f;
                Main.dust[index].velocity = Main.dust[index].velocity - projectile.velocity * 0.1f;
            }

            /*float distance = projectile.width * projectile.scale; //aura dust
            for (int i = 0; i < 10; i++)
            {
                Vector2 offset = new Vector2();
                double angle = Main.rand.NextDouble() * 2d * Math.PI;
                offset.X += (float)(Math.Sin(angle) * distance);
                offset.Y += (float)(Math.Cos(angle) * distance);
                Dust dust = Main.dust[Dust.NewDust(
                    projectile.Center + projectile.velocity + offset - new Vector2(4, 4), 0, 0,
                    DustID.Shadowflame, 0, 0, 100, Color.White, 1f)];
                dust.velocity = projectile.velocity;
                if (Main.rand.NextBool(3))
                    dust.velocity += Vector2.Normalize(offset) * -3f;
                dust.noGravity = true;
            }*/
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            SpriteEffects spriteEffects = projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, spriteEffects, 0f);
            return false;
        }
    }
}