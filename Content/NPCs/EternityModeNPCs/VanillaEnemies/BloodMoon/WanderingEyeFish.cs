using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Projectiles.Masomode;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.BloodMoon
{
    public class WanderingEyeFish : Night.DemonEyes
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.EyeballFlyingFish);

        public int SickleTimer;
        public int SpawnTimer = 60;

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (SpawnTimer > 0 && --SpawnTimer % 5 == 0)
            {
                if (FargoSoulsUtil.HostCheck)
                    FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromAI(), npc.Center, NPCID.DemonEye, velocity: Main.rand.NextVector2Circular(8, 8));
            }

            if (npc.life < npc.lifeMax / 2 && ++SickleTimer > 15)
            {
                SickleTimer = 0;
                if (FargoSoulsUtil.HostCheck)
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, 1.5f * Vector2.Normalize(npc.velocity), ModContent.ProjectileType<BloodScythe>(), FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage), 0f, Main.myPlayer);
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(ModContent.BuffType<CurseoftheMoonBuff>(), 120);
            target.AddBuff(ModContent.BuffType<AnticoagulationBuff>(), 600);
        }
    }
}
