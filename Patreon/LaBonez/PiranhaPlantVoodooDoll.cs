using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.ID;
using FargowiltasSouls.Items;

namespace FargowiltasSouls.Patreon.LaBonez
{
    public class PiranhaPlantVoodooDoll : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("Piranha Plant Voodoo Doll");
            Tooltip.SetDefault(
@"Toggle that will grant all enemies the ability to inflict random debuffs
'In loving memory of Masochist mode EX. I always hated you.'");*/
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.maxStack = 1;
            item.rare = 1;
            item.useAnimation = 30;
            item.useTime = 30;
            item.useStyle = 4;
            item.consumable = false;
        }

        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine line = new TooltipLine(mod, "tooltip", Language.GetTextValue("Mods.FargowiltasSouls.Patreon.Tooltip"));
            line.overrideColor = Color.Orange;
            tooltips.Add(line);
        }

        public override bool UseItem(Player player)
        {
            PatreonPlayer patreonPlayer = player.GetModPlayer<PatreonPlayer>();
            patreonPlayer.PiranhaPlantMode = !patreonPlayer.PiranhaPlantMode;

            string text = patreonPlayer.PiranhaPlantMode ? Language.GetTextValue("Mods.FargowiltasSouls.PiranhaPlantVoodooDoll.Activate") : Language.GetTextValue("Mods.FargowiltasSouls.PiranhaPlantVoodooDoll.Deactivate");
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText(text, 175, 75, 255);
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.BroadcastChatMessage(NetworkText.FromLiteral(text), new Color(175, 75, 255));
                NetMessage.SendData(MessageID.WorldData); //sync world
            }

            Main.PlaySound(SoundID.Roar, (int)player.position.X, (int)player.position.Y, 0);

            return true;
        }
    }
}