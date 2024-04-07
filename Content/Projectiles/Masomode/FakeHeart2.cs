using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class FakeHeart2 : FakeHeart
    {
        public override string Texture => "FargowiltasSouls/Content/Projectiles/Masomode/FakeHeart";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 7;
            ProjectileID.Sets.TrailingMode[Type] = 2;

        }
        public override void SetDefaults()
        {
            base.SetDefaults();

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            float rand = Main.rand.Next(90, 111) * 0.01f * (Main.essScale * 0.5f);
            Lighting.AddLight(Projectile.Center, 0.5f * rand, 0.1f * rand, 0.1f * rand);

            Projectile.ai[0]--;
            if (Projectile.ai[0] > 0)
            {
                Projectile.rotation = -Projectile.velocity.ToRotation();
            }
            else if (Projectile.ai[0] == 0)
                Projectile.velocity = Vector2.Zero;
            else
            {
                Projectile.ai[1]--;
                if (Projectile.ai[1] == 0)
                {
                    Player target = FargoSoulsUtil.PlayerExists(Player.FindClosest(Projectile.Center, 0, 0));
                    if (target != null)
                    {
                        Projectile.velocity = Projectile.SafeDirectionTo(target.Center) * 20;
                        Projectile.netUpdate = true;
                    }
                }
                if (Projectile.ai[1] <= 0)
                {
                    Projectile.rotation = Projectile.velocity.ToRotation();
                }
            }

            Projectile.rotation -= (float)Math.PI / 2;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = Color.HotPink with { A = 50 } * 0.75f * Projectile.Opacity;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, SpriteEffects.None, 0);
            }

            return base.PreDraw(ref lightColor);
        }
    }
}