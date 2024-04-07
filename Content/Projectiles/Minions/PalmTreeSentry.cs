using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Minions
{
    public class PalmTreeSentry : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Palm Tree");
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 82;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.timeLeft = 7200;
            Projectile.FargoSouls().NinjaCanSpeedup = false;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            FargoSoulsPlayer modPlayer = player.FargoSouls();


            if (!(player.Alive() && player.HasEffect<PalmwoodEffect>()))
            {
                Projectile.Kill();
                return;
            }//this is to work properly with sentry despawning
            bool forceEffect = modPlayer.ForceEffect<PalmWoodEnchant>();

            //BIG palm sentry!
            Projectile.scale = forceEffect ? 2 : 1;
            Projectile.height = 82 * (int)Projectile.scale;
            Projectile.width = 80 * (int)Projectile.scale;
            //Projectile.height = forcePalm ? 110 : 82; //stupid fucking idiot dumbass hatred way of making palm not clip into death

            Projectile.velocity.Y = Projectile.velocity.Y + 0.2f;
            if (Projectile.velocity.Y > 16f)
            {
                Projectile.velocity.Y = 16f;
            }

            Projectile.ai[1] += 1f;

            int attackRate = forceEffect ? 30 : 45;

            if (Projectile.ai[1] >= attackRate)
            {
                float num = 2000f;
                int npcIndex = -1;
                for (int i = 0; i < 200; i++)
                {
                    float dist = Vector2.Distance(Projectile.Center, Main.npc[i].Center);

                    if (dist < num && dist < 300 && Main.npc[i].CanBeChasedBy(Projectile, false))
                    {
                        npcIndex = i;
                        num = dist;
                    }
                }

                if (npcIndex != -1)
                {
                    NPC target = Main.npc[npcIndex];

                    if (Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, target.position, target.width, target.height))
                    {
                        Vector2 velocity = Vector2.Normalize(target.Center - Projectile.Center) * 10;

                        int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ProjectileID.SeedlerNut, Projectile.damage, 2, Projectile.owner);
                        if (p != Main.maxProjectiles)
                            Main.projectile[p].DamageType = DamageClass.Summon;
                    }
                }
                Projectile.ai[1] = 0f;

                //kill if too far away
                if (Vector2.Distance(Main.player[Projectile.owner].Center, Projectile.Center) > 2000)
                {
                    Projectile.Kill();
                }
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true;
        }

        public override bool PreDraw(ref Microsoft.Xna.Framework.Color lightColor)
        {
            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value,
                Projectile.Center - Main.screenPosition,
                null,
                lightColor,
                Projectile.rotation,
                TextureAssets.Projectile[Type].Size() / 2, // this is the only reason why
                Projectile.scale,
                SpriteEffects.None);
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.position += Projectile.velocity;
            Projectile.velocity = Vector2.Zero;
            return false;
        }
    }
}
