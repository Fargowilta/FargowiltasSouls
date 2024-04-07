using FargowiltasSouls.Content.Buffs.Boss;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.MutantBoss
{
    public class MutantSansHead : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Assets/ExtraTextures/Resprites/NPC_246B";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.width = 70;
            Projectile.height = 70;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            CooldownSlot = 1;

            Projectile.timeLeft = 420;
            Projectile.hide = true;

            Projectile.FargoSouls().DeletionImmuneRank = 1;
        }

        public override bool CanHitPlayer(Player target)
        {
            return target.hurtCooldowns[1] == 0;
        }

        public override bool? CanDamage() => Projectile.ai[0] < 0;

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;
                Projectile.rotation = Projectile.ai[2] == 1 ? 0 : MathHelper.Pi;
            }

            if (--Projectile.ai[0] == 0)
            {
                Projectile.velocity = Vector2.Zero;
                Projectile.netUpdate = true;

                if (FargoSoulsUtil.HostCheck)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center,
                        Vector2.UnitY * Projectile.ai[2],
                        ModContent.ProjectileType<MutantSansBeam>(),
                        Projectile.damage, Projectile.knockBack, Projectile.owner,
                        0f, Projectile.identity);
                }
            }
            else if (Projectile.ai[0] < -50 - 120)
            {
                Projectile.velocity.X *= 1.025f;
            }
            else if (Projectile.ai[0] < -50)
            {
                Projectile.velocity.X = Projectile.ai[1];
                Projectile.velocity.Y = 0;
            }

            Projectile.frame = 1;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (WorldSavingSystem.EternityMode)
            {
                target.FargoSouls().MaxLifeReduction += 100;
                target.AddBuff(ModContent.BuffType<OceanicMaulBuff>(), 5400);
                target.AddBuff(ModContent.BuffType<MutantFangBuff>(), 180);
            }
            target.AddBuff(ModContent.BuffType<DefenselessBuff>(), 300);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = TextureAssets.Projectile[Projectile.type].Value;
            int num156 = TextureAssets.Projectile[Projectile.type].Value.Height / 6; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            SpriteEffects effects = SpriteEffects.None;

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, Projectile.rotation, origin2, Projectile.scale, effects, 0);

            //idk why these arent working boohoo
            Texture2D eyes = TextureAssets.Golem[1].Value;
            Rectangle eyeRectangle = new(0, eyes.Height / 2, eyes.Width, eyes.Height / 2);
            Vector2 eyeOrigin = eyeRectangle.Size() / 2f;
            eyeOrigin.Y -= 4;
            Main.EntitySpriteDraw(eyes, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(eyeRectangle), Color.White * Projectile.Opacity, Projectile.rotation, eyeOrigin, Projectile.scale, effects, 0);
            return false;
        }
    }
}

