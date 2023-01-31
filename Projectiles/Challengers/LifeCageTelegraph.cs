using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Challengers
{
    public class LifeCageTelegraph : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Projectiles/Challengers/LifeCageProjectile";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cage Telegraph");
        }
        public override void SetDefaults()
        {
            Projectile.width = 800;
            Projectile.height = 800;
            Projectile.aiStyle = 0;
            Projectile.hostile = false;
            AIType = 14;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 0;
            Projectile.light = 2f;
        }

        public override void AI()
        {
            Player player = Main.player[(int)Projectile.ai[1]];
            Projectile.Center = player.Center;
            if (Projectile.ai[0] > 240f)
            {
                Projectile.Kill();
            }
            Projectile.ai[0] += 1f;
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Rectangle rectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 origin = rectangle.Size() / 2f;
            Color color = Projectile.GetAlpha(lightColor);
            for (int i = 0; i < 26; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    Vector2 pos1 = new Vector2(Projectile.Center.X - 300 + (600 * j), Projectile.Center.Y - 300 + (24 * i));
                    Vector2 pos2 = new Vector2(Projectile.Center.X - 300 + (24 * i), Projectile.Center.Y - 300 + (600 * j));
                    Main.EntitySpriteDraw(texture, pos1 - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
                    Main.EntitySpriteDraw(texture, pos2 - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
                }
            }
            return false;
        }
    }
}
