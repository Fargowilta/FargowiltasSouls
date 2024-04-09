using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Night
{
    public class Wraith : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Wraith);

        public override void AI(NPC npc)
        {
            base.AI(npc);

            EModeGlobalNPC.Aura(npc, 80, BuffID.Obstructed, false, DustID.Clentaminator_Red);

            npc.aiStyle = NPCAIStyleID.Flying;
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(ModContent.BuffType<LivingWastelandBuff>(), 600);
            target.AddBuff(ModContent.BuffType<UnluckyBuff>(), 60 * 30);
            //if (target.whoAmI == Main.myPlayer && target.HasBuff(ModContent.BuffType<LoosePockets>()))
            //{
            //    bool IsSoul(int type)
            //    {
            //        return type == ItemID.SoulofFlight || type == ItemID.SoulofFright || type == ItemID.SoulofLight || type == ItemID.SoulofMight || type == ItemID.SoulofNight || type == ItemID.SoulofSight;
            //    };

            //    bool stolen = false;
            //    if (IsSoul(Main.mouseItem.type) && EModeGlobalNPC.StealFromInventory(target, ref Main.mouseItem))
            //    {
            //        stolen = true;
            //    }
            //    else
            //    {
            //        for (int j = 0; j < target.inventory.Length; j++)
            //        {
            //            Item item = target.inventory[j];

            //            if (IsSoul(item.type))
            //            {
            //                if (EModeGlobalNPC.StealFromInventory(target, ref target.inventory[j]))
            //                    stolen = true;
            //                break;
            //            }
            //        }
            //    }

            //    if (stolen)
            //    {
            //        string text = Language.GetTextValue($"Mods.{mod.Name}.Message.ItemStolen");
            //        Main.NewText(text, new Color(255, 50, 50));
            //        CombatText.NewText(target.Hitbox, new Color(255, 50, 50), text, true);
            //    }
            //}
            //target.AddBuff(ModContent.BuffType<LoosePockets>(), 240);
        }
    }
}
