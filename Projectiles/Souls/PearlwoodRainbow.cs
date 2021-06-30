using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Souls
{
    public class PearlwoodRainbow : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pearlwood Rainbow");
            ProjectileID.Sets.TrailCacheLength[base.projectile.type] = 25;
            ProjectileID.Sets.TrailingMode[base.projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 1;
            projectile.height = 1;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 150;
        }

        public override void AI()
        {
            Projectile p = Main.projectile[(int)projectile.ai[0]];

            projectile.position.X = p.position.X;// player.Center.X;
            projectile.position.Y = p.position.Y;// player.Center.Y + 5f;
        }

        public override bool PreDraw(SpriteBatch sb, Color lightColor)
        {
            float x = base.projectile.velocity.X;
            Main.instance.LoadProjectile(250);
            Texture2D texture2D32 = Main.projectileTexture[250];
            Vector2 origin9 = new Vector2((float)(texture2D32.Width / 2), 0f);
            Vector2 value36 = new Vector2((float)base.projectile.width, (float)base.projectile.height) / 2f;
            Color white2 = Color.White;
            white2.A = 127;
            for (int num271 = base.projectile.oldPos.Length - 1; num271 > 0; num271--)
            {
                Vector2 vector54 = base.projectile.oldPos[num271] + value36;
                if (!(vector54 == value36))
                {
                    Vector2 vector55 = base.projectile.oldPos[num271 - 1] + value36;
                    float rotation25 = Utils.ToRotation(vector55 - vector54) - 1.57079637f;
                    Vector2 scale10 = new Vector2(1f, Vector2.Distance(vector54, vector55) / (float)texture2D32.Height);
                    Color color53 = white2 * (1f - (float)num271 / (float)base.projectile.oldPos.Length);
                    Texture2D arg_D4F2_ = texture2D32;
                    Vector2 arg_D4F2_2 = vector54 - Main.screenPosition;
                    sb.Draw(arg_D4F2_, arg_D4F2_2, null, color53, rotation25, origin9, scale10, SpriteEffects.None, 0f);
                }
            }
            return true;
        }
    }
}