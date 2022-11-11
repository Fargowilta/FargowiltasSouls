using FargowiltasSouls.Buffs.Boss;
using FargowiltasSouls.Buffs.Masomode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.MutantBoss
{
    public class MutantMark2 : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_226";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystal Leaf");
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 900;
            Projectile.aiStyle = -1;
            CooldownSlot = 1;

            Projectile.hide = true;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().DeletionImmuneRank = 1;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            /*if (Projectile.localAI[0] == 0) //spawn surrounding crystals
            {
                Projectile.localAI[0] = 1;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    const int max = 5;
                    const float distance = 125f;
                    float rotation = 2f * (float)Math.PI / max;
                    for (int i = 0; i < max; i++)
                    {
                        Vector2 spawnPos = Projectile.Center + new Vector2(distance, 0f).RotatedBy(rotation * i);
                        Projectile.NewProjectile(Projectile.InheritSource(Projectile), spawnPos, Vector2.Zero, ModContent.ProjectileType<MutantCrystalLeaf>(), Projectile.damage, 0f, Projectile.owner, Projectile.whoAmI, rotation * i);
                        //int n = NPC.NewNPC((int)spawnPos.X, (int)spawnPos.Y, ModContent.NPCType<CrystalLeaf>(), 0, npc.whoAmI, distance, 300, rotation * i);
                    }
                }
            }*/

            if (--Projectile.ai[0] == 0)
            {
                Projectile.netUpdate = true;
                Projectile.velocity = Vector2.Zero;
            }
            if (--Projectile.ai[1] == 0)
            {
                Projectile.netUpdate = true;
                Player target = Main.player[Player.FindClosest(Projectile.position, Projectile.width, Projectile.height)];
                Projectile.velocity = Projectile.DirectionTo(target.Center) * 15;
                SoundEngine.PlaySound(SoundID.Item84, Projectile.Center);
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Poisoned, Main.rand.Next(60, 300));
            if (FargoSoulsWorld.EternityMode)
            {
                target.AddBuff(ModContent.BuffType<Infested>(), Main.rand.Next(60, 300));
                target.AddBuff(ModContent.BuffType<IvyVenom>(), Main.rand.Next(60, 300));
                target.AddBuff(ModContent.BuffType<MutantFang>(), 180);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}