using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs
{
    public class Coldheart : ModBuff
    {
        public override string Texture => "FargowiltasSouls/Buffs/PlaceholderDebuff";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Coldheart");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Terraria.ID.BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public const int STACK_DURATION = 240;

        float EndurancePenalty(int timeLeft)
        {
            const float maxPenaltyForOneStack = 0.1f;
            return maxPenaltyForOneStack * timeLeft / STACK_DURATION;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.endurance -= EndurancePenalty(player.buffTime[buffIndex]);
        }

        public override void ModifyBuffTip(ref string tip, ref int rare)
        {
            int buffIndex = Main.LocalPlayer.FindBuffIndex(Type);
            if (buffIndex == -1)
                return;
            int timeLeft = Main.LocalPlayer.buffTime[buffIndex];
            tip = FargoSoulsUtil.IsChinese() ? $"减少{System.Math.Round(EndurancePenalty(timeLeft) * 100)}%伤害减免" : $"{System.Math.Round(EndurancePenalty(timeLeft) * 100)}% less damage reduction";
        }

        public override bool ReApply(Player player, int time, int buffIndex)
        {
            player.buffTime[buffIndex] += time;
            return base.ReApply(player, time, buffIndex);
        }
    }
}