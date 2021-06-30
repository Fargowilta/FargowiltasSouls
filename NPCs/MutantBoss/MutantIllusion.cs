using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.NPCs.MutantBoss
{
    [AutoloadBossHead]
    public class MutantIllusion : ModNPC
    {
        public override string Texture => "FargowiltasSouls/NPCs/MutantBoss/MutantBoss";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mutant");
            DisplayName.AddTranslation(GameCulture.Chinese, "突变体");
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.width = 34;
            npc.height = 50;
            npc.damage = 360;
            npc.defense = 400;
            npc.lifeMax = 7000000;
            npc.dontTakeDamage = true;
            npc.HitSound = SoundID.NPCHit57;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.knockBackResist = 0f;
            npc.lavaImmune = true;
            npc.aiStyle = -1;
            npc.buffImmune[mod.BuffType("Sadism")] = true;
            npc.buffImmune[mod.BuffType("ClippedWings")] = true;
            npc.buffImmune[mod.BuffType("MutantNibble")] = true;
            npc.buffImmune[mod.BuffType("OceanicMaul")] = true;
            npc.buffImmune[mod.BuffType("TimeFrozen")] = true;
            npc.GetGlobalNPC<FargoSoulsGlobalNPC>().SpecialEnchantImmune = true;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.damage = (int)(npc.damage * 0.5f);
            npc.lifeMax = (int)(npc.lifeMax * 0.5f * bossLifeScale);
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

        public override void AI()
        {
            if (npc.ai[0] < 0f || npc.ai[0] >= 200f)
            {
                npc.StrikeNPCNoInteraction(9999, 0f, 0);
                npc.active = false;
                return;
            }
            NPC mutant = Main.npc[(int)npc.ai[0]];
            if (!mutant.active || mutant.type != mod.NPCType("MutantBoss") || mutant.ai[0] > 19 || mutant.life <= 1)
            {
                npc.StrikeNPCNoInteraction(9999, 0f, 0);
                npc.active = false;
                for (int i = 0; i < 40; i++)
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, 5);
                    Main.dust[d].velocity *= 2.5f;
                    Main.dust[d].scale += 0.5f;
                }
                for (int i = 0; i < 20; i++)
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, 229, 0f, 0f, 0, default(Color), 2f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].noLight = true;
                    Main.dust[d].velocity *= 9f;
                }
                return;
            }

            npc.target = mutant.target;
            npc.damage = mutant.damage;
            npc.defDamage = mutant.damage;

            npc.frame.Y = mutant.frame.Y;

            if (npc.HasValidTarget)
            {
                Vector2 target = Main.player[mutant.target].Center;
                Vector2 distance = target - mutant.Center;
                npc.Center = target;
                npc.position.X += distance.X * npc.ai[1];
                npc.position.Y += distance.Y * npc.ai[2];
                npc.direction = npc.spriteDirection = npc.position.X < Main.player[npc.target].position.X ? 1 : -1;
            }
            else
            {
                npc.Center = mutant.Center;
            }

            /*Vector2 target = new Vector2(mutant.localAI[1], mutant.localAI[2]);
            Vector2 distance = target - mutant.Center;
            npc.Center = target;
            npc.position.X += distance.X * npc.ai[1];
            npc.position.Y += distance.Y * npc.ai[2];*/

            if (--npc.ai[3] == 0)
            {
                int ai0;
                if (npc.ai[1] < 0)
                    ai0 = 0;
                else if (npc.ai[2] < 0)
                    ai0 = 1;
                else
                    ai0 = 2;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(npc.Center, Vector2.UnitY * -5, mod.ProjectileType("MutantPillar"), mutant.damage / 3, 0, Main.myPlayer, ai0, npc.whoAmI);
            }
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool PreNPCLoot()
        {
            return false;
        }

        public override void FindFrame(int frameHeight)
        {
            /*if (++npc.frameCounter > 6)
            {
                npc.frameCounter = 0;
                npc.frame.Y += frameHeight;
                if (npc.frame.Y >= 4 * frameHeight)
                    npc.frame.Y = 0;
            }*/
        }

        public override void BossHeadSpriteEffects(ref SpriteEffects spriteEffects)
        {
            //spriteEffects = npc.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        }
    }
}