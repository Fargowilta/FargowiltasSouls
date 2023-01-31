using System.IO;
using Terraria.ModLoader.IO;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.NPCs;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.FrostMoon
{
    public class Krampus : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Krampus);

        public int JumpTimer;

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            binaryWriter.Write7BitEncodedInt(JumpTimer);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            JumpTimer = binaryReader.Read7BitEncodedInt();
        }

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            JumpTimer = Main.rand.Next(60);
        }

        public override bool SafePreAI(NPC npc)
        {
            bool result = base.SafePreAI(npc);

            const float gravity = 0.4f;

            if (++JumpTimer > 600) //initiate jump
            {
                JumpTimer = 0;
                int t = npc.HasPlayerTarget ? npc.target : npc.FindClosestPlayer();
                if (t != -1 && Main.netMode != NetmodeID.MultiplayerClient && npc.Distance(Main.player[t].Center) < 900)
                {
                    const float time = 90;
                    Vector2 distance;
                    if (Main.player[t].active && !Main.player[t].dead && !Main.player[t].ghost)
                        distance = Main.player[t].Center - npc.Bottom;
                    else
                        distance = new Vector2(npc.Center.X < Main.player[t].Center.X ? -300 : 300, -100);
                    distance.X = distance.X / time;
                    distance.Y = distance.Y / time - 0.5f * gravity * time;
                    npc.ai[1] = time;
                    npc.ai[2] = distance.X;
                    npc.ai[3] = distance.Y;
                    npc.netUpdate = true;
                }

                return false;
            }

            if (JumpTimer == 540)
            {
                if (npc.HasValidTarget && npc.Distance(Main.player[npc.target].Center) < 900)
                {
                    FargoSoulsUtil.DustRing(npc.Center, 64, DustID.RedTorch, 12f, default, 4f);
                }
                else
                {
                    JumpTimer -= 120;
                    NetSync(npc);
                }
            }

            if (JumpTimer > 540)
            {
                npc.velocity.X = 0;
                return false; //pause before jump
            }

            if (npc.ai[1] > 0f) //while jumping
            {
                npc.ai[1]--;
                npc.noTileCollide = true;
                npc.velocity.X = npc.ai[2];
                npc.velocity.Y = npc.ai[3];
                npc.ai[3] += gravity;

                int num22 = 5;
                for (int index1 = 0; index1 < num22; ++index1)
                {
                    Vector2 vector2_2 = ((float)(Main.rand.NextDouble() * 3.14159274101257) - 1.570796f).ToRotationVector2() * Main.rand.Next(3, 8);
                    int index2 = Dust.NewDust(npc.position, npc.width, npc.height, DustID.RedTorch, vector2_2.X * 2f, vector2_2.Y * 2f, 100, new Color(), 1.4f);
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].noLight = true;
                    Main.dust[index2].velocity /= 4f;
                    Main.dust[index2].velocity -= npc.velocity;
                }

                JumpTimer = 0;

                return false;
            }
            else
            {
                if (npc.noTileCollide)
                {
                    JumpTimer = 0;
                    npc.noTileCollide = Collision.SolidCollision(npc.position, npc.width, npc.height);
                    return false;
                }
            }

            return result;
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(ModContent.BuffType<Midas>(), 600);
            target.AddBuff(ModContent.BuffType<Unlucky>(), 60 * 30);

            //if (target.whoAmI == Main.myPlayer && target.HasBuff(ModContent.BuffType<LoosePockets>()))
            //{
            //    //try stealing mouse item, then selected item
            //    bool stolen = EModeGlobalNPC.StealFromInventory(target, ref Main.mouseItem);
            //    if (!stolen)
            //        stolen = EModeGlobalNPC.StealFromInventory(target, ref target.inventory[target.selectedItem]);

            //    for (int i = 0; i < 15; i++)
            //    {
            //        int toss = Main.rand.Next(3, 8 + target.extraAccessorySlots); //pick random accessory slot
            //        if (Main.rand.NextBool(3) && target.armor[toss + 10].stack > 0) //chance to pick vanity slot if accessory is there
            //            toss += 10;
            //        if (EModeGlobalNPC.StealFromInventory(target, ref target.armor[toss]))
            //        {
            //            stolen = true;
            //            break;
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
