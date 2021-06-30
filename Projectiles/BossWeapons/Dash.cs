using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class Dash : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = Player.defaultWidth;
            projectile.height = Player.defaultHeight;
            projectile.melee = true;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.hide = true;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().CanSplit = false;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().TimeFreezeImmune = true;

            projectile.extraUpdates = 5; //more granular movement, less likely to clip through surfaces
            projectile.timeLeft = 15 * (projectile.extraUpdates + 1);
        }

        public override bool CanDamage()
        {
            return false;
        }

        public float baseSpeed;

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            if (player.dead || !player.active)
            {
                projectile.timeLeft = 0;
                return;
            }

            if (projectile.Center.X < 50 || projectile.Center.X > Main.maxTilesX * 16 - 50
                || projectile.Center.Y < 50 || projectile.Center.Y > Main.maxTilesY * 16 - 50)
            {
                projectile.Kill();
                return;
            }

            projectile.GetGlobalProjectile<FargoGlobalProjectile>().TimeFreezeImmune = player.GetModPlayer<FargoPlayer>().StardustEnchant;

            if (player.mount.Active)
                player.mount.Dismount(player);

            player.Center = projectile.Center;
            //if (projectile.timeLeft > 1) player.position += projectile.velocity; //trying to avoid wallclipping
            player.velocity = projectile.velocity * 0.5f;

            if (projectile.velocity.X != 0)
                player.ChangeDir(projectile.velocity.X > 0 ? 1 : -1);
            player.itemRotation = (float)Math.Atan2(projectile.velocity.Y * projectile.direction, projectile.velocity.X * projectile.direction);

            player.controlLeft = false;
            player.controlRight = false;
            player.controlJump = false;
            player.controlDown = false;
            //player.controlUseItem = false;
            player.controlUseTile = false;
            player.controlHook = false;
            player.controlMount = false;

            player.itemTime = 2;
            player.itemAnimation = 2;

            if (baseSpeed == 0)
                baseSpeed = projectile.velocity.Length();
            
            if ((projectile.ai[1] != 2 || projectile.localAI[1] > 0) //dont give invul on the tick it spawns for dive
                && projectile.localAI[0] == 0) //only give invul with a full speed dash
            {
                player.immune = true;
                player.immuneTime = Math.Max(player.immuneTime, 2);
                player.hurtCooldowns[0] = Math.Max(player.hurtCooldowns[0], 2);
                player.hurtCooldowns[1] = Math.Max(player.hurtCooldowns[1], 2);
                player.immuneNoBlink = true;
            }
            player.fallStart = (int)(player.position.Y / 16f);
            player.fallStart2 = player.fallStart;

            if (projectile.velocity.Length() < baseSpeed * 0.9f)
                projectile.localAI[0] = 1; //flag for if speed dips too low for some reason, hentaispear checks this

            if (projectile.owner == Main.myPlayer)
            {
                //only run once per tick and dont do if mashing face against a wall
                if (projectile.timeLeft % projectile.MaxUpdates == 0)
                {
                    if (projectile.localAI[0] == 0) //if still at high speed, do additional projectiles
                    {
                        if (projectile.localAI[1] == 1 && projectile.ai[1] == 1) //super dash rays on spawn, 1 tick delay
                        {
                            Vector2 speed = projectile.ai[0].ToRotationVector2();
                            Projectile.NewProjectile(player.Center + speed * 1500, speed, mod.ProjectileType("HentaiSpearDeathray2"), projectile.damage, projectile.knockBack, player.whoAmI);
                            Projectile.NewProjectile(player.Center + speed * 1500, -speed, mod.ProjectileType("HentaiSpearDeathray2"), projectile.damage, projectile.knockBack, player.whoAmI);
                        }

                        if (projectile.localAI[1] > 0) //edge case, dont do this on the tick it spawns
                        {
                            if (projectile.ai[1] == 0) //regular dash trail
                            {
                                Projectile.NewProjectile(player.Center.X, player.Center.Y, 0, 0, ModContent.ProjectileType<PhantasmalSphere>(), projectile.damage, projectile.knockBack, projectile.owner);
                            }
                            else if (projectile.ai[1] == 1) //super dash trail
                            {
                                Vector2 baseVel = projectile.ai[0].ToRotationVector2().RotatedBy(Math.PI / 2);
                                Projectile.NewProjectile(player.Center, 16f * baseVel,
                                    ModContent.ProjectileType<PhantasmalSphere>(), projectile.damage, projectile.knockBack / 2, projectile.owner, 1f);
                                Projectile.NewProjectile(player.Center, 16f * -baseVel,
                                    ModContent.ProjectileType<PhantasmalSphere>(), projectile.damage, projectile.knockBack / 2, projectile.owner, 1f);
                            }
                        }
                    }

                    projectile.localAI[1]++;
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            Player player = Main.player[projectile.owner];
            player.itemAnimation = 0;
            player.itemTime = 0;

            //successful dive
            if (projectile.owner == Main.myPlayer && projectile.ai[1] == 2 && projectile.localAI[1] > 2 && timeLeft > 0)
            {
                Vector2 spawnPos = player.Center;
                spawnPos.Y -= 144 * 1.5f;
                Projectile.NewProjectile(spawnPos, Vector2.Zero, ModContent.ProjectileType<HentaiNuke>(), projectile.damage, projectile.knockBack * 10f, projectile.owner);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return projectile.ai[1] == 2; //die if vertical dive
        }
    }
}