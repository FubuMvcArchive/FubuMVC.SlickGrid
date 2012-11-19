using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using OpenQA.Selenium;
using System.Collections.Generic;
using Serenity;
using Serenity.Fixtures;
using FubuCore;

namespace FubuMVC.SlickGrid.Serenity
{
    public class GridDriver
    {
        private readonly string _id;
        private readonly By _finder;
        private readonly Lazy<IWebElement> _grid;
        public const string CellClass = "slick-cell";

        public GridDriver(IWebDriver driver, string id)
        {
            _id = id;
            _finder = By.Id(id);
            Driver = driver;

            _grid = new Lazy<IWebElement>(() => Driver.FindElement(_finder));
        }

        protected IWebElement grid
        {
            get { return _grid.Value; }
        }

        protected IWebDriver Driver { get; private set; }

        public IEnumerable<string> DisplayedColumnFields()
        {
            // getDisplayedColumnFields
            var js = "return $('#{0}').get(0).getDisplayedColumnFields()".ToFormat(_id);
            var intermediate = Driver.InjectJavascript<ReadOnlyCollection<object>>(js);

            return intermediate.Select(x => x.ToString()).ToArray();
        } 

        public IEnumerable<string> Columns
        {
            get
            {
                return grid
                    .FindElements(By.CssSelector(".slick-header .slick-header-column"))
                    .Select(x => x.GetAttribute("title"))
                    .ToArray();
            }
        }

        public IEnumerable<IWebElement> Rows
        {
            get { return grid.FindElement(By.ClassName("slick-viewport")).FindElements(By.ClassName("slick-row")); }
        }

        public DataTable BuildGrid()
        {
            var table = new DataTable();
            Columns.Each(col => table.Columns.Add(col, typeof(string)));

            fillTable(table);

            return table;
        }

        private string textForCell(IWebElement element)
        {
            var child = element.FindElements(By.CssSelector("*:first-child")).FirstOrDefault();
            return child == null ? element.Text : child.Text;
        }

        private void fillTable(DataTable table)
        {
            Rows.Each(row =>
            {
                var dataRow = table.NewRow();
                var i = 0;
                row.FindElements(By.ClassName(CellClass)).Each(x =>
                {
                    dataRow[i] = textForCell(x);
                    ++i;
                });

                table.Rows.Add(dataRow);
            });
        }
    }



    
}