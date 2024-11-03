using System;
using Microsoft.AspNetCore.DataProtection;

namespace KR.Security.DataProtection;

public interface IKRDataProtection
{

}

public sealed class ProtectProvider(IDataProtectionProvider provider) : IKRDataProtection
{
    private readonly IDataProtector _protector = provider.CreateProtector("KRPROTECT");

    public string Protect(string input)
    {
        return _protector.Protect(input);
    }

    public string Unprotect(string protectedInput) =>
         _protector.Unprotect(protectedInput);

}
