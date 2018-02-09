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
        /// Recupera um token tempor�rio de acesso.
        /// </summary>
        /// <returns>Token formado por numeros conforme definido na configura��o.</returns>
        public string GetToken()
        {
            return _totp.getCodeString();
        }

        /// <summary>
        /// Recupera um hash de acesso tempor�rio, valido at� a data de expira��o informada.
        /// </summary>
        /// <param name="expiresOn">Data de expira��o do token.</param>
        /// <returns>Hash baseado em datas para valida��o de acesso tempor�rio.</returns>
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

            var decodedToken = Encoding.UTF8.GetString(Convert.FromBase64String(token));
            var splitedToken = decodedToken.Split('.');
            if (splitedToken.Length != 3) return false;
            var tokenCode = _totp.getCodeString(ulong.Parse(splitedToken[2]));

            var isInTime = FromUnixEpochMilliseconds(ulong.Parse(splitedToken[0])) < DateTime.UtcNow;
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
        /// <returns>Valor num�rio representando os segundos transcorridos desde 01/01/1970 at� a data informada.</returns>
        public static ulong GetGMTUnixEpochNow(DateTime? date = null)
        {
            var timestamp = (ulong)Convert.ToInt64(((date ?? DateTime.UtcNow) - _utcEpoch).TotalSeconds);
            return timestamp;
        }
    }
}