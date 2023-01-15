using System.IO;
using Terraria.ModLoader.IO;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.Items.Accessories.Masomode;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Boss
{
    public class MartianSaucer : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.MartianSaucerCore);

        public int AttackTimer;
        public int RayCounter;

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            binaryWriter.Write7BitEncodedInt(AttackTimer);
            binaryWriter.Write7BitEncodedInt(RayCounter);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            AttackTimer = binaryReader.Read7BitEncodedInt();
            RayCounter = binaryReader.Read7BitEncodedInt();
        }

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            npc.buffImmune[ModContent.BuffType<ClippedWings>()] = true;
        }

        public override bool SafePreAI(NPC npc)
        {
            if (RayCounter > 3)
            {
                npc.velocity = Vector2.Zero;

                const int time = 300;
                const int interval = 4;
                const int maxSplit = 10;

                if (++AttackTimer % interval == 0)
                {
                    if (npc.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 baseVel = 10f * npc.DirectionFrom(Main.player[npc.target].Center);
                        const int splitInterval = time / maxSplit;
                        int max = AttackTimer / splitInterval + 2;
                        for (int i = 0; i < max; i++)
                        {
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center,
                                baseVel.RotatedBy(MathHelper.TwoPi / max * i), ProjectileID.SaucerLaser, FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer);
                        }
                    }
                }

                if (AttackTimer >= time - 5)
                {
                    AttackTimer = 0;
                    RayCounter = 0;
                    npc.ai[0] = 0;
                    npc.ai[1] = 0;
                    npc.ai[2] = 0;
                    npc.ai[3] = 0;
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

            NPCs.EModeGlobalNPC.Aura(npc, 200, BuffID.VortexDebuff, false, DustID.Vortex);

            if (!npc.dontTakeDamage && npc.HasValidTarget)
            {
                if ((npc.ai[3] - 60) % 120 == 65)
                {
                    RayCounter++;
                }

                if ((npc.ai[3] - 60) % 120 == 0)
                {
                    AttackTimer = 20;
                }

                if (AttackTimer > 0 && --AttackTimer % 2 == 0)
                {
                    Vector2 speed = 14f * npc.DirectionTo(Main.player[npc.target].Center).RotatedBy((Main.rand.NextDouble() - 0.5) * 0.785398185253143 / 5.0);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, speed, ProjectileID.SaucerLaser, FargoSoulsUtil.ScaledProjectileDamage(npc.damage, 4f / 6), 0f, Main.myPlayer);
                }
            }
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ModContent.ItemType<SaucerControlConsole>(), 5));
        }
    }
}
