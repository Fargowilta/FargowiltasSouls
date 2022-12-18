using System.IO;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.EternityMode.Net
{
    public interface ISendStrategy
    {
        void Send(object value, BitWriter bitWriter, BinaryWriter binaryWriter);
    }
}
