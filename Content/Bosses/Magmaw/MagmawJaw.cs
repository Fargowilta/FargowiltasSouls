
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Magmaw
{
    public class MagmawJaw : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.width = 96;
            Projectile.height = 58;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale = 1f;
            Projectile.light = 1;
            Projectile.FargoSouls().DeletionImmuneRank = 10;

            Projectile.hide = true;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {

        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
            writer.Write(Projectile.localAI[2]);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
            Projectile.localAI[2] = reader.ReadSingle();
        }
        public ref float ParentID => ref Projectile.ai[0];
        public override void AI()
        {
            int parentID = (int)ParentID;
            if (!parentID.IsWithinBounds(Main.maxNPCs))
            {
                Projectile.Kill();
                return;
            }
            NPC parent = Main.npc[parentID];
            if (!parent.TypeAlive<Magmaw>())
            {
                Projectile.Kill();
                return;
            }

            Projectile.timeLeft = 60; //don't despawn

            Vector2 desiredPos = parent.As<Magmaw>().JawCenter;
            Projectile.velocity = (desiredPos - Projectile.Center) * 0.95f;

            Projectile.damage = parent.damage;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            if (Projectile.hide)
            {
                behindNPCs.Add(index);
            }

        }

        #region Help Methods
        public NPC GetParent()
        {
            int parentID = (int)ParentID;
            if (!parentID.IsWithinBounds(Main.maxNPCs))
            {
                Projectile.Kill();
                return null;
            }
            NPC parent = Main.npc[parentID];
            if (!parent.TypeAlive<Magmaw>())
            {
                Projectile.Kill();
                return null;
            }
            return parent;
        }
        #endregion
    }
}
