using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.NPCs;
using FargowiltasSouls.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.GoblinInvasion
{
    public class Goblins : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.GoblinArcher,
            NPCID.GoblinPeon,
            NPCID.GoblinScout,
            NPCID.GoblinSorcerer,
            NPCID.GoblinSummoner,
            NPCID.GoblinThief,
            NPCID.GoblinWarrior
        );

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            if (npc.type == NPCID.GoblinWarrior)
                npc.knockBackResist /= 10;
        }

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            if (!FargoSoulsWorld.downedAnyBoss && (npc.type == NPCID.GoblinWarrior || npc.type == NPCID.GoblinThief || npc.type == NPCID.GoblinArcher) && NPC.CountNPCS(npc.type) > 3)
            {
                npc.Transform(NPCID.GoblinPeon);
            }
        }

        //public override void AI(NPC npc)
        //{
        //    base.AI(npc);

        //    if (npc.type != NPCID.GoblinSummoner)
        //    {
        //        if (npc.HasPlayerTarget && (!Main.player[npc.target].active || Main.player[npc.target].dead))
        //        {
        //            npc.TargetClosest();
        //            if (npc.HasPlayerTarget && (!Main.player[npc.target].active || Main.player[npc.target].dead))
        //            {
        //                npc.noTileCollide = true;
        //            }
        //        }
        //        if (npc.noTileCollide) //fall through the floor
        //        {
        //            npc.position.Y++;
        //            npc.velocity.Y++;
        //        }
        //    }
        //}

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            if (Main.hardMode)
                target.AddBuff(ModContent.BuffType<Shadowflame>(), 300);

            if (npc.type == NPCID.GoblinThief)
            {
                target.AddBuff(ModContent.BuffType<Midas>(), 600);

                if (target.whoAmI == Main.myPlayer && target.HasBuff(ModContent.BuffType<LoosePockets>()))
                {
                    //try stealing mouse item, then selected item
                    bool stolen = EModeGlobalNPC.StealFromInventory(target, ref Main.mouseItem);
                    if (!stolen)
                        stolen = EModeGlobalNPC.StealFromInventory(target, ref target.inventory[target.selectedItem]);

                    if (stolen)
                    {
                        string text = Language.GetTextValue($"Mods.{mod.Name}.Message.ItemStolen");
                        Main.NewText(text, new Color(255, 50, 50));
                        CombatText.NewText(target.Hitbox, new Color(255, 50, 50), text, true);
                    }

                    /*byte extraTries = 30;
                    for (int i = 0; i < 3; i++)
                    {
                        bool successfulSteal = StealFromInventory(target, ref target.inventory[Main.rand.Next(target.inventory.Length)]);

                        if (!successfulSteal && extraTries > 0)
                        {
                            extraTries--;
                            i--;
                        }
                    }*/
                }
                target.AddBuff(ModContent.BuffType<LoosePockets>(), 240);
            }
        }

        public override void OnKill(NPC npc)
        {
            base.OnKill(npc);

            if (Main.netMode != NetmodeID.MultiplayerClient)
                Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, new Vector2(Main.rand.NextFloat(-2f, 2f), -5), ModContent.ProjectileType<GoblinSpikyBall>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0, Main.myPlayer);

            if (NPC.downedGoblins && !FargoSoulsWorld.haveForcedAbomFromGoblins)
            {
                FargoSoulsWorld.haveForcedAbomFromGoblins = true;

                if (ModContent.TryFind("Fargowiltas", "Abominationn", out ModNPC modNPC) && !NPC.AnyNPCs(modNPC.Type))
                {
                    int p = Player.FindClosest(npc.Center, 0, 0);
                    if (p != -1)
                        NPC.SpawnOnPlayer(p, modNPC.Type);
                }
            }
        }
    }
}
