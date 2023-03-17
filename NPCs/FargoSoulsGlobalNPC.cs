using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.Buffs.Souls;
using FargowiltasSouls.ItemDropRules.Conditions;
using FargowiltasSouls.Items.Accessories.Enchantments;
using FargowiltasSouls.Items.Accessories.Masomode;
using FargowiltasSouls.Items.Weapons.BossDrops;
using FargowiltasSouls.Items.Weapons.Misc;
using FargowiltasSouls.NPCs.EternityMode;
using FargowiltasSouls.Projectiles.Masomode;
using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.NPCs
{
    public class FargoSoulsGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public static int boss = -1;

        public int originalDefense;
        public bool BrokenArmor;

        public bool FirstTick;
        //        //debuffs
        public bool OriPoison;
        public bool SBleed;
        //        public bool Shock;
        public bool Rotting;
        public bool LeadPoison;
        public bool Needled;
        public bool SolarFlare;
        public bool TimeFrozen;
        public bool HellFire;
        public bool Infested;
        public int MaxInfestTime;
        public float InfestedDust;
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
        //        public bool Villain;
        public bool FlamesoftheUniverse;
        public bool Lethargic;
        public int LethargicCounter;
        //        public bool ExplosiveCritter = false;
        //        private int critterCounter = 120;

        public bool SnowChilled;
        public int SnowChilledTimer;

        public bool Chilled;
        public bool Smite;
        public bool MoltenAmplify;
        public bool Anticoagulation;
        public bool BloodDrinker;

        public int NecroDamage;

        public bool PungentGazeWasApplied;
        public int PungentGazeTime;

        public int GrazeCD;

        //        public static bool Revengeance => CalamityMod.World.CalamityWorld.revenge;

        static HashSet<int> RareNPCs = new HashSet<int>();

        public override void Unload()
        {
            base.Unload();
            RareNPCs = null;
        }

        public override void ResetEffects(NPC npc)
        {
            BrokenArmor = false;
            TimeFrozen = false;
            SBleed = false;
            //            Shock = false;
            Rotting = false;
            LeadPoison = false;
            SolarFlare = false;
            HellFire = false;
            OriPoison = false;
            Infested = false;
            Electrified = false;
            CurseoftheMoon = false;
            Sadism = false;
            OceanicMaul = false;
            MutantNibble = false;
            GodEater = false;
            Suffocation = false;
            //            //SnowChilled = false;
            Chilled = false;
            Smite = false;
            MoltenAmplify = false;
            Anticoagulation = false;
            BloodDrinker = false;
            FlamesoftheUniverse = false;
            PungentGazeTime = 0;
        }

        public override void SetDefaults(NPC npc)
        {
            if (npc.rarity > 0 && !RareNPCs.Contains(npc.type))
                RareNPCs.Add(npc.type);
        }

        public override bool PreAI(NPC npc)
        {
            if (npc.boss || npc.type == NPCID.EaterofWorldsHead)
                boss = npc.whoAmI;

            bool retval = base.PreAI(npc);

            if (TimeFrozen)
            {
                npc.position = npc.oldPosition;
                npc.frameCounter = 0;
                retval = false;
            }

            if (!FirstTick)
            {
                originalDefense = npc.defense;


                //                switch (npc.type)
                //                {
                //                    case NPCID.TheDestroyer:
                //                    case NPCID.TheDestroyerBody:
                //                    case NPCID.TheDestroyerTail:
                //                        npc.buffImmune[ModContent.BuffType<TimeFrozen>()] = false;
                //                        npc.buffImmune[ModContent.BuffType<Frozen>()] = false;
                //                        //npc.buffImmune[BuffID.Darkness] = false;
                //                        break;

                //                    /*case NPCID.WallofFlesh:
                //                    case NPCID.WallofFleshEye:
                //                    case NPCID.MoonLordCore:
                //                    case NPCID.MoonLordHand:
                //                    case NPCID.MoonLordHead:
                //                    case NPCID.MoonLordLeechBlob:
                //                    case NPCID.TargetDummy:
                //                    case NPCID.GolemFistLeft:
                //                    case NPCID.GolemFistRight:
                //                    case NPCID.GolemHead:
                //                    case NPCID.DungeonGuardian:
                //                    case NPCID.DukeFishron:
                //                        SpecialEnchantImmune = true;
                //                        break;*/

                //                    case NPCID.Squirrel:
                //                    case NPCID.SquirrelRed:
                //                        if (!npc.SpawnedFromStatue)
                //                        {
                //                            int p = Player.FindClosest(npc.position, npc.width, npc.height);
                //                            if ((p == -1 || npc.Distance(Main.player[p].Center) > 800) && Main.rand.NextBool(5))
                //                                npc.Transform(ModContent.NPCType<TophatSquirrelCritter>());
                //                        }
                //                        break;

                //                    default:
                //                        break;
                //                }

                //                //critters
                //                if (npc.damage == 0 && !npc.townNPC && npc.lifeMax == 5)
                //                {
                //                    Player player = Main.player[Main.myPlayer];

                //                    /*if ( npc.releaseOwner == player.whoAmI && player.GetModPlayer<FargoSoulsPlayer>().WoodEnchant)
                //                    {
                //                        switch (npc.type)
                //                        {
                //                            case NPCID.Bunny:


                //                                npc.active = false;
                //                                break;
                //                        }



                //                        ExplosiveCritter = true;
                //                    }*/
                //                }

                FirstTick = true;
            }

            if (Lethargic && ++LethargicCounter > 3)
            {
                LethargicCounter = 0;
                retval = false;
            }

            //            if (ExplosiveCritter)
            //            {
            //                critterCounter--;

            //                if (critterCounter <= 0)
            //                {
            //                    Player player = Main.player[npc.releaseOwner];
            //                    FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            //                    int damage = 25;

            //                    if (modPlayer.WoodForce || modPlayer.WizardEnchant)
            //                    {
            //                        damage *= 5;
            //                    }

            //                    Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<ExplosionSmall>(), modPlayer.HighestDamageTypeScaling(damage), 4, npc.releaseOwner);
            //                    //gold critters make coin value go up of hit enemy, millions of other effects eeech
            //                }

            //            }

            if (SnowChilled)
            {
                SnowChilledTimer--;

                if (SnowChilledTimer <= 0)
                    SnowChilled = false;

                if (SnowChilledTimer % 3 == 1)
                {
                    npc.position = npc.oldPosition;
                    retval = false;
                }
            }

            return retval;
        }

        public override void PostAI(NPC npc)
        {
            if (BrokenArmor)
            {
                npc.defense = originalDefense - 10;
            }

            if (SnowChilled)
            {
                int dustId = Dust.NewDust(npc.position, npc.width, npc.height, 76, npc.velocity.X, npc.velocity.Y, 100, default(Color), 1f);
                Main.dust[dustId].noGravity = true;

                npc.position -= npc.velocity * 0.5f;
            }

            SuffocationTimer += Suffocation ? 1 : -3;
            if (SuffocationTimer < 0)
                SuffocationTimer = 0;

            if (!npc.friendly && npc.damage > 0
                && Main.LocalPlayer.active && !Main.LocalPlayer.dead)
            {
                if (--GrazeCD < 0) //managed by the npc itself so worm segments dont make it count down faster
                    GrazeCD = 6;

                NPC realLifeNPC = FargoSoulsUtil.NPCExists(npc.realLife);
                FargoSoulsGlobalNPC npcForGrazeCD = realLifeNPC is NPC ? realLifeNPC.GetGlobalNPC<FargoSoulsGlobalNPC>() : npc.GetGlobalNPC<FargoSoulsGlobalNPC>();

                if (npcForGrazeCD.GrazeCD == 0)
                {
                    FargoSoulsPlayer fargoPlayer = Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>();
                    if (fargoPlayer.Graze && !Main.LocalPlayer.immune && Main.LocalPlayer.hurtCooldowns[0] <= 0 && Main.LocalPlayer.hurtCooldowns[1] <= 0)
                    {
                        Vector2 point = FargoSoulsUtil.ClosestPointInHitbox(npc.Hitbox, Main.LocalPlayer.Center);
                        int dummy = -1;
                        if (Main.LocalPlayer.Distance(point) < fargoPlayer.GrazeRadius
                            && NPCLoader.CanHitPlayer(npc, Main.LocalPlayer, ref dummy)
                            && (npc.ModNPC == null ? true : npc.ModNPC.CanHitPlayer(Main.LocalPlayer, ref dummy))
                            && (npc.noTileCollide || Collision.CanHitLine(point, 0, 0, Main.LocalPlayer.Center, 0, 0)))
                        {
                            npcForGrazeCD.GrazeCD = 30;

                            if (fargoPlayer.DeviGraze)
                                SparklingAdoration.OnGraze(fargoPlayer, npc.damage);
                            if (fargoPlayer.CirnoGraze)
                                IceQueensCrown.OnGraze(fargoPlayer, npc.damage);
                        }
                    }
                }
            }
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
                    int dust = Dust.NewDust(new Vector2(npc.position.X - 2f, npc.position.Y - 2f), npc.width + 4, npc.height + 4, DustID.PinkTorch, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default(Color), 1f);
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

            //            /*if (Infested)
            //            {
            //                if (Main.rand.Next(4) < 3)
            //                {
            //                    int dust = Dust.NewDust(new Vector2(npc.position.X - 2f, npc.position.Y - 2f), npc.width + 4, npc.height + 4, 44, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, Color.LimeGreen, InfestedDust);
            //                    Main.dust[dust].noGravity = true;
            //                    Main.dust[dust].velocity *= 1.8f;
            //                    Dust expr_1CCF_cp_0 = Main.dust[dust];
            //                    expr_1CCF_cp_0.velocity.Y = expr_1CCF_cp_0.velocity.Y - 0.5f;
            //                    if (Main.rand.NextBool(4))
            //                    {
            //                        Main.dust[dust].noGravity = false;
            //                        Main.dust[dust].scale *= 0.5f;
            //                    }
            //                }

            //                Lighting.AddLight((int)(npc.position.X / 16f), (int)(npc.position.Y / 16f + 1f), 1f, 0.3f, 0.1f);
            //            }*/

            if (Suffocation)
                drawColor = Colors.RarityPurple;

            //            if (Villain)
            //            {
            //                if (Main.rand.Next(4) < 3)
            //                {
            //                    int dust = Dust.NewDust(new Vector2(npc.position.X - 2f, npc.position.Y - 2f), npc.width + 4, npc.height + 4, DustID.AncientLight, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100);
            //                    Main.dust[dust].noGravity = true;
            //                    Main.dust[dust].velocity *= 1.8f;
            //                    Dust expr_1CCF_cp_0 = Main.dust[dust];
            //                    expr_1CCF_cp_0.velocity.Y = expr_1CCF_cp_0.velocity.Y - 0.5f;
            //                    if (Main.rand.NextBool(4))
            //                    {
            //                        Main.dust[dust].noGravity = false;
            //                        Main.dust[dust].scale *= 0.5f;
            //                    }
            //                }

            //                Lighting.AddLight((int)(npc.position.X / 16f), (int)(npc.position.Y / 16f + 1f), 1f, 0.3f, 0.1f);
            //            }

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

            if (Smite)
            {
                if (!Main.rand.NextBool(4))
                {
                    Color color = Main.DiscoColor;
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, 91, 0.0f, 0.0f, 100, color, 2.5f);
                    Main.dust[d].velocity *= 2f;
                    Main.dust[d].noGravity = true;
                }
            }

            if (Anticoagulation)
            {
                if (!Main.rand.NextBool(4))
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood);
                    Main.dust[d].velocity *= 2f;
                    Main.dust[d].scale += 1f;
                }
            }

            if (BloodDrinker)
            {
                if (!Main.rand.NextBool(3))
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, DustID.LifeDrain, npc.velocity.X * 0.2f, npc.velocity.Y * 0.2f, 0, Color.White, 2.5f);
                    Main.dust[d].noGravity = true;
                }
            }

            if (PungentGazeTime > 0)
            {
                if (Main.rand.NextBool())
                {
                    float ratio = (float)PungentGazeTime / PungentGaze.MAX_TIME;
                    int d = Dust.NewDust(npc.Center, 0, 0, DustID.GemRuby, npc.velocity.X * 0.2f, npc.velocity.Y * 0.2f, 0, Color.White);
                    Main.dust[d].scale = MathHelper.Lerp(0.5f, 3f, ratio);
                    Main.dust[d].velocity *= Main.dust[d].scale;
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
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

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

                int dot = npc.type == NPCID.EaterofWorldsBody ? 4 : 20;

                if (modPlayer.TerraForce)
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
                if (npc.wet)
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
                npc.lifeRegen -= (30 + 50 + 48 + 30) / 2;
                if (damage < 20)
                    damage = 20;
            }

            if (Anticoagulation)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;
                npc.lifeRegen -= 16;
                if (damage < 6)
                    damage = 6;
            }

            if (modPlayer.OriEnchantItem != null && npc.lifeRegen < 0)
            {
                OrichalcumEnchant.OriDotModifier(npc, modPlayer, ref damage);
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
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

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

            //if (modPlayer.BuilderMode) maxSpawns = 0;
        }

        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.GetModPlayer<FargoSoulsPlayer>().PungentEyeball)
            {
                foreach (var entry in pool)
                {
                    if (RareNPCs.Contains(entry.Key))
                    {
                        pool[entry.Key] = entry.Value * 5;
                    }
                }
            }
        }

        public override bool PreKill(NPC npc)
        {
            Player player = Main.player[npc.lastInteraction];
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            if (modPlayer.NecroEnchantActive && player.GetToggleValue("Necro") && !npc.boss)
            {
                NecroEnchant.NecroSpawnGraveEnemy(npc, player, modPlayer);
            }

            return true;
        }

        private bool lootMultiplierCheck;
        private static int[] illegalLootMultiplierNPCs => new int[] {
            NPCID.DD2Betsy,
            NPCID.EaterofWorldsBody,
            NPCID.EaterofWorldsHead,
            NPCID.EaterofWorldsTail
        };

        public override void OnKill(NPC npc)
        {
            Player player = Main.player[npc.lastInteraction];
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            if (!lootMultiplierCheck)
            {
                lootMultiplierCheck = true;

                if (modPlayer.SinisterIconDrops && !npc.boss && !illegalLootMultiplierNPCs.Contains(npc.type))
                {
                    npc.NPCLoot();
                }

                if (modPlayer.PlatinumEnchantActive && !npc.boss && Main.rand.NextBool(5) && !illegalLootMultiplierNPCs.Contains(npc.type))
                {
                    npc.extraValue /= 5;

                    for (int i = 0; i < 4; i++)
                        npc.NPCLoot();
                }
            }

            if (npc.boss && !FargoSoulsWorld.downedAnyBoss)
            {
                FargoSoulsWorld.downedAnyBoss = true;
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.WorldData);
            }
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            IItemDropRule BossDrop(int item)
            {
                return new DropBasedOnEMode(ItemDropRule.Common(item, 3), ItemDropRule.Common(item, 10));
            }

            switch (npc.type)
            {
                case NPCID.KingSlime:
                    npcLoot.Add(BossDrop(ModContent.ItemType<SlimeKingsSlasher>()));
                    break;

                case NPCID.EyeofCthulhu:
                    npcLoot.Add(BossDrop(ModContent.ItemType<LeashOfCthulhu>()));
                    break;

                case NPCID.EaterofWorldsHead:
                case NPCID.EaterofWorldsBody:
                case NPCID.EaterofWorldsTail:
                    {
                        LeadingConditionRule lastEater = new LeadingConditionRule(new Conditions.LegacyHack_IsABoss());
                        lastEater.OnSuccess(BossDrop(ModContent.ItemType<EaterStaff>()));
                        npcLoot.Add(lastEater);
                    }
                    break;

                case NPCID.BrainofCthulhu:
                    npcLoot.Add(BossDrop(ModContent.ItemType<BrainStaff>()));
                    break;

                case NPCID.QueenBee:
                    npcLoot.Add(BossDrop(ModContent.ItemType<TheSmallSting>()));
                    break;

                case NPCID.SkeletronHead:
                    npcLoot.Add(BossDrop(ModContent.ItemType<BoneZone>()));
                    //npcLoot.Add(BossDrop(ModContent.ItemType<BrittleBone>(), 200));
                    break;

                case NPCID.WallofFlesh:
                    npcLoot.Add(BossDrop(ModContent.ItemType<FleshHand>()));
                    break;

                case NPCID.TheDestroyer:
                    npcLoot.Add(BossDrop(ModContent.ItemType<DestroyerGun>()));
                    break;

                case NPCID.SkeletronPrime:
                    npcLoot.Add(BossDrop(ModContent.ItemType<RefractorBlaster>()));
                    break;

                case NPCID.Retinazer:
                case NPCID.Spazmatism:
                    {
                        LeadingConditionRule noTwin = new LeadingConditionRule(new Conditions.MissingTwin());
                        noTwin.OnSuccess(BossDrop(ModContent.ItemType<TwinRangs>()));
                        npcLoot.Add(noTwin);
                    }
                    break;

                case NPCID.Plantera:
                    npcLoot.Add(BossDrop(ModContent.ItemType<Dicer>()));
                    break;

                case NPCID.Golem:
                    npcLoot.Add(BossDrop(ModContent.ItemType<RockSlide>()));
                    break;

                case NPCID.DukeFishron:
                    npcLoot.Add(BossDrop(ModContent.ItemType<FishStick>()));
                    break;

                case NPCID.DD2Betsy:
                    npcLoot.Add(BossDrop(ModContent.ItemType<DragonBreath>()));
                    break;

                case NPCID.BigMimicJungle:
                    npcLoot.Add(ItemDropRule.OneFromOptions(1,
                        ModContent.ItemType<Vineslinger>(),
                        ModContent.ItemType<Mahoguny>(),
                        ModContent.ItemType<OvergrownKey>()));
                    break;

                default:
                    break;
            }

            //if (Fargowiltas.Instance.CalamityLoaded && Revengeance && FargoSoulsWorld.EternityMode && Main.bloodMoon && Main.moonPhase == 0 && Main.raining && Main.rand.NextBool(10))
            //{
            //    Mod calamity = ModLoader.GetMod("CalamityMod");

            //    if (npc.type == calamity.NPCType("DevourerofGodsHeadS"))
            //    {
            //        Item.NewItem(npc.Hitbox, calamity.ItemType("CosmicPlushie"));
            //    }
            //}
        }

        public override bool CheckDead(NPC npc)
        {
            if (TimeFrozen)
            {
                npc.life = 1;
                return false;
            }

            Player player = FargoSoulsUtil.PlayerExists(npc.lastInteraction);
            if (player == null)
                return base.CheckDead(npc);

            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            //            /*if (npc.boss && FargoSoulsUtil.BossIsAlive(ref mutantBoss, ModContent.NPCType<MutantBoss.MutantBoss>()) && npc.type != ModContent.NPCType<MutantBoss.MutantBoss>())
            //            {
            //                npc.active = false;
            //                SoundEngine.PlaySound(npc.DeathSound, npc.Center);
            //                return false;
            //            }*/

            if (modPlayer.WoodEnchantItem != null)
            {
                WoodEnchant.WoodCheckDead(modPlayer, npc);
            }

            if (Needled && npc.lifeMax > 1 && npc.lifeMax != int.MaxValue) //super dummy
            {
                CactusEnchant.CactusProc(npc, player);
            }

            return base.CheckDead(npc);
        }

        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            ModifyHitByBoth(npc, player, ref damage);

            //            /*if (Chilled)
            //            {
            //                damage =  (int)(damage * 1.25f);
            //            }*/
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            Player player = Main.player[projectile.owner];
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            ModifyHitByBoth(npc, player, ref damage);
        }

        public void ModifyHitByBoth(NPC npc, Player player, ref int damage)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            if (modPlayer.NecroEnchantActive && player.GetToggleValue("Necro") && npc.boss)
            {
                NecroEnchant.NecroSpawnGraveBoss(this, npc, player, damage);
            }
        }

        public override void OnHitByItem(NPC npc, Player player, Item item, int damage, float knockback, bool crit)
        {
            OnHitByEither(npc, player, damage, knockback, crit);
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit)
        {
            OnHitByEither(npc, Main.player[projectile.owner], damage, knockback, crit);
        }

        public void OnHitByEither(NPC npc, Player player, int damage, float knockback, bool crit)
        {
            if (Anticoagulation && player.whoAmI == Main.myPlayer)
            {
                int type = ModContent.ProjectileType<Bloodshed>();
                if (Main.rand.NextBool(player.ownedProjectileCounts[type] + 2))
                {
                    const float speed = 12f;
                    Projectile.NewProjectile(npc.GetSource_OnHurt(player), npc.Center, Main.rand.NextVector2Circular(speed, speed), type, 0, 0f, Main.myPlayer, 1f);
                }
            }
        }

        public override bool CanHitPlayer(NPC npc, Player target, ref int CooldownSlot)
        {
            if (TimeFrozen)
                return false;
            return true;
        }

        public override void ModifyHitPlayer(NPC npc, Player target, ref int damage, ref bool crit)
        {
            if (target.HasBuff(ModContent.BuffType<ShellHide>()))
                damage *= 2;

            if (BloodDrinker)
                damage = (int)(damage * 1.3);
        }

        public override void ModifyHitNPC(NPC npc, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            if (target.type == ModContent.NPCType<CreeperGutted>())
                damage /= 20;
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
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            if (Smite)
            {
                damage *= 1.1;
            }

            if (MoltenAmplify)
            {
                damage *= 1.25;
            }

            if (PungentGazeTime > 0)
            {
                damage *= 1.0 + 0.15 * PungentGazeTime / PungentGaze.MAX_TIME;
            }

            //            //if (modPlayer.KnightEnchant && Villain && !npc.boss)
            //            //{
            //            //    damage *= 1.5;
            //            //}

            //            if (crit && modPlayer.ShroomEnchant && !modPlayer.TerrariaSoul && player.stealth == 0)
            //            {
            //                damage *= 1.5;
            //            }

            if (modPlayer.DeviGraze)
            {
                damage *= 1.0 + modPlayer.DeviGrazeBonus;
            }

            //            //normal damage calc
            return true;
        }

        public override void SetupShop(int type, Chest shop, ref int nextSlot)
        {
            Player player = Main.player[Main.myPlayer];
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            if (modPlayer.WoodEnchantDiscount)
            {
                WoodEnchant.WoodDiscount(shop);
            }
        }

    }
}
