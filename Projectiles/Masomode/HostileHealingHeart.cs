using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles
{
    public class HostileHealingHeart : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Projectiles/Masomode/FakeHeart";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hostile Healing Heart");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.timeLeft = 600;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.extraUpdates = 1;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            if (--Projectile.ai[1] < 0)
            {
                NPC n = FargoSoulsUtil.NPCExists(Projectile.ai[0]);
                if (n != null && !n.friendly)
                {
                    for (int i = 0; i < 3; i++) //make up for real spectre bolt having 3 extraUpdates
                    {
                        Vector2 change = Projectile.DirectionTo(n.Center) * 2f;
                        Projectile.velocity = (Projectile.velocity * 29f + change) / 30f;
                    }

                    if (Projectile.Distance(n.Center) < 16) //die and feed it
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            n.life += Projectile.damage;
                            n.HealEffect(Projectile.damage);
                            if (n.life > n.lifeMax)
                                n.life = n.lifeMax;
                            Projectile.Kill();
                        }
                    }
                }
                else
                {
                    Projectile.Kill();
                }
            }

            for (int i = 0; i < 3; i++) //make up for real spectre bolt having 3 extraUpdates
            {
                Projectile.position += Projectile.velocity;
            }

            if (Projectile.velocity != Vector2.Zero)
                Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 86, 0f, 0f, 0, default(Color), 1.5f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 4f;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, lightColor.G, lightColor.B, lightColor.A) * Projectile.Opacity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = color26 * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}