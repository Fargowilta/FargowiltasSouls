using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class WoodEnchant : BaseEnchant
    {
        protected override Color nameColor => new Color(151, 107, 75);
        public override string wizardEffect => Language.GetTextValue("Mods.FargowiltasSouls.WizardEffect.Wood");

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Wood Enchantment");
            Tooltip.SetDefault(
@"Bestiary and banner entries complete twice as fast
You gain a shop discount based on bestiary completion
Discount effect works in vanity slots");
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            base.SafeModifyTooltips(list);

            double discount = (Main.GetBestiaryProgressReport().CompletionPercent / 2);
            discount *= 100;
            discount = Math.Round(discount, 2);
            list.Add(new TooltipLine(Mod, "Discount", Language.GetTextValue("Mods.FargowiltasSouls.ItemExtra.WoodenDiscount", discount)));
            list.Add(new TooltipLine(Mod, "Flavor", Language.GetTextValue("Mods.FargowiltasSouls.ItemExtra.WoodenTooltip")));
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
			player.GetModPlayer<FargoSoulsPlayer>().WoodEnchantDiscount = true;
		}

        public override void UpdateInventory(Player player)
        {
            player.GetModPlayer<FargoSoulsPlayer>().WoodEnchantDiscount = true;
        }

        public static void WoodEffect(Player player, Item item)
        {
            player.GetModPlayer<FargoSoulsPlayer>().WoodEnchantItem = item;
			player.GetModPlayer<FargoSoulsPlayer>().WoodEnchantDiscount = true;
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
            if (modPlayer.WoodForce)
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

        public static void WoodDiscount(Chest shop)
        {
            BestiaryUnlockProgressReport bestiaryProgressReport = Main.GetBestiaryProgressReport();
            float discount = 1f - (bestiaryProgressReport.CompletionPercent / 2f); //50% discount at 100% bestiary

            for (int i = 0; i < 40; i++)
            {
                int? originalPrice = shop.item[i].shopCustomPrice == null ? shop.item[i].value : shop.item[i].shopCustomPrice;

                shop.item[i].shopCustomPrice = (int)((float)originalPrice * discount);
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
