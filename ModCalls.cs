using FargowiltasSouls.Content.Bosses.AbomBoss;
using FargowiltasSouls.Content.Bosses.DeviBoss;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Content.Items.Accessories.Masomode;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.Systems;
using Luminance.Core.ModCalls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static FargowiltasSouls.FargowiltasSouls;

namespace FargowiltasSouls
{
    internal sealed class EternityCall : ModCall
    {
        public override IEnumerable<string> GetCallCommands()
        {
            yield return "Emode";
            yield return "EMode";
            yield return "EternityMode";
        }
        public override IEnumerable<Type> GetInputTypes() => null;
        protected override object SafeProcess(params object[] argsWithoutCommand)
        {
            return WorldSavingSystem.EternityMode;
        }
    }
    internal sealed class EternityVanillaBossBehaviourCall : ModCall
    {
        public override IEnumerable<string> GetCallCommands()
        {
            yield return "EternityVanillaBossBehaviour";
        }
        public override IEnumerable<Type> GetInputTypes()
        {
            yield return typeof(bool);
        }
        protected override object SafeProcess(params object[] argsWithoutCommand)
        {
            bool BehaviourWasOn = WorldSavingSystem.EternityVanillaBehaviour;
            bool? arg = argsWithoutCommand[0] as bool?;
            if (arg != null)
            {
                WorldSavingSystem.EternityVanillaBehaviour = (bool)arg;
            }
            return BehaviourWasOn;
        }
    }
    internal sealed class MasoCall : ModCall
    {
        public override IEnumerable<string> GetCallCommands()
        {
            yield return "Masomode";
            yield return "MasoMode";
            yield return "MasochistMode";
            yield return "ForgottenMode";
            yield return "Forgor";
            yield return "ForgorMode";
            yield return "MasomodeReal";
            yield return "MasoModeReal";
            yield return "MasochistModeReal";
            yield return "RealMode";
            yield return "GetReal";
        }
        public override IEnumerable<Type> GetInputTypes() => null;
        protected override object SafeProcess(params object[] argsWithoutCommand)
        {
            return WorldSavingSystem.EternityMode;
        }
    }
    internal sealed class DownedMutantCall : ModCall
    {
        public override IEnumerable<string> GetCallCommands()
        {
            yield return "DownedMutant";
        }
        public override IEnumerable<Type> GetInputTypes() => null;
        protected override object SafeProcess(params object[] argsWithoutCommand)
        {
            return WorldSavingSystem.DownedMutant;
        }
    }
    internal sealed class DownedAbomCall : ModCall
    {
        public override IEnumerable<string> GetCallCommands()
        {
            yield return "DownedAbom";
            yield return "DownedAbominationn";
        }
        public override IEnumerable<Type> GetInputTypes() => null;
        protected override object SafeProcess(params object[] argsWithoutCommand)
        {
            return WorldSavingSystem.DownedAbom;
        }
    }
    internal sealed class DownedChampCall : ModCall
    {
        public override IEnumerable<string> GetCallCommands()
        {
            yield return "DownedChamp";
            yield return "DownedAbominationn";
        }
        public override IEnumerable<Type> GetInputTypes()
        {
            yield return typeof(string);
        }
        protected override object SafeProcess(params object[] argsWithoutCommand)
        {
            return WorldSavingSystem.DownedBoss[(int)Enum.Parse<WorldSavingSystem.Downed>(argsWithoutCommand[1] as string, true)];
        }
    }
    internal sealed class DownedEridanusCall : ModCall
    {
        public override IEnumerable<string> GetCallCommands()
        {
            yield return "DownedEri";
            yield return "DownedEridanus";
            yield return "DownedCosmos";
            yield return "DownedCosmosChamp";
            yield return "DownedCosmosChampion";
        }
        public override IEnumerable<Type> GetInputTypes() => null;
        protected override object SafeProcess(params object[] argsWithoutCommand)
        {
            return WorldSavingSystem.DownedBoss[(int)WorldSavingSystem.Downed.CosmosChampion];
        }
    }
    internal sealed class DownedDeviCall : ModCall
    {
        public override IEnumerable<string> GetCallCommands()
        {
            yield return "DownedDevi";
            yield return "DownedDeviantt";
        }
        public override IEnumerable<Type> GetInputTypes() => null;
        protected override object SafeProcess(params object[] argsWithoutCommand)
        {
            return WorldSavingSystem.DownedDevi;
        }
    }
    internal sealed class DownedFishEXCall : ModCall
    {
        public override IEnumerable<string> GetCallCommands()
        {
            yield return "DownedFishronEX";
        }
        public override IEnumerable<Type> GetInputTypes() => null;
        protected override object SafeProcess(params object[] argsWithoutCommand)
        {
            return WorldSavingSystem.DownedFishronEX;
        }
    }
    internal sealed class PureHeartCall : ModCall
    {
        public override IEnumerable<string> GetCallCommands()
        {
            yield return "PureHeart";
        }
        public override IEnumerable<Type> GetInputTypes() => null;
        protected override object SafeProcess(params object[] argsWithoutCommand)
        {
            return Main.LocalPlayer.FargoSouls().PureHeart;
        }
    }
    internal sealed class MutantAntibodiesCall : ModCall
    {
        public override IEnumerable<string> GetCallCommands()
        {
            yield return "MutantAntibodies";
        }
        public override IEnumerable<Type> GetInputTypes() => null;
        protected override object SafeProcess(params object[] argsWithoutCommand)
        {
            return Main.LocalPlayer.FargoSouls().MutantAntibodies;
        }
    }
    internal sealed class SinisterIconCall : ModCall
    {
        public override IEnumerable<string> GetCallCommands()
        {
            yield return "SinisterIcon";
        }
        public override IEnumerable<Type> GetInputTypes() => null;
        protected override object SafeProcess(params object[] argsWithoutCommand)
        {
            return Main.LocalPlayer.HasEffect<SinisterIconEffect>();
        }
    }
    internal sealed class AbomAliveCall : ModCall
    {
        public override IEnumerable<string> GetCallCommands()
        {
            yield return "AbomAlive";
        }
        public override IEnumerable<Type> GetInputTypes() => null;
        protected override object SafeProcess(params object[] argsWithoutCommand)
        {
            return FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.abomBoss, ModContent.NPCType<AbomBoss>());
        }
    }
    internal sealed class MutantAliveCall : ModCall
    {
        public override IEnumerable<string> GetCallCommands()
        {
            yield return "MutantAlive";
        }
        public override IEnumerable<Type> GetInputTypes() => null;
        protected override object SafeProcess(params object[] argsWithoutCommand)
        {
            return FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.mutantBoss, ModContent.NPCType<MutantBoss>());
        }
    }
    internal sealed class DeviAliveCall : ModCall
    {
        public override IEnumerable<string> GetCallCommands()
        {
            yield return "DeviAlive";
            yield return "DeviBossAlive";
            yield return "DevianttAlive";
            yield return "DevianttBossAlive";
        }
        public override IEnumerable<Type> GetInputTypes() => null;
        protected override object SafeProcess(params object[] argsWithoutCommand)
        {
            return FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.deviBoss, ModContent.NPCType<DeviBoss>());
        }
    }
    internal sealed class MutantPactCall : ModCall
    {
        public override IEnumerable<string> GetCallCommands()
        {
            yield return "MutantPact";
            yield return "MutantsPact";
            yield return "MutantCreditCard";
            yield return "MutantsCreditCard";
        }
        public override IEnumerable<Type> GetInputTypes() => null;
        protected override object SafeProcess(params object[] argsWithoutCommand)
        {
            return Main.LocalPlayer.FargoSouls().MutantsCreditCard;
        }
    }
    internal sealed class MutantDiscountCardCall : ModCall
    {
        public override IEnumerable<string> GetCallCommands()
        {
            yield return "MutantDiscountCard";
            yield return "MutantsDiscountCard";
        }
        public override IEnumerable<Type> GetInputTypes() => null;
        protected override object SafeProcess(params object[] argsWithoutCommand)
        {
            return Main.LocalPlayer.FargoSouls().MutantsDiscountCard;
        }
    }
    internal sealed class NekomiArmorCall : ModCall
    {
        public override IEnumerable<string> GetCallCommands()
        {
            yield return "NekomiArmor";
            yield return "NekomiArmour";
        }
        public override IEnumerable<Type> GetInputTypes() => null;
        protected override object SafeProcess(params object[] argsWithoutCommand)
        {
            return Main.LocalPlayer.FargoSouls().NekomiSet;
        }
    }
    internal sealed class EridanusArmorCall : ModCall
    {
        public override IEnumerable<string> GetCallCommands()
        {
            yield return "EridanusArmor";
            yield return "EridanusArmour";
        }
        public override IEnumerable<Type> GetInputTypes() => null;
        protected override object SafeProcess(params object[] argsWithoutCommand)
        {
            return Main.LocalPlayer.FargoSouls().EridanusSet;
        }
    }
    internal sealed class StyxArmorCall : ModCall
    {
        public override IEnumerable<string> GetCallCommands()
        {
            yield return "StyxArmor";
            yield return "StyxArmour";
        }
        public override IEnumerable<Type> GetInputTypes() => null;
        protected override object SafeProcess(params object[] argsWithoutCommand)
        {
            return Main.LocalPlayer.FargoSouls().StyxSet;
        }
    }
    internal sealed class MutantArmorCall : ModCall
    {
        public override IEnumerable<string> GetCallCommands()
        {
            yield return "MutantArmor";
            yield return "MutantArmour";
        }
        public override IEnumerable<Type> GetInputTypes() => null;
        protected override object SafeProcess(params object[] argsWithoutCommand)
        {
            return Main.LocalPlayer.FargoSouls().MutantSetBonusItem != null;
        }
    }
    internal sealed class GiftsReceivedCall : ModCall
    {
        public override IEnumerable<string> GetCallCommands()
        {
            yield return "GiftsReceived";
        }
        public override IEnumerable<Type> GetInputTypes() => null;
        protected override object SafeProcess(params object[] argsWithoutCommand)
        {
            return Main.LocalPlayer.FargoSouls().ReceivedMasoGift;
        }
    }
    internal sealed class GiveDevianttGiftsCall : ModCall
    {
        public override IEnumerable<string> GetCallCommands()
        {
            yield return "GiveDevianttGifts";
        }
        public override IEnumerable<Type> GetInputTypes() => null;
        protected override object SafeProcess(params object[] argsWithoutCommand)
        {
            Main.LocalPlayer.FargoSouls().ReceivedMasoGift = true;
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                DropDevianttsGift(Main.LocalPlayer);
            }
            else if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                var netMessage = Instance.GetPacket(); // Broadcast item request to server
                netMessage.Write((byte)PacketID.RequestDeviGift);
                netMessage.Write((byte)Main.LocalPlayer.whoAmI);
                netMessage.Send();
            }
            Main.npcChatText = Language.GetTextValue("Mods.Fargowiltas.NPCs.Deviantt.Chat.GiveGifts"); // mutant mod entry
            return default;
        }
    }
    internal sealed class SummonCritCall : ModCall
    {
        public override IEnumerable<string> GetCallCommands()
        {
            yield return "SummonCrit";
            yield return "SummonCritChance";
            yield return "GetSummonCrit";
            yield return "GetSummonCritChance";
            yield return "SummonerCrit";
            yield return "SummonerCritChance";
            yield return "GetSummonerCrit";
            yield return "GetSummonerCritChance";
            yield return "MinionCrit";
            yield return "MinionCritChance";
            yield return "GetMinionCrit";
            yield return "GetMinionCritChance";
        }
        public override IEnumerable<Type> GetInputTypes() => null;
        protected override object SafeProcess(params object[] argsWithoutCommand)
        {
            return (int)Main.LocalPlayer.ActualClassCrit(DamageClass.Summon);
        }
    }
}
