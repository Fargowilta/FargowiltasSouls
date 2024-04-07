namespace FargowiltasSouls.Core.NPCMatching.Conditions
{
    public class MatchEverythingCondition : INPCMatchCondition
    {
        public bool Satisfies(int type) => true;
    }
}
