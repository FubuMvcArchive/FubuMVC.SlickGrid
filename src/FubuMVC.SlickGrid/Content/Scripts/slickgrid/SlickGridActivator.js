$(document).ready(function() {
    $('.slick-grid').each(function(i, div) {
        makeSlickGrid(div);
        div.update({});
    });
});

(function($) {
    var SlickGridColumns = function(columns) {
        var columnHash = {};

        $(columns).each(function(i, col) {
            if (col.displayed == undefined) {
                col.displayed = true;
            }
            columnHash[col.id] = col;
        });

        this.__columnHash = columnHash;
    };

    SlickGridColumns.prototype.applyCustomizations = function(customizations) {
        var columnHash = this.__columnHash;
        var column, columnId;

        if (customizations.columns) {
            for (columnId in customizations.columns) {
                column = columnHash[columnId];
                $.extend(column, customizations.columns[columnId]);
            }
        }
    };
    SlickGridColumns.prototype.getDisplayedColumns = function() {
        var columnHash = this.__columnHash;
        var columns = [];
        var column, name;

        for (name in columnHash) {
            column = columnHash[name];
            if (column.displayed) {
                columns.push(column);
            }
        }

        return columns;
    };
    SlickGridColumns.prototype.getAllColumns = function(grid) {
        var columnHash = this.__columnHash;
        var allColumns = {displayed:[], hidden:[]};
        var displayed = grid.getColumns();
        var col, name;
        
        $(displayed).each(function (i, col) {
            allColumns.displayed.push({ id: col.id, name: col.name });
        });

        for (name in columnHash) {
            col = columnHash[name];
            if (!col.displayed) {
                allColumns.hidden.push({ id: col.id, name: col.name });
            }
        }

        return allColumns;
    };
    SlickGridColumns.prototype.displayColumns = function(names) {
        var columnHash = this.__columnHash;
        var columns = [];
        var column, name, i;

        for (name in columnHash) {
            column = columnHash[name];
            column.displayed = false;
        }

        for (i = 0; i < names.length; i++) {
            column = columnHash[names[i]];
            column.displayed = true;

            columns.push(column);
        }

        return columns;
    };
    SlickGridColumns.prototype.getFrozenColumns = function() {
        var i, column;
        var columns = [];
        var displayedColumns = this.getDisplayedColumns();

        for (i = 0; i < displayedColumns.length; i++) {
            column = displayedColumns[i];
            if (!column.frozen) break;
            columns.push(column);
        }

        return columns;
    };
    SlickGridColumns.prototype.getFrozenColumnIndex = function() {
        return this.getFrozenColumns().length - 1;
    };

    // register namespace
    $.extend(true, window, {
        "Slick": {
            "Formatters": { },
            "GridColumns": SlickGridColumns
        }
    });

    $.fn.updateSlickGrid = function(query) {
        this.get(0).update(query);
    };

    $.fn.setSlickGridDisplayedColumns = function(names) {
        this.get(0).setDisplayedColumns(names);
    };

    $.fn.getSlickGridColumns = function() {
        return this.get(0).getAllColumns();
    };

})(jQuery);

function makeSlickGrid(div) {
    var customJson = $('#' + div.id + "-custom").text().trim();
    var columnJson = $(div).data('columns');
    var columnData = null;
    var customizations = null;
    var url = $(div).data('url');
    var options = {};
    var modification = function() {
        // Do nothing.
    };
    var defaultOptions = {
        enableCellNavigation: true,
        enableColumnReorder: true
    };
    var columns, gridOptions, grid;

    eval('columnData = ' + columnJson);
    columns = new Slick.GridColumns(columnData);

    if (customJson) {
        eval('customizations = ' + customJson);
        columns.applyCustomizations(customizations);

        if (customizations.options) {
            options = customizations.options;
        }

        if (customizations.modify) {
            modification = customizations.modify;
        }
    }

    gridOptions = $.extend({}, defaultOptions, options);
    grid = new Slick.Grid("#" + div.id, [], columns.getDisplayedColumns(), gridOptions);

    grid.onSort.subscribe(function(e, args) {
        var sortdir, sortcol;
        var defaultSorter = function(a, b) {
            var x = a[sortcol], y = b[sortcol];
            return sortdir * (x == y ? 0 : (x > y ? 1 : -1));
        };

        sortdir = args.sortAsc ? 1 : -1;
        sortcol = args.sortCol.field;

        args.grid.getData().sort(args.sortCol.sorter || defaultSorter, sortdir);
        args.grid.invalidateAllRows();
        args.grid.render();
    });

    grid.renderWithFrozenColumn = function() {
        var frozenColumnIndex = columns.getFrozenColumnIndex();
        this.setOptions({ 'frozenColumn': frozenColumnIndex });
    };

    div.getAllColumns = function() {
        return columns.getAllColumns(grid);
    };

    div.getDisplayedColumnFields = function() {
        return _.map(div.getAllColumns().displayed, function(item) {
            return item.id;
        });
    };

    div.getFrozenColumnFields = function() {
        return _.map(columns.getFrozenColumns(), function(item) {
            return item.id;
        });
    };

    div.setDisplayedColumns = function(names) {
        var displayed = columns.displayColumns(names);

        grid.setColumns(displayed);
        grid.renderWithFrozenColumn();
    };

    div.findRowIndex = function(search) {
        var data = grid.getData();
        var prop, i;
        var filter = function(row) {
            for (prop in search) {
                if (row[prop] != search[prop]) return false;
            }

            return true;
        };

        // Allow for DataView usage
        if (typeof (data.getItems) == 'function') {
            data = data.getItems();
        }

        for (i = 0; i < data.length; i++) {
            if (filter(data[i])) return i;
        }

        return -1;
    };

    div.findColumnIndex = function(name) {
        return grid.getColumnIndex(name);
    };

    div.activateCell = function(search, columnName) {
        var row = div.findRowIndex(search);
        var column = 0;

        grid.scrollRowIntoView(row);

        if (columnName) {
            column = grid.getColumnIndex(columnName);
        }

        grid.setActiveCell(row, column);
    };

    div.editCell = function(search, columnName) {
        div.activateCell(search, columnName);
        grid.editActiveCell();
    };

    div.markCell = function(search, columnName, id) {
        var row = div.findRowIndex(search);
        var column = 0;

        grid.scrollRowIntoView(row);

        if (columnName) {
            column = grid.getColumnIndex(columnName);
        }

        $('#trace').text(row + ", " + column);
        grid.getCellNode(row, column).attr('id', id);
    };

    div.update = function(query, dataLoaded) {
        if (query == null) {
            query = {};
        }

        $.ajax({
            url: url,
            dataType: 'json',
            type: 'POST',
            contentType: 'text/json',
            data: JSON.stringify(query),
            success: function(data) {
                grid.setData(data); // A different, empty or sorted array.
                grid.renderWithFrozenColumn();

                if (gridOptions.autoresize) {
                    grid.autosizeColumns();
                }

                if ($.isFunction(dataLoaded)) {
                    dataLoaded();
                }
            }
        });

        modification(grid, div);
    };
}