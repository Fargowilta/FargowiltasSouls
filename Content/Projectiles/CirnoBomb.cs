using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Content.Items.Accessories.Masomode;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles
{
    public class CirnoBomb : GlowRing
    {
        public override string Texture => "FargowiltasSouls/Content/Projectiles/GlowRing";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cirno Bomb");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.alpha = 0;
            Projectile.hostile = false;
            Projectile.friendly = true;
            color = Color.Cyan;

            Projectile.FargoSouls().TimeFreezeImmune = true;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;

                for (int i = 0; i < 30; i++)
                {
                    int d = Dust.NewDust(Projectile.Center, 0, 0, DustID.IceTorch, 0, 0, 0, new Color(), 2f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 4f;
                }

                SoundEngine.PlaySound(SoundID.Item4, Projectile.Center);
            }

            Player player = Main.player[Projectile.owner];
            FargoSoulsPlayer fargoPlayer = player.FargoSouls();

            if (!player.active || player.dead || player.ghost || Projectile.owner == Main.myPlayer && (!fargoPlayer.CirnoGraze || fargoPlayer.CirnoGrazeCounter < IceQueensCrown.CIRNO_GRAZE_THRESHOLD))
            {
                Projectile.Kill();
                return;
            }

            if (Projectile.ai[0] != 1) //if ai0 = 1, time to die
                Projectile.timeLeft = 2;

            Vector2 target = player.Top + new Vector2(-32f * player.direction, -8);
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, (target - Projectile.Center) / 9, 0.9f / 2);

            Projectile.rotation += MathHelper.TwoPi / 30 * player.direction;

            Projectile.Opacity = Main.mouseTextColor / 255f;
            Projectile.Opacity *= Projectile.Opacity;

            float ratio = (float)(fargoPlayer.CirnoGrazeCounter - IceQueensCrown.CIRNO_GRAZE_THRESHOLD) / (IceQueensCrown.CIRNO_GRAZE_MAX - IceQueensCrown.CIRNO_GRAZE_THRESHOLD);
            Projectile.scale = ratio * 0.7f;
            if (Projectile.scale <= 0.01)
                Projectile.Kill();

            color = Color.Cyan * Projectile.Opacity;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                int d = Dust.NewDust(Projectile.Center, 0, 0, DustID.IceTorch, 0, 0, 0, new Color(), 2f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 3f;
            }

            SoundEngine.PlaySound(SoundID.Item27, Projectile.Center);

            if (Projectile.ai[0] == 1)
            {
                Player player = Main.player[Projectile.owner];

                int freezeRange = 16 * 150;
                int freezeDuration = 180;
                //int slowDuration = freezeDuration + 180;

                foreach (NPC n in Main.npc.Where(n => n.active && !n.friendly && n.damage > 0 && player.Distance(FargoSoulsUtil.ClosestPointInHitbox(n, player.Center)) < freezeRange && !n.dontTakeDamage && !n.buffImmune[ModContent.BuffType<TimeFrozenBuff>()]))
                {
                    n.AddBuff(ModContent.BuffType<TimeFrozenBuff>(), freezeDuration);
                    //n.FargoSouls().SnowChilled = true;
                    //n.FargoSouls().SnowChilledTimer = slowDuration;
                    //n.netUpdate = true;
                }

                foreach (Projectile p in Main.projectile.Where(p => p.active && p.hostile && p.damage > 0 && player.Distance(FargoSoulsUtil.ClosestPointInHitbox(p, player.Center)) < freezeRange && !p.FargoSouls().TimeFreezeImmune && p.FargoSouls().TimeFrozen == 0))
                {
                    p.FargoSouls().TimeFrozen = freezeDuration;
                    if (FargoSoulsUtil.CanDeleteProjectile(p))
                        p.FargoSouls().CirnoBurst = freezeDuration;
                    //p.FargoSouls().ChilledProj = true;
                    //p.FargoSouls().ChilledTimer = slowDuration;
                    //p.netUpdate = true;
                }

                for (int i = 0; i < 40; i++)
                {
                    int d = Dust.NewDust(player.Center, 0, 0, DustID.Snow, 0, 0, 100, Color.White, 2.5f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 6f;
                }

                for (int i = 0; i < 20; i++)
                {
                    int d = Dust.NewDust(player.Center, 0, 0, DustID.IceTorch, 0, 0, 0, default, 2f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 6f;
                }
            }
        }
    }
}