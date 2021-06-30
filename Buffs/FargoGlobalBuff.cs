using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs
{
    internal class FargoGlobalBuff : GlobalBuff
    {
        public override void ModifyBuffTip(int type, ref string tip, ref int rare)
        {
            if (FargoSoulsWorld.MasochistMode)
            {
                if (type == BuffID.ShadowDodge)
                    tip += "\nEternity Mode: Dodging will reduce your damage output";
                else if (type == BuffID.IceBarrier)
                    tip += "\nEternity Mode: Effectiveness reduced to 15%";
            }
        }

        public override void Update(int type, Player player, ref int buffIndex)
        {
            switch(type)
            {
                case BuffID.Slimed:
                    Main.buffNoTimeDisplay[type] = false;
                    if (FargoSoulsWorld.MasochistMode)
                        player.GetModPlayer<FargoPlayer>().Slimed = true;
                    break;

                case BuffID.OnFire:
                    if (FargoSoulsWorld.MasochistMode && Main.raining && player.position.Y < Main.worldSurface
                        && Framing.GetTileSafely(player.Center).wall == WallID.None && player.buffTime[buffIndex] > 1)
                        player.buffTime[buffIndex] -= 1;
                    break;

                default:
                    break;
            }

            base.Update(type, player, ref buffIndex);
        }

        public override void Update(int type, NPC npc, ref int buffIndex)
        {
            switch(type)
            {
                case BuffID.Chilled:
                    npc.GetGlobalNPC<NPCs.FargoSoulsGlobalNPC>().Chilled = true;
                    break;

                case BuffID.Darkness:
                    npc.color = Color.Gray;

                    if (npc.buffTime[buffIndex] % 30 == 0)
                    {
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            NPC target = Main.npc[i];
                            if (target.active && !target.friendly && Vector2.Distance(npc.Center, target.Center) < 250)
                            {
                                Vector2 velocity = Vector2.Normalize(target.Center - npc.Center) * 5;
                                Projectile.NewProjectile(npc.Center, velocity, ProjectileID.ShadowFlame, 40 + npc.damage / 4, 0, Main.myPlayer);
                                if (Main.rand.Next(3) == 0)
                                    break;
                            }
                        }
                    }
                    break;

                case BuffID.Electrified:
                    npc.GetGlobalNPC<NPCs.FargoSoulsGlobalNPC>().Electrified = true;
                    break;

                case BuffID.Suffocation:
                    npc.GetGlobalNPC<NPCs.FargoSoulsGlobalNPC>().Suffocation = true;
                    break;

                case BuffID.OnFire:
                    if (FargoSoulsWorld.MasochistMode && Main.raining && npc.position.Y < Main.worldSurface
                        && Framing.GetTileSafely(npc.Center).wall == WallID.None && npc.buffTime[buffIndex] > 1)
                        npc.buffTime[buffIndex] -= 1;
                    break;

                default:
                    break;
            }
        }
    }
}