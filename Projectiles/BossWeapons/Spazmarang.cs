using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class Spazmarang : ModProjectile
    {
        bool hitSomething = false;
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Spazmarang");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.melee = true;
            projectile.friendly = true;
            projectile.light = 0.4f;

            projectile.width = 50;
            projectile.height = 50;
            projectile.penetrate = 1;
            projectile.aiStyle = -1;
        }

        public override void AI()
        {
            //travelling out
            if (projectile.ai[0] == 0)
            {
                projectile.ai[1]++;

                if (projectile.ai[1] > 20)
                {
                    projectile.ai[0] = 1;
                    projectile.netUpdate = true;
                }
            }
            //travel back to player
            else 
            {
                projectile.extraUpdates = 0;
                projectile.velocity = Vector2.Normalize(Main.player[projectile.owner].Center - projectile.Center) * 45;

                //kill when back to player
                if (projectile.Distance(Main.player[projectile.owner].Center) <= 30)
                    projectile.Kill();
            }

            //spin
            projectile.rotation += projectile.direction * -0.8f;

            //dust!
            int dustId = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y + 2f), projectile.width, projectile.height + 5, 75, projectile.velocity.X * 0.2f,
                projectile.velocity.Y * 0.2f, 100, default(Color), 2f);
            Main.dust[dustId].noGravity = true;
        }

        private void spawnFire()
        {
            if (!hitSomething)
            {
                hitSomething = true;
                if (projectile.owner == Main.myPlayer)
                {
                    Main.PlaySound(SoundID.Item74, projectile.Center);
                    FargoSoulsUtil.XWay(12, projectile.Center, ModContent.ProjectileType<EyeFireFriendly>(), 3, projectile.damage / 2, projectile.knockBack);
                }
                projectile.ai[0] = 1;
                projectile.penetrate = 4;
                projectile.netUpdate = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.CursedInferno, 120);
            spawnFire();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.Distance(Main.player[projectile.owner].Center) >= 50)
            {
                spawnFire();
            }
            projectile.tileCollide = false;
            
            return false;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            //smaller tile hitbox
            width = 22;
            height = 22;
            return true;
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

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Color color27 = color26;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}