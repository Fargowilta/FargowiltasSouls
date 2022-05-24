using FargowiltasSouls.Projectiles.Minions;
using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
//using FargowiltasSouls.Projectiles.Minions;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class SilverEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Silver Enchantment");

            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "银魔石");

            string tooltip =
@"Summons a sword familiar that scales with minion damage
Drastically increases minion speed
Reduces minion damage to compensate for increased speed
'Have you power enough to wield me?'";
            Tooltip.SetDefault(tooltip);

            //             string tooltip_ch =
            // @"召唤一柄剑，剑的伤害取决于你的召唤伤害
            // '你有足够的力量驾驭我吗？'";
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, tooltip_ch);

        }

        protected override Color nameColor => new Color(180, 180, 204);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Blue;
            Item.value = 30000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            modPlayer.SilverEnchantActive = true;
            modPlayer.AddMinion(Item, player.GetToggleValue("Silver"), ModContent.ProjectileType<SilverSword>(), 20, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ItemID.SilverHelmet)
            .AddIngredient(ItemID.SilverChainmail)
            .AddIngredient(ItemID.SilverGreaves)
            .AddIngredient(ItemID.SilverBroadsword)
            //.AddIngredient(ItemID.SilverBow);
            .AddIngredient(ItemID.SapphireStaff)
            .AddIngredient(ItemID.BluePhaseblade)
            //leather whip
            //.AddIngredient(ItemID.TreeNymphButterfly);
            //roasted duck

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}
