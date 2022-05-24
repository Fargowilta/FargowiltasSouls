using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.MutantBoss
{
    public class BossRush : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_454";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mutant Seal");
            base.SetStaticDefaults();
            Main.projFrames[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 46;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.hide = true;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().TimeFreezeImmune = true;
        }

        public override void AI()
        {
            NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[0], ModContent.NPCType<NPCs.MutantBoss.MutantBoss>());
            if (npc == null)
            {
                Projectile.Kill();
                return;
            }

            Projectile.Center = npc.Center;
            Projectile.timeLeft = 2;

            if (--Projectile.ai[1] < 0)
            {
                Projectile.ai[1] = 180;
                Projectile.netUpdate = true;
                switch ((int)Projectile.localAI[0]++)
                {
                    case 0:
                        NPC.SpawnOnPlayer(npc.target, NPCID.EyeofCthulhu);
                        if (Main.dayTime)
                        {
                            Main.dayTime = false;
                            Main.time = 0;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.WorldData); //sync world
                        }
                        break;

                    case 1:
                        NPC.SpawnOnPlayer(npc.target, NPCID.EaterofWorldsHead);
                        NPC.SpawnOnPlayer(npc.target, NPCID.BrainofCthulhu);
                        break;

                    case 2:
                        NPC.SpawnOnPlayer(npc.target, NPCID.QueenBee);
                        break;

                    case 3:
                        ManualSpawn(npc, NPCID.SkeletronHead);
                        if (Main.dayTime)
                        {
                            Main.dayTime = false;
                            Main.time = 0;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.WorldData); //sync world
                        }
                        break;

                    case 4:
                        NPC.SpawnOnPlayer(npc.target, NPCID.Retinazer);
                        NPC.SpawnOnPlayer(npc.target, NPCID.Spazmatism);
                        if (Main.dayTime)
                        {
                            Main.dayTime = false;
                            Main.time = 0;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.WorldData); //sync world
                        }
                        break;

                    case 5:
                        ManualSpawn(npc, NPCID.SkeletronPrime);
                        if (Main.dayTime)
                        {
                            Main.dayTime = false;
                            Main.time = 0;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.WorldData); //sync world
                        }
                        break;

                    case 6:
                        NPC.SpawnOnPlayer(npc.target, NPCID.Plantera);
                        break;

                    case 7:
                        ManualSpawn(npc, NPCID.Golem);
                        break;

                    case 8:
                        ManualSpawn(npc, NPCID.DD2Betsy);
                        break;

                    case 9:
                        ManualSpawn(npc, NPCID.DukeFishron);
                        break;

                    case 10:
                        ManualSpawn(npc, NPCID.MoonLordCore);
                        break;

                    default:
                        if (!Main.dayTime)
                        {
                            Main.dayTime = true;
                            Main.time = 27000;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.WorldData); //sync world
                        }
                        Projectile.Kill();
                        break;
                }
            }
        }

        private void ManualSpawn(NPC npc, int type)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int n = FargoSoulsUtil.NewNPCEasy(Terraria.Entity.InheritSource(Projectile), npc.Center, type);
                if (n != Main.maxNPCs)
                    FargoSoulsUtil.PrintText(Main.npc[n].FullName + " has awoken!", 175, 75, 255);
            }
        }
    }
}