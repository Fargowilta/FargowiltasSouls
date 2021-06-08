using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.NPCs.Champions;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Summons
{
    public class SigilOfChampions : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sigil of Champions");
            Tooltip.SetDefault(@"Summons the Champions
Summons vary depending on time and biome
Right click to check for possible summons
Not consumed on use");
            DisplayName.AddTranslation(GameCulture.Chinese, "唤灵魔符");
            Tooltip.AddTranslation(GameCulture.Chinese, @"召唤英灵
根据所处环境和时间召唤不同的英灵
右键使用以查看可能召唤出的英灵
不消耗");
        }

        public override void SetDefaults()
        {
            item.width = 40;
            item.height = 40;
            item.rare = ItemRarityID.Purple;
            item.maxStack = 1;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.consumable = false;
            item.value = Item.buyPrice(1);
        }

        public override bool CanUseItem(Player player)
        {
            List<int> bosses = new List<int>(new int[] {
                ModContent.NPCType<CosmosChampion>(),
                ModContent.NPCType<EarthChampion>(),
                ModContent.NPCType<LifeChampion>(),
                ModContent.NPCType<NatureChampion>(),
                ModContent.NPCType<ShadowChampion>(),
                ModContent.NPCType<SpiritChampion>(),
                ModContent.NPCType<TerraChampion>(),
                ModContent.NPCType<TimberChampion>(),
                ModContent.NPCType<WillChampion>()
            });

            for (int i = 0; i < Main.maxNPCs; i++) //no using during another champ fight
            {
                if (Main.npc[i].active && i == NPCs.EModeGlobalNPC.championBoss && bosses.Contains(Main.npc[i].type))
                    return false;
            }
            return true;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool UseItem(Player player)
        {
            Color color = new Color(175, 75, 255);

            if (player.ZoneUndergroundDesert)
            {
                if (player.altFunctionUse == 2)
                    //矍铄精神，振奋人心...
                    Main.NewText("A strong spirit stirs...", color);
                else
                    NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<SpiritChampion>());
            }
            else if (player.ZoneUnderworldHeight)
            {
                if (player.altFunctionUse == 2)
                    //星球之核，隆隆作响...
                    Main.NewText("The core of the planet rumbles...", color);
                else
                    NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<EarthChampion>());
            }
            else if (player.Center.Y >= Main.worldSurface * 16) //is underground
            {
                if (player.ZoneSnow)
                {
                    if (player.altFunctionUse == 2)
                        //青翠之风，骤然吹起...
                        Main.NewText("A verdant wind is blowing...", color);
                    else
                        NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<NatureChampion>());
                }
                else
                {
                    if (player.altFunctionUse == 2)
                        //磐石震动，四野颤抖...
                        Main.NewText("The stones tremble around you...", color);
                    else
                        NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<TerraChampion>());
                }
            }
            else //above ground
            {
                if (player.ZoneSkyHeight)
                {
                    if (player.altFunctionUse == 2)
                        //群星连珠，命定此刻...
                        Main.NewText("The stars are aligning...", color);
                    else
                        NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<CosmosChampion>());
                }
                else if (player.ZoneBeach)
                {
                    if (player.altFunctionUse == 2)
                        //金铁呜咽，回荡深渊...
                        Main.NewText("Metallic groans echo from the depths...", color);
                    else
                        NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<WillChampion>());
                }
                else if (player.ZoneHoly && Main.dayTime)
                {
                    if (player.altFunctionUse == 2)
                        //温温生命，透体而过...
                        Main.NewText("A wave of warmth passes over you...", color);
                    else
                        NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<LifeChampion>());
                }
                else if ((player.ZoneCorrupt || player.ZoneCrimson) && !Main.dayTime) //night
                {
                    if (player.altFunctionUse == 2)
                        //黑夜之暗，愈深愈沉...
                        Main.NewText("The darkness of the night feels deeper...", color);
                    else
                        NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<ShadowChampion>());
                }
                else if (!player.ZoneHoly && !player.ZoneCorrupt && !player.ZoneCrimson
                    && !player.ZoneDesert && !player.ZoneSnow && !player.ZoneJungle && Main.dayTime) //purity day
                {
                    if (player.altFunctionUse == 2)
                        //林叶沙沙，环绕四周...
                        Main.NewText("You are surrounded by the rustling of trees...", color);
                    else
                        NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<TimberChampion>());
                }
                else //nothing to summon
                {
                    if (player.altFunctionUse == 2)
                        //看起来没有东西回应你的召唤...
                        Main.NewText("Nothing seems to answer the call...", color);
                }
            }

            return true;
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = Main.DiscoColor;
                }
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Acorn, 5);
            recipe.AddRecipeGroup("IronBar", 5);
            recipe.AddIngredient(ItemID.HellstoneBar, 5);
            recipe.AddIngredient(ItemID.FrostCore, 5);
            recipe.AddIngredient(ItemID.SoulofLight, 5);
            recipe.AddIngredient(ItemID.SoulofNight, 5);
            recipe.AddIngredient(ItemID.AncientBattleArmorMaterial, 5);
            recipe.AddIngredient(ItemID.Coral, 5);
            recipe.AddIngredient(ItemID.LunarBar, 5);

            //recipe.AddTile(ModLoader.GetMod("Fargowiltas").TileType("CrucibleCosmosSheet"));
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
