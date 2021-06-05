using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    [AutoloadEquip(EquipType.Waist)]
    public class TribalCharm : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tribal Charm");
            Tooltip.SetDefault(@"Works in your inventory
Grants immunity to Webbed and Purified
Grants autofire to all weapons
'An idol of the ancient jungle dwellers'");
            DisplayName.AddTranslation(GameCulture.Chinese, "部落护符");
            Tooltip.AddTranslation(GameCulture.Chinese, @"放置在背包中即可生效
使你免疫被网住和净化减益
允许所有武器自动挥舞
'远古丛林居民所崇拜之物的小神像'");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.rare = ItemRarityID.Pink;
            item.value = Item.sellPrice(0, 4);
            item.defense = 6;
        }

        public override void UpdateInventory(Player player)
        {
            player.GetModPlayer<FargoPlayer>().TribalCharm = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[BuffID.Webbed] = true;
            player.buffImmune[mod.BuffType("Purified")] = true;
            player.GetModPlayer<FargoPlayer>().TribalCharm = true;
        }
    }
}
