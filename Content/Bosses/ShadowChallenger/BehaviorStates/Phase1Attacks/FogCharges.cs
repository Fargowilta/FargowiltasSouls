
using Luminance.Common.StateMachines;

namespace FargowiltasSouls.Content.Bosses.ShadowChallenger
{
    public partial class ShadowChallenger
    {
        public const int FogCharges_AttackLength = 360;

        [AutomatedMethodInvoke]
        public void LoadTransition_FogCharges()
        {
            StateMachine.RegisterTransition(BehaviorStates.FogCharges, BehaviorStates.FogTears, false, () => Timer > FogCharges_AttackLength);
        }

        [AutoloadAsBehavior<EntityAIState<BehaviorStates>, BehaviorStates>(BehaviorStates.FogCharges)]
        public void DoBehavior_FogCharges()
        {
            // TODO: Program attack.
        }
    }
}