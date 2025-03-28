﻿using FargowiltasSouls.Content.Projectiles.BossWeapons;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using System;

namespace FargowiltasSouls.Content.Items.Weapons.BossDrops
{
    public class SlimeKingsSlasher : SoulsItem
    {
        int Timer = 0;
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 35;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 44;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ItemRarityID.Green;
            //Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.scale = 1.5f;

            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.shoot = ModContent.ProjectileType<SlimeKingSlasherProj>();
            Item.shootSpeed = 1f;
        }
        public override void HoldItem(Player player)
        {
            
        }
        public override void ModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (target.onFire || target.onFire2 || target.onFire3)
            {
                modifiers.FinalDamage *= 1.2f;
            }
                
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            
        }
    }
}