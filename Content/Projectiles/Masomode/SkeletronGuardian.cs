using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class SkeletronGuardian : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_197";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Baby Guardian");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 42;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            //CooldownSlot = 1;

            Projectile.timeLeft = 360;
            Projectile.hide = true;

            Projectile.light = 0.5f;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;
                Projectile.rotation = Main.rand.NextFloat(0, 2 * (float)Math.PI);
                Projectile.hide = false;

                SoundEngine.PlaySound(SoundID.Item21, Projectile.Center);

                for (int i = 0; i < 50; i++)
                {
                    Vector2 pos = new(Projectile.Center.X + Main.rand.Next(-20, 20), Projectile.Center.Y + Main.rand.Next(-20, 20));
                    int dust = Dust.NewDust(pos, Projectile.width, Projectile.height, DustID.Blood, 0, 0, 100, default, 2f);
                    Main.dust[dust].noGravity = true;
                }
            }

            if (Projectile.ai[0] == 0)
            {
                Projectile.velocity -= new Vector2(Projectile.ai[1], 0).RotatedBy(Projectile.velocity.ToRotation());

                if (Projectile.velocity.Length() < 1)
                {
                    int p = Player.FindClosest(Projectile.Center, 0, 0);
                    if (p != -1)
                    {
                        Projectile.velocity = Projectile.SafeDirectionTo(Main.player[p].Center);
                        Projectile.ai[0] = 1f;
                        Projectile.ai[1] = p; //now used for tracking player
                        Projectile.netUpdate = true;

                        SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);
                    }
                    else
                    {
                        Projectile.Kill();
                    }
                }
            }
            else //weak homing
            {
                if (++Projectile.localAI[0] < 45)
                    Projectile.velocity *= 1.08f;

                if (Projectile.localAI[0] < 65)
                {
                    float rotation = Projectile.velocity.ToRotation();
                    Vector2 vel = Main.player[(int)Projectile.ai[1]].Center - Projectile.Center;
                    float targetAngle = vel.ToRotation();
                    Projectile.velocity = new Vector2(Projectile.velocity.Length(), 0f).RotatedBy(rotation.AngleLerp(targetAngle, 0.065f));
                }
            }

            Projectile.direction = Projectile.velocity.X < 0 ? -1 : 1;
            Projectile.rotation += Projectile.direction * .3f;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 50; i++)
            {
                Vector2 pos = new(Projectile.Center.X + Main.rand.Next(-20, 20), Projectile.Center.Y + Main.rand.Next(-20, 20));
                int dust = Dust.NewDust(pos, Projectile.width, Projectile.height, DustID.Blood, 0, 0, 100, default, 2f);
                Main.dust[dust].noGravity = true;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<DefenselessBuff>(), 300);
            target.AddBuff(ModContent.BuffType<LethargicBuff>(), 300);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            NPC sourceNPC = Projectile.GetSourceNPC();
            bool recolor =
                (sourceNPC != null && (sourceNPC.type == NPCID.SkeletronHead || sourceNPC.type == NPCID.SkeletronHand || sourceNPC.type == NPCID.DungeonGuardian)) &&
                SoulConfig.Instance.BossRecolors && WorldSavingSystem.EternityMode;

            Texture2D texture2D13 = recolor ? ModContent.Request<Texture2D>("FargowiltasSouls/Content/Bosses/DeviBoss/DeviGuardian_Recolor").Value : TextureAssets.Projectile[Type].Value;
            FargoSoulsUtil.ProjectileWithTrailDraw(Projectile, new Color(255, 200, 255, 0) * Projectile.Opacity, texture2D13);
            FargoSoulsUtil.GenericProjectileDraw(Projectile, lightColor, texture2D13);
            return false;
        }
    }
}