using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items
{
    public class MasochistReal : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Forgotten Gift");
            //Tooltip.SetDefault("Toggles Masochist Mode");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.maxStack = 1;
            item.rare = ItemRarityID.Blue;
            item.useAnimation = 30;
            item.useTime = 30;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.consumable = false;
        }

        public override bool UseItem(Player player)
        {
            if (!FargoSoulsUtil.AnyBossAlive())
            {
                FargoSoulsWorld.MasochistModeReal = !FargoSoulsWorld.MasochistModeReal;
                FargoSoulsWorld.EternityMode = true;
                Main.expertMode = true;

                Main.PlaySound(SoundID.Roar, (int)player.position.X, (int)player.position.Y, 0);

                FargoSoulsUtil.PrintText(FargoSoulsWorld.MasochistModeReal ? Language.GetTextValue("Mods.FargowiltasSouls.MasochistReal.Activate") : Language.GetTextValue("Mods.FargowiltasSouls.MasochistReal.Deactivate"), new Color(255, 51, 153));

                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.WorldData); //sync world
            }
            return true;
        }
    }
}