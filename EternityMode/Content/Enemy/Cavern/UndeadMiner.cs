using System.IO;
using Terraria.ModLoader.IO;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.NPCs;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.Cavern
{
    public class UndeadMiner : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.UndeadMiner);

        public int Counter;

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            binaryWriter.Write7BitEncodedInt(Counter);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            Counter = binaryReader.Read7BitEncodedInt();
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (Counter == 180)
            {
                if (npc.DeathSound != null)
                    SoundEngine.PlaySound(npc.DeathSound.Value, npc.Center);
                FargoSoulsUtil.DustRing(npc.Center, 32, DustID.Teleporter, 5f, default, 2f);
            }

            if (++Counter > 240)
            {
                Counter = 0;
                NetSync(npc);

                if (Main.netMode != NetmodeID.MultiplayerClient && npc.HasValidTarget && npc.Distance(Main.player[npc.target].Center) < 800)
                {
                    Vector2 speed = Main.player[npc.target].Center - npc.Center;
                    speed.Y -= Math.Abs(speed.X) * 0.25f; //account for gravity
                    speed.X += Main.rand.Next(-20, 21);
                    speed.Y += Main.rand.Next(-20, 21);
                    speed.Normalize();
                    speed *= 12f;
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, speed, ProjectileID.BombSkeletronPrime, (int)(npc.damage * .7), 0f, Main.myPlayer);
                }
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(ModContent.BuffType<Lethargic>(), 600);
            target.AddBuff(BuffID.Blackout, 300);
            target.AddBuff(BuffID.NoBuilding, 300);
            
            //if (target.whoAmI == Main.myPlayer && target.HasBuff(ModContent.BuffType<LoosePockets>()))
            //{
            //    bool stolen = false;
            //    for (int i = 0; i < 59; i++)
            //    {
            //        if (target.inventory[i].pick != 0 || target.inventory[i].hammer != 0 || target.inventory[i].axe != 0)
            //        {
            //            if (EModeGlobalNPC.StealFromInventory(target, ref target.inventory[i]))
            //                stolen = true;
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
