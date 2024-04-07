using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Patreon.LaBonez
{
    public class PiranhaPlantVoodooDoll : PatreonModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 1;
            Item.rare = ItemRarityID.Blue;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
        }

        public override bool? UseItem(Player player)
        {
            PatreonPlayer patreonPlayer = player.GetModPlayer<PatreonPlayer>();
            patreonPlayer.PiranhaPlantMode = !patreonPlayer.PiranhaPlantMode;

            FargoSoulsUtil.PrintLocalization(patreonPlayer.PiranhaPlantMode ? $"Mods.{Mod.Name}.Items.{Name}.On" : $"Mods.{Mod.Name}.Items.{Name}.Off", 175, 75, 255);
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData); //sync world

            SoundEngine.PlaySound(SoundID.Roar, player.Center);

            return true;
        }
    }
}