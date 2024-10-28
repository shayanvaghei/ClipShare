let pageNumber = 1;
let pageSize = 10;
let searchBy = '';
let categoryId = 0;
let utcDateTimeNowString;

function getUtcDateTimeNow() {
    return utcDateTimeNowString;
}

function setUtcDateTimeNow(date) {
    utcDateTimeNowString = date;
}

function getMyVideos() {
    const parameters = {
        pageNumber,
        pageSize,
        searchBy,
        categoryId
    }

    $.ajax({
        url: "/Home/GetVideosForHomeGrid",
        type: "GET",
        data: parameters,
        success: function (data) {
            const result = data.result;

            $('#videosTableBody').empty();
            $('#paginationSummery').empty();
            $('#paginationBtnGroup').empty();
            $('#itemsPerPageDisplay').empty();

            populateVideoTableBody(result.items);

            if (result.totalItemsCount > 0) {
                $('#itemsPerPageDisplay').text(pageSize);

                const from = (result.pageNumber - 1) * result.pageSize + 1;
                const to = result.pageNumber * result.pageSize > result.totalItemsCount ? result.totalItemsCount : result.pageNumber * result.pageSize;
                $('#paginationSummery').text(`${from}-${to} of ${result.totalItemsCount}`);


                // Next Page, Last Page, Previous Page, First Page buttons and functionalities
                let firstPageBtn = '';
                firstPageBtn += `<button type="button" class="btn btn-secondary btn-sm paginationBtn" ${result.pageNumber == 1 ? 'disabled' : ''} data-value="1" data-bs-toggle="tooltip" data-bs-placement="bottom" title="First Page">`;
                firstPageBtn += '<i class="bi bi-chevron-bar-left"></i>';
                firstPageBtn += '</button>';
                $('#paginationBtnGroup').append(firstPageBtn);

                let previousePageBtn = '';
                previousePageBtn += `<button type="button" class="btn btn-secondary btn-sm paginationBtn" ${result.pageNumber == 1 ? 'disabled' : ''} data-value="${result.pageNumber - 1}" data-bs-toggle="tooltip" data-bs-placement="bottom" title="Previous Page">`;
                previousePageBtn += '<i class="bi bi-chevron-left"></i>';
                previousePageBtn += '</button>';
                $('#paginationBtnGroup').append(previousePageBtn);

                let nextPageBtn = '';
                nextPageBtn += `<button type="button" class="btn btn-secondary btn-sm paginationBtn" ${result.pageNumber == result.totalPages ? 'disabled' : ''} data-value="${result.pageNumber + 1}" data-bs-toggle="tooltip" data-bs-placement="bottom" title="Next Page">`;
                nextPageBtn += '<i class="bi bi-chevron-right"></i>';
                nextPageBtn += '</button>';
                $('#paginationBtnGroup').append(nextPageBtn);

                let lastPageBtn = '';
                lastPageBtn += `<button type="button" class="btn btn-secondary btn-sm paginationBtn" ${result.pageNumber == result.totalPages ? 'disabled' : ''}  data-value="${result.totalPages}" data-bs-toggle="tooltip" data-bs-placement="bottom" title="Last Page">`;
                lastPageBtn += '<i class="bi bi-chevron-bar-right"></i>';
                lastPageBtn += '</button>';
                $('#paginationBtnGroup').append(lastPageBtn);

                // On paginationBtn click event
                $('.paginationBtn').click(function () {
                    pageNumber = $(this).data('value');
                    getMyVideos();
                });
            } else {
                $('#itemsPerPageDropdown').hide();
            }
        }
    });

    // On dropdown "Rows per page" selection event
    $('.pageSizeBtn').click(function () {
        pageSize = $(this).data('value');
        getMyVideos();
    });


    // On dropdown "CategoryDropdown" selection event
    $('#categoryDropdown').on('change', function () {
        var selectedValue = $(this).val(); // Get selected value
        categoryId = selectedValue;
        getMyVideos();
    });


    // On searchBtn click
    $("#searchBtn").click(function () {
        const searchInput = $('#searchInput').val();
        searchBy = searchInput;
        getMyVideos();
    });


    // On Search input when enter is pressed
    $('#searchInput').on('keyup', function (event) {
        if (event.key === "Enter" || event.keyCode === 13) {
            var searchInput = $(this).val();
            searchBy = searchInput;
            getMyVideos();
        }
    });


    function populateVideoTableBody(videos) {
        let divTag = '';

        if (videos.length > 0) {
            videos.map((v, index) => {

                if (index % 4 == 0) {
                    divTag += '<div class="row">';
                }

                divTag += '<div class="col-xl-3 col-md-6 pt-2">';
                divTag += '<div class="p-2 border rounded text-center">';
                divTag += `<div><a href="/Video/Watch/${v.id}"><img src="${v.thumbnailUrl}" class="rounded preview-image" /></a></div>`;
                divTag += `<a href="/Video/Watch/${v.id}" class="text-danger-emphasis" style="text-decoration: none;">${v.title}</a>`;
                divTag += '<div><span style="font-size: small">';
                divTag += `<a href="/Member/Channel/${v.channelId}" style="text-decoration: none;" class="text-primary">${v.channelName}</a> <br />`;
                divTag += `${formatView(v.views)} - ${timeAgo(v.createdAt, getUtcDateTimeNow())}</span></div>`;
                divTag += '</div></div>';

                if ((index + 1) % 4 == 0) {
                    divTag += '</div>';
                }
            });
        } else {
            divTag += '<div class="row"><div class="col text-center">No Videos</div></div>';
        }

        $('#videosTableBody').append(divTag);
    }
}