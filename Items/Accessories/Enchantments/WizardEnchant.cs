using FargowiltasSouls.Utilities;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class WizardEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Wizard Enchantment");
            Tooltip.SetDefault(
@"Enhances the power of all other Enchantments to their Force effects
'I'm a what?'");
        }

        protected override Color nameColor => new Color(50, 80, 193);
        public override string wizardEffect => "";

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.LightRed;
            Item.value = 100000;
        }

        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
            base.SafeModifyTooltips(tooltips);

            if (tooltips.TryFindTooltipLine("ItemName", out TooltipLine itemNameLine))
                itemNameLine.OverrideColor = nameColor;

            Player player = Main.player[ Main.myPlayer];

            for (int i = 3; i <= 9; i++)
            {
                if (!player.armor[i].IsAir)
                {
                    ModItem item = player.armor[i].ModItem;

                    if (item is BaseEnchant)
                    {
                        BaseEnchant enchant = item as BaseEnchant;
                        string wizardText = enchant.wizardEffect;

                        if (wizardEffect.Length == 0)
                        {
                            continue;
                        }

                        tooltips.Add(new TooltipLine(Mod, "wizard", $"[i:{item.Type}]" + wizardText));
                    }
                }
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoSoulsPlayer>().WizardEnchantActive = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ItemID.WizardHat)
            //.AddIngredient(ItemID.AmethystRobe);
            //.AddIngredient(ItemID.TopazRobe);

            .AddIngredient(ItemID.SapphireRobe)
            .AddIngredient(ItemID.EmeraldRobe)
            .AddIngredient(ItemID.RubyRobe)
            .AddIngredient(ItemID.DiamondRobe)
            //amber robe
            .AddIngredient(ItemID.RareEnchantment)

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
}
