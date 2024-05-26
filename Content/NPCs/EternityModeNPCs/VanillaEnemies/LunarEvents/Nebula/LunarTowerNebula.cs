using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.LunarEvents.Nebula
{
    public class LunarTowerNebula : LunarTowers
    {
        public override int ShieldStrength
        {
            get => NPC.ShieldStrengthTowerNebula;
            set => NPC.ShieldStrengthTowerNebula = value;
        }

        public override NPCMatcher CreateMatcher() =>
            new NPCMatcher().MatchType(NPCID.LunarTowerNebula);

        public LunarTowerNebula() : base(ModContent.BuffType<ReverseManaFlowBuff>(), 58) { }

        public override int MaxHP => 60000;
        public override int Damage => 80;
        public enum Attacks
        {
            Idle,
            MirageDeathray,
            TeleportJumpscare,
            MassiveNebulaArcanum

        }
        public override List<int> RandomAttacks =>
        //these are randomly chosen attacks in p1
        [
            (int)Attacks.MirageDeathray,
            (int)Attacks.TeleportJumpscare,
            (int)Attacks.MassiveNebulaArcanum
        ];
        public override void ShieldsDownAI(NPC npc)
        {
            Player target = Main.player[npc.target];
            if (npc.HasPlayerTarget && target.active)
            {
                switch (Attack)
                {
                    case (int)Attacks.MirageDeathray:
                        MirageDeathray(npc, target);
                        break;
                    case (int)Attacks.TeleportJumpscare:
                        TeleportJumpscare(npc, target);
                        break;
                    case (int)Attacks.MassiveNebulaArcanum:
                        MassiveNebulaArcanum(npc, target);
                        break;
                    case (int)Attacks.Idle:
                        Idle(npc, target);
                        break;
                }
            }
            /*
            if (--AttackTimer < 0)
            {
                AttackTimer = 300;
                npc.TargetClosest(false);
                NetSync(npc);
                for (int i = 0; i < 40; ++i)
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, DustID.PinkTorch, 0.0f, 0.0f, 0, new Color(), 1f);
                    Dust dust = Main.dust[d];
                    dust.velocity *= 4f;
                    Main.dust[d].noGravity = true;
                    Main.dust[d].scale += 1.5f;
                }
                if (npc.HasPlayerTarget && FargoSoulsUtil.HostCheck && npc.Distance(Main.player[npc.target].Center) < 3000)
                {
                    int x = (int)Main.player[npc.target].Center.X / 16;
                    int y = (int)Main.player[npc.target].Center.Y / 16;
                    for (int i = 0; i < 100; i++)
                    {
                        int newX = x + Main.rand.Next(10, 31) * (Main.rand.NextBool() ? 1 : -1);
                        int newY = y + Main.rand.Next(-15, 16);
                        Vector2 newPos = new(newX * 16, newY * 16);
                        if (!Collision.SolidCollision(newPos, npc.width, npc.height))
                        {
                            //npc.Center = newPos;
                            Projectile.NewProjectile(npc.GetSource_FromThis(), newPos, Vector2.Zero, ModContent.ProjectileType<GlowRingHollow>(), 0, 0f, Main.myPlayer, 10, npc.whoAmI);
                            break;
                        }
                    }
                }
                for (int i = 0; i < 40; ++i)
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, DustID.PinkTorch, 0.0f, 0.0f, 0, new Color(), 1f);
                    Dust dust = Main.dust[d];
                    dust.velocity *= 4f;
                    Main.dust[d].noGravity = true;
                    Main.dust[d].scale += 1.5f;
                }
                SoundEngine.PlaySound(SoundID.Item8, npc.Center);
                npc.netUpdate = true;
            }
            */
        }
        #region Attacks
        private void MirageDeathray(NPC npc, Player player)
        {
            const int WindupDuration = 60 * 1;
            const int AttackDuration = 60 * 2;
            const int EndlagDuration = 60 * 0;
            void Windup()
            {
                if (AttackTimer == 1 && FargoSoulsUtil.HostCheck)
                {
                    int randReal = Main.rand.Next(4);
                    float rand = Main.rand.NextFloat();
                    for (int i = 0; i < 4; i++)
                    {
                        float num = i + rand;
                        int fake = randReal == i ? 0 : 1;
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<NebulaPillarProj>(), FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage * 2), 3f, Main.myPlayer, num, fake, npc.whoAmI);
                    }
                }
            }
            void Attack()
            {

            }
            void Endlag()
            {

            }

            if (AttackTimer <= WindupDuration)
            {
                Windup();
            }
            else if (AttackTimer <= WindupDuration + AttackDuration)
            {
                Attack();
            }
            else
            {
                Endlag();
            }
            if (AttackTimer > WindupDuration + AttackDuration + EndlagDuration)
            {
                EndAttack(npc);
            }
        }
        private Vector2 tpPos = Vector2.Zero;
        private void TeleportJumpscare(NPC npc, Player player)
        {
            const int WindupDuration = 100;
            const int AttackDuration = 45;
            const int EndlagDuration = 60;
            const int SafeAngle = 30;
            void Windup()
            {
                if (AttackTimer == 1) //set random
                {
                    const int tpVar = 300;
                    tpPos = player.Center + (Vector2.UnitY * Main.rand.NextFloat(0, tpVar)) + (Main.rand.NextFloat(-tpVar, tpVar) * Vector2.UnitX);
                    SoundEngine.PlaySound(SoundID.NPCDeath58, tpPos);
                    npc.netUpdate = true;
                    NetSync(npc);
                }
                if (AttackTimer == 10) //use random, telegraph tp
                {
                    npc.netUpdate = true;
                    NetSync(npc);
                    if (FargoSoulsUtil.HostCheck)
                    {
                        int time = WindupDuration - 10;
                        Projectile.NewProjectile(npc.GetSource_FromThis(), tpPos, Vector2.Zero, ModContent.ProjectileType<NebulaTelegraph>(), 0, 0, Main.myPlayer, time);
                        for (int i = -1; i < 2; i += 2)
                        {
                            float angle = (-Vector2.UnitY).RotatedBy(i * MathHelper.ToRadians(SafeAngle)).ToRotation();
                            Projectile.NewProjectile(npc.GetSource_FromThis(), tpPos, Vector2.Zero, ModContent.ProjectileType<BloomLine>(), 0, 0, Main.myPlayer, 5, angle, time);
                            Projectile.NewProjectile(npc.GetSource_FromThis(), tpPos, Vector2.Zero, ModContent.ProjectileType<BloomLine>(), 0, 0, Main.myPlayer, 6, i, time);
                        }
                    }
                }
            }
            void Attack()
            {
                if (AttackTimer - WindupDuration == 1)
                {
                    npc.Center = tpPos;
                    npc.netUpdate = true;
                    NetSync(npc);
                    SoundEngine.PlaySound(SoundID.Item8, tpPos);
                }
                float ratio = ((float)AttackTimer - WindupDuration) / AttackDuration;
                const int AttackCD = 2;
                if ((AttackTimer - WindupDuration) % AttackCD == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item20, npc.Center);
                    if (FargoSoulsUtil.HostCheck)
                    {
                        for (int i = -1; i < 2; i += 2)
                        {
                            Vector2 angle = (Vector2.UnitY).RotatedBy(i * ratio * MathHelper.ToRadians(150));
                            float speed = 16;
                            Vector2 vel = angle * speed;
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, vel, ModContent.ProjectileType<PillarNebulaBlaze>(), FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage), 3f, Main.myPlayer, 0.02f, ai2: npc.whoAmI);
                        }
                    }
                }
            }
            void Endlag()
            {

            }

            if (AttackTimer <= WindupDuration)
            {
                Windup();
            }
            else if (AttackTimer <= WindupDuration + AttackDuration)
            {
                Attack();
            }
            else
            {
                Endlag();
            }
            if (AttackTimer > WindupDuration + AttackDuration + EndlagDuration)
            {
                EndAttack(npc);
            }
        }
        private void MassiveNebulaArcanum(NPC npc, Player player)
        {
            const int WindupDuration = 0;
            const int AttackDuration = 60 * 4;
            const int EndlagDuration = 60 * 1;
            void Attack()
            {
                if (AttackTimer == WindupDuration + 1)
                {
                    SoundEngine.PlaySound(SoundID.Item117, npc.Center);
                    if (FargoSoulsUtil.HostCheck)
                    {
                        Vector2 offset = -(Vector2.UnitY * npc.height / 2);
                        Vector2 vel = Vector2.Normalize(offset);
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center + offset, vel, ModContent.ProjectileType<PillarArcanum>(), FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage), 3f, Main.myPlayer, 0f, 1);
                    }
                }
            }
            void Endlag()
            {

            }

            if (AttackTimer <= WindupDuration)
            {
            }
            else if (AttackTimer <= WindupDuration + AttackDuration)
            {
                Attack();
            }
            else
            {
                Endlag();
            }
            if (AttackTimer > WindupDuration + AttackDuration + EndlagDuration)
            {
                EndAttack(npc);
            }
        }
        private void Idle(NPC npc, Player player)
        {
            const int WindupDuration = 60 * 0;
            const int AttackDuration = 60 * 2;
            const int EndlagDuration = 60 * 2;
            void Windup()
            {
            }
            void Attack()
            {
                /*
                const int AttackCD = 40;
                
                if (AttackTimer % AttackCD == AttackCD - 1)
                {
                    SoundEngine.PlaySound(SoundID.Item20, npc.Center);
                    if (FargoSoulsUtil.HostCheck)
                    {
                        int speed = Main.rand.Next(6, 8);
                        Vector2 pos = npc.Center - (0.65f * npc.height * Vector2.UnitY) + Vector2.UnitX * Main.rand.NextFloat(-npc.width / 3, npc.width / 3);
                        Vector2 vel = Vector2.Normalize(pos - npc.Center) * speed;
                        Projectile.NewProjectile(npc.GetSource_FromThis(), pos, vel, ModContent.ProjectileType<PillarNebulaBlaze>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 3f, Main.myPlayer, 0.03f, ai2: npc.whoAmI);
                    }
                    
                }
                */
                const int Attack1 = WindupDuration + 5;
                const int Attack2 = WindupDuration + AttackDuration - 5;
                if (AttackTimer == Attack1 || AttackTimer == Attack2)
                {
                    SoundEngine.PlaySound(SoundID.Item20, npc.Center);
                    if (FargoSoulsUtil.HostCheck)
                    {
                        const int CircleProjs = 5;
                        for (int i = 0; i < CircleProjs; i++)
                        {
                            float degOff = i;
                            if (AttackTimer == Attack2)
                                degOff += 0.5f;
                            Vector2 posOffset = Vector2.UnitX.RotatedBy(MathHelper.TwoPi * (float)degOff / CircleProjs);
                            posOffset *= 1300;
                            float speed = Main.rand.NextFloat(5f, 6f);
                            speed *= 0.9f;
                            Vector2 pos = player.Center + posOffset;
                            Vector2 vel = pos.SafeDirectionTo(player.Center) * speed;
                            Projectile.NewProjectile(npc.GetSource_FromThis(), pos, vel, ModContent.ProjectileType<PillarNebulaBlaze>(), FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage), 3f, Main.myPlayer, 0.03f, ai2: npc.whoAmI);
                        }

                    }
                }
            }
            void Endlag()
            {

            }

            if (AttackTimer <= WindupDuration)
            {
                Windup();
            }
            else if (AttackTimer <= WindupDuration + AttackDuration)
            {
                Attack();
            }
            else
            {
                Endlag();
            }
            if (AttackTimer > WindupDuration + AttackDuration + EndlagDuration)
            {
                RandomAttack(npc);
            }
        }
        #endregion
    }
}
