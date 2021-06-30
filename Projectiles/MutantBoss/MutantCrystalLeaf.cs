using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.MutantBoss
{
    public class MutantCrystalLeaf : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_226";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystal Leaf");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.hostile = true;
            projectile.timeLeft = 900;
            projectile.aiStyle = -1;
            projectile.scale = 2.5f;
            cooldownSlot = 1;
        }

        public override void AI()
        {
            if (++projectile.localAI[0] == 0)
            {
                for (int index1 = 0; index1 < 30; ++index1)
                {
                    int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 157, 0f, 0f, 0, new Color(), 2f);
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].velocity *= 5f;
                }
            }

            Lighting.AddLight(projectile.Center, 0.1f, 0.4f, 0.2f);
            projectile.scale = (Main.mouseTextColor / 200f - 0.35f) * 0.2f + 0.95f;
            projectile.scale *= 2.5f;

            int byUUID = FargoGlobalProjectile.GetByUUIDReal(projectile.owner, (int)projectile.ai[0], ModContent.ProjectileType<MutantMark2>());
            if (byUUID != -1)
            {
                Vector2 offset = new Vector2(125, 0).RotatedBy(projectile.ai[1]);
                projectile.Center = Main.projectile[byUUID].Center + offset;

                float modifier = 0.15f * Math.Max(0, 150 - Main.projectile[byUUID].ai[1]) / 150;
                projectile.ai[1] += modifier;
            }

            projectile.rotation = projectile.ai[1] + (float)Math.PI / 2f;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Poisoned, Main.rand.Next(60, 300));
            if (FargoSoulsWorld.MasochistMode)
            {
                target.AddBuff(mod.BuffType("Infested"), Main.rand.Next(60, 300));
                target.AddBuff(mod.BuffType("IvyVenom"), Main.rand.Next(60, 300));
                target.AddBuff(mod.BuffType("MutantFang"), 180);
            }
        }

        public override Color? GetAlpha(Color drawColor)
        {
            float num4 = Main.mouseTextColor / 200f - 0.3f;
            int num5 = (int)(byte.MaxValue * num4) + 50;
            if (num5 > byte.MaxValue)
                num5 = byte.MaxValue;
            return new Color(num5, num5, num5, 200);
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

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Color color27 = Color.White * projectile.Opacity;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, SpriteEffects.None, 0f);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}