using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace KR.Infrastructure.Datastore.Configuration
{
	public class DbSettings
	{
        public string Server { get; set; }
        public string Port { get; set; }
        public string Database { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }


        public bool IsValid => !string.IsNullOrEmpty(Server) &&
                               !string.IsNullOrEmpty(Port) &&
                               !string.IsNullOrEmpty(Database) &&
                               !string.IsNullOrEmpty(UserName) &&
                               !string.IsNullOrEmpty(Password);

        public string ConnectionString => IsValid ? $"Server={Server};Port={Port};Database={Database};Username={UserName};Password={Password}"
                                                    : string.Empty;
    }  
}

