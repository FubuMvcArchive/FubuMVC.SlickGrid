(function ($) {
    // register namespace
    $.extend(true, window, {
        "Slick": {
            "Formatters": {

            }
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
    var columnJson = $(div).data('columns');

    eval('var columns = ' + columnJson);

    var url = $(div).data('url');

    var options = {};
    var modification = function (grid, div) {
        // Do nothing.
    };

    var customJson = $('#' + div.id + "-custom").text().trim();
    if (customJson) {

        eval('var customizations = ' + customJson);

        if (customizations.columns) {
            var columnHash = {};
            $(columns).each(function (i, col) {
                columnHash[col.id] = col;
            });
        
            for (var columnId in customizations.columns) {
                var column = columnHash[columnId];
                $.extend(column, customizations.columns[columnId]);
            }
        }

        
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
    var grid = new Slick.Grid("#" + div.id, [], columns, gridOptions);

    

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