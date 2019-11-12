using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using FargowiltasSouls.Items.Accessories.Enchantments.Thorium;
using Microsoft.Xna.Framework;
using ThoriumMod.Items.MeleeItems;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class BeetleEnchant : EnchantmentItem
    {
        public const string TOOLTIP =
            @"'The unseen life of dung courses through your veins'
Beetles protect you from damage
Increases flight time by 50%";


        public BeetleEnchant() : base("Beetle Enchantment", TOOLTIP, 20, 20, 
            TileID.CrystalBall, Item.sellPrice(gold: 5), ItemRarityID.Yellow, new Color(109, 92, 133))
        {
            
        }


        public override void SetStaticDefaults()
        {
            base.SetDefaults();

            string tooltip_ch = 
@"'你的血管里流淌着看不见的粪便生命'
甲虫保护你免受伤害
增加100%飞行时间";

            DisplayName.AddTranslation(GameCulture.Chinese, "甲虫魔石");
            Tooltip.AddTranslation(GameCulture.Chinese, tooltip_ch);
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();
            //defense beetle bois
            modPlayer.BeetleEffect();
            modPlayer.wingTimeModifier += .5f;
        }


        protected override void AddRecipeBase(ModRecipe recipe)
        {
            recipe.AddIngredient(ItemID.BeetleHelmet);
            recipe.AddRecipeGroup("FargowiltasSouls:AnyBeetle");
            recipe.AddIngredient(ItemID.BeetleLeggings);

            recipe.AddIngredient(ItemID.BeetleWings);
            recipe.AddIngredient(ItemID.BeeWings);
            recipe.AddIngredient(ItemID.ButterflyWings);
            recipe.AddIngredient(ItemID.MothronWings);
        }

        protected override void AddThoriumRecipe(ModRecipe recipe, Mod thorium)
        {
            recipe.AddIngredient(ModContent.ItemType<FlightEnchant>());
            recipe.AddIngredient(ModContent.ItemType<SolScorchedSlab>());

            recipe.AddIngredient(ItemID.GolemFist);
        }
    }
}
