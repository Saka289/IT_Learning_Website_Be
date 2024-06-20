using LW.Data.Entities;
using Nest;

namespace LW.API.Mappings;

public static class MappingElastic
{
    public static void AddDefaultMappings(ConnectionSettings settings)
    {
        settings.DefaultMappingFor<Grade>(m => m.Ignore(g => g.Level));
        settings.DefaultMappingFor<Document>(m => m.Ignore(d => d.Grade));
        settings.DefaultMappingFor<Topic>(m => m.Ignore(d => d.Document));
    }
}