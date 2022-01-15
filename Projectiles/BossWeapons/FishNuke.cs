using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class FishNuke : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Fish Nuke");
            ProjectileID.Sets.Homing[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 22;
            projectile.height = 22;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.ignoreWater = true;
            projectile.extraUpdates = 1;
            //aiType = ProjectileID.Bullet;
            projectile.scale = 2f;
        }

        public override void AI()
        {
            if (projectile.ai[0] >= 0 && projectile.ai[0] < Main.maxNPCs)
            {
                int ai0 = (int)projectile.ai[0];
                if (Main.npc[ai0].CanBeChasedBy())
                {
                    double num4 = (Main.npc[ai0].Center - projectile.Center).ToRotation() - projectile.velocity.ToRotation();
                    if (num4 > Math.PI)
                        num4 -= 2.0 * Math.PI;
                    if (num4 < -1.0 * Math.PI)
                        num4 += 2.0 * Math.PI;
                    projectile.velocity = projectile.velocity.RotatedBy(num4 * 0.1f);
                }
                else
                {
                    projectile.ai[0] = -1f;
                    projectile.netUpdate = true;
                }
            }
            else
            {
                if (++projectile.localAI[1] > 12f)
                {
                    projectile.localAI[1] = 0f;
                    projectile.ai[0] = FargoSoulsUtil.FindClosestHostileNPC(projectile.Center, 500, true);
                    projectile.netUpdate = true;
                }
            }

            if (++projectile.localAI[0] >= 24f)
            {
                projectile.localAI[0] = 0f;
                const int max = 18;
                for (int index1 = 0; index1 < max; ++index1)
                {
                    Vector2 vector2 = (Vector2.UnitX * (float)-projectile.width / 2f + -Vector2.UnitY.RotatedBy((double)index1 * 2 * 3.14159274101257 / max, new Vector2()) * new Vector2(8f, 16f)).RotatedBy((double)projectile.rotation - 1.57079637050629, new Vector2());
                    int index2 = Dust.NewDust(projectile.Center, 0, 0, 135, 0.0f, 0.0f, 160, new Color(), 1f);
                    Main.dust[index2].scale = 2f;
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].position = projectile.Center + vector2 * 2f;
                    Main.dust[index2].velocity = Vector2.Normalize(projectile.Center - projectile.velocity * 3f - Main.dust[index2].position) * 1.25f;
                    //Main.dust[index2].velocity *= 2f;
                }
            }
            Vector2 vector21 = Vector2.UnitY.RotatedBy(projectile.rotation, new Vector2()) * 8f * 2;
            int index21 = Dust.NewDust(projectile.Center, 0, 0, 6, 0.0f, 0.0f, 0, new Color(), 1f);
            Main.dust[index21].position = projectile.Center + vector21;
            Main.dust[index21].scale = 1.25f;
            Main.dust[index21].noGravity = true;

            projectile.rotation = projectile.velocity.ToRotation() + (float)Math.PI / 2f;
        }

        /*public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (target.whoAmI == NPCs.FargoSoulsGlobalNPC.fishBossEX)
            {
                target.life += damage;
                if (target.life > target.lifeMax)
                    target.life = target.lifeMax;
                CombatText.NewText(target.Hitbox, CombatText.HealLife, damage);
                damage = 0;
                crit = false;
            }
        }*/

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            /*if (damage < target.lifeMax / 25)
                damage = target.lifeMax / 25;
            if (projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(projectile.Center, Vector2.Zero, mod.ProjectileType("FishNukeExplosion"),
                    damage, projectile.knockBack * 2f, projectile.owner);*/

            /*target.AddBuff(mod.BuffType("OceanicMaul"), 900);
            target.AddBuff(mod.BuffType("MutantNibble"), 900);
            target.AddBuff(mod.BuffType("CurseoftheMoon"), 900);*/
            target.AddBuff(BuffID.Frostburn, 300);
        }

        /*public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(projectile.Center, Vector2.Zero, mod.ProjectileType("FishNukeExplosion"),
                    projectile.damage, projectile.knockBack * 2f, projectile.owner);
            return true;
        }*/

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item84, projectile.Center);
            if (projectile.owner == Main.myPlayer)
            {
                int modifier = Main.rand.NextBool() ? 1 : -1;
                SpawnRazorbladeRing(6, 17f, 1f * -modifier);
                SpawnRazorbladeRing(6, 17f, 0.5f * modifier);
                /*const int max = 16;
                Vector2 baseVel = Vector2.UnitX.RotatedByRandom(2 * Math.PI);
                for (int i = 0; i < max; i++)
                {
                    float speed = i % 2 == 0 ? 16f : 14f;
                    Projectile.NewProjectile(projectile.Center, speed * baseVel.RotatedBy(2 * Math.PI / max * i),
                        ModContent.ProjectileType<RazorbladeTyphoonFriendly>(), projectile.damage / 2, projectile.knockBack, projectile.owner);
                }*/
                Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<FishNukeExplosion>(), projectile.damage / 2, projectile.knockBack * 2f, projectile.owner);
            }
            int num1 = 36;
            for (int index1 = 0; index1 < num1; ++index1)
            {
                Vector2 vector2_1 = (Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f).RotatedBy((double)(index1 - (num1 / 2 - 1)) * 6.28318548202515 / (double)num1, new Vector2()) + projectile.Center;
                Vector2 vector2_2 = vector2_1 - projectile.Center;
                int index2 = Dust.NewDust(vector2_1 + vector2_2, 0, 0, 172, vector2_2.X * 2f, vector2_2.Y * 2f, 100, new Color(), 1.4f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].noLight = true;
                Main.dust[index2].velocity = vector2_2;
            }
        }

        private void SpawnRazorbladeRing(int max, float speed, float rotationModifier)
        {
            float rotation = 2f * (float)Math.PI / max;
            Vector2 vel = Vector2.UnitX.RotatedByRandom(2 * Math.PI); //projectile.velocity; vel.Normalize();
            vel *= speed;
            int type = ModContent.ProjectileType<RazorbladeTyphoonFriendly>();
            for (int i = 0; i < max; i++)
            {
                vel = vel.RotatedBy(rotation);
                Projectile.NewProjectile(projectile.Center, vel, type, projectile.damage / 2,
                    projectile.knockBack, projectile.owner, rotationModifier, 6f);
            }
        }
    }
}