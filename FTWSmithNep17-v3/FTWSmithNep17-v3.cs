using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using System.Numerics;

namespace FTWContracts
{
    [SupportedStandards("NEP-17")]
    [ContractPermission("*", "*")]
    public partial class FTWSmithNep17 : Nep17Token
    {
        protected const string Prefix_Symbol = "symbol";
        protected const string Prefix_Decimals = "decimals";
        protected const string Prefix_Version = "version";
        protected const string VERSION = "v3";

        [Safe]
        public override string Symbol()
        {
            return (string)Storage.Get(Storage.CurrentContext, Prefix_Symbol.ToByteArray());
        }

        [Safe]
        public override byte Decimals()
        {
            var decimals = (BigInteger)Storage.Get(Storage.CurrentContext, Prefix_Decimals.ToByteArray());
            return decimals.ToByte();
        }

        [Safe]
        public static string Version() => VERSION;

        public static void Init(UInt160 contractOwner, string symbol, int decimals, BigInteger totalSupply)
        {
            if(Storage.Get(Storage.CurrentContext, Prefix_Symbol.ToByteArray()) != null){
                ExecutionEngine.Assert(false, "Contract already initiated.");
            }

            Storage.Put(Storage.CurrentContext, Prefix_Symbol.ToByteArray(), symbol);
            Storage.Put(Storage.CurrentContext, Prefix_Decimals.ToByteArray(), decimals);
            Storage.Put(Storage.CurrentContext, Prefix_Version.ToByteArray(), VERSION.ToByteArray());
            BigInteger _decimals = BigInteger.Pow(10, decimals);
            Mint(contractOwner, totalSupply * _decimals);
        }

        public static void _deploy(object data, bool update)
        {
            if (update) return;
        }
    }
}
