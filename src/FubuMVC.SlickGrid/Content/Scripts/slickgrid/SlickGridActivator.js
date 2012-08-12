$(document).ready(function () {
    $('.slick-grid').each(function (i, div) {
        makeSlickGrid(div);

        div.update({});
    });
});


(function ($) {
    var slickGridColumns = function (columns) {
        var columnHash = {};
        $(columns).each(function (i, col) {
            if (col.displayed == undefined){
                col.displayed = true;
            }

            columnHash[col.id] = col;
        });

        this.applyCustomizations = function (customizations) {
            if (customizations.columns) {
                for (var columnId in customizations.columns) {
                    var column = columnHash[columnId];
                    $.extend(column, customizations.columns[columnId]);
                }
            }
        }

        this.getDisplayedColumns = function () {
            var columns = [];
            for (var name in columnHash) {
                var column = columnHash[name];
                if (column.displayed) {
                    columns.push(column);
                }
            }

            return columns;
        }

        this.getAllColumns = function(grid){
            var allColumns = {displayed:[], hidden:[]};

            var displayed = grid.getColumns();
            $(displayed).each(function(i, col){
                allColumns.displayed.push({id: col.id, name: col.name});
            });

            for (var name in columnHash){
                var col = columnHash[name];
                if (!col.displayed){
                    allColumns.hidden.push({id: col.id, name: col.name});
                }   
            }


            return allColumns;
        }

        this.displayColumns = function(names){
            for (var name in columnHash) {
                var column = columnHash[name];
                column.displayed = false;
            }
            
            var columns = [];

            for (var i = 0; i < names.length; i++){
                var column = columnHash[names[i]];
                column.displayed = true;

                columns.push(column);
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

    $.fn.updateSlickGrid = function(query){
        this.get(0).update(query);
    }

    $.fn.setSlickGridDisplayedColumns = function(names){
        this.get(0).setDisplayedColumns(names);
    }

    $.fn.getSlickGridColumns = function(){
        return this.get(0).getAllColumns();
    }

})(jQuery);




function makeSlickGrid(div) {
    var columnJson = $(div).data('columns');
    eval('var columnData = ' + columnJson);

    var columns = Slick.GridColumns(columnData);

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
    var grid = new Slick.Grid("#" + div.id, [], columns.getDisplayedColumns(), gridOptions);

    div.getAllColumns = function(){
        return columns.getAllColumns(grid);
    }

    div.setDisplayedColumns = function(names){
        var displayed = columns.displayColumns(names);

        grid.setColumns(displayed);
        grid.render();
    }

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

                if (gridOptions.autoresize) {
                    grid.autosizeColumns();
                }
            }
        });


        modification(grid, div);
    }


}