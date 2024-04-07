using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Content.Projectiles.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class MeteorEnchant : BaseEnchant
    {

        public override Color nameColor => new(95, 71, 82);


        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Green;
            Item.value = 100000;
        }

        public const int METEOR_ADDED_DURATION = 450;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            AddEffects(player, Item);
        }
        public static void AddEffects(Player player, Item item)
        {
            player.AddEffect<MeteorMomentumEffect>(item);
            player.AddEffect<MeteorTrailEffect>(item);
            player.AddEffect<MeteorEffect>(item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ItemID.MeteorHelmet)
            .AddIngredient(ItemID.MeteorSuit)
            .AddIngredient(ItemID.MeteorLeggings)
            .AddIngredient(ItemID.StarCannon)
            .AddIngredient(ItemID.Magiluminescence)
            .AddIngredient(ItemID.PlaceAbovetheClouds)

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
    public class MeteorMomentumEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<CosmoHeader>();
        public override int ToggleItemType => ModContent.ItemType<MeteorEnchant>();
        public override void PostUpdateEquips(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                if (!player.FargoSouls().NoMomentum && !player.mount.Active) //overriden by nomomentum
                {
                    player.runAcceleration *= 1.3f;
                    player.runSlowdown *= 1.3f;

                }
                player.hasMagiluminescence = true;
                if (player.HasEffect<MeteorTrailEffect>())
                {
                    const int SparkDelay = 2;
                    int Timer = (int)(Main.GlobalTimeWrappedHourly * 60) % 60;
                    if (player.velocity != Vector2.Zero && Timer % SparkDelay == 0)
                    {
                        for (int i = -1; i < 2; i += 2)
                        {
                            Vector2 vel = (-player.velocity).RotatedBy(i * MathHelper.Pi / 7).RotatedByRandom(MathHelper.Pi / 12);
                            int damage = 22;
                            Vector2 pos = player.Center;
                            Vector2 offset = Vector2.Normalize(player.velocity).RotatedBy(MathHelper.PiOver2 * -i) * (player.width / 2);
                            Projectile.NewProjectile(GetSource_EffectItem(player), pos + offset, vel, ModContent.ProjectileType<MeteorFlame>(), FargoSoulsUtil.HighestDamageTypeScaling(player, damage), 0.5f, player.whoAmI);
                        }

                        /*
                        int p = Projectile.NewProjectile(Player.GetSource_Accessory(item), pos, vel, ProjectileID.Flames, FargoSoulsUtil.HighestDamageTypeScaling(Player, damage), 0.5f, Player.whoAmI);
                        if (p != Main.maxProjectiles)
                        {
                            Main.projectile[p].DamageType = DamageClass.Generic;
                            Main.projectile[p].friendly = true;
                            Main.projectile[p].hostile = false; //just making sure
                            Main.projectile[p].scale = 0.05f;
                        }
                        */
                    }
                }
            }
        }
    }
    public class MeteorTrailEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<CosmoHeader>();
        public override int ToggleItemType => ModContent.ItemType<MeteorEnchant>();
    }
    public class MeteorEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<CosmoHeader>();
        public override int ToggleItemType => ModContent.ItemType<MeteorEnchant>();
        public override bool ExtraAttackEffect => true;
        public override void PostUpdateEquips(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (player.whoAmI == Main.myPlayer)
            {
                bool forceEffect = player.FargoSouls().ForceEffect<MeteorEnchant>();
                int damage = forceEffect ? 50 : 20;
                if (modPlayer.MeteorShower)
                {
                    if (modPlayer.MeteorTimer % (forceEffect ? 2 : 10) == 0)
                    {
                        Vector2 pos = new(player.Center.X + Main.rand.NextFloat(-1000, 1000), player.Center.Y - 1000);
                        Vector2 vel = new(Main.rand.NextFloat(-2, 2), Main.rand.NextFloat(8, 12));

                        //chance to focus on a nearby enemy with slight predictive aim
                        if (Main.rand.NextBool())
                        {
                            List<NPC> targetables = Main.npc.Where(n => n.CanBeChasedBy() && n.Distance(player.Center) < 900).ToList();
                            if (targetables.Count > 0)
                            {
                                NPC target = targetables[Main.rand.Next(targetables.Count)];
                                pos.X = target.Center.X + Main.rand.NextFloat(-32, 32);

                                //can retarget better at them, but dont aim meteors upwards
                                Vector2 predictive = Main.rand.NextFloat(10f, 30f) * target.velocity;
                                pos.X += predictive.X;
                                Vector2 targetPos = target.Center + predictive;
                                if (pos.Y < targetPos.Y)
                                {
                                    Vector2 accurateVel = vel.Length() * pos.SafeDirectionTo(targetPos);
                                    vel = Vector2.Lerp(vel, accurateVel, Main.rand.NextFloat());
                                }
                            }
                        }

                        Projectile.NewProjectile(GetSource_EffectItem(player), pos, vel, Main.rand.Next(424, 427), FargoSoulsUtil.HighestDamageTypeScaling(player, damage), 0.5f, player.whoAmI, 0, 0.5f + (float)Main.rand.NextDouble() * 0.3f);
                    }

                    if (--modPlayer.MeteorTimer <= 0)
                    {
                        modPlayer.MeteorShower = false;
                        modPlayer.MeteorCD = forceEffect ? 240 : 600;
                    }
                }
                else
                {
                    modPlayer.MeteorTimer = 150 + MeteorEnchant.METEOR_ADDED_DURATION / (forceEffect ? 1 : 10);

                    if (modPlayer.WeaponUseTimer > 0)
                    {
                        if (--modPlayer.MeteorCD <= 0)
                            modPlayer.MeteorShower = true;
                    }
                    else if (modPlayer.MeteorCD < 150) //when not using weapons, gradually increment back up
                    {
                        modPlayer.MeteorCD++;
                    }
                }
            }
        }
    }

    public class MeteorGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
            => entity.type == ProjectileID.Meteor1 || entity.type == ProjectileID.Meteor2 || entity.type == ProjectileID.Meteor3;

        bool fromEnch;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source is EntitySource_ItemUse itemSource && (itemSource.Item.type == ModContent.ItemType<MeteorEnchant>() || itemSource.Item.type == ModContent.ItemType<CosmoForce>()))
            {
                fromEnch = true;
                projectile.FargoSouls().CanSplit = false;

                //if (ModLoader.GetMod("Fargowiltas") != null)
                //    ModLoader.GetMod("Fargowiltas").Call("LowRenderProj", Main.projectile[p]);
            }
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (fromEnch)
            {
                const int maxHits = 75;
                Main.player[projectile.owner].FargoSouls().MeteorTimer -= MeteorEnchant.METEOR_ADDED_DURATION / maxHits;
            }
        }
    }
}
