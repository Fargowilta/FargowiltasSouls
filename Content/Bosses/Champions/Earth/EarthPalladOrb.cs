using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Champions.Earth
{
    public class EarthPalladOrb : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Content/Projectiles/Souls/PalladOrb";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Palladium Life Orb");
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.hostile = true;
            Projectile.timeLeft = 1200;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale = 2f;
            Projectile.extraUpdates = 3;

            CooldownSlot = 1;
        }

        public override void AI()
        {
            if (Projectile.localAI[1] == 0)
            {
                Projectile.localAI[1] = 1;
                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            }

            if (Projectile.timeLeft % (Projectile.extraUpdates + 1) == 0 && ++Projectile.localAI[1] > 30)
            {
                if (Projectile.localAI[1] < 90) //accelerate
                {
                    Projectile.velocity *= 1.035f;
                }

                if (Projectile.localAI[1] > 60 && Projectile.localAI[1] < 150
                    && FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.championBoss, ModContent.NPCType<EarthChampion>())
                    && Main.npc[EModeGlobalNPC.championBoss].HasValidTarget) //home
                {
                    float rotation = Projectile.velocity.ToRotation();
                    Vector2 vel = Main.player[Main.npc[EModeGlobalNPC.championBoss].target].Center
                        + Main.player[Main.npc[EModeGlobalNPC.championBoss].target].velocity * 10f - Projectile.Center;
                    float targetAngle = vel.ToRotation();
                    Projectile.velocity = new Vector2(Projectile.velocity.Length(), 0f).RotatedBy(rotation.AngleLerp(targetAngle, 0.03f));
                }
            }

            int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Main.rand.NextBool() ? 174 : 259, 0f, 0f, 100, new Color(), 2f);
            Main.dust[index2].noGravity = true;
            Main.dust[index2].velocity *= 3;
            int index3 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Main.rand.NextBool() ? 174 : 259, 0f, 0f, 100, new Color(), 1f);
            Main.dust[index3].velocity *= 2f;
            Main.dust[index3].noGravity = true;

            Projectile.rotation += 0.4f;

            if (++Projectile.frameCounter > 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (timeLeft > 0)
            {
                Projectile.timeLeft = 0;
                Projectile.position = Projectile.Center;
                Projectile.width = 500;
                Projectile.height = 500;
                Projectile.Center = Projectile.position;
                Projectile.penetrate = -1;
                Projectile.Damage();
            }

            //if (!Main.dedServ && Main.LocalPlayer.active) Main.LocalPlayer.FargoSouls().Screenshake = 30;

            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 3.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 7f;
                dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 1.5f);
                Main.dust[dust].velocity *= 3f;
            }

            for (int index1 = 0; index1 < 20; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Main.rand.NextBool() ? 174 : 259, 0f, 0f, 100, new Color(), 4f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity *= 21f * Projectile.scale;
                int index3 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Main.rand.NextBool() ? 174 : 259, 0f, 0f, 100, new Color(), 2.5f);
                Main.dust[index3].velocity *= 12f;
                Main.dust[index3].noGravity = true;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (WorldSavingSystem.EternityMode)
            {
                target.AddBuff(ModContent.BuffType<Buffs.Masomode.PurifiedBuff>(), 300);
                target.AddBuff(ModContent.BuffType<Buffs.Masomode.LethargicBuff>(), 300);
            }

            if (Projectile.timeLeft > 0)
                Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            SpriteEffects effects = Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }
    }
}