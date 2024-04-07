using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Cavern
{
    public class Spiders : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.BloodCrawler,
            NPCID.BloodCrawlerWall,
            NPCID.JungleCreeper,
            NPCID.JungleCreeperWall,
            NPCID.WallCreeper,
            NPCID.WallCreeperWall,
            NPCID.BlackRecluse,
            NPCID.BlackRecluseWall
        );

        public int Counter;
        public bool TargetIsWebbed;

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (npc.type == NPCID.WallCreeperWall || npc.type == NPCID.BloodCrawlerWall || npc.type == NPCID.JungleCreeperWall)
            {
                if (Counter == 300)
                    FargoSoulsUtil.DustRing(npc.Center, 32, DustID.Web, 9f, scale: 1.5f);

                if (Counter > 300)
                    npc.position -= npc.velocity;

                if (++Counter > 360)
                {
                    Counter = 0;
                    if (npc.HasValidTarget && npc.Distance(Main.player[npc.target].Center) < 450 && Collision.CanHitLine(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0) && FargoSoulsUtil.HostCheck)
                    {
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, 14f * npc.SafeDirectionTo(Main.player[npc.target].Center), ProjectileID.WebSpit, 9, 0f, Main.myPlayer);
                    }
                }
            }

            if (npc.type == NPCID.BlackRecluse || npc.type == NPCID.BlackRecluseWall)
            {
                if (++Counter > 10)
                {
                    Counter = 0;
                    TargetIsWebbed = false;

                    int t = npc.HasPlayerTarget ? npc.target : npc.FindClosestPlayer();
                    if (t != -1)
                    {
                        Player player = Main.player[t];
                        int b = player.FindBuffIndex(BuffID.Webbed);
                        TargetIsWebbed = b != -1; //remember if target is webbed until counter activates again
                        if (TargetIsWebbed)
                            player.AddBuff(ModContent.BuffType<DefenselessBuff>(), player.buffTime[b]);
                    }
                }

                if (TargetIsWebbed)
                {
                    npc.position += npc.velocity;
                }
            }
        }

        public override Color? GetAlpha(NPC npc, Color drawColor)
        {
            if (TargetIsWebbed)
            {
                drawColor.R = 255;
                drawColor.G /= 3;
                drawColor.B /= 3;
                return drawColor;
            }

            return base.GetAlpha(npc, drawColor);
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(ModContent.BuffType<InfestedBuff>(), 300);
        }

    }
}
