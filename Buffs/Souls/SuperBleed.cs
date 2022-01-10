using FargowiltasSouls.NPCs;
using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Buffs.Souls
{
    public class SuperBleed : ModBuff
    {
        public override void SetDefaults()
        {
            //DisplayName.SetDefault("Blood Geyser");
            //Description.SetDefault("Spewing blood in self defense");
            Main.buffNoSave[Type] = true;
            //DisplayName.AddTranslation(GameCulture.Chinese, "大出血");
            //Description.AddTranslation(GameCulture.Chinese, "出于自卫而喷发出的血");
        }

        public override bool Autoload(ref string name, ref string texture)
        {
            texture = "FargowiltasSouls/Buffs/PlaceholderBuff";
            return true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<FargoSoulsGlobalNPC>().SBleed = true;
        }
    }
}