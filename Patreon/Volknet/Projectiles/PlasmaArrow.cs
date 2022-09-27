using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Patreon.Volknet.Projectiles
{
    public class PlasmaArrow : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plasma Arrow");
            //DisplayName.AddTranslation(GameCulture.Chinese, "等离子矢");
        }
        public override void SetDefaults()
        {
            Projectile.width = 15;
            Projectile.height = 15;
            Projectile.scale = 0.5f;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 10;
            Projectile.alpha = 255;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.arrow = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Projectile.localAI[1]++;
            if (Projectile.velocity.Length() < 30)
            {
                Projectile.velocity *= 1.04f;
            }
            if (Projectile.localAI[1] > 10 && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
            {
                Projectile.tileCollide = true;
            }
            Projectile.alpha -= 50;
            if (Projectile.alpha < 0) Projectile.alpha = 0;
            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X);
        }

        public override Color? GetAlpha(Color lightColor) => Color.White * 0.4f * Projectile.Opacity;

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.ai[0] = (Projectile.ai[0] + 1) % 40;
            Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            for (float i = 0; i < 3; i++)
            {
                Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition + (i * MathHelper.TwoPi / 3 + Projectile.ai[0] / 40 * MathHelper.TwoPi).ToRotationVector2() * 2, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            }
            return false;
        }
        
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, 20, 20, 157, default, default, default, default, 3);
                dust.velocity = Vector2.Normalize(dust.position - Projectile.Center) * Main.rand.Next(20);
                dust.noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, 20, 20, 157, default, default, default, default, 3);
                dust.velocity = Vector2.Normalize(dust.position - Projectile.Center) * Main.rand.Next(20);
                dust.noGravity = true;
            }
            target.AddBuff(BuffID.WitheredArmor, 600);
            target.AddBuff(BuffID.Confused, 600);
        }
    }
}