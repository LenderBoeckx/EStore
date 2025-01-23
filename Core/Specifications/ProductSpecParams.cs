using System;

namespace Core.Specifications;

public class ProductSpecParams : PagingParams
{
    private List<string> _merken = [];
    private List<string> _types = [];
    private string? _search;

    public List<string> Merken
    {
        get => _merken;
        set 
        { 
            _merken = value.SelectMany(x => x.Split(',', StringSplitOptions.RemoveEmptyEntries)).ToList();
        }
    }
    public List<string> Types 
    {
        get => _types;
        set 
        {

            _types = value.SelectMany(x => x.Split(',', StringSplitOptions.RemoveEmptyEntries)).ToList();
        }
    }
    
    public string Search
    {
        get => _search ?? "";
        set => _search = value.ToLower();
    }

    public string? Sort {get; set;}
    
}
