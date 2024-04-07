using FargowiltasSouls.Content.Buffs.Minions;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Masomode
{
	[AutoloadEquip(EquipType.Neck)]
	public class SkullCharm : SoulsItem
	{
		public override bool Eternity => true;

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Skull Charm");
			/* Tooltip.SetDefault(@"Grants immunity to Dazed
Increases damage dealt by 15% and damage taken by 10%
Enemies are less likely to target you
The crystal skull charges energy to attack as you attack
'No longer in the zone'"); */
			//             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "头骨挂坠");
			//             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, @"'不在这个区域了'
			// 免疫眩晕
			// 增加10%所受和造成的伤害
			// 敌人不太可能以你为目标
			// 地牢外的装甲和魔法骷髅敌意减小");

			Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.accessory = true;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.sellPrice(0, 6);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.buffImmune[BuffID.Dazed] = true;
			player.GetDamage(DamageClass.Generic) += 0.15f;
			player.endurance -= 0.1f;
			player.aggro -= 400;
			player.FargoSouls().SkullCharm = true;
			/*if (!player.ZoneDungeon)
            {
                player.npcTypeNoAggro[NPCID.SkeletonSniper] = true;
                player.npcTypeNoAggro[NPCID.SkeletonCommando] = true;
                player.npcTypeNoAggro[NPCID.TacticalSkeleton] = true;
                player.npcTypeNoAggro[NPCID.DiabolistRed] = true;
                player.npcTypeNoAggro[NPCID.DiabolistWhite] = true;
                player.npcTypeNoAggro[NPCID.Necromancer] = true;
                player.npcTypeNoAggro[NPCID.NecromancerArmored] = true;
                player.npcTypeNoAggro[NPCID.RaggedCaster] = true;
                player.npcTypeNoAggro[NPCID.RaggedCasterOpenCoat] = true;
            }*/
			player.AddEffect<PungentMinion>(Item);

		}
	}
	public class PungentMinion : AccessoryEffect
	{
		public override Header ToggleHeader => Header.GetHeader<LumpofFleshHeader>();
		public override int ToggleItemType => ModContent.ItemType<SkullCharm>();
		public override bool MinionEffect => true;
		public override void PostUpdateEquips(Player player)
		{
			if (!player.FargoSouls().LumpOfFlesh && !player.HasBuff<SouloftheMasochistBuff>())
				player.AddBuff(ModContent.BuffType<Buffs.Minions.CrystalSkullBuff>(), 5);
		}
	}
}