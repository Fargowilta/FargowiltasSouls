using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Minions
{
    public class EridanusFist : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Eridanus Fist");
            Main.projFrames[Projectile.type] = 11;
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.alpha = 0;
            Projectile.timeLeft = 300;
            Projectile.extraUpdates = 1;
            Projectile.hide = true;
            Projectile.scale = 1.25f;
        }

        public override void AI()
        {
            if (Projectile.Distance(Main.player[Projectile.owner].Center) > Projectile.ai[0])
            {
                Projectile.Kill();
                return;
            }

            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;
                Projectile.frame = Main.rand.Next(Main.projFrames[Projectile.type]);
                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            }

            Projectile.hide = false;
            Projectile.direction = Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.spriteDirection < 0)
                Projectile.rotation += (float)Math.PI;

            if (++Projectile.frameCounter > 2)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }

            int index = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height,
                DustID.Torch, Projectile.velocity.X, Projectile.velocity.Y, 100, new Color(), 1.2f);
            Main.dust[index].position = (Main.dust[index].position + Projectile.Center) / 2f;
            Main.dust[index].noGravity = true;
            Main.dust[index].velocity = Main.dust[index].velocity * 0.3f;
            Main.dust[index].velocity = Main.dust[index].velocity - Projectile.velocity * 0.1f;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

            for (int i = 0; i < 5; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 3f);
                Main.dust[dust].velocity *= 1.4f;
            }

            for (int i = 0; i < 5; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 3.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 7f;
                dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 1.5f);
                Main.dust[dust].velocity *= 3f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 600);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            SpriteEffects effects = Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }
    }
}