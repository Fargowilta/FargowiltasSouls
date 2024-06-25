using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs
{
    public class SlimeSwarm : ModNPC
    {
        public override string Texture => "Terraria/Images/NPC_1";

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = Main.npcFrameCount[NPCID.BlueSlime];
            //NPCID.Sets.TrailCacheLength[NPC.type] = 6;
            //NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            NPCID.Sets.SpecificDebuffImmunity[Type] = NPCID.Sets.SpecificDebuffImmunity[NPCID.KingSlime];

            this.ExcludeFromBestiary();
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.BlueSlime);

            NPC.aiStyle = -1;
            NPC.knockBackResist = 0;
            //NPC.timeLeft = NPC.activeTime * 30;
            NPC.timeLeft = 60 * 10;
            NPC.noTileCollide = false;

            NPC.noGravity = false;

            //NPC.scale *= 1.5f;
            NPC.lifeMax *= 3;
            NPC.damage = 32;

            NPC.GravityMultiplier *= 2;
        }

        //public override bool CanHitNPC(NPC target) => false;

        //public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;

        public override void AI()
        {
            ref float direction = ref NPC.ai[0];
            ref float timer = ref NPC.ai[1];

            float accelX = 0.1f;
            int maxSpeedX = 12;
            int jumpSpeed = 5;

            if (Math.Abs(NPC.velocity.X) < maxSpeedX)
            {
                NPC.velocity.X += accelX * direction;
            }
            if (NPC.velocity.Y == 0)
            {
                NPC.velocity.Y = -jumpSpeed;
            }
            bool playerFar = true;
            int p = Player.FindClosest(NPC.Center, 0, 0);
            if (p.IsWithinBounds(Main.maxPlayers) && Main.player[p].Distance(NPC.Center) < 1500)
                playerFar = false;

            NPC.damage = (timer > 60) ? NPC.defDamage : 0;

            if (++timer > 60 * 15 || playerFar)
            {
                NPC.alpha += 17;
                if (NPC.alpha >= 250)
                {
                    NPC.active = false;
                }
            }

        }


        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 4)
            {
                NPC.frame.Y += frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= Main.npcFrameCount[NPC.type] * frameHeight)
                NPC.frame.Y = 0;
        }
    }
}
