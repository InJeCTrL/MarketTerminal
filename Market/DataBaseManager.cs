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
                OracleCommand NewGoodstbl = new OracleCommand(@"create table MarketTerminal_Goods
                                                                (
                                                                    GoodsNo VARCHAR(20) PRIMARY KEY,
                                                                    GoodsName VARCHAR(100) NOT NULL,
                                                                    In_Price NUMBER NOT NULL,
                                                                    Out_Price NUMBER NOT NULL,
                                                                    MarketLeft INT NOT NULL,
                                                                    TrunkLeft INT NOT NULL,
                                                                    Brand VARCHAR(30) NOT NULL,
                                                                    Unit VARCHAR(8) NOT NULL
                                                                )");//商品表建表语句
                NewGoodstbl.Connection = Connect;//指定连接
                NewGoodstbl.ExecuteNonQuery();//执行建表
                return true;//创建成功
            }
            catch (Exception)
            {
                return false;//创建商品表失败则返回false
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 创建账目表
        /// </summary>
        /// <returns>返回 true：成功 false：失败</returns>
        private Boolean CreateAccountTbl()
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库
                OracleCommand NewAccounttbl = new OracleCommand(@"create table MarketTerminal_Account
                                                                    (
                                                                        AccountNo VARCHAR(20) PRIMARY KEY,
                                                                        GoodsNo VARCHAR(20) NOT NULL,
                                                                        GoodsName VARCHAR(100) NOT NULL,
                                                                        In_Price NUMBER NOT NULL,
                                                                        Out_Price NUMBER NOT NULL,
                                                                        OutNum INT NOT NULL,
                                                                        Unit VARCHAR(8) NOT NULL,
                                                                        Price NUMBER NOT NULL
                                                                    )");//账目表建表语句
                NewAccounttbl.Connection = Connect;//指定连接
                NewAccounttbl.ExecuteNonQuery();//执行建表
                return true;//创建成功
            }
            catch (Exception)
            {
                return false;//创建失败则返回false
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 创建商品表上针对于账目表的触发器
        /// </summary>
        /// <returns>返回 true：成功 false：失败</returns>
        private Boolean CreateTrigger()
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库
                OracleCommand NewTrigger = new OracleCommand(@"create or replace trigger T_UpAcc after update
                                                                on MarketTerminal_Goods
                                                                for each row
                                                                begin
                                                                 IF :new.MarketLeft<:old.MarketLeft THEN
                                                                    insert into MARKETTERMINAL_ACCOUNT
                                                                    values(to_char(systimestamp,'YYYYMMDDHH24MISSFF'),:new.GOODSNO,:new.GOODSName,:new.IN_PRICE,:new.OUT_PRICE,:old.marketleft-:new.marketleft,:new.unit,(:old.marketleft-:new.marketleft)*(:new.OUT_PRICE-:new.IN_PRICE));
                                                                 END IF;
                                                                end;");//创建触发器语句
                NewTrigger.Connection = Connect;//指定连接
                NewTrigger.ExecuteNonQuery();//执行创建
                return true;//创建成功
            }
            catch (Exception)
            {
                return false;//创建失败则返回false
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
                return false;//失败则返回false
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 删除商品表
        /// </summary>
        /// <returns>返回 true：成功 false：失败</returns>
        private Boolean DeleteGoodsTbl()
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库
                OracleCommand DelGoodstbl = new OracleCommand(@"drop table MarketTerminal_Goods purge");//删除商品表
                DelGoodstbl.Connection = Connect;//指定连接
                DelGoodstbl.ExecuteNonQuery();//执行删除表
                return true;//删除成功
            }
            catch (Exception)
            {
                return false;//失败则返回false
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 删除账目表
        /// </summary>
        /// <returns>返回 true：成功 false：失败</returns>
        private Boolean DeleteAccountTbl()
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库
                OracleCommand DelAccounttbl = new OracleCommand(@"drop table MarketTerminal_Account purge");//删除账目表
                DelAccounttbl.Connection = Connect;//指定连接
                DelAccounttbl.ExecuteNonQuery();//执行删除表
                return true;//删除成功
            }
            catch (Exception)
            {
                return false;//失败则返回false
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 删除触发器
        /// </summary>
        /// <returns>返回 true：成功 false：失败</returns>
        private Boolean DeleteTrigger()
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库
                OracleCommand DelTrigger = new OracleCommand(@"drop trigger T_UpAcc");//删除触发器
                DelTrigger.Connection = Connect;//指定连接
                DelTrigger.ExecuteNonQuery();//执行删除
                return true;//删除成功
            }
            catch (Exception)
            {
                return false;//失败则返回false
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
                if (CreateGoodsTbl() == true)//商品表创建成功
                {
                    if (CreateAccountTbl() == true)//创建账目表成功
                    {
                        if (CreateTrigger() == true)//创建触发器成功
                            return true;
                        else
                        {//回滚账目表、商品表、员工表
                            DeleteAccountTbl();//删除账目表
                            DeleteStaffTbl();//删除员工表
                            DeleteGoodsTbl();//删除商品表
                            return false;
                        }
                    }
                    else
                    {//回滚商品表、员工表
                        DeleteStaffTbl();//删除员工表
                        DeleteGoodsTbl();//删除商品表
                        return false;
                    }
                }
                else
                {//回滚员工表
                    DeleteStaffTbl();//删除员工表
                    return false;
                }
            }
            else
                return false;//员工表创建不成功，直接失败
        }
        /// <summary> 删除数据库
        /// </summary>
        /// <returns>返回 true：删除成功 false：删除失败</returns>
        public Boolean DataBaseDelete()
        {
            DeleteStaffTbl();//删除员工表
            DeleteTrigger();//删除触发器
            DeleteGoodsTbl();//删除商品表
            DeleteAccountTbl();//删除账目表
            return true;
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
        /// <summary> 获得商品列表
        /// </summary>
        /// <returns>返回 商品列表</returns>
        public List<String[]> GetGoodsList()
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            List<String[]> GoodsList = new List<string[]>();//初始化商品列表
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库
                OracleCommand QueryGoodsList = new OracleCommand(@"select * from MarketTerminal_Goods");//查询语句
                QueryGoodsList.Connection = Connect;//指定连接
                OracleDataReader GoodsListReader = QueryGoodsList.ExecuteReader();//执行查询
                while (GoodsListReader.Read())//按行读取，直到结尾
                {
                    GoodsList.Add(new String[] {GoodsListReader[0].ToString(),//商品编号
                                                GoodsListReader[1].ToString(),//商品名
                                                GoodsListReader[2].ToString(),//进价
                                                GoodsListReader[3].ToString(),//售价
                                                GoodsListReader[4].ToString(),//货架剩余
                                                GoodsListReader[5].ToString(),//仓库剩余
                                                GoodsListReader[6].ToString(),//品牌
                                                GoodsListReader[7].ToString()});//单位
                }
                return GoodsList;//查询成功
            }
            catch (Exception)
            {
                return null;//查询商品表失败则返回null
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
        /// <summary> 更新员工姓名
        /// </summary>
        /// <param name="_StaffNo">员工工号</param>
        /// <param name="_NewName">员工新姓名</param>
        /// <returns>返回 true：成功 false：失败</returns>
        public Boolean UpdateStaffName(String _StaffNo, String _NewName)
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库
                OracleCommand ChangeStaffName = new OracleCommand(@"update MarketTerminal_Staff
                                                                    set STAFFNAME = '" + _NewName + @"'
                                                                    where STAFFNO = '" + _StaffNo + @"'");//姓名更新语句
                ChangeStaffName.Connection = Connect;//指定连接
                ChangeStaffName.ExecuteNonQuery();//执行更新
                return true;//更新成功
            }
            catch (Exception)
            {
                return false;//更新失败
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }

        }
        /// <summary> 更新员工密码
        /// </summary>
        /// <param name="_StaffNo">员工工号</param>
        /// <param name="_NewName">员工新密码</param>
        /// <returns>返回 true：成功 false：失败</returns>
        public Boolean UpdateStaffPwd(String _StaffNo, String _NewPassword)
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库
                OracleCommand ChangeStaffPwd = new OracleCommand(@"update MarketTerminal_Staff
                                                                    set STAFFNAME = '" + _NewPassword + @"'
                                                                    where STAFFNO = '" + _StaffNo + @"'");//密码更新语句
                ChangeStaffPwd.Connection = Connect;//指定连接
                ChangeStaffPwd.ExecuteNonQuery();//执行更新
                return true;//更新成功
            }
            catch (Exception)
            {
                return false;//更新失败
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 更新员工超级管理员状态
        /// </summary>
        /// <param name="_GoodsNo">员工工号</param>
        /// <param name="_SUstatus">员工SU状态</param>
        /// <returns>返回 true：成功 false：失败</returns>
        public Boolean UpdateStaffPower(String _StaffNo,Boolean _SUstatus)
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库
                OracleCommand ChangeStaffSU;//用于存放更新SU状态语句
                if (_SUstatus == true)//赋予超级管理员
                {
                    ChangeStaffSU = new OracleCommand(@"update MarketTerminal_Staff
                                                         set SUPERUSER = '1'
                                                         where STAFFNO = '" + _StaffNo + @"'");//SU更新语句
                }
                else//撤销超级管理员
                {
                    ChangeStaffSU = new OracleCommand(@"update MarketTerminal_Staff
                                                         set SUPERUSER = '0'
                                                         where STAFFNO = '" + _StaffNo + @"'");//SU更新语句
                }
                ChangeStaffSU.Connection = Connect;//指定连接
                ChangeStaffSU.ExecuteNonQuery();//执行更新
                return true;//更新成功
            }
            catch (Exception)
            {
                return false;//更新失败
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 删除员工
        /// </summary>
        /// <param name="_StaffNo">员工工号</param>
        /// <returns>返回 true：成功 false：失败</returns>
        public Boolean DeleteStaff(String _StaffNo)
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库
                OracleCommand DeleteStaff = new OracleCommand(@"delete 
                                                                    from MARKETTERMINAL_STAFF
                                                                    where STAFFNO='" + _StaffNo + @"'");//员工行删除语句
                DeleteStaff.Connection = Connect;//指定连接
                DeleteStaff.ExecuteNonQuery();//执行删除
                return true;//删除成功
            }
            catch (Exception)
            {
                return false;//删除失败
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 获取单个员工信息
        /// </summary>
        /// <param name="_StaffNo">员工工号</param>
        /// <returns>返回 String[]：员工信息 null：失败</returns>
        public String[] GetStaff(String _StaffNo)
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库
                OracleCommand GetUser = new OracleCommand(@"select * from MarketTerminal_Staff
                                                              where StaffNo='" + _StaffNo + @"'");//查询语句
                GetUser.Connection = Connect;//指定连接
                OracleDataReader Reader = GetUser.ExecuteReader();//执行查询
                if (Reader.Read())
                    return new String[] { Reader[0].ToString(), Reader[1].ToString(), Reader[2].ToString(), Reader[3].ToString() };//返回信息集
                else
                    return null;//无此员工返回null
            }
            catch (Exception)
            {
                return null;//失败则返回null
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }

        }
        /// <summary> 新增员工
        /// </summary>
        /// <param name="_StaffInfo">员工信息集</param>
        /// <returns>返回 true：成功 false：失败</returns>
        public Boolean AddStaff(String[] _StaffInfo)
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            int SU_flag;//SU标志
            try
            {
                if (_StaffInfo[2].Equals("是"))//有超级管理员
                    SU_flag = 1;
                else//无超级管理员
                    SU_flag = 0;
                Connect.Open();//尝试连接数据库
                OracleCommand AddStaff = new OracleCommand(@"insert into MarketTerminal_Staff
                                                             values ('" + _StaffInfo[0] + "','" + _StaffInfo[1] + "','" + SU_flag + "','" +
                                                            _StaffInfo[3] + @"')");//用于存放新增员工语句
                AddStaff.Connection = Connect;//指定连接
                AddStaff.ExecuteNonQuery();//执行添加
                return true;//添加成功
            }
            catch (Exception)
            {
                return false;//添加失败
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 删除商品
        /// </summary>
        /// <param name="_GoodsNo">商品编号</param>
        /// <returns>返回 true：成功 false：失败</returns>
        public Boolean DeleteGoods(String _GoodsNo)
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库
                OracleCommand DeleteGoods = new OracleCommand(@"delete 
                                                                    from MARKETTERMINAL_Goods
                                                                    where GOODSNO='" + _GoodsNo + @"'");//商品行删除语句
                DeleteGoods.Connection = Connect;//指定连接
                DeleteGoods.ExecuteNonQuery();//执行删除
                return true;//删除成功
            }
            catch (Exception)
            {
                return false;//删除失败
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 检查商品是否存在
        /// </summary>
        /// <param name="_GoodsNo">商品编号</param>
        /// <returns>返回 true：是 false：否</returns>
        public Boolean IsGoodsExists(String _GoodsNo)
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库
                OracleCommand CheckGoods = new OracleCommand(@"select * from MarketTerminal_Goods
                                                              where GoodsNo='" + _GoodsNo + @"'");//查询语句
                CheckGoods.Connection = Connect;//指定连接
                OracleDataReader Reader = CheckGoods.ExecuteReader();//执行查询
                if (Reader.Read())//查询到结果
                    return true;//存在
                else
                    return false;//不存在
            }
            catch (Exception)
            {
                return false;//不存在
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 更新商品数量
        /// </summary>
        /// <param name="_GoodsNo">商品编号</param>
        /// <param name="Num">新增数</param>
        /// <returns>返回 true：成功 false：失败</returns>
        public Boolean UpdateGoodsNum(String _GoodsNo, int Num)
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库
                OracleCommand AddNum = new OracleCommand(@"update MarketTerminal_Goods
                                                         set TrunkLeft = TrunkLeft + '" + Num + @"'
                                                         where GoodsNo = '" + _GoodsNo + @"'");//用于存放增加数量语句
                AddNum.Connection = Connect;//指定连接
                AddNum.ExecuteNonQuery();//执行更新
                return true;//更新成功
            }
            catch (Exception)
            {
                return false;//更新失败
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 更新商品数量_货架存量
        /// </summary>
        /// <param name="_GoodsNo">商品编号</param>
        /// <param name="Num">新增数</param>
        /// <returns>返回 true：成功 false：失败</returns>
        public Boolean UpdateGoodsNum_MarketLeft(String _GoodsNo, int Num)
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库
                OracleCommand AddNum = new OracleCommand(@"update MarketTerminal_Goods
                                                         set MARKETLEFT = MARKETLEFT + '" + Num + @"'
                                                         where GoodsNo = '" + _GoodsNo + @"'");//用于存放增加数量语句
                AddNum.Connection = Connect;//指定连接
                AddNum.ExecuteNonQuery();//执行更新
                return true;//更新成功
            }
            catch (Exception)
            {
                return false;//更新失败
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 新增商品
        /// </summary>
        /// <param name="_StaffInfo">商品信息集</param>
        /// <returns>返回 true：成功 false：失败</returns>
        public Boolean AddGoods(String[] _GoodsInfo)
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库
                OracleCommand AddStaff = new OracleCommand(@"insert into MarketTerminal_Goods
                                                             values ('" + _GoodsInfo[0] + "','" + _GoodsInfo[1] + "','" + _GoodsInfo[2] + "','" +
                                                            _GoodsInfo[3] + "','" + _GoodsInfo[4] + "','" + _GoodsInfo[5] + "','" +
                                                            _GoodsInfo[6] + "','" + _GoodsInfo[7] + @"')");//用于存放新增商品语句
                AddStaff.Connection = Connect;//指定连接
                AddStaff.ExecuteNonQuery();//执行添加
                return true;//添加成功
            }
            catch (Exception)
            {
                return false;//添加失败
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 获取单个商品信息
        /// </summary>
        /// <param name="_GoodsNo">商品编号</param>
        /// <returns>返回 String[]：商品信息 null：失败</returns>
        public String[] GetGoodsInfo(String _GoodsNo)
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库
                OracleCommand GetGoods = new OracleCommand(@"select * from MarketTerminal_Goods
                                                              where GoodsNo='" + _GoodsNo + @"'");//查询语句
                GetGoods.Connection = Connect;//指定连接
                OracleDataReader Reader = GetGoods.ExecuteReader();//执行查询
                if (Reader.Read())
                    return new String[] { Reader[0].ToString(), Reader[1].ToString(), Reader[2].ToString(), Reader[3].ToString(), Reader[4].ToString(),
                                            Reader[5].ToString(), Reader[6].ToString(), Reader[7].ToString()};//返回信息集
                else
                    return null;//无此商品返回null
            }
            catch (Exception)
            {
                return null;//失败则返回null
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 更新商品信息
        /// </summary>
        /// <param name="_GoodsInfo">商品信息集</param>
        /// <returns>返回 true：成功 false：失败</returns>
        public Boolean UpdateGoodsInfo(String[] _GoodsInfo)
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库
                OracleCommand UpGoods = new OracleCommand(@"update MarketTerminal_Goods
                                                             set GOODSNAME='" + _GoodsInfo[1] + "'," +
                                                                 "IN_PRICE='" + _GoodsInfo[2] + "'," +
                                                                 "OUT_PRICE='" + _GoodsInfo[3] + "'," +
                                                                 "MARKETLEFT='" + _GoodsInfo[4] + "'," +
                                                                 "TRUNKLEFT='" + _GoodsInfo[5] + "'," +
                                                                 "BRAND='" + _GoodsInfo[6] + "'," +
                                                                 "UNIT='" + _GoodsInfo[7] + "' " +
                                                                 "where GOODSNO='" + _GoodsInfo[0] + "'");//用于存放更新商品语句
                UpGoods.Connection = Connect;//指定连接
                UpGoods.ExecuteNonQuery();//执行修改
                return true;//修改成功
            }
            catch (Exception)
            {
                return false;//修改失败
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 获得账目列表
        /// </summary>
        /// <returns>返回 账目列表</returns>
        public List<String[]> GetAccountList()
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            List<String[]> AccountList = new List<string[]>();//初始化账目列表
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库
                OracleCommand QueryAccountList = new OracleCommand(@"select * from MarketTerminal_Account");//查询语句
                QueryAccountList.Connection = Connect;//指定连接
                OracleDataReader AccountListReader = QueryAccountList.ExecuteReader();//执行查询
                while (AccountListReader.Read())//按行读取，直到结尾
                {
                    AccountList.Add(new String[] {AccountListReader[0].ToString(),//账目流水号
                                                    AccountListReader[1].ToString(),//商品编号
                                                    AccountListReader[2].ToString(),//商品名
                                                    AccountListReader[3].ToString(),//进价
                                                    AccountListReader[4].ToString(),//售价
                                                    AccountListReader[5].ToString(),//出售量
                                                    AccountListReader[6].ToString(),//单位
                                                    AccountListReader[7].ToString()});//利润
                }
                return AccountList;//查询成功
            }
            catch (Exception)
            {
                return null;//查询账目表失败则返回null
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 更新账目信息
        /// </summary>
        /// <param name="_GoodsInfo">账目信息集</param>
        /// <returns>返回 true：成功 false：失败</returns>
        public Boolean UpdateAccountInfo(String[] _AccountInfo)
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库
                OracleCommand UpAccount = new OracleCommand(@"update MarketTerminal_Account
                                                             set GOODSNO='" + _AccountInfo[1] + "'," +
                                                                 "GOODSNAME='" + _AccountInfo[2] + "'," +
                                                                 "IN_PRICE='" + _AccountInfo[3] + "'," +
                                                                 "OUT_PRICE='" + _AccountInfo[4] + "'," +
                                                                 "OUTNUM='" + _AccountInfo[5] + "'," +
                                                                 "UNIT='" + _AccountInfo[6] + "'," +
                                                                 "PRICE='" + _AccountInfo[7] + "' " +
                                                                 "where ACCOUNTNO='" + _AccountInfo[0] + "'");//用于存放更新商品语句
                UpAccount.Connection = Connect;//指定连接
                UpAccount.ExecuteNonQuery();//执行修改
                return true;//修改成功
            }
            catch (Exception)
            {
                return false;//修改失败
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 删除账目
        /// </summary>
        /// <param name="_GoodsNo">账目编号</param>
        /// <returns>返回 true：成功 false：失败</returns>
        public Boolean DeleteAccount(String _AccountNo)
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库
                OracleCommand DeleteAcount = new OracleCommand(@"delete 
                                                                    from MARKETTERMINAL_Account
                                                                    where ACCOUNTNO='" + _AccountNo + @"'");//账目行删除语句
                DeleteAcount.Connection = Connect;//指定连接
                DeleteAcount.ExecuteNonQuery();//执行删除
                return true;//删除成功
            }
            catch (Exception)
            {
                return false;//删除失败
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 模糊查找商品编号
        /// </summary>
        /// <param name="_GoodsNo">商品编号(part)</param>
        /// <returns>返回 String[]：商品信息 null：失败</returns>
        public List<String[]> GetGoodsInfo_Part(String _GoodsNo)
        {
            List<String[]> InfoList = new List<string[]>();//找到的商品列表
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库
                OracleCommand GetGoods = new OracleCommand(@"select * from MarketTerminal_Goods
                                                              where GoodsNo like '%" + _GoodsNo + @"%'");//查询语句
                GetGoods.Connection = Connect;//指定连接
                OracleDataReader Reader = GetGoods.ExecuteReader();//执行查询
                while (Reader.Read())
                {
                    InfoList.Add(new String[] { Reader[0].ToString(), Reader[1].ToString(), Reader[2].ToString(), Reader[3].ToString(), Reader[4].ToString(),
                                            Reader[5].ToString(), Reader[6].ToString(), Reader[7].ToString()});
                }
                return InfoList;//查询成功
            }
            catch (Exception)
            {
                return null;//失败则返回null
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 查找货架存量不足的商品
        /// </summary>
        /// <param name="Threshold">阈值</param>
        /// <returns>返回 String[]：商品信息 null：失败</returns>
        public List<String[]> GetGoodsMarketleftNotOk(int Threshold = 10)
        {
            List<String[]> InfoList = new List<string[]>();//找到的商品列表
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库
                OracleCommand GetGoods = new OracleCommand(@"select * from MarketTerminal_Goods
                                                              where Marketleft < '" + Threshold + @"'");//查询语句
                GetGoods.Connection = Connect;//指定连接
                OracleDataReader Reader = GetGoods.ExecuteReader();//执行查询
                while (Reader.Read())
                {
                    InfoList.Add(new String[] { Reader[0].ToString(), Reader[1].ToString(), Reader[2].ToString(), Reader[3].ToString(), Reader[4].ToString(),
                                            Reader[5].ToString(), Reader[6].ToString(), Reader[7].ToString()});
                }
                return InfoList;//查询成功
            }
            catch (Exception)
            {
                return null;//失败则返回null
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 查找库存量不足的商品
        /// </summary>
        /// <param name="Threshold">阈值</param>
        /// <returns>返回 String[]：商品信息 null：失败</returns>
        public List<String[]> GetGoodsTrunkleftNotOk(int Threshold = 50)
        {
            List<String[]> InfoList = new List<string[]>();//找到的商品列表
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库
                OracleCommand GetGoods = new OracleCommand(@"select * from MarketTerminal_Goods
                                                              where TrunkLeft < '" + Threshold + @"'");//查询语句
                GetGoods.Connection = Connect;//指定连接
                OracleDataReader Reader = GetGoods.ExecuteReader();//执行查询
                while (Reader.Read())
                {
                    InfoList.Add(new String[] { Reader[0].ToString(), Reader[1].ToString(), Reader[2].ToString(), Reader[3].ToString(), Reader[4].ToString(),
                                            Reader[5].ToString(), Reader[6].ToString(), Reader[7].ToString()});
                }
                return InfoList;//查询成功
            }
            catch (Exception)
            {
                return null;//失败则返回null
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
    }
}
