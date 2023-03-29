using FargowiltasSouls.Items.Materials;
using FargowiltasSouls.NPCs.MutantBoss;
using FargowiltasSouls.Utilities;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Summons
{
    public class AbominationnVoodooDoll : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Abominationn Voodoo Doll");
            Tooltip.SetDefault("Summons Abominationn to your town" +
                "\n'You are a terrible person'");

            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "憎恶巫毒娃娃");
            //Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "你可真是个坏东西");

            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Purple;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.maxStack = 20;
            Item.value = Item.sellPrice(gold: 1);
        }

        public override bool CanUseItem(Player player) => ModContent.TryFind("Fargowiltas", "Abominationn", out ModNPC modNPC) && !NPC.AnyNPCs(modNPC.Type);

        public override bool? UseItem(Player player)
        {
            if (ModContent.TryFind("Fargowiltas", "Abominationn", out ModNPC modNPC))
                NPC.SpawnOnPlayer(player.whoAmI, modNPC.Type);

            return true;
        }

        bool hasDeclaredTeleport;

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            if (Item.lavaWet && Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (ModContent.TryFind("Fargowiltas", "Abominationn", out ModNPC a))
                {
                    int p = Player.FindClosest(Item.Center, 0, 0);
                    NPC abom = FargoSoulsUtil.NPCExists(NPC.FindFirstNPC(a.Type));
                    if (p != -1)
                    {
                        if (Main.player[p].Center.Y / 16 > Main.worldSurface)
                        {
                            if (!hasDeclaredTeleport)
                            {
                                hasDeclaredTeleport = true;
                                FargoSoulsUtil.PrintLocalization("Mods.FargowiltasSouls.Message.AbominationnVoodooDollFail", new Color(175, 75, 255));
                            }
                                
                            Item.Center = Main.player[p].Center;
                            Item.noGrabDelay = 0;
                        }
                        else if (abom != null)
                        {
                            abom.life = 0;
                            abom.StrikeNPC(9999, 0f, 0);

                            FargoSoulsUtil.SpawnBossNetcoded(Main.player[p], ModContent.NPCType<MutantBoss>(), false);

                            Item.active = false;
                            Item.type = 0;
                            Item.stack = 0;
                        }
                    }
                }
            }
        }

        public override void UpdateInventory(Player player)
        {
            hasDeclaredTeleport = false;
        }

        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
            if (tooltips.TryFindTooltipLine("ItemName", out TooltipLine itemNameLine))
                itemNameLine.OverrideColor = new Color(Main.DiscoR, 51, 255 - (int)(Main.DiscoR * 0.4));
        }

        public override void AddRecipes()
        {
            CreateRecipe(5)
            .AddIngredient(ModContent.ItemType<AbomEnergy>(), 5)
            .AddIngredient(ItemID.GuideVoodooDoll)
            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))
            .Register();
        }
    }
}