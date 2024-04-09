using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.BanishedBaron
{

    public class BaronWhirlpool : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Banished Baron's Mine");
            Main.projFrames[Type] = 16;
        }
        public override void SetDefaults()
        {
            Projectile.width = 186;
            Projectile.height = 48;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale = 1f;
            Projectile.light = 1;
            Projectile.alpha = 255;
            Projectile.FargoSouls().DeletionImmuneRank = 1;
        }
        public bool Fade = false;
        //public bool Animate = false;

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (!WorldSavingSystem.EternityMode)
            {
                return;
            }
            //target.FargoSouls().MaxLifeReduction += 50;
            //target.AddBuff(ModContent.BuffType<OceanicMaulBuff>(), 60 * 20);
            target.AddBuff(BuffID.Rabies, 60 * 10);
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
            writer.Write(Projectile.localAI[2]);
            writer.Write(parentNPC);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
            Projectile.localAI[2] = reader.ReadSingle();
            parentNPC = reader.ReadBoolean();
        }
        bool parentNPC = false;
        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_Parent parent && parent.Entity is NPC)
            {
                parentNPC = true;
            }
        }
        public override void AI()
        {
            ref float ParentID = ref Projectile.ai[0];
            ref float Number = ref Projectile.ai[1];
            ref float Variant = ref Projectile.ai[2];
            ref float Timer = ref Projectile.localAI[0];
            ref float ChildID = ref Projectile.localAI[1];
            ref float attackRandom = ref Projectile.localAI[2];

            //Projectile.netUpdate = true; //it's choppy if this isn't done always

            if (Timer > 60 * 4)
            {
                Fade = true;
            }
            //Animate = true;
            /*
            if (parentNPC)
            {
                
                NPC parent = Main.npc[(int)ParentID];
                
                const int BaronHeight = 132 / 2;
                
                if (parent != null && parent.active && parent.type == ModContent.NPCType<BanishedBaron>())
                {
                    Projectile.Center = parent.Center + (Vector2.UnitY * ((Projectile.height / 2) + BaronHeight));
                }
                
                
                
            }
            else
            {
                Projectile parent = Main.projectile[(int)ParentID];
                if (parent != null && parent.active && parent.type == Type && !Animate)
                {
                    //Projectile.Center = parent.Center + Vector2.UnitY * Projectile.height;
                    int frame = parent.frame - 1;
                    if (frame < 0 || frame >= Main.projFrames[Type])
                    {
                        frame = Main.projFrames[Type] - 1;
                    }
                    Projectile.frame = frame; //makes it look good and connected
                }
                else
                {
                    Animate = true;
                    Fade = true;
                }
            }
            */
            const int projTime = 50;
            const int projTimeVar = 46;
            if (Projectile.alpha == 0)
            {
                int everyThird = (int)(Number % 3);
                everyThird += (int)attackRandom;
                if (Variant == 1)
                {
                    int variantTime = projTimeVar; //+ (int)attackRandom;
                    if (Timer % variantTime == 0 && Timer % (variantTime * 3) == variantTime * everyThird)
                    {
                        //attackRandom = Main.rand.NextBool() ? 1 : 0;
                        //attackRandom = Main.rand.Next(-12, 0);
                        SoundEngine.PlaySound(SoundID.Item21, Projectile.Center);
                        if (FargoSoulsUtil.HostCheck)
                        {
                            for (int i = -1; i < 2; i += 2)
                            {
                                Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center + Vector2.UnitX * i * Projectile.width * Main.rand.NextFloat(0.2f, 0.35f) + Vector2.UnitY * Main.rand.Next(-Projectile.height / 4, Projectile.height / 4), (Vector2.UnitX * i).RotatedBy(Main.rand.NextFloat(MathHelper.Pi / 24)), ModContent.ProjectileType<BaronWhirlpoolBolt>(), (int)(Projectile.damage * 0.8f), Projectile.knockBack, Projectile.owner, 1);
                            }
                        }
                    }
                }
                else
                {
                    if (Timer % projTime == 0 && Timer % (projTime * 3) == projTime * everyThird)
                    {
                        SoundEngine.PlaySound(SoundID.Item21, Projectile.Center);
                        if (FargoSoulsUtil.HostCheck)
                        {
                            for (int i = -1; i < 2; i += 2)
                            {
                                Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center + Vector2.UnitX * i * Projectile.width * Main.rand.NextFloat(0.2f, 0.35f) + Vector2.UnitY * Main.rand.Next(-Projectile.height / 4, Projectile.height / 4), (Vector2.UnitX * i).RotatedBy(Main.rand.NextFloat(MathHelper.Pi / 24)), ModContent.ProjectileType<BaronWhirlpoolBolt>(), (int)(Projectile.damage * 0.8f), Projectile.knockBack, Projectile.owner, 1);
                            }
                        }
                    }
                }
            }
            if (Timer < 20)
            {
                Projectile.alpha -= 17;
                if (Projectile.alpha <= 0)
                {
                    Projectile.alpha = 0;
                }
            }
            bool collision = WorldSavingSystem.MasochistModeReal ? false : Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height); //no collision check in maso
            if (collision && Number > 3)
            {
                Number = 3;
            }
            if (Timer == 8 + (Variant == 1 ? 5 : 0) && Number > 0 && FargoSoulsUtil.HostCheck)
            {
                ChildID = Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center + Vector2.UnitY * Projectile.height, Vector2.Zero, Type, Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.identity, Number - 1, Variant);
                int frame = Projectile.frame - 1;
                if (frame < 0 || frame >= Main.projFrames[Type])
                {
                    frame = Main.projFrames[Type] - 1;
                }
                Main.projectile[(int)ChildID].frame = frame;
            }
            if (Fade)
            {
                Projectile.alpha += 17;
                if (Projectile.alpha >= 238)
                {
                    Projectile child = Main.projectile[(int)ChildID];
                    if (child.TypeAlive(Type))
                        child.As<BaronWhirlpool>().Fade = true;

                    Projectile.Kill();
                }
            }

            if (++Projectile.frameCounter > 2)
            {
                if (++Projectile.frame >= Main.projFrames[Type])
                {
                    Projectile.frame = 0;
                }
                Projectile.frameCounter = 0;
            }
            Timer++;

        }
    }
}