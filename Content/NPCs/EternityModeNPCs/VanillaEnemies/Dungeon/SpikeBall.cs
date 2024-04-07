using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Projectiles.Masomode;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Dungeon
{
    public class SpikeBall : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.SpikeBall);

        public int Counter;
        public bool OutsideDungeon;

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            if (Framing.GetTileSafely(npc.Center).WallType == WallID.LihzahrdBrickUnsafe)
            {
                npc.damage *= 3;
                npc.defDamage *= 3;
            }

            int p = npc.FindClosestPlayer();
            if (p != -1 && !Main.player[p].ZoneDungeon)
                OutsideDungeon = true;
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (OutsideDungeon)
            {
                if (++Counter > 1800)
                {
                    if (FargoSoulsUtil.HostCheck)
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<FusedExplosion>(), npc.damage, 0f, Main.myPlayer);
                    npc.life = 0;
                    npc.HitEffect();
                    npc.SimpleStrikeNPC(int.MaxValue, 0, false, 0, null, false, 0, true);
                }
                else if (Counter > 1800 - 300)
                {
                    int dust = Dust.NewDust(npc.Center, 0, 0, DustID.Torch, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 0, default, 2f);
                    Main.dust[dust].velocity *= 2f;
                    if (Main.rand.NextBool(4))
                    {
                        Main.dust[dust].scale += 0.5f;
                        Main.dust[dust].noGravity = true;
                    }
                }
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(BuffID.BrokenArmor, 600);
            if (OutsideDungeon)
                target.AddBuff(ModContent.BuffType<DefenselessBuff>(), 600);
        }
    }
}
