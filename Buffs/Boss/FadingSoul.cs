using FargowiltasSouls;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Boss
{

	public class FadingSoul : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Fading Soul");
			Description.SetDefault("Losing life");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;

            Terraria.ID.BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

		public override void Update(Player Player, ref int buffIndex)
		{
			Player.GetModPlayer<FargoSoulsPlayer>().FadingSoul = true;
			if (Player.statLife < 20)
				Player.KillMe(PlayerDeathReason.ByCustomReason(Player.name + "'s soul faded away."), 999999, 0);
        }
	}
}
