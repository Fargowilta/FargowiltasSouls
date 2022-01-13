using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.NPCs.EternityMode
{
    [AutoloadBossHead]
    public class BrainIllusion : ModNPC
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Brain of Cthulhu");
            //DisplayName.AddTranslation(GameCulture.Chinese, "克苏鲁之脑");
        }

        public override void SetDefaults()
        {
            npc.width = 160;
            npc.height = 110;
            npc.damage = 0;
            npc.defense = 9999;
            npc.lifeMax = 9999;
            npc.dontTakeDamage = true;
            npc.hide = true;
            npc.HitSound = SoundID.NPCHit9;
            npc.DeathSound = SoundID.NPCDeath11;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.knockBackResist = 0f;
            npc.lavaImmune = true;
            npc.aiStyle = -1;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.damage = 0;
            npc.lifeMax = 9999;
        }

        public override void AI()
        {
            NPC brain = FargoSoulsUtil.NPCExists(npc.ai[0], NPCID.BrainofCthulhu);
            if (brain == null)
            {
                npc.life = 0;
                npc.HitEffect();
                npc.StrikeNPCNoInteraction(9999, 0f, 0);
                npc.active = false;
                return;
            }

            npc.target = brain.target;
            if (npc.HasPlayerTarget)
            {
                Vector2 distance = Main.player[npc.target].Center - brain.Center;
                npc.Center = Main.player[npc.target].Center;
                npc.position.X += distance.X * npc.ai[1];
                npc.position.Y += distance.Y * npc.ai[2];
            }
            else
            {
                npc.Center = brain.Center;
            }

            if (Fargowiltas.Instance.MasomodeEXLoaded)
            {
                npc.damage = brain.damage;
                npc.defDamage = brain.defDamage;
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
            if (npc.life <= 0)
            {
                //Main.PlaySound(npc.DeathSound, npc.Center);
                for (int i = 0; i < 40; i++)
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, 5);
                    Main.dust[d].velocity *= 2.5f;
                    Main.dust[d].scale += 0.5f;
                }
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
    }
}