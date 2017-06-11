namespace ErpMvc.Reportes
{
    partial class ComprasConComprobantes
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            DevExpress.DataAccess.Sql.TableQuery tableQuery1 = new DevExpress.DataAccess.Sql.TableQuery();
            DevExpress.DataAccess.Sql.RelationInfo relationInfo1 = new DevExpress.DataAccess.Sql.RelationInfo();
            DevExpress.DataAccess.Sql.RelationColumnInfo relationColumnInfo1 = new DevExpress.DataAccess.Sql.RelationColumnInfo();
            DevExpress.DataAccess.Sql.TableInfo tableInfo1 = new DevExpress.DataAccess.Sql.TableInfo();
            DevExpress.DataAccess.Sql.ColumnInfo columnInfo1 = new DevExpress.DataAccess.Sql.ColumnInfo();
            DevExpress.DataAccess.Sql.TableInfo tableInfo2 = new DevExpress.DataAccess.Sql.TableInfo();
            DevExpress.DataAccess.Sql.ColumnInfo columnInfo2 = new DevExpress.DataAccess.Sql.ColumnInfo();
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.ReportHeader = new DevExpress.XtraReports.UI.ReportHeaderBand();
            this.xrLabel4 = new DevExpress.XtraReports.UI.XRLabel();
            this.titulo_reporte = new DevExpress.XtraReports.UI.XRLabel();
            this.PageHeader = new DevExpress.XtraReports.UI.PageHeaderBand();
            this.ReportFooter = new DevExpress.XtraReports.UI.ReportFooterBand();
            this.sqlDataSource1 = new DevExpress.DataAccess.Sql.SqlDataSource("DefaultConnection");
            this.ComprasReport = new DevExpress.XtraReports.UI.DetailReportBand();
            this.Detail1 = new DevExpress.XtraReports.UI.DetailBand();
            this.xrTable5 = new DevExpress.XtraReports.UI.XRTable();
            this.xrTableRow8 = new DevExpress.XtraReports.UI.XRTableRow();
            this.fechaCompraCell = new DevExpress.XtraReports.UI.XRTableCell();
            this.compraTiendaCell = new DevExpress.XtraReports.UI.XRTableCell();
            this.compraProductosCell = new DevExpress.XtraReports.UI.XRTableCell();
            this.comprasCandidadCell = new DevExpress.XtraReports.UI.XRTableCell();
            this.compraImporteCell = new DevExpress.XtraReports.UI.XRTableCell();
            this.ReportHeader1 = new DevExpress.XtraReports.UI.ReportHeaderBand();
            this.xrTable4 = new DevExpress.XtraReports.UI.XRTable();
            this.xrTableRow7 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell7 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell1 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell8 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell10 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell12 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.HeightF = 0F;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // TopMargin
            // 
            this.TopMargin.HeightF = 51F;
            this.TopMargin.Name = "TopMargin";
            this.TopMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.TopMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // BottomMargin
            // 
            this.BottomMargin.HeightF = 100F;
            this.BottomMargin.Name = "BottomMargin";
            this.BottomMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // ReportHeader
            // 
            this.ReportHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel4,
            this.titulo_reporte});
            this.ReportHeader.HeightF = 110.9999F;
            this.ReportHeader.Name = "ReportHeader";
            // 
            // xrLabel4
            // 
            this.xrLabel4.Font = new System.Drawing.Font("Times New Roman", 50F, System.Drawing.FontStyle.Bold);
            this.xrLabel4.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrLabel4.Name = "xrLabel4";
            this.xrLabel4.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel4.SizeF = new System.Drawing.SizeF(545.8333F, 69.99995F);
            this.xrLabel4.StylePriority.UseFont = false;
            this.xrLabel4.StylePriority.UseTextAlignment = false;
            this.xrLabel4.Text = "Amelia del Mar";
            this.xrLabel4.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // titulo_reporte
            // 
            this.titulo_reporte.Font = new System.Drawing.Font("Times New Roman", 18F, System.Drawing.FontStyle.Bold);
            this.titulo_reporte.LocationFloat = new DevExpress.Utils.PointFloat(0F, 80.83328F);
            this.titulo_reporte.Name = "titulo_reporte";
            this.titulo_reporte.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.titulo_reporte.SizeF = new System.Drawing.SizeF(545.8333F, 23F);
            this.titulo_reporte.StylePriority.UseFont = false;
            this.titulo_reporte.StylePriority.UseTextAlignment = false;
            this.titulo_reporte.Text = "Resumen de operaciones";
            this.titulo_reporte.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // PageHeader
            // 
            this.PageHeader.HeightF = 36.66661F;
            this.PageHeader.Name = "PageHeader";
            // 
            // ReportFooter
            // 
            this.ReportFooter.HeightF = 40F;
            this.ReportFooter.Name = "ReportFooter";
            // 
            // sqlDataSource1
            // 
            tableQuery1.Name = "Ventas";
            relationColumnInfo1.NestedKeyColumn = "Id";
            relationColumnInfo1.ParentKeyColumn = "DiaContableId";
            relationInfo1.KeyColumns.Add(relationColumnInfo1);
            relationInfo1.NestedTable = "contb_dia_contable";
            relationInfo1.ParentTable = "Ventas";
            tableQuery1.Relations.Add(relationInfo1);
            tableInfo1.Name = "Ventas";
            columnInfo1.Name = "Importe";
            tableInfo1.SelectedColumns.Add(columnInfo1);
            tableInfo2.Name = "contb_dia_contable";
            columnInfo2.Name = "Fecha";
            tableInfo2.SelectedColumns.Add(columnInfo2);
            tableQuery1.Tables.Add(tableInfo1);
            tableQuery1.Tables.Add(tableInfo2);
            this.sqlDataSource1.Queries.AddRange(new DevExpress.DataAccess.Sql.SqlQuery[] {
            tableQuery1});
            this.sqlDataSource1.ResultSchemaSerializable = "PERhdGFTZXQ+PFZpZXcgTmFtZT0iVmVudGFzIj48RmllbGQgTmFtZT0iSW1wb3J0ZSIgVHlwZT0iRGVja" +
    "W1hbCIgLz48RmllbGQgTmFtZT0iRmVjaGEiIFR5cGU9IkRhdGVUaW1lIiAvPjwvVmlldz48L0RhdGFTZ" +
    "XQ+";
            // 
            // ComprasReport
            // 
            this.ComprasReport.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail1,
            this.ReportHeader1});
            this.ComprasReport.Level = 0;
            this.ComprasReport.Name = "ComprasReport";
            // 
            // Detail1
            // 
            this.Detail1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrTable5});
            this.Detail1.HeightF = 25F;
            this.Detail1.Name = "Detail1";
            // 
            // xrTable5
            // 
            this.xrTable5.Borders = DevExpress.XtraPrinting.BorderSide.Bottom;
            this.xrTable5.Font = new System.Drawing.Font("Times New Roman", 12F);
            this.xrTable5.LocationFloat = new DevExpress.Utils.PointFloat(12.5F, 0F);
            this.xrTable5.Name = "xrTable5";
            this.xrTable5.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRow8});
            this.xrTable5.SizeF = new System.Drawing.SizeF(630F, 25F);
            this.xrTable5.StylePriority.UseBorders = false;
            this.xrTable5.StylePriority.UseFont = false;
            this.xrTable5.StylePriority.UseTextAlignment = false;
            this.xrTable5.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrTableRow8
            // 
            this.xrTableRow8.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.fechaCompraCell,
            this.compraTiendaCell,
            this.compraProductosCell,
            this.comprasCandidadCell,
            this.compraImporteCell});
            this.xrTableRow8.Name = "xrTableRow8";
            this.xrTableRow8.Weight = 1D;
            // 
            // fechaCompraCell
            // 
            this.fechaCompraCell.Name = "fechaCompraCell";
            this.fechaCompraCell.Text = "Fecha";
            this.fechaCompraCell.Weight = 0.43650787447244438D;
            // 
            // compraTiendaCell
            // 
            this.compraTiendaCell.Name = "compraTiendaCell";
            this.compraTiendaCell.Text = "Tienda";
            this.compraTiendaCell.Weight = 0.53366909255616279D;
            // 
            // compraProductosCell
            // 
            this.compraProductosCell.Multiline = true;
            this.compraProductosCell.Name = "compraProductosCell";
            this.compraProductosCell.Text = "Productos";
            this.compraProductosCell.Weight = 1.0853780769311632D;
            // 
            // comprasCandidadCell
            // 
            this.comprasCandidadCell.Multiline = true;
            this.comprasCandidadCell.Name = "comprasCandidadCell";
            this.comprasCandidadCell.Text = "Cantidad";
            this.comprasCandidadCell.Weight = 0.48412710819796989D;
            // 
            // compraImporteCell
            // 
            this.compraImporteCell.Name = "compraImporteCell";
            this.compraImporteCell.Text = "Importe";
            this.compraImporteCell.Weight = 0.46031784784226187D;
            // 
            // ReportHeader1
            // 
            this.ReportHeader1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrTable4,
            this.xrLabel1});
            this.ReportHeader1.HeightF = 52.5F;
            this.ReportHeader1.Name = "ReportHeader1";
            // 
            // xrTable4
            // 
            this.xrTable4.Borders = DevExpress.XtraPrinting.BorderSide.Bottom;
            this.xrTable4.Font = new System.Drawing.Font("Times New Roman", 14F, System.Drawing.FontStyle.Bold);
            this.xrTable4.ForeColor = System.Drawing.Color.DimGray;
            this.xrTable4.LocationFloat = new DevExpress.Utils.PointFloat(12.5F, 27.5F);
            this.xrTable4.Name = "xrTable4";
            this.xrTable4.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRow7});
            this.xrTable4.SizeF = new System.Drawing.SizeF(630F, 25F);
            this.xrTable4.StylePriority.UseBorders = false;
            this.xrTable4.StylePriority.UseFont = false;
            this.xrTable4.StylePriority.UseForeColor = false;
            this.xrTable4.StylePriority.UseTextAlignment = false;
            this.xrTable4.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrTableRow7
            // 
            this.xrTableRow7.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell7,
            this.xrTableCell1,
            this.xrTableCell8,
            this.xrTableCell10,
            this.xrTableCell12});
            this.xrTableRow7.Name = "xrTableRow7";
            this.xrTableRow7.Weight = 1D;
            // 
            // xrTableCell7
            // 
            this.xrTableCell7.Borders = DevExpress.XtraPrinting.BorderSide.Bottom;
            this.xrTableCell7.Name = "xrTableCell7";
            this.xrTableCell7.StylePriority.UseBorders = false;
            this.xrTableCell7.Text = "Fecha";
            this.xrTableCell7.Weight = 0.43650787447244438D;
            // 
            // xrTableCell1
            // 
            this.xrTableCell1.Borders = DevExpress.XtraPrinting.BorderSide.Bottom;
            this.xrTableCell1.Name = "xrTableCell1";
            this.xrTableCell1.StylePriority.UseBorders = false;
            this.xrTableCell1.Text = "Tienda";
            this.xrTableCell1.Weight = 0.53366909255616279D;
            // 
            // xrTableCell8
            // 
            this.xrTableCell8.Borders = DevExpress.XtraPrinting.BorderSide.Bottom;
            this.xrTableCell8.Name = "xrTableCell8";
            this.xrTableCell8.StylePriority.UseBorders = false;
            this.xrTableCell8.Text = "Productos";
            this.xrTableCell8.Weight = 1.0853780769311632D;
            // 
            // xrTableCell10
            // 
            this.xrTableCell10.Borders = DevExpress.XtraPrinting.BorderSide.Bottom;
            this.xrTableCell10.Name = "xrTableCell10";
            this.xrTableCell10.StylePriority.UseBorders = false;
            this.xrTableCell10.Text = "Cantidad";
            this.xrTableCell10.Weight = 0.48412710819796989D;
            // 
            // xrTableCell12
            // 
            this.xrTableCell12.Borders = DevExpress.XtraPrinting.BorderSide.Bottom;
            this.xrTableCell12.Name = "xrTableCell12";
            this.xrTableCell12.StylePriority.UseBorders = false;
            this.xrTableCell12.Text = "Importe";
            this.xrTableCell12.Weight = 0.46031784784226187D;
            // 
            // xrLabel1
            // 
            this.xrLabel1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.xrLabel1.Font = new System.Drawing.Font("Times New Roman", 18F, System.Drawing.FontStyle.Bold);
            this.xrLabel1.ForeColor = System.Drawing.Color.DimGray;
            this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(12.50004F, 0F);
            this.xrLabel1.Name = "xrLabel1";
            this.xrLabel1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel1.SizeF = new System.Drawing.SizeF(629.9999F, 23F);
            this.xrLabel1.StylePriority.UseBackColor = false;
            this.xrLabel1.StylePriority.UseFont = false;
            this.xrLabel1.StylePriority.UseForeColor = false;
            this.xrLabel1.StylePriority.UseTextAlignment = false;
            this.xrLabel1.Text = "Compras";
            this.xrLabel1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // ComprasConComprobantes
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin,
            this.ReportHeader,
            this.PageHeader,
            this.ReportFooter,
            this.ComprasReport});
            this.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.Margins = new System.Drawing.Printing.Margins(100, 100, 51, 100);
            this.Version = "14.1";
            ((System.ComponentModel.ISupportInitialize)(this.xrTable5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.UI.ReportHeaderBand ReportHeader;
        private DevExpress.XtraReports.UI.XRLabel titulo_reporte;
        private DevExpress.XtraReports.UI.PageHeaderBand PageHeader;
        private DevExpress.XtraReports.UI.ReportFooterBand ReportFooter;
        private DevExpress.DataAccess.Sql.SqlDataSource sqlDataSource1;
        private DevExpress.XtraReports.UI.DetailReportBand ComprasReport;
        private DevExpress.XtraReports.UI.DetailBand Detail1;
        private DevExpress.XtraReports.UI.ReportHeaderBand ReportHeader1;
        private DevExpress.XtraReports.UI.XRLabel xrLabel1;
        private DevExpress.XtraReports.UI.XRTable xrTable5;
        private DevExpress.XtraReports.UI.XRTableRow xrTableRow8;
        private DevExpress.XtraReports.UI.XRTableCell fechaCompraCell;
        private DevExpress.XtraReports.UI.XRTableCell compraProductosCell;
        private DevExpress.XtraReports.UI.XRTableCell comprasCandidadCell;
        private DevExpress.XtraReports.UI.XRTableCell compraImporteCell;
        private DevExpress.XtraReports.UI.XRTable xrTable4;
        private DevExpress.XtraReports.UI.XRTableRow xrTableRow7;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell7;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell8;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell10;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell12;
        private DevExpress.XtraReports.UI.XRTableCell compraTiendaCell;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell1;
        private DevExpress.XtraReports.UI.XRLabel xrLabel4;
    }
}
