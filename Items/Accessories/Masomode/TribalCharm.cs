using FargowiltasSouls.Toggler;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    [AutoloadEquip(EquipType.Waist)]
    public class TribalCharm : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tribal Charm");
            Tooltip.SetDefault(@"When attacking by manually clicking, increases non-summon damage by 30%
Grants immunity to Webbed and Purified
Grants autofire to all weapons (this effect also works in your inventory)
'An idol of the ancient jungle dwellers'");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "部落挂坠");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, @"'远古丛林居民的偶像'
            // 免疫织网和净化
            // 所有武器自动连发");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(0, 4);
            Item.defense = 6;
        }

        public override void UpdateInventory(Player player)
        {
            player.GetModPlayer<FargoSoulsPlayer>().TribalCharm = true;
        }

        public override void UpdateVanity(Player player)
        {
            player.GetModPlayer<FargoSoulsPlayer>().TribalCharm = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[BuffID.Webbed] = true;
            player.buffImmune[ModContent.BuffType<Buffs.Masomode.Purified>()] = true;
            player.GetModPlayer<FargoSoulsPlayer>().TribalCharm = true;
            player.GetModPlayer<FargoSoulsPlayer>().TribalCharmEquipped = true;
        }
    }
}