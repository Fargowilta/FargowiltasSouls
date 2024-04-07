using FargowiltasSouls.Content.NPCs.EternityModeNPCs;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Masomode
{
    public class GuttedHeart : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Gutted Heart");
            /* Tooltip.SetDefault(@"Grants immunity to Bloodthirsty
10% increased max life
Creepers hover around you blocking some damage
A new Creeper appears every 15 seconds, and 5 can exist at once
Creeper respawn speed increases when not moving
'Once beating in the mind of a defeated foe'"); */
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "破碎的心");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, @"'曾经还在敌人的脑中跳动着'
            // 免疫嗜血
            // 增加10%最大生命值
            // 爬行者徘徊周围来阻挡伤害
            // 每15秒生成一个新的爬行者,最多同时存在5个");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(0, 2);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statLifeMax2 += player.statLifeMax / 10;
            player.buffImmune[ModContent.BuffType<Buffs.Masomode.BloodthirstyBuff>()] = true;
            player.AddEffect<GuttedHeartEffect>(Item);
            player.AddEffect<GuttedHeartMinions>(Item);
        }
    }
    public class GuttedHeartEffect : AccessoryEffect
    {
        public override Header ToggleHeader => null;
        public override int ToggleItemType => ModContent.ItemType<GuttedHeart>();
        public override void PostUpdateEquips(Player player)
        {
            Player Player = player;
            if (Player.whoAmI != Main.myPlayer)
                return;
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            modPlayer.GuttedHeartCD--;

            if (Player.velocity == Vector2.Zero && Player.itemAnimation == 0)
                modPlayer.GuttedHeartCD--;

            if (modPlayer.GuttedHeartCD <= 0)
            {
                int cd = (int)Math.Round(Utils.Lerp(10 * 60, 15 * 60, (float)Player.statLife / Player.statLifeMax2));
                modPlayer.GuttedHeartCD = cd;
                if (Player.HasEffect<GuttedHeartMinions>())
                {
                    int count = 0;
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<CreeperGutted>() && Main.npc[i].ai[0] == Player.whoAmI)
                            count++;
                    }
                    if (count < 5)
                    {
                        int multiplier = 1;
                        if (modPlayer.PureHeart)
                            multiplier = 2;
                        if (modPlayer.MasochistSoul)
                            multiplier = 5;
                        if (Main.netMode == NetmodeID.SinglePlayer)
                        {
                            int n = NPC.NewNPC(NPC.GetBossSpawnSource(Player.whoAmI), (int)Player.Center.X, (int)Player.Center.Y, ModContent.NPCType<CreeperGutted>(), 0, Player.whoAmI, 0f, multiplier);
                            if (n != Main.maxNPCs)
                                Main.npc[n].velocity = Vector2.UnitX.RotatedByRandom(2 * Math.PI) * 8;
                        }
                        else if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            var netMessage = FargowiltasSouls.Instance.GetPacket();
                            netMessage.Write((byte)FargowiltasSouls.PacketID.RequestGuttedCreeper);
                            netMessage.Write((byte)Player.whoAmI);
                            netMessage.Write((byte)multiplier);
                            netMessage.Send();
                        }
                    }
                    else
                    {
                        int lowestHealth = -1;
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<CreeperGutted>() && Main.npc[i].ai[0] == Player.whoAmI)
                            {
                                if (lowestHealth < 0)
                                    lowestHealth = i;
                                else if (Main.npc[i].life < Main.npc[lowestHealth].life)
                                    lowestHealth = i;
                            }
                        }
                        if (Main.npc[lowestHealth].life < Main.npc[lowestHealth].lifeMax)
                        {
                            if (Main.netMode == NetmodeID.SinglePlayer)
                            {
                                int damage = Main.npc[lowestHealth].lifeMax - Main.npc[lowestHealth].life;
                                Main.npc[lowestHealth].life = Main.npc[lowestHealth].lifeMax;
                                CombatText.NewText(Main.npc[lowestHealth].Hitbox, CombatText.HealLife, damage);
                            }
                            else if (Main.netMode == NetmodeID.MultiplayerClient)
                            {
                                var netMessage = FargowiltasSouls.Instance.GetPacket();
                                netMessage.Write((byte)FargowiltasSouls.PacketID.RequestCreeperHeal);
                                netMessage.Write((byte)Player.whoAmI);
                                netMessage.Write((byte)lowestHealth);
                                netMessage.Send();
                            }
                        }
                    }
                }
            }
        }

    }
    public class GuttedHeartMinions : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<PureHeartHeader>();
        public override int ToggleItemType => ModContent.ItemType<GuttedHeart>();
        public override bool MinionEffect => true;

        public static void NurseHeal(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];

                    if (npc.type == ModContent.NPCType<CreeperGutted>() && npc.ai[0] == player.whoAmI)
                    {
                        int heal = npc.lifeMax - npc.life;

                        if (Main.netMode == NetmodeID.SinglePlayer)
                        {
                            if (heal > 0)
                            {
                                npc.HealEffect(heal);
                                npc.life = npc.lifeMax;
                            }
                        }
                        else if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            var netMessage = FargowiltasSouls.Instance.GetPacket();
                            netMessage.Write((byte)FargowiltasSouls.PacketID.RequestCreeperHeal);
                            netMessage.Write((byte)player.whoAmI);
                            netMessage.Write((byte)i);
                            netMessage.Send();
                        }
                    }
                }
            }
        }
    }
}