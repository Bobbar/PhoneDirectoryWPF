using PhoneDirectoryWPF.Data.Classes;
using System.ComponentModel;
using System.Data;

namespace PhoneDirectoryWPF.Containers
{
    public sealed class Extension : DataMapObject, INotifyPropertyChanged
    {
        private string firstName;
        private string lastName;
        private string user;
        private string number;
        private string department;

        public string ID { get; set; }

        [DataColumnName("id")]
        public override string Guid { get { return ID; } set { ID = value; } }

        public override string TableName { get; set; } = "extensions";

        [DataColumnName("extension")]
        public string Number
        {
            get
            {
                return number;
            }
            set
            {
                number = value;
                OnPropertyChanged(nameof(this.Number));
            }
        }

        [DataColumnName("user")]
        public string User
        {
            get
            {
                return user;
            }
            set
            {
                user = value;
                OnPropertyChanged(nameof(this.User));
            }
        }

        [DataColumnName("department")]
        public string Department
        {
            get
            {
                return department;
            }
            set
            {
                department = value;
                OnPropertyChanged(nameof(this.Department));
            }
        }

        [DataColumnName("firstname")]
        public string FirstName
        {
            get
            {
                return firstName;
            }
            set
            {
                firstName = value;
                FormatUser();
                OnPropertyChanged(nameof(this.FirstName));
            }
        }

        [DataColumnName("lastname")]
        public string LastName
        {
            get
            {
                return lastName;
            }
            set
            {
                lastName = value;
                FormatUser();
                OnPropertyChanged(nameof(this.LastName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Extension()
        {
        }

        public Extension(string id, string number, string firstName, string lastName)
        {
            this.ID = id;
            this.Number = number;
            this.FirstName = firstName;
            this.LastName = lastName;

            FormatUser();
        }

        public Extension(DataTable data) : base(data)
        {
        }

        public Extension(DataRow data) : base(data)
        {
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void CopyValues(Extension source)
        {
            this.Department = source.Department;
            this.FirstName = source.FirstName;
            this.Guid = source.Guid;
            this.ID = source.ID;
            this.LastName = source.LastName;
            this.Number = source.Number;
            this.User = source.User;
        }

        public override string ToString()
        {
            return string.Format("User: {0}  Number: {1}  Department: {2}  Firstname: {3}   Lastname: {4}", User, Number, Department, FirstName, LastName);
        }

        private void FormatUser()
        {
            this.User = string.Format("{0}, {1}", this.LastName, this.FirstName);
        }
    }
}