using System.Collections;
using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;
using UnityEditor;
using UnityEngine;

public class ToolsExcelWrite
{
    /// <summary>
    /// 除了 英语，中文，繁体中文，日语，其他都是机器翻译
    /// </summary>
    [MenuItem("Tools/WriteExcelTable")]
    static void WriteExcelTable()
    {
        string filePath = Application.dataPath + "/Editor/Tables/testLanguage.txt";
        List<string> listStr = new List<string>();
        // 使用StreamReader读取文件
        using (StreamReader sr = new StreamReader(filePath))
        {
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                listStr.Add(line);
            }
        }

        EnumTableName tableName = EnumTableName.TableLanguageStatement;
        string strTemp = Application.dataPath + "/Editor/Tables/" + tableName + ".xlsx";
        FileInfo newFile = new FileInfo(strTemp);
        using (ExcelPackage package = new ExcelPackage(newFile))
        {
            ExcelWorksheet workSheet = package.Workbook.Worksheets["farm"];
            for (int i = 0; i < listStr.Count; i++)
            {
                Debug.Log(listStr[i]);
                workSheet.Cells[i + 2, 11].Value = listStr[i];
            }
            package.Save();
        }
        //File.WriteAllBytes(Application.dataPath + "/Resources/DataTables/" + (int)tableName + ".json", Tools.GetFileByte(strTemp));
    }
}
