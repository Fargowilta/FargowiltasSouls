using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class TorchGodFlame : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Torch God");
            Main.projFrames[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.scale = 1.5f;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
        }

        public override bool? CanDamage() => Projectile.alpha == 0 ? base.CanDamage() : false;

        public override void AI()
        {
            if (Projectile.wet)
            {
                Projectile.timeLeft = 0;
                return;
            }

            Projectile.localAI[0] += 1;
            if (Projectile.localAI[0] == 0)
                SoundEngine.PlaySound(SoundID.Item117, Projectile.Center);

            const float danceTime = 240;
            float interval = MathHelper.TwoPi / danceTime * Projectile.localAI[0];
            Projectile.velocity = new Vector2(Projectile.ai[0] * (float)Math.Sin(interval), Projectile.ai[1] * (float)Math.Cos(interval));

            if (Projectile.localAI[0] > 600)
            {
                Projectile.alpha += 6;
                if (Projectile.alpha > 255)
                {
                    Projectile.alpha = 255;
                    Projectile.Kill();
                    return;
                }
            }
            else
            {
                Projectile.alpha -= 6;
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;
            }

            Lighting.AddLight((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16, TorchID.Torch, Projectile.Opacity);

            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }

            if (Main.rand.NextBool())
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch);
                Main.dust[d].velocity += Projectile.velocity * 2f;
                Main.dust[d].velocity.Y -= 3f;
                Main.dust[d].velocity *= 0.5f;
                Main.dust[d].scale *= 1.25f;
            }
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 100) * Projectile.Opacity;

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OnFire, 60);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            SpriteEffects effects = SpriteEffects.None;

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Vector2.UnitY * Projectile.scale * 2 - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}