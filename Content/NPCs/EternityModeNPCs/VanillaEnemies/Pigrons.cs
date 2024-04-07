using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies
{
    public class Pigrons : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.PigronCorruption,
            NPCID.PigronCrimson,
            NPCID.PigronHallow
        );

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(ModContent.BuffType<SqueakyToyBuff>(), 120);
            //target.AddBuff(ModContent.BuffType<AnticoagulationBuff>(), 600);
            target.FargoSouls().MaxLifeReduction += 50;
            target.AddBuff(ModContent.BuffType<OceanicMaulBuff>(), 1800);
        }

        public override void OnKill(NPC npc)
        {
            base.OnKill(npc);

            if (FargoSoulsUtil.HostCheck)
                Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ProjectileID.Cthulunado, FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer, 16, 11);
        }
    }
}
