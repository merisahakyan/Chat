$(function () {

    $('#chatBody').hide();
    $('#loginBlock').show();

    var chat = $.connection.chatHub;
    chat.client.addMessage = function (name, message) {

        $('#chatroom').append('<p><b>' + name
            + '</b>: ' + message + '</p>');
        $("#chatroom").scrollTop($("#chatroom")[0].scrollHeight);

    };

    chat.client.onConnected = function (id, userName, allUsers, rooms) {

        $('#loginBlock').hide();
        $('#chatBody').show();

        $('#hdId').val(id);
        $('#username').val(userName);
        var name = userName;
        if (userName.length > 15)
            var name = userName.substr(0, 15) + '...';

        $('#header').html('<h3  title="' + userName + '">Welcome, ' + name + '</h3>');


        for (var i = 0; i < allUsers.length; i++) {

            AddUser(allUsers[i].ConnectionId, allUsers[i].Name);
        }
        for (var i = 0; i < rooms.length; i++) {
            AddGroup(rooms[i].RoomID, $('#userid').val(), rooms[i].RoomName, $('#username').val());
        }
    }
    chat.client.onNewUserConnected = function (id, name) {

        AddUser(id, name);
    }
    chat.client.onUserDisconnected = function (id, userName) {

        $('#' + id).remove();
    }

    chat.client.onCreating = function (rooms) {
        $("#rooms").empty();
        for (var i = 0; i < rooms.length; i++) {
            AddGroup(rooms[i].RoomID, $('#userid').val(), rooms[i].RoomName, $('#username').val());
        }
    }
    chat.client.onNewGroupCreating = function (roomid, roomname) {
        AddGroup(roomid, $('#userid').val(), roomname, $('#username').val());
    }
    chat.client.onRoomConnected = function (id, userName, members) {


        $('#chatBody').show();

        $('#userid').val(id);
        $('#username').val(userName);
        var name = $('#username').val();
        if ($('#username').val().length > 15)
            var name = userName.substr(0, 15) + '...';

        $('#header').html('<h3  title="' + $('#username').val() + '">Welcome, ' + name + '</h3>');


        for (var i = 0; i < allUsers.length; i++) {

            AddUserToRoom(members[i].ConnectionId, members[i].Name, $('#roomid').val());

        }

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
        $("#create").click(function () {
            $("#create").hide();

            $('<input/>').attr({ type: 'text', id: 'roomname' }).appendTo('#rooms');

            var b = $('<button>',
                {
                    text: 'Create',
                    id: 'createroom',
                    click: function (roomname) {
                        $("#create").show();
                        roomname = $('#roomname').val();
                        $("#roomname").remove();
                        $("#createroom").remove();
                        chat.server.create(roomname);

                    }
                })
            $("#rooms").append(b);
        });

        $("#test").click(function () {
           
            chat.server.joinGroup('m');
        });

    });
});



function AddUser(id, userName) {

    var userId = $('#hdId').val();

    if (userId != id) {
        var name = userName;
        if (userName.length > 17) {
            var name = userName.substr(0, 17) + '...';
        }
        $("#chatusers").append('<p id="' + id + '"><b title="' + userName + '">' + name + '</b></p>');
    }
}

function AddGroup(roomid, userid, roomname, username) {
    var hr = 'http://localhost:48088/Home/Room?id=' + roomid + '&userid=' + userid + '&username=' + username + '&roomname=' + roomname;
    $("#rooms").append('<p id="' + roomname + '"><a  href="' + hr + '" >' + roomname + '</a></p>');
    
    
}
function AddUserToRoom(id, username, roomid) {
    var rid = $("#roomid").val();
    var uid = $('#userid').val();
    if (uid != id && rid == roomid) {
        $("#members").append('<p id="' + id + '"><b title="' + username + '">' + username + '</b></p>');
    }
}