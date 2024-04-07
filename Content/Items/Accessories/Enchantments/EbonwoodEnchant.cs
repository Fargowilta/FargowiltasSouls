using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.ModPlayers;
using FargowiltasSouls.Core.Toggler;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class EbonwoodEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Ebonwood Enchantment");
            /* Tooltip.SetDefault(
@"You are surrounded by an aura of Shadowflame
Any projectiles that would deal less than 10 damage to you are destroyed
'Untapped potential'"); */
            //in force damage theshold increased to 25 AND any npc that has less than 200 HP is instantly killed in the aura
        }

        public override Color nameColor => new(100, 90, 141);


        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Blue;
            Item.value = 10000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<EbonwoodEffect>(Item);

        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ItemID.EbonwoodHelmet)
            .AddIngredient(ItemID.EbonwoodBreastplate)
            .AddIngredient(ItemID.EbonwoodGreaves)
            .AddIngredient(ItemID.VileMushroom)
            .AddIngredient(ItemID.BlackCurrant)
            .AddIngredient(ItemID.LightlessChasms)


            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
    public class EbonwoodEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<TimberHeader>();
        public override int ToggleItemType => ModContent.ItemType<EbonwoodEnchant>();

        public override void PostUpdateEquips(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (player.whoAmI != Main.myPlayer)
                return;

            bool forceEffect = modPlayer.ForceEffect<EbonwoodEnchant>();
            int dist = forceEffect ? 400 : 200;
            foreach (NPC npc in Main.npc.Where(n => n.active && !n.friendly && n.lifeMax > 5 && !n.dontTakeDamage))
            {
                Vector2 npcComparePoint = FargoSoulsUtil.ClosestPointInHitbox(npc, player.Center);
                if (player.Distance(npcComparePoint) < dist && (forceEffect || Collision.CanHitLine(player.Center, 0, 0, npcComparePoint, 0, 0)))
                {
                    if (!(npc.HasBuff<CorruptedBuffForce>() || npc.HasBuff<CorruptedBuff>()))
                    {
                        npc.AddBuff(ModContent.BuffType<CorruptingBuff>(), 2);
                    }
                }
                if (npc.FargoSouls().EbonCorruptionTimer > 60 * 3 && (!(npc.HasBuff<CorruptedBuffForce>() || npc.HasBuff<CorruptedBuff>())))
                {
                    EbonwoodProc(player, npc, dist, forceEffect, 5);
                }
            }
            //dust
            for (int i = 0; i < 20; i++)
            {
                Vector2 offset = new();
                double angle = Main.rand.NextDouble() * 2d * Math.PI;
                offset.X += (float)(Math.Sin(angle) * dist);
                offset.Y += (float)(Math.Cos(angle) * dist);
                Vector2 spawnPos = player.Center + offset - new Vector2(4, 4);
                if (forceEffect || Collision.CanHitLine(player.Left, 0, 0, spawnPos, 0, 0) || Collision.CanHitLine(player.Right, 0, 0, spawnPos, 0, 0))
                {
                    Dust dust = Main.dust[Dust.NewDust(
                        spawnPos, 0, 0,
                        DustID.Shadowflame, 0, 0, 100, Color.White, 1f
                        )];
                    dust.velocity = player.velocity;
                    if (Main.rand.NextBool(3))
                        dust.velocity += Vector2.Normalize(offset) * -5f;
                    dust.noGravity = true;
                }
            }
        }
        public static void EbonwoodProc(Player player, NPC npc, int AoE, bool force, int limit)
        {
            //corrupt all in vicinity
            foreach (NPC npcToProcOn in Main.npc.Where(n => n.active && !n.friendly && n.lifeMax > 5 && !n.dontTakeDamage))
            {
                Vector2 npcComparePoint = FargoSoulsUtil.ClosestPointInHitbox(npcToProcOn, npc.Center);
                if (npc.Distance(npcComparePoint) < AoE && !npc.HasBuff<CorruptedBuffForce>() && !npc.HasBuff<CorruptedBuff>() && limit > 0)
                {
                    EbonwoodProc(player, npc, AoE, force, limit - 1); //yes this chains (up to 3 times deep)
                }
            }

            Corrupt(npc, force);
            SoundEngine.PlaySound(SoundID.NPCDeath55, npc.Center);
            npc.FargoSouls().EbonCorruptionTimer = 0;

            //dust
            for (int i = 0; i < 60; i++)
            {
                Vector2 offset = new();
                double angle = Main.rand.NextDouble() * 2d * Math.PI;
                offset.X += (float)(Math.Sin(angle) * AoE);
                offset.Y += (float)(Math.Cos(angle) * AoE);
                Vector2 spawnPos = npc.Center + offset - new Vector2(4, 4);
                Dust dust = Main.dust[Dust.NewDust(spawnPos, 0, 0, DustID.Shadowflame, 0, 0, 100, Color.White, 1f)];
                dust.velocity = npc.velocity;
                if (Main.rand.NextBool(3))
                    dust.velocity += Vector2.Normalize(offset) * -5f;
                dust.noGravity = true;
            }
        }
        private static void Corrupt(NPC npc, bool force)
        {
            if (npc.HasBuff<CorruptedBuffForce>() || npc.HasBuff<CorruptedBuff>()) //don't stack the buffs under any circumstances
            {
                return;
            }
            if (force)
            {
                npc.AddBuff(ModContent.BuffType<CorruptedBuffForce>(), 60 * 4);
                return;
            }
            npc.AddBuff(ModContent.BuffType<CorruptedBuff>(), 60 * 4);
        }
    }
}
