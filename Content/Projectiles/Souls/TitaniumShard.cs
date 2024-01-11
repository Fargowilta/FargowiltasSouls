using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;

namespace FargowiltasSouls.Content.Projectiles.Souls
{
	public class TitaniumShard : ModProjectile
    {
        //private readonly int shrinkTimer = 0;
        public override string Texture => "Terraria/Images/Projectile_908";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Rain Cloud");
            Main.projFrames[Projectile.type] = 12;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.TitaniumStormShard);
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            if (Projectile.owner == Main.myPlayer && Main.LocalPlayer.HasEffect<TitaniumEffect>())
            {
                Projectile.Kill();
            }
            //else
            //{
            //    Projectile.timeLeft = 2;
            //}


            //if (!player.active || player.dead || !player.hasTitaniumStormBuff)
            //{
            //    this.Kill();
            //    return;
            //}
            if (Projectile.frameCounter == 0)
            {
                Projectile.frameCounter = 1;
                Projectile.frame = Main.rand.Next(12);
                Projectile.rotation = Main.rand.NextFloat() * 6.28318548f;
            }
            Projectile.rotation += 0.0157079641f;
            AI_GetMyGroupIndexAndFillBlackList(null, out int num, out int num2);
            float f = (num / (float)num2 + player.miscCounterNormalized * 6f) * 6.28318548f;
            float scaleFactor = 24f + num2 * 6f;
            Vector2 value = player.position - player.oldPosition;
            Projectile.Center += value;
            Vector2 vector = f.ToRotationVector2();
            Projectile.localAI[0] = vector.Y;
            Vector2 value2 = player.Center + vector * new Vector2(1f, 0.05f) * scaleFactor;
            Projectile.Center = Vector2.Lerp(Projectile.Center, value2, 0.3f);


        }

        private void AI_GetMyGroupIndexAndFillBlackList(List<int> blackListedTargets, out int index, out int totalIndexesInGroup)
        {
            index = 0;
            totalIndexesInGroup = 0;
            for (int i = 0; i < 1000; i++)
            {
                Projectile projectile = Main.projectile[i];
                if (projectile.active && projectile.owner == Projectile.owner && projectile.type == Projectile.type && (projectile.type != 759 || projectile.frame == Main.projFrames[projectile.type] - 1))
                {
                    if (Projectile.whoAmI > i)
                    {
                        index++;
                    }
                    totalIndexesInGroup++;
                }
            }
        }
    }
}