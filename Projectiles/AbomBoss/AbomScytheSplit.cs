using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.AbomBoss
{
    public class AbomScytheSplit : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_274";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Abominationn Scythe");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.aiStyle = -1;
            projectile.timeLeft = 600;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            cooldownSlot = 1;

            projectile.scale = 2f;
        }

        public override void AI()
        {
            /*if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = 1;
                Main.PlaySound(SoundID.Item71, projectile.Center);
            }*/

            projectile.rotation += 1f;

            if (--projectile.ai[0] <= 0)
                projectile.Kill();
        }

        public override void Kill(int timeLeft)
        {
            int dustMax = projectile.ai[1] >= 0 ? 50 : 25;
            float speed = projectile.ai[1] >= 0 ? 15 : 6;
            for (int i = 0; i < dustMax; i++)
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 70, Scale: 3.5f);
                Main.dust[d].velocity *= speed;
                Main.dust[d].noGravity = true;
            }

            if (projectile.ai[1] >= 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int p = Player.FindClosest(projectile.Center, 0, 0);
                    if (p != -1)
                    {
                        Vector2 vel = projectile.ai[1] == 0 ? Vector2.Normalize(projectile.velocity) : projectile.DirectionTo(Main.player[p].Center);
                        vel *= 30f;
                        int max = projectile.ai[1] == 0 ? 6 : 10;
                        for (int i = 0; i < max; i++)
                        {
                            Projectile.NewProjectile(projectile.Center, vel.RotatedBy(MathHelper.TwoPi / max * i), ModContent.ProjectileType<AbomSickle3>(), projectile.damage, projectile.knockBack, projectile.owner, p);
                        }
                    }
                }
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
            {
                target.AddBuff(mod.BuffType("AbomFang"), 300);
                //target.AddBuff(mod.BuffType("Unstable"), 240);
                target.AddBuff(mod.BuffType("Berserked"), 120);
            }
            target.AddBuff(BuffID.Bleeding, 600);
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

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 0) * projectile.Opacity;
        }
    }
}