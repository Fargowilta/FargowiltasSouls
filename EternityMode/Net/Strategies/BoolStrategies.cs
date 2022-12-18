using System.IO;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.EternityMode.Net.Strategies
{
    public static class BoolStrategies
    {
        public static CompoundStrategy CompoundStrategy = new CompoundStrategy(new SendBool(), new RecieveBool());

        public class SendBool : ISendStrategy
        {
            public void Send(object value, BitWriter bitWriter, BinaryWriter binaryWriter)
            {
                bitWriter.WriteBit((bool)value);
            }
        }

        public class RecieveBool : IRecieveStrategy
        {
            public void Recieve(ref object value, BitReader bitReader, BinaryReader binaryReader)
            {
                value = bitReader.ReadBit();
            }
        }
    }
}
