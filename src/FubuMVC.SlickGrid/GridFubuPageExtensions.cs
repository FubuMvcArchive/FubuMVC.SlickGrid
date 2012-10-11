using FubuCore;
using FubuMVC.Core.Assets;
using FubuMVC.Core.View;
using HtmlTags;

namespace FubuMVC.SlickGrid
{
    // Depending on manual and E2E tests for this bad boy
    public static class GridFubuPageExtensions
    {
        public static HtmlTag RenderGrid<T>(this IFubuPage page, string id) where T : IGridDefinition, new()
        {
            return page.Get<GridTagWriter<T>>().Write(id);
        }
    }
}