using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Buffs.Minions;
using FargowiltasSouls.Content.Items.Materials;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Masomode
{
    public class ChaliceoftheMoon : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
            base.SafeModifyTooltips(tooltips);

            if (Main.keyState.IsKeyDown(Keys.LeftShift))
            {
                TooltipLine line = new(Mod, "tooltip", Language.GetTextValue($"Mods.{Mod.Name}.Items.{Name}.ExtTooltip"));
                tooltips.Add(line);
            }

            else
            {
                TooltipLine line = new(Mod, "tooltip", Language.GetTextValue("Mods.FargowiltasSouls.Items.Extra.PressShift"));
                tooltips.Add(line);
            }

        }   
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 54;
            Item.accessory = true;
            Item.rare = ItemRarityID.Red;
            Item.value = Item.sellPrice(0, 7);
            Item.defense = 8;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer fargoPlayer = player.FargoSouls();

            //magical bulb
            MagicalBulb.AddEffects(player, Item);

            //lihzahrd treasure
            player.buffImmune[BuffID.Burning] = true;
            player.buffImmune[ModContent.BuffType<FusedBuff>()] = true;
            player.buffImmune[ModContent.BuffType<LihzahrdCurseBuff>()] = true;
            player.buffImmune[ModContent.BuffType<LowGroundBuff>()] = true;
            fargoPlayer.LihzahrdTreasureBoxItem = Item;
            player.AddEffect<LihzahrdGroundPound>(Item);
            player.AddEffect<LihzahrdBoulders>(Item);

            //celestial rune
            player.buffImmune[ModContent.BuffType<MarkedforDeathBuff>()] = true;
            player.AddEffect<CelestialRuneAttacks>(Item);

            //chalice
            player.buffImmune[ModContent.BuffType<AtrophiedBuff>()] = true;
            player.buffImmune[ModContent.BuffType<JammedBuff>()] = true;
            player.buffImmune[ModContent.BuffType<ReverseManaFlowBuff>()] = true;
            player.buffImmune[ModContent.BuffType<AntisocialBuff>()] = true;
            fargoPlayer.MoonChalice = true;
            //player.AddEffect<CultistMinionEffect>(Item);

        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ModContent.ItemType<MagicalBulb>())
            .AddIngredient(ModContent.ItemType<LihzahrdTreasureBox>())
            .AddIngredient(ModContent.ItemType<CelestialRune>())
            .AddIngredient(ItemID.FragmentSolar, 1)
            .AddIngredient(ItemID.FragmentVortex, 1)
            .AddIngredient(ItemID.FragmentNebula, 1)
            .AddIngredient(ItemID.FragmentStardust, 1)
            .AddIngredient(ModContent.ItemType<DeviatingEnergy>(), 10)

            .AddTile(TileID.LunarCraftingStation)

            .Register();
        }
    }
   /*
    public class CultistMinionEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<ChaliceHeader>();
        public override int ToggleItemType => ModContent.ItemType<ChaliceoftheMoon>();
        public override bool MinionEffect => true;
        public override void PostUpdateEquips(Player player)
        {
            if (!player.HasBuff<SouloftheMasochistBuff>())
                player.AddBuff(ModContent.BuffType<LunarCultistBuff>(), 2);
        }
    }*/
}  