/*
options
-------
url
paged




*/

function SlickGridDataLoader(div, grid, options) {
    var self = this;

    var req = null; // ajax request
    self.onDataLoaded = new Slick.Event();
    self.onDataLoading = new Slick.Event();

    self.query = {};
    self.data = [];
    self.dataLoaded = null;
    self.pageCount = 0;

    var getData = function (continuation) {
        self.onDataLoading.notify({});

        req = $.ajax({
            url: options.url,
            dataType: 'json',
            type: 'POST',
            contentType: 'text/json',
            data: JSON.stringify(self.query),
            success: function (results) {
                continuation(results.data);

                if ($.isFunction(self.dataLoaded)) {
                    self.dataLoaded();
                }

                self.pageCount = results["pageCount"];

                req = null;
            }
        });
    };

    self.changeQuery = function (query, dataLoaded) {
        if (query == null) {
            query = {};
        }

        self.dataLoaded = dataLoaded;
        self.query = query;

        if (options.paged) {
            self.query.page = 1;
        }

        getData(function (data) {
            self.data = data;
            grid.setData(self.data);
            div.resizeColumns();
        });
    };

    var fetchNextPage = function () {
        self.query.page = self.query.page + 1;

        getData(function (data) {
            var from = self.data.length - 1;
            self.data = self.data.concat(data);
            grid.setData(self.data);

            var to = self.data.length - 1;

            for (var i = from; i <= to; i++) {
                grid.invalidateRow(i);
            }

            grid.updateRowCount();
            grid.render();

            self.onDataLoaded.notify({ from: from, to: to, data: self.data });
        });
    };

    self.ensureData = function (top, bottom) {

        if (bottom < self.data.length) return;
        if (self.query.page == self.pageCount) return;

        if (req) {
            req.done(fetchNextPage);
        } else {
            fetchNextPage();
        }
    };
    




    if (options.paged) {
        grid.onViewportChanged.subscribe(function (e, args) {
            var vp = grid.getViewport();
            self.ensureData(vp.top, vp.bottom);
        });

        grid.onSort.subscribe(function (e, args) {
            self.setSort(args.sortCol.field, args.sortAsc);
            var vp = grid.getViewport();
            self.ensureData(vp.top, vp.bottom);
        });

        self.setSort = function (field, sortAsc) {
            // Later
        };
    }



    return self;
}