using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Summons
{
    public class DevisCurse : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Deviantt's Curse");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(4, 7));
            DisplayName.AddTranslation(GameCulture.Chinese, "戴维安的诅咒");
        }

        /*public override bool Autoload(ref string name)
        {
            return false;
        }*/

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = ItemRarityID.LightRed;
            item.maxStack = 999;
            item.useAnimation = 30;
            item.useTime = 30;
            item.useStyle = ItemUseStyleID.HoldingUp;
            ItemID.Sets.ItemNoGravity[item.type] = true;
            item.consumable = true;
            item.value = Item.buyPrice(0, 2);
            item.noUseGraphic = true;
        }
        public override Color? GetAlpha(Color lightColor) => Color.White;

        public override bool UseItem(Player player)
        {
            int mutant = NPC.FindFirstNPC(ModLoader.GetMod("Fargowiltas").NPCType("Deviantt"));
            if (mutant > -1 && Main.npc[mutant].active)
            {
                Main.npc[mutant].Transform(mod.NPCType("DeviBoss"));
                if (Main.netMode == NetmodeID.SinglePlayer)
                    Main.NewText("Deviantt has awoken!", 175, 75, 255);
                else if (Main.netMode == NetmodeID.Server)
                    NetMessage.BroadcastChatMessage(NetworkText.FromLiteral("Deviantt has awoken!"), new Color(175, 75, 255));
            }
            else
            {
                NPC.SpawnOnPlayer(player.whoAmI, mod.NPCType("DeviBoss"));
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Gel);
            recipe.AddIngredient(ItemID.Lens);
            recipe.AddIngredient(ItemID.RottenChunk);
            recipe.AddIngredient(ItemID.Stinger);
            //recipe.AddIngredient(ItemID.Bone);
            recipe.AddIngredient(ItemID.HellstoneBar);
            //recipe.AddIngredient(mod.ItemType("CrackedGem"), 5);

            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();

            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Gel);
            recipe.AddIngredient(ItemID.Lens);
            recipe.AddIngredient(ItemID.Vertebrae);
            recipe.AddIngredient(ItemID.Stinger);
            //recipe.AddIngredient(ItemID.Bone);
            recipe.AddIngredient(ItemID.HellstoneBar);
            //recipe.AddIngredient(mod.ItemType("CrackedGem"), 5);

            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
