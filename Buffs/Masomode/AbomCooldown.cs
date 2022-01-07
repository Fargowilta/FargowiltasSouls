using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class AbomCooldown : ModBuff
    {
        public override void SetDefaults()
        {
            /*//DisplayName.SetDefault("Abominable Cooldown");
            //Description.SetDefault("Cannot endure another attack yet");*/
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            longerExpertDebuff = false;
            canBeCleared = true;
        }

        public override bool Autoload(ref string name, ref string texture)
        {
            texture = "FargowiltasSouls/Buffs/PlaceholderDebuff";
            return true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.GetModPlayer<FargoPlayer>().AbominableWandRevived)
                player.buffTime[buffIndex] = 2;
        }
    }
}