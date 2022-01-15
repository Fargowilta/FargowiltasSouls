using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class MimicCoin : ModProjectile
    {
        public int coinType = -1;

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Coin");
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.aiStyle = 1;
            projectile.hostile = true;
            projectile.timeLeft = 600;
            projectile.extraUpdates = 1;
            projectile.alpha = 255;
        }

        public override bool PreAI()
        {
            if (coinType == -1)
            {
                coinType = (int)projectile.ai[0];
                switch (coinType)
                {
                    case 0: aiType = ProjectileID.CopperCoin; projectile.frame = 0; break;
                    case 1: aiType = ProjectileID.SilverCoin; projectile.frame = 1; break;
                    case 2: aiType = ProjectileID.GoldCoin; projectile.frame = 2; break;
                    default: aiType = ProjectileID.PlatinumCoin; projectile.frame = 3; break;
                }
            }
            return true;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Dig, (int)projectile.position.X, (int)projectile.position.Y, 1, 1f, 0.0f);
            int dusttype;
            switch (coinType)
            {
                case 0: dusttype = 9; break;
                case 1: dusttype = 11; break;
                case 2: dusttype = 19; break;
                default: dusttype = 11; break;
            }
            for (int index1 = 0; index1 < 10; ++index1)
            {
                int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, dusttype, 0.0f, 0.0f, 0, new Color(), 1f);
                Main.dust[index2].noGravity = true;
                Dust dust = Main.dust[index2];
                dust.velocity = dust.velocity - projectile.velocity * 0.5f;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(mod.BuffType("Midas"), 300);
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