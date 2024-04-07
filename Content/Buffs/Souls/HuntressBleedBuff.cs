using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Souls
{
    public class HuntressBleedBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Huntress Bleed");
            Main.buffNoSave[Type] = true;
            Terraria.ID.BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
            Main.debuff[Type] = true;
        }

        public override string Texture => "FargowiltasSouls/Content/Buffs/PlaceholderDebuff";

        public override void Update(NPC npc, ref int buffIndex)
        {
            //npc.FargoSouls().OriPoison = true;
        }
    }
}