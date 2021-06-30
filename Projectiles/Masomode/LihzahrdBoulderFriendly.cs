using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class LihzahrdBoulderFriendly : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_261";

        public bool spawned;
        public float vel;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Boulder");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.BoulderStaffOfEarth);
            aiType = ProjectileID.BoulderStaffOfEarth;
            projectile.penetrate = -1;
            projectile.magic = false;
            projectile.melee = true;
            projectile.timeLeft = 150;

            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 20;

            if (ModLoader.GetMod("Fargowiltas") != null)
                ModLoader.GetMod("Fargowiltas").Call("LowRenderProj", projectile);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(vel);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            vel = reader.ReadSingle();
        }

        public override void AI()
        {
            if (!spawned)
            {
                spawned = true;
                Main.PlaySound(SoundID.Item, projectile.Center, 14);

                for (int i = 0; i < 10; i++)
                {
                    int dust = Dust.NewDust(projectile.position, projectile.width,
                        projectile.height, 31, 0f, 0f, 100, default(Color), 3f);
                    Main.dust[dust].velocity *= 1.4f;
                }

                for (int i = 0; i < 5; i++)
                {
                    int dust = Dust.NewDust(projectile.position, projectile.width,
                        projectile.height, 6, 0f, 0f, 100, default(Color), 3.5f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 7f;
                    dust = Dust.NewDust(projectile.position, projectile.width,
                        projectile.height, 6, 0f, 0f, 100, default(Color), 1.5f);
                    Main.dust[dust].velocity *= 3f;
                }

                float scaleFactor9 = 0.5f;
                int gore = Gore.NewGore(new Vector2(projectile.Center.X, projectile.Center.Y), default(Vector2), Main.rand.Next(61, 64));
                Main.gore[gore].velocity *= scaleFactor9;
                Main.gore[gore].velocity.X += 1f;
                Main.gore[gore].velocity.Y += 1f;

                vel = projectile.velocity.X;
            }

            if (Math.Sign(projectile.velocity.X) == Math.Sign(vel))
                projectile.velocity.X = vel;
            else
                projectile.timeLeft = 0;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.velocity.Y != oldVelocity.Y && oldVelocity.Y > 1) //bouncy
                projectile.velocity.Y = -oldVelocity.Y * 0.8f;
            return false;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            width = 26;
            height = 26;
            return true;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Dig, (int)projectile.position.X, (int)projectile.position.Y, 1, 1f, 0.0f);
            for (int index = 0; index < 5; ++index)
                Dust.NewDust(projectile.position, projectile.width, projectile.height, 148, 0.0f, 0.0f, 0, new Color(), 1f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            SpriteEffects spriteEffects = projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Color color26 = lightColor;
            color26 = projectile.GetAlpha(color26);

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Color color27 = color26;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, spriteEffects, 0f);
            }

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, spriteEffects, 0f);
            return false;
        }
    }
}