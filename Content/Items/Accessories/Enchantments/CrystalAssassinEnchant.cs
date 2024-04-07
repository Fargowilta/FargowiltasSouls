using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
	public class CrystalAssassinEnchant : BaseEnchant
	{
		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();

			// DisplayName.SetDefault("Crystal Assassin Enchantment");
			/* Tooltip.SetDefault(
@"Allows the ability to dash
Use Assassin hotkey to throw a smoke bomb, use it again to teleport to it and gain the First Strike Buff
Using the Rod of Discord will also grant this buff
First Strike ensures your next attack hits a vital spot dealing 3x damage and reducing defense by 10
'Now you see me, now you don’t'"); */
		}

		public override Color nameColor => new(36, 157, 207);

		public override void SetDefaults()
		{
			base.SetDefaults();

			Item.rare = ItemRarityID.Pink;
			Item.value = 150000;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			AddEffects(player, Item);
		}

		public static void AddEffects(Player player, Item item)
		{
			FargoSoulsPlayer modPlayer = player.FargoSouls();
			modPlayer.CrystalEnchantActive = true;

			//cooldown
			if (modPlayer.SmokeBombCD != 0)
			{
				modPlayer.SmokeBombCD--;
			}

			if (!modPlayer.HasDash && player.AddEffect<CrystalAssassinDash>(item))
				player.dashType = 5;
		}

		public static void SmokeBombKey(FargoSoulsPlayer modPlayer)
		{
			//throw smoke bomb
			if (modPlayer.CrystalSmokeBombProj == null)
			{
				const float gravity = 0.18f;
				float time = 60f;
				Vector2 distance = Main.MouseWorld - modPlayer.Player.Center;
				distance.X /= time;
				distance.Y = distance.Y / time - 0.5f * gravity * time;

				modPlayer.CrystalSmokeBombProj = Main.projectile[Projectile.NewProjectile(modPlayer.Player.GetSource_Misc(""), modPlayer.Player.Center, distance + Main.rand.NextVector2Square(0, 0) * 2,
						ProjectileID.SmokeBomb, 0, 0f, Main.myPlayer)];

				modPlayer.SmokeBombCD = 15;
			}
			//already threw smoke bomb, tele to it
			else
			{
				Vector2 teleportPos = new(modPlayer.CrystalSmokeBombProj.position.X, modPlayer.CrystalSmokeBombProj.position.Y - 30);
				Vector2 originalPos = new(teleportPos.X, teleportPos.Y);

				//spiral out to find a save spot
				int count = 0;
				int increase = 10;
				while (Collision.SolidCollision(teleportPos, modPlayer.Player.width, modPlayer.Player.height))
				{
					teleportPos = originalPos;

					switch (count)
					{
						case 0:
							teleportPos.X -= increase;
							break;
						case 1:
							teleportPos.X += increase;
							break;
						case 2:
							teleportPos.Y += increase;
							break;
						default:
							teleportPos.Y -= increase;
							increase += 10;
							break;
					}
					count++;

					if (count >= 4)
					{
						count = 0;
					}

					if (increase > 100)
					{
						return;
					}
				}

				if (teleportPos.X > 50 && teleportPos.X < (double)(Main.maxTilesX * 16 - 50) && teleportPos.Y > 50 && teleportPos.Y < (double)(Main.maxTilesY * 16 - 50))
				{
					modPlayer.Player.Teleport(teleportPos, 1);
					NetMessage.SendData(MessageID.TeleportEntity, -1, -1, null, 0, modPlayer.Player.whoAmI, teleportPos.X, teleportPos.Y, 1);

					modPlayer.Player.AddBuff(ModContent.BuffType<FirstStrikeBuff>(), 60);

					modPlayer.CrystalSmokeBombProj.timeLeft = 120;
					modPlayer.SmokeBombCD = 300;
					modPlayer.CrystalSmokeBombProj = null;
				}
			}
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.CrystalNinjaHelmet)
				.AddIngredient(ItemID.CrystalNinjaChestplate)
				.AddIngredient(ItemID.CrystalNinjaLeggings)
				.AddIngredient(ItemID.FlyingKnife)
				.AddIngredient(ItemID.UnicornonaStick)
				.AddIngredient(ItemID.SmokeBomb, 50)

				.AddTile(TileID.CrystalBall)
				.Register();
		}
	}
	public class CrystalAssassinDash : AccessoryEffect
	{
		public override Header ToggleHeader => Header.GetHeader<ShadowHeader>();
		public override int ToggleItemType => ModContent.ItemType<CrystalAssassinEnchant>();
		public override bool IgnoresMutantPresence => true;
	}
}
