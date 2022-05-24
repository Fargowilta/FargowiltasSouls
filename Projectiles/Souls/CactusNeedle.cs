using FargowiltasSouls.NPCs;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Souls
{
    public class CactusNeedle : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cactus Needle");
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.PineNeedleFriendly);
            Projectile.aiStyle = 336;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.timeLeft = 30;
            Projectile.tileCollide = true;

            Projectile.penetrate = 2;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().noInteractionWithNPCImmunityFrames = true;
        }

        public override void AI()
        {
            //Projectile.ai[0] += 1f;

            //if (Projectile.ai[0] >= 50f)
            //{
            //	Projectile.ai[0] = 50f;
            //	Projectile.velocity.Y += 0.5f;
            //}
            //if (Projectile.ai[0] >= 15f)
            //{
            //	Projectile.ai[0] = 15f;
            //	Projectile.velocity.Y += 0.1f;
            //}

            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + 1.57f;

            //if (Projectile.velocity.Y > 16f)
            //{
            //	Projectile.velocity.Y = 16f;
            //}
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Projectile.timeLeft = 0;

            if (Projectile.ai[0] == 1)
            {
                target.GetGlobalNPC<FargoSoulsGlobalNPC>().Needled = true;
            }
        }

        public override void Kill(int timeLeft)
        {
            int num11;
            for (int num420 = 0; num420 < 6; num420 = num11 + 1)
            {
                int num421 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 196, 0f, 0f, 0, default(Color), 1f);
                Main.dust[num421].noGravity = true;
                Main.dust[num421].scale = Projectile.scale;
                num11 = num420;
            }
        }
    }
}
