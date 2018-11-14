
$('#console-lines').append($("#template").clone().removeAttr('id'));

$('#console-lines').on('keypress', 'input', handleLineEnter);

loadLog();

function handleLineEnter(event) {
    if (event.keyCode === 13) {
        $.post($('#loader').data('request-url'), { key: this.value })
            .done( (response) => {

                printRR(null, response);

                var log = { input: this.value, output: response };

                $.post($('#saver').data('request-url'), { addToLog: JSON.stringify(log) });
            });

        event.preventDefault();
    }
}

function loadLog() {
    $.get($('#saver').data('request-url'))
        .done(function(response) {
            $.each(JSON.parse(response),
                (pair) => {
                    alert(JSON.stringify(pair));
                });
        });
}

function printRR(request, response) {
    var clone = $("#template").clone().removeAttr('id');

    if (response) {
        var r_clone = $("#template-response").clone().removeAttr('id');
        var el_response = r_clone.find('.console-input');

        $(function() {
            el_response.flexible();
        });

        el_response.val(response);
        r_clone.find(':input').trigger("updateHeight");

        $('#console-lines').append(r_clone);
    }

    if (request) {
        var el_request = clone.find('.console-input');
        el_request.val(request);
    }

    $('#console-lines').append(clone);
}