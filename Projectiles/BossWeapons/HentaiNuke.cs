using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Buffs.Masomode;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class HentaiNuke : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_645";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Phantasmal Blast");
            Main.projFrames[projectile.type] = 16;
        }

        public override void SetDefaults()
        {
            projectile.width = 470;
            projectile.height = 624;
            projectile.aiStyle = -1;
            //aiType = ProjectileID.LunarFlare;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.tileCollide = false;
            //projectile.extraUpdates = 5;
            projectile.penetrate = -1;
            projectile.scale = 1.5f;
            projectile.alpha = 0;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().CanSplit = false;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().DeletionImmuneRank = 2;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            projHitbox.X = projHitbox.X + projHitbox.Width / 2;
            projHitbox.Y = projHitbox.Y + projHitbox.Height / 2;
            projHitbox.Width = (int)(420 * projectile.scale);
            projHitbox.Height = (int)(420 * projectile.scale);
            projHitbox.X = projHitbox.X - projHitbox.Width / 2;
            projHitbox.Y = projHitbox.Y - projHitbox.Height / 2;
            return null;
        }

        public override void AI()
        {
            if (projectile.position.HasNaNs())
            {
                projectile.Kill();
                return;
            }

            if (++projectile.frameCounter >= 3)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= Main.projFrames[projectile.type])
                {
                    projectile.frame--;
                    projectile.Kill();
                }
            }

            if (projectile.localAI[0] == 0f)
            {
                projectile.localAI[0] = 1f;
                Main.PlaySound(SoundID.Item88, projectile.Center);

                if (!Main.dedServ && Main.LocalPlayer.active)
                    Main.LocalPlayer.GetModPlayer<FargoPlayer>().Screenshake = 30;

                if (!Main.dedServ)
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Thunder").WithVolume(0.8f).WithPitchVariance(-0.5f), projectile.Center);

                for (int a = 0; a < 4; a++)
                {
                    for (int index1 = 0; index1 < 3; ++index1)
                    {
                        int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 31, 0.0f, 0.0f, 100, new Color(), 1.5f);
                        Main.dust[index2].position = new Vector2((float)(projectile.width / 2), 0.0f).RotatedBy(6.28318548202515 * Main.rand.NextDouble(), new Vector2()) * (float)Main.rand.NextDouble() + projectile.Center;
                    }
                    for (int index1 = 0; index1 < 10; ++index1)
                    {
                        int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 229, 0.0f, 0.0f, 0, new Color(), 2.5f);
                        Main.dust[index2].position = new Vector2((float)(projectile.width / 2), 0.0f).RotatedBy(6.28318548202515 * Main.rand.NextDouble(), new Vector2()) * (float)Main.rand.NextDouble() + projectile.Center;
                        Main.dust[index2].noGravity = true;
                        Dust dust1 = Main.dust[index2];
                        dust1.velocity = dust1.velocity * 1f;
                        int index3 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 229, 0.0f, 0.0f, 100, new Color(), 1.5f);
                        Main.dust[index3].position = new Vector2((float)(projectile.width / 2), 0.0f).RotatedBy(6.28318548202515 * Main.rand.NextDouble(), new Vector2()) * (float)Main.rand.NextDouble() + projectile.Center;
                        Dust dust2 = Main.dust[index3];
                        dust2.velocity = dust2.velocity * 1f;
                        Main.dust[index3].noGravity = true;
                    }

                    for (int i = 0; i < 10; i++)
                    {
                        int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 229, 0f, 0f, 100, default, 3f);
                        Main.dust[dust].velocity *= 1.4f;
                    }

                    for (int i = 0; i < 10; i++)
                    {
                        int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, 0f, 0f, 100, default, 3.5f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= 7f;
                        dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, 0f, 0f, 100, default, 1.5f);
                        Main.dust[dust].velocity *= 3f;
                    }

                    for (int index1 = 0; index1 < 10; ++index1)
                    {
                        int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 229, 0f, 0f, 100, new Color(), 2f);
                        Main.dust[index2].noGravity = true;
                        Main.dust[index2].velocity *= 21f * projectile.scale;
                        Main.dust[index2].noLight = true;
                        int index3 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 229, 0f, 0f, 100, new Color(), 1f);
                        Main.dust[index3].velocity *= 12f;
                        Main.dust[index3].noGravity = true;
                        Main.dust[index3].noLight = true;
                    }

                    for (int i = 0; i < 10; i++)
                    {
                        int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 229, 0f, 0f, 100, default, Main.rand.NextFloat(2f, 3.5f));
                        if (Main.rand.NextBool(3))
                            Main.dust[d].noGravity = true;
                        Main.dust[d].velocity *= Main.rand.NextFloat(9f, 12f);
                        Main.dust[d].position = Main.player[projectile.owner].Center;
                    }
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<CurseoftheMoon>(), 600);
            target.immune[projectile.owner] = 1;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 200);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = mod.GetTexture("Projectiles/BossWeapons/HentaiNuke/HentaiNuke_" + projectile.frame.ToString());
            Rectangle rectangle = texture2D13.Bounds;
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}

