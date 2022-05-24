using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.NPCs.EternityMode
{
    [AutoloadBossHead]
    public class BrainIllusion : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brain of Cthulhu");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "克苏鲁之脑");

            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Hide = true
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 160;
            NPC.height = 110;
            NPC.damage = 0;
            NPC.defense = 9999;
            NPC.lifeMax = 9999;
            NPC.dontTakeDamage = true;
            NPC.hide = true;
            NPC.HitSound = SoundID.NPCHit9;
            NPC.DeathSound = SoundID.NPCDeath11;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0f;
            NPC.lavaImmune = true;
            NPC.aiStyle = -1;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.damage = 0;
            NPC.lifeMax = 9999;
        }

        public override void AI()
        {
            NPC brain = FargoSoulsUtil.NPCExists(NPC.ai[0], NPCID.BrainofCthulhu);
            if (brain == null)
            {
                NPC.life = 0;
                NPC.HitEffect();
                NPC.StrikeNPCNoInteraction(9999, 0f, 0);
                NPC.active = false;
                return;
            }

            NPC.target = brain.target;
            if (NPC.HasPlayerTarget)
            {
                Vector2 distance = Main.player[NPC.target].Center - brain.Center;
                NPC.Center = Main.player[NPC.target].Center;
                NPC.position.X += distance.X * NPC.ai[1];
                NPC.position.Y += distance.Y * NPC.ai[2];
            }
            else
            {
                NPC.Center = brain.Center;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Poisoned, 120);
            target.AddBuff(BuffID.Darkness, 120);
            target.AddBuff(BuffID.Bleeding, 120);
            target.AddBuff(BuffID.Slow, 120);
            target.AddBuff(BuffID.Weak, 120);
            target.AddBuff(BuffID.BrokenArmor, 120);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                //SoundEngine.PlaySound(NPC.DeathSound, NPC.Center);
                for (int i = 0; i < 40; i++)
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 5);
                    Main.dust[d].velocity *= 2.5f;
                    Main.dust[d].scale += 0.5f;
                }
            }
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool CheckDead()
        {
            return false;
        }
    }
}