﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Windows.Forms;
using CrystalReportApps.Model;
using CrystalReportApps.Reports;

namespace CrystalReportApps
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            string connectString = System.Configuration.ConfigurationManager.ConnectionStrings["ApplicationDbContext"].ToString();
            string sqlSelect =
                "select b.CHeadName BankName,SUM(t.DeepositBalance) as CreditBalance from dbBankinfo b join tblBankTransaction t on b.ID=t.BankId where t.Date BETWEEN '2019-03-07' AND '2019-06-22' group by b.CHeadName;" +
                "select b.CHeadName BankName,SUM(t.WithdrawBalance) as DebitBalance from dbBankinfo b join tblBankTransaction t on b.ID=t.BankId where t.Date BETWEEN '2019-03-07' AND '2019-06-22' group by b.CHeadName;"+
                "select p.PartyName ClientName,SUM(t.ReciveAmount)as PaidAmount from dbPartyInfo p join tblAccountTransaction t on p.Id=t.CustomerId where t.MakeDate BETWEEN '2019-03-07' AND '2019-06-22'group by p.PartyName;"+
                "select p.PartyName ClientName,sum(t.GrandTotal)-SUM(t.ReciveAmount)as ReceivableAmount from dbPartyInfo p join tblAccountTransaction t on p.Id=t.CustomerId where t.MakeDate BETWEEN '2019-03-07' AND '2019-06-22' group by p.PartyName;"+
                "select  Balance as CashInHand, TotalSale,ClientDebit,BankDebit,ExpenseDebit, Part1 as TotalDebit,BankCredit,ClientCredit,labour as Labour, ExpenseCredit, Part2 as TotalCredit,CashInHand as CurrentCashInHand from tblBalanceSheet  where MakeDate='2019-04-01';" +
                "select  PrevAccReceiv as PreviousAccountReceive, TAR as TodayAccountReceive, CashCollected, Summary TotalAccountReceive  from tblBalanceSheet where MakeDate='2019-04-01';";
                               
            SqlConnection sqlConnection =new SqlConnection(connectString);
            sqlConnection.Open();
            SqlDataAdapter da = new SqlDataAdapter(sqlSelect, connectString);
            da.SelectCommand.CommandType = CommandType.Text;

            DataSet ds = new DataSet();
            da.Fill(ds);
            sqlConnection.Close();

            CrystalReport2 crystalReport = new CrystalReport2();
            crystalReport.Database.Tables["BankCredit"].SetDataSource(ds.Tables[0]);
            crystalReport.Database.Tables["BankDebit"].SetDataSource(ds.Tables[1]);
            crystalReport.Database.Tables["ClientPaidAmount"].SetDataSource(ds.Tables[2]);
            crystalReport.Database.Tables["ClientReceivableAmount"].SetDataSource(ds.Tables[3]);
            crystalReport.Database.Tables["BalanceSheet"].SetDataSource(ds.Tables[4]);
            crystalReport.Database.Tables["Summary"].SetDataSource(ds.Tables[5]);

            this.crystalReportViewer1.ReportSource = crystalReport;
            this.crystalReportViewer1.RefreshReport();
        }
    }
}
