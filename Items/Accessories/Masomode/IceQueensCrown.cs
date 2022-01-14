using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Toggler;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    [AutoloadEquip(EquipType.Face)]
    public class IceQueensCrown : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("Ice Queen's Crown");
            Tooltip.SetDefault(@"Grants immunity to Frozen and Hypothermia
Increases damage reduction by 5%
Freeze nearby enemies when hurt
Summons a friendly super Flocko
'The royal symbol of a defeated foe'");
            DisplayName.AddTranslation(GameCulture.Chinese, "冰雪女王的皇冠");
            Tooltip.AddTranslation(GameCulture.Chinese, @"'被打败的敌人的皇家象征'
免疫冻结
增加5%伤害减免
召唤一个友善的超级圣诞雪灵");*/
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.rare = ItemRarityID.Yellow;
            item.value = Item.sellPrice(0, 6);
            item.defense = 5;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.endurance += 0.05f;
            player.buffImmune[BuffID.Frozen] = true;
            player.buffImmune[ModContent.BuffType<Buffs.Masomode.Hypothermia>()] = true;
            player.GetModPlayer<FargoPlayer>().IceQueensCrown = true;
            if (player.GetToggleValue("MasoFlocko"))
                player.AddBuff(mod.BuffType("SuperFlocko"), 2);
        }
    }
}