using System;
using System.Collections.Generic;
using System.Text;

namespace OHC.MqttService
{
    public class MqttSettings
    {
        public MqttSettings(string host, string username, string password, string clientId)
        {
            if (string.IsNullOrEmpty(host))
                throw new ArgumentNullException("Host");

            if (string.IsNullOrEmpty(username))
                throw new ArgumentNullException("Username");

            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException("Password");

            if (string.IsNullOrEmpty(clientId))
                throw new ArgumentNullException("ClientId");

            this.Host = host;
            this.Username = username;
            this.Password = password;
            this.ClientId = clientId;
        }

        public string Host{ get; }
        public string ClientId { get; }
        public string Username { get; }
        public string Password { get; }
    }
}
