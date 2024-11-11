using System;
using KR.Common.Extensions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;

namespace KR.Security.DataProtection;

public interface IDataProtection
{
    T Unprotect<T>(string protectedInput,string key, T model) where T : class;
}

public class ProtectProvider(IDataProtectionProvider Provider) : IDataProtection
{
    private readonly IDataProtector _protector = Provider.CreateProtector("KEYPROTECT");

    public  string Protect(string input)
        => _protector.Protect(input);
    
    public  string Unprotect(string protectedInput) =>
         _protector.Unprotect(protectedInput);

    public T Unprotect<T>(string protectedInput,string key, T model) 
        where T : class
    {
        ArgumentNullException.ThrowIfNull(model);
        var value = _protector.Unprotect(protectedInput);
        ArgumentNullException.ThrowIfNullOrEmpty(value);
        model.ExpressionSetter<T>(key,value);        
        return model;
    }
}
