using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasSouls.Content.Items.BossBags;
using FargowiltasSouls.Content.Projectiles.Deathrays;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Weapons.Challengers
{
	public class RoseTintedVisor : SoulsItem
	{

		public override void SetStaticDefaults()
		{

			Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 280;
			Item.mana = 22;
			Item.DamageType = DamageClass.Magic;
			Item.width = 20;
			Item.height = 20;
			Item.useTime = 45;
			Item.useAnimation = 45;
			Item.useStyle = ItemUseStyleID.HiddenAnimation;
			Item.knockBack = 6;
			Item.value = Item.sellPrice(0, 5);
			Item.rare = ItemRarityID.LightRed;
			//Item.UseSound = SoundID.Item40;
			Item.shoot = ModContent.ProjectileType<RoseTintedVisorDeathray>(); //guns just have this, don't ask
			Item.shootSpeed = 3f;
			Item.channel = true;
			Item.noUseGraphic = true;
			Item.noMelee = true;
			Item.autoReuse = true;
		}
		int Charges = 0;
		public override void ModifyWeaponCrit(Player player, ref float crit)
		{
			crit += Charges * (100f / 6);
		}
		public override void HoldItem(Player player)
		{
			if (Charges == 6)
			{
				Vector2 pos = (player.gravDir > 0 ? player.Top + Vector2.UnitY * 7 : player.Bottom - Vector2.UnitY * 7);
				Particle p = new SmallSparkle(pos, Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(2, 5), Color.DeepPink, 1f, 10,
					Main.rand.NextFloat(MathHelper.TwoPi), Main.rand.NextFloat(-MathHelper.Pi / 24, MathHelper.Pi / 24));
				p.Spawn();
			}
			if ((Main.mouseLeftRelease || player.dead) && Charges > 0 && player.whoAmI == Main.myPlayer)
			{
				for (int i = 0; i < Charges; i++)
				{
					Vector2 vel = Vector2.Normalize(Main.MouseWorld - player.Center).RotatedByRandom(MathHelper.Pi / 32) * Item.shootSpeed;
					Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Top + Vector2.UnitY * 8, vel, Item.shoot, (int)(player.ActualClassDamage(DamageClass.Magic) * Item.damage), Item.knockBack, player.whoAmI);
					player.velocity -= vel;
				}
				if (Charges >= 6)
				{
					SoundEngine.PlaySound(SoundID.Item68, player.Center);
				}
				player.reuseDelay = 45;
				Charges = 0;
			}
		}
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			if (Charges < 6)
			{
				SoundEngine.PlaySound(SoundID.Item25 with { Pitch = -0.5f + (Charges / 6f) }, player.Center);
				Charges++;
			}
			return false;
		}
		public override void AddRecipes()
		{
			CreateRecipe().AddIngredient<BanishedBaronBag>(2).AddTile(TileID.Solidifier).DisableDecraft().Register();
		}
	}
}