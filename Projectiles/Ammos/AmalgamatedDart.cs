//using Terraria.ID;
//using Terraria.ModLoader;

//namespace FargowiltasSouls.Projectiles.Ammos
//{
//    public class AmalgamatedDart : ModProjectile
//    {
//        private int cursedCounter = 0;
//        private int[] dusts = new int[] { 130, 55, 133, 131, 132 };
//        private int currentDust = 0;

//        public override void SetStaticDefaults()
//        {
//            DisplayName.SetDefault("Fargo Dart");
//        }

//        public override void SetDefaults()
//        {
//            Projectile.width = 14;
//            Projectile.height = 14;
//            Projectile.aiStyle = 1;
//            Projectile.friendly = true;
//            Projectile.DamageType = DamageClass.Ranged;


//            Projectile.penetrate = -1; //same as luminite
//            Projectile.timeLeft = 600;
//            //Projectile.light = 1f;
//            Projectile.ignoreWater = true;
//            Projectile.tileCollide = true;
//            Projectile.extraUpdates = 1;


//            //Projectile.usesLocalNPCImmunity = true;
//            //Projectile.localNPCHitCooldown = 2;
//        }


//        public override void AI()
//        {
//            //dust
//            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dusts[currentDust], Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 150, default(Color), 1.2f);
//            currentDust++;
//            if (currentDust > 4)
//            {
//                currentDust = 0;
//            }

//            //all
//            Projectile.localAI[0] += 1f;
//            if (Projectile.localAI[0] > 3f)
//            {
//                Projectile.alpha = 0;
//            }
//            if (Projectile.ai[0] >= 20f)
//            {
//                Projectile.ai[0] = 20f;
//                if (Projectile.type != 477)
//                {
//                    Projectile.velocity.Y = Projectile.velocity.Y + 0.075f;
//                }
//            }
//            //crystal
//            if (Projectile.localAI[1] < 5f)
//            {
//                Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + 1.57f;
//                Projectile.localAI[1] += 1f;
//            }
//            else
//            {
//                Projectile.rotation = (Projectile.rotation * 2f + (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + 1.57f) / 3f;
//            }

//            //cursed
//            cursedCounter++;
//            if (cursedCounter > 10)
//            {
//                cursedCounter = 0;
//                Projectile.NewProjectile(Projectile.Center.X, Projectile.Center.Y, 0f, 0f, ProjectileID.CursedDartFlame, (int)((double)Projectile.damage * 0.8), Projectile.knockBack * 0.5f, Projectile.owner, 0f, 0);
//            }


//            //ichor
//            if (Projectile.ai[1] >= 0f)
//            {
//                Projectile.penetrate = -1;
//            }
//            else if (Projectile.penetrate < 0)
//            {
//                Projectile.penetrate = 1;
//            }
//            if (Projectile.ai[1] >= 0f)
//            {
//                Projectile.ai[1] += 1f;
//            }
//            if (Projectile.ai[1] > (float)Main.rand.Next(5, 30))
//            {
//                Projectile.ai[1] = -1000f;
//                float scaleFactor4 = Projectile.velocity.Length();
//                Vector2 velocity = Projectile.velocity;
//                velocity.Normalize();
//                int num194 = Main.rand.Next(5, 7);
//                if (Main.rand.NextBool(4))
//                {
//                    num194++;
//                }
//                for (int num195 = 0; num195 < num194; num195++)
//                {
//                    Vector2 vector21 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
//                    vector21.Normalize();
//                    vector21 += velocity * 2f;
//                    vector21.Normalize();
//                    vector21 *= scaleFactor4;
//                    Projectile.NewProjectile(Projectile.Center.X, Projectile.Center.Y, vector21.X, vector21.Y, Projectile.type, Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, -1000f);
//                }
//            }

//        }

//        public override bool OnTileCollide(Vector2 oldVelocity)
//        {
//            OnHit();

//            //crystal wtf
//            /*Projectile.penetrate--;
//            if (Projectile.penetrate <= 0)
//            {
//                Projectile.Kill();
//            }
//            Projectile.velocity.X = -Projectile.velocity.X;
//            Projectile.velocity.Y = -Projectile.velocity.Y;
//           /* if (Projectile.velocity.X != velocity.X)
//            {

//            }
//            if (Projectile.velocity.Y != velocity.Y)
//            {

//            }
//            /*if (Projectile.penetrate > 0 && Projectile.owner == Main.myPlayer)
//            {
//                int[] array = new int[10];
//                int num17 = 0;
//                int num18 = 700;
//                int num19 = 20;
//                for (int i = 0; i < 200; i++)
//                {
//                    if (Main.npc[i].CanBeChasedBy(Projectile, false))
//                    {
//                        float num20 = (Projectile.Center - Main.npc[i].Center).Length();
//                        if (num20 > (float)num19 && num20 < (float)num18 && Collision.CanHitLine(Projectile.Center, 1, 1, Main.npc[i].Center, 1, 1))
//                        {
//                            array[num17] = i;
//                            num17++;
//                            if (num17 >= 9)
//                            {
//                                break;
//                            }
//                        }
//                    }
//                }
//                if (num17 > 0)
//                {
//                    num17 = Main.rand.Next(num17);
//                    Vector2 value7 = Main.npc[array[num17]].Center - Projectile.Center;
//                    float scaleFactor = Projectile.velocity.Length();
//                    value7.Normalize();
//                    Projectile.velocity = value7 * scaleFactor;
//                    Projectile.netUpdate = true;
//                }
//            }*/

//            return false;
//        }

//        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
//        {
//            OnHit();

//            /*//crystal
//            int[] array = new int[10];
//            int num16 = 0;
//            int num17 = 700;
//            int num18 = 20;
//            for (int k = 0; k < 200; k++)
//            {
//                if (k != i && Main.npc[k].CanBeChasedBy(Projectile, false))
//                {
//                    float num19 = (Projectile.Center - Main.npc[k].Center).Length();
//                    if (num19 > (float)num18 && num19 < (float)num17 && Collision.CanHitLine(Projectile.Center, 1, 1, Main.npc[k].Center, 1, 1))
//                    {
//                        array[num16] = k;
//                        num16++;
//                        if (num16 >= 9)
//                        {
//                            break;
//                        }
//                    }
//                }
//            }
//            if (num16 > 0)
//            {
//                num16 = Main.rand.Next(num16);
//                Vector2 value4 = Main.npc[array[num16]].Center - Projectile.Center;
//                float scaleFactor3 = Projectile.velocity.Length();
//                value4.Normalize();
//                Projectile.velocity = value4 * scaleFactor3;
//                Projectile.netUpdate = true;
//            }*/






//            //poison
//            target.AddBuff(BuffID.Poisoned, 600);
//            //cursed
//            //target.AddBuff(BuffID.CursedFlames, 600);
//            //ichor
//            target.AddBuff(BuffID.Ichor, 600);
//        }

//        public void OnHit()
//        {

//        }

//        public override void Kill(int timeleft)
//        {
//            //cursed
//            if (Projectile.owner == Main.myPlayer)
//            {
//                Projectile.NewProjectile(Projectile.Center.X, Projectile.Center.Y, 0f, 0f, ProjectileID.CursedDartFlame, (int)((double)Projectile.damage * 0.8), Projectile.knockBack * 0.5f, Projectile.owner, 0f, 0);
//            }

//            OnHit();
//        }
//    }
//}