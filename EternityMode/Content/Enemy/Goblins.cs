using Fargowiltas.NPCs;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.Net;
using FargowiltasSouls.EternityMode.Net.Strategies;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.NPCs;
using FargowiltasSouls.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy
{
    public class Goblins : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() =>  new NPCMatcher().MatchTypeRange(
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

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (npc.type != NPCID.GoblinSummoner)
            {
                if (npc.HasPlayerTarget && (!Main.player[npc.target].active || Main.player[npc.target].dead))
                {
                    npc.TargetClosest();
                    if (npc.HasPlayerTarget && (!Main.player[npc.target].active || Main.player[npc.target].dead))
                    {
                        npc.noTileCollide = true;
                    }
                }
                if (npc.noTileCollide) //fall through the floor
                {
                    npc.position.Y++;
                    npc.velocity.Y++;
                }
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            if (Main.hardMode)
                target.AddBuff(ModContent.BuffType<Shadowflame>(), 300);

            if (npc.type == NPCID.GoblinThief)
            {
                target.AddBuff(ModContent.BuffType<Midas>(), 600);

                if (target.whoAmI == Main.myPlayer && !target.GetModPlayer<FargoPlayer>().SecurityWallet && Main.rand.NextBool())
                {
                    //try stealing mouse item, then selected item
                    bool stolen = npc.GetGlobalNPC<EModeGlobalNPC>().StealFromInventory(target, ref Main.mouseItem);
                    if (!stolen)
                        stolen = npc.GetGlobalNPC<EModeGlobalNPC>().StealFromInventory(target, ref target.inventory[target.selectedItem]);

                    if (stolen)
                    {
                        Main.NewText(Language.GetTextValue("Mods.FargowiltasSouls.EternityMode.Stolen"), new Color(255, 50, 50));
                        CombatText.NewText(target.Hitbox, new Color(255, 50, 50), Language.GetTextValue("Mods.FargowiltasSouls.EternityMode.Stolen"), true);
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
            }
        }

        public override bool CheckDead(NPC npc)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
                Projectile.NewProjectile(npc.Center, new Vector2(Main.rand.NextFloat(-2f, 2f), -5), ModContent.ProjectileType<GoblinSpikyBall>(), npc.damage / 4, 0, Main.myPlayer);

            return base.CheckDead(npc);
        }

        public override void NPCLoot(NPC npc)
        {
            base.NPCLoot(npc);

            if (NPC.downedGoblins && FargoSoulsWorld.firstGoblins)
            {
                FargoSoulsWorld.firstGoblins = false;
                //WorldGen.dropMeteor();
                if (!NPC.AnyNPCs(ModContent.NPCType<Abominationn>()))
                {
                    int p = Player.FindClosest(npc.Center, 0, 0);
                    if (p != -1)
                        NPC.SpawnOnPlayer(p, ModContent.NPCType<Abominationn>());
                }
            }
        }
    }

    public class GoblinSorcerer : FireImp
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.GoblinSorcerer);

        public override void OnSpawn(NPC npc)
        {
            base.OnSpawn(npc);

            if (!Main.hardMode && !NPC.downedSlimeKing && !NPC.downedBoss1 && NPC.CountNPCS(npc.type) > 2)
                npc.Transform(NPCID.GoblinPeon);
        }
    }
}
