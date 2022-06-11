const Sortable = {
    baseUrl: "",
    sortBy: 0,
    searchTerm: "",
    Search() {
        var searchKey = $('#txtSearch').val();
        window.location.href = this.baseUrl + "?id=" + searchKey;
    },
    Sort(sortBy) {
        window.location.href = this.baseUrl + "?sortBy=" + sortBy;
    }
}

var apiHandler = {
    GET(url) {
        $.ajax({
            url: url,
            type: 'GET',
            success: function (res) {
                debugger;
            }
        });
    },
    POST(url, object) {
        object = {
            Id: 5,
            Name: "asd",
        }

        $.ajax({
            url: url,
            type: 'GET',
            data: object,
            success: function (res) {
                debugger;
            }
        });
    },
    DELETE(url) {
        if (confirm("Are you sure?")) {
            $.ajax({
                url: url,
                type: 'GET',
                success: function (res) {
                    if (res.Success == true) {
                        debugger;
                        location.href = res.returnUrl;
                    }
                    else {
                        alert(res.Message);
                    }
                }
            });
        }
        else {
            alert("Ok. No deletion.")
        }

    }
};