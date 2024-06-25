using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;

namespace FargowiltasSouls.Content.Buffs.Souls
{
    public class TimberBleedBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
        }

        public override string Texture => "FargowiltasSouls/Content/Buffs/PlaceholderBuff";

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.FargoSouls().TimberBleed = true;
        }
    }
}
