namespace sdORM.Common
{
    public class DataBaseConfig
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string DataBase { get; set; }

        public string Server { get; set; }

        public override string ToString()
        {
            return $"Server={this.Server}; database={this.DataBase}; UID={this.UserName}; password={this.Password}";
        }
    }
}