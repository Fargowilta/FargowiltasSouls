using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.BloodMoon
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
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromAI(), npc.Center, NPCID.DemonEye, velocity: Main.rand.NextVector2Circular(8, 8));
            }

            if (npc.life < npc.lifeMax / 2 && ++SickleTimer > 15)
            {
                SickleTimer = 0;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, 1.5f * Vector2.Normalize(npc.velocity), ModContent.ProjectileType<BloodScythe>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer);
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(ModContent.BuffType<CurseoftheMoon>(), 120);
            target.AddBuff(ModContent.BuffType<Anticoagulation>(), 600);
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.FrogLeg, 10));
            FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.BalloonPufferfish, 10));
        }
    }
}
