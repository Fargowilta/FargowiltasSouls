using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.BloodMoon
{
    public class Clown : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Clown);

        public int FuseTimer;

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.lifeMax *= 2;
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            /*if (!masoBool[0]) //roar when spawn
            {
                masoBool[0] = true;
                SoundEngine.PlaySound(SoundID.Roar, npc.Center);
                if (Main.netMode == NetmodeID.SinglePlayer)
                    Main.NewText("A Clown has begun ticking!", 175, 75, 255);
                else if (Main.netMode == NetmodeID.Server)
                    ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("A Clown has begun ticking!"), new Color(175, 75, 255));
            }*/

            const int maxFuseTime = 300 * 5;

            float ratio = (float)FuseTimer / maxFuseTime; //caught down bad 4k
            int dust = Dust.NewDust(npc.Top, 0, 0, DustID.Torch, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f);
            Main.dust[dust].velocity.Y -= 0.5f + 2.5f * ratio;
            Main.dust[dust].velocity *= 1f + 3f * ratio;
            Main.dust[dust].scale = 0.5f + 5.5f * ratio;
            if (Main.rand.NextBool(4))
            {
                Main.dust[dust].scale += 0.5f;
                Main.dust[dust].noGravity = true;
            }

            if (++FuseTimer >= maxFuseTime)
            {
                npc.life = 0;
                npc.HitEffect();
                if (npc.DeathSound != null)
                    SoundEngine.PlaySound(npc.DeathSound.Value, npc.Center);
                npc.active = false;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (FargoSoulsUtil.AnyBossAlive())
                    {
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ProjectileID.BouncyGrenade, 60, 8f, Main.myPlayer);
                    }
                    else
                    {
                        for (int i = 0; i < 30; i++)
                        {
                            int p = Projectile.NewProjectile(npc.GetSource_FromThis(), npc.position.X + Main.rand.Next(npc.width), npc.position.Y + Main.rand.Next(npc.height), Main.rand.Next(-1000, 1001) / 100, Main.rand.Next(-2000, 101) / 100, ModContent.ProjectileType<ClownBomb>(), 100, 8f, Main.myPlayer);
                            Main.projectile[p].timeLeft -= Main.rand.Next(120);
                        }

                        for (int i = 0; i < 30; i++)
                        {
                            int type = ProjectileID.Grenade;
                            int damage = 250;
                            float knockback = 8f;
                            switch (Main.rand.Next(10))
                            {
                                case 0:
                                case 1:
                                case 2: type = ProjectileID.HappyBomb; break;
                                case 3:
                                case 4:
                                case 5:
                                case 6: type = ProjectileID.BouncyGrenade; break;
                                case 7:
                                case 8:
                                case 9: type = ProjectileID.StickyGrenade; break;
                            }

                            int p = Projectile.NewProjectile(npc.GetSource_FromThis(), npc.position.X + Main.rand.Next(npc.width), npc.position.Y + Main.rand.Next(npc.height), Main.rand.Next(-1000, 1001) / 100, Main.rand.Next(-2000, 101) / 100, type, damage, knockback, Main.myPlayer);
                            Main.projectile[p].timeLeft += Main.rand.Next(-120, 120);
                        }
                    }
                }
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(ModContent.BuffType<Fused>(), 1800);
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.PartyGirlGrenade, 1, 1, 10));
        }
    }
}
