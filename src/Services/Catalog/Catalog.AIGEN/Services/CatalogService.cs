//implement the ICatalogService from ICatalogService.cs interface
//
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EshopOnAI.ProductGenerator.Models;
using ProductGen.Extensions;

namespace EshopOnAI.ProductGenerator.Services
{
    public class CatalogService : ICatalogService
    {
        public Task<List<CatalogBrand>> GetProjectsAsync(List<Suggestion> data)
        {
            return Task.FromResult(data.Select(x => new CatalogBrand(x.id, x.name)).ToList());
        }

        public Task<List<CatalogType>> GetCatalogTypesAsync(List<Suggestion> data)
        {
            // return Task.FromResult(root.merchandises.Select(x => new CatalogType(x.type.Length, x.type)).ToList());
            var types = data.SelectMany(x => x.merchandises.Select(y => new CatalogType(y.type.SumAsciiValues(), y.type))).Distinct().ToList();
            return Task.FromResult(types);
        }

        public Task<List<CatalogItem>> GetCatalogItemsAsync(List<Suggestion> data)
        {
            // var items =  Task.FromResult(root.merchandises.Select(x => new CatalogItem(x.id, x.name, x.price, "", x.type.Length, x.type.Length, x.availableStock)).ToList());
            var items = data.SelectMany(x => x.merchandises.Select(y => new CatalogItem(y.id, y.name, y.price, $"{y.id}.png", y.type.SumAsciiValues(), x.id, y.availableStock))).ToList();
            return Task.FromResult(items);
        }
    }
}
