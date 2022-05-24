using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.NPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.Hallow
{
    public class Pixie : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Pixie);

        public int Counter;

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.noTileCollide = true;
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (npc.HasPlayerTarget)
            {
                if (npc.velocity.Y < 0f && npc.position.Y < Main.player[npc.target].position.Y)
                    npc.velocity.Y = 0f;
                if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) < 200)
                    Counter++;
            }
            if (Counter >= 60)
            {
                if (!Main.dedServ)
                    SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Sounds/Navi") { Pitch = 0.5f }, npc.Center);
                Counter = 0;
            }
            EModeGlobalNPC.Aura(npc, 100, ModContent.BuffType<SqueakyToy>());
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.EmpressButterfly, 20));
        }
    }
}
