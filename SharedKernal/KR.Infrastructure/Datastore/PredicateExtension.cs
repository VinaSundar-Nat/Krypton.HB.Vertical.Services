using System;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;
using KR.Common.Attributes.Persistance;

namespace KR.Infrastructure.Datastore;

public static class PredicateExtension
{
    public static Expression<Func<T, bool>> PredicateBuilder<T, K>(this DbSet<T> entity, K filter, ICollection<string> reqFilters)
    where T : class
    where K : class
    {
        BinaryExpression? dynamicExp = null;
        ParameterExpression pe = Expression.Parameter(typeof(T), "pe");

        foreach (var pi in filter.GetType()
        .GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {

            var ignoreAttr = pi.GetCustomAttribute(typeof(IgnoreAttribute));

            if ((!reqFilters.Any(a => a == pi.Name) && ignoreAttr == null) || ignoreAttr != null)
                continue;

            var propValue = pi.GetValue(filter, null);

            var deepMapAttr = pi.GetCustomAttribute(typeof(DeepObjectAttribute));

            Expression lMem = null;

            //construct:pe.property eg: pe.Id or pe.property deepMap eg: pe.Contact.Email
            if (deepMapAttr != null)
            {
                var mappingObj = (DeepObjectAttribute)deepMapAttr;

                string[] props = mappingObj.DeepMap.Split('.').ToArray();

                Expression refExp = pe;
                
                foreach (var _prop in props)
                {
                    var _type = typeof(T);
                    PropertyInfo? cpi = _type.GetProperty(_prop, BindingFlags.Public | BindingFlags.Instance);
           
                    refExp = Expression.Property(refExp, cpi);
                    _type = cpi.PropertyType;
                }

                lMem = Expression.PropertyOrField(refExp,
                            string.IsNullOrEmpty(mappingObj.Alias) ? pi.Name : mappingObj.Alias);
            }
            else
            {
                lMem = Expression.Property(pe,
                typeof(T).GetProperty(pi.Name, BindingFlags.Public | BindingFlags.Instance));
            }

            if (pi.PropertyType == typeof(string))
            {
                lMem = Expression.Call(lMem, typeof(string)
                        .GetMethod("ToLower", System.Type.EmptyTypes));

                if (propValue != null)
                    propValue = Convert.ToString(propValue).ToLower();
            }

            //construct: value eg: 1
            Expression rMem = Expression.Constant(propValue);

            dynamicExp = dynamicExp == null ?
                //construct:pe.property == value eg: pe.Id == 1
                Expression.Equal(lMem, rMem) :
                //construct: Add ANDALSO operator to expression eg: pe.Id == 1 && pe.FName == "Test"
                Expression.AndAlso(dynamicExp, Expression.Equal(lMem, rMem));

        }


        if (dynamicExp != null)
        {
            return Expression.Lambda<Func<T, bool>>(dynamicExp, pe);
        }

        return null;
    }


}

