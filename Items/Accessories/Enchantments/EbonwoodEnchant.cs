using FargowiltasSouls.Projectiles;
using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class EbonwoodEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Ebonwood Enchantment");
            Tooltip.SetDefault(
@"You are surrounded by an aura of Shadowflame
Any projectiles that would deal less than 10 damage to you are destroyed
'Untapped potential'");
            //in force damage theshold increased to 25 AND any npc that has less than 200 HP is instantly killed in the aura
        }

        protected override Color nameColor => new Color(100, 90, 141);
        public override string wizardEffect => Language.GetTextValue("Mods.FargowiltasSouls.WizardEffect.Ebonwood");

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Green;
            Item.value = 10000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoSoulsPlayer>().EbonwoodEnchantItem = Item;
        }

        static int ebonTimer;

        public static void EbonwoodEffect(Player player)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            if (!player.GetToggleValue("Ebon") || player.whoAmI != Main.myPlayer)
                return;

            int dist = modPlayer.WoodForce ? 400 : 200;

            int rate = modPlayer.WoodForce ? 2 : 4;
            if (++ebonTimer >= rate)
            {
                ebonTimer = 0;

                int damageThreshold = modPlayer.WoodForce ? 25 : 10;

                float defenseFactor = 0.5f;
                if (Main.masterMode)
                    defenseFactor = 1f;
                else if (Main.expertMode)
                    defenseFactor = 0.75f;

                int closestProj = -1;
                float closestProjDist = dist;

                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile proj = Main.projectile[i];

                    if (proj.active && proj.hostile && proj.damage > 0 && proj.aiStyle != ProjAIStyleID.FallingTile && (proj.ModProjectile == null || proj.ModProjectile.CanDamage() != false))
                    {
                        float projDist = proj.Distance(player.Center);
                        if (projDist < closestProjDist && proj.GetGlobalProjectile<FargoSoulsGlobalProjectile>().DeletionImmuneRank <= 0)
                        {
                            int calcDamage = (int)(proj.damage * 2 * FargoSoulsUtil.ProjWorldDamage);
                            int dealtDamage = (int)(calcDamage - player.statDefense * defenseFactor);

                            if (dealtDamage <= damageThreshold)
                            {
                                closestProj = proj.whoAmI;
                                closestProjDist = projDist;
                            }
                        }
                    }
                }

                if (closestProj != -1)
                {
                    Projectile proj = Main.projectile[closestProj];

                    //Main.NewText($"proj calc: {calcDamage} damage vs {player.statDefense * defenseFactor} defense factor");
                    SoundEngine.PlaySound(SoundID.NPCDeath52 with { Volume = 0.5f }, proj.Center);
                    for (int j = 0; j < 20; j++)
                    {
                        int d = Dust.NewDust(proj.Center, 0, 0, DustID.Shadowflame, Scale: 2f);
                        Main.dust[d].velocity *= 2f;
                        Main.dust[d].noGravity = true;
                    }

                    proj.Kill();
                }
            }

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (npc.active && !npc.friendly && npc.lifeMax > 5 && !npc.dontTakeDamage)
                {
                    Vector2 npcComparePoint = FargoSoulsUtil.ClosestPointInHitbox(npc, player.Center);
                    if (player.Distance(npcComparePoint) < dist)
                    {
                        if (modPlayer.WoodForce && npc.life < 200 && !npc.boss && !(npc.realLife > -1 && Main.npc[npc.realLife].active && Main.npc[npc.realLife].boss))
                        {
                            npc.StrikeNPC(npc.lifeMax + npc.defense, 0f, 0);
                        }
                        else if (modPlayer.WoodForce || Collision.CanHitLine(player.Center, 0, 0, npcComparePoint, 0, 0))
                        {
                            npc.AddBuff(BuffID.ShadowFlame, 10);
                        }
                    }
                }
            }

            //dust
            for (int i = 0; i < 20; i++)
            {
                Vector2 offset = new Vector2();
                double angle = Main.rand.NextDouble() * 2d * Math.PI;
                offset.X += (float)(Math.Sin(angle) * dist);
                offset.Y += (float)(Math.Cos(angle) * dist);
                Vector2 spawnPos = player.Center + offset - new Vector2(4, 4);
                if (modPlayer.WoodForce || Collision.CanHitLine(player.Left, 0, 0, spawnPos, 0, 0) || Collision.CanHitLine(player.Right, 0, 0, spawnPos, 0, 0))
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
}
