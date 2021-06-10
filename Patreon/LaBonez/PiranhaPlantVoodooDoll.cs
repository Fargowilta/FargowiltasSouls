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
            DisplayName.SetDefault("Piranha Plant Voodoo Doll");
            Tooltip.SetDefault(
@"Toggle that will grant all enemies the ability to inflict random debuffs
'In loving memory of Masochist mode EX. I always hated you.'");
            DisplayName.AddTranslation(GameCulture.Chinese, "食人花巫毒娃娃");
            Tooltip..AddTranslation(GameCulture.Chinese, 
@"使用后使所有敌人的攻击会造成随机减益，再次使用以关闭此效果
'以敬爱的受虐EX为名，我恨你一辈子'");
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
            TooltipLine line = new TooltipLine(mod, "tooltip", ">> Patreon Item <<");
            line.overrideColor = Color.Orange;
            tooltips.Add(line);
        }

        public override bool UseItem(Player player)
        {
            PatreonPlayer patreonPlayer = player.GetModPlayer<PatreonPlayer>();
            patreonPlayer.PiranhaPlantMode = !patreonPlayer.PiranhaPlantMode;

            // TODO: Localization
            //苦难仍在继续. 苦难已结束.
            string text = patreonPlayer.PiranhaPlantMode ? "The suffering continues." : "The suffering wanes.";
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
