/// <reference path="highcharts.js" />
//public Nullable<decimal> Cost { get; set; }
//public string JobDescription { get; set; }
var resultobj = null; var dataobject = [];
var nameobjext = [];

var costupdationobj = [];
var costdataobj = [];
var costnameobj = [];
var Recievabledataobj = [];
var Recievablenameobj = [];

 var Payeblenameobj =[];
 var Payebledataobj = [];


$(document).ready(function () {
    GetData();
    GetCostUpdation();
    GetReceivable();
    GetPAYBLE();



});


//Start JOB

function GetData() {
    $.ajax({

        url: "/Job/GETLOCALCostForDashboard/",
        type: "GET",
        dataType: "json",
        success: function (data) {
            resultobj = data;
            loadjobchart();
        }
    });
}



function loadjobchart() {
    for (var i = 0; i < resultobj.length; i++) {
        dataobject.push(resultobj[i].Cost);
        nameobjext.push(resultobj[i].JobDescription);
    }
    var charts = [],
        $containers = $('#JobChart'),
        datasets = [{
            name: 'JOB',
            data: dataobject
        }];
    $.each(datasets, function (i, dataset) {
        charts.push(new Highcharts.Chart({

            chart: {
                renderTo: $containers[i],
                type: 'bar'

            },

            title: {
                text: "",
                align: 'left',
                x: i === 0 ? 90 : 0
            },

            credits: {
                enabled: false
            },

            xAxis: {
                categories: nameobjext,
                labels: {
                    enabled: i === 0
                }
            },

            yAxis: {
                allowDecimals: false,
                title: {
                    text: null
                },
                min: 0,
                max: 5000000
            },


            legend: {
                enabled: false
            },

            series: [dataset]

        }));
    });

}

//End JOB

//Cost Updation
function GetCostUpdation() {
    $.ajax({

        url: "/CostUpdation/GetAllCurrencyCostUpdation/",
        type: "GET",
        dataType: "json",
        success: function (data) {
            costupdationobj = data;
            loadcostupdationchart();

        }
    });
}

function loadcostupdationchart() {
    
    costnameobj = ["Provision", "Updated"];
    costdataobj.push(costupdationobj[0].Provision);
    costdataobj.push(costupdationobj[0].Updated);
    var charts = [],
    $containers = $('#CostUpdationChart'),
    datasets = [{
        name: 'COSTUPDATION',
        data: costdataobj
    }];


    $.each(datasets, function (i, dataset) {
        charts.push(new Highcharts.Chart({

            chart: {
                renderTo: $containers[i],
                type: 'column'

            },

            title: {
                text: "",
                align: 'left',
                x: i === 0 ? 90 : 0
            },

            credits: {
                enabled: false
            },

            xAxis: {
                categories: costnameobj,
                labels: {
                    enabled: i === 0
                }
            },

            yAxis: {
                allowDecimals: false,
                title: {
                    text: null
                },
                min: 0,
                max: 5000000
            },


            legend: {
                enabled: false
            },

            series: [dataset]

        }));
    });
}

//End Cost Updation

//Receibvable
function GetReceivable() {
    $.ajax({
      
        url: "/CustomerReciept/GetAllCurrencyCustReciept/",
        type: "GET",
        dataType: "json",
        success: function (data) {
           
            custRecievableobj = data;
            loadreceivable();

        }
    });


}

function loadreceivable() {
    debugger;
    Recievablenameobj = ["Invoiced", "Recieved"];
    Recievabledataobj.push(custRecievableobj[0].Invoiced);
    Recievabledataobj.push(custRecievableobj[0].Recievable);

    var charts = [],
     $containers = $('#receivable'),
     datasets = [{
         name: 'RECEIVABLE',
         data: Recievabledataobj
     }];
    $.each(datasets, function (i, dataset) {
        charts.push(new Highcharts.Chart({

            chart: {
                renderTo: $containers[i],
                type: 'column'

            },

            title: {
                text: "",
                align: 'left',
                x: i === 0 ? 90 : 0
            },

            credits: {
                enabled: false
            },

            xAxis: {
                categories: Recievablenameobj,
                labels: {
                    enabled: i === 0
                }
            },

            yAxis: {
                allowDecimals: false,
                title: {
                    text: null
                },
                min: 0,
                max: 5000000
            },


            legend: {
                enabled: false
            },

            series: [dataset]

        }));
    });
}

//End Receibvable


//PAYBLE
function GetPAYBLE() {
    $.ajax({

        url: "/SupplierPayment/GetAllCurrencySupplierPayble/",
        type: "GET",
        dataType: "json",
        success: function (data) {
            SupplierPayobj = data;
            loadpayable();

        }
    });


}


function loadpayable() {
    Payeblenameobj = ["Invoiced", "Paid"];
    Payebledataobj.push(SupplierPayobj[0].Invoiced);
    Payebledataobj.push(SupplierPayobj[0].Paid);



    var charts = [],
        $containers = $('#payble'),
        datasets = [{
            name: 'PAYBLE',
            data: Payebledataobj
        }];


    $.each(datasets, function (i, dataset) {
        charts.push(new Highcharts.Chart({

            chart: {
                renderTo: $containers[i],
                type: 'column'

            },

            title: {
                text: "",
                align: 'left',
                x: i === 0 ? 90 : 0
            },

            credits: {
                enabled: false
            },

            xAxis: {
                categories: Payeblenameobj,
                labels: {
                    enabled: i === 0
                }
            },

            yAxis: {
                allowDecimals: false,
                title: {
                    text: null
                },
                min: 0,
                max: 5000000
            },


            legend: {
                enabled: false
            },

            series: [dataset]

        }));
    });
}


//End PAYBLE