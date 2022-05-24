using FargowiltasSouls.Buffs.Masomode;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    public class CelestialRune : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Celestial Rune");
            Tooltip.SetDefault("Grants immunity to Marked for Death" +
                "\nYou may periodically fire additional attacks depending on weapon type" +
                "\nTaking damage creates a friendly Ancient Vision to attack enemies" +
                "\n'A fallen enemy's spells, repurposed'");

            // DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "天界符文");
            // Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "'堕落的敌人的咒语,被改换用途'" +
            //     "\n免疫死亡标记" +
            //     "\n根据武器类型定期发动额外的攻击" +
            //     "\n受伤时创造一个友好的远古幻象来攻击敌人");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Cyan;
            Item.value = Item.sellPrice(gold: 7);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[ModContent.BuffType<MarkedforDeath>()] = true;
            player.GetModPlayer<FargoSoulsPlayer>().CelestialRuneItem = Item;
            player.GetModPlayer<FargoSoulsPlayer>().AdditionalAttacks = true;
        }
    }
}