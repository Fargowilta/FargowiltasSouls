using FargowiltasSouls.Content.Items.BossBags;
using FargowiltasSouls.Content.Projectiles.ChallengerItems;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Weapons.Challengers
{
	public class TheBaronsTusk : SoulsItem
	{
		public override void SetStaticDefaults()
		{

			Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;

			ItemID.Sets.BonusAttackSpeedMultiplier[Type] = 0.25f;
		}

		public override void SetDefaults()
		{
			Item.damage = 70;
			Item.DamageType = DamageClass.Melee;
			Item.width = 66;
			Item.height = 64;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 6;
			Item.value = Item.sellPrice(0, 4, 0);
			Item.rare = ItemRarityID.LightRed;
			//Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<BaronTuskShrapnel>();
			Item.shootSpeed = 15f;
		}
		int Timer = 0;
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) => false;
		public override void HoldItem(Player player) //fancy momentum swing, this should be generalized and applied to other swords imo
		{
			if (player.itemAnimation == 0)
			{
				Timer = 0;
				return;
			}

			if (player.itemAnimation == player.itemAnimationMax)
			{
				Timer = player.itemAnimationMax;
			}
			if (player.itemAnimation > 0)
			{
				Timer--;
			}

			if (Timer == player.itemAnimationMax / 2)
			{
				SoundEngine.PlaySound(SoundID.Item1, player.Center);
				SoundEngine.PlaySound(SoundID.Item39, player.Center);
				for (int i = 0; i < 3; i++)
				{
					Vector2 vel = (Item.shootSpeed + Main.rand.Next(-2, 2)) * Vector2.Normalize(Main.MouseWorld - player.itemLocation).RotatedByRandom(MathHelper.Pi / 14);
					int p = Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.itemLocation, vel, Item.shoot, (int)(player.ActualClassDamage(DamageClass.Melee) * Item.damage / 3f), Item.knockBack, player.whoAmI);
				}

			}
			if (Timer > 2 * player.itemAnimationMax / 3)
			{
				player.itemAnimation = player.itemAnimationMax;
				Item.noMelee = true;
			}
			else
			{
				Item.noMelee = false;
				float prog = (float)Timer / (2 * player.itemAnimationMax / 3);
				player.itemAnimation = (int)(player.itemAnimationMax * Math.Pow(MomentumProgress(prog), 2));
			}

		}
		public override void ModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers)
		{
			IEnumerable<Projectile> embeddedShrapnel = Main.projectile.Where(p => p.TypeAlive<BaronTuskShrapnel>() && p.owner == player.whoAmI && p.As<BaronTuskShrapnel>().EmbeddedNPC == target);
			int shrapnel = embeddedShrapnel.Count();
			if (shrapnel >= 15)
			{
				SoundEngine.PlaySound(SoundID.Item68, target.Center);
				modifiers.FlatBonusDamage += 15 * Item.damage / 2.5f + (shrapnel * Item.damage / 6);
				modifiers.SetCrit();
				foreach (Projectile proj in embeddedShrapnel)
					proj.ai[1] = 2;
			}
		}
		//this is ripped from my own game project
		///<summary>
		///Returns distance progress by a sine formula based on linear progress = (% between 1-0). f(1) = 1, f(0) = 0.
		///</summary>
		public static float MomentumProgress(float x)
		{
			return (x * x * 3) - (x * x * x * 2);
		}
		public override void AddRecipes()
		{
			CreateRecipe().AddIngredient<BanishedBaronBag>(2).AddTile(TileID.Solidifier).DisableDecraft().Register();
		}
	}
}