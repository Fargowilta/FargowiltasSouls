using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Souls
{
    public class TitaniumShard : ModProjectile
    {
        private int shrinkTimer = 0;
        public override string Texture => "Terraria/Images/Projectile_908";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rain Cloud");
            Main.projFrames[Projectile.type] = 12;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.TitaniumStormShard);
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            if ((Projectile.owner == Main.myPlayer && (!player.GetToggleValue("Titanium") || modPlayer.TitaniumEnchantItem == null)))
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
            int num;
            int num2;
            AI_GetMyGroupIndexAndFillBlackList(null, out num, out num2);
            float f = ((float)num / (float)num2 + player.miscCounterNormalized * 6f) * 6.28318548f;
            float scaleFactor = 24f + (float)num2 * 6f;
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