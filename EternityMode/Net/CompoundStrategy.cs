using System.IO;
using Terraria.ModLoader.IO;
namespace FargowiltasSouls.EternityMode.Net
{
    public struct CompoundStrategy
    {
        public ISendStrategy SendStrategy;
        public IRecieveStrategy RecieveStrategy;

        public CompoundStrategy(ISendStrategy sendStrategy, IRecieveStrategy recieveStrategy)
        {
            SendStrategy = sendStrategy;
            RecieveStrategy = recieveStrategy;
        }

        public void Send(object value, BitWriter bitWriter, BinaryWriter binaryWriter)
            => SendStrategy.Send(value, bitWriter, binaryWriter);

        public void Recieve(ref object value, BitReader bitReader, BinaryReader binaryReader)
            => RecieveStrategy.Recieve(ref value, bitReader, binaryReader);
    }
}
