﻿using System;

public partial class E_Table : DbPage
{

    public E_Table()
    {
        PAGENAME = "/output/summary.aspx";
 
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);

        // Set up session
        ApiUtil.SetSessionInfo(userDir);
    }


}
//FIELDS = new string[] { "EquipID", "EquipDesc", "description", "Totalt", "SetupUtilt", "RunUtilt", "LabWaitUtilt", "RepUtilt", "Idlet", "LaborDesc", "QProcess", "QWait", "QTotal" };
//        HEADERS = new string[] { null, "Equipment Group Name", "What-If Scenario", "Total Util [min]", "Setup Util [min]", "Run Util [min]", "Repair Util [min]", "Wait for Labor Util [min]", "Idle [min]", "Assigned Labor", "Pieces in Process", "Pieces Waiting", "Total WIP" };
//        tableQueryString = "SELECT DISTINCTROW tblEquip.EquipDesc, IIf([zstblwhatif].[display],[zstblwhatif].[name],\"_skip\") AS description, tblLabor.LaborDesc, IIf([tblequip].[grpsiz]>0,[tblequip].[grpsiz],1)*[tblRsEquip].[SetupUtil]*([tblequip].[ot]/100+1)*[tblgeneral].[rtu1b]*[tblgeneral].[rtu1c]/100 AS SetupUtilt, IIf([tblequip].[grpsiz]>0,[tblequip].[grpsiz],1)*[tblRsEquip].[RunUtil]*([tblequip].[ot]/100+1)*[tblgeneral].[rtu1b]*[tblgeneral].[rtu1c]/100 AS RunUtilt, IIf([tblequip].[grpsiz]>0,[tblequip].[grpsiz],1)*[tblRsEquip].[LabWaitUtil]*([tblequip].[ot]/100+1)*[tblgeneral].[rtu1b]*[tblgeneral].[rtu1c]/100 AS LabWaitUtilt, IIf([tblequip].[grpsiz]>0,[tblequip].[grpsiz],1)*[tblRsEquip].[RepUtil]*([tblequip].[ot]/100+1)*[tblgeneral].[rtu1b]*[tblgeneral].[rtu1c]/100 AS RepUtilt, IIf([tblequip].[grpsiz]>0,[tblequip].[grpsiz],1)*[tblRsEquip].[Idle]*([tblequip].[ot]/100+1)*[tblgeneral].[rtu1b]*[tblgeneral].[rtu1c]/100 AS Idlet, IIf([tblequip].[grpsiz]>0,[tblequip].[grpsiz],1)*([tblrsequip].[Setuputil]+[tblrsequip].[runUtil]+[tblrsequip].[repUtil]+[tblrsequip].[labWaitUtil])*([tblequip].[ot]/100+1)*[tblgeneral].[rtu1b]*[tblgeneral].[rtu1c]/100 AS Totalt, tblRsEquip.Qprocess, tblRsEquip.QWait, tblRsEquip.Qtotal, tblRsEquip.SetupUtil, tblRsEquip.RunUtil, tblRsEquip.LabWaitUtil, tblRsEquip.RepUtil, tblRsEquip.Idle, ([tblrsequip].[Setuputil]+[tblrsequip].[runUtil]+[tblrsequip].[repUtil]+[tblrsequip].[labWaitUtil]) AS Total, tblRsEquip.EquipID, zstblwhatif.familyid" +
//                     " FROM (((tblRsEquip INNER JOIN tblEquip ON tblRsEquip.EquipID = tblEquip.EquipID) INNER JOIN tblLabor ON tblEquip.Labor = tblLabor.LaborID) INNER JOIN zstblwhatif ON tblRsEquip.WID = zstblwhatif.WID) INNER JOIN tblgeneral ON zstblwhatif.dummyline = tblgeneral.dummylink" +
//                     " WHERE (((tblEquip.EquipDesc)<>\"None\") AND ((IIf([zstblwhatif].[display],[zstblwhatif].[name],\"_skip\"))<>\"_skip\"))"; // +
//                                                                                                                                                 //" ORDER BY tblRsEquip.WID, tblEquip.EquipDesc;";


//        sortedTableName = "tblRsEquip0";
//        defaultSortString = "ORDER BY tblRsEquip.WID, tblEquip.EquipDesc";