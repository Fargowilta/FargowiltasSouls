using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Souls
{
    public class IronParry : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Iron Parry");
            Main.projFrames[Projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            Projectile.width = 78;
            Projectile.height = 78;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.scale = 2f;
        }

        public override void AI()
        {
            Projectile.light = Projectile.ai[0] != 1 ? 1 : 0; // only give light on perfect parry

            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;
                Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                SoundEngine.PlaySound(SoundID.NPCHit4, Projectile.Center);

                for (int i = 0; i < 20; i++)
                {
                    int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemTopaz, 0f, 0f, 0, new Color(255, 255, 255, 150), 1.5f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 3f;
                }
            }

            //Projectile.Center = Main.player[Projectile.owner].Center + new Vector2(Projectile.ai[0], Projectile.ai[1]);

            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame--;
                    Projectile.Kill();
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 150);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            int frameWidth = texture.Width / 2;
            int frameHeight = texture.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            bool imperfectSheet = Projectile.ai[0] == 0; // imperfect parry, use second sheet
            int frameX = imperfectSheet ? frameWidth : 0;
            int frameY = frameHeight * Projectile.frame;
            Rectangle rectangle = new(frameX, frameY, frameWidth, frameHeight);

            Vector2 origin = rectangle.Size() / 2f;
            SpriteEffects effects = Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            float scale = Projectile.scale;
            if (!imperfectSheet)
            {
                //Main.spriteBatch.End();
                //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                scale *= 1.5f;
            }

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), rectangle, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, scale, effects, 0);

            if (!imperfectSheet)
            {
                //Main.spriteBatch.End();
                //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            }
            return false;
        }
    }
}

