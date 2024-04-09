using FargowiltasSouls.Core.Systems;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Masomode
{
    public class MutantNibbleBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Mutant Nibble");
            // Description.SetDefault("You cannot heal at all");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;

            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "突变啃啄");
            //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "无法恢复生命");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            //disables potions, moon bite effect, feral bite effect, disables lifesteal
            player.FargoSouls().MutantNibble = true;

            //player.potionDelay = player.buffTime[buffIndex];
            player.moonLeech = true;

            //feral bite stuff
            player.rabid = true;

            if (WorldSavingSystem.MasochistModeReal && Main.rand.NextBool(1200))
            {
                switch (Main.rand.Next(10))
                {
                    case 0: player.AddBuff(ModContent.BuffType<DefenselessBuff>(), Main.rand.Next(300)); break;
                    case 1: player.AddBuff(ModContent.BuffType<LethargicBuff>(), Main.rand.Next(240)); break;
                    case 2: player.AddBuff(ModContent.BuffType<FlippedBuff>(), Main.rand.Next(120)); break;
                    case 3: player.AddBuff(ModContent.BuffType<HexedBuff>(), Main.rand.Next(120)); break;
                    case 4: player.AddBuff(ModContent.BuffType<MarkedforDeathBuff>(), Main.rand.Next(120)); break;
                    case 5: player.AddBuff(ModContent.BuffType<PurifiedBuff>(), Main.rand.Next(60)); break;
                    case 6: player.AddBuff(ModContent.BuffType<RottingBuff>(), Main.rand.Next(300)); break;
                    case 7: player.AddBuff(ModContent.BuffType<SqueakyToyBuff>(), Main.rand.Next(120)); break;
                    case 8: player.AddBuff(ModContent.BuffType<UnstableBuff>(), Main.rand.Next(90)); break;
                    case 9: player.AddBuff(ModContent.BuffType<BerserkedBuff>(), Main.rand.Next(180)); break;
                }
            }

            player.GetDamage(DamageClass.Generic) += 0.2f;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.FargoSouls().MutantNibble = true;
        }
    }
}