using FargowiltasSouls.Content.Buffs;
using FargowiltasSouls.Content.Projectiles.BossWeapons;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Weapons.BossDrops
{
    public class MoonBow : SoulsItem
    {
        public override bool IsLoadingEnabled(Mod mod) => false;

        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 125;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 28;
            Item.height = 62;
            Item.useTime = 9;
            Item.useAnimation = 9;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2f;
            Item.value = Item.sellPrice(0, 15);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item5;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.autoReuse = true;
            Item.shootSpeed = 24f;

            Item.useAmmo = AmmoID.Arrow;
            Item.noMelee = true;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            Item.useAmmo = player.altFunctionUse == 2 ? AmmoID.None : AmmoID.Arrow;
            Item.noUseGraphic = player.altFunctionUse == 2;
            Item.UseSound = player.HasBuff<MoonBowBuff>() ? SoundID.Item124 : SoundID.Item5;
            return base.CanUseItem(player);
        }

        public override bool CanConsumeAmmo(Item ammo, Player player) => Main.rand.NextBool(3);

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                type = ModContent.ProjectileType<MoonBowHeld>();
                return;
            }

            if (type == ProjectileID.WoodenArrowFriendly)
                type = ProjectileID.MoonlordArrowTrail;

            Vector2 newPos = position + new Vector2(Main.rand.NextFloat(-12, 12), Main.rand.NextFloat(-28, 28)).RotatedBy(velocity.ToRotation());
            if (!Collision.SolidCollision(newPos - new Vector2(4, 4), 8, 8))
            {
                position = newPos;
                if (velocity != Vector2.Zero)
                    velocity = newPos.SafeDirectionTo(Main.MouseWorld) * velocity.Length();
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //shoot an additional arrow when buffed
            if (player.HasBuff<MoonBowBuff>())
            {
                Projectile.NewProjectile(source, player.Center, player.SafeDirectionTo(Main.MouseWorld) * velocity.Length(), type, damage, knockback, player.whoAmI);
            }
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }
    }

    public class MoonBowGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        //public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => entity.arrow;

        bool isMoonBowArrow;
        bool noGravArrow;
        bool spawned;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (projectile.arrow || projectile.type == ProjectileID.MoonlordArrowTrail)
            {
                if (source is EntitySource_ItemUse itemUse && itemUse.Item.ModItem is MoonBow)
                    isMoonBowArrow = true;

                if (source is EntitySource_Parent parent && parent.Entity is Projectile)
                {
                    Projectile sourceProj = parent.Entity as Projectile;
                    if (sourceProj != null && sourceProj.GetGlobalProjectile<MoonBowGlobalProjectile>().isMoonBowArrow)
                        isMoonBowArrow = true;
                }
            }
        }

        public override bool PreAI(Projectile projectile)
        {
            if (!spawned)
            {
                spawned = true;
                if ((projectile.arrow || projectile.type == ProjectileID.MoonlordArrowTrail) && Main.player[projectile.owner].HasBuff<MoonBowBuff>())
                {
                    noGravArrow = true;
                    projectile.extraUpdates += 1;
                }
            }

            if (noGravArrow)
                projectile.velocity.Y -= 0.1f;

            return base.PreAI(projectile);
        }

        void TryShootPortalArrow(Projectile projectile)
        {
            if (isMoonBowArrow && projectile.owner == Main.myPlayer)
            {
                isMoonBowArrow = false;
                for (int i = 0; i < 20; i++)
                {
                    const int variance = 12 * 16;
                    Vector2 spawnPos = Main.player[projectile.owner].Center;
                    spawnPos += Main.rand.NextVector2Circular(variance, variance);
                    if (Collision.CanHitLine(Main.player[projectile.owner].Center, 0, 0, spawnPos, 0, 0))
                    {
                        Projectile.NewProjectile(Entity.InheritSource(projectile), spawnPos, Vector2.Zero, ModContent.ProjectileType<MoonBowPortal>(), projectile.damage, projectile.knockBack, projectile.owner);
                        break;
                    }
                }
            }
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            TryShootPortalArrow(projectile);
        }

        public override void OnKill(Projectile projectile, int timeLeft)
        {
            TryShootPortalArrow(projectile);
        }
    }
}
