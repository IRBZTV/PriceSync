using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PriceSync
{
    public partial class Form1 : Form
    {
        double Counter = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Interval = 500;
            timer1.Enabled = true;
        }
        protected void DoJob()
        {


            try
            {
                timer1.Enabled = false;
                timer1.Interval = 3600000;
                // timer1.Interval = 20000;

                richTextBox1.Text = "";
                richTextBox1.Text += "*****************" + "\n";
                richTextBox1.Text += DateTime.Now.ToString() + "\n";
                richTextBox1.Text += "*****************" + "\n";

                MyDBTableAdapters.WebSiteTableAdapter Web_Ta = new MyDBTableAdapters.WebSiteTableAdapter();

                PriceSrvc.PriceSoapClient s = new PriceSrvc.PriceSoapClient();
                s.DeletePrices("gASutRUwesW+XEmAhA7ruzaSTuswun", "b_w5t5AkUbuZEn-8h65+z#gespuP!#");


                //Web_Ta.Delete_Values();
                //Web_Ta.Delete_AllEconomic_Tree();


                MyDBTableAdapters.ECONOMY_TREETableAdapter Tree_Ta = new MyDBTableAdapters.ECONOMY_TREETableAdapter();
                MyDB.ECONOMY_TREEDataTable Tree_Dt = Tree_Ta.SelectAllActiveParents();
                for (int i = 0; i < Tree_Dt.Count; i++)
                {
                    bool Updated = false;


                    MyDB.ECONOMY_TREEDataTable Child_Dt2 = Tree_Ta.SelectChildByPid(short.Parse(Tree_Dt[i]["ID"].ToString()));
                    MyDBTableAdapters.STATISTIC_VALTableAdapter Val_Ta2 = new MyDBTableAdapters.STATISTIC_VALTableAdapter();
                    for (int m = 0; m < Child_Dt2.Count; m++)
                    {
                        MyDB.STATISTIC_VALDataTable Val_Dt2 = Val_Ta2.SelectValues_ByGroupId(short.Parse(Child_Dt2[m]["ID"].ToString()), DateTime.Now.ToShortDateString() + " " + "00:00:01", DateTime.Now.ToShortDateString() + " " + "23:59:59");
                        if (Val_Dt2.Count > 0)
                        {
                            Updated = true;
                        }
                    }


                    if (Updated)
                    {


                        //int InsertedId = int.Parse(Web_Ta.Insert_MasterPrice(Tree_Dt[i]["TITLE"].ToString(),
                        //                        Tree_Dt[i]["UNIT"].ToString(),
                        //                        short.Parse(i.ToString()), DateTime.Now).ToString());
                        int InsertedId = s.InsertTree(Tree_Dt[i]["TITLE"].ToString(), short.Parse(i.ToString()), "gASutRUwesW+XEmAhA7ruzaSTuswun", "b_w5t5AkUbuZEn-8h65+z#gespuP!#");


                        richTextBox1.Text += "======================" + "\n";
                        richTextBox1.Text += Tree_Dt[i]["TITLE"].ToString() + "\n";
                        richTextBox1.Text += "======================" + "\n";

                        MyDB.ECONOMY_TREEDataTable Child_Dt = Tree_Ta.SelectChildByPid(short.Parse(Tree_Dt[i]["ID"].ToString()));
                        MyDBTableAdapters.STATISTIC_VALTableAdapter Val_Ta = new MyDBTableAdapters.STATISTIC_VALTableAdapter();
                        for (int j = 0; j < Child_Dt.Count; j++)
                        {
                            Counter++;

                            MyDB.STATISTIC_VALDataTable Val_Dt = Val_Ta.SelectValues_ByGroupId(short.Parse(Child_Dt[j]["ID"].ToString()), DateTime.Now.ToShortDateString() + " " + "00:00:01", DateTime.Now.ToShortDateString() + " " + "23:59:59");

                            string Diff = "0";
                            if (Val_Dt.Count > 0)
                            {

                                if (Val_Dt[0]["diff"].ToString().Trim().Length != 0)
                                {
                                    Diff = Val_Dt[0]["diff"].ToString().Trim();
                                }
                                if (Val_Dt[0]["val"].ToString().Trim() != "0" && Val_Dt[0]["val"].ToString().Trim().Length > 0)
                                {
                                    //  Web_Ta.Inser_Values(InsertedId, Val_Dt[0]["val"].ToString(), Diff, Child_Dt[j]["TITLE"].ToString(), Child_Dt[j]["UNIT"].ToString());
                                    s.InsertValue(Child_Dt[j]["TITLE"].ToString(), Val_Dt[0]["val"].ToString(), Diff, Child_Dt[j]["UNIT"].ToString(), InsertedId, "gASutRUwesW+XEmAhA7ruzaSTuswun", "b_w5t5AkUbuZEn-8h65+z#gespuP!#");
                                    richTextBox1.Text += Counter + "  ]     " + Child_Dt[j]["TITLE"].ToString() + "  " + Val_Dt[0]["val"].ToString() + "    " + Child_Dt[j]["UNIT"].ToString() + "\n";
                                    richTextBox1.SelectionStart = richTextBox1.Text.Length;
                                    richTextBox1.ScrollToCaret();
                                }
                            }
                        }
                    }
                }

                richTextBox1.Text += "*****************" + "\n";
                richTextBox1.Text += DateTime.Now.ToString() + "\n";
                richTextBox1.Text += "*****************" + "\n";
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.ScrollToCaret();
                timer1.Enabled = true;

            }
            catch (Exception Exp)
            {
                richTextBox1.Text = "\n" + Exp.Message;
                timer1.Interval = 500;
                timer1.Enabled = true;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (DateTime.Now > DateTime.Parse(DateTime.Now.ToShortDateString() + " " + "09:00:00") && DateTime.Now < DateTime.Parse(DateTime.Now.ToShortDateString() + " " + "23:59:00"))
                {
                    bool Updated = false;

                    MyDBTableAdapters.STATISTIC_VALTableAdapter Val_Ta2 = new MyDBTableAdapters.STATISTIC_VALTableAdapter();
                    MyDB.STATISTIC_VALDataTable Val_Dt2 = Val_Ta2.GetUpdated(DateTime.Now.ToShortDateString() + " " + "00:00:01", DateTime.Now.ToShortDateString() + " " + "23:59:59");
                    if (Val_Dt2.Count > 0)
                    {
                        Updated = true;
                    }

                    if (Updated)
                    {
                        DoJob();
                    }
                }
            }
            catch (Exception Exp)
            {
                richTextBox1.Text = "\n" + Exp.Message;
                timer1.Interval = 500;
                timer1.Enabled = true;
            }


        }
    }
}
