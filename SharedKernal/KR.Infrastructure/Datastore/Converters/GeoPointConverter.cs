using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetTopologySuite.Geometries;

namespace Account.Infrastructure.Datastore.Converters;

public record Cordinates (double Latitude, double Longitude);
    
public class GeoPointConverter : ValueConverter<Cordinates, Point>
{
    public GeoPointConverter() : base(      
        g => new Point(g.Longitude, g.Latitude),
        p => new Cordinates(p.Y, p.X)
        )       
    { }
}


