using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class BetsyElectrosphere2 : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_443";

        public float ai0;

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Sky Dragon's Fury");
            Main.projFrames[projectile.type] = Main.projFrames[ProjectileID.Electrosphere];
        }

        public override void SetDefaults()
        {
            projectile.width = 80;
            projectile.height = 80;
            projectile.aiStyle = 77;
            projectile.alpha = 255;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 300;
            projectile.hostile = true;
            projectile.penetrate = -1;
        }

        public override bool PreAI()
        {
            if (projectile.timeLeft == 300)
                ai0 = projectile.ai[0];
            return true;
        }

        public override void AI()
        {
            projectile.velocity += projectile.velocity.RotatedBy(Math.PI / 2) / ai0 * projectile.velocity.Length();
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            //target.AddBuff(BuffID.OnFire, 600);
            //target.AddBuff(BuffID.Ichor, 600);
            target.AddBuff(BuffID.WitheredArmor, Main.rand.Next(60, 300));
            target.AddBuff(BuffID.WitheredWeapon, Main.rand.Next(60, 300));
            target.AddBuff(BuffID.Electrified, 300);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 128) * projectile.Opacity;
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

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}