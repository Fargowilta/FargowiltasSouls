using System.IO;

namespace FargowiltasSouls.Content.Bosses.ShadowChallenger
{
    public partial class ShadowChallenger
    {
        public override void SendExtraAI(BinaryWriter writer)
        {
            // 1. Write the number of states on the stack.
            writer.Write(StateMachine.StateStack.Count);

            // 2. Write the state IDs as ints to the stack in the order they are on the stack. Also write the timers.
            var stackArray = StateMachine.StateStack.ToArray();
            for (int i = 0; i < StateMachine.StateStack.Count; i++)
            {
                writer.Write((int)stackArray[i].Identifier);
                writer.Write(stackArray[i].Time);
            }
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            // 1. Read the number of states that were written.
            int stackCount = reader.ReadInt32();
            StateMachine.StateStack.Clear();

            // 2. Read the state IDs, read and assign the timer, and push to the stack.
            for (int i = 0; i < stackCount; i++)
            {
                var behavior = StateMachine.StateRegistry[(BehaviorStates)reader.ReadInt32()];
                behavior.Time = reader.ReadInt32();
                StateMachine.StateStack.Push(behavior);
            }
        }
    }
}
