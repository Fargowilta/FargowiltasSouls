using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class LeashFlail : ModProjectile
    {
        private bool eyeSpawn = true;

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Leash Flail");
        }

        public override void SetDefaults()
        {
            projectile.width = 58;
            projectile.height = 52;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.melee = true;
        }

        public override void AI()
        {
            if (projectile.timeLeft == 120) projectile.ai[0] = 1f;

            if (Main.player[projectile.owner].dead)
            {
                projectile.Kill();
                return;
            }

            if (projectile.alpha == 0)
            {
                if (projectile.position.X + projectile.width / 2 > Main.player[projectile.owner].position.X + Main.player[projectile.owner].width / 2)
                    Main.player[projectile.owner].ChangeDir(1);
                else
                    Main.player[projectile.owner].ChangeDir(-1);
            }

            Vector2 vector14 = new Vector2(projectile.position.X + projectile.width * 0.5f, projectile.position.Y + projectile.height * 0.5f);
            float num166 = Main.player[projectile.owner].position.X + Main.player[projectile.owner].width / 2 - vector14.X;
            float num167 = Main.player[projectile.owner].position.Y + Main.player[projectile.owner].height / 2 - vector14.Y;
            float distance = (float)Math.Sqrt(num166 * num166 + num167 * num167);
            if (projectile.ai[0] == 0f)
            {
                if (distance > 300f) projectile.ai[0] = 1f;
                projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + 1.57f;
                projectile.ai[1] += 1f;
                if (projectile.ai[1] > 8f) projectile.ai[1] = 8f;
                if (projectile.velocity.X < 0f)
                    projectile.spriteDirection = -1;
                else
                    projectile.spriteDirection = 1;
            }
            //plz retract sir
            else if (projectile.ai[0] == 1f)
            {
                projectile.tileCollide = false;
                projectile.rotation = (float)Math.Atan2(num167, num166) - 1.57f;
                float num169 = 30f;

                if (distance < 50f) projectile.Kill();
                distance = num169 / distance;
                num166 *= distance;
                num167 *= distance;
                projectile.velocity.X = num166 * 2;
                projectile.velocity.Y = num167 * 2;
                if (projectile.velocity.X < 0f)
                    projectile.spriteDirection = 1;
                else
                    projectile.spriteDirection = -1;
            }

            //Spew eye
            /*if ((int)projectile.ai[0] == 1f && projectile.owner == Main.myPlayer && eyeSpawn)
            {
                Vector2 vector54 = Main.player[projectile.owner].Center - projectile.Center;
                Vector2 vector55 = vector54 * -1f;
                vector55.Normalize();
                vector55 *= Main.rand.Next(45, 65) * 0.1f;
                vector55 = vector55.RotatedBy((Main.rand.NextDouble() - 0.5) * 1.5707963705062866);
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, vector55.X, vector55.Y, mod.ProjectileType("EyeProjectile"), projectile.damage, projectile.knockBack,
                    projectile.owner, -10f);

                eyeSpawn = false;
            }*/
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (projectile.ai[0] != 1f)
            {
                int dist = 1000;

                for (int i = 0; i < 2; i++)
                {
                    Vector2 offset = new Vector2();
                    double angle = Main.rand.NextDouble() * 2d * Math.PI;
                    offset.X += (float)(Math.Sin(angle) * dist);
                    offset.Y += (float)(Math.Cos(angle) * dist);

                    Vector2 position = target.Center + offset - new Vector2(4, 4);
                    Vector2 velocity = Vector2.Normalize(target.Center - position) * 25;

                    int p = Projectile.NewProjectile(position, velocity,
                        ModContent.ProjectileType<EyeProjectile>(), projectile.damage, projectile.knockBack, projectile.owner, -10f);

                    if (p != Main.maxProjectiles)
                    {
                        Main.projectile[p].tileCollide = false;
                    }
                }
            }

            //retract
            projectile.ai[0] = 1f;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            //smaller tile hitbox
            width = 25;
            height = 25;
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            //retract
            projectile.ai[0] = 1f;
            return false;
        }


        // chain voodoo
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = ModContent.GetTexture("FargowiltasSouls/Projectiles/BossWeapons/LeashFlailChain");

            Vector2 position = projectile.Center;
            Vector2 mountedCenter = Main.player[projectile.owner].MountedCenter;
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

            return true;
        }
    }
}