using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.Numerics;

namespace FTWContracts
{
    public class Metadata : Nep11TokenState
    {
        public string Image;
        public string Description;
        public Map<string, object> Attributes;
    }

    [SupportedStandards("NEP-11")]
    [ContractPermission("*", "*")]
    public class FTWSmithNep11 : Nep11Token<Metadata>
    {
        protected const string Prefix_Owner = "owner";
        protected const string Prefix_Symbol = "symbol";

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
        public override Map<string, object> Properties(ByteString tokenId)
        {
            StorageMap tokenMap = new(Storage.CurrentContext, Prefix_Token);
            Metadata meta = (Metadata)StdLib.Deserialize(tokenMap[tokenId]);
            Map<string, object> map = new();
            map["owner"] = meta.Owner;
            map["name"] = meta.Name;
            map["image"] = meta.Image;
            map["description"] = meta.Description;
            map["attributes"] = meta.Attributes;
            return map;
        }
     
        public static void Init(UInt160 contractOwner, string symbol)
        {
            if (Storage.Get(Storage.CurrentContext, Prefix_Owner) != null) throw new Exception("Contract already initiated.");
            if (!contractOwner.IsValid) throw new Exception("Not valid owner hash.");
          
            Storage.Put(Storage.CurrentContext, Prefix_Owner, contractOwner);
            Storage.Put(Storage.CurrentContext, Prefix_Symbol, symbol);
        }

        public static void MintNFT(string name, string description, string image, string attributes)
        {
            UInt160 contractOwner = (UInt160)Storage.Get(Storage.CurrentContext, Prefix_Owner);
            if (!Runtime.CheckWitness(contractOwner)) throw new Exception("No permission.");

            BigInteger tokenNo = TotalSupply() + 1;

            Metadata meta = new Metadata
            {
                Owner = contractOwner,
                Name = name,
                Image = image,
                Description = description,
                Attributes = (Map<string, object>)StdLib.JsonDeserialize(attributes)
            };

            string tokenId = tokenNo.ToString();
            Mint(tokenId, meta);
        }

        public static void _deploy(object data, bool update)
        {
            if (update) return;
        }

        public static void Update(ByteString nefFile, string manifest)
        {
            var contractOwner = (UInt160)Storage.Get(Storage.CurrentContext, Prefix_Owner);
            if (!Runtime.CheckWitness(contractOwner)) throw new Exception("No permission.");
            ContractManagement.Update(nefFile, manifest, null);
        }

        public static void OnNEP11Payment(UInt160 from, BigInteger amount, object data)
        {
            Runtime.Log("Thanks!");
        }

        public static void WithdrawFee(UInt160 contractHash)
        {
            var contractOwner = (UInt160)Storage.Get(Storage.CurrentContext, Prefix_Owner);
            if (!Runtime.CheckWitness(contractOwner)) throw new Exception("No permission.");

            BigInteger balance = (BigInteger)Contract.Call(contractHash, "balanceOf", CallFlags.ReadOnly, new object[] { Runtime.ExecutingScriptHash });
            Contract.Call(contractHash, "transfer", CallFlags.All, new object[] { Runtime.ExecutingScriptHash, contractOwner, balance, 1 });
        }
    }
}
