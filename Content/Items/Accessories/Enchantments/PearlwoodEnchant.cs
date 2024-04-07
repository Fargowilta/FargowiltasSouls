
using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
	public class PearlwoodEnchant : BaseEnchant
	{
		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();

			// DisplayName.SetDefault("Pearlwood Enchantment");
			/* Tooltip.SetDefault(
@"Attacks may spawn a homing star when they hit something
'Too little, too late…'"); */
		}

		public override Color nameColor => new(173, 154, 95);


		public override void SetDefaults()
		{
			base.SetDefaults();

			Item.rare = ItemRarityID.Pink;
			Item.value = 20000;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.AddEffect<PearlwoodEffect>(Item);
		}
		public override void AddRecipes()
		{
			CreateRecipe()
			.AddIngredient(ItemID.PearlwoodHelmet)
			.AddIngredient(ItemID.PearlwoodBreastplate)
			.AddIngredient(ItemID.PearlwoodGreaves)
			.AddIngredient(ItemID.PearlwoodSword)
			.AddIngredient(ItemID.LightningBug)
			.AddIngredient(ItemID.Starfruit)

			.AddTile(TileID.CrystalBall)
			.Register();
		}
	}
	public class PearlwoodEffect : AccessoryEffect
	{

		public override Header ToggleHeader => Header.GetHeader<TimberHeader>();
		public override int ToggleItemType => ModContent.ItemType<PearlwoodEnchant>();
		public override void PostUpdateEquips(Player player)
		{
			PearlwoodStar(player, EffectItem(player));
		}
		public static void PearlwoodStar(Player player, Item item)
		{
			FargoSoulsPlayer modPlayer = player.FargoSouls();

			if (!modPlayer.PearlwoodStar)
				return;

			player.AddBuff(ModContent.BuffType<PearlwoodStarBuff>(), 2);

			if (modPlayer.PearlwoodTrail[modPlayer.PearlwoodIndex] != Vector2.Zero) //check if trail actually exists
			{
				modPlayer.PStarelinePos = modPlayer.PearlwoodTrail[modPlayer.PearlwoodIndex]; //set stareline position

				if (!modPlayer.PStarelineActive) //check if stareline is active
				{
					if (modPlayer.PearlwoodGrace == 120) //spawn after 2 seconds 
					{
						Projectile.NewProjectile(player.GetSource_EffectItem<PearlwoodEffect>(), modPlayer.PStarelinePos, Vector2.Zero, ProjectileID.FairyQueenMagicItemShot, 1000, 0f);
						//modPlayer.PStarelineActive = true;
						modPlayer.PearlwoodGrace = 0;
					}
					else
					{
						if (player.velocity.Length() > 0)
							modPlayer.PearlwoodGrace += 1;
					}
				}
			}


			modPlayer.PearlwoodTrail[modPlayer.PearlwoodIndex] = player.Center; //set position of stareline next cycle

			modPlayer.PearlwoodIndex++; //read next in array
			if (modPlayer.PearlwoodIndex >= modPlayer.PearlwoodTrail.Length) { modPlayer.PearlwoodIndex = 0; } //loop around
		}
		public override void OnHitNPCEither(Player player, NPC target, NPC.HitInfo hitInfo, DamageClass damageClass, int baseDamage, Projectile projectile, Item item)
		{
			if (!player.FargoSouls().PStarelineActive)
				return;

			if (hitInfo.Crit)
			{
				SoundEngine.PlaySound(SoundID.Item25, target.position);
				for (int i = 0; i < 30; i++)
				{ //idk how to make dust look good (3)
					Dust.NewDust(target.position, target.width, target.height, DustID.YellowStarDust);
				}
			}
		}
		public override void ModifyHitNPCWithProj(Player player, Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
		{
			PearlwoodCritReroll(player, ref modifiers, proj.DamageType);
		}
		public override void ModifyHitNPCWithItem(Player player, Item item, NPC target, ref NPC.HitModifiers modifiers)
		{
			PearlwoodCritReroll(player, ref modifiers, item.DamageType);
		}
		public static void PearlwoodCritReroll(Player player, ref NPC.HitModifiers modifiers, DamageClass damageClass)
		{
			FargoSoulsPlayer modPlayer = player.FargoSouls();

			if (!modPlayer.PStarelineActive)
				return;

			int rerolls = modPlayer.ForceEffect<PearlwoodEnchant>() ? 2 : 1;
			for (int i = 0; i < rerolls; i++)
			{
				if (Main.rand.Next(0, 100) <= player.ActualClassCrit(damageClass))
				{
					modifiers.SetCrit();
				}
			}

		}
	}
}
