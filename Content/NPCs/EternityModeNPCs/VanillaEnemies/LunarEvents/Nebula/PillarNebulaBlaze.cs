using FargowiltasSouls.Content.Bosses.Champions.Cosmos;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.LunarEvents.Nebula
{
    public class PillarNebulaBlaze : CosmosNebulaBlaze
    {
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            //no debuffs
        }
        public override void AI() //vanilla code echprimebegone
        {
            int pillar = (int)Projectile.ai[2];
            if (++Projectile.localAI[1] < 45 * 3
                && FargoSoulsUtil.BossIsAlive(ref pillar, NPCID.LunarTowerNebula)
                && Main.npc[pillar].HasValidTarget) //home
            {
                float rotation = Projectile.velocity.ToRotation();
                Vector2 vel = Main.player[Main.npc[pillar].target].Center - Projectile.Center;
                float targetAngle = vel.ToRotation();
                Projectile.velocity = new Vector2(Projectile.velocity.Length(), 0f).RotatedBy(rotation.AngleLerp(targetAngle, Projectile.ai[0]));
            }

            float num1 = 5f;
            //float num2 = 250f;
            //float num3 = 6f;
            Vector2 vector2_1 = new(8f, 10f);
            float num4 = 1.2f;
            Vector3 rgb = new(0.7f, 0.1f, 0.5f);
            int num5 = 4 * Projectile.MaxUpdates;
            int Type1 = Utils.SelectRandom(Main.rand, new int[5] { 242, 73, 72, 71, byte.MaxValue });
            int Type2 = byte.MaxValue;
            if (Projectile.ai[1] == 0.0)
            {
                Projectile.ai[1] = 1f;
                Projectile.localAI[0] = -Main.rand.Next(48);
                SoundEngine.PlaySound(SoundID.Item34, Projectile.position);
            }
            /*else if ((double)Projectile.ai[1] == 1.0 && Projectile.owner == Main.myPlayer)
            {
                
            }
            else if ((double)Projectile.ai[1] > (double)num1)
            {
                
            }*/
            if (Projectile.ai[1] >= 1.0 && Projectile.ai[1] < (double)num1)
            {
                ++Projectile.ai[1];
                if (Projectile.ai[1] == (double)num1)
                    Projectile.ai[1] = 1f;
            }
            Projectile.alpha = Projectile.alpha - 40;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.frameCounter = Projectile.frameCounter + 1;
            if (Projectile.frameCounter >= num5)
            {
                Projectile.frame = Projectile.frame + 1;
                Projectile.frameCounter = 0;
                if (Projectile.frame >= 4)
                    Projectile.frame = 0;
            }
            //Lighting.AddLight(Projectile.Center, rgb);
            Projectile.rotation = Projectile.velocity.ToRotation();
            ++Projectile.localAI[0];
            if (Projectile.localAI[0] == 48.0)
                Projectile.localAI[0] = 0.0f;
            else if (Projectile.alpha == 0)
            {
                if (Main.rand.NextBool(3))
                {
                    Vector2 vector2_2 = Vector2.UnitX * -30f;
                    Vector2 vector2_3 = -Vector2.UnitY.RotatedBy(Projectile.localAI[0] * 0.130899697542191 + 3.14159274101257, new Vector2()) * vector2_1 - Projectile.rotation.ToRotationVector2() * 10f;
                    int index2 = Dust.NewDust(Projectile.Center, 0, 0, Type2, 0.0f, 0.0f, 160, new Color(), 1f);
                    Main.dust[index2].scale = num4;
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].position = Projectile.Center + vector2_3 + Projectile.velocity * 2f;
                    Main.dust[index2].velocity = Vector2.Normalize(Projectile.Center + Projectile.velocity * 2f * 8f - Main.dust[index2].position) * 2f + Projectile.velocity * 2f;
                    Main.dust[index2].velocity *= Projectile.MaxUpdates / 3;
                }
            }
            if (Main.rand.NextBool(12))
            {
                Vector2 vector2_2 = -Vector2.UnitX.RotatedByRandom(0.785398185253143).RotatedBy((double)Projectile.velocity.ToRotation(), new Vector2());
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Type1, 0.0f, 0.0f, 0, new Color(), 1.2f);
                Main.dust[index2].velocity *= 0.3f;
                Main.dust[index2].velocity *= Projectile.MaxUpdates / 3;
                Main.dust[index2].noGravity = true;
                Main.dust[index2].position = Projectile.Center + vector2_2 * Projectile.width / 2f;
                if (Main.rand.NextBool())
                    Main.dust[index2].fadeIn = 1.4f;
            }
        }
    }
}