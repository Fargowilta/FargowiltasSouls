using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class Hungry2 : Hungry
    {
        public override string Texture => "FargowiltasSouls/Projectiles/BossWeapons/Hungry";

        int baseWidth;
        int baseHeight;

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Hungry");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.Homing[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            projectile.minion = false;
            projectile.magic = true;
            projectile.aiStyle = -1;
            projectile.penetrate = -1;

            projectile.tileCollide = false;
            projectile.ignoreWater = true;

            baseWidth = projectile.width;
            baseHeight = projectile.height;

            projectile.GetGlobalProjectile<FargoGlobalProjectile>().CanSplit = false;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().DeletionImmuneRank = 2;
        }

        public override bool CanDamage()
        {
            return projectile.ai[0] != 0;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0) //override tungsten
            {
                projectile.localAI[0] = 1;
                projectile.scale = 1f;
            }

            //dust!
            int dustId = Dust.NewDust(projectile.position, projectile.width, projectile.height, 60, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 100, default(Color), 2f);
            Main.dust[dustId].noGravity = true;
            int dustId3 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 60, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 100, default(Color), 2f);
            Main.dust[dustId3].noGravity = true;

            if (projectile.ai[0] == 0)
            {
                Player player = Main.player[projectile.owner];
                if (player.active && !player.dead && player.channel && player.HeldItem.type == ModContent.ItemType<Items.Weapons.SwarmDrops.FleshCannon>() && player.CheckMana(player.HeldItem.mana))
                {
                    projectile.damage = player.GetWeaponDamage(player.HeldItem);
                    projectile.knockBack = player.GetWeaponKnockback(player.HeldItem, player.HeldItem.knockBack);
                }
                else
                {
                    Main.PlaySound(new LegacySoundStyle(4, 13), projectile.Center);
                    projectile.ai[0] = 1;
                    projectile.penetrate = 1;
                    projectile.maxPenetrate = 1;
                    projectile.netUpdate = true;
                    return;
                }

                if (projectile.scale < 5f)
                {
                    projectile.scale *= 1.008f;

                    if (projectile.scale >= 5f) //dust indicates full charge
                    {
                        for (int i = 0; i < 42; i++)
                        {
                            Vector2 vector6 = Vector2.UnitY * 18f;
                            vector6 = vector6.RotatedBy((i - (36 / 2 - 1)) * 6.28318548f / 42) + projectile.Center;
                            Vector2 vector7 = vector6 - projectile.Center;
                            int d = Dust.NewDust(vector6 + vector7, 0, 0, DustID.Fire, 0f, 0f, 0, default, 5f);
                            Main.dust[d].noGravity = true;
                            Main.dust[d].scale = 5f;
                            Main.dust[d].velocity = vector7;
                        }
                    }
                }

                projectile.rotation = player.itemRotation;
                if (player.direction < 0)
                    projectile.rotation += (float)Math.PI;
                projectile.velocity = projectile.rotation.ToRotationVector2() * projectile.velocity.Length();
                float offset = 60f + 40f * projectile.scale / 5f;
                projectile.Center = player.Center + offset * player.HeldItem.scale * Vector2.UnitX.RotatedBy(projectile.rotation);
                projectile.position -= projectile.velocity;

                projectile.timeLeft = 240;
            }
            else
            {
                if (!projectile.tileCollide && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
                    projectile.tileCollide = true;
                projectile.ignoreWater = false;

                const int aislotHomingCooldown = 0;
                const int homingDelay = 5;
                const float desiredFlySpeedInPixelsPerFrame = 60;
                const float amountOfFramesToLerpBy = 10; // minimum of 1, please keep in full numbers even though it's a float!

                projectile.ai[aislotHomingCooldown]++;
                if (projectile.ai[aislotHomingCooldown] > homingDelay)
                {
                    projectile.ai[aislotHomingCooldown] = homingDelay; //cap this value 

                    NPC n = FargoSoulsUtil.NPCExists(FargoSoulsUtil.FindClosestHostileNPC(projectile.Center, 600, true));
                    if (n != null)
                    {
                        Vector2 desiredVelocity = projectile.DirectionTo(n.Center) * desiredFlySpeedInPixelsPerFrame;
                        projectile.velocity = Vector2.Lerp(projectile.velocity, desiredVelocity, 1f / amountOfFramesToLerpBy);
                    }
                    else if (projectile.timeLeft > 120)
                    {
                        projectile.timeLeft = 120;
                    }
                }
                /*else
                {
                    Main.NewText(amountOfFramesToLerpBy.ToString());
                    //Main.NewText(Main.player[projectile.owner].numMinions.ToString() + " " + Main.player[projectile.owner].maxMinions.ToString() + " " + Main.player[projectile.owner].slotsMinions.ToString());
                }*/

                projectile.rotation = projectile.velocity.ToRotation();
            }

            projectile.position = projectile.Center;
            projectile.width = (int)(baseWidth * projectile.scale);
            projectile.height = (int)(baseHeight * projectile.scale);
            projectile.Center = projectile.position;

            projectile.rotation += (float)Math.PI / 2;

            projectile.damage = (int)(projectile.ai[1] * projectile.scale);
            if (projectile.scale < 5f / 2f)
                projectile.damage /= 2;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (projectile.scale >= 5)
            {
                damage = (int)(damage * 1.5);
                crit = true;
            }
        }

        public override void Kill(int timeleft)
        {
            if (projectile.scale >= 5f)
            {
                for (int i = 0; i < 40; i++)
                {
                    int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire, 0f, 0f, 100, default, Main.rand.NextFloat(3f, 6f));
                    if (Main.rand.NextBool(3))
                        Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= Main.rand.NextFloat(12f, 24f);
                    Main.dust[d].position = projectile.Center;
                }
            }

            Main.PlaySound(SoundID.Item, (int)projectile.position.X, (int)projectile.position.Y, 14);

            for (int i = 0; i < 30; i++)
            {
                int dust = Dust.NewDust(projectile.position, projectile.width,
                    projectile.height, 31, 0f, 0f, 100, default(Color), 3f);
                Main.dust[dust].velocity *= 1.4f;
            }

            for (int i = 0; i < 20; i++)
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
            for (int j = 0; j < 4; j++)
            {
                int gore = Gore.NewGore(new Vector2(projectile.Center.X, projectile.Center.Y),
                    default(Vector2),
                    Main.rand.Next(61, 64));

                Main.gore[gore].velocity *= scaleFactor9;
                Main.gore[gore].velocity.X += 1f;
                Main.gore[gore].velocity.Y += 1f;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.velocity.X != oldVelocity.X)
                projectile.velocity.X = -oldVelocity.X;
            if (projectile.velocity.Y != oldVelocity.Y)
                projectile.velocity.Y = -oldVelocity.Y;

            return false;
        }
    }
}