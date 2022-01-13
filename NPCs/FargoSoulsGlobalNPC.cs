using FargowiltasSouls.Projectiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.Items.Weapons.BossDrops;
using FargowiltasSouls.NPCs.Critters;
using FargowiltasSouls.Projectiles.Souls;
using FargowiltasSouls.Buffs.Souls;
using Fargowiltas.NPCs;
using FargowiltasSouls.Items.Weapons.Misc;
using FargowiltasSouls.Toggler;
using Terraria.Localization;

namespace FargowiltasSouls.NPCs
{
    public class FargoSoulsGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity
        {
            get
            {
                return true;
            }
        }

        public bool FirstTick = false;
        //debuffs
        public bool OriPoison;
        public bool SBleed;
        public bool Shock;
        public bool Rotting;
        public bool LeadPoison;
        public bool SolarFlare;
        public bool TimeFrozen;
        public bool HellFire;
        public bool Infested;
        public int MaxInfestTime;
        public float InfestedDust;
        public bool Needles;
        public bool Electrified;
        public bool CurseoftheMoon;
        public int lightningRodTimer;
        public bool Sadism;
        public bool OceanicMaul;
        public bool MutantNibble;
        public int LifePrevious = -1;
        public bool GodEater;
        public bool Suffocation;
        public int SuffocationTimer;
        public bool Villain;
        public bool FlamesoftheUniverse;
        public bool Lethargic;
        public int LethargicCounter;
        public bool ExplosiveCritter = false;
        private int critterCounter = 120;

        public bool SnowChilled = false;
        public int SnowChilledTimer;

        public bool Chilled = false;

        private int necroDamage = 0;

        public static bool Revengeance => CalamityMod.World.CalamityWorld.revenge;

        public override void ResetEffects(NPC npc)
        {
            TimeFrozen = false;
            SBleed = false;
            Shock = false;
            Rotting = false;
            LeadPoison = false;
            SolarFlare = false;
            HellFire = false;
            OriPoison = false;
            Infested = false;
            Needles = false;
            Electrified = false;
            CurseoftheMoon = false;
            Sadism = false;
            OceanicMaul = false;
            MutantNibble = false;
            GodEater = false;
            Suffocation = false;
            //SnowChilled = false;
            Chilled = false;
            FlamesoftheUniverse = false;
        }

        public override void SetDefaults(NPC npc)
        {
        }

        public override bool PreAI(NPC npc)
        {
            if (TimeFrozen)
            {
                npc.position = npc.oldPosition;
                npc.frameCounter = 0;
                return false;
            }

            if (!FirstTick)
            {
                switch (npc.type)
                {
                    case NPCID.TheDestroyer:
                    case NPCID.TheDestroyerBody:
                    case NPCID.TheDestroyerTail:
                        npc.buffImmune[ModContent.BuffType<TimeFrozen>()] = false;
                        npc.buffImmune[ModContent.BuffType<Frozen>()] = false;
                        //npc.buffImmune[BuffID.Darkness] = false;
                        break;

                    /*case NPCID.WallofFlesh:
                    case NPCID.WallofFleshEye:
                    case NPCID.MoonLordCore:
                    case NPCID.MoonLordHand:
                    case NPCID.MoonLordHead:
                    case NPCID.MoonLordLeechBlob:
                    case NPCID.TargetDummy:
                    case NPCID.GolemFistLeft:
                    case NPCID.GolemFistRight:
                    case NPCID.GolemHead:
                    case NPCID.DungeonGuardian:
                    case NPCID.DukeFishron:
                        SpecialEnchantImmune = true;
                        break;*/

                    case NPCID.Squirrel:
                    case NPCID.SquirrelRed:
                        if (!npc.SpawnedFromStatue)
                        {
                            int p = Player.FindClosest(npc.position, npc.width, npc.height);
                            if ((p == -1 || npc.Distance(Main.player[p].Center) > 800) && Main.rand.NextBool(5))
                                npc.Transform(ModContent.NPCType<TophatSquirrelCritter>());
                        }
                        break;

                    default:
                        break;
                }

                //critters
                if (npc.damage == 0 && !npc.townNPC && npc.lifeMax == 5)
                {
                    Player player = Main.player[Main.myPlayer];

                    /*if ( npc.releaseOwner == player.whoAmI && player.GetModPlayer<FargoPlayer>().WoodEnchant)
                    {
                        switch (npc.type)
                        {
                            case NPCID.Bunny:
                                
                                
                                npc.active = false;
                                break;
                        }



                        ExplosiveCritter = true;
                    }*/
                }

                FirstTick = true;
            }

            if (Lethargic && ++LethargicCounter > 3)
            {
                LethargicCounter = 0;
                return false;
            }

            if (ExplosiveCritter)
            {
                critterCounter--;

                if (critterCounter <= 0)
                {
                    Player player = Main.player[npc.releaseOwner];
                    FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();

                    int damage = 25;

                    if (modPlayer.WoodForce || modPlayer.WizardEnchant)
                    {
                        damage *= 5;
                    }

                    Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<ExplosionSmall>(), modPlayer.HighestDamageTypeScaling(damage), 4, npc.releaseOwner);
                    //gold critters make coin value go up of hit enemy, millions of other effects eeech
                }
                
            }

            if (SnowChilled)
            {
                SnowChilledTimer--;

                if (SnowChilledTimer <= 0)
                    SnowChilled = false;

                if (SnowChilledTimer % 2 == 1)
                    return false;
            }
            
            return true;
        }

        public override void AI(NPC npc)
        {
            if (SnowChilled)
            {
                int dustId = Dust.NewDust(npc.position, npc.width, npc.height, 76, npc.velocity.X, npc.velocity.Y, 100, default(Color), 1f);
                Main.dust[dustId].noGravity = true;

                npc.position -= npc.velocity * 0.5f;
            }

            SuffocationTimer += Suffocation ? 1 : -2;
            if (SuffocationTimer < 0)
                SuffocationTimer = 0;
        }

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (LeadPoison)
            {
                if (Main.rand.Next(4) < 3)
                {
                    int dust = Dust.NewDust(new Vector2(npc.position.X - 2f, npc.position.Y - 2f), npc.width + 4, npc.height + 4, DustID.Lead, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default(Color), 1f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Dust expr_1CCF_cp_0 = Main.dust[dust];
                    expr_1CCF_cp_0.velocity.Y = expr_1CCF_cp_0.velocity.Y - 0.5f;
                    if (Main.rand.NextBool(4))
                    {
                        Main.dust[dust].noGravity = false;
                        Main.dust[dust].scale *= 0.5f;
                    }
                }
            }


            if (OriPoison)
            {
                if (Main.rand.Next(4) < 3)
                {
                    int dust = Dust.NewDust(new Vector2(npc.position.X - 2f, npc.position.Y - 2f), npc.width + 4, npc.height + 4, DustID.PinkFlame, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default(Color), 1f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Dust expr_1CCF_cp_0 = Main.dust[dust];
                    expr_1CCF_cp_0.velocity.Y = expr_1CCF_cp_0.velocity.Y - 0.5f;
                    if (Main.rand.NextBool(4))
                    {
                        Main.dust[dust].noGravity = false;
                        Main.dust[dust].scale *= 0.5f;
                    }
                }
            }

            if (HellFire)
            {
                if (Main.rand.Next(4) < 3)
                {
                    int dust = Dust.NewDust(new Vector2(npc.position.X - 2f, npc.position.Y - 2f), npc.width + 4, npc.height + 4, DustID.SolarFlare, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].shader = GameShaders.Armor.GetSecondaryShader(56, Main.LocalPlayer);

                    Dust expr_1CCF_cp_0 = Main.dust[dust];
                    expr_1CCF_cp_0.velocity.Y = expr_1CCF_cp_0.velocity.Y - 0.5f;
                    if (Main.rand.NextBool(4))
                    {
                        Main.dust[dust].noGravity = false;
                        Main.dust[dust].scale *= 0.5f;
                    }
                }
            }

            if (SBleed)
            {
                if (Main.rand.Next(4) < 3)
                {
                    int dust = Dust.NewDust(new Vector2(npc.position.X - 2f, npc.position.Y - 2f), npc.width + 4, npc.height + 4, DustID.Blood, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].shader = GameShaders.Armor.GetSecondaryShader(56, Main.LocalPlayer);

                    Dust expr_1CCF_cp_0 = Main.dust[dust];
                    expr_1CCF_cp_0.velocity.Y = expr_1CCF_cp_0.velocity.Y - 0.5f;
                    if (Main.rand.NextBool(4))
                    {
                        Main.dust[dust].noGravity = false;
                        Main.dust[dust].scale *= 0.5f;
                    }
                }
            }

            /*if (Infested)
            {
                if (Main.rand.Next(4) < 3)
                {
                    int dust = Dust.NewDust(new Vector2(npc.position.X - 2f, npc.position.Y - 2f), npc.width + 4, npc.height + 4, 44, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, Color.LimeGreen, InfestedDust);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Dust expr_1CCF_cp_0 = Main.dust[dust];
                    expr_1CCF_cp_0.velocity.Y = expr_1CCF_cp_0.velocity.Y - 0.5f;
                    if (Main.rand.NextBool(4))
                    {
                        Main.dust[dust].noGravity = false;
                        Main.dust[dust].scale *= 0.5f;
                    }
                }

                Lighting.AddLight((int)(npc.position.X / 16f), (int)(npc.position.Y / 16f + 1f), 1f, 0.3f, 0.1f);
            }*/

            if (Suffocation)
                drawColor = Colors.RarityPurple;

            if (Villain)
            {
                if (Main.rand.Next(4) < 3)
                {
                    int dust = Dust.NewDust(new Vector2(npc.position.X - 2f, npc.position.Y - 2f), npc.width + 4, npc.height + 4, DustID.AncientLight, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Dust expr_1CCF_cp_0 = Main.dust[dust];
                    expr_1CCF_cp_0.velocity.Y = expr_1CCF_cp_0.velocity.Y - 0.5f;
                    if (Main.rand.NextBool(4))
                    {
                        Main.dust[dust].noGravity = false;
                        Main.dust[dust].scale *= 0.5f;
                    }
                }

                Lighting.AddLight((int)(npc.position.X / 16f), (int)(npc.position.Y / 16f + 1f), 1f, 0.3f, 0.1f);
            }

            if (Electrified)
            {
                if (Main.rand.Next(4) < 3)
                {
                    int dust = Dust.NewDust(new Vector2(npc.position.X - 2f, npc.position.Y - 2f), npc.width + 4, npc.height + 4, 229, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    if (Main.rand.NextBool(3))
                    {
                        Main.dust[dust].noGravity = false;
                        Main.dust[dust].scale *= 0.5f;
                    }
                }

                Lighting.AddLight((int)npc.Center.X / 16, (int)npc.Center.Y / 16, 0.3f, 0.8f, 1.1f);
            }

            if (CurseoftheMoon)
            {
                int d = Dust.NewDust(npc.Center, 0, 0, 229, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 3f;
                Main.dust[d].scale += 0.5f;

                if (Main.rand.Next(4) < 3)
                {
                    d = Dust.NewDust(npc.position, npc.width, npc.height, 229, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity.Y -= 1f;
                    Main.dust[d].velocity *= 2f;
                }
            }

            if (Sadism)
            {
                if (Main.rand.NextBool(7))
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, 156, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 0, Color.White, 4f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 2f;
                }
            }

            if (GodEater)
            {
                if (Main.rand.NextBool(7))
                {
                    int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, 86, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 0, Color.White, 4f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.2f;
                    Main.dust[dust].velocity.Y -= 0.15f;
                }
                Lighting.AddLight(npc.position, 0.15f, 0.03f, 0.09f);
            }

            if (Chilled)
            {
                int d = Dust.NewDust(npc.Center, 0, 0, 15, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 3f;
                Main.dust[d].scale += 0.5f;

                if (Main.rand.Next(4) < 3)
                {
                    d = Dust.NewDust(npc.position, npc.width, npc.height, 15, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity.Y -= 1f;
                    Main.dust[d].velocity *= 2f;
                }
            }

            if (FlamesoftheUniverse)
            {
                if (!Main.rand.NextBool(3))
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, 203, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 0, new Color(50 * Main.rand.Next(6) + 5, 50 * Main.rand.Next(6) + 5, 50 * Main.rand.Next(6) + 5, 0), 2.5f);
                    Main.dust[d].velocity.Y -= 1;
                    Main.dust[d].velocity *= 1.5f;
                    Main.dust[d].noGravity = true;
                }
            }
        }

        public override Color? GetAlpha(NPC npc, Color drawColor)
        {
            if (Chilled)
            {
                drawColor = Color.LightBlue;
                return drawColor;
            }

            return null;
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            Player player = Main.player[Main.myPlayer];
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();

            if (Rotting)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;

                npc.lifeRegen -= 100;

                if (damage < 5)
                    damage = 5;
            }

            if (LeadPoison)
            {
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }

                int dot = npc.type == NPCID.EaterofWorldsBody ? 2 : 10;

                if (modPlayer.TerraForce || modPlayer.WizardEnchant)
                {
                    dot *= 3;
                }

                npc.lifeRegen -= dot;
            }

            //50 dps
            if (SolarFlare)
            {
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }

                npc.lifeRegen -= 100;

                if (damage < 10)
                {
                    damage = 10;
                }
            }

            //100 dps
            if (HellFire)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;

                npc.lifeRegen -= 200;

                if (damage < 20)
                {
                    damage = 20;
                }
            }

            //20 dps
            if (OriPoison)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;

                npc.lifeRegen -= 40;

                if (damage < 4)
                    damage = 4;
            }

            if (Infested)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;

                npc.lifeRegen -= InfestedExtraDot(npc);

                if (damage < 8)
                    damage = 8;
            }
            else
            {
                MaxInfestTime = 0;
            }

            if (Electrified)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;

                npc.lifeRegen -= 4;
                if (npc.velocity != Vector2.Zero)
                    npc.lifeRegen -= 16;

                if (damage < 4)
                    damage = 4;
            }

            if (CurseoftheMoon)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;

                npc.lifeRegen -= 24;

                if (damage < 6)
                    damage = 6;
            }

            if (OceanicMaul)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;

                npc.lifeRegen -= 48;

                if (damage < 12)
                    damage = 12;
            }

            if (Sadism)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;

                npc.lifeRegen -= 170 + 48 + 60 + 8 + 4 + 16;

                if (damage < 70)
                    damage = 70;
            }

            if (MutantNibble)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;
                if (npc.lifeRegenCount > 0)
                    npc.lifeRegenCount = 0;

                if (npc.life > 0 && LifePrevious > 0) //trying to prevent some wack despawn stuff
                {
                    if (npc.life > LifePrevious)
                        npc.life = LifePrevious;
                    else
                        LifePrevious = npc.life;
                }
            }
            else
            {
                LifePrevious = npc.life;
            }

            if (GodEater)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;

                npc.lifeRegen -= 4200;

                if (damage < 777)
                    damage = 777;
            }

            if (Suffocation)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;
                npc.lifeRegen -= (int)(40f * Math.Min(1f, 1f * SuffocationTimer / 480));
                if (damage < 5)
                    damage = 5;
            }

            if (FlamesoftheUniverse)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;
                npc.lifeRegen -= 30 + 50 + 48 + 30;
                if (damage < 20)
                    damage = 20;
            }

            if (modPlayer.OriEnchant && npc.lifeRegen < 0)
            {
                int multiplier = 3;

                if (modPlayer.EarthForce || modPlayer.WizardEnchant)
                {
                    multiplier = 5;
                }

                npc.lifeRegen *= multiplier;
                damage *= multiplier;

                //half as effective if daybreak applied
                if (npc.daybreak)
                {
                    npc.lifeRegen /= 2;
                    damage /= 2;
                }
            }

            if (TimeFrozen && npc.life == 1)
            {
                if (npc.lifeRegen < 0)
                    npc.lifeRegen = 0;
            }
        }

        private int InfestedExtraDot(NPC npc)
        {
            int buffIndex = npc.FindBuffIndex(ModContent.BuffType<Infested>());
            if (buffIndex == -1)
                return 0;

            int timeLeft = npc.buffTime[buffIndex];
            if (MaxInfestTime <= 0)
                MaxInfestTime = timeLeft;
            float baseVal = (MaxInfestTime - timeLeft) / 30f; //change the denominator to adjust max power of DOT
            int dmg = (int)(baseVal * baseVal + 8);

            InfestedDust = baseVal / 15 + .5f;
            if (InfestedDust > 5f)
                InfestedDust = 5f;

            return dmg;
        }

        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();

            if (modPlayer.Bloodthirsty)
            {
                //100x spawn rate
                spawnRate = (int)(spawnRate * 0.01);
                //2x max spawn
                maxSpawns = (int)(maxSpawns * 3);
            }

            if (modPlayer.SinisterIcon)
            {
                spawnRate /= 2;
                maxSpawns *= 2;
            }

            if (modPlayer.BuilderMode)
            {
                maxSpawns = 0;
            }
        }

        private bool firstLoot = true;
        private bool firstIconLoot = true;

        public override bool PreNPCLoot(NPC npc)
        {
            Player player = Main.player[npc.lastInteraction];
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();

            if (modPlayer.NecroEnchant && player.GetToggleValue("Necro") && !npc.boss && modPlayer.NecroCD == 0 && player.ownedProjectileCounts[ModContent.ProjectileType<NecroGrave>()] < 5)
            {
                Projectile.NewProjectile(npc.Center, new Vector2(0, -3), ModContent.ProjectileType<NecroGrave>(), 0, 0, player.whoAmI, npc.lifeMax / 4);

                if (modPlayer.ShadowForce || modPlayer.WizardEnchant)
                {
                    modPlayer.NecroCD = 15;
                }
                else
                {
                    modPlayer.NecroCD = 30;
                }
            }

            if (firstIconLoot)
            {
                firstIconLoot = false;

                if ((modPlayer.MasochistSoul || npc.life <= 2000) && !npc.boss && modPlayer.SinisterIconDrops)
                {
                    if (!modPlayer.MasochistSoul && npc.value > 1)
                        npc.value = 1;

                    npc.NPCLoot();
                }
            }

            return true;
        }

        public override void NPCLoot(NPC npc)
        {
            Player player = Main.player[npc.lastInteraction];
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();

            if (modPlayer.PlatinumEnchant && !npc.boss && firstLoot)
            {
                int chance = 5;
                int bonus = 2;

                if (modPlayer.WillForce || modPlayer.WizardEnchant)
                {
                    bonus = 5;
                }

                if (Main.rand.Next(chance) == 0)
                {
                    firstLoot = false;
                    for (int i = 1; i < bonus; i++)
                    {
                        npc.NPCLoot();
                    }

                    int num1 = 36;
                    for (int index1 = 0; index1 < num1; ++index1)
                    {
                        Vector2 vector2_1 = (Vector2.Normalize(npc.velocity) * new Vector2((float)npc.width / 2f, (float)npc.height) * 0.75f).RotatedBy((double)(index1 - (num1 / 2 - 1)) * 6.28318548202515 / (double)num1, new Vector2()) + npc.Center;
                        Vector2 vector2_2 = vector2_1 - npc.Center;
                        int index2 = Dust.NewDust(vector2_1 + vector2_2, 0, 0, DustID.PlatinumCoin, vector2_2.X * 2f, vector2_2.Y * 2f, 100, new Color(), 1.4f);
                        Main.dust[index2].noGravity = true;
                        Main.dust[index2].noLight = true;
                        Main.dust[index2].velocity = vector2_2;
                    }
                }
            }

            firstLoot = false;

            //patreon gang
            if (SoulConfig.Instance.PatreonCrimetroid && npc.type == NPCID.BrainofCthulhu && Main.rand.NextBool(25))
            {
                Item.NewItem(npc.Hitbox, ModContent.ItemType<Patreon.Shucks.CrimetroidEgg>());
            }

            if (SoulConfig.Instance.PatreonOrb && npc.type == NPCID.Golem && Main.rand.NextBool(10))
            {
                Item.NewItem(npc.Hitbox, ModContent.ItemType<Patreon.Daawnz.ComputationOrb>());
            }

            if (SoulConfig.Instance.PatreonDoor && npc.type == NPCID.Squid && Main.rand.NextBool(50))
            {
                Item.NewItem(npc.Hitbox, ModContent.ItemType<Patreon.Sam.SquidwardDoor>());
            }

            if (SoulConfig.Instance.PatreonKingSlime && npc.type == NPCID.KingSlime && FargoSoulsWorld.EternityMode && Main.rand.NextBool(100))
            {
                Item.NewItem(npc.Hitbox, ModContent.ItemType<Patreon.Catsounds.MedallionoftheFallenKing>());
            }

            if (SoulConfig.Instance.PatreonPlant && npc.type == NPCID.Dryad && Main.bloodMoon && player.ZoneJungle)
            {
                Item.NewItem(npc.Hitbox, ModContent.ItemType<Patreon.LaBonez.PiranhaPlantVoodooDoll>());
            }

            if (SoulConfig.Instance.PatreonDevious && npc.type == NPCID.MoonLordCore && FargoSoulsWorld.EternityMode && Main.rand.NextBool(20))
            {
                Item.NewItem(npc.Hitbox, ModContent.ItemType<Patreon.DevAesthetic.DeviousAestheticus>());
            }

            if (SoulConfig.Instance.PatreonPrime && npc.type == NPCID.SkeletronPrime && FargoSoulsWorld.EternityMode && Main.rand.NextBool(20))
            {
                Item.NewItem(npc.Hitbox, ModContent.ItemType<Patreon.Purified.PrimeStaff>());
            }

            //boss drops
            if (Main.rand.Next(FargoSoulsWorld.EternityMode ? 3 : 10) == 0)
            {
                switch (npc.type)
                {
                    case NPCID.KingSlime:
                        Item.NewItem(npc.Hitbox, ModContent.ItemType<SlimeKingsSlasher>());
                        break;

                    case NPCID.EyeofCthulhu:
                        Item.NewItem(npc.Hitbox, ModContent.ItemType<LeashOfCthulhu>());
                        break;

                    case NPCID.EaterofWorldsHead:
                    case NPCID.EaterofWorldsBody:
                    case NPCID.EaterofWorldsTail:
                        bool dropItems = true;
                        for (int i = 0; i < 200; i++)
                        {
                            if (Main.npc[i].active && i != npc.whoAmI && (Main.npc[i].type == NPCID.EaterofWorldsHead || Main.npc[i].type == NPCID.EaterofWorldsBody || Main.npc[i].type == NPCID.EaterofWorldsTail))
                            {
                                dropItems = false;
                                break;
                            }
                        }
                        if (dropItems)
                        {
                            Item.NewItem(npc.Hitbox, ModContent.ItemType<EaterStaff>());
                        }
                        break;

                    case NPCID.BrainofCthulhu:
                        Item.NewItem(npc.Hitbox, ModContent.ItemType<BrainStaff>());
                        break;

                    case NPCID.QueenBee:
                        Item.NewItem(npc.Hitbox, ModContent.ItemType<TheSmallSting>());
                        break;

                    case NPCID.SkeletronHead:
                        Item.NewItem(npc.Hitbox, ModContent.ItemType<BoneZone>());
                        //Item.NewItem(npc.Hitbox, ModContent.ItemType<BrittleBone>(), 200);
                        break;

                    case NPCID.WallofFlesh:
                        Item.NewItem(npc.Hitbox, ModContent.ItemType<FleshHand>());
                        break;

                    case NPCID.TheDestroyer:
                        Item.NewItem(npc.Hitbox, ModContent.ItemType<DestroyerGun>());
                        break;

                    case NPCID.SkeletronPrime:
                        Item.NewItem(npc.Hitbox, ModContent.ItemType<RefractorBlaster>());
                        break;

                    case NPCID.Retinazer:
                        if (!FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.spazBoss, NPCID.Spazmatism))
                        {
                            Item.NewItem(npc.Hitbox, ModContent.ItemType<TwinRangs>());
                        }
                        break;

                    case NPCID.Spazmatism:
                        if (!FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.retiBoss, NPCID.Retinazer))
                        {
                            Item.NewItem(npc.Hitbox, ModContent.ItemType<TwinRangs>());
                        }
                        break;

                    case NPCID.Plantera:
                        Item.NewItem(npc.Hitbox, ModContent.ItemType<Dicer>());
                        break;

                    case NPCID.Golem:
                        Item.NewItem(npc.Hitbox, ModContent.ItemType<RockSlide>());
                        break;

                    case NPCID.DukeFishron:
                        Item.NewItem(npc.Hitbox, ModContent.ItemType<FishStick>());
                        break;

                    case NPCID.DD2Betsy:
                        Item.NewItem(npc.Hitbox, ModContent.ItemType<DragonBreath>());
                        break;
                }
            }

            if (npc.type == NPCID.BigMimicJungle)
            {
                switch (Main.rand.Next(3))
                {
                    case 0: Item.NewItem(npc.Hitbox, ModContent.ItemType<Vineslinger>()); break;
                    case 1: Item.NewItem(npc.Hitbox, ModContent.ItemType<Mahoguny>()); break;
                    case 2: Item.NewItem(npc.Hitbox, ModContent.ItemType<OvergrownKey>()); break;
                }
            }

            if (Fargowiltas.Instance.CalamityLoaded && Revengeance && FargoSoulsWorld.EternityMode && Main.bloodMoon && Main.moonPhase == 0 && Main.raining && Main.rand.NextBool(10))
            {
                Mod calamity = ModLoader.GetMod("CalamityMod");

                if (npc.type == calamity.NPCType("DevourerofGodsHeadS"))
                {
                    Item.NewItem(npc.Hitbox, calamity.ItemType("CosmicPlushie"));
                }
            }
        }

        public override bool CheckDead(NPC npc)
        {
            Player player = Main.player[Main.myPlayer];
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();

            if (TimeFrozen)
            {
                npc.life = 1;
                return false;
            }

            /*if (npc.boss && FargoSoulsUtil.BossIsAlive(ref mutantBoss, ModContent.NPCType<MutantBoss.MutantBoss>()) && npc.type != ModContent.NPCType<MutantBoss.MutantBoss>())
            {
                npc.active = false;
                Main.PlaySound(npc.DeathSound, npc.Center);
                return false;
            }*/

            if (Needles && npc.lifeMax > 1 && Main.rand.NextBool() && npc.type != ModContent.NPCType<SuperDummy>())
            {
                int dmg = 15;
                int numNeedles = 8;

                if (modPlayer.LifeForce || modPlayer.WizardEnchant)
                {
                    dmg = 50;
                    numNeedles = 16;
                }

                Projectile[] projs = FargoSoulsUtil.XWay(numNeedles, npc.Center, ModContent.ProjectileType<CactusNeedle>(), 5, modPlayer.HighestDamageTypeScaling(dmg), 5f);

                for (int i = 0; i < projs.Length; i++)
                {
                    if (projs[i] == null) continue;
                    Projectile p = projs[i];
                    p.GetGlobalProjectile<FargoGlobalProjectile>().CanSplit = false;
                }
            }

            return true;
        }

        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();

            if (modPlayer.CactusEnchant)
            {
                Needles = true;
            }

            /*if (Chilled)
            {
                damage =  (int)(damage * 1.25f);
            }*/
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            Player player = Main.player[projectile.owner];
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();

            if (modPlayer.CactusEnchant)
                Needles = true;

            //bees ignore defense
            /*if (modPlayer.BeeEnchant && !modPlayer.TerrariaSoul && projectile.type == ProjectileID.GiantBee)
                damage = (int)(damage + npc.defense * .5);*/

            if (modPlayer.SpiderEnchant && (projectile.minion || projectile.sentry || projectile.minionSlots > 0 || ProjectileID.Sets.MinionShot[projectile.type] || ProjectileID.Sets.SentryShot[projectile.type]) && Main.rand.Next(101) <= modPlayer.SummonCrit && player.GetToggleValue("Spider", false))
            {
                /*if (modPlayer.LifeForce || modPlayer.WizardEnchant)
                {
                    damage = (int)(damage * 1.5f);
                }*/
               
                crit = true;
            }

            if (projectile.GetGlobalProjectile<FargoGlobalProjectile>().TungstenProjectile && crit)
            {
                damage = (int)(damage * 1.1f);

                if (!crit)
                {
                    crit = Main.rand.Next(0, 100) <= modPlayer.HighestCritChance();
                }
            }

            /*if (Chilled)
            {
                damage = (int)(damage * 1.2f);
            }*/

            if (modPlayer.NecroEnchant && player.GetToggleValue("Necro") && npc.boss && player.ownedProjectileCounts[ModContent.ProjectileType<NecroGrave>()] < 5)
            {
                necroDamage += damage;

                if (necroDamage > npc.lifeMax / 10)
                {
                    necroDamage = 0;

                    Projectile.NewProjectile(npc.Center, new Vector2(0, -3), ModContent.ProjectileType<NecroGrave>(), 0, 0, player.whoAmI, npc.lifeMax / 40);
                }
            }
        }

        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
        {
            if (TimeFrozen)
                return false;
            return true;
        }

        public override void ModifyHitPlayer(NPC npc, Player target, ref int damage, ref bool crit)
        {
            FargoPlayer modPlayer = target.GetModPlayer<FargoPlayer>();

            if (target.HasBuff(ModContent.BuffType<ShellHide>()))
                damage *= 2;

            if (SoulConfig.Instance.PatreonWolf && npc.type == NPCID.Wolf && damage > target.statLife)
            {
                Item.NewItem(npc.Hitbox, ModContent.ItemType<Patreon.ParadoxWolf.ParadoxWolfSoul>());
            }
        }

        public override bool? CanBeHitByItem(NPC npc, Player player, Item item)
        {
            if (TimeFrozen && npc.life == 1)
                return false;
            return null;
        }

        public override bool? CanBeHitByProjectile(NPC npc, Projectile projectile)
        {
            if (TimeFrozen && npc.life == 1)
                return false;
            return null;
        }

        public override bool StrikeNPC(NPC npc, ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            Player player = Main.player[Main.myPlayer];
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();

            if (OceanicMaul)
            {
                damage += 15;
                //damage *= 1.3;
            }
            if (CurseoftheMoon)
            {
                damage += 5;
                //damage *= 1.1;
            }
            if (Rotting)
            {
                damage += 5;
            }

            //if (modPlayer.KnightEnchant && Villain && !npc.boss)
            //{
            //    damage *= 1.5;
            //}

            if (crit && modPlayer.ShroomEnchant && !modPlayer.TerrariaSoul && player.stealth == 0)
            {
                damage *= 1.5;
            }

            if (crit && modPlayer.Graze)
            {
                damage *= 1.0 + modPlayer.GrazeBonus;
            }

            //normal damage calc
            return true;
        }

        public override void SetupShop(int type, Chest shop, ref int nextSlot)
        {
            if (SoulConfig.Instance.PatreonRoomba && type == NPCID.Steampunker)
            {
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<Patreon.Gittle.RoombaPet>());
                shop.item[nextSlot].shopCustomPrice = 50000;
                nextSlot++;
            }
        }

        public static void DropEnches(NPC npc, int forceType, bool dropPerPlayer = false)
        {
            int max = 1;
            if (Main.expertMode)
                max++;
            if (FargoSoulsWorld.EternityMode)
                max++;

            RecipeFinder finder = new RecipeFinder();
            finder.SetResult(forceType);
            Recipe exactRecipe = finder.SearchRecipes()[0];

            List<int> enches = new List<int>();
            foreach (Item material in exactRecipe.requiredItem)
            {
                if (material.Name.EndsWith(Language.GetTextValue("Mods.FargowiltasSouls.GlobalNPC.EnchesEnd")))
                    enches.Add(material.type);
            }

            while (enches.Count > max)
                enches.RemoveAt(Main.rand.Next(enches.Count));

            foreach (int itemType in enches)
            {
                if (!dropPerPlayer || Main.netMode == NetmodeID.SinglePlayer)
                    Item.NewItem(npc.position, npc.Size, itemType);
                else if (Main.netMode == NetmodeID.Server)
                    npc.DropItemInstanced(npc.position, npc.Size, itemType);
            }
        }
    }
}
