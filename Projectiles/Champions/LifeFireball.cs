using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Buffs.Masomode;

namespace FargowiltasSouls.Projectiles.Champions
{
    public class LifeFireball : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_258";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Fireball");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.aiStyle = -1;
            projectile.penetrate = -1;
            projectile.alpha = 100;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 240;
            projectile.scale = 2f;
            cooldownSlot = 1;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = 1f;
                Main.PlaySound(SoundID.Item20, projectile.position);
            }

            for (int index1 = 0; index1 < 4; ++index1)
            {
                int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 6,
                    projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 100, new Color(), 2.5f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity.X *= 0.5f;
                Main.dust[index2].velocity.Y *= 0.5f;
            }

            if (--projectile.ai[0] > 0)
            {
                float speed = projectile.velocity.Length();
                speed += projectile.ai[1];
                projectile.velocity = Vector2.Normalize(projectile.velocity) * speed;
            }
            else if (projectile.ai[0] == 0)
            {
                projectile.ai[1] = Player.FindClosest(projectile.Center, 0, 0);

                if (projectile.ai[1] != -1 && Main.player[(int)projectile.ai[1]].active && !Main.player[(int)projectile.ai[1]].dead)
                {
                    projectile.velocity = projectile.DirectionTo(Main.player[(int)projectile.ai[1]].Center);
                    projectile.netUpdate = true;
                }
                else
                {
                    projectile.Kill();
                }
            }
            else
            {
                projectile.tileCollide = true;

                if (++projectile.localAI[1] < 90) //accelerate
                {
                    projectile.velocity *= 1.04f;
                }

                if (projectile.localAI[1] < 120)
                {
                    float rotation = projectile.velocity.ToRotation();
                    Vector2 vel = Main.player[(int)projectile.ai[1]].Center - projectile.Center;
                    float targetAngle = vel.ToRotation();
                    projectile.velocity = new Vector2(projectile.velocity.Length(), 0f).RotatedBy(rotation.AngleLerp(targetAngle, 0.025f));
                }

                /*if (projectile.velocity.Y <= 0) //don't home upwards ever
                {
                    projectile.velocity = projectile.oldVelocity;
                    if (projectile.localAI[1] < 90)
                        projectile.velocity *= 1.04f;
                }*/
            }

            projectile.rotation += 0.2f;
        }

        public override void Kill(int timeLeft)
        {
            if (timeLeft > 0)
            {
                for (int i = 0; i < 5; i++) //drop greek fire
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, Main.rand.NextFloat(-6, 6), Main.rand.NextFloat(-10, 0),
                              Main.rand.Next(326, 329), projectile.damage / 4, 0f, Main.myPlayer);
                    }
                }
            }

            Main.PlaySound(SoundID.Item, (int)projectile.position.X, (int)projectile.position.Y, 14);
            
            for (int i = 0; i < 30; i++)
            {
                int dust = Dust.NewDust(projectile.position, projectile.width,
                    projectile.height, 31, 0f, 0f, 100, default(Color), 3f);
                Main.dust[dust].velocity *= 1.4f;
            }

            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(projectile.position, projectile.width,
                    projectile.height, 6, 0f, 0f, 100, default(Color), 3.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 7f;
                dust = Dust.NewDust(projectile.position, projectile.width,
                    projectile.height, 6, 0f, 0f, 100, default(Color), 1.5f);
                Main.dust[dust].velocity *= 3f;
            }

            float scaleFactor9 = 0.5f;
            for (int j = 0; j < 4; j++)
            {
                int gore = Gore.NewGore(new Vector2(projectile.Center.X, projectile.Center.Y),
                    default(Vector2),
                    Main.rand.Next(61, 64));

                Main.gore[gore].velocity *= scaleFactor9;
                Main.gore[gore].velocity.X += 1f;
                Main.gore[gore].velocity.Y += 1f;
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            fallThrough = false;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
            {
                target.AddBuff(BuffID.CursedInferno, 120);
                target.AddBuff(ModContent.BuffType<Shadowflame>(), 120);
            }
            target.AddBuff(BuffID.OnFire, 120);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, 25);
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

            SpriteEffects effects = projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Color color27 = Color.Red * projectile.Opacity * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, effects, 0f);
            }

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, effects, 0f);
            return false;
        }
    }
}