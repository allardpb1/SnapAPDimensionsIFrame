$(document).ready(function () {
    $(".dimension").change(function () {
        var dimensionListViewModel = [];
        $(".dimension").each(function () {
            if ($(this).find("option:selected").text() != '' && $(this).find("option:selected").text() != 'Mapping Required') {
                dimensionListViewModel.push({
                    ObjectName: this.id,
                    ObjectValue: $(this).find("option:selected").text(),
                    OneToOneRelations: $(this).find("option:selected").attr('relation')
                });
            }
        });

        $.ajax({
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            url: 'Dimension/GetRestrictions',
            type: 'POST',
            data: JSON.stringify(dimensionListViewModel)
        }).done(function (dataArray) { 
            $(".dimension > option:hidden").each(function () {
                $(this).show();
            });
            dataArray.forEach(function (data) {
                let objectValue = data.objectValue;
                const dimSplit = objectValue.split(",");
                $("#" + data.objectName.toUpperCase() + " > option").each(function () {
                    if (dimSplit.indexOf(this.value) == -1 && dimSplit.indexOf(this.text) == -1) {
                        $(this).hide();
                    }
                });
            });
        });  
    });
});

function GetXML() {
    var xmlBuilder = ""
    // The reason why we're not doing an easy loop here through all options is because the XML in Intacct needs to be inserted in a specific order.
    if ($("#LOCATION").length) {
        let fullText = $("#LOCATION").find("option:selected").text();
        if (fullText != "") {
            const textSplit = fullText.split("--");
            xmlBuilder += "<LOCATIONID>" + textSplit[0] + "</LOCATIONID>";
        }
    }
    if ($("#DEPARTMENT").length) {
        let fullText = $("#DEPARTMENT").find("option:selected").text();
        if (fullText != "") {
            const textSplit = fullText.split("--");
            xmlBuilder += "<DEPARTMENTID>" + textSplit[0] + "</DEPARTMENTID>";
        }
    }
    if ($("#PROJECT").length) {
        let fullText = $("#PROJECT").find("option:selected").text();
        if (fullText != "") {
            const textSplit = fullText.split("--");
            xmlBuilder += "<PROJECTID>" + textSplit[0] + "</PROJECTID>";
        }
    }
    if ($("#TASK").length) {
        let fullText = $("#TASK").find("option:selected").text();
        if (fullText != "") {
            const textSplit = fullText.split("--");
            xmlBuilder += "<TASKID>" + textSplit[0] + "</TASKID>";
        }
    }
    if ($("#COSTTYPE").length) {
        let fullText = $("#COSTTYPE").find("option:selected").text();
        if (fullText != "") {
            const textSplit = fullText.split("--");
            xmlBuilder += "<COSTTYPEID>" + textSplit[0] + "</COSTTYPEID>";
        }
    }
    if ($("#CUSTOMER").length) {
        let fullText = $("#CUSTOMER").find("option:selected").text();
        if (fullText != "") {
            const textSplit = fullText.split("--");
            xmlBuilder += "<CUSTOMERID>" + textSplit[0] + "</CUSTOMERID>";
        }
    }
    if ($("#VENDOR").length) {
        let fullText = $("#VENDOR").find("option:selected").text();
        if (fullText != "") {
            const textSplit = fullText.split("--");
            xmlBuilder += "<VENDORID>" + textSplit[0] + "</VENDORID>";
        }
    }
    if ($("#EMPLOYEE").length) {
        let fullText = $("#EMPLOYEE").find("option:selected").text();
        if (fullText != "") {
            const textSplit = fullText.split("--");
            xmlBuilder += "<EMPLOYEEID>" + textSplit[0] + "</EMPLOYEEID>";
        }
    }
    if ($("#ITEM").length) {
        let fullText = $("#ITEM").find("option:selected").text();
        if (fullText != "") {
            const textSplit = fullText.split("--");
            xmlBuilder += "<ITEMID>" + textSplit[0] + "</ITEMID>";
        }
    }
    if ($("#CLASS").length) {
        let fullText = $("#CLASS").find("option:selected").text();
        if (fullText != "") {
            const textSplit = fullText.split("--");
            xmlBuilder += "<CLASSID>" + textSplit[0] + "</CLASSID>";
        }
    }
    if ($("#CONTRACT").length) {
        let fullText = $("#CONTRACT").find("option:selected").text();
        if (fullText != "") {
            const textSplit = fullText.split("--");
            xmlBuilder += "<CONTRACTID>" + textSplit[0] + "</CONTRACTID>";
        }
    }
    if ($("#WAREHOUSE").length) {
        let fullText = $("#WAREHOUSE").find("option:selected").text();
        if (fullText != "") {
            const textSplit = fullText.split("--");
            xmlBuilder += "<WAREHOUSEID>" + textSplit[0] + "</WAREHOUSEID>";
        }
    }

    $("[userdefined='True']").each(function () {
        var value = $(this).find("option:selected").text();
        if (value != "") {
            xmlBuilder += "<" + this.id + ">" + value + "</" + this.id + ">";
        }
    });

    $("#xmlresult").text(xmlBuilder);
}