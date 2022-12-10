using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.NPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.Hell
{
    public class Demons : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.Demon,
            NPCID.VoodooDemon,
            NPCID.RedDevil
        );

        public int Counter;

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            if (Main.hardMode && Main.rand.NextBool(4))
                EModeGlobalNPC.Horde(npc, Main.rand.Next(5) + 1);
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (npc.HasValidTarget)
            {
                //doing it this way so demon regains collision as soon as any part of it has line of sight
                Vector2 myClosestPoint = FargoSoulsUtil.ClosestPointInHitbox(npc, Main.player[npc.target].Center);
                Vector2 targetClosestPoint = FargoSoulsUtil.ClosestPointInHitbox(Main.player[npc.target], npc.Center);
                npc.noTileCollide = !Collision.CanHitLine(myClosestPoint, 0, 0, targetClosestPoint, 0, 0);
            }

            if ((npc.type == NPCID.Demon && npc.ai[0] == 100f)
                || (npc.type == NPCID.RedDevil && ++Counter > 300))
            {
                Counter = 0;

                int t = npc.HasPlayerTarget ? npc.target : npc.FindClosestPlayer();
                if (t != -1 && npc.Distance(Main.player[t].Center) < 800 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int amount = npc.type == NPCID.RedDevil ? 9 : 6;
                    int damage = FargoSoulsUtil.ScaledProjectileDamage(npc.damage, npc.type == NPCID.RedDevil ? 4f / 3 : 1);
                    FargoSoulsUtil.XWay(amount, npc.GetSource_FromThis(), npc.Center, ProjectileID.DemonSickle, 1, damage, .5f);
                }
            }

            if (npc.type == NPCID.VoodooDemon) //can ignite itself to burn up its doll
            {
                const int dollBurningTime = 720;

                if (npc.lavaWet && npc.HasValidTarget
                    && (npc.Distance(Main.player[npc.target].Center) < 450 || Collision.CanHitLine(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0)))
                {
                    npc.buffImmune[BuffID.OnFire] = false;
                    npc.buffImmune[BuffID.OnFire3] = false;
                    npc.AddBuff(BuffID.OnFire, dollBurningTime + 60);
                }

                if (npc.onFire || npc.onFire3)
                {
                    SoundEngine.PlaySound(SoundID.NPCDeath10 with { Pitch = 0.5f }, npc.Center);

                    for (int i = 0; i < 3; i++) //NOTICE ME
                    {
                        int d = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Torch, 0f, 0f, 0, default(Color), (float)Counter / dollBurningTime * 5f);
                        Main.dust[d].noGravity = !Main.rand.NextBool(5);
                        Main.dust[d].noLight = true;
                        Main.dust[d].velocity *= Main.rand.NextFloat(12f);
                    }

                    if (++Counter > dollBurningTime) //doll fully burned up
                    {
                        npc.Transform(NPCID.Demon);

                        int guide = NPC.FindFirstNPC(NPCID.Guide);
                        if (guide != -1 && Main.npc[guide].active && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Main.npc[guide].StrikeNPC(9999, 0f, 0);

                            int p = npc.HasPlayerTarget ? npc.target : npc.FindClosestPlayer();
                            if (p != -1 && !FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.wallBoss, NPCID.WallofFlesh))
                                NPC.SpawnWOF(Main.player[npc.target].Center);

                        }
                    }
                }
            }
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            base.UpdateLifeRegen(npc, ref damage);

            if (npc.type == NPCID.VoodooDemon && npc.onFire)
                damage /= 2;
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.Blindfold, 50));
        }
    }
}
