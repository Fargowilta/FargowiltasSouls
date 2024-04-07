using FargowiltasSouls.Assets.ExtraTextures;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Lifelight
{

    public class LifeBombExplosion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Life Bomb");
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.timeLeft = 3600;
            if (Main.getGoodWorld)
                Projectile.timeLeft *= 10;
        }

        public override bool? CanDamage() => Projectile.alpha < 100;

        public static int MaxTime => Main.getGoodWorld ? 2400 * 10 : 2400;

        public override void AI()
        {
            /*
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 3)
                    Projectile.frame = 0;
            }
            */
            Projectile.rotation += 2f;
            if (Main.rand.NextBool(6))
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemTopaz);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 0.5f;
            }

            //pulsate
            if (Projectile.localAI[0] == 0)
                Projectile.localAI[0] += Main.rand.Next(60);
            Projectile.scale = 1.1f + 0.1f * (float)Math.Sin(MathHelper.TwoPi / 15 * ++Projectile.localAI[1]);

            if (Projectile.ai[0] > MaxTime - 30)
            {
                Projectile.alpha += 8;
                if (Projectile.alpha > 255)
                    Projectile.alpha = 255;
            }

            if (Projectile.ai[0] > MaxTime || NPC.CountNPCS(ModContent.NPCType<LifeChallenger>()) < 1)
            {
                for (int i = 0; i < 20; i++)
                {
                    int d2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemTopaz);
                    Main.dust[d2].noGravity = true;
                    Main.dust[d2].velocity *= 0.5f;
                }
                Projectile.Kill();
            }
            Projectile.ai[0] += 1f;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (WorldSavingSystem.EternityMode)
                target.AddBuff(ModContent.BuffType<Buffs.Masomode.SmiteBuff>(), 60 * 3);
        }
        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 610 - Main.mouseTextColor * 2) * Projectile.Opacity * 0.9f;

        public override bool PreDraw(ref Color lightColor)
        {

            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            //draw bloom
            float bloomScale = Projectile.scale * 1.5f;
            float bloomOpacity = 1;
            Texture2D bloomTexture = FargosTextureRegistry.BloomParticleTexture.Value;
            Main.spriteBatch.Draw(bloomTexture, drawPos, null, Color.DarkGoldenrod with { A = 0 } * bloomOpacity, Projectile.rotation, bloomTexture.Size() * 0.5f, bloomScale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(bloomTexture, drawPos, null, Color.Gold with { A = 0 } * 0.4f * bloomOpacity, Projectile.rotation, bloomTexture.Size() * 0.5f, bloomScale * 0.66f, SpriteEffects.None, 0f);
            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            return false;

            /*
            for (int j = 0; j < 12; j++)
            {
                Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12f).ToRotationVector2() * 1f;
                Color glowColor = new Color(1f, 1f, 0f, 0f) * 0.7f;

                Main.spriteBatch.Draw(texture2D13, drawPos + afterimageOffset, new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(glowColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0f);
            }

            Main.EntitySpriteDraw(texture2D13, drawPos, new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
            */
        }
    }
}
