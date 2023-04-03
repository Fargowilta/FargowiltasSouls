using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Souls
{
    [AutoloadEquip(EquipType.Wings)]
    public class FlightMasterySoul : FlightMasteryWings
    {
        protected override bool HasSupersonicSpeed => false;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Flight Mastery Soul");
            Tooltip.SetDefault(
@"Allows for infinite flight
Hold DOWN and JUMP to hover
Hold UP to boost faster
Increases gravity
Allows the control of gravity
'Ascend'");
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "飞行大师之魂");
            //Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese,
//@"使你获得无限飞行能力
//按住'下'和'跳跃'键悬停
//允许你控制重力
//'飞升'");
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "飞行大师之魂");

        }

        protected override Color? nameColor => new Color(56, 134, 255);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            modPlayer.FlightMasterySoul();
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ItemID.EmpressFlightBooster) //soaring insignia

            .AddIngredient(ItemID.BatWings) //bat wings
            .AddIngredient(ItemID.CreativeWings) //fledgling wings
            .AddIngredient(ItemID.FairyWings)
            .AddIngredient(ItemID.HarpyWings)
            .AddIngredient(ItemID.BoneWings)
            .AddIngredient(ItemID.FrozenWings)
            .AddIngredient(ItemID.FlameWings)
            .AddIngredient(ItemID.TatteredFairyWings)
            .AddIngredient(ItemID.FestiveWings)
            .AddIngredient(ItemID.BetsyWings)
            .AddIngredient(ItemID.FishronWings)
            .AddIngredient(ItemID.RainbowWings) //empress wings
            .AddIngredient(ItemID.LongRainbowTrailWings) //celestial starboard

            .AddIngredient(ItemID.GravityGlobe)

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))


            .Register();
        }
    }
}
