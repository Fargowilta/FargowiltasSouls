using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Patreon.LaBonez
{
    public class PiranhaPlantVoodooDoll : PatreonModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Piranha Plant Voodoo Doll");
            Tooltip.SetDefault(
@"Toggle that will grant all enemies the ability to inflict random debuffs
'In loving memory of Masochist mode EX. I always hated you.'");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 1;
            Item.rare = 1;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = 4;
            Item.consumable = false;
        }

        public override bool? UseItem(Player player)
        {
            PatreonPlayer patreonPlayer = player.GetModPlayer<PatreonPlayer>();
            patreonPlayer.PiranhaPlantMode = !patreonPlayer.PiranhaPlantMode;

            FargoSoulsUtil.PrintLocalization(patreonPlayer.PiranhaPlantMode ? $"Mods.{Mod.Name}.Message.{Name}On" : $"Mods.{Mod.Name}.Message.{Name}Off", 175, 75, 255);
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData); //sync world

            SoundEngine.PlaySound(SoundID.Roar, player.Center);

            return true;
        }
    }
}