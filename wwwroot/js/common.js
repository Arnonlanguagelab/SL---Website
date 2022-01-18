/* 

common.js 
Contains common variables and methods

*/

/* Ajax request wrapper */
function sendAjaxRequest(_pageURL, _funcName, _data, _cbSuccess, _cbError, _cbComplete) {
    $.ajax({
        type:           "POST",
        contentType:    "application/json; charset=utf-8",
        dataType:       "json",
        async:          true,
        url:            _pageURL + '/' + _funcName,
        data:           JSON.stringify(_data),
        success:        _cbSuccess == null ? defaultAjaxSuccess : _cbSuccess,
        error:          _cbError == null ? defaultAjaxError : _cbError,
        complete:       _cbComplete == null ? defaultAjaxComplete : _cbComplete
    });
}

/* Default ajax success handler */
function defaultAjaxSuccess(data, status, xhr) {

}

/* Default ajax error handler */
function defaultAjaxError(xhr, status, err) {

}

/* Default ajax complete handler */
function defaultAjaxComplete(xhr, status) {

}

/* Shuffle an array */
function shuffle(a) {
    var j, x, i;
    for (i = a.length; i; i--) {
        j = Math.floor(Math.random() * i);
        x = a[i - 1];
        a[i - 1] = a[j];
        a[j] = x;
    }
}

/* Shuffle a 'triplets' array with no immediate repetitions */
function shuffle_triplets_no_immediate_repetitions(a) {
    var j, x, i;
    for (i = a.length; i; i--) {
        do {
            j = Math.floor(Math.random() * a.length);
        } while ((a[i] && a[j].tripletID == a[i].tripletID) || (a[i - 2] && a[j].tripletID == a[i - 2].tripletID) ||
                 (a[j + 1] && a[i - 1].tripletID == a[j + 1].tripletID) || (a[j - 1] && a[i - 1].tripletID == a[j - 1].tripletID));

        x = a[i - 1];
        a[i - 1] = a[j];
        a[j] = x;
    }
}