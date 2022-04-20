using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.Numerics;

namespace FTWContracts
{
    [SupportedStandards("NEP-17")]
    [ContractPermission("*", "*")]
    public class FTWSmithNep17 : Nep17Token
    {
        protected const string Prefix_Owner = "owner";
        protected const string Prefix_Symbol = "symbol";
        protected const string Prefix_Name = "name";
        protected const string Prefix_Author = "author";
        protected const string Prefix_Description = "description";
        protected const string Prefix_Decimals = "decimals";
        protected const string Prefix_Logo = "logo";
        protected const string Prefix_Website = "website";

        public override byte Decimals()
        {
            BigInteger decimals = (BigInteger)Storage.Get(Storage.CurrentContext, Prefix_Decimals);
            return decimals.ToByte();
        }

        [Safe]
        public static UInt160 Owner()
        {
            return (UInt160)Storage.Get(Storage.CurrentContext, Prefix_Owner);
        }

        [Safe]
        public override string Symbol()
        {
            return (string)Storage.Get(Storage.CurrentContext, Prefix_Symbol);
        }

        [Safe]
        public static string Name()
        {
            return (string)Storage.Get(Storage.CurrentContext, Prefix_Name);
        }

        [Safe]
        public static string Author()
        {
            return (string)Storage.Get(Storage.CurrentContext, Prefix_Author);
        }

        [Safe]
        public static string Description()
        {
            return (string)Storage.Get(Storage.CurrentContext, Prefix_Description);
        }

        [Safe]
        public static string Logo()
        {
            return (string)Storage.Get(Storage.CurrentContext, Prefix_Logo);
        }

        [Safe]
        public static string Website()
        {
            return (string)Storage.Get(Storage.CurrentContext, Prefix_Website);
        }

        public static void Init(UInt160 contractOwner, string name, string author, string description, string symbol, BigInteger totalSupply, int decimals)
        {
            if (Storage.Get(Storage.CurrentContext, Prefix_Owner) != null) throw new Exception("Contract already initiated.");
            Storage.Put(Storage.CurrentContext, Prefix_Owner, contractOwner);
            Storage.Put(Storage.CurrentContext, Prefix_Symbol, symbol);
            Storage.Put(Storage.CurrentContext, Prefix_Name, name);
            Storage.Put(Storage.CurrentContext, Prefix_Author, author);
            Storage.Put(Storage.CurrentContext, Prefix_Description, description);
            Storage.Put(Storage.CurrentContext, Prefix_Decimals, decimals);
            BigInteger _decimals = BigInteger.Pow(10, decimals);
            Mint(contractOwner, totalSupply * _decimals);
        }
        
        public static bool Blackhole(UInt160 from, BigInteger amount)
        {
            if (from is null || !from.IsValid)
                throw new Exception("The argument \"from\" is invalid.");

            if (!Runtime.CheckWitness(from)) return false;

            Burn(from, amount);
            return true;
        }

        public static void _deploy(object data, bool update)
        {
            if (update) return;
        }

        public static void Update(ByteString nefFile, string manifest)
        {
            IsContractOwner();
            ContractManagement.Update(nefFile, manifest, null);
        }

        public static void UpdateWebsite(string val)
        {
            IsContractOwner();
            Storage.Put(Storage.CurrentContext, Prefix_Website, val);
        }

        public static void UpdateLogo(string val)
        {
            IsContractOwner();
            Storage.Put(Storage.CurrentContext, Prefix_Logo, val);
        }

        public static void ChangeOwnership(UInt160 newOwner)
        {
            if (newOwner is null || !newOwner.IsValid)
                throw new Exception("The argument \"newOwner\" is invalid.");

            IsContractOwner();
            Storage.Put(Storage.CurrentContext, Prefix_Owner, newOwner);
        }

        public static void OnNEP17Payment(UInt160 from, BigInteger amount, object data) { }

        public static void WithdrawFee(UInt160 contractHash)
        {
            IsContractOwner();
            BigInteger balance = (BigInteger)Contract.Call(contractHash, "balanceOf", CallFlags.ReadOnly, new object[] { Runtime.ExecutingScriptHash });
            var contractOwner = (UInt160)Storage.Get(Storage.CurrentContext, Prefix_Owner);
            Contract.Call(contractHash, "transfer", CallFlags.All, new object[] { Runtime.ExecutingScriptHash, contractOwner, balance, 1 });
        }

        private static void IsContractOwner()
        {
            var contractOwner = (UInt160)Storage.Get(Storage.CurrentContext, Prefix_Owner);
            if (!Runtime.CheckWitness(contractOwner)) throw new Exception("No permission.");
        }
    }
}
