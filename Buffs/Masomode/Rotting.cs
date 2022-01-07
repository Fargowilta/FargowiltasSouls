using FargowiltasSouls.NPCs;
using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class Rotting : ModBuff
    {
        public override void SetDefaults()
        {
            //DisplayName.SetDefault("Rotting");
            //Description.SetDefault("Your body is wasting away");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            canBeCleared = true;
            //DisplayName.AddTranslation(GameCulture.Chinese, "腐败");
            //Description.AddTranslation(GameCulture.Chinese, "身体在逐渐衰弱");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            //inflicts DOT (8 per second) and almost every stat reduced (move speed by 25%, use time by 10%)
            player.GetModPlayer<FargoPlayer>().Rotting = true;
            //player.GetModPlayer<FargoPlayer>().AttackSpeed -= .1f;

            /*player.statLifeMax2 -= player.statLifeMax / 5;
            player.statDefense -= 10;
            //player.endurance -= 0.1f;
            //if (player.statDefense < 0) player.statDefense = 0;
            //if (player.endurance < 0) player.endurance = 0;

            player.meleeDamage -= 0.1f;
            player.magicDamage -= 0.1f;
            player.rangedDamage -= 0.1f;
            player.minionDamage -= 0.1f;*/
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<FargoSoulsGlobalNPC>().Rotting = true;
        }
    }
}