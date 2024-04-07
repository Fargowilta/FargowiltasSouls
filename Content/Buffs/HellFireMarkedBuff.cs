using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs
{
    public class HellFireMarkedBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Hell Fire");
            Main.buffNoSave[Type] = true;
            Terraria.ID.BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
            Main.debuff[Type] = true;
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "地狱火");
        }

        public override string Texture => "FargowiltasSouls/Content/Buffs/PlaceholderDebuff";

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.FargoSouls().HellFireMarked = true;
        }
    }
}