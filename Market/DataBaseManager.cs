using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OracleClient;
using System.Windows.Forms;
using System.IO;

namespace Market
{
    /// <summary> 数据库管理器
    /// </summary>
    class DataBaseManager
    {
        /// <summary> 数据库用户名
        /// </summary>
        private String User;
        /// <summary> 数据库密码
        /// </summary>
        private String Password;
        /// <summary> 登录验证成功标志
        /// </summary>
        public Boolean Success = false;
        /// <summary> 用于获取登录参数字符串
        /// </summary>
        /// <returns>返回 登录参数字符串</returns>
        private String GetConnectStr(String _User,String _Password)
        {
            return "User ID=" + _User + ";" + //用户名
                   "Password=" + _Password + //密码
                   ";Data Source=(DESCRIPTION = (ADDRESS_LIST= (ADDRESS = (PROTOCOL = TCP)(HOST = 127.0.0.1)(PORT = 1521))) (CONNECT_DATA = (SERVICE_NAME = xe)))";//默认localhost，端口1521
        }
        /// <summary> 初始化数据库管理器，测试连接
        /// </summary>
        /// <param name="_User">数据库账户名</param>
        /// <param name="_Password">数据库密码</param>
        public DataBaseManager(String _User,String _Password)
        {
            DataBaseLogin(_User,_Password);//验证登录数据库
        }
        /// <summary> 初始化数据库管理器，访问配置文件实现自动验证登录数据库
        /// </summary>
        public DataBaseManager()
        {
            if (File.Exists(Environment.CurrentDirectory + "/Config.inf"))//根目录存在配置文件时
            {
                String[] Config_Lines = File.ReadAllLines(Environment.CurrentDirectory + "/Config.inf");//读取配置文件中所有行
                if (Config_Lines.Length >= 3)//配置文件已记录数据库登录信息
                {
                    User = Config_Lines[1];//第二行为保存的数据库用户名
                    Password = Config_Lines[2];//第三行为保存的数据库密码
                    Success = true;//标记登录验证成功
                }   
            }
        }
        /// <summary> 数据库登录验证
        /// </summary>
        /// <param name="_User">数据库账户名</param>
        /// <param name="_Password">数据库密码</param>
        public void DataBaseLogin(String _User, String _Password)
        {
            String Connect_Str = GetConnectStr(_User, _Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库
                User = _User;//保存用户名
                Password = _Password;//保存密码
                Success = true;//标记数据库登录成功
            }
            catch (Exception)
            {
                MessageBox.Show(null, "无法连接到Oracle数据库，请检查Oracle数据库是否正常安装或用户名密码错误！", "数据库连接失败");//无法连接数据库则提示
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 创建员工表并插入默认超级管理员
        /// </summary>
        /// <returns>返回 true：成功 false：失败</returns>
        private Boolean CreateStaffTbl()
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库
                OracleCommand NewStafftbl = new OracleCommand(@"create table MarketTerminal_Staff
                                                                (
                                                                    StaffNo VARCHAR(5) PRIMARY KEY,
                                                                    StaffName VARCHAR(100) NOT NULL,
                                                                    SuperUser INT DEFAULT 0,
                                                                    Password VARCHAR(20)
                                                                )");//员工表
                NewStafftbl.Connection = Connect;//指定连接
                NewStafftbl.ExecuteNonQuery();//执行建表
                OracleCommand DefaultAdminInsert = new OracleCommand(@"insert into MarketTerminal_Staff
                                                                        values ('admin','DefaultSuperUser',1,'123')");//默认初始管理员
                DefaultAdminInsert.Connection = Connect;//指定连接
                DefaultAdminInsert.ExecuteNonQuery();//执行插入
                return true;//创建成功、插入成功
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return false;//创建员工表失败则返回false
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 创建商品表
        /// </summary>
        /// <returns>返回 true：成功 false：失败</returns>
        private Boolean CreateGoodsTbl()
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库
                OracleCommand NewStafftbl = new OracleCommand(@"create table MarketTerminal_Staff
                                                                (
                                                                    StaffNo VARCHAR(5) PRIMARY KEY,
                                                                    StaffName VARCHAR(100) NOT NULL,
                                                                    SuperUser INT DEFAULT 0,
                                                                    Password VARCHAR(20)
                                                                );");//员工表
                NewStafftbl.ExecuteNonQuery();//执行建表
                OracleCommand DefaultAdminInsert = new OracleCommand(@"insert into MarketTerminal_Staff
                                                                        values ('admin','DefaultSuperUser',1,'123');");//默认初始管理员
                NewStafftbl.ExecuteNonQuery();//执行插入
                return true;//创建成功、插入成功
            }
            catch (Exception)
            {
                return false;//创建员工表失败则返回false
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 删除员工表
        /// </summary>
        /// <returns>返回 true：成功 false：失败</returns>
        private Boolean DeleteStaffTbl()
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库
                OracleCommand DelStafftbl = new OracleCommand(@"drop table MarketTerminal_Staff purge");//删除员工表
                DelStafftbl.Connection = Connect;//指定连接
                DelStafftbl.ExecuteNonQuery();//执行删除表
                return true;//删除成功
            }
            catch (Exception)
            {
                return false;//创建员工表失败则返回false
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 初始化数据库，包括数据库创建、表结构创建
        /// </summary>
        /// <returns>返回 true：初始化成功 false：初始化失败</returns>
        public Boolean DataBaseSetting()
        {
            if (CreateStaffTbl() == true)//员工表创建成功
            {
                return true;
            }
            else
                return false;//员工表创建不成功，直接失败
        }
        /// <summary> 删除数据库
        /// </summary>
        /// <returns>返回 true：删除成功 false：删除失败</returns>
        public Boolean DataBaseDelete()
        {
            if (DeleteStaffTbl() == true)//删除员工表成功
            {
                return true;
            }
            else
                return false;//员工表删除不成功，直接失败
        }
        /// <summary> 创建配置文件，存放连接参数
        /// </summary>
        public void CreateConfigFile()
        {
            String[] Config_Line = {"DBReady = True",User,Password};//配置行
            File.WriteAllLines(Environment.CurrentDirectory + "/Config.inf",Config_Line,Encoding.UTF8);//向配置文件写入配置行
        }
        /// <summary> 删除配置文件
        /// </summary>
        public void DeleteConfigFile()
        {
            File.Delete(Environment.CurrentDirectory + "/Config.inf");//删除根目录下的配置文件
        }
        /// <summary> 获得员工列表
        /// </summary>
        /// <returns>返回 员工列表</returns>
        public List<String[]> GetStaffList()
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            List<String[]> StaffList = new List<string[]>();//初始化员工列表
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库
                OracleCommand QueryStaffList = new OracleCommand(@"select * from MarketTerminal_Staff");//查询语句
                QueryStaffList.Connection = Connect;//指定连接
                OracleDataReader StaffListReader = QueryStaffList.ExecuteReader();//执行建表
                while (StaffListReader.Read())//按行读取，直到结尾
                {
                    StaffList.Add(new String[] {StaffListReader[0].ToString(),//员工号
                                                StaffListReader[1].ToString(),//姓名
                                                StaffListReader[2].ToString() == "1"?"是":"否",//是否超级管理员
                                                StaffListReader[3].ToString()});//密码
                }
                return StaffList;//查询成功
            }
            catch (Exception)
            {
                return null;//查询员工表失败则返回null
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 验证员工工号与密码
        /// </summary>
        /// <param name="_User">员工工号</param>
        /// <param name="_Password">员工密码</param>
        /// <returns>返回 true：正确 false：错误</returns>
        public Boolean CheckStaff(String _User, String _Password)
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库
                OracleCommand CheckUser = new OracleCommand(@"select * from MarketTerminal_Staff
                                                              where StaffNo='" + _User + @"'and Password='" + _Password + @"'");//查询语句
                CheckUser.Connection = Connect;//指定连接
                OracleDataReader Reader = CheckUser.ExecuteReader();//执行查询
                if (Reader.Read())//查询到结果
                    return true;//验证成功
                else
                    return false;//验证失败
            }
            catch (Exception)
            {
                return false;//验证失败则返回false
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 检查员工是否是超级管理员
        /// </summary>
        /// <param name="_User">员工工号</param>
        /// <returns>返回 true：是 false：否</returns>
        public Boolean IsSuperUser(String _User)
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库
                OracleCommand CheckSU = new OracleCommand(@"select * from MarketTerminal_Staff
                                                              where StaffNo='" + _User + @"'and SuperUser='1'");//查询语句
                CheckSU.Connection = Connect;//指定连接
                OracleDataReader Reader = CheckSU.ExecuteReader();//执行查询
                if (Reader.Read())//查询到结果
                    return true;//验证成功
                else
                    return false;//验证失败
            }
            catch (Exception)
            {
                return false;//超级管理员则返回false
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }

        }
    }
}
