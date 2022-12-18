using System.IO;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.EternityMode.Net
{
    public interface IRecieveStrategy
    {
        void Recieve(ref object value, BitReader bitReader, BinaryReader binaryReader);
    }
}
