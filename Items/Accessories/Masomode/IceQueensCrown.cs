using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.Toggler;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    [AutoloadEquip(EquipType.Face)]
    public class IceQueensCrown : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ice Queen's Crown");
            Tooltip.SetDefault(@"Grants immunity to Hypothermia
Increases damage reduction by 5%
Freeze nearby enemies when hurt
Summons a friendly super Flocko
'The royal symbol of a defeated foe'");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "冰雪女王的皇冠");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, @"'被打败的敌人的皇家象征'
            // 免疫冻结
            // 增加5%伤害减免
            // 召唤一个友善的超级圣诞雪灵");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(0, 6);
            Item.defense = 5;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.endurance += 0.05f;
            player.buffImmune[ModContent.BuffType<Hypothermia>()] = true;
            player.GetModPlayer<FargoSoulsPlayer>().IceQueensCrown = true;
            if (player.GetToggleValue("MasoFlocko"))
                player.AddBuff(ModContent.BuffType<Buffs.Minions.SuperFlocko>(), 2);
        }
    }
}