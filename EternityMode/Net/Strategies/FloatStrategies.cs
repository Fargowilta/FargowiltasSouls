using System.IO;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.EternityMode.Net.Strategies
{
    public static class FloatStrategies
    {
        public static CompoundStrategy CompoundStrategy = new CompoundStrategy(new SendFloat(), new RecieveFloat());

        public class SendFloat : ISendStrategy
        {
            public void Send(object value, BitWriter bitWriter, BinaryWriter binaryWriter)
            {
                binaryWriter.Write((float)value);
            }
        }

        public class RecieveFloat : IRecieveStrategy
        {
            public void Recieve(ref object value, BitReader bitReader, BinaryReader binaryReader)
            {
                value = binaryReader.ReadSingle();
            }
        }
    }
}
