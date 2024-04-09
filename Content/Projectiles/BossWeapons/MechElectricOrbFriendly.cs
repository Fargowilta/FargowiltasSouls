using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.BossWeapons
{
    public class MechElectricOrbFriendly : Masomode.MechElectricOrb
    {
        public override string Texture => "FargowiltasSouls/Content/Projectiles/Masomode/MechElectricOrb";

        bool hasIframes = true;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.timeLeft = 75;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_Parent parent && parent.Entity is Projectile sourceProj && sourceProj.type == ModContent.ProjectileType<RefractorBlaster2Held>())
            {
                Projectile.penetrate = 1;
                hasIframes = false;
            }
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.rotation + (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * Projectile.direction;
            Projectile.soundDelay = 0;
            if (Projectile.velocity.Length() < 22) //fix stars not being aligned properly by making sure their total velocity is the same???
            {
                Projectile.velocity.Normalize();
                Projectile.velocity *= 22;
            }
            Projectile.velocity *= 1.02f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (hasIframes)
                target.immune[Projectile.owner] = 6;
        }

        public override bool PreKill(int timeleft)
        {
            int num1 = 10;
            int num2 = 3;

            int dustID = ColorType switch
            {
                Blue => DustID.BlueTorch,
                Green => DustID.GreenTorch,
                Yellow => DustID.YellowTorch,
                _ => DustID.RedTorch
            };

            for (int index = 0; index < num1; ++index)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustID, Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, 150, new Color(), 1.2f);
            for (int index = 0; index < num2; ++index)
            {
                int Type = Main.rand.Next(16, 18);
                if (Projectile.type == 503)
                    Type = 16;
                //if (!Main.dedServ)
                //Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position, new Vector2(Projectile.velocity.X * 0.05f, Projectile.velocity.Y * 0.05f), Type, 1f);
            }

            for (int index = 0; index < 10; ++index)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustID, Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, 150, new Color(), 1.2f);
            //for (int index = 0; index < 3; ++index)
            //if (!Main.dedServ)
            //Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position, new Vector2(Projectile.velocity.X * 0.05f, Projectile.velocity.Y * 0.05f), Main.rand.Next(16, 18), 1f);

            return false;
        }
    }
}