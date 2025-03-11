using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Student_Info
{
    public partial class Form1 : Form
    {
        public abstract class Person
        {
            public string ID { get; set; }
            public string Name { get; set; }
            public abstract override string ToString();
        }

        public class Student : Person
        {
            public string Major { get; set; }
            public string AdvisorID { get; set; }
            public double GPA { get; set; }

            public override string ToString() => $"{ID} - {Name} ({Major}) - GPA: {GPA}, Advisor: {AdvisorID}";
        }

        public class Teacher : Person
        {
            public string Major { get; set; }
            private readonly List<Student> advisees = new List<Student>();

            public void AddAdvisee(Student student) => advisees.Add(student);
            public List<Student> GetAdvisees() => new List<Student>(advisees);

            public override string ToString() => $"{ID} - {Name} ({Major})";
        }

        private readonly List<Student> students = new List<Student>();
        private readonly List<Teacher> teachers = new List<Teacher>();

        public Form1()
        {
            InitializeComponent();
        }

        private void AddStudent_Click(object sender, EventArgs e)
        {
            if (!double.TryParse(txtGPA.Text.Trim(), out double gpa))
            {
                ShowError("กรุณากรอก GPA เป็นตัวเลข");
                return;
            }

            var student = new Student
            {
                ID = txtID.Text.Trim(),
                Name = txtName.Text.Trim(),
                Major = txtMajor.Text.Trim(),
                GPA = gpa,
                AdvisorID = txtAdvisor.Text.Trim()
            };
            students.Add(student);
            lstStudents.Items.Add(student);

            var advisor = teachers.FirstOrDefault(t => t.ID == student.AdvisorID);
            advisor?.AddAdvisee(student);
        }

        private void AddTeacher_Click(object sender, EventArgs e)
        {
            var teacher = new Teacher
            {
                ID = txtTeacherID.Text.Trim(),
                Name = txtTeacherName.Text.Trim(),
                Major = txtTeacherMajor.Text.Trim()
            };

            if (teachers.Any(t => t.ID == teacher.ID))
            {
                ShowError("อาจารย์คนนี้มีอยู่แล้ว!");
                return;
            }

            teachers.Add(teacher);
            lstTeachers.Items.Add(teacher);
        }

        private void ShowTopGPA_Click(object sender, EventArgs e)
        {
            if (students.Count == 0)
            {
                ShowInfo("ยังไม่มีนักศึกษา");
                return;
            }

            var topStudent = students.OrderByDescending(s => s.GPA).First();
            lblTopStudent.Text = $"{topStudent.Name} ({topStudent.GPA})";
        }

        private void ShowAdvisees_Click(object sender, EventArgs e)
        {
            if (lstTeachers.SelectedItem is Teacher selectedTeacher)
            {
                var adviseeList = selectedTeacher.GetAdvisees().Any()
                    ? string.Join("\n", selectedTeacher.GetAdvisees())
                    : "ไม่มีนักศึกษาในที่ปรึกษา";
                ShowInfo(adviseeList, $"นักศึกษาของ {selectedTeacher.Name}");
            }
        }

        private void ShowError(string message) => MessageBox.Show(message, "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
        private void ShowInfo(string message, string title = "ข้อมูล") => MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
