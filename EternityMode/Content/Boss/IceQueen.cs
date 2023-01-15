using System.IO;
using Terraria.ModLoader.IO;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.Items.Accessories.Masomode;
using FargowiltasSouls.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Boss
{
    public class IceQueen : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.IceQueen);

        public int AttackTimer;

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            binaryWriter.Write7BitEncodedInt(AttackTimer);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            AttackTimer = binaryReader.Read7BitEncodedInt();
        }

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            npc.buffImmune[ModContent.BuffType<ClippedWings>()] = true;
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            /*Counter[0]++;

            short countCap = 14;
            if (npc.life < npc.lifeMax * 3 / 4)
                countCap--;
            if (npc.life < npc.lifeMax / 2)
                countCap -= 2;
            if (npc.life < npc.lifeMax / 4)
                countCap -= 3;
            if (npc.life < npc.lifeMax / 10)
                countCap -= 4;

            if (Counter[0] > countCap)
            {
                Counter[0] = 0;
                if (++Counter[1] > 25)
                {
                    Counter[1] = 0;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, NPCID.Flocko);
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                    }
                }
                Vector2 speed = new Vector2(Main.rand.Next(-1000, 1001), Main.rand.Next(-1000, 1001));
                speed.Normalize();
                speed *= 12f;
                Vector2 spawn = npc.Center;
                spawn.Y -= 20f;
                spawn += speed * 4f;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(spawn, speed, ProjectileID.FrostShard, 30, 0f, Main.myPlayer);
            }*/

            if (--AttackTimer <= 0)
            {
                AttackTimer = 120;
                if (npc.whoAmI == NPC.FindFirstNPC(npc.type) && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (npc.ai[0] == 2) //stationary, spinning
                    {
                        AttackTimer = 75;

                        for (int i = 0; i < 16; i++)
                        {
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center,
                                8f * npc.DirectionTo(Main.player[npc.target].Center).RotatedBy(MathHelper.Pi / 8 * i),
                                ProjectileID.FrostWave, FargoSoulsUtil.ScaledProjectileDamage(npc.damage, 0.8f), 0f, Main.myPlayer);
                        }
                    }
                    else
                    {
                        Vector2 speed = new Vector2(Main.rand.NextFloat(40f), Main.rand.NextFloat(-20f, 20f));
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, speed,
                            ModContent.ProjectileType<QueenFlocko>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage, 0.8f), 0f, Main.myPlayer, npc.whoAmI, -1);
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, -speed,
                            ModContent.ProjectileType<QueenFlocko>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage, 0.8f), 0f, Main.myPlayer, npc.whoAmI, 1);
                    }
                }
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(ModContent.BuffType<Hypothermia>(), 600);
            target.AddBuff(BuffID.Frostburn, 180);
            target.GetModPlayer<FargoSoulsPlayer>().AddBuffNoStack(BuffID.Frozen, 30);
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ModContent.ItemType<IceQueensCrown>(), 5));
        }
    }
}
