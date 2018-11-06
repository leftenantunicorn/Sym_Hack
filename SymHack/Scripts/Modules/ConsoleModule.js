
$('#console-lines').append($("#template").clone().removeAttr('id'));

$('#console-lines').on('keypress', 'input', handleLineEnter);

function handleLineEnter(event) {
    if (event.keyCode === 13) {
        $.post($('#loader').data('request-url'), { key: this.value })
            .done(function (response) {
                var clone = $("#template").clone().removeAttr('id');
                var rclone = $("#template-response").clone().removeAttr('id');

                $('#console-lines').append(rclone).find(':input').last().val(response);
                $('#console-lines').append(clone);

                $(rclone).val(response);
            });

        event.preventDefault();
    }
}
