using FargowiltasSouls.Assets.ExtraTextures;


using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Projectiles.Deathrays;
using FargowiltasSouls.Core;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.BossWeapons
{
	public class HentaiSpearBigDeathray : MutantSpecialDeathray
	{
		public HentaiSpearBigDeathray() : base(60, 1.5f) { }

		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();

			// DisplayName.SetDefault("Phantasmal Deathray");
		}

		public override void SetDefaults()
		{
			base.SetDefaults();
			CooldownSlot = -1;
			Projectile.hostile = false;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;

			Projectile.FargoSouls().CanSplit = false;
			Projectile.FargoSouls().TimeFreezeImmune = true;
			Projectile.FargoSouls().DeletionImmuneRank = 2;

			Projectile.hide = true;
			Projectile.penetrate = -1;
		}

		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
		{
			behindProjectiles.Add(index);
		}

		public Vector2 TipOffset => 9f * Projectile.velocity * Projectile.scale; //offset to look like is at tip proper

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			if (Projectile.Distance(FargoSoulsUtil.ClosestPointInHitbox(targetHitbox, Projectile.Center)) < TipOffset.Length() * 2)
				return true;

			return base.Colliding(projHitbox, targetHitbox);
		}

		public override void AI()
		{
			base.AI();

			Player player = Main.player[Projectile.owner];

			if (!Main.dedServ && Main.LocalPlayer.active)
				Main.LocalPlayer.FargoSouls().Screenshake = 2;

			Vector2? vector78 = null;
			if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
			{
				Projectile.velocity = -Vector2.UnitY;
			}

			Projectile spear = FargoSoulsUtil.ProjectileExists(FargoSoulsUtil.GetProjectileByIdentity(Projectile.owner, (int)Projectile.ai[1], ModContent.ProjectileType<HentaiSpearWand>()));
			if (spear != null)
			{
				Projectile.timeLeft = 2;
				float itemrotate = player.direction < 0 ? MathHelper.Pi : 0;
				if (Math.Abs(player.itemRotation) > Math.PI / 2)
					itemrotate = itemrotate == 0 ? MathHelper.Pi : 0;
				Projectile.velocity = (player.itemRotation + itemrotate).ToRotationVector2();
				Projectile.Center = spear.Center + Main.rand.NextVector2Circular(5, 5);

				Projectile.position += Projectile.velocity * 164 * spear.scale * 0.45f; //offset by part of spear's length (wand)

				Projectile.damage = player.GetWeaponDamage(player.HeldItem);
				Projectile.CritChance = player.GetWeaponCrit(player.HeldItem);
				Projectile.knockBack = player.GetWeaponKnockback(player.HeldItem, player.HeldItem.knockBack);
			}
			else if (++Projectile.localAI[0] > 5) //leeway for mp lag
			{
				Projectile.Kill();
				return;
			}

			if (Projectile.localAI[0] == 0f)
			{
				if (!Main.dedServ)
					SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Assets/Sounds/Zombie_104"), Projectile.Center);
			}
			float num801 = 10f;

			if (Projectile.localAI[0] == maxTime / 2)
			{
				if (Projectile.owner == Main.myPlayer && !(player.controlUseTile && player.altFunctionUse == 2 && player.HeldItem.type == ModContent.ItemType<Items.Weapons.FinalUpgrades.HentaiSpear>()))
					Projectile.localAI[0] += 1f; //if stop firing, proceed to die
				else
					Projectile.localAI[0] -= 1f; //otherwise, stay (also for multiplayer!)
			}
			else
			{
				Projectile.localAI[0] += 1f;
			}

			if (Projectile.localAI[0] >= maxTime)
			{
				Projectile.Kill();
				return;
			}
			//Projectile.scale = num801;
			Projectile.scale = (float)Math.Sin(Projectile.localAI[0] * 3.14159274f / maxTime) * 1.5f * num801;
			if (Projectile.scale > num801)
			{
				Projectile.scale = num801;
			}

			Projectile.scale *= spear.scale / 1.3f;
			Projectile.position += TipOffset;

			//float num804 = Projectile.velocity.ToRotation();
			//num804 += Projectile.ai[0];
			//Projectile.rotation = num804 - 1.57079637f;
			//float num804 = Main.npc[(int)Projectile.ai[1]].ai[3] - 1.57079637f;
			//if (Projectile.ai[0] != 0f) num804 -= (float)Math.PI;
			//Projectile.rotation = num804;
			//num804 += 1.57079637f;
			//Projectile.velocity = num804.ToRotationVector2();
			float num805 = 3f;
			float num806 = Projectile.width;
			Vector2 samplingPoint = Projectile.Center;
			if (vector78.HasValue)
			{
				samplingPoint = vector78.Value;
			}
			float[] array3 = new float[(int)num805];
			//Collision.LaserScan(samplingPoint, Projectile.velocity, num806 * Projectile.scale, 3000f, array3);
			for (int i = 0; i < array3.Length; i++)
				array3[i] = 3000f;
			float num807 = 0f;
			int num3;
			for (int num808 = 0; num808 < array3.Length; num808 = num3 + 1)
			{
				num807 += array3[num808];
				num3 = num808;
			}
			num807 /= num805;
			float amount = 0.5f;
			Projectile.localAI[1] = MathHelper.Lerp(Projectile.localAI[1], num807, amount);
			/*Vector2 vector79 = Projectile.Center + Projectile.velocity * (Projectile.localAI[1] - 14f);
            for (int num809 = 0; num809 < 2; num809 = num3 + 1)
            {
                float num810 = Projectile.velocity.ToRotation() + ((Main.rand.Next(2) == 1) ? -1f : 1f) * 1.57079637f;
                float num811 = (float)Main.rand.NextDouble() * 2f + 2f;
                Vector2 vector80 = new Vector2((float)Math.Cos((double)num810) * num811, (float)Math.Sin((double)num810) * num811);
                int num812 = Dust.NewDust(vector79, 0, 0, 244, vector80.X, vector80.Y, 0, default(Color), 1f);
                Main.dust[num812].noGravity = true;
                Main.dust[num812].scale = 1.7f;
                num3 = num809;
            }
            if (Main.rand.NextBool(5))
            {
                Vector2 value29 = Projectile.velocity.RotatedBy(1.5707963705062866, default(Vector2)) * ((float)Main.rand.NextDouble() - 0.5f) * (float)Projectile.width;
                int num813 = Dust.NewDust(vector79 + value29 - Vector2.One * 4f, 8, 8, 244, 0f, 0f, 100, default(Color), 1.5f);
                Dust dust = Main.dust[num813];
                dust.velocity *= 0.5f;
                Main.dust[num813].velocity.Y = -Math.Abs(Main.dust[num813].velocity.Y);
            }*/
			//DelegateMethods.v3_1 = new Vector3(0.3f, 0.65f, 0.7f);
			//Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[1], (float)Projectile.width * Projectile.scale, new Utils.PerLinePoint(DelegateMethods.CastLight));

			Projectile.position -= Projectile.velocity;
			//float oldRot = Projectile.rotation;
			Projectile.rotation = Projectile.velocity.ToRotation() - 1.57079637f;

			if (++Projectile.ai[0] > 60)
			{
				Projectile.ai[0] = 0;

				SoundEngine.PlaySound(SoundID.Item84, player.Center);

				if (Projectile.owner == Main.myPlayer)
				{
					const int ringMax = 10;
					const float speed = 12f;
					const float rotation = 0.5f;
					for (int i = 0; i < ringMax; i++)
					{
						Vector2 vel = speed * Projectile.velocity.RotatedBy(2 * Math.PI / ringMax * i);
						Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center, vel, ModContent.ProjectileType<HentaiSphereRing>(),
							Projectile.damage, Projectile.knockBack, Projectile.owner, rotation, speed);
						Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center, vel, ModContent.ProjectileType<HentaiSphereRing>(),
							Projectile.damage, Projectile.knockBack, Projectile.owner, -rotation, speed);
					}
				}
			}

			//if (--dustTimer < -1)
			//{
			//    dustTimer = 50;

			//    float diff = MathHelper.WrapAngle(Projectile.rotation - oldRot);
			//    //if (npc.HasPlayerTarget && Math.Abs(MathHelper.WrapAngle(npc.SafeDirectionTo(Main.player[npc.target].Center).ToRotation() - Projectile.velocity.ToRotation())) < Math.Abs(diff)) diff = 0;
			//    diff *= 15f;

			//    const int ring = 220; //LAUGH
			//    for (int i = 0; i < ring; ++i)
			//    {
			//        Vector2 speed = Projectile.velocity.RotatedBy(diff) * 24f;

			//        Vector2 vector2 = (-Vector2.UnitY.RotatedBy(i * 3.14159274101257 * 2 / ring) * new Vector2(8f, 16f)).RotatedBy(Projectile.velocity.ToRotation() + diff);
			//        int index2 = Dust.NewDust(Main.player[Projectile.owner].Center, 0, 0, 111, 0.0f, 0.0f, 0, new Color(), 1f);
			//        Main.dust[index2].scale = 2.5f;
			//        Main.dust[index2].noGravity = true;
			//        Main.dust[index2].position = Main.player[Projectile.owner].Center;
			//        Main.dust[index2].velocity = vector2 * 2.5f + speed;

			//        index2 = Dust.NewDust(Main.player[Projectile.owner].Center, 0, 0, 111, 0.0f, 0.0f, 0, new Color(), 1f);
			//        Main.dust[index2].scale = 2.5f;
			//        Main.dust[index2].noGravity = true;
			//        Main.dust[index2].position = Main.player[Projectile.owner].Center;
			//        Main.dust[index2].velocity = vector2 * 1.75f + speed * 2;
			//    }
			//}
		}

		public override void PostAI()
		{
			base.PostAI();

			Projectile.hide = true;

			if (!Main.dedServ)
			{
				ManagedScreenFilter filter = ShaderManager.GetFilter("FargowiltasSouls.FinalSpark");
				filter.Activate();
				if (SoulConfig.Instance.ForcedFilters && Main.WaveQuality == 0)
					Main.WaveQuality = 1;
			}
		}

		public override void OnKill(int timeLeft)
		{
			base.OnKill(timeLeft);
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			target.immune[Projectile.owner] = 1; //balanceing
			target.AddBuff(ModContent.BuffType<CurseoftheMoonBuff>(), 600);
		}

		public bool BeBrighter => Projectile.ai[0] > 0f;


		public float WidthFunction(float trailInterpolant)
		{
			// Grow rapidly from the start to full length. Any more than this notably distorts the texture.
			float baseWidth = Projectile.scale * Projectile.width;
			//if (trailInterpolant < 0.05f)
			return baseWidth;

			// Grow to 2x width by the end. Any more than this distorts the texture too much.
			//return MathHelper.Lerp(baseWidth, baseWidth * 2, trailInterpolant);
		}

		public static Color ColorFunction(float trailInterpolant) =>
			Color.Lerp(
				FargoSoulsUtil.AprilFools ? new Color(255, 0, 0, 100) : new(31, 187, 192, 100),
				FargoSoulsUtil.AprilFools ? new Color(255, 191, 51, 100) : new(51, 255, 191, 100),
				trailInterpolant);

		public override bool PreDraw(ref Color lightColor)
		{
			// This should never happen, but just in case.
			if (Projectile.velocity == Vector2.Zero)
				return false;

			ManagedShader shader = ShaderManager.GetShader("FargowiltasSouls.MutantDeathray");

			// Get the laser end position.
			Vector2 laserEnd = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitY) * drawDistance;

			// Create 8 points that span across the draw distance from the projectile center.

			// This allows the drawing to be pushed back, which is needed due to the shader fading in at the start to avoid
			// sharp lines.
			Vector2 initialDrawPoint = Projectile.Center - Projectile.velocity * 400f;
			Vector2[] baseDrawPoints = new Vector2[8];
			for (int i = 0; i < baseDrawPoints.Length; i++)
				baseDrawPoints[i] = Vector2.Lerp(initialDrawPoint, laserEnd, i / (float)(baseDrawPoints.Length - 1f));

			// Set shader parameters. This one takes a fademap and a color.

			// The laser should fade to white in the middle.
			Color brightColor = new(194, 255, 242, 100);
			shader.TrySetParameter("mainColor", brightColor);
			FargoSoulsUtil.SetTexture1(FargosTextureRegistry.MutantStreak.Value);
			// Draw a big glow above the start of the laser, to help mask the intial fade in due to the immense width.

			Texture2D glowTexture = ModContent.Request<Texture2D>("FargowiltasSouls/Content/Projectiles/GlowRing").Value;

			Vector2 glowDrawPosition = Projectile.Center - Projectile.velocity * (BeBrighter ? 90f : 180f);

			Main.EntitySpriteDraw(glowTexture, glowDrawPosition - Main.screenPosition, null, brightColor, Projectile.rotation, glowTexture.Size() * 0.5f, Projectile.scale * 0.4f, SpriteEffects.None, 0);
			PrimitiveRenderer.RenderTrail(baseDrawPoints, new(WidthFunction, ColorFunction, Shader: shader), 60);
			return false;
		}
	}
}