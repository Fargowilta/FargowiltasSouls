using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class VenomSpit : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_472";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Venom Spit");
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = 1;
            AIType = ProjectileID.WebSpit;
            Projectile.hostile = true;
            Projectile.timeLeft = 300;
        }

        public override void OnKill(int timeLeft)
        {
            for (int index1 = 0; index1 < 20; ++index1)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Web);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 0.45f;
                Main.dust[d].velocity += Projectile.velocity * 0.9f;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, lightColor.G, 255, lightColor.A);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Venom, 180);
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