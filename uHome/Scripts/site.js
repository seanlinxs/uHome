function showSpinner() {
    $('#spinner').show();
}

function hideSpinner() {
    $('#spinner').hide();
}

function GetAntiForgeryToken() {
    var tokenField = $("input[type='hidden'][name$='RequestVerificationToken']");
    if (tokenField.length == 0) {
        return null;
    } else {
        return {
            name: tokenField[0].name,
            value: tokenField[0].value
        };
    }
}

$.ajaxPrefilter(function (options, localOptions, jqXHR) {
    var token, tokenQuery;
    if (options.type.toLowerCase() != 'get') {
        token = GetAntiForgeryToken();
        if (options.data.indexOf(token.name) === -1) {
            tokenQuery = token.name + '=' + token.value;
            options.data = options.data ? (options.data + '&' + tokenQuery)
                : tokenQuery;
        }
    }
});
