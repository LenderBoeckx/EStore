using System;
using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Data;

public class SpecificationEvaluator<T> where T : BaseEntity
{
    public static IQueryable<T> GetQuery(IQueryable<T> query, ISpecification<T> spec){
        if(spec.Criteria != null)
        {
            //query opvullen met wat er gezocht moet worden in de database
            //voorbeeld 'merk' --> x => x.merk = merk
            query = query.Where(spec.Criteria);
        }

        if(spec.OrderBy != null){
            query = query.OrderBy(spec.OrderBy);
        }

        if(spec.OrderByDesc != null){
            query = query.OrderByDescending(spec.OrderByDesc);
        }

        return query;
    }
}
