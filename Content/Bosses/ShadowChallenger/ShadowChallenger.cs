using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.ShadowChallenger
{
    public partial class ShadowChallenger : ModNPC
    {
        // Change this to true to be able to fight the boss. Do NOT push with this being true until it is done and you are called Toasty.
        public override bool IsLoadingEnabled(Mod mod) => false;

        /// <summary>
        /// The behavior states used by the bosses state machine.
        /// </summary>
        public enum BehaviorStates
        {
            FightIntro,

            // Phase 1
            FogCharges,
            FogTears,

            // State Machine stuff.
            RefillStates,
            Count
        }

        /// <summary>
        /// The current phase the boss is in.
        /// </summary>
        public ref float CurrentPhase => ref NPC.ai[0];

        public float Timer
        {
            get => StateMachine.CurrentState.Time;
            set => StateMachine.CurrentState.Time = (int)value;
        }

        /// <summary>
        /// An auto netsynced variable whos value is reset each state change. Use for per attack information storage.
        /// </summary>
        public ref float AI1 => ref NPC.ai[1];

        /// <summary>
        /// An auto netsynced variable whos value is reset each state change. Use for per attack information storage.
        /// </summary>
        public ref float AI2 => ref NPC.ai[2];

        /// <summary>
        /// An auto netsynced variable whos value is reset each state change. Use for per attack information storage.
        /// </summary>
        public ref float AI3 => ref NPC.ai[3];

        public override void SetDefaults()
        {
            NPC.width = 50;
            NPC.height = 50;
            NPC.lifeMax = 50000;

            // TODO: Add more and make the above correct.
        }

        public override void AI()
        {
            // Refill the state machine if it is empty.
            if ((StateMachine?.StateStack?.Count ?? 1) <= 0)
                StateMachine.StateStack.Push(StateMachine.StateRegistry[BehaviorStates.RefillStates]);

            StateMachine.PerformBehaviors();
            StateMachine.PerformStateTransitionCheck();

            if (StateMachine.StateStack.Count > 0)
                Timer++;
        }
    }
}
