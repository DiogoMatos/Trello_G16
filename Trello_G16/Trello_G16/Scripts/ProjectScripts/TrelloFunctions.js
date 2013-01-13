jQuery(function() {
    //*** AUTOCOMPLETE ***//
    $(":input[data-autocomplete]").each(function() {
        var input = $(this);
        input.autocomplete({ source: input.attr('data-autocomplete') });
    });

    //*** DRAG-N-DROP ***//
    $("#sortable").sortable();
    $("#sortable").disableSelection();
    
    //*** DATEPICKER ***//
    $(".datepicker").datepicker({
        dateFormat: "dd-mm-yy"
    });

});

var TrelloFunctions = {

    flagDrop: 0,

    saveCardsOrder: function (listid) {
        var list = [];     
        $('#listCards').each(function () {
            $(this).find('td').each(function() {
                list.push($(this).attr('id').replace("c_pos_", ""));
            });
        });
        //Commit changes
        $.ajax({
            url: "/Lists/ChangeCardsOrder",
            type: 'POST',
            dataType: "json",
            contentType: "application/json",
            traditional: true,
            data: JSON.stringify({
                list: list,
                id: listid
            }),
            success: function () {
                $("#changeMsgSuccess").fadeIn('slow');
                $("#changeMsgSuccess").fadeOut(5000);
            },
            error: function () {
                $("#changeMsgError").fadeIn('slow');
                $("#changeMsgError").fadeOut(5000);
            }
        });
    },
    
    clearLog: function () {
        $("#LogError").text('');
    },
    
    showOptions: function (boardId) {
        $("#cardOptions").fadeIn(0);
        $("#btnOption").val("Hide Options");
        $('#btnOption').attr('onclick', '').unbind('click');
        if(TrelloFunctions.flagDrop == 0)
            TrelloFunctions.fillListDropDown(boardId);
        $("#btnOption").click(function () {
            $("#btnOption").val("Show Options");
            $("#cardOptions").fadeOut(0);
            $('#btnOption').attr('onclick', '').unbind('click');
            $("#btnOption").click(TrelloFunctions.showOptions);
        });
    },
    
    fillListDropDown: function (boardId) {
        $.ajax({
            url: "/Boards/GetListsFromBoard",
            type: "POST",
            dataType: "json",
            contentType: "application/json",
            data: JSON.stringify({
                id: boardId
            }),
            success: function (jsonData) {
                TrelloFunctions.flagDrop++;
                for (var i = 0; i < jsonData.len; i += 2) {
                    $('#listDropDown')
                        .append($("<option></option>")
                        .attr("value", jsonData.list[i])
                        .text(jsonData.list[i + 1]));
                }
            }
        });
    },
    
    moveCard: function () {
        var listId = $('#listDropDown').find(":selected").attr("value");
        var cardId = $('#cardDropDown').find(":selected").attr("value");  

        //Commit changes
        $.ajax({
            url: "/Lists/MoveCard",
            type: 'POST',
            dataType: "json",
            contentType: "application/json",
            traditional: true,
            data: JSON.stringify({
                listId: listId,
                cardId: cardId
            }),
            success: function () {
                //Delete card from dropdown
                $('#cardDropDown').find(":selected").remove();
                //Reload Page
                location.reload();
            },
        });
    }
}