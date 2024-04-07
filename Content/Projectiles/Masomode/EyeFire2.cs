using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class EyeFire2 : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_96";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Eye Fire");
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.EyeFire); //has 4 updates per tick
            AIType = ProjectileID.EyeFire;
            Projectile.DamageType = DamageClass.Default;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            Projectile.alpha = 0;
        }

        public override void AI()
        {
            Projectile.rotation += 0.3f;
        }

        public override void OnKill(int timeLeft)
        {
            for (int index1 = 0; index1 < 5; ++index1) //vanilla code
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.CursedTorch, -Projectile.velocity.X * 0.2f, -Projectile.velocity.Y * 0.2f, 100, default, 2f * Projectile.scale);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity *= 2f;
                int index3 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.CursedTorch, -Projectile.velocity.X * 0.2f, -Projectile.velocity.Y * 0.2f, 100, default, 1f * Projectile.scale);
                Main.dust[index3].velocity *= 2f;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (Main.rand.NextBool(6))
                target.AddBuff(39, 480, true);
            else if (Main.rand.NextBool(4))
                target.AddBuff(39, 300, true);
            else if (Main.rand.NextBool())
                target.AddBuff(39, 180, true);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, 25) * Projectile.Opacity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}