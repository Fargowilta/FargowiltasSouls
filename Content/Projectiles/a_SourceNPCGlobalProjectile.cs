using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.Content.Projectiles
{
    //weird name so it goes before the other globalprojectiles
    //TODO: make this a proper inherited class and a modproj equivalent, axe the dictionary
    public class A_SourceNPCGlobalProjectile : GlobalProjectile
    {
        internal static Dictionary<int, bool> SourceNPCSync = new();
        internal static Dictionary<int, bool> DamagingSync = new();

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

        public static bool NeedsSync(Dictionary<int, bool> dict, int projectileType)
            => dict.TryGetValue(projectileType, out bool value) && value;

        public override void SendExtraAI(Projectile projectile, BitWriter bits, BinaryWriter writer)
        {
            if (NeedsSync(SourceNPCSync, projectile.type))
                writer.Write7BitEncodedInt(sourceNPC is not null ? sourceNPC.whoAmI : Main.maxNPCs);

            if (NeedsSync(DamagingSync, projectile.type))
            {
                bits.WriteBit(projectile.friendly);
                bits.WriteBit(projectile.hostile);
                bits.WriteBit(projectile.CountsAsClass(DamageClass.Default));
            }
        }

        public override void ReceiveExtraAI(Projectile projectile, BitReader bits, BinaryReader reader)
        {
            if (NeedsSync(SourceNPCSync, projectile.type))
                sourceNPC = FargoSoulsUtil.NPCExists(reader.Read7BitEncodedInt());

            if (NeedsSync(DamagingSync, projectile.type))
            {
                projectile.friendly = bits.ReadBit();
                projectile.hostile = bits.ReadBit();
                if (bits.ReadBit())
                    projectile.DamageType = DamageClass.Default;
            }
        }

        public override bool PreAI(Projectile projectile)
        {
            if (sourceNPC is not null && !sourceNPC.active)
                sourceNPC = null;

            return base.PreAI(projectile);
        }
    }
}
