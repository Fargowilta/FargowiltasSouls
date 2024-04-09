using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.LunarEvents.Vortex
{
    public class LunarTowerVortex : LunarTowers
    {
        public override int ShieldStrength
        {
            get => NPC.ShieldStrengthTowerVortex;
            set => NPC.ShieldStrengthTowerVortex = value;
        }

        public override NPCMatcher CreateMatcher() =>
            new NPCMatcher().MatchType(NPCID.LunarTowerVortex);

        public LunarTowerVortex() : base(ModContent.BuffType<JammedBuff>(), DustID.Vortex) { }
        public override int MaxHP => 65000;
        public override int Damage => 80;
        public enum Attacks
        {
            Idle,
            VortexVortex,
            LightningBall,
            SkyLightning,
            LightningElderHu,
            VortexShield

        }
        public override List<int> RandomAttacks => new() //these are randomly chosen attacks in p1
        {
            (int)Attacks.LightningBall,
            (int)Attacks.SkyLightning,
            (int)Attacks.LightningElderHu,
            (int)Attacks.VortexShield

        };
        public override void ShieldsDownAI(NPC npc)
        {
            Player target = Main.player[npc.target];
            npc.ai[1] += 0.5f; // spawns portals half as often
            if (npc.HasPlayerTarget && target.active)
            {
                switch (Attack)
                {
                    case (int)Attacks.VortexVortex:
                        VortexVortex(npc, target);
                        break;
                    case (int)Attacks.LightningBall:
                        LightningBall(npc, target);
                        break;
                    case (int)Attacks.SkyLightning:
                        SkyLightning(npc, target);
                        break;
                    case (int)Attacks.LightningElderHu:
                        LightningElderHu(npc, target);
                        break;
                    case (int)Attacks.VortexShield:
                        VortexShield(npc, target);
                        break;
                    case (int)Attacks.Idle:
                        Idle(npc, target);
                        break;
                }
            }
            else
            {

                if (Attack == (int)Attacks.VortexVortex)
                {
                    EndAttack(npc);
                    foreach (Projectile projectile in Main.projectile.Where(p => p.TypeAlive<VortexVortex>()))
                    {
                        projectile.Kill();
                    }
                }

            }
        }
        #region Attacks
        private int Vortex = -1;
        private void VortexVortex(NPC npc, Player player)
        {
            //no end
            void Attack()
            {


                if (FargoSoulsUtil.HostCheck && AttackTimer % 20 == 1) //anti lag
                {
                    bool vortexAlive = false;
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        if (Main.projectile[i].type == ModContent.ProjectileType<VortexVortex>() && Main.projectile[i].whoAmI == Vortex)
                        {
                            vortexAlive = true;
                            break;
                        }
                    }
                    if (!vortexAlive)
                    {
                        Vector2 pos = npc.Center - Vector2.UnitY * npc.height * 0.8f;
                        Vortex = Projectile.NewProjectile(npc.GetSource_FromThis(), pos, Vector2.Zero, ModContent.ProjectileType<VortexVortex>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer, 0f, 1);
                    }

                }
                if (Vortex.IsWithinBounds(Main.maxProjectiles))
                {
                    if (Main.projectile[Vortex].active)
                    {
                        Main.projectile[Vortex].velocity = (npc.Center - Vector2.UnitY * npc.height * 0.8f) - Main.projectile[Vortex].Center;
                    }
                }
            }
            Attack();
        }
        private void LightningBall(NPC npc, Player player)
        {
            const int WindupDuration = 60 * 1;
            const int AttackDuration = 60 * 4;
            const int EndlagDuration = 60 * 2;
            void Windup()
            {

            }
            void Attack()
            {
                const int AttackDelay = 180;
                if ((AttackTimer - WindupDuration) % AttackDelay == 1)
                {
                    bool second = false;
                    if ((AttackTimer - WindupDuration) % (AttackDelay * 2) == AttackDelay + 1) //every other attack
                    {
                        second = true;
                    }
                    if (FargoSoulsUtil.HostCheck)
                    {
                        Vector2 pos = npc.Center - Vector2.UnitY * npc.height * 0.8f;
                        Vector2 vel = second ? pos.SafeDirectionTo(player.Center) * 6 : Vector2.Zero;
                        Projectile.NewProjectile(npc.GetSource_FromThis(), pos, vel.RotatedBy(Math.PI / 2), ProjectileID.CultistBossLightningOrb, FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 3f, Main.myPlayer);
                        if (second)
                        {
                            Projectile.NewProjectile(npc.GetSource_FromThis(), pos, vel.RotatedBy(-Math.PI / 2), ProjectileID.CultistBossLightningOrb, FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 3f, Main.myPlayer);
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
        private void SkyLightning(NPC npc, Player player)
        {
            const int WindupDuration = 60 * 0;
            const int AttackDuration = 78 * 4;
            const int EndlagDuration = (int)(60 * 1.5f);
            void Windup()
            {

            }
            void Attack()
            {
                const int distance = 180;
                const int AttackDelay = 120;
                if ((AttackTimer - WindupDuration) % AttackDelay == 1)
                {
                    bool second = false;
                    if ((AttackTimer - WindupDuration) % (AttackDelay * 2) == AttackDelay + 1) //every other attack
                    {
                        second = true;
                    }
                    const int Bolts = 24;
                    SoundEngine.PlaySound(NukeBeep, player.Center);
                    for (int i = 0; i < Bolts; i++)
                    {
                        int x = i;
                        if (x >= Bolts / 2)
                        {
                            x = (Bolts / 2) - 1 - x; //split i into 1 to bolts/2 and -1 to -bolts/2
                        }
                        if (FargoSoulsUtil.HostCheck)
                        {
                            int offset = second ? distance / 2 : 0;
                            Vector2 pos = npc.Center + Vector2.UnitX * (distance * x + offset);
                            SpawnLightning(npc, pos);
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
        private void LightningElderHu(NPC npc, Player player)
        {
            const int WindupDuration = 60 * 0;
            const int AttackDuration = 60 * 6;
            const int EndlagDuration = 60 * 0;
            void Windup()
            {

            }
            void Attack()
            {
                const int distance = 100 + 30;
                const int Volleys = 12;
                const int AttackDelay = AttackDuration / Volleys;
                if ((AttackTimer - WindupDuration) % AttackDelay == 0)
                {
                    int i = Volleys - ((AttackTimer - WindupDuration) / AttackDelay);

                    for (int side = -1; side < 2; side += 2)
                    {
                        Vector2 pos = npc.Center + Vector2.UnitX * (distance * i * side);
                        SoundEngine.PlaySound(NukeBeep, pos);
                        if (FargoSoulsUtil.HostCheck)
                        {

                            SpawnLightning(npc, pos);
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
        private void VortexShield(NPC npc, Player player)
        {
            const int AttackDuration = 60 * 3;
            if (AttackTimer == 1) //triggers "shield going down" animation
            {
                npc.ai[3] = 1f;
                npc.netUpdate = true;
                NetSync(npc);
            }
            const int ReactionTime = 40;
            npc.reflectsProjectiles = AttackTimer >= ReactionTime;
            if (npc.reflectsProjectiles) //dust
            {
                for (int i = 0; i < 20; i++)
                {
                    Vector2 offset = new();
                    double angle = Main.rand.NextDouble() * 2d * Math.PI;
                    offset.X += (float)(Math.Sin(angle) * npc.height / 2);
                    offset.Y += (float)(Math.Cos(angle) * npc.height / 2);
                    Dust dust = Main.dust[Dust.NewDust(
                        npc.Center + offset - new Vector2(4, 4), 0, 0,
                        DustID.Vortex, 0, 0, 100, Color.White, 1f
                        )];
                    dust.noGravity = true;
                }
            }

            if (AttackTimer > AttackDuration)
            {
                npc.reflectsProjectiles = false;
                EndAttack(npc);
            }
        }
        const int IdleTime = 120;
        private void Idle(NPC npc, Player player)
        {
            if (AttackTimer > IdleTime)
            {
                RandomAttack(npc);
            }
        }

        private readonly SoundStyle NukeBeep = new("FargowiltasSouls/Assets/Sounds/NukeBeep");
        private void SpawnLightning(NPC parent, Vector2 position)
        {

            if (FargoSoulsUtil.HostCheck)
            {
                Vector2 posOrig = position;
                posOrig.Y = Main.player[parent.target].Center.Y + (150 * 7);
                for (int i = 0; i < 14; i++)
                {
                    Vector2 pos = posOrig - (Vector2.UnitY * 150 * i);
                    Projectile.NewProjectile(parent.GetSource_FromThis(), pos, Vector2.Zero, ModContent.ProjectileType<LightningTelegraph>(), FargoSoulsUtil.ScaledProjectileDamage(parent.damage), 2f, Main.myPlayer, i);
                }
            }
        }
        #endregion
    }
}
