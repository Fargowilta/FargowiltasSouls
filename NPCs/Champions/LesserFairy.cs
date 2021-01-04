using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.NPCs.Champions
{
    public class LesserFairy : ModNPC
    {
        public override string Texture => "Terraria/NPC_75";
        
        public int counter;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lesser Fairy");
            DisplayName.AddTranslation(GameCulture.Chinese, "小精灵");
            Main.npcFrameCount[npc.type] = Main.npcFrameCount[NPCID.Pixie];
        }

        public override void SetDefaults()
        {
            npc.width = 20;
            npc.height = 20;
            npc.damage = 180;
            npc.defense = 0;
            npc.lifeMax = 1;
            npc.HitSound = SoundID.NPCHit5;
            npc.DeathSound = SoundID.NPCDeath7;
            npc.value = 0f;
            npc.knockBackResist = 0f;

            animationType = NPCID.Pixie;
            npc.aiStyle = -1;

            npc.dontTakeDamage = true;
            npc.noTileCollide = true;
            npc.noGravity = true;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return true;
        }

        public override void AI()
        {
            if (Main.rand.Next(6) == 0)
            {
                int d = Dust.NewDust(npc.position, npc.width, npc.height, 87);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 0.5f;
            }

            if (Main.rand.Next(40) == 0)
            {
                Main.PlaySound(SoundID.Pixie, npc.Center, 1);
            }

            npc.direction = npc.spriteDirection = npc.velocity.X < 0 ? -1 : 1;
            npc.rotation = npc.velocity.X * 0.1f;

            if (++counter > 60 && counter < 240)
            {
                if (!npc.HasValidTarget)
                    npc.TargetClosest();

                if (npc.Distance(Main.player[npc.target].Center) < 300)
                {
                    npc.velocity = npc.DirectionTo(Main.player[npc.target].Center) * npc.velocity.Length();
                }
            }
            else if (counter > 300)
            {
                npc.StrikeNPCNoInteraction(9999, 0f, 0);
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.MasochistMode)
                target.AddBuff(ModContent.BuffType<Buffs.Masomode.Purified>(), 300);
        }

        public override Color? GetAlpha(Color drawColor)
        {
            return Color.White;
        }

        public override void FindFrame(int frameHeight)
        {
            if (++npc.frameCounter >= 4)
            {
                npc.frame.Y += frameHeight;
                npc.frameCounter = 0;
            }

            if (npc.frame.Y >= frameHeight * Main.npcFrameCount[npc.type])
            {
                npc.frame.Y = 0;
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                for (int i = 0; i < 20; i++)
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, 87, 0f, 0f, 0, default(Color), 1.5f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 4f;
                }
            }
        }
    }
}
