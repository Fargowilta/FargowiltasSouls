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
            /*DisplayName.SetDefault("Tribal Charm");
            Tooltip.SetDefault(@"Works in your inventory
Grants immunity to Webbed and Purified
Grants autofire to all weapons
'An idol of the ancient jungle dwellers'");
            DisplayName.AddTranslation(GameCulture.Chinese, "部落挂坠");
            Tooltip.AddTranslation(GameCulture.Chinese, @"'远古丛林居民的偶像'
免疫织网和净化
所有武器自动连发");*/
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