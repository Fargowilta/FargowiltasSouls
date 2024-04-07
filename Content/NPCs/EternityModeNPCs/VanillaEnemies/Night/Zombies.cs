using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Night
{
    public class Zombies : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.Zombie,
            NPCID.ArmedZombie,
            NPCID.ArmedZombieCenx,
            NPCID.ArmedZombiePincussion,
            NPCID.ArmedZombieSlimed,
            NPCID.ArmedZombieSwamp,
            NPCID.ArmedZombieTwiggy,
            NPCID.BaldZombie,
            NPCID.FemaleZombie,
            NPCID.PincushionZombie,
            NPCID.SlimedZombie,
            NPCID.TwiggyZombie,
            NPCID.ZombiePixie,
            NPCID.ZombieRaincoat,
            NPCID.ZombieSuperman,
            NPCID.ZombieSweater,
            NPCID.ZombieXmas,
            NPCID.SwampZombie,
            NPCID.SmallSwampZombie,
            NPCID.BigSwampZombie,
            NPCID.ZombieDoctor,
            NPCID.ZombieEskimo,
            NPCID.ArmedZombieEskimo,
            NPCID.ZombieMushroom,
            NPCID.ZombieMushroomHat,
            NPCID.ZombieElf,
            NPCID.ZombieElfBeard,
            NPCID.ZombieElfGirl,
            NPCID.SmallSlimedZombie,
            NPCID.BigSlimedZombie,
            NPCID.ZombieMerman
        );

        private static void transformZombie(NPC npc, int armedId = -1)
        {
            if (Main.LocalPlayer.ZoneSnow && Main.rand.NextBool())
            {
                npc.Transform(NPCID.ZombieEskimo);
            }

            if (Main.rand.NextBool(8) && npc.FargoSouls().CanHordeSplit)
                EModeGlobalNPC.Horde(npc, 6);
            if (armedId != -1 && Main.rand.NextBool(5))
                npc.Transform(armedId);
        }

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            switch (npc.type)
            {
                case NPCID.Zombie: transformZombie(npc, NPCID.ArmedZombie); break;
                case NPCID.ZombieEskimo: transformZombie(npc, NPCID.ArmedZombieEskimo); break;
                case NPCID.PincushionZombie: transformZombie(npc, NPCID.ArmedZombiePincussion); break;
                case NPCID.FemaleZombie: transformZombie(npc, NPCID.ArmedZombieCenx); break;
                case NPCID.SlimedZombie: transformZombie(npc, NPCID.ArmedZombieSlimed); break;
                case NPCID.TwiggyZombie: transformZombie(npc, NPCID.ArmedZombieTwiggy); break;
                case NPCID.SwampZombie: transformZombie(npc, NPCID.ArmedZombieSwamp); break;

                default: break;
            }
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (npc.type == NPCID.ZombieRaincoat)
            {
                if (npc.wet)
                {
                    //slime ai
                    npc.aiStyle = 1;
                }
                else
                {
                    //zombie ai
                    npc.aiStyle = 3;
                }
            }

            if (npc.ai[2] >= 45f && npc.ai[3] == 0f && FargoSoulsUtil.HostCheck)
            {
                int tileX = (int)(npc.position.X + npc.width / 2 + 15 * npc.direction) / 16;
                int tileY = (int)(npc.position.Y + npc.height - 15) / 16 - 1;
                Tile tile = Framing.GetTileSafely(tileX, tileY);
                if (tile.TileType == TileID.ClosedDoor || tile.TileType == TileID.TallGateClosed)
                {
                    //WorldGen.KillTile(tileX, tileY);
                    WorldGen.OpenDoor(tileX, tileY, npc.direction);
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, tileX, tileY);
                }
            }
        }



        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(ModContent.BuffType<RottingBuff>(), 300);

            switch (npc.type)
            {
                case NPCID.ZombieMushroom:
                case NPCID.ZombieMushroomHat:
                    target.AddBuff(ModContent.BuffType<InfestedBuff>(), 300);
                    break;

                case NPCID.ZombieEskimo:
                case NPCID.ArmedZombieEskimo:
                    target.AddBuff(ModContent.BuffType<HypothermiaBuff>(), 300);
                    break;

                case NPCID.ZombieElf:
                case NPCID.ZombieElfBeard:
                case NPCID.ZombieElfGirl:
                    target.AddBuff(ModContent.BuffType<RottingBuff>(), 900);
                    break;

                default: break;
            }
        }

        public override void OnHitNPC(NPC npc, NPC target, NPC.HitInfo hit)
        {
            base.OnHitNPC(npc, target, hit);

            if (target.townNPC && (hit.InstantKill || target.life < hit.Damage))
            {
                target.Transform(npc.type);
            }
        }

        public override void OnKill(NPC npc)
        {
            base.OnKill(npc);

            switch (npc.type)
            {
                case NPCID.SmallSlimedZombie:
                case NPCID.SlimedZombie:
                case NPCID.BigSlimedZombie:
                case NPCID.ArmedZombieSlimed:
                    if (Main.rand.NextBool() && FargoSoulsUtil.HostCheck)
                        FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromAI(), npc.Center, NPCID.BlueSlime);
                    break;

                default:
                    break;
            }
        }
    }
}
