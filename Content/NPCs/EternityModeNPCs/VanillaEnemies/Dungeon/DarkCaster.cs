using FargowiltasSouls.Core.NPCMatching;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Dungeon
{
    public class DarkCaster : DungeonTeleporters
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.DarkCaster);

        public int AttackTimer;

        public bool SpawnedByTim;

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            bitWriter.WriteBit(SpawnedByTim);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            SpawnedByTim = bitReader.ReadBit();
        }

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            base.OnSpawn(npc, source);

            if (source is EntitySource_Parent parent && parent.Entity is NPC sourceNPC && sourceNPC.type == NPCID.Tim)
                SpawnedByTim = true;
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (++AttackTimer > 300)
            {
                AttackTimer = 0;

                if (!SpawnedByTim)
                {
                    for (int i = 0; i < 5; i++) //spray water bolts
                    {
                        SoundEngine.PlaySound(SoundID.Item21, npc.Center);
                        if (FargoSoulsUtil.HostCheck)
                        {
                            int p = Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Main.rand.NextVector2CircularEdge(-4.5f, 4.5f), ProjectileID.WaterBolt, FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage), 0f, Main.myPlayer);
                            if (p != Main.maxProjectiles)
                                Main.projectile[p].timeLeft = Main.rand.Next(180, 360);
                        }
                    }
                }
            }
        }

        public override bool CheckDead(NPC npc)
        {
            if (SpawnedByTim || !NPC.downedBoss3)
            {
                npc.life = 0;
                npc.HitEffect();
                npc.active = false;
                if (npc.DeathSound != null)
                    SoundEngine.PlaySound(npc.DeathSound.Value, npc.Center);
                return false;
            }

            return base.CheckDead(npc);
        }

    }
}
