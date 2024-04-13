
using Luminance.Common.StateMachines;

namespace FargowiltasSouls.Content.Bosses.ShadowChallenger
{
    public partial class ShadowChallenger
    {
        public const int FogTears_AttackLength = 240;

        [AutomatedMethodInvoke]
        public void LoadTransition_FogTears()
        {
            StateMachine.RegisterTransition(BehaviorStates.FogTears, null, false, () => Timer > FogTears_AttackLength);
        }

        [AutoloadAsBehavior<EntityAIState<BehaviorStates>, BehaviorStates>(BehaviorStates.FogTears)]
        public void DoBehavior_FogTears()
        {

        }
    }
}
