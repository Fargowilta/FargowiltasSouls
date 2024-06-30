using FargowiltasSouls.Content.Buffs;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Minions
{
    public class GladiatorSpirit : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 60;
            Projectile.height = 100;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.timeLeft = 60 * 60;

            Projectile.FargoSouls().DeletionImmuneRank = 2;
        }
        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            FargoSoulsPlayer localModPlayer = Main.LocalPlayer.FargoSouls();

            if (player.whoAmI == Main.myPlayer && (player.dead || !player.HasEffect<WillGladiatorEffect>()))
            {
                Projectile.Kill();
                return;
            }
            Projectile.timeLeft = 60;


            // movement
            int deadzone = 500;
            float distance = Projectile.Distance(player.Center);
            if (distance > deadzone)
            {
                float distanceFactor = (distance - deadzone);
                distanceFactor /= 6000;
                //if (distanceFactor > 1)
                    //distanceFactor = 1;
                float speed = 60 * distanceFactor;
                if (speed < 7)
                    speed = 7;
                Vector2 desiredVel = Projectile.DirectionTo(player.Center) * speed;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVel, 0.05f);
            }
            else
            {
                Projectile.velocity *= 0.97f;
            }

            int AuraSize = 600;

            FargoSoulsUtil.AuraDust(Projectile, AuraSize, DustID.GoldCoin);
            if (FargoSoulsUtil.ClosestPointInHitbox(Main.LocalPlayer.Hitbox, Projectile.Center).Distance(Projectile.Center) < AuraSize && player.HasEffect<WillGladiatorEffect>() && !localModPlayer.Purified)
            {
                Main.LocalPlayer.AddBuff(ModContent.BuffType<GladiatorSpiritBuff>(), 2);
            }

            if (++Projectile.frameCounter > 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }
        }
    }
}
