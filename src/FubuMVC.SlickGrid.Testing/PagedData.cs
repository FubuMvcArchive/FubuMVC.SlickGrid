﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.SlickGrid.Testing
{
    public class Item
    {
        public string Name { get; set; }
    }

    public class ItemGrid : GridDefinition<Item>
    {
        public ItemGrid()
        {
            SourceIs<ItemSource>();

            Column(x => x.Name);
        }
    }

    public class ItemQuery : PagedQuery
    {
        
    }

    public class ItemSource : IPagedGridDataSource<Item, ItemQuery>
    {
        public static Item[] Items;
        public const int PageSize = 25;

        static ItemSource()
        {
            Items = new Item[500];

            for (int i = 0; i < Items.Length; i++)
            {
                Items[i] = new Item{Name = Guid.NewGuid().ToString()};
            }
        }

        public PagedResults<Item> GetData(ItemQuery query)
        {
            var data = Items.Skip(PageSize * query.page).Take(PageSize);
            return new PagedResults<Item>
            {
                PageCount = Items.Length / PageSize,
                Data = data
            };
        }
    }
}