﻿using System.Xml;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;



//   ,  as value for evel all ?  

/*  todaytoday    if ...  public const short LAB_OVER_U = 1;
         *            public const short EQ_OVER_U = 2;
             show ....  can we see results from start of page / return to this page ???
         * 
         */

public partial class Runchoices : MultiViewOnePage {

    int whatif_err;







    ClassF classE1_1;

    string resultsMessage;

    public Runchoices() {
        PAGENAME = "Run_choices.aspx";
        addView("Basic Calculations");
        addView("Optimize Lotsizes");
        addView("Errors &amp; Results Summary");
    }

    protected void Page_PreRender(Object sender, System.EventArgs e) {
        this.Form.Attributes.Remove("onsubmit");
        this.Form.Attributes.Add("onsubmit", "ShowProgressRun('" + pnlLoading.ClientID + "'); return true;");
        Buttonr1_v.Attributes.Add("onclick", " this.disabled = true; scriptVerifyData(); return true;");
        Buttonr1_b.Attributes.Add("onclick", " this.disabled = true; startProgressCheck();$find('modalPopupLoadingBehavior').show(); return true;");
        Buttonr1_a.Attributes.Add("onclick", " this.disabled = true; startProgressCheck();$find('modalPopupLoadingBehavior').show(); return true;");
        Button38.Attributes.Add("onclick", " this.disabled = true; startProgressCheck();$find('modalPopupLoadingBehavior').show(); return true;");


    }

    protected void Page_Load(Object sender, System.EventArgs e) {   //Handles Me.Load
        base.Page_Load(sender, e);
        classE1_1.Open();
        classE1_1.username = username;
        // Load the first page 
        string str1;
        if (!Page.IsPostBack) {
            SetLotsizesData();

        }
        //test 
        //classE1_1.UpdateMPX();


        try {
            classE1_1.SetBasicModelInfo();  //  sets glngwid etc.


            whatif_err = CalcClass.WHATIFSTOP_FLAG;

            if (!Page.IsPostBack) {

                MPXRunChoices.SetActiveView(PageR1);
                SetActiveView(0);

                SetCalculationLabels();
                RadioButtonList1.Items[0].Selected = true;
                RadioButtonList2.Items[0].Selected = true;
                RadioButtonList3.Items[0].Selected = true;
            }

        } catch (Exception ex) {
            Master.ShowErrorMessage("MPX internal error has occured.");
        }
        classE1_1.Close();
        if (!Page.IsPostBack) {
            txtResults.Text = GetResMessage();
            txtErrors.Text = GetErrMessage();
            pnlResults.Visible = true;
            bool vis1 = !txtErrors.Text.Equals(String.Empty);
            if (vis1 == true)
            {
                pnlErrors.Visible = true;
                pnlResults.BorderColor = Color.Black;
                pnlResults.BorderWidth = 3;
                pnlResults.BorderStyle = BorderStyle.Solid;
            }
            else
            {
                pnlErrors.Visible = false;
                pnlResults.BorderStyle = BorderStyle.None;
            }
        }

    }  // end sub

    //--------------------------------------------------------


    private void SetCalculationLabels() {
        string str1;
        if (classE1_1.glngwid == 0) {
            RadioButtonList1.Visible = true;
            RadioButtonList2.Visible = false;
            RadioButtonList3.Visible = false;
            Buttonr1_b.Text = " --  Calculate results for Base Case -- ";

        } else {

            str1 = classE1_1.get_widname(classE1_1.glngwid);

            if (str1.Length == 0)
                str1 = "No name ?";
            Buttonr1_b.Text = " --  Calculate results for Current What-If Scenario '" + str1 + "' -- ";
            RadioButtonList2.Items[0].Text = "Keep lot size changes in What-If Scenario '" + str1 + "'";
            RadioButtonList1.Visible = false;
            RadioButtonList2.Visible = true;
            RadioButtonList3.Visible = true;
            RadioButtonList3.Items[0].Text = "Save recent changes in Whatif '" + str1 + "'";
        }
    }

    public string postvalue(string varname) {

        string str1 = "";

        //!!!!!!!  getting value POST  ........................

        foreach (string query in Request.Form.AllKeys)     //  foreach (string query in Request.QueryString...
       {
            if (query.IndexOf(varname) >= 0) {
                str1 = Request.Form.Get(query);
                if (str1 != null) {
                    if (str1.Length > 0) { str1 = MyUtilities.clean(str1); }
                }
                return str1;
            }
        }

        return "";
    }




    //----------------------------------------------------

    // verify input data
    public void buttonr1_v_click(Object sender, System.EventArgs e) {

        logFiles.RunLog();
        if (DbUse.InRunProcess(userDir)) {
            Master.ShowErrorMessage("Cannot start verification. The verification and calculations are still in process from the previous run. Please wait.");
            logFiles.DuplicateRunEndLog();
            return;
        }
        CreateRunFile();
        try {
            classE1_1.Open();
            classE1_1.SetBasicModelInfo();  //  sets glngwid etc. 

            classE1_1.errorMessageGlobal = "";

            classE1_1.calc_return = 0;
            classE1_1.ValidateData();

            MPXRunChoices.SetActiveView(viewResults);
            SetActiveView(2);


            resultsMessage = do_calc_msg(classE1_1.calc_return, 0);
            string errorMessage = GetErrorMessage();
            txtErrors.Text = errorMessage;
            txtResults.Text = resultsMessage;
            save_errors_results(resultsMessage, errorMessage);
            //  gwwd  see below ... pnlErrors.Visible = !errorMessage.Trim().Equals(String.Empty);
            pnlResults.Visible = true;
            bool vis1 = !errorMessage.Trim().Equals(String.Empty);
            if (vis1 == true)
            {
                pnlErrors.Visible = true;
                pnlResults.BorderColor = Color.Black;
                pnlResults.BorderWidth = 3;
                pnlResults.BorderStyle = BorderStyle.Solid;
            }
            else
            {
                pnlErrors.Visible = false;
                pnlResults.BorderStyle = BorderStyle.None;
            }

            classE1_1.Close();
        } catch (Exception ex) {
            classE1_1.Close();
            logFiles.ErrorLog(ex);
            Master.ShowErrorMessage("Value Stream Modeling internal error has occured. " + classE1_1.errorMessageGlobal);

        }
        DeleteRunFile();
        logFiles.RunEndLog();
    }

    public void buttonr1_b_click(Object sender, System.EventArgs e) {
        try {
            classE1_1.username = username;
            CalcClass.CalculationResult result = CalcClass.CalculateResults(classE1_1);

            txtResults.Text = result.resultMessage;
            txtErrors.Text = result.errorMessage;

            pnlResults.Visible = true;
            pnlErrors.Visible = !result.errorMessage.Trim().Equals(String.Empty);
            bool vis1 = !result.errorMessage.Trim().Equals(String.Empty);
            if (vis1 == true)
            {
                pnlErrors.Visible = true;
                pnlErrors.BorderColor = Color.Black;
                pnlErrors.BorderWidth = 3;
                pnlErrors.BorderStyle = BorderStyle.Solid;
            }
            else
            {
                pnlErrors.Visible = false;
                pnlResults.BorderStyle = BorderStyle.None;
            }

            MPXRunChoices.SetActiveView(viewResults);
            SetActiveView(2);

        } catch (Exception calcEx) {
            Master.ShowErrorMessage(calcEx.Message);
        }

    }

    public void buttonr1_a_click(Object sender, System.EventArgs e) {
        LogFiles logFiles = new LogFiles(this.username);
        logFiles.RunLog();

        if (DbUse.InRunProcess(userDir)) {
            Master.ShowErrorMessage("Cannot start verification and calculations. The verification and calculations are still in process from the previous run. Please wait.");
            logFiles.DuplicateRunEndLog();
            return;
        }
        CreateRunFile();

        int ret;
     
        try {
            classE1_1.Open();
            classE1_1.SetBasicModelInfo();  //  sets glngwid etc.

            classE1_1.calc_return = 0;     //0 - none, 1 labor, 2 eq over util, 4 warnings 8 errors 


            //-----------------------------------------------------------------

            // count all whatifs to be calculated and write it to the main mysql database
            ADODB.Recordset recWhatifs = new ADODB.Recordset();
            bool recOpened = DbUse.OpenAdoRec(classE1_1.globaldb, recWhatifs, "SELECT tblWhatIf.*, tblWhatIf.recalc  FROM tblWhatIf WHERE FamilyID = 0;");
            int totalCalc = 1;
            int currentCalc = 0;
            while (!recWhatifs.EOF) {
                totalCalc++;
                recWhatifs.MoveNext();
            }
            DbUse.CloseAdoRec(recWhatifs);


            DbUse.RunMysql("INSERT INTO usercalc (id) SELECT userlist.id FROM userlist WHERE userlist.sessionid = '" + HttpContext.Current.Session.SessionID + "';");
            DbUse.RunMysql("UPDATE usercalc INNER JOIN userlist ON usercalc.id = userlist.id SET total = " + totalCalc + ", calc = " + currentCalc + ", lastCheck = " + DateTime.Now.Ticks + ", cancel = 0 WHERE userlist.sessionid = '" + HttpContext.Current.Session.SessionID + "';");
            //////////////////////////////////////////////////////////////////////////////////////

            classE1_1.global_runalldone = false;
            classE1_1.global_initwid = classE1_1.glngwid;
            classE1_1.errorMessageGlobal = "";

            while (classE1_1.global_runalldone == false) {
                classE1_1.calc_return = 0;
                classE1_1.runall_from_display();
            }


            if (classE1_1.global_initwid != classE1_1.glngwid) {
                if (classE1_1.global_initwid != 0) {
                    classE1_1.LoadBaseCase();
                    ret = classE1_1.LoadWhatIf(classE1_1.global_initwid);
                } else {
                    classE1_1.LoadBaseCase();
                }
            };


            //-----------------------------------------------------------------

            MPXRunChoices.SetActiveView(viewResults);
            SetActiveView(2);
            if ((classE1_1.calc_return & CalcClass.ERR_FLAG) > 0) {
                resultsMessage = do_calc_msg(classE1_1.calc_return, 0);
            } else {
                resultsMessage = do_calc_msg(classE1_1.calc_return, 1);
             }
                /*  todaytoday    if ...  public const short LAB_OVER_U = 1;
                 *            public const short EQ_OVER_U = 2;
                     show ....
                 * 
                 */
            string errorsMessage = GetErrorMessage();
            txtResults.Text = resultsMessage;
            txtErrors.Text = errorsMessage;
            save_errors_results(resultsMessage, errorsMessage);
            pnlResults.Visible = true;
            pnlErrors.Visible = !errorsMessage.Trim().Equals(String.Empty);

            
            classE1_1.runsqlado("UPDATE zs0tblWhatif SET display = -1 WHERE WID = " + classE1_1.glngwid + ";");

        } catch (Exception ex) {
            logFiles.ErrorLog(ex);
            Master.ShowErrorMessage("MPX internal error has occured. " + classE1_1.errorMessageGlobal);
        }
        classE1_1.Close();
        DbUse.RunMysql("DELETE usercalc.* FROM usercalc INNER JOIN userlist ON usercalc.id = userlist.id WHERE userlist.sessionid = '" + HttpContext.Current.Session.SessionID + "';");
        DeleteRunFile();
        logFiles.RunEndLog();
    }

    string do_calc_msg(int calc_r, int mtype) {
        return CalcClass.do_calc_msg(calc_r, mtype);

    }

    string do_baseline_opt_msg(int calc_r) {
        string str1;

        str1 = "<br/><br/>Calculations completed.  <br/><br/>";
        if (((calc_r & CalcClass.LAB_OVER_U) == 0) & ((calc_r & CalcClass.EQ_OVER_U) == 0)) { str1 = ""; } else {
            str1 += "   Production level cannot be sustained."
                ;
            if ((calc_r & CalcClass.LAB_OVER_U) > 0) { str1 += "<br/>   Some Labor Resources are Overutilized in the long run. "; }
            if ((calc_r & CalcClass.EQ_OVER_U) > 0) { str1 += "<br/>   Some Equipment Resources are Overutilized in the long run. "; }
            str1 += "<br/>   Manufacturing Critical-Path Time (MCT) and WIP cannot be computed. ";
            str1 += "<br/>   Optimizations must start from a position where that MCT and WIP can be computed. ";
        }
        return str1;
    }
    //-----------------------------------------------------------


    public void buttonr4_exe2(Object sender, System.EventArgs e) {


        TextBox22.Visible = false;
        Buttonr4_exe2.Visible = false;

        //Button38.Enabled = true;

        MPXRunChoices.SetActiveView(PageR1);
        SetActiveView(0);

        return;

    }



    public void buttonr4_opt(Object sender, System.EventArgs e) {
        bool b1;

        /*options 1) basecase  a.  add opt here    b. new whatif name     if not new return = message whatif xxx exists already
               2) in whatif    a. add opt here  b. new whatif name    (+save/close cur whatif xxx or trash changes since last save for cur whatif xxx 
      *                                          if not new return = message whatif xxx exists already
               
     */

        int wid;
        string str1;
        string str2;
        ADODB.Recordset recprod = null;
        int fstatus;
        string name1;
        int newwhatifid;
        double val1;
        double val2;
        double val3;
        int ret1;


        logFiles.RunLog();
        if (DbUse.InRunProcess(userDir)) {
            Master.ShowErrorMessage("Cannot start verification and calculations. The verification and calculations are still in process from the previous run. Please wait.");
            logFiles.DuplicateRunEndLog();
            return;
        }
        bool unavailable = false;
        if (unavailable) {
            Master.ShowInfoMessage("Optimize lot sizes feature is under construction and currently unavailable.");
            return;
        }
        CreateRunFile();
        
        long currTime = DateTime.Now.Ticks;
        DbUse.RunMysql("INSERT INTO usercalc (id) SELECT userlist.id FROM userlist WHERE userlist.sessionid = '" + HttpContext.Current.Session.SessionID + "';");
        DbUse.RunMysql("UPDATE usercalc INNER JOIN userlist ON usercalc.id = userlist.id SET total = 2, calc = 1, lastCheck = " + currTime + ", cancel = 0 WHERE userlist.sessionid = '" + HttpContext.Current.Session.SessionID + "';");
        try {
            classE1_1.Open();
            //today   whatif name change to whatif field on second time around  ???

            classE1_1.SetBasicModelInfo();  //  sets glngwid etc.

            name1 = MyUtilities.clean(TextBox9.Text).ToUpper();

            //name1 = name1.ToUpper();






            //  get  into whatif as needed 
            /*
            1. in basecase - stay 
            2. in whatif - stay 
            3. if basecase/whatif - switch to new whatif 
            4. if in whatif getting new - save old whatif 
             * 
            */
            str1 = "New What-If Scenario for lot size optimization";
            int currentWhatif = classE1_1.glngwid;
            if (classE1_1.glngwid == 0) {

                if (RadioButtonList1.Items[1].Selected) {
                    //start ewhatif
                    if (name1.Length == 0) {
                        classE1_1.Close();
                        Master.ShowErrorMessage("Invalid name for the new What-If Scenario. Please enter a name for the optimization What-If Scenario to be created.");
                        DeleteRunFile();
                        DbUse.RunMysql("DELETE usercalc.* FROM usercalc INNER JOIN userlist ON usercalc.id = userlist.id WHERE userlist.sessionid = '" + HttpContext.Current.Session.SessionID + "';");
                        logFiles.RunEndLog();
                        return;
                    }
                    if (WhatifExists(name1)) {
                        classE1_1.Close();
                        Master.ShowErrorMessage("A What-If Scenario with the same name already exists. Please choose a different name.");
                        DeleteRunFile();
                        DbUse.RunMysql("DELETE usercalc.* FROM usercalc INNER JOIN userlist ON usercalc.id = userlist.id WHERE userlist.sessionid = '" + HttpContext.Current.Session.SessionID + "';");
                        logFiles.RunEndLog();
                        return;
                    }
                    // add new whatif , get into it 
                    newwhatifid = classE1_1.addnewwhatif(name1, str1);
                    classE1_1.glngwid = newwhatifid;
                    classE1_1.saveWid();
                    classE1_1.model_modified = -1;
                    classE1_1.saveModel_modified();
                    classE1_1.dowhatif_all_start(); // start a new whatif
                    Master.PassCurrentWhatifName(name1);
                    Master.SetCurrentWhatifLabel();
                    WriteTableValuesToDb();
                } else {
                    WriteTableValuesToDb();
                }

            } else { // in whatif now 

                if (RadioButtonList3.Items[0].Selected) {
                    //savewehatif
                    classE1_1.dowhatif_all_end();
                    classE1_1.SaveWhatIfAudit(classE1_1.glngwid);
                }

                if (RadioButtonList2.Items[1].Selected) {
                    //close whatif, start new whatif
                    if (name1.Length == 0) {
                        classE1_1.Close();
                        Master.ShowErrorMessage("Invalid name for the new What-If Scenario. Please enter a name for the optimization What-If Scenario to be created.");
                        DeleteRunFile();
                        DbUse.RunMysql("DELETE usercalc.* FROM usercalc INNER JOIN userlist ON usercalc.id = userlist.id WHERE userlist.sessionid = '" + HttpContext.Current.Session.SessionID + "';");
                        logFiles.RunEndLog();
                        return;
                    }
                    if (WhatifExists(name1)) {
                        classE1_1.Close();
                        Master.ShowErrorMessage("A What-If Scenario with the same name already exists. Please choose a different name.");
                        DeleteRunFile();
                        DbUse.RunMysql("DELETE usercalc.* FROM usercalc INNER JOIN userlist ON usercalc.id = userlist.id WHERE userlist.sessionid = '" + HttpContext.Current.Session.SessionID + "';");
                        logFiles.RunEndLog();
                        return;
                    }

                    // get out of old whatif ...
                    classE1_1.LoadBaseCase();
                    classE1_1.model_modified = -1;
                    classE1_1.saveModel_modified();
                    // start whatif  
                    // add new whatif , get into it 
                    newwhatifid = classE1_1.addnewwhatif(name1, str1);
                    classE1_1.glngwid = newwhatifid;
                    classE1_1.saveWid();
                    classE1_1.model_modified = -1;
                    classE1_1.saveModel_modified();
                    str1 = "UPDATE zstblwhatifaudit SET zstblwhatifaudit.WID = " + newwhatifid + ";";
                    classE1_1.runsql(str1);
                    classE1_1.dowhatif_all_start();
                    Master.PassCurrentWhatifName(name1);
                    Master.SetCurrentWhatifLabel();
                    SetCalculationLabels();

                }
                WriteTableValuesToDb();
            }

            if (CalcClass.CalculationsCancelled(HttpContext.Current.Session.SessionID)) {
                classE1_1.Close();
                DeleteRunFile();
                DbUse.RunMysql("DELETE usercalc.* FROM usercalc INNER JOIN userlist ON usercalc.id = userlist.id WHERE userlist.sessionid = '" + HttpContext.Current.Session.SessionID + "';");
                logFiles.RunEndLog();
                Master.ShowInfoMessage("Value Stream Modeling optimization cancelled.");
                return;
            }



            classE1_1.runsqlado("UPDATE zs0tblWhatif SET display = -1 WHERE WID = " + classE1_1.glngwid + ";");

            //  base line run ...
            classE1_1.calc_return = 0;
            ret1 = classE1_1.RunDLL();
            if (ret1 == 0) {
                // problem in dll run
                str1 = do_calc_msg(classE1_1.calc_return, 1);
                string resultsMessage = str1;
                string errorsMessage = GetErrorMessage();
                save_errors_results(resultsMessage, errorsMessage);
                MPXRunChoices.SetActiveView(viewResults);
                SetActiveView(2);
                /*  todaytoday    if ...  public const short LAB_OVER_U = 1;
                 *            public const short EQ_OVER_U = 2;
                     show ....
                 * 
                 */
                txtResults.Text = resultsMessage + DbUse.toRedColor(" You need to fix the errors before starting optimization.");
                txtErrors.Text = errorsMessage;
                pnlResults.Visible = true;
                pnlErrors.Visible = !errorsMessage.Trim().Equals(String.Empty);
                classE1_1.Close();
                DeleteRunFile();
                DbUse.RunMysql("DELETE usercalc.* FROM usercalc INNER JOIN userlist ON usercalc.id = userlist.id WHERE userlist.sessionid = '" + HttpContext.Current.Session.SessionID + "';");
                logFiles.RunEndLog();
                return;
            }
            //--------------------------------------------------------------


            str1 = do_baseline_opt_msg(classE1_1.calc_return);


            if (str1.Length > 0) {

                //   "warn user about problem - not in allowed area";

                //Button38.Enabled = false;   // .ViewStateMode; // = Disabled;

                TextBox22.Rows = 8;
                TextBox22.Height = 128;
                //TextBox22.Visible = true;
                //Buttonr4_exe2.Visible = true;
                TextBox22.Text = str1;

                // Lucie commmented out 11-3-14
                /*if (classE1_1.glngwid != 0) {
                    classE1_1.LoadBaseCase();
                    classE1_1.dowhatif_all_start();

                    Master.PassCurrentWhatifName("");
                    Master.SetCurrentWhatifLabel();
                }*/

                classE1_1.Close();
                MPXRunChoices.SetActiveView(PageR1);
                SetActiveView(0);
                Master.ShowErrorMessage(str1);
                DeleteRunFile();
                DbUse.RunMysql("DELETE usercalc.* FROM usercalc INNER JOIN userlist ON usercalc.id = userlist.id WHERE userlist.sessionid = '" + HttpContext.Current.Session.SessionID + "';");
                logFiles.RunEndLog();
                return;
            }

            val1 = get_opt_value();  //  value of J at start ...

            if (val1 <= 0.0) {
                str1 = "current WIP value = 0, no optimization can be done.";
                TextBox22.Rows = 2;
                TextBox22.Height = 128;
                //TextBox22.Visible = true;
                //Buttonr4_exe2.Visible = true;
                TextBox22.Text = str1;
                classE1_1.Close();
                MPXRunChoices.SetActiveView(PageR1);
                SetActiveView(0);
                Master.ShowErrorMessage("Optimization cancelled because current WIP value is 0. Production must be achieved before starting optimization.");
                DeleteRunFile();
                DbUse.RunMysql("DELETE usercalc.* FROM usercalc INNER JOIN userlist ON usercalc.id = userlist.id WHERE userlist.sessionid = '" + HttpContext.Current.Session.SessionID + "';");
                logFiles.RunEndLog();
                return;
            }

            if (CalcClass.CalculationsCancelled(HttpContext.Current.Session.SessionID)) {
                classE1_1.Close();
                DeleteRunFile();
                DbUse.RunMysql("DELETE usercalc.* FROM usercalc INNER JOIN userlist ON usercalc.id = userlist.id WHERE userlist.sessionid = '" + HttpContext.Current.Session.SessionID + "';");
                logFiles.RunEndLog();
                Master.ShowInfoMessage("Value Stream Modeling optimization cancelled.");
                return;
            }
            long lastTime = currTime;
            currTime = DateTime.Now.Ticks;
            long timePerCalc = currTime - lastTime;
            DbUse.RunMysql("UPDATE usercalc INNER JOIN userlist ON usercalc.id = userlist.id SET calc = 2, lastCheck = " + currTime + ", timePerCalc = " + timePerCalc + " WHERE userlist.sessionid = '" + HttpContext.Current.Session.SessionID + "';");
            //classA.Opt(long numberOfParts, long *partid, float * weight, long * optimizeLotSize, long * OptimizeTbatch, float * lotsizeValue, float * tbatchValue) 
            classE1_1.do_opt();
            string popupResult = "Lot size optimization has finished. Original WIP value was " + Convert.ToString(classE1_1.dig_round(val1, 4)) + ".";
            str1 = do_calc_msg(classE1_1.calc_return, 1);
            str1 += "<br/><br/> Original WIP value was " + Convert.ToString(classE1_1.dig_round(val1, 4)) + ".";

            val2 = get_opt_value();  //   value of J at start ...
            str1 += "<br/> New WIP value is " + Convert.ToString(classE1_1.dig_round(val2, 4)) + ".";
            popupResult += " New WIP value is " + Convert.ToString(classE1_1.dig_round(val2, 4)) + ".";
            val3 = 100 * (val1 - val2) / val1;
            if (Math.Abs(val3) < 0.1)
                val3 = 0.0;
            str1 += "<br/><br/> Change in WIP value is " + classE1_1.dig_round(val3, 3) + " % decrease.";
            popupResult += " Change in WIP value is " + classE1_1.dig_round(val3, 3) + " % decrease.";
            MPXRunChoices.SetActiveView(viewResults);
            SetActiveView(2);
            string msgResults = str1;
            txtResults.Text = msgResults;
            save_errors_results(msgResults, "");
            pnlResults.Visible = true;
            pnlErrors.Visible = false;
            Master.ShowInfoMessage(popupResult);
        } catch (Exception ex) {
            logFiles.ErrorLog(ex);
            Master.ShowErrorMessage("MPX internal error has occured. ");
        } finally {
            classE1_1.Close();
            DeleteRunFile();
            DbUse.RunMysql("DELETE usercalc.* FROM usercalc INNER JOIN userlist ON usercalc.id = userlist.id WHERE userlist.sessionid = '" + HttpContext.Current.Session.SessionID + "';");
            logFiles.RunEndLog();
        }




        /* long all lotsize and demand range 
         * 
         * start 
         *   setup whatif family for the whole ...
         * 
         * loop lot size 
         *   do range for 1 paert 
         *     do loop for next part in dataset .
         *     no more 
         *     
         * 
         * start
         * demand loop
         *   do range for 1 part 
         *       do loop for next part in recordset 
         *       no more ? 
         *       
         *   do whatif new (name + number)
         *     rundll
        */

        /* start with just 1 part demand and lot size 
     

         string whatif_family;
        int whatiffamily_no;
        ADODB.Recordset  recprod_d = null;
        ADODB.Recordset  recprod_l = null;
        string prod_name_d;
        string prod_name_l;
        void strart_big_whatif() { 

            string str1;
            string str2;
            str1= "delete zswhatif_prod _l";
            runsql(str1);
            str1 = "delete zswhatif_prod_d";
            runsql(str1);
            open_ado_rec(globaldb, ref recwhatif, "whatif - max family id"); 





            open_ado_rec(global_db, ref recprod_l, "sdasda");
    */
        // xxx


        /*
  
        2. The plan ??
   
         * 
        //doing the big whatif  --  adding whatif glngwid issues

        //testing ...
        // whatif  reread all code and add whatif glngwid issues code ...
        //do optmization stuff   
    
  

  
        */

    }


    double get_opt_value() {

        double retval;
        string str1;
        ADODB.Recordset recprod = null;

        retval = 0;
        //classE1_1.Open();
        str1 = "SELECT Sum(([Value]*[WIP])) AS Expr1, tblRsProd.WID FROM tblRsProd INNER JOIN tblProdFore ON tblRsProd.ProdID = tblProdFore.ProdID GROUP BY tblRsProd.WID HAVING (((tblRsProd.WID)=" + classE1_1.glngwid + "));";
        DbUse.open_ado_rec(classE1_1.globaldb, ref recprod, str1);
        if (recprod != null) {
            if (recprod.EOF == false) {
                retval = (double)recprod.Fields["Expr1"].Value;
            }
        }
        DbUse.CloseAdoRec(recprod);
        //classE1_1.Close();
        return retval;

    }

    /*   
     *     1.     Add   Error text box on results page before results table Done.
     *     
     *     2.     Add   buttons to see      results/errors page Done.
     *     
     *     3.     add zstblresults   to mpxmdb.mdb  (send me your copy of mpxmdb.mdb so I can also update the sort table)
     * 
     *     4.     After  do_calc_msg  
     *            call
     *                 do_err_msg 
     *                 save_errors_results ( msg1 from   do_vcalc_msg,  msg2 from do_err_msg)
     *                 
     *           msg2 from do_err_msg goes into  error tex box.... 
     *           hide warning/error text box  if msg2 = null
     * 
     * */



    /*
    * 
    * 
    * -------------------------------------------------
     * */
    public string GetErrorMessage() {
        return CalcClass.GetErrorMessage(classE1_1);
    }

    public string GetResMessage() {
        ADODB.Recordset reccust = null;
        string resMessage = "";
        try {
            classE1_1.Open();
            DbUse.open_ado_rec(classE1_1.globaldb, ref reccust, "zstblresults");

            resMessage = "";
            if (!reccust.EOF) {
                resMessage = resMessage + (string)reccust.Fields["Results"].Value;
                reccust.MoveNext();

            }
            if (reccust != null) {
                reccust.Close();
                reccust = null;
            }
            classE1_1.Close();
        } catch (Exception ex) {
            logFiles.ErrorLog(ex);
        }
        resMessage = resMessage.Replace("\n", "<br/>");

        return DbUse.reproduceRedColor(resMessage);
    }

    public string GetErrMessage() {
        ADODB.Recordset reccust = null;
        string errMessage = "";
        try {
            classE1_1.Open();
            DbUse.open_ado_rec(classE1_1.globaldb, ref reccust, "zstblresults");

            errMessage = "";
            if (!reccust.EOF) {
                errMessage = errMessage + (string)reccust.Fields["errors"].Value;

            }
            if (reccust != null) {
                reccust.Close();
                reccust = null;
            }
            classE1_1.Close();
        } catch (Exception) { }
        return errMessage.Replace("\n", "<br/>");
    }





    public void save_errors_results(string msgResults, string msgErrors) {
        CalcClass.save_errors_results(classE1_1, msgResults, msgErrors);
    }


    public void Clear_errors_results() {
        string commandString;

        commandString = "Delete zstblresults.* from  zstblresults;";
        classE1_1.runsql(commandString);
    }



    protected void buttonshowequip(object sender, EventArgs e) {
        Response.Redirect("results_equip.aspx");

    }
    protected void buttonshowlabor(object sender, EventArgs e) {
        Response.Redirect("results_labor_table.aspx");

    }
    protected void buttonshowproduct(object sender, EventArgs e) {
        Response.Redirect("results_prod_table.aspx");

    }

    protected override void OnInit(EventArgs e) {
        base.OnInit(e);
        srcOptimizeTable.DataFile = GetDirectory() + userDir + MAIN_USER_DATABASE;
        classE1_1 = new ClassF(GetDirectory() + userDir);
        classE1_1.Close();

    }

    protected override Control GetMenuContainer() {
        return pnlMenu;
    }

    protected override Control GetTabsDiv() {
        return tabsDiv;
    }

    protected override MultiView GetMultiView() {
        return MPXRunChoices;
    }

    protected override void productMenu_MenuItemClick(object sender, MenuEventArgs e) {
        base.productMenu_MenuItemClick(sender, e);
        int itemNum = int.Parse(e.Item.Value);

        string str1;


        try {
            str1 = "Enter name for What-If Scenario:  (blank means to ";
            if (classE1_1.glngwid == 0) {
                str1 += " overwrite Base Case)";
            } else {
                classE1_1.Open();
                str1 = classE1_1.get_widname(classE1_1.glngwid);
                classE1_1.Close();
                if (str1.Length == 0)
                    str1 = "No name ?";
                str1 += "add changes to What-If Scenario '" + str1 + "' ";
            }
            // Label15.Text = str1;

            whatif_err = CalcClass.WHATIFSTOP_FLAG;

            TextBox22.Visible = false;
        } catch (Exception ex) {

            Master.ShowErrorMessage("MPX internal error has occured.");
        }
    }

    protected void btnSelectLotsize_Click(object sender, EventArgs e) {
        SetCheckboxes("checkLotsize", true);
    }
    protected void btnClearLotsize_Click(object sender, EventArgs e) {
        SetCheckboxes("checkLotsize", false);
    }
    protected void btnSelectTbatch_Click(object sender, EventArgs e) {
        SetCheckboxes("checkTbatch", true);
    }
    protected void btnClearTbatch_Click(object sender, EventArgs e) {
        SetCheckboxes("checkTbatch", false);
    }

    protected void SetCheckboxes(string checkBoxName, bool select) {
        foreach (GridViewRow row in gridLotsizes.Rows) {
            CheckBox check = row.FindControl(checkBoxName) as CheckBox;
            if (check != null) {
                check.Checked = select;
            }
        }
    }

    protected void gridLotsizes_RowDataBound(object o, GridViewRowEventArgs e) {
        if (e.Row.RowType == DataControlRowType.DataRow) {
            foreach (TableCell cell in e.Row.Cells) {
                try {

                    foreach (Control control in cell.Controls) {
                        if (control is TextBox) {
                            TextBox txt = control as TextBox;
                            txt.Attributes.Add("style", "text-align:right");
                            txt.Width = 80;
                            cell.HorizontalAlign = HorizontalAlign.Center;
                            break;
                        } else if (control is CheckBox) {
                            cell.HorizontalAlign = HorizontalAlign.Center;
                            break;
                        }
                    }

                } catch (Exception) {

                }
            }
        }
    }

    protected void btnResetLotsizes_Click(object sender, EventArgs e) {
        SetLotsizesData();
    }

    protected bool WhatifExists(string whatifName) {
        bool exists = false;
        ADODB.Recordset rec = new ADODB.Recordset();
        DbUse.OpenAdoRec(classE1_1.globaldb, rec, "SELECT WID, Name FROM tblWhatif;");
        try {
            int i = 0;
            while (!rec.EOF && !exists) {
                if (i == 0) {
                    rec.MoveFirst();
                } else {
                    rec.MoveNext();
                }
                exists = rec.Fields["Name"].Value.ToString().ToUpper().Equals(whatifName.ToUpper());
                i++;
            }
        } catch (Exception) { }
        DbUse.CloseAdoRec(rec);
        return exists;
    }

    public void WriteTableValuesToDb() {
        for (int i = 0; i < gridLotsizes.Rows.Count; i++) {
            CheckBox checkLotsize = gridLotsizes.Rows[i].FindControl("checkLotsize") as CheckBox;
            //CheckBox checkTbatch = gridLotsizes.Rows[i].FindControl("checkTbatch") as CheckBox;
            string optLotsize = (checkLotsize.Checked) ? "1" : "0";
            //string optTbatch = (checkTbatch.Checked) ? "1" : "0";
            string optTbatch = "0";
            TextBox txtVal = gridLotsizes.Rows[i].FindControl("txtVal") as TextBox;
            Label lblProduct = gridLotsizes.Rows[i].FindControl("lblProdDesc") as Label;
            double value;
            try {
                value = Double.Parse(txtVal.Text);
                classE1_1.runsql("UPDATE tblProdfore SET [optimizelotsize] = " + optLotsize + ", [optimizetbatch] = " + optTbatch + ", [value] = " + txtVal.Text + " WHERE ProdId = " + gridLotsizes.DataKeys[i]["ProdID"].ToString() + ";");
            } catch (Exception) {
                Master.ShowErrorMessage("Invalid data for product '" + lblProduct.Text + "'. Please correct the data in the table.");
            }
        }
        UpdateSql("UPDATE tblProdFore SET tblProdFore.[Value] = 1 WHERE (((tblProdFore.Value)<1));");

    }

    public void SetLotsizesData() {
        try {
            UpdateSql("UPDATE tblProdFore SET tblProdFore.[Value] = 1 WHERE (((tblProdFore.Value)<1));");
            gridLotsizes.DataBind();
            if (gridLotsizes.Rows.Count == 0) {
                Master.ShowErrorMessage("No products are defined yet. Please create some products first.");
            }
        } catch (Exception ex) {
            logFiles.ErrorLog(ex);
            if (!TablesLinked()) {
                Master.ShowErrorMessage("An error has occured. Current model '" + Master.GetCurrentModel() + "' is not loaded properly because some tables are missing. Please go to the models page and load the model again.");
            } else {
                Master.ShowErrorMessage();
            }
        }

    }
    protected void gridLotsizes_PageIndexChanging(object sender, GridViewPageEventArgs e) {
        gridLotsizes.PageIndex = e.NewPageIndex;
        SetLotsizesData();
    }

    protected void CreateRunCookie() {
        string cookieName = DbUse.RUN_COOKIE + username;
        HttpCookie cookie = new HttpCookie(cookieName);
        cookie.Value = username;
        cookie.Expires = DateTime.Now.AddMinutes(5);
        Response.Cookies.Add(cookie);
    }

    protected void CreateRunFile() {
        DbUse.CreateRunFile(GetDirectory() + userDir, username);
    }

    protected void DeleteRunFile() {
        DbUse.DeleteRunFile(GetDirectory() + userDir, username);
    }



    protected void DeleteRunCookie() {
        string cookieName = DbUse.RUN_COOKIE + username;
        HttpCookie cookie = Request.Cookies[cookieName];
        if (cookie != null) {
            cookie.Expires = cookie.Expires.AddDays(-1);
        }
    }




    protected void btnWriteTableValues_Click(object sender, EventArgs e) {
        WriteTableValuesToDb();
    }
}  //  End Class;
