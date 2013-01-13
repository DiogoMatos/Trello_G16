(function () {

    window.addEventListener("DOMContentLoaded", function () {
        var forms = document.getElementsByTagName("form");

        for (var i = 0; i < forms.length; i++) {
            addValidateFormEvent(forms[i]);
        }
        $("#DivLogError").fadeIn(0);
    });

    var asyncUtils = {
        waitingAsyncRequests: [],
        asyncRequestCount: 0,
        submitEvent: null,
        requestFail: false
    };

    var addValidateFormEvent = function (form) {

        form.addEventListener("submit", submitEventFunction);
    };

    var submitEventFunction = function (event) {

        asyncUtils.requestFail = false;
        asyncUtils.submitEvent = event;

        var inputs = event.target.getElementsByTagName("input");

        var valMsgFields = event.target.getElementsByClassName("field-validation-valid");

        for (var idx = 0; idx < valMsgFields.length; idx++) {

            if (valMsgFields[idx].hasChildNodes()) {
                var childNodes = valMsgFields[idx].childNodes;

                for (var j = 0; j < childNodes.length; j++) {
                    valMsgFields[idx].removeChild(childNodes[j]);
                }
            }
        }

        for (var i = 0; i < inputs.length; i++) {
            if (inputs[i].type === 'submit') {
                inputs[i].disabled = true;
            }

            if (!validateInput(inputs[i], valMsgFields)) {
                event.preventDefault();
                asyncUtils.requestFail = true;
            }
        }

        if (asyncUtils.waitingAsyncRequests.length === 0) {
            for (i = 0; i < inputs.length; i++) {
                inputs[i].disabled = false;
            }
            return;
        }

        event.preventDefault();
        for (i = 0; i < asyncUtils.waitingAsyncRequests.length; i++) {
            var xhr = asyncUtils.waitingAsyncRequests[i];
            xhr.send();
        }

        asyncUtils.waitingAsyncRequests = [];
    };

    var validateInput = function (input, valMsgFields) {

        if (input.getAttribute("data-val")) {

            input.disabled = true;

            var attributes = input.attributes;
            var valMsgField = getValMsgField(input, valMsgFields);

            for (var i = 0; i < attributes.length; i++) {
                var val = valContainer[attributes[i].name];
                if (val && !val(input, valMsgField)) {
                    if (valMsgField) {
                        var valMsg = document.createTextNode(attributes[i].value);
                        valMsgField.appendChild(valMsg);
                        $("#LogError").append(attributes[i].value + '\n');
                    }
                    return false;
                }
            }
        }

        return true;
    };

    var getValMsgField = function (input, valMsgFields) {

        for (var j = 0; j < valMsgFields.length; j++) {

            var valMsgFieldFor = valMsgFields[j].getAttribute("data-valmsg-for");
            if (valMsgFieldFor === input.getAttribute("id")) {
                return valMsgFields[j];
            }
        }

        return null;
    };

    var valContainer = {};

    valContainer["data-val-required"] = function (input) {

        return input.value.trim();
    };

    valContainer["data-val-equalto"] = function (input) {

        var otherInputName = input.getAttribute("data-val-equalto-other");
        otherInputName = otherInputName.substr(2, otherInputName.length);

        var otherInputNode = document.getElementById(otherInputName);

        return otherInputNode.value === input.value;
    };

    valContainer["data-val-type-email"] = function (input) {
        //utilização de uma expressão regular 
        var emailRegExp = /\S+@\S+\.\S+/;

        return emailRegExp.test(input.value);
    };

    valContainer["data-val-name-user"] = function (input, valMsgField) {

        if (!input.value.trim()) {
            return true;
        }

        var link = input.getAttribute("data-val-name-link");
        link = link + "?name=" + input.value;

        assyncRequestFunc(input.getAttribute('data-val-name-user'), link, valMsgField);

        return true;
    };

    var assyncRequestFunc = function (attribute, link, valMsgField) {

        var xhr = new XMLHttpRequest();
        xhr.open("GET", link);

        xhr.onreadystatechange = function () {
            if (xhr.readyState === XMLHttpRequest.DONE) {

                var inputs = asyncUtils.submitEvent.target.getElementsByTagName('input');
                for (var i = 0; i < inputs.length; i++) {
                    inputs[i].disabled = false;
                }

                if (xhr.status === 200) {
                    if (xhr.responseText === "False") {
                        if (valMsgField) {
                            var valMsg = document.createTextNode(attribute);
                            valMsgField.appendChild(valMsg);
                            $("#LogError").append(attribute + '\n');
                        }
                        asyncUtils.requestFail = true;
                    } else {
                        if (asyncUtils.asyncRequestCount == 1 && !asyncUtils.requestFail) {
                            asyncUtils.submitEvent.target.removeEventListener('submit', submitEventFunction);
                            asyncUtils.submitEvent.target.submit();
                        }
                    }
                }
                --asyncUtils.asyncRequestCount;
            }
        };

        asyncUtils.waitingAsyncRequests[asyncUtils.waitingAsyncRequests.length] = xhr;
        asyncUtils.asyncRequestCount++;
    };
})()
