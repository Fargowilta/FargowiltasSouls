using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class ThiefCD : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Thief Cooldown");
            Description.SetDefault("Your items cannot be stolen again yet");
            Main.debuff[Type] = true;
            Terraria.ID.BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.buffImmune[ModContent.BuffType<LoosePockets>()] = true;
        }
    }
}