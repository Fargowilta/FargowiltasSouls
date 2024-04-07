using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Masomode
{
    public class NeurotoxinBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Neurotoxin");
            // Description.SetDefault("Your body is shutting down");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Terraria.ID.BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            FargoSoulsPlayer p = player.FargoSouls();

            player.poisoned = true;
            player.venom = true;
            player.slowOgreSpit = true;

            player.ClearBuff(ModContent.BuffType<InfestedBuff>());

            p.MaxInfestTime = 2;
            p.FirstInfection = false;
            p.Infested = true;
        }
    }
}