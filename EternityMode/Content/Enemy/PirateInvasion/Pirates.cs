using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.NPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.PirateInvasion
{
    public class Pirates : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.PirateCaptain,
            NPCID.PirateCorsair,
            NPCID.PirateCrossbower,
            NPCID.PirateDeadeye,
            NPCID.PirateShipCannon,
            NPCID.PirateDeckhand,
            NPCID.PirateGhost
        );

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

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
            }
            target.AddBuff(ModContent.BuffType<LoosePockets>(), 240);

            target.AddBuff(ModContent.BuffType<Midas>(), 600);
            //target.AddBuff(ModContent.BuffType<LivingWasteland>(), 600);
        }
    }
}
