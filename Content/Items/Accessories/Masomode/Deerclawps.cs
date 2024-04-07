using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Masomode
{
	public class Deerclawps : SoulsItem
	{

		public override bool Eternity => true;

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Deerclawps");
			/* Tooltip.SetDefault("Grants immunity to Slow and Frozen" +
                "\nDashing leaves a trail of ice spikes" +
                "\n'The trimmed nails of a defeated foe'"); */

			//             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "冰鹿爪");
			//             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, @"免疫缓慢和冰冻
			// 冲刺会留下一串冰刺
			// “从被击败的敌人的脚上剪下来的指甲”");

			Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.accessory = true;
			Item.rare = ItemRarityID.Orange;
			Item.value = Item.sellPrice(0, 3);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.buffImmune[BuffID.Slow] = true;
			player.buffImmune[BuffID.Frozen] = true;
			player.AddEffect<DeerclawpsDive>(Item);
			player.AddEffect<DeerclawpsEffect>(Item);
		}
	}
	public class DeerclawpsDive : AccessoryEffect
	{
		public override Header ToggleHeader => Header.GetHeader<LumpofFleshHeader>();
		public override int ToggleItemType => ModContent.ItemType<Deerclawps>();
		public static void DeerclawpsLandingSpikes(Player player, Vector2 pos)
		{
			if (player.whoAmI == Main.myPlayer)
			{
				const int max = 4;
				for (int i = -max; i <= max; i++)
				{
					Vector2 vel = 16f * -Vector2.UnitY.RotatedBy(MathHelper.PiOver2 / max * i).RotatedByRandom(MathHelper.ToRadians(10));

					int dam = 32;
					int type = ProjectileID.DeerclopsIceSpike;
					float ai0 = -15f;
					float ai1 = Main.rand.NextFloat(0.5f, 1f);
					if (player.FargoSouls().LumpOfFlesh)
					{
						dam = 48;
						type = ProjectileID.SharpTears;
						ai0 *= 2f;
						ai1 += 0.5f;
					}
					dam = (int)(dam * player.ActualClassDamage(DamageClass.Melee));

					Projectile.NewProjectile(player.GetSource_EffectItem<DeerclawpsEffect>(), pos, vel, type, dam, 4f, Main.myPlayer, ai0, ai1);
				}
			}
		}
	}
	public class DeerclawpsEffect : AccessoryEffect
	{
		public override Header ToggleHeader => Header.GetHeader<LumpofFleshHeader>();
		public override int ToggleItemType => ModContent.ItemType<Deerclawps>();
		public static void DeerclawpsAttack(Player player, Vector2 pos)
		{
			if (player.whoAmI == Main.myPlayer)
			{
				Vector2 vel = 16f * -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(30));

				int dam = 32;
				int type = ProjectileID.DeerclopsIceSpike;
				float ai0 = -15f;
				float ai1 = Main.rand.NextFloat(0.5f, 1f);
				if (player.FargoSouls().LumpOfFlesh)
				{
					dam = 48;
					type = ProjectileID.SharpTears;
					ai0 *= 2f;
					ai1 += 0.5f;
				}
				dam = (int)(dam * player.ActualClassDamage(DamageClass.Melee));

				if (player.velocity.Y == 0)
					Projectile.NewProjectile(player.GetSource_EffectItem<DeerclawpsEffect>(), pos, vel, type, dam, 4f, Main.myPlayer, ai0, ai1);
				else
					Projectile.NewProjectile(player.GetSource_EffectItem<DeerclawpsEffect>(), pos, vel * (Main.rand.NextBool() ? 1 : -1), type, dam, 4f, Main.myPlayer, ai0, ai1 / 2);
			}
		}
	}
}
