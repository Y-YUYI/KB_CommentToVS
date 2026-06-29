using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace KB_CommentToVS
{
    public class SettingsDialog : Form
    {
        private FlowLayoutPanel linesPanel;
        private List<TextBox> lineBoxes = new List<TextBox>();
        private TextBox txtModifier;
        private ComboBox cmbDateFormat;
        private ComboBox cmbPrefix;
        private Button btnOK;
        private Button btnApply;
        private Button btnCancel;
        private Button btnAddLine;
        private SnippetConfig config;

        private const int LabelWidth = 150;
        private const int RowHeight = 32;
        private const int TextBoxHeight = 26;

        public SettingsDialog()
        {
            InitializeComponent();
            this.Load += (s, e) =>
            {
                config = SnippetConfig.Load();
                LoadSettings();
            };
        }

        private void InitializeComponent()
        {
            this.Text = "代码注释格式设置";
            this.Size = new Size(750, 650);
            this.MinimumSize = new Size(580, 400);
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            this.StartPosition = FormStartPosition.CenterParent;
            this.ShowInTaskbar = false;
            this.Font = SystemFonts.MessageBoxFont;
            this.Padding = new Padding(14);

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 7,
                Padding = new Padding(0),
                Margin = Padding.Empty
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, LabelWidth));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            // 第0行：注释内容标题 + 滚动面板
            var lblLinesTitle = new Label
            {
                Text = "注释内容:",
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleLeft,
                Height = 28,
                Margin = new Padding(0, 4, 0, 0)
            };
            layout.Controls.Add(lblLinesTitle, 0, 0);

            // 右侧：滚动面板 + 添加按钮
            var rightPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(0),
                Margin = Padding.Empty
            };
            rightPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            rightPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            rightPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));

            // 可滚动的行编辑面板
            var scrollPanel = new Panel { Dock = DockStyle.Fill, AutoScroll = true, BorderStyle = BorderStyle.None };
            linesPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(0),
                Margin = Padding.Empty
            };
            scrollPanel.Controls.Add(linesPanel);

            // 添加一行按钮
            btnAddLine = new Button
            {
                Text = "+ 添加一行",
                Dock = DockStyle.Fill,
                FlatStyle = FlatStyle.System,
                Height = 28
            };
            btnAddLine.Click += (s, e) => AddLine("");

            rightPanel.Controls.Add(scrollPanel, 0, 0);
            rightPanel.Controls.Add(btnAddLine, 0, 1);

            layout.Controls.Add(rightPanel, 1, 0);

            // 占满第0行
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            // 第1行：修改人
            var lblModifier = new Label { Text = "修改人:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
            txtModifier = new TextBox { Dock = DockStyle.Fill, Height = TextBoxHeight };
            layout.Controls.Add(lblModifier, 0, 1);
            layout.Controls.Add(txtModifier, 1, 1);
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, RowHeight));

            // 第2行：日期格式（下拉可选，也可手动输入）
            var lblDateFormat = new Label { Text = "日期格式:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
            cmbDateFormat = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDown,  // 可下拉也可输入
                Height = TextBoxHeight
            };
            cmbDateFormat.Items.AddRange(new object[]
            {
                "yyyy-MM-dd HH:mm",
                "yyyy-MM-dd HH:mm:ss",
                "yyyy/MM/dd HH:mm",
                "yyyy-MM-dd",
                "MM-dd HH:mm",
                "MM月dd日 HH:mm",
                "yyyy年MM月dd日"
            });
            layout.Controls.Add(lblDateFormat, 0, 2);
            layout.Controls.Add(cmbDateFormat, 1, 2);
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, RowHeight));

            // 第3行：注释前缀
            var lblPrefix = new Label { Text = "注释前缀:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
            cmbPrefix = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDown, Height = TextBoxHeight };
            cmbPrefix.Items.AddRange(new object[] { "///", "//", "<!--", "#", "/*", "--" });
            layout.Controls.Add(lblPrefix, 0, 3);
            layout.Controls.Add(cmbPrefix, 1, 3);
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, RowHeight));

            // 第4行：快捷键信息
            var lblShortcutInfo = new Label { Text = "当前快捷键:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
            var lblShortcut = new Label
            {
                Text = "可在 VS: 工具 → 选项 → 环境 → 键盘 中修改",
                Dock = DockStyle.Fill,
                ForeColor = Color.Gray,
                TextAlign = ContentAlignment.MiddleLeft
            };
            layout.Controls.Add(lblShortcutInfo, 0, 4);
            layout.Controls.Add(lblShortcut, 1, 4);
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, RowHeight));

            // 第5行：提示
            var lblInfo = new Label
            {
                Text = "提示: 每行会自动加上前缀(///)。可用 {Modifier} 和 {DateTime} 作为占位符",
                Dock = DockStyle.Fill,
                ForeColor = Color.Gray,
                TextAlign = ContentAlignment.MiddleLeft
            };
            layout.Controls.Add(lblInfo, 0, 5);
            layout.SetColumnSpan(lblInfo, 2);
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, RowHeight));

            // 第6行：按钮
            var btnPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = Padding.Empty,
                Margin = Padding.Empty
            };
            btnCancel = new Button { Text = "取消", Size = new Size(80, 30), Margin = new Padding(4, 0, 0, 0) };
            btnCancel.Click += (s, e) => this.Close();
            btnApply = new Button { Text = "应用", Size = new Size(80, 30), Margin = new Padding(4, 0, 0, 0) };
            btnApply.Click += (s, e) => ApplySettings();
            btnOK = new Button { Text = "确定", Size = new Size(80, 30), Margin = new Padding(4, 0, 0, 0) };
            btnOK.Click += (s, e) => { ApplySettings(); this.DialogResult = DialogResult.OK; this.Close(); };
            btnPanel.Controls.AddRange(new Control[] { btnCancel, btnApply, btnOK });
            layout.Controls.Add(btnPanel, 0, 6);
            layout.SetColumnSpan(btnPanel, 2);
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));

            this.Controls.Add(layout);
        }

        private void LoadSettings()
        {
            txtModifier.Text = config.Modifier;

            // 日期格式
            cmbDateFormat.Text = config.DateFormat;

            // 注释前缀
            int idx = cmbPrefix.Items.IndexOf(config.CommentPrefix);
            cmbPrefix.SelectedIndex = idx >= 0 ? idx : 0;

            // 按行加载模板（跳过空行）
            var lines = config.Template.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                string trimmed = line.Trim();
                if (!string.IsNullOrEmpty(trimmed))
                    AddLine(trimmed);
            }
            if (lineBoxes.Count == 0)
                AddLine("");
        }

        private void AddLine(string text)
        {
            if (linesPanel == null) return;

            int panelWidth = linesPanel.ClientSize.Width;
            if (panelWidth <= 20) panelWidth = 500;

            var row = new Panel
            {
                Height = 30,
                Width = panelWidth - 5,
                Margin = new Padding(0, 2, 0, 2),
                Padding = Padding.Empty
            };

            var tb = new TextBox
            {
                Text = text ?? "",
                Location = new Point(0, 1),
                Width = row.Width - 32,
                Height = TextBoxHeight,
                Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right
            };

            var btnDel = new Button
            {
                Text = "×",
                Location = new Point(row.Width - 28, 0),
                Size = new Size(26, 26),
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnDel.Click += (sender, args) =>
            {
                int index = lineBoxes.IndexOf(tb);
                if (index >= 0)
                {
                    lineBoxes.RemoveAt(index);
                    linesPanel.Controls.Remove(row);
                    row.Dispose();
                }
            };

            row.Controls.Add(tb);
            row.Controls.Add(btnDel);
            linesPanel.Controls.Add(row);
            lineBoxes.Add(tb);
        }

        private void UpdateRowWidths()
        {
            if (linesPanel == null) return;
            int panelWidth = linesPanel.ClientSize.Width;
            if (panelWidth < 50) panelWidth = 500;
            foreach (Control c in linesPanel.Controls)
            {
                if (c == null) continue;
                c.Width = panelWidth - 5;
                if (c is Panel p && p.Controls.Count >= 1 && p.Controls[0] is TextBox tb)
                {
                    tb.Width = p.Width - 32;
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateRowWidths();
        }

        private void ApplySettings()
        {
            // 收集非空行
            var lines = new List<string>();
            foreach (var tb in lineBoxes)
            {
                string text = tb.Text.Trim();
                if (!string.IsNullOrEmpty(text))
                    lines.Add(text);
            }
            config.Template = string.Join("\n", lines);
            config.Modifier = txtModifier.Text.Trim();
            config.DateFormat = cmbDateFormat.Text.Trim();
            config.CommentPrefix = cmbPrefix.SelectedItem?.ToString() ?? "///";
            config.Save();
        }
    }
}
