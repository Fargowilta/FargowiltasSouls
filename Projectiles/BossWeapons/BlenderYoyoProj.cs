using Microsoft.Xna.Framework;
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
            DisplayName.SetDefault("The Blender");
            // Vanilla values range from 3f(Wood) to 16f(Chik), and defaults to -1f. Leaving as -1 will make the time infinite.
            ProjectileID.Sets.YoyosLifeTimeMultiplier[projectile.type] = -1f;
            // Vanilla values range from 130f(Wood) to 400f(Terrarian), and defaults to 200f
            ProjectileID.Sets.YoyosMaximumRange[projectile.type] = 750f;
            // Vanilla values range from 9f(Wood) to 17.5f(Terrarian), and defaults to 10f
            ProjectileID.Sets.YoyosTopSpeed[projectile.type] = 25f;
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
                    Projectile yoyo = FargoGlobalProjectile.NewProjectileDirectSafe(projectile.Center, Vector2.Zero, ModContent.ProjectileType<BlenderOrbital>(), projectile.damage, projectile.knockBack, projectile.owner, 5, radians);
                    yoyo.localAI[0] = projectile.whoAmI;
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
            /*if (Main.rand.Next(2) == 0)
            {
                int dustIndex = Dust.NewDust(projectile.position, projectile.width, projectile.height, 16, 0f, 0f, 0, default(Color), 1f);
                Main.dust[dustIndex].noGravity = true;
                Main.dust[dustIndex].scale = 1.6f;
            }*/
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return (projectile.Distance(targetHitbox.Center()) <= 70);
        }
        int hitcounter;
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            //target.immune[projectile.owner] = 6;
            Player player = Main.player[projectile.owner];
            hitcounter++;
            if (player.ownedProjectileCounts[ProjectileID.BlackCounterweight] < 5)
            {
                Projectile.NewProjectile(player.Center, Main.rand.NextVector2Circular(10, 10), ProjectileID.BlackCounterweight, projectile.damage, projectile.knockBack, projectile.owner);
            }
            if(hitcounter % 5 == 0)
            {
                Vector2 velocity = Vector2.UnitY;
                velocity = velocity.RotatedByRandom(Math.PI / 4);
                for (int i = 0; i < 8; i++)
                {
                    Vector2 newvel = velocity.RotatedBy(i * Math.PI / 4);
                    Projectile.NewProjectile(projectile.Center, newvel * 8, ModContent.ProjectileType<BlenderPetal>(), projectile.damage, projectile.knockBack, projectile.owner);
                }
            }
            if(soundtimer == 0)
            {
                soundtimer = 15;
                Main.PlaySound(SoundID.Item, (int)projectile.Center.X, (int)projectile.Center.Y, 22, 1.5f, 1f);
            }
        }
    }
}