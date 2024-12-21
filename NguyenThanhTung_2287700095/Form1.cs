using NguyenThanhTung_2287700095.Module;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NguyenThanhTung_2287700095
{
    public partial class frmSinhvien : Form
    {
        private Model1 db;
        public frmSinhvien()
        { 
            InitializeComponent();
            db = new Model1();
        }

        private void frmSinhvien_Load(object sender, EventArgs e)
        {
            LoadData();
            LoadLop();
            lvSinhvien.CellClick += lvSinhvien_CellClick; // Kết nối sự kiện CellClick
        }
        private void LoadLop()
        {
            var lops = db.Lops.ToList();
            cboLop.DataSource = lops;
            cboLop.DisplayMember = "TenLop";
            cboLop.ValueMember = "MaLop";
        }
        private void LoadData()
        {
            var data = db.Sinhviens
                .Select(sv => new
                {
                    MaSV = sv.MaSV,
                    HotenSV = sv.HotenSV,
                    NgaySinh = sv.NgaySinh,
                    MaLop = sv.Lop.MaLop,
                    TenLop = sv.Lop.TenLop
                })
                .ToList();

            lvSinhvien.AutoGenerateColumns = false;
            lvSinhvien.Columns.Clear();

            lvSinhvien.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Mã SV", DataPropertyName = "MaSV", Name = "MaSV" });
            lvSinhvien.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Họ và Tên", DataPropertyName = "HotenSV", Name = "HotenSV" });
            lvSinhvien.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Ngày Sinh", DataPropertyName = "NgaySinh", Name = "NgaySinh" });
            lvSinhvien.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Lớp", DataPropertyName = "TenLop", Name = "TenLop" });

            lvSinhvien.DataSource = data;
        }


        private void btTim_Click(object sender, EventArgs e)
        {

        }

        private void btThem_Click(object sender, EventArgs e)
        {
            txtMaSV.Clear();
            txtHotenSV.Clear();
            dtNgaysinh.Value = DateTime.Now;
            cboLop.SelectedIndex = -1;
            btLuu.Enabled = true;
            btKhong.Enabled = true;
        }


        private void btXoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMaSV.Text))
            {
                MessageBox.Show("Vui lòng chọn một sinh viên để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirmResult = MessageBox.Show("Bạn có chắc chắn muốn xóa sinh viên này?",
                                                "Xác nhận xóa",
                                                MessageBoxButtons.YesNo,
                                                MessageBoxIcon.Warning);

            if (confirmResult == DialogResult.Yes)
            {
                var sinhvien = db.Sinhviens.FirstOrDefault(sv => sv.MaSV == txtMaSV.Text.Trim());
                if (sinhvien != null)
                {
                    db.Sinhviens.Remove(sinhvien);
                    db.SaveChanges();
                    LoadData();

                    txtMaSV.Clear();
                    txtHotenSV.Clear();
                    dtNgaysinh.Value = DateTime.Now;
                    cboLop.SelectedIndex = -1;

                    MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Không tìm thấy sinh viên để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }




        private void btSua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(originalMaSV))
            {
                MessageBox.Show("Vui lòng chọn một sinh viên để sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var sinhvien = db.Sinhviens.FirstOrDefault(sv => sv.MaSV == originalMaSV);
            if (sinhvien == null)
            {
                MessageBox.Show("Không tìm thấy sinh viên để sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            sinhvien.HotenSV = txtHotenSV.Text.Trim();
            sinhvien.NgaySinh = dtNgaysinh.Value;
            sinhvien.MaLop = ((Lop)cboLop.SelectedItem)?.MaLop;

            db.SaveChanges();
            LoadData();

            MessageBox.Show("Sửa thông tin thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }





        private void btLuu_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaSV.Text) || string.IsNullOrWhiteSpace(txtHotenSV.Text) || cboLop.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var maLop = ((Lop)cboLop.SelectedItem)?.MaLop;
            if (string.IsNullOrWhiteSpace(maLop))
            {
                MessageBox.Show("Vui lòng chọn lớp hợp lệ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var exists = db.Sinhviens.Any(sv => sv.MaSV == txtMaSV.Text.Trim());
            if (exists)
            {
                MessageBox.Show("Mã sinh viên đã tồn tại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Tạo sinh viên mới
            var sinhvien = new Sinhvien
            {
                MaSV = txtMaSV.Text.Trim(),
                HotenSV = txtHotenSV.Text.Trim(),
                NgaySinh = dtNgaysinh.Value,
                MaLop = maLop
            };

            db.Sinhviens.Add(sinhvien);
            db.SaveChanges();

            LoadData(); // Cập nhật danh sách
            MessageBox.Show("Thêm sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

            btLuu.Enabled = false;
            btKhong.Enabled = false;
        }




        private void btKhong_Click(object sender, EventArgs e)
        {
            txtMaSV.Clear();
            txtHotenSV.Clear();
            dtNgaysinh.Value = DateTime.Now;
            cboLop.SelectedIndex = -1;
            btLuu.Enabled = false;
            btKhong.Enabled = false;
        }


        private void btThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private string originalMaSV = null;
        private void lvSinhvien_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = lvSinhvien.Rows[e.RowIndex];
                originalMaSV = row.Cells["MaSV"].Value?.ToString();
                txtMaSV.Text = originalMaSV;
                txtMaSV.ReadOnly = true;
                txtHotenSV.Text = row.Cells["HotenSV"].Value?.ToString() ?? string.Empty;

                if (row.Cells["NgaySinh"].Value != null && DateTime.TryParse(row.Cells["NgaySinh"].Value.ToString(), out DateTime ngaySinh))
                {
                    dtNgaysinh.Value = ngaySinh;
                }
                else
                {
                    dtNgaysinh.Value = DateTime.Now;
                }

                if (row.Cells["TenLop"].Value != null)
                {
                    string tenLop = row.Cells["TenLop"].Value.ToString();
                    var lop = cboLop.Items.Cast<Lop>().FirstOrDefault(l => l.TenLop == tenLop);
                    cboLop.SelectedValue = lop?.MaLop ?? null;
                }
                else
                {
                    cboLop.SelectedIndex = -1;
                }
            }
        }

        private void lvSinhvien_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
