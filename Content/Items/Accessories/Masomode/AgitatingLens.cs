using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Projectiles.Masomode;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Masomode
{
	public class AgitatingLens : SoulsItem
	{
		public override bool Eternity => true;

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Agitating Lens");
			/* Tooltip.SetDefault("Grants immunity to Berserked" +
                "\nWhile dashing or running quickly you will create a trail of demon scythes" +
                "\nPress the Debuff Install key while holding UP and DOWN to go berserk" +
                "\nWhen berserk, massively increased offenses, massively lowered defenses, and you cannot stop attacking or moving" +
                "\nBerserk state lasts for 7.5 seconds and you are stunned for 2.5 seconds afterwards" +
                "\n'The irritable remnant of a defeated foe'"); */

			// DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "躁动晶状体");
			// Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "使你狂暴免疫减益" +
			//     "\n生命值低于50%时增加10%伤害" +
			//     "\n冲刺或奔跑时会在身后留下一串恶魔镰刀" +
			//     "\n'被打败的敌人的躁动残渣'");

			Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.accessory = true;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.sellPrice(gold: 1);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.buffImmune[ModContent.BuffType<BerserkedBuff>()] = true;

			player.AddEffect<AgitatingLensEffect>(Item);
			player.AddEffect<AgitatingLensInstall>(Item);
		}
	}
	public class AgitatingLensEffect : AccessoryEffect
	{
		public override Header ToggleHeader => Header.GetHeader<SupremeFairyHeader>();
		public override int ToggleItemType => ModContent.ItemType<AgitatingLens>();
		public override bool ExtraAttackEffect => true;
		public override void PostUpdateEquips(Player player)
		{
			FargoSoulsPlayer modPlayer = player.FargoSouls();
			if (modPlayer.AgitatingLensCD++ > 15)
			{
				modPlayer.AgitatingLensCD = 0;
				if ((Math.Abs(player.velocity.X) >= 5 || Math.Abs(player.velocity.Y) >= 5) && player.whoAmI == Main.myPlayer)
				{
					int damage = 18;
					if (modPlayer.SupremeDeathbringerFairy)
						damage *= 2;
					if (modPlayer.MasochistSoul)
						damage *= 2;
					damage = (int)(damage * player.ActualClassDamage(DamageClass.Magic));
					Projectile.NewProjectile(GetSource_EffectItem(player), player.Center, player.velocity * 0.1f, ModContent.ProjectileType<BloodScytheFriendly>(), damage, 5f, player.whoAmI);
				}
			}
		}
	}
	public class AgitatingLensInstall : AccessoryEffect
	{
		public override Header ToggleHeader => Header.GetHeader<SupremeFairyHeader>();
		public override int ToggleItemType => ModContent.ItemType<AgitatingLens>();
		public override bool IgnoresMutantPresence => true;
	}
}
