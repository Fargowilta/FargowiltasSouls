namespace FargowiltasSouls.Core.NPCMatching.Conditions
{
    public class MatchTypeCondition : INPCMatchCondition
    {
        public int Type;

        public MatchTypeCondition(int type)
        {
            Type = type;
        }

        public bool Satisfies(int type) => type == Type;
    }
}
