using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Misc
{
    public class MutantsFury : SoulsItem
    {
        public override string Texture => "FargowiltasSouls/Content/Items/Placeholder";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Mutant's Fury");
            // Tooltip.SetDefault("'REALLY enrages Mutant... or doesn't'");
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "突变狂怒");
            //Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "'真·正激怒突变体... 也许并不'");
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Purple;
            Item.maxStack = 999;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
            Item.value = Item.buyPrice(1);
        }

        public override bool? UseItem(Player player)
        {
            WorldSavingSystem.AngryMutant = !WorldSavingSystem.AngryMutant;
            string text = WorldSavingSystem.AngryMutant ? $"Mods.{Mod.Name}.Items.{Name}.On" : $"Mods.{Mod.Name}.Items.{Name}.Off";
            FargoSoulsUtil.PrintLocalization(text, 175, 75, 255);
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData); //sync world
            SoundEngine.PlaySound(SoundID.Roar, player.Center);
            return true;
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.Mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.OverrideColor = new Color(Main.DiscoR, 51, 255 - (int)(Main.DiscoR * 0.4));
                }
            }
        }
    }
}