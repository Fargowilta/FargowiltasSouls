using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.ID;
using FargowiltasSouls.NPCs.Challengers;

namespace FargowiltasSouls.Projectiles.Challengers
{
    public class LifeCageProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Life Cage");
        }
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.aiStyle = 0;
            Projectile.hostile = false;
            AIType = 14;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.light = 0.5f;
        }

        public override void AI()
        {
            if (Main.LocalPlayer.active && Projectile.Colliding(Projectile.Hitbox, Main.LocalPlayer.Hitbox))
            {
                switch (Projectile.ai[0])
                { 
                case 0: //left
                    Main.LocalPlayer.velocity.X = (Main.LocalPlayer.Center.X - Projectile.Center.X)*0.2f; 
                    break;
                case 1: //right
                    Main.LocalPlayer.velocity.X = (Main.LocalPlayer.Center.X - Projectile.Center.X)*0.2f; 
                    break;
                case 2: //up
                    Main.LocalPlayer.velocity.Y = (Main.LocalPlayer.Center.Y - Projectile.Center.Y)*0.2f; 
                    break;
                case 3: //down
                    Main.LocalPlayer.velocity.Y = (Main.LocalPlayer.Center.Y - Projectile.Center.Y)*0.2f; 
                    Main.LocalPlayer.wingTime = Main.LocalPlayer.wingTimeMax; 
                    break; 
                }
                SoundEngine.PlaySound(SoundID.Item56, Projectile.Center);
            }
            if (NPC.CountNPCS(ModContent.NPCType<LifeChallenger>()) < 1)
            {
                    Projectile.Kill();
            }
        }
    }
}