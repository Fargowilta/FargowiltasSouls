using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.Net;
using FargowiltasSouls.EternityMode.Net.Strategies;
using FargowiltasSouls.EternityMode.NPCMatching;
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
    public class Harpy : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Harpy);

        public int FeatherRingTimer;

        public override Dictionary<Ref<object>, CompoundStrategy> GetNetInfo() =>
            new Dictionary<Ref<object>, CompoundStrategy> {
                { new Ref<object>(FeatherRingTimer), IntStrategies.CompoundStrategy },
            };

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.buffImmune[BuffID.Suffocation] = true;
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (++FeatherRingTimer > 300)
            {
                FeatherRingTimer = 0;
                FargoSoulsUtil.XWay(8, npc.Center, ProjectileID.HarpyFeather, 4f, npc.damage / 4, 0f);
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(ModContent.BuffType<ClippedWings>(), 300);
            if (target.whoAmI == Main.myPlayer && !target.GetModPlayer<FargoPlayer>().SecurityWallet)
            {
                bool stolen = false;
                if (Main.mouseItem.healLife > 0 && npc.GetGlobalNPC<NPCs.EModeGlobalNPC>().StealFromInventory(target, ref Main.mouseItem))
                {
                    stolen = true;
                }
                else
                {
                    for (int j = 0; j < target.inventory.Length; j++)
                    {
                        Item item = target.inventory[j];
                        if (item.healLife > 0)
                        {
                            if (npc.GetGlobalNPC<NPCs.EModeGlobalNPC>().StealFromInventory(target, ref target.inventory[j]))
                                stolen = true;
                            break;
                        }
                    }
                }

                if (stolen)
                {
                    Main.NewText(Language.GetTextValue("Mods.FargowiltasSouls.EternityMode.Stolen"), new Color(255, 50, 50));
                    CombatText.NewText(target.Hitbox, new Color(255, 50, 50), Language.GetTextValue("Mods.FargowiltasSouls.EternityMode.Stolen"), true);
                }
            }
        }
    }
}
