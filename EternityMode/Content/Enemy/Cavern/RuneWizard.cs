using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.Items.Accessories.Masomode;
using FargowiltasSouls.NPCs;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.Cavern
{
    public class RuneWizard : Teleporters
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.RuneWizard);

        public int AttackTimer;

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.lavaImmune = true;
            npc.lifeMax *= 4;
            npc.damage /= 2;
        }

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            base.OnSpawn(npc, source);

            npc.buffImmune[BuffID.OnFire] = true;
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (++AttackTimer > 300)
            {
                AttackTimer = 0;
                if (Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget)
                {
                    Vector2 vel = npc.DirectionFrom(Main.player[npc.target].Center) * 8f;
                    for (int i = 0; i < 5; i++)
                    {
                        int p = Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, vel.RotatedBy(2 * Math.PI / 5 * i),
                            ProjectileID.RuneBlast, 30, 0f, Main.myPlayer, 1);
                        if (p != Main.maxProjectiles)
                            Main.projectile[p].timeLeft = 300;
                    }
                }
            }

            EModeGlobalNPC.Aura(npc, 450f, true, 74, Color.GreenYellow, ModContent.BuffType<Hexed>());
            EModeGlobalNPC.Aura(npc, 150f, false, 73, default, ModContent.BuffType<Hexed>(), BuffID.Suffocation);
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ModContent.ItemType<MysticSkull>(), 5));
        }
    }
}
