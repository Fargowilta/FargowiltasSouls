using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.NPCMatching;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.LunarEvents.Solar
{
    public class LunarTowerSolar : LunarTowers
    {
        public override int ShieldStrength
        {
            get => NPC.ShieldStrengthTowerSolar;
            set => NPC.ShieldStrengthTowerSolar = value;
        }

        public override NPCMatcher CreateMatcher() =>
            new NPCMatcher().MatchType(NPCID.LunarTowerSolar);

        public LunarTowerSolar() : base(ModContent.BuffType<AtrophiedBuff>(), DustID.SolarFlare) { }

        public override int MaxHP => 70000;
        public override int Damage => 80;
        public enum Attacks
        {
            Idle,
            PillarSlam,
            FireballVomit,
            MeteorRain,

        }
        public override List<int> RandomAttacks =>
        //these are randomly chosen attacks in p1
        [
            (int)Attacks.PillarSlam,
            (int)Attacks.FireballVomit,
            (int)Attacks.MeteorRain
        ];
        public override void ShieldsDownAI(NPC npc)
        {
            Player target = Main.player[npc.target];
            if (npc.HasPlayerTarget && target.active)
            {
                switch (Attack)
                {
                    case (int)Attacks.PillarSlam:
                        PillarSlam(npc, target);
                        break;
                    case (int)Attacks.FireballVomit:
                        FireballVomit(npc, target);
                        break;
                    case (int)Attacks.MeteorRain:
                        MeteorRain(npc, target);
                        break;
                    case (int)Attacks.Idle:
                        Idle(npc, target);
                        break;
                }
            }
        }
        #region Attacks
        private Vector2 OriginalLocation;
        private bool HitFloor = false;
        private bool HitFloorEffect = false;
        private void PillarSlam(NPC npc, Player player)
        {
            const int WindupDuration = (int)(60 * 1);
            const int AttackDuration = 60 * 5;
            void Windup()
            {
                if (AttackTimer <= 1)
                {
                    OriginalLocation = npc.Center;
                    HitFloor = false;
                }
                else
                {
                    Vector2 desiredSpot = OriginalLocation - (Vector2.UnitY * 200);
                    npc.velocity = (desiredSpot - npc.Center) * 0.01f;
                }
            }
            void Attack()
            {
                if (AttackTimer <= WindupDuration + 2)
                {
                    npc.velocity = Vector2.Zero;
                    return;
                }
                if (Collision.SolidCollision(npc.position, npc.width, npc.height))
                {
                    HitFloor = true;
                    HitFloorEffect = false;
                }
                if (HitFloor && HitFloorEffect == false) //make sure it only happens once
                {
                    SoundEngine.PlaySound(SoundID.Item14, npc.Center);
                    const int distance = 160;
                    for (int i = 0; i < 16; i++)
                    {
                        int x = i;
                        if (x >= 8)
                        {
                            x = 7 - x; //split i into 1 to 8 and -1 to -8
                        }
                        if (FargoSoulsUtil.HostCheck)
                        {
                            float random = Main.rand.Next(-distance / 5, distance / 4);
                            Vector2 pos = OriginalLocation + Vector2.UnitX * (distance * x + random);
                            Vector2 vel = Vector2.UnitY * 16;
                            Projectile.NewProjectile(npc.GetSource_FromThis(), pos, vel, ModContent.ProjectileType<PillarSpawner>(), FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage), 3f, Main.myPlayer, ai0: 1);
                        }

                    }
                    HitFloorEffect = true;
                }
                if (!HitFloor)
                {
                    npc.velocity.Y = 25;
                }
                else
                {
                    npc.velocity = (OriginalLocation - npc.Center) * 0.05f;
                }
            }

            if (AttackTimer <= WindupDuration)
            {
                Windup();
            }
            else if (AttackTimer <= WindupDuration + AttackDuration)
            {
                Attack();
            }
            if (AttackTimer > WindupDuration + AttackDuration)
            {
                npc.velocity = Vector2.Zero;
                npc.Center = OriginalLocation;
                EndAttack(npc);
            }
        }
        private void FireballVomit(NPC npc, Player player)
        {
            const int AttackDuration = 60 * 5;
            const int EndlagDuration = 60 * 2;
            void Attack()
            {
                //rotate
                if (Math.Abs(npc.rotation) < Math.PI / 7)
                {
                    int direction = Math.Sign(player.Center.X - npc.Center.X);
                    npc.rotation += (float)(direction * (Math.PI / 16) / (60 * 2f)); //reach pi/7 in 2 seconds
                }
                Fireball();
            }
            void Endlag()
            {
                //rotate back to 0
                npc.rotation *= 0.97f;
                if (Math.Abs(npc.rotation) < Math.PI / 400)
                {
                    npc.rotation = 0;
                }
                Fireball();
            }
            void Fireball()
            {
                //spew fireballs
                float progress = ((float)AttackTimer / AttackDuration);
                const int MaxSpeed = 16;
                const int SpewFrames = 18;

                if (AttackTimer % SpewFrames == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item45, npc.Center);
                    if (FargoSoulsUtil.HostCheck)
                    {
                        float vel = MaxSpeed * Math.Min(1, progress * 1.5f);
                        float rotation = npc.rotation - (float)Math.PI / 2;
                        Vector2 aim = vel * (rotation).ToRotationVector2().RotatedByRandom(Math.PI / 8);
                        Vector2 pos = npc.Center + (rotation.ToRotationVector2() * npc.height * 0.45f);
                        int n = FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromThis(), pos, NPCID.SolarGoop, velocity: aim);
                        if (Main.npc[n].active)
                        {
                            Main.npc[n].damage = npc.damage;
                        }
                    }
                }
            }

            if (AttackTimer <= AttackDuration)
            {
                Attack();
            }
            if (AttackTimer > AttackDuration)
            {
                Endlag();
            }
            if (AttackTimer > AttackDuration + EndlagDuration)
            {
                EndAttack(npc);
            }
        }
        private void MeteorRain(NPC npc, Player player)
        {
            const int AttackDuration = 60 * 5;
            const int EndlagDuration = 60 * 2;
            void Attack()
            {
                //vibrate
                float progress = ((float)AttackTimer / AttackDuration);
                float vibration = Math.Min(1, progress * 4);
                npc.rotation = vibration * Main.rand.NextFloat((float)(-Math.PI / 200), (float)(Math.PI / 200));
                Meteor();
            }
            void Endlag()
            {
                //do nothing lo
            }
            void Meteor()
            {
                //spew fireballs
                const int MaxSpeed = 8;
                const int SpewFrames = 8;

                if (AttackTimer % SpewFrames == 0)
                {
                    if (FargoSoulsUtil.HostCheck)
                    {
                        float angle = (float)(Math.PI / 8f);
                        Vector2 aim = new Vector2(0, MaxSpeed).RotatedBy(-angle);
                        const int width = 2400;
                        const int height = 1000;
                        int offset = (int)(height * Math.Tan(angle));
                        Vector2 pos = player.Center + new Vector2(Main.rand.Next(-width - offset, width - offset), -height);
                        Projectile.NewProjectile(npc.GetSource_FromThis(), pos, aim, ModContent.ProjectileType<SolarMeteor>(), FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage), 3f, Main.myPlayer, ai1: aim.X, ai2: aim.Y);
                    }
                }
            }
            if (AttackTimer <= AttackDuration)
            {
                Attack();
            }
            if (AttackTimer > AttackDuration)
            {
                Endlag();
            }
            if (AttackTimer > AttackDuration + EndlagDuration)
            {
                EndAttack(npc);
            }
        }
        private const int IdleTime = 60 * 2;
        private void Idle(NPC npc, Player player)
        {
            if (AttackTimer == (int)(IdleTime * 0.75f))
            {
                if (FargoSoulsUtil.HostCheck)
                {
                    const float rotate = (float)Math.PI / 4f;
                    Vector2 speed = player.Center - npc.Center;
                    speed.Normalize();
                    speed *= 5f;
                    for (int i = -2; i <= 2; i++)
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, speed.RotatedBy(i * rotate), ProjectileID.CultistBossFireBall, 40, 0f, Main.myPlayer);

                    if (NPC.CountNPCS(NPCID.SolarCrawltipedeHead) <= 0 && WorldSavingSystem.MasochistModeReal) //spawn john
                    {
                        int n = NPC.NewNPC(npc.GetSource_FromThis(), (int)npc.Center.X, (int)(npc.Center.Y - npc.height * 0.45f), NPCID.SolarCrawltipedeHead);
                        if (Main.npc[n].active)
                        {
                            Main.npc[n].GivenName = "John Crawltipede";
                            Main.npc[n].life *= 6;

                        }
                    }
                }

            }
            if (AttackTimer > IdleTime)
            {
                /*
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    
                    if (Main.npc[i].active && (Main.npc[i].type == NPCID.SolarCrawltipedeBody || Main.npc[i].type == NPCID.SolarCrawltipedeTail))
                    {
                        Main.npc[i].GivenName = "John Crawltipede";
                    }
                }
                */
                RandomAttack(npc);
            }
        }
        #endregion
    }
}
