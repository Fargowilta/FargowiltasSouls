using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.MutantBoss
{
    public class MutantMark2 : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_226";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Crystal Leaf");
        }

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.hostile = true;
            projectile.timeLeft = 900;
            projectile.aiStyle = -1;
            cooldownSlot = 1;

            projectile.hide = true;
        }

        public override bool CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            /*if (projectile.localAI[0] == 0) //spawn surrounding crystals
            {
                projectile.localAI[0] = 1;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    const int max = 5;
                    const float distance = 125f;
                    float rotation = 2f * (float)Math.PI / max;
                    for (int i = 0; i < max; i++)
                    {
                        Vector2 spawnPos = projectile.Center + new Vector2(distance, 0f).RotatedBy(rotation * i);
                        Projectile.NewProjectile(spawnPos, Vector2.Zero, mod.ProjectileType("MutantCrystalLeaf"), projectile.damage, 0f, projectile.owner, projectile.whoAmI, rotation * i);
                        //int n = NPC.NewNPC((int)spawnPos.X, (int)spawnPos.Y, mod.NPCType("CrystalLeaf"), 0, npc.whoAmI, distance, 300, rotation * i);
                    }
                }
            }*/

            if (--projectile.ai[0] == 0)
            {
                projectile.netUpdate = true;
                projectile.velocity = Vector2.Zero;
            }
            if (--projectile.ai[1] == 0)
            {
                projectile.netUpdate = true;
                Player target = Main.player[Player.FindClosest(projectile.position, projectile.width, projectile.height)];
                projectile.velocity = projectile.DirectionTo(target.Center) * 15;
                Main.PlaySound(SoundID.Item84, projectile.Center);
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Poisoned, Main.rand.Next(60, 300));
            if (FargoSoulsWorld.EternityMode)
            {
                target.AddBuff(mod.BuffType("Infested"), Main.rand.Next(60, 300));
                target.AddBuff(mod.BuffType("IvyVenom"), Main.rand.Next(60, 300));
                target.AddBuff(mod.BuffType("MutantFang"), 180);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}