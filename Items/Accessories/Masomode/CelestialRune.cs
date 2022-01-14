using Terraria;
using Terraria.Localization;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    public class CelestialRune : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("Celestial Rune");
            Tooltip.SetDefault("Grants immunity to Marked for Death" +
                "\nYou may periodically fire additional attacks depending on weapon type" +
                "\nTaking damage creates a friendly Ancient Vision to attack enemies" +
                "\n'A fallen enemy's spells, repurposed'");

            DisplayName.AddTranslation(GameCulture.Chinese, "天界符文");
            Tooltip.AddTranslation(GameCulture.Chinese, "'堕落的敌人的咒语,被改换用途'" +
                "\n免疫死亡标记" +
                "\n根据武器类型定期发动额外的攻击" +
                "\n受伤时创造一个友好的远古幻象来攻击敌人");*/
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.rare = ItemRarityID.Cyan;
            item.value = Item.sellPrice(gold: 7);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[mod.BuffType("MarkedforDeath")] = true;
            player.GetModPlayer<FargoPlayer>().CelestialRune = true;
            player.GetModPlayer<FargoPlayer>().AdditionalAttacks = true;
        }
    }
}