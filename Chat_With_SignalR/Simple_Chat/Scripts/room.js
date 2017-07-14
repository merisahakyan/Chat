$(function () {
    var chat = $.connection.chatHub;
    chat.client.onRoomConnected = function (id, userName, members) {

        
        $('#chatBody').show();

        $('#hdId').val(id);
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

    $(document).ready(function () {
        
        chat.server.onroomconnect($("#userid").val(), $("#username").val(), $("#roomname").val());
    });
 
});

function AddUser(id, userName) {

    var userId = $('#hdId').val();

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
    var uid = $('#hdId').val();
    if(uid !=id && rid==roomid)
    {
        $("#members").append('<p id="' + id + '"><b title="' + username + '">' + username + '</b></p>');
    }
}