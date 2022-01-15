using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Critters
{
    public class BaseBirdProj : ModProjectile
    {
		private int npcType;
		private int goreType;

		private NPC target;

		protected BaseBirdProj(int npcType, int goreType)
		{
			this.npcType = npcType;
			this.goreType = goreType;
		}

        public override string Texture => "Terraria/NPC_" + npcType;

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Bird");
        }

        public override void SetDefaults()
        {
			projectile.minion = true;
            projectile.width = 14;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.penetrate = 2;
            projectile.timeLeft = 300;

			Main.projFrames[projectile.type] = 5;
		}

        public override void AI()
        {
			projectile.frameCounter++;
			if (projectile.frameCounter >= 4)
			{
				projectile.frameCounter = 0;
				projectile.frame = (projectile.frame + 1) % 5;

				if (projectile.frame == 4)
				{
					projectile.frame = 0;
				}
			}



			projectile.spriteDirection = projectile.velocity.X > 0 ? -1 : 1;



			//bird npc ai
			if (projectile.localAI[0] == 0)
			{
				if (projectile.ai[0] == 0f)
				{
					if (Main.netMode != 1)
					{
						if (projectile.velocity.X != 0f || projectile.velocity.Y < 0f || (double)projectile.velocity.Y > 0.3)
						{
							projectile.ai[0] = 1f;
							projectile.netUpdate = true;
							projectile.direction = -projectile.direction;
						}
						else
						{
							Rectangle rectangle3 = new Rectangle((int)Main.player[projectile.owner].position.X, (int)Main.player[projectile.owner].position.Y, Main.player[projectile.owner].width, Main.player[projectile.owner].height);
							Rectangle rectangle4 = new Rectangle((int)projectile.position.X - 100, (int)projectile.position.Y - 100, projectile.width + 200, projectile.height + 200);
							if (rectangle4.Intersects(rectangle3))
							{
								projectile.ai[0] = 1f;
								projectile.velocity.Y = projectile.velocity.Y - 6f;
								projectile.netUpdate = true;
								projectile.direction = -projectile.direction;
							}
						}
					}
				}
				else if (!Main.player[projectile.owner].dead)
				{
					if (projectile.direction == -1 && projectile.velocity.X > -3f)
					{
						projectile.velocity.X = projectile.velocity.X - 0.1f;
						if (projectile.velocity.X > 3f)
						{
							projectile.velocity.X = projectile.velocity.X - 0.1f;
						}
						else if (projectile.velocity.X > 0f)
						{
							projectile.velocity.X = projectile.velocity.X - 0.05f;
						}
						if (projectile.velocity.X < -3f)
						{
							projectile.velocity.X = -3f;
						}
					}
					else if (projectile.direction == 1 && projectile.velocity.X < 3f)
					{
						projectile.velocity.X = projectile.velocity.X + 0.1f;
						if (projectile.velocity.X < -3f)
						{
							projectile.velocity.X = projectile.velocity.X + 0.1f;
						}
						else if (projectile.velocity.X < 0f)
						{
							projectile.velocity.X = projectile.velocity.X + 0.05f;
						}
						if (projectile.velocity.X > 3f)
						{
							projectile.velocity.X = 3f;
						}
					}
					int num317 = (int)((projectile.position.X + (float)(projectile.width / 2)) / 16f) + projectile.direction;
					int num318 = (int)((projectile.position.Y + (float)projectile.height) / 16f);
					bool flag26 = true;
					int num319 = 15;
					bool flag27 = false;
					for (int num320 = num318; num320 < num318 + num319; num320++)
					{
						if (Main.tile[num317, num320] == null)
						{
							Main.tile[num317, num320] = new Tile();
						}
						if ((Main.tile[num317, num320].nactive() && Main.tileSolid[(int)Main.tile[num317, num320].type]) || Main.tile[num317, num320].liquid > 0)
						{
							if (num320 < num318 + 5)
							{
								flag27 = true;
							}
							flag26 = false;
							break;
						}
					}
					if (flag26)
					{
						projectile.velocity.Y = projectile.velocity.Y + 0.05f;
					}
					else
					{
						projectile.velocity.Y = projectile.velocity.Y - 0.1f;
					}
					if (flag27)
					{
						projectile.velocity.Y = projectile.velocity.Y - 0.2f;
					}
					if (projectile.velocity.Y > 2f)
					{
						projectile.velocity.Y = 2f;
					}
					if (projectile.velocity.Y < -4f)
					{
						projectile.velocity.Y = -4f;
					}
				}


				if (projectile.timeLeft <= 270)
				{
					int dist = 500;

					//check for nearby enemies
					for (int i = 0; i < Main.maxNPCs; i++)
					{
						NPC npc = Main.npc[i];

						if (npc.active && !npc.friendly && npc.type != NPCID.TargetDummy && Vector2.Distance(projectile.Center, npc.Center) <= dist && Collision.CanHitLine(npc.Center, npc.width, npc.height, projectile.Center, projectile.width, projectile.height))
						{
							target = npc;
							dist = (int)Vector2.Distance(projectile.Center, npc.Center);
						}
					}

					if (target != null)
					{
						projectile.localAI[0] = 1;
						projectile.velocity = projectile.DirectionTo(target.Center) * 10f;
					}
					
				}
				

			}
			else
			{
				//home on target

			}

			
			
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (projectile.velocity.X != oldVelocity.X)
			{
				projectile.direction *= -1;
				projectile.velocity.X = projectile.oldVelocity.X * -0.5f;
				if (projectile.direction == -1 && projectile.velocity.X > 0f && projectile.velocity.X < 2f)
				{
					projectile.velocity.X = 2f;
				}
				if (projectile.direction == 1 && projectile.velocity.X < 0f && projectile.velocity.X > -2f)
				{
					projectile.velocity.X = -2f;
				}
			}
			if (projectile.velocity.Y != oldVelocity.Y)
			{
				projectile.velocity.Y = projectile.oldVelocity.Y * -0.5f;
				if (projectile.velocity.Y > 0f && projectile.velocity.Y < 1f)
				{
					projectile.velocity.Y = 1f;
				}
				if (projectile.velocity.Y < 0f && projectile.velocity.Y > -1f)
				{
					projectile.velocity.Y = -1f;
				}
			}

			return false;
		}

        public override void Kill(int timeLeft)
        {
			for (int num377 = 0; num377 < 10; num377++)
			{
				Dust.NewDust(projectile.position, projectile.width, projectile.height, 5, 0, -2f, 0, default(Color), 1f);
			}

			Gore.NewGore(projectile.position, projectile.velocity, goreType, 1f);
			
		}
    }

	public class BirdProj : BaseBirdProj
	{
		public BirdProj() : base(NPCID.Bird, 100)
        {
        }
	}

	public class CardinalProj : BaseBirdProj
	{
		public CardinalProj() : base(NPCID.BirdRed, 432)
		{
		}

		public override void SetDefaults()
		{
			base.SetDefaults();
			projectile.penetrate = 1;
		}
	}

	public class BlueJayProj : BaseBirdProj
	{
		public BlueJayProj() : base(NPCID.BirdBlue, 431)
		{
		}

		public override void SetDefaults()
		{
			base.SetDefaults();
			projectile.penetrate = 5;
		}
	}
}
