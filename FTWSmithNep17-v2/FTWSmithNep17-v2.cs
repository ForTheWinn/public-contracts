using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using System.Numerics;

namespace FTWContracts
{
    [ManifestExtra("Email", "hello@forthewin.network")]
    [ManifestExtra("Author", "Forthewin Network")]
    [ManifestExtra("Description", "NEP-17 v2 for FTWSmith")]
    [SupportedStandards("NEP-17")]
    [ContractPermission("*", "*")]
    public partial class FTWSmithNep17 : Nep17Token
    {
        protected const string Prefix_Owner = "owner";
        protected const string Prefix_Symbol = "symbol";
        protected const string Prefix_Decimals = "decimals";
        protected const string Prefix_Name = "name";
        protected const string Prefix_Author = "author";
        protected const string Prefix_Description = "description";
        protected const string Prefix_Email = "email";
        protected const string Prefix_Logo = "logo";
        protected const string Prefix_Website = "website";
        protected const string Prefix_Version = "version";
        protected const string VERSION = "v2";

        public static Map<string, object> ContractInfo()
        {
            var map = new Map<string, object>();
            map["owner"] = ContractOwner();
            map["decimals"] = (BigInteger)Storage.Get(Storage.CurrentContext, Prefix_Decimals); ;
            map["symbol"] = (string)Storage.Get(Storage.CurrentContext, Prefix_Symbol);
            map["name"] = Name();
            map["author"] = Author();
            map["description"] = Description();
            map["website"] = Website();
            map["logo"] = Logo();
            map["version"] = Version();
            return map;
        }

        [Safe]
        public override string Symbol()
        {
            return (string)Storage.Get(Storage.CurrentContext, Prefix_Symbol);
        }

        public override byte Decimals()
        {
            BigInteger decimals = (BigInteger)Storage.Get(Storage.CurrentContext, Prefix_Decimals);
            return decimals.ToByte();
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

        [Safe]
        public static string Version()
        {
            return (string)Storage.Get(Storage.CurrentContext, Prefix_Version);
        }

        [Safe]
        public static UInt160 Owner()
        {
            return (UInt160)Storage.Get(Storage.CurrentContext, Prefix_Owner);
        }

        [Safe]
        public static UInt160 ContractOwner()
        {
            return (UInt160)Storage.Get(Storage.CurrentContext, Prefix_Owner);
        }

        public static void Init(UInt160 contractOwner, string symbol, int decimals, BigInteger totalSupply, string name, string author, string description, string email, string website, string logo)
        {
            Assert(ContractOwner() == null, "Contract already initiated.");
          
            Storage.Put(Storage.CurrentContext, Prefix_Owner, contractOwner);
            Storage.Put(Storage.CurrentContext, Prefix_Symbol, symbol);
            Storage.Put(Storage.CurrentContext, Prefix_Decimals, decimals);
            Storage.Put(Storage.CurrentContext, Prefix_Name, name);
            Storage.Put(Storage.CurrentContext, Prefix_Author, author);
            Storage.Put(Storage.CurrentContext, Prefix_Description, description);
            Storage.Put(Storage.CurrentContext, Prefix_Email, email);
            Storage.Put(Storage.CurrentContext, Prefix_Website, website);
            Storage.Put(Storage.CurrentContext, Prefix_Logo, logo);
            Storage.Put(Storage.CurrentContext, Prefix_Version, VERSION);
            BigInteger _decimals = BigInteger.Pow(10, decimals);
            Mint(contractOwner, totalSupply * _decimals);
        }
        
        public static bool Blackhole(UInt160 from, BigInteger amount)
        {
            Assert(from != null && from.IsValid, "The argument \"from\" is invalid.");
        
            if (!Runtime.CheckWitness(from)) return false;
        
            Burn(from, amount);
            return true;
        }

        public static void OnNEP17Payment(UInt160 from, BigInteger amount, object data) { }
        public static void OnNEP11Payment(UInt160 from, BigInteger amount, ByteString tokenId, object data) { }
    }
}
