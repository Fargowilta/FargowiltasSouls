using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class DragonFireball : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_711";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fireball");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 180;
            Projectile.alpha = 60;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            //CooldownSlot = 1;
        }

        Vector2 initialvel;
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + (float)Math.PI / 2;
            Projectile.ai[1]++;
            if (initialvel.Length() < 36)
                initialvel *= 1.03f;

            if (Projectile.velocity == Vector2.Zero)
                Projectile.active = false;

            const int interval = 15;
            if (Projectile.ai[0] == 0)
            {
                initialvel = Projectile.velocity;
                Projectile.ai[0]++;
                Projectile.ai[1] = Main.rand.Next(interval);
                Projectile.localAI[1] = Main.rand.NextBool() ? 1 : -1;
                Projectile.netUpdate = true;
            }
            else
            {
                Projectile.velocity = initialvel.RotatedBy(Projectile.localAI[1] * Math.PI / 8 * Math.Sin(2f * MathHelper.Pi * Projectile.ai[1] / interval));
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
            if (Projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<DragonFireballBoom>(), 0, 0, Main.myPlayer);
            SoundEngine.PlaySound(SoundID.DD2_BetsysWrathImpact, Projectile.Center);

            for (int i = 0; i < 30; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width,
                    Projectile.height, 31, 0f, 0f, 100, default(Color), 3f);
                Main.dust[dust].velocity *= 1.4f;
            }

            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width,
                    Projectile.height, 6, 0f, 0f, 100, default(Color), 3.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 7f;
                Main.dust[dust].shader = GameShaders.Armor.GetSecondaryShader(41, Main.LocalPlayer);
                int dust2 = Dust.NewDust(Projectile.position, Projectile.width,
                    Projectile.height, 6, 0f, 0f, 100, default(Color), 1.5f);
                Main.dust[dust2].velocity *= 3f;
                Main.dust[dust2].shader = GameShaders.Armor.GetSecondaryShader(41, Main.LocalPlayer);
            }

            float scaleFactor9 = 0.5f;
            for (int j = 0; j < 4; j++)
            {
                int gore = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                    default(Vector2),
                    Main.rand.Next(61, 64));

                Main.gore[gore].velocity *= scaleFactor9;
                Main.gore[gore].velocity.X += 1f;
                Main.gore[gore].velocity.Y += 1f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = Color.Fuchsia;
            color26 = Projectile.GetAlpha(color26);
            color26.A = (byte)Projectile.alpha;

            SpriteEffects effects = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                float lerpamount = 0.1f;
                if (i > 3 && i < 5)
                    lerpamount = 0.6f;
                if (i >= 5)
                    lerpamount = 0.8f;

                Color color27 = Color.Lerp(Color.Fuchsia, Color.Black, lerpamount) * 0.75f * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                float scale = Projectile.scale * (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}