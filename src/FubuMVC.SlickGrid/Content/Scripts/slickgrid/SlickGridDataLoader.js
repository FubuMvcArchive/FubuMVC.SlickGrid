/*
options
-------
url
paged




*/

function SlickGridDataLoader(div, grid, options) {
    var self = this;

    self.query = {};
    self.data = [];
    self.dataLoaded = null;

    var getData = function (continuation) {
        $.ajax({
            url: options.url,
            dataType: 'json',
            type: 'POST',
            contentType: 'text/json',
            data: JSON.stringify(self.query),
            success: function (data) {
                continuation(data);

                if ($.isFunction(self.dataLoaded)) {
                    self.dataLoaded();
                }
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

    return self;
}