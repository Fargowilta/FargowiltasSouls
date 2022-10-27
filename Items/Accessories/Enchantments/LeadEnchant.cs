using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class LeadEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Lead Enchantment");

            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "铅魔石");

            string tooltip =
@"You take 50% less from damage over time
Attacks inflict enemies with Lead Poisoning
Lead Poisoning deals damage over time and spreads to nearby enemies
'Not recommended for eating'";
            Tooltip.SetDefault(tooltip);
            //             string tooltip_ch =
            // @"攻击有几率造成铅中毒减益
            // 铅中毒减益持续造成伤害并且会扩散至周围的敌人
            // '不建议食用'";
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, tooltip_ch);
        }

        protected override Color nameColor => new Color(67, 69, 88);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Blue;
            Item.value = 20000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            LeadEffect(player);
        }

        public static void LeadEffect(Player player)
        {
            player.GetModPlayer<FargoSoulsPlayer>().LeadEnchantActive = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.LeadHelmet)
                .AddIngredient(ItemID.LeadChainmail)
                .AddIngredient(ItemID.LeadGreaves)
                .AddIngredient(ItemID.LeadShortsword)
                .AddIngredient(ItemID.BlackPaint, 100)
                .AddIngredient(ItemID.GrayPaint, 100)

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}
