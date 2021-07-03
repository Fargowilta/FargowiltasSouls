using FargowiltasSouls.NPCs;
using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Buffs.Souls
{
    public class OriPoison : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Orichalcum Poison");
            DisplayName.AddTranslation(GameCulture.Chinese, "山铜中毒");
            Main.buffNoSave[Type] = true;
            canBeCleared = false;
            Main.debuff[Type] = true;
        }

        public override bool Autoload(ref string name, ref string texture)
        {
            texture = "FargowiltasSouls/Buffs/PlaceholderDebuff";
            return true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<FargoSoulsGlobalNPC>().OriPoison = true;
        }
    }
}
