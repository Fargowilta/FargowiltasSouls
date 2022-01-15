using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Buffs.Masomode;

namespace FargowiltasSouls.Projectiles.Champions
{
    public class EarthChainBlast : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_687";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Chain Blast");
            Main.projFrames[projectile.type] = Main.projFrames[ProjectileID.LunarFlare];
        }

        public override void SetDefaults()
        {
            projectile.width = 100;
            projectile.height = 100;
            projectile.aiStyle = -1;
            //aiType = ProjectileID.LunarFlare;
            projectile.hostile = true;
            projectile.tileCollide = false;
            //projectile.extraUpdates = 5;
            projectile.penetrate = -1;
            projectile.scale = 1f;
            projectile.alpha = 0;

            projectile.GetGlobalProjectile<FargoGlobalProjectile>().DeletionImmuneRank = 1;
        }

        public override bool CanDamage()
        {
            return projectile.frame == 3 || projectile.frame == 4;
        }

        public override void AI()
        {
            if (projectile.position.HasNaNs())
            {
                projectile.Kill();
                return;
            }
            /*Dust dust = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 229, 0f, 0f, 0, new Color(), 1f)];
            dust.position = projectile.Center;
            dust.velocity = Vector2.Zero;
            dust.noGravity = true;
            dust.noLight = true;*/

            if (++projectile.frameCounter >= 2)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= Main.projFrames[projectile.type])
                {
                    projectile.frame--;
                    projectile.Kill();
                    return;
                }
                if (projectile.frame == 3)
                    projectile.GetGlobalProjectile<FargoGlobalProjectile>().GrazeCD = 0;
            }
            //if (++projectile.ai[0] > Main.projFrames[projectile.type] * 3) projectile.Kill();

            if (++projectile.localAI[1] == 8 && projectile.ai[1] > 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                projectile.ai[1]--;

                Vector2 baseDirection = projectile.ai[0].ToRotationVector2();
                float random = MathHelper.ToRadians(15);
                if (projectile.ai[1] > 2)
                {
                    for (int i = -1; i <= 1; i++) //split into more explosions
                    {
                        if (i == 0)
                            continue;
                        Vector2 offset = projectile.width * 1.25f * baseDirection.RotatedBy(MathHelper.ToRadians(60) * i + Main.rand.NextFloat(-random, random));
                        Projectile.NewProjectile(projectile.Center + offset, Vector2.Zero, projectile.type,
                            projectile.damage, 0f, projectile.owner, projectile.ai[0], projectile.ai[1]);
                    }
                }
                else
                {
                    Vector2 offset = projectile.width * 2.25f * baseDirection.RotatedBy(Main.rand.NextFloat(-random, random));
                    Projectile.NewProjectile(projectile.Center + offset, Vector2.Zero, projectile.type,
                        projectile.damage, 0f, projectile.owner, projectile.ai[0], projectile.ai[1]);
                }
            }

            if (projectile.localAI[0] == 0f)
            {
                projectile.localAI[0] = 1f;
                Main.PlaySound(SoundID.Item88, projectile.Center);

                projectile.position = projectile.Center;
                projectile.scale = Main.rand.NextFloat(1f, 3f);
                projectile.width = (int)(projectile.width * projectile.scale);
                projectile.height = (int)(projectile.height * projectile.scale);
                projectile.Center = projectile.position;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 300);
            if (FargoSoulsWorld.EternityMode)
            {
                target.AddBuff(BuffID.Burning, 300);
                target.AddBuff(ModContent.BuffType<Lethargic>(), 300);
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 4; ++i)
                Dust.NewDust(projectile.position, projectile.width, projectile.height, 31, 0f, 0f, 100, new Color(), 1.5f);
            /*for (int i = 0; i < 4; ++i)
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 229, 0f, 0f, 0, new Color(), 2.5f);
                Main.dust[d].velocity *= 3f;
                Main.dust[d].noGravity = true;
                Main.dust[d].noLight = true;
                d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 229, 0f, 0f, 100, new Color(), 1.5f);
                Main.dust[d].velocity *= 2f;
                Main.dust[d].noGravity = true;
                Main.dust[d].noLight = true;
            }*/
            if (Main.rand.NextBool(4))
            {
                int i2 = Gore.NewGore(projectile.position + new Vector2(projectile.width * Main.rand.Next(100) / 100f, projectile.height * Main.rand.Next(100) / 100f) - Vector2.One * 10f, new Vector2(), Main.rand.Next(61, 64), 1f);
                Main.gore[i2].velocity *= 0.3f;
                Main.gore[i2].velocity.X += Main.rand.Next(-10, 11) * 0.05f;
                Main.gore[i2].velocity.Y += Main.rand.Next(-10, 11) * 0.05f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Color color = Color.White;
            if(projectile.ai[1] > 3)
                color = Color.Lerp(new Color(255, 255, 255, 0), new Color(255, 95, 46, 50), (7-projectile.ai[1]) / 4);

            else
                color = Color.Lerp(new Color(255, 95, 46, 50), new Color(150, 35, 0, 100), (3-projectile.ai[1]) / 3);

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, 
                projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}

