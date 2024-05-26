using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Hallow
{
    public class Unicorn : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Unicorn);

        public int Counter;

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (Math.Abs(npc.velocity.X) >= 3f)
            {
                //spawn rainbows in mid jump only
                if (++Counter >= 3)
                {
                    Counter = 0;

                    int direction = npc.velocity.X > 0 ? 1 : -1;
                    int p = Projectile.NewProjectile(npc.GetSource_FromThis(), new Vector2(npc.Center.X - direction * (npc.width / 2), npc.Center.Y), npc.velocity, ProjectileID.RainbowBack, FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage, 4f / 3), 1);
                    if (p != Main.maxProjectiles)
                    {
                        Main.projectile[p].friendly = false;
                        Main.projectile[p].hostile = true;
                        Main.projectile[p].FargoSouls().Rainbow = true;
                    }
                }
            }

            //jump initiated
            /* if (npc.velocity.Y <= -6 && !masoBool[0])
             {
                 masoBool[0] = true;
                 Counter[1] = 20;
             }

             //spawn rainbows in mid jump only
             if (Counter[1] > 0 && Counter[0]++ >= 3)
             {
                 if (npc.velocity.Length() > 3)
                 {
                     int direction = npc.velocity.X > 0 ? 1 : -1;
                     int p = Projectile.NewProjectile(new Vector2(npc.Center.X - direction * (npc.width / 2), npc.Center.Y), npc.velocity, ProjectileID.RainbowBack, FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage, 4f / 3), 1);
                     if (p < 1000)
                     {
                         Main.projectile[p].friendly = false;
                         Main.projectile[p].hostile = true;
                         Main.projectile[p].FargoSouls().Rainbow = true;
                     }
                 }

                 Counter[0] = 0;
                 Counter[1]--;

                 if (Counter[1] == 0)
                 {
                     masoBool[0] = false;
                 }
             }*/
        }
    }
}
