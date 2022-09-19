using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.Toggler;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    public class SinisterIcon : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sinister Icon");
            Tooltip.SetDefault(@"Grants immunity to Unlucky and Stunned
Increases spawn rate
Enemies with 2000 or less max life will drop doubled loot
'Most definitely not alive'");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "邪恶画像");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, @"'肯定不是活着的'
            // 阻止受虐模式导致的Boss自然生成
            // 提高刷怪速率
            // 小于等于2000血量的敌人掉落双倍物品，但不掉落钱币");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(0, 3);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[ModContent.BuffType<Unlucky>()] = true;
            player.buffImmune[ModContent.BuffType<Stunned>()] = true;

            if (player.GetToggleValue("MasoIcon"))
                player.GetModPlayer<FargoSoulsPlayer>().SinisterIcon = true;

            if (player.GetToggleValue("MasoIconDrops"))
                player.GetModPlayer<FargoSoulsPlayer>().SinisterIconDrops = true;

            //player.GetModPlayer<FargoSoulsPlayer>().Graze = true;
        }
    }
}