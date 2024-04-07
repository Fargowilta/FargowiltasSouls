using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Souls
{
    public class Chlorofuck : ModProjectile
    {
        public const float Cooldown = 50f;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Chlorofuck");
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 22;
            Projectile.height = 42;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 18000;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.FargoSouls().CanSplit = false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            if (player.whoAmI == Main.myPlayer && (player.dead || !player.HasEffect<ChloroMinion>()))
            {
                Projectile.Kill();
                Projectile.netUpdate = true;
                return;
            }

            Projectile.netUpdate = true;


            float num395 = Main.mouseTextColor / 200f - 0.35f;
            num395 *= 0.2f;
            Projectile.scale = num395 + 0.95f;

            if (true)
            {
                //rotation mumbo jumbo
                float distanceFromPlayer = 75;

                Lighting.AddLight(Projectile.Center, 0.1f, 0.4f, 0.2f);

                Projectile.position = player.Center + new Vector2(distanceFromPlayer, 0f).RotatedBy(Projectile.ai[1]);
                Projectile.position.X -= Projectile.width / 2;
                Projectile.position.Y -= Projectile.height / 2;
                float rotation = 0.03f;
                Projectile.ai[1] -= rotation;
                if (Projectile.ai[1] > (float)Math.PI)
                {
                    Projectile.ai[1] -= 2f * (float)Math.PI;
                    Projectile.netUpdate = true;
                }
                Projectile.rotation = Projectile.ai[1] + (float)Math.PI / 2f;


                //wait for CD
                if (Projectile.ai[0] != 0f)
                {
                    Projectile.ai[0] -= 1f;
                    return;
                }

                //trying to shoot
                float num396 = Projectile.position.X;
                float num397 = Projectile.position.Y;
                float num398 = 700f;
                bool flag11 = false;

                NPC npc = FargoSoulsUtil.NPCExists(FargoSoulsUtil.FindClosestHostileNPCPrioritizingMinionFocus(Projectile, 700, true));
                if (npc != null)
                {
                    num396 = npc.Center.X;
                    num397 = npc.Center.Y;
                    num398 = Projectile.Distance(npc.Center);
                    flag11 = true;
                }

                //shoot
                if (flag11)
                {
                    Vector2 vector29 = new(Projectile.position.X + Projectile.width * 0.5f, Projectile.position.Y + Projectile.height * 0.5f);
                    float num404 = num396 - vector29.X;
                    float num405 = num397 - vector29.Y;
                    float num406 = (float)Math.Sqrt(num404 * num404 + num405 * num405);
                    num406 = 10f / num406;
                    num404 *= num406;
                    num405 *= num406;
                    if (Projectile.owner == Main.myPlayer)
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(num404, num405), ProjectileID.CrystalLeafShot, Projectile.damage, Projectile.knockBack, Projectile.owner);
                    Projectile.ai[0] = Cooldown;
                }
            }

            if (Main.netMode == NetmodeID.Server)
                Projectile.netUpdate = true;
        }
    }
}