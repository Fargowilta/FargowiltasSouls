using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Masomode
{
    public class GodEaterBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("God Eater");
            // Description.SetDefault("Your soul is cursed by divine wrath");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            BuffID.Sets.IsATagBuff[Type] = true; //ignore most debuff immunity
                                                 //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "噬神者");
                                                 //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "你的灵魂被神明的忿怒所诅咒");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            //defense removed, endurance removed, colossal DOT (45 per second)
            player.FargoSouls().GodEater = true;
            player.FargoSouls().noDodge = true;
            player.FargoSouls().MutantPresence = true;
            player.moonLeech = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.defense = 0;
            npc.defDefense = 0;
            npc.FargoSouls().GodEater = true;
            npc.FargoSouls().HellFire = true;
        }
    }
}