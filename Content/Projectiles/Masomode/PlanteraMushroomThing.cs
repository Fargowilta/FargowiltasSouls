using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class PlanteraMushroomThing : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 3;
            ProjectileID.Sets.TrailCacheLength[Type] = 3;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.PoisonSeedPlantera);
            Projectile.aiStyle = -1;
            Projectile.alpha = 0;
            Projectile.width = Projectile.height = 24;
        }
        public override void AI()
        {
            bool recolor = SoulConfig.Instance.BossRecolors && WorldSavingSystem.EternityMode;
            if (recolor)
                Lighting.AddLight(Projectile.Center, 25f / 255, 47f / 255, 64f / 255);
            else
                Lighting.AddLight(Projectile.Center, 0.1f, 0.4f, 0.2f);

            if (Projectile.localAI[0] != 1)
            {
                SoundEngine.PlaySound(SoundID.NPCDeath13, Projectile.Center);
                Projectile.localAI[0] = 1;
            }
            if (++Projectile.frameCounter > 4)
            {
                if (++Projectile.frame >= Main.projFrames[Type])
                    Projectile.frame = 0;
                Projectile.frameCounter = 0;
            }
            /*
            if (Projectile.ai[0] >= 35f)
            {
                Projectile.ai[0] = 35f;
                Projectile.velocity.Y += 0.025f;
            }
            */
            if (Projectile.ai[0] < 20)
                Projectile.ai[0] += 0.5f;
            float num216 = Projectile.ai[0] + 4;
            int num217 = Player.FindClosest(Projectile.Center, 1, 1);
            Vector2 vector30 = Main.player[num217].Center - Projectile.Center;
            vector30.Normalize();
            vector30 *= num216;
            int num218 = 70;
            Projectile.velocity = (Projectile.velocity * (num218 - 1) + vector30) / num218;
            if (Projectile.velocity.Length() != Projectile.ai[0])
            {
                Projectile.velocity.Normalize();
                Projectile.velocity *= Projectile.ai[0];
            }
            Projectile.tileCollide = false;
            if (Projectile.timeLeft > 180)
            {
                Projectile.timeLeft = 180;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<IvyVenomBuff>(), 240);
            target.AddBuff(BuffID.Poisoned, 300);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            bool recolor = SoulConfig.Instance.BossRecolors && WorldSavingSystem.EternityMode;
            Texture2D texture = recolor ? TextureAssets.Projectile[Type].Value : ModContent.Request<Texture2D>(Texture + "Vanilla").Value;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color2 = recolor ? Color.Blue : Color.Pink * 0.75f;
                color2 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                color2 *= 0.5f;
                Vector2 pos = Projectile.oldPos[i] + Projectile.Size / 2;
                float rot = Projectile.oldRot[i];
                FargoSoulsUtil.GenericProjectileDraw(Projectile, color2, texture: texture, drawPos: pos, rotation: rot);
            }
            FargoSoulsUtil.GenericProjectileDraw(Projectile, Color.White, texture: texture);
            return false;
        }
    }
}
