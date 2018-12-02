using System;

namespace gdo.Ryobi
{
    public class Sys
    {
        public string ip { get; set; }
        public long? lastSeen { get; set; }
    }

    public class WskAuthAttempt
    {
        public string varName { get; set; }
        public string apiKey { get; set; }
        public string ts { get; set; }
        public bool? success { get; set; }
    }

    public class MetaData
    {
        public string companyName { get; set; }
        public string surName { get; set; }
        public string givenName { get; set; }
        public Sys sys { get; set; }
        public bool? autoLogout { get; set; }
        public WskAuthAttempt[] wskAuthAttempts { get; set; }
        public int? authCount { get; set; }
    }

    public class AccountOptions
    {
        public string email { get; set; }
        public string alertPhone { get; set; }
        public string alertEmail { get; set; }
        public bool? receiveEmailUpdates { get; set; }
        public bool? receiveEmailAlerts { get; set; }
        public bool? receiveSmsAlerts { get; set; }
    }

    public class RoleMap
    {
        public object[] roleSelectors { get; set; }
        public string[] roleRegex { get; set; }
        public string[] roleNames { get; set; }
    }

    public class Auth
    {
        public string apiKey { get; set; }
        public string regPin { get; set; }
        public string clientUserName { get; set; }
        public DateTimeOffset? createdDate { get; set; }
        public string[] childSelectors { get; set; }
        public RoleMap roleMap { get; set; }
        public string[] roleIds { get; set; }
        public string clientSchema { get; set; }
    }

    public class LoginResult
    {
        public string _id { get; set; }
        public string varName { get; set; }
        public MetaData metaData { get; set; }
        public AccountOptions accountOptions { get; set; }
        public bool? enabled { get; set; }
        public bool? deleted { get; set; }
        public DateTimeOffset createdDate { get; set; }
        public int? activated { get; set; }
        public object[] notificationTransports { get; set; }
        public Auth auth { get; set; }
    }

    public class LoginResponse
    {
        public LoginResult result { get; set; }
    }
}