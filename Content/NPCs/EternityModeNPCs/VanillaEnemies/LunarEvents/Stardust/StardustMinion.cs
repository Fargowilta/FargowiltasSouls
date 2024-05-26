using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using System.IO;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using FargowiltasSouls.Common.Utilities;
using FargowiltasSouls.Content.Buffs.Masomode;
using System.Linq;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.LunarEvents.Stardust
{
	public class StardustMinion : ModNPC
    {
        public enum States 
        {
            Idle = 1,
            PrepareExpand,
            Expand,
            Contract,
            PrepareRush,
            Rush,
            PrepareScissor,
            Scissor,
            ScissorContract,
            Curve
        }
        public List<int> NoDamage =
        [
            (int)States.PrepareScissor
        ];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Crystal Slime");
            Main.npcFrameCount[NPC.type] = 2;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type] = NPCID.Sets.SpecificDebuffImmunity[NPCID.LunarTowerStardust];
            this.ExcludeFromBestiary();
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.StardustCellBig);

            NPC.lifeMax = 8000;
            NPC.damage = 110;

            NPC.aiStyle = -1;
            NPC.knockBackResist = 0;
            NPC.timeLeft = 60 * 60 * 30;
            NPC.noTileCollide = true;

            NPC.scale = 1f;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
        }
        public override bool CanHitPlayer(Player target, ref int CooldownSlot)
        {
            return !NoDamage.Contains((int)NPC.ai[1]);
        }
        public override void AI()
        {
            StardustMinionAI();
        }
        private Vector2 initialLock = Vector2.Zero;
        private Vector2 LockPos = Vector2.Zero;
        int HealCounter = 0;
        public void StardustMinionAI()
        {
            ref float Timer = ref NPC.ai[0];
            ref float State = ref NPC.ai[1];
            ref float parentIndex = ref NPC.ai[2];
            ref float num = ref NPC.ai[3];

            ref float substate = ref NPC.localAI[0];

            NPC parent = Main.npc[(int)parentIndex];
            if (!parent.active || parent.type != NPCID.LunarTowerStardust)
            {
                NPC.active = false;
                return;
            }
            LunarTowerStardust parentModNPC = parent.GetGlobalNPC<LunarTowerStardust>();
            float NearParent = parent.height * 0.8f;

            if (++HealCounter > 60)
            {
                HealCounter = 0;
                if (!parent.HasValidTarget || parent.Distance(Main.player[parent.target].Center) > parentModNPC.AuraSize)
                {
                    const int heal = 500;
                    NPC.life += heal;
                    if (NPC.life > NPC.lifeMax)
                        NPC.life = NPC.lifeMax;
                    if (NPC.life >= NPC.lifeMax)
                    {
                        NPC.dontTakeDamage = false;
                        NPC.position = NPC.Center;
                        NPC.width = 48;
                        NPC.height = 48;
                        NPC.Center = NPC.position;
                    }
                    NPC.netUpdate = true;
                    CombatText.NewText(NPC.Hitbox, CombatText.HealLife, heal);
                }
            }

            switch (State)
            {
                case (int)States.Idle: //default, chill around center of pillar
                    {
                        const float speedFactor = 0.05f;
                        //rotating at 1/2 rotations per second
                        float rotation = MathHelper.Pi * parentModNPC.CellRotation / 60f;
                        rotation += MathHelper.TwoPi * (num / LunarTowerStardust.CellAmount);
                        Vector2 desiredLocation = parent.Center + NearParent * rotation.ToRotationVector2();
                        NPC.velocity = (desiredLocation - NPC.Center) * speedFactor;
                        break;
                    }
                case (int)States.PrepareExpand:
                    {
                        const float speedFactor = 0.05f;
                        //rotating at 1/12 rotations per second
                        float rotation = (MathHelper.Pi / 6) * parentModNPC.CellRotation / 60f;
                        rotation += MathHelper.TwoPi * (num / LunarTowerStardust.CellAmount);
                        Vector2 desiredLocation = parent.Center + NearParent * rotation.ToRotationVector2();
                        NPC.velocity = (desiredLocation - NPC.Center) * speedFactor;
                        break;
                    }
                case (int)States.Expand:
                    {
                        const int expansionSpeed = 16;
                        //rotating at 1/12 rotations per second
                        float rotation = (MathHelper.Pi / 6) * parentModNPC.CellRotation / 60f;
                        rotation += MathHelper.TwoPi * (num / LunarTowerStardust.CellAmount);
                        Vector2 desiredLocation = parent.Center + (parentModNPC.AuraSize + 400) * rotation.ToRotationVector2();
                        NPC.velocity = Vector2.Normalize(desiredLocation - NPC.Center) * expansionSpeed;
                        if (NPC.Distance(parent.Center) > parentModNPC.AuraSize)
                        {
                            State = (int)States.Contract;
                        }
                        break;
                    }
                case (int)States.Contract:
                    {
                        const int contractionSpeed = 16;
                        //rotating at 1/12 rotations per second
                        float rotation = (MathHelper.Pi / 6) * parentModNPC.CellRotation / 60f;
                        rotation += MathHelper.TwoPi * (num / LunarTowerStardust.CellAmount);
                        Vector2 desiredLocation = parent.Center + NearParent * rotation.ToRotationVector2();
                        NPC.velocity = Vector2.Normalize(NPC.velocity) * contractionSpeed;
                        RotateTowards(desiredLocation, 2);
                        if (NPC.Distance(desiredLocation) <= 100)
                        {
                            State = (int)States.Idle;
                        }
                        break;
                    }
                case (int)States.PrepareRush:
                    {
                        const int ReactionTime = 60;
                        const float speedFactor = 0.05f;
                        Player player = Main.player[parent.target];
                        //rotating at 1/3 rotations per second
                        float rotation = (MathHelper.Pi / 1.5f) * parentModNPC.CellRotation / 60f;
                        rotation += MathHelper.TwoPi * (num / LunarTowerStardust.CellAmount);
                        Vector2 desiredLocation = parent.Center + NearParent * rotation.ToRotationVector2();
                        NPC.velocity = (desiredLocation - NPC.Center) * speedFactor;
                        float fromCenter = NPC.DirectionFrom(parent.Center).ToRotation();
                        float toPlayer = NPC.SafeDirectionTo(player.Center).ToRotation();
                        if (parentModNPC.AttackTimer > ReactionTime)
                        {
                            if (parentModNPC.AttackTimer > ReactionTime && player.active && !player.ghost && Math.Abs(toPlayer - fromCenter) < MathHelper.Pi / 8)
                            {

                                substate = 0;
                                SoundEngine.PlaySound(SoundID.Item96, NPC.Center);
                                State = (int)States.Rush;
                            }
                        }
                        break;
                    }
                case (int)States.Rush:
                    {
                        const int RushSpeed = 20;
                        Player player = Main.player[parent.target];
                        if (substate != (float)States.Rush && player.active && !player.ghost)
                        {
                            NPC.velocity = NPC.SafeDirectionTo(player.Center) * RushSpeed;
                            substate = (float)States.Rush;
                        }
                        if (NPC.Distance(parent.Center) > parentModNPC.AuraSize)
                        {
                            substate = 0;
                            State = (int)States.Contract;
                        }
                        break;
                    }
                case (int)States.PrepareScissor:
                    {
                        Player player = Main.player[parent.target];
                        if (substate != (int)States.PrepareScissor)
                        {
                            initialLock = player.Center;
                            substate = (int)States.PrepareScissor;
                        }
                        if (WorldSavingSystem.MasochistModeReal)
                        {
                            initialLock = player.Center; //keep tracking
                        }
                        Scissor(parent, num, parentModNPC, true);
                        break;
                    }
                case (int)States.Scissor:
                    {
                        substate = 0;
                        Scissor(parent, num, parentModNPC, false);
                        break;
                    }
                case (int)States.ScissorContract:
                    {
                        float contractionSpeed = NPC.velocity.Length();
                        //rotating at 1/12 rotations per second
                        float rotation = (MathHelper.Pi / 6) * parentModNPC.CellRotation / 60f;
                        rotation += MathHelper.TwoPi * (num / LunarTowerStardust.CellAmount);
                        Vector2 desiredLocation = parent.Center + NearParent * rotation.ToRotationVector2();
                        NPC.velocity = Vector2.Normalize(NPC.velocity) * contractionSpeed;
                        RotateTowards(desiredLocation, 3);
                        if (NPC.Distance(desiredLocation) <= 100)
                        {
                            State = (int)States.Idle;
                        }
                        break;
                    }
                case (int)States.Curve:
                    {
                        const int HomeBack = -1;
                        Player player = Main.player[parent.target];
                        if (substate != (float)States.Curve && substate != HomeBack && player.active && !player.ghost)
                        {
                            int side = Main.rand.NextBool() ? 1 : -1;
                            NPC.velocity = NPC.SafeDirectionTo(player.Center).RotatedBy(90 * side);
                            LockPos = (player.Center - parent.Center) * Main.rand.NextFloat(0.8f, 1.2f);
                            substate = (float)States.Curve;
                            NPC.netUpdate = true;
                            Timer = 0;
                        }
                        int Speed = 16;
                        Vector2 desiredLocation = parent.Center + LockPos;
                        NPC.velocity = Vector2.Normalize(NPC.velocity) * Speed;
                        RotateTowards(desiredLocation, 2);
                        Timer++;
                        if ((NPC.Distance(desiredLocation) <= 300 && substate != HomeBack) || Timer > 60 * 3)
                        {
                            LockPos = Vector2.Zero;
                            substate = -1;
                        }
                        if (NPC.Distance(parent.Center) <= NearParent && substate == HomeBack)
                        {
                            substate = 0;
                            State = (float)States.Idle;
                        }
                        break;
                    }
            }
        }
        
        public void Scissor(NPC parent, float num, LunarTowerStardust parentModNPC, bool telegraph)
        {
            const float speedFactor = 0.05f;
            int Side = (num >= LunarTowerStardust.CellAmount / 2) ? 1 : -1;
            int x = Side == 1 ? (int)num - LunarTowerStardust.CellAmount / 2 : (int)num;
            float ScissorSpeed = 1f + (0.08f * (5-x));
            int Distance = (x * parentModNPC.AuraSize / (LunarTowerStardust.CellAmount / 2)) + (parent.width / 2);
            if (Side == 1)
            {
                Distance += parentModNPC.AuraSize / (LunarTowerStardust.CellAmount); //offset one side by half the distance between cells so it overlaps with space on other side
            }
            float rotation;
            if (telegraph)
            {
                rotation = parent.SafeDirectionTo(initialLock).ToRotation() + (parentModNPC.CellRotation * Side);
                
            }
            else
            {
                rotation = LockPos.ToRotation();
                LockPos = LockPos.RotatedBy(MathHelper.ToRadians(-ScissorSpeed * Side));
            }
            Vector2 desiredLocation = parent.Center + Distance * rotation.ToRotationVector2();
            NPC.velocity = (desiredLocation - NPC.Center) * speedFactor;
            if (telegraph)
            {
                LockPos = (parent.SafeDirectionTo(initialLock).ToRotation() + (parentModNPC.CellRotation * Side)).ToRotationVector2();
            }
        }
        public override bool CheckDead()
        {
            ref float State = ref NPC.ai[1];
            ref float parentIndex = ref NPC.ai[2];
            NPC parent = Main.npc[(int)parentIndex];
            if (!parent.active)
            {
                return true;
            }
            NPC.life = NPC.lifeMax;
            NPC.dontTakeDamage = true;
            NPC.position = NPC.Center;
            NPC.width = 26;
            NPC.height = 26;
            NPC.Center = NPC.position;
            return false;
        }
        public override void DrawBehind(int index)
        {
            Main.instance.DrawCacheNPCProjectiles.Add(index);
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frame.Y = NPC.dontTakeDamage ? frameHeight : 0;
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool PreKill()
        {
            return false;
        }

        public override bool? DrawHealthBar(byte hbPos, ref float scale, ref Vector2 Pos)
        {
            return false;
        }

        void RotateTowards(Vector2 target, float speed)
        {
            Vector2 PV = NPC.SafeDirectionTo(target);
            Vector2 LV = NPC.velocity;
            float anglediff = (float)(Math.Atan2(PV.Y * LV.X - PV.X * LV.Y, LV.X * PV.X + LV.Y * PV.Y)); //real
            //change rotation towards target
            NPC.velocity = NPC.velocity.RotatedBy(Math.Sign(anglediff) * Math.Min(Math.Abs(anglediff), speed * MathHelper.Pi / 180));
        }
        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            ref float parentIndex = ref NPC.ai[2];
            NPC parent = Main.npc[(int)parentIndex];
            if (!parent.active || parent.type != NPCID.LunarTowerStardust)
            {
                return;
            }
            LunarTowerStardust parentModNPC = parent.GetGlobalNPC<LunarTowerStardust>();
            bool anyPlayersClose = parentModNPC.AnyPlayerWithin(parent, parentModNPC.AuraSize);
            if (!anyPlayersClose)
            {
                modifiers.Null();
            }
        }
    }
}