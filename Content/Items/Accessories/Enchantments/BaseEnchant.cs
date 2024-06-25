using FargowiltasSouls.Content.Items.Accessories.Forces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public abstract class BaseEnchant : SoulsItem
    {
        public abstract Color nameColor { get; }
        public string wizardEffect()
        {
            string text = Language.GetTextValue($"Mods.{Mod.Name}.WizardEffect.{Name.Replace("Enchantment", "").Replace("Enchant", "")}");
            if (text.Contains($"Mods.{Mod.Name}.WizardEffect") || text.Length <= 1) //if there's no localization entry or it's empty
            {
                return Language.GetTextValue($"Mods.FargowiltasSouls.WizardEffect.NoUpgrade");
            }
            return text;
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            ItemID.Sets.ItemNoGravity[Type] = true;
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
            base.SafeModifyTooltips(tooltips);

            if (tooltips.TryFindTooltipLine("ItemName", out TooltipLine itemNameLine))
                itemNameLine.OverrideColor = nameColor;

            FargoSoulsPlayer localSoulsPlayer = Main.LocalPlayer.FargoSouls();
            if (localSoulsPlayer.WizardTooltips)
            {
                if (Type == ModContent.ItemType<WizardEnchant>())
                {
                    return;
                }

                if (localSoulsPlayer.ForceEffect(Type))
                {
                    if (wizardEffect().Length != 0)
                        tooltips.Add(new TooltipLine(Mod, "wizard", $"{Language.GetTextValue($"Mods.FargowiltasSouls.WizardEffect.Active")} [i:{ModContent.ItemType<WizardEnchant>()}]: " + wizardEffect()));
                }
                else
                {
                    if (wizardEffect().Length != 0)
                    {
                        tooltips.Add(new TooltipLine(Mod, "wizard", $"{Language.GetTextValue($"Mods.FargowiltasSouls.WizardEffect.Inactive")} [i:{ModContent.ItemType<WizardEnchant>()}]: " + wizardEffect()));
                        tooltips[tooltips.Count - 1].OverrideColor = Color.Gray;
                    }
                }
            }

        }
        /// <summary>
        /// IDs for enchants that craft into other enchants. Index is material, value is result. Default value is -1.
        /// </summary>
        public static int[] CraftsInto;
        /// <summary>
        /// IDs for the corresponding Force of each enchant.
        /// </summary>
        public static int[] Force;

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
        }
        int drawTimer = 0;
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            //draw glow if wizard effect
            Player player = Main.LocalPlayer;
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            if (modPlayer.WizardedItem == Item)
            {
                for (int j = 0; j < 12; j++)
                {
                    Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12f).ToRotationVector2() * 1f;
                    float modifier = 0.5f + ((float)Math.Sin(drawTimer / 30f) / 6);
                    Color glowColor = Color.Lerp(Color.Blue with { A = 0 }, Color.Silver with { A = 0 }, modifier) * 0.5f;

                    Texture2D texture = Terraria.GameContent.TextureAssets.Item[Item.type].Value;
                    Main.EntitySpriteDraw(texture, position + afterimageOffset, null, glowColor, 0, texture.Size() * 0.5f, Item.scale, SpriteEffects.None, 0f);
                }
            }
            drawTimer++;
            return base.PreDrawInInventory(spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        }
        public sealed override void UpdateEquip(Player player)
        {
            //todo, change this to sealed UpdateAccessory and refactor every single enchantment file to accommodate
            player.FargoSouls().EquippedEnchants.Add(this);
        }
    }

    public class EnchantSystem : ModSystem
    {
        public override void PostSetupRecipes()
        {
            SetFactory factory = ItemID.Sets.Factory;
            BaseEnchant.CraftsInto = factory.CreateIntSet();
            foreach (BaseEnchant modItem in ModContent.GetContent<BaseEnchant>())
            {
                Recipe recipe = Main.recipe.FirstOrDefault(r => r.ContainsIngredient(modItem.Type) && r.createItem.ModItem != null && r.createItem.ModItem is BaseEnchant, null);
                if (recipe != null)
                    BaseEnchant.CraftsInto[modItem.Type] = recipe.createItem.type;
            }

            BaseEnchant.Force = factory.CreateIntSet();
            foreach (var enchantsPerForceDict in BaseForce.Enchants)
            {
                foreach (int enchant in enchantsPerForceDict.Value)
                    BaseEnchant.Force[enchant] = enchantsPerForceDict.Key;
            }
        }
    }
}
