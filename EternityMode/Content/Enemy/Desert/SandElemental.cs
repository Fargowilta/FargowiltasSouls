using FargowiltasSouls.EternityMode.Net;
using FargowiltasSouls.EternityMode.Net.Strategies;
using FargowiltasSouls.EternityMode.NPCMatching;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.Desert
{
    public class SandElemental : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.SandElemental);

        public int WormTimer;
        public int AttackTimer;
        public Vector2 AttackTarget;

        public override Dictionary<Ref<object>, CompoundStrategy> GetNetInfo() =>
            new Dictionary<Ref<object>, CompoundStrategy> {
                { new Ref<object>(AttackTimer), IntStrategies.CompoundStrategy }
            };

        public override void AI(NPC npc)
        {
            base.AI(npc);

            //if (++WormTimer % 60 == 0)
            //{
            //    if (NPC.AnyNPCs(NPCID.DuneSplicerHead)) //effectively, timer starts counting up when splicers are dead
            //    {
            //        WormTimer = 0;
            //    }
            //    else if (WormTimer >= 360 && Main.netMode != NetmodeID.MultiplayerClient)
            //    {
            //        FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromAI(), npc.Center, NPCID.DuneSplicerHead);
            //    }
            //}

            if (++AttackTimer == 270)
            {
                if (!npc.HasValidTarget)
                {
                    AttackTimer = 0;
                }
                else
                {
                    SoundEngine.PlaySound(SoundID.Roar, npc.Center);

                    AttackTarget = Main.player[npc.target].Center;
                    AttackTarget.Y -= 650;

                    int length = (int)npc.Distance(AttackTarget) / 10;
                    Vector2 offset = npc.DirectionTo(AttackTarget) * 10f;
                    for (int i = 0; i < length; i++) //dust warning line
                    {
                        int d = Dust.NewDust(npc.Center + offset * i, 0, 0, DustID.Sandnado, 0f, 0f, 0, new Color());
                        Main.dust[d].noLight = true;
                        Main.dust[d].scale = 1.5f;
                    }
                }

                NetSync(npc);
            }

            if (AttackTimer > 300 && AttackTimer % 3 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(npc.GetSource_FromThis(),
                    AttackTarget + Main.rand.NextVector2Circular(80, 80),
                    new Vector2(Main.rand.NextFloat(-.5f, .5f), Main.rand.NextFloat(3f)),
                    ModContent.ProjectileType<Projectiles.Champions.SpiritCrossBone>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer);
            }

            if (AttackTimer > 390)
            {
                AttackTimer = 0;
                NetSync(npc);
            }

            //if (++AttackTimer > 360)
            //{
            //    AttackTimer = 0;

            //    if (npc.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient)
            //    {
            //        Vector2 target = Main.player[npc.target].Center;
            //        target.Y -= 150;
            //        Projectile.NewProjectile(target, Vector2.Zero, ProjectileID.SandnadoHostileMark, 0, 0f, Main.myPlayer);

            //        int length = (int)npc.Distance(target) / 10;
            //        Vector2 offset = npc.DirectionTo(target) * 10f;
            //        for (int i = 0; i < length; i++) //dust warning line for sandnado
            //        {
            //            int d = Dust.NewDust(npc.Center + offset * i, 0, 0, 269, 0f, 0f, 0, new Color());
            //            Main.dust[d].noLight = true;
            //            Main.dust[d].scale = 1.25f;
            //        }
            //    }
            //}
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.ByCondition(new Conditions.IsHardmode(), ModContent.ItemType<Items.Accessories.Masomode.SandsofTime>(), 5));
            FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.OasisCrateHard));
            FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.SandstorminaBottle, 20));
        }
    }
}
