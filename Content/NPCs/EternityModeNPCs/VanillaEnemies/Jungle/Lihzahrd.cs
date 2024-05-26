using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Jungle
{
    public class Lihzahrd : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.Lihzahrd,
            NPCID.LihzahrdCrawler
        );

        public int FireballCounter;

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.trapImmune = true;
        }

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            npc.buffImmune[BuffID.Poisoned] = true;
            npc.buffImmune[BuffID.Venom] = true;
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (++FireballCounter > 30)
            {
                FireballCounter = -90;
                if (npc.HasPlayerTarget && FargoSoulsUtil.HostCheck && Collision.CanHitLine(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0))
                {
                    Vector2 vel = npc.SafeDirectionTo(Main.player[npc.target].Center) * 12f;
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, vel, ProjectileID.Fireball, FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage), 0f, Main.myPlayer);
                }
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(ModContent.BuffType<InfestedBuff>(), 300);
            //target.AddBuff(ModContent.BuffType<Bloodthirsty>(), 120);
        }

        public override void OnKill(NPC npc)
        {
            base.OnKill(npc);

            /*if (FargoSoulsUtil.HostCheck)
                Projectile.NewProjectile(npc.Center, Vector2.UnitY * -6, ProjectileID.SpikyBallTrap, 30, 0f, Main.myPlayer);*/
        }
    }
}
