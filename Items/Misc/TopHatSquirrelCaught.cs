using FargowiltasSouls.NPCs.Critters;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Misc
{
    public class TopHatSquirrelCaught : SoulsItem
    {
        public override string Texture => "FargowiltasSouls/Items/Weapons/Misc/TophatSquirrelWeapon";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Top Hat Squirrel");
            DisplayName.AddTranslation(GameCulture.Chinese, "高顶礼帽松鼠"); 
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.maxStack = 10;
            item.rare = ItemRarityID.Blue;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useAnimation = 15;
            item.useTime = 10;
            item.consumable = true;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.UseSound = SoundID.Item44;
            item.makeNPC = (short)ModContent.NPCType<TophatSquirrelCritter>();
            //Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(6, Main.npcFrameCount[ModContent.NPCType<TophatSquirrelCritter>()]));
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("FargowiltasSouls:AnySquirrel");
            recipe.AddIngredient(ItemID.TopHat);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
