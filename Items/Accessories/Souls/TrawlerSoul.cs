using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Souls
{
    //[AutoloadEquip(EquipType.Back)]
    public class TrawlerSoul : BaseSoul
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Trawler Soul");

            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "捕鱼之魂");

            string tooltip =
@"Increases fishing skill substantially
All fishing rods will have 10 extra lures
You catch fish almost instantly
Permanent Sonar and Crate Buffs
Effects of Angler Tackle Bag and Spore Sac 
Effects of Pink Horseshoe Balloon and Arctic Diving Gear
'The fish catch themselves'";
            Tooltip.SetDefault(tooltip);

            //string tooltip_ch =
//@"大幅增加渔力
//钓竿会额外扔出10根鱼线
//你几乎能立刻就钓到鱼
//拥有声呐和宝匣效果
//拥有渔夫渔具袋和狍子囊效果
//拥有粉马掌气球和北极潜水装备效果
//'愿者上钩'";
            //Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, tooltip_ch);
        
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.value = 750000;
        }

        protected override Color? nameColor => new Color(0, 238, 125);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            modPlayer.TrawlerSoul(Item, hideVisual);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(null, "AnglerEnchant")
            //inner tube
            .AddIngredient(ItemID.BalloonHorseshoeSharkron)
            .AddIngredient(ItemID.ArcticDivingGear)
            //frog gear
            //volatile gel
            .AddIngredient(ItemID.SporeSac)

            //engineer rod
            .AddIngredient(ItemID.SittingDucksFishingRod)
            //hotline fishing
            .AddIngredient(ItemID.GoldenFishingRod)
            .AddIngredient(ItemID.GoldenCarp)
            .AddIngredient(ItemID.ReaverShark)
            .AddIngredient(ItemID.Bladetongue)
            .AddIngredient(ItemID.ObsidianSwordfish)
            .AddIngredient(ItemID.FuzzyCarrot)
            .AddIngredient(ItemID.HardySaddle)
            //.AddIngredient(ItemID.ZephyrFish);

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))


            .Register();
        }
    }
}
