using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Souls
{
    public class EarthPoison : ModBuff
    {
        public override string Texture => "FargowiltasSouls/Content/Buffs/Souls/LeadPoisonBuff";
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Terraria.ID.BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
            Main.debuff[Type] = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.FargoSouls().EarthPoison = true;
        }
    }
}
