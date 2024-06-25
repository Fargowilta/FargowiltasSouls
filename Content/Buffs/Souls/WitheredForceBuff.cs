using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Souls
{
    public class WitheredForceBuff : ModBuff
    {

        public override string Texture => "FargowiltasSouls/Content/Buffs/Souls/WitheredBuff";

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.FargoSouls().CorruptedForce = true;
        }
    }
}