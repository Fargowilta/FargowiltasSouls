using System;
using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Souls
{
	public class Chlorofuck : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Chlorofuck");
            Main.projPet[projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.Homing[projectile.type] = true;
        }

		public override void SetDefaults()
		{
			projectile.netImportant = true;
			projectile.width = 22;
			projectile.height = 42;
			projectile.friendly = true;
            projectile.minion = true;
			projectile.penetrate = -1; 
			projectile.timeLeft = 18000;
			projectile.tileCollide = false;
			projectile.ignoreWater = true;
		}
		
		public override void AI()
        {
            Player player = Main.player[projectile.owner];
			FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();

			if (player.whoAmI == Main.myPlayer && (player.dead || !(modPlayer.ChloroEnchant || modPlayer.TerrariaSoul) || !player.GetToggleValue("Chlorophyte")))
			{
				modPlayer.ChloroEnchant = false;
                projectile.Kill();
				projectile.netUpdate = true;
                return;
            }

			projectile.netUpdate = true;

            float cooldown = 50f;
			
			float num395 = Main.mouseTextColor / 200f - 0.35f;
			num395 *= 0.2f;
			projectile.scale = num395 + 0.95f;

            if (true)
			{
                //rotation mumbo jumbo
                float distanceFromPlayer = 75;

                Lighting.AddLight(projectile.Center, 0.1f, 0.4f, 0.2f);

                projectile.position = player.Center + new Vector2(distanceFromPlayer, 0f).RotatedBy(projectile.ai[1]);
                projectile.position.X -= projectile.width / 2;
                projectile.position.Y -= projectile.height / 2;
                float rotation = 0.03f;
                projectile.ai[1] -= rotation;
                if (projectile.ai[1] > (float)Math.PI)
                {
                    projectile.ai[1] -= 2f * (float)Math.PI;
                    projectile.netUpdate = true;
                }
                projectile.rotation = projectile.ai[1] + (float)Math.PI / 2f;


                //wait for CD
                if (projectile.ai[0] != 0f)
				{
					projectile.ai[0] -= 1f;
					return;
				}

                //trying to shoot
				float num396 = projectile.position.X;
				float num397 = projectile.position.Y;
				float num398 = 700f;
				bool flag11 = false;

				for (int i = 0; i < 200; i++)
				{
					if (Main.npc[i].CanBeChasedBy(projectile, true))
					{
						float num400 = Main.npc[i].position.X + Main.npc[i].width / 2;
						float num401 = Main.npc[i].position.Y + Main.npc[i].height / 2;
						float num402 = Math.Abs(projectile.position.X + projectile.width / 2 - num400) + Math.Abs(projectile.position.Y + projectile.height / 2 - num401);

						if (num402 < num398 && Collision.CanHit(projectile.position, projectile.width, projectile.height, Main.npc[i].position, Main.npc[i].width, Main.npc[i].height))
						{
							num398 = num402;
							num396 = num400;
							num397 = num401;
							flag11 = true;
						}
					}
				}

                //shoot
				if (flag11)
				{
					Vector2 vector29 = new Vector2(projectile.position.X + projectile.width * 0.5f, projectile.position.Y + projectile.height * 0.5f);
					float num404 = num396 - vector29.X;
					float num405 = num397 - vector29.Y;
					float num406 = (float)Math.Sqrt(num404 * num404 + num405 * num405);
					num406 = 10f / num406;
					num404 *= num406;
					num405 *= num406;
					Projectile.NewProjectile(projectile.Center.X - 4f, projectile.Center.Y, num404, num405, ProjectileID.CrystalLeafShot, projectile.damage, projectile.knockBack, projectile.owner);
					projectile.ai[0] = cooldown;
                }
			}

			if (Main.netMode == NetmodeID.Server)
				projectile.netUpdate = true;
        }
	}
}