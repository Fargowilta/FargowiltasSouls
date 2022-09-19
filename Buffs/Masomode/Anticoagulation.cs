using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class Anticoagulation : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Anticoagulation");
            Description.SetDefault("Losing life, shed blood when hurt, enemies will drink it and grow stronger");
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "凝血失效");
            //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "生命流失，受伤时鲜血四溅，敌怪会吸收你溅出的鲜血并变得更强");

            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.bleed = true;
            player.GetModPlayer<FargoSoulsPlayer>().Anticoagulation = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<NPCs.FargoSoulsGlobalNPC>().Anticoagulation = true;
        }
    }
}