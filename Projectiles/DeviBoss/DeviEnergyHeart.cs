using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.NPCs;

namespace FargowiltasSouls.Projectiles.DeviBoss
{
    public class DeviEnergyHeart : ModProjectile
    {   
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Energy Heart");
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.penetrate = -1;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.aiStyle = -1;
            cooldownSlot = 1;

            projectile.alpha = 150;
            projectile.timeLeft = 90;

            projectile.GetGlobalProjectile<FargoGlobalProjectile>().DeletionImmuneRank = 1;
        }

        public override bool CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = 1;
                Main.PlaySound(SoundID.Item44, projectile.Center);
            }

            // Fade into 50 alpha from 150
            if (projectile.alpha >= 60)
                projectile.alpha -= 10;

            projectile.rotation = projectile.ai[0];
            projectile.scale += 0.01f;

            float speed = projectile.velocity.Length();
            speed += projectile.ai[1];
            projectile.velocity = Vector2.Normalize(projectile.velocity) * speed;
        }

        public override void Kill(int timeLeft)
        {
            FargoSoulsUtil.HeartDust(projectile.Center, projectile.rotation + MathHelper.PiOver2);

            /*for (int i = 0; i < 10; i++)
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 86, 0f, 0f, 0, default(Color), 2f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 8f;
            }*/

            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.deviBoss, mod.NPCType("DeviBoss")))
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Projectile.NewProjectile(projectile.Center, Vector2.UnitX.RotatedBy(projectile.rotation + (float)Math.PI / 2 * i),
                            mod.ProjectileType("DeviDeathray"), projectile.damage, projectile.knockBack, projectile.owner);
                    }
                }
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(mod.BuffType("Lovestruck"), 240);
        }

        public override Color? GetAlpha(Color lightColor) => Color.White * projectile.Opacity;
    }
}