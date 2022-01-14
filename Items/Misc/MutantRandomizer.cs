using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Misc
{
    public class MutantRandomizer : SoulsItem
    {
        public override string Texture => "FargowiltasSouls/Items/Placeholder";

        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("???");
            Tooltip.SetDefault("Alters Eternity Mutant fight");*/
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = ItemRarityID.Purple;
            item.maxStack = 1;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.consumable = false;
            item.value = Item.sellPrice(copper: 1);
        }

        public override bool CanUseItem(Player player) => FargoSoulsWorld.EternityMode && !NPC.AnyNPCs(ModContent.NPCType<NPCs.MutantBoss.MutantBoss>());

        public override bool UseItem(Player player)
        {
            FargoSoulsWorld.SuppressRandomMutant = !FargoSoulsWorld.SuppressRandomMutant;
            string text = FargoSoulsWorld.SuppressRandomMutant ? Language.GetTextValue("Mods.FargowiltasSouls.MutantRandomizer.NotRandom") : Language.GetTextValue("Mods.FargowiltasSouls.MutantRandomizer.Random");
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText(text, Color.LimeGreen);
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.BroadcastChatMessage(NetworkText.FromLiteral(text), Color.LimeGreen);
                NetMessage.SendData(MessageID.WorldData); //sync world
            }
            Main.PlaySound(SoundID.Roar, (int)player.position.X, (int)player.position.Y, 0);
            return true;
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = new Color(Main.DiscoR, 51, 255 - (int)(Main.DiscoR * 0.4));
                }
            }
        }

        /*public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<AbomEnergy>(), 5);
            recipe.AddIngredient(ModContent.ItemType<Summons.AbominationnVoodooDoll>(), 1);
            recipe.AddTile(ModContent.TileType<CrucibleCosmosSheet>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }*/
    }
}