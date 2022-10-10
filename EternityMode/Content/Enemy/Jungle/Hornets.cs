using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.NPCs;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.Jungle
{
    public class Hornets : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.Hornet,
            NPCID.HornetFatty,
            NPCID.HornetHoney,
            NPCID.HornetLeafy,
            NPCID.HornetSpikey,
            NPCID.HornetStingy,
            NPCID.BigHornetFatty,
            NPCID.BigHornetHoney,
            NPCID.BigHornetLeafy,
            NPCID.BigHornetSpikey,
            NPCID.BigHornetStingy,
            NPCID.BigMossHornet,
            NPCID.GiantMossHornet,
            NPCID.LittleHornetFatty,
            NPCID.LittleHornetHoney,
            NPCID.LittleHornetLeafy,
            NPCID.LittleHornetSpikey,
            NPCID.LittleHornetStingy,
            NPCID.LittleMossHornet,
            NPCID.MossHornet,
            NPCID.TinyMossHornet
        );

        public int Timer;

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            npc.buffImmune[BuffID.Poisoned] = true;
            npc.buffImmune[BuffID.Venom] = true;
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (++Timer > 420) //put here so they dont all dash at once after you get swarming
            {
                Timer = 0;

                //move in more frequently when especially far away
                if (npc.HasPlayerTarget && npc.Distance(Main.player[npc.target].Center) > 1200)
                    Timer += 90;
            }

            if (npc.HasPlayerTarget)
            {
                bool shouldNotTileCollide = npc.HasValidTarget
                    && Main.player[npc.target].GetModPlayer<FargoSoulsPlayer>().Swarming
                    && !Collision.CanHitLine(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0);
                if (shouldNotTileCollide)
                    npc.noTileCollide = true;
                else if (npc.noTileCollide && !Collision.SolidCollision(npc.position, npc.width, npc.height)) //still intangible, but should stop, and isnt on tiles
                    npc.noTileCollide = false;

                if (npc.noTileCollide || (npc.HasValidTarget && Main.player[npc.target].GetModPlayer<FargoSoulsPlayer>().Swarming))
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, 44, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f);
                    Main.dust[d].noGravity = true;

                    if (Timer == 0) //if player behind blocks, periodically dash closer
                    {
                        if (!Collision.CanHitLine(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0))
                            npc.velocity = Math.Min(6f, npc.velocity.Length()) * npc.DirectionTo(Main.player[npc.target].Center);

                        npc.netUpdate = true;
                        NetSync(npc);
                    }
                }
            }
        }

        public override bool CheckDead(NPC npc)
        {
            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.beeBoss, NPCID.QueenBee))
            {
                npc.active = false;
                if (npc.DeathSound != null)
                    SoundEngine.PlaySound(npc.DeathSound.Value, npc.Center);
                return false;
            }

            return base.CheckDead(npc);
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(ModContent.BuffType<Infested>(), 300);
            target.AddBuff(ModContent.BuffType<Swarming>(), 600);
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            if (npc.type == NPCID.MossHornet)
                FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.Stinger, 2));

            FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.JungleGrassSeeds, 10));
        }
    }
}
