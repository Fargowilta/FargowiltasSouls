using FargowiltasSouls.Toggler;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    public class FrigidGemstone : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frigid Gemstone");
            Tooltip.SetDefault(@"Grants immunity to Frostburn
Your attacks summon Frostfireballs to attack your enemies
'A shard of ancient magical ice'");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "寒玉");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, @"'一块古老的魔法冰碎片'
            // 免疫寒焰
            // 攻击召唤霜火球攻击敌人");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(0, 4);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[BuffID.Frostburn] = true;
            if (player.GetToggleValue("MasoFrigid"))
            {
                FargoSoulsPlayer fargoPlayer = player.GetModPlayer<FargoSoulsPlayer>();
                fargoPlayer.FrigidGemstoneItem = Item;
                if (fargoPlayer.FrigidGemstoneCD > 0)
                    fargoPlayer.FrigidGemstoneCD--;
            }
        }
    }
}