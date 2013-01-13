
function VerifyInputNotNull(inputId) {
    var elem = $("#" + inputId);
    if (elem.length != 0) {
        if (elem.val().trim() == "") {
            return false;
        }
    }
    return true;
}

function VerifyElementNotNull(inputId, nameOfInput) {
    var b = VerifyInputNotNull(inputId);
    if (!b) {
        $("#inputError").text(nameOfInput + " cannot be blank.");
    } else {
        $("#inputError").text("");
    }
    return b;
}

function VerifyDateFormat(inputId) {
    var elem = $("#" + inputId);
    if (elem.length != 0) {
        var split = elem.val().split('-');
        if (split.length == 3 && !isNaN(split[0]) && split[0].length == 2 && !isNaN(split[1]) && split[1].length == 2 && !isNaN(split[2]) && split[2].length == 4) {
            return true;
        }
    }
    return false;
}

function VerifyNewCardInputs() {
    var error = "";
    var b1 = VerifyInputNotNull("name");
    var b2 = VerifyDateFormat("end_date");
    if (!b1) {
        error += "Name cannot be blank. ";
    }
    if (!b2) {
        error += "End Date must have the 'dd-mm-yyyy' format. ";
    }
    $("#inputError").text(error);
    return (b1 && b2);
}