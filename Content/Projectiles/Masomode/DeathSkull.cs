using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class DeathSkull : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Death Skull");
            Main.projFrames[Projectile.type] = 8;
        }

        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 60;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            Projectile.timeLeft = 2;

            Player player = Main.player[Projectile.owner];
            if (!player.active || player.dead || player.ghost || !player.FargoSouls().DeathMarked)
            {
                Projectile.Kill();
                return;
            }

            Projectile.Center = player.Center - 60f * Vector2.UnitY;

            if (Main.rand.NextBool())
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Asphalt, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 0, default, 1.5f);
                Main.dust[dust].velocity.Y--;
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 3f;
                Main.dust[dust].velocity.Y -= 0.5f;
            }

            if (++Projectile.frameCounter > 2)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 8)
                    Projectile.frame = 0;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}