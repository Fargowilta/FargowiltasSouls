using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Misc
{
    public class MutantsFury : SoulsItem
    {
        public override string Texture => "FargowiltasSouls/Items/Placeholder";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mutant's Fury");
            Tooltip.SetDefault("'REALLY enrages Mutant... or doesn't'");
            DisplayName.AddTranslation(GameCulture.Chinese, "突变狂怒");
            Tooltip.AddTranslation(GameCulture.Chinese, "'真·激怒突变体...或使其冷静下来'");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = ItemRarityID.Purple;
            item.maxStack = 999;
            item.useAnimation = 30;
            item.useTime = 30;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.consumable = false;
            item.value = Item.buyPrice(1);
        }

        public override bool UseItem(Player player)
        {
            FargoSoulsWorld.AngryMutant = !FargoSoulsWorld.AngryMutant;
            if (Language.ActiveCulture == GameCulture.Chinese)
            {
            string text = FargoSoulsWorld.AngryMutant ? "突变体被激怒了！" : "突变体冷静下来了.";
            }
            else
            {
            string text = FargoSoulsWorld.AngryMutant ? "Mutant is angered!" : "Mutant is calm.";
            }
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
            if (Language.ActiveCulture == GameCulture.Chinese)
            {
                Main.NewText(text, 175, 75, 255);
            }
            else
            {
                Main.NewText(text, 175, 75, 255);
            }
            }
            else if (Main.netMode == NetmodeID.Server)
            {
            if (Language.ActiveCulture == GameCulture.Chinese)
            {
                NetMessage.BroadcastChatMessage(NetworkText.FromLiteral(text), new Color(175, 75, 255));
                NetMessage.SendData(MessageID.WorldData); //sync world
            }
            else
            {
            NetMessage.BroadcastChatMessage(NetworkText.FromLiteral(text), new Color(175, 75, 255));
            NetMessage.SendData(MessageID.WorldData); //sync world
            }
            }
            Main.PlaySound(SoundID.Roar, (int)player.position.X, (int)player.position.Y, 0);
            return true;
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = new Color(Main.DiscoR, 51, 255 - (int)(Main.DiscoR * 0.4));
                }
            }
        }
    }
}
