using System.IO;
using Terraria.ModLoader.IO;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.ItemDropRules.Conditions;
using FargowiltasSouls.Items.Accessories.Masomode;
using FargowiltasSouls.NPCs;
using FargowiltasSouls.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Boss.PHM
{
    public class EaterofWorlds : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(NPCID.EaterofWorldsHead, NPCID.EaterofWorldsBody, NPCID.EaterofWorldsTail);

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            npc.buffImmune[BuffID.CursedInferno] = true;
        }

        public override bool CheckDead(NPC npc)
        {
            if (FargoSoulsWorld.SwarmActive)
                return base.CheckDead(npc);

            int count = 0;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && i != npc.whoAmI && (Main.npc[i].type == NPCID.EaterofWorldsHead || Main.npc[i].type == NPCID.EaterofWorldsBody || Main.npc[i].type == NPCID.EaterofWorldsTail))
                    count++;
            }

            if (count > 2)
                return false;

            return base.CheckDead(npc);
        }

        public override void SafeModifyHitByItem(NPC npc, Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            base.SafeModifyHitByItem(npc, player, item, ref damage, ref knockback, ref crit);

            if (EaterofWorldsHead.HaveSpawnDR > 0)
                damage /= 10;
        }

        public override void SafeModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            base.SafeModifyHitByProjectile(npc, projectile, ref damage, ref knockback, ref crit, ref hitDirection);

            if (EaterofWorldsHead.HaveSpawnDR > 0)
                damage /= projectile.numHits + 1;
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            LeadingConditionRule emodeRule = new LeadingConditionRule(new EModeDropCondition());
            LeadingConditionRule lastEater = new LeadingConditionRule(new Conditions.LegacyHack_IsABoss());
            emodeRule.OnSuccess(lastEater);
            lastEater.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<DarkenedHeart>()));
            lastEater.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ItemID.CorruptFishingCrate, 5));

            //to make up for no loot until dead
            lastEater.OnSuccess(ItemDropRule.Common(ItemID.ShadowScale, 1, 60, 60));
            lastEater.OnSuccess(ItemDropRule.Common(ItemID.DemoniteOre, 1, 200, 200));

            npcLoot.Add(emodeRule);
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(BuffID.CursedInferno, 180);
            target.AddBuff(ModContent.BuffType<Rotting>(), 600);
        }

        public override void LoadSprites(NPC npc, bool recolor)
        {
            base.LoadSprites(npc, recolor);

            LoadNPCSprite(recolor, npc.type);
        }
    }

    public class EaterofWorldsHead : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.EaterofWorldsHead);

        public int FlamethrowerCDOrUTurnStoredTargetX;
        public int UTurnTotalSpacingDistance;
        public int UTurnIndividualSpacingPosition;
        public int UTurnAITimer;
        public static int UTurnCountdownTimer;
        public static int CursedFlameTimer;
        public static int HaveSpawnDR;

        public bool UTurn;
        public static bool DoTheWave;

        public bool DroppedSummon;

        public int NoSelfDestructTimer = 15;


        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            binaryWriter.Write7BitEncodedInt(FlamethrowerCDOrUTurnStoredTargetX);
            binaryWriter.Write7BitEncodedInt(UTurnTotalSpacingDistance);
            binaryWriter.Write7BitEncodedInt(UTurnIndividualSpacingPosition);
            binaryWriter.Write7BitEncodedInt(UTurnAITimer);
            binaryWriter.Write7BitEncodedInt(UTurnCountdownTimer);
            binaryWriter.Write7BitEncodedInt(CursedFlameTimer);
            bitWriter.WriteBit(UTurn);
            bitWriter.WriteBit(DoTheWave);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            FlamethrowerCDOrUTurnStoredTargetX = binaryReader.Read7BitEncodedInt();
            UTurnTotalSpacingDistance = binaryReader.Read7BitEncodedInt();
            UTurnIndividualSpacingPosition = binaryReader.Read7BitEncodedInt();
            UTurnAITimer = binaryReader.Read7BitEncodedInt();
            UTurnCountdownTimer = binaryReader.Read7BitEncodedInt();
            CursedFlameTimer = binaryReader.Read7BitEncodedInt();
            UTurn = bitReader.ReadBit();
            DoTheWave = bitReader.ReadBit();
        }

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.defense += 10;
            npc.damage = (int)(npc.damage * 4.0 / 3.0);
        }

        public override bool SafePreAI(NPC npc)
        {
            EModeGlobalNPC.eaterBoss = npc.whoAmI;
            FargoSoulsGlobalNPC.boss = npc.whoAmI;

            if (FargoSoulsWorld.SwarmActive)
                return true;

            if (!npc.HasValidTarget || npc.Distance(Main.player[npc.target].Center) > 3000)
            {
                npc.velocity.Y += 0.25f;
                if (npc.timeLeft > 120)
                    npc.timeLeft = 120;
            }

            //if (eaterResist > 0 && npc.whoAmI == NPC.FindFirstNPC(npc.type)) eaterResist--;

            int firstEater = NPC.FindFirstNPC(npc.type);

            if (npc.whoAmI == firstEater)
            {
                UTurnCountdownTimer++;
                if (HaveSpawnDR > 0)
                    HaveSpawnDR--;
            }

            if (Main.netMode != NetmodeID.MultiplayerClient && npc.whoAmI == firstEater && ++CursedFlameTimer > 300) //only let one eater increment this
            {
                bool shoot = true;
                if (!FargoSoulsWorld.MasochistModeReal)
                {
                    for (int i = 0; i < Main.maxNPCs; i++) //cancel if anyone is doing the u-turn
                    {
                        if (Main.npc[i].active && Main.npc[i].type == npc.type && Main.npc[i].GetGlobalNPC<EaterofWorldsHead>().UTurn)
                        {
                            shoot = false;
                            CursedFlameTimer -= 30;
                        }
                    }
                }

                if (shoot)
                {
                    CursedFlameTimer = 0;

                    int minimumToShoot = FargoSoulsWorld.MasochistModeReal ? 18 : 6;

                    int counter = 0;
                    int delay = 0;
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        if (Main.npc[i].active)
                        {
                            /*if (Main.npc[i].type == npc.type && !Main.npc[i].GetGlobalNPC<EaterofWorldsHead>().masobool0)
                            {
                                Main.npc[i].GetGlobalNPC<EaterofWorldsHead>().counter2 = 0; //stop others from triggering it
                            }
                            else */
                            if (Main.npc[i].type == NPCID.EaterofWorldsHead || Main.npc[i].type == NPCID.EaterofWorldsBody || Main.npc[i].type == NPCID.EaterofWorldsTail)
                            {
                                if (++counter > (FargoSoulsWorld.MasochistModeReal ? 2 : 6)) //wave of redirecting flames
                                {
                                    counter = 0;

                                    minimumToShoot--;

                                    Vector2 vel = (Main.player[npc.target].Center - Main.npc[i].Center) / 45;
                                    Projectile.NewProjectile(npc.GetSource_FromThis(), Main.npc[i].Center, vel,
                                        ModContent.ProjectileType<CursedFireballHoming>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage, 0.8f), 0f, Main.myPlayer, npc.target, delay);

                                    delay += FargoSoulsWorld.MasochistModeReal ? 4 : 10;
                                }
                            }
                        }
                    }

                    for (int i = 0; i < minimumToShoot; i++)
                    {
                        Vector2 vel = (Main.player[npc.target].Center - npc.Center) / 45;
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, vel,
                            ModContent.ProjectileType<CursedFireballHoming>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage, 0.8f), 0f, Main.myPlayer, npc.target, delay);
                        delay += FargoSoulsWorld.MasochistModeReal ? 4 : 8;
                    }
                }
            }

            if (NoSelfDestructTimer <= 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient && UTurnCountdownTimer % 6 == 3) //chose this number at random to avoid edge case
                {
                    //die if segment behind me is invalid
                    int ai0 = (int)npc.ai[0];
                    if (!(ai0 > -1 && ai0 < Main.maxNPCs && Main.npc[ai0].active && Main.npc[ai0].ai[1] == npc.whoAmI
                        && (Main.npc[ai0].type == NPCID.EaterofWorldsBody || Main.npc[ai0].type == NPCID.EaterofWorldsTail)))
                    {
                        //Main.NewText("ai0 npc invalid");
                        npc.life = 0;
                        npc.HitEffect();
                        npc.checkDead();
                        npc.active = false;
                        npc.netUpdate = false;
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                        return false;
                    }
                }
            }
            else
            {
                NoSelfDestructTimer--;
            }

            if (!UTurn)
            {
                if (++FlamethrowerCDOrUTurnStoredTargetX >= 6)
                {
                    FlamethrowerCDOrUTurnStoredTargetX = 0;
                    if (FargoSoulsWorld.MasochistModeReal && Main.netMode != NetmodeID.MultiplayerClient) //cursed flamethrower, roughly same direction as head
                    {
                        Vector2 velocity = new Vector2(5f, 0f).RotatedBy(npc.rotation - Math.PI / 2.0 + MathHelper.ToRadians(Main.rand.Next(-15, 16)));
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, velocity, ProjectileID.EyeFire, FargoSoulsUtil.ScaledProjectileDamage(npc.damage, 0.8f), 0f, Main.myPlayer);
                    }
                }

                if (npc.whoAmI == firstEater)
                {
                    if (UTurnCountdownTimer == 700 - 90) //roar telegraph
                        SoundEngine.PlaySound(SoundID.Roar, Main.player[npc.target].Center);

                    if (UTurnCountdownTimer > 700 && Main.netMode != NetmodeID.MultiplayerClient) //initiate mass u-turn
                    {
                        UTurnCountdownTimer = 0;
                        if (npc.HasValidTarget && npc.Distance(Main.player[npc.target].Center) < 2400)
                        {
                            UTurn = true;
                            DoTheWave = !DoTheWave;
                            UTurnTotalSpacingDistance = NPC.CountNPCS(npc.type) / 2;

                            int headCounter = 0; //determine position of this head in the group
                            for (int i = 0; i < Main.maxNPCs; i++) //synchronize
                            {
                                if (Main.npc[i].active && Main.npc[i].type == npc.type)
                                {
                                    Main.npc[i].GetGlobalNPC<EaterofWorldsHead>().UTurnAITimer = DoTheWave && UTurnTotalSpacingDistance != 0 ? headCounter * 90 / UTurnTotalSpacingDistance / 2 - 60 : 0;
                                    if (FargoSoulsWorld.MasochistModeReal)
                                        Main.npc[i].GetGlobalNPC<EaterofWorldsHead>().UTurnAITimer += 60;
                                    Main.npc[i].GetGlobalNPC<EaterofWorldsHead>().UTurnTotalSpacingDistance = UTurnTotalSpacingDistance;
                                    Main.npc[i].GetGlobalNPC<EaterofWorldsHead>().UTurnIndividualSpacingPosition = headCounter;
                                    Main.npc[i].GetGlobalNPC<EaterofWorldsHead>().UTurn = true;

                                    Main.npc[i].netUpdate = true;
                                    NetSync(Main.npc[i]);

                                    headCounter *= -1; //alternate 0, 1, -1, 2, -2, 3, -3, etc.
                                    if (headCounter >= 0)
                                        headCounter++;
                                }
                            }

                            npc.netUpdate = true;
                        }
                    }
                }
            }
            else //flying u-turn ai
            {
                if (++UTurnAITimer < 120)
                {
                    Vector2 target = Main.player[npc.target].Center;
                    if (UTurnTotalSpacingDistance != 0)
                        target.X += 900f / UTurnTotalSpacingDistance * UTurnIndividualSpacingPosition; //space out
                    target.Y += 600f;

                    float speedModifier = 0.6f;
                    float speedCap = 24;
                    if (npc.Top.Y > Main.player[npc.target].Bottom.Y + npc.height)
                    {
                        speedModifier *= 1.5f;
                        speedCap *= 1.5f;
                        npc.position += (Main.player[npc.target].position - Main.player[npc.target].oldPosition) / 2;
                    }

                    if (npc.Center.X < target.X)
                    {
                        npc.velocity.X += speedModifier;
                        if (npc.velocity.X < 0)
                            npc.velocity.X += speedModifier * 2;
                    }
                    else
                    {
                        npc.velocity.X -= speedModifier;
                        if (npc.velocity.X > 0)
                            npc.velocity.X -= speedModifier * 2;
                    }
                    if (npc.Center.Y < target.Y)
                    {
                        npc.velocity.Y += speedModifier;
                        if (npc.velocity.Y < 0)
                            npc.velocity.Y += speedModifier * 2;
                    }
                    else
                    {
                        npc.velocity.Y -= speedModifier;
                        if (npc.velocity.Y > 0)
                            npc.velocity.Y -= speedModifier * 2;
                    }

                    if (Math.Abs(npc.velocity.X) > speedCap)
                        npc.velocity.X = speedCap * Math.Sign(npc.velocity.X);
                    if (Math.Abs(npc.velocity.Y) > speedCap)
                        npc.velocity.Y = speedCap * Math.Sign(npc.velocity.Y);

                    npc.localAI[0] = 1f;

                    if (Main.netMode == NetmodeID.Server && --npc.netSpam < 0) //manual mp sync control
                    {
                        npc.netSpam = 5;
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                    }
                }
                else if (UTurnAITimer == 120) //fly up
                {
                    SoundEngine.PlaySound(SoundID.Roar, Main.player[npc.target].Center);
                    npc.velocity = Vector2.UnitY * -15f;
                    FlamethrowerCDOrUTurnStoredTargetX = (int)Main.player[npc.target].Center.X; //store their initial location

                    npc.netUpdate = true;
                }
                else if (UTurnAITimer < 240) //cancel early and turn once we fly past player
                {
                    if (npc.Center.Y < Main.player[npc.target].Center.Y - (FargoSoulsWorld.MasochistModeReal ? 200 : 450))
                        UTurnAITimer = 239;
                }
                else if (UTurnAITimer == 240) //recalculate velocity to u-turn and dive back down in the same spacing over player
                {
                    Vector2 target;
                    target.X = Main.player[npc.target].Center.X;
                    if (UTurnTotalSpacingDistance != 0)
                        target.X += 900f / UTurnTotalSpacingDistance * UTurnIndividualSpacingPosition; //space out
                    target.Y = npc.Center.Y;

                    float radius = Math.Abs(target.X - npc.Center.X) / 2;
                    float speed = MathHelper.Pi * radius / 30;
                    if (speed < 8f)
                        speed = 8f;
                    npc.velocity = Vector2.Normalize(npc.velocity) * speed;

                    FlamethrowerCDOrUTurnStoredTargetX = Math.Sign(Main.player[npc.target].Center.X - FlamethrowerCDOrUTurnStoredTargetX); //which side player moved to from original pos

                    npc.netUpdate = true;
                }
                else if (UTurnAITimer < 270) //u-turn
                {
                    npc.velocity = npc.velocity.RotatedBy(MathHelper.ToRadians(6f) * FlamethrowerCDOrUTurnStoredTargetX);
                }
                else if (UTurnAITimer == 270)
                {
                    npc.velocity = Vector2.Normalize(npc.velocity) * 15f;
                    npc.netUpdate = true;
                }
                else if (UTurnAITimer > 300)
                {
                    UTurnAITimer = 0;
                    UTurnCountdownTimer = 0;
                    UTurnTotalSpacingDistance = 0;
                    UTurnIndividualSpacingPosition = 0;
                    UTurn = false;

                    //for (int i = 0; i < Main.maxNPCs; i++)
                    //{
                    //    if (Main.npc[i].active)
                    //    {
                    //        if (Main.npc[i].type == npc.type)
                    //        {
                    //            Main.npc[i].GetGlobalNPC<EaterofWorldsHead>().UTurnTotalSpacingDistance = 0;
                    //            Main.npc[i].GetGlobalNPC<EaterofWorldsHead>().UTurnIndividualSpacingPosition = 0;
                    //            Main.npc[i].GetGlobalNPC<EaterofWorldsHead>().UTurn = false;
                    //            Main.npc[i].netUpdate = true;
                    //            if (Main.netMode == NetmodeID.Server)
                    //                NetSync(npc);
                    //        }
                    //        else if (Main.npc[i].type == NPCID.EaterofWorldsBody || Main.npc[i].type == NPCID.EaterofWorldsTail)
                    //        {
                    //            Main.npc[i].netUpdate = true;
                    //        }
                    //    }
                    //}

                    npc.netUpdate = true;
                }

                npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) + 1.57f;

                if (npc.netUpdate)
                {
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                        NetSync(npc);
                    }
                    npc.netUpdate = false;
                }
                return false;
            }

            //drop summon
            if (npc.HasPlayerTarget && !DroppedSummon)
            {
                Player player = Main.player[npc.target];

                //eater meme
                if (!player.dead && player.GetModPlayer<FargoSoulsPlayer>().FreeEaterSummon)
                {
                    player.GetModPlayer<FargoSoulsPlayer>().FreeEaterSummon = false;

                    if (!NPC.downedBoss2 && Main.netMode != NetmodeID.MultiplayerClient && ModContent.TryFind("Fargowiltas", "WormyFood", out ModItem modItem))
                        Item.NewItem(npc.GetSource_Loot(), player.Hitbox, modItem.Type);

                    DroppedSummon = true;
                    UTurnCountdownTimer = 0;
                    HaveSpawnDR = 180;
                    npc.velocity.Y += 6;
                }
            }

            return true;
        }

        public override bool StrikeNPC(NPC npc, ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            damage /= 2;
            return base.StrikeNPC(npc, ref damage, defense, ref knockback, hitDirection, ref crit);
        }

        public override void LoadSprites(NPC npc, bool recolor)
        {
            base.LoadSprites(npc, recolor);

            LoadBossHeadSprite(recolor, 2);
            LoadGoreRange(recolor, 24, 29);
        }
    }

    public class EaterofWorldsSegment : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(NPCID.EaterofWorldsBody, NPCID.EaterofWorldsTail);

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.damage *= 2;
        }

        public override bool StrikeNPC(NPC npc, ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if (npc.type == NPCID.EaterofWorldsBody)
            {
                NPC head = FargoSoulsUtil.NPCExists(npc.ai[1], NPCID.EaterofWorldsHead);
                if (head != null) //segment directly behind head takes less damage too
                    damage /= 2;
            }

            return base.StrikeNPC(npc, ref damage, defense, ref knockback, hitDirection, ref crit);
        }

        public override bool CheckDead(NPC npc)
        {
            //no loot unless every other segment is dead (doesn't apply during swarms - if swarm, die and drop loot normally)
            if (!FargoSoulsWorld.SwarmActive && Main.npc.Any(n => n.active && n.whoAmI != npc.whoAmI && (n.type == NPCID.EaterofWorldsBody || n.type == NPCID.EaterofWorldsHead || n.type == NPCID.EaterofWorldsTail)))
            {
                npc.active = false;
                if (npc.DeathSound != null)
                    SoundEngine.PlaySound(npc.DeathSound.Value, npc.Center);
                return false;
            }

            return base.CheckDead(npc);
        }
    }

    public class VileSpitEaterofWorlds : Enemy.Corruption.VileSpit
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.VileSpitEaterOfWorlds);

        public int SuicideCounter;

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.scale *= 2;

            if (FargoSoulsWorld.MasochistModeReal)
                npc.dontTakeDamage = true;
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (++SuicideCounter > 600)
                npc.StrikeNPCNoInteraction(9999, 0f, 0);
        }

        public override void OnKill(NPC npc)
        {
            base.OnKill(npc);

            if (FargoSoulsWorld.MasochistModeReal && Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < 8; i++)
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.UnitY.RotatedBy(2 * Math.PI / 8 * i) * 4f, ProjectileID.CorruptSpray, 0, 0f, Main.myPlayer, 8f);
            }
        }
    }
}
