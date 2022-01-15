using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    internal class BlenderYoyoProj : ModProjectile
    {
        public bool yoyosSpawned = false;

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("The Blender");
            // Vanilla values range from 3f(Wood) to 16f(Chik), and defaults to -1f. Leaving as -1 will make the time infinite.
            ProjectileID.Sets.YoyosLifeTimeMultiplier[projectile.type] = -1f;
            // Vanilla values range from 130f(Wood) to 400f(Terrarian), and defaults to 200f
            ProjectileID.Sets.YoyosMaximumRange[projectile.type] = 750f;
            // Vanilla values range from 9f(Wood) to 17.5f(Terrarian), and defaults to 10f
            ProjectileID.Sets.YoyosTopSpeed[projectile.type] = 25f;

            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.Kraken);
            projectile.width = 46;
            projectile.height = 46;
            //yoyo ai
            projectile.aiStyle = 99;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.melee = true;
            projectile.scale = 1f;

            projectile.extraUpdates = 1;

            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 15;
        }

        int soundtimer;
        public override void AI()
        {
            if (!yoyosSpawned && projectile.owner == Main.myPlayer)
            {
                int maxYoyos = 5;
                for (int i = 0; i < maxYoyos; i++)
                {
                    float radians = (360f / (float)maxYoyos) * i * (float)(Math.PI / 180);
                    Projectile yoyo = FargoSoulsUtil.NewProjectileDirectSafe(projectile.Center, Vector2.Zero,
                        ModContent.ProjectileType<BlenderOrbital>(), projectile.damage, projectile.knockBack, projectile.owner, i, radians);
                    yoyo.localAI[0] = projectile.identity;
                }

                yoyosSpawned = true;
            }

            if (soundtimer > 0)
                soundtimer--;

            if (Main.player[projectile.owner].HeldItem.type == ModContent.ItemType<Items.Weapons.SwarmDrops.Blender>())
            {
                projectile.damage = Main.player[projectile.owner].GetWeaponDamage(Main.player[projectile.owner].HeldItem);
                projectile.knockBack = Main.player[projectile.owner].GetWeaponKnockback(Main.player[projectile.owner].HeldItem, Main.player[projectile.owner].HeldItem.knockBack);
            }
        }

        public override void PostAI()
        {
            int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, Main.rand.NextBool() ? 107 : 157);
            Main.dust[d].noGravity = true;
            Main.dust[d].velocity *= 0.2f;
            Main.dust[d].velocity += projectile.velocity * 0.8f;
            Main.dust[d].scale = 1.5f;
        }

        int hitcounter;
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            //target.immune[projectile.owner] = 6;

            if (projectile.owner == Main.myPlayer)
            {
                Player player = Main.player[projectile.owner];
                hitcounter++;
                if (player.ownedProjectileCounts[ProjectileID.BlackCounterweight] < 5)
                {
                    Projectile.NewProjectile(player.Center, Main.rand.NextVector2Circular(10, 10), ProjectileID.BlackCounterweight, projectile.damage, projectile.knockBack, projectile.owner);
                }
                if (hitcounter % 5 == 0)
                {
                    Vector2 velocity = Vector2.UnitY;
                    velocity = velocity.RotatedByRandom(Math.PI / 4);
                    for (int i = 0; i < 8; i++)
                    {
                        Vector2 newvel = velocity.RotatedBy(i * Math.PI / 4);
                        Projectile.NewProjectile(projectile.Center, newvel * 8, ModContent.ProjectileType<BlenderPetal>(), projectile.damage, projectile.knockBack, projectile.owner);
                    }
                }
            }

            if(soundtimer == 0)
            {
                soundtimer = 15;
                Main.PlaySound(SoundID.Item, (int)projectile.Center.X, (int)projectile.Center.Y, 22, 1.5f, 1f);
            }
        }

        //reduce tile hitbox
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            width = 30;
            height = 30;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough);
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

            SpriteEffects effects = projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Color color27 = color26 * 0.75f;
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