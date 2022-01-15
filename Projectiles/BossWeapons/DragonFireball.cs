using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class DragonFireball : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_711";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Fireball");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.timeLeft = 180;
            projectile.alpha = 60;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            //cooldownSlot = 1;
        }

        Vector2 initialvel;
        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + (float)Math.PI / 2;
            projectile.ai[1]++;
            if (initialvel.Length() < 36)
                initialvel *= 1.03f;

            if (projectile.velocity == Vector2.Zero)
                projectile.active = false;

            const int interval = 15;
            if (projectile.ai[0] == 0)
            {
                initialvel = projectile.velocity;
                projectile.ai[0]++;
                projectile.ai[1] = Main.rand.Next(interval);
                projectile.localAI[1] = Main.rand.NextBool() ? 1 : -1;
                projectile.netUpdate = true;
            }
            else
            {
                projectile.velocity = initialvel.RotatedBy(projectile.localAI[1] * Math.PI / 8 * Math.Sin(2f * MathHelper.Pi * projectile.ai[1] / interval));
            }

        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 180, false);
            target.AddBuff(BuffID.Oiled, 180, false);
            target.AddBuff(BuffID.BetsysCurse, 180, false);
        }

        public override void Kill(int timeLeft)
        {
            if (projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(projectile.Center, Vector2.Zero, mod.ProjectileType("DragonFireballBoom"), 0, 0, Main.myPlayer);
            Main.PlaySound(SoundID.DD2_BetsysWrathImpact, projectile.Center);

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
                Main.dust[dust].shader = GameShaders.Armor.GetSecondaryShader(41, Main.LocalPlayer);
                int dust2 = Dust.NewDust(projectile.position, projectile.width,
                    projectile.height, 6, 0f, 0f, 100, default(Color), 1.5f);
                Main.dust[dust2].velocity *= 3f;
                Main.dust[dust2].shader = GameShaders.Armor.GetSecondaryShader(41, Main.LocalPlayer);
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

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = Color.Fuchsia;
            color26 = projectile.GetAlpha(color26);
            color26.A = (byte)projectile.alpha;

            SpriteEffects effects = projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                float lerpamount = 0.1f;
                if (i > 3 && i < 5)
                    lerpamount = 0.6f;
                if (i >= 5)
                    lerpamount = 0.8f;
                    
                Color color27 = Color.Lerp(Color.Fuchsia, Color.Black, lerpamount) * 0.75f * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                float scale = projectile.scale * (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, scale, effects, 0f);
            }

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, projectile.rotation, origin2, projectile.scale, effects, 0f);
            return false;
        }
    }
}