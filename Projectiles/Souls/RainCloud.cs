using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Souls
{
    public class RainCloud : ModProjectile
    {
        private int shrinkTimer = 0;
        public override string Texture => "Terraria/Projectile_238";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Rain Cloud");
            Main.projFrames[projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.RainCloudRaining);
            aiType = ProjectileID.RainCloudRaining;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().CanSplit = false;

            projectile.timeLeft = 300;

            projectile.penetrate = -1;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();

            //destroy duplicates if they somehow spawn
            if (player.ownedProjectileCounts[projectile.type] > 1
                || (projectile.owner == Main.myPlayer && (!player.GetToggleValue("Rain") || !modPlayer.RainEnchant)))
            {
                projectile.Kill();
            }
            else
            {
                projectile.timeLeft = 2;
            }

            //follow player
            float dist = Vector2.Distance(projectile.Center, player.Center);

            if (dist > 200)
            {
                Vector2 velocity = Vector2.Normalize(player.Center - projectile.Center) * player.velocity.Length();
                projectile.position += velocity;
            }
            else
            {
                projectile.velocity.Y = 0;
            }

            //always max size
            if (modPlayer.NatureForce)
            {
                projectile.scale = 3f;
                shrinkTimer = 1;
            }

            //absorb projectiles
            if (projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile proj = Main.projectile[i];

                    if (proj.active && proj.friendly && !proj.hostile && proj.owner == player.whoAmI && proj.damage > 0 && !proj.minion && proj.Hitbox.Intersects(projectile.Hitbox)
                        && proj.type != projectile.type && proj.type != ProjectileID.RainFriendly && proj.type != ModContent.ProjectileType<LightningArc>() && proj.whoAmI != Main.player[proj.owner].heldProj
                        && Array.IndexOf(FargoGlobalProjectile.noSplit, projectile.type) <= -1 && proj.type != ModContent.ProjectileType<Chlorofuck>())
                    {
                        if (projectile.scale < 3f)
                        {
                            projectile.scale *= 1.1f;
                        }
                        else
                        {
                            Vector2 rotationVector2 = (proj.Center + proj.velocity * 25) - projectile.Center;
                            rotationVector2.Normalize();

                            Vector2 vector2_3 = rotationVector2 * 10f;
                            float ai_1 = Main.rand.Next(80);
                            int p = Projectile.NewProjectile(projectile.position.X + Main.rand.NextFloat(projectile.width), projectile.position.Y + Main.rand.NextFloat(projectile.height), vector2_3.X, vector2_3.Y,
                                mod.ProjectileType("LightningArc"), proj.maxPenetrate == 1 ? proj.damage * 2 : (int)(proj.damage * 1.2), projectile.knockBack, projectile.owner,
                                rotationVector2.ToRotation(), ai_1);
                            if (p != Main.maxProjectiles)
                            {
                                Main.projectile[p].ranged = false;
                                Main.projectile[p].magic = true;
                                Main.projectile[p].usesIDStaticNPCImmunity = false;
                                Main.projectile[p].idStaticNPCHitCooldown = 0;
                                Main.projectile[p].GetGlobalProjectile<FargoGlobalProjectile>().noInteractionWithNPCImmunityFrames = false;
                                if (proj.maxPenetrate == 1)
                                    Main.projectile[p].penetrate = Main.projectile[p].maxPenetrate = 3;
                            }
                        }

                        proj.active = false;
                        shrinkTimer = 120;

                        break;
                    }
                }
            }

            //shrink over time if no projectiles absorbed
            if (shrinkTimer > 0 && projectile.scale > 1f)
            {
                shrinkTimer--;

                if (shrinkTimer == 0)
                {
                    projectile.scale *= 0.9f;
                    shrinkTimer = 10;
                }
            }

            //cancel normal rain
            projectile.ai[0] = 0;

            projectile.localAI[1]++;

            //bigger = more rain
            if (projectile.scale > 3f)
            {
                projectile.localAI[1] += 4;
            }
            else if (projectile.scale > 2f)
            {
                projectile.localAI[1] += 3;
            }
            else if (projectile.scale > 1.5f)
            {
                projectile.localAI[1] += 2;
            }
            else
            {
                projectile.localAI[1]++;
            }

            //do the rain
            if (projectile.localAI[1] >= 8)
            {
                projectile.localAI[1] = 0;
                if (projectile.owner == Main.myPlayer)
                {
                    int num414 = (int)(projectile.Center.X + (float)Main.rand.Next((int)(-20 * projectile.scale), (int)(20 * projectile.scale)));
                    int num415 = (int)(projectile.position.Y + (float)projectile.height + 4f);
                    int p = Projectile.NewProjectile((float)num414, (float)num415, 0f, 5f, ProjectileID.RainFriendly, projectile.damage / 4, 0f, projectile.owner, 0f, 0f);
                    if (p != Main.maxProjectiles)
                    {
                        Main.projectile[p].penetrate = 1;
                        Main.projectile[p].timeLeft = 45 * Main.projectile[p].MaxUpdates;
                    }
                }
            }
        }
    }
}