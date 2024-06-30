using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Souls
{
    public class WitheredWizardBuff : ModBuff
    {

        public override string Texture => "FargowiltasSouls/Content/Buffs/Souls/WitheredBuff";

        public override void Update(NPC npc, ref int buffIndex)
        {
            bool check = Main.player.Any(p => p.Alive() && p.HasEffect<EbonwoodEffect>() && p.ForceEffect<EbonwoodEffect>());
            if (check)
            {
                npc.buffTime[buffIndex] = 60;
            }
            npc.FargoSouls().CorruptedForce = true;
        }
    }
}