using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.NPCs;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.EternityMode.Content.Enemy.Cavern
{
    public abstract class Beetles : EModeNPCBehaviour
    {
        protected virtual int DustType { get; }

        protected virtual void BeetleEffect(NPC affectedNPC) { }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            for (int i = 0; i < 10; i++)
            {
                Vector2 offset = new Vector2();
                double angle = Main.rand.NextDouble() * 2d * Math.PI;
                offset.X += (float)(Math.Sin(angle) * 400);
                offset.Y += (float)(Math.Cos(angle) * 400);
                if (Collision.SolidCollision(npc.Center + offset, 0, 0))
                    continue;
                Dust dust = Main.dust[Dust.NewDust(
                    npc.Center + offset - new Vector2(4, 4), 0, 0,
                    DustType, 0, 0, 100, Color.White, 0.5f
                    )];
                dust.velocity = npc.velocity;
                if (Main.rand.NextBool(3))
                    dust.velocity += Vector2.Normalize(offset) * -5f;
                dust.noGravity = true;
            }

            foreach (NPC n in Main.npc.Where(n => n.active && !n.friendly && n.type != NPCID.CochinealBeetle && n.Distance(npc.Center) < 400))
            {
                BeetleEffect(n);
                n.GetGlobalNPC<EModeGlobalNPC>().BeetleTimer = 60;
                if (Main.rand.NextBool())
                {
                    int d = Dust.NewDust(n.position, n.width, n.height, 60, 0f, -1.5f, 0, new Color());
                    Main.dust[d].velocity *= 0.5f;
                    Main.dust[d].noLight = true;
                }
            }
        }
    }

    public class CochinealBeetle : Beetles
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.CochinealBeetle);
        protected override int DustType => DustID.RedTorch;
        protected override void BeetleEffect(NPC affectedNPC) => affectedNPC.GetGlobalNPC<EModeGlobalNPC>().BeetleOffenseAura = true;
    }

    public class CyanBeetle : Beetles
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.CyanBeetle);
        protected override int DustType => 187;
        protected override void BeetleEffect(NPC affectedNPC) => affectedNPC.GetGlobalNPC<EModeGlobalNPC>().BeetleUtilAura = true;
    }

    public class LacBeetle : Beetles
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.LacBeetle);
        protected override int DustType => 21;
        protected override void BeetleEffect(NPC affectedNPC) => affectedNPC.GetGlobalNPC<EModeGlobalNPC>().BeetleDefenseAura = true;
    }
}
