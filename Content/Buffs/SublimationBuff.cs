using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs
{
    public class SublimationBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true;
        }

        public override string Texture => "FargowiltasSouls/Content/Buffs/PlaceholderDebuff";

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.FargoSouls().Sublimation = true;
        }
    }
}