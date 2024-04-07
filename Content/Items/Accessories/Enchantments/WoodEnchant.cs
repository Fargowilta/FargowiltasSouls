using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class WoodEnchant : BaseEnchant
    {
        public override Color nameColor => new(151, 107, 75);

        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
            base.SafeModifyTooltips(tooltips);

            double discount = Main.GetBestiaryProgressReport().CompletionPercent / 2;
            discount *= 100;
            discount = Math.Round(discount, 2);

            int i = tooltips.FindIndex(line => line.Name == "Tooltip3");
            if (i != -1)
                tooltips[i].Text = string.Format(tooltips[i].Text, discount);
            else
            {
                i = tooltips.FindIndex(line => line.Name == "SocialDesc");
                if (i != -1)
                {
                    tooltips.RemoveAt(i);
                    ItemTooltip tooltip = ItemTooltip.FromLocalization(Tooltip);
                    tooltips.Insert(i, new TooltipLine(Mod, "WoodEnchantVanity0", tooltip.GetLine(1)));
                    tooltips.Insert(i + 1, new TooltipLine(Mod, "WoodEnchantVanity1", tooltip.GetLine(2)));
                    tooltips.Insert(i + 2, new TooltipLine(Mod, "WoodEnchantVanity2", string.Format(tooltip.GetLine(3), discount)));
                }
            }
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Wood Enchantment");
            /* Tooltip.SetDefault(
@"Bestiary and banner entries complete twice as fast
You gain a shop discount based on bestiary completion
Discount effect works in vanity slots"); */
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Blue;
            Item.value = 10000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            WoodEffect(player, Item);
        }

        public override void UpdateVanity(Player player)
        {
            player.FargoSouls().WoodEnchantDiscount = true;
        }

        public override void UpdateInventory(Player player)
        {
            player.FargoSouls().WoodEnchantDiscount = true;
        }

        public static void WoodEffect(Player player, Item item)
        {
            player.FargoSouls().WoodEnchantItem = item;
            player.FargoSouls().WoodEnchantDiscount = true;
        }

        public static void WoodCheckDead(FargoSoulsPlayer modPlayer, NPC npc)
        {
            if (npc.ExcludedFromDeathTally())
                return;
            int banner = Item.NPCtoBanner(npc.BannerID());
            if (banner <= 0)
                return;

            //register extra kill per kill
            int addedKillBonus = 1;
            if (modPlayer.ForceEffect<WoodEnchant>())
                addedKillBonus = 4;

            //for nonstandard banner thresholds, e.g. some ooa npcs at 100 or 200
            int bannerThreshold = ItemID.Sets.KillsToBanner[Item.BannerToItem(banner)];

            for (int i = 0; i < addedKillBonus; i++)
            {
                //stop at 49, 99, 149, etc. so that game will increment on its own
                //not doing this causes it to skip the banner
                if (NPC.killCount[banner] % bannerThreshold != bannerThreshold - 1)
                {
                    NPC.killCount[banner]++;
                    Main.BestiaryTracker.Kills.RegisterKill(npc);
                }
            }
        }

        public static void WoodDiscount(Item[] items)
        {
            BestiaryUnlockProgressReport bestiaryProgressReport = Main.GetBestiaryProgressReport();
            float discount = 1f - bestiaryProgressReport.CompletionPercent / 2f; //50% discount at 100% bestiary
            foreach (Item item in items)
            {
                if (item == null) continue;
                int? originalPrice = item.shopCustomPrice == null ? item.value : item.shopCustomPrice;

                item.shopCustomPrice = (int)((float)originalPrice * discount);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.WoodHelmet)
            .AddIngredient(ItemID.WoodBreastplate)
            .AddIngredient(ItemID.WoodGreaves)
            .AddIngredient(ItemID.Daybloom)
            .AddIngredient(ItemID.Apple)
            .AddRecipeGroup("FargowiltasSouls:AnySquirrel")

            .AddTile(TileID.DemonAltar)
            .Register();

        }
    }
}
