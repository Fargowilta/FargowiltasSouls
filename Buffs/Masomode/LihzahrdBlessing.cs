using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class LihzahrdBlessing : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Lihzahrd Blessing");
            Description.SetDefault("Wires enabled and reduced spawn rates in Jungle Temple");
            DisplayName.AddTranslation(GameCulture.Chinese, "蜥蜴人的祝福");
            Description.AddTranslation(GameCulture.Chinese, "电路可用，减少神庙中的刷怪率");
            canBeCleared = true;
        }

        public override bool Autoload(ref string name, ref string texture)
        {
            texture = "FargowiltasSouls/Buffs/PlaceholderBuff";
            return true;
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
