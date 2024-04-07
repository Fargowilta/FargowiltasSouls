using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class SkeletronGuardian2 : ModProjectile
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

                for (int i = 0; i < 50; i++)
                {
                    Vector2 pos = new(Projectile.Center.X + Main.rand.Next(-20, 20), Projectile.Center.Y + Main.rand.Next(-20, 20));
                    int dust = Dust.NewDust(pos, Projectile.width, Projectile.height, DustID.Blood, 0, 0, 100, default, 2f);
                    Main.dust[dust].noGravity = true;
                }
            }

            if (++Projectile.localAI[0] > 30 && Projectile.localAI[0] < 120)
                Projectile.velocity *= 1.04f;

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