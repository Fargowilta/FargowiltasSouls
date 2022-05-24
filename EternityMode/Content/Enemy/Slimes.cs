using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy
{
    public class Slimes : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.BlueSlime,
            NPCID.BlackSlime,
            NPCID.Pinky,
            NPCID.SlimeRibbonGreen,
            NPCID.SlimeRibbonRed,
            NPCID.SlimeRibbonWhite,
            NPCID.SlimeRibbonYellow,
            NPCID.SlimeMasked,
            NPCID.Slimeling,
            NPCID.Slimer,
            NPCID.Slimer2,
            NPCID.SlimeSpiked,
            NPCID.BabySlime,
            NPCID.BlackSlime,
            NPCID.CorruptSlime,
            NPCID.DungeonSlime,
            NPCID.DungeonSlime,
            NPCID.GoldenSlime,
            NPCID.GreenSlime,
            NPCID.IceSlime,
            NPCID.JungleSlime,
            NPCID.IlluminantSlime,
            NPCID.LavaSlime,
            NPCID.MotherSlime,
            NPCID.PurpleSlime,
            NPCID.QueenSlimeMinionPink,
            NPCID.QueenSlimeMinionPurple,
            NPCID.RainbowSlime,
            NPCID.RedSlime,
            NPCID.SandSlime,
            NPCID.SpikedIceSlime,
            NPCID.SpikedJungleSlime,
            NPCID.UmbrellaSlime,
            NPCID.YellowSlime,
            NPCID.Crimslime,
            NPCID.BigCrimslime,
            NPCID.LittleCrimslime,
            NPCID.ToxicSludge
        );

        public int Counter;

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            //slimes target nearest player on spawn
            npc.TargetClosest(true);

            if (npc.type == NPCID.JungleSlime && Main.rand.NextBool(5))
                npc.Transform(NPCID.SpikedJungleSlime);

            if (npc.type == NPCID.IceSlime && Main.rand.NextBool(5))
                npc.Transform(NPCID.SpikedIceSlime);

            if (npc.type == NPCID.BlueSlime)
            {
                if (Main.rand.NextBool(500))
                {
                    npc.Transform(NPCID.GoldenSlime);
                }
                else if (Main.slimeRain)
                {
                    if (Main.rand.NextBool(8))
                    {
                        npc.SetDefaults(Main.rand.Next(new int[] {
                            NPCID.RedSlime,
                            NPCID.PurpleSlime,
                            NPCID.YellowSlime,
                            NPCID.BlackSlime,
                            NPCID.SlimeSpiked
                        }));

                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                    }
                    /*else if (Main.rand.NextBool(50))
                    {
                        npc.SetDefaults(NPCID.Pinky);

                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                    }*/
                }
            }
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (npc.type == NPCID.BlueSlime && npc.netID == NPCID.Pinky)
            {
                //always be bouncing
                npc.ai[0] = -2000;
            }

            if (npc.type == NPCID.UmbrellaSlime)
            {
                if (npc.wet)
                    Counter = 30;

                if (Counter > 0)
                    Counter--;

                if (Counter <= 0 && npc.velocity.Y > 0)
                    npc.velocity.Y /= 10;
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(BuffID.Slimed, 120);

            switch (npc.type)
            {
                case NPCID.BlueSlime:
                    if (npc.type == NPCID.BlackSlime)
                    {
                        target.AddBuff(BuffID.Darkness, 300);
                    }

                    if (npc.netID == NPCID.Pinky)
                    {
                        target.GetModPlayer<FargoSoulsPlayer>().AddBuffNoStack(ModContent.BuffType<Stunned>(), 120);
                        target.velocity = Vector2.Normalize(target.Center - npc.Center) * 30;
                    }
                    break;

                case NPCID.UmbrellaSlime:
                    target.AddBuff(BuffID.Wet, 600);
                    break;

                case NPCID.IceSlime:
                case NPCID.SpikedIceSlime:
                    target.AddBuff(ModContent.BuffType<Hypothermia>(), 300);
                    break;

                case NPCID.JungleSlime:
                case NPCID.SpikedJungleSlime:
                    target.AddBuff(BuffID.Poisoned, 180);
                    break;

                case NPCID.MotherSlime:
                    target.AddBuff(ModContent.BuffType<Antisocial>(), 1200);
                    break;

                case NPCID.ToxicSludge:
                    target.AddBuff(ModContent.BuffType<Infested>(), 360);
                    break;

                case NPCID.CorruptSlime:
                    target.AddBuff(ModContent.BuffType<Rotting>(), 1200);
                    break;

                case NPCID.Crimslime:
                    target.AddBuff(ModContent.BuffType<Bloodthirsty>(), 300);
                    break;

                case NPCID.IlluminantSlime:
                    target.AddBuff(ModContent.BuffType<Purified>(), 300);
                    break;

                case NPCID.GoldenSlime:
                    target.AddBuff(ModContent.BuffType<Midas>(), 600);
                    break;

                default:
                    break;
            }
        }

        public override void OnKill(NPC npc)
        {
            base.OnKill(npc);

            if (npc.type == NPCID.BlueSlime)
            {
                void SplitIntoSlimes(int type, int amount)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = 0; i < amount; i++)
                        {
                            if (Main.rand.Next(3) != 0)
                                continue;

                            int n = FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromAI(), npc.Center, type,
                                velocity: new Vector2(npc.velocity.X * 2, npc.velocity.Y));

                            if (n != Main.maxNPCs)
                            {
                                Main.npc[n].velocity.X += Main.rand.Next(-20, 20) * 0.1f + i * npc.direction * 0.3f;
                                Main.npc[n].velocity.Y -= Main.rand.Next(0, 10) * 0.1f + i;
                            }
                        }
                    }
                }

                switch (npc.netID)
                {
                    case NPCID.YellowSlime:
                        SplitIntoSlimes(NPCID.PurpleSlime, 2);
                        break;

                    case NPCID.PurpleSlime:
                        SplitIntoSlimes(NPCID.RedSlime, 2);
                        break;

                    case NPCID.RedSlime:
                        SplitIntoSlimes(NPCID.GreenSlime, 2);
                        break;

                    case NPCID.Pinky:
                        SplitIntoSlimes(NPCID.YellowSlime, 2);
                        SplitIntoSlimes(NPCID.MotherSlime, 1);
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
