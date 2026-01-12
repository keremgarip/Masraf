<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Masraf._default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="row contentSticky">
        <div class="col-lg-12 col-md-12 col-xs-12 mb-30 ">
            <br />
            <br />
            <br />
            <br />
            <br />
        </div>
    </div>
    <div class="box box-block bg-white">
        <div class="table-responsive">
            <div class="row">
                <div class="d-flex align-items-center justify-content-between">
                    <div class=" chart" style="display: none;">
                        <p style="padding-left: 90px;"><%=generalSumStrMasrafOnaylanma%></p>
                        <canvas id="doughnutChartGeneralSumMasrafOnaylanma"></canvas>
                        <p id="doughnutChartGeneralSumNoDataMasrafOnaylanma" style="display: none">
                            No Data
                        </p>
                        <div style="width: 100%; font-weight: bold; text-align: center;">Total: <%=generalSumCountMasrafOnaylanma%></div>
                    </div>
                </div>
                <div class="d-flex align-items-center justify-content-between">
                    <div class=" chart" style="display: none;">
                        <p style="padding-left: 90px;"><%=generalSumStrMasrafAy%></p>
                        <canvas id="doughnutChartGeneralSumPeriod"></canvas>
                        <p id="doughnutChartGeneralSumNoDataPeriod" style="display: none">
                            No Data
                        </p>
                        <div style="width: 100%; font-weight: bold; text-align: center;">Total: <%=generalSumCountMasrafAy%></div>
                    </div>
                </div>
                <div class="d-flex align-items-center justify-content-between">
                    <div class=" chart" style="display: none;">
                        <p style="padding-left: 10px;"><%=generalSumStrProofMagaza%></p>
                        <canvas id="doughnutChartGeneralSumMagaza"></canvas>
                        <p id="doughnutChartGeneralSumNoDataMagaza" style="display: none">
                            No Data
                        </p>
                        <div style="width: 100%; font-weight: bold; text-align: center;">Total: <%=generalSumCountProofMagaza%> TL</div>
                    </div>
                </div>
            </div>
        </div>
        <div class="box box-block bg-white" style="width: 100%;">
            <div id="divPersonellerim" runat="server" visible="false" class="table-responsive">
                <p style="padding-left: 10px; font-weight: bold;">Personeller (Geçen ay)</p>
                <div class="box box-block bg-white">
                    <div class="table-responsive">
                        <table class="recordTable dataTable no-footer" role="grid" style="width: 1231px;">
                            <thead>
                                <tr>
                                    <th>Ad Soyad</th>
                                    <th>Email</th>
                                    <th>Telefon</th>
                                    <th id="th1" runat="server">Gönderilen Masraf adet</th>
                                    <th id="th3" runat="server">Toplam Tutar</th>
                                    <th id="th2" runat="server">Gönderilmeyen Masraf adet</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater runat="server" ID="rptpersonel" OnItemDataBound="rptpersonel_ItemDataBound">
                                    <ItemTemplate>
                                        <tr id="trRow" runat="server" role="row" class="odd">
                                            <td><%#Eval("usr_name") + " " + Eval("usr_surname") %></td>
                                            <td><%#Eval("usr_email_address") %></td>
                                            <td><%#Eval("usr_phone") %></td>
                                            <td>
                                                <asp:Literal ID="ltrlgonderilenMasrafAdet" runat="server"></asp:Literal>
                                            </td>
                                            <td>
                                                <asp:Literal ID="ltrlToplamTutar" runat="server"></asp:Literal>
                                            </td>
                                            <td>
                                                <asp:Literal ID="ltrlgonderilmeyenMasrafAdet" runat="server"></asp:Literal>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                                <asp:Repeater runat="server" ID="rptpersoneladmin" Visible="false" OnItemDataBound="rptpersoneladmin_ItemDataBound">
                                    <ItemTemplate>
                                        <tr id="trRow" runat="server" role="row" class="odd">
                                            <td><%#Eval("usr_name") + " " + Eval("usr_surname") %></td>
                                            <td><%#Eval("usr_email_address") %></td>
                                            <td><%#Eval("usr_phone") %></td>
                                            <td>
                                                <%#Eval("ToplamTutar") %>&nbsp;TL
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>
                    <div style="width: 100%; font-weight: bold; text-align: center;"></div>
                </div>
            </div>
            <div id="divPersonel" runat="server" visible="false" class="table-responsive">
                <p style="padding-left: 10px; font-weight: bold;">Masraflarım</p>
                <div class="box box-block bg-white">
                    <div class="table-responsive">
                        <table class="recordTable dataTable no-footer" role="grid" style="width: 1231px;">
                            <thead>
                                <tr>
                                    <th>Tarih</th>
                                    <th>Dönem</th>
                                    <th>Toplam Tutar</th>
                                    <th>&nbsp;</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater runat="server" ID="rptrPersonelKendisi" OnItemDataBound="rptrPersonelKendisi_ItemDataBound">
                                    <ItemTemplate>
                                        <tr id="trRow" runat="server" role="row" class="odd">
                                            <td>
                                                <asp:Literal ID="ltrlTarih" runat="server"></asp:Literal>
                                            </td>
                                            <td>
                                                <asp:Literal ID="ltrlDonem" runat="server"></asp:Literal>
                                            </td>
                                            <td>
                                                <asp:Literal ID="ltrlToplamTutar" runat="server"></asp:Literal>
                                            </td>
                                            <td>
                                                <a id="aDetay" runat="server">Detay</a>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>
                    <div style="width: 100%; font-weight: bold; text-align: center;"></div>
                </div>
            </div>
        </div>

        <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
        <script src="https://cdn.jsdelivr.net/gh/emn178/chartjs-plugin-labels/src/chartjs-plugin-labels.js"></script>
        <script>
            var doughnutChartGeneralSumMasrafOnaylanma = document.getElementById('doughnutChartGeneralSumMasrafOnaylanma').getContext('2d');
            var doughnutChartGeneralSumMasrafOnaylanma = new Chart(doughnutChartGeneralSumMasrafOnaylanma, {
                type: 'doughnut',
                data: {
                    datasets: [{
                        data: [<%=generalSumNumberMasrafOnaylanma%>],
                        backgroundColor: [
                            '#5C6BC0',
                            '#B39DDB'
                        ]
                    }],
                    labels: [<%=generalSumTitleMasrafOnaylanma%>]
                },
                options: {
                    responsive: true,
                    legend: {
                        position: 'right'
                    },
                    plugins: {
                        labels: {
                            render: 'percentage',
                            fontColor: ['white', 'white', 'white', 'white', 'white', 'white', 'white', 'white', 'white'],
                            precision: 2
                        }
                    },
                }
            });

            var doughnutChartGeneralSumPeriod = document.getElementById('doughnutChartGeneralSumPeriod').getContext('2d');
            var doughnutChartGeneralSumPeriod = new Chart(doughnutChartGeneralSumPeriod, {
                type: 'doughnut',
                data: {
                    datasets: [{
                        data: [<%=generalSumNumberMasrafAy%>],
                        backgroundColor: [
                            '#5C6BC0',
                            '#B39DDB'
                        ]
                    }],
                    labels: [<%=generalSumTitleMasrafAy%>]
                },
                options: {
                    responsive: true,
                    legend: {
                        position: 'right'
                    },
                    plugins: {
                        labels: {
                            render: 'percentage',
                            fontColor: ['white', 'white', 'white', 'white', 'white', 'white', 'white', 'white', 'white'],
                            precision: 2
                        }
                    },
                }
            });

            var doughnutChartGeneralSumMagaza = document.getElementById('doughnutChartGeneralSumMagaza').getContext('2d');
            var doughnutChartGeneralSumMagaza = new Chart(doughnutChartGeneralSumMagaza, {
                type: 'doughnut',
                data: {
                    datasets: [{
                        data: [<%=generalSumNumberProofMagaza%>],
                        backgroundColor: [
                            '#5C6BC0',
                            '#B39DDB'
                        ]
                    }],
                    labels: [<%=generalSumTitleProofMagaza%>]
                },
                options: {
                    responsive: true,
                    legend: {
                        position: 'right'
                    },
                    plugins: {
                        labels: {
                            render: 'percentage',
                            fontColor: ['white', 'white', 'white', 'white', 'white', 'white', 'white', 'white', 'white'],
                            precision: 2
                        }
                    },
                }
            });

        </script>
</asp:Content>
