using FargowiltasSouls.Toggler;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    public class SaucerControlConsole : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Saucer Control Console");
            Tooltip.SetDefault(@"Grants immunity to Electrified and Distorted
Summons a friendly Mini Saucer
The saucer's tractor beam slows enemies hit by it
'Just keep it in airplane mode'");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "飞碟控制台");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, @"'保持在飞行模式'
            // 免疫带电
            // 召唤一个友善的迷你飞碟");

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
            player.buffImmune[BuffID.Electrified] = true;
            player.buffImmune[BuffID.VortexDebuff] = true;
            if (player.GetToggleValue("MasoUfo"))
                player.AddBuff(Terraria.ModLoader.ModContent.BuffType<Buffs.Minions.SaucerMinion>(), 2);
        }
    }
}