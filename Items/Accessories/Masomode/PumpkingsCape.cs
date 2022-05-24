using FargowiltasSouls.Buffs.Masomode;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    [AutoloadEquip(EquipType.Front, EquipType.Back)]
    public class PumpkingsCape : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pumpking's Cape");
            Tooltip.SetDefault(@"Grants immunity to Living Wasteland
Increases damage and critical strike chance by 5%
Your critical strikes inflict Rotting
You may periodically fire additional attacks depending on weapon type
'Somehow, it's the right size'");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "南瓜王的披肩");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, @"'不知怎么的,它的尺寸正好合适'
            // 免疫人形废土
            // 增加5%伤害和暴击率
            // 暴击造成腐败
            // 根据武器类型定期发动额外的攻击");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(0, 6);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer fargoPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            player.GetDamage(DamageClass.Generic) += 0.05f;
            player.GetCritChance(DamageClass.Generic) += 5;
            fargoPlayer.PumpkingsCapeItem = Item;
            fargoPlayer.AdditionalAttacks = true;
            player.buffImmune[ModContent.BuffType<LivingWasteland>()] = true;
        }
    }
}