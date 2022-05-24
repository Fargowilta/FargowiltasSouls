using FargowiltasSouls.NPCs;
using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Souls
{
    public class Snowstorm : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Snowstorm");
        }

        public override string Texture => "FargowiltasSouls/Projectiles/Empty";

        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;

            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
            Projectile.width = 1;
            Projectile.height = 1;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            Projectile.timeLeft++;

            if (player.dead || !player.active || !modPlayer.SnowEnchantActive)
                Projectile.Kill();

            if (player == Main.LocalPlayer)
            {
                Projectile.Center = Main.MouseWorld;

                if (!player.GetToggleValue("Snow"))
                    Projectile.Kill();
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
                    Projectile.Center + offset - new Vector2(4, 4), 0, 0,
                    76, 0, 0, 100, Color.White, .75f)];

                dust.noGravity = true;
            }

            //for (int i = 0; i < 20; i++)
            //{
            //    Vector2 offset = new Vector2();
            //    double angle = Main.rand.NextDouble() * 2d * Math.PI;
            //    offset.X += (float)(Math.Sin(angle) * dist);
            //    offset.Y += (float)(Math.Cos(angle) * dist);
            //    if (!Collision.SolidCollision(Projectile.Center + offset - new Vector2(4, 4), 0, 0))
            //    {
            //        Dust dust = Main.dust[Dust.NewDust(
            //          Projectile.Center + offset - new Vector2(4, 4), 0, 0,
            //          76, 0, 0, 100, Color.White, 1f
            //          )];
            //        dust.velocity = Projectile.velocity;
            //        dust.noGravity = true;
            //    }
            //}



            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];


                if (proj.active && proj.hostile && proj.damage > 0 && Projectile.Distance(FargoSoulsUtil.ClosestPointInHitbox(proj, Projectile.Center)) < dist && FargoSoulsUtil.CanDeleteProjectile(proj))
                {
                    FargoSoulsGlobalProjectile globalProj = proj.GetGlobalProjectile<FargoSoulsGlobalProjectile>();
                    globalProj.ChilledProj = true;
                    globalProj.ChilledTimer = 15;
                    Projectile.netUpdate = true;
                }
            }


            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (npc.active && !npc.friendly && npc.damage > 0 && Projectile.Distance(FargoSoulsUtil.ClosestPointInHitbox(npc, Projectile.Center)) < dist && !npc.dontTakeDamage)
                {
                    npc.GetGlobalNPC<FargoSoulsGlobalNPC>().SnowChilled = true;
                    npc.GetGlobalNPC<FargoSoulsGlobalNPC>().SnowChilledTimer = 15;
                    npc.netUpdate = true;
                }
            }
        }
    }
}
