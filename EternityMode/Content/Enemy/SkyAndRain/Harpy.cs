using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.Net;
using FargowiltasSouls.EternityMode.Net.Strategies;
using FargowiltasSouls.EternityMode.NPCMatching;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.SkyAndRain
{
    public class Harpy : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Harpy);

        public int FeatherRingTimer;

        public override Dictionary<Ref<object>, CompoundStrategy> GetNetInfo() =>
            new Dictionary<Ref<object>, CompoundStrategy> {
                { new Ref<object>(FeatherRingTimer), IntStrategies.CompoundStrategy },
            };

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (++FeatherRingTimer > 300)
            {
                FeatherRingTimer = 0;
                FargoSoulsUtil.XWay(8, npc.GetSource_FromThis(), npc.Center, ProjectileID.HarpyFeather, 4f, FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0);
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(ModContent.BuffType<ClippedWings>(), 300);
            if (target.whoAmI == Main.myPlayer && target.HasBuff(ModContent.BuffType<LoosePockets>()))
            {
                bool stolen = false;
                if (Main.mouseItem.healLife > 0 && NPCs.EModeGlobalNPC.StealFromInventory(target, ref Main.mouseItem))
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
                            if (NPCs.EModeGlobalNPC.StealFromInventory(target, ref target.inventory[j]))
                                stolen = true;
                            break;
                        }
                    }
                }

                if (stolen)
                {
                    string text = Language.GetTextValue($"Mods.{mod.Name}.Message.ItemStolen");
                    Main.NewText(text, new Color(255, 50, 50));
                    CombatText.NewText(target.Hitbox, new Color(255, 50, 50), text, true);
                }
            }
            target.AddBuff(ModContent.BuffType<LoosePockets>(), 240);
        }
    }
}
