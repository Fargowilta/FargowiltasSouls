using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class LihzahrdBlessing : ModBuff
    {
        public override void SetDefaults()
        {
            //DisplayName.SetDefault("Lihzahrd Blessing");
            //Description.SetDefault("Wires enabled and reduced spawn rates in Jungle Temple");
            canBeCleared = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.buffImmune[ModContent.BuffType<LihzahrdCurse>()] = true;
            if (Framing.GetTileSafely(player.Center).wall == WallID.LihzahrdBrickUnsafe)
            {
                player.sunflower = true;
                player.ZonePeaceCandle = true;
                player.calmed = true;
            }
        }
    }
}