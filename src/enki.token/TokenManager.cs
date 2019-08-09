using enki.totp;
using System;
using System.Text;

namespace enki.token
{
    public class TokenManager
    {
        private static DateTime _utcEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static DateTime _localEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);
        private static DateTime _unspecifiedEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);

        private readonly string _base32String;
        private readonly int _timeoutSeconds = 30;
        private readonly int _digits = 6;
        private Totp _totp;

        public TokenManager(string base32SecretString)
        {
            _base32String = base32SecretString;
            _totp = new Totp(base32SecretString);
        }

        public TokenManager(string base32SecretString, int timeoutSeconds, int digits)
        {
            _base32String = base32SecretString;
            _timeoutSeconds = timeoutSeconds;
            _digits = digits;
            _totp = new Totp(base32SecretString, timeoutSeconds, digits);
        }

        /// <summary>
        /// Recupera um token temporário de acesso.
        /// </summary>
        /// <returns>Token formado por numeros conforme definido na configuração.</returns>
        public string GetToken()
        {
            return _totp.getCodeString();
        }

        /// <summary>
        /// Recupera um hash de acesso temporário, valido até a data de expiração informada.
        /// </summary>
        /// <param name="expiresOn">Data GMT/UTC de expirção do token.</param>
        /// <returns>Hash baseado em datas para validação de acesso temporário.</returns>
        public string GetHashWithExpireOn(DateTime expiresOn)
        {
            var tokenTimestamp = GetGMTUnixEpochNow();
            var token = _totp.getCodeString(tokenTimestamp);
            var expiresTimestamp = GetGMTUnixEpochNow(expiresOn);
            var data = string.Concat(expiresTimestamp, ".", token, ".", tokenTimestamp);
            byte[] encData_byte = new byte[data.Length];
            encData_byte = Encoding.UTF8.GetBytes(data);
            return Convert.ToBase64String(encData_byte);
        }

        public bool IsValidToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return false;

            if(token.Length == _digits)
            {
                return (_totp.getCodeString() == token);
            }

            byte[] tokenInBytes;
            try
            {
                tokenInBytes = Convert.FromBase64String(token);
            }
            catch(FormatException)
            {
                return false;
            }

            var decodedToken = Encoding.UTF8.GetString(tokenInBytes);
            var splitedToken = decodedToken.Split('.');
            if (splitedToken.Length != 3) return false;
            var tokenCode = _totp.getCodeString(ulong.Parse(splitedToken[2]));

            var nowEpoch = GetGMTUnixEpochNow();
            var limitEpoch = ulong.Parse(splitedToken[0]);
            var isInTime = nowEpoch < limitEpoch;
            var isSameToken = tokenCode == splitedToken[1];
            if (isInTime && isSameToken) return true;
            return false;
        }

        /// <summary>
        /// Recupera a Data GMT atualizada com o tempo informado.
        /// </summary>
        /// <param name="seconds">Tempo em segundos a serem adicionados ao EPOCH</param>
        /// <returns>Data resultante</returns>
        public static DateTime FromUnixEpochMilliseconds(ulong seconds)
        {
            return _utcEpoch.AddSeconds(seconds);
        }

        /// <summary>
        /// Converte a data atual (UTC/GMT) para UNIX Timestamp
        /// </summary>
        /// <returns>Valor numérico representando os segundos transcorridos desde 01/01/1970 até a data informada.</returns>
        public static ulong GetGMTUnixEpochNow(DateTime? date = null)
        {
            var timestamp = (ulong)Convert.ToInt64(((date ?? DateTime.UtcNow) - _utcEpoch).TotalSeconds);
            return timestamp;
        }
    }
}