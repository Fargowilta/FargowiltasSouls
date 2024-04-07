using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class SkeletronBone : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_471";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Bone");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.SkeletonBone);
            AIType = ProjectileID.SkeletonBone;
            Projectile.light = 1f;
            Projectile.scale = 1.5f;
            Projectile.timeLeft = 240;
            Projectile.tileCollide = false;
            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.guardBoss, NPCID.DungeonGuardian)
                || FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.skeleBoss, NPCID.SkeletronHead) && Main.npc[EModeGlobalNPC.skeleBoss].ai[1] == 2f)
            {
                CooldownSlot = 1;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.guardBoss, NPCID.DungeonGuardian))
            {
                target.AddBuff(ModContent.BuffType<MarkedforDeathBuff>(), 300);
                /*target.AddBuff(ModContent.BuffType<GodEater>(), 420);
                target.AddBuff(ModContent.BuffType<FlamesoftheUniverse>(), 420);
                target.immune = false;
                target.immuneTime = 0;
                target.hurtCooldowns[1] = 0;*/
            }
            target.AddBuff(ModContent.BuffType<LethargicBuff>(), 300);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            NPC sourceNPC = Projectile.GetSourceNPC();
            bool recolor =
                (sourceNPC != null && (sourceNPC.type == NPCID.SkeletronHead || sourceNPC.type == NPCID.SkeletronHand || sourceNPC.type == NPCID.DungeonGuardian)) &&
                SoulConfig.Instance.BossRecolors && WorldSavingSystem.EternityMode;

            Texture2D texture = recolor ? ModContent.Request<Texture2D>("FargowiltasSouls/Content/Projectiles/Masomode/SkeletronBone_Recolor").Value : TextureAssets.Projectile[Type].Value;
            FargoSoulsUtil.ProjectileWithTrailDraw(Projectile, Color.White * Projectile.Opacity, texture, additiveTrail: true);
            FargoSoulsUtil.GenericProjectileDraw(Projectile, lightColor, texture);
            return false;
        }
    }
}