using FargowiltasSouls.Content.Buffs;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Projectiles.Masomode;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Masomode
{
    [AutoloadEquip(EquipType.Waist)]
    public class WretchedPouch : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Wretched Pouch");
            /* Tooltip.SetDefault(
@"Grants immunity to Shadowflame
While attacking, increases damage by 120% but reduces damage reduction by 20% and massively decreases movement
While attacking, shadowflame tentacles lash out at nearby enemies
Attack speed bonuses are half as effective
'The accursed incendiary powder of a defeated foe'"); */
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "诅咒袋子");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, @"'被打败的敌人的诅咒燃烧炸药'
            // 免疫暗影烈焰
            // 受伤时爆发暗影烈焰触须");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(0, 4);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[BuffID.ShadowFlame] = true;
            player.buffImmune[ModContent.BuffType<ShadowflameBuff>()] = true;

            player.AddEffect<WretchedPouchEffect>(Item);
        }
    }
    public class WretchedPouchEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<BionomicHeader>();
        public override int ToggleItemType => ModContent.ItemType<WretchedPouch>();
        public override bool ExtraAttackEffect => true;
        
        public override void PostUpdateEquips(Player player)
        {
            Player Player = player;
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            if (!Player.controlUseItem && !Player.controlUseTile && modPlayer.WeaponUseTimer <= 6) //remove extra 6 added to the timer, makes it a lot less awkward
                return;

            if (Player.HeldItem.IsAir || Player.HeldItem.damage <= 0 || Player.HeldItem.pick > 0 || Player.HeldItem.axe > 0 || Player.HeldItem.hammer > 0)
                return;

            if (!Player.HasBuff(ModContent.BuffType<WretchedHexBuff>()))
                return;

            int d = Dust.NewDust(Player.position, Player.width, Player.height, DustID.Shadowflame, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 0, new Color(), 3f);
            Main.dust[d].noGravity = true;
            Main.dust[d].velocity *= 5f;

            Player.GetDamage(DamageClass.Generic) += 1.20f;
            Player.endurance -= 0.20f;
            Player.velocity *= 0.875f;

            if (--modPlayer.WretchedPouchCD <= 0)
            {
                modPlayer.WretchedPouchCD = 25;

                if (Player.whoAmI == Main.myPlayer)
                {
                    Vector2 vel = Main.rand.NextVector2Unit();

                    NPC target = Main.npc.FirstOrDefault(n => n.active && n.Distance(Player.Center) < 360 && n.CanBeChasedBy() && Collision.CanHit(Player.position, Player.width, Player.height, n.position, n.width, n.height));
                    if (target != null)
                        vel = Player.SafeDirectionTo(target.Center);

                    vel *= 8f;

                    SoundEngine.PlaySound(SoundID.Item103, Player.Center);

                    int dam = 40;
                    if (modPlayer.MasochistSoul)
                        dam *= 3;
                    dam = (int)(dam * Player.ActualClassDamage(DamageClass.Magic));

                    void ShootTentacle(Vector2 baseVel, float variance, int aiMin, int aiMax)
                    {
                        Vector2 speed = baseVel.RotatedBy(variance * (Main.rand.NextDouble() - 0.5));
                        float ai0 = Main.rand.Next(aiMin, aiMax) * (1f / 1000f);
                        if (Main.rand.NextBool())
                            ai0 *= -1f;
                        float ai1 = Main.rand.Next(aiMin, aiMax) * (1f / 1000f);
                        if (Main.rand.NextBool())
                            ai1 *= -1f;
                        Projectile.NewProjectile(GetSource_EffectItem(Player), Player.Center, speed, ModContent.ProjectileType<ShadowflameTentacle>(), dam, 4f, Player.whoAmI, ai0, ai1);
                    };

                    int max = target == null ? 3 : 6;
                    float rotationOffset = MathHelper.TwoPi / max;
                    if (target != null)
                    {
                        for (int i = 0; i < max / 2; i++) //shoot right at them
                            ShootTentacle(vel, rotationOffset, 60, 90);
                    }
                    for (int i = 0; i < max; i++) //shoot everywhere
                        ShootTentacle(vel.RotatedBy(rotationOffset * i), rotationOffset, 30, 50);
                }
            }
        }
    }
}