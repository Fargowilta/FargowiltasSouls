using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.NPCs;
using FargowiltasSouls.Toggler;

namespace FargowiltasSouls.Projectiles.Souls
{
    public class Snowstorm : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Snowstorm");
        }

        public override string Texture => "FargowiltasSouls/Projectiles/Empty";

        public override void SetDefaults()
        {
            projectile.aiStyle = -1;

            projectile.penetrate = -1;
            projectile.timeLeft = 2;
            projectile.width = 1;
            projectile.height = 1;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();

            projectile.timeLeft++;

            if (player.dead || !player.active || !modPlayer.SnowEnchant)
                projectile.Kill();

            if (player == Main.LocalPlayer)
            {
                projectile.Center = Main.MouseWorld;

                if (!player.GetToggleValue("Snow"))
                    projectile.Kill();
            }

            //dust
            int dist = 50;

            if (modPlayer.NatureForce)
            {
                dist = 100;
            }


            for (int i = 0; i < 15; i++)
            {
                Vector2 offset = new Vector2();
                double angle = Main.rand.NextDouble() * 2d * Math.PI;
                offset.X += (float)(Math.Sin(angle) * Main.rand.Next(dist + 1));
                offset.Y += (float)(Math.Cos(angle) * Main.rand.Next(dist + 1));
                Dust dust = Main.dust[Dust.NewDust(
                    projectile.Center + offset - new Vector2(4, 4), 0, 0,
                    76, 0, 0, 100, Color.White, .75f)];

                dust.noGravity = true;
            }

            //for (int i = 0; i < 20; i++)
            //{
            //    Vector2 offset = new Vector2();
            //    double angle = Main.rand.NextDouble() * 2d * Math.PI;
            //    offset.X += (float)(Math.Sin(angle) * dist);
            //    offset.Y += (float)(Math.Cos(angle) * dist);
            //    if (!Collision.SolidCollision(projectile.Center + offset - new Vector2(4, 4), 0, 0))
            //    {
            //        Dust dust = Main.dust[Dust.NewDust(
            //          projectile.Center + offset - new Vector2(4, 4), 0, 0,
            //          76, 0, 0, 100, Color.White, 1f
            //          )];
            //        dust.velocity = projectile.velocity;
            //        dust.noGravity = true;
            //    }
            //}



            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                

                if (proj.active && proj.hostile && proj.damage > 0 && projectile.Distance(FargoSoulsUtil.ClosestPointInHitbox(proj, projectile.Center)) < dist && FargoSoulsUtil.CanDeleteProjectile(proj))
                {
                    FargoGlobalProjectile globalProj = proj.GetGlobalProjectile<FargoGlobalProjectile>();
                    globalProj.ChilledProj = true;
                    globalProj.ChilledTimer = 15;
                    projectile.netUpdate = true;
                }
            }


            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (npc.active && !npc.friendly && npc.damage > 0 && projectile.Distance(FargoSoulsUtil.ClosestPointInHitbox(npc, projectile.Center)) < dist && !npc.dontTakeDamage)
                {
                    npc.GetGlobalNPC<FargoSoulsGlobalNPC>().SnowChilled = true;
                    npc.GetGlobalNPC<FargoSoulsGlobalNPC>().SnowChilledTimer = 15;
                    npc.netUpdate = true;
                }
            }
        }
    }
}
