$(function () {



    var chat = $.connection.chatHub;

    chat.client.addMessage = function (name, message) {
        if (sessionStorage["roomname"] == 'general') {
            $('#chatroom').append('<p><b>' + name
                + '</b>: ' + message + '</p>');
            $("#chatroom").scrollTop($("#chatroom")[0].scrollHeight);
        }
    };
    chat.client.showAllMessages = function (roomname, messages) {

        for (var i = 0; i < messages.length; i++)
            $('#chatroom').append('<p><b>' + messages[i].Sender
                + '</b>: ' + messages[i].Text + '</p>');
    }

    chat.client.addMessageToRoom = function (name, message, roomname) {

        if (roomname == sessionStorage["roomname"]) {
            $('#chatroom').append('<p><b>' + name
               + '</b>: ' + message + '</p>');
            $("#chatroom").scrollTop($("#chatroom")[0].scrollHeight);
        }

    };

    chat.client.onConnected = function (id, userName, allUsers, joinedrooms, rooms) {

        //localStorage["userid"] = id;
        //localStorage["username"] = userName;
        //localStorage["users"] = allUsers;
        $("username").val(userName);

        var name = userName;
        if (userName.length > 15)
            var name = userName.substr(0, 15) + '...';

        $('#header').append('<h3  title="' + userName + '">Welcome, ' + name + '</h3>');


        for (var i = 0; i < allUsers.length; i++) {

            AddUser(allUsers[i].ConnectionId, allUsers[i].Name);
        }
        for (var i = 0; i < rooms.length; i++) {
            AddGroup(rooms[i].RoomName);

        }
        for (var i = 0; i < joinedrooms.length; i++) {
            AddJoinedGroup(joinedrooms[i].RoomName);

        }
    }
    chat.client.onNewUserConnected = function (id, name) {
        $("#" + name).remove();
        AddUser(id, name);
    }
    chat.client.onUserDisconnected = function (id, userName) {


        $('#' + userName).remove();
    }


    chat.client.onNewGroupCreating = function (roomname) {
        $("#roomvalidation").hide();
        AddGroup(roomname);

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

    chat.client.onRoomOut = function (username, roomname) {
        if (roomname == sessionStorage["roomname"])
            $('#' + username).remove();
    }
    chat.client.outFromRoom = function (username) {
        if (sessionStorage["username"] == username) {
            window.location.href = 'http://localhost:48088//Home/Index';
            sessionStorage["username"] = username;
            sessionStorage["roomname"] = 'general';
        }
    }
    chat.client.toGeneral = function (username) {
        window.location.href = 'http://localhost:48088//Home/Index';
        sessionStorage["username"] = username;
        sessionStorage["roomname"] = 'general';
    }
    chat.client.onRegistration = function (t) {
        if (t) {
            $("#registrationform").hide();
            $("#activate").show();
        }
        else {
            alert('The username already exists!')
        }
    }
    chat.client.onLoginFail = function () {
        $("#loginvalidation").show();
    }
    chat.client.onLogin = function () {
        window.location.href = 'http://localhost:48088/Home/Index';
    }
    chat.client.failedRoomCreating = function () {
        $("#roomvalidation").show();

    }
    //$.connection.hub.disconnected(function () {


    //    $.connection.hub.start();
    //});


    $.connection.hub.start().done(function () {
        $("#joindiv").hide();
        $("#activate").hide();
        $("#validation").hide();
        $("#loginvalidation").hide();
        $("#roomvalidation").hide();
        $("#namevalidation").hide();
        $("#entervalidation").hide();
        chat.server.starting(sessionStorage["username"], sessionStorage["roomname"]);

        $('#sendmessage').click(function () {
            if ($('#message').val() != '') {
                chat.server.send(sessionStorage["username"], $('#message').val());
                $('#message').val('');
            }
        });

        $(document).ready(function () {
            $('#message').keypress(function (event) {

                if (event.which == 13 && $('#message').val() != '') {
                    chat.server.send(sessionStorage["username"], $('#message').val());
                    $('#message').val('');
                }
            });
        });


        $("#create").click(function () {
            $("#create").hide();

            $('<input/>').attr({ type: 'text', id: 'roomname' }).appendTo('#creatingdiv');

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
            $("#creatingdiv").append(b);
        });

        $(document).ready(function () {
            $('#txtUserName').keypress(function (event) {

                if (event.which == 13) {
                    sessionStorage["username"] = $("#txtUserName").val();
                    sessionStorage["roomname"] = 'general';
                    var name = $("#txtUserName").val();
                    if (name.length > 0) {

                        chat.server.connect(name, $("#password").val());

                    }

                    else {
                        alert("Enter name..!!");
                    }
                }
            });
        });
        $("#btnLogin").click(function () {

            sessionStorage["username"] = $("#txtUserName").val();
            sessionStorage["roomname"] = 'general';
            var name = $("#txtUserName").val();
            if (name.length > 0) {

                chat.server.connect(name, $("#password").val());

            }
            else {
                alert("Enter name..!!");
            }

        });
        $("#submitregistration").click(function () {
            if ($("#r_username").val() != '' && $("#r_password").val() != '' && $("#r_email").val() != ''
                && $("#r_username").val().length < 20)
                chat.server.submitRegistration($("#r_username").val(), $("#r_password").val(), $("#r_email").val());
            else if ($("#r_username").val().length>20) {
                $("#namevalidation").show();
            }
            else {
                $("#entervalidation").show();

            }
        });
        $("#join").click(function () {
            window.location.href = 'http://localhost:48088/Home/Room?roomname=' + sessionStorage["roomname"] + '&username=' + $("username").val();
            chat.server.disconnect('join', sessionStorage["roomname"]);
        });
        $("#sendgroupmessage").click(function () {
            if ($("#groupmessage").val() != '') {
                chat.server.sendToGroup(sessionStorage["username"], $("#groupmessage").val(), sessionStorage["roomname"]);
                $("#groupmessage").val('');
            }
        })
        $('#groupmessage').keypress(function (event) {

            if (event.which == 13) {
                if ($("#groupmessage").val() != '') {
                    chat.server.sendToGroup(sessionStorage["username"], $("#groupmessage").val(), sessionStorage["roomname"]);
                    $("#groupmessage").val('');
                }
            }
        });
        $("#out").click(function () {
            chat.server.outFromRoom(sessionStorage["username"], sessionStorage["roomname"]);

        });
        $("#toGeneral").click(function () {
            chat.server.toGeneral(sessionStorage["username"], sessionStorage["roomname"]);

        });


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

function AddGroup(roomname) {

    var name = roomname;
    if (name.length > 17)
        name = roomname.substr(0, 17) + '...';
    $("#rooms").append('<p id="' + roomname + '" title="'+roomname+'">' + name + '</p>');
    $("#" + roomname).click(function () {
        $("#joindiv").show();
        sessionStorage["roomname"] = roomname;
    })

}
function AddJoinedGroup(roomname) {

    var name = roomname;
    if (name.length > 17)
        name = roomname.substr(0, 17) + '...';
    $("#joinedrooms").append('<p id="' + roomname + '" title="'+roomname+'">' + name + '</p>');
    $("#" + roomname).click(function () {
        $("#joindiv").show();
        sessionStorage["roomname"] = roomname;
    })

}
function AddUserToRoom(id, username) {

    var name = username;
    if (name.length > 17)
        name = username.substr(0, 17) + '...';
    $("#members").append('<p id="' + username + '"><b title="' + username + '">' + name + '</b></p>');

}
