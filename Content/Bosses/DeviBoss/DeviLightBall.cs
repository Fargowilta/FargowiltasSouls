using FargowiltasSouls.Assets.ExtraTextures;


using FargowiltasSouls.Content.Projectiles.Deathrays;
using FargowiltasSouls.Content.Projectiles.Masomode;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.DeviBoss
{
    public class DeviLightBall : LightBall, IPixelatedPrimitiveRenderer
    {
        public override bool DoNotSpawnDust => true;

        public override string Texture => "FargowiltasSouls/Content/Projectiles/Masomode/LightBall";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailingMode[Type] = 1;
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Projectile.penetrate = -1;
            Projectile.timeLeft = 270;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);

            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            for (int index1 = 0; index1 < 10; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GoldCoin, -Projectile.velocity.X * 0.2f, -Projectile.velocity.Y * 0.2f, 100, new Color(), 2f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity *= 2f;
                int index3 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GoldCoin, -Projectile.velocity.X * 0.2f, -Projectile.velocity.Y * 0.2f, 100, new Color(), 1f);
                Main.dust[index3].velocity *= 2f;
            }

            if (Projectile.ai[0] == 0f && FargoSoulsUtil.HostCheck)
            {
                //wind velocity back slightly
                Vector2 vel = Vector2.Normalize(Projectile.velocity).RotatedBy(MathHelper.ToRadians(30f) * -Math.Sign(Projectile.ai[1]));
                //make it cover the windback and then some more
                float ai0 = MathHelper.ToRadians(30 + 60) * Math.Sign(Projectile.ai[1]);
                Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, vel, ModContent.ProjectileType<DeviLightBeam>(),
                    Projectile.damage, Projectile.knockBack, Projectile.owner, ai0);
            }
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 1.3f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public static Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(new(250, 249, 128), Color.Transparent, completionRatio) * 0.7f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int num156 = TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch)
        {
            ManagedShader shader = ShaderManager.GetShader("FargowiltasSouls.BlobTrail");
            FargoSoulsUtil.SetTexture1(FargosTextureRegistry.FadedStreak.Value);
            PrimitiveRenderer.RenderTrail(Projectile.oldPos, new(WidthFunction, ColorFunction, _ => Projectile.Size * 0.5f, Pixelate: true, Shader: shader), 25);
        }
    }
}