using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Masomode
{
    public class LihzahrdBlessingBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Lihzahrd Blessing");
            // Description.SetDefault("Wires enabled and reduced spawn rates in Jungle Temple");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.buffImmune[ModContent.BuffType<LihzahrdCurseBuff>()] = true;
            if (Framing.GetTileSafely(player.Center).WallType == WallID.LihzahrdBrickUnsafe)
            {
                player.sunflower = true;
                player.ZonePeaceCandle = true;
                player.calmed = true;
            }
        }
    }
}