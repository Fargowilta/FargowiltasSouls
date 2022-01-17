using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using FargowiltasSouls.NPCs;
using FargowiltasSouls.Projectiles;
using FargowiltasSouls.Buffs.Souls;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.Projectiles.Souls;
using FargowiltasSouls.Projectiles.BossWeapons;
using FargowiltasSouls.Projectiles.Masomode;
using FargowiltasSouls.Projectiles.Minions;
using FargowiltasSouls.Items.Weapons.SwarmDrops;
using FargowiltasSouls.NPCs.MutantBoss;

using FargowiltasSouls.Items.Summons;
using FargowiltasSouls.NPCs.EternityMode;
using Microsoft.Xna.Framework.Graphics;
using FargowiltasSouls.Items.Accessories.Enchantments;
using Terraria.Graphics.Shaders;
using FargowiltasSouls.Toggler;
using FargowiltasSouls.Items.Accessories.Masomode;
using FargowiltasSouls.Items.Accessories.Souls;
using Terraria.Localization;

namespace FargowiltasSouls
{
    public partial class FargoPlayer : ModPlayer
    {
        public ToggleBackend Toggler = new ToggleBackend();
        public Dictionary<string, bool> TogglesToSync = new Dictionary<string, bool>();

        //for convenience
        public bool IsStandingStill;
        public float AttackSpeed;
        public float wingTimeModifier;

        public bool FreeEaterSummon = true;
        public int Screenshake;

        public bool Wood;
        public bool QueenStinger;
        public int QueenStingerCD;
        public bool EridanusEmpower;
        public int EridanusTimer;
        public bool GaiaSet;
        public bool GaiaOffense;
        public bool StyxSet;
        public int StyxMeter;

        //minions
        public bool BrainMinion;
        public bool EaterMinion;
        public bool BigBrainMinion;
        public bool DukeFishron;

        //mount
        public bool SquirrelMount;

        //pet
        public bool ChibiDevi;
        public bool MutantSpawn;
        public bool BabyAbom;

        #region enchantments
        public bool PetsActive = true;
        public bool ShadowEnchant;
        public bool CrimsonEnchant;
        public bool CrimsonRegen;
        public int CrimsonTotalToRegen = 0;
        public int CrimsonRegenSoFar = 0;
        public int CrimsonRegenTimer = 0;
        public bool SpectreEnchant;
        public bool BeeEnchant;
        private int beeCD = 0;
        public bool SpiderEnchant;
        public int SummonCrit;
        public bool StardustEnchant;
        public bool FreezeTime = false;
        public int freezeLength = 540; //300;
        public bool MythrilEnchant;
        public bool FossilEnchant;
        public bool JungleEnchant;
        public int JungleCD;
        public bool ElementEnchant;
        public bool ShroomEnchant;
        public bool CobaltEnchant;
        public int CobaltCD = 0;
        public bool SpookyEnchant;
        public bool HallowEnchant;
        public bool ChloroEnchant;
        public bool VortexEnchant;
        public bool VortexStealth = false;
        public bool AdamantiteEnchant;
        public int AdamantiteCD = 0;
        public bool FrostEnchant;
        public int IcicleCount = 0;
        private int icicleCD = 0;
        public bool PalladEnchant;
        public int PalladCounter;
        //private int palladiumCD = 0;
        public bool OriEnchant;
        public bool MeteorEnchant;
        private int meteorTimer = 150;
        private int meteorCD = 0;
        public bool meteorShower;
        public bool MoltenEnchant;
        public bool CopperEnchant;
        private int copperCD = 0;
        public bool NinjaEnchant;
        private Projectile ninjaSmokeBomb = null;
        public bool FirstStrike;
        public bool NearSmoke;
        private bool hasSmokeBomb;
        private int smokeBombCD;
        private int smokeBombSlot;
        public bool IronEnchant;
        public bool IronGuard;
        public int IronDebuffImmuneTime;
        public bool TurtleEnchant;
        public int TurtleCounter = 0;
        public int TurtleShellHP = 25;
        private int turtleRecoverCD = 240;
        public bool ShellHide;
        public bool LeadEnchant;
        public bool GladEnchant;
        private int gladCount = 0;
        public bool GoldEnchant;
        public bool GoldShell;
        private int goldHP;
        public bool CactusEnchant;
        public bool ForbiddenEnchant;
        public bool MinerEnchant;
        public bool PumpkinEnchant;
        private int pumpkinCD = 0;
        public bool SilverEnchant;
        public bool PlatinumEnchant;
        public bool NecroEnchant;
        public int NecroCD;
        public bool ObsidianEnchant;
        private int obsidianCD = 0;
        public bool LavaWet;
        public bool TinEnchant;
        public int TinCritMax = 0;
        private int tinCD = 0;
        public int TinCrit = 4;
        public bool TikiEnchant;
        public bool TikiMinion;
        public int actualMinions;
        public bool TikiSentry;
        public int actualSentries;
        public bool SolarEnchant;
        public bool ShinobiEnchant;
        public int dashCD;
        public bool ValhallaEnchant;
        public bool DarkEnchant;
        public bool DarkSpawn;
        public int DarkSpawnCD = 0;
        private int apprenticeCD = 0;
        public bool RedEnchant;
        public bool TungstenEnchant;
        private float tungstenPrevSizeSave = -1;
        public int TungstenCD = 0;

        public bool MahoganyEnchant;
        public bool BorealEnchant;
        public bool WoodEnchant;
        public int CritterAttackTimer = 0;
        public bool PalmEnchant;
        public bool ShadeEnchant;
        private int shadewoodCD = 0;
        public bool PearlEnchant;
        private int pearlCD = 0;

        public bool RainEnchant;

        public bool AncientCobaltEnchant;
        public bool AncientShadowEnchant;
        public bool SquireEnchant;
        public bool squireReduceIframes;
        public bool ApprenticeEnchant;
        public bool HuntressEnchant;
        private int huntressCD = 0;
        public bool MonkEnchant;
        public int MonkDashing = 0;
        private int monkTimer;
        public bool SnowEnchant;
        public bool SnowVisual;
        public bool WizardEnchant;

        public bool Solar;
        public bool Nebula;

        public bool CosmoForce;
        public bool EarthForce;
        public bool LifeForce;
        public bool NatureForce;
        public bool SpiritForce;
        public bool ShadowForce;
        public bool TerraForce;
        public bool WillForce;
        public bool WoodForce;

        

        #endregion

        //soul effects
        public bool MagicSoul;
        public bool ThrowSoul;
        public bool RangedSoul;
        public bool RangedEssence;
        public bool BuilderMode;
        public bool UniverseEffect;
        public bool NecroPet; //SoD
        public bool FishSoul1;
        public bool FishSoul2;
        public bool TerrariaSoul;
        public bool VoidSoul;
        public int HealTimer;
        public int HurtTimer;
        public bool Eternity;
        public float eternityDamage = 0;

        //maso items
        public bool SlimyShield;
        public bool SlimyShieldFalling;
        public bool AgitatingLens;
        public int AgitatingLensCD;
        public bool CorruptHeart;
        public int CorruptHeartCD;
        public bool GuttedHeart;
        public int GuttedHeartCD = 60; //should prevent spawning despite disabled toggle when loading into world
        public bool NecromanticBrew;
        public bool PureHeart;
        public bool PungentEyeballMinion;
        public bool CrystalSkullMinion;
        public bool FusedLens;
        public bool GroundStick;
        public bool Probes;
        public bool MagicalBulb;
        public bool SkullCharm;
        public bool PumpkingsCape;
        public bool LihzahrdTreasureBox;
        public int GroundPound;
        public bool BetsysHeart;
        public bool BetsyDashing;
        public int BetsyDashCD = 0;
        public bool MutantAntibodies;
        public bool GravityGlobeEX;
        public bool CelestialRune;
        public bool AdditionalAttacks;
        public int AdditionalAttacksTimer;
        public bool MoonChalice;
        public bool LunarCultist;
        public bool TrueEyes;
        public bool CyclonicFin;
        public int CyclonicFinCD;
        public bool MasochistSoul;
        public bool MasochistHeart;
        public bool CelestialSeal;
        public bool SandsofTime;
        public bool DragonFang;
        public bool SecurityWallet;
        public bool FrigidGemstone;
        public bool WretchedPouch;
        public int WretchedPouchCD;
        public int FrigidGemstoneCD;
        public bool NymphsPerfume;
        public bool NymphsPerfumeRespawn;
        public int NymphsPerfumeCD = 30;
        public bool SqueakyAcc;
        public bool RainbowSlime;
        public bool SkeletronArms;
        public bool SuperFlocko;
        public bool IceQueensCrown;
        public bool MiniSaucer;
        public bool TribalCharm;
        public bool TribalAutoFire;
        public bool SupremeDeathbringerFairy;
        public bool GodEaterImbue;
        public bool MutantSetBonus;
        public bool Abominationn;
        public bool PhantasmalRing;
        public bool MutantsDiscountCard;
        public bool MutantsPact;
        public bool RabiesVaccine;
        public bool TwinsEX;
        public bool TimsConcoction;
        public bool ReceivedMasoGift;
        public bool Graze;
        public float GrazeRadius;
        public int GrazeCounter;
        public double GrazeBonus;
        public bool DevianttHearts;
        public int DevianttHeartsCD;
        public bool MutantEye;
        public bool MutantEyeVisual;
        public int MutantEyeCD;
        public bool AbominableWandRevived;
        public bool AbomRebirth;
        public bool WasHurtBySomething;

        //public int PreNerfDamage;

        //debuffs
        public bool Hexed;
        public bool Unstable;
        private int unstableCD = 0;
        public bool Fused;
        public bool Shadowflame;
        public bool Oiled;
        public bool DeathMarked;
        public bool Hypothermia;
        public bool noDodge;
        public bool noSupersonic;
        public bool Bloodthirsty;
        public bool DisruptedFocus;
        public bool SinisterIcon;
        public bool SinisterIconDrops;

        public bool GodEater;               //defense removed, endurance removed, colossal DOT
        public bool FlamesoftheUniverse;    //activates various vanilla debuffs
        public bool MutantNibble;           //disables potions, moon bite effect, feral bite effect, disables lifesteal
        public int StatLifePrevious = -1;   //used for mutantNibble
        public bool Asocial;                //disables minions, disables pets
        public bool WasAsocial;
        public bool HidePetToggle0 = true;
        public bool HidePetToggle1 = true;
        public bool Kneecapped;             //disables running :v
        public bool Defenseless;            //-30 defense, no damage reduction, cross necklace and knockback prevention effects disabled
        public bool Purified;               //purges all other buffs
        public bool Infested;               //weak DOT that grows exponentially stronger
        public int MaxInfestTime;
        public bool FirstInfection = true;
        public float InfestedDust;
        public bool Rotting;                //inflicts DOT and almost every stat reduced
        public bool SqueakyToy;             //all attacks do one damage and make squeaky noises
        public bool Atrophied;              //melee speed and damage reduced. maybe player cannot fire melee projectiles?
        public bool Jammed;                 //ranged damage and speed reduced, all non-custom ammo set to baseline ammos
        public bool Slimed;
        public byte lightningRodTimer;
        public bool ReverseManaFlow;
        public bool CurseoftheMoon;
        public bool OceanicMaul;
        public int MaxLifeReduction;
        public bool Midas;
        public bool MutantPresence;
        public bool DevianttPresence;
        public bool Swarming;
        public bool LowGround;
        public bool Flipped;
        public bool Mash;
        public bool[] MashPressed = new bool[4];
        public int MashCounter;
        public int StealingCooldown;
        public bool LihzahrdCurse;
        public bool LihzahrdBlessing;
        public bool Berserked;
        public bool HolyPrice;
        public bool NanoInjection;

        public int ReallyAwfulDebuffCooldown;

        public int MasomodeCrystalTimer;
        public int MasomodeFreezeTimer;
        public int MasomodeSpaceBreathTimer;
        public int MasomodeWeaponUseTimer;
        public int MasomodeMinionNerfTimer;
        public const int MaxMasomodeMinionNerfTimer = 300;
        public bool ReduceMasomodeMinionNerf;

        public IList<string> disabledSouls = new List<string>();

        private Mod dbzMod = ModLoader.GetMod("DBZMOD");

        public bool DoubleTap
        {
            get
            {
                return Main.ReversedUpDownArmorSetBonuses ?
                    player.controlUp && player.releaseUp && player.doubleTapCardinalTimer[1] > 0 && player.doubleTapCardinalTimer[1] != 15
                    : player.controlDown && player.releaseDown && player.doubleTapCardinalTimer[0] > 0 && player.doubleTapCardinalTimer[0] != 15;
            }
        }

        public override TagCompound Save()
        {
            //idk ech
            string name = "FargoDisabledSouls" + player.name;
            var FargoDisabledSouls = new List<string>();

            if (CelestialSeal) FargoDisabledSouls.Add("CelestialSeal");
            if (MutantsDiscountCard) FargoDisabledSouls.Add("MutantsDiscountCard");
            if (MutantsPact) FargoDisabledSouls.Add("MutantsPact");
            if (ReceivedMasoGift) FargoDisabledSouls.Add("ReceivedMasoGift");
            if (RabiesVaccine) FargoDisabledSouls.Add("RabiesVaccine");

            Toggler.Save();

            return new TagCompound {
                    {name, FargoDisabledSouls}
                }; ;
        }

        public override void Load(TagCompound tag)
        {
            string name = "FargoDisabledSouls" + player.name;
            //string log = name + " loaded: ";

            disabledSouls = tag.GetList<string>(name);

            CelestialSeal = disabledSouls.Contains("CelestialSeal");
            MutantsDiscountCard = disabledSouls.Contains("MutantsDiscountCard");
            MutantsPact = disabledSouls.Contains("MutantsPact");
            ReceivedMasoGift = disabledSouls.Contains("ReceivedMasoGift");
            RabiesVaccine = disabledSouls.Contains("RabiesVaccine");
        }

        public override void Initialize()
        {
            Toggler.Load(this);
        }

        public override void OnEnterWorld(Player player)
        {
            disabledSouls.Clear();

            if (ModLoader.GetMod("FargowiltasMusic") == null)
            {
                Main.NewText(Language.GetTextValue("Mods.FargowiltasSouls.FargoPlayer.Music1"), Color.LimeGreen);
                Main.NewText(Language.GetTextValue("Mods.FargowiltasSouls.FargoPlayer.Music2"), Color.LimeGreen);
            }

            /*for (int i = 0; i < 200; i++)
            {
                if (Main.npc[i].type == NPCID.LunarTowerSolar
                || Main.npc[i].type == NPCID.LunarTowerVortex
                || Main.npc[i].type == NPCID.LunarTowerNebula
                || Main.npc[i].type == NPCID.LunarTowerStardust)
                {
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        var netMessage = mod.GetPacket();
                        netMessage.Write((byte)1);
                        netMessage.Write((byte)i);
                        netMessage.Send();
                        Main.npc[i].lifeMax *= 2;
                    }
                    else
                    {
                        Main.npc[i].GetGlobalNPC<FargoSoulsGlobalNPC>().SetDefaults(Main.npc[i]);
                        Main.npc[i].life = Main.npc[i].lifeMax;
                    }
                }
            }*/
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (Mash)
            {
                const int increment = 1;

                if (triggersSet.Up)
                {
                    if (!MashPressed[0])
                        MashCounter += increment;
                    MashPressed[0] = true;
                }
                else
                {
                    MashPressed[0] = false;
                }

                if (triggersSet.Left)
                {
                    if (!MashPressed[1])
                        MashCounter += increment;
                    MashPressed[1] = true;
                }
                else
                {
                    MashPressed[1] = false;
                }

                if (triggersSet.Right)
                {
                    if (!MashPressed[2])
                        MashCounter += increment;
                    MashPressed[2] = true;
                }
                else
                {
                    MashPressed[2] = false;
                }

                if (triggersSet.Down)
                {
                    if (!MashPressed[3])
                        MashCounter += increment;
                    MashPressed[3] = true;
                }
                else
                {
                    MashPressed[3] = false;
                }
            }

            if (Fargowiltas.GoldKey.JustPressed && GoldEnchant)
            {
                if (!player.HasBuff(ModContent.BuffType<GoldenStasis>()) && !player.HasBuff(ModContent.BuffType<GoldenStasisCD>()))
                {
                    int duration = 300;

                    if (WillForce)
                    {
                        duration *= 2;
                    }

                    player.AddBuff(ModContent.BuffType<GoldenStasis>(), duration);
                    player.AddBuff(ModContent.BuffType<GoldenStasisCD>(), 3600);

                    goldHP = player.statLife;
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Zhonyas").WithVolume(1f), player.Center);
                }
                //cancel it early
                else
                {
                    player.ClearBuff(ModContent.BuffType<GoldenStasis>());
                }
            }

            if (GoldShell)
            {
                return;
            }

            if (Fargowiltas.FreezeKey.JustPressed && StardustEnchant && !player.HasBuff(ModContent.BuffType<TimeStopCD>()))
            {
                int cooldownInSeconds = 60;
                if (CosmoForce)
                    cooldownInSeconds = 50;
                if (TerrariaSoul)
                    cooldownInSeconds = 40;
                if (Eternity)
                    cooldownInSeconds = 30;
                player.AddBuff(ModContent.BuffType<TimeStopCD>(), cooldownInSeconds * 60);
                FreezeTime = true;
                freezeLength = 540;
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/ZaWarudo").WithVolume(1f).WithPitchVariance(.5f), player.Center);
            }

            

            if (Fargowiltas.SmokeBombKey.JustPressed && NinjaEnchant && hasSmokeBomb && smokeBombCD == 0)
            {
                //already threw smoke bomb, tele to it
                if (ninjaSmokeBomb != null)
                {
                    Vector2 teleportPos = new Vector2(ninjaSmokeBomb.position.X, ninjaSmokeBomb.position.Y - 30);
                    Vector2 originalPos = new Vector2(teleportPos.X, teleportPos.Y);

                    //spiral out to find a save spot
                    int count = 0;
                    int increase = 10;
                    while (Collision.SolidCollision(teleportPos, player.width, player.height))
                    {
                        teleportPos = originalPos;

                        switch (count)
                        {
                            case 0:
                                teleportPos.X -= increase;
                                break;
                            case 1:
                                teleportPos.X += increase;
                                break;
                            case 2:
                                teleportPos.Y += increase;
                                break;
                            default:
                                teleportPos.Y -= increase;
                                increase += 10;
                                break;
                        }
                        count++;

                        if (count >= 4)
                        {
                            count = 0;
                        }

                        if (increase > 100)
                        {
                            return;
                        }
                    }

                    if (teleportPos.X > 50 && teleportPos.X < (double)(Main.maxTilesX * 16 - 50) && teleportPos.Y > 50 && teleportPos.Y < (double)(Main.maxTilesY * 16 - 50))
                    {
                        player.Teleport(teleportPos, 1);
                        NetMessage.SendData(MessageID.Teleport, -1, -1, null, 0, player.whoAmI, teleportPos.X, teleportPos.Y, 1);

                        player.AddBuff(ModContent.BuffType<FirstStrike>(), 60);

                        ninjaSmokeBomb.timeLeft = 120;
                        smokeBombCD = 300;
                        ninjaSmokeBomb = null;
                    }
                }
                //throw smoke bomb
                else
                {
                    const float gravity = 0.18f;
                    float time = 60f;
                    Vector2 distance = Main.MouseWorld - player.Center;
                    distance.X = distance.X / time;
                    distance.Y = distance.Y / time - 0.5f * gravity * time;

                    ninjaSmokeBomb = Main.projectile[Projectile.NewProjectile(player.Center, distance + Main.rand.NextVector2Square(0, 0) * 2,
                            ProjectileID.SmokeBomb, 0, 0f, Main.myPlayer)];

                    smokeBombCD = 15;
                    player.inventory[smokeBombSlot].stack--;
                }
            }

            if (Fargowiltas.BetsyDashKey.JustPressed && BetsysHeart && BetsyDashCD <= 0)
            {
                BetsyDashCD = 180;
                if (player.whoAmI == Main.myPlayer)
                {
                    /*player.controlLeft = false;
                    player.controlRight = false;
                    player.controlJump = false;
                    player.controlDown = false;*/
                    player.controlUseItem = false;
                    player.controlUseTile = false;
                    player.controlHook = false;
                    player.controlMount = false;

                    player.immune = true;
                    player.immuneTime = Math.Max(player.immuneTime, 2);
                    player.hurtCooldowns[0] = Math.Max(player.hurtCooldowns[0], 2);
                    player.hurtCooldowns[1] = Math.Max(player.hurtCooldowns[1], 2);

                    player.itemAnimation = 0;
                    player.itemTime = 0;

                    Vector2 vel = player.DirectionTo(Main.MouseWorld) * (MasochistHeart ? 25 : 20);
                    Projectile.NewProjectile(player.Center, vel, ModContent.ProjectileType<Projectiles.Masomode.BetsyDash>(), (int)(100 * player.meleeDamage), 0f, player.whoAmI);
                    player.AddBuff(ModContent.BuffType<Buffs.Souls.BetsyDash>(), 20);

                    //immune to all debuffs
                    foreach (int debuff in Fargowiltas.DebuffIDs)
                    {
                        if (!player.HasBuff(debuff))
                        {
                            player.buffImmune[debuff] = true;
                        }
                    }
                }
            }

            if (Fargowiltas.MutantBombKey.JustPressed && MutantEye && MutantEyeCD <= 0)
            {
                MutantEyeCD = 3600;

                if (!Main.dedServ && Main.LocalPlayer.active)
                    Main.LocalPlayer.GetModPlayer<FargoPlayer>().Screenshake = 30;

                const int invulTime = 90;
                player.immune = true;
                player.immuneTime = invulTime;
                player.hurtCooldowns[0] = invulTime;
                player.hurtCooldowns[1] = invulTime;

                Main.PlaySound(SoundID.Item84, player.Center);

                const int max = 100; //make some indicator dusts
                for (int i = 0; i < max; i++)
                {
                    Vector2 vector6 = Vector2.UnitY * 30f;
                    vector6 = vector6.RotatedBy((i - (max / 2 - 1)) * 6.28318548f / max) + Main.LocalPlayer.Center;
                    Vector2 vector7 = vector6 - Main.LocalPlayer.Center;
                    int d = Dust.NewDust(vector6 + vector7, 0, 0, 229, 0f, 0f, 0, default(Color), 3f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity = vector7;
                }

                for (int i = 0; i < 50; i++) //make some indicator dusts
                {
                    int d = Dust.NewDust(player.position, player.width, player.height, 229, 0f, 0f, 0, default(Color), 2.5f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].noLight = true;
                    Main.dust[d].velocity *= 24f;
                }

                FargoSoulsUtil.ClearHostileProjectiles(1);

                void SpawnSphereRing(int ringMax, float speed, int damage2, float rotationModifier)
                {
                    float rotation = 2f * (float)Math.PI / ringMax;
                    Vector2 vel = Vector2.UnitY * speed;
                    int type = ModContent.ProjectileType<PhantasmalSphereRing>();
                    for (int i = 0; i < ringMax; i++)
                    {
                        vel = vel.RotatedBy(rotation);
                        Projectile.NewProjectile(player.Center, vel, type, damage2, 0f, Main.myPlayer, rotationModifier * player.direction, speed);
                    }
                }

                int damage = (int)(1700 * player.magicDamage);
                SpawnSphereRing(24, 12f, damage, -1f);
                SpawnSphereRing(24, 12f, damage, 1f);
            }

            if (triggersSet.Left && player.confused && player.gravControl)
            {
                player.gravDir *= -1;
            }

            if (Fargowiltas.SoulToggleKey.JustPressed)
            {
                Fargowiltas.UserInterfaceManager.ToggleSoulToggler();
            }
        }

        public override void ResetEffects()
        {
            if (CelestialSeal)
            {
                player.extraAccessory = true;
                player.extraAccessorySlots = 2;
            }

            SummonCrit = 0;

            AttackSpeed = 1f;
            if (Screenshake > 0)
                Screenshake--;

            Wood = false;

            wingTimeModifier = 1f;

            QueenStinger = false;
            EridanusEmpower = false;
            GaiaSet = false;
            StyxSet = false;

            BrainMinion = false;
            EaterMinion = false;
            BigBrainMinion = false;
            DukeFishron = false;

            SquirrelMount = false;

            ChibiDevi = false;
            MutantSpawn = false;
            BabyAbom = false;

            #region enchantments 
            PetsActive = true;
            ShadowEnchant = false;
            CrimsonEnchant = false;
            CrimsonRegen = false;
            SpectreEnchant = false;
            BeeEnchant = false;
            SpiderEnchant = false;
            StardustEnchant = false;
            MythrilEnchant = false;
            FossilEnchant = false;
            JungleEnchant = false;
            ElementEnchant = false;
            ShroomEnchant = false;
            CobaltEnchant = false;
            SpookyEnchant = false;
            HallowEnchant = false;
            ChloroEnchant = false;
            VortexEnchant = false;
            AdamantiteEnchant = false;
            FrostEnchant = false;
            PalladEnchant = false;
            OriEnchant = false;
            MeteorEnchant = false;
            MoltenEnchant = false;
            CopperEnchant = false;
            NinjaEnchant = false;
            FirstStrike = false;
            NearSmoke = false;
            hasSmokeBomb = false;
            IronEnchant = false;
            IronGuard = false;
            TurtleEnchant = false;
            ShellHide = false;
            LeadEnchant = false;
            GladEnchant = false;
            GoldEnchant = false;
            GoldShell = false;
            CactusEnchant = false;
            ForbiddenEnchant = false;
            MinerEnchant = false;
            PumpkinEnchant = false;
            SilverEnchant = false;
            PlatinumEnchant = false;
            NecroEnchant = false;
            ObsidianEnchant = false;
            LavaWet = false;
            TinEnchant = false;
            TikiEnchant = false;
            TikiMinion = false;
            TikiSentry = false;
            SolarEnchant = false;
            ShinobiEnchant = false;
            ValhallaEnchant = false;
            DarkEnchant = false;
            RedEnchant = false;
            TungstenEnchant = false;

            MahoganyEnchant = false;
            BorealEnchant = false;
            WoodEnchant = false;
            PalmEnchant = false;
            ShadeEnchant = false;
            PearlEnchant = false;

            RainEnchant = false;
            AncientCobaltEnchant = false;
            AncientShadowEnchant = false;
            SquireEnchant = false;
            squireReduceIframes = false;
            ApprenticeEnchant = false;
            HuntressEnchant = false;
            MonkEnchant = false;
            SnowEnchant = false;
            SnowVisual = false;

            Solar = false;
            Nebula = false;

            CosmoForce = false;
            EarthForce = false;
            LifeForce = false;
            NatureForce = false;
            SpiritForce = false;
            TerraForce = false;
            ShadowForce = false;
            WillForce = false;
            WoodForce = false;

            #endregion

            //souls
            MagicSoul = false;
            ThrowSoul = false;
            RangedSoul = false;
            RangedEssence = false;
            BuilderMode = false;
            UniverseEffect = false;
            NecroPet = false;
            FishSoul1 = false;
            FishSoul2 = false;
            TerrariaSoul = false;
            VoidSoul = false;
            Eternity = false;

            //maso
            SlimyShield = false;
            AgitatingLens = false;
            CorruptHeart = false;
            GuttedHeart = false;
            NecromanticBrew = false;
            PureHeart = false;
            PungentEyeballMinion = false;
            CrystalSkullMinion = false;
            FusedLens = false;
            GroundStick = false;
            Probes = false;
            MagicalBulb = false;
            SkullCharm = false;
            PumpkingsCape = false;
            LihzahrdTreasureBox = false;
            BetsysHeart = false;
            BetsyDashing = false;
            MutantAntibodies = false;
            GravityGlobeEX = false;
            CelestialRune = false;
            AdditionalAttacks = false;
            MoonChalice = false;
            LunarCultist = false;
            TrueEyes = false;
            CyclonicFin = false;
            MasochistSoul = false;
            MasochistHeart = false;
            SandsofTime = false;
            DragonFang = false;
            SecurityWallet = false;
            FrigidGemstone = false;
            WretchedPouch = false;
            NymphsPerfume = false;
            NymphsPerfumeRespawn = false;
            SqueakyAcc = false;
            RainbowSlime = false;
            SkeletronArms = false;
            SuperFlocko = false;
            IceQueensCrown = false;
            MiniSaucer = false;
            TribalCharm = false;
            SupremeDeathbringerFairy = false;
            GodEaterImbue = false;
            MutantSetBonus = false;
            Abominationn = false;
            PhantasmalRing = false;
            TwinsEX = false;
            TimsConcoction = false;
            Graze = false;
            GrazeRadius = 100f;
            DevianttHearts = false;
            MutantEye = false;
            MutantEyeVisual = false;
            AbomRebirth = false;
            WasHurtBySomething = false;

            //debuffs
            Hexed = false;
            Unstable = false;
            Fused = false;
            Shadowflame = false;
            Oiled = false;
            Slimed = false;
            noDodge = false;
            noSupersonic = false;
            Bloodthirsty = false;
            DisruptedFocus = false;
            SinisterIcon = false;
            SinisterIconDrops = false;

            GodEater = false;
            FlamesoftheUniverse = false;
            MutantNibble = false;
            Asocial = false;
            Kneecapped = false;
            Defenseless = false;
            Purified = false;
            Infested = false;
            Rotting = false;
            SqueakyToy = false;
            Atrophied = false;
            Jammed = false;
            ReverseManaFlow = false;
            CurseoftheMoon = false;
            OceanicMaul = false;
            DeathMarked = false;
            Hypothermia = false;
            Midas = false;
            MutantPresence = MutantPresence ? player.HasBuff(ModContent.BuffType<Buffs.Boss.MutantPresence>()) : false;
            DevianttPresence = false;
            Swarming = false;
            LowGround = false;
            Flipped = false;
            LihzahrdCurse = false;
            LihzahrdBlessing = false;
            Berserked = false;
            HolyPrice = false;
            NanoInjection = false;
            ReduceMasomodeMinionNerf = false;

            if (WizardEnchant)
            {
                WizardEnchant = false;
                for (int i = 3; i <= 9; i++)
                {
                    if (!player.armor[i].IsAir && (player.armor[i].type == ModContent.ItemType<WizardEnchant>() || player.armor[i].type == ModContent.ItemType<Items.Accessories.Forces.WillForce>()))
                    {
                        WizardEnchant = true;
                        CosmoForce = true;
                        EarthForce = true;
                        LifeForce = true;
                        NatureForce = true;
                        ShadowForce = true;
                        SpiritForce = true;
                        TerraForce = true;
                        WillForce = true;
                        WoodForce = true;
                        break;
                    }
                }
            }

            if (!Mash && MashCounter > 0)
            {
                MashCounter--;
            }
            Mash = false;
        }

        public override void OnRespawn(Player player)
        {
            if (NymphsPerfumeRespawn && !FargoSoulsUtil.AnyBossAlive())
            {
                player.statLife = player.statLifeMax2;
            }
        }

        public override void UpdateDead()
        {
            if (SandsofTime && !FargoSoulsUtil.AnyBossAlive() && player.respawnTimer > 10)
                player.respawnTimer -= Eternity ? 6 : 1;

            ReallyAwfulDebuffCooldown = 0;
            IronDebuffImmuneTime = 0;

            FreezeTime = false;
            freezeLength = 0;

            /*if (!Main.dedServ)
            {
                if (Fargowiltas.OldMusicFade > Main.musicVolume)
                {
                    Main.musicVolume = Fargowiltas.OldMusicFade;
                    Fargowiltas.OldMusicFade = 0;
                }
            }*/

            wingTimeModifier = 1f;
            FreeEaterSummon = true;
            if (Screenshake > 0)
                Screenshake--;

            EridanusEmpower = false;
            EridanusTimer = 0;
            GaiaSet = false;
            GaiaOffense = false;
            StyxSet = false;
            StyxMeter = 0;

            //debuffs
            Hexed = false;
            Unstable = false;
            unstableCD = 0;
            Fused = false;
            Shadowflame = false;
            Oiled = false;
            Slimed = false;
            noDodge = false;
            noSupersonic = false;
            lightningRodTimer = 0;

            BuilderMode = false;

            SlimyShieldFalling = false;
            CorruptHeartCD = 60;
            GuttedHeartCD = 60;
            NecromanticBrew = false;
            GroundPound = 0;
            NymphsPerfume = false;
            NymphsPerfumeCD = 30;
            PungentEyeballMinion = false;
            CrystalSkullMinion = false;
            MagicalBulb = false;
            LunarCultist = false;
            TrueEyes = false;
            BetsyDashing = false;

            WretchedPouch = false;
            WretchedPouchCD = 0;

            GodEater = false;
            FlamesoftheUniverse = false;
            MutantNibble = false;
            Asocial = false;
            Kneecapped = false;
            Defenseless = false;
            Purified = false;
            Infested = false;
            Rotting = false;
            SqueakyToy = false;
            Atrophied = false;
            Jammed = false;
            CurseoftheMoon = false;
            OceanicMaul = false;
            DeathMarked = false;
            Hypothermia = false;
            Midas = false;
            Bloodthirsty = false;
            DisruptedFocus = false;
            SinisterIcon = false;
            SinisterIconDrops = false;
            Graze = false;
            GrazeRadius = 100f;
            GrazeBonus = 0;
            DevianttHearts = false;
            MutantEye = false;
            MutantEyeVisual = false;
            MutantEyeCD = 60;
            AbominableWandRevived = false;
            AbomRebirth = false;
            WasHurtBySomething = false;
            Mash = false;
            WizardEnchant = false;
            MashCounter = 0;

            MaxLifeReduction = 0;

            MasomodeWeaponUseTimer = 0;
            MasomodeMinionNerfTimer = 0;
        }

        public override void PreUpdate()
        {
            if (HurtTimer > 0)
                HurtTimer--;

            IsStandingStill = Math.Abs(player.velocity.X) < 0.05 && Math.Abs(player.velocity.Y) < 0.05;
            
            player.npcTypeNoAggro[0] = true;

            if (FargoSoulsWorld.EternityMode)
            {
                //falling gives you dazed. wings save you
                if (player.velocity.Y == 0f && player.wingsLogic == 0 && !player.noFallDmg && !player.ghost && !player.dead)
                {
                    int num21 = 25;
                    num21 += player.extraFall;
                    int num22 = (int)(player.position.Y / 16f) - player.fallStart;
                    if (player.mount.CanFly)
                    {
                        num22 = 0;
                    }
                    if (player.mount.Cart && Minecart.OnTrack(player.position, player.width, player.height))
                    {
                        num22 = 0;
                    }
                    if (player.mount.Type == 1)
                    {
                        num22 = 0;
                    }
                    player.mount.FatigueRecovery();

                    if (((player.gravDir == 1f && num22 > num21) || (player.gravDir == -1f && num22 < -num21)))
                    {
                        player.immune = false;
                        int dmg = (int)(num22 * player.gravDir - num21) * 10;
                        if (player.mount.Active)
                            dmg = (int)(dmg * player.mount.FallDamage);

                        player.Hurt(PlayerDeathReason.ByOther(0), dmg, 0);
                        player.AddBuff(BuffID.Dazed, 120);
                    }
                    player.fallStart = (int)(player.position.Y / 16f);
                }

                if (!NPC.downedBoss3 && player.ZoneDungeon && !NPC.AnyNPCs(NPCID.DungeonGuardian))
                {
                    NPC.SpawnOnPlayer(player.whoAmI, NPCID.DungeonGuardian);
                }

                if (player.ZoneUnderworldHeight)
                {
                    if (!(player.fireWalk || PureHeart || player.lavaMax > 0))
                        player.AddBuff(BuffID.OnFire, Main.expertMode && Main.expertDebuffTime > 1 ? 1 : 2);
                }

                if (player.ZoneJungle)
                {
                    if (Framing.GetTileSafely(player.Center).wall == WallID.LihzahrdBrickUnsafe)
                        player.AddBuff(ModContent.BuffType<LihzahrdCurse>(), 2);

                    if (player.wet && !player.lavaWet && !player.honeyWet && !MutantAntibodies)
                        player.AddBuff(BuffID.Poisoned, Main.expertMode && Main.expertDebuffTime > 1 ? 1 : 2);
                }

                if (player.ZoneSnow)
                {
                    //if (!PureHeart && !Main.dayTime && Framing.GetTileSafely(player.Center).wall == WallID.None)
                    //    player.AddBuff(BuffID.Chilled, Main.expertMode && Main.expertDebuffTime > 1 ? 1 : 2);

                    if (player.wet && !player.lavaWet && !player.honeyWet && !MutantAntibodies)
                    {
                        //player.AddBuff(BuffID.Frostburn, Main.expertMode && Main.expertDebuffTime > 1 ? 1 : 2);
                        MasomodeFreezeTimer++;
                        if (MasomodeFreezeTimer >= 600)
                        {
                            player.AddBuff(BuffID.Frozen, Main.expertMode && Main.expertDebuffTime > 1 ? 60 : 120);
                            MasomodeFreezeTimer = -300;
                        }
                    }
                    else
                    {
                        MasomodeFreezeTimer = 0;
                    }
                }
                else
                {
                    MasomodeFreezeTimer = 0;
                }

                /*if (player.wet && !MutantAntibodies)
                {
                    if (player.ZoneDesert)
                        player.AddBuff(BuffID.Slow, Main.expertMode && Main.expertDebuffTime > 1 ? 1 : 2);
                    if (player.ZoneDungeon)
                        player.AddBuff(BuffID.Cursed, Main.expertMode && Main.expertDebuffTime > 1 ? 1 : 2);
                    Tile currentTile = Framing.GetTileSafely(player.Center);
                    if (currentTile.wall == WallID.GraniteUnsafe)
                        player.AddBuff(BuffID.Weak, Main.expertMode && Main.expertDebuffTime > 1 ? 1 : 2);
                    if (currentTile.wall == WallID.MarbleUnsafe)
                        player.AddBuff(BuffID.BrokenArmor, Main.expertMode && Main.expertDebuffTime > 1 ? 1 : 2);
                }*/

                if (player.ZoneCorrupt)
                {
                    if (!PureHeart)
                        player.AddBuff(BuffID.Darkness, Main.expertMode && Main.expertDebuffTime > 1 ? 1 : 2);
                    if (player.wet && !player.lavaWet && !player.honeyWet && !MutantAntibodies)
                        player.AddBuff(BuffID.CursedInferno, Main.expertMode && Main.expertDebuffTime > 1 ? 1 : 2);
                }

                if (player.ZoneCrimson)
                {
                    if (!PureHeart)
                        player.AddBuff(BuffID.Bleeding, Main.expertMode && Main.expertDebuffTime > 1 ? 1 : 2);
                    if (player.wet && !player.lavaWet && !player.honeyWet && !MutantAntibodies)
                        player.AddBuff(BuffID.Ichor, Main.expertMode && Main.expertDebuffTime > 1 ? 1 : 2);
                }

                if (player.ZoneHoly && (player.ZoneRockLayerHeight || player.ZoneDirtLayerHeight) && player.active)
                {
                    if (!PureHeart)
                        player.AddBuff(ModContent.BuffType<FlippedHallow>(), 120);
                    if (player.wet && !player.lavaWet && !player.honeyWet && !MutantAntibodies)
                        player.AddBuff(BuffID.Confused, Main.expertMode && Main.expertDebuffTime > 1 ? 1 : 2);
                }

                if (!PureHeart && Main.raining && (player.ZoneOverworldHeight || player.ZoneSkyHeight) && player.HeldItem.type != ItemID.Umbrella)
                {
                    Tile currentTile = Framing.GetTileSafely(player.Center);
                    if (currentTile.wall == WallID.None)
                    {
                        if (player.ZoneSnow)
                            player.AddBuff(ModContent.BuffType<Hypothermia>(), 2);
                        else
                            player.AddBuff(BuffID.Wet, 2);
                        /*if (Main.hardMode)
                        {
                            lightningCounter++;

                            if (lightningCounter >= 600)
                            {
                                //tends to spawn in ceilings if the player goes indoors/underground
                                Point tileCoordinates = player.Top.ToTileCoordinates();

                                tileCoordinates.X += Main.rand.Next(-25, 25);
                                tileCoordinates.Y -= 15 + Main.rand.Next(-5, 5);

                                for (int index = 0; index < 10 && !WorldGen.SolidTile(tileCoordinates.X, tileCoordinates.Y) && tileCoordinates.Y > 10; ++index) tileCoordinates.Y -= 1;

                                Projectile.NewProjectile(tileCoordinates.X * 16 + 8, tileCoordinates.Y * 16 + 17, 0f, 0f, ProjectileID.VortexVortexLightning, 0, 2f, Main.myPlayer,
                                    0f, 0f);

                                lightningCounter = 0;
                            }
                        }*/
                    } 
                }

                if (player.wet && !player.lavaWet && !player.honeyWet && !(player.accFlipper || player.gills || MutantAntibodies))
                    player.AddBuff(ModContent.BuffType<Lethargic>(), 2);

                if (!PureHeart && !player.buffImmune[BuffID.Suffocation] && player.ZoneSkyHeight && player.whoAmI == Main.myPlayer)
                {
                    bool inLiquid = Collision.DrownCollision(player.position, player.width, player.height, player.gravDir);
                    if (!inLiquid)
                    {
                        player.breath -= 3;
                        if (++MasomodeSpaceBreathTimer > 10)
                        {
                            MasomodeSpaceBreathTimer = 0;
                            player.breath--;
                        }
                        if (player.breath == 0)
                            Main.PlaySound(SoundID.Item3);
                        if (player.breath <= 0)
                            player.AddBuff(BuffID.Suffocation, 2);
                    }
                }

                if (!PureHeart && !player.buffImmune[BuffID.Webbed] && player.stickyBreak > 0)
                {
                    Vector2 tileCenter = player.Center;
                    tileCenter.X /= 16;
                    tileCenter.Y /= 16;
                    Tile currentTile = Framing.GetTileSafely((int)tileCenter.X, (int)tileCenter.Y);
                    if (currentTile != null && currentTile.wall == WallID.SpiderUnsafe)
                    {
                        player.AddBuff(BuffID.Webbed, 30);
                        player.AddBuff(BuffID.Slow, 90);
                        player.stickyBreak = 0;

                        Vector2 vector = Collision.StickyTiles(player.position, player.velocity, player.width, player.height);
                        if (vector.X != -1 && vector.Y != -1)
                        {
                            int num3 = (int)vector.X;
                            int num4 = (int)vector.Y;
                            WorldGen.KillTile(num3, num4, false, false, false);
                            if (Main.netMode == NetmodeID.MultiplayerClient && !Main.tile[num3, num4].active())
                                NetMessage.SendData(MessageID.TileChange, -1, -1, null, 0, num3, num4, 0f, 0, 0, 0);
                        }
                    }
                }

                if (!PureHeart && Main.bloodMoon)
                    player.AddBuff(BuffID.WaterCandle, 2);

                if (!SandsofTime)
                {
                    Vector2 tileCenter = player.Center;
                    tileCenter.X /= 16;
                    tileCenter.Y /= 16;
                    Tile currentTile = Framing.GetTileSafely((int)tileCenter.X, (int)tileCenter.Y);
                    if (currentTile != null && currentTile.type == TileID.Cactus && currentTile.nactive())
                    {
                        int damage = 10;
                        if (player.ZoneCorrupt)
                        {
                            damage *= 2;
                            player.AddBuff(BuffID.CursedInferno, Main.expertMode && Main.expertDebuffTime > 1 ? 150 : 300);
                        }
                        if (player.ZoneCrimson)
                        {
                            damage *= 2;
                            player.AddBuff(BuffID.Ichor, Main.expertMode && Main.expertDebuffTime > 1 ? 150 : 300);
                        }
                        if (player.ZoneHoly)
                        {
                            damage *= 2;
                            player.AddBuff(BuffID.Confused, Main.expertMode && Main.expertDebuffTime > 1 ? 150 : 300);
                        }

                        if (Main.hardMode)
                            damage *= 2;

                        if (player.hurtCooldowns[0] <= 0) //same i-frames as spike tiles
                            player.Hurt(PlayerDeathReason.ByCustomReason(player.name + Language.GetTextValue("Mods.FargowiltasSouls.FargoPlayer.KilledByCactus")), damage, 0, false, false, false, 0);
                    }
                }

                if (MasomodeCrystalTimer > 0)
                    MasomodeCrystalTimer--;
            }

            if (!Infested && !FirstInfection)
                FirstInfection = true;

            if (Eternity && TinCrit < 50)
                TinCrit = 50;
            else if(TerrariaSoul && TinCrit < 20)
                TinCrit = 20;
            else if (TerraForce && TinCrit < 10)
                TinCrit = 10;

            if (tinCD > 0)
                tinCD--;

            if (VortexStealth && !VortexEnchant)
                VortexStealth = false;

            if (Unstable && player.whoAmI == Main.myPlayer)
            {
                if (unstableCD == 0)
                {
                    Vector2 pos = player.position;

                    int x = Main.rand.Next((int)pos.X - 500, (int)pos.X + 500);
                    int y = Main.rand.Next((int)pos.Y - 500, (int)pos.Y + 500);
                    Vector2 teleportPos = new Vector2(x, y);

                    while (Collision.SolidCollision(teleportPos, player.width, player.height) && teleportPos.X > 50 && teleportPos.X < (double)(Main.maxTilesX * 16 - 50) && teleportPos.Y > 50 && teleportPos.Y < (double)(Main.maxTilesY * 16 - 50))
                    {
                        x = Main.rand.Next((int)pos.X - 500, (int)pos.X + 500);
                        y = Main.rand.Next((int)pos.Y - 500, (int)pos.Y + 500);
                        teleportPos = new Vector2(x, y);
                    }

                    player.Teleport(teleportPos, 1);
                    NetMessage.SendData(MessageID.Teleport, -1, -1, null, 0, player.whoAmI, teleportPos.X, teleportPos.Y, 1);

                    unstableCD = 60;
                }
                unstableCD--;
            }

            if (CopperEnchant && copperCD > 0)
                copperCD--;

            if (ObsidianEnchant && obsidianCD > 0)
                obsidianCD--;

            if (PearlEnchant && pearlCD > 0)
                pearlCD--;

            if (TungstenEnchant && TungstenCD > 0)
                TungstenCD--;

            if (BeeEnchant && beeCD > 0)
                beeCD--;

            if (GoldShell)
            {
                player.immune = true;
                player.immuneTime = 2;
                player.hurtCooldowns[0] = 2;
                player.hurtCooldowns[1] = 2;
                player.stealth = 1;

                //immune to DoT
                if (player.statLife < goldHP)
                    player.statLife = goldHP;

                if (player.ownedProjectileCounts[ModContent.ProjectileType<GoldShellProj>()] <= 0)
                    Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, 0f, ModContent.ProjectileType<GoldShellProj>(), 0, 0, Main.myPlayer);
            }

            if ((CobaltEnchant || AncientCobaltEnchant) && CobaltCD > 0)
                CobaltCD--;

            if ((AdamantiteEnchant) && AdamantiteCD > 0)
                AdamantiteCD--;

            if (LihzahrdTreasureBox && player.gravDir > 0 && player.GetToggleValue("MasoGolem"))
            {
                if (player.controlDown && !player.mount.Active && !player.controlJump)
                {
                    if (player.velocity.Y != 0f)
                    {
                        if (player.velocity.Y < 15f)
                            player.velocity.Y = 15f;
                        if (GroundPound <= 0)
                            GroundPound = 1;
                    }
                }
                if (GroundPound > 0)
                {
                    if (player.velocity.Y < 0f || player.mount.Active)
                    {
                        GroundPound = 0;
                    }
                    else if (player.velocity.Y == 0f)
                    {
                        if (player.whoAmI == Main.myPlayer)
                        {
                            int x = (int)(player.Center.X) / 16;
                            int y = (int)(player.position.Y + player.height + 8) / 16;
                            if (GroundPound > 15 && x >= 0 && x < Main.maxTilesX && y >= 0 && y < Main.maxTilesY
                                && Main.tile[x, y] != null && Main.tile[x, y].nactive() && Main.tileSolid[Main.tile[x, y].type])
                            {
                                GroundPound = 0;

                                int baseDamage = 60;
                                if (MasochistSoul)
                                    baseDamage *= 3;
                                Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<ExplosionSmall>(), baseDamage * 2, 12f, player.whoAmI);
                                y -= 2;
                                for (int i = -3; i <= 3; i++)
                                {
                                    if (i == 0)
                                        continue;
                                    int tilePosX = x + 16 * i;
                                    int tilePosY = y;
                                    if (Main.tile[tilePosX, tilePosY] != null && tilePosX >= 0 && tilePosX < Main.maxTilesX)
                                    {
                                        while (Main.tile[tilePosX, tilePosY] != null && tilePosY >= 0 && tilePosY < Main.maxTilesY
                                            && !(Main.tile[tilePosX, tilePosY].nactive() && Main.tileSolid[Main.tile[tilePosX, tilePosY].type]))
                                        {
                                            tilePosY++;
                                        }
                                        Projectile.NewProjectile(tilePosX * 16 + 8, tilePosY * 16 + 8, 0f, -8f, ModContent.ProjectileType<GeyserFriendly>(), baseDamage, 8f, player.whoAmI);
                                    }
                                }
                            }
                        }
                        
                    }
                    else
                    {
                        player.maxFallSpeed = 15f;
                        GroundPound++;
                    }
                }
            }

            //horizontal dash
            if (MonkDashing > 0)
            {
                MonkDashing--;

                //no loss of height
                //player.maxFallSpeed = 0f;
                //player.fallStart = (int)(player.position.Y / 16f);
                //player.gravity = 0f;
                //player.position.Y = player.oldPosition.Y;
                

                //Main.NewText(MonkDashing);

                /*if (MonkDashing == 0)
                {
                    player.velocity *= 0f;
                    player.dashDelay = 0;
                }*/
            }
            //vertical dash
            else if (MonkDashing < 0)
            {
                MonkDashing++;

                player.immune = true;
                player.maxFallSpeed *= 30f;
                player.gravity = 1.5f;

                /*if (MonkDashing == 0)
                {
                    player.velocity *= 0.5f;
                    player.dashDelay = 0;
                }*/
            }
        }

        public override void PostUpdateBuffs()
        {
            if (FargoSoulsWorld.EternityMode)
            {
                player.pickSpeed -= 0.25f;

                player.tileSpeed += 0.25f;
                player.wallSpeed += 0.25f;

                player.moveSpeed += 0.25f;
                
                player.manaRegenDelay = Math.Min(player.manaRegenDelay, 30);
                player.manaRegenBonus += 5;

                player.wellFed = true; //no longer expert half regen unless fed
            }
        }

        public void BionomicPassiveEffect()
        {
            player.buffImmune[BuffID.WindPushed] = true;
            player.buffImmune[BuffID.Suffocation] = true;
            player.manaFlower = true;
            player.nightVision = true;
            SandsofTime = true;
            SecurityWallet = true;
            TribalCharm = true;
            NymphsPerfumeRespawn = true;
            if (player.GetToggleValue("MasoCarrot", false))
                player.scope = true;
        }

        public override void PostUpdateEquips()
        {
            player.wingTimeMax = (int)(player.wingTimeMax * wingTimeModifier);

            if (player.armor.Any(i => i.active && (i.type == ModContent.ItemType<BionomicCluster>() || i.type == ModContent.ItemType<MasochistSoul>())))
                BionomicPassiveEffect();

            if (noDodge)
            {
                player.onHitDodge = false;
                player.shadowDodge = false;
                player.blackBelt = false;
            }

            if (FargoSoulsWorld.EternityMode && player.iceBarrier)
                player.endurance -= 0.1f;

            if (player.setSquireT2 || player.setSquireT3 || player.setMonkT2 || player.setMonkT3 || player.setHuntressT2 || player.setHuntressT3 || player.setApprenticeT2 || player.setApprenticeT3 || player.setForbidden)
                ReduceMasomodeMinionNerf = true;

            if (SquireEnchant)
                player.setSquireT2 = true;

            if (ValhallaEnchant)
                player.setSquireT3 = true;

            if (ApprenticeEnchant)
                player.setApprenticeT2 = true;

            if (DarkEnchant)
                player.setApprenticeT3 = true;

            if (HuntressEnchant)
                player.setHuntressT2 = true;

            if (RedEnchant)
                player.setHuntressT3 = true;

            if (MonkEnchant)
                player.setMonkT2 = true;

            if (ShinobiEnchant)
                player.setMonkT3 = true;
        }

        public override void PostUpdateMiscEffects()
        {
            //disable minion nerf during ooa
            if (DD2Event.Ongoing && !FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.betsyBoss, NPCID.DD2Betsy))
            {
                int n = NPC.FindFirstNPC(NPCID.DD2EterniaCrystal);
                if (n != Main.maxNPCs && player.Distance(Main.npc[n].Center) < 3000)
                {
                    MasomodeMinionNerfTimer -= 2;
                    if (MasomodeMinionNerfTimer < 0)
                        MasomodeMinionNerfTimer = 0;
                }
            }

            if (MasomodeWeaponUseTimer > 0)
            {
                MasomodeWeaponUseTimer -= 1;
                MasomodeMinionNerfTimer += 1;
            }
            else
            {
                if (MasomodeMinionNerfTimer > 0)
                    MasomodeMinionNerfTimer -= 1;
            }

            if (MasomodeMinionNerfTimer > MaxMasomodeMinionNerfTimer)
                MasomodeMinionNerfTimer = MaxMasomodeMinionNerfTimer;

            if (dashCD > 0)
                dashCD--;

            if (ReallyAwfulDebuffCooldown > 0)
                ReallyAwfulDebuffCooldown--;

            if (IronDebuffImmuneTime > 0)
                IronDebuffImmuneTime--;

            if (OceanicMaul && FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.fishBossEX, NPCID.DukeFishron))
            {
                player.statLifeMax2 /= 5;
                if (player.statLifeMax2 < 100)
                    player.statLifeMax2 = 100;
            }

            if (GaiaOffense && !GaiaSet)
                GaiaOffense = false;

            if (QueenStinger && QueenStingerCD > 0)
            {
                QueenStingerCD--;
            }

            if (SpiderEnchant)
            {
                SummonCrit += LifeForce ? 30 : 15;
                if (TerrariaSoul)
                {
                    SummonCrit = Math.Max(SummonCrit, player.meleeCrit);
                    SummonCrit = Math.Max(SummonCrit, player.rangedCrit);
                    SummonCrit = Math.Max(SummonCrit, player.magicCrit);
                }
            }

            if (RabiesVaccine)
            {
                player.buffImmune[BuffID.Rabies] = true;
            }

            if (StealingCooldown > 0 && !player.dead)
                StealingCooldown--;

            if (LihzahrdCurse)
            {
                player.dangerSense = false;
                player.InfoAccMechShowWires = false;
            }

            /*if ((player.HeldItem.type == ItemID.WireCutter || player.HeldItem.type == ItemID.WireKite)
                && (LihzahrdCurse || !player.buffImmune[ModContent.BuffType<LihzahrdCurse>()])
                && (Framing.GetTileSafely(player.Center).wall == WallID.LihzahrdBrickUnsafe
                || Framing.GetTileSafely(Main.MouseWorld).wall == WallID.LihzahrdBrickUnsafe))
                player.controlUseItem = false;*/

            if (Solar)
            {
                if (!player.setSolar && !TerrariaSoul) //nerf DR
                {
                    player.endurance -= 0.15f;
                }

                player.AddBuff(BuffID.SolarShield3, 5, false);
                player.setSolar = true;
                player.solarCounter++;
                int solarCD = 240;
                if (player.solarCounter >= solarCD)
                {
                    if (player.solarShields > 0 && player.solarShields < 3)
                    {
                        for (int i = 0; i < 22; i++)
                        {
                            if (player.buffType[i] >= BuffID.SolarShield1 && player.buffType[i] <= BuffID.SolarShield2)
                            {
                                player.DelBuff(i);
                            }
                        }
                    }
                    if (player.solarShields < 3)
                    {
                        player.AddBuff(BuffID.SolarShield1 + player.solarShields, 5, false);
                        for (int i = 0; i < 16; i++)
                        {
                            Dust dust = Main.dust[Dust.NewDust(player.position, player.width, player.height, 6, 0f, 0f, 100)];
                            dust.noGravity = true;
                            dust.scale = 1.7f;
                            dust.fadeIn = 0.5f;
                            dust.velocity *= 5f;
                        }
                        player.solarCounter = 0;
                    }
                    else
                    {
                        player.solarCounter = solarCD;
                    }
                }
                for (int i = player.solarShields; i < 3; i++)
                {
                    player.solarShieldPos[i] = Vector2.Zero;
                }
                for (int i = 0; i < player.solarShields; i++)
                {
                    player.solarShieldPos[i] += player.solarShieldVel[i];
                    Vector2 value = (player.miscCounter / 100f * 6.28318548f + i * (6.28318548f / player.solarShields)).ToRotationVector2() * 6f;
                    value.X = player.direction * 20;
                    player.solarShieldVel[i] = (value - player.solarShieldPos[i]) * 0.2f;
                }
                if (player.dashDelay >= 0)
                {
                    player.solarDashing = false;
                    player.solarDashConsumedFlare = false;
                }
                bool flag = player.solarDashing && player.dashDelay < 0;
                if (player.solarShields > 0 || flag)
                {
                    player.dash = 3;
                }
            }

            if (Nebula && !player.setNebula)
            {
                player.setNebula = true;

                if (player.nebulaCD > 0)
                    player.nebulaCD--;

                if (!TerrariaSoul && !CosmoForce) //cap boosters
                {
                    void DecrementBuff(int buffType)
                    {
                        for (int i = 0; i < player.buffType.Length; i++)
                        {
                            if (player.buffType[i] == buffType && player.buffTime[i] > 3)
                            {
                                player.buffTime[i] = 3;
                                break;
                            }
                        }
                    };

                    if (player.nebulaLevelDamage == 3)
                        DecrementBuff(BuffID.NebulaUpDmg3);
                    if (player.nebulaLevelLife == 3)
                        DecrementBuff(BuffID.NebulaUpLife3);
                    if (player.nebulaLevelMana == 3)
                        DecrementBuff(BuffID.NebulaUpMana3);
                }
            }

            if (CyclonicFin)
            {
                if (AbominableWandRevived) //has been revived already
                {
                    if (player.statLife >= player.statLifeMax2) //can revive again
                    {
                        AbominableWandRevived = false;

                        Main.PlaySound(SoundID.Item28, player.Center);

                        const int max = 50; //make some indicator dusts
                        for (int i = 0; i < max; i++)
                        {
                            Vector2 vector6 = Vector2.UnitY * 8f;
                            vector6 = vector6.RotatedBy((i - (max / 2 - 1)) * 6.28318548f / max) + Main.LocalPlayer.Center;
                            Vector2 vector7 = vector6 - Main.LocalPlayer.Center;
                            int d = Dust.NewDust(vector6 + vector7, 0, 0, 87, 0f, 0f, 0, default(Color), 2f);
                            Main.dust[d].noGravity = true;
                            Main.dust[d].velocity = vector7;
                        }

                        for (int i = 0; i < 30; i++)
                        {
                            int d = Dust.NewDust(player.position, player.width, player.height, 87, 0f, 0f, 0, default(Color), 2.5f);
                            Main.dust[d].noGravity = true;
                            Main.dust[d].velocity *= 8f;
                        }
                    }
                    else //cannot currently revive
                    {
                        player.AddBuff(ModContent.BuffType<AbomCooldown>(), 2);
                    }
                }
            }

            if (Flipped && !player.gravControl)
            {
                player.gravControl = true;
                player.controlUp = false;
                player.gravDir = -1f;
                //player.fallStart = (int)(player.position.Y / 16f);
                //player.jump = 0;
            }

            if (DevianttHearts)
            {
                if (DevianttHeartsCD > 0)
                    DevianttHeartsCD--;
            }

            if (Graze && ++GrazeCounter > 60) //decrease graze bonus over time
            {
                GrazeCounter = 0;
                if (GrazeBonus > 0f)
                    GrazeBonus -= 0.01;
            }

            if (LowGround)
            {
                player.waterWalk = false;
                player.waterWalk2 = false;
            }

            if (BetsysHeart && BetsyDashCD > 0)
            {
                BetsyDashCD--;
                if (BetsyDashCD == 0)
                {
                    Main.PlaySound(SoundID.Item9, player.Center);

                    for (int i = 0; i < 30; i++)
                    {
                        int d = Dust.NewDust(player.position, player.width, player.height, 87, 0, 0, 0, default, 2.5f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].velocity *= 4f;
                    }
                }
            }

            if (GravityGlobeEX && player.GetToggleValue("MasoGrav2", false))
                player.gravity = Player.defaultGravity;

            if (TikiEnchant && player.GetToggleValue("Tiki"))
            {
                actualMinions = player.maxMinions;
                player.maxMinions = 999;

                if (player.slotsMinions >= actualMinions)
                    TikiMinion = true;

                actualSentries = player.maxTurrets;
                player.maxTurrets = 999;

                if (getNumSentries() >= actualSentries)
                    TikiSentry = true;
                
            }

            if (NinjaEnchant)
            {
                for (int j = 0; j < player.inventory.Length; j++)
                {
                    if (hasSmokeBomb)
                    {
                        break;
                    }

                    Item item = player.inventory[j];

                    if (item.type == ItemID.SmokeBomb)
                    {
                        hasSmokeBomb = true;
                        smokeBombSlot = j;
                    }
                }

                if (smokeBombCD != 0)
                {
                    smokeBombCD--;
                }
            }

            if (JungleEnchant)
            {
                JungleEffect();
            }

            if (Atrophied)
            {
                player.meleeDamage = 0.01f;
                player.meleeCrit = 0;
            }

            if (GuttedHeart && player.whoAmI == Main.myPlayer)
            {
                //player.statLifeMax2 += player.statLifeMax / 10;
                GuttedHeartCD--;

                if (player.velocity == Vector2.Zero && player.itemAnimation == 0)
                    GuttedHeartCD--;

                if (GuttedHeartCD <= 0)
                {
                    GuttedHeartCD = 900;
                    if (player.GetToggleValue("MasoBrain"))
                    {
                        int count = 0;
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<CreeperGutted>() && Main.npc[i].ai[0] == player.whoAmI)
                                count++;
                        }
                        if (count < 5)
                        {
                            int multiplier = 1;
                            if (PureHeart)
                                multiplier = 2;
                            if (MasochistSoul)
                                multiplier = 5;
                            if (Main.netMode == NetmodeID.SinglePlayer)
                            {
                                int n = NPC.NewNPC((int)player.Center.X, (int)player.Center.Y, ModContent.NPCType<CreeperGutted>(), 0, player.whoAmI, 0f, multiplier);
                                if (n != Main.maxNPCs)
                                    Main.npc[n].velocity = Vector2.UnitX.RotatedByRandom(2 * Math.PI) * 8;
                            }
                            else if (Main.netMode == NetmodeID.MultiplayerClient)
                            {
                                var netMessage = mod.GetPacket();
                                netMessage.Write((byte)0);
                                netMessage.Write((byte)player.whoAmI);
                                netMessage.Write((byte)multiplier);
                                netMessage.Send();
                            }
                        }
                        else
                        {
                            int lowestHealth = -1;
                            for (int i = 0; i < Main.maxNPCs; i++)
                            {
                                if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<CreeperGutted>() && Main.npc[i].ai[0] == player.whoAmI)
                                {
                                    if (lowestHealth < 0)
                                        lowestHealth = i;
                                    else if (Main.npc[i].life < Main.npc[lowestHealth].life)
                                        lowestHealth = i;
                                }
                            }
                            if (Main.npc[lowestHealth].life < Main.npc[lowestHealth].lifeMax)
                            {
                                if (Main.netMode == NetmodeID.SinglePlayer)
                                {
                                    int damage = Main.npc[lowestHealth].lifeMax - Main.npc[lowestHealth].life;
                                    Main.npc[lowestHealth].life = Main.npc[lowestHealth].lifeMax;
                                    CombatText.NewText(Main.npc[lowestHealth].Hitbox, CombatText.HealLife, damage);
                                }
                                else if (Main.netMode == NetmodeID.MultiplayerClient)
                                {
                                    var netMessage = mod.GetPacket();
                                    netMessage.Write((byte)11);
                                    netMessage.Write((byte)player.whoAmI);
                                    netMessage.Write((byte)lowestHealth);
                                    netMessage.Send();
                                }
                            }
                        }
                    }
                }
            }

            //additive with gutted heart
            //if (PureHeart) player.statLifeMax2 += player.statLifeMax / 10;

            if (Slimed)
            {
                //slowed effect
                player.moveSpeed *= .7f;
                player.jump /= 2;
            }

            if (GodEater)
            {
                player.statDefense = 0;
                player.endurance = 0;
            }

            if (MutantNibble) //disables lifesteal, mostly
            {
                if (player.statLife > 0 && StatLifePrevious > 0 && player.statLife > StatLifePrevious)
                    player.statLife = StatLifePrevious;
                if (player.potionDelay < 2)
                    player.potionDelay = 2;
            }

            if (Defenseless)
            {
                player.statDefense -= 30;
                player.endurance = 0;
                player.longInvince = false;
                player.noKnockback = false;
            }

            if (Purified)
            {
                KillPets();

                //removes all buffs/debuffs, but it interacts really weirdly with luiafk infinite potions.
                for (int i = 21; i >= 0; i--)
                {
                    if (player.buffType[i] > 0 && player.buffTime[i] > 0 && !Main.debuff[player.buffType[i]])
                        player.DelBuff(i);
                }
            }
            else if (Asocial)
            {
                KillPets();
                player.maxMinions = 0;
                player.maxTurrets = 0;
                player.minionDamage -= .5f;
            }
            else if (WasAsocial) //should only occur when above debuffs end
            {
                player.hideMisc[0] = HidePetToggle0;
                player.hideMisc[1] = HidePetToggle1;

                WasAsocial = false;
            }

            if (Rotting)
            {
                player.moveSpeed *= 0.9f;
                //player.statLifeMax2 -= player.statLifeMax / 5;
                player.statDefense -= 10;
                player.endurance -= 0.1f;
                AttackSpeed -= 0.1f;
                AllDamageUp(-.1f);
            }

            if (Kneecapped)
            {
                player.accRunSpeed = player.maxRunSpeed;
            }

            if (OceanicMaul)
            {
                if (MaxLifeReduction > player.statLifeMax2 - 100)
                    MaxLifeReduction = player.statLifeMax2 - 100;
                player.statLifeMax2 -= MaxLifeReduction;
                //if (player.statLife > player.statLifeMax2) player.statLife = player.statLifeMax2;
            }
            else
            {
                MaxLifeReduction = 0;
            }

            if (Eternity)
                player.statManaMax2 = 999;
            else if (UniverseEffect)
                player.statManaMax2 += 300;

            Item heldItem = player.HeldItem;
            if (Toggler != null && TungstenEnchant && player.GetToggleValue("Tungsten"))
            {
                if (heldItem.damage > 0 && heldItem.scale < 2.5f)
                {
                    tungstenPrevSizeSave = heldItem.scale;
                    heldItem.scale = 2.5f;
                }
            }
            else if (((Toggler != null && !player.GetToggleValue("Tungsten", false)) || !TungstenEnchant) && tungstenPrevSizeSave != -1)
            {
                heldItem.scale = tungstenPrevSizeSave;
            }

            if (AdditionalAttacks && AdditionalAttacksTimer > 0)
                AdditionalAttacksTimer--;

            if (WoodEnchant && CritterAttackTimer > 0)
            {
                CritterAttackTimer--;
            }

            if (player.whoAmI == Main.myPlayer && player.controlUseItem && player.HeldItem.type == ModContent.ItemType<EaterLauncher>())
            {
                for (int i = 0; i < 20; i++)
                {
                    Vector2 offset = new Vector2();
                    double angle = Main.rand.NextDouble() * 2d * Math.PI;
                    offset.X += (float)(Math.Sin(angle) * 300);
                    offset.Y += (float)(Math.Cos(angle) * 300);
                    Dust dust = Main.dust[Dust.NewDust(
                        player.Center + offset - new Vector2(4, 4), 0, 0,
                        DustID.PurpleCrystalShard, 0, 0, 100, Color.White, 1f
                        )];
                    dust.velocity = player.velocity;
                    if (Main.rand.NextBool(3))
                        dust.velocity += Vector2.Normalize(offset) * 5f;
                    dust.noGravity = true;

                    Vector2 offset2 = new Vector2();
                    double angle2 = Main.rand.NextDouble() * 2d * Math.PI;
                    offset2.X += (float)(Math.Sin(angle2) * 500);
                    offset2.Y += (float)(Math.Cos(angle2) * 500);
                    Dust dust2 = Main.dust[Dust.NewDust(
                        player.Center + offset2 - new Vector2(4, 4), 0, 0,
                        DustID.PurpleCrystalShard, 0, 0, 100, Color.White, 1f
                        )];
                    dust2.velocity = player.velocity;
                    if (Main.rand.NextBool(3))
                        dust2.velocity += Vector2.Normalize(offset2) * -5f;
                    dust2.noGravity = true;
                }
            }
            
            if (MutantPresence)
            {
                player.statDefense /= 2;
                player.endurance /= 2;
                player.shinyStone = false;
            }

            /*if (DevianttPresence)
            {
                player.statDefense /= 2;
                player.endurance /= 2;
            }*/

            if (TinEnchant)
            {
                TinCritMax = HighestCritChance() * 2;
                if (SpiderEnchant && TinCritMax < SummonCrit * 2)
                    TinCritMax = SummonCrit * 2;

                if (TinCrit > TinCritMax)
                    TinCrit = TinCritMax;
                AllCritEquals(TinCrit);

                if (Eternity)
                {
                    if (eternityDamage > 47.5f)
                        eternityDamage = 47.5f;
                    AllDamageUp(eternityDamage);
                    player.statDefense += (int)(eternityDamage * 100); //10 defense per .1 damage
                }
            }

            if (SlimyShield || LihzahrdTreasureBox)
            {
                //player.justJumped use this tbh
                if (SlimyShieldFalling) //landing
                {
                    if (player.velocity.Y < 0f)
                        SlimyShieldFalling = false;

                    if (player.velocity.Y == 0f)
                    {
                        SlimyShieldFalling = false;
                        if (player.whoAmI == Main.myPlayer && player.gravDir > 0)
                        {
                            if (SlimyShield && player.GetToggleValue("MasoSlime"))
                            {
                                Main.PlaySound(SoundID.Item21, player.Center);
                                Vector2 mouse = Main.MouseWorld;
                                int damage = 8;
                                if (SupremeDeathbringerFairy)
                                    damage = 16;
                                if (MasochistSoul)
                                    damage = 80;
                                damage = (int)(damage * player.meleeDamage);
                                for (int i = 0; i < 3; i++)
                                {
                                    Vector2 spawn = new Vector2(mouse.X + Main.rand.Next(-200, 201), mouse.Y - Main.rand.Next(600, 901));
                                    if (Collision.CanHitLine(mouse, 0, 0, spawn, 0, 0))
                                    {
                                        Vector2 speed = mouse - spawn;
                                        speed.Normalize();
                                        speed *= 10f;
                                        Projectile.NewProjectile(spawn, speed, ModContent.ProjectileType<SlimeBall>(), damage, 1f, Main.myPlayer);
                                    }
                                }
                            }

                            if (LihzahrdTreasureBox && player.GetToggleValue("MasoBoulder"))
                            {
                                int dam = 60;
                                if (MasochistSoul)
                                    dam *= 3;
                                for (int i = -5; i <= 5; i += 2)
                                {
                                    Projectile.NewProjectile(player.Center, -12f * Vector2.UnitY.RotatedBy(Math.PI / 2 / 6 * i),
                                        ModContent.ProjectileType<LihzahrdBoulderFriendly>(), (int)(dam * player.meleeDamage), 7.5f, player.whoAmI);
                                }
                            }
                        }
                    }
                }
                else if (player.velocity.Y > 3f)
                {
                    SlimyShieldFalling = true;
                }
            }

            if (AgitatingLens)
            {
                if (AgitatingLensCD++ > 15)
                {
                    AgitatingLensCD = 0;
                    if ((Math.Abs(player.velocity.X) >= 5 || Math.Abs(player.velocity.Y) >= 5) && player.whoAmI == Main.myPlayer && player.GetToggleValue("MasoEye"))
                    {
                        int damage = 12;
                        if (SupremeDeathbringerFairy)
                            damage = 24;
                        if (MasochistSoul)
                            damage = 60;
                        damage = (int)(damage * player.magicDamage);
                        int proj = Projectile.NewProjectile(player.Center, player.velocity * 0.1f, mod.ProjectileType("BloodScytheFriendly"), damage, 5f, player.whoAmI);

                    }
                }
            }

            if (WretchedPouch)
            {
                if (--WretchedPouchCD <= 0)
                {
                    WretchedPouchCD = 25;

                    if (player.whoAmI == Main.myPlayer && player.GetToggleValue("MasoPouch"))
                    {
                        NPC target = Main.npc.FirstOrDefault(n => n.active && n.Distance(player.Center) < 360 && n.CanBeChasedBy() && Collision.CanHit(player.position, player.width, player.height, n.position, n.width, n.height));
                        if (target != null)
                        {
                            Main.PlaySound(SoundID.Item103, player.Center);

                            int dam = 40;
                            if (MasochistSoul)
                                dam *= 3;
                            dam = (int)(dam * player.magicDamage);

                            void ShootTentacle(Vector2 baseVel, float variance, int aiMin, int aiMax)
                            {
                                Vector2 speed = baseVel.RotatedBy(variance * (Main.rand.NextDouble() - 0.5));
                                float ai0 = Main.rand.Next(aiMin, aiMax) * (1f / 1000f);
                                if (Main.rand.NextBool())
                                    ai0 *= -1f;
                                float ai1 = Main.rand.Next(aiMin, aiMax) * (1f / 1000f);
                                if (Main.rand.NextBool())
                                    ai1 *= -1f;
                                Projectile.NewProjectile(player.Center, speed, ModContent.ProjectileType<ShadowflameTentacle>(), dam, 3.75f, player.whoAmI, ai0, ai1);
                            };

                            Vector2 vel = 8f * player.DirectionTo(target.Center);
                            const int max = 6;
                            const float rotationOffset = MathHelper.TwoPi / max;
                            for (int i = 0; i < 3; i++) //shoot right at them
                                ShootTentacle(vel, rotationOffset, 60, 90);
                            for (int i = 0; i < 6; i++) //shoot everywhere
                                ShootTentacle(vel.RotatedBy(rotationOffset * i), rotationOffset, 30, 50);
                        }
                    }
                }
            }

            if (PalladEnchant)
            {
                int increment = player.statLife - StatLifePrevious;
                if (increment > 0)
                {
                    PalladCounter += increment;
                    if (PalladCounter > 80)
                    {
                        PalladCounter = 0;
                        if (player.whoAmI == Main.myPlayer && player.statLife < player.statLifeMax2 && player.GetToggleValue("PalladiumOrb"))
                        {
                            int damage = EarthForce ? 80 : 40;
                            Projectile.NewProjectile(player.Center, -Vector2.UnitY, ModContent.ProjectileType<PalladOrb>(),
                                HighestDamageTypeScaling(damage), 10f, player.whoAmI, -1);
                        }
                    }
                }
            }

            StatLifePrevious = player.statLife;
        }

        public override float UseTimeMultiplier(Item item)
        {
            int useTime = item.useTime;
            int useAnimate = item.useAnimation;

            if (useTime == 0 || useAnimate == 0 || item.damage <= 0)
            {
                return 1f;
            }

            /*if (RangedEssence && item.ranged)
            {
                AttackSpeed += .05f;
            }

            if (RangedSoul && item.ranged)
            {
                AttackSpeed += .2f;
            }*/

            if (Berserked)
            {
                AttackSpeed += .1f;
            }

            if (MagicSoul && item.magic)
            {
                AttackSpeed += .2f;
            }

            if (ThrowSoul && item.thrown)
            {
                AttackSpeed += .2f;
            }

            if (item.summon && (TikiMinion || TikiSentry))
            {
                AttackSpeed *= 0.75f;
            }

            //checks so weapons dont break
            while (useTime / AttackSpeed < 1)
            {
                AttackSpeed -= .1f;
            }

            while (useAnimate / AttackSpeed < 3)
            {
                AttackSpeed -= .1f;
            }

            return AttackSpeed;
        }

        public override void UpdateBadLifeRegen()
        {
            if (NanoInjection)
            {
                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;

                player.lifeRegenTime = 0;
                player.lifeRegen -= 10;
            }

            if (Shadowflame)
            {
                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;

                player.lifeRegenTime = 0;
                player.lifeRegen -= 10;
            }

            if (GodEater)
            {
                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;
                player.lifeRegen -= 170;

                player.lifeRegenTime = 0;

                if (player.lifeRegenCount > 0)
                    player.lifeRegenCount = 0;

                player.lifeRegenCount -= 70;
            }

            if (MutantNibble)
            {
                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;

                if (player.lifeRegenCount > 0)
                    player.lifeRegenCount = 0;

                player.lifeRegenTime = 0;
            }

            if (Infested)
            {
                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;

                player.lifeRegenTime = 0;
                player.lifeRegen -= InfestedExtraDot();
            }

            if (Rotting)
            {
                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;

                player.lifeRegenTime = 0;
                player.lifeRegen -= 2;
            }

            if (CurseoftheMoon)
            {
                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;

                if (player.lifeRegenCount > 0)
                    player.lifeRegenCount = 0;

                player.lifeRegenTime = 0;
                player.lifeRegen -= 20;
            }

            if (Oiled && player.lifeRegen < 0)
            {
                player.lifeRegen *= 2;
            }

            if (MutantPresence)
            {
                if (player.lifeRegen > 5)
                    player.lifeRegen = 5;
            }

            if (AbomRebirth)
            {
                player.lifeRegen = 0;
                player.lifeRegenCount = 0;
                player.lifeRegenTime = 0;
            }

            if (FlamesoftheUniverse)
            {
                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;

                if (player.lifeRegenCount > 0)
                    player.lifeRegenCount = 0;

                player.lifeRegenTime = 0;
                player.lifeRegen -= 30 + 50 + 48 + 30;
            }
        }

        public override void DrawEffects(PlayerDrawInfo drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (squireReduceIframes && (SquireEnchant || ValhallaEnchant))
            {
                if (Main.rand.NextBool(3))
                {
                    float scale = ValhallaEnchant ? 2f : 1.5f;
                    int type = ValhallaEnchant ? 87 : 91;
                    int dust = Dust.NewDust(player.position, player.width, player.height, type, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 87, default(Color), scale);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    if (Main.rand.NextBool(4))
                    {
                        Main.dust[dust].noGravity = false;
                        Main.dust[dust].scale *= 0.5f;
                    }
                    Main.playerDrawDust.Add(dust);
                }
                fullBright = true;
            }

            if (Shadowflame)
            {
                if (Main.rand.NextBool(4) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), player.width, player.height, DustID.Shadowflame, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 100, default(Color), 2f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    Main.playerDrawDust.Add(dust);
                }
                fullBright = true;
            }

            if (Rotting)
            {
                if (drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), player.width, player.height, DustID.Blood, player.velocity.X * 0.1f, player.velocity.Y * 0.1f, 0, default(Color), 2f);
                    Main.dust[dust].noGravity = Main.rand.NextBool();
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    Main.playerDrawDust.Add(dust);
                }
            }

            if (Purified)
            {
                if (drawInfo.shadow == 0f)
                {
                    int index2 = Dust.NewDust(player.position, player.width, player.height, 91, 0.0f, 0.0f, 100, default, 2.5f);
                    Dust dust = Main.dust[index2];
                    Main.dust[index2].velocity *= 2f;
                    Main.dust[index2].noGravity = true;
                    Main.playerDrawDust.Add(index2);
                }
            }

            if (Hexed)
            {
                if (Main.rand.NextBool(3) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), player.width, player.height, DustID.BubbleBlock, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 100, default(Color), 2.5f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 2f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    Main.dust[dust].color = Color.GreenYellow;
                    Main.playerDrawDust.Add(dust);
                }
                if (Main.rand.NextBool() && drawInfo.shadow == 0f)
                {
                    int index2 = Dust.NewDust(player.position, player.width, player.height, 106, 0.0f, 0.0f, 100, default, 2.5f);
                    Dust dust = Main.dust[index2];
                    Main.dust[index2].noGravity = true;
                    Main.playerDrawDust.Add(index2);
                }
                fullBright = true;
            }

            if (Infested)
            {
                if (Main.rand.NextBool(4) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), player.width, player.height, 44, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 100, default(Color), InfestedDust);
                    Main.dust[dust].noGravity = true;
                    //Main.dust[dust].velocity *= 1.8f;
                    // Main.dust[dust].velocity.Y -= 0.5f;
                    Main.playerDrawDust.Add(dust);
                }
                fullBright = true;
            }

            if (GodEater)
            {
                if (Main.rand.NextBool(3) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), player.width + 4, player.height + 4, 86, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 100, default(Color), 3f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.2f;
                    Main.dust[dust].velocity.Y -= 0.15f;
                    Main.playerDrawDust.Add(dust);
                }
                r *= 0.15f;
                g *= 0.03f;
                b *= 0.09f;
                fullBright = true;
            }

            if (FlamesoftheUniverse)
            {
                /*drawInfo.drawPlayer.onFire = true;
                drawInfo.drawPlayer.onFire2 = true;
                drawInfo.drawPlayer.onFrostBurn = true;
                drawInfo.drawPlayer.ichor = true;
                drawInfo.drawPlayer.burned = true;*/
                if (Main.rand.NextBool(4) && drawInfo.shadow == 0f)
                {
                    int d = Dust.NewDust(player.position, player.width, player.height, 21, player.velocity.X * 0.2f, player.velocity.Y * 0.2f, 100, new Color(50 * Main.rand.Next(6) + 5, 50 * Main.rand.Next(6) + 5, 50 * Main.rand.Next(6) + 5), 2.5f);
                    Main.dust[d].velocity.Y -= 1;
                    Main.dust[d].velocity *= 2f;
                    Main.dust[d].noGravity = true;
                    Main.playerDrawDust.Add(d);
                }
                fullBright = true;
            }

            if (CurseoftheMoon)
            {
                if (Main.rand.NextBool(5))
                {
                    int d = Dust.NewDust(player.Center, 0, 0, 229, player.velocity.X * 0.4f, player.velocity.Y * 0.4f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 3f;
                    Main.playerDrawDust.Add(d);
                }
                if (Main.rand.NextBool(5))
                {
                    int d = Dust.NewDust(player.position, player.width, player.height, 229, player.velocity.X * 0.4f, player.velocity.Y * 0.4f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity.Y -= 1f;
                    Main.dust[d].velocity *= 2f;
                    Main.playerDrawDust.Add(d);
                }
            }

            if (DeathMarked)
            {
                if (Main.rand.NextBool() && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), player.width, player.height, 109, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 0, default(Color), 1.5f);
                    Main.dust[dust].velocity.Y--;
                    if (Main.rand.Next(3) != 0)
                    {
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].scale += 0.5f;
                        Main.dust[dust].velocity *= 3f;
                        Main.dust[dust].velocity.Y -= 0.5f;
                    }
                    Main.playerDrawDust.Add(dust);
                }
                r *= 0.2f;
                g *= 0.2f;
                b *= 0.2f;
                fullBright = true;
            }

            if (Fused)
            {
                if (Main.rand.NextBool() && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.position + new Vector2(player.width / 2, player.height / 5), 0, 0, DustID.Fire, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 0, default(Color), 2f);
                    Main.dust[dust].velocity.Y -= 2f;
                    Main.dust[dust].velocity *= 2f;
                    if (Main.rand.NextBool(4))
                    {
                        Main.dust[dust].scale += 0.5f;
                        Main.dust[dust].noGravity = true;
                    }
                    Main.playerDrawDust.Add(dust);
                }
            }

            if (ForbiddenEnchant && drawInfo.shadow == 0f)
            {
                Color color12 = player.GetImmuneAlphaPure(Lighting.GetColor((int)(drawInfo.position.X + player.width * 0.5) / 16, (int)(drawInfo.position.Y + player.height * 0.5) / 16, Color.White), drawInfo.shadow);
                Color color21 = Color.Lerp(color12, value2: Color.White, 0.7f);

                Texture2D texture2D2 = Main.extraTexture[74];
                Texture2D texture = Main.glowMaskTexture[217];
                bool flag8 = !player.setForbiddenCooldownLocked;
                int num52 = (int)((player.miscCounter / 300f * 6.28318548f).ToRotationVector2().Y * 6f);
                float num53 = (player.miscCounter / 75f * 6.28318548f).ToRotationVector2().X * 4f;
                Color color22 = new Color(80, 70, 40, 0) * (num53 / 8f + 0.5f) * 0.8f;
                if (!flag8)
                {
                    num52 = 0;
                    num53 = 2f;
                    color22 = new Color(80, 70, 40, 0) * 0.3f;
                    color21 = color21.MultiplyRGB(new Color(0.5f, 0.5f, 1f));
                }
                Vector2 vector4 = new Vector2(((int)(drawInfo.position.X - Main.screenPosition.X - (player.bodyFrame.Width / 2) + (player.width / 2))), ((int)(drawInfo.position.Y - Main.screenPosition.Y + player.height - player.bodyFrame.Height + 4f))) + player.bodyPosition + new Vector2((player.bodyFrame.Width / 2), (player.bodyFrame.Height / 2));
                vector4 += new Vector2((float)(-(float)player.direction * 10), (float)(-20 + num52));
                DrawData value = new DrawData(texture2D2, vector4, null, color21, player.bodyRotation, texture2D2.Size() / 2f, 1f, drawInfo.spriteEffects, 0);

                int num6 = 0;
                if (player.dye[1] != null)
                {
                    num6 = player.dye[1].dye;
                }
                value.shader = num6;
                Main.playerDrawData.Add(value);
                for (float num54 = 0f; num54 < 4f; num54 += 1f)
                {
                    value = new DrawData(texture, vector4 + (num54 * 1.57079637f).ToRotationVector2() * num53, null, color22, player.bodyRotation, texture2D2.Size() / 2f, 1f, drawInfo.spriteEffects, 0);
                    Main.playerDrawData.Add(value);
                }
            }
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (proj.hostile)
                return;

            //reduce minion damage in emode if using a weapon, scales as you use weapons
            if (FargoSoulsUtil.IsMinionDamage(proj) && FargoSoulsWorld.EternityMode && MasomodeMinionNerfTimer > 0)
            {
                double modifier = ReduceMasomodeMinionNerf ? 0.5 : 0.75;
                modifier *= Math.Min((double)MasomodeMinionNerfTimer / MaxMasomodeMinionNerfTimer, 1.0);

                damage = (int)(damage * (1.0 - modifier));
            }

            if (apprenticeBonusDamage)
            {
                if (ShadowForce)
                {
                    damage = (int)(damage * 2.5f);
                }
                else
                {
                    damage = (int)(damage * 1.5f);
                }
                
                apprenticeBonusDamage = false;
                apprenticeSwitchReady = false;
                apprenticeCD = 0;

                //dust
                int dustId = Dust.NewDust(new Vector2(proj.position.X, proj.position.Y + 2f), proj.width, proj.height + 5, DustID.FlameBurst, 0, 0, 100, Color.Black, 2f);
                Main.dust[dustId].noGravity = true;

                Projectile.NewProjectile(target.Center, Vector2.Zero, ProjectileID.InfernoFriendlyBlast, damage, 0, player.whoAmI);
            }


            if (HolyPrice)
                damage = (int)(0.75 * damage);

            if (Eternity)
            {
                if (crit)
                {
                    damage *= 5;
                }
            }
            else if (UniverseEffect)
            {
                if (crit)
                {
                    damage = (int)(damage * 2.5f);
                }
            }

            if (Hexed || (ReverseManaFlow && proj.magic))
            {
                target.life += damage;
                target.HealEffect(damage);

                if (target.life > target.lifeMax)
                {
                    target.life = target.lifeMax;
                }

                damage = 0;
                knockback = 0;
                crit = false;

                return;

            }

            if (SqueakyToy)
            {
                damage = 1;
                Squeak(target.Center);
                return;
            }

            if (FirstStrike)
            {
                crit = true;
                damage = (int)(damage * 1.5f);
                player.ClearBuff(ModContent.BuffType<FirstStrike>());
            }

            if (Asocial && FargoSoulsUtil.IsMinionDamage(proj))
            {
                damage = 0;
                knockback = 0;
                crit = false;
            }

            if ((proj.melee || proj.thrown) && Atrophied)
            {
                damage = 0;
                knockback = 0;
                crit = false;
            }
        }

        public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            if (HolyPrice)
                damage = (int)(2.0 / 3.0 * damage);

            if (Eternity)
            {
                if (crit)
                {
                    damage *= 5;
                }
            }
            else if (UniverseEffect)
            {
                if (crit)
                {
                    damage = (int)(damage * 2.5f);
                }
            }

            if (Hexed || (ReverseManaFlow && item.magic))
            {
                target.life += damage;
                target.HealEffect(damage);

                if (target.life > target.lifeMax)
                {
                    target.life = target.lifeMax;
                }

                damage = 0;
                knockback = 0;
                crit = false;

                return;

            }

            if (SqueakyToy)
            {
                damage = 1;
                Squeak(target.Center);
                return;
            }

            if (FirstStrike)
            {
                crit = true;
                damage = (int)(damage * 1.5f);
                player.ClearBuff(ModContent.BuffType<FirstStrike>());
            }

            if (Atrophied)
            {
                damage = 0;
                knockback = 0;
                crit = false;
            }

            if (TungstenEnchant && Toggler != null && player.GetToggleValue("Tungsten"))
            {
                damage = (int)(damage * 1.1f);

                if (!crit)
                {
                    crit = Main.rand.Next(0, 100) <= HighestCritChance();
                }
            }
        }

        public override void ModifyHitPvp(Item item, Player target, ref int damage, ref bool crit)
        {
            if (!SqueakyToy) return;
            damage = 1;
            Squeak(target.Center);
        }

        public override void ModifyHitPvpWithProj(Projectile proj, Player target, ref int damage, ref bool crit)
        {
            if (!SqueakyToy) return;
            damage = 1;
            Squeak(target.Center);
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            if (target.type == NPCID.TargetDummy || target.friendly)
                return;

            if (proj.minion && proj.type != ModContent.ProjectileType<CelestialRuneAncientVision>() && proj.type != ModContent.ProjectileType<SpookyScythe>())
                TryAdditionalAttacks(proj.damage, proj.melee, proj.ranged, proj.magic, proj.minion);

            OnHitNPCEither(target, damage, knockback, crit, projectile: proj);

            if (OriEnchant && proj.type == ProjectileID.FlowerPetal)
            {
                //int[] fireDebuffs = { BuffID.OnFire, BuffID.CursedInferno, BuffID.Frostburn, BuffID.ShadowFlame};
                //int debuff = Main.rand.Next(4);

                target.AddBuff(ModContent.BuffType<OriPoison>(), 300);
                target.immune[proj.owner] = 2;
            }

            if (PearlEnchant && player.GetToggleValue("Pearl") && pearlCD == 0 && proj.type != ProjectileID.HallowStar && proj.damage > 0)
            {
                //holy stars
                Main.PlaySound(SoundID.Item10, proj.position);
                for (int num479 = 0; num479 < 10; num479++)
                {
                    Dust.NewDust(proj.position, proj.width, proj.height, 58, proj.velocity.X * 0.1f, proj.velocity.Y * 0.1f, 150, default(Color), 1.2f);
                }
                for (int num480 = 0; num480 < 3; num480++)
                {
                    Gore.NewGore(proj.position, new Vector2(proj.velocity.X * 0.05f, proj.velocity.Y * 0.05f), Main.rand.Next(16, 18), 1f);
                }
                float x = proj.position.X + (float)Main.rand.Next(-400, 400);
                float y = proj.position.Y - (float)Main.rand.Next(600, 900);
                Vector2 vector12 = new Vector2(x, y);
                float num483 = proj.position.X + (float)(proj.width / 2) - vector12.X;
                float num484 = proj.position.Y + (float)(proj.height / 2) - vector12.Y;
                int num485 = 22;
                float num486 = (float)Math.Sqrt((double)(num483 * num483 + num484 * num484));
                num486 = (float)num485 / num486;
                num483 *= num486;
                num484 *= num486;
                int num487 = proj.damage;
                int num488 = Projectile.NewProjectile(x, y, num483, num484, ProjectileID.HallowStar, num487, proj.knockBack, proj.owner, 0f, 0f);
                if (num488 != 1000)
                    Main.projectile[num488].ai[1] = proj.position.Y;

                pearlCD = (WoodForce) ? 15 : 30;
            }
        }

        private void OnHitNPCEither(NPC target, int damage, float knockback, bool crit, Projectile projectile = null, Item item = null)
        {
            if (StyxSet)
            {
                StyxMeter += damage;
            }

            if (BeeEnchant && player.GetToggleValue("Bee") && beeCD <= 0 && target.realLife == -1
                && (projectile == null || (projectile.type != ProjectileID.Bee && projectile.type != ProjectileID.GiantBee && projectile.maxPenetrate != 1 && projectile.owner == Main.myPlayer)))
            {
                bool force = LifeForce;
                if (force || Main.rand.NextBool())
                {
                    int beeDamage = projectile != null ? projectile.damage : item != null ? item.damage : damage;
                    if (beeDamage > 0)
                    {
                        if (!TerrariaSoul)
                            beeDamage = Math.Min(beeDamage, HighestDamageTypeScaling(300));
                        float beeKB = projectile != null ? projectile.knockBack : item != null ? item.knockBack : knockback;
                        int p = Projectile.NewProjectile(target.Center.X, target.Center.Y, Main.rand.Next(-35, 36) * 0.2f, Main.rand.Next(-35, 36) * 0.2f,
                            force ? ProjectileID.GiantBee : player.beeType(), beeDamage, player.beeKB(beeKB), player.whoAmI);
                        if (p != Main.maxProjectiles)
                        {
                            if (projectile != null)
                            {
                                Main.projectile[p].melee = projectile.melee;
                                Main.projectile[p].ranged = projectile.ranged;
                                Main.projectile[p].magic = projectile.magic;
                                Main.projectile[p].minion = projectile.minion;
                            }
                            else if (item != null)
                            {
                                Main.projectile[p].melee = item.melee;
                                Main.projectile[p].ranged = item.ranged;
                                Main.projectile[p].magic = item.magic;
                                Main.projectile[p].minion = item.summon;
                            }
                        }
                    }
                    beeCD = 15;
                }
            }

            if (QueenStinger && QueenStingerCD <= 0 && player.GetToggleValue("MasoHoney"))
            {
                QueenStingerCD = SupremeDeathbringerFairy ? 300 : 600;

                for (int j = 0; j < 15; j++) //spray honey
                {
                    Projectile.NewProjectile(target.Center, new Vector2(Main.rand.NextFloat(-6, 6), Main.rand.NextFloat(-8, -5)), 
                        ModContent.ProjectileType<HoneyDrop>(), 0, 0f, Main.myPlayer);
                }
            }

            if (PalladEnchant && !player.onHitRegen)
            {
                player.AddBuff(BuffID.RapidHealing, Math.Min(300, damage / 3)); //heal time based on damage dealt, capped at 5sec
            }

            if (player.GetToggleValue("Copper") && CopperEnchant && copperCD == 0)
            {
                CopperEffect(target);
            }

            if (player.GetToggleValue("Obsidian") && ObsidianEnchant && obsidianCD == 0)
            {
                Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<ExplosionSmall>(), damage, 0, player.whoAmI);
                obsidianCD = 30;
            }

            if (player.GetToggleValue("Shade") && target.HasBuff(ModContent.BuffType<SuperBleed>()) && shadewoodCD == 0 && (projectile == null || projectile.type != ModContent.ProjectileType<SuperBlood>()) && player.whoAmI == Main.myPlayer)
            {
                for(int i = 0; i < Main.rand.Next(3, 6); i++)
                {
                    Projectile.NewProjectile(target.Center.X, target.Center.Y - 20, 0f + Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(-5, 5), ModContent.ProjectileType<SuperBlood>(), 20, 0f, Main.myPlayer);
                }

                if (WoodForce)
                {
                    target.AddBuff(BuffID.Ichor, 30);
                }

                shadewoodCD = 30;
            }

            if (DevianttHearts && DevianttHeartsCD <= 0 && player.GetToggleValue("MasoDevianttHearts") 
                && (projectile == null || (projectile.type != ModContent.ProjectileType<FriendRay>() && projectile.type != ModContent.ProjectileType<FriendHeart>())))
            {
                DevianttHeartsCD = CyclonicFin ? 300 : 600;

                if (Main.myPlayer == player.whoAmI)
                {
                    Vector2 offset = 300 * player.DirectionFrom(Main.MouseWorld);
                    for (int i = -3; i <= 3; i++)
                    {
                        Vector2 spawnPos = player.Center + offset.RotatedBy(Math.PI / 7 * i);
                        Vector2 speed = Vector2.Normalize(Main.MouseWorld - spawnPos);

                        int heartDamage = CyclonicFin ? 170 : 17;
                        heartDamage = (int)(heartDamage * player.minionDamage);

                        float ai1 = (Main.MouseWorld - spawnPos).Length() / 17;

                        if (MutantEye)
                            Projectile.NewProjectile(spawnPos, speed, ModContent.ProjectileType<FriendRay>(), heartDamage, 3f, player.whoAmI, (float)Math.PI / 7 * i);
                        else
                            Projectile.NewProjectile(spawnPos, 17f * speed, ModContent.ProjectileType<FriendHeart>(), heartDamage, 3f, player.whoAmI, -1, ai1);

                        FargoSoulsUtil.HeartDust(spawnPos, speed.ToRotation());
                    }
                }
            }

            if (GodEaterImbue)
            {
                if (target.FindBuffIndex(ModContent.BuffType<GodEater>()) < 0 && target.aiStyle != 37)
                {
                    if (target.type != ModContent.NPCType<MutantBoss>())
                    {
                        target.DelBuff(4);
                        target.buffImmune[ModContent.BuffType<GodEater>()] = false;
                    }
                    target.AddBuff(ModContent.BuffType<GodEater>(), 420);
                }
            }

            if (GladEnchant && player.whoAmI == Main.myPlayer && player.GetToggleValue("Gladiator") && gladCount <= 0 && (projectile == null || projectile.type != ModContent.ProjectileType<GladiatorJavelin>()))
            {
                gladCount = WillForce ? 30 : 60;

                int spearDamage = projectile != null ? projectile.damage : item != null ? item.damage : damage;
                spearDamage /= 2;

                if (spearDamage > 0)
                {
                    if (!TerrariaSoul)
                        spearDamage = Math.Min(spearDamage, HighestDamageTypeScaling(300));
                    for (int i = 0; i < 10; i++)
                    {
                        Vector2 spawn = new Vector2(target.Center.X + Main.rand.NextFloat(-400, 400), target.Center.Y - Main.rand.Next(600, 801));

                        Vector2 speed = target.Center + target.velocity * i * 5 * Main.rand.NextFloat(0.5f, 1.5f) - spawn;
                        speed.Normalize();
                        speed *= 15f * Main.rand.NextFloat(0.8f, 1.2f);

                        Projectile.NewProjectile(spawn, speed, ModContent.ProjectileType<GladiatorJavelin>(), spearDamage, 4f, Main.myPlayer);
                    }
                }
            }

            if (SolarEnchant && player.GetToggleValue("SolarFlare") && Main.rand.NextBool(4))
                target.AddBuff(ModContent.BuffType<SolarFlare>(), 300);

            if (tinCD <= 0)
            {
                if (Eternity)
                {
                    if (crit && TinCrit < 100)
                    {
                        TinCrit += 10;
                    }
                    else if (TinCrit >= 100)
                    {
                        if (damage / 10 > 0 && !player.moonLeech)
                        {
                            player.statLife += damage / 10;
                            player.HealEffect(damage / 10);
                            int max = MutantNibble ? StatLifePrevious : player.statLifeMax2;
                            if (player.statLife > max)
                                player.statLife = max;
                        }

                        if (player.GetToggleValue("Eternity", false))
                        {
                            eternityDamage += .05f;
                        }
                    }
                }
                else if (TerrariaSoul)
                {
                    if (crit && TinCrit < 100)
                    {
                        TinCrit += 5;
                        tinCD = 15;
                    }
                    else if (TinCrit >= 100)
                    {
                        if (HealTimer <= 0 && damage / 25 > 0)
                        {
                            if (!player.moonLeech)
                            {
                                player.statLife += damage / 25;
                                player.HealEffect(damage / 25);
                                int max = MutantNibble ? StatLifePrevious : player.statLifeMax2;
                                if (player.statLife > max)
                                    player.statLife = max;
                            }
                            HealTimer = 10;
                        }
                        else
                        {
                            HealTimer--;
                        }
                    }
                }
                else if (TinEnchant && crit && TinCrit < TinCritMax)
                {
                    if (TerraForce)
                    {
                        TinCrit += 5;
                        tinCD = 20;
                    }
                    else
                    {
                        TinCrit += 5;
                        tinCD = 30;
                    }

                    if (TinCrit > TinCritMax)
                        TinCrit = TinCritMax;
                }

                if (TinCrit > 100)
                    TinCrit = 100;
            }
            

            /*if (PalladEnchant && !TerrariaSoul && palladiumCD == 0 && !target.immortal && !player.moonLeech)
            {
                int heal = damage / 10;

                if ((EarthForce) && heal > 16)
                    heal = 16;
                else if (!EarthForce && !WizardEnchant && heal > 8)
                    heal = 8;
                else if (heal < 1)
                    heal = 1;
                player.statLife += heal;
                player.HealEffect(heal);
                palladiumCD = 240;
            }*/

            if (NymphsPerfume && NymphsPerfumeCD <= 0 && !target.immortal && !player.moonLeech)
            {
                NymphsPerfumeCD = 600;
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Item.NewItem(target.Hitbox, ItemID.Heart);
                }
                else if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    var netMessage = mod.GetPacket();
                    netMessage.Write((byte)9);
                    netMessage.Write((byte)target.whoAmI);
                    netMessage.Send();
                }
            }

            if (UniverseEffect)
                target.AddBuff(ModContent.BuffType<FlamesoftheUniverse>(), 240, true);

            if (MasochistSoul)
            {
                if (target.FindBuffIndex(ModContent.BuffType<Sadism>()) < 0 && target.aiStyle != 37)
                {
                    if (target.type != ModContent.NPCType<MutantBoss>())
                    {
                        target.DelBuff(4);
                        target.buffImmune[ModContent.BuffType<Sadism>()] = false;
                    }
                    target.AddBuff(ModContent.BuffType<Sadism>(), 600);
                }
            }
            else
            {
                if (BetsysHeart && crit)
                    target.AddBuff(BuffID.BetsysCurse, 300);

                if (PumpkingsCape && crit)
                    target.AddBuff(ModContent.BuffType<Rotting>(), 300);

                if (QueenStinger)
                    target.AddBuff(SupremeDeathbringerFairy ? BuffID.Venom : BuffID.Poisoned, 120, true);

                if (FusedLens)
                    target.AddBuff(Main.rand.NextBool() ? BuffID.CursedInferno : BuffID.Ichor, 360);
            }

            if (!TerrariaSoul)
            {
                if (AncientShadowEnchant && player.GetToggleValue("AncientShadow") && (projectile == null || projectile.type != ProjectileID.ShadowFlame) && Main.rand.NextBool(5))
                    target.AddBuff(BuffID.Darkness, 600, true);

                if (LeadEnchant && (Main.rand.NextBool(5) || TerraForce))
                    target.AddBuff(ModContent.BuffType<LeadPoison>(), 30);
            }

            if (GroundStick && Main.rand.NextBool(10) && player.GetToggleValue("MasoLightning"))
                target.AddBuff(ModContent.BuffType<LightningRod>(), 300);

            if (GoldEnchant)
                target.AddBuff(BuffID.Midas, 120, true);

            if (DragonFang && !target.boss && !target.buffImmune[ModContent.BuffType<ClippedWings>()] && Main.rand.NextBool(10))
            {
                target.velocity.X = 0f;
                target.velocity.Y = 10f;
                target.AddBuff(ModContent.BuffType<ClippedWings>(), 240);
                target.netUpdate = true;
            }

            if (SpectreEnchant && player.GetToggleValue("Spectre") && !target.immortal && Main.rand.NextBool())
            {
                if (projectile == null)
                {
                    //forced orb spawn reeeee
                    float num = 4f;
                    float speedX = Main.rand.Next(-100, 101);
                    float speedY = Main.rand.Next(-100, 101);
                    float num2 = (float)Math.Sqrt((double)(speedX * speedX + speedY * speedY));
                    num2 = num / num2;
                    speedX *= num2;
                    speedY *= num2;
                    Projectile p = FargoSoulsUtil.NewProjectileDirectSafe(target.position, new Vector2(speedX, speedY), ProjectileID.SpectreWrath, damage / 2, 0, player.whoAmI, target.whoAmI);

                    if ((SpiritForce || (crit && Main.rand.NextBool(5))) && p != null)
                    {
                        SpectreHeal(target, p);
                    }
                }
                else if (projectile.type != ProjectileID.SpectreWrath)
                {
                    SpectreHurt(projectile);

                    if (SpiritForce || (crit && Main.rand.NextBool(5)))
                    {
                        SpectreHeal(target, projectile);
                    }
                }
            }
            
            if (CyclonicFin)
            {
                //target.AddBuff(ModContent.BuffType<OceanicMaul>(), 900);
                //target.AddBuff(ModContent.BuffType<CurseoftheMoon>(), 900);
                if (crit && CyclonicFinCD <= 0 && player.GetToggleValue("MasoFishron"))
                {
                    CyclonicFinCD = 360;

                    float screenX = Main.screenPosition.X;
                    if (player.direction < 0)
                        screenX += Main.screenWidth;
                    float screenY = Main.screenPosition.Y;
                    screenY += Main.rand.Next(Main.screenHeight);
                    Vector2 spawn = new Vector2(screenX, screenY);
                    Vector2 vel = target.Center - spawn;
                    vel.Normalize();
                    vel *= 27f;

                    int dam = 150;
                    if (MutantEye)
                        dam *= 3;

                    int damageType = -1;
                    if (projectile == null)
                    {
                        dam = (int)(dam * player.meleeDamage);
                        damageType = 1;
                    }
                    else if (projectile.type != ModContent.ProjectileType<RazorbladeTyphoonFriendly>())
                    {
                        if (projectile.melee)
                        {
                            dam = (int)(dam * player.meleeDamage);
                            damageType = 1;
                        }
                        else if (projectile.ranged)
                        {
                            dam = (int)(dam * player.rangedDamage);
                            damageType = 2;
                        }
                        else if (projectile.magic)
                        {
                            dam = (int)(dam * player.magicDamage);
                            damageType = 3;
                        }
                        else if (FargoSoulsUtil.IsMinionDamage(projectile))
                        {
                            dam = (int)(dam * player.minionDamage);
                            damageType = 4;
                        }
                    }

                    if (damageType != -1)
                        Projectile.NewProjectile(spawn, vel, ModContent.ProjectileType<SpectralFishron>(), dam, 10f, player.whoAmI, target.whoAmI, damageType);
                }
            }

            if (CorruptHeart && CorruptHeartCD <= 0)
            {
                CorruptHeartCD = 60;
                if (player.GetToggleValue("MasoEater") && (projectile == null || projectile.type != ProjectileID.TinyEater))
                {
                    Main.PlaySound(SoundID.NPCHit, (int)player.Center.X, (int)player.Center.Y, 1, 1f, 0.0f);
                    for (int index1 = 0; index1 < 20; ++index1)
                    {
                        int index2 = Dust.NewDust(player.position, player.width, player.height, 184, 0.0f, 0.0f, 0, new Color(), 1f);
                        Dust dust = Main.dust[index2];
                        dust.scale = dust.scale * 1.1f;
                        Main.dust[index2].noGravity = true;
                    }
                    for (int index1 = 0; index1 < 30; ++index1)
                    {
                        int index2 = Dust.NewDust(player.position, player.width, player.height, 184, 0.0f, 0.0f, 0, new Color(), 1f);
                        Dust dust1 = Main.dust[index2];
                        dust1.velocity = dust1.velocity * 2.5f;
                        Dust dust2 = Main.dust[index2];
                        dust2.scale = dust2.scale * 0.8f;
                        Main.dust[index2].noGravity = true;
                    }
                    int num = 2;
                    if (Main.rand.NextBool(3))
                        ++num;
                    if (Main.rand.NextBool(6))
                        ++num;
                    if (Main.rand.NextBool(9))
                        ++num;
                    int dam = PureHeart ? 30 : 12;
                    if (MasochistSoul)
                        dam *= 2;
                    for (int index = 0; index < num; ++index)
                        Projectile.NewProjectile(player.Center.X, player.Center.Y, Main.rand.Next(-35, 36) * 0.02f * 10f,
                            Main.rand.Next(-35, 36) * 0.02f * 10f, ProjectileID.TinyEater, (int)(dam * player.meleeDamage), 1.75f, player.whoAmI);
                }
            }

            if (FrigidGemstone && FrigidGemstoneCD <= 0 && !target.immortal && (projectile == null || projectile.type != ModContent.ProjectileType<Shadowfrostfireball>()))
            {
                FrigidGemstoneCD = 30;
                float screenX = Main.screenPosition.X;
                if (player.direction < 0)
                    screenX += Main.screenWidth;
                float screenY = Main.screenPosition.Y;
                screenY += Main.rand.Next(Main.screenHeight);
                Vector2 spawn = new Vector2(screenX, screenY);
                Vector2 vel = target.Center - spawn;
                vel.Normalize();
                vel *= 8f;
                int dam = (int)(40 * player.magicDamage);
                if (MasochistSoul)
                    dam *= 2;
                Projectile.NewProjectile(spawn, vel, ModContent.ProjectileType<Shadowfrostfireball>(), dam, 6f, player.whoAmI, target.whoAmI);
            }
        }

        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
            if (target.type == NPCID.TargetDummy || target.friendly)
                return;

            OnHitNPCEither(target, damage, knockback, crit, item: item);
        }

        public override void MeleeEffects(Item item, Rectangle hitbox)
        {
            if (ShroomEnchant && player.GetToggleValue("ShroomiteShroom") && player.stealth == 0 && !item.noMelee && (player.itemAnimation == (int)((double)player.itemAnimationMax * 0.1) || player.itemAnimation == (int)((double)player.itemAnimationMax * 0.3) || player.itemAnimation == (int)((double)player.itemAnimationMax * 0.5) || player.itemAnimation == (int)((double)player.itemAnimationMax * 0.7) || player.itemAnimation == (int)((double)player.itemAnimationMax * 0.9)))
            {
                //hellish code from hammush
                float num340 = 0f;
                float num341 = 0f;
                float num342 = 0f;
                float num343 = 0f;
                if (player.itemAnimation == (int)((double)player.itemAnimationMax * 0.9))
                {
                    num340 = -7f;
                }
                if (player.itemAnimation == (int)((double)player.itemAnimationMax * 0.7))
                {
                    num340 = -6f;
                    num341 = 2f;
                }
                if (player.itemAnimation == (int)((double)player.itemAnimationMax * 0.5))
                {
                    num340 = -4f;
                    num341 = 4f;
                }
                if (player.itemAnimation == (int)((double)player.itemAnimationMax * 0.3))
                {
                    num340 = -2f;
                    num341 = 6f;
                }
                if (player.itemAnimation == (int)((double)player.itemAnimationMax * 0.1))
                {
                    num341 = 7f;
                }
                if (player.itemAnimation == (int)((double)player.itemAnimationMax * 0.7))
                {
                    num343 = 26f;
                }
                if (player.itemAnimation == (int)((double)player.itemAnimationMax * 0.3))
                {
                    num343 -= 4f;
                    num342 -= 20f;
                }
                if (player.itemAnimation == (int)((double)player.itemAnimationMax * 0.1))
                {
                    num342 += 6f;
                }
                if (player.direction == -1)
                {
                    if (player.itemAnimation == (int)((double)player.itemAnimationMax * 0.9))
                    {
                        num343 -= 8f;
                    }
                    if (player.itemAnimation == (int)((double)player.itemAnimationMax * 0.7))
                    {
                        num343 -= 6f;
                    }
                }
                num340 *= 1.5f;
                num341 *= 1.5f;
                num343 *= (float)player.direction;
                num342 *= player.gravDir;
                Projectile.NewProjectile((float)(hitbox.X + hitbox.Width / 2) + num343, (float)(hitbox.Y + hitbox.Height / 2) + num342, (float)player.direction * num341, num340 * player.gravDir, ModContent.ProjectileType<ShroomiteShroom>(), item.damage / 5, 0f, player.whoAmI, 0f, 0f);
            }
        }

        public override bool CanBeHitByNPC(NPC npc, ref int cooldownSlot)
        {
            if (BetsyDashing)
                return false;
            return true;
        }

        public override bool CanBeHitByProjectile(Projectile proj)
        {
            if (BetsyDashing)
                return false;
            return true;
        }

        public override void ModifyHitByNPC(NPC npc, ref int damage, ref bool crit)
        {
            if (npc.coldDamage && Hypothermia)
                damage = (int)(damage * 1.2);

            if (npc.GetGlobalNPC<FargoSoulsGlobalNPC>().CurseoftheMoon)
                damage = (int)(damage * 0.8);

            if (IronDebuffImmuneTime > 0 || BetsyDashing || GoldShell || player.HasBuff(ModContent.BuffType<ShellHide>()) || MonkDashing > 0)
            {
                foreach (int debuff in Fargowiltas.DebuffIDs) //immune to all debuffs
                {
                    if (!player.HasBuff(debuff))
                        player.buffImmune[debuff] = true;
                }
            }
        }

        public override void ModifyHitByProjectile(Projectile proj, ref int damage, ref bool crit)
        {
            if (proj.coldDamage && Hypothermia)
                damage = (int)(damage * 1.2);
            
            if (IronDebuffImmuneTime > 0 || BetsyDashing || GoldShell || player.HasBuff(ModContent.BuffType<ShellHide>()) || MonkDashing > 0)
            {
                foreach (int debuff in Fargowiltas.DebuffIDs) //immune to all debuffs
                {
                    if (!player.HasBuff(debuff))
                        player.buffImmune[debuff] = true;
                }
            }
        }

        public override void OnHitByNPC(NPC npc, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode && player.shadowDodge) //prehurt hook not called on titanium dodge
                player.AddBuff(ModContent.BuffType<HolyPrice>(), 600);
        }

        public override void OnHitByProjectile(Projectile proj, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode && player.shadowDodge) //prehurt hook not called on titanium dodge
                player.AddBuff(ModContent.BuffType<HolyPrice>(), 600);
        }

        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.deviBoss, ModContent.NPCType<NPCs.DeviBoss.DeviBoss>()))
            {
                ((NPCs.DeviBoss.DeviBoss)Main.npc[EModeGlobalNPC.deviBoss].modNPC).playerInvulTriggered = true;
            }

            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.abomBoss, ModContent.NPCType<NPCs.AbomBoss.AbomBoss>()))
            {
                ((NPCs.AbomBoss.AbomBoss)Main.npc[EModeGlobalNPC.abomBoss].modNPC).playerInvulTriggered = true;
            }

            if (IronGuard && ironShieldTimer > 0 && !player.immune)
            {
                player.immune = true;
                int invul = player.longInvince ? 90 : 60;
                player.immuneTime = invul;
                player.hurtCooldowns[0] = invul;
                player.hurtCooldowns[1] = invul;
                player.AddBuff(BuffID.ParryDamageBuff, 300);
                Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<IronParry>(), 0, 0f, Main.myPlayer);

                IronDebuffImmuneTime = invul;
                ironShieldCD = invul + 40;

                foreach (int debuff in Fargowiltas.DebuffIDs) //immune to all debuffs
                {
                    if (!player.HasBuff(debuff))
                        player.buffImmune[debuff] = true;
                }

                return false;
            }

            if (FargoSoulsWorld.EternityMode && FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.moonBoss, NPCID.MoonLordCore)
                && player.Distance(Main.npc[EModeGlobalNPC.moonBoss].Center) < 2500)
            {
                player.AddBuff(ModContent.BuffType<CurseoftheMoon>(), 180);
            }

            if (player.whoAmI == Main.myPlayer && SqueakyAcc && player.GetToggleValue("MasoSqueak") && Main.rand.NextBool(10))
            {
                Squeak(player.Center);
                damage = 1;
            }

            if (DeathMarked)
            {
                damage = (int)(damage * 1.5);
            }

            if (CrimsonEnchant && player.GetToggleValue("Crimson"))
            {
                //if was already healing, kill it
                if (player.HasBuff(ModContent.BuffType<CrimsonRegen>()))
                {
                    damage += CrimsonRegenSoFar;
                }
                else
                {
                    player.AddBuff(ModContent.BuffType<CrimsonRegen>(), 2);
                }

                CrimsonTotalToRegen = (damage - CrimsonRegenSoFar) / 2;

                if (NatureForce)
                {
                    CrimsonTotalToRegen *= 2;
                }

                CrimsonRegenSoFar = 0;
            }

            if (StyxSet && !BetsyDashing && !GoldShell && damage > 1 && player.ownedProjectileCounts[ModContent.ProjectileType<StyxArmorScythe>()] > 0)
            {
                int scythesSacrificed = 0;
                const int maxSacrifice = 4;
                const double maxDR = 0.2;
                int scytheType = ModContent.ProjectileType<StyxArmorScythe>();
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].type == scytheType && Main.projectile[i].owner == player.whoAmI)
                    {
                        if (player.whoAmI == Main.myPlayer)
                            Main.projectile[i].Kill();
                        if (++scythesSacrificed >= maxSacrifice)
                            break;
                    }
                }

                damage = (int)(damage * (1.0 - maxDR / maxSacrifice * scythesSacrificed));
            }

            return true;
        }

        public override void Hurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit)
        {
            WasHurtBySomething = true;

            if (MythrilEnchant && !TerrariaSoul)
            {
                player.AddBuff(ModContent.BuffType<DisruptedFocus>(), 300);
            }

            if(TinEnchant)
            {
                if (Eternity)
                {
                    TinCrit = 50;
                    eternityDamage = 0;
                }
                else if (TerrariaSoul && TinCrit != 20)
                {
                    TinCrit = 20;
                }
                else if((TerraForce) && TinCrit != 10)
                {
                    TinCrit = 10;
                }
                else if(TinCrit != 4)
                {
                    TinCrit = 4;
                }
            }

            if (ShellHide)
            {
                TurtleShellHP--;

                //some funny dust
                const int max = 30;
                for (int i = 0; i < max; i++)
                {
                    Vector2 vector6 = Vector2.UnitY * 5f;
                    vector6 = vector6.RotatedBy((i - (max / 2 - 1)) * 6.28318548f / max) + Main.LocalPlayer.Center;
                    Vector2 vector7 = vector6 - Main.LocalPlayer.Center;
                    int d = Dust.NewDust(vector6 + vector7, 0, 0, DustID.GoldFlame, 0f, 0f, 0, default(Color), 2f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity = vector7;
                }
            }

            if (HurtTimer <= 0)
            {
                HurtTimer = 20;

                if (MoonChalice)
                {
                    if (player.GetToggleValue("MasoVision"))
                    {
                        int dam = 50;
                        if (MasochistSoul)
                            dam *= 2;
                        for (int i = 0; i < 5; i++)
                            Projectile.NewProjectile(player.Center.X, player.Center.Y, Main.rand.Next(-10, 11), Main.rand.Next(-10, 11),
                                ModContent.ProjectileType<AncientVision>(), (int)(dam * player.minionDamage), 6f, player.whoAmI);
                    }
                }
                else if (CelestialRune && player.GetToggleValue("MasoVision"))
                {
                    Projectile.NewProjectile(player.Center, new Vector2(0, -10), ModContent.ProjectileType<AncientVision>(),
                        (int)(40 * player.minionDamage), 3f, player.whoAmI);
                }

                /*if (LihzahrdTreasureBox && SoulConfig.Instance.GetValue(SoulConfig.Instance.LihzahrdBoxSpikyBalls))
                {
                    int dam = 60;
                    if (MasochistSoul)
                        dam *= 2;
                    for (int i = 0; i < 9; i++)
                        Projectile.NewProjectile(player.Center.X, player.Center.Y, Main.rand.Next(-10, 11), Main.rand.Next(-10, 11),
                            ModContent.ProjectileType<LihzahrdSpikyBallFriendly>(), (int)(dam * player.meleeDamage), 2f, player.whoAmI);
                }*/

                if (MoltenEnchant && player.GetToggleValue("MoltenE") && player.whoAmI == Main.myPlayer/* && Main.netMode != NetModeID.MultiplayerClient*/)
                {
                    int baseDamage = 50;
                    int multiplier = 2;
                    int cap = 150;

                    if (NatureForce)
                    {
                        baseDamage = 50;
                        multiplier = 4;
                        cap = 250;
                    }

                    if (TerrariaSoul)
                    {
                        baseDamage = 250;
                        multiplier = 5;
                        cap = 500;
                    }

                    int explosionDamage = baseDamage + (int)damage * multiplier;
                    if (explosionDamage > cap)
                        explosionDamage = cap;

                    Projectile p = FargoSoulsUtil.NewProjectileDirectSafe(player.Center, Vector2.Zero, ModContent.ProjectileType<Explosion>(), (int)(explosionDamage * player.meleeDamage), 0f, Main.myPlayer);
                    if (p != null)
                        p.GetGlobalProjectile<FargoGlobalProjectile>().CanSplit = false;
                }

                if (FossilEnchant)
                {
                    player.immune = true;
                    player.immuneTime = 60;

                    if (player.GetToggleValue("Fossil"))
                    {
                        //spawn bones
                        int damageCopy = (int)damage;
                        for (int i = 0; i < 3; i++)
                        {
                            if (damageCopy < 30)
                                break;
                            damageCopy -= 30;

                            float velX = Main.rand.Next(-5, 6) * 3f;
                            float velY = Main.rand.Next(-5, 6) * 3f;
                            int p = Projectile.NewProjectile(player.position.X + velX, player.position.Y + velY, velX, velY, ModContent.ProjectileType<FossilBone>(), 0, 0f, player.whoAmI);
                        }
                    }
                }

                if (IceQueensCrown && player.GetToggleValue("IceQueensCrown"))
                {
                    int freezeRange = 16 * 20;
                    if (MasochistHeart)
                        freezeRange = 16 * 40;
                    if (MasochistSoul)
                        freezeRange = 16 * 60;

                    int freezeDuration = MasochistHeart || MasochistSoul ? 90 : 60;

                    foreach (NPC n in Main.npc.Where(n => n.active && !n.friendly && n.damage > 0 && player.Distance(FargoSoulsUtil.ClosestPointInHitbox(n, player.Center)) < freezeRange && !n.dontTakeDamage))
                    {
                        n.GetGlobalNPC<FargoSoulsGlobalNPC>().SnowChilled = true;
                        n.GetGlobalNPC<FargoSoulsGlobalNPC>().SnowChilledTimer = freezeDuration;
                        n.netUpdate = true;
                    }

                    foreach (Projectile p in Main.projectile.Where(p => p.active && p.hostile && p.damage > 0 && player.Distance(FargoSoulsUtil.ClosestPointInHitbox(p, player.Center)) < freezeRange && FargoSoulsUtil.CanDeleteProjectile(p)))
                    {
                        p.GetGlobalProjectile<FargoGlobalProjectile>().ChilledProj = true;
                        p.GetGlobalProjectile<FargoGlobalProjectile>().ChilledTimer = freezeDuration;
                        p.netUpdate = true;
                    }

                    for (int i = 0; i < 40; i++)
                    {
                        int d = Dust.NewDust(player.Center, 0, 0, 76, 0, 0, 100, Color.White, 2.5f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].velocity *= 6f;
                    }

                    for (int i = 0; i < 20; i++)
                    {
                        int d = Dust.NewDust(player.Center, 0, 0, 88, 0, 0, 0, default, 2f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].velocity *= 6f;
                    }
                }
            }

            if (Midas && Main.myPlayer == player.whoAmI)
                player.DropCoins();

            GrazeBonus = 0;
            GrazeCounter = 0;
        }

        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            bool retVal = true;

            if (player.statLife <= 0) //revives
            {
                /*if (MutantSetBonus && player.whoAmI == Main.myPlayer && retVal && player.FindBuffIndex(ModContent.BuffType<MutantRebirth>()) == -1)
                {
                    player.statLife = player.statLifeMax2;
                    player.HealEffect(player.statLifeMax2);
                    player.immune = true;
                    player.immuneTime = 180;
                    player.hurtCooldowns[0] = 180;
                    player.hurtCooldowns[1] = 180;
                    Main.NewText(Language.GetTextValue("Mods.FargowiltasSouls.FargoPlayer.Revive"), Color.LimeGreen);
                    player.AddBuff(ModContent.BuffType<MutantRebirth>(), 10800);
                    Projectile.NewProjectile(player.Center, -Vector2.UnitY, ModContent.ProjectileType<GiantDeathray>(), (int)(7000 * player.minionDamage), 10f, player.whoAmI);
                    retVal = false;
                }*/
                if (player.whoAmI == Main.myPlayer && retVal && AbomRebirth)
                {
                    if (!WasHurtBySomething)
                    {
                        player.statLife = 1;
                        //CombatText.NewText(player.Hitbox, Color.SandyBrown, Language.GetTextValue("Mods.FargowiltasSouls.FargoPlayer.Revive"));
                        return false; //this is deliberate
                    }
                }

                if (player.whoAmI == Main.myPlayer && retVal && player.FindBuffIndex(ModContent.BuffType<Revived>()) == -1)
                {
                    void Revive(int healAmount, int reviveCooldown)
                    {
                        player.statLife = healAmount;
                        player.HealEffect(healAmount);

                        player.immune = true;
                        player.immuneTime = 120;
                        player.hurtCooldowns[0] = 120;
                        player.hurtCooldowns[1] = 120;

                        CombatText.NewText(player.Hitbox, Color.SandyBrown, Language.GetTextValue("Mods.FargowiltasSouls.FargoPlayer.Revive"), true);
                        Main.NewText(Language.GetTextValue("Mods.FargowiltasSouls.FargoPlayer.Revive"), Color.SandyBrown);

                        player.AddBuff(ModContent.BuffType<Revived>(), reviveCooldown);
                        retVal = false;
                    };

                    if (Eternity)
                    {
                        Revive(player.statLifeMax2 / 2 > 200 ? player.statLifeMax2 / 2 : 200, 10800);
                        FargoSoulsUtil.XWay(30, player.Center, ModContent.ProjectileType<FossilBone>(), 15, 0, 0);
                    }
                    else if (TerrariaSoul)
                    {
                        Revive(200, 14400);
                        FargoSoulsUtil.XWay(25, player.Center, ModContent.ProjectileType<FossilBone>(), 15, 0, 0);
                    }
                    else if (FossilEnchant)
                    {
                        Revive(SpiritForce ? 50 : 1, 18000);
                        FargoSoulsUtil.XWay(SpiritForce ? 20 : 10, player.Center, ModContent.ProjectileType<FossilBone>(), 15, 0, 0);
                    }
                }

                if (player.whoAmI == Main.myPlayer && retVal && CyclonicFin && !AbominableWandRevived)
                {
                    AbominableWandRevived = true;
                    int heal = 1;
                    player.statLife = heal;
                    player.HealEffect(heal);
                    player.immune = true;
                    player.immuneTime = 120;
                    player.hurtCooldowns[0] = 120;
                    player.hurtCooldowns[1] = 120;
                    CombatText.NewText(player.Hitbox, Color.Yellow, Language.GetTextValue("Mods.FargowiltasSouls.FargoPlayer.Revive"), true);
                    Main.NewText(Language.GetTextValue("Mods.FargowiltasSouls.FargoPlayer.Revive"), Color.Yellow);
                    player.AddBuff(ModContent.BuffType<AbomRebirth>(), MutantEye ? 600 : 900);
                    retVal = false;
                    for (int i = 0; i < 24; i++)
                    {
                        Projectile.NewProjectile(player.Center, Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(4f, 16f),
                            ModContent.ProjectileType<StyxArmorScythe2>(), 0, 10f, Main.myPlayer, -60 - Main.rand.Next(60), -1);
                    }
                }
            }

            if (TinEnchant)
            {
                if (Eternity)
                {
                    TinCrit = 50;
                    eternityDamage = 0;
                }
                else if (TerrariaSoul)
                {
                    TinCrit = 20;
                }
                else if (TerraForce)
                {
                    TinCrit = 10;
                }
                else
                {
                    TinCrit = 4;
                }
            }

            //add more tbh
            if (Infested && damage == 10.0 && hitDirection == 0 && damageSource.SourceOtherIndex == 8)
            {
                damageSource = PlayerDeathReason.ByCustomReason(player.name + Language.GetTextValue("Mods.FargowiltasSouls.FargoPlayer.KilledByInfested"));
            }

            if (Rotting && damage == 10.0 && hitDirection == 0 && damageSource.SourceOtherIndex == 8)
            {
                damageSource = PlayerDeathReason.ByCustomReason(player.name + Language.GetTextValue("Mods.FargowiltasSouls.FargoPlayer.KilledByRotting"));
            }

            if ((GodEater || FlamesoftheUniverse || CurseoftheMoon) && damage == 10.0 && hitDirection == 0 && damageSource.SourceOtherIndex == 8)
            {
                damageSource = PlayerDeathReason.ByCustomReason(player.name + Language.GetTextValue("Mods.FargowiltasSouls.FargoPlayer.KilledByGodEater"));
            }

            if (DeathMarked)
            {
                damageSource = PlayerDeathReason.ByCustomReason(player.name + Language.GetTextValue("Mods.FargowiltasSouls.FargoPlayer.KilledByDeathMarked"));
            }

            /*if (MutantPresence)
            {
                damageSource = PlayerDeathReason.ByCustomReason(player.name + " was penetrated.");
            }*/

            if (StatLifePrevious > 0 && player.statLife > StatLifePrevious)
                StatLifePrevious = player.statLife;

            if (MutantSetBonus && player.whoAmI == Main.myPlayer && player.statLife > 0 && player.GetToggleValue("MasoReviveDeathray"))
            {
                player.immune = true;
                if (player.immuneTime < 180)
                    player.immuneTime = 180;
                if (player.hurtCooldowns[0] < 180)
                    player.hurtCooldowns[0] = 180;
                if (player.hurtCooldowns[1] < 180)
                    player.hurtCooldowns[1] = 180;
                Projectile.NewProjectile(player.Center, -Vector2.UnitY, ModContent.ProjectileType<GiantDeathray>(), (int)(7000 * player.minionDamage), 10f, player.whoAmI);
            }

            return retVal;
        }

        public override void ModifyDrawInfo(ref PlayerDrawInfo drawInfo)
        {
            if (GaiaOffense)
            {
                int gaiaShader = GameShaders.Armor.GetShaderIdFromItemId(mod.ItemType("GaiaDye")); //set armor and accessory shaders to gaia shader if set bonus is triggered
                drawInfo.bodyArmorShader = gaiaShader;
                drawInfo.headArmorShader = gaiaShader;
                drawInfo.legArmorShader = gaiaShader;
                drawInfo.wingShader = gaiaShader;
                drawInfo.handOnShader = gaiaShader;
                drawInfo.handOffShader = gaiaShader;
                drawInfo.shoeShader = gaiaShader;
            }

            if (IronGuard)
            {
                player.bodyFrame.Y = player.bodyFrame.Height * 10;
                if (ironShieldTimer > 0)
                {
                    int ironShader = GameShaders.Armor.GetShaderIdFromItemId(ItemID.ReflectiveSilverDye);
                    drawInfo.bodyArmorShader = ironShader;
                    drawInfo.headArmorShader = ironShader;
                    drawInfo.legArmorShader = ironShader;
                    drawInfo.wingShader = ironShader;
                    drawInfo.handOnShader = ironShader;
                    drawInfo.handOffShader = ironShader;
                    drawInfo.shoeShader = ironShader;
                }
            }
        }

        public void AddPet(bool toggle, bool vanityToggle, int buff, int proj)
        {
            if (vanityToggle)
            {
                PetsActive = false;
                return;
            }

            if (player.whoAmI == Main.myPlayer && toggle && player.FindBuffIndex(buff) == -1 && player.ownedProjectileCounts[proj] < 1)
            {
                Projectile p = Main.projectile[Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -1f, proj, 0, 0f, player.whoAmI)];
                p.netUpdate = true;
            }
        }

        public void AddMinion(bool toggle, int proj, int damage, float knockback)
        {
            if (player.whoAmI != Main.myPlayer) return;
            if (player.ownedProjectileCounts[proj] < 1 && player.whoAmI == Main.myPlayer && toggle)
            {
                Projectile pro = Main.projectile[Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -1f, proj, damage, knockback, Main.myPlayer)];
                pro.netUpdate = true;
            }
        }

        private void KillPets()
        {
            int petId = player.miscEquips[0].buffType;
            int lightPetId = player.miscEquips[1].buffType;

            player.buffImmune[petId] = true;
            player.buffImmune[lightPetId] = true;

            player.ClearBuff(petId);
            player.ClearBuff(lightPetId);

            //memorizes player selections
            if (!WasAsocial)
            {
                HidePetToggle0 = player.hideMisc[0];
                HidePetToggle1 = player.hideMisc[1];

                WasAsocial = true;
            }

            //disables pet and light pet too!
            if (!player.hideMisc[0])
            {
                player.TogglePet();
            }

            if (!player.hideMisc[1])
            {
                player.ToggleLight();
            }

            player.hideMisc[0] = true;
            player.hideMisc[1] = true;
        }

        public void Squeak(Vector2 center)
        {
            if (!Main.dedServ)
            {
                int rng = Main.rand.Next(6);

                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/SqueakyToy/squeak" + (rng + 1)).WithVolume(1f).WithPitchVariance(.5f), center);
            }
        }

        private int InfestedExtraDot()
        {
            int buffIndex = player.FindBuffIndex(ModContent.BuffType<Infested>());
            if (buffIndex == -1)
            {
                buffIndex = player.FindBuffIndex(ModContent.BuffType<InfestedEX>());
                if (buffIndex == -1)
                    return 0;
            }

            int timeLeft = player.buffTime[buffIndex];
            float baseVal = (float)(MaxInfestTime - timeLeft) / 90; //change the denominator to adjust max power of DOT
            int modifier = (int)(baseVal * baseVal + 4);

            InfestedDust = baseVal / 10 + 1f;
            if (InfestedDust > 5f)
                InfestedDust = 5f;

            return modifier * 2;
        }

        public void AllDamageUp(float dmg)
        {
            player.magicDamage += dmg;
            player.meleeDamage += dmg;
            player.rangedDamage += dmg;
            player.minionDamage += dmg;
        }

        public void AllCritUp(int crit)
        {
            player.meleeCrit += crit;
            player.rangedCrit += crit;
            player.magicCrit += crit;
        }

        public void AllCritEquals(int crit)
        {
            player.meleeCrit = crit;
            player.rangedCrit = crit;
            player.magicCrit = crit;
            if (SpiderEnchant)
                SummonCrit = crit;
        }

        public int HighestDamageTypeScaling(int dmg)
        {
            List<float> types = new List<float> { player.meleeDamage, player.rangedDamage, player.magicDamage, player.minionDamage};
            
            return (int)(types.Max() * dmg);
        }

        public int HighestCritChance()
        {
            List<int> types = new List<int> { player.meleeCrit, player.rangedCrit, player.magicCrit };

            return types.Max();
        }

        public override bool PreItemCheck()
        {
            if (Berserked || (TribalCharm && player.GetToggleValue("TribalCharm", false) && player.HeldItem.type != ItemID.RodofDiscord && player.HeldItem.fishingPole == 0 ))
            {
                TribalAutoFire = player.HeldItem.autoReuse;
                player.HeldItem.autoReuse = true;
            }    

            /*if (FargoSoulsWorld.MasochistMode) //maso item nerfs
            {
                PreNerfDamage = player.HeldItem.damage;
                player.HeldItem.damage = (int)(player.HeldItem.damage * MasoItemNerfs(player.HeldItem.type));
            }*/

            return true;
        }

        public override void PostItemCheck()
        {
            if (Berserked || (TribalCharm && player.GetToggleValue("TribalCharm", false) && player.HeldItem.type != ItemID.RodofDiscord && player.HeldItem.fishingPole == 0))
            {
                player.HeldItem.autoReuse = TribalAutoFire;
            }

            /*if (FargoSoulsWorld.MasochistMode) //revert maso item nerfs
            {
                player.HeldItem.damage = PreNerfDamage;
            }*/
        }

        public override void ModifyWeaponDamage(Item item, ref float add, ref float mult, ref float flat)
        {
            if (FargoSoulsWorld.EternityMode)
            {
                mult *= MasoItemNerfs(item.type);

                if (item.ranged) //change all of these to additive
                {
                    //shroomite headpieces
                    if (item.useAmmo == AmmoID.Arrow || item.useAmmo == AmmoID.Stake)
                    {
                        mult /= player.arrowDamage;
                        add += player.arrowDamage - 1f;
                    }
                    if (item.useAmmo == AmmoID.Bullet || item.useAmmo == AmmoID.CandyCorn)
                    {
                        mult /= player.bulletDamage;
                        add += player.bulletDamage - 1f;
                    }
                    if (item.useAmmo == AmmoID.Rocket || item.useAmmo == AmmoID.StyngerBolt || item.useAmmo == AmmoID.JackOLantern || item.useAmmo == AmmoID.NailFriendly)
                    {
                        mult /= player.bulletDamage;
                        add += player.bulletDamage - 1f;
                    }
                }
            }
        }

        private float MasoItemNerfs(int type)
        {
            switch (type)
            {
                case ItemID.BlizzardStaff:
                    AttackSpeed *= 0.5f;
                    return 2f / 3f;

                case ItemID.DemonScythe:
                    if (!NPC.downedBoss2)
                    {
                        AttackSpeed *= 0.75f;
                        return 0.5f;
                    }
                    return 2f / 3f;

                case ItemID.StarCannon:
                case ItemID.ElectrosphereLauncher:
                case ItemID.DaedalusStormbow:
                case ItemID.BeesKnees:
                case ItemID.LaserMachinegun:
                    return 2f / 3f;

                case ItemID.Beenade:
                case ItemID.Razorpine:
                    AttackSpeed *= 2f / 3f;
                    return 2f / 3f;
                    
                case ItemID.DD2BetsyBow:
                case ItemID.Uzi:
                case ItemID.PhoenixBlaster:
                case ItemID.LastPrism:
                case ItemID.OnyxBlaster:
                case ItemID.Handgun:
                case ItemID.SpikyBall:
                case ItemID.SDMG:
                case ItemID.Xenopopper:
                case ItemID.NebulaArcanum:
                case ItemID.PainterPaintballGun:
                case ItemID.MoltenFury:
                case ItemID.Phantasm:
                    return 0.75f;

                case ItemID.VampireKnives:
                    AttackSpeed *= 0.75f;
                    return 0.75f;

                case ItemID.SnowmanCannon:
                case ItemID.SkyFracture:
                    return 0.8f;

                case ItemID.SpaceGun:
                    if (!NPC.downedBoss2)
                    {
                        AttackSpeed *= 0.75f;
                        return 0.75f;
                    }
                    return 0.85f;

                case ItemID.Tsunami:
                case ItemID.Flairon:
                case ItemID.ChlorophyteShotbow:
                case ItemID.HellwingBow:
                case ItemID.DartPistol:
                case ItemID.DartRifle:
                case ItemID.Megashark:
                case ItemID.BatScepter:
                case ItemID.ChainGun:
                case ItemID.VortexBeater:
                case ItemID.RavenStaff:
                case ItemID.XenoStaff:
                case ItemID.StardustDragonStaff:
                    return 0.85f;

                case ItemID.BeeGun:
                case ItemID.Grenade:
                case ItemID.StickyGrenade:
                case ItemID.BouncyGrenade:
                    AttackSpeed *= 2f / 3f;
                    return 1f;

                case ItemID.DD2BallistraTowerT1Popper:
                case ItemID.DD2BallistraTowerT2Popper:
                case ItemID.DD2BallistraTowerT3Popper:
                case ItemID.DD2ExplosiveTrapT1Popper:
                case ItemID.DD2ExplosiveTrapT2Popper:
                case ItemID.DD2ExplosiveTrapT3Popper:
                case ItemID.DD2FlameburstTowerT1Popper:
                case ItemID.DD2FlameburstTowerT2Popper:
                case ItemID.DD2FlameburstTowerT3Popper:
                case ItemID.DD2LightningAuraT1Popper:
                case ItemID.DD2LightningAuraT2Popper:
                case ItemID.DD2LightningAuraT3Popper:
                    AttackSpeed *= 2f / 3f;
                    return 1f;

                case ItemID.DD2SquireBetsySword: //flying dragon
                    AttackSpeed *= 4f / 3f;
                    return 4f / 3f;

                case ItemID.MonkStaffT3: //sky dragon's fury
                    return 1.25f;

                default:
                    return 1f;
            }
        }

        public override void CatchFish(Item fishingRod, Item bait, int power, int liquidType, int poolSize, int worldLayer, int questFish, ref int caughtType, ref bool junk)
        {
            if (bait.type == ModContent.ItemType<TruffleWormEX>())
            {
                caughtType = 0;
                bool spawned = false;
                for (int i = 0; i < 1000; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].bobber
                        && Main.projectile[i].owner == player.whoAmI && player.whoAmI == Main.myPlayer)
                    {
                        Main.projectile[i].ai[0] = 2f; //cut fishing lines
                        Main.projectile[i].netUpdate = true;

                        if (!spawned && Main.projectile[i].wet && FargoSoulsWorld.EternityMode && !NPC.AnyNPCs(NPCID.DukeFishron)) //should spawn boss
                        {
                            spawned = true;
                            if (Main.netMode == NetmodeID.SinglePlayer) //singleplayer
                            {
                                EModeGlobalNPC.spawnFishronEX = true;
                                NPC.NewNPC((int)Main.projectile[i].Center.X, (int)Main.projectile[i].Center.Y + 100,
                                    NPCID.DukeFishron, 0, 0f, 0f, 0f, 0f, player.whoAmI);
                                EModeGlobalNPC.spawnFishronEX = false;
                                Main.NewText(Language.GetTextValue("Mods.FargowiltasSouls.FargoPlayer.SpawnDukeFishronEX"), 50, 100, 255);
                            }
                            else if (Main.netMode == NetmodeID.MultiplayerClient) //MP, broadcast(?) packet from spawning player's client
                            {
                                var netMessage = mod.GetPacket();
                                netMessage.Write((byte)77);
                                netMessage.Write((byte)player.whoAmI);
                                netMessage.Write((int)Main.projectile[i].Center.X);
                                netMessage.Write((int)Main.projectile[i].Center.Y + 100);
                                netMessage.Send();
                            }
                            else if (Main.netMode == NetmodeID.Server)
                            {
                                NetMessage.BroadcastChatMessage(NetworkText.FromLiteral("???????"), Color.White);
                            }
                        }
                    }
                }
                if (spawned)
                {
                    bait.stack--;
                    if (bait.stack <= 0)
                        bait.SetDefaults(0);
                }
            }
        }

        public override void PostNurseHeal(NPC nurse, int health, bool removeDebuffs, int price)
        {
            if (player.whoAmI == Main.myPlayer && GuttedHeart && player.GetToggleValue("MasoBrain"))
            {
                for (int i = 0; i < 200; i++)
                {
                    NPC npc = Main.npc[i];

                    if (npc.type == ModContent.NPCType<CreeperGutted>() && npc.ai[0] == player.whoAmI)
                    {
                        int heal = npc.lifeMax - npc.life;

                        if (heal > 0)
                        {
                            npc.HealEffect(heal);
                            npc.life = npc.lifeMax;
                        }
                    }
                }
            }
        }

        public override bool ConsumeAmmo(Item weapon, Item ammo)
        {
            if (weapon.ranged)
            {
                if (RangedEssence && Main.rand.NextBool(10))
                    return false;
                if (RangedSoul && Main.rand.NextBool(5))
                    return false;
            }
            if (GaiaSet && Main.rand.NextBool(10))
                return false;
            return true;
        }

        int frameCounter = 0;
        int frameSnow = 1;
        int frameMutantAura = 0;
        //int frameMutantLightning = 0;

        public static readonly PlayerLayer BlizzardEffect = new PlayerLayer("FargowiltasSouls", "MiscEffects", PlayerLayer.MiscEffectsFront, delegate (PlayerDrawInfo drawInfo)
        {
            if (drawInfo.shadow != 0f)
            {
                return;
            }
            Player drawPlayer = drawInfo.drawPlayer;
            Mod mod = ModLoader.GetMod("FargowiltasSouls");
            FargoPlayer modPlayer = drawPlayer.GetModPlayer<FargoPlayer>();
            
            if (++modPlayer.frameCounter > 60)
                modPlayer.frameCounter = 0;

            if (!drawPlayer.dead)
            {
                if (modPlayer.MutantSetBonus)
                {
                    if (modPlayer.frameCounter % 4 == 0)
                    {
                        if (++modPlayer.frameMutantAura >= 19)
                            modPlayer.frameMutantAura = 0;
                    }

                    Texture2D texture = mod.GetTexture("NPCs/MutantBoss/MutantAura");
                    int frameSize = texture.Height / 19;
                    int drawX = (int)(drawPlayer.MountedCenter.X - Main.screenPosition.X);
                    int drawY = (int)(drawPlayer.MountedCenter.Y - Main.screenPosition.Y - 16 * drawPlayer.gravDir);
                    DrawData data = new DrawData(texture, new Vector2(drawX, drawY), new Rectangle(0, frameSize * modPlayer.frameMutantAura, texture.Width, frameSize), Color.White, drawPlayer.gravDir < 0 ? MathHelper.Pi : 0, new Vector2(texture.Width / 2f, frameSize / 2f), 1f, drawPlayer.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
                    Main.playerDrawData.Add(data);
                }
                /*if (modPlayer.MutantSetBonus)
                {
                    if (modPlayer.frameCounter % 4 == 0)
                    {
                        if (++modPlayer.frameMutantLightning >= 20)
                            modPlayer.frameMutantLightning = 0;
                    }

                    Texture2D texture = mod.GetTexture("NPCs/MutantBoss/MutantLightning");
                    int frameSize = texture.Height / 20;
                    int drawX = (int)(drawInfo.position.X + drawPlayer.width / 2f - Main.screenPosition.X);
                    int drawY = (int)(drawInfo.position.Y + drawPlayer.height / 2f - Main.screenPosition.Y);
                    DrawData data = new DrawData(texture, new Vector2(drawX, drawY), new Rectangle(0, frameSize * modPlayer.frameMutantLightning, texture.Width, frameSize), Color.White, 0f, new Vector2(texture.Width / 2f, frameSize / 2f), 1f, SpriteEffects.None, 0);
                    Main.playerDrawData.Add(data);
                }*/
                if (modPlayer.SnowVisual)
                {
                    if (modPlayer.frameCounter % 5 == 0)
                    {
                        if (++modPlayer.frameSnow > 20)
                            modPlayer.frameSnow = 1;
                    }

                    Texture2D texture = mod.GetTexture("Projectiles/Souls/SnowBlizzard");
                    int frameSize = texture.Height / 20;
                    int drawX = (int)(drawPlayer.MountedCenter.X - Main.screenPosition.X);
                    int drawY = (int)(drawPlayer.MountedCenter.Y - Main.screenPosition.Y);
                    DrawData data = new DrawData(texture, new Vector2(drawX, drawY), new Rectangle(0, frameSize * modPlayer.frameSnow, texture.Width, frameSize), Lighting.GetColor((int)((drawInfo.position.X + drawPlayer.width / 2f) / 16f), (int)((drawInfo.position.Y + drawPlayer.height / 2f) / 16f)), drawPlayer.gravDir < 0 ? MathHelper.Pi : 0f, new Vector2(texture.Width / 2f, frameSize / 2f), 1f, SpriteEffects.None, 0);
                    Main.playerDrawData.Add(data);
                }

                //GameShaders.Armor.Apply(GameShaders.Armor.GetShaderIdFromItemId(drawPlayer.dye[1].type, drawPlayer, data);
            }
        });

        public static readonly PlayerLayer MashLayer = new PlayerLayer("FargowiltasSouls", "MiscEffects", PlayerLayer.MiscEffectsFront, delegate (PlayerDrawInfo drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            if (drawPlayer.whoAmI != Main.myPlayer || !drawPlayer.active || drawPlayer.dead || drawPlayer.ghost)
                return;
            
            Texture2D dpad = ModContent.GetTexture("FargowiltasSouls/UI/DPad");
            int num156 = dpad.Height / 4; //ypos of lower right corner of sprite to draw
            int y3 = num156 * (int)(Main.GlobalTime % 0.5 * 8); //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, dpad.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Vector2 drawPos = (drawPlayer.gravDir > 0 ? drawPlayer.Bottom : drawPlayer.Top) - Main.screenPosition;
            drawPos.Y += 48 * drawPlayer.gravDir;
            DrawData data = new DrawData(dpad, drawPos, rectangle, Color.White, drawPlayer.gravDir < 0 ? MathHelper.Pi : 0f, rectangle.Size() / 2, 2.5f, SpriteEffects.None, 0);
            Main.playerDrawData.Add(data);
        });

        public override void ModifyDrawLayers(List<PlayerLayer> layers)
        {
            BlizzardEffect.visible = true;
            layers.Add(BlizzardEffect);

            if (Mash)
            {
                MashLayer.visible = true;
                layers.Add(MashLayer);
            }

            if (BetsyDashing || ShellHide || GoldShell) //dont draw player during betsy dash
                while (layers.Count > 0)
                    layers.RemoveAt(0);

            if (SquirrelMount)
            {
                foreach (PlayerLayer playerLayer in layers)
                {
                    if (playerLayer != PlayerLayer.MountBack && playerLayer != PlayerLayer.MountFront && playerLayer != PlayerLayer.MiscEffectsFront && playerLayer != PlayerLayer.MiscEffectsBack)
                    {
                        playerLayer.visible = false;
                    }
                }
            }
        }

        private int getHealMultiplier(int heal)
        {
            float bonus = 0f;

            if (player.GetToggleValue("Valhalla", false))
            {
                //if (TerrariaSoul) bonus = 1f; else 
                if (WillForce || (ValhallaEnchant && WizardEnchant))
                    bonus = 1f / 2f;
                else if (ValhallaEnchant || (SquireEnchant && WizardEnchant))
                    bonus = 1f / 3f;
                else if (SquireEnchant)
                    bonus = 1f / 4f;
            }

            heal = (int)(heal * (1 + bonus));

            return heal;
        }

        public override void GetHealLife(Item item, bool quickHeal, ref int healValue)
        {
            healValue = getHealMultiplier(healValue);
        }

        public void HealPlayer(int amount)
        {
            amount = getHealMultiplier(amount);

            player.statLife += amount;
            if (player.statLife > player.statLifeMax2)
                player.statLife = player.statLifeMax2;
            player.HealEffect(amount);
        }

        public override void ModifyScreenPosition()
        {
            if (Screenshake > 0)
                Main.screenPosition += Main.rand.NextVector2Circular(7, 7);
        }

        public override void clientClone(ModPlayer clientClone)
        {
            FargoPlayer modPlayer = clientClone as FargoPlayer;
            modPlayer.Toggler = Toggler;
        }

        public void SyncToggle(string key)
        {
            if (!TogglesToSync.ContainsKey(key))
                TogglesToSync.Add(key, player.GetToggle(key).ToggleBool);
        }

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            foreach (KeyValuePair<string, bool> toggle in TogglesToSync)
            {
                ModPacket packet = mod.GetPacket();

                packet.Write((byte)80);
                packet.Write((byte)player.whoAmI);
                packet.Write(toggle.Key);
                packet.Write(toggle.Value);

                packet.Send(toWho, fromWho);
            }
            
            TogglesToSync.Clear();
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            FargoPlayer modPlayer = clientPlayer as FargoPlayer;
            if (modPlayer.Toggler.Toggles != Toggler.Toggles)
            {
                ModPacket packet = mod.GetPacket();
                packet.Write((byte)79);
                packet.Write((byte)player.whoAmI);
                packet.Write((byte)Toggler.Toggles.Count);

                for (int i = 0; i < Toggler.Toggles.Count; i++)
                {
                    packet.Write(Toggler.Toggles.Values.ElementAt(i).ToggleBool);
                }

                packet.Send();
            }
        }

        public void AddBuffNoStack(int buff, int duration)
        {
            if (!player.HasBuff(buff) && ReallyAwfulDebuffCooldown <= 0)
            {
                player.AddBuff(buff, duration);
                int d = player.FindBuffIndex(buff);
                if (d != -1) //if debuff successfully applied
                {
                    ReallyAwfulDebuffCooldown = player.buffTime[d] + 240;
                }
            }
        }

        public void TryAdditionalAttacks(int damage, bool melee, bool ranged, bool magic, bool minion)
        {
            if (player.whoAmI != Main.myPlayer)
                return;

            if (AdditionalAttacks && AdditionalAttacksTimer <= 0)
            {
                AdditionalAttacksTimer = 60;

                Vector2 position = player.Center;
                Vector2 velocity = Vector2.Normalize(Main.MouseWorld - position);

                if (BorealEnchant && player.GetToggleValue("Boreal"))
                {
                    Vector2 vel = Vector2.Normalize(Main.MouseWorld - player.Center) * 17f;
                    int snowballDamage = damage / 2;
                    if (!TerrariaSoul)
                        snowballDamage = Math.Min(snowballDamage, HighestDamageTypeScaling(WoodForce ? 300 : 20));
                    int p = Projectile.NewProjectile(player.Center, vel, ProjectileID.SnowBallFriendly, snowballDamage, 1, Main.myPlayer);

                    int numSnowballs = WoodForce ? 5 : 3;
                    if (p != Main.maxProjectiles)
                        FargoGlobalProjectile.SplitProj(Main.projectile[p], numSnowballs, MathHelper.Pi / 10, 1);
                }

                if (CelestialRune && player.GetToggleValue("MasoCelest"))
                {
                    if (melee) //fireball
                    {
                        Main.PlaySound(SoundID.Item34, position);
                        for (int i = 0; i < 3; i++)
                        {
                            Projectile.NewProjectile(position, velocity.RotatedByRandom(Math.PI / 6) * Main.rand.NextFloat(6f, 10f),
                                ModContent.ProjectileType<CelestialRuneFireball>(), (int)(50f * player.meleeDamage), 9f, player.whoAmI);
                        }
                    }
                    if (ranged) //lightning
                    {
                        for (int i = -1; i <= 1; i++)
                        {
                            float ai1 = Main.rand.Next(100);
                            Vector2 vel = Vector2.Normalize(velocity.RotatedByRandom(Math.PI / 4)).RotatedBy(MathHelper.ToRadians(5) * i) * 7f;
                            Projectile.NewProjectile(position, vel, ModContent.ProjectileType<CelestialRuneLightningArc>(),
                                (int)(50f * player.rangedDamage), 1f, player.whoAmI, velocity.ToRotation(), ai1);
                        }
                    }
                    if (magic) //ice mist
                    {
                        Projectile.NewProjectile(position, velocity * 4.25f, ModContent.ProjectileType<CelestialRuneIceMist>(), (int)(50f * player.magicDamage), 4f, player.whoAmI);
                    }
                    if (minion) //ancient vision
                    {
                        Projectile.NewProjectile(position, velocity * 16f, ModContent.ProjectileType<CelestialRuneAncientVision>(), (int)(50f * player.minionDamage), 3f, player.whoAmI);
                    }
                }

                if (PumpkingsCape && player.GetToggleValue("MasoPump"))
                {
                    if (melee) //flaming jack
                    {
                        float distance = 2000f;
                        int target = -1;
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            if (Main.npc[i].active && Main.npc[i].CanBeChasedBy())
                            {
                                float newDist = Main.npc[i].Distance(player.Center);
                                if (newDist < distance)
                                {
                                    distance = newDist;
                                    target = i;
                                }
                            }
                        }
                        if (target != -1)
                            Projectile.NewProjectile(position, velocity * 8f, ProjectileID.FlamingJack, (int)(75f * player.meleeDamage), 7.5f, player.whoAmI, target, 0f);
                    }
                    if (ranged) //jack o lantern
                    {
                        Projectile.NewProjectile(position, velocity * 11f, ProjectileID.JackOLantern, (int)(65f * player.rangedDamage), 8f, player.whoAmI);
                    }
                    if (magic) //bat scepter
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            Vector2 newVel = velocity * 10f;
                            newVel.X += Main.rand.Next(-35, 36) * 0.02f;
                            newVel.Y += Main.rand.Next(-35, 36) * 0.02f;
                            Projectile.NewProjectile(position, newVel, ProjectileID.Bat, (int)(45f * player.magicDamage), 3f, player.whoAmI);
                        }
                    }
                    if (minion)
                    {
                        const int max = 6;
                        for (int i = 0; i < max; i++)
                        {
                            Projectile.NewProjectile(position, velocity.RotatedBy(MathHelper.TwoPi / max * i) * 20f, ModContent.ProjectileType<SpookyScythe>(), (int)(40 * player.minionDamage), 2, player.whoAmI);
                        }
                    }
                }
            }
        }
    }
}
