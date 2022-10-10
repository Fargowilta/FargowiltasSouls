using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    [AutoloadEquip(EquipType.Shield)]
    public class IronEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Iron Enchantment");

            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "铁魔石");

            string tooltip =
@"Right Click to guard with your shield
Guard just before being hit to negate damage
You attract items from a larger range
'Strike while the iron is hot'";
            Tooltip.SetDefault(tooltip);

            //             string tooltip_ch =
            // @"右键进行盾牌格挡
            // 如果时机正确则抵消这次伤害
            // 扩大你的拾取范围
            // '趁热打铁'";
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, tooltip_ch);
        }

        protected override Color nameColor => new Color(152, 142, 131);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Green;
            Item.value = 40000;
            //Item.shieldSlot = 5;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            if (player.GetToggleValue("IronS"))
            {
                //shield
                modPlayer.IronEffect();
            }
            //magnet
            if (player.GetToggleValue("IronM", false))
            {
                modPlayer.IronEnchantActive = true;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.IronHelmet)
            .AddIngredient(ItemID.IronChainmail)
            .AddIngredient(ItemID.IronGreaves)
            .AddIngredient(ItemID.EmptyBucket)
            .AddIngredient(ItemID.IronBroadsword)
            .AddIngredient(ItemID.IronAnvil)
            //.AddIngredient(ItemID.IronBow);
            //apricot (high in iron pog)
            //.AddIngredient(ItemID.TreasureMagnet)

            .AddTile(TileID.DemonAltar)
            .Register();

        }
    }
}
