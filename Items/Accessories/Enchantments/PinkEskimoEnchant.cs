//using Terraria.ID;
//using Terraria.ModLoader;
//using Terraria.Localization;

//namespace FargowiltasSouls.Items.Accessories.Enchantments
//{
//    public class PinkEskimoEnchant : BaseEnchant
//    {
//        public override bool Autoload(ref string name)
//        {
//            return false;
//        }

//        public override void SetStaticDefaults()
//        {
//            base.SetStaticDefaults();
//
//            DisplayName.SetDefault("Pink Eskimo Enchantment");
//            Tooltip.SetDefault(
//@"''");
//            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "粉爱斯基摩魔石");
//            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese,
//@"''");
//        }

//        public override void SetDefaults()
//        {
//            base.SetDefaults();
//
//            item.rare = ItemRarityID.Lime;
//            item.value = 100000;
//        }

//        public override void UpdateAccessory(Player player, bool hideVisual)
//        {
//            /*
//             * if(player.walkingOnWater)
//{
//	Create Ice Rod Projectile right below you
//}

//NearbyEffects:

//if(modPlayer.EskimoEnchant && tile.type == IceRodBlock)
//{
//	Create spikes
//}
//             */
//        }

//        public override void AddRecipes()
//        {
//            CreateRecipe()

//            .AddIngredient(ItemID.PinkEskimoHood)
//            .AddIngredient(ItemID.PinkEskimoCoat)
//            .AddIngredient(ItemID.PinkEskimoPants)
//            //.AddIngredient(ItemID.IceRod);
//            .AddIngredient(ItemID.FrostMinnow)
//            .AddIngredient(ItemID.AtlanticCod)
//            .AddIngredient(ItemID.MarshmallowonaStick)

//            //grinch pet? or steal pets from frost??!

//            .AddTile(TileID.CrystalBall)
//            
//            .Register();
//        }
//    }
//}
