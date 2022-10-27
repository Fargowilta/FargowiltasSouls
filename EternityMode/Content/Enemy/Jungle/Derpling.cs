using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.NPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.Jungle
{
    public class Derpling : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Derpling);

        public int Counter;

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.scale *= .5f;
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            EModeGlobalNPC.Aura(npc, 200, BuffID.Bleeding, false, DustID.Blood);

            if (++Counter > 10)
            {
                Counter = 0;

                if (Main.LocalPlayer.active && !Main.LocalPlayer.ghost && !Main.LocalPlayer.dead
                    && Main.LocalPlayer.bleed && npc.Distance(Main.LocalPlayer.Center) < 200)
                {
                    const int damage = 5;

                    Player target = Main.LocalPlayer;
                    target.statLife -= damage;
                    CombatText.NewText(target.Hitbox, Color.Red, damage, false, true);

                    if (target.statLife < 0)
                    {
                        target.KillMe(PlayerDeathReason.ByCustomReason(Language.GetTextValue("Mods.FargowiltasSouls.DeathMessage.Derpling", target.name)), 999, 0);
                    }

                    npc.life += damage;
                    if (npc.life > npc.lifeMax)
                        npc.life = npc.lifeMax;

                    npc.HealEffect(damage);
                }
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(BuffID.Confused, 180);
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.TrifoldMap, 50));
        }
    }
}
