using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.ID;
using FargowiltasSouls.NPCs.Challengers;

namespace FargowiltasSouls.Projectiles.Challengers
{

    public class LifeBlaster : ModProjectile
    {
        private bool Init = false;

        private int damage;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Life Blaster");
        }
        public override void SetDefaults()
        {
            Projectile.width = 96;
            Projectile.height = 52;
            Projectile.aiStyle = 0;
            Projectile.hostile = true;
            AIType = 14;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.light = 0.5f;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            if (!Init)
            {
                Projectile.rotation = Projectile.velocity.RotatedBy(-Math.PI / 2).ToRotation();
                Projectile.velocity = Vector2.Zero;
                damage = Projectile.damage;
                Projectile.damage = 0;
                Init = true;
                Projectile.netUpdate = true;
            }
            Projectile.ai[0]++;
            Projectile.alpha -= 15;
            if ((Projectile.ai[0] >= 60 && Projectile.ai[1] <= 1) || (Projectile.ai[0] >= 120 && Projectile.ai[1] == 2))
            {
                //Projectile.frame = 1;
                if ((Projectile.ai[0] == 60 && Projectile.ai[1] <= 1) || (Projectile.ai[0] == 120 && Projectile.ai[1] == 2))
                {
                    SoundEngine.PlaySound(SoundID.Item12, Projectile.Center);
                    SoundEngine.PlaySound(SoundID.Item66, Projectile.Center);
                    //fire beam
                    Vector2 rot = Vector2.Normalize(Projectile.rotation.ToRotationVector2()).RotatedBy(Math.PI / 2);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        //visual    center - height/2 + 32 + beamheight/2
                        //Vector2 visualpos = Projectile.Center + (rot * (1005));
                        //Projectile.NewProjectile(Projectile.GetSource_FromThis(), visualpos, rot * 0.000000001f, ModContent.ProjectileType<LifeBlasterLaser>(), 0, 0, Main.myPlayer);

                        //real deathray lol
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, rot,
                                    ModContent.ProjectileType<LifeChalBlasterDeathray>(), damage, 0f, Main.myPlayer, 0, Projectile.whoAmI);


                    }
                    Projectile.netUpdate = true;
                }
                //damage projectiles
                /*if ((Projectile.ai[0] >= 60 && Projectile.ai[0] % 11 == 10 && Projectile.ai[1] <= 1) || (Projectile.ai[0] >= 120 && Projectile.ai[0] % 11 == 10 && Projectile.ai[1] == 2))
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 rot = Vector2.Normalize(Projectile.rotation.ToRotationVector2()).RotatedBy(Math.PI / 2);
                        for (int i = 0; i < 2; i++)
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + (rot * (i * 450)), rot * 35f, ModContent.ProjectileType<LifeBlasterLaserHitbox>(), damage, 3f, Main.myPlayer, Projectile.ai[0]);
                    }
                }*/
            }
            //else
                //Projectile.frame = 0;
            if ((Projectile.ai[0] == 95 && Projectile.ai[1] <= 1) || ((Projectile.ai[0] == 155 && Projectile.ai[1] == 2)))
                Projectile.Kill();
        }
        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 255 - Projectile.alpha) * Projectile.Opacity;
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
                target.AddBuff(ModContent.BuffType<Buffs.Masomode.Smite>(), 600);
        }
    }
}
