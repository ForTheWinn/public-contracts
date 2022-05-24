using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;
using System.Numerics;

namespace FTWContracts
{
    public partial class FTWSmithNep17
    {
        public static void _deploy(object data, bool update)
        {
            if (update) return;
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
            Assert(newOwner != null && newOwner.IsValid, "The argument \"newOwner\" is invalid.");
            IsContractOwner();
            Storage.Put(Storage.CurrentContext, Prefix_Owner, newOwner);
        }

        public static void WithdrawNEP17(UInt160 contractHash)
        {
            IsContractOwner();
            BigInteger balance = (BigInteger)Contract.Call(contractHash, "balanceOf", CallFlags.ReadOnly, new object[] { Runtime.ExecutingScriptHash });
            var contractOwner = ContractOwner();
            Safe17Transfer(contractHash, Runtime.ExecutingScriptHash, contractOwner, balance);
        }

        public static void WithdrawNEP11(UInt160 contractHash, ByteString tokenId)
        {
            IsContractOwner();
            var contractOwner = ContractOwner();
            Safe11Transfer(contractHash, contractOwner, tokenId);
        }

        private static void IsContractOwner()
        {
            var contractOwner = (UInt160)Storage.Get(Storage.CurrentContext, Prefix_Owner);
            Assert(Runtime.CheckWitness(contractOwner), "No permission.");
        }
    }
}
