using System.IO;
using Terraria.ModLoader.IO;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.Projectiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.LunarEvents
{
    public class VortexEnemies : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.VortexLarva,
            NPCID.VortexHornet,
            NPCID.VortexHornetQueen,
            NPCID.VortexSoldier,
            NPCID.VortexRifleman
        );

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            npc.buffImmune[BuffID.Suffocation] = true;
            npc.buffImmune[ModContent.BuffType<ClippedWings>()] = true;
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(ModContent.BuffType<LightningRod>(), 300);
        }
    }

    public class VortexHornetQueen : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.VortexHornetQueen);

        public int Counter;

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (++Counter >= 240)
            {
                Counter = Main.rand.Next(30);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<LightningVortexHostile>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer);
            }
        }
    }

    public class VortexRifleman : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.VortexRifleman);

        public int Counter;

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            binaryWriter.Write7BitEncodedInt(Counter);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            Counter = binaryReader.Read7BitEncodedInt();
        }

        public override bool SafePreAI(NPC npc)
        {
            if (Counter > 0)
            {
                npc.velocity = Vector2.Zero;

                if (Counter >= 20 && Counter % 10 == 0)
                {
                    Vector2 vector2_1 = npc.Center;
                    vector2_1.X += npc.direction * 30f;
                    vector2_1.Y += 2f;

                    Vector2 vec = Vector2.UnitX * npc.direction * 8f;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int Damage = Main.expertMode ? 50 : 75;
                        for (int index = 0; index < 4; ++index)
                        {
                            Vector2 vector2_2 = vec + Utils.RandomVector2(Main.rand, -0.8f, 0.8f);
                            Projectile.NewProjectile(npc.GetSource_FromThis(), vector2_1.X, vector2_1.Y, vector2_2.X, vector2_2.Y, ProjectileID.MoonlordBullet, Damage, 1f, Main.myPlayer);
                        }
                    }

                    SoundEngine.PlaySound(SoundID.Item36, npc.Center);
                }

                if (++Counter >= 80)
                {
                    Counter = 0;
                    npc.netUpdate = true;
                    NetSync(npc);
                }

                return false;
            }

            return base.SafePreAI(npc);
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            //default: if (npc.localAI[2] >= 360f + Main.rand.Next(360) && etc)
            if (npc.localAI[2] >= 180f + Main.rand.Next(180) && npc.Distance(Main.player[npc.target].Center) < 400f && Math.Abs(npc.DirectionTo(Main.player[npc.target].Center).Y) < 0.5f && Collision.CanHitLine(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0))
            {
                npc.localAI[2] = 0f;
                Counter = 1;
                NetSync(npc);
            }
        }
    }
}
