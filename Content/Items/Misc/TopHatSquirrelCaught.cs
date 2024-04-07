using FargowiltasSouls.Content.NPCs.Critters;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Misc
{
    public class TopHatSquirrelCaught : SoulsItem
    {
        public override string Texture => "FargowiltasSouls/Content/Items/Weapons/Misc/TophatSquirrelWeapon";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Top Hat Squirrel");
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 10;
            Item.rare = ItemRarityID.Blue;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.consumable = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.UseSound = SoundID.Item44;
            Item.makeNPC = (short)ModContent.NPCType<TophatSquirrelCritter>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddRecipeGroup("FargowiltasSouls:AnySquirrel")
            .AddIngredient(ItemID.TopHat)
            .Register();
        }
    }
}
