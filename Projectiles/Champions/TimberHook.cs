using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Champions
{
    public class TimberHook : Challengers.TrojanHook
    {
        public override string Texture => "Terraria/Images/Projectile_13";

        public override void SetDefaults()
        {
            base.SetDefaults();

            Projectile.extraUpdates = 2;

            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().DeletionImmuneRank = 2;
        }

        public override bool? CanDamage() => false;
        protected override bool flashingZapEffect => false;

        Vector2 oldPos;

        public override void AI()
        {
            NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[0], ModContent.NPCType<NPCs.Champions.TimberChampion>());
            if (npc == null)
            {
                Projectile.Kill();
                return;
            }

            Projectile.rotation = npc.DirectionTo(Projectile.Center).ToRotation() + MathHelper.PiOver2;

            if (--Projectile.ai[1] > 0)
            {
                if (!Projectile.tileCollide && !Collision.SolidCollision(Projectile.Center, 0, 0))
                    Projectile.tileCollide = true;

                Projectile.velocity = npc.DirectionTo(Main.player[npc.target].Center) * Projectile.velocity.Length();
            }
            else
            {
                Projectile.extraUpdates = 0;
                Projectile.tileCollide = false;
                Projectile.velocity = Vector2.Zero;

                if (Projectile.localAI[0] == 0)
                {
                    //flag to turn off y collision
                    npc.localAI[0] = Math.Sign(Projectile.Center.X - npc.Center.X);

                    Projectile.localAI[0] = 1;

                    Projectile.localAI[1] = npc.DirectionTo(Projectile.Center).ToRotation();
                }

                if (Projectile.Distance(npc.Center) > 600)
                    npc.localAI[0] = Math.Sign(Projectile.Center.X - npc.Center.X);

                if (Math.Abs(MathHelper.WrapAngle(npc.DirectionTo(Main.player[npc.target].Center).ToRotation() - npc.DirectionTo(Projectile.Center).ToRotation())) > MathHelper.PiOver2)
                {
                    Projectile.Kill();
                    return;
                }

                Vector2 tug = 42f * npc.DirectionTo(Projectile.Center);
                float lerp = Math.Min(npc.Distance(Projectile.Center) / 2400, 1f);
                lerp = lerp * 0.8f + 0.2f;
                lerp *= 0.06f;
                npc.velocity = Vector2.Lerp(npc.velocity, tug, lerp);

                if (Projectile.timeLeft > 180)
                    Projectile.timeLeft = 180;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.ai[1] = 0;
            return false;
        }
    }
}