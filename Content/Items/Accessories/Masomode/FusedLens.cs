using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Masomode
{
    public class FusedLens : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Fused Lens");
            /* Tooltip.SetDefault(@"Grants immunity to Cursed Inferno and Ichor
When inflicted with Cursed Inferno, 15% increased attack speed
When inflicted with Ichor, 15% increased critical strike chance
When losing health to damage over time, you inflict Cursed Inferno and Ichor
Press the Debuff Install key to inflict yourself with Cursed Inferno and Ichor for 30 seconds
'Too melted to improve vision'"); */
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "融合晶状体");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, @"'融化过度,无法改善视力'
            // 免疫诅咒地狱和脓液
            // 攻击造成诅咒地狱和脓液");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.sellPrice(0, 4);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[BuffID.CursedInferno] = true;
            player.buffImmune[BuffID.Ichor] = true;

            player.FargoSouls().FusedLens = true;
            player.AddEffect<FusedLensInstall>(Item);

            if (player.onFire2)
                player.FargoSouls().AttackSpeed += 0.15f;
            if (player.ichor)
                player.GetCritChance(DamageClass.Generic) += 15;
        }
    }
    public class FusedLensInstall : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<DubiousHeader>();
        public override int ToggleItemType => ModContent.ItemType<FusedLens>();
        public override bool IgnoresMutantPresence => true;
    }
}