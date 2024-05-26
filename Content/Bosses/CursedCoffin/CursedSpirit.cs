using FargowiltasSouls.Content.Buffs.Boss;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using FargowiltasSouls.Content.Buffs;
using FargowiltasSouls.Common.Graphics.Particles;
using Terraria.ModLoader.IO;
using FargowiltasSouls.Content.WorldGeneration;
using System.Linq;
using Terraria.ModLoader;
using Terraria.ID;

namespace FargowiltasSouls.Content.Bosses.CursedCoffin
{
    //[AutoloadBossHead]
    public class CursedSpirit : ModNPC
    {
        //TODO: re-enable boss checklist compat, localizationhelper addSpawnInfo
        public override bool IsLoadingEnabled(Mod mod) => CursedCoffin.Enabled;

        #region Variables


        private int Frame = 0;

        //NPC.ai[] overrides
        public ref float Owner => ref NPC.ai[0];
        public ref float Timer => ref NPC.ai[1];
        public ref float State => ref NPC.ai[2];
        public ref float AI3 => ref NPC.ai[3];

        public ref float StartupFadein => ref NPC.localAI[0];

        public static readonly Color GlowColor = new(224, 196, 252, 0);

        public int BiteTimer;
        public int BittenPlayer = -1;

        public bool RotateToVelocity = true;

        Vector2 LockVector1 = new();

        #endregion
        #region Standard
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 9;
            NPCID.Sets.TrailCacheLength[NPC.type] = 20;
            NPCID.Sets.TrailingMode[NPC.type] = 3;
            NPCID.Sets.MPAllowedEnemies[Type] = true;

            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            this.ExcludeFromBestiary();
            NPC.AddDebuffImmunities(
            [
                BuffID.Confused,
                BuffID.Chilled,
                BuffID.Suffocation,
                ModContent.BuffType<LethargicBuff>(),
                ModContent.BuffType<ClippedWingsBuff>()
            ]);
        }
        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = CursedCoffin.BaseHP;
            NPC.defense = 10;
            NPC.damage = 35;
            NPC.knockBackResist = 0f;
            NPC.width = 110;
            NPC.height = 110;
            //NPC.boss = true;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit54;
            NPC.DeathSound = SoundID.NPCDeath52;

            NPC.hide = true;

            NPC.value = Item.buyPrice(0, 0);
            NPC.Opacity = 0;

        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * balance);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
            writer.Write7BitEncodedInt(BiteTimer);
            writer.Write7BitEncodedInt(BittenPlayer);
            writer.WriteVector2(LockVector1);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
            BiteTimer = reader.Read7BitEncodedInt();
            BittenPlayer = reader.Read7BitEncodedInt();
            LockVector1 = reader.ReadVector2();
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (SlowChargeStates.Contains(State))
                target.longInvince = true;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (SlowChargeStates.Contains(State))
            {
                if (target.HasBuff<GrabbedBuff>())
                    return;
                target.buffImmune[ModContent.BuffType<CoffinTossBuff>()] = true;

                BittenPlayer = target.whoAmI;
                BiteTimer = 360;
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, NPC.whoAmI);

                NPC owner = FargoSoulsUtil.NPCExists(Owner, ModContent.NPCType<CursedCoffin>());
                if (owner.TypeAlive<CursedCoffin>())
                {
                    // Forces Coffin to enter grab punish state
                    owner.As<CursedCoffin>().ForceGrabPunish = 1;
                    owner.netUpdate = true;
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, owner.whoAmI);
                }
            }
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            if (NPC.Opacity < 1)
                return false;
            Vector2 boxPos = target.position;
            Vector2 boxDim = target.Size;
            return Collides(boxPos, boxDim);
        }
        public override bool CanHitNPC(NPC target)
        {
            Vector2 boxPos = target.position;
            Vector2 boxDim = target.Size;
            return Collides(boxPos, boxDim);
        }
        public bool Collides(Vector2 boxPos, Vector2 boxDim)
        {
            //circular hitbox-inator
            Vector2 ellipseDim = NPC.Size;
            Vector2 ellipseCenter = NPC.position + 0.5f * new Vector2(NPC.width, NPC.height);

            float x = 0f; //ellipse center
            float y = 0f; //ellipse center
            if (boxPos.X > ellipseCenter.X)
            {
                x = boxPos.X - ellipseCenter.X; //left corner
            }
            else if (boxPos.X + boxDim.X < ellipseCenter.X)
            {
                x = boxPos.X + boxDim.X - ellipseCenter.X; //right corner
            }
            if (boxPos.Y > ellipseCenter.Y)
            {
                y = boxPos.Y - ellipseCenter.Y; //top corner
            }
            else if (boxPos.Y + boxDim.Y < ellipseCenter.Y)
            {
                y = boxPos.Y + boxDim.Y - ellipseCenter.Y; //bottom corner
            }
            float a = ellipseDim.X / 2f;
            float b = ellipseDim.Y / 2f;

            return x * x / (a * a) + y * y / (b * b) < 1; //point collision detection
        }

        #endregion
        readonly List<float> SlowChargeStates =
        [
            (float)CursedCoffin.BehaviorStates.PhaseTransition,
            (float)CursedCoffin.BehaviorStates.WavyShotCircle,
            (float)CursedCoffin.BehaviorStates.WavyShotSlam,
            (float)CursedCoffin.BehaviorStates.RandomStuff,
            (float)CursedCoffin.BehaviorStates.GrabbyHands
        ];
        public override bool CheckActive() => false;
        public override void OnKill()
        {
            NPC owner = FargoSoulsUtil.NPCExists(Owner, ModContent.NPCType<CursedCoffin>());
            if (!owner.TypeAlive<CursedCoffin>())
            {
                return;
            }
            if (FargoSoulsUtil.HostCheck)
                owner.StrikeInstantKill();
        }
        #region AI
        public override void AI()
        {
            NPC owner = FargoSoulsUtil.NPCExists(Owner, ModContent.NPCType<CursedCoffin>());
            if (!owner.TypeAlive<CursedCoffin>())
            {
                NPC.StrikeInstantKill();
                return;
            }

            if (StartupFadein < 10)
            {
                StartupFadein++;
                NPC.Opacity = 0;
            }
            else if (StartupFadein == 10)
            {
                NPC.Opacity = 1;
                StartupFadein++;
            }

            // share healthbar
            NPC.lifeMax = owner.lifeMax = Math.Min(NPC.lifeMax, owner.lifeMax);
            NPC.life = owner.life = Math.Min(NPC.life, owner.life);

            RotateToVelocity = true;
            NPC.dontTakeDamage = NPC.scale < 0.5f;
            NPC.noTileCollide = true;

            if (!(owner.target.IsWithinBounds(Main.maxPlayers) && Main.player[owner.target] is Player player && player.Alive()))
                return;
            CursedCoffin coffin = owner.As<CursedCoffin>();

            if (coffin.AttackCounter < 3 && !coffin.Enraged)
            {
                NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.SafeDirectionTo(owner.Center) * Math.Min(Math.Max(20, owner.velocity.Length()), NPC.Distance(owner.Center)), 0.2f);
                LerpOpacity(0.15f);
                LerpScale(0.4f);
                if (NPC.Distance(owner.Center) < NPC.width / 2)
                    NPC.active = false;
                return;
            }

            if (BittenPlayer != -1)
            {
                // being held
                Player victim = Main.player[BittenPlayer];
                if (BiteTimer > 0 && victim.active && !victim.ghost && !victim.dead
                    && (NPC.Distance(victim.Center) < 160 || victim.whoAmI != Main.myPlayer)
                    && victim.FargoSouls().MashCounter < 20)
                {
                    victim.AddBuff(ModContent.BuffType<GrabbedBuff>(), 2);
                    NPC.velocity *= 0.2f;
                    victim.velocity = Vector2.Zero;
                    victim.Center = Vector2.Lerp(victim.Center, NPC.Center, 0.1f);
                }
                else // escaped
                {
                    BittenPlayer = -1;
                    BiteTimer = -90; //cooldown

                    // dash away otherwise it's bullshit
                    NPC.velocity = -NPC.SafeDirectionTo(victim.Center) * 12;

                    NPC.netUpdate = true;
                    Timer = 0;
                    AI3 = 0;

                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, NPC.whoAmI);
                }
                return;
            }

            if (!coffin.StateMachine.StateStack.Any())
                return;

            bool newState = (float)coffin.StateMachine.CurrentState.Identifier != State;

            switch (coffin.StateMachine.CurrentState.Identifier)
            {
                case CursedCoffin.BehaviorStates.StunPunish:
                    if (newState)
                    {
                        Timer = 0;
                        AI3 = 0;
                    }
                    Movement(player.Center + player.Center.SafeDirectionTo(NPC.Center) * 300, 0.1f, 10, 5, 0.08f, 20);
                    break;
                case CursedCoffin.BehaviorStates.HoveringForSlam:
                    if (newState)
                    {
                        Timer = 0;
                        AI3 = 0;
                    }
                    Artillery(owner);
                    break;
                case CursedCoffin.BehaviorStates.SlamWShockwave:
                    if (newState)
                    {
                        Timer = 0;
                        AI3 = 0;
                    }
                    SlamSupport(owner);
                    break;
                    /*
                case CursedCoffin.StateEnum.GrabbyHands:
                    {
                        Timer = 0;
                        AI3 = 0;
                    }
                    GrabbyHands(owner);
                    break;
                    */
                case var _ when SlowChargeStates.Contains((float)coffin.StateMachine.CurrentState.Identifier):
                    if (!SlowChargeStates.Contains(State))
                    {
                        Timer = 0;
                        AI3 = 0;
                    }
                    SlowCharges(owner, coffin.StateMachine.CurrentState.Identifier);
                    break;
                case CursedCoffin.BehaviorStates.PhaseTransition:
                    {
                        NPC.Center = owner.Center;
                        NPC.scale = 0.2f;
                    }
                    break;
                default:
                    break;
            }
            State = (float)coffin.StateMachine.CurrentState.Identifier;
            if (RotateToVelocity)
                NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
        }
        void SlamSupport(NPC owner)
        {
            CursedCoffin coffin = owner.As<CursedCoffin>();
            Player player = Main.player[owner.target];

            if (AI3 == 0) //falling
            {
                if (coffin.Timer < 0 || owner.velocity.Y == 0)
                    AI3 = 1;
                NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.SafeDirectionTo(owner.Center) * Math.Min(Math.Max(20, owner.velocity.Length()), NPC.Distance(owner.Center)), 0.2f);
                LerpOpacity(0.15f);
                LerpScale(0.4f);
            }
            else if (AI3 == 1) //do slam
            {
                if (NPC.Distance(owner.Center) > 50) // teleport
                {
                    for (int i = 0; i < 20; i++)
                    {
                        Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Shadowflame, Main.rand.NextFloat(-3, 3), Main.rand.NextFloat(-3, 3));
                    }
                    //SoundEngine.PlaySound(poof, NPC.Center);
                    NPC.Center = owner.Center;
                    NPC.netUpdate = true;
                }
                LerpOpacity(1f, 0.4f);
                LerpScale(1f, 0.4f);
                AI3 = 2;
                NPC.velocity = Vector2.UnitY * 7;

                SoundEngine.PlaySound(CursedCoffin.SoulShotSFX, NPC.Center);
                if (FargoSoulsUtil.HostCheck)
                {
                    int cap = WorldSavingSystem.MasochistModeReal ? 3 : 2;
                    for (int i = -cap; i <= cap; i++)
                    {
                        if (i == 0)
                            continue;
                        if (!WorldSavingSystem.EternityMode && (i == 1 || i == -1))
                            continue;
                        Vector2 vel = Vector2.UnitY.RotatedBy(i * MathF.Tau * (0.041f + Main.rand.NextFloat(-0.02f, 0.01f))) * (6 + Math.Abs(i));
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Bottom + NPC.velocity, vel, ModContent.ProjectileType<CoffinDarkSouls>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 1f, Main.myPlayer, NPC.whoAmI, -0.135f);
                        // ghost projs, neg grav
                    }
                }
            }
            else
            {
                if (coffin.Timer > 0)
                    AI3 = 0;
                NPC.velocity *= 0.97f;
                LerpOpacity(1f, 0.4f);
                LerpScale(1f, 0.4f);
                //Movement(player.Center + player.Center.SafeDirectionTo(NPC.Center) * 300, 0.1f, 10, 5, 0.08f, 20);
            }
        }
        void SlowCharges(NPC owner, CursedCoffin.BehaviorStates state)
        {
            
            

            if (Timer < 80)
            {
                LerpScale(0.8f, 0.4f);
                LerpOpacity(0.5f, 0.4f);
            }
            else
            {
                LerpScale(1f, 0.4f);
                LerpOpacity(1f, 0.4f);
            }
                

            Player player = Main.player[owner.target];
            if (Timer <= 1)
            {
                AI3 = NPC.SafeDirectionTo(player.Center).ToRotation() + Main.rand.NextFloat(-MathHelper.PiOver2 * 0.6f, MathHelper.PiOver2 * 0.6f);
                NPC.netUpdate = true;
                // Get the corner with the biggest difference in angle to player (to make npc avoid player on course to the corner)
                static float CrossProduct(Vector2 v1, Vector2 v2) => (v1.X * v2.Y) - (v1.Y * v2.X); // Ordering by cross product gives the biggest undirected angle difference between two vectors
                List<Vector2> corners = CoffinArena.TopArenaCorners(NPC);
                CursedCoffin coffin = owner.As<CursedCoffin>();
                if (coffin.StateMachine.StateStack.Any() && coffin.StateMachine.CurrentState.Identifier == CursedCoffin.BehaviorStates.WavyShotSlam)
                    LockVector1 = corners.OrderByDescending(x => x.Distance(player.Center)).First();
                else
                    LockVector1 = corners.OrderByDescending(x => Math.Abs(CrossProduct(NPC.DirectionTo(x), NPC.DirectionTo(player.Center)))).First();
            }
            else if (Timer < 80)
            {
                //Vector2 dir = Vector2.Lerp(player.DirectionTo(NPC.Center), owner.DirectionTo(player.Center), Timer / 140);
                //Vector2 desiredPos = CursedCoffin.ClampWithinArena(player.Center + dir * 300, NPC);
                Movement(LockVector1, 0.2f, 20, 10, 0.1f, 20);
                
                NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.DirectionTo(player.Center).ToRotation() + MathHelper.PiOver2, 0.1f);
                RotateToVelocity = false;
            }
            else if (Timer < 90)
            {
                NPC.velocity = Vector2.Lerp(Vector2.Zero, NPC.DirectionTo(player.Center) * 3, 0.2f);
            }
            else //if (Timer < 240) edit: no longer loops, just continues charging
            {
                SoundEngine.PlaySound(CursedCoffin.SpiritDroneSFX, NPC.Center);

                if (state != CursedCoffin.BehaviorStates.RandomStuff) // Charges
                {
                    if (Timer > 110)
                        NPC.noTileCollide = false;

                    if (Timer > 180 || (Timer > 110 && Collision.SolidTiles(NPC.position + NPC.velocity, NPC.width, NPC.height)))
                    {
                        Timer = 0;
                        return;
                    }
                    if (NPC.velocity.LengthSquared() < 10 * 10)
                    {
                        NPC.velocity += Utils.SafeNormalize(NPC.velocity, Vector2.Zero) * 0.5f;
                    }
                    NPC.velocity = NPC.velocity.ClampLength(0, 10);
                }
                else // Slow crawling
                {
                    Vector2 vectorToIdlePosition = player.Center - NPC.Center;
                    float speed = 6.5f;
                    if (!WorldSavingSystem.EternityMode)
                        speed /= 2;
                    else if (!WorldSavingSystem.MasochistModeReal)
                        speed /= 1.1f;
                    float inertia = 20f;
                    vectorToIdlePosition.Normalize();
                    vectorToIdlePosition *= speed;
                    NPC.velocity = (NPC.velocity * (inertia - 1f) + vectorToIdlePosition) / inertia;
                    if (NPC.velocity == Vector2.Zero)
                    {
                        NPC.velocity.X = -0.15f;
                        NPC.velocity.Y = -0.05f;
                    }
                    if (NPC.velocity.Length() > 6.5f)
                        NPC.velocity *= 0.97f;
                }
            }
            Timer++;
        }
        void Artillery(NPC owner)
        {
            if (NPC.Opacity > 0.9f)
                NPC.Opacity = 0.9f;
            LerpOpacity(0.4f);
            LerpScale(0.6f);
            Vector2 desiredPos = owner.Center - Vector2.UnitY * owner.height;
            Movement(desiredPos, 0.1f, Math.Max(25, owner.velocity.Length()), owner.velocity.Length(), 0.08f, 20);
            if (NPC.Distance(desiredPos) < owner.height * 0.75f)
            {
                const int shotTime = 20;
                if (Timer % shotTime == shotTime - 1)
                {
                    SoundEngine.PlaySound(CursedCoffin.SoulShotSFX, NPC.Center);
                    if (FargoSoulsUtil.HostCheck)
                    {
                        Vector2 vel = -Vector2.UnitY.RotatedBy(MathF.Tau * 0.14f * Math.Sin(MathF.Tau * (Timer + Main.rand.Next(20)) / 53)) * 4;
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel, ModContent.ProjectileType<CoffinDarkSouls>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 1f, Main.myPlayer, NPC.whoAmI, 0.18f);
                    }
                }
                Timer++;
            }
        }
        void GrabbyHands(NPC owner)
        {
            CursedCoffin coffin = owner.As<CursedCoffin>();
            Player player = Main.player[owner.target];

            if (coffin.Timer < 40)
            {
                Vector2 offset = -Vector2.UnitY * 300 - Vector2.UnitX * Math.Sign(owner.Center.X - player.Center.X) * 200;
                Vector2 desiredPos = player.Center + offset;
                Movement(desiredPos, 0.1f, 10, 5, 0.08f, 20);
            }
            else
            {
                NPC.velocity *= 0.97f;
            }

            if (coffin.Timer < 40)
            {
                LerpOpacity(0.15f);
                LerpScale(0.4f);
            }
            else
            {
                LerpOpacity(1f, 0.3f);
                LerpScale(1f, 0.3f);
            }

            if (coffin.Timer == 40)
            {
                if (FargoSoulsUtil.HostCheck)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, (NPC.rotation + MathHelper.PiOver2).ToRotationVector2() * 4, ModContent.ProjectileType<CoffinHand>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage, 0.1f), 1f, Main.myPlayer, owner.whoAmI, 1, 1);
                    //Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, (NPC.rotation - MathHelper.PiOver2).ToRotationVector2() * 4, ModContent.ProjectileType<CoffinHand>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage, 0.1f), 1f, Main.myPlayer, owner.whoAmI, 1, -1);
                }
            }

        }
        void Movement(Vector2 pos, float accel = 0.03f, float maxSpeed = 20, float lowspeed = 5, float decel = 0.03f, float slowdown = 30)
        {
            if (NPC.Distance(pos) > slowdown)
            {
                NPC.velocity = Vector2.Lerp(NPC.velocity, (pos - NPC.Center).SafeNormalize(Vector2.Zero) * maxSpeed, accel);
            }
            else
            {
                NPC.velocity = Vector2.Lerp(NPC.velocity, (pos - NPC.Center).SafeNormalize(Vector2.Zero) * lowspeed, decel);
            }
        }
        void LerpOpacity(float target, float speed = 0.15f) => NPC.Opacity = (float)Utils.Lerp(NPC.Opacity, target, speed);
        void LerpScale(float target, float speed = 0.15f) => NPC.scale = (float)Utils.Lerp(NPC.scale, target, speed);
        #endregion
        #region Overrides
        public override void HitEffect(NPC.HitInfo hit)
        {
            //TODO: gore
            /*
            if (NPC.life <= 0)
            {
                for (int i = 1; i <= 4; i++)
                {
                    Vector2 pos = NPC.position + new Vector2(Main.rand.NextFloat(NPC.width), Main.rand.NextFloat(NPC.height));
                    if (!Main.dedServ)
                        Gore.NewGore(NPC.GetSource_FromThis(), pos, NPC.velocity, ModContent.Find<ModGore>(Mod.Name, $"BaronGore{i}").Type, NPC.scale);
                }
            }
            */
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D bodytexture = Terraria.GameContent.TextureAssets.Npc[NPC.type].Value;
            Vector2 drawPos = NPC.Center - screenPos;
            SpriteEffects spriteEffects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Color glowColor = CursedCoffin.GlowColor;

            int trailLength = NPCID.Sets.TrailCacheLength[NPC.type];
            if (NPC.scale < 0.5f)
                trailLength /= 2;

            for (int i = 0; i < trailLength; i++)
            {
                Vector2 oldPos = NPC.oldPos[i];

                DrawData oldGlow = new(bodytexture, oldPos + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame, NPC.GetAlpha(glowColor * (0.8f / i)), NPC.oldRot[i], NPC.Size / 2, NPC.scale, spriteEffects, 0);
                GameShaders.Misc["LCWingShader"].UseColor(Color.Blue).UseSecondaryColor(Color.Black);
                GameShaders.Misc["LCWingShader"].Apply(oldGlow);
                oldGlow.Draw(spriteBatch);
            }
            for (int j = 0; j < 12; j++)
            {
                float spinOffset = (Main.GameUpdateCount * 0.001f * j) % 12;
                float magnitude = 1f + ((j % 5) * 2f * MathF.Sin(Main.GameUpdateCount * MathHelper.TwoPi / (10 + ((j - 6f) * 28f))));
                Vector2 afterimageOffset = (MathHelper.TwoPi * (j + spinOffset) / 12f).ToRotationVector2() * magnitude * NPC.scale;

                spriteBatch.Draw(bodytexture, drawPos + afterimageOffset, NPC.frame, NPC.GetAlpha(glowColor * NPC.Opacity), NPC.rotation, NPC.Size / 2, NPC.scale, spriteEffects, 0f);
            }
            /*
            for (int j = 0; j < 12; j++)
            {
                float spinOffset = 0;// Main.GameUpdateCount * 0.001f * j;
                float magnitude = 12;
                Vector2 afterimageOffset = (MathHelper.TwoPi * (j + spinOffset) / 12f).ToRotationVector2() * magnitude * NPC.scale;
               
                spriteBatch.Draw(bodytexture, drawPos + afterimageOffset, NPC.frame, glowColor, NPC.rotation, NPC.Size / 2, NPC.scale, spriteEffects, 0f);
            }
            */
            spriteBatch.Draw(bodytexture, drawPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.Size / 2, NPC.scale, spriteEffects, 0f);
            return false;
        }
        public override void DrawBehind(int index)
        {
            if (NPC.hide)
            {
                Main.instance.DrawCacheNPCsBehindNonSolidTiles.Add(index);
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (++NPC.frameCounter > 4)
            {
                if (++Frame >= Main.npcFrameCount[Type] - 1)
                    Frame = 0;
                NPC.frameCounter = 0;
            }
            NPC.spriteDirection = NPC.direction;
            NPC.frame.Y = frameHeight * Frame;
            NPC.frame.Width = 120;

            if (SlowChargeStates.Contains(State))
                NPC.frame.X = 120;
            else
                NPC.frame.X = 0;
        }

        #endregion
    }
}