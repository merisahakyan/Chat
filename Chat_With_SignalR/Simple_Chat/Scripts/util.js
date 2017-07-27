$(function () {
    $.fn.easyNotify = function (options) {

        var settings = $.extend({
            title: "Notification",
            options: {
                body: "message",
                icon: "",
                lang: 'pt-BR',
                onClose: "",
                onClick: "",
                onError: ""
            }
        }, options);

        this.init = function () {
            var notify = this;
            if (!("Notification" in window)) {
                alert("This browser does not support desktop notification");
            } else if (Notification.permission === "granted") {

                var notification = new Notification(settings.title, settings.options);

                notification.onclose = function () {
                    if (typeof settings.options.onClose == 'function') {
                        settings.options.onClose();
                    }
                };

                notification.onclick = function () {
                    if (typeof settings.options.onClick == 'function') {
                        settings.options.onClick();
                    }
                };

                notification.onerror = function () {
                    if (typeof settings.options.onError == 'function') {
                        settings.options.onError();
                    }
                };

            } else if (Notification.permission !== 'denied') {
                Notification.requestPermission(function (permission) {
                    if (permission === "granted") {
                        notify.init();
                    }

                });
            }

        };

        this.init();
        return this;
    };

    var chat = $.connection.chatHub;

    chat.client.addMessage = function (message) {
        if (sessionStorage["roomname"] == 'general') {
            AddMessage(message.ID, message.UserName, message.Message, message.Time)
            $("#chatroom").scrollTop($("#chatroom")[0].scrollHeight);

            if (message.UserName != sessionStorage["username"]) {
                var options = {
                    title: sessionStorage["roomname"],
                    options: {
                        body: name + ': ' + message.Message,
                        lang: 'en-US',
                    }
                };
                $("#easyNotify").easyNotify(options);
            }
        }
    };
    chat.client.desktopNot = function (roomname, name, message) {
        var options = {
            title: sessionStorage["roomname"],
            options: {
                body: name + ': ' + message,
                lang: 'en-US',
            }
        };
        if (name != sessionStorage["username"])
            $("#easyNotify").easyNotify(options);
    }
    chat.client.showAllMessages = function (roomname, messages) {

        for (var i = 0; i < messages.length; i++)
            AddMessage(messages[i].ID, messages[i].UserName, messages[i].Message, messages[i].Time);
        $("#chatroom").scrollTop($("#chatroom")[0].scrollHeight);
    }

    chat.client.addMessageToRoom = function (message) {

        if (message.RoomName == sessionStorage["roomname"]) {
            AddMessage(message.ID, message.UserName, message.Message, message.Time);
            $("#chatroom").scrollTop($("#chatroom")[0].scrollHeight);
        }
    };
    chat.client.onEditingMsg = function (id, newmsg) {
        $("#m" + id).empty();
        $("#m" + id).append(newmsg);
        $("#datetime" + id).append(' edited');
    }
    chat.client.onConnected = function (id, userName, allUsers, joinedrooms, rooms) {
        $("username").val(userName);

        var name = userName;
        if (userName.length > 15)
            var name = userName.substr(0, 15) + '...';
        $("#welcome").append(name + '!');
        $("#welcome").attr('title', userName);

        for (var i = 0; i < allUsers.length; i++) {

            AddUser(allUsers[i].ConnectionId, allUsers[i].UserName);
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
    chat.client.onUserDisconnected = function (userName) {
        $('#' + userName).remove();
    }
    chat.client.onLogOut = function () {
        sessionStorage["roomname"] = '';
        sessionStorage["username"] = '';
        window.location.href = 'http://localhost:48088//Home/Login';
    }


    chat.client.onNewGroupCreating = function (roomname) {
        $("#roomvalidation").hide();
        AddGroup(roomname);
    }
    chat.client.onRoomConnected = function (id, userName, members) {


        $('#chatBody').show();

        for (var i = 0; i < members.length; i++) {
            AddUserToRoom(members[i].ConnectionId, members[i].UserName);
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
    chat.client.outRoomButton = function (roomname) {
        $("#" + roomname).remove();
        AddGroup(roomname);
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
            $("#validation").show();
            $("#validation").append('Username or email already in use <br/>');
        }
    }
    chat.client.onLoginFail = function () {
        $("#loginvalidation").empty();
        $("#loginvalidation").show();
        $("#loginvalidation").append('Login failed');
    }
    chat.client.onLogin = function () {
        window.location.href = 'http://localhost:48088/Home/Index';
    }
    chat.client.failedRoomCreating = function () {
        $("#roomvalidation").show();

    }
    chat.client.onCallingHistory = function (history) {
        if (history.length > 0) {
            var b = $('<button>', {
                text: 'Close',
                id: 'closehistory',
                click: function () {
                    $("#history").empty();
                }
            })
            b.css('float', 'right');
            $("#history").append(b);
        }
        for (var i = 0; i < history.length; i++) {
            $("#history").append('</br><div>' + history[i].Message + '</br ><span style="font-size:60%">' + history[i].Edited.substr(0, 10) + ' ' + history[i].Edited.substr(11, 5) + '</span></div>');

        }
    }

    $.connection.hub.start().done(function () {

        if (window.history && window.history.pushState) {
            window.history.pushState('forward', null, './#forward');
            $(window).on('popstate', function () {
                if (sessionStorage["roomname"] != 'general' && sessionStorage["roomname"] != '') {
                    chat.server.toGeneral(sessionStorage["username"], sessionStorage["roomname"]);
                }
            });
        }

        $("#joindiv").hide();
        $("#activate").hide();
        $("#validation").hide();
        $("#loginvalidation").hide();
        $("#roomvalidation").hide();


        if (sessionStorage["username"] != '' && sessionStorage["username"] != null)
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
            $("#roomvalidation").hide();
            $("#create").hide();
            $('<input/>').attr({
                type: 'text',
                id: 'roomname1'
            }).appendTo('#creatingdiv');

            var b = $('<button>',
    {
        text: 'Create',
        id: 'createroom',
        click: function (roomname) {
            $("#create").show();
            roomname = $('#roomname1').val();
            $("#roomname1").remove();
            $("#createroom").remove();
            chat.server.create(roomname);

        }
    });
            $("#creatingdiv").append(b);
        });

        $(document).ready(function () {
            $('#txtUserName').keypress(function (event) {
                $("#loginvalidation").empty();
                if (event.which == 13) {
                    sessionStorage["username"] = $("#txtUserName").val();
                    sessionStorage["roomname"] = 'general';
                    var name = $("#txtUserName").val();
                    if (name.length > 0 && $("#password").val().length > 0) {

                        chat.server.connect(name, $("#password").val());
                    }

                    else {
                        $("#loginvalidation").empty();
                        $("#loginvalidation").show();
                        $("#loginvalidation").append('Login Failed!');
                    }
                }
            });
            $('#password').keypress(function (event) {
                $("#loginvalidation").empty();
                if (event.which == 13) {
                    sessionStorage["username"] = $("#txtUserName").val();
                    sessionStorage["roomname"] = 'general';
                    var name = $("#txtUserName").val();
                    if (name.length > 0 && $("#password").val().length > 0) {

                        chat.server.connect(name, $("#password").val());
                    }

                    else {
                        $("#loginvalidation").empty();
                        $("#loginvalidation").show();
                        $("#loginvalidation").append('Login Failed!');
                    }
                }
            });
        });
        $("#btnLogin").click(function () {

            sessionStorage["username"] = $("#txtUserName").val();
            sessionStorage["roomname"] = 'general';
            var name = $("#txtUserName").val();
            if (name.length > 0 && $("#password").val().length > 0) {
                chat.server.connect(name, $("#password").val());
            }
            else {
                $("#loginvalidation").empty();
                $("#loginvalidation").show();
                $("#loginvalidation").append('Login Failed!');
            }

        });
        $('#r_username').keypress(function (event) {
            $("#validation").empty();
            if (event.which == 13) {
                Registration();
            }
        });
        $('#r_password').keypress(function (event) {
            $("#validation").empty();
            if (event.which == 13) {
                Registration();
            }
        });
        $('#r_email').keypress(function (event) {
            $("#validation").empty();
            if (event.which == 13) {
                Registration();
            }
        });
        $("#submitregistration").click(function () {
            Registration();
        });

        function Registration() {
            $("#validation").empty();
            $("#validation").show();
            var email = new RegExp('^[a-zA-Z0-9.!#$%&\'*+\/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$');

            if ($("#r_username").val() != '' && $("#r_password").val() != '' && $("#r_email").val() != ''
                && $("#r_username").val().length < 20 && $("#r_password").val().length > 5 && email.test($("#r_email").val()))
                chat.server.submitRegistration($("#r_username").val(), $("#r_password").val(), $("#r_email").val());
            if ($("#r_username").val().length > 20) {
                $("#validation").append('Username must contain less than 20 characters <br/>')
            }
            if (!email.test($("#r_email").val())) {
                $("#validation").append('Uncorrect email <br/>')
            }
            if ($("#r_password").val().length < 6) {
                $("#validation").append('Password must contain more than 6 characters <br/>')
            }
            if ($("#r_username").val() == '' || $("#r_password").val() == '' || $("#r_email").val() == '') {
                $("#validation").append('Enter all properties <br/>')
            }
        }


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
        $("#logout").click(function () {
            chat.server.logOut(sessionStorage["username"]);
        });

    });

    function AddGroup(roomname) {
        if (roomname != 'general') {
            var name = roomname;
            if (name.length > 17)
                name = roomname.substr(0, 17) + '...';
            $("#rooms").append('<p id="' + roomname + '" title="' + roomname + '">' + name + '</p>');

            $("#" + roomname).mouseenter(function () {
                var b = $('<button>',
            {
                text: 'Join',
                id: 'join' + roomname,
                click: function () {
                    sessionStorage["roomname"] = roomname;
                    window.location.href = 'http://localhost:48088/Home/Room?roomname=' + sessionStorage["roomname"] + '&username=' + sessionStorage["username"];
                    chat.server.disconnect('join', sessionStorage["roomname"]);
                }
            });
                b.css('float', 'right');
                $("#" + roomname).append(b);
            });
            $("#" + roomname).mouseleave(function () {
                $("#join" + roomname).remove();

            });
        }
    }
    function AddJoinedGroup(roomname) {

        var name = roomname;
        if (name.length > 17)
            name = roomname.substr(0, 17) + '...';
        $("#joinedrooms").append('<p id="' + roomname + '" title="' + roomname + '">' + name + '</p>');

        $("#" + roomname).mouseenter(function () {
            var b = $('<button>',
        {
            text: 'Join',
            id: 'join' + roomname,
            click: function () {
                sessionStorage["roomname"] = roomname;
                window.location.href = 'http://localhost:48088/Home/Room?roomname=' + sessionStorage["roomname"] + '&username=' + sessionStorage["username"];
                chat.server.disconnect('join', sessionStorage["roomname"]);

            }
        });
            b.css('float', 'right');
            var c = $('<button>',
        {
            text: 'Out',
            id: 'out' + roomname,
            click: function () {
                chat.server.outButton(sessionStorage["username"], roomname);
            }
        });
            c.css('float', 'right');
            $("#" + roomname).append(c);
            $("#" + roomname).append(b);


        });
        $("#" + roomname).mouseleave(function () {
            $("#join" + roomname).remove();
            $("#out" + roomname).remove();

        });
    }

    function AddMessage(id, username, message, time) {
        $('#chatroom').append('</br><p id="' + id + '"><b>' + username + '</b >: ' + '<span id="m' + id + '">' + message + '</span>  </p>');
        $("#" + id).append('<span id="datetime' + id + '" style="font-size:60%"></br>' + time.substr(0, 10) + ' ' + time.substr(11, 5) + '</span>');
        var flag = true;
        if (username == sessionStorage["username"]) {

            $("#" + id).mouseenter(function () {
                if (flag) {
                    var b = $('<button>',
           {
               text: 'Edit',
               id: 'edit' + id,
               click: function () {
                   flag = false;
                   $("#m" + id).hide();
                   $("#edit" + id).hide();
                   $("#datetime" + id).remove();
                   $("#history").empty();
                   var edit = $('<input>', {
                       id: 'foredit' + id,
                       val: $("#m" + id).text(),
                       keypress: function (e) {
                           if (e.which == 13) {
                               if ($("#foredit" + id).val() != '') {
                                   flag = true;
                                   $("#edit" + id).show();
                                   $("#m" + id).show();
                                   var msg = $("#foredit" + id).val();
                                   $("#foredit" + id).remove();
                                   $("#" + id).append('<span id="datetime' + id + '" style="font-size:60%"></br>' + time.substr(0, 10) + ' ' + time.substr(11, 5) + '</span>');
                                   chat.server.editMessage(id, msg);
                               }
                           }
                       }
                   })

                   $("#" + id).append(edit);
               }
           });
                    b.css('float', 'right');
                    $("#" + id).append(b);

                    var c = $('<button>', {
                        text: 'History',
                        id: 'h' + id,
                        click: function () {
                            $("#history").empty();
                            chat.server.getHistory(id);
                        }
                    })
                    c.css('float', 'right');
                    $("#" + id).append(c);
                }

            });
            $("#" + id).mouseleave(function () {
                $("#edit" + id).remove();
                $("#h" + id).remove();
            })
        }
    }
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


function AddUserToRoom(id, username) {

    var name = username;
    if (name.length > 17)
        name = username.substr(0, 17) + '...';
    $("#members").append('<p id="' + username + '"><b title="' + username + '">' + name + '</b></p>');

}
function not(roomname, username, message) {
    var options = {
        title: roomname,
        options: {
            body: username + ': ' + message,
            lang: 'en-US',
        }
    };
    $("#easyNotify").easyNotify(options);
}