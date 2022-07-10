using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.Projectiles.Masomode;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.Cavern
{
    public class Mimics : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.Mimic,
            NPCID.PresentMimic,
            NPCID.IceMimic,
            NPCID.BigMimicCorruption,
            NPCID.BigMimicCrimson,
            NPCID.BigMimicHallow,
            NPCID.BigMimicJungle
        );

        public int InvulFrameTimer;

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (npc.type == NPCID.Mimic || npc.type == NPCID.PresentMimic)
            {
                npc.dontTakeDamage = false;
                if (npc.justHit && Main.hardMode)
                    InvulFrameTimer = 20;
                if (InvulFrameTimer > 0)
                {
                    InvulFrameTimer--;
                    npc.dontTakeDamage = true;
                }
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(ModContent.BuffType<Midas>(), 600);
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.GoldenCrate));
        }

        public override void OnKill(NPC npc)
        {
            base.OnKill(npc);

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int max = 5;
                for (int i = 0; i < max; i++)
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.position.X + Main.rand.Next(npc.width), npc.position.Y + Main.rand.Next(npc.height),
                        Main.rand.Next(-30, 31) * .1f, Main.rand.Next(-40, -15) * .1f, ModContent.ProjectileType<FakeHeart>(), 20, 0f, Main.myPlayer);
            }
        }
    }
}
