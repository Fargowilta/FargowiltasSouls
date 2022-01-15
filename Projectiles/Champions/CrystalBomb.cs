using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Champions
{
    public class CrystalBomb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Crystal Bomb");
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.aiStyle = -1;
            projectile.hostile = true;
            projectile.timeLeft = 600;
            //projectile.tileCollide = false;
            //projectile.ignoreWater = true;

            projectile.alpha = 255;
            projectile.hide = true;
            cooldownSlot = 1;

            projectile.scale = 2.5f;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0f)
            {
                projectile.localAI[0] = Main.rand.Next(2) == 1 ? 1f : -1f;
                projectile.rotation = Main.rand.NextFloat((float)Math.PI * 2);
                projectile.hide = false;
            }

            if (--projectile.localAI[1] < 0)
            {
                projectile.localAI[1] = 60;
                Main.PlaySound(SoundID.Item27, projectile.position);
            }

            projectile.alpha -= 10;
            if (projectile.alpha < 0)
                projectile.alpha = 0;
            if (projectile.alpha > 255)
                projectile.alpha = 255;

            projectile.rotation += (float)Math.PI / 40f * projectile.localAI[0];

            Lighting.AddLight(projectile.Center, 0.3f, 0.75f, 0.9f);

            int index3 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 197, 0.0f, 0.0f, 100, Color.Transparent, 1f);
            Main.dust[index3].noGravity = true;

            projectile.velocity *= 1.03f;

            if (projectile.Center.Y > projectile.ai[0])
                projectile.tileCollide = true;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
                target.AddBuff(BuffID.Chilled, 180);
            target.AddBuff(BuffID.Frostburn, 180);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item27, projectile.position);

            for (int index1 = 0; index1 < 40; ++index1)
            {
                int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 68, 0f, 0f, 0, default(Color), 1f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity *= 1.5f;
                Main.dust[index2].scale *= 0.9f;
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int index = 0; index < 24; ++index)
                {
                    float SpeedX = projectile.velocity.Length() * Main.rand.Next(-60, 61) * 0.01f + Main.rand.Next(-20, 21) * 0.4f;
                    float SpeedY = projectile.velocity.Length() * Main.rand.Next(-60, 61) * 0.01f + Main.rand.Next(-20, 21) * 0.4f;
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, SpeedX, SpeedY,
                        ModContent.ProjectileType<CrystalBombShard>(), projectile.damage, 0f, projectile.owner);
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * projectile.Opacity;
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

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, effects, 0f);
            return false;
        }
    }
}