using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class Lethargic : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lethargic");
            Description.SetDefault("Your weapons feel sluggish");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "昏昏欲睡");
            Description.AddTranslation((int)GameCulture.CultureName.Chinese, "你感觉你的武器变得缓慢");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            //all item speed reduced to 75%
            player.GetModPlayer<FargoSoulsPlayer>().AttackSpeed -= .25f;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<NPCs.FargoSoulsGlobalNPC>().Lethargic = true;
        }
    }
}