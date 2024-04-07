
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Boss
{
    public class MutantFangBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Mutant Fang");
            // Description.SetDefault("The power of Eternity Mode compels you");
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "突变毒牙");
            //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "永恒模式的力量压迫着你");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Terraria.ID.BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            FargoSoulsPlayer fargoPlayer = player.FargoSouls();
            player.poisoned = true;
            player.venom = true;
            player.ichor = true;
            player.onFire2 = true;
            player.electrified = true;
            fargoPlayer.OceanicMaul = true;
            fargoPlayer.CurseoftheMoon = true;
            if (fargoPlayer.FirstInfection)
            {
                fargoPlayer.MaxInfestTime = player.buffTime[buffIndex];
                fargoPlayer.FirstInfection = false;
            }
            fargoPlayer.Infested = true;
            fargoPlayer.Rotting = true;
            fargoPlayer.MutantNibble = true;
            fargoPlayer.noDodge = true;
            fargoPlayer.noSupersonic = true;
            fargoPlayer.MutantPresence = true;
            fargoPlayer.MutantFang = true;
            player.moonLeech = true;
            /*if (FargowiltasSouls.Instance.MasomodeEXLoaded && !WorldSavingSystem.downedFishronEX && player.buffTime[buffIndex] > 1
                && FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.mutantBoss, ModContent.NPCType<MutantBoss>()))
            {
                player.AddBuff(ModLoader.GetMod("MasomodeEX").BuffType("MutantJudgement"), player.buffTime[buffIndex]);
                player.buffTime[buffIndex] = 1;
            }*/
        }
    }
}
