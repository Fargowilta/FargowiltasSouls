using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class Dash : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = Player.defaultWidth;
            Projectile.height = Player.defaultHeight;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.hide = true;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().CanSplit = false;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().TimeFreezeImmune = true;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().DeletionImmuneRank = 2;

            Projectile.extraUpdates = 5; //more granular movement, less likely to clip through surfaces
            Projectile.timeLeft = 15 * (Projectile.extraUpdates + 1);
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public float baseSpeed;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.dead || !player.active)
            {
                Projectile.timeLeft = 0;
                return;
            }

            if (Projectile.Center.X < 50 || Projectile.Center.X > Main.maxTilesX * 16 - 50
                || Projectile.Center.Y < 50 || Projectile.Center.Y > Main.maxTilesY * 16 - 50)
            {
                Projectile.Kill();
                return;
            }

            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().TimeFreezeImmune = player.GetModPlayer<FargoSoulsPlayer>().StardustEnchantActive;

            if (player.mount.Active)
                player.mount.Dismount(player);

            player.Center = Projectile.Center;
            //if (Projectile.timeLeft > 1) player.position += Projectile.velocity; //trying to avoid wallclipping
            player.velocity = Projectile.velocity * 0.5f;

            if (Projectile.velocity.X != 0)
                player.ChangeDir(Projectile.velocity.X > 0 ? 1 : -1);
            player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);

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

            player.GetModPlayer<FargoSoulsPlayer>().IsDashingTimer = 0;

            if (baseSpeed == 0 && Projectile.velocity != Vector2.Zero)
            {
                baseSpeed = Projectile.velocity.Length();

                if (Projectile.owner == Main.myPlayer) //delete hook
                {
                    foreach (Projectile hook in Main.projectile.Where(p => p.active && p.owner == Projectile.owner && p.aiStyle == 7))
                    {
                        hook.Kill();
                    }
                }
            }

            if ((Projectile.ai[1] != 2 || Projectile.localAI[1] > 0) //dont give invul on the tick it spawns for dive
                && Projectile.localAI[0] == 0) //only give invul with a full speed dash
            {
                player.immune = true;
                player.immuneTime = Math.Max(player.immuneTime, 2);
                player.hurtCooldowns[0] = Math.Max(player.hurtCooldowns[0], 2);
                player.hurtCooldowns[1] = Math.Max(player.hurtCooldowns[1], 2);
                player.immuneNoBlink = true;
            }
            player.fallStart = (int)(player.position.Y / 16f);
            player.fallStart2 = player.fallStart;

            if (Projectile.velocity.Length() < baseSpeed * 0.9f)
                Projectile.localAI[0] = 1; //flag for if speed dips too low for some reason, hentaispear checks this

            if (Projectile.owner == Main.myPlayer)
            {
                //only run once per tick and dont do if mashing face against a wall
                if (Projectile.timeLeft % Projectile.MaxUpdates == 0)
                {
                    if (Projectile.localAI[0] == 0) //if still at high speed, do additional Projectiles
                    {
                        if (Projectile.localAI[1] == 1 && Projectile.ai[1] == 1) //super dash rays on spawn, 1 tick delay
                        {
                            Vector2 speed = Projectile.ai[0].ToRotationVector2();
                            Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), player.Center + speed * 1500, speed, ModContent.ProjectileType<HentaiSpearDeathray2>(), Projectile.damage, Projectile.knockBack, player.whoAmI);
                            Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), player.Center + speed * 1500, -speed, ModContent.ProjectileType<HentaiSpearDeathray2>(), Projectile.damage, Projectile.knockBack, player.whoAmI);
                        }

                        if (Projectile.localAI[1] > 0) //edge case, dont do this on the tick it spawns
                        {
                            if (Projectile.ai[1] == 0) //regular dash trail
                            {
                                Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), player.Center.X, player.Center.Y, 0, 0, ModContent.ProjectileType<PhantasmalSphere>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                            }
                            else if (Projectile.ai[1] == 1) //super dash trail
                            {
                                Vector2 baseVel = Projectile.ai[0].ToRotationVector2().RotatedBy(Math.PI / 2);
                                Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), player.Center, 16f * baseVel,
                                    ModContent.ProjectileType<PhantasmalSphere>(), Projectile.damage, Projectile.knockBack / 2, Projectile.owner, 1f);
                                Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), player.Center, 16f * -baseVel,
                                    ModContent.ProjectileType<PhantasmalSphere>(), Projectile.damage, Projectile.knockBack / 2, Projectile.owner, 1f);
                            }
                        }
                    }

                    Projectile.localAI[1]++;
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            player.itemAnimation = 0;
            player.itemTime = 0;

            //successful dive
            if (Projectile.owner == Main.myPlayer && Projectile.ai[1] == 2 && Projectile.localAI[1] > 2 && timeLeft > 0)
            {
                Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), player.Center, Vector2.Zero, ModContent.ProjectileType<HentaiNuke>(), Projectile.damage, Projectile.knockBack * 10f, Projectile.owner);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return Projectile.ai[1] == 2; //die if vertical dive
        }
    }
}