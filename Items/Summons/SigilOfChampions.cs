using FargowiltasSouls.NPCs.Champions;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

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
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.rare = ItemRarityID.Purple;
            Item.maxStack = 1;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
            Item.value = Item.buyPrice(1);
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

        private void PrintChampMessage(string key)
        {
            //using the raw name SigilOfChampions here because ChampionySigil runs this code too, can't use "Name"
            Main.NewText(Language.GetTextValue($"Mods.{Mod.Name}.Message.SigilOfChampions.{key}"), new Color(175, 75, 255));
        }

        public override bool? UseItem(Player player)
        {
            if (player.ZoneUndergroundDesert)
            {
                if (player.altFunctionUse == 2)
                    PrintChampMessage("Spirit");
                else
                    FargoSoulsUtil.SpawnBossNetcoded(player, ModContent.NPCType<SpiritChampion>());
            }
            else if (player.ZoneUnderworldHeight)
            {
                if (player.altFunctionUse == 2)
                    PrintChampMessage("Earth");
                else
                    FargoSoulsUtil.SpawnBossNetcoded(player, ModContent.NPCType<EarthChampion>());
            }
            else if (player.Center.Y >= Main.worldSurface * 16) //is underground
            {
                if (player.ZoneSnow)
                {
                    if (player.altFunctionUse == 2)
                        PrintChampMessage("Nature");
                    else
                        FargoSoulsUtil.SpawnBossNetcoded(player, ModContent.NPCType<NatureChampion>());
                }
                else
                {
                    if (player.altFunctionUse == 2)
                        PrintChampMessage("Terra");
                    else
                        FargoSoulsUtil.SpawnBossNetcoded(player, ModContent.NPCType<TerraChampion>());
                }
            }
            else //above ground
            {
                if (player.ZoneSkyHeight)
                {
                    if (player.altFunctionUse == 2)
                        PrintChampMessage("Cosmos");
                    else
                        FargoSoulsUtil.SpawnBossNetcoded(player, ModContent.NPCType<CosmosChampion>());
                }
                else if (player.ZoneBeach)
                {
                    if (player.altFunctionUse == 2)
                        PrintChampMessage("Will");
                    else
                        FargoSoulsUtil.SpawnBossNetcoded(player, ModContent.NPCType<WillChampion>());
                }
                else if (player.ZoneHallow && Main.dayTime)
                {
                    if (player.altFunctionUse == 2)
                        PrintChampMessage("Life");
                    else
                        FargoSoulsUtil.SpawnBossNetcoded(player, ModContent.NPCType<LifeChampion>());
                }
                else if ((player.ZoneCorrupt || player.ZoneCrimson) && !Main.dayTime) //night
                {
                    if (player.altFunctionUse == 2)
                        PrintChampMessage("Shadow");
                    else
                        FargoSoulsUtil.SpawnBossNetcoded(player, ModContent.NPCType<ShadowChampion>());
                }
                else if (!player.ZoneHallow && !player.ZoneCorrupt && !player.ZoneCrimson
                    && !player.ZoneDesert && !player.ZoneSnow && !player.ZoneJungle && Main.dayTime) //purity day
                {
                    if (player.altFunctionUse == 2)
                        PrintChampMessage("Timber");
                    else
                        FargoSoulsUtil.SpawnBossNetcoded(player, ModContent.NPCType<TimberChampion>());
                }
                else //nothing to summon
                {
                    if (player.altFunctionUse == 2)
                        PrintChampMessage("Nothing");
                }
            }

            return true;
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.Mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.OverrideColor = Main.DiscoColor;
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.Acorn, 5)
            .AddRecipeGroup("IronBar", 5)
            .AddIngredient(ItemID.HellstoneBar, 5)
            .AddIngredient(ItemID.FrostCore, 5)
            .AddIngredient(ItemID.SoulofLight, 5)
            .AddIngredient(ItemID.SoulofNight, 5)
            .AddIngredient(ItemID.AncientBattleArmorMaterial, 5)
            .AddIngredient(ItemID.Coral, 5)
            .AddIngredient(ItemID.LunarBar, 5)

            .AddTile(TileID.LunarCraftingStation)

            .Register();
        }
    }
}