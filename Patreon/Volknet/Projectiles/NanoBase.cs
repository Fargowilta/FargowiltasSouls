using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Patreon.Volknet.Projectiles
{
    public class NanoBase : ModProjectile
    {
        public int AtkTimer = 0;
        public float MeleeDamageModifier = 1;                   //may helpful with modifying damage
        public float RangedDamageModifier = 1;
        public float MagicDamageModifier = 1;
        public float SummonDamageModifier = 1;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nano Core");
            //DisplayName.AddTranslation(GameCulture.Chinese, "纳米基核");
        }
        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.hide = true;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.timeLeft = 2;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.damage = 1;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Player owner = Main.player[Projectile.owner];

            if (owner.channel && owner.GetModPlayer<NanoPlayer>().NanoCoreMode == 1 && NPCUtils.AnyProj(ModContent.ProjectileType<NanoProbe>(), owner.whoAmI))
            {
                bool Allset = true;
                foreach (Projectile proj in Main.projectile)
                {
                    if (proj.active && proj.type == ModContent.ProjectileType<NanoProbe>() && proj.owner == owner.whoAmI && proj.ai[1] == 0)
                    {
                        Allset = false;
                    }
                }
                if (Allset)
                {
                    Vector2 TM = Projectile.rotation.ToRotationVector2();
                    Vector2 begin = owner.Center + TM * 40 + (TM.ToRotation() + MathHelper.Pi / 2).ToRotationVector2() * 30 + (TM.ToRotation() + MathHelper.Pi / 4 * 3).ToRotationVector2() * 60;
                    Vector2 end = owner.Center + TM * 40 + (TM.ToRotation() - MathHelper.Pi / 2).ToRotationVector2() * 30 + (TM.ToRotation() - MathHelper.Pi / 4 * 3).ToRotationVector2() * 60;
                    Terraria.Utils.DrawLine(Main.spriteBatch, begin, end, Color.LightGreen, Color.DarkGreen, 3);
                }
            }
            return false;
        }

        public override bool? CanDamage() => false;

        public override void AI()
        {
            if (Main.player[Projectile.owner].active)
            {
                Player owner = Main.player[Projectile.owner];
                if (!owner.dead && owner.HeldItem.type == ModContent.ItemType<NanoCore>())
                {
                    MeleeDamageModifier = owner.ActualClassDamage(DamageClass.Melee);
                    RangedDamageModifier = owner.ActualClassDamage(DamageClass.Ranged);
                    MagicDamageModifier = owner.ActualClassDamage(DamageClass.Magic);
                    SummonDamageModifier = owner.ActualClassDamage(DamageClass.Summon);

                    Projectile.timeLeft = 2;
                    Projectile.Center = owner.Center;
                    Projectile.rotation = (Main.MouseWorld - owner.Center).ToRotation();
                    if (owner.channel)
                    {
                        owner.itemTime = 2;
                        owner.itemAnimation = 2;
                    }

                    if (owner.ownedProjectileCounts[ModContent.ProjectileType<NanoProbe>()] < 7)
                    {
                        int count = 7 - owner.ownedProjectileCounts[ModContent.ProjectileType<NanoProbe>()];
                        for (int i = 0; i < count; i++)
                        {
                            Projectile.NewProjectile(owner.GetSource_ItemUse(owner.HeldItem), Projectile.Center, (Projectile.rotation + Main.rand.NextFloat() * MathHelper.Pi / 3 * 2 - MathHelper.Pi / 3).ToRotationVector2() * 14, ModContent.ProjectileType<NanoProbe>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                        }
                    }

                    int t = 0;
                    foreach (Projectile proj in Main.projectile)
                    {
                        if (proj.active && proj.type == ModContent.ProjectileType<NanoProbe>() && proj.owner == Projectile.owner)
                        {
                            proj.ai[0] = t;
                            t++;
                        }
                    }



                    if (owner.GetModPlayer<NanoPlayer>().NanoCoreMode == 0)              //blade
                    {
                        if (AllSet(owner))
                        {
                            if (!NPCUtils.AnyProj(ModContent.ProjectileType<NanoBlade>(), owner.whoAmI))
                            {
                                Projectile.NewProjectile(owner.GetSource_ItemUse(owner.HeldItem), owner.Center, Vector2.Zero, ModContent.ProjectileType<NanoBlade>(), (int)(1.5 * Projectile.damage * MeleeDamageModifier), Projectile.knockBack, owner.whoAmI);
                            }
                        }
                    }

                    if (owner.GetModPlayer<NanoPlayer>().NanoCoreMode == 1)            //bow
                    {
                        if (AtkTimer > 0) AtkTimer--;
                        if (AllSet(owner))
                        {

                            if (AtkTimer == 0)
                            {
                                AtkTimer = 6;

                                SoundEngine.PlaySound(SoundID.Item75, Projectile.Center);
                                bool Consume = Main.rand.NextBool(4);
                                int damage = 0, type = 0, usedAmmoItemId = 0;
                                float kb = 0, speed = 0;
                                bool cs = true;
                                owner.PickAmmo(owner.HeldItem, out type, out speed, out damage, out kb, out usedAmmoItemId, !Consume);
                                speed *= 4;
                                speed += 32;
                                if (owner.archery)
                                {
                                    damage = (int)(damage * 1.2f);
                                }
                                if (owner.magicQuiver)
                                {
                                    damage = (int)(damage * 1.1f);
                                    speed = (int)(speed * 1.1f);
                                }
                                if (Main.rand.NextBool(4))
                                {
                                    type = ModContent.ProjectileType<PlasmaArrow>();
                                    damage = (int)(damage * 2f);
                                    speed = 3;
                                }
                                damage = (int)(damage / 2.5f);
                                damage = (int)(damage * RangedDamageModifier);
                                if (cs)
                                {
                                    Projectile.NewProjectile(owner.GetSource_ItemUse(owner.HeldItem), owner.Center + Main.rand.NextVector2Circular(8, 8) + (Projectile.rotation + MathHelper.Pi / 2).ToRotationVector2() * 15 + Projectile.rotation.ToRotationVector2() * 35, Projectile.rotation.ToRotationVector2() * speed * 0.8f, type, damage, kb, owner.whoAmI);
                                    Projectile.NewProjectile(owner.GetSource_ItemUse(owner.HeldItem), owner.Center + Main.rand.NextVector2Circular(8, 8) + (Projectile.rotation - MathHelper.Pi / 2).ToRotationVector2() * 15 + Projectile.rotation.ToRotationVector2() * 35, Projectile.rotation.ToRotationVector2() * speed * 0.8f, type, damage, kb, owner.whoAmI);
                                    Projectile.NewProjectile(owner.GetSource_ItemUse(owner.HeldItem), owner.Center + Main.rand.NextVector2Circular(8, 8) + Projectile.rotation.ToRotationVector2() * 35, Projectile.rotation.ToRotationVector2() * speed, type, (int)(damage * 1.25f), kb, owner.whoAmI);
                                }
                            }

                        }
                    }


                    if (owner.GetModPlayer<NanoPlayer>().NanoCoreMode == 2)            //laser cannon
                    {
                        if (owner.channel)
                        {
                            if (AllSet(owner))
                            {

                                if (!owner.CheckMana(2, true))
                                {
                                    owner.channel = false;
                                    AtkTimer = 180;
                                    return;
                                }
                                owner.manaRegenDelay = 10;
                                if (!NPCUtils.AnyProj(ModContent.ProjectileType<PlasmaDeathRay>(), owner.whoAmI))
                                {
                                    Vector2 FirePos = owner.Center + Vector2.Normalize(Main.MouseWorld - owner.Center) * 130;
                                    float num1 = 0.33f;
                                    for (int i = 0; i < 9; i++)
                                    {
                                        if (Main.rand.NextFloat() >= num1)
                                        {
                                            float f = Main.rand.NextFloat() * MathHelper.TwoPi;
                                            float num2 = Main.rand.NextFloat();
                                            Dust dust = Dust.NewDustPerfect(FirePos + f.ToRotationVector2() * (110 + 200 * num2), 157, (f - MathHelper.Pi).ToRotationVector2() * (14 + 8 * num2), 0, default, 1f);  //GreenFx
                                            dust.scale = 0.9f;
                                            dust.fadeIn = 1.15f + num2 * 0.3f;
                                            dust.noGravity = true;
                                            dust.customData = owner;
                                        }
                                    }

                                }

                                if (AtkTimer > 0) AtkTimer--;
                                if (AtkTimer == 0)
                                {
                                    AtkTimer = 180;

                                    if (!Main.dedServ)
                                    {
                                        SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Sounds/Zombie_104"), Projectile.Center);
                                    }

                                    Projectile.NewProjectile(owner.GetSource_ItemUse(owner.HeldItem), owner.Center, Vector2.Zero, ModContent.ProjectileType<PlasmaDeathRay>(), (int)(Projectile.damage * 2.5 * MagicDamageModifier), Projectile.knockBack, owner.whoAmI);
                                }
                            }

                            foreach (Dust dust1 in Main.dust)
                            {
                                if (dust1.active && dust1.type == 157)
                                {
                                    if (dust1.customData != null && dust1.customData is Player player)
                                    {
                                        dust1.position += player.position - player.oldPosition;
                                    }
                                }
                            }
                        }
                        else
                        {
                            AtkTimer = 120;
                        }
                    }


                    if (owner.GetModPlayer<NanoPlayer>().NanoCoreMode == 3)               //bombing
                    {
                        if (owner.channel)
                        {
                            if (AllSet(owner))
                            {
                                /*
								if (!owner.CheckMana(2, true)) 
                                {
									owner.channel = false;
									AtkTimer = 0;
									return;
                                }
								*/

                                AtkTimer = (AtkTimer + 1) % 30;
                                if (AtkTimer % 10 == 3)
                                {
                                    foreach (Projectile proj in Main.projectile)
                                    {
                                        if (proj.active && proj.type == ModContent.ProjectileType<NanoProbe>() && proj.owner == owner.whoAmI)
                                        {
                                            if (proj.ai[0] == AtkTimer / 5 || proj.ai[0] == AtkTimer / 5 + 1 || proj.ai[0] == 6)
                                            {
                                                SoundEngine.PlaySound(SoundID.Item91, proj.Center);
                                                Projectile.NewProjectile(owner.GetSource_ItemUse(owner.HeldItem), proj.Center, proj.rotation.ToRotationVector2().RotatedByRandom(MathHelper.ToRadians(2)) * 30, ModContent.ProjectileType<PlasmaProj>(), (int)(proj.damage * 0.5f * SummonDamageModifier), proj.knockBack, owner.whoAmI);
                                            }

                                        }
                                    }
                                }

                            }
                        }
                        else
                        {
                            AtkTimer = 0;
                        }
                    }

                }
                else
                {
                    Projectile.Kill();
                }
            }
            else
            {
                Projectile.Kill();
            }
        }


        public bool AllSet(Player owner)
        {
            bool channel = owner.channel;
            bool anyp = NPCUtils.AnyProj(ModContent.ProjectileType<NanoProbe>(), owner.whoAmI);
            bool Allset = true;
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && proj.type == ModContent.ProjectileType<NanoProbe>() && proj.owner == owner.whoAmI && proj.ai[1] == 0)
                {
                    Allset = false;
                }
            }
            return channel && anyp && Allset;
        }
    }
}