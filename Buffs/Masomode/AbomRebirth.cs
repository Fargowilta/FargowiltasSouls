using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class AbomRebirth : ModBuff
    {
        public override void SetDefaults()
        {
            /*DisplayName.SetDefault("Abominable Rebirth");
            Description.SetDefault("You cannot heal at all and cannot die unless struck");*/
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
            canBeCleared = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoPlayer>().MutantNibble = true;
            player.GetModPlayer<FargoPlayer>().AbomRebirth = true;
        }
    }
}
