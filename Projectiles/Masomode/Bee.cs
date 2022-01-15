using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class Bee : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_566";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Bee");
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 600;
            projectile.aiStyle = -1;
        }

        public override void AI()
        {
            projectile.spriteDirection = Math.Sign(projectile.velocity.X);
            projectile.rotation = projectile.velocity.X * .1f;

            if (++projectile.frameCounter >= 3)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= 3)
                    projectile.frame = 0;
            }

            if ((projectile.wet || projectile.lavaWet) && !projectile.honeyWet) //die in liquids
            {
                projectile.Kill();
            }

            if (++projectile.localAI[0] > 30 && projectile.localAI[0] < 90)
            {
                float rotation = projectile.velocity.ToRotation();
                Vector2 vel = Main.player[(int)projectile.ai[0]].Center - projectile.Center;
                float targetAngle = vel.ToRotation();
                projectile.velocity = new Vector2(projectile.velocity.Length(), 0f).RotatedBy(rotation.AngleLerp(targetAngle, projectile.ai[1]));
            }

            projectile.tileCollide = projectile.localAI[0] > 180;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Poisoned, 300);
            target.AddBuff(mod.BuffType("Infested"), 300);
            target.AddBuff(mod.BuffType("Swarming"), 600);
        }
    }
}