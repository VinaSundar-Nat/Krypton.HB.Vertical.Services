namespace KR.Document.HB.Domain;

public sealed class BlobConfiguration
{
    public string? BaseUrl { get; set; }
    public string? Container { get; set; }
    public string? ManagedIdentityId { get; set; }
    public bool ExcludeEnvironmentCredential { get; set; } = false;
    public bool ExcludeManagedIdentityCredential { get; set; } = false;
    public bool ExcludeAzureCliCredential { get; set; } = true;
    public int SasValidity { get; set; } = 1;
 
    public bool IsValid =>  !( string.IsNullOrEmpty(BaseUrl)
                            && string.IsNullOrEmpty(Container)
                            && string.IsNullOrEmpty(ManagedIdentityId) );
}
