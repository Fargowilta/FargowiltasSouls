using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs
{
    public class MagicalCleanseCD : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Magical Cleanse Cooldown");
            Description.SetDefault("You cannot cleanse debuffs yet");
            Main.debuff[Type] = true;
            Terraria.ID.BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
		
		public override void Update(Player player, ref int buffIndex)
		{
			if (player.buffTime[buffIndex] > 2 && player.GetModPlayer<FargoSoulsPlayer>().WeaponUseTimer <= 0)
				player.buffTime[buffIndex] -= 1;
		}
    }
}