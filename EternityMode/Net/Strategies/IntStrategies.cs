using System.IO;
using Terraria.IO;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.EternityMode.Net.Strategies
{
    public static class IntStrategies
    {
        public static CompoundStrategy CompoundStrategy = new CompoundStrategy(new SendInt(), new RecieveInt());

        public class SendInt : ISendStrategy
        {
            public void Send(object value, BitWriter bitWriter, BinaryWriter binaryWriter)
            {
                binaryWriter.Write7BitEncodedInt((int)value);
            }
        }

        public class RecieveInt : IRecieveStrategy
        {
            public void Recieve(ref object value, BitReader bitReader, BinaryReader binaryReader)
            {
                value = binaryReader.Read7BitEncodedInt();
            }
        }
    }
}
