﻿using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Masomode
{
    public class Deerclawps : SoulsItem
    {
        public override bool Eternity => true;
        public override void SetStaticDefaults()
        {

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(0, 3);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[BuffID.Slow] = true;
            player.buffImmune[BuffID.Frozen] = true;
            player.AddEffect<DeerclawpsEffect>(Item);
            player.AddEffect<DeerclawpsDashDR>(Item);
        }

    }
    public class DeerclawpsDashDR : AccessoryEffect
    {
        public override Header ToggleHeader => null;
        public override int ToggleItemType => ModContent.ItemType<Deerclawps>();
        public override float ContactDamageDR(Player player, NPC npc, ref Player.HurtModifiers modifiers)
        {
            return DashDR(player);
        }
        public override float ProjectileDamageDR(Player player, Projectile projectile, ref Player.HurtModifiers modifiers)
        {
            return DashDR(player);
        }


        public static float DashDR(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            float dr = 0;
            if (modPlayer.IsInADashState)
            {
                dr += 0.15f;
            }

            return dr;
        }
    }
    public class DeerclawpsEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<SupremeFairyHeader>();
        public override int ToggleItemType => ModContent.ItemType<Deerclawps>();
        public static void DeerclawpsAttack(Player player, Vector2 pos)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                Vector2 vel = 16f * -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(30));

                int dam = 32;
                int type = ProjectileID.DeerclopsIceSpike;
                float ai0 = -15f;
                float ai1 = Main.rand.NextFloat(0.5f, 1f);
                if (player.FargoSouls().SupremeDeathbringerFairy)
                {
                    dam = 32;
                    type = ProjectileID.SharpTears;
                    ai0 *= 2f;
                    ai1 += 0.5f;
                }
                dam = (int)(dam * player.ActualClassDamage(DamageClass.Melee));

                if (player.velocity.Y == 0)
                    Projectile.NewProjectile(player.GetSource_EffectItem<DeerclawpsEffect>(), pos, vel, type, dam, 4f, Main.myPlayer, ai0, ai1);
                else
                {
                    int npcID = FargoSoulsUtil.FindClosestHostileNPC(pos, 300, true, true);
                    if (!npcID.IsWithinBounds(Main.maxNPCs))
                        return;
                    NPC npc = Main.npc[npcID];
                    if (!npc.Alive())
                        return;
                    vel = pos.DirectionTo(npc.Center) * vel.Length();
                    Projectile.NewProjectile(player.GetSource_EffectItem<DeerclawpsEffect>(), pos, vel.RotatedByRandom(MathHelper.PiOver2 * 0.3f), type, dam, 4f, Main.myPlayer, ai0, ai1 / 2);

                }
                    
            }
        }
    }
}
