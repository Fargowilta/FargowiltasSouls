using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class MoltenEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Molten Enchantment");
            Tooltip.SetDefault(
@"Grants immunity to fire and lava
You have normal movement and can swim in lava
Nearby enemies are ignited
The closer they are to you the more damage they take

While standing in lava or lava wet, your attacks spawn explosions
'They shall know the fury of hell' ");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "熔融魔石");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese,
            // @"引燃你附近的敌人
            // 离你越近的敌人受到的伤害越高
            // 你受到伤害时会剧烈爆炸并伤害附近的敌人
            // '他们将感受到地狱的愤怒' ");
        }

        protected override Color nameColor => new Color(193, 43, 43);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Orange;
            Item.value = 50000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoSoulsPlayer>().MoltenEffect();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.MoltenHelmet)
            .AddIngredient(ItemID.MoltenBreastplate)
            .AddIngredient(ItemID.MoltenGreaves)
            .AddIngredient(ItemID.Sunfury)
            //.AddIngredient(ItemID.MoltenFury);
            .AddIngredient(ItemID.PhoenixBlaster)
            //.AddIngredient(ItemID.DarkLance);
            .AddIngredient(ItemID.Lavafly)
            .AddIngredient(ItemID.DemonsEye)

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}
