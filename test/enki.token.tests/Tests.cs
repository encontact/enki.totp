using Xunit;
using enki.totp;
using System;

namespace enki.token.tests
{
    public class TotpTests
    {
        // [Fact]
        // public void FromBase32StringTest()
        // {
        //     byte[] data =
        //     {
        //         0xec, 0x02, 0x9b, 0x80, 0x27, 0x9d, 0x19, 0x25, 0x86, 0x03, 0x97, 0xb5, 0x65, 0x4c, 0x8c,
        //         0x6d, 0x0b, 0xb8, 0x2e, 0x63
        //     };
        //     var database32 = "5QBJXABHTUMSLBQDS62WKTEMNUF3QLTD";
        //     var dut = new Totp(database32);
        //     CollectionAssert.Equal(data, dut.key);
        // }

        [Fact]
        public void getCodeTest()
        {
            var secret = "GEZDGNBVGY3TQOJQGEZDGNBVGY3TQOJQ";
            var dut = new Totp(secret, 30, 8);
            Assert.Equal(94287082, dut.getCode(59));
            Assert.Equal(7081804, dut.getCode(1111111109));
            Assert.Equal(14050471, dut.getCode(1111111111));
            Assert.Equal(89005924, dut.getCode(1234567890));
            Assert.Equal(69279037, dut.getCode(2000000000));
            Assert.Equal(65353130, dut.getCode(20000000000));
        }

        [Fact]
        public void getCodeStringTest()
        {
            var secret = "GEZDGNBVGY3TQOJQGEZDGNBVGY3TQOJQ";
            var dut = new Totp(secret, 30, 8);
            Assert.Equal("94287082", dut.getCodeString(59));
            Assert.Equal("07081804", dut.getCodeString(1111111109));
            Assert.Equal("14050471", dut.getCodeString(1111111111));
            Assert.Equal("89005924", dut.getCodeString(1234567890));
            Assert.Equal("69279037", dut.getCodeString(2000000000));
            Assert.Equal("65353130", dut.getCodeString(20000000000));

            dut = new Totp(secret, 30, 6);
            Assert.Equal("287082", dut.getCodeString(59));
            Assert.Equal("081804", dut.getCodeString(1111111109));
            Assert.Equal("050471", dut.getCodeString(1111111111));
            Assert.Equal("005924", dut.getCodeString(1234567890));
            Assert.Equal("279037", dut.getCodeString(2000000000));
            Assert.Equal("353130", dut.getCodeString(20000000000));
        }

        // ESTA FALHANDO NA MAQUINA DE BUILD.
        [Fact]
        public void DeveGerarEValidarTokenComDataDeExpiracao()
        {
           var manager = new TokenManager("GEZDGNBVGY3TQOJQGEZDGNBVGY3TQOJQ");
           var hash = manager.GetHashWithExpireOn(DateTime.UtcNow.AddMinutes(5));
           Assert.True(manager.IsValidToken(hash));
        }

        [Fact]
        public void DeveGerarERecuperarTimestamp()
        {
            var date = DateTime.UtcNow;
            var timestamp = TokenManager.GetGMTUnixEpochNow(date);
            Assert.Equal(timestamp, TokenManager.GetGMTUnixEpochNow(date));
            var recoveredDate = TokenManager.FromUnixEpochMilliseconds(timestamp);
            Assert.Equal(date.Year, recoveredDate.Year);
            Assert.Equal(date.Month, recoveredDate.Month);
            Assert.Equal(date.Day, recoveredDate.Day);
            Assert.Equal(date.Hour, recoveredDate.Hour);
            Assert.Equal(date.Minute, recoveredDate.Minute);
        }
    }
}
