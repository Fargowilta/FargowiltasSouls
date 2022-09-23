using FargowiltasSouls.NPCs;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Souls
{
    public class OriPoison : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Orichalcum Poison");
            Main.buffNoSave[Type] = true;
            Terraria.ID.BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
            Main.debuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<FargoSoulsGlobalNPC>().OriPoison = true;
        }
    }
}