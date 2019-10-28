using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.World.Generation;

namespace FargowiltasSouls
{
	public class MMWorld : ModWorld
	{
		public static int MMPoints = 0;
        public static int MMWaveThreshhold = 30;
        public static int MMWave = 0;
		public static bool MMArmy;

        public static bool downedMage = false;
        public static bool downedSummoner = false;
        public static bool downedDutchman = false;
        public static bool downedOgre = false;
        public static bool downedWood = false;
        public static bool downedPumpking = false;
        public static bool downedEverscream = false;
        public static bool downedSantaNK1 = false;
        public static bool downedElsa = false;
        public static bool downedBetsy = false;
        public static bool downedAbom = false;

        public override void Initialize()
        {
            downedMage = false;
            downedSummoner = false;
            downedDutchman = false;
            downedOgre = false;
            downedWood = false;
            downedPumpking = false;
            downedEverscream = false;
            downedSantaNK1 = false;
            downedElsa = false;
            downedBetsy = false;
            downedAbom = false;
            MMArmy = false;
            MMPoints = 0;
            MMWave = 0;
        }

		public override void PostUpdate()
		{
            if (MMWave >= 21)
            {
                FargoSoulsWorld.downedMM = true;
                MMPoints = 0;
                MMArmy = false;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData); // Immediately inform clients of new world state.
                }
            }
        }

        public void WaveCheck()
        {
            if (MMWave == 0 && MMPoints >= MMWaveThreshhold)
            {
                MMWave = 1; MMPoints = 0; MMWaveThreshhold = 30;
            }
            else if (MMWave == 1 && MMPoints >= MMWaveThreshhold)
            {
                MMWave = 2; MMPoints = 0; MMWaveThreshhold = 35;
            }
            else if (MMWave == 2 && MMPoints >= MMWaveThreshhold)
            {
                MMWave = 3; MMPoints = 0; MMWaveThreshhold = 40;
            }
            else if (MMWave == 3 && MMPoints >= MMWaveThreshhold)
            {
                MMWave = 4; MMPoints = 0; StartSandstorm(); MMWaveThreshhold = 45;
            }
            else if (MMWave == 4 && MMPoints >= MMWaveThreshhold)
            {
                MMWave = 5; MMPoints = 0; MMWaveThreshhold = 50;
            }
            else if (MMWave == 5 && MMPoints >= MMWaveThreshhold)
            {
                MMWave = 6; MMPoints = 0; MMWaveThreshhold = 55;
                Sandstorm.TimeLeft = 10;
            }
            else if (MMWave == 6 && MMPoints >= MMWaveThreshhold)
            {
                MMWave = 7; MMPoints = 0; MMWaveThreshhold = 60;
            }
            else if (MMWave == 7 && MMPoints >= MMWaveThreshhold)
            {
                MMWave = 8; MMPoints = 0; MMWaveThreshhold = 65;
            }
            else if (MMWave == 8 && MMPoints >= MMWaveThreshhold)
            {
                MMWave = 9; MMPoints = 0; MMWaveThreshhold = 70;
            }
            else if (MMWave == 9 && MMPoints >= MMWaveThreshhold)
            {
                MMWave = 10; MMPoints = 0; MMWaveThreshhold = 75;
            }
            else if (MMWave == 10 && MMPoints >= MMWaveThreshhold)
            {
                MMWave = 11; MMPoints = 0; MMWaveThreshhold = 80;
            }
            else if (MMWave == 11 && MMPoints >= MMWaveThreshhold)
            {
                MMWave = 12; MMPoints = 0; MMWaveThreshhold = 85;
            }
            else if (MMWave == 12 && MMPoints >= MMWaveThreshhold)
            {
                MMWave = 13; MMPoints = 0; MMWaveThreshhold = 90;
            }
            else if (MMWave == 13 && MMPoints >= MMWaveThreshhold)
            {
                MMWave = 14; MMPoints = 0; MMWaveThreshhold = 95;
            }
            else if (MMWave == 14 && MMPoints >= MMWaveThreshhold)
            {
                MMWave = 15; MMPoints = 0; MMWaveThreshhold = 100;
            }
            else if (MMWave == 15 && MMPoints >= MMWaveThreshhold)
            {
                MMWave = 16; MMPoints = 0; MMWaveThreshhold = 105;
            }
            else if (MMWave == 16 && MMPoints >= MMWaveThreshhold)
            {
                MMWave = 17; MMPoints = 0; MMWaveThreshhold = 120;
            }
            else if (MMWave == 17 && MMPoints >= MMWaveThreshhold)
            {
                MMWave = 18; MMPoints = 0; MMWaveThreshhold = 125;
            }
            else if (MMWave == 18 && MMPoints >= MMWaveThreshhold)
            {
                MMWave = 19; MMPoints = 0; MMWaveThreshhold = 130;
            }
            else if (MMWave == 19 && MMPoints >= MMWaveThreshhold)
            {
                MMWave = 12; MMPoints = 0; MMWaveThreshhold = 1;
            }
            else if (MMWave == 20 && MMPoints >= MMWaveThreshhold)
            {
                Main.NewText("The defeat of the Abominationn causes the armies of terraria to flee in terror", 250, 170, 50);

                MMWave = 0; MMPoints = 0; MMArmy = false;
            }
        }

        private static void StartSandstorm()
        {
            Sandstorm.Happening = true;
            Sandstorm.TimeLeft = (int)(3600f * (8f + Main.rand.NextFloat() * 16f));
            ChangeSeverityIntentions();
        }

        private static void ChangeSeverityIntentions()
        {
            if (Sandstorm.Happening)
            {
                Sandstorm.IntendedSeverity = 0.4f + Main.rand.NextFloat();
            }
            else if (Main.rand.Next(3) == 0)
            {
                Sandstorm.IntendedSeverity = 0f;
            }
            else
            {
                Sandstorm.IntendedSeverity = Main.rand.NextFloat() * 0.3f;
            }
            if (Main.netMode != 1)
            {
                NetMessage.SendData(7, -1, -1, null, 0, 0f, 0f, 0f, 0, 0, 0);
            }
        }
    }

    public class MMNPC : GlobalNPC
    {

        public override void SetDefaults(NPC npc)
        {
            if (MMWorld.MMArmy && !npc.boss)
            {
                npc.life *= 10;
                npc.defense *= 5;
                npc.damage *= 4;
            }
        }

        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            bool monsterMadhouse = MMWorld.MMArmy;

            if (monsterMadhouse)
            {
                spawnRate = 20;
            }
        }

        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            bool monsterMadhouse = MMWorld.MMArmy;

            if (monsterMadhouse)
            {
                pool.Clear();
                switch (MMWorld.MMWave)
                {
                    case 0: //Blood Moon
                        pool.Add(NPCID.BloodZombie, 35f);
                        pool.Add(NPCID.Drippler, 35f);
                        pool.Add(NPCID.CorruptBunny, 15f);
                        pool.Add(NPCID.CrimsonBunny, 15f);
                        break;

                    case 1: //Goblin Army
                        pool.Add(NPCID.GoblinPeon, 30f);
                        pool.Add(NPCID.GoblinThief, 30f);
                        pool.Add(NPCID.GoblinArcher, 30f);
                        pool.Add(NPCID.GoblinArcher, 25f);
                        pool.Add(NPCID.GoblinSorcerer, 25f);
                        break;

                    case 2: //Sandstorm
                        pool.Add(NPCID.Tumbleweed, 25f);
                        pool.Add(NPCID.FlyingAntlion, 30f);
                        pool.Add(NPCID.WalkingAntlion, 30f);
                        break;

                    case 3: //OOA 1
                        pool.Add(552, 30f);
                        pool.Add(555, 30f);
                        pool.Add(558, 30f);
                        pool.Add(561, 30f);
                        pool.Add(564, 5f);
                        break;

                    case 4: //Frost Leigon
                        pool.Add(NPCID.MisterStabby, 30f);
                        pool.Add(NPCID.SnowmanGangsta, 30f);
                        pool.Add(NPCID.SnowBalla, 30f);
                        break;

                    case 5: //Hardmode Bloodmoon
                        pool.Add(NPCID.BloodZombie, 35f);
                        pool.Add(NPCID.Drippler, 35f);
                        pool.Add(NPCID.CorruptBunny, 15f);
                        pool.Add(NPCID.CrimsonBunny, 15f);
                        pool.Add(NPCID.TheGroom, 10f);
                        pool.Add(NPCID.TheBride, 10f);
                        pool.Add(NPCID.Clown, 10f);
                        break;


                    case 6: //Hardmode Gobs
                        pool.Add(NPCID.GoblinPeon, 30f);
                        pool.Add(NPCID.GoblinThief, 30f);
                        pool.Add(NPCID.GoblinArcher, 30f);
                        pool.Add(NPCID.GoblinArcher, 25f);
                        pool.Add(NPCID.GoblinSorcerer, 25f);
                        pool.Add(NPCID.GoblinSummoner, 5f);
                        break;

                    case 7: //Hardmode Sandstorm
                        pool.Add(NPCID.Tumbleweed, 30f);
                        pool.Add(NPCID.FlyingAntlion, 25f);
                        pool.Add(NPCID.WalkingAntlion, 25f);
                        pool.Add(NPCID.SandShark, 25f);
                        pool.Add(NPCID.DuneSplicerHead, 25f);
                        pool.Add(NPCID.SandElemental, 10f);
                        break;

                    case 8: //Pirates
                        pool.Add(NPCID.PirateDeckhand, 30f);
                        pool.Add(NPCID.PirateCorsair, 25f);
                        pool.Add(NPCID.PirateDeadeye, 25f);
                        pool.Add(NPCID.PirateCrossbower, 25f);
                        pool.Add(NPCID.Parrot, 25f);
                        pool.Add(NPCID.PirateCaptain, 10f);
                        pool.Add(NPCID.PirateShip, 5f);
                        break;

                    case 9: //OOA 2
                        pool.Add(553, 30f);
                        pool.Add(556, 30f);
                        pool.Add(559, 30f);
                        pool.Add(562, 30f);
                        pool.Add(568, 30f);
                        pool.Add(570, 30f);
                        pool.Add(572, 30f);
                        pool.Add(574, 30f);
                        pool.Add(576, 5f);
                        break;

                    case 10: //Eclipse
                        pool.Add(NPCID.SwampThing, 25f);
                        pool.Add(NPCID.Frankenstein, 25f);
                        pool.Add(NPCID.Eyezor, 25f);
                        pool.Add(NPCID.Vampire, 25f);
                        pool.Add(NPCID.ThePossessed, 25f);
                        pool.Add(NPCID.CreatureFromTheDeep, 25f);
                        pool.Add(NPCID.Fritz, 25f);
                        pool.Add(NPCID.Vampire, 25f);
                        pool.Add(NPCID.Reaper, 25f);
                        pool.Add(NPCID.Mothron, 5f);
                        break;

                    case 11: //Eclipse 2
                        pool.Add(NPCID.SwampThing, 25f);
                        pool.Add(NPCID.Frankenstein, 25f);
                        pool.Add(NPCID.Eyezor, 25f);
                        pool.Add(NPCID.Vampire, 25f);
                        pool.Add(NPCID.ThePossessed, 25f);
                        pool.Add(NPCID.CreatureFromTheDeep, 25f);
                        pool.Add(NPCID.Fritz, 25f);
                        pool.Add(NPCID.Vampire, 25f);
                        pool.Add(NPCID.Reaper, 25f);
                        pool.Add(NPCID.Butcher, 15f);
                        pool.Add(NPCID.DeadlySphere, 15f);
                        pool.Add(NPCID.DrManFly, 15f);
                        pool.Add(NPCID.Nailhead, 15f);
                        pool.Add(NPCID.Psycho, 15f);
                        pool.Add(NPCID.Mothron, 5f);
                        break;

                    case 12: //Pumpkin Moon 1
                        pool.Add(NPCID.Scarecrow1, 30f);
                        pool.Add(NPCID.Splinterling, 25f);
                        pool.Add(NPCID.Hellhound, 25f);
                        pool.Add(NPCID.MourningWood, 8f);
                        break;

                    case 13: //Pumpkin Moon 2
                        pool.Add(NPCID.Scarecrow1, 30f);
                        pool.Add(NPCID.Splinterling, 25f);
                        pool.Add(NPCID.Hellhound, 25f);
                        pool.Add(NPCID.Poltergeist, 20f);
                        pool.Add(NPCID.HeadlessHorseman, 10f);
                        pool.Add(NPCID.Poltergeist, 20f);
                        pool.Add(NPCID.MourningWood, 8f);
                        pool.Add(NPCID.Pumpking, 5f);
                        break;

                    case 14: //Frost Moon 1
                        pool.Add(NPCID.GingerbreadMan, 30f);
                        pool.Add(NPCID.ZombieElf, 25f);
                        pool.Add(NPCID.ZombieElfGirl, 25f);
                        pool.Add(NPCID.ElfArcher, 25f);
                        pool.Add(NPCID.Everscream, 10f);
                        break;

                    case 15: //Frost Moon 2
                        pool.Add(NPCID.GingerbreadMan, 30f);
                        pool.Add(NPCID.ZombieElf, 25f);
                        pool.Add(NPCID.ZombieElfGirl, 25f);
                        pool.Add(NPCID.ElfArcher, 25f);
                        pool.Add(NPCID.Nutcracker, 20f);
                        pool.Add(NPCID.Yeti, 20f);
                        pool.Add(NPCID.Everscream, 10f);
                        pool.Add(NPCID.SantaNK1, 8f);
                        break;

                    case 16: //Frost Moon 2
                        pool.Add(NPCID.GingerbreadMan, 30f);
                        pool.Add(NPCID.ZombieElf, 25f);
                        pool.Add(NPCID.ZombieElfGirl, 25f);
                        pool.Add(NPCID.ElfArcher, 25f);
                        pool.Add(NPCID.Nutcracker, 20f);
                        pool.Add(NPCID.Yeti, 20f);
                        pool.Add(NPCID.ElfCopter, 15f);
                        pool.Add(NPCID.Krampus, 15f);
                        pool.Add(NPCID.Everscream, 10f);
                        pool.Add(NPCID.SantaNK1, 8f);
                        pool.Add(NPCID.IceQueen, 5f);
                        break;

                    case 17: //Martians
                        pool.Add(NPCID.GrayGrunt, 30f);
                        pool.Add(NPCID.BrainScrambler, 30f);
                        pool.Add(NPCID.RayGunner, 25f);
                        pool.Add(NPCID.MartianOfficer, 25f);
                        pool.Add(NPCID.MartianEngineer, 25f);
                        pool.Add(NPCID.GigaZapper, 20f);
                        pool.Add(NPCID.MartianDrone, 20f);
                        pool.Add(NPCID.MartianWalker, 15f);
                        pool.Add(NPCID.Scutlix, 15f);
                        pool.Add(NPCID.MartianSaucer, 8f);
                        break;

                    case 18: //OOA 3
                        pool.Add(554, 30f);
                        pool.Add(557, 30f);
                        pool.Add(560, 30f);
                        pool.Add(563, 30f);
                        pool.Add(569, 30f);
                        pool.Add(572, 30f);
                        pool.Add(573, 30f);
                        pool.Add(574, 30f);
                        pool.Add(578, 30f);
                        pool.Add(564, 5f);
                        pool.Add(576, 5f);
                        pool.Add(551, 2f);
                        break;

                    case 19: //Best Of
                        pool.Add(NPCID.Parrot, 10f);
                        pool.Add(NPCID.PirateCaptain, 10f);
                        pool.Add(NPCID.Reaper, 20f);
                        pool.Add(NPCID.Butcher, 10f);
                        pool.Add(NPCID.DeadlySphere, 10f);
                        pool.Add(NPCID.DrManFly, 10f);
                        pool.Add(NPCID.Nailhead, 10f);
                        pool.Add(NPCID.Psycho, 10f);
                        pool.Add(NPCID.ElfCopter, 10f);
                        pool.Add(NPCID.Krampus, 10f);
                        pool.Add(NPCID.HeadlessHorseman, 10f);
                        pool.Add(NPCID.Poltergeist, 10f);
                        pool.Add(NPCID.MartianDrone, 20f);
                        pool.Add(NPCID.MartianWalker, 15f);
                        pool.Add(NPCID.GoblinSummoner, 5f);
                        pool.Add(NPCID.MourningWood, 2f);
                        pool.Add(NPCID.Pumpking, 2f);
                        pool.Add(NPCID.MartianSaucer, 2f);
                        pool.Add(NPCID.PirateShip, 2f);
                        pool.Add(NPCID.SandElemental, 2f);
                        pool.Add(NPCID.IceQueen, 2f);
                        pool.Add(NPCID.Pumpking, 2f);
                        pool.Add(NPCID.Everscream, 2f);
                        pool.Add(NPCID.SantaNK1, 2f);
                        pool.Add(NPCID.Mothron, 2f);
                        break;
                    case 20: //Abom

                        break;
                }
            }
        }
    }

    public class MMPlayer : ModPlayer
    {
        public override void PostUpdate()
        {
            if (MMWorld.MMWave == 5 || MMWorld.MMWave == 6)
            {
                player.ZoneDesert = true;
            }
            else
            {
                player.ZoneDesert = false;
            }
            if (MMWorld.MMPoints >= 1000)
            {
                FargoSoulsWorld.downedMM = true;
                Main.NewText("The armies of Terraria retreat! Victory!", 250, 170, 50);
                MMWorld.MMPoints = 0;
                MMWorld.MMArmy = false;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
        }
    }
}
