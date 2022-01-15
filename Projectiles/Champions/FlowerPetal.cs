using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Champions
{
    public class FlowerPetal : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_221";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Flower Petal");
            Main.projFrames[projectile.type] = 3;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.aiStyle = -1;
            projectile.hostile = true;
            projectile.timeLeft = 240;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;

            projectile.alpha = 0;
            projectile.hide = true;
            cooldownSlot = 1;
        }

        public override void AI()
        {
            if (projectile.ai[0] == 0 && projectile.timeLeft > 105)
                projectile.timeLeft = 105;

            if (projectile.localAI[0] == 0f)
            {
                projectile.localAI[0] = 1f;
                projectile.scale = Main.rand.NextFloat(1.5f, 2f);
                projectile.frame = Main.rand.Next(3);
                projectile.hide = false;
                Main.PlaySound(SoundID.Item8, projectile.Center);
            }

            if (++projectile.localAI[1] > 30 && projectile.localAI[1] < 100)
            {
                projectile.velocity *= 1.06f;
            }

            projectile.rotation += projectile.velocity.X * 0.01f;

            int dust = Dust.NewDust(projectile.Center, 0, 0, 86);
            Main.dust[dust].noGravity = true;
            Main.dust[dust].scale *= 2f;
            Main.dust[dust].velocity *= 0.1f;

            Dust.NewDust(projectile.position, projectile.width, projectile.height, 86, projectile.velocity.X, projectile.velocity.Y);
        }

        public override void Kill(int timeLeft)
        {
            if (projectile.ai[0] == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = -1; i <= 1; i++) //split
                {
                    Projectile.NewProjectile(projectile.Center, projectile.velocity.RotatedBy(MathHelper.ToRadians(5) * i) / 2f,
                        projectile.type, projectile.damage, 0f, Main.myPlayer, 1f);
                }
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
                target.AddBuff(ModContent.BuffType<Buffs.Masomode.Purified>(), 300);
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

            Color color26 = lightColor;
            color26 = projectile.GetAlpha(color26);

            SpriteEffects effects = projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Color color27 = Color.White * projectile.Opacity * 0.75f * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, effects, 0f);
            }

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, effects, 0f);
            return false;
        }
    }
}