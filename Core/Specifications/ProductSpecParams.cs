using System;

namespace Core.Specifications;

public class ProductSpecParams
{
    private const int MaxPageSize = 50;
    public int PageIndex {get; set;} = 1;
    private int _pageSize = 6;
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
    public int PageSize{
        get => _pageSize;
        set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
    }
    public string Search
    {
        get => _search ?? "";
        set => _search = value.ToLower();
    }

    public string? Sort {get; set;}
    
}
