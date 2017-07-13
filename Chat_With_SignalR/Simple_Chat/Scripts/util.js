$(function () {

    $('#chatBody').hide();
    $('#loginBlock').show();

    var chat = $.connection.chatHub;
    chat.client.addMessage = function (name, message) {

        $('#chatroom').append('<p><b>' + name
            + '</b>: ' + message + '</p>');
        $("#chatroom").scrollTop($("#chatroom")[0].scrollHeight);

    };

    chat.client.onConnected = function (id, userName, allUsers) {

        $('#loginBlock').hide();
        $('#chatBody').show();

        $('#hdId').val(id);
        $('#username').val(userName);
        $('#header').html('<h3>Welcome, ' + userName + '</h3>');


        for (var i = 0; i < allUsers.length; i++) {

            AddUser(allUsers[i].ConnectionId, allUsers[i].Name);
        }
    }
    chat.client.onNewUserConnected = function (id, name) {

        AddUser(id, name);
    }
    chat.client.onUserDisconnected = function (id, userName) {

        $('#' + id).remove();
    }


    $(document).ready(function () {
        $('#txtUserName').keypress(function (event) {

            if (event.which == 13) {
                var name = $("#txtUserName").val();
                if (name.length > 0) {
                    chat.server.connect(name);

                }
                else {
                    alert("Enter name..!!");
                }
            }
        });
    });


    $.connection.hub.start().done(function () {

        $('#sendmessage').click(function () {
            if ($('#message').val() != '') {
                chat.server.send($('#username').val(), $('#message').val());
                $('#message').val('');
            }
        });

        $(document).ready(function () {
            $('#message').keypress(function (event) {

                if (event.which == 13 && $('#message').val() != '') {
                    chat.server.send($('#username').val(), $('#message').val());
                    $('#message').val('');
                }
            });
        });

        $("#btnLogin").click(function () {

            var name = $("#txtUserName").val();
            if (name.length > 0) {
                chat.server.connect(name);

            }
            else {
                alert("Enter name..!!");
            }

        });
    });
});



function AddUser(id, name) {

    var userId = $('#hdId').val();

    if (userId != id) {

        $("#chatusers").append('<p id="' + id + '"><b>' + name + '</b></p>');
    }
}

