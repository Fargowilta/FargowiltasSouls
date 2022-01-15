using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class BrainProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Brain Proj");
            Main.projFrames[projectile.type] = 11;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.Homing[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }
        public override void SetDefaults()
        {
            projectile.width = 74;
            projectile.height = 70;
            projectile.netImportant = true;
            projectile.friendly = true;
            //projectile.minionSlots = 1f;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.minion = true;
            projectile.tileCollide = false;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().DeletionImmuneRank = 2;
        }

        public override bool CanDamage() => false;

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();
            if (player.dead) modPlayer.BrainMinion = false;
            if (modPlayer.BrainMinion) projectile.timeLeft = 2;

            projectile.frameCounter++;
            if (projectile.frameCounter >= 8)
            {
                projectile.frameCounter = 0;
                projectile.frame = (projectile.frame + 1) % 11;
            }

            projectile.velocity = Vector2.Lerp(projectile.velocity, projectile.DirectionTo(player.Center), 0.05f);

            projectile.ai[0]++;
            projectile.alpha = (int)(Math.Cos(projectile.ai[0] * MathHelper.TwoPi / 180) * 122.5 + 122.5);
            if(projectile.ai[0] == 180)
            {

                projectile.Center = player.Center + Main.rand.NextVector2CircularEdge(300, 300);
                projectile.velocity = projectile.DirectionTo(player.Center) * 8;
                projectile.netUpdate = true;
                projectile.ai[0] = 0;
            }    
        }
    }
}