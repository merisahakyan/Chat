$(function () {



    var chat = $.connection.chatHub;

    chat.client.addMessage = function (name, message) {

        $('#chatroom').append('<p><b>' + name
            + '</b>: ' + message + '</p>');
        $("#chatroom").scrollTop($("#chatroom")[0].scrollHeight);

    };

    chat.client.onConnected = function (id, userName, allUsers, rooms) {

        //localStorage["userid"] = id;
        //localStorage["username"] = userName;
        //localStorage["users"] = allUsers;

        var name = userName;
        if (userName.length > 15)
            var name = userName.substr(0, 15) + '...';

        $('#header').append('<h3  title="' + userName + '">Welcome, ' + name + '</h3>');


        for (var i = 0; i < allUsers.length; i++) {

            AddUser(allUsers[i].ConnectionId, allUsers[i].Name);
        }
        for (var i = 0; i < rooms.length; i++) {
            AddGroup(rooms[i].RoomID, rooms[i].RoomName);

        }
    }
    chat.client.onNewUserConnected = function (id, name, rooms) {

        AddUser(id, name);
        for (var i = 0; i < rooms.length; i++) {
            AddGroup(rooms[i].RoomID, rooms[i].RoomName);
        }
    }
    chat.client.onUserDisconnected = function (id, userName) {


        $('#' + userName).remove();
    }

    chat.client.onCreating = function (rooms) {

        for (var i = 0; i < rooms.length; i++) {
            AddGroup(rooms[i].RoomID, rooms[i].RoomName);

        }
    }
    chat.client.onNewGroupCreating = function (roomid, roomname) {
        AddGroup(roomid, roomname);

    }
    chat.client.onRoomConnected = function (id, userName, members) {


        $('#chatBody').show();

        for (var i = 0; i < members.length; i++) {

            AddUserToRoom(members[i].ConnectionId, members[i].Name);

        }

    }
    chat.client.onNewRoomConnect = function (id, username, roomname) {
        if (roomname == sessionStorage["roomname"])
            AddUserToRoom(id, username);
    }

    $.connection.hub.disconnected(function () {
        $.connection.hub.start();
    });

    $.connection.hub.start().done(function () {
        $("#joindiv").hide();
        chat.server.starting($("#username").val(), sessionStorage["roomname"]);

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

        $(document).ready(function () {
            $('#txtUserName').keypress(function (event) {

                if (event.which == 13) {
                    localStorage["username"] = $("#txtUserName").val();
                    var name = $("#txtUserName").val();
                    if (name.length > 0) {
                        window.location.href = 'http://localhost:48088/Home/Index';
                        //chat.server.disconnect();
                        //$.connection.hub.start();
                        chat.server.connect(chat.ConnectionId, name);

                    }
                    else {
                        alert("Enter name..!!");
                    }
                }
            });
        });
        $("#btnLogin").click(function () {


            var name = $("#txtUserName").val();
            if (name.length > 0) {
                window.location.href = 'http://localhost:48088/Home/Index';
                //chat.server.disconnect();
                //$.connection.hub.start();
                chat.server.connect(chat.ConnectionId, name);

            }
            else {
                alert("Enter name..!!");
            }

        });
        $("#join").click(function () {
            window.location.href = 'http://localhost:48088/Home/Room?roomname=' + sessionStorage["roomname"];
            chat.server.disconnect('join', sessionStorage["roomname"]);
        });
        $("#sendgroupmessage").click(function () {
            chat.server.SendToGroup(username, $("message").val(), $("roomname").val())
        })

    });
});



function AddUser(id, userName) {

    var userId = localStorage["userid"];

    if (userId != id) {
        var name = userName;
        if (userName.length > 17) {
            var name = userName.substr(0, 17) + '...';
        }
        $("#chatusers").append('<p id="' + userName + '"><b title="' + userName + '">' + name + '</b></p>');
    }
}

function AddGroup(roomid, roomname) {

    $("#rooms").append('<p id="' + roomname + '">' + roomname + '</p>');
    //$("#rooms").append('<p id="' + roomname + '">' + roomname + '</p>');
    $("#" + roomname).click(function () {
        $("#joindiv").show();
        sessionStorage["roomname"] = roomname;
    })

}
function AddUserToRoom(id, username) {

    $("#members").append('<p id="' + id + '"><b title="' + username + '">' + username + '</b></p>');

}
