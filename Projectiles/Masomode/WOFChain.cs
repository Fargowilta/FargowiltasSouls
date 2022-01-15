using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.NPCs;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class WOFChain : ModProjectile
    {
        public override string Texture => "Terraria/NPC_115";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("The Hungry");
            Main.projFrames[projectile.type] = Main.npcFrameCount[NPCID.TheHungry];
            /*ProjectileID.Sets.TrailCacheLength[projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;*/
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.aiStyle = -1;
            projectile.penetrate = -1;
            projectile.timeLeft = 900;
            projectile.tileCollide = false;
            projectile.hostile = true;
            //cooldownSlot = 1;

            projectile.extraUpdates = 2;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0.1f, 0.5f, 0.7f);

            if (projectile.ai[0] == 0)
            {
                projectile.ai[0] = 1;
                projectile.localAI[0] = projectile.Center.X;
                projectile.localAI[1] = projectile.Center.Y;
                Main.projectileTexture[projectile.type] = Main.npcTexture[NPCID.TheHungry];
            }

            if (projectile.velocity != Vector2.Zero && Main.rand.NextBool(3))
            {
                int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 88, projectile.velocity.X * 0.4f, projectile.velocity.Y * 0.4f, 114, default(Color), 2f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 1.8f;
                Main.dust[dust].velocity.Y -= 0.5f;
            }

            //stop moving at vertical limits of underworld
            if ((projectile.velocity.Y > 0 && projectile.Center.Y / 16 >= Main.maxTilesY)
                || (projectile.velocity.Y < 0 && projectile.Center.Y / 16 <= Main.maxTilesY - 200))
            {
                projectile.position -= projectile.velocity * 2f;
                projectile.velocity = Vector2.Zero;
            }

            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.wallBoss, NPCID.WallofFlesh)
                && Math.Abs(projectile.Center.X - Main.npc[EModeGlobalNPC.wallBoss].Center.X) < 50)
            {
                projectile.Kill(); //chain dies when wall moves over it
            }

            if (projectile.velocity != Vector2.Zero)
            {
                projectile.rotation = projectile.velocity.ToRotation();

                if (++projectile.frameCounter > 6 * (projectile.extraUpdates + 1))
                {
                    projectile.frameCounter = 0;
                    if (++projectile.frame >= Main.projFrames[projectile.type])
                        projectile.frame = 0;
                }
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (Fargowiltas.Instance.MasomodeEXLoaded)
            {
                if (!target.tongued)
                    Main.PlaySound(SoundID.ForceRoar, target.Center, -1);
                target.AddBuff(BuffID.TheTongue, 10);
            }
            target.AddBuff(BuffID.OnFire, 300);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            //spriteBatch.End(); spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);

            if (projectile.ai[0] != 0)
            {
                Texture2D texture = Main.chain12Texture;
                Vector2 position = projectile.Center;
                Vector2 mountedCenter = new Vector2(projectile.localAI[0], projectile.localAI[1]);
                Rectangle? sourceRectangle = new Rectangle?();
                Vector2 origin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
                float num1 = texture.Height;
                Vector2 vector24 = mountedCenter - position;
                float rotation = (float)Math.Atan2(vector24.Y, vector24.X) - 1.57f;
                bool flag = true;
                if (float.IsNaN(position.X) && float.IsNaN(position.Y))
                    flag = false;
                if (float.IsNaN(vector24.X) && float.IsNaN(vector24.Y))
                    flag = false;
                while (flag)
                    if (vector24.Length() < num1 + 1.0)
                    {
                        flag = false;
                    }
                    else
                    {
                        Vector2 vector21 = vector24;
                        vector21.Normalize();
                        position += vector21 * num1;
                        vector24 = mountedCenter - position;
                        Color color2 = Lighting.GetColor((int)position.X / 16, (int)(position.Y / 16.0));
                        color2 = projectile.GetAlpha(color2);
                        Main.spriteBatch.Draw(texture, position - Main.screenPosition, sourceRectangle, color2, rotation, origin, 1f, SpriteEffects.None, 0.0f);
                    }
            }

            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = projectile.GetAlpha(color26);

            SpriteEffects effects = projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            /*for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i += 2)
            {
                Color color27 = color26 * projectile.Opacity * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, effects, 0f);
            }*/

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, effects, 0f);

            //spriteBatch.End(); spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            return false;
        }
    }
}