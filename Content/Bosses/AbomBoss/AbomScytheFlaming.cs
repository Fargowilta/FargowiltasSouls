using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.AbomBoss
{
    public class AbomScytheFlaming : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Abominationn Scythe");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 80;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 720;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            CooldownSlot = 1;
        }

        public override bool? CanDamage()
        {
            return Projectile.ai[1] <= 0 || WorldSavingSystem.MasochistModeReal;
        }
        public ref float TargetID => ref Projectile.ai[2];
        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = Main.rand.NextBool() ? 1 : -1;
                Projectile.localAI[1] = Projectile.ai[1] - Projectile.ai[0]; //store difference for animated spin startup
                Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            }

            if (--Projectile.ai[0] == 0)
            {
                Projectile.netUpdate = true;
                Projectile.velocity = Vector2.Zero;
            }
            int targetID = (int)TargetID;
            if (--Projectile.ai[1] == 0 && targetID.IsWithinBounds(Main.maxPlayers))
            {
                Projectile.netUpdate = true;
                Player target = Main.player[targetID];
                if (!target.Alive())
                    return;
                Projectile.velocity = Projectile.SafeDirectionTo(target.Center);
                if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.abomBoss, ModContent.NPCType<AbomBoss>()) && Main.npc[EModeGlobalNPC.abomBoss].localAI[3] > 1)
                    Projectile.velocity *= 7f;
                else
                    Projectile.velocity *= 24f;
                SoundEngine.PlaySound(SoundID.Item84, Projectile.Center);
            }

            float rotation = Projectile.ai[0] < 0 && Projectile.ai[1] > 0 ? 1f - Projectile.ai[1] / Projectile.localAI[1] : 0.8f;
            Projectile.rotation += rotation * Projectile.localAI[0];
        }

        public override void OnKill(int timeLeft)
        {
            int dustMax = 20;
            float speed = 12;
            for (int i = 0; i < dustMax; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemTopaz, Scale: 3.5f);
                Main.dust[d].velocity *= speed;
                Main.dust[d].noGravity = true;
            }
            for (int i = 0; i < dustMax; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Scale: 3.5f);
                Main.dust[d].velocity *= speed;
                Main.dust[d].noGravity = true;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (WorldSavingSystem.EternityMode)
            {
                target.AddBuff(ModContent.BuffType<Buffs.Boss.AbomFangBuff>(), 300);
                //target.AddBuff(BuffID.Burning, 180);
                //target.AddBuff(ModContent.BuffType<Rotting>(), 900);
                //target.AddBuff(ModContent.BuffType<LivingWasteland>(), 900);
            }
            target.AddBuff(BuffID.OnFire, 900);
            target.AddBuff(BuffID.Weak, 900);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = color26;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, Projectile.ai[1] < 0 ? 150 : 255) * Projectile.Opacity * (Projectile.ai[1] <= 0 || WorldSavingSystem.MasochistModeReal ? 1f : 0.5f);
        }
    }
}