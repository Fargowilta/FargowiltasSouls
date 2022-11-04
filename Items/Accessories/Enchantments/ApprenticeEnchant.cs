using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class ApprenticeEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Apprentice Enchantment");
            Tooltip.SetDefault(
@"After attacking for 2 seconds you will be enveloped in flames
Switching weapons will increase the next attack's damage by 50% and spawn an inferno
Flameburst field of view and range are dramatically increased
'A long way to perfection'");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "学徒魔石");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, 
            // @"持续攻击两秒后你将被火焰包裹
            // 切换武器后使下次攻击的伤害增加50%
            // 大幅增加爆炸烈焰哨兵的索敌范围和攻击距离
            // '追求完美的漫漫长路'");
        }

        protected override Color nameColor => new Color(93, 134, 166);
        public override string wizardEffect => "";

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Yellow;
            Item.value = 150000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoSoulsPlayer>().ApprenticeEffect();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.ApprenticeHat)
            .AddIngredient(ItemID.ApprenticeRobe)
            .AddIngredient(ItemID.ApprenticeTrousers)
            //.AddIngredient(ItemID.ApprenticeScarf);
            .AddIngredient(ItemID.DD2FlameburstTowerT2Popper)
            //magic missile
            //ice rod
            //golden shower
            .AddIngredient(ItemID.BookStaff)
            .AddIngredient(ItemID.ClingerStaff)

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
}
