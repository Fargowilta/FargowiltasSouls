using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    internal class BlenderProj3 : ModProjectile
    {
        public int Counter = 1;

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("The Blender");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.Homing[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.extraUpdates = 0;
            projectile.width = 19;
            projectile.height = 19;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.melee = true;
            projectile.scale = 0.75f;
            projectile.timeLeft = 120;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() - MathHelper.Pi;
            Counter++;

            const int aislotHomingCooldown = 0;
            const int homingDelay = 30;
            const float desiredFlySpeedInPixelsPerFrame = 30;
            const float amountOfFramesToLerpBy = 30; // minimum of 1, please keep in full numbers even though it's a float!

            projectile.ai[aislotHomingCooldown]++;
            if (projectile.ai[aislotHomingCooldown] > homingDelay)
            {
                projectile.ai[aislotHomingCooldown] = homingDelay; //cap this value 

                NPC n = FargoSoulsUtil.NPCExists(FargoSoulsUtil.FindClosestHostileNPC(projectile.Center, 1000));
                if (n != null)
                {
                    Vector2 desiredVelocity = projectile.DirectionTo(n.Center) * desiredFlySpeedInPixelsPerFrame;
                    projectile.velocity = Vector2.Lerp(projectile.velocity, desiredVelocity, 1f / amountOfFramesToLerpBy);
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            int proj2 = ModContent.ProjectileType<BlenderSpray>(); //374;

            /*if (projectile.owner == Main.myPlayer)
            {
                if (Main.rand.NextBool())
                {
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 10f, proj2, projectile.damage, projectile.knockBack, Main.myPlayer);
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 10f, 0f, proj2, projectile.damage, projectile.knockBack, Main.myPlayer);
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, -10f, proj2, projectile.damage, projectile.knockBack, Main.myPlayer);
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, -10f, 0f, proj2, projectile.damage, projectile.knockBack, Main.myPlayer);
                }
                else
                {
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 8f, 8f, proj2, projectile.damage, projectile.knockBack, Main.myPlayer);
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, -8f, -8f, proj2, projectile.damage, projectile.knockBack, Main.myPlayer);
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 8f, -8f, proj2, projectile.damage, projectile.knockBack, Main.myPlayer);
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, -8f, 8f, proj2, projectile.damage, projectile.knockBack, Main.myPlayer);
                }
            }*/

            for (int i = 0; i < 2; i++)
            {
                Gore.NewGore(new Vector2(projectile.position.X + (float)Main.rand.Next(projectile.width), 
                    projectile.position.Y + (float)Main.rand.Next(projectile.height)), 
                    projectile.velocity, 
                    mod.GetGoreSlot("Gores/Blender/Tentacle" + i.ToString()),
                    projectile.scale);
            }

            Vector2 velocity = Vector2.UnitY;
            velocity = velocity.RotatedByRandom(Math.PI / 4);
            for (int i = 0; i < 8; i++)
            {
                Vector2 newvel = velocity.RotatedBy(i * Math.PI / 4);
                Projectile.NewProjectile(projectile.Center, newvel * 4, mod.ProjectileType("BlenderPetal"), projectile.damage, projectile.knockBack, projectile.owner);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            //target.immune[projectile.owner] = 6;
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

            SpriteEffects effects = SpriteEffects.None;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Color color27 = color26 * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, effects, 0f);
            }

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, effects, 0f);
            return false;
        }
    }
}