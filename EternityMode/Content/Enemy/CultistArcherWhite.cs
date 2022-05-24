using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.NPCs;
using FargowiltasSouls.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy
{
    public class CultistArcherWhite : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.CultistArcherWhite);

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.chaseable = true;
            npc.lavaImmune = false;
            npc.value = Item.buyPrice(0, 1);
            npc.lifeMax *= 2;
        }

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            if (Main.rand.NextBool(3) && NPC.downedGolemBoss)
                EModeGlobalNPC.Horde(npc, Main.rand.Next(2, 10));
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (npc.ai[1] > 0)
            {
                if (npc.ai[1] == 41) //skip vanilla shooting
                    npc.ai[1] = 39;

                if (npc.ai[1] > 10 && npc.ai[1] < 40 && npc.ai[1] % 10 == 5 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 speed = Main.player[npc.target].Center - npc.Center;
                    speed.Y -= Math.Abs(speed.X) * 0.1f; //account for gravity
                    speed.X += Main.rand.Next(-20, 21);
                    speed.Y += Main.rand.Next(-20, 21);
                    speed.Normalize();
                    speed *= 12f;

                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, speed, ModContent.ProjectileType<CultistArrow>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage, 4f * 4 / 9), 0f, Main.myPlayer);
                }
            }
        }
    }
}
