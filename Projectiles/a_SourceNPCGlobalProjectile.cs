using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.Projectiles
{
    //weird name so it goes before the other globalprojectiles
    //TODO: make this a proper inherited class and a modproj equivalent, axe the dictionary
    public class A_SourceNPCGlobalProjectile : GlobalProjectile
    {
        public static Dictionary<int, bool> SourceNPCSync = new Dictionary<int, bool>();
        public static Dictionary<int, bool> DamagingSync = new Dictionary<int, bool>();

        public override void Unload()
        {
            base.Unload();

            SourceNPCSync.Clear();
            DamagingSync.Clear();
        }

        public override bool InstancePerEntity => true;

        public NPC sourceNPC;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source is EntitySource_Parent parent)
            {
                if (parent.Entity is NPC)
                {
                    projectile.SetSourceNPC(parent.Entity as NPC);
                }
                else if (parent.Entity is Projectile)
                {
                    Projectile sourceProj = parent.Entity as Projectile;
                    projectile.SetSourceNPC(sourceProj.GetSourceNPC());
                }
            }
        }

        private bool NeedsNPCSync(Projectile projectile)
            => SourceNPCSync.TryGetValue(projectile.type, out bool value) && value;

        private bool NeedsDmgSync(Projectile projectile)
            => DamagingSync.TryGetValue(projectile.type, out bool value) && value;

        private bool NeedsSync(Dictionary<int, bool> dict, Projectile projectile)
            => dict.TryGetValue(projectile.type, out bool value) && value;

        public override void SendExtraAI(Projectile projectile, BitWriter bits, BinaryWriter writer)
        {
            if (NeedsSync(SourceNPCSync, projectile))
                writer.Write(sourceNPC is NPC ? sourceNPC.whoAmI : -1);

            if (NeedsSync(DamagingSync, projectile))
            {
                bits.WriteBit(projectile.friendly);
                bits.WriteBit(projectile.hostile);
                bits.WriteBit(projectile.DamageType == DamageClass.Default);
            }
        }

        public override void ReceiveExtraAI(Projectile projectile, BitReader bits, BinaryReader reader)
        {
            if (NeedsSync(SourceNPCSync, projectile))
                sourceNPC = FargoSoulsUtil.NPCExists(reader.ReadInt32());

            if (NeedsSync(DamagingSync, projectile))
            {
                projectile.friendly = bits.ReadBit();
                projectile.hostile = bits.ReadBit();
                if (bits.ReadBit())
                    projectile.DamageType = DamageClass.Default;
            }
        }

        public override bool PreAI(Projectile projectile)
        {
            if (sourceNPC is NPC && !sourceNPC.active)
                sourceNPC = null;

            return base.PreAI(projectile);
        }
    }
}
