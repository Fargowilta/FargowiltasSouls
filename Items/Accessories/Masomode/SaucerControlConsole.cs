using FargowiltasSouls.Toggler;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    public class SaucerControlConsole : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Saucer Control Console");
            Tooltip.SetDefault(@"Grants immunity to Electrified
Summons a friendly Mini Saucer
'Just keep it in airplane mode'");
            DisplayName.AddTranslation(GameCulture.Chinese, "飞碟控制台");
            Tooltip.AddTranslation(GameCulture.Chinese, @"免疫带电
召唤一个友善的迷你飞碟
'保持在飞行模式'");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.rare = ItemRarityID.Yellow;
            item.value = Item.sellPrice(0, 6);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[BuffID.Electrified] = true;
            if (player.GetToggleValue("MasoUfo"))
                player.AddBuff(mod.BuffType("SaucerMinion"), 2);
        }
    }
}
