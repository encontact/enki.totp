using System;
using System.Security.Cryptography;

namespace enki.totp
{
    public class Totp
    {
        private DateTime unixEpochDate = new DateTime(1970, 1, 1);
        private readonly int digits = 6;
        private readonly HMACSHA1 hmac;
        private readonly Int32 t1 = 30;
        internal byte[] key;

        public Totp(string base32string)
        {
            key = FromBase32String(base32string);
            hmac = new HMACSHA1(key);
        }

        public Totp(string base32string, int timeoutSeconds, int digits) : this(base32string)
        {
            t1 = timeoutSeconds;
            this.digits = digits;
        }

        private byte[] FromBase32String(string base32string)
        {
            return Base32.FromBase32String(base32string);
        }

        public static string ToBase32String(byte[] data)
        {
            return Base32.ToBase32String(data);
        }

        public int getCode()
        {
            var unixTimestamp = (UInt64)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            return getCode(unixTimestamp);
        }

        public int getCode(UInt64 timeStamp)
        {
            var message = timeStamp / (UInt64)t1;
            var msg_byte = BitConverter.GetBytes(message);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(msg_byte, 0, msg_byte.Length);
            var hash = hmac.ComputeHash(msg_byte);
            var offset = (hash[hash.Length - 1] & 0xf);
            var i = ((hash[offset] & 0x7f) << 24) | ((hash[offset + 1] & 0xff) << 16) | ((hash[offset + 2] & 0xff) << 8) |
                (hash[offset + 3] & 0xff);
            var code = i % (int)Math.Pow(10, digits);
            return code;
        }

        public String getCodeString()
        {
            var unixTimestamp = (UInt64)(DateTime.UtcNow.Subtract(unixEpochDate)).TotalSeconds;

            return getCodeString(unixTimestamp);
        }

        public String getCodeString(UInt64 timeStamp)
        {
            var ret = getCode(timeStamp) + "";
            return ret.PadLeft(digits, '0');
        }

        public static bool CheckBase32(string base32string)
        {
            try
            {
                var tmp = Base32.FromBase32String(base32string);
                return tmp != null;
            }
            catch
            {
                return false;
            }
        }

        public int RemainingSeconds()
        {
            const long ticksToSeconds = 10000000L;
            const long unixEpochTicks = 621355968000000000L; //Ticks at Midnight Jan 1st 1970;

            return t1 - (int)(((DateTime.UtcNow.Ticks - unixEpochTicks) / ticksToSeconds) % t1);
        }

    }
}