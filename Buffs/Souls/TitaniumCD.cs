using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Souls
{
    public class TitaniumCD : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Titanium Shield Cooldown");
            Description.SetDefault("You are charging up Titanium Shield");
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            modPlayer.TitaniumCD = true;
        }
    }
}
