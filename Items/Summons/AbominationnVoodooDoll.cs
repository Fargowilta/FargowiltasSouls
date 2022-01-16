using Fargowiltas.Items.Tiles;
using FargowiltasSouls.Items.Misc;
using FargowiltasSouls.Utilities;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Summons
{
    public class AbominationnVoodooDoll : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("Abominationn Voodoo Doll");
            Tooltip.SetDefault("Summons Abominationn to your town" +
                "\n'You are a terrible person'");

            DisplayName.AddTranslation(GameCulture.Chinese, "憎恶巫毒娃娃");
            Tooltip.AddTranslation(GameCulture.Chinese, "你可真是个坏东西");*/
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = ItemRarityID.Purple;
            item.useAnimation = 30;
            item.useTime = 30;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.maxStack = 20;
            item.value = Item.sellPrice(gold: 1);
        }

        public override bool CanUseItem(Player player) => !NPC.AnyNPCs(ModLoader.GetMod("Fargowiltas").NPCType("Abominationn"));

        public override bool UseItem(Player player)
        {
            NPC.SpawnOnPlayer(player.whoAmI, ModLoader.GetMod("Fargowiltas").NPCType("Abominationn"));

            return true;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            if (item.lavaWet)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC abom = FargoSoulsUtil.NPCExists(NPC.FindFirstNPC(ModLoader.GetMod("Fargowiltas").NPCType("Abominationn")));
                    NPC mutant = FargoSoulsUtil.NPCExists(NPC.FindFirstNPC(ModLoader.GetMod("Fargowiltas").NPCType("Mutant")));

                    if (abom != null)
                    {
                        abom.StrikeNPC(9999, 0f, 0);

                        if (mutant != null)
                        {
                            mutant.Transform(mod.NPCType("MutantBoss"));

                            // TODO: Localization
                            FargoSoulsUtil.PrintText(Language.GetTextValue("Mods.FargowiltasSouls.AbominationnVoodooDoll.Spawn"), new Color(175, 75, 255));
                        }
                    }

                    item.active = false;
                    item.type = 0;
                    item.stack = 0;
                }
            }
        }

        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
            if (tooltips.TryFindTooltipLine("ItemName", out TooltipLine itemNameLine))
                itemNameLine.overrideColor = new Color(Main.DiscoR, 51, 255 - (int)(Main.DiscoR * 0.4));
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<AbomEnergy>(), 5);
            recipe.AddIngredient(ItemID.GuideVoodooDoll);
            recipe.AddTile(ModContent.TileType<CrucibleCosmosSheet>());
            recipe.SetResult(this, 5);
            recipe.AddRecipe();
        }
    }
}