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
        public override string Texture => "Terraria/Images/Projectile_238";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rain Cloud");
            Main.projFrames[Projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.RainCloudRaining);
            AIType = ProjectileID.RainCloudRaining;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().CanSplit = false;

            Projectile.timeLeft = 300;

            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            //destroy duplicates if they somehow spawn
            if (player.ownedProjectileCounts[Projectile.type] > 1
                || (Projectile.owner == Main.myPlayer && (!player.GetToggleValue("Rain") || !modPlayer.RainEnchantActive)))
            {
                Projectile.Kill();
            }
            else
            {
                Projectile.timeLeft = 2;
            }

            //follow cursor
            if (player == Main.LocalPlayer)
            {
                Vector2 mouse = Main.MouseWorld;

                Projectile.Center = new Vector2(mouse.X, mouse.Y - 30);
            }

            //always max size in force
            if (modPlayer.NatureForce)
            {
                Projectile.scale = 3f;
                shrinkTimer = 1;
            }

            ////absorb Projectiles
            //if (Projectile.owner == Main.myPlayer)
            //{
            //    for (int i = 0; i < Main.maxProjectiles; i++)
            //    {
            //        Projectile proj = Main.projectile[i];

            //        if (proj.active && proj.friendly && !proj.hostile && proj.owner == player.whoAmI && proj.damage > 0 && FargoSoulsUtil.CanDeleteProjectile(proj) && !FargoSoulsUtil.IsSummonDamage(proj, false) && proj.Hitbox.Intersects(Projectile.Hitbox)
            //            && proj.type != Projectile.type && proj.type != ProjectileID.RainFriendly && proj.type != ModContent.ProjectileType<LightningArc>() && proj.whoAmI != Main.player[proj.owner].heldProj
            //            && Array.IndexOf(FargoSoulsGlobalProjectile.noSplit, Projectile.type) <= -1 && proj.type != ModContent.ProjectileType<Chlorofuck>())
            //        {
            //            if (Projectile.scale < 3f)
            //            {
            //                Projectile.scale *= 1.1f;
            //            }
            //            else
            //            {
            //                Vector2 rotationVector2 = (proj.Center + proj.velocity * 25) - Projectile.Center;
            //                rotationVector2.Normalize();

            //                Vector2 vector2_3 = rotationVector2 * 10f;
            //                float ai_1 = Main.rand.Next(80);
            //                int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + Main.rand.NextFloat(Projectile.width), Projectile.position.Y + Main.rand.NextFloat(Projectile.height), vector2_3.X, vector2_3.Y,
            //                    ModContent.ProjectileType<LightningArc>(), proj.maxPenetrate == 1 ? proj.damage * 2 : (int)(proj.damage * 1.2), Projectile.knockBack, Projectile.owner,
            //                    rotationVector2.ToRotation(), ai_1);
            //                if (p != Main.maxProjectiles)
            //                {
            //                    Main.projectile[p].DamageType = DamageClass.Magic;
            //                    Main.projectile[p].usesIDStaticNPCImmunity = false;
            //                    Main.projectile[p].idStaticNPCHitCooldown = 0;
            //                    Main.projectile[p].GetGlobalProjectile<FargoSoulsGlobalProjectile>().noInteractionWithNPCImmunityFrames = false;
            //                    if (proj.maxPenetrate == 1)
            //                        Main.projectile[p].penetrate = Main.projectile[p].maxPenetrate = 3;
            //                }
            //            }

            //            proj.active = false;
            //            shrinkTimer = 120;

            //            break;
            //        }
            //    }
            //}

            //shrink over time if no Projectiles absorbed
            //if (shrinkTimer > 0 && Projectile.scale > 1f)
            //{
            //    shrinkTimer--;

            //    if (shrinkTimer == 0)
            //    {
            //        Projectile.scale *= 0.9f;
            //        shrinkTimer = 10;
            //    }
            //}

            //cancel normal rain
            Projectile.ai[0] = 0;

            Projectile.localAI[1]++;

            //bigger = more rain
            if (Projectile.scale > 3f)
            {
                Projectile.localAI[1] += 4;
            }
            else if (Projectile.scale > 2f)
            {
                Projectile.localAI[1] += 3;
            }
            else if (Projectile.scale > 1.5f)
            {
                Projectile.localAI[1] += 2;
            }
            else
            {
                Projectile.localAI[1]++;
            }

            //only attack when not in tiles 
            if (Collision.CanHitLine(player.Center, 2, 2, Projectile.Center, 2, 2 ))
            {
                //do the rain
                if (Projectile.localAI[1] >= 8)
                {
                    Projectile.localAI[1] = 0;
                    if (Projectile.owner == Main.myPlayer)
                    {
                        int num414 = (int)(Projectile.Center.X + (float)Main.rand.Next((int)(-20 * Projectile.scale), (int)(20 * Projectile.scale)));
                        int num415 = (int)(Projectile.position.Y + (float)Projectile.height + 4f);
                        int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), (float)num414, (float)num415, 0f, 5f, ProjectileID.RainFriendly, Projectile.damage / 4, 0f, Projectile.owner, 0f, 0);
                        if (p != Main.maxProjectiles)
                        {
                            Main.projectile[p].penetrate = 1;
                            Main.projectile[p].timeLeft = 45 * Main.projectile[p].MaxUpdates;
                        }

                        //lightning
                        if (Main.rand.Next(10) == 0)
                        {
                            Vector2 rotationVector2 = new Vector2(Main.rand.NextFloat(-2, 2), 5);
                            rotationVector2.Normalize();
                            Vector2 vector2_3 = rotationVector2 * 10f;
                            float ai_1 = Main.rand.Next(80);

                            p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + Main.rand.NextFloat(Projectile.width), Projectile.Center.Y + Projectile.height / 2, vector2_3.X, vector2_3.Y,
                                    ModContent.ProjectileType<LightningArc>(), Projectile.damage, Projectile.knockBack, Projectile.owner,
                                    rotationVector2.ToRotation(), ai_1);

                            if (p != Main.maxProjectiles)
                            {
                                Main.projectile[p].DamageType = DamageClass.Magic;
                                Main.projectile[p].usesIDStaticNPCImmunity = false;
                                Main.projectile[p].idStaticNPCHitCooldown = 0;
                                Main.projectile[p].GetGlobalProjectile<FargoSoulsGlobalProjectile>().noInteractionWithNPCImmunityFrames = false;
                                //if (proj.maxPenetrate == 1)
                                //    Main.projectile[p].penetrate = Main.projectile[p].maxPenetrate = 3;
                            }
                        }
                    }
                }
            }

            
        }
    }
}