using FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.LunarEvents.Solar;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.LunarEvents
{
    public class PillarSpawner : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cosmic Meteor");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override string Texture => FargoSoulsUtil.EmptyTexture;
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.DamageType = DamageClass.Default;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 60 * 30;
            Projectile.scale = 1;
            AIType = 0;
            Projectile.aiStyle = 0;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 2)
                Projectile.Kill();
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.ai[0] == 2)
            {
                int p = Player.FindClosest(Projectile.Center, 0, 0);
                if (p.IsWithinBounds(Main.maxPlayers))
                {
                    Player player = Main.player[p];
                    if (player.active && player.Center.Y > Projectile.Center.Y)
                    {
                        return false;
                    }
                }
            }
            Projectile.Kill();
            return true;
        }
        public override void OnKill(int timeLeft)
        {
            switch (Projectile.ai[0])
            {
                case 1: //solar pillar flamepillar
                    {
                        SoundEngine.PlaySound(SoundID.Item34, Projectile.Center);
                        if (FargoSoulsUtil.HostCheck)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<SolarFlamePillar>(), Projectile.damage, Projectile.knockBack, Main.myPlayer);
                        }
                        break;
                    }
                default:
                    {
                        Main.NewText("you shouldn't be seeing this, show javyz");
                        break;
                    }
            }

        }
    }
}