using System;
namespace KR.Infrastructure.Server.Model;

public abstract class ServerConfiguration
{
    public const string HostingOptions = "HostingOptions";
}

public sealed class KerstalConfiguration: ServerConfiguration
{
    
    public bool UseKerstal { get; set; }
	public string CertPath { get; set; }
    public string CertPassword { get; set; }
}


