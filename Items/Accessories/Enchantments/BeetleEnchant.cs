using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class BeetleEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Beetle Enchantment");

            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "甲虫魔石");

            string tooltip =
@"Beetles increase your damage and melee speed
When hit, beetles instead protect you from damage for 10 seconds
Beetle buffs capped at level two
'The unseen life of dung courses through your veins'";
            Tooltip.SetDefault(tooltip);

            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "甲虫魔石");

            //             string tooltip =
            // @"Beetles protect you from damage, up to 15% damage reduction only
            // Increases flight time by 25%
            // 'The unseen life of dung courses through your veins'";
            //             Tooltip.SetDefault(tooltip);

            //             string tooltip_ch =
            // @"甲虫会保护你，减免下次受到的伤害，至多减免15%下次受到的伤害
            // 延长25%飞行时间
            // '你的血管里流淌着看不见的粪便生命'";
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, tooltip_ch);
        }

        protected override Color nameColor => new Color(109, 92, 133);
        public override string wizardEffect => "";

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Yellow;
            Item.value = 250000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            //beetle bois
            modPlayer.BeetleEffect();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.BeetleHelmet)
            .AddRecipeGroup("FargowiltasSouls:AnyBeetle")
            .AddIngredient(ItemID.BeetleLeggings)
            .AddIngredient(ItemID.BeetleWings)
            .AddIngredient(ItemID.BeeWings)
            .AddIngredient(ItemID.ButterflyWings)
            //.AddIngredient(ItemID.MothronWings);
            //breaker blade
            //amarok
            //beetle minecart

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
}
