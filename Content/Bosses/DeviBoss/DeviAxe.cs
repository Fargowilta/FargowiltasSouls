using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.DeviBoss
{
    public class DeviAxe : ModProjectile
    {
        public override string Texture => FargoSoulsUtil.EmptyTexture;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Sparkling Love");
        }

        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 46;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60 * 10;
            Projectile.hide = true;
            Projectile.penetrate = -1;
            Projectile.FargoSouls().DeletionImmuneRank = 2;
            CooldownSlot = 1;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
                return true;

            //linger the hitbox so player doesn't phase through by flying towards it
            Rectangle trailHitbox = projHitbox;
            trailHitbox.X = (int)Projectile.oldPosition.X;
            trailHitbox.Y = (int)Projectile.oldPosition.Y;
            if (trailHitbox.Intersects(targetHitbox))
                return true;

            //lerp so there's no gap
            trailHitbox = projHitbox;
            trailHitbox.X = (int)MathHelper.Lerp(Projectile.position.X, Projectile.oldPosition.X, 0.5f);
            trailHitbox.Y = (int)MathHelper.Lerp(Projectile.position.Y, Projectile.oldPosition.Y, 0.5f);
            if (trailHitbox.Intersects(targetHitbox))
                return true;

            return false;
        }

        public override void AI()
        {
            //the important part
            NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[0], ModContent.NPCType<DeviBoss>());
            if (npc != null)
            {
                if (Projectile.localAI[0] == 0)
                {
                    Projectile.localAI[0] = 1;
                    Projectile.localAI[1] = Projectile.DirectionFrom(npc.Center).ToRotation();
                }

                Vector2 offset = new Vector2(Projectile.ai[1], 0).RotatedBy(npc.ai[3] + Projectile.localAI[1]);
                Projectile.Center = npc.Center + offset;
            }
            else
            {
                Projectile.Kill();
                return;
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath6, Projectile.Center);
            /*Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 208;
            Projectile.Center = Projectile.position;
            for (int index1 = 0; index1 < 3; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 31, 0.0f, 0.0f, 100, new Color(), 1.5f);
                Main.dust[index2].position = new Vector2((float)(Projectile.width / 2), 0.0f).RotatedBy(6.28318548202515 * Main.rand.NextDouble(), new Vector2()) * (float)Main.rand.NextDouble() + Projectile.Center;
            }
            for (int index1 = 0; index1 < 10; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 86, 0.0f, 0.0f, 0, new Color(), 2.5f);
                Main.dust[index2].position = new Vector2((float)(Projectile.width / 2), 0.0f).RotatedBy(6.28318548202515 * Main.rand.NextDouble(), new Vector2()) * (float)Main.rand.NextDouble() + Projectile.Center;
                Main.dust[index2].noGravity = true;
                Dust dust1 = Main.dust[index2];
                dust1.velocity = dust1.velocity * 1f;
                int index3 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 86, 0.0f, 0.0f, 100, new Color(), 1.5f);
                Main.dust[index3].position = new Vector2((float)(Projectile.width / 2), 0.0f).RotatedBy(6.28318548202515 * Main.rand.NextDouble(), new Vector2()) * (float)Main.rand.NextDouble() + Projectile.Center;
                Dust dust2 = Main.dust[index3];
                dust2.velocity = dust2.velocity * 1f;
                Main.dust[index3].noGravity = true;
            }

            for (int i = 0; i < 10; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 86, 0f, 0f, 100, default, 3f);
                Main.dust[dust].velocity *= 1.4f;
            }

            for (int i = 0; i < 10; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6, 0f, 0f, 100, default, 3.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 7f;
                dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6, 0f, 0f, 100, default, 1.5f);
                Main.dust[dust].velocity *= 3f;
            }

            for (int index1 = 0; index1 < 15; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 86, 0f, 0f, 100, new Color(), 2f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity *= 21f * Projectile.scale;
                int index3 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 86, 0f, 0f, 100, new Color(), 1f);
                Main.dust[index3].velocity *= 12f;
                Main.dust[index3].noGravity = true;
            }

            for (int i = 0; i < 15; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 86, 0f, 0f, 100, default, Main.rand.NextFloat(2f, 3.5f));
                if (Main.rand.NextBool(3))
                    Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= Main.rand.NextFloat(9f, 12f);
                Main.dust[d].position = Projectile.Center;
            }*/
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.velocity.X = target.Center.X < Main.npc[(int)Projectile.ai[0]].Center.X ? -15f : 15f;
            target.velocity.Y = -10f;
            target.AddBuff(ModContent.BuffType<Buffs.Masomode.LovestruckBuff>(), 240);
        }
    }
}