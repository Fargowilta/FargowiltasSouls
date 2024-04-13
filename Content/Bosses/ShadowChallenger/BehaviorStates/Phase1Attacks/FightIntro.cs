
using Luminance.Common.StateMachines;

namespace FargowiltasSouls.Content.Bosses.ShadowChallenger
{
    public partial class ShadowChallenger
    {
        public const int FightIntro_AttackLength = 120;

        [AutomatedMethodInvoke]
        public void LoadTransitions_FightIntro()
        {
            StateMachine.RegisterTransition(BehaviorStates.FightIntro, BehaviorStates.FogCharges, false, () => Timer > FightIntro_AttackLength);
        }

        [AutoloadAsBehavior<EntityAIState<BehaviorStates>, BehaviorStates>(BehaviorStates.FightIntro)]
        public void DoBehavior_FightIntro()
        {
            // TODO: Program attack.
        }
    }
}
