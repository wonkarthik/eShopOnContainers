﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eShopWeb.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using eShopWeb.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace eShopWeb.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly CatalogContext _context;
        public CatalogService(CatalogContext context)
        {
            _context = context;
        }

        public async Task<Catalog> GetCatalogItems(int pageIndex, int itemsPage, int? brandId, int? typeId)
        {
            var root = (IQueryable<CatalogItem>)_context.CatalogItems;

            if (typeId.HasValue)
            {
                root = root.Where(ci => ci.CatalogTypeId == typeId);
            }

            if (brandId.HasValue)
            {
                root = root.Where(ci => ci.CatalogBrandId == brandId);
            }

            var totalItems = await root
                .LongCountAsync();

            var itemsOnPage = await root
                .Skip(itemsPage * pageIndex)
                .Take(itemsPage)
                .ToListAsync();

            return new Catalog() { Data = itemsOnPage, PageIndex = pageIndex, Count = (int)totalItems };           
        }

        public async Task<IEnumerable<SelectListItem>> GetBrands()
        {
            var brands = await _context.CatalogBrands.ToListAsync();
            var items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Value = null, Text = "All", Selected = true });
            foreach (CatalogBrand brand in brands)
            {
                items.Add(new SelectListItem() { Value = brand.Id.ToString(), Text = brand.Brand });
            }

            return items;
        }

        public async Task<IEnumerable<SelectListItem>> GetTypes()
        {
            var types = await _context.CatalogTypes.ToListAsync();
            var items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Value = null, Text = "All", Selected = true });
            foreach (CatalogType type in types)
            {
                items.Add(new SelectListItem() { Value = type.Id.ToString(), Text = type.Type });
            }

            return items;
        }
    }
}
