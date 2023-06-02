using BLL;
using BLL.FunctionClasses.Account;
using BLL.FunctionClasses.Transaction;
using BLL.PropertyClasses.Master;
using BLL.PropertyClasses.Transaction;
using DevExpress.XtraEditors;
using STORE.Class;
using STORE.Report;
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;

namespace STORE.Transaction
{
    public partial class FrmBillTransaction : DevExpress.XtraEditors.XtraForm
    { 
        BLL.FormEvents objBOFormEvents = new BLL.FormEvents();
        BLL.Validation Val = new BLL.Validation();
        ItemPurchase ObjItemPurchase = new ItemPurchase();
        ItemPurchaseMaster ObjPurchase = new ItemPurchaseMaster();
        Invoice_Entry ObjInvoiceEntry = new Invoice_Entry();
        string Form_Type = "";

        public FrmBillTransaction()
        {
            InitializeComponent();
        }
        public void ShowForm()
        {
            Val.frmGenSet(this);
            AttachFormEvents();
            this.Show();
        }
        private void AttachFormEvents()
        {
            objBOFormEvents.CurForm = this;
            objBOFormEvents.FormKeyPress = true;
            objBOFormEvents.FormKeyDown = true;
            objBOFormEvents.FormResize = true;
            objBOFormEvents.FormClosing = true;
            objBOFormEvents.ObjToDispose.Add(Val);
            objBOFormEvents.ObjToDispose.Add(objBOFormEvents);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (ValSave() == false)
            {
                return;
            }

            if (Global.Confirm("Are You Sure To Save ?", "STORE", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
            {
                return;
            }
            Int64 IntRes = 0;
            Invoice_EntryProperty Invoice_EntryPropertyNew = new Invoice_EntryProperty();
            
            Invoice_EntryProperty Invoice_EntryProperty = new Invoice_EntryProperty();

            Invoice_EntryProperty.TransactionMasterID = Val.ToInt64(txtTransactionID.Text);
            Invoice_EntryProperty.Invoice_No = Val.ToString(txtInvoiceNo.Text);
            Invoice_EntryProperty.Unload_Date = Val.DBDate(DTPUnloadDate.Text);
            Invoice_EntryProperty.Transaction_Date = Val.DBDate(DTPTranDate.Text);

            Invoice_EntryProperty.From_Party_Code = Val.ToInt64(LookupFromParty.EditValue);
            Invoice_EntryProperty.To_Party_Code = Val.ToInt64(LookupToParty.EditValue);

            Invoice_EntryProperty.From_Destination = Val.ToInt64(LookupFromDestination.EditValue);
            Invoice_EntryProperty.To_Destination = Val.ToInt64(LookupToDestination.EditValue);
            Invoice_EntryProperty.Truck_No = Val.ToInt64(LookupTruckNo.EditValue);

            Invoice_EntryProperty.LR_No = Val.ToString(txtLRNo.Text);
            Invoice_EntryProperty.MT = Val.ToString(txtMT.Text);

            Invoice_EntryProperty.Advance = Val.ToDecimal(txtAdvance.Text);
            Invoice_EntryProperty.Diesel = Val.ToDecimal(txtDiesel.Text);
            Invoice_EntryProperty.Baki = Val.ToDecimal(txtBaki.Text);
            Invoice_EntryProperty.Freight = Val.ToDecimal(txtFreight.Text);
            Invoice_EntryProperty.Commission = Val.ToDecimal(txtCommission.Text);
            Invoice_EntryProperty.Net_Amt = Val.ToDouble(txtNetAmt.Text);
            Invoice_EntryProperty.Remark = Val.ToString(txtRemark.Text);

            Invoice_EntryProperty.Holding = Val.ToDecimal(txtHolding.Text);

            IntRes = ObjInvoiceEntry.SaveBillTransaction(Invoice_EntryProperty);
           
            Invoice_EntryPropertyNew = null;
          
            if (IntRes != 0)
            {
                Global.Confirm("Save Data Successfully");
                GetData();
                btnClear_Click(null, null);
            }
            else
            {
                Global.Confirm("Error in Data Save");
                txtInvoiceNo.Focus();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtTransactionID.Text = "";
            DTPUnloadDate.Text = "";
            LookupFromParty.EditValue = null;
            LookupToParty.EditValue = null;
            CmbPaymentMode.Text = "";
            txtPaymentDays.Text = "";
            txtRemark.Text = "";
            txtHolding.Text = "0";

            LookupFromDestination.EditValue = null;
            LookupToDestination.EditValue = null;
            LookupTruckNo.EditValue = null;

            txtAdvance.Text = "0";
            txtDiesel.Text = "0";
            txtCommission.Text = "0";

            ChkOwnTruck.Checked = false;
            txtCommission.Enabled = true;

            txtInvoiceNo.Text = "";

            //txtSGST.Text = "";
            //txtCGST.Text = "";
            //txtIGST.Text = "";
            txtTotalAddAmount.Text = "0";
            txtTotalLessAmount.Text = "0";
            txtGrossAmtLocal.Text = "0";
            txtNetAmt.Text = "0";
            DTPTranDate.Properties.Mask.Culture = new System.Globalization.CultureInfo("en-US");
            DTPTranDate.Properties.Mask.EditMask = "dd/MMM/yyyy";
            DTPTranDate.Properties.Mask.UseMaskAsDisplayFormat = true;
            DTPTranDate.Properties.CharacterCasing = CharacterCasing.Upper;

            DTPUnloadDate.Properties.Mask.Culture = new System.Globalization.CultureInfo("en-US");
            DTPUnloadDate.Properties.Mask.EditMask = "dd/MMM/yyyy";
            DTPUnloadDate.Properties.Mask.UseMaskAsDisplayFormat = true;
            DTPUnloadDate.Properties.CharacterCasing = CharacterCasing.Upper;

            DTPTranDate.EditValue = DateTime.Now;
            DTPUnloadDate.EditValue = DateTime.Now;

            CmbPaymentMode.SelectedIndex = 0;
            txtTransactionID.Text = ObjInvoiceEntry.FindNewID(Form_Type).ToString();
            txtChallanNo.Text = "";

            txtLRNo.Text = "";
            txtMT.Text = "";
            txtBaki.Text = "0";
            txtFreight.Text = "0";
            txtRemark.Text = "";
            
            PanelShow.Enabled = true;

            txtInvoiceNo.Focus();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #region functions

        private bool ValSave()
        {
            if (string.IsNullOrEmpty(txtInvoiceNo.Text.Trim()))
            {
                Global.Confirm("Invoice No Is Required");
                txtInvoiceNo.Focus();
                return false;
            }

            if (Val.ToString(LookupFromParty.Text.Trim()) == "")
            {
                Global.Confirm("From Party Is Required");
                LookupFromParty.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(DTPTranDate.Text.Trim()))
            {
                Global.Confirm("Party Tansaction Date Is Required");
                DTPTranDate.Focus();
                return false;
            }
          
            return true;
        }

        #endregion

        private void LookupFromParty_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (e.Button.Index == 1)
            {
                FrmFromPartyMaster frmCnt = new FrmFromPartyMaster();
                frmCnt.ShowDialog();
                Ledger_MasterProperty Party = new Ledger_MasterProperty();
                Party.Party_Type = "";
                Global.LOOKUPFromParty(LookupFromParty, Party);
                Party = null;
            }
        }

        private void FrmItemPurchaseMaster_Shown(object sender, EventArgs e)
        {
            btnClear_Click(btnClear, null);
            Ledger_MasterProperty Party = new Ledger_MasterProperty();
            Party.Party_Type = "";
            Global.LOOKUPFromParty(LookupFromParty, Party);
            Global.LOOKUPToParty(LookupToParty, Party);
            Party = null;
            Global.LOOKUPCity(LookupFromDestination);
            Global.LOOKUPCity(LookupToDestination);
            Global.LOOKUPTruck(LookupTruckNo);

            //this.Text = "Purchase Invoice";
            dtpSearchFromDate.Properties.Mask.Culture = new System.Globalization.CultureInfo("en-US");
            dtpSearchFromDate.Properties.Mask.EditMask = "dd/MMM/yyyy";
            dtpSearchFromDate.Properties.Mask.UseMaskAsDisplayFormat = true;
            dtpSearchFromDate.Properties.CharacterCasing = CharacterCasing.Upper;

            dtpSearchToDate.Properties.Mask.Culture = new System.Globalization.CultureInfo("en-US");
            dtpSearchToDate.Properties.Mask.EditMask = "dd/MMM/yyyy";
            dtpSearchToDate.Properties.Mask.UseMaskAsDisplayFormat = true;
            dtpSearchToDate.Properties.CharacterCasing = CharacterCasing.Upper;

            dtpSearchFromDate.EditValue = DateTime.Now;
            dtpSearchToDate.EditValue = DateTime.Now;
            GetData();
            btnClear_Click(btnClear, null);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            PanelShow.Enabled = true;           
        }

        private Boolean ValDelete()
        {
            if (Val.Val(txtInvoiceNo.Text) == 0)
            {
                Global.Message("Invoice No Is Required");
                txtInvoiceNo.Focus();
                return false;
            }
            return true;
        }

        private void gridView2_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            if (e.RowHandle >= 0)
            {
                if (e.Clicks == 2)
                {
                    DataRow DRow = GrdBillTransaction.GetDataRow(e.RowHandle);

                    txtTransactionID.Text = Val.ToString(DRow["TransactionID"]);

                    txtInvoiceNo.Text = Val.ToString(DRow["Inovice_No"]);
                    DTPTranDate.EditValue = Val.DBDate(DRow["Transaction_Date"].ToString());
                    DTPUnloadDate.EditValue = Val.DBDate(DRow["Unload_Date"].ToString());
                    LookupFromParty.EditValue = Val.ToInt64(DRow["From_Party_Code"]);
                    LookupToParty.EditValue = Val.ToInt64(DRow["To_Party_Code"]);
                    LookupFromDestination.EditValue = Val.ToInt64(DRow["From_City_Code"]);
                    LookupToDestination.EditValue = Val.ToInt64(DRow["To_City_Code"]);
                    LookupTruckNo.EditValue = Val.ToInt64(DRow["Truck_ID"]);
                    txtRemark.Text = Val.ToString(DRow["Remark"]);    
                    txtCommission.Text = Val.ToString(DRow["Commission"]);
                    txtAdvance.Text = Val.ToString(DRow["Advance"]);
                    txtDiesel.Text = Val.ToString(DRow["Diesel"]);
                    txtLRNo.Text = Val.ToString(DRow["LR_NO"]);
                    txtMT.Text = Val.ToString(DRow["MT"]);
                    txtBaki.Text = Val.ToString(DRow["Baki"]);
                    txtFreight.Text = Val.ToString(DRow["Freight"]);
                    txtHolding.Text = Val.ToString(DRow["Holding"]);

                    txtNetAmt.Text = Val.ToString(DRow["Net_Amount"]);
                    PanelShow.Enabled = true;
                    txtInvoiceNo.Focus();
                }
            }
        }

        public DataTable GetData()
        {
            Invoice_EntryProperty Invoice_EntryProperty = new Invoice_EntryProperty();
            Invoice_EntryProperty.From_Date = Val.DBDate(dtpSearchFromDate.Text);
            Invoice_EntryProperty.To_Date = Val.DBDate(dtpSearchToDate.Text);
            DataTable DTab = ObjInvoiceEntry.Bill_Transaction_GetData(Invoice_EntryProperty);
            dgvBillTransaction.DataSource = DTab;
            dgvBillTransaction.RefreshDataSource();
            GrdBillTransaction.BestFitColumns();
            Invoice_EntryProperty = null;
            return DTab;           
        }

        private void txtTotalAddAmount_EditValueChanged(object sender, EventArgs e)
        {
            double GrsAmt = Val.ToDouble(txtGrossAmtLocal.Text);
            double AddAmt = Val.ToDouble(txtTotalAddAmount.Text);
            double LessAmt = Val.ToDouble(txtTotalLessAmount.Text);
            txtNetAmt.Text = Val.ToDouble(GrsAmt + AddAmt - LessAmt).ToString();
        }

        private void txtTotalLessAmount_EditValueChanged(object sender, EventArgs e)
        {
            double GrsAmt = Val.ToDouble(txtGrossAmtLocal.Text);
            double AddAmt = Val.ToDouble(txtTotalAddAmount.Text);
            double LessAmt = Val.ToDouble(txtTotalLessAmount.Text);
            txtNetAmt.Text = Val.ToDouble(GrsAmt + AddAmt - LessAmt).ToString();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            Invoice_EntryProperty Invoice_EntryPropertyNew = new Invoice_EntryProperty();
            Invoice_EntryPropertyNew.Invoice_Date = Val.DBDate(DTPUnloadDate.Text);
            Invoice_EntryPropertyNew.Invoice_No = Val.ToString(txtInvoiceNo.Text);
            Invoice_EntryPropertyNew.Trn_Id = Val.ToInt64(txtTransactionID.Text);
            Invoice_EntryPropertyNew.Type = Val.ToString(Form_Type);

            DataTable dtpur = new DataTable();
            dtpur = ObjInvoiceEntry.GetPrintData(Invoice_EntryPropertyNew); //ObjInvoice.GetPrintData(Property);


            FrmReportViewer FrmReportViewer = new FrmReportViewer();
            FrmReportViewer.DS.Tables.Add(dtpur);
            FrmReportViewer.GroupBy = "";
            FrmReportViewer.RepName = "";
            FrmReportViewer.RepPara = "";
            this.Cursor = Cursors.Default;
            FrmReportViewer.AllowSetFormula = true;

            FrmReportViewer.ShowForm("Purchase_Memo", 120, FrmReportViewer.ReportFolder.ACCOUNT);

            Invoice_EntryPropertyNew = null;
            dtpur = null;
            FrmReportViewer.DS.Tables.Clear();
            FrmReportViewer.DS.Clear();
            FrmReportViewer = null;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (Global.Confirm("Are You Sure To Delete ?", "STORE", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
            {
                return;
            }
            int IntRes = 0;
            Invoice_EntryProperty Invoice_EntryPropertyNew = new Invoice_EntryProperty();
            Invoice_EntryPropertyNew.TransactionMasterID = Val.ToInt64(txtTransactionID.Text);
            IntRes = ObjInvoiceEntry.DeleteBillTransaction(Invoice_EntryPropertyNew);           

            if (IntRes != 0)
            {
                Global.Confirm("Data Deleted Successfully");
                GetData();
                btnClear_Click(null, null);
            }
            else
            {
                Global.Confirm("Error in Data Delete");
                txtInvoiceNo.Focus();
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            GetData();
        }

        private void LookupToParty_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (e.Button.Index == 1)
            {
                FrmToPartyMaster frmCnt = new FrmToPartyMaster();
                frmCnt.ShowDialog();
                Ledger_MasterProperty Party = new Ledger_MasterProperty();
                Party.Party_Type = "";
                Global.LOOKUPToParty(LookupToParty, Party);
                Party = null;
            }
        }

        private void LookupFromDestination_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (e.Button.Index == 1)
            {
                FrmCityMaster frmCnt = new FrmCityMaster();
                frmCnt.ShowDialog();
                Global.LOOKUPCity(LookupFromDestination);
            }
        }

        private void LookupToDestination_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (e.Button.Index == 1)
            {
                FrmCityMaster frmCnt = new FrmCityMaster();
                frmCnt.ShowDialog();
                Global.LOOKUPCity(LookupToDestination);
            }
        }

        private void ChkOwnTruck_CheckedChanged(object sender, EventArgs e)
        {
            if(ChkOwnTruck.Checked == true)
            {
                txtCommission.Text = "0";
                txtCommission.Enabled = false;
            }
            else
            {
                txtCommission.Text = "0";
                txtCommission.Enabled = true;
            }
        }

        private void LookupTruckNo_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (e.Button.Index == 1)
            {
                FrmTruckMaster frmCnt = new FrmTruckMaster();
                frmCnt.ShowDialog();
                Global.LOOKUPTruck(LookupTruckNo);
            }
        }

        private void txtDiesel_TextChanged(object sender, EventArgs e)
        {
            decimal Diesel = Val.ToDecimal(txtDiesel.Text);
            decimal Advance = Val.ToDecimal(txtAdvance.Text);
            decimal Baki = Val.ToDecimal(txtBaki.Text);
            decimal Fright = Diesel + Advance + Baki;
            decimal Other = Val.ToDecimal(txtCommission.Text);
            decimal Holding = Val.ToDecimal(txtHolding.Text);
            txtFreight.Text = Val.ToDecimal(Fright).ToString();
            txtNetAmt.Text = Val.ToDecimal(Fright + Other + Holding).ToString();
        }

        private void txtAdvance_TextChanged(object sender, EventArgs e)
        {
            decimal Diesel = Val.ToDecimal(txtDiesel.Text);
            decimal Advance = Val.ToDecimal(txtAdvance.Text);
            decimal Baki = Val.ToDecimal(txtBaki.Text);
            decimal Fright = Diesel + Advance + Baki;
            decimal Other = Val.ToDecimal(txtCommission.Text);
            decimal Holding = Val.ToDecimal(txtHolding.Text);
            txtFreight.Text = Val.ToDecimal(Fright).ToString();
            txtNetAmt.Text = Val.ToDecimal(Fright + Other + Holding).ToString();
        }

        private void txtBaki_TextChanged(object sender, EventArgs e)
        {
            decimal Diesel = Val.ToDecimal(txtDiesel.Text);
            decimal Advance = Val.ToDecimal(txtAdvance.Text);
            decimal Baki = Val.ToDecimal(txtBaki.Text);
            decimal Fright = Diesel + Advance + Baki;
            decimal Other = Val.ToDecimal(txtCommission.Text);
            decimal Holding = Val.ToDecimal(txtHolding.Text);
            txtFreight.Text = Val.ToDecimal(Fright).ToString();
            txtNetAmt.Text = Val.ToDecimal(Fright + Other + Holding).ToString();
        }

        private void txtCommission_TextChanged(object sender, EventArgs e)
        {
            decimal Freight = Val.ToDecimal(txtFreight.Text);
            decimal Commission = Val.ToDecimal(txtCommission.Text);
            decimal Holding = Val.ToDecimal(txtHolding.Text);
            txtNetAmt.Text = Val.ToDecimal(Freight + Commission + Holding).ToString();
        }

        private void txtHolding_TextChanged(object sender, EventArgs e)
        {
            decimal Freight = Val.ToDecimal(txtFreight.Text);
            decimal Commission = Val.ToDecimal(txtCommission.Text);
            decimal Holding = Val.ToDecimal(txtHolding.Text);
            txtNetAmt.Text = Val.ToDecimal(Freight + Commission + Holding).ToString();
        }

        private void txtDiesel_EditValueChanged(object sender, EventArgs e)
        {
           
        }

        private void txtCommission_EditValueChanged(object sender, EventArgs e)
        {
           
        }
    }
}