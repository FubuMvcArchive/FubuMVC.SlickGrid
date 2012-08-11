(function ($) {
    var slickGridColumns = function (div) {
        var columnJson = $(div).data('columns');
        eval('var columns = ' + columnJson);

        var columnHash = {};
        $(columns).each(function (i, col) {
            columnHash[col.id] = $.extend(col, { displayed: true });
        });

        this.applyCustomizations = function (customizations) {
            if (customizations.columns) {
                for (var columnId in customizations.columns) {
                    var column = columnHash[columnId];
                    $.extend(column, customizations.columns[columnId]);
                }
            }
        }

        this.determineColumns = function (grid) {
            // TODO -- use the order of the columns displayed in the grid
            // TODO - check if grid is null before you do anything

            var columns = [];
            for (var name in columnHash) {
                var column = columnHash[name];
                if (column.displayed) {
                    columns.push(column);
                }
            }

            return columns;
        }

        return this;
    }



    // register namespace
    $.extend(true, window, {
        "Slick": {
            "Formatters": {

        },
        "GridColumns": slickGridColumns

    }
});

})(jQuery);



$(document).ready(function () {
    $('.slick-grid').each(function (i, div) {
        makeSlickGrid(div);

        div.update({});
    });


});


function makeSlickGrid(div) {
    var columns = Slick.GridColumns(div);

    var url = $(div).data('url');

    var options = {};
    var modification = function (grid, div) {
        // Do nothing.
    };

    var customJson = $('#' + div.id + "-custom").text().trim();
    if (customJson) {

        eval('var customizations = ' + customJson);
        columns.applyCustomizations(customizations);
        
        if (customizations.options) {
            options = customizations.options;
        }

        if (customizations.modify) {
            modification = customizations.modify;
        }
        
    }
    
    
    var defaultOptions = {
        enableCellNavigation: true,
        enableColumnReorder: true
    };



    var gridOptions = $.extend({}, defaultOptions, options);
    var grid = new Slick.Grid("#" + div.id, [], columns.determineColumns(), gridOptions);

    

    div.update = function (query) {
        if (query == null) {
            query = {};
        }

        $.ajax({
            url: url,
            dataType: 'json',
            type: 'POST',
            contentType: 'text/json',
            data: JSON.stringify(query),
            success: function (data) {
                grid.setData(data); // A different, empty or sorted array.
                grid.updateRowCount();
                grid.render();

                // TODO -- this is where we'll do other things to activate stuff that just 
                // got rendered.
            }


        });


        modification(grid, div);
    }


}