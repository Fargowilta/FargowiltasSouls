using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Souls
{
    public class SpookyScythe : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Scythe");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 106;
            projectile.height = 84;
            projectile.aiStyle = 0;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.penetrate = 10;
            projectile.timeLeft = 50;
            aiType = ProjectileID.CrystalBullet;
            projectile.tileCollide = false;
            projectile.scale *= .5f;
            projectile.timeLeft = 300;

            //this deals more dmg generally but e
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = -1;

            //projectile.usesIDStaticNPCImmunity = true;
            //projectile.idStaticNPCHitCooldown = 10;
        }

        public override void AI()
        {
            //dust!
            int dustId = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y + 2f), projectile.width, projectile.height + 5, 55, projectile.velocity.X * 0.2f,
                projectile.velocity.Y * 0.2f, 100, default(Color), 1f);
            Main.dust[dustId].noGravity = true;
            int dustId3 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y + 2f), projectile.width, projectile.height + 5, 55, projectile.velocity.X * 0.2f,
                projectile.velocity.Y * 0.2f, 100, default(Color), 1f);
            Main.dust[dustId3].noGravity = true;

            projectile.rotation += 0.4f;

            if (projectile.penetrate < 10)
            {
                projectile.timeLeft = 10;
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Dig, (int) projectile.position.X, (int) projectile.position.Y);
            for (int num489 = 0; num489 < 5; num489++)
            {
                int num490 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 55, 10f, 30f, 100);
                Main.dust[num490].noGravity = true;
                Main.dust[num490].velocity *= 1.5f;
                Main.dust[num490].scale *= 0.9f;
            }

            /*for (int i = 0; i < 3; i++)
            {
                float x = -projectile.velocity.X * Main.rand.Next(40, 70) * 0.01f + Main.rand.Next(-20, 21) * 0.4f;
                float y = -projectile.velocity.Y * Main.rand.Next(40, 70) * 0.01f + Main.rand.Next(-20, 21) * 0.4f;
                int p = Projectile.NewProjectile(projectile.position.X + x, projectile.position.Y + y, x, y, 45, (int) (projectile.damage * 0.5), 0f, projectile.owner);

                Main.projectile[p].GetGlobalProjectile<FargoGlobalProjectile>().CanSplit = false;
            } */
        }
    }
}