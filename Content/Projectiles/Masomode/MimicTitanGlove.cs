using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class MimicTitanGlove : ModProjectile
    {
        public override string Texture => "Terraria/Images/Item_536";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Titan Glove");
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 28;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            CooldownSlot = 1;
        }

        public override void AI()
        {
            NPC npc = Main.npc[(int)Projectile.ai[1]];
            Player player = Main.player[npc.target];
            Projectile.position = npc.Center - new Vector2(Projectile.width / 2, Projectile.height / 2);
            if (Projectile.ai[0] < 60)
                Projectile.rotation = (player.Center - Projectile.Center).ToRotation() + (float)Math.PI / 2;
            if (Projectile.ai[0] > 120)
                Projectile.Kill();
            Projectile.ai[0]++;
        }
    }
}
