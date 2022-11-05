using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class DarkArtistEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Dark Artist Enchantment");

            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "暗黑艺术家魔石");

            string tooltip =
@"Summons a Flameburst minion that will travel to your mouse after charging up
It will then act as a sentry
After attacking for 2 seconds you will be enveloped in flames
Switching weapons will increase the next attack's damage by 50% and spawn an inferno
Greatly enhances Flameburst effectiveness
'The shadows hold more than they seem'";

            Tooltip.SetDefault(tooltip);
            //             string tooltip_ch =
            // @"召唤一个爆炸烈焰哨兵，在充能完毕后会移动至光标位置
            // 然后将其算作一个哨兵
            // 持续攻击两秒后你将被火焰包裹
            // 切换武器后使下次攻击的伤害增加50%
            // 大幅强化爆炸烈焰哨兵的效果
            // '阴影蕴含之物远超目之所及'";
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, tooltip_ch);
        }

        protected override Color nameColor => new Color(155, 92, 176);
        public override string wizardEffect => Language.GetTextValue("Mods.FargowiltasSouls.WizardEffect.DarkArtist");

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Yellow;
            Item.value = 250000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            modPlayer.DarkArtistEffect(hideVisual);
            modPlayer.ApprenticeEffect();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.ApprenticeAltHead)
            .AddIngredient(ItemID.ApprenticeAltShirt)
            .AddIngredient(ItemID.ApprenticeAltPants)
            .AddIngredient(null, "ApprenticeEnchant")
            .AddIngredient(ItemID.DD2FlameburstTowerT3Popper)
            //.AddIngredient(ItemID.ShadowbeamStaff);
            .AddIngredient(ItemID.InfernoFork)
            //Razorpine
            //staff of earth
            //.AddIngredient(ItemID.DD2PetGhost);

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
}
