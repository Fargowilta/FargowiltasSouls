using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
	public class SpectreEnchant : BaseEnchant
	{
		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();

			// DisplayName.SetDefault("Spectre Enchantment");

			//             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "幽魂魔石");

			// Tooltip.SetDefault(tooltip);

			//             string tooltip_ch =
			// @"伤害敌人时有几率生成幽魂珠
			// 攻击造成暴击时有几率生成治疗珠
			// '他们的生命力将毁灭他们自己'";
			//             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, tooltip_ch);

		}

		public override Color nameColor => new(172, 205, 252);


		public override void SetDefaults()
		{
			base.SetDefaults();

			Item.rare = ItemRarityID.Lime;
			Item.value = 250000;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.AddEffect<SpectreEffect>(Item);
		}

		public override void AddRecipes()
		{
			CreateRecipe()

			.AddRecipeGroup("FargowiltasSouls:AnySpectreHead")
			.AddIngredient(ItemID.SpectreRobe)
			.AddIngredient(ItemID.SpectrePants)
			//spectre wings
			.AddIngredient(ItemID.UnholyTrident)
			//nettle burst
			//.AddIngredient(ItemID.Keybrand);
			.AddIngredient(ItemID.SpectreStaff)
			.AddIngredient(ItemID.BatScepter)
			//bat scepter
			//.AddIngredient(ItemID.WispinaBottle);

			.AddTile(TileID.CrystalBall)
			.Register();
		}
	}
	public class SpectreEffect : AccessoryEffect
	{

		public override Header ToggleHeader => Header.GetHeader<SpiritHeader>();
		public override int ToggleItemType => ModContent.ItemType<SpectreEnchant>();
		public override void OnHitNPCEither(Player player, NPC target, NPC.HitInfo hitInfo, DamageClass damageClass, int baseDamage, Projectile projectile, Item item)
		{
			FargoSoulsPlayer modPlayer = player.FargoSouls();
			if (!target.immortal && modPlayer.SpectreCD <= 0 && Main.rand.NextBool())
			{
				bool spectreForceEffect = modPlayer.ForceEffect<SpectreEnchant>();
				if (projectile == null)
				{
					//forced orb spawn reeeee
					float num = 4f;
					float speedX = Main.rand.Next(-100, 101);
					float speedY = Main.rand.Next(-100, 101);
					float num2 = (float)Math.Sqrt((double)(speedX * speedX + speedY * speedY));
					num2 = num / num2;
					speedX *= num2;
					speedY *= num2;
					Projectile p = FargoSoulsUtil.NewProjectileDirectSafe(GetSource_EffectItem(player), target.position, new Vector2(speedX, speedY), ProjectileID.SpectreWrath, hitInfo.Damage / 2, 0, player.whoAmI, target.whoAmI);

					if ((spectreForceEffect || (hitInfo.Crit && Main.rand.NextBool(5))) && p != null)
					{
						SpectreHeal(player, target, p);
						modPlayer.SpectreCD = spectreForceEffect ? 5 : 20;
					}
				}
				else if (projectile.type != ProjectileID.SpectreWrath)
				{
					SpectreHurt(projectile);

					if (spectreForceEffect || (hitInfo.Crit && Main.rand.NextBool(5)))
						SpectreHeal(player, target, projectile);

					modPlayer.SpectreCD = spectreForceEffect ? 5 : 20;
				}
			}
		}
		public void SpectreHeal(Player player, NPC npc, Projectile proj)
		{
			if (npc.canGhostHeal && !player.moonLeech)
			{
				float num = 0.2f;
				num -= proj.numHits * 0.05f;
				if (num <= 0f)
				{
					return;
				}
				float num2 = proj.damage * num;
				if ((int)num2 <= 0)
				{
					return;
				}
				if (Main.player[Main.myPlayer].lifeSteal <= 0f)
				{
					return;
				}
				Main.player[Main.myPlayer].lifeSteal -= num2 * 5; //original damage

				float num3 = 0f;
				int num4 = proj.owner;
				for (int i = 0; i < 255; i++)
				{
					if (Main.player[i].active && !Main.player[i].dead && ((!Main.player[proj.owner].hostile && !Main.player[i].hostile) || Main.player[proj.owner].team == Main.player[i].team))
					{
						float num5 = Math.Abs(Main.player[i].position.X + (Main.player[i].width / 2) - proj.position.X + (proj.width / 2)) + Math.Abs(Main.player[i].position.Y + (Main.player[i].height / 2) - proj.position.Y + (proj.height / 2));
						if (num5 < 1200f && (Main.player[i].statLifeMax2 - Main.player[i].statLife) > num3)
						{
							num3 = Main.player[i].statLifeMax2 - Main.player[i].statLife;
							num4 = i;
						}
					}
				}
				Projectile.NewProjectile(proj.GetSource_FromThis(), proj.position.X, proj.position.Y, 0f, 0f, ProjectileID.SpiritHeal, 0, 0f, proj.owner, num4, num2);
			}
		}

		public void SpectreHurt(Projectile proj)
		{
			int num = proj.damage / 2;
			if (proj.damage / 2 <= 1)
			{
				return;
			}
			int num2 = 1000;
			if (Main.player[Main.myPlayer].ghostDmg > (float)num2)
			{
				return;
			}
			Main.player[Main.myPlayer].ghostDmg += (float)num;
			int[] array = new int[200];
			int num3 = 0;
			int num4 = 0;
			for (int i = 0; i < 200; i++)
			{
				if (Main.npc[i].CanBeChasedBy(this, false))
				{
					float num5 = Math.Abs(Main.npc[i].position.X + (Main.npc[i].width / 2) - proj.position.X + (proj.width / 2)) + Math.Abs(Main.npc[i].position.Y + (Main.npc[i].height / 2) - proj.position.Y + (proj.height / 2));
					if (num5 < 800f)
					{
						if (Collision.CanHit(proj.position, 1, 1, Main.npc[i].position, Main.npc[i].width, Main.npc[i].height) && num5 > 50f)
						{
							array[num4] = i;
							num4++;
						}
						else if (num4 == 0)
						{
							array[num3] = i;
							num3++;
						}
					}
				}
			}
			if (num3 == 0 && num4 == 0)
			{
				return;
			}
			int num6;
			if (num4 > 0)
			{
				num6 = array[Main.rand.Next(num4)];
			}
			else
			{
				num6 = array[Main.rand.Next(num3)];
			}
			float num7 = 4f;
			float num8 = Main.rand.Next(-100, 101);
			float num9 = Main.rand.Next(-100, 101);
			float num10 = (float)Math.Sqrt((double)(num8 * num8 + num9 * num9));
			num10 = num7 / num10;
			num8 *= num10;
			num9 *= num10;
			Projectile.NewProjectile(proj.GetSource_FromThis(), proj.position.X, proj.position.Y, num8, num9, ProjectileID.SpectreWrath, num, 0f, proj.owner, (float)num6, 0);
		}
	}
}
