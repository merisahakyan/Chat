$(function () {
    var chat = $.connection.chatHub;
    chat.client.onRoomConnected = function (id, userName, members) {

        
        $('#chatBody').show();

        $('#userid').val(id);
        $('#username').val(userName);
        var name = $('#username').val();
        if ($('#username').val().length > 15)
            var name = userName.substr(0, 15) + '...';

        $('#header').html('<h3  title="' + $('#username').val() + '">Welcome, ' + name + '</h3>');


        for (var i = 0; i < allUsers.length; i++) {

            AddToRoom(members[i].ConnectionId, members[i].Name, $('#roomid').val());
        }
       
    }
    chat.client.onNewRoomConnecting=function(id,username,roomid)
    {
        AddUserToRoom(id, username,roomid);
    }

    $.connection.start().done(function () {
        $(document).ready(function () {
            chat.server.onroomconnection('','','');
        });
    });
});

function AddUser(id, userName) {

    var userId = $('#userid').val();

    if (userId != id) {
        var name = userName;
        if (userName.length > 17) {
            var name = userName.substr(0, 17) + '...';
        }
        $("#members").append('<p id="' + id + '"><b title="' + userName + '">' + name + '</b></p>');
    }
}
function AddUserToRoom(id,username,roomid)
{
    var rid = $("#roomid").val();
    var uid = $('#userid').val();
    if(uid !=id && rid==roomid)
    {
        $("#members").append('<p id="' + id + '"><b title="' + username + '">' + username + '</b></p>');
    }
}