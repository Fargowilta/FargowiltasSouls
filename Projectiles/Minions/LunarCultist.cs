using FargowiltasSouls.EternityMode;
using FargowiltasSouls.EternityMode.Content.Boss.HM;
using FargowiltasSouls.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class LunarCultist : ModProjectile
    {
        Vector2 target;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lunar Cultist");
            Main.projFrames[Projectile.type] = 12;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 9;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            //ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 30;
            Projectile.height = 60;
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

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
            writer.WriteVector2(target);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
            target = reader.ReadVector2();
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.active && !player.dead && player.GetModPlayer<FargoSoulsPlayer>().LunarCultist)
                Projectile.timeLeft = 2;

            if (Projectile.ai[0] >= 0 && Projectile.ai[0] < Main.maxNPCs) //has target
            {
                NPC minionAttackTargetNpc = Projectile.OwnerMinionAttackTargetNPC;
                if (minionAttackTargetNpc != null && Projectile.ai[0] != minionAttackTargetNpc.whoAmI && minionAttackTargetNpc.CanBeChasedBy())
                    Projectile.ai[0] = minionAttackTargetNpc.whoAmI;

                Projectile.localAI[0]++;
                NPC npc = Main.npc[(int)Projectile.ai[0]];
                if (npc.CanBeChasedBy())
                {
                    if (Projectile.ai[1] % 2 != 0) //when attacking, check for emode ml
                    {
                        NPC moonLord = FargoSoulsUtil.NPCExists(NPCs.EModeGlobalNPC.moonBoss, NPCID.MoonLordCore);
                        if (moonLord != null)
                        {
                            switch (moonLord.GetGlobalNPC<MoonLordCore>().VulnerabilityState)
                            {
                                case 0: Projectile.ai[1] = 1; break;
                                case 1: Projectile.ai[1] = 3; break;
                                case 2: Projectile.ai[1] = 5; break;
                                case 3: Projectile.ai[1] = 7; break;
                                default: break;
                            }
                        }
                    }

                    Projectile.localAI[1] = Projectile.ai[1] + 1;
                    switch ((int)Projectile.ai[1])
                    {
                        case 0: //chase
                            Projectile.localAI[0] = 0f;
                            Projectile.velocity = target - Projectile.Center;
                            float length = Projectile.velocity.Length();
                            if (length > 1000f) //too far, lose target
                            {
                                Projectile.ai[0] = -1f;
                                Projectile.ai[1] = 1f;
                                Projectile.netUpdate = true;
                            }
                            else if (length > 24f)
                            {
                                Projectile.velocity.Normalize();
                                Projectile.velocity *= 24f;
                            }
                            else
                            {
                                Projectile.ai[1]++;
                            }
                            break;

                        case 1: //shoot fireballs
                            Projectile.velocity = Vector2.Zero;
                            if (Projectile.localAI[0] <= 30 && Projectile.localAI[0] % 10 == 0)
                            {
                                SoundEngine.PlaySound(SoundID.Item34, Projectile.position);
                                Vector2 spawn = Projectile.Center;
                                spawn.X -= 30 * Projectile.spriteDirection;
                                spawn.Y += 12f;
                                Vector2 vel = (npc.Center - spawn).RotatedByRandom(Math.PI / 6);
                                vel.Normalize();
                                vel *= Main.rand.NextFloat(6f, 10f);
                                if (Projectile.owner == Main.myPlayer)
                                {
                                    int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawn, vel, ModContent.ProjectileType<LunarCultistFireball>(), Projectile.damage, 9f, Projectile.owner, 0f, Projectile.ai[0]);
                                    if (p != Main.maxProjectiles)
                                        Main.projectile[p].CritChance = (int)player.ActualClassCrit(DamageClass.Melee);
                                }
                            }
                            if (Projectile.localAI[0] > 60f)
                            {
                                Projectile.ai[1]++;
                                target = npc.Center;
                                target.Y -= npc.height + 100;
                            }
                            break;

                        case 2: goto case 0;
                        case 3: //lightning orb
                            Projectile.velocity = Vector2.Zero;
                            if (Projectile.localAI[0] == 15f)
                            {
                                SoundEngine.PlaySound(SoundID.Item121, Projectile.position);
                                Vector2 spawn = Projectile.Center;
                                spawn.Y -= 100;
                                if (Projectile.owner == Main.myPlayer)
                                {
                                    int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawn, Vector2.Zero, ModContent.ProjectileType<LunarCultistLightningOrb>(), Projectile.damage, 8f, Projectile.owner, Projectile.whoAmI);
                                    if (p != Main.maxProjectiles)
                                        Main.projectile[p].CritChance = (int)player.ActualClassCrit(DamageClass.Ranged);
                                }
                            }
                            if (Projectile.localAI[0] > 90f)
                            {
                                Projectile.ai[1]++;
                                target = npc.Center;
                                target.Y -= npc.height + 100;
                            }
                            break;

                        case 4: goto case 0;
                        case 5: //ice mist
                            Projectile.velocity = Vector2.Zero;
                            if (Projectile.localAI[0] == 20f)
                            {
                                Vector2 spawn = Projectile.Center;
                                spawn.X -= 30 * Projectile.spriteDirection;
                                spawn.Y += 12f;
                                Vector2 vel = npc.Center - spawn;
                                vel.Normalize();
                                vel *= 4.25f;
                                if (Projectile.owner == Main.myPlayer)
                                {
                                    int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawn, vel, ModContent.ProjectileType<LunarCultistIceMist>(), Projectile.damage, Projectile.knockBack * 2f, Projectile.owner);
                                    if (p != Main.maxProjectiles)
                                        Main.projectile[p].CritChance = (int)player.ActualClassCrit(DamageClass.Magic);
                                }
                            }
                            if (Projectile.localAI[0] > 60f)
                            {
                                Projectile.ai[1]++;
                                target = npc.Center;
                                target.Y -= npc.height + 100;
                            }
                            break;

                        case 6: goto case 0;
                        case 7: //ancient visions
                            Projectile.velocity = Vector2.Zero;
                            if (Projectile.localAI[0] == 30f)
                            {
                                Vector2 spawn = Projectile.Center;
                                spawn.Y -= Projectile.height;
                                if (Projectile.owner == Main.myPlayer)
                                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawn, Vector2.UnitX * -Projectile.spriteDirection * 12f, ModContent.ProjectileType<AncientVisionLunarCultist>(), Projectile.damage, Projectile.knockBack * 3f, Projectile.owner);
                            }
                            if (Projectile.localAI[0] > 90f)
                            {
                                Projectile.ai[1]++;
                                target = npc.Center;
                                target.Y -= npc.height + 100;
                            }
                            break;

                        /*case 8: goto case 0;
                        case 9: //ancient light
                            Projectile.velocity = Vector2.Zero;
                            if (Projectile.localAI[0] == 30f)
                            {
                                Vector2 spawn = Projectile.Center;
                                spawn.X -= 30 * Projectile.spriteDirection;
                                spawn.Y += 12f;
                                Vector2 vel = npc.Center - spawn;
                                vel.Normalize();
                                vel *= 9f;
                                for (int i = -2; i <= 2; i++)
                                {
                                    if (Projectile.owner == Main.myPlayer)
                                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawn, vel.RotatedBy(Math.PI / 7 * i), ModContent.ProjectileType<LunarCultistLight>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, (Main.rand.NextFloat() - 0.5f) * 0.3f * 6.28318548202515f / 60f);
                                }
                            }
                            if (Projectile.localAI[0] > 60f)
                            {
                                Projectile.ai[1]++;
                                target = npc.Center;
                                target.Y -= npc.height + 100;
                            }
                            break;*/

                        default:
                            Projectile.ai[1] = 0f;
                            goto case 0;
                    }

                    if (Projectile.velocity.X == 0)
                    {
                        float distance = npc.Center.X - Projectile.Center.X;
                        if (distance != 0)
                            Projectile.spriteDirection = distance < 0 ? 1 : -1;
                    }
                    else
                    {
                        Projectile.spriteDirection = (Projectile.velocity.X < 0) ? 1 : -1;
                    }
                }
                else //forget target
                {
                    TargetEnemies();
                }
            }
            else //no target
            {
                if (Projectile.ai[1] == 0f) //follow player
                {
                    if (target == Vector2.Zero)
                    {
                        target = Main.player[Projectile.owner].Center;
                        target.Y -= 100f;
                    }

                    Projectile.velocity = target - Projectile.Center;
                    float length = Projectile.velocity.Length();
                    if (length > 1500f) //teleport when too far away
                    {
                        Projectile.Center = Main.player[Projectile.owner].Center;
                        Projectile.velocity = Vector2.Zero;
                        Projectile.ai[1] = 1f;
                    }
                    else if (length > 24f)
                    {
                        Projectile.velocity.Normalize();
                        Projectile.velocity *= 24f;
                    }
                    else //in close enough range to stop
                    {
                        Projectile.ai[1] = 1f;
                    }
                }
                else //now above player, wait
                {
                    Projectile.velocity = Vector2.Zero;

                    Projectile.localAI[0]++;
                    if (Projectile.localAI[0] > 30)
                    {
                        TargetEnemies();
                        Projectile.localAI[0] = 0f;
                    }
                }

                if (Projectile.velocity.X == 0)
                {
                    float distance = Main.player[Projectile.owner].Center.X - Projectile.Center.X;
                    if (distance != 0)
                        Projectile.spriteDirection = distance < 0 ? 1 : -1;
                }
                else
                {
                    Projectile.spriteDirection = (Projectile.velocity.X < 0) ? 1 : -1;
                }
            }

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % 3;
                if (Projectile.ai[0] > -1f && Projectile.ai[0] < 200f)
                {
                    switch ((int)Projectile.ai[1])
                    {
                        case 1: Projectile.frame += 6; break;
                        case 3: Projectile.frame += 3; break;
                        case 5: Projectile.frame += 6; break;
                        case 7: Projectile.frame += 3; break;
                        case 9: Projectile.frame += 6; break;
                        default: break;
                    }
                }
            }
        }

        private void TargetEnemies()
        {
            Projectile.ai[0] = FargoSoulsUtil.FindClosestHostileNPCPrioritizingMinionFocus(Projectile, 1000f, true, Main.player[Projectile.owner].Center);
            if (Projectile.ai[0] != -1)
            {
                target = Main.npc[(int)Projectile.ai[0]].Center;
                target.Y -= Main.npc[(int)Projectile.ai[0]].height + 100;
                Projectile.ai[1] = Projectile.localAI[1];
                if (Projectile.ai[1] % 2 != 0)
                    Projectile.ai[1]--;
            }
            else
            {
                target = Main.player[Projectile.owner].Center;
                target.Y -= Main.player[Projectile.owner].height + 100;
                Projectile.ai[1] = 0f;
            }
            Projectile.netUpdate = true;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            Texture2D texture2D14 = FargowiltasSouls.Instance.Assets.Request<Texture2D>("Projectiles/Minions/LunarCultistTrail", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i += 3)
            {
                Color color27 = color26;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D14, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
            return false;
        }
    }
}