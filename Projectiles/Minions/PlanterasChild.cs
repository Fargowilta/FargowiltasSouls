using FargowiltasSouls.Buffs.Masomode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class PlanterasChild : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plantera's Child");
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            //ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 46;
            Projectile.height = 46;
            Projectile.timeLeft *= 5;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.active && !player.dead && player.GetModPlayer<FargoSoulsPlayer>().MagicalBulb)
                Projectile.timeLeft = 2;

            NPC minionAttackTargetNpc = Projectile.OwnerMinionAttackTargetNPC;
            if (minionAttackTargetNpc != null && Projectile.ai[0] != minionAttackTargetNpc.whoAmI && minionAttackTargetNpc.CanBeChasedBy())
            {
                Projectile.ai[0] = minionAttackTargetNpc.whoAmI;
                Projectile.netUpdate = true;
            }

            if (Projectile.ai[0] >= 0 && Projectile.ai[0] < Main.maxNPCs) //has target
            {
                NPC npc = Main.npc[(int)Projectile.ai[0]];

                if (npc.CanBeChasedBy())
                {
                    Vector2 target = npc.Center - Projectile.Center;
                    float length = target.Length();
                    if (length > 1000f) //too far, lose target
                    {
                        Projectile.ai[0] = -1f;
                        Projectile.netUpdate = true;
                    }
                    else if (length > 50f)
                    {
                        target.Normalize();
                        target *= 16f;
                        Projectile.velocity = (Projectile.velocity * 40f + target) / 41f;
                    }

                    Projectile.localAI[0]++;
                    if (Projectile.localAI[0] > 15f) //shoot seed/spiky ball
                    {
                        Projectile.localAI[0] = 0f;
                        if (Projectile.owner == Main.myPlayer)
                        {
                            Vector2 speed = Projectile.velocity;
                            speed.Normalize();
                            speed *= 17f;
                            float damage = Projectile.originalDamage * 2f / 3f;
                            int type;
                            if (Main.rand.NextBool())
                            {
                                damage = damage * 5f / 4f;
                                type = ModContent.ProjectileType<PoisonSeedPlanterasChild>();
                                SoundEngine.PlaySound(SoundID.Item17, Projectile.position);
                            }
                            else if (Main.rand.NextBool(6))
                            {
                                damage = damage * 3f / 2f;
                                type = ModContent.ProjectileType<SpikyBallPlanterasChild>();
                            }
                            else
                            {
                                type = ModContent.ProjectileType<SeedPlanterasChild>();
                                SoundEngine.PlaySound(SoundID.Item17, Projectile.position);
                            }

                            if (Projectile.owner == Main.myPlayer)
                            {
                                FargoSoulsUtil.NewSummonProjectile(Projectile.GetSource_FromThis(),
                                    Projectile.Center, speed, type, (int)damage, Projectile.knockBack, Projectile.owner);
                            }
                        }
                    }
                }
                else //forget target
                {
                    Projectile.ai[0] = -1f;
                    Projectile.netUpdate = true;
                }
            }
            else //no target
            {
                Vector2 target = player.Center - Projectile.Center;
                target.Y -= 50f;
                float length = target.Length();
                if (length > 2000f)
                {
                    Projectile.Center = player.Center;
                    Projectile.ai[0] = -1f;
                    Projectile.netUpdate = true;
                }
                else if (length > 70f)
                {
                    target.Normalize();
                    target *= (length > 200f) ? 10f : 6f;
                    Projectile.velocity = (Projectile.velocity * 40f + target) / 41f;
                }

                Projectile.localAI[1]++;
                if (Projectile.localAI[1] > 6f)
                {
                    Projectile.localAI[1] = 0f;
                    Projectile.ai[0] = FargoSoulsUtil.FindClosestHostileNPCPrioritizingMinionFocus(Projectile, 1000, true, player.Center);
                    Projectile.netUpdate = true;
                }
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Infested>(), 360);
            target.AddBuff(BuffID.Venom, 360);
            target.AddBuff(BuffID.Poisoned, 360);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}