using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.NPCs.EternityMode
{
    public class BabyGuardian : ModNPC
    {
        public override string Texture => "Terraria/Projectile_197";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Baby Guardian");
            //DisplayName.AddTranslation(GameCulture.Chinese, "守卫者宝宝");
        }

        public override void SetDefaults()
        {
            npc.width = 42;
            npc.height = 42;
            npc.damage = 100;
            npc.defense = 9999;
            npc.lifeMax = 999;
            npc.HitSound = SoundID.NPCHit2;
            npc.DeathSound = SoundID.NPCDeath2;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.knockBackResist = 0f;
            npc.lavaImmune = true;
            for (int i = 0; i < npc.buffImmune.Length; i++)
                npc.buffImmune[i] = true;
            npc.aiStyle = -1;
            npc.value = 50000f;
        }

        public override void AI()
        {
            if (npc.buffType[0] != 0)
                npc.DelBuff(0);

            npc.damage = npc.defDamage;
            npc.defense = npc.defDefense;

            if (npc.life < (int)(npc.lifeMax * .9)) //become aggressive
            {
                npc.ai[0] = -1f;
                npc.netUpdate = true;
            }

            if (npc.ai[0] >= 0f) //just spawned, fading out
            {
                if (npc.justHit)
                {
                    npc.ai[0] = 0f;
                    npc.netUpdate = true;
                }
                if (++npc.ai[0] > 255f)
                {
                    npc.StrikeNPCNoInteraction(9999, 0f, 0);
                    for (int i = 0; i < 50; i++)
                    {
                        int d = Dust.NewDust(npc.position, npc.width, npc.height, 20);
                        Main.dust[d].scale += 1f;
                        Main.dust[d].noGravity = true;
                        Main.dust[d].velocity *= 5f;
                    }
                    return;
                }
                npc.alpha = (int)npc.ai[0];
            }
            else //in aggressive mode
            {
                if (npc.localAI[1] == 0f) //roar again
                {
                    npc.localAI[1] = 1f;
                    Main.PlaySound(SoundID.Roar, npc.Center, 0);
                }
                npc.TargetClosest(true); //chase nearest player, etc
                npc.alpha = 0;
                npc.rotation += npc.direction * .3f;
                if (npc.HasValidTarget)
                {
                    npc.velocity = npc.DirectionTo(Main.player[npc.target].Center) * 6f;

                    if (++npc.localAI[2] > 180) //shoot skulls when below half health
                    {
                        npc.localAI[2] = 0f;
                        if (npc.life < npc.lifeMax / 2 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 speed = Main.player[npc.target].Center - npc.Center;
                            speed.X += Main.rand.Next(-20, 21);
                            speed.Y += Main.rand.Next(-20, 21);
                            speed.Normalize();
                            speed *= 3f;
                            speed += npc.velocity;
                            Projectile.NewProjectile(npc.Center, speed, ProjectileID.Skull, npc.damage / 4, 0, Main.myPlayer, -1f, 0f);
                        }
                    }
                }
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(mod.BuffType("MarkedforDeath"), 240);
        }

        public override bool PreNPCLoot()
        {
            return npc.ai[0] < 0;
        }

        public override void NPCLoot()
        {
            Item.NewItem(npc.position, npc.Size, mod.ItemType("SinisterIcon"));
        }
    }
}