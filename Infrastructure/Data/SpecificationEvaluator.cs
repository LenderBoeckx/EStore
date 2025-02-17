using System;
using System.Reflection.Metadata;
using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

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

        if(spec.IsDistinct)
        {
            query = query.Distinct();
        }

        if(spec.IsPagingEnabled)
        {
            query = query.Skip(spec.Skip).Take(spec.Take);
        }

        //mogelijkheid tot includeren van andere gerelateerde entiteiten
        query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));

        query = spec.IncludeStrings.Aggregate(query, (current, include) => current.Include(include));

        return query;
    }

    public static IQueryable<TResult> GetQuery<TSpec, TResult>(IQueryable<T> query, ISpecification<T, TResult> spec){
        if(spec.Criteria != null)
        {
            query = query.Where(spec.Criteria);
        }

        if(spec.OrderBy != null){
            query = query.OrderBy(spec.OrderBy);
        }

        if(spec.OrderByDesc != null){
            query = query.OrderByDescending(spec.OrderByDesc);
        }

        var selectQuery = query as IQueryable<TResult>;

        if(spec.Select != null)
        {
            selectQuery = query.Select(spec.Select);
        }

        if(spec.IsDistinct)
        {
            selectQuery = selectQuery?.Distinct();
        }

        if(spec.IsPagingEnabled)
        {
            selectQuery = selectQuery?.Skip(spec.Skip).Take(spec.Take);
        }

        return selectQuery ?? query.Cast<TResult>();
    }
}
