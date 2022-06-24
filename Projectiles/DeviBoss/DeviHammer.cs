using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.DeviBoss
{
    public class DeviHammer : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_300";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Paladin's Hammer");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.scale = 2f;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            CooldownSlot = 1;
        }

        public override void AI()
        {
            if (++Projectile.localAI[0] > Projectile.ai[1])
                Projectile.Kill();
            if (Projectile.localAI[1] == 0)
                Projectile.localAI[1] = Projectile.velocity.Length();

            Vector2 acceleration = Vector2.Normalize(Projectile.velocity).RotatedBy(Math.PI / 2) * Projectile.ai[0];
            Projectile.velocity = Vector2.Normalize(Projectile.velocity) * Projectile.localAI[1] + acceleration;

            Projectile.rotation += 1f * Math.Sign(Projectile.ai[0]);

            int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 246, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 0, new Color());
            Main.dust[d].noLight = true;

            //Main.NewText(Projectile.Center);
        }

        public override void Kill(int timeLeft)
        {
            for (int index1 = 0; index1 < 10; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 246, 0f, 0f, 100, new Color(), 1.5f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity *= 3.5f;
                Main.dust[index2].noLight = true;
                int index3 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 246, 0f, 0f, 100, new Color(), 1f);
                Main.dust[index3].velocity *= 2f;
                Main.dust[index3].noGravity = true;
                Main.dust[index3].noLight = true;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (!target.HasBuff(ModContent.BuffType<Buffs.Masomode.Stunned>()))
                target.AddBuff(ModContent.BuffType<Buffs.Masomode.Stunned>(), 60);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i += 2)
            {
                Color color27 = new Color(255, 255, 255, 50) * 0.75f * Projectile.Opacity;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}