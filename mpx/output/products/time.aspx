﻿<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="time.aspx.cs" Inherits="P_Time" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <meta charset="UTF-8">
    <meta name="google" content="notranslate">
    <meta http-equiv="Content-Language" content="en">
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js"></script>

    <%-- GOOGLE CHARTS IMPLEMENTATION. --%>
    <%--TO-DO
    LINK WINDOW ONRESIZE TRIGGER TO MYCHART.DRAW--%>

    <script type="text/javascript">

        google.charts.load('current', { 'packages': ['corechart', 'controls', 'table'] });
        google.charts.setOnLoadCallback(startGoogle);

        //THIS FUNCTION TRIGGERS WHEN GOOGLE CHARTS LIBRARIES ARE LOADED; RUNS SERVER-SIDE CODE TO QUERY USER MODEL
        //-- TO DO --
        //-- TRY TO PASS REFERENCE TO CURRENT CLASS TO SERVER-SIDE SUCH AS "THIS"

        function startGoogle() {

            $.ajax(
                {

                    type: 'POST',
                    dataType: 'json',
                    contentType: 'application/json',
                    url: '/api/mpx/v1/output/p_time.aspx/SetGoogleData',

                    success: function (response) {
                        drawVisualization(response.d); // calling method  
                    },

                    error: function () {
                        alert("Error loading data! Please try again.");
                    }

                });

        }

        //THIS FUNCTION INITIALIZES THE CHART -- SPL 1/14/2018
        function drawVisualization(dataValues) {

            var data = new google.visualization.DataTable();
            data.addColumn({ type: 'string', role: 'domain' });
            data.addColumn({ type: 'string', role: 'annotation', label: 'NAME' });
            data.addColumn({ type: 'string', role: 'annotation', label: 'SCENARIO' });
            data.addColumn({ 'type': 'string', 'role': 'tooltip', 'p': { 'html': true } })
            data.addColumn('number', 'WAIT FOR EQUIPMENT');
            data.addColumn('number', 'WAIT FOR LABOR');
            data.addColumn('number', 'SETUP');
            data.addColumn('number', 'RUN');
            data.addColumn('number', 'WAIT FOR LOT');
            data.addColumn({ type: 'number', role: 'annotation', label: 'MCT' });
            data.addColumn({ type: 'number', role: 'annotation', label: 'RANK' });

            var maxLength = 0;

            for (var i = 0; i < dataValues.length; i++) {

                var total = 0;

                if (Number(dataValues[i][3]) != null) {
                    var equip = Number(dataValues[i][3]);
                } else {
                    var equip = 0;
                }

                if (Number(dataValues[i][4]) != null) {
                    var labor = Number(dataValues[i][4]);
                } else {
                    var labor = 0;
                }

                if (Number(dataValues[i][5]) != null) {
                    var setup = Number(dataValues[i][5]);
                } else {
                    var setup = 0;
                }

                if (Number(dataValues[i][6]) != null) {
                    var run = Number(dataValues[i][6]);
                } else {
                    var run = 0;
                }

                if (Number(dataValues[i][7]) != null) {
                    var lot = Number(dataValues[i][7]);
                } else {
                    var lot = 0;
                }

                total = equip + labor + setup + run + lot;

                var name = dataValues[i][0];
                var scenario = dataValues[i][1];
                var concat;

                if (scenario == "BASECASE") {
                    concat = name;
                } else {
                    concat = name + '-' + scenario;
                }

                maxLength = Math.max(maxLength, concat.length);

                if (total != 0) {
                    data.addRow([concat, name, scenario, myFunction(name, scenario, equip, labor, setup, run, lot, total), equip, labor, setup, run, lot, total, null]);
                }


            }

            if (data.getNumberOfRows() != 0) {

                var topPadding = 60;
                var bottomPadding = maxLength * 10;
                var windowHeight = document.getElementById("dvChart").clientHeight - topPadding;

                var myHeight = 100 - (((windowHeight - topPadding - bottomPadding) / windowHeight) * 100) - 2;

                myHeight = myHeight + "%";

                data.sort([1]);

                var uniqueNames = [];
                var numRows = data.getNumberOfRows();
                uniqueNames.push(data.getValue(0, 1));
                var uniqueNameCounter = 0;
                for (var i = 0; i < numRows; i++) {
                    var tempName = data.getValue(i, 1);
                    if (uniqueNames[uniqueNameCounter] != tempName) {
                        uniqueNames.push(data.getValue(i, 1));
                        uniqueNameCounter++;
                    }
                }

                var numNames = uniqueNames.length;
                var nameValues = [];



                for (var i = 0; i < numNames; i++) {
                    tempView = new google.visualization.DataView(data);
                    tempView.setRows(tempView.getFilteredRows([{ column: 1, value: uniqueNames[i] }]));
                    var nameMax = tempView.getColumnRange(9);
                    var myObject = { name: uniqueNames[i], value: nameMax.max };
                    nameValues.push(myObject);
                }

                nameValues.sort(function (a, b) { return b.value - a.value })

                for (var i = 0; i < numRows; i++) {
                    var myIndex = nameValues.findIndex(myRank);

                    function myRank(a) {
                        return a.name == data.getValue(i, 1);
                    }

                    data.setValue(i, 10, myIndex);
                }

                formatter = new google.visualization.NumberFormat({
                    pattern: "###.##"
                });


                formatter.format(data, 4);
                formatter.format(data, 5);
                formatter.format(data, 6);
                formatter.format(data, 7);
                formatter.format(data, 8);
                formatter.format(data, 9);

                data.sort([10, { column: 9, desc: true }]);

                var setup = "#f39c12";
                var run = '#27ae60';
                var red = '#c0392b';
                var darker = '#9E2E24';
                var darkest = '#82261E';
                var custom = '#50A2A7';

                options = {
                    width: '100%',
                    height: '100%',
                    isStacked: 'true',
                    backgroundColor: { fill: 'transparent' },
                    fontName: 'Roboto',
                    annotations: {

                        textStyle: { fontName: 'Roboto', fontSize: 12, bold: false, opacity: 0, color: 'transparent' },
                        domain: { stem: { color: 'transparent', opacity: 0 } },
                        datum: { stem: { color: 'transparent', opacity: 0 } }
                    },
                    tooltip: { ignoreBounds: false, isHtml: true },
                    colors: [red, darker, setup, run, darkest],
                    hAxis: { textPosition: 'out', textStyle: { color: '#555', bold: false, fontSize: 12 }, slantedText: true, slantedTextAngle: 90, maxAlternation: 1, viewWindowMode: 'pretty' },
                    focusTarget: 'category',
                    vAxis: { textStyle: { color: '#555', fontSize: 12 }, format: '#,###.##', viewWindowMode: 'pretty', baseline: 0, minValue: 0, textPosition: 'out', minorGridlines: { count: 1 }, gridlines: { count: -1 } },
                    chartArea: { left: '6%', top: topPadding, bottom: bottomPadding, width: '90%', height: myHeight, backgroundColor: 'transparent' },
                    legend: { 'position': 'top', 'alignment': 'center', textStyle: { color: '#555', fontSize: 14, bold: false } },
                    titlePosition: 'none', axisTitlesPosition: 'out'

                };

                var dashboard = new google.visualization.Dashboard(document.getElementById("dvChart"))

                myChart = new google.visualization.ChartWrapper({
                    'chartType': 'ColumnChart',
                    'containerId': 'dvMyChart',
                    'options': options,
                    'view': { "columns": [0, 1, 2, 3, 4, 5, 6, 7, 8] }

                });

                var classes = {
                    headerCell: 'googHeaderCell',
                    tableCell: 'googRow',
                };

                myTable = new google.visualization.ChartWrapper({
                    'chartType': 'Table',
                    'containerId': 'dvMyTable',
                    'options': { width: '100%', height: '100%', sort: 'disable', cssClassNames: classes },
                    'view': { "columns": [1, 2, 9, 4, 5, 6, 7, 8] }

                });

                var scenarioFilter = new google.visualization.ControlWrapper({
                    'controlType': 'CategoryFilter',
                    'containerId': 'filter0',
                    'options': {
                        'filterColumnIndex': 2,
                        'ui': { 'allowTyping': false, 'caption': 'SELECT A SCENARIO' }
                    }
                });

                var domainFilter = new google.visualization.ControlWrapper({
                    'controlType': 'CategoryFilter',
                    'containerId': 'filter1',
                    'options': {
                        'filterColumnIndex': 1,
                        'ui': { 'allowTyping': false, 'caption': 'SELECT A NAME' }
                    }
                });

                dashboard.bind(scenarioFilter, myChart);
                dashboard.bind(domainFilter, myChart);
                dashboard.bind(scenarioFilter, myTable);
                dashboard.bind(domainFilter, myTable);

                google.visualization.events.addListener(myTable, 'ready', function () {
                    document.getElementById("tableToggle").addEventListener("mousedown", toggleChart);
                    //var x = document.getElementsByClassName("google-visualization-table-table")[0];
                    //x.style.position = "absolute";
                });

                dashboard.draw(data);

            } else {
                document.getElementById("error-popup").className = "error-popup";
            }


        }

        $(function () {
            $(window).on("resize", debounce(googleScale, 250, false));
        });

        function debounce(func, wait, immediate) {
            var timeout;
            return function () {
                var context = this, args = arguments;
                var later = function () {
                    timeout = null;
                    if (!immediate) func.apply(context, args);
                };
                var callNow = immediate && !timeout;
                clearTimeout(timeout);
                timeout = setTimeout(later, wait);
                if (callNow) func.apply(context, args);
            };
        };

        function googleScale() {
            //add on resize event listener to call this function. Grab SVG width. Compare to new viewWidth. Calculate scaler.
            myChart.draw();
            //startGoogle();
        }


        function myFunction(myName, myScenario, myWFE, myWFL, mySetup, myRun, myWFLot, myTotal) {

            var array = [];

            if (myWFLot != 0) {
                array.push('<div><span style="">WAIT FOR LOT: </span><span style="font-weight:900;">' + myWFLot.toFixed(1) + ' (' + Number((myWFLot / myTotal) * 100).toFixed(2) + '%)</span></div>');
            }
            if (myRun != 0) {
                array.push('<div><span style="">RUN: </span><span style="font-weight:900;">' + myRun.toFixed(1) + ' (' + Number((myRun / myTotal) * 100).toFixed(2) + '%)</span></div>');
            }
            if (mySetup != 0) {
                array.push('<div><span style="">SETUP: </span><span style="font-weight:900;">' + mySetup.toFixed(1) + ' (' + Number((mySetup / myTotal) * 100).toFixed(2) + '%)</span></div>');
            }

            if (myWFL != 0) {
                array.push('<div><span style="">WAIT FOR LABOR: </span><span style="font-weight:900;">' + myWFL.toFixed(1) + ' (' + Number((myWFL / myTotal) * 100).toFixed(2) + '%)</span></div>');
            }
            if (myWFE != 0) {
                array.push('<div><span style="">WAIT FOR EQUIPMENT: </span><span style="font-weight:900;">' + myWFE.toFixed(1) + ' (' + Number((myWFE / myTotal) * 100).toFixed(2) + '%)</span></div>');
            }




            var myreturnString = '<div style="padding:10px; display:flex;flex-direction:column;font-size:12px;color:#555;">' +
                '<span style="font-weight:900; font-size:16px;">' + myName + ', ' + myTotal.toFixed(1) + ' CALENDAR DAYS</span>' +
                '<span style="font-weight:900;font-size:14px;margin-bottom:10px;">' + myScenario + '</span>';

            for (i = 0; i < array.length; i++) {
                myreturnString += array[i];
            }

            return myreturnString + '</div>';

        }


        //THIS FUNCTION TOGGLES BETWEEN SORTING BY SCENARIO-NAME AND NAME-SCENARIO -- SPL 1/14/2018
        function googleSort() {

            var intArray = view.getSortedRows([0, 1]);
            view.setRows(intArray);
            myChart.draw();

        }

        //THIS FUNCTION CAPTURES THE CURRENT CHARTAREA, WRITES IMAGE IN NEW TAB -- SPL 1/14/2018 
        //-- TO DO --
        //-- TRY PRINT() OR LINK
        function googlePrint() {

            var tempURL = myChart.getChart().getImageURI();
            var html = "<img src='" + tempURL + "' alt='from canvas'/>";
            var tab = window.open();
            tab.document.write(html);
            tempURL = null;
            html = null;

        }

        function googleCopy() {
            var sel = window.getSelection();
            sel.removeAllRanges();
            var myTableforCopy = $(".google-visualization-table-table")[0];
            var range = document.createRange();
            range.selectNode(myTableforCopy);
            sel.addRange(range);

            try {
                // Now that we've selected the anchor text, execute the copy command  
                var successful = document.execCommand('copy');
                var msg = successful ? 'successful' : 'unsuccessful';
                alert('Copy command was ' + msg);
            } catch (err) {
                alert('Oops, unable to copy');
            }

            sel.removeAllRanges();
        }

        function googleStacked() {

            if (myChart.getOption('isStacked') == "true") {


                myChart.setOption('isStacked', 'percent');
                myChart.setOption('vAxis.format', "###.##%");

            } else {

                myChart.setOption('isStacked', "true");
                myChart.setOption('vAxis.format', '#,###.##');

            }

            myChart.draw();
        }

        //THIS FUNCTION STARTS A DOWNLOAD OF THE CURRENT CHART AS PNG IMAGE
        //-- TODO --
        //-- NAME FILE USING CURRENT MODEL AND SCENARIO
        function googleDownload() {

            var download = document.getElementById('btn-download');
            var tempURL = myChart.getChart().getImageURI();

            download.href = tempURL;
            //need to name file based on current model/scenario(?)
            download.download = "VSM_MCT_" + Math.floor(Date.now() / 1000);
            tempURL = null;

        }




        //THIS FUNCTION CONTROLS THE CHART DIV HEIGHT TO ALLOW USER TO SEE DATATABLE. MAY BE BETTER TO CONTROL CHART.HEIGHT AND PLACE DATATABLE OVER THE MAIN CHART-DIV
        //-- TO DO --
        //-- Replace with google table?

        var expanded = false;
        var thisHeight = 0;

        function toggleChart() {

            var tableHeight = document.getElementById("dvMyTable").clientHeight;
            var windowHeight = document.getElementById("data-content").clientHeight
            var tempPercent = ((windowHeight - tableHeight) / windowHeight) * 100;
            var maxPercent = 30;
            var maxSize = windowHeight * (maxPercent / 100);

            var heightProportionCheck = (tableHeight / windowHeight) * 100;

            if (!expanded) {

                document.getElementById("lblToggle").innerHTML = "<i class='fas fa-eye-slash fa-fw row-icon'></i>";
                document.getElementById("tableToggle").innerHTML = "<i class='fa fa-chevron-down toggleIcon'></i>";

                document.getElementById("dvTable").style.setProperty("--table-pos", "calc(100% - 15vh)");

                expanded = true;


            } else {

                document.getElementById("lblToggle").innerHTML = "<i class='fas fa-eye-slash fa-fw-slash row-icon'></i>";
                document.getElementById("tableToggle").innerHTML = "<i class='fa fa-chevron-up toggleIcon'></i>";

                document.getElementById("dvTable").style.setProperty("--table-pos", "calc(100% - 1.5vh)");


                expanded = false;

            }

        }

    </script>


</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div id="error-popup" class="hidden">

        <div style="display: flex; margin: auto; background: #c0392ba3; border-top: 2px solid #c0392b; border-bottom: 2px solid #c0392b;">
            <div style="padding: 0px 12.5vw 0px 9vw">
                <i class="fa fa-commenting" style="color: white; font-size: 40vh; padding: 10px;"></i>
            </div>
            <div style="margin: auto; padding: 0px 12.5vw 0px 9vw; color: white;">
                <span style="font-size: 4vh; text-align: left;">RESULTS COULD NOT BE LOADED</span>

                <p>This page does not contain results beacuase:</p>
                <ul>
                    <li>Calculations have not been executed yet. Please use the "Run" navigation menu item</li>
                    <p>or...</p>
                    <li>Production volumes could not be achieved within the model timeframe and capacity limitations. Investigate the Labor and Equipment Utilization graphs</li>

                </ul>


            </div>
        </div>
    </div>

    <script>

        var myChartString = "<li><div id='dvToggle' class='mnuDropIcon' title='Toggle Data Table Visibility' onmousedown='toggleChart()'><label id='lblToggle' class='icon-menu'><i class='fas fa-eye-slash fa-fw-slash row-icon'></i></label><span id='spnToggle'>SHOW/HIDE</span></div></li>";
        var myAxisString = "<li><div class='mnuDropIcon' id='dvPrint' title='Toggle the Vertical Axis' onmousedown='googleStacked()'><label id='btn-print' class='icon-menu'><i class='fas fa-sort fa-fw row-icon'></i></label><span id='spnPrint'>TOGGLE AXIS</span></div></li>";
        var myDownloadString = "<li><a title='Download an Image of the Current Chart' class='mnuDropIcon' id='btn-download' href='#' download='' onmousedown='googleDownload()'><label id='lblDownload' class='icon-menu'><i class='fas fa-file-image fa-fw row-icon'></i></label><span id='spnDownload'>SAVE IMAGE</span></a></li>";
        var myCopyString = "<li><div class='mnuDropIcon' id='dvCopy' title='Show Data Table' onmousedown='googleCopy()'><label id='lblCopy' title='Show Data' class='icon-menu'><i class='fas fa-copy fa-fw row-icon'></i></label><span id='spnCopy'>COPY DATA</span></div></li>";
        var myHelpString = "<li><div class='mnuDropIcon' title='Click for page instructions' id='dvHelp'><label class='icon-menu'><i class='fas fa-question-circle fa-fw row-icon'></i><span id='spnHelp'>HELP</span></label></div>";
        var myHelpContext = "Please Help Me";

        var myEllipsisControls = [myChartString, myAxisString, myDownloadString, myCopyString, myHelpString];

        $(function () {

            var printString = "";
            var len = myEllipsisControls.length;
            for (i = 0; i < len; i++) {
                printString += myEllipsisControls[i];
            }
            document.getElementById("page-control-content").innerHTML = printString;
            document.getElementById("dvHelp").addEventListener("click", function () {
                alert(myHelpContext);
            }, false);
        });

    </script>


    <script>

        $(function () {

            document.getElementById("add-button").setAttribute("disabled", "disabled");
            document.getElementById("back-button").setAttribute("disabled", "disabled");
            //document.getElementById("inner-header-dropdown").style.visibility = "hidden";

        });

        var filters = 2;
        var printString = "";

        for (i = 0; i < filters; i++) {
            printString += "<div id='filter" + i + "'></div>";
        }

        document.getElementById("filterContainer").innerHTML = printString;

    </script>


    <div class="" id="data-content">

        <div id="dvChart" class="chartDefault" style="display: flex; --chart-height: 100%;">

            <div id="dvMyChart" class="defaultCanvas" style="--canvas-height: 100%;"></div>

        </div>
        <div id="dvTable" style="position: absolute; width: 100%; top: var(--table-pos); --table-pos: calc(100% - 1.5vh); height: 15vh; transition: all ease-out 0.25s;">

            <div id="tableToggle" onmousedown=""><i class="fa fa-chevron-up toggleIcon" style="margin: auto;"></i></div>
            <div id="dvMyTable" class="" style=""></div>

        </div>
    </div>



</asp:Content>

