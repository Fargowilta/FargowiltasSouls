using Luminance.Common.StateMachines;

namespace FargowiltasSouls.Content.Bosses.ShadowChallenger
{
    public partial class ShadowChallenger
    {
        private PushdownAutomata<EntityAIState<BehaviorStates>, BehaviorStates> stateMachine;

        /// <summary>
        /// The state machine that controls the behavior of this NPC.
        /// </summary>
        public PushdownAutomata<EntityAIState<BehaviorStates>, BehaviorStates> StateMachine
        {
            get
            {
                if (stateMachine == null)
                    SetupStateMachine();

                return stateMachine;
            }
            private set => stateMachine = value;
        }

        private void SetupStateMachine()
        {
            StateMachine = new(new(BehaviorStates.FightIntro));

            for (int i = 0; i < (int)BehaviorStates.Count; i++)
                StateMachine.RegisterState(new((BehaviorStates)i));

            StateMachine.OnStateTransition += OnAnyStateTransition;

            // This autoloads the behavior states into the machine.
            AutoloadAsBehavior<EntityAIState<BehaviorStates>, BehaviorStates>.FillStateMachineBehaviors(StateMachine, this);

            // This loads the transitions.
            AutomatedMethodInvokeAttribute.InvokeWithAttribute(this);
        }

        private void OnAnyStateTransition(bool stateWasPopped, EntityAIState<BehaviorStates> oldState)
        {
            NPC.netUpdate = true;
            NPC.TargetClosest(false);

            // Only reset these if the last state was popped, otherwise things such as teleport states that return to the previous state would cause important information to be lost.
            if (stateWasPopped)
            {
                AI1 = 0f;
                AI2 = 0f;
                AI3 = 0f;
            }
        }
    }
}
