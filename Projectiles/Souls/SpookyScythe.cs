using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Souls
{
    public class SpookyScythe : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Scythe");
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 106;
            Projectile.height = 84;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = 10;
            Projectile.timeLeft = 50;
            AIType = ProjectileID.CrystalBullet;
            Projectile.tileCollide = false;
            Projectile.scale *= .5f;
            Projectile.timeLeft = 300;

            //this deals more dmg generally but e
            /*Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;*/

            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().noInteractionWithNPCImmunityFrames = true;
        }

        public override void AI()
        {
            //dust!
            int dustId = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y + 2f), Projectile.width, Projectile.height + 5, 55, Projectile.velocity.X * 0.2f,
                Projectile.velocity.Y * 0.2f, 100, default(Color), 1f);
            Main.dust[dustId].noGravity = true;
            int dustId3 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y + 2f), Projectile.width, Projectile.height + 5, 55, Projectile.velocity.X * 0.2f,
                Projectile.velocity.Y * 0.2f, 100, default(Color), 1f);
            Main.dust[dustId3].noGravity = true;

            Projectile.rotation += 0.4f;

            if (Projectile.penetrate < 10)
            {
                Projectile.timeLeft = 10;
            }
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            for (int num489 = 0; num489 < 5; num489++)
            {
                int num490 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 55, 10f, 30f, 100);
                Main.dust[num490].noGravity = true;
                Main.dust[num490].velocity *= 1.5f;
                Main.dust[num490].scale *= 0.9f;
            }

            /*for (int i = 0; i < 3; i++)
            {
                float x = -Projectile.velocity.X * Main.rand.Next(40, 70) * 0.01f + Main.rand.Next(-20, 21) * 0.4f;
                float y = -Projectile.velocity.Y * Main.rand.Next(40, 70) * 0.01f + Main.rand.Next(-20, 21) * 0.4f;
                int p = Projectile.NewProjectile(Projectile.position.X + x, Projectile.position.Y + y, x, y, 45, (int) (Projectile.damage * 0.5), 0f, Projectile.owner);

                Main.projectile[p].GetGlobalProjectile<FargoSoulsGlobalProjectile>().CanSplit = false;
            } */
        }
    }
}