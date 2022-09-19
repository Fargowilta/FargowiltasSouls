using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class Smite : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Smite");
            Description.SetDefault("Life regen reduced and 10% more damage taken");
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "惩戒");
            //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "降低生命恢复速度，受到的伤害增加10%");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoSoulsPlayer>().Smite = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<NPCs.FargoSoulsGlobalNPC>().Smite = true;
        }
    }
}