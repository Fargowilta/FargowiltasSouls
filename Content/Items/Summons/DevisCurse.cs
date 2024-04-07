using FargowiltasSouls.Content.Bosses.DeviBoss;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Summons
{
    public class DevisCurse : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Deviantt's Curse");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 7));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
        }

        /*public override bool Autoload(ref string name)
        {
            return false;
        }*/

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.LightRed;
            Item.maxStack = 20;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = true;
            Item.value = Item.buyPrice(0, 2);
        }
        public override Color? GetAlpha(Color lightColor) => Color.White;

        public override bool? UseItem(Player player)
        {
            FargoSoulsUtil.SpawnBossNetcoded(player, ModContent.NPCType<DeviBoss>());
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.PinkGel)
            .AddRecipeGroup("FargowiltasSouls:AnyGoldOre")
            .AddRecipeGroup("FargowiltasSouls:AnyRottenChunk")
            .AddIngredient(ItemID.Stinger)
            .AddIngredient(ItemID.ChumBucket)
            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}