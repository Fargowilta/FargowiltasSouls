using FargowiltasSouls.NPCs;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class Rotting : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rotting");
            Description.SetDefault("Your body is wasting away");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;

            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "腐败");
            //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "身体在逐渐衰弱");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            //inflicts DOT (8 per second) and almost every stat reduced (move speed by 25%, use time by 10%)
            player.GetModPlayer<FargoSoulsPlayer>().Rotting = true;
            //player.GetModPlayer<FargoSoulsPlayer>().AttackSpeed -= .1f;

            /*player.statLifeMax2 -= player.statLifeMax / 5;
            player.statDefense -= 10;
            //player.endurance -= 0.1f;
            //if (player.statDefense < 0) player.statDefense = 0;
            //if (player.endurance < 0) player.endurance = 0;

            player.GetDamage(DamageClass.Melee) -= 0.1f;
            player.GetDamage(DamageClass.Magic) -= 0.1f;
            player.GetDamage(DamageClass.Ranged) -= 0.1f;
            player.GetDamage(DamageClass.Summon) -= 0.1f;*/
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<FargoSoulsGlobalNPC>().Rotting = true;
        }
    }
}