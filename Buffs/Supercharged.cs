using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs
{
    public class Supercharged : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Supercharged");
            Description.SetDefault("20% increased move speed, 10% increased attack speed, your attacks electrify");
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true;
            Terraria.ID.BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.moveSpeed += 0.20f;
            player.GetModPlayer<FargoSoulsPlayer>().AttackSpeed += 0.10f;
            player.GetModPlayer<FargoSoulsPlayer>().Supercharged = true;
        }
    }
}