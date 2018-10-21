using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Consul.Clear.Winform
{
    public partial class Main : Form
    {
        private const string query_addr = "/v1/health/state/critical";
        private const string clear_addr = "/v1/agent/service/deregister/";
        private string input_addr = "";

        public Main()
        {
            InitializeComponent();
            dataGridView1.AutoGenerateColumns = false;
        }

        private async void btn_query_Click(object sender, EventArgs e)
        {
            string ip = tb_ip.Text;
            string port = tb_port.Text;
            if (string.IsNullOrEmpty(ip))
            {
                MessageBox.Show("请输入服务地址");
                return;
            }

            input_addr = string.IsNullOrEmpty(port) ? ip : ip + ":" + port;
            JArray ja = await getService();

            if (ja == null)
            {
                return;
            }

            await Task.Factory.StartNew(() => SetData(ja));
        }

        private async Task<JArray> getService()
        {
            if (string.IsNullOrEmpty(input_addr))
            {
                return null;
            }

            //建立 HttpClient
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync(string.Format("http://{0}{1}", input_addr, query_addr));

                    if (!response.IsSuccessStatusCode)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("请求返回失败\r\n");
                        sb.Append(" 返回码:" + response.StatusCode + "\r\n");
                        sb.Append(" 返回信息:" + response.Content + "\r\n");
                        AppendLog(sb.ToString());
                        return null;
                    }

                    string result = await response.Content.ReadAsStringAsync();
                    JArray ja = (JArray)JsonConvert.DeserializeObject(result);
                    AppendLog("获取数据成功，获取到 " + ja.Count + " 条服务");
                    return ja;
                }
                catch (Exception ex)
                {
                    AppendLog(ex.Message);
                    return null;
                }
            }
        }

        private void btn_select_Click(object sender, EventArgs e)
        {
            bool state = Convert.ToInt16(btn_select.Tag) == 1;
            foreach (DataGridViewRow item in dataGridView1.Rows)
            {
                item.Cells[0].Value = state;
            }
            btn_select.Tag = state ? 0 : 1;
            btn_select.Text = state ? "全不选" : "全选";
        }

        private async void btn_clear_Click(object sender, EventArgs e)
        {
            List<string> list = new List<string>();

            foreach (DataGridViewRow item in dataGridView1.Rows)
            {
                bool isChecked = Convert.ToBoolean(item.Cells[0].EditedFormattedValue);
                if (isChecked)
                {
                    list.Add(item.Cells["ServiceID"].Value.ToString());
                }
            }

            if (list.Count == 0)
            {
                MessageBox.Show("您当前没有选中的服务");
                return;
            }

            if (MessageBox.Show("您当前选中了 " + list.Count + " 条服务\r\n是否确认删除", "", MessageBoxButtons.OKCancel) != DialogResult.OK) { return; }

            List<string> removed = await delService(list);

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                string serviceID = dataGridView1.Rows[i].Cells["ServiceID"].Value.ToString();
                if (!removed.Contains(serviceID)) { continue; }
                dataGridView1.Rows.RemoveAt(i);
                i--;
            }

            AppendLog("删除成功，已删除 " + removed.Count + " 条服务");
        }

        private async Task<List<string>> delService(List<string> ids)
        {
            List<string> list = new List<string>();

            using (HttpClient client = new HttpClient())
            {
                foreach (string id in ids)
                {
                    try
                    {
                        var response = await client.PutAsync(string.Format("http://{0}{1}{2}", input_addr, clear_addr, id), null);

                        if (!response.IsSuccessStatusCode)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.Append("请求返回失败\r\n");
                            sb.Append(" 返回码:" + response.StatusCode + "\r\n");
                            sb.Append(" 返回信息:" + response.Content + "\r\n");
                            AppendLog(sb.ToString());
                            continue;
                        }

                        AppendLog("删除成功，服务ID： " + id);
                        list.Add(id);
                    }
                    catch (Exception ex)
                    {
                        AppendLog(ex.Message);
                    }
                }
            }

            return list;
        }

        #region 操作UI
        private delegate void appendLog(string log);

        private void AppendLog(string log)
        {
            if (tb_log.InvokeRequired)
            {
                BeginInvoke(new appendLog(AppendLog), log);
            }
            else
            {
                tb_log.AppendText(log + "\r\n");
            }
        }

        private delegate void setData(JArray array);

        private void SetData(JArray array)
        {
            if (dataGridView1.InvokeRequired)
            {
                BeginInvoke(new setData(SetData), array);
            }
            else
            {
                dataGridView1.DataSource = array;
                foreach (DataGridViewRow item in dataGridView1.Rows)
                {
                    item.Cells[0].Value = true;
                }
            }
        }
        #endregion
    }
}
