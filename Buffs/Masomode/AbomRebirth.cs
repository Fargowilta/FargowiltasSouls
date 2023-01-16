using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class AbomRebirth : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Abominable Rebirth");
            Description.SetDefault("You cannot die unless struck");
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            //player.GetModPlayer<FargoSoulsPlayer>().MutantNibble = true;
            player.GetModPlayer<FargoSoulsPlayer>().AbomRebirth = true;
        }
    }
}