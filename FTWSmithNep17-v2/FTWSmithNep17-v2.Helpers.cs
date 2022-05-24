using System;
using System.Numerics;
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;

namespace FTWContracts
{
    public partial class FTWSmithNep17
    {
        private static void Assert(bool condition, string message, object data = null)
        {
            if (!condition)
            {
                OnFault(message, data);
                ExecutionEngine.Assert(false, message);
            }
        }
        private static void Safe17Transfer(UInt160 tokenHash, UInt160 from, UInt160 to, BigInteger amount)
        {
            try
            {
                var result = (bool)Contract.Call(tokenHash, "transfer", CallFlags.All, new object[] { from, to, amount, null });
                Assert(result, "NEP17 transfer failed.");
            }
            catch (Exception)
            {
                Assert(false, "NEP17 transfer failed.", tokenHash);
            }
        }

        private static void Safe11Transfer(UInt160 tokenHash, UInt160 to, ByteString tokenId)
        {
            try
            {
                var result = (bool)Contract.Call(tokenHash, "transfer", CallFlags.All, new object[] { to, tokenId, null });
                Assert(result, "NEP11 transfer failed.", tokenHash);
            }
            catch (Exception)
            {
                Assert(false, "NEP11 transfer failed.", tokenHash);
            }
        }
    }
}
