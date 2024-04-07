using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.Projectiles.BossWeapons
{
    public class SparklingLoveHeart2 : SparklingLoveHeart
    {
        public override string Texture => "FargowiltasSouls/Content/Projectiles/Masomode/FakeHeart";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Projectile.DamageType = Terraria.ModLoader.DamageClass.Summon;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Lovestruck, 300);
            target.immune[Projectile.owner] = 1;

            /*if (Projectile.owner == Main.myPlayer)
            {
                int healAmount = 2;
                Main.player[Main.myPlayer].HealEffect(healAmount);
                Main.player[Main.myPlayer].statLife += healAmount;

                if (Main.player[Main.myPlayer].statLife > Main.player[Main.myPlayer].statLifeMax2)
                    Main.player[Main.myPlayer].statLife = Main.player[Main.myPlayer].statLifeMax2;
            }*/
        }
    }
}