using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs
{
    public class SuperchargedBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Supercharged");
            // Description.SetDefault("20% increased move speed, 10% increased attack speed, your attacks electrify");
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true;
            Terraria.ID.BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.moveSpeed += 0.20f;
            player.FargoSouls().AttackSpeed += 0.10f;
            player.FargoSouls().Supercharged = true;
        }
    }
}