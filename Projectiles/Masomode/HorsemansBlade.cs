using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class HorsemansBlade : ModProjectile
    {
        public override string Texture => "Terraria/Item_1826";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("The Horseman's Blade");
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.aiStyle = -1;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.scale = 1.15f;
            projectile.timeLeft = 300;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0f)
            {
                projectile.localAI[0] = 1f;
                Main.PlaySound(SoundID.Item1, projectile.Center);
            }

            projectile.ai[1]++;
            if (projectile.ai[1] > 60f)
            {
                projectile.velocity.X *= 0.97f;
                projectile.velocity.Y += 0.45f;
            }
            else if (projectile.ai[1] == 60f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                const int max = 4;
                for (int i = 0; i < max; i++)
                    Projectile.NewProjectile(projectile.Center, Vector2.Normalize(projectile.velocity).RotatedBy(2 * Math.PI / max * i) * 8f,
                        mod.ProjectileType("FlamingJack"), projectile.damage, 0f, Main.myPlayer, projectile.ai[0], 30f);
            }

            projectile.rotation += projectile.velocity.Length() / (projectile.velocity.X > 0 ? 30f : -30f);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 600);
            target.AddBuff(mod.BuffType("LivingWasteland"), 600);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, 0);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}